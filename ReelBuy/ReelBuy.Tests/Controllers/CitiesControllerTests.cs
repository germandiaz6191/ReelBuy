using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ReelBuy.Backend.Controllers;
using ReelBuy.Backend.UnitsOfWork.Implementations;
using ReelBuy.Backend.UnitsOfWork.Interfaces;
using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Responses;
using Xunit;

namespace ReelBuy.Tests.Controllers
{
    public class CitiesControllerTests : TestBase
    {
        private readonly Mock<IGenericUnitOfWork<City>> _mockUnitOfWork;
        private readonly Mock<ICitiesUnitOfWork> _citiesUnitOfWorkMock;
        private readonly CitiesController _controller;
        
        public CitiesControllerTests()
        {
            _mockUnitOfWork = new Mock<IGenericUnitOfWork<City>>();
            _citiesUnitOfWorkMock = new Mock<ICitiesUnitOfWork>();
            _controller = new CitiesController(_mockUnitOfWork.Object, _citiesUnitOfWorkMock.Object);
        }
        
        [Fact]
        public async Task GetAsync_ReturnsOkResult_WhenSuccess()
        {
            // Arrange
            var expectedResponse = new ActionResponse<IEnumerable<City>>
            {
                WasSuccess = true,
                Result = new List<City> { new City { Id = 1, Name = "Test" } }
            };
            _citiesUnitOfWorkMock.Setup(x => x.GetAsync()).ReturnsAsync(expectedResponse);
            
            // Act
            var result = await _controller.GetAsync();
            
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<City>>(okResult.Value);
            Assert.Single(returnValue);
        }
        
        [Fact]
        public async Task GetAsync_ReturnsBadRequest_WhenFailure()
        {
            // Arrange
            var expectedResponse = new ActionResponse<IEnumerable<City>>
            {
                WasSuccess = false,
                Message = "Error"
            };
            _citiesUnitOfWorkMock.Setup(x => x.GetAsync()).ReturnsAsync(expectedResponse);
            
            // Act
            var result = await _controller.GetAsync();
            
            // Assert
            Assert.IsType<BadRequestResult>(result);
        }
        
        [Fact]
        public async Task GetAsync_WithId_ReturnsOkResult_WhenSuccess()
        {
            // Arrange
            var expectedResponse = new ActionResponse<City>
            {
                WasSuccess = true,
                Result = new City { Id = 1, Name = "Test" }
            };
            _citiesUnitOfWorkMock.Setup(x => x.GetAsync(1)).ReturnsAsync(expectedResponse);
            
            // Act
            var result = await _controller.GetAsync(1);
            
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<City>(okResult.Value);
            Assert.Equal(1, returnValue.Id);
        }
        
        [Fact]
        public async Task GetAsync_WithId_ReturnsNotFound_WhenFailure()
        {
            // Arrange
            var expectedResponse = new ActionResponse<City>
            {
                WasSuccess = false,
                Message = "Not found"
            };
            _citiesUnitOfWorkMock.Setup(x => x.GetAsync(1)).ReturnsAsync(expectedResponse);
            
            // Act
            var result = await _controller.GetAsync(1);
            
            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Not found", notFoundResult.Value);
        }
        
        [Fact]
        public async Task GetComboAsync_ReturnsOkResult()
        {
            // Arrange
            var expectedList = new List<City> { new City { Id = 1, Name = "Test" } };
            _citiesUnitOfWorkMock.Setup(x => x.GetComboAsync()).ReturnsAsync(expectedList);
            
            // Act
            var result = await _controller.GetComboAsync();
            
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<City>>(okResult.Value);
            Assert.Single(returnValue);
        }
        
        [Fact]
        public async Task PostAsync_ReturnsOkResult_WhenSuccess()
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
            
            _citiesUnitOfWorkMock.Setup(c => c.AddAsync(cityDTO))
                .ReturnsAsync(actionResponse);
                
            var controller = new CitiesController(_mockUnitOfWork.Object, _citiesUnitOfWorkMock.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = GetUserWithRole("Admin")
                }
            };
            
