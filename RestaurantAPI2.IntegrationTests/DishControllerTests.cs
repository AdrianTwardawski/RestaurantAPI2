using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RestaurantAPI2.Entities;
using RestaurantAPI2.IntegrationTests.Helpers;
using RestaurantAPI2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace RestaurantAPI2.IntegrationTests
{
    public class DishControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private HttpClient _client;
        private WebApplicationFactory<Startup> _factory;

        public DishControllerTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        var dbContextOptions = services
                        .SingleOrDefault(service => service.ServiceType == typeof(DbContextOptions<RestaurantDbContext>));

                        services.Remove(dbContextOptions);

                        services.AddDbContext<RestaurantDbContext>(options =>
                        options.UseInMemoryDatabase("RestaurantDb2"));
                    });
                });

                _client = _factory.CreateClient();
        }

        private void SeedRestaurant(Restaurant restaurant)
        {
            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();
            var _dbContext = scope.ServiceProvider.GetService<RestaurantDbContext>();

            _dbContext.Restaurants.Add(restaurant);           
            _dbContext.SaveChanges();
        }


        [Fact]
        public async Task DeleteDish_ForExistingRestaurantAndExistingDish_ReturnsNoContent()
        {
            // arrange
            var restaurant = new Restaurant()
            {
                Name = "test",
                Dishes = new List<Dish>()
                {
                    new Dish()
                    {
                        Name = "pizza"
                    },
                    new Dish()
                    {
                        Name = "spaghetti"
                    },
                }
            };
            SeedRestaurant(restaurant);

            // act
            int dishId = restaurant.Dishes.Find(d => d.Name == "pizza").Id;
            var response = await _client.DeleteAsync("api/restaurant/" + restaurant.Id + "/dish/" + dishId);

            // assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task DeleteDishes_ForExistingRestaurant_ReturnsNoContent()
        {
            var restaurant = new Restaurant()
            {
                Name = "test"
            };
            SeedRestaurant(restaurant);
            // act

            var response = await _client.DeleteAsync("api/restaurant/"+ restaurant.Id + "/dish");

            // assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task DeleteDishes_ForNonExistingRestaurant_ReturnsNotFound()
        {
            // act

            var response = await _client.DeleteAsync("api/restaurant/987/dish");

            // assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetAll_ForValidRestaurantId_ReturnsOk()
        {
            // arrange
            var restaurant = new Restaurant()
            {
                Name = "test"
            };

            var dishes = new List<Dish>()
            {
                new Dish()
                {
                    Name = "testDish",
                    RestaurantId = restaurant.Id
                },
                new Dish()
                {
                    Name = "testDish2",
                    RestaurantId = restaurant.Id
                },
            };

            SeedRestaurant(restaurant);

            // act
            var response = await _client.GetAsync("api/restaurant/" + restaurant.Id + "/dish");

            // assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task Get_ForValidRestaurantIdAndDishId_ReturnOk()
        {
            // arrange
            var restaurant = new Restaurant()
            {
                Name = "test",
                Dishes = new List<Dish>()
                {
                    new Dish()
                    {
                        Name = "pizza"
                    },
                    new Dish()
                    {
                        Name = "spaghetti"
                    },
                }
            };

            SeedRestaurant(restaurant);
            int dishId = restaurant.Dishes.Find(d => d.Name == "pizza").Id;

            // act
            var response = await _client.GetAsync("api/restaurant/" + restaurant.Id + "/dish/" + dishId);

            // assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task Post_ForExistingRestaurantAndWithValidModel_ReturnsCreatedStatus()
        {
            // arrange
            var restaurant = new Restaurant()
            {
                Name = "test"
            };

            SeedRestaurant(restaurant);

            var dishDto = new CreateDishDto()
            {
                Name = "testDish",
                RestaurantId = restaurant.Id
            };


            var httpContent = dishDto.ToJsonHttpContent();

            // act
            var response = await _client.PostAsync("api/restaurant/" + dishDto.RestaurantId + "/dish", httpContent);

            // assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
        }

        [Fact]
        public async Task Post_ForExistingRestaurantAndWithInvalidModel_ReturnsBadRequest()
        {
            // arrange
            var restaurant = new Restaurant()
            {
                Name = "test"
            };

            SeedRestaurant(restaurant);

            var dishDto = new CreateDishDto()
            {
                Description = "test",
                RestaurantId = restaurant.Id
            };

            var httpContent = dishDto.ToJsonHttpContent();

            // act
            var response = await _client.PostAsync("api/restaurant/" + dishDto.RestaurantId + "/dish", httpContent);

            // assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Post_ForNonExistingRestaurantAndWithValidModel_ReturnsBadRequest()
        {
            // arrange

            var dishDto = new CreateDishDto()
            {
                Description = "test",
                RestaurantId = 787
            };

            var httpContent = dishDto.ToJsonHttpContent();

            // act
            var response = await _client.PostAsync("api/restaurant/" + dishDto.RestaurantId + "/dish", httpContent);

            // assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }
    }
}
