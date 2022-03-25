using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using FluentAssertions;
using System.Net.Http;

namespace RestaurantAPI2.IntegrationTests
{
    public class RestaurantControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {

        private HttpClient _client;

        public RestaurantControllerTests(WebApplicationFactory<Startup> factory)
        {
           //var factory = new WebApplicationFactory<Startup>();  //Tworzymy fabrykę do której przekazujemy typ statupu. Klasa ta przyjmuje jako parametr generyczny entry point, czyli klasę startową projektu,
                                                                 //Dzięki temu klasa WebApplicationFactory będzie wiedziała w jaki sposób uruchomić projekt i odpowiednio go skonfiguruje
            _client = factory.CreateClient();  // za pomocą tego klienta możemy odwołać się do różnych metod z naszego API
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
