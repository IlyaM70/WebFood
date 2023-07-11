﻿using WebFood.Models.Entities;

namespace WebFood.Service.RestaurantService
{
    public interface IDaoRestaurant
    {
        public Task<List<Restaurant>> GetAllAsync();
        public void AddAsync(Restaurant restaurant);
        public void Update(Restaurant restaurant);
        public void DeleteAsync(int id);
        public Task<Restaurant> GetAsync(int id);
    }
}
