using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI2.Entities;
using RestaurantAPI2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantAPI2.Services
{
    //RestaurantService został utworzony, ponieważ jedynym zadaniem w akcjach w kontrolerze powinno być odebranie zapytania, ewentualnie jego walidacja 
    //oraz przesłanie takiego zapytania do jakiegoś serwisu i to własnie ten serwis powinien go obsłużyć
    public class RestaurantService : IRestaurantService
    {
        private readonly RestaurantDbContext _dbContext;
        private readonly IMapper _mapper;

        public RestaurantService(RestaurantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public RestaurantDto GetById(int id)
        {
            var restaurant = _dbContext
              .Restaurants
              .Include(r => r.Address)
              .Include(r => r.Dishes)
              .FirstOrDefault(r => r.Id == id);

            if (restaurant is null) return null;

            var result = _mapper.Map<RestaurantDto>(restaurant);
            return result;
        }

        public IEnumerable<RestaurantDto> GetAll()
        {
            var restuarants = _dbContext
                 .Restaurants
                 .Include(r => r.Address)
                 .Include(r => r.Dishes)
                 .ToList();

            var restaurantsDtos = _mapper.Map<List<RestaurantDto>>(restuarants);

            return restaurantsDtos;
        }

        public int Create(CreateRestaurantDto dto)
        {
            var restaurant = _mapper.Map<Restaurant>(dto);
            _dbContext.Restaurants.Add(restaurant);
            _dbContext.SaveChanges();

            return restaurant.Id;
        }
    }
}
