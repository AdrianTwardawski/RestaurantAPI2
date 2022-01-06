using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
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
using RestaurantAPI2.Authorization;
using RestaurantAPI2.Entities;
using RestaurantAPI2.Middleware;
using RestaurantAPI2.MIddleware;
using RestaurantAPI2.Models;
using RestaurantAPI2.Models.Validator;
using RestaurantAPI2.Models.Validators;
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

            /*odnosz¹c siê do sekcji Authentication  z appsetting.json Bindujemy (³¹czymy wartoœci) do authenticationSettings,
            czyli po tej linii kodu wartoœci, które mamy w pliku appsetting.json bêd¹ dostêpne na obiekcie authenticationSettings*/
            Configuration.GetSection("Authentication").Bind(authenticationSettings);

            services.AddSingleton(authenticationSettings);
            services.AddAuthentication(option =>     //konfiguracja Autentykacji
            {
                option.DefaultAuthenticateScheme = "Bearer"; //Domyœlny schemat autentykacji
                option.DefaultScheme = "Bearer";
                option.DefaultChallengeScheme = "Bearer";
            }).AddJwtBearer(cfg =>
            {
                cfg.RequireHttpsMetadata = false; //nie wymuszamy od klienta tokena przez protokó³ https
                cfg.SaveToken = true; //token powinien zostaæ zapisany po stronie servera do celów autentykacji
                cfg.TokenValidationParameters = new TokenValidationParameters 
                //parametry walidacji, sprawdzenie czy token wys³any przez klienta jest zgodny z tym co wie server
                {
                    ValidIssuer = authenticationSettings.JwtIssuer, //Issuer - wydawca danego tokenu
                    ValidAudience = authenticationSettings.JwtIssuer, //Audience - jakie podmioty mog¹ u¿ywaæ danego tokenu
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtKey)),
                    //klucz prywatny wygenerowany na podstawie tej wartoœci JwtKey, która zosta³a zapisana w appsetting.json
                };
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("HasNationality", builder => builder.RequireClaim("Nationality", "German", "Polish")); //auoryzacja wartoœci¹ Claim'u
                options.AddPolicy("AtLeast20", builder => builder.AddRequirements(new MinimumAgeRequirement(20)));
                options.AddPolicy("CreatedAtLeast2Restaurants", builder => builder.AddRequirements(new CreatedMultipleRestaurantsRequirement(2)));
                
            });
            services.AddScoped<IAuthorizationHandler, MinimumAgeRequirementHandler>();
            services.AddScoped<IAuthorizationHandler, ResourceOperationRequirementHandler>();
            services.AddScoped<IAuthorizationHandler, CreatedMultipleRestaurantsRequirementHandler>();
            services.AddControllers().AddFluentValidation();
            services.AddDbContext<RestaurantDbContext>(); //rejestracja kontekstu bazy danych
            services.AddScoped<RestaurantSeeder>(); //rejestracja serwisu seeduj¹cego
            services.AddAutoMapper(this.GetType().Assembly); //rejestracja AutoMappera
            services.AddScoped<IRestaurantService, RestaurantService>(); //rejestacja serwisu do obs³ugi metod kontrolera
            services.AddScoped<IDishService, DishService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ErrorHandlingMiddleware>(); //rejestracja serwisu do obs³ugi wyj¹tków
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            services.AddScoped<IValidator<RegisterUserDto>, RegisterUserDtoValidator>();
            services.AddScoped<IValidator<RestaurantQuery>, RestaurantQueryValidator>();
            services.AddScoped<RequestTimeMiddleware>();
            services.AddScoped<IUserContextService, UserContextService>();
            services.AddHttpContextAccessor(); //umo¿liwia wstrzykniêcie IHttpContextAccessor w UserContextService.cs
            services.AddSwaggerGen();
            services.AddCors(options =>
            {
                options.AddPolicy("FrontedClient", builder =>

                    builder.AllowAnyMethod()
                        .AllowAnyHeader()
                        .WithOrigins(Configuration["AllowedOrigins"])

                );                
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, RestaurantSeeder seeder)
        {
            app.UseCors("FrontEndClient");
            seeder.Seed(); /*ka¿de zapytanie do API przejdzie przez proces seedowanie, przez co encje
            zostan¹ dodane ju¿ przy pierwszym zapytaniu w API*/
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<ErrorHandlingMiddleware>();
            app.UseMiddleware<RequestTimeMiddleware>();
            app.UseAuthentication(); //ka¿dy request wys³any przez klienta API bêdzie podlega³ procesowi Autentykacji
            app.UseHttpsRedirection();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Restaurant API");
            });

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
