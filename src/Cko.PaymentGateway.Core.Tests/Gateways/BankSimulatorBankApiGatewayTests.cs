using Cko.Common.Infrastructure.DomainModel;
using Cko.Common.Infrastructure.Helpers;
using Cko.PaymentGateway.Core.Gateways;
using Cko.PaymentGateway.Core.Tests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Cko.PaymentGateway.Core.Tests.Gateways
{
    public class BankSimulatorBankApiGatewayTests
    {
        private Mock<IHttpClientFactory> _httpClientFactoryMock;
        private Mock<ILogger<BankSimulatorBankApiGateway>> _loggerMock;

        public class MockHttpMessageHandler : DelegatingHandler
        {
            private readonly object _response;
            private readonly HttpStatusCode _responseStatusCode;
            private readonly bool _throwException;

            public MockHttpMessageHandler(object response, HttpStatusCode responseStatusCode, bool throwException)
            {
                _response = response;
                _responseStatusCode = responseStatusCode;
                _throwException = throwException;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                if (_throwException) { throw new Exception("Foo exception"); }

                var httpResponse = new HttpResponseMessage(_responseStatusCode)
                {
                    Content = new StringContent(JsonSerializer.Serialize(_response, Constants.JsonSerializerOptions), Encoding.UTF8, "application/json")
                };

                var tsc = new TaskCompletionSource<HttpResponseMessage>();
                tsc.SetResult(httpResponse);
                return tsc.Task;
            }
        }

        public static IEnumerable<object[]> callsData => new List<object[]>
        {
            new object[] { new List<string>(), new List<string>(), "transactionid1", HttpStatusCode.OK},
            new object[] { new List<string>() { "Error" }, new List<string>(), "transactionid2", HttpStatusCode.PreconditionFailed },
            new object[] { new List<string>(), new List<string>() { "Error" }, "transactionid3", HttpStatusCode.PreconditionFailed },
            new object[] { new List<string>() { "Error "}, new List<string>() { "Error" }, "transactionid4", HttpStatusCode.PreconditionFailed }
        };

        [Theory]
        [MemberData(nameof(callsData))]
        public async Task ProcessTransactionAsync_CallsApiAndReturnsTransactionResult(List<string> fromErrors, List<string> toErrors, string transactionId, HttpStatusCode responseCode)
        {
            // Arrange
            var httpResponse = new BankTransactionResult
            {
                FromErrorReasons = fromErrors,
                ToErrorReasons = toErrors,
                TransactionId = transactionId
            };

            SetupDefaultMocks();
            SetupHttpClientFactory(httpResponse, responseCode, false);

            var transaction = new Transaction();

            // Act

            var gateway = GetGateway();
            var transactionResult = await gateway.ProcessTransactionAsync(transaction);

            // Assert
            Assert.Equal(httpResponse.TransactionId, transactionResult.BankTransactionResult.TransactionId);
            Assert.Equal(httpResponse.FromErrorReasons, transactionResult.BankTransactionResult.FromErrorReasons);
            Assert.Equal(httpResponse.ToErrorReasons, transactionResult.BankTransactionResult.ToErrorReasons);
            _loggerMock.VerifyLoggerWasCalledWithMessage(null, LogLevel.Error, 0);
        }

        [Fact]
        public async Task ProcessTransactionAsync_ReturnsAndLogs_ConnectionErrors()
        {
            // Arrange
            var httpResponse = new BankTransactionResult
            {
                FromErrorReasons = new List<string>(),
                ToErrorReasons = new List<string>(),
                TransactionId = "transactionid"
            };

            SetupDefaultMocks();
            SetupHttpClientFactory(httpResponse, HttpStatusCode.OK, true);

            var transaction = new Transaction();

            // Act
            var gateway = GetGateway();
            var transactionResult = await gateway.ProcessTransactionAsync(transaction);

            // Assert
            Assert.Equal(BankSimulatorBankApiGateway.ErrorCodes.ConnectionFailed, transactionResult.ConnectionError);
            Assert.Null(transactionResult.BankTransactionResult);
            _loggerMock.VerifyLoggerWasCalledWithMessage(null, LogLevel.Error, 1);
        }

        private void SetupHttpClientFactory(object response, HttpStatusCode responseStatusCode, bool throwException)
        {
            var httpClient = new HttpClient(new MockHttpMessageHandler(response, responseStatusCode, throwException));
            httpClient.BaseAddress = new Uri("https://foo.bar", UriKind.Absolute);
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _httpClientFactoryMock
                .Setup(m => m.CreateClient(nameof(BankSimulatorBankApiGateway)))
                .Returns(httpClient);
        }

        private void SetupDefaultMocks()
        {
            _loggerMock = new Mock<ILogger<BankSimulatorBankApiGateway>>();
        }

        private BankSimulatorBankApiGateway GetGateway()
        {
            return new BankSimulatorBankApiGateway(_httpClientFactoryMock.Object, _loggerMock.Object);
        }
    }
}
