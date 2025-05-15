using Moq;
using ReelBuy.Backend.Repositories.Interfaces;
using ReelBuy.Backend.UnitsOfWork.Implementations;
using ReelBuy.Shared.Entities;
using Xunit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReelBuy.Tests.UnitsOfWork
{
    public class CommentsUnitOfWorkTests
    {
        [Fact]
        public void CanInstantiateCommentsUnitOfWork()
        {
            var genericRepo = new Mock<IGenericRepository<Comments>>();
            var repo = new Mock<ICommentsRepository>();
            var unitOfWork = new CommentsUnitOfWork(genericRepo.Object, repo.Object);
            Assert.NotNull(unitOfWork);
        }

        [Fact]
        public void Constructor_CoversAllBranches()
        {
            var genericRepo = new Mock<IGenericRepository<Comments>>();
            var repo = new Mock<ICommentsRepository>();
            var unitOfWork = new CommentsUnitOfWork(genericRepo.Object, repo.Object);
            Assert.NotNull(unitOfWork);
        }

        [Fact]
        public async Task GetAsync_ShouldCallRepository()
        {
            var genericRepo = new Mock<IGenericRepository<Comments>>();
            var repo = new Mock<ICommentsRepository>();
            repo.Setup(r => r.GetAsync()).ReturnsAsync(new ReelBuy.Shared.Responses.ActionResponse<IEnumerable<Comments>>());
            var unitOfWork = new CommentsUnitOfWork(genericRepo.Object, repo.Object);
            var result = await unitOfWork.GetAsync();
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetAsync_ById_ShouldCallRepository()
        {
            var genericRepo = new Mock<IGenericRepository<Comments>>();
            var repo = new Mock<ICommentsRepository>();
            repo.Setup(r => r.GetAsync(It.IsAny<int>())).ReturnsAsync(new ReelBuy.Shared.Responses.ActionResponse<Comments>());
            var unitOfWork = new CommentsUnitOfWork(genericRepo.Object, repo.Object);
            var result = await unitOfWork.GetAsync(1);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetAsync_WithPagination_ShouldCallRepository()
        {
            var genericRepo = new Mock<IGenericRepository<Comments>>();
            var repo = new Mock<ICommentsRepository>();
            repo.Setup(r => r.GetAsync(It.IsAny<ReelBuy.Shared.DTOs.PaginationDTO>())).ReturnsAsync(new ReelBuy.Shared.Responses.ActionResponse<IEnumerable<Comments>>());
            var unitOfWork = new CommentsUnitOfWork(genericRepo.Object, repo.Object);
            var result = await unitOfWork.GetAsync(new ReelBuy.Shared.DTOs.PaginationDTO());
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetTotalRecordsAsync_ShouldCallRepository()
        {
            var genericRepo = new Mock<IGenericRepository<Comments>>();
            var repo = new Mock<ICommentsRepository>();
            repo.Setup(r => r.GetTotalRecordsAsync(It.IsAny<ReelBuy.Shared.DTOs.PaginationDTO>())).ReturnsAsync(new ReelBuy.Shared.Responses.ActionResponse<int>());
            var unitOfWork = new CommentsUnitOfWork(genericRepo.Object, repo.Object);
            var result = await unitOfWork.GetTotalRecordsAsync(new ReelBuy.Shared.DTOs.PaginationDTO());
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetCommentsByProductAsync_ShouldCallRepository()
        {
            var genericRepo = new Mock<IGenericRepository<Comments>>();
            var repo = new Mock<ICommentsRepository>();
            repo.Setup(r => r.GetCommentsByProductAsync(It.IsAny<ReelBuy.Shared.DTOs.PaginationDTO>())).ReturnsAsync(new ReelBuy.Shared.Responses.ActionResponse<IEnumerable<Comments>>());
            var unitOfWork = new CommentsUnitOfWork(genericRepo.Object, repo.Object);
            var result = await unitOfWork.GetCommentsByProductAsync(new ReelBuy.Shared.DTOs.PaginationDTO());
            Assert.NotNull(result);
        }

        [Fact]
        public async Task AddAsync_ShouldCallRepository()
        {
            var genericRepo = new Mock<IGenericRepository<Comments>>();
            var repo = new Mock<ICommentsRepository>();
            repo.Setup(r => r.AddAsync(It.IsAny<ReelBuy.Shared.DTOs.CommetDTO>())).ReturnsAsync(new ReelBuy.Shared.Responses.ActionResponse<Comments>());
            var unitOfWork = new CommentsUnitOfWork(genericRepo.Object, repo.Object);
            var result = await unitOfWork.AddAsync(new ReelBuy.Shared.DTOs.CommetDTO());
            Assert.NotNull(result);
        }
    }
} 