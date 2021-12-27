using RestaurantAPI2.Models;
using System.Collections.Generic;

namespace RestaurantAPI2.Services
{
    public interface IRestaurantService
    {
        int Create(CreateRestaurantDto dto);
        IEnumerable<RestaurantDto> GetAll();
        RestaurantDto GetById(int id);
    }
}