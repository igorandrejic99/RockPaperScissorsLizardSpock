using Xunit;
using Moq;
using System.Threading.Tasks;
using RockPaperScissorsGame.Models;
using RockPaperScissorsGame.Models.Queries;
using RockPaperScissorsGame.Services;
using RockPaperScissorsGame.Validators;

namespace RockPaperScissorsGame.UnitTests
{
    /// <summary>
    /// Unit tests for the <see cref="GetRandomChoiceQueryHandler"/> class, 
    /// which handles the processing of <see cref="GetRandomChoiceQuery"/> requests.
    /// </summary>
    public class GetRandomChoiceQueryHandlerTests
    {
        private readonly Mock<IGameService> _mockGameService;
        private readonly GetRandomChoiceQueryHandler _handler;
        private readonly GetRandomChoiceQueryValidator _validator;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetRandomChoiceQueryHandlerTests"/> class,
        /// setting up dependencies and instances required for testing.
        /// </summary>
        public GetRandomChoiceQueryHandlerTests()
        {
            // Mock IGameService to avoid real dependency on the game service.
            _mockGameService = new Mock<IGameService>();

            // Use the real validator to test the query validation behavior.
            _validator = new GetRandomChoiceQueryValidator();

            // Instantiate the handler with the mock game service and real validator.
            _handler = new GetRandomChoiceQueryHandler(_mockGameService.Object, _validator);
        }

        /// <summary>
        /// Verifies that <see cref="GetRandomChoiceQueryHandler.Handle(GetRandomChoiceQuery)"/>
        /// returns a random choice from the game service when given a valid query.
        /// </summary>
        [Fact]
        public async Task Handle_ShouldReturnRandomChoice()
        {
            // Arrange - Set up the expected choice and mock game service behavior.
            var choice = new Choice { Id = 1, Name = "Rock" };
            _mockGameService.Setup(service => service.GetRandomChoice()).ReturnsAsync(choice);

            // Provide a valid query to ensure no validation errors.
            var query = new GetRandomChoiceQuery();

            // Act - Execute the handler to retrieve a random choice.
            var result = await _handler.Handle(query);

            // Assert - Verify the result matches the expected choice.
            Assert.Equal(choice, result);
            _mockGameService.Verify(service => service.GetRandomChoice(), Times.Once);
        }
    }
}
