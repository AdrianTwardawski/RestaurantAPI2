using Microsoft.EntityFrameworkCore;
using RestaurantAPI2.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantAPI2
{
    public class RestaurantSeeder
    {
        private readonly RestaurantDbContext _dbContext;

        public RestaurantSeeder(RestaurantDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void Seed() //wywołanie metody seed następuje w Startup.cs w metodzie Configure()
        {
            if(_dbContext.Database.CanConnect()) //sprawdzenie połączenia z Db
            {
                var pendingMigrations = _dbContext.Database.GetPendingMigrations();
                if(pendingMigrations != null && pendingMigrations.Any())
                {
                    _dbContext.Database.Migrate();
                }
                   

                if(!_dbContext.Restaurants.Any()) //sprawdzenie czy Db jest pusta
                {
                    var restaurants = GetRestaurants();
                    _dbContext.Restaurants.AddRange(restaurants); //przekazanie listy typu restaurant do Db
                    _dbContext.SaveChanges(); //zapisanie zmian na kontekście Db
                }

                if(!_dbContext.Roles.Any())
                {
                    var roles = GetRoles();
                    _dbContext.Roles.AddRange(roles);
                    _dbContext.SaveChanges();
                }
            }
        }

        private IEnumerable<Restaurant> GetRestaurants()
        {
            var restaurants = new List<Restaurant>()
            {
                new Restaurant()
                {
                    Name = "KFC",
                    Category = "Fast Food",
                    Description = "KFC (short for Kentucky Fried Chicken) is an American fast food restaurant chain headquarted in Kentucky",
                    ContactEmail = "contact@kfc.com",
                    HasDelivery = true,
                    Dishes = new List<Dish>()
                    {
                        new Dish()
                        {
                            Name = "Nashville Hot Chicken",
                            Price = 10.30M,
                        },

                        new Dish()
                        {
                            Name = "Chicken Nuggets",
                            Price = 5.30M,
                        },
                    },
                    Address = new Address()
                    {
                        City = "Kraków",
                        Street = "Długa 5",
                        PostalCode = "30-001"
                    }
                },
                new Restaurant()
                {
                    Name = "MC Donalds",
                    Category = "Fast Food",
                    Description = "Chuj dupa i kamieni kupa",
                    ContactEmail = "contact@mc.com",
                    HasDelivery = true,
                    Dishes = new List<Dish>()
                    {
                        new Dish()
                        {
                            Name = "BigMac",
                            Price = 16.20M,
                        },

                        new Dish()
                        {
                            Name = "Hamburger",
                            Price = 2.30M,
                        },
                    },
                    Address = new Address()
                    {
                        City = "Opole",
                        Street = "Bonczyka 5",
                        PostalCode = "38-081"
                    }
                }
            };
            return restaurants;
        }

        public IEnumerable<Role> GetRoles()
        {
            var roles = new List<Role>()
            {
                new Role()
                {
                    Name = "User"
                },

                new Role()
                {
                    Name = "Manager"
                },

                new Role()
                {
                    Name = "Admin"
                }
            };
            return roles;
        }
    }
}

