using Moq;
using ReelBuy.Backend.Repositories.Interfaces;
using ReelBuy.Backend.UnitsOfWork.Implementations;
using Xunit;

namespace ReelBuy.Tests.UnitsOfWork
{
    public class LikesUnitOfWorkTests
    {
        [Fact]
        public void CanInstantiateLikesUnitOfWork()
        {
            var repo = new Mock<ILikesRepository>();
            var unitOfWork = new LikesUnitOfWork(repo.Object);
            Assert.NotNull(unitOfWork);
        }
    }
} 