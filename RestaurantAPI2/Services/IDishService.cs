using RestaurantAPI2.Models;

namespace RestaurantAPI2.Services
{
    public interface IDishService
    {
        int Create(int id, CreateDishDto dto);
    }
}