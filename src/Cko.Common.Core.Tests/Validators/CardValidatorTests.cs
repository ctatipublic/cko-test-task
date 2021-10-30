using Cko.Common.Core.Validators;
using Cko.Common.Infrastructure.DomainModel;
using Cko.Common.Infrastructure.Interfaces;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cko.Common.Core.Tests.Validators
{
    public class CardValidatorTests
    {
        private Mock<IStaticValuesProvider> _staticValuesProviderMock;

        [Fact]
        public async Task ValidateAsync_ReturnsError_When_CardDetails_AreNull()
        {
            // Arrange
            SetupDefaultMocks();
            CardDetails cardDetails = null;

            // Act
            var validator = GetCreditCardValidator();
            var validationResult = await validator.ValidateAsync(cardDetails);

            // Assert
            Assert.Single(validationResult);
            Assert.Equal(CreditCardValidator.ErrorCodes.InvalidCardDetails, validationResult.Single());
        }

        [Theory]
        [InlineData("5200828282828210", true)]
        [InlineData("5200-8282-8282-8210", true)]
        [InlineData("5200-8282-8282-82a0", false)]
        [InlineData("5200-8282-8282", false)]
        [InlineData("0000-0000-0000-0001", false)]
        [InlineData("0000000000000001", false)]
        public async Task ValidateAsync_Validates_CardNumber(string cardNumber, bool isValid)
        {
            // Arrage
            SetupDefaultMocks();
            var cardDetails = new CardDetails { CardNumber = cardNumber };

            // Act
            var validator = GetCreditCardValidator();
            var validationResult = await validator.ValidateAsync(cardDetails);

            // Assert
            if (isValid)
            {
                Assert.True(!validationResult.Any(r => r.StartsWith(CreditCardValidator.ErrorCodes.CardNumber)));
            }
            else
            {
                Assert.Single(validationResult.Where(r => r.StartsWith(CreditCardValidator.ErrorCodes.CardNumber)));
            }

        }

        [Theory]
        [InlineData("", false)]
        [InlineData(null, false)]
        [InlineData("Foo", false)]
        [InlineData("foob", false)]
        [InlineData("foobar", false)]
        [InlineData("12", false)]
        [InlineData("123", true)]
        [InlineData("1234", true)]
        [InlineData("12345", false)]
        public async Task ValidateAsync_Validates_Cvv(string cvv, bool isValid)
        {
            // Arrage
            SetupDefaultMocks();
            var cardDetails = new CardDetails { Cvv = cvv };

            // Act
            var validator = GetCreditCardValidator();
            var validationResult = await validator.ValidateAsync(cardDetails);

            // Assert
            if (isValid)
            {
                Assert.True(!validationResult.Any(r => r.Equals(CreditCardValidator.ErrorCodes.Cvv)));
            }
            else
            {
                Assert.Single(validationResult.Where(r => r.Equals(CreditCardValidator.ErrorCodes.Cvv)));
            }
        }

        [Theory]
        [InlineData("01/21", true)]
        [InlineData("12/20", true)]
        [InlineData("02/21", false)]
        [InlineData("foo", false)]
        public async Task ValidateAsync_Validates_ExpiryDate(string expiryDate, bool isValid)
        {
            // Arrage
            SetupDefaultMocks();
            var cardDetails = new CardDetails { ExpiryDate = expiryDate };

            // Act
            var validator = GetCreditCardValidator();
            var validationResult = await validator.ValidateAsync(cardDetails);

            // Assert
            if (isValid)
            {
                Assert.True(!validationResult.Any(r => r.Equals(CreditCardValidator.ErrorCodes.ExpiryDate)));
            }
            else
            {
                Assert.Single(validationResult.Where(r => r.Equals(CreditCardValidator.ErrorCodes.ExpiryDate)));
            }
        }

        [Theory]
        [InlineData("", false)]
        [InlineData(null, false)]
        [InlineData("Foo", false)]
        [InlineData("foob", true)]
        [InlineData("foobar", true)]
        public async Task ValidateAsync_Validates_CardHoldersName(string name, bool isValid)
        {
            // Arrage
            SetupDefaultMocks();
            var cardDetails = new CardDetails { CardHoldersName = name };

            // Act
            var validator = GetCreditCardValidator();
            var validationResult = await validator.ValidateAsync(cardDetails);

            // Assert
            if (isValid)
            {
                Assert.True(!validationResult.Any(r => r.Equals(CreditCardValidator.ErrorCodes.CardHoldersName)));
            }
            else
            {
                Assert.Single(validationResult.Where(r => r.Equals(CreditCardValidator.ErrorCodes.CardHoldersName)));
            }
        }

        private void SetupDefaultMocks()
        {
            _staticValuesProviderMock = new Mock<IStaticValuesProvider>();
            _staticValuesProviderMock.Setup(m => m.GetUtcNow()).Returns(DateTime.SpecifyKind(new DateTime(2021, 01, 01), DateTimeKind.Utc));
        }

        private CreditCardValidator GetCreditCardValidator()
        {
            return new CreditCardValidator(_staticValuesProviderMock.Object);
        }
    }
}
