using Cko.PaymentGateway.Core.Services;
using System;
using Xunit;

namespace Cko.PaymentGateway.Core.Tests.Services
{
    public class AppSettingsServiceTests
    {
        [Fact]
        public void AppSettingsService_PicksUpEnvironmentVariables()
        {
            // Arrange
            var bankApiUrl = "https://foo.bar";
            Environment.SetEnvironmentVariable("BANK_API_URL", bankApiUrl );

            // Act
            var appSettingsService = new AppSettingsService();

            // Assert
            Assert.Equal(bankApiUrl, appSettingsService.BankApiUrl);
        }
    }
}
