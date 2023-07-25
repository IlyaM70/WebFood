using WebFood.Models.Entities;
using WebFood.Service.RestaurantService;
using WebFood.Models;
using Moq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
//using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFood.Service.RestaurantService;
using WebPlanner.Models;

namespace UnitTests.Service.RestaurantService
{
    internal class IDaoRetaurantTest
    {
        private Mock<DbSet<Restaurant>> mockSet;
        private Mock<AppDbContext> mockContext;
        private DaoRestaurant service;
        private CancellationToken cancellationToken = CancellationToken.None;
        private IQueryable<Restaurant> mockRestaurants;



        [SetUp]
        public void Setup()
        {
            mockRestaurants = new List<Restaurant>() {
                new Restaurant{Id=1,Name="KFS"},
                new Restaurant{Id=2,Name="Burger Queen"},
            }.AsQueryable();


            mockSet = new Mock<DbSet<Restaurant>>();

            mockSet.As<IDbAsyncEnumerable<Restaurant>>()
                .Setup(m => m.GetAsyncEnumerator())
                .Returns(new TestDbAsyncEnumerator<Restaurant>(mockRestaurants.GetEnumerator()));

            mockSet.As<IQueryable<Restaurant>>()
                .Setup(m => m.Provider)
                .Returns(new TestDbAsyncQueryProvider<Restaurant>(mockRestaurants.Provider));



            mockSet.As<IQueryable<Restaurant>>()
                .Setup(m => m.Provider)
                .Returns(mockRestaurants.Provider);

            mockSet.As<IQueryable<Restaurant>>()
                .Setup(m => m.Expression)
                .Returns(mockRestaurants.Expression);

            mockSet.As<IQueryable<Restaurant>>()
                .Setup(m => m.ElementType)
                .Returns(mockRestaurants.ElementType);

            mockSet.As<IQueryable<Restaurant>>()
                .Setup(m => m.GetEnumerator())
                .Returns(() => mockRestaurants.GetEnumerator());


            mockContext = new Mock<AppDbContext>();
            mockContext.SetupGet(x => x.Restaurants).Returns(mockSet.Object);

            service = new DaoRestaurant(mockContext.Object);

        }

        [Test]
        public async Task AddAsync()
        {
            service.AddAsync(new Restaurant() { Id = 3, Name = "Added" });
            mockSet.Verify(m => m.AddAsync(It.IsAny<Restaurant>(), cancellationToken), Times.Once());
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [Test]
        public void Update()
        {
            service.Update(new Restaurant() { Id = 1, Name = "Updated" });
            mockSet.Verify(m => m.Update(It.IsAny<Restaurant>()), Times.Once());
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

            
        [Test]
        public async Task GetAllAsync()
        {
            List<Restaurant> restaurants = await service.GetAllAsync();
            Assert.That(restaurants.Count, Is.EqualTo(2));
            Assert.That(restaurants[0].Name, Is.EqualTo("KFS"));
            Assert.That(restaurants[1].Name, Is.EqualTo("Burger Queen"));
        }

        // https://learn.microsoft.com/en-us/ef/core/testing/testing-without-the-database#inmemory-provider
        // https://github.com/romantitov/MockQueryable



    }
}