            // Act
            var result = await controller.PostAsync(cityDTO);
            
            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedCity = okResult.Value.Should().BeAssignableTo<City>().Subject;
            returnedCity.Id.Should().Be(1);
            returnedCity.Name.Should().Be("New City");
        }
        
        [Fact]
        public async Task PostAsync_ReturnsBadRequest_WhenNotSuccess()
        {
            // Arrange
            var cityDTO = new CityDTO
            {
                Name = "New City",
                DepartmentId = null // Invalid - missing required field
            };
            
            var actionResponse = new ActionResponse<City>
            {
                WasSuccess = false,
                Message = "Department is required"
            };
            
            _citiesUnitOfWorkMock.Setup(c => c.AddAsync(cityDTO))
                .ReturnsAsync(actionResponse);
                
            var controller = new CitiesController(_mockUnitOfWork.Object, _citiesUnitOfWorkMock.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = GetUserWithRole("Admin")
                }
            };
            
            // Act
            var result = await controller.PostAsync(cityDTO);
            
            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().Be("Department is required");
        }
        
        [Fact]
        public async Task PutAsync_ReturnsOkResult_WhenSuccess()
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
            
            _citiesUnitOfWorkMock.Setup(c => c.UpdateAsync(cityDTO))
                .ReturnsAsync(actionResponse);
                
            var controller = new CitiesController(_mockUnitOfWork.Object, _citiesUnitOfWorkMock.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = GetUserWithRole("Admin")
                }
            };
            
            // Act
            var result = await controller.PutAsync(cityDTO);
            
            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedCity = okResult.Value.Should().BeAssignableTo<City>().Subject;
            returnedCity.Id.Should().Be(1);
            returnedCity.Name.Should().Be("Updated City");
        }
        
        [Fact]
        public async Task PutAsync_ReturnsBadRequest_WhenNotSuccess()
        {
            // Arrange
            var cityDTO = new CityDTO
            {
                Id = 999, // Non-existent ID
                Name = "Updated City",
                DepartmentId = 1
            };
            
            var actionResponse = new ActionResponse<City>
            {
                WasSuccess = false,
                Message = "City not found"
            };
            
            _citiesUnitOfWorkMock.Setup(c => c.UpdateAsync(cityDTO))
                .ReturnsAsync(actionResponse);
                
            var controller = new CitiesController(_mockUnitOfWork.Object, _citiesUnitOfWorkMock.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = GetUserWithRole("Admin")
                }
            };
            
            // Act
            var result = await controller.PutAsync(cityDTO);
            
            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().Be("City not found");
        }

        [Fact]
        public async Task GetAsync_WithPagination_ReturnsOkResult_WhenSuccess()
        {
            // Arrange
            var pagination = new PaginationDTO { Page = 1, RecordsNumber = 10 };
            var expectedResponse = new ActionResponse<IEnumerable<City>>
            {
                WasSuccess = true,
                Result = new List<City> { new City { Id = 1, Name = "Test" } }
            };
            _citiesUnitOfWorkMock.Setup(x => x.GetAsync(pagination)).ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.GetAsync(pagination);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<City>>(okResult.Value);
            Assert.Single(returnValue);
        }

        [Fact]
        public async Task GetAsync_WithPagination_ReturnsBadRequest_WhenFailure()
        {
            // Arrange
            var pagination = new PaginationDTO { Page = 1, RecordsNumber = 10 };
            var expectedResponse = new ActionResponse<IEnumerable<City>>
            {
                WasSuccess = false,
                Message = "Error"
            };
            _citiesUnitOfWorkMock.Setup(x => x.GetAsync(pagination)).ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.GetAsync(pagination);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task GetTotalRecordsAsync_ReturnsOkResult_WhenSuccess()
        {
            // Arrange
            var pagination = new PaginationDTO { Page = 1, RecordsNumber = 10 };
            var expectedResponse = new ActionResponse<int>
            {
                WasSuccess = true,
                Result = 1
            };
            _citiesUnitOfWorkMock.Setup(x => x.GetTotalRecordsAsync(pagination)).ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.GetTotalRecordsAsync(pagination);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<int>(okResult.Value);
            Assert.Equal(1, returnValue);
        }

        [Fact]
        public async Task GetTotalRecordsAsync_ReturnsBadRequest_WhenFailure()
        {
            // Arrange
            var pagination = new PaginationDTO { Page = 1, RecordsNumber = 10 };
            var expectedResponse = new ActionResponse<int>
            {
                WasSuccess = false,
                Message = "Error"
            };
            _citiesUnitOfWorkMock.Setup(x => x.GetTotalRecordsAsync(pagination)).ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.GetTotalRecordsAsync(pagination);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }
    }
} 