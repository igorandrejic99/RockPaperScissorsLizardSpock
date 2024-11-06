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
using RockPaperScissorsGame.Services;
using RockPaperScissorsGame.Infrastructure;
using RockPaperScissorsGame.Configuration;

namespace RockPaperScissorsGame.UnitTests
{
    /// <summary>
    /// Unit tests for <see cref="GameService"/>, covering choice retrieval, random choice generation,
    /// and the game play result logic.
    /// </summary>
    public class GameServiceTests
    {
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly Mock<ILogger<RandomNumberService>> _mockRandomNumberLogger;
        private readonly Mock<IOptions<RandomNumberSettings>> _mockSettings;
        private readonly RandomNumberService _randomNumberService;
        private readonly Mock<ILogger<GameService>> _mockLogger;
        private readonly GameService _gameService;

        /// <summary>
        /// Initializes dependencies and mocks for testing <see cref="GameService"/>.
        /// </summary>
        public GameServiceTests()
        {
            // Mock HttpMessageHandler to simulate HttpClient requests
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
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
                    Content = new StringContent("{\"random_number\": 42}") // Mocked JSON response
                });

            var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://example.com/")
            };

            // Mock configuration for RandomNumberSettings
            _mockSettings = new Mock<IOptions<RandomNumberSettings>>();
            _mockSettings.Setup(s => s.Value).Returns(new RandomNumberSettings
            {
                ApiUrl = "https://example.com/api/random-number"
            });

            // Mock logger for RandomNumberService
            _mockRandomNumberLogger = new Mock<ILogger<RandomNumberService>>();

            // Create RandomNumberService instance with mocked dependencies
            _randomNumberService = new RandomNumberService(httpClient, _mockRandomNumberLogger.Object, _mockSettings.Object);

            // Mock logger for GameService
            _mockLogger = new Mock<ILogger<GameService>>();

            // Create GameService instance using RandomNumberService and a mock logger
            _gameService = new GameService(_randomNumberService, _mockLogger.Object);
        }

        /// <summary>
        /// Tests that GetChoices returns the expected list of game choices.
        /// </summary>
        [Fact]
        public void GetChoices_ShouldReturnListOfChoices()
        {
            // Act
            var choices = _gameService.GetChoices();

            // Assert
            Assert.NotNull(choices);
            Assert.Equal(5, choices.Count); // Ensures all choices are returned
            Assert.Contains(choices, c => c.Name == "Rock");
            Assert.Contains(choices, c => c.Name == "Paper");
            Assert.Contains(choices, c => c.Name == "Scissors");
            Assert.Contains(choices, c => c.Name == "Lizard");
            Assert.Contains(choices, c => c.Name == "Spock");
        }

        /// <summary>
        /// Tests that GetRandomChoice returns a valid choice when random number retrieval is successful.
        /// </summary>
        [Fact]
        public async Task GetRandomChoice_ShouldReturnChoice_WhenRandomNumberIsSuccessful()
        {
            // Act - retrieves a random choice
            var choice = await _gameService.GetRandomChoice();

            // Assert - checks that choice is within valid range
            Assert.NotNull(choice);
            Assert.True(choice.Id >= 1 && choice.Id <= 5);
        }

        /// <summary>
        /// Tests that GetRandomChoice logs an error and returns a fallback choice when random number fails.
        /// </summary>
        [Fact]
        public async Task GetRandomChoice_ShouldLogErrorAndReturnFallback_WhenRandomNumberFails()
        {
            // Arrange - Simulate HttpRequestException when making HTTP request
            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .Throws(new HttpRequestException("Random number generation failed"));

            // Act - retrieve choice to see fallback logic
            var choice = await _gameService.GetRandomChoice();

            // Assert - choice is not null and falls within valid range
            Assert.NotNull(choice);
            Assert.True(choice.Id >= 1 && choice.Id <= 5);

            // Verify error log was written once
            _mockRandomNumberLogger.Verify(
                logger => logger.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Unexpected error while fetching random number.")),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Once);
        }

        /// <summary>
        /// Tests that PlayGameAsync returns the expected result (win, lose, tie) based on player and computer choices.
        /// </summary>
        /// <param name="playerChoice">The player's choice.</param>
        /// <param name="computerChoice">The computer's choice.</param>
        /// <param name="expectedResult">Expected game result based on choices.</param>
        [Theory]
        [InlineData(1, 3, "win")]  // Rock vs Scissors
        [InlineData(2, 1, "win")]  // Paper vs Rock
        [InlineData(3, 4, "win")]  // Scissors vs Lizard
        [InlineData(4, 5, "win")]  // Lizard vs Spock
        [InlineData(5, 2, "lose")]  // Spock vs Paper
        [InlineData(1, 2, "lose")] // Rock vs Paper
        [InlineData(3, 1, "lose")] // Scissors vs Rock
        [InlineData(4, 3, "lose")] // Lizard vs Scissors
        [InlineData(5, 4, "lose")] // Spock vs Lizard
        [InlineData(2, 2, "tie")]  // Paper vs Paper
        [InlineData(4, 4, "tie")]  // Lizard vs Lizard
        public async Task PlayGameAsync_ShouldReturnExpectedResult(int playerChoice, int computerChoice, string expectedResult)
        {
            // Arrange - mock the random number service to return computerChoice
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
                    Content = new StringContent($"{{\"random_number\": {computerChoice - 1}}}") // Adjust to mock correct choice
                });

            // Act - play the game with a specific player choice
            var gameResult = await _gameService.PlayGameAsync(playerChoice);

            // Assert - verify the player's choice, computer's choice, and game result
            Assert.Equal(playerChoice, gameResult.Player);
            Assert.Equal(computerChoice, gameResult.Computer);
            Assert.Equal(expectedResult, gameResult.Results);
        }
    }
}
