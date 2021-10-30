using Microsoft.Extensions.Logging;
using Moq;
using System;

namespace Cko.PaymentGateway.Core.Tests.Helpers
{
    public static class ILoggerMockExtensions
    {
        // Based on https://adamstorr.azurewebsites.net/blog/mocking-ilogger-with-moq
        public static Mock<ILogger<T>> VerifyLoggerWasCalledWithMessage<T>(this Mock<ILogger<T>> logger, string expectedMessage, LogLevel logLevel, int numberOfTimes)
        {
            Func<object, Type, bool> state = (v, t) =>
                string.IsNullOrWhiteSpace(expectedMessage) || v.ToString().CompareTo(expectedMessage) == 0;

            logger.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == logLevel),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => state(v, t)),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Exactly(numberOfTimes));

            return logger;
        }
    }
}
