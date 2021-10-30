using Cko.BankSimulator.Controllers;
using Cko.BankSimulator.Infrastructure.Interfaces.Services;
using Cko.Common.Infrastructure.DomainModel;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Cko.BankSimulator.Tests
{
    public class TransactionControllerTests
    {
        private Mock<ITransactionService> _transactionServiceMock;

        [Fact]
        public async Task PostTransactionAsync_CallsServiceAndReturnsCorrectResposeWhenNoErrors()
        {
            // Arrange
            SetupDefaultMocks();
            _transactionServiceMock
                .Setup(m => m.ProcessTransactionAsync(It.IsAny<Transaction>()))
                .ReturnsAsync(new BankTransactionResult
                {
                    TransactionId = "transactionid",
                    FromErrorReasons = new string[0],
                    ToErrorReasons = new string[0]
                });

            // Act
            var controller = GetController();
            var response = await controller.PostTransactionAsync(new Transaction()) as ObjectResult;
            var responseValue = response.Value as BankTransactionResult;

            // Assert
            Assert.Equal(202, response.StatusCode);
            Assert.Equal("transactionid", responseValue.TransactionId);
            Assert.Equal(new string[0], responseValue.FromErrorReasons);
            Assert.Equal(new string[0], responseValue.ToErrorReasons);
        }

        public static IEnumerable<object[]> responseData => new List<object[]>
        {
            new object[] { new string[] { "error1" }, new string[0] },
            new object[] { new string[0], new string[] {"error2" } },
            new object[] { new string[] { "error1" }, new string[] {"error2" } },
        };

        [Theory]
        [MemberData(nameof(responseData))]
        public async Task PostTransactionAsync_CallsServiceAndReturnsCorrectResposeWhenErrors(IEnumerable<string> errorsFrom, IEnumerable<string> errorsTo)
        {
            // Arrange
            SetupDefaultMocks();
            _transactionServiceMock
                .Setup(m => m.ProcessTransactionAsync(It.IsAny<Transaction>()))
                .ReturnsAsync(new BankTransactionResult
                {
                    TransactionId = "transactionid",
                    FromErrorReasons = errorsFrom,
                    ToErrorReasons = errorsTo
                });

            // Act
            var controller = GetController();
            var response = await controller.PostTransactionAsync(new Transaction()) as ObjectResult;
            var responseValue = response.Value as BankTransactionResult;

            // Assert
            Assert.Equal(412, response.StatusCode);
            Assert.Equal("transactionid", responseValue.TransactionId);
            Assert.Equal(errorsFrom, responseValue.FromErrorReasons);
            Assert.Equal(errorsTo, responseValue.ToErrorReasons);
        }



        private void SetupDefaultMocks()
        {
            _transactionServiceMock = new Mock<ITransactionService>();
        }

        private TransactionController GetController()
        {
            var controller = new TransactionController(_transactionServiceMock.Object);
            return controller;
        }
    }
}
