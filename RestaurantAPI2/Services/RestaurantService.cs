using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RestaurantAPI2.Entities;
using RestaurantAPI2.Exceptions;
using RestaurantAPI2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantAPI2.Services
{
    public interface IRestaurantService
    {
        int Create(CreateRestaurantDto dto);
        void Delete(int id);
        IEnumerable<RestaurantDto> GetAll();
        RestaurantDto GetById(int id);
        void Update(int id, UpdateRestaurantDto dto);
    }


    //RestaurantService został utworzony, ponieważ jedynym zadaniem w akcjach w kontrolerze powinno być odebranie zapytania, ewentualnie jego walidacja 
    //oraz przesłanie takiego zapytania do jakiegoś serwisu i to własnie ten serwis powinien go obsłużyć
    public class RestaurantService : IRestaurantService
    {
        private readonly RestaurantDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<RestaurantService> _logger;

        public RestaurantService(RestaurantDbContext dbContext, IMapper mapper, ILogger<RestaurantService> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
        }

        public RestaurantDto GetById(int id)
        {
            var restaurant = _dbContext
              .Restaurants
              .Include(r => r.Address)
              .Include(r => r.Dishes)
              .FirstOrDefault(r => r.Id == id);

            if (restaurant is null)
                throw new NotFoundException("RestaurantNotFound");

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

        public void Delete(int id)
        {
            _logger.LogError($"Restaurant with id: {id} DELETE action invoked");

            var restaurant = _dbContext
              .Restaurants             
              .FirstOrDefault(r => r.Id == id);

            if (restaurant is null)
                throw new NotFoundException("RestaurantNotFound");

            _dbContext.Restaurants.Remove(restaurant);
            _dbContext.SaveChanges();                      
        }

        public void Update(int id, UpdateRestaurantDto dto)
        {
            var restaurant = _dbContext
              .Restaurants          
              .FirstOrDefault(r => r.Id == id);

            if (restaurant is null)
                throw new NotFoundException("RestaurantNotFound");

            restaurant.Name = dto.Name;
            restaurant.Description = dto.Description;
            restaurant.HasDelivery = dto.HasDelivery;
            
            _dbContext.SaveChanges();
        }
    }
}
