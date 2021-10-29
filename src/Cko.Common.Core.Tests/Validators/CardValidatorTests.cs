using Cko.Common.Core.Validators;
using Cko.Common.Infrastructure.DomainModel;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cko.Common.Core.Tests.Validators
{
    public class CardValidatorTests
    {
        [Fact]
        public async Task ValidateAsync_ReturnsError_When_CardDetails_AreNull()
        {
            // Arrange
            CardDetails cardDetails = null;

            // Act
            var validator = new CreditCardValidator();
            var validationResult = await validator.ValidateAsync(cardDetails);

            // Assert
            Assert.Single(validationResult);
            Assert.Equal("CardDetails are null", validationResult.Single());
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
            var cardDetails = new CardDetails { CardNumber = cardNumber };

            // Act
            var validator = new CreditCardValidator();
            var validationResult = await validator.ValidateAsync(cardDetails);

            // Assert
            if (isValid)
            {
                Assert.True(!validationResult.Any(r => r.StartsWith("CARD_NUMBER")));
            }
            else
            {
                Assert.Single(validationResult.Where(r => r.StartsWith("CARD_NUMBER")));
            }

        }
    }
}
