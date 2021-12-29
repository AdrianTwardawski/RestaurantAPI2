using RestaurantAPI2.Models;
using System.Collections.Generic;

namespace RestaurantAPI2.Services
{
    public interface IDishService
    {
        int Create(int id, CreateDishDto dto);
        List<DishDto> GetAll(int restaurantId);
        DishDto GetById(int restaurantId, int dishId);
        void RemoveAll(int restaurantId);
        void Remove(int restaurantId, int dishId);
    }
}