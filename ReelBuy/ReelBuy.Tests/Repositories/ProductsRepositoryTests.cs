using Microsoft.EntityFrameworkCore;
using ReelBuy.Backend.Data;
using ReelBuy.Backend.Repositories.Implementations;
using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;
using ReelBuy.Shared.Enums;

namespace ReelBuy.Tests.Repositories;

public class ProductsRepositoryTests : IDisposable
{
    private readonly DataContext _context;
    private readonly ProductsRepository _repository;
    private readonly City _testCity;

    public ProductsRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new DataContext(options);
        _repository = new ProductsRepository(_context);

        // Add test data
        var country = new Country { Name = "Test Country" };
        _context.Countries.Add(country);
        _context.SaveChanges();

        var department = new Department { Name = "Test Department", CountryId = country.Id };
        _context.Departments.Add(department);
        _context.SaveChanges();

        _testCity = new City { Name = "Test City", DepartmentId = department.Id };
        _context.Cities.Add(_testCity);
        _context.SaveChanges();

        // Add required status
        var status = new Status { Name = "Estado 1" };
        _context.Statuses.Add(status);

        // Add required category
        var category = new Category { Name = "Categoría 1" };
        _context.Categories.Add(category);

        // Add required marketplace
        var marketplace = new Marketplace { Name = "Marketplace 1", Domain = "marketplace1.com" };
        _context.Marketplaces.Add(marketplace);

        // Add required store
        var store = new Store { Name = "Tienda 1", UserId = "user1", CityId = _testCity.Id };
        _context.Stores.Add(store);

