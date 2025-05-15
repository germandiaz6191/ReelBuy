using Microsoft.EntityFrameworkCore;
using ReelBuy.Backend.Data;
using ReelBuy.Backend.Repositories.Implementations;
using ReelBuy.Shared.DTOs;
using ReelBuy.Shared.Entities;


namespace ReelBuy.Tests.Repositories;

public class CommentsRepositoryTests : IDisposable
{
    private readonly DataContext _context;
    private readonly CommentsRepository _repository;
    private readonly int _testCityId;

    public CommentsRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new DataContext(options);
        _repository = new CommentsRepository(_context);

        // Crear City de prueba y guardar
        var city = new City { Name = "Test City", DepartmentId = 1 };
        _context.Cities.Add(city);
        _context.SaveChanges();
        _testCityId = city.Id;
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task GetAsync_ShouldReturnAllComments()
    {
        // Arrange
        var category = new Category { Name = "Categoría 1" };
        var status = new Status { Name = "Estado 1" };
        var marketplace = new Marketplace { Name = "Marketplace 1", Domain = "marketplace1.com" };
        var store = new Store { Name = "Tienda 1", UserId = "user1", CityId = _testCityId };

        await _context.Categories.AddAsync(category);
        await _context.Statuses.AddAsync(status);
        await _context.Marketplaces.AddAsync(marketplace);
        await _context.Stores.AddAsync(store);
        await _context.SaveChangesAsync();

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

        var comments = new List<Comments>
        {
            new() { Description = "Comentario 1", UserId = "user1", ProductId = product.Id, RegistrationDate = DateTime.UtcNow },
            new() { Description = "Comentario 2", UserId = "user2", ProductId = product.Id, RegistrationDate = DateTime.UtcNow },
            new() { Description = "Comentario 3", UserId = "user1", ProductId = product.Id, RegistrationDate = DateTime.UtcNow }
        };
        await _context.Comments.AddRangeAsync(comments);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAsync();

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetAsync_WithId_ShouldReturnComment()
    {
        // Arrange
        var category = new Category { Name = "Categoría 1" };
        var status = new Status { Name = "Estado 1" };
        var marketplace = new Marketplace { Name = "Marketplace 1", Domain = "marketplace1.com" };
        var store = new Store { Name = "Tienda 1", UserId = "user1", CityId = _testCityId };

        await _context.Categories.AddAsync(category);
        await _context.Statuses.AddAsync(status);
        await _context.Marketplaces.AddAsync(marketplace);
        await _context.Stores.AddAsync(store);
        await _context.SaveChangesAsync();

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

        var comment = new Comments
        {
            Description = "Comentario 1",
            UserId = "user1",
            ProductId = product.Id,
            RegistrationDate = DateTime.UtcNow
        };
        await _context.Comments.AddAsync(comment);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAsync(comment.Id);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetAsync_WithInvalidId_ShouldReturnError()
    {
        // Act
        var result = await _repository.GetAsync(999);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.WasSuccess);
        Assert.Null(result.Result);
        Assert.Equal("ERR001", result.Message);
    }

    [Fact]
    public async Task GetCommentsByProductAsync_ShouldReturnProductComments()
    {
        // Arrange
        var category = new Category { Name = "Categoría 1" };
        var status = new Status { Name = "Estado 1" };
        var marketplace = new Marketplace { Name = "Marketplace 1", Domain = "marketplace1.com" };
        var store = new Store { Name = "Tienda 1", UserId = "user1", CityId = _testCityId };

        await _context.Categories.AddAsync(category);
        await _context.Statuses.AddAsync(status);
        await _context.Marketplaces.AddAsync(marketplace);
        await _context.Stores.AddAsync(store);
        await _context.SaveChangesAsync();

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

        var comments = new List<Comments>
        {
            new() { Description = "Comentario 1", UserId = "user1", ProductId = product.Id, RegistrationDate = DateTime.UtcNow },
            new() { Description = "Comentario 2", UserId = "user2", ProductId = product.Id, RegistrationDate = DateTime.UtcNow }
        };
        await _context.Comments.AddRangeAsync(comments);
        await _context.SaveChangesAsync();

        var pagination = new PaginationDTO { Filter = product.Id.ToString() };

        // Act
        var result = await _repository.GetCommentsByProductAsync(pagination);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetAsync_WithPagination_ShouldReturnPaginatedComments()
    {
        // Arrange
        var category = new Category { Name = "Categoría 1" };
        var status = new Status { Name = "Estado 1" };
        var marketplace = new Marketplace { Name = "Marketplace 1", Domain = "marketplace1.com" };
        var store = new Store { Name = "Tienda 1", UserId = "user1", CityId = _testCityId };

        await _context.Categories.AddAsync(category);
        await _context.Statuses.AddAsync(status);
        await _context.Marketplaces.AddAsync(marketplace);
        await _context.Stores.AddAsync(store);
        await _context.SaveChangesAsync();

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

        var comments = new List<Comments>
        {
            new() { Description = "Comentario 1", UserId = "user1", ProductId = product.Id, RegistrationDate = DateTime.UtcNow },
            new() { Description = "Comentario 2", UserId = "user2", ProductId = product.Id, RegistrationDate = DateTime.UtcNow },
            new() { Description = "Comentario 3", UserId = "user1", ProductId = product.Id, RegistrationDate = DateTime.UtcNow },
            new() { Description = "Comentario 4", UserId = "user2", ProductId = product.Id, RegistrationDate = DateTime.UtcNow },
            new() { Description = "Comentario 5", UserId = "user1", ProductId = product.Id, RegistrationDate = DateTime.UtcNow }
        };
        await _context.Comments.AddRangeAsync(comments);
        await _context.SaveChangesAsync();

        var pagination = new PaginationDTO { Page = 1, RecordsNumber = 2 };

        // Act
        var result = await _repository.GetAsync(pagination);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetTotalRecordsAsync_ShouldReturnTotalCount()
    {
        // Arrange
        var category = new Category { Name = "Categoría 1" };
        var status = new Status { Name = "Estado 1" };
        var marketplace = new Marketplace { Name = "Marketplace 1", Domain = "marketplace1.com" };
        var store = new Store { Name = "Tienda 1", UserId = "user1", CityId = _testCityId };

        await _context.Categories.AddAsync(category);
        await _context.Statuses.AddAsync(status);
        await _context.Marketplaces.AddAsync(marketplace);
        await _context.Stores.AddAsync(store);
        await _context.SaveChangesAsync();

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

        var comments = new List<Comments>
        {
            new() { Description = "Comentario 1", UserId = "user1", ProductId = product.Id, RegistrationDate = DateTime.UtcNow },
            new() { Description = "Comentario 2", UserId = "user2", ProductId = product.Id, RegistrationDate = DateTime.UtcNow }
        };
        await _context.Comments.AddRangeAsync(comments);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetTotalRecordsAsync(new PaginationDTO());

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task AddAsync_WithValidData_ShouldAddComment()
    {
        // Arrange
        var category = new Category { Name = "Categoría 1" };
        var status = new Status { Name = "Estado 1" };
        var marketplace = new Marketplace { Name = "Marketplace 1", Domain = "marketplace1.com" };
        var store = new Store { Name = "Tienda 1", UserId = "user1", CityId = _testCityId };

        await _context.Categories.AddAsync(category);
        await _context.Statuses.AddAsync(status);
        await _context.Marketplaces.AddAsync(marketplace);
        await _context.Stores.AddAsync(store);
        await _context.SaveChangesAsync();

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

        var user = new User
        {
            FirstName = "Usuario 1",
            LastName = "Apellido 1",
            Email = "usuario1@test.com",
            UserName = "usuario1",
            PhoneNumber = "1234567890"
        };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var commentDTO = new CommetDTO
        {
            Description = "Nuevo Comentario",
            UserId = user.Id,
            ProductId = product.Id,
            RegistrationDate = DateTime.UtcNow
        };

        // Act
        var result = await _repository.AddAsync(commentDTO);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task AddAsync_WithInvalidProductId_ShouldReturnError()
    {
        // Arrange
        var commentDTO = new CommetDTO
        {
            Description = "Nuevo Comentario",
            UserId = "user1",
            ProductId = 999,
            RegistrationDate = DateTime.UtcNow
        };

        // Act
        var result = await _repository.AddAsync(commentDTO);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.WasSuccess);
        Assert.Null(result.Result);
        Assert.Equal("ERR009", result.Message);
    }

    [Fact]
    public async Task AddAsync_WithInvalidUserId_ShouldReturnError()
    {
        // Arrange
        var category = new Category { Name = "Categoría 1" };
        var status = new Status { Name = "Estado 1" };
        var marketplace = new Marketplace { Name = "Marketplace 1", Domain = "marketplace1.com" };
        var store = new Store { Name = "Tienda 1", UserId = "user1", CityId = _testCityId };

        await _context.Categories.AddAsync(category);
        await _context.Statuses.AddAsync(status);
        await _context.Marketplaces.AddAsync(marketplace);
        await _context.Stores.AddAsync(store);
        await _context.SaveChangesAsync();

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

        var commentDTO = new CommetDTO
        {
            Description = "Nuevo Comentario",
            UserId = "invalid-user",
            ProductId = product.Id,
            RegistrationDate = DateTime.UtcNow
        };

        // Act
        var result = await _repository.AddAsync(commentDTO);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.WasSuccess);
        Assert.Null(result.Result);
        Assert.Equal("ERR009", result.Message);
    }
} 