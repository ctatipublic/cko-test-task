using Cko.PaymentGateway.Core.Repositories;
using Cko.PaymentGateway.Infrastructure.DomainModel;
using Cko.PaymentGateway.Infrastructure.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Cko.PaymentGateway.Core.Tests.Repositories
{
    public class TransactionRepositoryTests
    {
        private Mock<IDocumentPersistance<PaymentGatewayTransactionResult>> _persistanceMock;

        [Fact]
        public async Task GetTransactionAsync_RetrievesTransaction()
        {
            // Arrange
            var transactionId = "transactionId";
            var retrievedTransaction = new PaymentGatewayTransactionResult();
            SetupDefaultMocks();
            _persistanceMock.Setup(m =>
                m.RetrieveDocumentByKeyAsync("transactionId", transactionId))
            .ReturnsAsync(retrievedTransaction);

            // Act
            var repository = GetRepository();
            var result = await repository.GetTransactionAsync(transactionId);

            // Assert
            _persistanceMock.Verify(m => m.RetrieveDocumentByKeyAsync("transactionId", transactionId), Times.Once);
            Assert.Equal(retrievedTransaction.GetHashCode(), result.GetHashCode());
        }

        [Fact]
        public async Task StoreTransactionAsync_StoresTransaction()
        {
            // Arrange
            var transactionToStore = new PaymentGatewayTransactionResult();
            SetupDefaultMocks();

            // Act
            var repository = GetRepository();
            await repository.StoreTransactionAsync(transactionToStore);

            // Assert
            _persistanceMock.Verify(m => m.StoreDocumentAsync(transactionToStore), Times.Once);
        }

        private void SetupDefaultMocks()
        {
            _persistanceMock = new Mock<IDocumentPersistance<PaymentGatewayTransactionResult>>();
        }

        private TransactionRepository GetRepository()
        {
            return new TransactionRepository(_persistanceMock.Object);
        }
    }
}
