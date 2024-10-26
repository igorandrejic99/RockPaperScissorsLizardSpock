using Xunit;
using Moq;
using Moq.Protected;
using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RockPaperScissorsGame.Models;
using RockPaperScissorsGame.Infrastructure;
using RockPaperScissorsGame.Configuration;

namespace RockPaperScissorsGame.UnitTests
{
    /// <summary>
    /// Unit tests for the <see cref="RandomNumberService"/> class, verifying that it correctly handles HTTP responses
    /// and properly retries requests when errors occur.
    /// </summary>
    public class RandomNumberServiceTests
    {
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly Mock<ILogger<RandomNumberService>> _mockLogger;
        private readonly Mock<IOptions<RandomNumberSettings>> _mockSettings;
        private readonly RandomNumberService _randomNumberService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RandomNumberServiceTests"/> class,
        /// setting up mocked dependencies for <see cref="RandomNumberService"/>.
        /// </summary>
        public RandomNumberServiceTests()
        {
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _mockLogger = new Mock<ILogger<RandomNumberService>>();
            _mockSettings = new Mock<IOptions<RandomNumberSettings>>();
            _mockSettings.Setup(s => s.Value).Returns(new RandomNumberSettings
            {
                ApiUrl = "https://example.com/api/random-number"
            });

            var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://example.com/")
            };

            _randomNumberService = new RandomNumberService(httpClient, _mockLogger.Object, _mockSettings.Object);
        }

        /// <summary>
        /// Verifies that <see cref="RandomNumberService.GetRandomNumberAsync"/> returns a successful result
        /// when a valid random number is retrieved from the API.
        /// </summary>
        [Fact]
        public async Task GetRandomNumberAsync_ShouldReturnSuccess_WhenRandomNumberIsValid()
        {
            // Arrange: Setup a valid JSON response with a positive random number.
            var randomNumber = 42;
            var jsonResponse = JsonSerializer.Serialize(new RandomResponse { RandomNumber = randomNumber });

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse)
                });

            // Act: Call the method to retrieve a random number.
            var result = await _randomNumberService.GetRandomNumberAsync();

            // Assert: Ensure the result is successful and matches the expected random number.
            Assert.True(result.IsSuccess);
            Assert.Equal(randomNumber, result.Value);

            // Verify that the logger was called with the correct log level and message.
            _mockLogger.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Successfully retrieved random number: {randomNumber}")),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Once);
        }

        /// <summary>
        /// Verifies that <see cref="RandomNumberService.GetRandomNumberAsync"/> retries the request and returns
        /// a fallback value when an <see cref="HttpRequestException"/> occurs.
        /// </summary>
        [Fact]
        public async Task GetRandomNumberAsync_ShouldRetryAndReturnFallback_WhenHttpExceptionOccurs()
        {
            // Arrange: Setup to throw an HttpRequestException for each retry attempt.
            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new HttpRequestException("Network error"));

            // Act: Call the method to retrieve a random number with retries.
            var result = await _randomNumberService.GetRandomNumberAsync();

            // Assert: Ensure the fallback value is returned within the specified range.
            Assert.True(result.IsSuccess);
            Assert.InRange(result.Value, 1, 100);

            // Verify that retries occurred three times.
            _mockLogger.Verify(
                logger => logger.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Retry")),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Exactly(3));

            // Verify the error and fallback messages were logged.
            _mockLogger.Verify(
                logger => logger.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Unexpected error while fetching random number.")),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Once);

            _mockLogger.Verify(
                logger => logger.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("All retries failed")),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Once);
        }

        /// <summary>
        /// Verifies that <see cref="RandomNumberService.GetRandomNumberAsync"/> returns a failure result
        /// when an invalid random number is received.
        /// </summary>
        [Fact]
        public async Task GetRandomNumberAsync_ShouldReturnFailure_WhenRandomNumberIsInvalid()
        {
            // Arrange: Setup a JSON response with an invalid random number.
            var jsonResponse = JsonSerializer.Serialize(new RandomResponse { RandomNumber = -1 });

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse)
                });

            // Act: Call the method to retrieve a random number.
            var result = await _randomNumberService.GetRandomNumberAsync();

            // Assert: Ensure the result is a failure with the expected error message.
            Assert.False(result.IsSuccess);
            Assert.Equal("Invalid random number received.", result.Error);
        }

        /// <summary>
        /// Verifies that <see cref="RandomNumberService.GetRandomNumberAsync"/> falls back to a random number
        /// within the range when the JSON response format is invalid.
        /// </summary>
        [Fact]
        public async Task GetRandomNumberAsync_ShouldReturnFallback_WhenJsonIsInvalid()
        {
            // Arrange: Setup an invalid JSON response that cannot be deserialized.
            var jsonResponse = "Invalid JSON";

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse)
                });

            // Act: Call the method to retrieve a random number, expecting fallback handling.
            var result = await _randomNumberService.GetRandomNumberAsync();

            // Assert: Ensure the fallback result is within the expected range.
            Assert.True(result.IsSuccess);
            Assert.InRange(result.Value, 1, 100);

            // Verify that the warning message for all retries failing was logged.
            _mockLogger.Verify(
                logger => logger.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("All retries failed")),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Once);
        }
    }
}
