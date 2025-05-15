using Moq;
using ReelBuy.Backend.Repositories.Interfaces;
using ReelBuy.Backend.UnitsOfWork.Implementations;
using ReelBuy.Shared.Entities;
using Xunit;

namespace ReelBuy.Tests.UnitsOfWork
{
    public class FavoritesUnitOfWorkTests
    {
        [Fact]
        public void CanInstantiateFavoritesUnitOfWork()
        {
            var genericRepo = new Mock<IGenericRepository<Favorite>>();
            var repo = new Mock<IFavoritesRepository>();
            var unitOfWork = new FavoritesUnitOfWork(genericRepo.Object, repo.Object);
            Assert.NotNull(unitOfWork);
        }
    }
} 