using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI2.Entities;
using RestaurantAPI2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantAPI2.Controllers
{
    [Route("api/restaurant")]
    public class RestaurantController : ControllerBase
    {
        private readonly RestaurantDbContext _dbContext;
        private readonly IMapper _mapper;

        public RestaurantController(RestaurantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<RestaurantDto>> GetAll()
        {
            var restuarants = _dbContext
                 .Restaurants
                 .Include(r => r.Address)
                 .Include(r => r.Dishes)
                 .ToList();

            /* Z MAPPOWANIEM RĘCZNYM */
            //var restaurantsDtos = restuarants.Select(r => new RestaurantDto()
            //{
            //    Name = r.Name,
            //    Description = r.Description,
            //    Category = r.Category,
            //    HasDelivery = r.HasDelivery   //itd.          
            //}) ;

            /* Z MAPPOWANIEM PRZY UŻYCIU AUTOMAPPERA*/
            var restaurantsDtos = _mapper.Map<List<RestaurantDto>>(restuarants);
            return Ok(restaurantsDtos);
        }

        [HttpGet("{id}")]
        public ActionResult<RestaurantDto> Get([FromRoute] int id)
        {
            var restaurant = _dbContext
                .Restaurants
                .Include(r => r.Address)
                .Include(r => r.Dishes)
                .FirstOrDefault(r => r.Id == id);

            if (restaurant == null)
            { 
                return NotFound();
            }

            var restaurantDto = _mapper.Map<RestaurantDto>(restaurant);
             
            return Ok(restaurantDto);
        }

        [HttpPost]
        public ActionResult CreateRestaurant([FromBody] CreateRestaurantDto dto)
        {
            var restaurant = _mapper.Map<Restaurant>(dto);
            _dbContext.Restaurants.Add(restaurant);
            _dbContext.SaveChanges();

            return Created($"/api/restaurant/{restaurant.Id}", null);
        }
    }
}
