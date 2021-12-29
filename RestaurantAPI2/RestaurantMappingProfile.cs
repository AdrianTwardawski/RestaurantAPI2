using AutoMapper;
using RestaurantAPI2.Entities;
using RestaurantAPI2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantAPI2
{
    public class RestaurantMappingProfile : Profile
    {
        public RestaurantMappingProfile()
        {
            CreateMap<Restaurant, RestaurantDto>()  //mapujemy z typy Restaurant do Typu RestaurantDto
                .ForMember(m => m.City, c => c.MapFrom(s => s.Address.City))  //mapujemy dla właściwości City z klasy RestaurantDto z właściwości Adress.City z klasy Restaurant
                .ForMember(m => m.Street, c => c.MapFrom(s => s.Address.Street))
                .ForMember(m => m.PostalCode, c => c.MapFrom(s => s.Address.PostalCode));
            // jeśli typy i nazwy właściwości pomiędzy klasami Restaurant i RestaurantDto się zgadzają to AutoMapper 
            // automatycznie zmappuje te właściwości i nie musimy ich określać

            CreateMap<Dish, DishDto>();

            CreateMap<CreateRestaurantDto, Restaurant>() 
                .ForMember(r => r.Address,
                    c => c.MapFrom(dto => new Address()
                        { City = dto.City, PostalCode = dto.PostalCode, Street = dto.Street }));

            CreateMap<CreateDishDto, Dish>();
                 
        }
    }
}
