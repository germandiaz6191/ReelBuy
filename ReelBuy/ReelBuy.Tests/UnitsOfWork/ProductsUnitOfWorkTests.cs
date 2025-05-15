using System;
using System.Collections.Generic;
using System.Linq;
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
    public class ProductsUnitOfWorkTests
    {
        private readonly Mock<IGenericRepository<Product>> _genericRepositoryMock;
        private readonly Mock<IProductsRepository> _productsRepositoryMock;

        public ProductsUnitOfWorkTests()
        {
            _genericRepositoryMock = new Mock<IGenericRepository<Product>>();
            _productsRepositoryMock = new Mock<IProductsRepository>();
        }

        [Fact]
        public async Task GetAsync_ShouldReturnProductList()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Product 1" },
                new Product { Id = 2, Name = "Product 2" }
            };

            var actionResponse = new ActionResponse<IEnumerable<Product>>
            {
                WasSuccess = true,
                Result = products
            };

            _productsRepositoryMock.Setup(r => r.GetAsync())
                .ReturnsAsync(actionResponse);

            var unitOfWork = new ProductsUnitOfWork(_genericRepositoryMock.Object, _productsRepositoryMock.Object);

            // Act
            var result = await unitOfWork.GetAsync();

            // Assert
            result.WasSuccess.Should().BeTrue();
            result.Result.Should().HaveCount(2);
            result.Result.Should().Contain(p => p.Name == "Product 1");
            result.Result.Should().Contain(p => p.Name == "Product 2");
        }

        [Fact]
        public async Task GetAsync_WithId_ShouldReturnProduct()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Product 1" };

            var actionResponse = new ActionResponse<Product>
            {
                WasSuccess = true,
                Result = product
            };

            _productsRepositoryMock.Setup(r => r.GetAsync(1))
                .ReturnsAsync(actionResponse);

            var unitOfWork = new ProductsUnitOfWork(_genericRepositoryMock.Object, _productsRepositoryMock.Object);

            // Act
            var result = await unitOfWork.GetAsync(1);

            // Assert
            result.WasSuccess.Should().BeTrue();
            result.Result.Should().NotBeNull();
            result.Result!.Id.Should().Be(1);
            result.Result.Name.Should().Be("Product 1");
        }

        [Fact]
        public async Task GetComboAsync_ShouldReturnProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Product 1" },
                new Product { Id = 2, Name = "Product 2" }
            };

            _productsRepositoryMock.Setup(r => r.GetComboAsync())
                .ReturnsAsync(products);

            var unitOfWork = new ProductsUnitOfWork(_genericRepositoryMock.Object, _productsRepositoryMock.Object);

            // Act
            var result = await unitOfWork.GetComboAsync();

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(p => p.Name == "Product 1");
            result.Should().Contain(p => p.Name == "Product 2");
        }

        [Fact]
        public async Task GetAsync_WithPagination_ShouldReturnPaginatedProducts()
        {
            // Arrange
            var pagination = new PaginationDTO
            {
                Page = 1,
                RecordsNumber = 10
            };

            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Product 1" },
                new Product { Id = 2, Name = "Product 2" }
            };

            var actionResponse = new ActionResponse<IEnumerable<Product>>
            {
                WasSuccess = true,
                Result = products
            };

            _productsRepositoryMock.Setup(r => r.GetAsync(pagination))
                .ReturnsAsync(actionResponse);

            var unitOfWork = new ProductsUnitOfWork(_genericRepositoryMock.Object, _productsRepositoryMock.Object);

            // Act
            var result = await unitOfWork.GetAsync(pagination);

            // Assert
            result.WasSuccess.Should().BeTrue();
            result.Result.Should().HaveCount(2);
            result.Result.Should().Contain(p => p.Name == "Product 1");
            result.Result.Should().Contain(p => p.Name == "Product 2");
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

            _productsRepositoryMock.Setup(r => r.GetTotalRecordsAsync(pagination))
                .ReturnsAsync(actionResponse);

            var unitOfWork = new ProductsUnitOfWork(_genericRepositoryMock.Object, _productsRepositoryMock.Object);

            // Act
            var result = await unitOfWork.GetTotalRecordsAsync(pagination);

            // Assert
            result.WasSuccess.Should().BeTrue();
            result.Result.Should().Be(10);
        }

        [Fact]
        public async Task GetProductsByLikeAsync_ShouldReturnFilteredProducts()
        {
            // Arrange
            var search = new PrincipalSearchDTO
            {
                keyword = "Product"
            };

            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Product 1" },
                new Product { Id = 2, Name = "Product 2" }
            };

            var actionResponse = new ActionResponse<IEnumerable<Product>>
            {
                WasSuccess = true,
                Result = products
            };

            _productsRepositoryMock.Setup(r => r.GetProductsByLikeAsync(search))
                .ReturnsAsync(actionResponse);

            var unitOfWork = new ProductsUnitOfWork(_genericRepositoryMock.Object, _productsRepositoryMock.Object);

            // Act
            var result = await unitOfWork.GetProductsByLikeAsync(search);

            // Assert
            result.WasSuccess.Should().BeTrue();
            result.Result.Should().HaveCount(2);
            result.Result.Should().Contain(p => p.Name == "Product 1");
            result.Result.Should().Contain(p => p.Name == "Product 2");
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Product 1" },
                new Product { Id = 2, Name = "Product 2" }
            };

            var actionResponse = new ActionResponse<int>
            {
                WasSuccess = true,
                Result = 2
            };

            _productsRepositoryMock.Setup(r => r.UpdateAsync(products))
                .ReturnsAsync(actionResponse);

            var unitOfWork = new ProductsUnitOfWork(_genericRepositoryMock.Object, _productsRepositoryMock.Object);

            // Act
            var result = await unitOfWork.UpdateAsync(products);

            // Assert
            result.WasSuccess.Should().BeTrue();
            result.Result.Should().Be(2);
        }
    }
} 