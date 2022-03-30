using FluentValidation.TestHelper;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI2.Entities;
using RestaurantAPI2.Models;
using RestaurantAPI2.Models.Validator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace RestaurantAPI2.IntegrationTests.Validators
{
    public class RegisterUserDtoValidatorTests
    {
        private RestaurantDbContext _dbContext;
        public static IEnumerable<object[]> GetSampleValidData()
        {
            var list = new List<RegisterUserDto>()
            {
                new RegisterUserDto()
                {
                    Email = "test@test.com",
                    Password = "pass123",
                    ConfirmPassword = "pass123"
                },
                new RegisterUserDto()
                {
                    Email = "test4@test.com",
                    Password = "pass883",
                    ConfirmPassword = "pass883"
                },
                new RegisterUserDto()
                {
                    Email = "test5@test.com",
                    Password = "pass093",
                    ConfirmPassword = "pass093"
                }
            };

            return list.Select(q => new object[] { q });
        }

        public static IEnumerable<object[]> GetSampleInvalidData()
        {
            var list = new List<RegisterUserDto>()
            {
                new RegisterUserDto()
                {
                    Email = "test2@test.com",
                    Password = "pass123",
                    ConfirmPassword = "pass123"
                },
                new RegisterUserDto()
                {
                    Email = "test3@test.com",
                    Password = "pass883",
                    ConfirmPassword = "pass883"
                },
                new RegisterUserDto()
                {
                    Email = "test1@test.com",
                    Password = "pass123",
                    ConfirmPassword = "pass321"
                }
            };

            return list.Select(q => new object[] { q });
        }

        public RegisterUserDtoValidatorTests()
        {
            var builder = new DbContextOptionsBuilder<RestaurantDbContext>();
            builder.UseInMemoryDatabase("TestDb");

            _dbContext = new RestaurantDbContext(builder.Options);
            Seed();
        }

        public void Seed()
        {
            var testUsers = new List<User>()
            {
                new User()
                {
                    Email = "test2@test.com"
                },
                new User()
                {
                    Email = "test3@test.com"
                }
            };

            _dbContext.Users.AddRange(testUsers);
            _dbContext.SaveChanges();
        }

        [Theory]
        [MemberData(nameof(GetSampleValidData))]
        public void Validate_ForValidModel_ReturnsSuccess(RegisterUserDto model)
        {
             // arrange                 
            var validator = new RegisterUserDtoValidator(_dbContext);

            // act
            var result = validator.TestValidate(model);

            // assert
            result.ShouldNotHaveAnyValidationErrors();

        }

        [Theory]
        [MemberData(nameof(GetSampleInvalidData))]
        public void Validate_ForInValidModel_ReturnsFailure(RegisterUserDto model)
        {
            // arrange
            var validator = new RegisterUserDtoValidator(_dbContext);

            // act
            var result = validator.TestValidate(model);

            // assert
            result.ShouldHaveAnyValidationError();

        }
    }
}
