using Cko.BankSimulator.Core.Services;
using Cko.Common.Infrastructure.DomainModel;
using Cko.Common.Infrastructure.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cko.BankSimulator.Core.Tests.Services
{
    public class TransactionServiceTests
    {
        private Mock<IStaticValuesProvider> _staticValuesProvider;
        private List<Mock<IValidator<CardDetails>>> _cardDetailsValidators = new List<Mock<IValidator<CardDetails>>>();
        private string _guid = "b70628ee-7c31-44b9-b7c2-22175150df25";

        [Fact]
        public async Task ProcessTransactionAsync_ProcessesTransactionSuccefully()
        {
            // Arrange
            SetupDefaultMocks();

            // Act
            var service = GetService();
            var result = await service.ProcessTransactionAsync(new Transaction { });

            // Assert
            Assert.Equal(_guid, result.TransactionId);
            Assert.Empty(result.FromErrorReasons);
            Assert.Empty(result.ToErrorReasons);
        }

        [Fact]
        public async Task ProcessTransactionAsync_DoesNotProcessesNextValidatorIfErrors()
        {
            // Arrange
            SetupDefaultMocks();

            _cardDetailsValidators.ElementAt(0).Setup(m => m.ValidateAsync(
                It.IsAny<CardDetails>())
            )
            .ReturnsAsync(new string[] { "ErrorFromValidator1" });

            _cardDetailsValidators.ElementAt(1).Setup(m => m.ValidateAsync(
                It.IsAny<CardDetails>())
            )
            .ReturnsAsync(new string[] { "ErrorFromValidator2" });

            // Act
            var service = GetService();
            var result = await service.ProcessTransactionAsync(new Transaction
            {
                From = new CardDetails { },
                To = new CardDetails { }
            });

            // Assert
            Assert.Equal(_guid, result.TransactionId);
            Assert.Equal("ErrorFromValidator1", result.FromErrorReasons.Single());
            Assert.Equal("ErrorFromValidator1", result.ToErrorReasons.Single());
        }

        [Fact]
        public async Task ProcessTransactionAsync_ProcessesUntilValidatorReturnsErrors()
        {
            // Arrange
            SetupDefaultMocks();

            _cardDetailsValidators.ElementAt(0).Setup(m => m.ValidateAsync(
                It.IsAny<CardDetails>())
            )
            .ReturnsAsync(new string[0]);

            _cardDetailsValidators.ElementAt(1).Setup(m => m.ValidateAsync(
                It.IsAny<CardDetails>())
            )
            .ReturnsAsync(new string[] { "ErrorFromValidator2" });

            // Act
            var service = GetService();
            var result = await service.ProcessTransactionAsync(new Transaction
            {
                From = new CardDetails { },
                To = new CardDetails { }
            });

            // Assert
            Assert.Equal(_guid, result.TransactionId);
            Assert.Equal("ErrorFromValidator2", result.FromErrorReasons.Single());
            Assert.Equal("ErrorFromValidator2", result.ToErrorReasons.Single());
        }

        [Fact]
        public async Task ProcessTransactionAsync_ProcessesBothFromAndToWithSameValidator()
        {
            // Arrange
            SetupDefaultMocks();

            _cardDetailsValidators.ElementAt(0).Setup(m => m.ValidateAsync(
                It.Is((CardDetails d) => d.CardNumber == "1"))
            )
            .ReturnsAsync(new string[] { "Error1" });

            _cardDetailsValidators.ElementAt(0).Setup(m => m.ValidateAsync(
                It.Is((CardDetails d) => d.CardNumber == "2"))
            )
            .ReturnsAsync(new string[] { "Error2" });

            var cardDetailsFrom = new CardDetails { CardNumber = "1" };
            var cardDetailsTo = new CardDetails { CardNumber = "2" };

            // Act
            var service = GetService();
            var result = await service.ProcessTransactionAsync(new Transaction
            {
                From = cardDetailsFrom,
                To = cardDetailsTo
            });

            // Assert
            Assert.Equal(_guid, result.TransactionId);
            Assert.Equal("Error1", result.FromErrorReasons.Single());
            Assert.Equal("Error2", result.ToErrorReasons.Single());
        }



        private void SetupDefaultMocks()
        {
            _staticValuesProvider = new Mock<IStaticValuesProvider>();
            _staticValuesProvider.Setup(m => m.GetGuid()).Returns(Guid.Parse(_guid));

            var cardDetailsValidator1 = new Mock<IValidator<CardDetails>>();
            cardDetailsValidator1.Setup(m => m.ValidateAsync(It.IsAny<CardDetails>())).ReturnsAsync(new string[0]);

            var cardDetailsValidator2 = new Mock<IValidator<CardDetails>>();
            cardDetailsValidator2.Setup(m => m.ValidateAsync(It.IsAny<CardDetails>())).ReturnsAsync(new string[0]);

            _cardDetailsValidators.Add(cardDetailsValidator1);
            _cardDetailsValidators.Add(cardDetailsValidator2);
        }

        private TransactionService GetService()
        {
            return new TransactionService(_cardDetailsValidators.Select(m => m.Object), _staticValuesProvider.Object);
        }
    }
}
