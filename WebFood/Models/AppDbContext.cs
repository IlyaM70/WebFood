using Microsoft.EntityFrameworkCore;
using WebFood.Models.Entities;
using WebFood.Utility;

namespace WebFood.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext()
        {
                
        }
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }


        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<Profile> Profiles { get; set; }
        public virtual DbSet<Restaurant> Restaurants { get; set; }
        public virtual DbSet<TypeOfRestaurant> TypesOfRestaurants { get; set; }
        public virtual DbSet<RestaurantType> RestaurantType { get; set; }
        public virtual DbSet<CategoryOfMeal> CategoriesOfMeals { get; set; }
        public virtual DbSet<Meal> Meals { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderMeal> OrderMeal { get; set; }
        
    }
}
