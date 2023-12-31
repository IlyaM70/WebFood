﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using System.Security.Claims;
using WebFood.Models.Entities;
using WebFood.Models.ViewModels;
using WebFood.Service.CategoryOfMealService;
using WebFood.Service.MealService;
using WebFood.Service.RestaurantService;
using WebFood.Utility;

namespace WebFood.Controllers
{
    public class MealController : Controller
    {
        private readonly IDaoMeal _daoMeal;
        private readonly IDaoRestaurant _daoRestaurant;
        private readonly IDaoCategoryOfMeal _daoCategoryOfMeal;
        private readonly IConfiguration _config;

        public MealController(IDaoMeal daoMeal,
                              IDaoRestaurant daoRestaurant,
                              IDaoCategoryOfMeal daoCategoryOfMeal,
                              IConfiguration configuration)
        {
            _daoMeal = daoMeal;     
            _daoRestaurant = daoRestaurant;
            _daoCategoryOfMeal = daoCategoryOfMeal;
            _config = configuration;
        }

        [HttpGet]
        [Authorize(Roles = $"{Roles.Administator}, {Roles.Manager}")]
        public IActionResult AddMeal(int restaurantId)
        {
            Restaurant restaurant = _daoRestaurant.GetAsync(restaurantId).Result;
            if (AcessChecker.IsAdminOrManager(restaurant, User))
            {
                MealViewModel viewModel = new MealViewModel();
                viewModel.Meal.RestaurantId = restaurantId;
                viewModel.Categories = GetCategoriesOfMeal(restaurantId);

                return View(viewModel);
            }
            
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Authorize(Roles = $"{Roles.Administator}, {Roles.Manager}")]
        public IActionResult AddMeal(MealViewModel viewModel, [FromForm(Name = "Meal.ImageUrl")] IFormFile Imageurl)
        {
            Restaurant restaurant = _daoRestaurant.GetAsync(viewModel.Meal.RestaurantId).Result;
            if (AcessChecker.IsAdminOrManager(restaurant, User))
            {
                if (ModelState.IsValid)
                {
                    AddMealToDb(viewModel.Meal, viewModel.Meal.CategoryId, Imageurl);
                    ViewBag.Message = $"Блюдо {viewModel.Meal.Name} добавлено";
                }

                viewModel.Categories = GetCategoriesOfMeal(viewModel.Meal.RestaurantId);
                return View(viewModel);
            }
            
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize(Roles = $"{Roles.Administator}, {Roles.Manager}")]
        public IActionResult EditMeal(int mealId)
        {
            Meal meal = _daoMeal.GetAsync(mealId).Result;
            Restaurant restaurant = _daoRestaurant.GetAsync(meal.RestaurantId).Result;
            if (AcessChecker.IsAdminOrManager(restaurant, User))
            {
                MealViewModel viewModel = new MealViewModel();
                viewModel.Meal = meal;
                viewModel.Categories = GetCategoriesOfMeal(meal.RestaurantId);

                return View(viewModel);
            }
            
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Authorize(Roles = $"{Roles.Administator}, {Roles.Manager}")]
        public IActionResult EditMeal(MealViewModel viewModel)
        {
            Restaurant restaurant = _daoRestaurant.GetAsync(viewModel.Meal.RestaurantId).Result;
            if (AcessChecker.IsAdminOrManager(restaurant, User))
            {
                if (ModelState.IsValid)
                {
                    _daoMeal.Update(viewModel.Meal);
                    ViewBag.Message = $"Блюдо {viewModel.Meal.Name} изменено";
                }

                viewModel.Categories = GetCategoriesOfMeal(viewModel.Meal.RestaurantId);
                return View(viewModel);
            }
            
            return RedirectToAction("Index", "Home");

        }

        [HttpGet]
        [Authorize(Roles = $"{Roles.Administator}, {Roles.Manager}")]
        public IActionResult ChangeMealImg(int mealId)
        {
            Meal meal = _daoMeal.GetAsync(mealId).Result;
            Restaurant restaurant = _daoRestaurant.GetAsync(meal.RestaurantId).Result;

            if (AcessChecker.IsAdminOrManager(restaurant, User))
            {
                return View(meal);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Authorize(Roles = $"{Roles.Administator}, {Roles.Manager}")]
        public IActionResult ChangeMealImg(Meal meal, [FromForm(Name = "ImageUrl")] IFormFile ImageUrl)
        {
            meal = _daoMeal.GetAsync(meal.Id).Result;
            Restaurant restaurant = _daoRestaurant.GetAsync(meal.RestaurantId).Result;

            if (AcessChecker.IsAdminOrManager(restaurant, User))
            {
                if (ModelState.IsValid)
                {

                    string filePath = _config.GetValue<string>("ImgFilesPath") + meal.ImageUrl;


                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }

                    meal.ImageUrl = FileHelper.GetImageUrl(ImageUrl).Result.ToString();

                    _daoMeal.Update(meal);

                    ViewBag.Message = "Изображение изменено";
                }
                return View(meal);
            }
            
            return RedirectToAction("Index", "Home");
        }


        public IActionResult Delete(int mealId, int restaurantId)
        {
            Meal meal = _daoMeal.GetAsync(mealId).Result;
            _daoMeal.Delete(meal);
            TempData["Message"] = $"Блюдо {meal.Name} удалено";
            return RedirectToAction("Restaurant", "Restaurant", new {restaurantId = restaurantId});
        }

                                            //  HELP METHODS

        private void AddMealToDb(Meal meal, int categoryId, IFormFile Imageurl)
        {
            meal.ImageUrl = FileHelper.GetImageUrl(Imageurl).Result.ToString();
            _daoMeal.AddAsync(meal);
        }

        private SelectList GetCategoriesOfMeal(int restaurantId)
        {
            
            List<CategoryOfMeal> categoriesDb = _daoCategoryOfMeal.GetAllAsync(restaurantId).Result;
            List<CategoryOfMeal> categories = new List<CategoryOfMeal>();
            categories.Add(new CategoryOfMeal() { Id = 0,Name="Нет" });

            if (categoriesDb != null)
            {
                categories.AddRange(categoriesDb);
            }

            return new SelectList(categories, "Id", "Name");
        }
    }
}
