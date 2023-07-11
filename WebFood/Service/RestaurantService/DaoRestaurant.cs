﻿using Microsoft.EntityFrameworkCore;
using WebFood.Models.Entities;
using WebPlanner.Models;

namespace WebFood.Service.RestaurantService
{
    public class DaoRestaurant : IDaoRestaurant
    {
        private readonly AppDbContext _db;
        public DaoRestaurant(AppDbContext db)
        {
            _db= db;
        }
        public async void AddAsync(Restaurant restaurant)
        {
            await _db.Restaurants.AddAsync(restaurant);
            _db.SaveChanges();
        }

        public void DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Restaurant>> GetAllAsync()
        {
            return await _db.Restaurants.ToListAsync();
        }

        public async Task<Restaurant> GetAsync(int id)
        {
           return await _db.Restaurants.FirstAsync(r => r.Id == id);
        }

        public void Update(Restaurant restaurant)
        {
            _db.Restaurants.Update(restaurant);
            _db.SaveChanges();
        }
    }
}
