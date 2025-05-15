using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using ReelBuy.Backend.Repositories.Interfaces;
using ReelBuy.Backend.UnitsOfWork.Implementations;
using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Responses;
using Xunit;

namespace ReelBuy.Tests.UnitsOfWork
{
    public class CitiesUnitOfWorkTests
    {
        private readonly Mock<IGenericRepository<City>> _genericRepositoryMock;
        private readonly Mock<ICitiesRepository> _citiesRepositoryMock;

        public CitiesUnitOfWorkTests()
        {
            _genericRepositoryMock = new Mock<IGenericRepository<City>>();
            _citiesRepositoryMock = new Mock<ICitiesRepository>();
        }

        [Fact]
        public async Task GetAsync_ShouldReturnCityList()
        {
            // Arrange
            var cities = new List<City>
            {
                new City { Id = 1, Name = "City 1" },
                new City { Id = 2, Name = "City 2" }
            };

            var actionResponse = new ActionResponse<IEnumerable<City>>
            {
                WasSuccess = true,
                Result = cities
            };

            _citiesRepositoryMock.Setup(r => r.GetAsync())
                .ReturnsAsync(actionResponse);

            var unitOfWork = new CitiesUnitOfWork(_genericRepositoryMock.Object, _citiesRepositoryMock.Object);

            // Act
            var result = await unitOfWork.GetAsync();

            // Assert
            result.WasSuccess.Should().BeTrue();
            result.Result.Should().HaveCount(2);
            result.Result.Should().Contain(c => c.Name == "City 1");
            result.Result.Should().Contain(c => c.Name == "City 2");
        }

        [Fact]
        public async Task GetAsync_WithId_ShouldReturnCity()
        {
            // Arrange
            var city = new City { Id = 1, Name = "City 1" };

            var actionResponse = new ActionResponse<City>
            {
                WasSuccess = true,
                Result = city
            };

            _citiesRepositoryMock.Setup(r => r.GetAsync(1))
                .ReturnsAsync(actionResponse);

            var unitOfWork = new CitiesUnitOfWork(_genericRepositoryMock.Object, _citiesRepositoryMock.Object);

            // Act
            var result = await unitOfWork.GetAsync(1);

            // Assert
            result.WasSuccess.Should().BeTrue();
            result.Result.Should().NotBeNull();
            result.Result!.Id.Should().Be(1);
            result.Result.Name.Should().Be("City 1");
        }

        [Fact]
        public async Task GetComboAsync_ShouldReturnCities()
        {
            // Arrange
            var cities = new List<City>
            {
                new City { Id = 1, Name = "City 1" },
                new City { Id = 2, Name = "City 2" }
            };

            _citiesRepositoryMock.Setup(r => r.GetComboAsync())
                .ReturnsAsync(cities);

            var unitOfWork = new CitiesUnitOfWork(_genericRepositoryMock.Object, _citiesRepositoryMock.Object);

            // Act
            var result = await unitOfWork.GetComboAsync();

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(c => c.Name == "City 1");
            result.Should().Contain(c => c.Name == "City 2");
        }

        [Fact]
        public async Task GetAsync_WithPagination_ShouldReturnPaginatedCities()
        {
            // Arrange
            var pagination = new PaginationDTO
            {
                Page = 1,
                RecordsNumber = 10
            };

            var cities = new List<City>
            {
                new City { Id = 1, Name = "City 1" },
                new City { Id = 2, Name = "City 2" }
            };

            var actionResponse = new ActionResponse<IEnumerable<City>>
            {
                WasSuccess = true,
                Result = cities
            };

            _citiesRepositoryMock.Setup(r => r.GetAsync(pagination))
                .ReturnsAsync(actionResponse);

            var unitOfWork = new CitiesUnitOfWork(_genericRepositoryMock.Object, _citiesRepositoryMock.Object);

            // Act
            var result = await unitOfWork.GetAsync(pagination);

            // Assert
            result.WasSuccess.Should().BeTrue();
            result.Result.Should().HaveCount(2);
            result.Result.Should().Contain(c => c.Name == "City 1");
            result.Result.Should().Contain(c => c.Name == "City 2");
        }

        [Fact]
        public async Task GetTotalRecordsAsync_ShouldReturnTotalRecords()
        {
            // Arrange
            var pagination = new PaginationDTO
            {
                Page = 1,
                RecordsNumber = 10
            };

            var actionResponse = new ActionResponse<int>
            {
                WasSuccess = true,
                Result = 10
            };

            _citiesRepositoryMock.Setup(r => r.GetTotalRecordsAsync(pagination))
                .ReturnsAsync(actionResponse);

            var unitOfWork = new CitiesUnitOfWork(_genericRepositoryMock.Object, _citiesRepositoryMock.Object);

            // Act
            var result = await unitOfWork.GetTotalRecordsAsync(pagination);

            // Assert
            result.WasSuccess.Should().BeTrue();
            result.Result.Should().Be(10);
        }

        [Fact]
        public async Task AddAsync_ShouldAddCity()
        {
            // Arrange
            var cityDTO = new CityDTO
            {
                Name = "New City",
                DepartmentId = 1
            };

            var city = new City
            {
                Id = 1,
                Name = "New City",
                DepartmentId = 1
            };

            var actionResponse = new ActionResponse<City>
            {
                WasSuccess = true,
                Result = city
            };

            _citiesRepositoryMock.Setup(r => r.AddAsync(cityDTO))
                .ReturnsAsync(actionResponse);

            var unitOfWork = new CitiesUnitOfWork(_genericRepositoryMock.Object, _citiesRepositoryMock.Object);

            // Act
            var result = await unitOfWork.AddAsync(cityDTO);

            // Assert
            result.WasSuccess.Should().BeTrue();
            result.Result.Should().NotBeNull();
            result.Result!.Id.Should().Be(1);
            result.Result.Name.Should().Be("New City");
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateCity()
        {
            // Arrange
            var cityDTO = new CityDTO
            {
                Id = 1,
                Name = "Updated City",
                DepartmentId = 1
            };

            var city = new City
            {
                Id = 1,
                Name = "Updated City",
                DepartmentId = 1
            };

            var actionResponse = new ActionResponse<City>
            {
                WasSuccess = true,
                Result = city
            };

            _citiesRepositoryMock.Setup(r => r.UpdateAsync(cityDTO))
                .ReturnsAsync(actionResponse);

            var unitOfWork = new CitiesUnitOfWork(_genericRepositoryMock.Object, _citiesRepositoryMock.Object);

            // Act
            var result = await unitOfWork.UpdateAsync(cityDTO);

            // Assert
            result.WasSuccess.Should().BeTrue();
            result.Result.Should().NotBeNull();
            result.Result!.Id.Should().Be(1);
            result.Result.Name.Should().Be("Updated City");
        }
    }
} 