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
using Microsoft.AspNetCore.Http;
using System.Security.Principal;
using System.Security.Claims;

namespace UnitTests.Service.RestaurantService
{
    public class RestaurantServiceTests
    {
        private Mock<IDaoRestaurant> daoRestaurantMock;
        private Mock<IDaoTypeOfRestaurant> daoTypeOfRestaurantMock;
        private Mock<IDaoRestaurantType> daoRestaurantTypeMock;
        private Mock<IDaoUser> daoUserMock;
        private HomeController homeController;
        private RestaurantController restaurantController;




        [SetUp]
        public void Setup()
        {
            daoRestaurantMock = new Mock<IDaoRestaurant>();

            daoTypeOfRestaurantMock = new Mock<IDaoTypeOfRestaurant>();

            daoTypeOfRestaurantMock
                .Setup(r => r.GetAllAsync().Result)
                .Returns(new List<TypeOfRestaurant> {
                            new TypeOfRestaurant(){Name="Type"},
                            new TypeOfRestaurant(){Name="Type2"}
                });



            daoRestaurantTypeMock = new Mock<IDaoRestaurantType>();

            daoUserMock = new Mock<IDaoUser>();

            ILogger<HomeController>? logger = null;

            homeController = new HomeController(logger,
                                                daoRestaurantMock.Object,
                                                daoTypeOfRestaurantMock.Object,
                                                daoRestaurantTypeMock.Object,
                                                daoUserMock.Object);

            restaurantController = new RestaurantController(daoRestaurantMock.Object,
                                    daoTypeOfRestaurantMock.Object,
                                    daoRestaurantTypeMock.Object,
                                    daoUserMock.Object);
        }

        [Test]
        public void GetAllAsync()
        {
            daoRestaurantMock
                .Setup(r => r.GetAllAsync().Result)
                .Returns(new List<Restaurant> {
                    new Restaurant(){Name="Restaurant"},
                    new Restaurant(){Name="Restaurant2"}
                });




            HomeViewModel viewModel = new HomeViewModel();

            var actionResult = homeController.Index(viewModel) as ViewResult;
            viewModel = actionResult.Model as HomeViewModel;
            List<Restaurant> restaurants = viewModel.Restaurants;

            daoRestaurantMock.Verify(r => r.GetAllAsync());
            Assert.That(restaurants[0].Name, Is.EqualTo("Restaurant"));
            Assert.That(restaurants[1].Name, Is.EqualTo("Restaurant2"));


        }

        [Test]
        public void AddAsync()
        {
            AddRestaurantVM viewModel = new AddRestaurantVM();
            Restaurant restaurant = new Restaurant()
            {
                Name = "Restaurant"
            };
            viewModel.Restaurant = restaurant;

            //Setup mock file using a memory stream
            var content = "Hello World from a Fake File";
            var fileName = "test.png";
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(content);
            writer.Flush();
            stream.Position = 0;

            //create FormFile with desired data
            IFormFile file = new FormFile(stream, 0, stream.Length, "id_from_form", fileName);

            homeController.AddRestaurant(viewModel, file);

            daoRestaurantMock.Verify(r => r.AddAsync(It.IsAny<Restaurant>()));

        }


        [Test]
        public void Update()
        {
            Restaurant restaurant = new Restaurant() {Id=1,Name="Restaurant"};

            daoRestaurantMock
            .Setup(r => r.GetAsync(1).Result)
            .Returns(restaurant);

            AddRestaurantVM viewModel = new AddRestaurantVM();
            Restaurant changed = new Restaurant() {Id=1,Name="Restaurant2" };
            viewModel.Restaurant = changed;

            // moq user is admin

            //var userMock = new Mock<ClaimsPrincipal>();
            //userMock.Setup(p => p.IsInRole("Administrator")).Returns(true);

            //var contextMock = new Mock<HttpContext>();
            //contextMock.SetupGet(ctx => ctx.User)
            //           .Returns(userMock.Object);

            //var controllerContextMock = new Mock<ControllerContext>();
            //controllerContextMock.SetupGet(con => con.HttpContext)
            //                     .Returns(contextMock.Object);

            //restaurantController.ControllerContext = controllerContextMock.Object;




            restaurantController.Edit(viewModel);






            daoRestaurantMock.Verify(r=>r.GetAsync(1).Result);
            Assert.That(restaurant.Name, Is.EqualTo("Restaurant2"));
        }
    }

}
