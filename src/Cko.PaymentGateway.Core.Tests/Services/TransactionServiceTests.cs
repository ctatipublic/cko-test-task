using Cko.Common.Infrastructure.DomainModel;
using Cko.Common.Infrastructure.Interfaces;
using Cko.PaymentGateway.Core.Services;
using Cko.PaymentGateway.Infrastructure.DomainModel;
using Cko.PaymentGateway.Infrastructure.Interfaces.Gateways;
using Cko.PaymentGateway.Infrastructure.Interfaces.Repository;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Cko.PaymentGateway.Core.Tests.Services
{
    public class TransactionServiceTests
    {
        private Mock<IBankApiGateway> _bankApiGatewayMock;
        private Mock<IStaticValuesProvider> _staticValuesProviderMock;
        private Mock<ITransactionRepository> _transactionRepositoryMock;

        [Fact]
        public async Task ProcessTransactionAsync_ReturnsExpectedValues_OnSuccess()
        {
            // Arrange
            SetupDefaultMocks();

            var bankTransactionResult = new BankTransactionResult
            {
                FromErrorReasons = new string[0],
                ToErrorReasons = new string[0],
                TransactionId = "transactionid"
            };

            _bankApiGatewayMock.Setup(m =>
                m.ProcessTransactionAsync(It.IsAny<Transaction>()))
            .ReturnsAsync(new PaymentGatewayBankTransactionResult
            {
                BankTransactionResult = bankTransactionResult,
                ConnectionError = null
            });

            // Act
            var service = GetService();
            var transaction = new Transaction
            {
                From = new CardDetails { CardNumber = "1111111111111111", Cvv = "111" },
                To = new CardDetails { CardNumber = "222222222222222222", Cvv = "222" }
            };
            var result = await service.ProcessTransactionAsync(transaction);

            // Assert
            Assert.Equal(transaction.GetHashCode(), result.OriginalTransaction.GetHashCode());
            Assert.Equal("bc335580-e38d-40ca-9c89-59c017bc7a24", result.TransactionId);
            Assert.Equal(DateTime.SpecifyKind(new DateTime(2021, 1, 1, 00, 01, 02), DateTimeKind.Utc), result.TransactionDateUtc);
            Assert.Equal(bankTransactionResult.TransactionId, result.BankTransactionResult.BankTransactionResult.TransactionId);
            Assert.Equal(bankTransactionResult.FromErrorReasons.Count(), result.BankTransactionResult.BankTransactionResult.FromErrorReasons.Count());
            Assert.Equal(bankTransactionResult.ToErrorReasons.Count(), result.BankTransactionResult.BankTransactionResult.ToErrorReasons.Count());
            Assert.Null(result.BankTransactionResult.ConnectionError);
            Assert.Equal(TransactionStatus.Success, result.TransactionStatus);
            Assert.Equal(TransactionStatus.Success.ToString(), result.TransactionStatusText);
            _transactionRepositoryMock.Verify(m => m.StoreTransactionAsync(result), Times.Once());

        }

        [Fact]
        public async Task ProcessTransactionAsync_ReturnsExpectedValues_OnBankDenied()
        {
            // Arrange
            SetupDefaultMocks();

            var bankTransactionResult = new BankTransactionResult
            {
                FromErrorReasons = new string[] { "fromerrorreason", "fromerrorreason2" },
                ToErrorReasons = new string[] { "toerrorreason", "toerrorreason2" },
                TransactionId = "transactionid"
            };

            _bankApiGatewayMock.Setup(m =>
                m.ProcessTransactionAsync(It.IsAny<Transaction>()))
            .ReturnsAsync(new PaymentGatewayBankTransactionResult
            {
                BankTransactionResult = bankTransactionResult,
                ConnectionError = null
            });

            // Act
            var service = GetService();
            var transaction = new Transaction
            {
                From = new CardDetails { CardNumber = "1111111111111111", Cvv = "111" },
                To = new CardDetails { CardNumber = "222222222222222222", Cvv = "222" }
            };
            var result = await service.ProcessTransactionAsync(transaction);

            // Assert
            Assert.Equal(transaction.GetHashCode(), result.OriginalTransaction.GetHashCode());
            Assert.Equal("bc335580-e38d-40ca-9c89-59c017bc7a24", result.TransactionId);
            Assert.Equal(DateTime.SpecifyKind(new DateTime(2021, 1, 1, 00, 01, 02), DateTimeKind.Utc), result.TransactionDateUtc);
            Assert.Equal(bankTransactionResult.TransactionId, result.BankTransactionResult.BankTransactionResult.TransactionId);
            Assert.Equal(bankTransactionResult.FromErrorReasons, result.BankTransactionResult.BankTransactionResult.FromErrorReasons);
            Assert.Equal(bankTransactionResult.ToErrorReasons, result.BankTransactionResult.BankTransactionResult.ToErrorReasons);
            Assert.Null(result.BankTransactionResult.ConnectionError);
            Assert.Equal(TransactionStatus.BankDenied, result.TransactionStatus);
            Assert.Equal(TransactionStatus.BankDenied.ToString(), result.TransactionStatusText);
            _transactionRepositoryMock.Verify(m => m.StoreTransactionAsync(result), Times.Once());
        }

        [Fact]
        public async Task ProcessTransactionAsync_ReturnsExpectedValues_OnConnectionFailed()
        {
            // Arrange
            SetupDefaultMocks();

            var bankTransactionResult = new BankTransactionResult();
            var connectionError = "fooconnectionerror";

            _bankApiGatewayMock.Setup(m =>
                m.ProcessTransactionAsync(It.IsAny<Transaction>()))
            .ReturnsAsync(new PaymentGatewayBankTransactionResult
            {
                ConnectionError = connectionError
            });

            // Act
            var service = GetService();
            var transaction = new Transaction
            {
                From = new CardDetails { CardNumber = "1111111111111111", Cvv = "111" },
                To = new CardDetails { CardNumber = "222222222222222222", Cvv = "222" }
            };
            var result = await service.ProcessTransactionAsync(transaction);

            // Assert
            Assert.Equal(transaction.GetHashCode(), result.OriginalTransaction.GetHashCode());
            Assert.Equal("bc335580-e38d-40ca-9c89-59c017bc7a24", result.TransactionId);
            Assert.Equal(DateTime.SpecifyKind(new DateTime(2021, 1, 1, 00, 01, 02), DateTimeKind.Utc), result.TransactionDateUtc);
            Assert.Null(result.BankTransactionResult.BankTransactionResult);
            Assert.Equal(connectionError, result.BankTransactionResult.ConnectionError);
            Assert.Equal(TransactionStatus.Failed, result.TransactionStatus);
            Assert.Equal(TransactionStatus.Failed.ToString(), result.TransactionStatusText);
            _transactionRepositoryMock.Verify(m => m.StoreTransactionAsync(result), Times.Once());
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task RetrieveTransactionAsync_ReturnsExpectedValues(bool successfullGet)
        {
            // Arrange
            SetupDefaultMocks();
            var transactionId = "transactionid";
            PaymentGatewayTransactionResult transactionResult = successfullGet ?
             new PaymentGatewayTransactionResult
             {
                 OriginalTransaction = new Transaction
                 {
                     From = new CardDetails { CardNumber = "1111222233334444", Cvv = "111" },
                     To = new CardDetails { CardNumber = "4444333322221111", Cvv = "2222" }
                 },
             } : null;

            _transactionRepositoryMock
                .Setup(m => m.GetTransactionAsync(transactionId))
                .ReturnsAsync(transactionResult);

            // Act
            var service = GetService();
            var result = await service.RetrieveTransactionAsync(transactionId);

            // Assert
            if (successfullGet)
            {
                Assert.Equal(transactionResult.GetHashCode(), result.GetHashCode());
                Assert.Equal("************4444", transactionResult.OriginalTransaction.From.CardNumber);
                Assert.Equal("***", transactionResult.OriginalTransaction.From.Cvv);
                Assert.Equal("************1111", transactionResult.OriginalTransaction.To.CardNumber);
                Assert.Equal("****", transactionResult.OriginalTransaction.To.Cvv);
            }
            else
            {
                Assert.Null(result);
            }
        }

        private void SetupDefaultMocks()
        {
            _bankApiGatewayMock = new Mock<IBankApiGateway>();

            _staticValuesProviderMock = new Mock<IStaticValuesProvider>();
            _staticValuesProviderMock.Setup(m => m.GetUtcNow()).Returns(DateTime.SpecifyKind(new DateTime(2021, 1, 1, 00, 01, 02), DateTimeKind.Utc));
            _staticValuesProviderMock.Setup(m => m.GetGuid()).Returns(Guid.Parse("bc335580-e38d-40ca-9c89-59c017bc7a24"));

            _transactionRepositoryMock = new Mock<ITransactionRepository>();
        }

        private TransactionService GetService()
        {
            return new TransactionService(_bankApiGatewayMock.Object, _staticValuesProviderMock.Object, _transactionRepositoryMock.Object);
        }
    }
}