        _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task GetAsync_ShouldReturnAllProducts()
    {
        // Arrange
        var category = await _context.Categories.FirstAsync();
        var status = await _context.Statuses.FirstAsync();
        var marketplace = await _context.Marketplaces.FirstAsync();
        var store = await _context.Stores.FirstAsync();

        var products = new List<Product>
        {
            new() { 
                Name = "Producto 1", 
                Description = "Descripción del producto 1", 
                Price = 100.00m, 
                CategoryId = category.Id, 
                StatusId = status.Id, 
                MarketplaceId = marketplace.Id, 
                StoreId = store.Id 
            },
            new() { 
                Name = "Producto 2", 
                Description = "Descripción del producto 2", 
                Price = 200.00m, 
                CategoryId = category.Id, 
                StatusId = status.Id, 
                MarketplaceId = marketplace.Id, 
                StoreId = store.Id 
            }
        };
        await _context.Products.AddRangeAsync(products);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAsync();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.WasSuccess);
        Assert.NotNull(result.Result);
        Assert.True(result.Result.Any());
        Assert.All(result.Result, product => Assert.NotNull(product));
        Assert.All(result.Result, product => Assert.NotNull(product.Name));
        Assert.All(result.Result, product => Assert.NotNull(product.Description));
        Assert.All(result.Result, product => Assert.True(product.Price > 0));
    }

    [Fact]
    public async Task GetAsync_WithId_ShouldReturnProduct()
    {
        // Arrange
        var category = await _context.Categories.FirstAsync();
        var status = await _context.Statuses.FirstAsync();
        var marketplace = await _context.Marketplaces.FirstAsync();
        var store = await _context.Stores.FirstAsync();

        var product = new Product
        {
            Name = "Producto 1",
            Description = "Descripción del producto 1",
            Price = 100.00m,
            CategoryId = category.Id,
            StatusId = status.Id,
            MarketplaceId = marketplace.Id,
            StoreId = store.Id
        };
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAsync(product.Id);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.WasSuccess);
        Assert.NotNull(result.Result);
        Assert.Equal(product.Id, result.Result.Id);
        Assert.Equal(product.Name, result.Result.Name);
        Assert.Equal(product.Description, result.Result.Description);
        Assert.Equal(product.Price, result.Result.Price);
        Assert.Equal(product.CategoryId, result.Result.CategoryId);
        Assert.Equal(product.StatusId, result.Result.StatusId);
        Assert.Equal(product.MarketplaceId, result.Result.MarketplaceId);
        Assert.Equal(product.StoreId, result.Result.StoreId);
    }

    [Fact]
    public async Task GetAsync_WithInvalidId_ShouldReturnError()
    {
        // Act
        var result = await _repository.GetAsync(999);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetComboAsync_ShouldReturnProducts()
    {
        // Arrange
        var category = new Category { Name = "Categoría 1" };
        var status = new Status { Name = "Estado 1" };
        var marketplace = new Marketplace { Name = "Marketplace 1", Domain = "marketplace1.com" };
        var store = new Store { Name = "Tienda 1", UserId = "user1", CityId = _testCity.Id };

        await _context.Categories.AddAsync(category);
        await _context.Statuses.AddAsync(status);
        await _context.Marketplaces.AddAsync(marketplace);
        await _context.Stores.AddAsync(store);
        await _context.SaveChangesAsync();

        var products = new List<Product>
        {
            new() { Name = "Producto 1", Description = "Descripción del producto 1", Price = 100.00m, CategoryId = category.Id, StatusId = status.Id, MarketplaceId = marketplace.Id, StoreId = store.Id },
            new() { Name = "Producto 2", Description = "Descripción del producto 2", Price = 200.00m, CategoryId = category.Id, StatusId = status.Id, MarketplaceId = marketplace.Id, StoreId = store.Id }
        };
        await _context.Products.AddRangeAsync(products);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetComboAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, product => Assert.NotNull(product));
        Assert.All(result, product => Assert.NotNull(product.Name));
    }

    [Fact]
    public async Task GetAsync_WithPagination_ShouldReturnPaginatedProducts()
    {
        // Arrange
        var category = new Category { Name = "Categoría 1" };
        var status = new Status { Name = "Estado 1" };
        var marketplace = new Marketplace { Name = "Marketplace 1", Domain = "marketplace1.com" };
        var store = new Store { Name = "Tienda 1", UserId = "user1", CityId = _testCity.Id };

        await _context.Categories.AddAsync(category);
        await _context.Statuses.AddAsync(status);
        await _context.Marketplaces.AddAsync(marketplace);
        await _context.Stores.AddAsync(store);
        await _context.SaveChangesAsync();

        var products = new List<Product>
        {
            new() { Name = "Producto 1", Description = "Descripción del producto 1", Price = 100.00m, CategoryId = category.Id, StatusId = status.Id, MarketplaceId = marketplace.Id, StoreId = store.Id },
            new() { Name = "Producto 2", Description = "Descripción del producto 2", Price = 200.00m, CategoryId = category.Id, StatusId = status.Id, MarketplaceId = marketplace.Id, StoreId = store.Id },
            new() { Name = "Producto 3", Description = "Descripción del producto 3", Price = 300.00m, CategoryId = category.Id, StatusId = status.Id, MarketplaceId = marketplace.Id, StoreId = store.Id }
        };
        await _context.Products.AddRangeAsync(products);
        await _context.SaveChangesAsync();

        var pagination = new PaginationDTO { Page = 1, RecordsNumber = 2 };

        // Act
        var result = await _repository.GetAsync(pagination);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.WasSuccess);
        Assert.NotNull(result.Result);
        Assert.Equal(2, result.Result.Count());
        Assert.All(result.Result, product => Assert.NotNull(product));
        Assert.All(result.Result, product => Assert.NotNull(product.Name));
        Assert.All(result.Result, product => Assert.NotNull(product.Description));
        Assert.All(result.Result, product => Assert.True(product.Price > 0));
    }

    [Fact]
    public async Task GetAsync_WithPaginationAndFilter_ShouldReturnFilteredProducts()
    {
        // Arrange
        var category = await _context.Categories.FirstAsync();
        var status = await _context.Statuses.FirstAsync();
        var marketplace = await _context.Marketplaces.FirstAsync();
        var store = await _context.Stores.FirstAsync();

        var products = new List<Product>
        {
            new() { 
                Name = "Producto 1", 
                Description = "Descripción del producto 1", 
                Price = 100.00m, 
                CategoryId = category.Id, 
                StatusId = (int)StatusProduct.Approved, 
                MarketplaceId = marketplace.Id, 
                StoreId = store.Id 
            },
            new() { 
                Name = "Producto 2", 
                Description = "Descripción del producto 2", 
                Price = 200.00m, 
                CategoryId = category.Id, 
                StatusId = (int)StatusProduct.Approved, 
                MarketplaceId = marketplace.Id, 
                StoreId = store.Id 
            },
            new() { 
                Name = "Otro Producto", 
                Description = "Descripción de otro producto", 
                Price = 300.00m, 
                CategoryId = category.Id, 
                StatusId = (int)StatusProduct.Pending, 
                MarketplaceId = marketplace.Id, 
                StoreId = store.Id 
            }
        };
        await _context.Products.AddRangeAsync(products);
        await _context.SaveChangesAsync();

        var pagination = new PaginationDTO 
        { 
            Page = 1, 
            RecordsNumber = 10, 
            Filter = "Producto",
            FilterStatus = (int)StatusProduct.Approved,
            StoreIds = store.Id.ToString()
        };

        // Act
        var result = await _repository.GetAsync(pagination);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.WasSuccess);
        Assert.NotNull(result.Result);
        Assert.True(result.Result.Any());
        Assert.All(result.Result, product => Assert.Contains("Producto", product.Name));
        Assert.All(result.Result, product => Assert.Equal(store.Id, product.StoreId));
        Assert.All(result.Result, product => Assert.Equal((int)StatusProduct.Approved, product.StatusId));
    }

    [Fact]
    public async Task GetTotalRecordsAsync_ShouldReturnTotalCount()
    {
        // Arrange
        var category = await _context.Categories.FirstAsync();
        var status = await _context.Statuses.FirstAsync();
        var marketplace = await _context.Marketplaces.FirstAsync();
        var store = await _context.Stores.FirstAsync();

        var products = new List<Product>
        {
            new() { 
                Name = "Producto 1", 
                Description = "Descripción del producto 1", 
                Price = 100.00m, 
                CategoryId = category.Id, 
                StatusId = (int)StatusProduct.Approved, 
                MarketplaceId = marketplace.Id, 
                StoreId = store.Id 
            },
            new() { 
                Name = "Producto 2", 
                Description = "Descripción del producto 2", 
                Price = 200.00m, 
                CategoryId = category.Id, 
                StatusId = (int)StatusProduct.Approved, 
                MarketplaceId = marketplace.Id, 
                StoreId = store.Id 
            },
            new() { 
                Name = "Otro Producto", 
                Description = "Descripción de otro producto", 
                Price = 300.00m, 
                CategoryId = category.Id, 
                StatusId = (int)StatusProduct.Pending, 
                MarketplaceId = marketplace.Id, 
                StoreId = store.Id 
            }
        };
        await _context.Products.AddRangeAsync(products);
        await _context.SaveChangesAsync();

        var pagination = new PaginationDTO 
        { 
            Filter = "Producto",
            FilterStatus = (int)StatusProduct.Approved,
            StoreIds = store.Id.ToString()
        };

        // Act
        var result = await _repository.GetTotalRecordsAsync(pagination);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.WasSuccess);
        Assert.True(result.Result > 0);
    }

    [Fact]
    public async Task GetProductsByLikeAsync_ShouldReturnProducts()
    {
        // Arrange
        var category = await _context.Categories.FirstAsync();
        var status = await _context.Statuses.FirstAsync();
        var marketplace = await _context.Marketplaces.FirstAsync();
        var store = await _context.Stores.FirstAsync();

        var products = new List<Product>
        {
            new() { 
                Name = "Producto 1", 
                Description = "Descripción del producto 1", 
                Price = 100.00m, 
                CategoryId = category.Id, 
                StatusId = (int)StatusProduct.Approved, 
                MarketplaceId = marketplace.Id, 
                StoreId = store.Id, 
                LikesGroup = 10 
            },
            new() { 
                Name = "Producto 2", 
                Description = "Descripción del producto 2", 
                Price = 200.00m, 
                CategoryId = category.Id, 
                StatusId = (int)StatusProduct.Approved, 
                MarketplaceId = marketplace.Id, 
                StoreId = store.Id, 
                LikesGroup = 5 
            },
            new() { 
                Name = "Otro Producto", 
                Description = "Descripción de otro producto", 
                Price = 300.00m, 
                CategoryId = category.Id, 
                StatusId = (int)StatusProduct.Pending, 
                MarketplaceId = marketplace.Id, 
                StoreId = store.Id, 
                LikesGroup = 15 
            }
        };
        await _context.Products.AddRangeAsync(products);
        await _context.SaveChangesAsync();

        var search = new PrincipalSearchDTO { keyword = "Producto" };

        // Act
        var result = await _repository.GetProductsByLikeAsync(search);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.WasSuccess);
        Assert.NotNull(result.Result);
        Assert.True(result.Result.Any());
        Assert.All(result.Result, product => Assert.Equal((int)StatusProduct.Approved, product.StatusId));
        Assert.All(result.Result, product => Assert.Contains("Producto", product.Name));
        Assert.True(result.Result.First().LikesGroup >= 5);
    }

    [Fact]
    public async Task UpdateAsync_WithValidData_ShouldUpdateProduct()
    {
        // Arrange
        var category = await _context.Categories.FirstAsync();
        var status = await _context.Statuses.FirstAsync();
        var marketplace = await _context.Marketplaces.FirstAsync();
        var store = await _context.Stores.FirstAsync();

        var product = new Product
        {
            Name = "Producto 1",
            Description = "Descripción del producto 1",
            Price = 100.00m,
            CategoryId = category.Id,
            StatusId = status.Id,
            MarketplaceId = marketplace.Id,
            StoreId = store.Id
        };
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        product.Name = "Producto Actualizado";
        product.Description = "Descripción actualizada";
        product.Price = 150.00m;

        // Act
        var result = await _repository.UpdateAsync(product);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.WasSuccess);
        Assert.NotNull(result.Result);
        Assert.Equal("Producto Actualizado", result.Result.Name);
        Assert.Equal("Descripción actualizada", result.Result.Description);
        Assert.Equal(150.00m, result.Result.Price);
        Assert.Equal(category.Id, result.Result.CategoryId);
        Assert.Equal(status.Id, result.Result.StatusId);
        Assert.Equal(marketplace.Id, result.Result.MarketplaceId);
        Assert.Equal(store.Id, result.Result.StoreId);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ShouldReturnError()
    {
        // Arrange
        var product = new Product
        {
            Id = 999,
            Name = "Producto Actualizado",
            Description = "Descripción actualizada",
            Price = 150.00m
        };

        // Act
        var result = await _repository.UpdateAsync(product);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task UpdateAsync_WithMultipleProducts_ShouldUpdateAll()
    {
        // Arrange
        var category = await _context.Categories.FirstAsync();
        var status = await _context.Statuses.FirstAsync();
        var marketplace = await _context.Marketplaces.FirstAsync();
        var store = await _context.Stores.FirstAsync();

        var products = new List<Product>
        {
            new() { 
                Name = "Producto 1", 
                Description = "Descripción del producto 1", 
                Price = 100.00m, 
                CategoryId = category.Id, 
                StatusId = status.Id, 
                MarketplaceId = marketplace.Id, 
                StoreId = store.Id 
            },
            new() { 
                Name = "Producto 2", 
                Description = "Descripción del producto 2", 
                Price = 200.00m, 
                CategoryId = category.Id, 
                StatusId = status.Id, 
                MarketplaceId = marketplace.Id, 
                StoreId = store.Id 
            }
        };
        await _context.Products.AddRangeAsync(products);
        await _context.SaveChangesAsync();

        // Update products
        products[0].Name = "Producto 1 Actualizado";
        products[1].Name = "Producto 2 Actualizado";

        // Act
        var result = await _repository.UpdateAsync(products);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.WasSuccess);
        Assert.True(result.Result > 0);

        var updatedProducts = await _context.Products.OrderBy(p => p.Id).ToListAsync();
        Assert.Contains(updatedProducts, p => p.Name == "Producto 1 Actualizado");
        Assert.Contains(updatedProducts, p => p.Name == "Producto 2 Actualizado");
    }
} 