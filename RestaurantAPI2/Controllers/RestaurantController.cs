using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI2.Entities;
using RestaurantAPI2.Models;
using RestaurantAPI2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantAPI2.Controllers
{
    [Route("api/restaurant")]
    [ApiController] /*używając ApiController pozbywamy się kodu, który był odpowiedzialny za 
    walidacje modelu i zwracanie odpowiedniego kodu statusu wraz z błędami walidacji do klienta*/
    [Authorize] //każda z akcji w kontrolerze będzie wymagała Autoryzacji
    public class RestaurantController : ControllerBase
    {
        private readonly IRestaurantService _restaurantService;

        public RestaurantController(IRestaurantService restaurantService)
        {
            _restaurantService = restaurantService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<RestaurantDto>> GetAll()
        {
            var restaurantsDtos = _restaurantService.GetAll();
            return Ok(restaurantsDtos);
        }

        [HttpGet("{id}")]
        public ActionResult<RestaurantDto> Get([FromRoute] int id)
        {
            var restaurant = _restaurantService.GetById(id);                   
            return Ok(restaurant);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Manager")] //atrybut Authorize nałożony na akcje ma większy priorytet niż Authorize na poziomie kontrolera
        public ActionResult CreateRestaurant([FromBody] CreateRestaurantDto dto)
        {
            /*POZBYTO SIĘ ZA POMOCĄ [ApiController]*/
            //if(!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}
            
            var id = _restaurantService.Create(dto);
            return Created($"/api/restaurant/{id}", null);
        }

        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute] int id)
        {
            _restaurantService.Delete(id);          
            return NoContent();
        }

        [HttpPut("{id}")]
        [AllowAnonymous] //sprawi, że automatycznie ta akcja będzie zezwalać na zapytania bez nagłówka autoryzacji
        public ActionResult Update([FromBody] UpdateRestaurantDto dto, [FromRoute] int id)
        {         
            _restaurantService.Update(id, dto);          
            return Ok();
        }
    }
}
