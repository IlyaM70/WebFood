using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFood.Service.RestaurantService;
using WebPlanner.Models;

namespace UnitTests.Service.RestaurantService
{
    internal class IDaoRetaurantTest
    {
        private IDaoRestaurant daoRestaurant;
        private AppDbContext db; 


        [SetUp]
        public void Setup()
        {
            //db = new AppDbContext()
            daoRestaurant= new DaoRestaurant(db);
        }

        [Test]
        public void Test() {
            
        }
    }
}
