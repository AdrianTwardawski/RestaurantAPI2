using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using RestaurantAPI2.Entities;
using RestaurantAPI2.Middleware;
using RestaurantAPI2.MIddleware;
using RestaurantAPI2.Models;
using RestaurantAPI2.Models.Validator;
using RestaurantAPI2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantAPI2
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var authenticationSettings = new AuthenticationSettings();

            /*odnosz�c si� do sekcji Authentication  z appsetting.json Bindujemy (��czymy warto�ci) do authenticationSettings,
            czyli po tej linii kodu warto�ci, kt�re mamy w pliku appsetting.json b�d� dost�pne na obiekcie authenticationSettings*/
            Configuration.GetSection("Authentication").Bind(authenticationSettings);

            services.AddSingleton(authenticationSettings);
            services.AddAuthentication(option =>     //konfiguracja Autentykacji
            {
                option.DefaultAuthenticateScheme = "Bearer"; //Domy�lny schemat autentykacji
                option.DefaultScheme = "Bearer";
                option.DefaultChallengeScheme = "Bearer";
            }).AddJwtBearer(cfg =>
            {
                cfg.RequireHttpsMetadata = false; //nie wymuszamy od klienta tokena przez protok� https
                cfg.SaveToken = true; //token powinien zosta� zapisany po stronie servera do cel�w autentykacji
                cfg.TokenValidationParameters = new TokenValidationParameters 
                //parametry walidacji, sprawdzenie czy token wys�any przez klienta jest zgodny z tym co wie server
                {
                    ValidIssuer = authenticationSettings.JwtIssuer, //Issuer - wydawca danego tokenu
                    ValidAudience = authenticationSettings.JwtIssuer, //Audience - jakie podmioty mog� u�ywa� danego tokenu
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtKey)),
                    //klucz prywatny wygenerowany na podstawie tej warto�ci JwtKey, kt�ra zosta�a zapisana w appsetting.json
                };
            });
            
            services.AddControllers().AddFluentValidation();
            services.AddDbContext<RestaurantDbContext>(); //rejestracja kontekstu bazy danych
            services.AddScoped<RestaurantSeeder>(); //rejestracja serwisu seeduj�cego
            services.AddAutoMapper(this.GetType().Assembly); //rejestracja AutoMappera
            services.AddScoped<IRestaurantService, RestaurantService>(); //rejestacja serwisu do obs�ugi metod kontrolera
            services.AddScoped<IDishService, DishService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ErrorHandlingMiddleware>(); //rejestracja serwisu do obs�ugi wyj�tk�w
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            services.AddScoped<IValidator<RegisterUserDto>, RegisterUserDtoValidator>();
            services.AddScoped<RequestTimeMiddleware>();
            services.AddSwaggerGen();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, RestaurantSeeder seeder)
        {
            seeder.Seed(); /*ka�de zapytanie do API przejdzie przez proces seedowanie, przez co encje
            zostan� dodane ju� przy pierwszym zapytaniu w API*/
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<ErrorHandlingMiddleware>();
            app.UseMiddleware<RequestTimeMiddleware>();
            app.UseAuthentication(); //ka�dy request wys�any przez klienta API b�dzie podlega� procesowi Autentykacji
            app.UseHttpsRedirection();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Restaurant API");
            });

            app.UseRouting();
 
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
