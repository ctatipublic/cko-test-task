using Cko.Common.Infrastructure.DomainModel;
using Cko.PaymentGateway.Controllers;
using Cko.PaymentGateway.Infrastructure.DomainModel;
using Cko.PaymentGateway.Infrastructure.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Cko.PaymentGateway.Tests.Controllers
{
    public class TransactionControllerTests
    {
        private Mock<ITransactionService> _transactionServiceMock;

        [Fact]
        public async Task PostTransactionAsync_CallsServiceAndReturnsCorrectResponse()
        {
            // Arrange
            SetupDefaultMocks();
            var transactionResult = new PaymentGatewayTransactionResult
            {
                TransactionId = "transactionid",
                BankTransactionResult = new PaymentGatewayBankTransactionResult
                {
                    BankTransactionResult = new BankTransactionResult
                    {
                        FromErrorReasons = new string[0],
                        ToErrorReasons = new string[0],
                    },
                    ConnectionError = null
                }
            };
            _transactionServiceMock.Setup(m => m.ProcessTransactionAsync(It.IsAny<Transaction>())).ReturnsAsync(transactionResult);

            // Act
            var controller = GetController();
            var response = await controller.PostTransactionAsync(new Transaction()) as ObjectResult;
            var responseValue = response.Value as PaymentGatewayTransactionResult;

            // Assert
            Assert.Equal(202, response.StatusCode);
            Assert.Equal(transactionResult.GetHashCode(), responseValue.GetHashCode());
        }

        public static IEnumerable<object[]> responseData => new List<object[]>
        {
            new object[] { new string[] { "error1" }, new string[0], null },
            new object[] { new string[0], new string[] {"error2" }, null },
            new object[] { new string[] { "error1" }, new string[] {"error2" }, null },
            new object[] { new string[0], new string[0], "a connection error" },
        };

        [Theory]
        [MemberData(nameof(responseData))]
        public async Task PostTransactionAsync_CallsServiceAndReturnsCorrectResposeWhenErrors(IEnumerable<string> errorsFrom, IEnumerable<string> errorsTo, string connectionError)
        {
            // Arrange
            SetupDefaultMocks();
            var transactionResult = new PaymentGatewayTransactionResult
            {
                TransactionId = "transactionid",
                BankTransactionResult = new PaymentGatewayBankTransactionResult
                {
                    BankTransactionResult = new BankTransactionResult
                    {
                        FromErrorReasons = errorsFrom,
                        ToErrorReasons = errorsTo,
                    },
                    ConnectionError = connectionError
                }
            };
            _transactionServiceMock.Setup(m => m.ProcessTransactionAsync(It.IsAny<Transaction>())).ReturnsAsync(transactionResult);

            // Act
            var controller = GetController();
            var response = await controller.PostTransactionAsync(new Transaction()) as ObjectResult;
            var responseValue = response.Value as PaymentGatewayTransactionResult;

            // Assert
            Assert.Equal(412, response.StatusCode);
            Assert.Equal(transactionResult.GetHashCode(), responseValue.GetHashCode());
        }

        [Fact]
        public async Task GetTransactionAsync_CallsServiceAndReturnsCorrectResponse_WhenFound()
        {
            // Arrange
            SetupDefaultMocks();
            var transactionResult = new PaymentGatewayTransactionResult
            {
                TransactionId = "transactionid",
                BankTransactionResult = new PaymentGatewayBankTransactionResult
                {
                    BankTransactionResult = new BankTransactionResult
                    {
                        FromErrorReasons = new string[0],
                        ToErrorReasons = new string[0],
                    },
                    ConnectionError = null
                }
            };
            _transactionServiceMock.Setup(m => m.RetrieveTransactionAsync(It.IsAny<string>())).ReturnsAsync(transactionResult);

            // Act
            var controller = GetController();
            var response = await controller.GetTransactionAsync("") as ObjectResult;
            var responseValue = response.Value as PaymentGatewayTransactionResult;

            // Assert
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(transactionResult.GetHashCode(), responseValue.GetHashCode());
        }

        [Fact]
        public async Task GetTransactionAsync_CallsServiceAndReturnsCorrectResponse_WhenNotFound()
        {
            // Arrange
            SetupDefaultMocks();
            _transactionServiceMock.Setup(m => m.RetrieveTransactionAsync(It.IsAny<string>())).ReturnsAsync(value: null);

            // Act
            var controller = GetController();
            var response = await controller.GetTransactionAsync("") as ObjectResult;
            var responseValue = response.Value as PaymentGatewayTransactionResult;

            // Assert
            Assert.Equal(404, response.StatusCode);
            Assert.Null(responseValue);
        }


        private void SetupDefaultMocks()
        {
            _transactionServiceMock = new Mock<ITransactionService>();
        }

        private TransactionController GetController()
        {
            return new TransactionController(_transactionServiceMock.Object);
        }
    }
}
