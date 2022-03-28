using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using FluentAssertions;
using System.Net.Http;
using RestaurantAPI2.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RestaurantAPI2.Models;
using Newtonsoft.Json;
using System.Text;
using Microsoft.AspNetCore.Authorization.Policy;
using RestaurantAPI2.IntegrationTests.Helpers;

namespace RestaurantAPI2.IntegrationTests
{
    public class RestaurantControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {

        private HttpClient _client;

        public RestaurantControllerTests(WebApplicationFactory<Startup> factory)
        {
           //var factory = new WebApplicationFactory<Startup>();  //Tworzymy fabrykę do której przekazujemy typ statupu. Klasa ta przyjmuje jako parametr generyczny entry point, czyli klasę startową projektu,
                                                                 //Dzięki temu klasa WebApplicationFactory będzie wiedziała w jaki sposób uruchomić projekt i odpowiednio go skonfiguruje
            _client = factory
                //WebHostBuilder umożliwi nam modyfikację budowanego webhosta. Jesteśmy w stanie nadpisać
                //konfigurację z rejestracji serwisów, po to aby usunąć istniejącą już rejestracje dbContextu,
                //a następnie zarejestrować nasz własny dbContext, który w tym przypadku będzie instancją InMemory.
                //Teraz API, które będzie działało podczas wywołania testów integracyjnych nie będzie już korzystać z bazy danych
                //msSql, tylko z baz danych InMemory
                .WithWebHostBuilder(builder =>  
                {           
                    builder.ConfigureServices(services =>
                    {
                        var dbContextOptions = services
                            .SingleOrDefault(services => services.ServiceType == typeof(DbContextOptions<RestaurantDbContext>));

                        services.Remove(dbContextOptions);

                        services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();

                        services.AddMvc(option => option.Filters.Add(new FakeUserFilter()));

                        services
                            .AddDbContext<RestaurantDbContext>(options => options.UseInMemoryDatabase("RestaurantDb"));
                    });
                })
                .CreateClient();  // za pomocą tego klienta możemy odwołać się do różnych metod z naszego API
        }


        [Fact]
        public async Task CreateRestaurant_WithValidModel_ReturnsCreatedStatus()
        {
            // arrange
            var model = new CreateRestaurantDto()
            {
                Name = "TestRestaurant",
                City = "Kraków",
                Street = "Długa 5"
            };

            var httpContent = model.ToJsonHttpContent();

            // act
            var response = await _client.PostAsync("/api/restaurant", httpContent);

            // assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
            response.Headers.Location.Should().NotBeNull();
        }


        [Fact]
        public async Task CreateRestaurant_WithInvalidModel_ReturnsBadRequest()
        {
            // arrange
            var model = new CreateRestaurantDto()
            {
                ContactEmail = "test@test.com",
                Description = "test desc",
                ContactNumber = "999 888 777"
            };

            var httpContent = model.ToJsonHttpContent();

            // act
            var response = await _client.PostAsync("/api/restaurant", httpContent);

            // assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }


        [Theory]
        [InlineData("pageSize=5&pageNumber=1")]
        [InlineData("pageSize=10&pageNumber=2")]
        [InlineData("pageSize=15&PageNumber=3")]
        public async Task GetAll_WithCorrectQueryParameters_ReturnsOkResult(string queryParamas)
        {    
            // act

            var response = await _client.GetAsync("/api/restaurant?" + queryParamas);

            // assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK); //sprawdzenie czy Status Code odpowiedzi jest równy wartości OK.

            //W ten sposób jesteśmy w stanie przetestować, czy API zwraca poprawny kod statusu dla prawidłowego zapytania.
        }


        [Theory]
        [InlineData("pageSize=100&pageNumber=1")]
        [InlineData("pageSize=11&pageNumber=2")]
        [InlineData(null)]
        [InlineData("")]
        public async Task GetAll_WithInvalidQueryParameters_ReturnsBadRequest(string queryParams)
        {
            // act

            var response = await _client.GetAsync("/api/restaurant?" + queryParams);

            // assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

    }
}
