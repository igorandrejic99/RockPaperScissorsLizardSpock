using Xunit;
using Moq;
using System.Threading.Tasks;
using RockPaperScissorsGame.Models;
using RockPaperScissorsGame.Models.Commands;
using RockPaperScissorsGame.Services;
using RockPaperScissorsGame.Validators;

namespace RockPaperScissorsGame.UnitTests
{
    /// <summary>
    /// Unit tests for the <see cref="PlayGameCommandHandler"/> class, 
    /// which handles commands to play a game round.
    /// </summary>
    public class PlayGameCommandHandlerTests
    {
        private readonly Mock<IGameService> _mockGameService;
        private readonly PlayGameCommandHandler _handler;
        private readonly PlayGameCommandValidator _validator;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayGameCommandHandlerTests"/> class,
        /// setting up the mock game service, validator, and handler for testing.
        /// </summary>
        public PlayGameCommandHandlerTests()
        {
            // Initialize the mock game service used to simulate game behavior.
            _mockGameService = new Mock<IGameService>();

            // Use the real validator to ensure valid command validation logic.
            _validator = new PlayGameCommandValidator();

            // Instantiate the handler with mock service and validator for testing.
            _handler = new PlayGameCommandHandler(_mockGameService.Object, _validator);
        }

        /// <summary>
        /// Tests that <see cref="PlayGameCommandHandler.Handle(PlayGameCommand)"/> 
        /// returns the expected <see cref="GameResult"/> when provided with a valid command.
        /// </summary>
        [Fact]
        public async Task Handle_ShouldReturnGameResult()
        {
            // Arrange
            var command = new PlayGameCommand { Player = 1 }; // Valid player choice input.
            var gameResult = new GameResult
            {
                Results = "Player Wins!",
                Player = 1,
                Computer = 3
            };

            // Set up the mock service to return the expected game result for a given command.
            _mockGameService.Setup(service => service.PlayGameAsync(command.Player)).ReturnsAsync(gameResult);

            // Act - Call the handler's Handle method with the test command.
            var result = await _handler.Handle(command);

            // Assert - Verify the expected game result and confirm PlayGameAsync was called once.
            Assert.Equal(gameResult, result);
            _mockGameService.Verify(service => service.PlayGameAsync(command.Player), Times.Once);
        }
    }
}
