using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using RockPaperScissorsGame.Controllers;
using RockPaperScissorsGame.Models;
using RockPaperScissorsGame.Models.Commands;
using RockPaperScissorsGame.Models.Queries;
using RockPaperScissorsGame.Services;
using RockPaperScissorsGame.Validators;
using Xunit;

namespace RockPaperScissorsGame.UnitTests
{
    /// <summary>
    /// Unit tests for the <see cref="GameController"/> class, testing each endpoint behavior.
    /// </summary>
    public class GameControllerTests
    {
        private readonly Mock<IGameService> _mockGameService;
        private readonly Mock<ILogger<GameController>> _mockLogger;
        private readonly GameController _controller;

        /// <summary>
        /// Initializes dependencies for GameController unit tests.
        /// </summary>
        public GameControllerTests()
        {
            _mockGameService = new Mock<IGameService>();
            _mockLogger = new Mock<ILogger<GameController>>();

            // Validators and handlers for controller dependencies
            var getChoicesQueryValidator = new GetChoicesQueryValidator();
            var getRandomChoiceQueryValidator = new GetRandomChoiceQueryValidator();
            var playGameCommandValidator = new PlayGameCommandValidator();

            var getChoicesQueryHandler = new GetChoicesQueryHandler(_mockGameService.Object, getChoicesQueryValidator);
            var getRandomChoiceQueryHandler = new GetRandomChoiceQueryHandler(_mockGameService.Object, getRandomChoiceQueryValidator);
            var playGameCommandHandler = new PlayGameCommandHandler(_mockGameService.Object, playGameCommandValidator);

            _controller = new GameController(
                playGameCommandHandler,
                getChoicesQueryHandler,
                getRandomChoiceQueryHandler,
                _mockLogger.Object);
        }

        /// <summary>
        /// Tests that GetChoices returns an OK response with a list of game choices.
        /// </summary>
        [Fact]
        public void GetChoices_ShouldReturnOkWithChoices()
        {
            // Arrange - setup mock service response
            var choices = new List<Choice>
            {
                new Choice { Id = 1, Name = "Rock" },
                new Choice { Id = 2, Name = "Paper" },
                new Choice { Id = 3, Name = "Scissors" },
                new Choice { Id = 4, Name = "Lizard" },
                new Choice { Id = 5, Name = "Spock" }
            };
            _mockGameService.Setup(service => service.GetChoices()).Returns(choices);

            // Act - call the controller method
            var result = _controller.GetChoices();

            // Assert - verify that the response is OK and contains the expected choices
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedChoices = Assert.IsType<List<Choice>>(okResult.Value);
            Assert.Equal(choices.Count, returnedChoices.Count);
        }

        /// <summary>
        /// Tests that GetRandomChoice returns an OK response with a valid random choice.
        /// </summary>
        [Fact]
        public async Task GetRandomChoice_ShouldReturnOkWithRandomChoice()
        {
            // Arrange - setup mock service response for random choice
            var randomChoice = new Choice { Id = 1, Name = "Rock" };
            _mockGameService.Setup(service => service.GetRandomChoice()).ReturnsAsync(randomChoice);

            // Act - call the controller method asynchronously
            var result = await _controller.GetRandomChoice();

            // Assert - verify that the response is OK and the returned choice matches
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedChoice = Assert.IsType<Choice>(okResult.Value);
            Assert.Equal("Rock", returnedChoice.Name);
        }

        /// <summary>
        /// Tests that Play endpoint returns OK response with game result when provided with a valid command.
        /// </summary>
        [Fact]
        public async Task Play_ShouldReturnOkWithGameResult()
        {
            // Arrange - setup mock service response with expected game result
            var command = new PlayGameCommand { Player = 1 };
            var gameResult = new GameResult { Results = "win", Player = 1, Computer = 3 };
            _mockGameService.Setup(service => service.PlayGameAsync(1)).ReturnsAsync(gameResult);

            // Act - call the Play method with a valid command
            var result = await _controller.Play(command);

            // Assert - verify that the response is OK and matches the expected result
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedResult = Assert.IsType<GameResult>(okResult.Value);
            Assert.Equal("win", returnedResult.Results);
            Assert.Equal(1, returnedResult.Player);
            Assert.Equal(3, returnedResult.Computer);
        }

        /// <summary>
        /// Tests that Play endpoint returns a BadRequest response when command is null.
        /// </summary>
        [Fact]
        public async Task Play_ShouldReturnBadRequestForNullCommand()
        {
            // Act - call Play method with null command
            var result = await _controller.Play(null);

            // Assert - verify that a BadRequest result is returned
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Command cannot be null.", badRequestResult.Value);
        }

        /// <summary>
        /// Tests that GetChoices returns an InternalServerError response when an exception is thrown.
        /// </summary>
        [Fact]
        public void GetChoices_ShouldReturnInternalServerErrorOnException()
        {
            // Arrange - setup mock service to throw exception
            _mockGameService.Setup(service => service.GetChoices()).Throws(new Exception("Something went wrong"));

            // Act - call GetChoices method
            var result = _controller.GetChoices();

            // Assert - verify that a 500 status code is returned with appropriate error message
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
            Assert.Equal("Internal server error. Please try again later.", objectResult.Value);
        }

        /// <summary>
        /// Tests that GetRandomChoice returns an InternalServerError response when an exception is thrown.
        /// </summary>
        [Fact]
        public async Task GetRandomChoice_ShouldReturnInternalServerErrorOnException()
        {
            // Arrange - setup mock service to throw exception
            _mockGameService.Setup(service => service.GetRandomChoice()).ThrowsAsync(new Exception("Something went wrong"));

            // Act - call GetRandomChoice method asynchronously
            var result = await _controller.GetRandomChoice();

            // Assert - verify that a 500 status code is returned with appropriate error message
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
            Assert.Equal("Internal server error. Please try again later.", objectResult.Value);
        }

        /// <summary>
        /// Tests that Play endpoint returns a BadRequest response when validation fails.
        /// </summary>
        [Fact]
        public async Task Play_ShouldReturnBadRequestWhenValidationFails()
        {
            // Arrange - setup an invalid command to trigger validation error
            var command = new PlayGameCommand { Player = -1 }; // Invalid choice
            _mockGameService.Setup(service => service.PlayGameAsync(It.IsAny<int>())).ThrowsAsync(new ArgumentException("Invalid input"));

            // Act - call Play method with invalid command
            var result = await _controller.Play(command);

            // Assert - verify that a BadRequest result is returned with expected error message
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.StartsWith("Invalid input: Player choice must be between 1 and 5.", badRequestResult.Value.ToString());
        }

        /// <summary>
        /// Tests that Play endpoint correctly handles a tie (draw) scenario.
        /// </summary>
        [Fact]
        public async Task Play_ShouldHandleDrawScenarioCorrectly()
        {
            // Arrange - setup mock service response for a draw scenario
            var command = new PlayGameCommand { Player = 2 }; // Example: Player chooses Paper
            var gameResult = new GameResult { Results = "tie", Player = 2, Computer = 2 };
            _mockGameService.Setup(service => service.PlayGameAsync(2)).ReturnsAsync(gameResult);

            // Act - call Play method with a command that leads to a draw
            var result = await _controller.Play(command);

            // Assert - verify that the response is OK and indicates a draw
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedResult = Assert.IsType<GameResult>(okResult.Value);
            Assert.Equal("tie", returnedResult.Results);
            Assert.Equal(2, returnedResult.Player);
            Assert.Equal(2, returnedResult.Computer);
        }
    }
}
