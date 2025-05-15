using Moq;
using ReelBuy.Backend.Repositories.Interfaces;
using ReelBuy.Backend.UnitsOfWork.Implementations;
using ReelBuy.Shared.Entities;
using Xunit;

namespace ReelBuy.Tests.UnitsOfWork
{
    public class ReelsUnitOfWorkTests
    {
        [Fact]
        public void CanInstantiateReelsUnitOfWork()
        {
            var genericRepo = new Mock<IGenericRepository<Reel>>();
            var repo = new Mock<IReelsRepository>();
            var unitOfWork = new ReelsUnitOfWork(genericRepo.Object, repo.Object);
            Assert.NotNull(unitOfWork);
        }
    }
} 