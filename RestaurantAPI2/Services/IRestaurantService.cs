using RestaurantAPI2.Models;
using System.Collections.Generic;

namespace RestaurantAPI2.Services
{
    public interface IRestaurantService
    {
        int Create(CreateRestaurantDto dto);
        bool Delete(int id);
        IEnumerable<RestaurantDto> GetAll();
        RestaurantDto GetById(int id);
        bool Update(int id, UpdateRestaurantDto dto);
    }
}