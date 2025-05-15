using Moq;
using ReelBuy.Backend.Repositories.Interfaces;
using ReelBuy.Backend.UnitsOfWork.Implementations;
using ReelBuy.Shared.Entities;
using Xunit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReelBuy.Tests.UnitsOfWork
{
    public class GenericUnitOfWorkTests
    {
        [Fact]
        public void CanInstantiateGenericUnitOfWork()
        {
            var repo = new Mock<IGenericRepository<City>>();
            var unitOfWork = new GenericUnitOfWork<City>(repo.Object);
            Assert.NotNull(unitOfWork);
        }

        [Fact]
        public async Task GetAsync_ShouldCallRepository()
        {
            var repo = new Mock<IGenericRepository<City>>();
            repo.Setup(r => r.GetAsync()).ReturnsAsync(new ReelBuy.Shared.Responses.ActionResponse<IEnumerable<City>>());
            var unitOfWork = new GenericUnitOfWork<City>(repo.Object);
            var result = await unitOfWork.GetAsync();
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetAsync_ById_ShouldCallRepository()
        {
            var repo = new Mock<IGenericRepository<City>>();
            repo.Setup(r => r.GetAsync(It.IsAny<int>())).ReturnsAsync(new ReelBuy.Shared.Responses.ActionResponse<City>());
            var unitOfWork = new GenericUnitOfWork<City>(repo.Object);
            var result = await unitOfWork.GetAsync(1);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task AddAsync_ShouldCallRepository()
        {
            var repo = new Mock<IGenericRepository<City>>();
            repo.Setup(r => r.AddAsync(It.IsAny<City>())).ReturnsAsync(new ReelBuy.Shared.Responses.ActionResponse<City>());
            var unitOfWork = new GenericUnitOfWork<City>(repo.Object);
            var result = await unitOfWork.AddAsync(new City());
            Assert.NotNull(result);
        }

        [Fact]
        public async Task UpdateAsync_ShouldCallRepository()
        {
            var repo = new Mock<IGenericRepository<City>>();
            repo.Setup(r => r.UpdateAsync(It.IsAny<City>())).ReturnsAsync(new ReelBuy.Shared.Responses.ActionResponse<City>());
            var unitOfWork = new GenericUnitOfWork<City>(repo.Object);
            var result = await unitOfWork.UpdateAsync(new City());
            Assert.NotNull(result);
        }

        [Fact]
        public async Task DeleteAsync_ShouldCallRepository()
        {
            var repo = new Mock<IGenericRepository<City>>();
            repo.Setup(r => r.DeleteAsync(It.IsAny<int>())).ReturnsAsync(new ReelBuy.Shared.Responses.ActionResponse<City>());
            var unitOfWork = new GenericUnitOfWork<City>(repo.Object);
            var result = await unitOfWork.DeleteAsync(1);
            Assert.NotNull(result);
        }
    }
} 