using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using WebFood.Service.RestaurantService;
using WebFood.Models.Entities;
using WebFood.Models.ViewModels;
using WebFood.Controllers;
using WebFood.Service.TypeOfRestaurantService;
using WebFood.Service.RestaurantTypeService;
using WebFood.Service.UserService;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using WebFood.Service.CategoryService;

namespace UnitTests.Service.RestaurantService
{
    public  class RestaurantServiceTests
    {
        private Mock<IDaoRestaurant> daoRestaurantMock;
        private Mock<IDaoTypeOfRestaurant> daoTypeOfRestaurantMock;
        private Mock<IDaoRestaurantType> daoRestaurantTypeMock;
        private Mock<IDaoUser> daoUserMock;
        private HomeController homeController;




        [SetUp]
        public void Setup()
        {
            daoRestaurantMock = new Mock<IDaoRestaurant>();

            //daoRestaurantMock
            //    .Setup(r => r.GetAsync(1).Result)
            //    .Returns(new Restaurant() {Id=1,Name="Restaurant"});



            daoTypeOfRestaurantMock = new Mock<IDaoTypeOfRestaurant>();

            daoRestaurantTypeMock = new Mock<IDaoRestaurantType>();
            
            daoUserMock = new Mock<IDaoUser>();

            ILogger<HomeController>? logger = null;


            homeController = new HomeController(logger,
                                                daoRestaurantMock.Object,
                                                daoTypeOfRestaurantMock.Object,
                                                daoRestaurantTypeMock.Object,
                                                daoUserMock.Object);
        }

        [Test]
        public void GetAllAsync()
        {
            daoRestaurantMock
                .Setup(r => r.GetAllAsync().Result)
                .Returns( new List<Restaurant> {
                    new Restaurant(){Name="Restaurant"},
                    new Restaurant(){Name="Restaurant2"}
                });

            daoTypeOfRestaurantMock
                    .Setup(r => r.GetAllAsync().Result)
                    .Returns(new List<TypeOfRestaurant> {
                        new TypeOfRestaurant(){Name="Type"},
                        new TypeOfRestaurant(){Name="Type2"}
                    });


            HomeViewModel viewModel = new HomeViewModel();

            var actionResult = homeController.Index(viewModel) as ViewResult;
            viewModel = actionResult.Model as HomeViewModel;
            List<Restaurant> restaurants = viewModel.Restaurants;

            daoRestaurantMock.Verify(r => r.GetAllAsync());
            Assert.That(restaurants[0].Name, Is.EqualTo("Restaurant"));
            Assert.That(restaurants[1].Name, Is.EqualTo("Restaurant2"));


        }
    }

}
