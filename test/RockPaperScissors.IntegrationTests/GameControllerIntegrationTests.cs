using System.Net;
using System.Net.Http.Json;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using RockPaperScissorsGame;
using RockPaperScissorsGame.Models;
using RockPaperScissorsGame.Models.Commands;

namespace RockPaperScissorsGame.IntegrationTests
{
    /// <summary>
    /// Integration tests for the GameController endpoints.
    /// </summary>
    public class GameControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly string _baseUrl;

        /// <summary>
        /// Initializes a new instance of <see cref="GameControllerIntegrationTests"/> with injected factory and configuration.
        /// </summary>
        /// <param name="factory">The web application factory used to create the test client.</param>
        public GameControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;

            // Load the configuration and set up the base URL for API requests
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Test.json", optional: false, reloadOnChange: true)
                .Build();

            _baseUrl = configuration["ApiSettings:BaseUrl"];
        }

        /// <summary>
        /// Tests that the GetChoices endpoint returns an HTTP 200 OK response and a list of choices.
        /// </summary>
        [Fact]
        public async Task GetChoices_ShouldReturnOkAndListOfChoices()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync($"{_baseUrl}/choices");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var choices = await response.Content.ReadFromJsonAsync<List<Choice>>();
            Assert.NotNull(choices);
            Assert.True(choices.Count > 0); // Verify choices are returned
        }

        /// <summary>
        /// Tests that the GetRandomChoice endpoint returns an HTTP 200 OK response with a valid random choice.
        /// </summary>
        [Fact]
        public async Task GetRandomChoice_ShouldReturnOkAndRandomChoice()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync($"{_baseUrl}/choice");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var choice = await response.Content.ReadFromJsonAsync<Choice>();
            Assert.NotNull(choice);
            Assert.InRange(choice.Id, 1, 5); // Valid range for choice ID
        }

        /// <summary>
        /// Tests that the Play endpoint returns an HTTP 200 OK response with a valid game result when provided with a valid player choice.
        /// </summary>
        [Fact]
        public async Task Play_ShouldReturnOkAndGameResult_WhenValidCommand()
        {
            // Arrange
            var client = _factory.CreateClient();
            var command = new PlayGameCommand { Player = 1 };

            // Act
            var response = await client.PostAsJsonAsync($"{_baseUrl}/play", command);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var gameResult = await response.Content.ReadFromJsonAsync<GameResult>();
            Assert.NotNull(gameResult);
            Assert.Equal(1, gameResult.Player); // Player choice should match command input
            Assert.InRange(gameResult.Computer, 1, 5); // Ensure computer choice is valid
            Assert.NotNull(gameResult.Results); // Ensure result is not null
        }

        /// <summary>
        /// Tests that the Play endpoint returns a BadRequest response when provided with an invalid player choice.
        /// </summary>
        [Fact]
        public async Task Play_ShouldReturnBadRequest_WhenCommandIsInvalid()
        {
            // Arrange
            var client = _factory.CreateClient();
            var command = new PlayGameCommand { Player = 6 }; // Invalid choice (out of range)

            // Act
            var response = await client.PostAsJsonAsync($"{_baseUrl}/play", command);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var errorMessage = await response.Content.ReadAsStringAsync();
            Assert.Contains("Player choice must be between 1 and 5", errorMessage); // Expected error message
        }

        /// <summary>
        /// Tests that the Play endpoint returns a BadRequest response when provided with various invalid player choices.
        /// </summary>
        /// <param name="playerChoice">Invalid player choice.</param>
        [Theory]
        [InlineData(-1)]
        [InlineData(6)]
        [InlineData(48)]
        public async Task Play_ShouldReturnBadRequest_WhenPlayerChoiceIsInvalid(int playerChoice)
        {
            // Arrange
            var client = _factory.CreateClient();
            var command = new PlayGameCommand { Player = playerChoice };

            // Act
            var response = await client.PostAsJsonAsync($"{_baseUrl}/play", command);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var errorMessage = await response.Content.ReadAsStringAsync();
            Assert.Contains("Player choice must be between 1 and 5", errorMessage); // Expected error message
        }

        /// <summary>
        /// Tests the full game flow by retrieving choices and playing a game, validating the returned game result.
        /// </summary>
        [Fact]
        public async Task FullGameFlow_ShouldReturnValidGameResult()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act - Retrieve choices and select a valid choice
            var choicesResponse = await client.GetAsync($"{_baseUrl}/choices");
            var choices = await choicesResponse.Content.ReadFromJsonAsync<List<Choice>>();
            var playerChoice = choices.First().Id;

            // Act - Play game with selected choice
            var command = new PlayGameCommand { Player = playerChoice };
            var playResponse = await client.PostAsJsonAsync($"{_baseUrl}/play", command);

            // Assert - Validate game result
            Assert.Equal(HttpStatusCode.OK, playResponse.StatusCode);
            var gameResult = await playResponse.Content.ReadFromJsonAsync<GameResult>();
            Assert.NotNull(gameResult);
            Assert.Equal(playerChoice, gameResult.Player);
            Assert.InRange(gameResult.Computer, 1, 5); // Computer choice validity
            Assert.NotNull(gameResult.Results); // Ensure result is returned
        }
    }
}
