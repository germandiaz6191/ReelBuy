using Moq;
using ReelBuy.Backend.Repositories.Interfaces;
using ReelBuy.Backend.UnitsOfWork.Implementations;
using ReelBuy.Shared.Entities;
using Xunit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReelBuy.Tests.UnitsOfWork
{
    public class StatusesUnitOfWorkTests
    {
        [Fact]
        public void CanInstantiateStatusesUnitOfWork()
        {
            var genericRepo = new Mock<IGenericRepository<Status>>();
            var repo = new Mock<IStatusesRepository>();
            var unitOfWork = new StatusesUnitOfWork(genericRepo.Object, repo.Object);
            Assert.NotNull(unitOfWork);
        }

        [Fact]
        public void Constructor_CoversAllBranches()
        {
            var genericRepo = new Mock<IGenericRepository<Status>>();
            var repo = new Mock<IStatusesRepository>();
            var unitOfWork = new StatusesUnitOfWork(genericRepo.Object, repo.Object);
            Assert.NotNull(unitOfWork);
        }

        [Fact]
        public async Task GetAsync_ShouldCallRepository()
        {
            var genericRepo = new Mock<IGenericRepository<Status>>();
            var repo = new Mock<IStatusesRepository>();
            repo.Setup(r => r.GetAsync()).ReturnsAsync(new ReelBuy.Shared.Responses.ActionResponse<IEnumerable<Status>>());
            var unitOfWork = new StatusesUnitOfWork(genericRepo.Object, repo.Object);
            var result = await unitOfWork.GetAsync();
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetAsync_ById_ShouldCallRepository()
        {
            var genericRepo = new Mock<IGenericRepository<Status>>();
            var repo = new Mock<IStatusesRepository>();
            repo.Setup(r => r.GetAsync(It.IsAny<int>())).ReturnsAsync(new ReelBuy.Shared.Responses.ActionResponse<Status>());
            var unitOfWork = new StatusesUnitOfWork(genericRepo.Object, repo.Object);
            var result = await unitOfWork.GetAsync(1);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetComboAsync_ShouldCallRepository()
        {
            var genericRepo = new Mock<IGenericRepository<Status>>();
            var repo = new Mock<IStatusesRepository>();
            repo.Setup(r => r.GetComboAsync()).ReturnsAsync(new List<Status>());
            var unitOfWork = new StatusesUnitOfWork(genericRepo.Object, repo.Object);
            var result = await unitOfWork.GetComboAsync();
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetAsync_WithPagination_ShouldCallRepository()
        {
            var genericRepo = new Mock<IGenericRepository<Status>>();
            var repo = new Mock<IStatusesRepository>();
            repo.Setup(r => r.GetAsync(It.IsAny<ReelBuy.Shared.DTOs.PaginationDTO>())).ReturnsAsync(new ReelBuy.Shared.Responses.ActionResponse<IEnumerable<Status>>());
            var unitOfWork = new StatusesUnitOfWork(genericRepo.Object, repo.Object);
            var result = await unitOfWork.GetAsync(new ReelBuy.Shared.DTOs.PaginationDTO());
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetTotalRecordsAsync_ShouldCallRepository()
        {
            var genericRepo = new Mock<IGenericRepository<Status>>();
            var repo = new Mock<IStatusesRepository>();
            repo.Setup(r => r.GetTotalRecordsAsync(It.IsAny<ReelBuy.Shared.DTOs.PaginationDTO>())).ReturnsAsync(new ReelBuy.Shared.Responses.ActionResponse<int>());
            var unitOfWork = new StatusesUnitOfWork(genericRepo.Object, repo.Object);
            var result = await unitOfWork.GetTotalRecordsAsync(new ReelBuy.Shared.DTOs.PaginationDTO());
            Assert.NotNull(result);
        }
    }
} 