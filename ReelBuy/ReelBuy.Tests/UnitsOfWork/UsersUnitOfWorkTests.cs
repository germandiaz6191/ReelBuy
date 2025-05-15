using Moq;
using ReelBuy.Backend.Repositories.Interfaces;
using ReelBuy.Backend.UnitsOfWork.Implementations;
using Xunit;

namespace ReelBuy.Tests.UnitsOfWork
{
    public class UsersUnitOfWorkTests
    {
        [Fact]
        public void CanInstantiateUsersUnitOfWork()
        {
            var repo = new Mock<IUsersRepository>();
            var unitOfWork = new UsersUnitOfWork(repo.Object);
            Assert.NotNull(unitOfWork);
        }
    }
} 