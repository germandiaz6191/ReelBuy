using System;
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
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace ReelBuy.Tests.Controllers
{
    public class StoresControllerTests : TestBase
    {
        private readonly Mock<IGenericUnitOfWork<Store>> _genericUnitOfWorkMock;
        private readonly Mock<IStoresUnitOfWork> _storesUnitOfWorkMock;
        private readonly Mock<IUsersUnitOfWork> _usersUnitOfWorkMock;
        private readonly StoresController _controller;
        
        public StoresControllerTests()
        {
            _genericUnitOfWorkMock = new Mock<IGenericUnitOfWork<Store>>();
            _storesUnitOfWorkMock = new Mock<IStoresUnitOfWork>();
            _usersUnitOfWorkMock = new Mock<IUsersUnitOfWork>();
            _controller = new StoresController(
                _genericUnitOfWorkMock.Object,
                _storesUnitOfWorkMock.Object,
                _usersUnitOfWorkMock.Object
            );

            // Setup default authorization
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "test@example.com"),
                new Claim(ClaimTypes.Role, "User")
            };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }
        
        [Fact]
        public async Task GetAsync_AsAdmin_ReturnsAllStores()
        {
            // Arrange
            var stores = new List<Store>
            {
                new Store { Id = 1, Name = "Store 1", UserId = "admin" },
                new Store { Id = 2, Name = "Store 2", UserId = "admin" }
            };
            var user = new User { Id = "admin", Email = "admin@example.com" };
            var actionResponse = new ActionResponse<IEnumerable<Store>>
            {
                WasSuccess = true,
                Result = stores
            };
            _usersUnitOfWorkMock.Setup(u => u.GetUserAsync(It.Is<string>(e => e == "admin@example.com")))
                .ReturnsAsync(user);
            _storesUnitOfWorkMock.Setup(s => s.GetAsync())
                .ReturnsAsync(actionResponse);
            var controller = CreateStoresController();
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.Email),
                        new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, user.Email),
                        new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id),
                        new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, "Admin")
                    }, "Test"))
                }
            };
            // Act
            var result = await controller.GetAsync();
            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedStores = okResult.Value.Should().BeAssignableTo<IEnumerable<Store>>().Subject;
            returnedStores.Should().HaveCount(2);
        }
        
        [Fact]
        public async Task GetAsync_AsUser_ReturnsOnlyUserStores()
        {
            // Arrange
            var userId = "user1";
            var stores = new List<Store>
            {
                new Store { Id = 1, Name = "Store 1", UserId = userId },
                new Store { Id = 2, Name = "Store 2", UserId = userId }
            };
            
            var user = new User { Id = userId, Email = "user1@example.com" };
            
            var actionResponse = new ActionResponse<IEnumerable<Store>>
            {
                WasSuccess = true,
                Result = stores
            };
            
            _usersUnitOfWorkMock.Setup(u => u.GetUserAsync(It.Is<string>(e => e == "user1@example.com")))
                .ReturnsAsync(user);
                
            _storesUnitOfWorkMock.Setup(s => s.GetStoresByUserAsync(userId))
                .ReturnsAsync(actionResponse);
                
            var controller = CreateStoresController();
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.Email),
                        new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, user.Email),
                        new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id),
                        new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, "User")
                    }, "Test"))
                }
            };
            
            // Act
            var result = await controller.GetAsync();
            
            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedStores = okResult.Value.Should().BeAssignableTo<IEnumerable<Store>>().Subject;
            returnedStores.Should().HaveCount(2);
        }
        
        [Fact]
        public async Task GetAsync_WithId_AsAdmin_ReturnsStore()
        {
            // Arrange
            var store = new Store { Id = 1, Name = "Store 1", UserId = "user1" };
            var user = new User { Id = "admin", Email = "admin@example.com" };
            
            var actionResponse = new ActionResponse<Store>
            {
                WasSuccess = true,
                Result = store
            };
            
            _usersUnitOfWorkMock.Setup(u => u.GetUserAsync(It.Is<string>(e => e == "admin@example.com")))
                .ReturnsAsync(user);
                
            _storesUnitOfWorkMock.Setup(s => s.GetAsync(1))
                .ReturnsAsync(actionResponse);
                
            var controller = CreateStoresController();
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.Email),
                        new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, user.Email),
                        new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id),
                        new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, "Admin")
                    }, "Test"))
                }
            };
            
            // Act
            var result = await controller.GetAsync(1);
            
            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedStore = okResult.Value.Should().BeAssignableTo<Store>().Subject;
            returnedStore.Id.Should().Be(1);
            returnedStore.Name.Should().Be("Store 1");
        }
        
        [Fact]
        public async Task GetAsync_WithId_AsOwner_ReturnsStore()
        {
            // Arrange
            var userId = "user1";
            var store = new Store { Id = 1, Name = "Store 1", UserId = userId };
            var user = new User { Id = userId, Email = "user1@example.com" };
            
            var actionResponse = new ActionResponse<Store>
            {
                WasSuccess = true,
                Result = store
            };
            
            _usersUnitOfWorkMock.Setup(u => u.GetUserAsync(It.Is<string>(e => e == "user1@example.com")))
                .ReturnsAsync(user);
                
            _storesUnitOfWorkMock.Setup(s => s.GetAsync(1))
                .ReturnsAsync(actionResponse);
                
            var controller = CreateStoresController();
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.Email),
                        new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, user.Email),
                        new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id),
                        new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, "Admin")
                    }, "Test"))
                }
            };
            
            // Act
            var result = await controller.GetAsync(1);
            
            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedStore = okResult.Value.Should().BeAssignableTo<Store>().Subject;
            returnedStore.Id.Should().Be(1);
            returnedStore.Name.Should().Be("Store 1");
        }
        
        [Fact]
        public async Task GetAsync_WithId_NotOwnerNotAdmin_ReturnsForbid()
        {
            // Arrange
            var store = new Store { Id = 1, Name = "Store 1", UserId = "otherUser" };
            var user = new User { Id = "user1", Email = "user1@example.com" };
            
            var actionResponse = new ActionResponse<Store>
            {
                WasSuccess = true,
                Result = store
            };
            
            _usersUnitOfWorkMock.Setup(u => u.GetUserAsync(It.Is<string>(e => e == "user1@example.com")))
                .ReturnsAsync(user);
                
            _storesUnitOfWorkMock.Setup(s => s.GetAsync(1))
                .ReturnsAsync(actionResponse);
                
            var controller = CreateStoresController();
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.Email),
                        new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, user.Email),
                        new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id),
                        new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, "User")
                    }, "Test"))
                }
            };
            
            // Act
            var result = await controller.GetAsync(1);
            
            // Assert
            result.Should().BeOfType<ForbidResult>();
        }
        
        [Fact]
        public async Task PostAsync_AsAdmin_CreatesStore()
        {
            // Arrange
            var storeDTO = new StoreDTO
            {
                Name = "New Store",
                UserId = "user1"
            };
            
            var store = new Store
            {
                Id = 1,
                Name = "New Store",
                UserId = "user1"
            };
            
            var user = new User { Id = "admin", Email = "admin@example.com" };
            
            var actionResponse = new ActionResponse<Store>
            {
                WasSuccess = true,
                Result = store
            };
            
            _usersUnitOfWorkMock.Setup(u => u.GetUserAsync(It.Is<string>(e => e == "admin@example.com")))
                .ReturnsAsync(user);
                
            _storesUnitOfWorkMock.Setup(s => s.AddAsync(It.IsAny<StoreDTO>()))
                .ReturnsAsync(actionResponse);
                
            var controller = CreateStoresController();
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.Email),
                        new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, user.Email),
                        new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id),
                        new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, "Admin")
                    }, "Test"))
                }
            };
            
            // Act
            var result = await controller.PostAsync(storeDTO);
            
            // Assert
            var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
            var returnedStore = createdResult.Value.Should().BeAssignableTo<Store>().Subject;
            returnedStore.Id.Should().Be(1);
            returnedStore.Name.Should().Be("New Store");
        }
        
        [Fact]
        public async Task GetStoresByUserAsync_AsAdmin_ReturnsStores()
        {
            // Arrange
            var user = new User { Id = "admin", Email = "admin@example.com" };
            var stores = new List<Store> { new Store { Id = 1, Name = "Store 1", UserId = user.Id } };
            var actionResponse = new ActionResponse<IEnumerable<Store>> { WasSuccess = true, Result = stores };
            _usersUnitOfWorkMock.Setup(u => u.GetUserAsync(It.IsAny<string>())).ReturnsAsync(user);
            _storesUnitOfWorkMock.Setup(s => s.GetStoresByUserAsync(user.Id)).ReturnsAsync(actionResponse);
            var controller = CreateStoresController();
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, user.Email),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.NameIdentifier, user.Id),
                        new Claim(ClaimTypes.Role, "Admin")
                    }, "Test"))
                }
            };
            // Act
            var result = await controller.GetStoresByUserAsync(user.Id);
            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedStores = okResult.Value.Should().BeAssignableTo<IEnumerable<Store>>().Subject;
            returnedStores.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetStoresByUserAsync_NotOwnerNotAdmin_ReturnsEmptyList()
        {
            // Arrange
            var user = new User { Id = "user1", Email = "user1@example.com" };
            var otherUserId = "otherUser";
            _usersUnitOfWorkMock.Setup(u => u.GetUserAsync(It.IsAny<string>())).ReturnsAsync(user);
            var controller = CreateStoresController();
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, user.Email),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.NameIdentifier, user.Id),
                        new Claim(ClaimTypes.Role, "User")
                    }, "Test"))
                }
            };
            // Act
            var result = await controller.GetStoresByUserAsync(otherUserId);
            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedStores = okResult.Value.Should().BeAssignableTo<IEnumerable<Store>>().Subject;
            returnedStores.Should().BeEmpty();
        }

        [Fact]
        public async Task GetComboAsync_AsAdmin_ReturnsAllStores()
        {
            // Arrange
            var user = new User { Id = "admin", Email = "admin@example.com" };
            var stores = new List<Store> { new Store { Id = 1, Name = "Store 1", UserId = user.Id } };
            _usersUnitOfWorkMock.Setup(u => u.GetUserAsync(It.IsAny<string>())).ReturnsAsync(user);
            _storesUnitOfWorkMock.Setup(s => s.GetComboAsync()).ReturnsAsync(stores);
            var controller = CreateStoresController();
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, user.Email),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.NameIdentifier, user.Id),
                        new Claim(ClaimTypes.Role, "Admin")
                    }, "Test"))
                }
            };
            // Act
            var result = await controller.GetComboAsync();
            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedStores = okResult.Value.Should().BeAssignableTo<IEnumerable<Store>>().Subject;
            returnedStores.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetComboAsync_AsUser_ReturnsOwnStores()
        {
            // Arrange
            var user = new User { Id = "user1", Email = "user1@example.com" };
            var stores = new List<Store> { new Store { Id = 1, Name = "Store 1", UserId = user.Id } };
            var actionResponse = new ActionResponse<IEnumerable<Store>> { WasSuccess = true, Result = stores };
            _usersUnitOfWorkMock.Setup(u => u.GetUserAsync(It.IsAny<string>())).ReturnsAsync(user);
            _storesUnitOfWorkMock.Setup(s => s.GetStoresByUserAsync(user.Id)).ReturnsAsync(actionResponse);
            var controller = CreateStoresController();
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, user.Email),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.NameIdentifier, user.Id),
                        new Claim(ClaimTypes.Role, "User")
                    }, "Test"))
                }
            };
            // Act
            var result = await controller.GetComboAsync();
            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedStores = okResult.Value.Should().BeAssignableTo<IEnumerable<Store>>().Subject;
            returnedStores.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetAsync_WithId_NotFound_ReturnsNotFound()
        {
            // Arrange
            var user = new User { Id = "admin", Email = "admin@example.com" };
            _usersUnitOfWorkMock.Setup(u => u.GetUserAsync(It.IsAny<string>())).ReturnsAsync(user);
            _storesUnitOfWorkMock.Setup(s => s.GetAsync(99)).ReturnsAsync(new ActionResponse<Store> { WasSuccess = false, Result = null, Message = "Not found" });
            var controller = CreateStoresController();
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, user.Email),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.NameIdentifier, user.Id),
                        new Claim(ClaimTypes.Role, "Admin")
                    }, "Test"))
                }
            };
            // Act
            var result = await controller.GetAsync(99);
            // Assert
            var notFound = result.Should().BeOfType<NotFoundObjectResult>().Subject;
            notFound.Value.Should().Be("Not found");
        }

        [Fact]
        public async Task GetAsync_WithId_UserNotOwner_ReturnsForbid()
        {
            // Arrange
            var user = new User { Id = "user1", Email = "user1@example.com" };
            var store = new Store { Id = 1, Name = "Store 1", UserId = "otherUser" };
            var actionResponse = new ActionResponse<Store>
            {
                WasSuccess = true,
                Result = store
            };
            _usersUnitOfWorkMock.Setup(u => u.GetUserAsync(It.IsAny<string>())).ReturnsAsync(user);
            _storesUnitOfWorkMock.Setup(s => s.GetAsync(1)).ReturnsAsync(actionResponse);
            var controller = CreateStoresController();
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, user.Email),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.NameIdentifier, user.Id),
                        new Claim(ClaimTypes.Role, "User")
                    }, "Test"))
                }
            };
            // Act
            var result = await controller.GetAsync(1);
            // Assert
            result.Should().BeOfType<ForbidResult>();
        }

        [Fact]
        public async Task GetTotalRecordsAsync_AsAdmin_ReturnsOk()
        {
            // Arrange
            var user = new User { Id = "admin", Email = "admin@example.com" };
            var actionResponse = new ActionResponse<int> { WasSuccess = true, Result = 5 };
            _usersUnitOfWorkMock.Setup(u => u.GetUserAsync(It.IsAny<string>())).ReturnsAsync(user);
            _storesUnitOfWorkMock.Setup(s => s.GetTotalRecordsAsync(It.IsAny<PaginationDTO>())).ReturnsAsync(actionResponse);
            var controller = CreateStoresController();
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, user.Email),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.NameIdentifier, user.Id),
                        new Claim(ClaimTypes.Role, "Admin")
                    }, "Test"))
                }
            };
            // Act
            var result = await controller.GetTotalRecordsAsync(new PaginationDTO());
            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be(5);
        }

        [Fact]
        public async Task GetTotalRecordsAsync_AsUser_ReturnsOk()
        {
            // Arrange
            var user = new User { Id = "user1", Email = "user1@example.com" };
            var actionResponse = new ActionResponse<int> { WasSuccess = true, Result = 2 };
            _usersUnitOfWorkMock.Setup(u => u.GetUserAsync(It.IsAny<string>())).ReturnsAsync(user);
            _storesUnitOfWorkMock.Setup(s => s.GetTotalRecordsAsync(It.IsAny<PaginationDTO>())).ReturnsAsync(actionResponse);
            var controller = CreateStoresController();
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, user.Email),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.NameIdentifier, user.Id),
                        new Claim(ClaimTypes.Role, "User")
                    }, "Test"))
                }
            };
            // Act
            var result = await controller.GetTotalRecordsAsync(new PaginationDTO());
            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be(2);
        }
        
        private StoresController CreateStoresController()
        {
            return new StoresController(
                _genericUnitOfWorkMock.Object,
                _storesUnitOfWorkMock.Object,
                _usersUnitOfWorkMock.Object
            );
        }
    }
} 