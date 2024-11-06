using Xunit;
using Moq;
using RockPaperScissorsGame.Models;
using RockPaperScissorsGame.Models.Queries;
using RockPaperScissorsGame.Services;
using RockPaperScissorsGame.Validators;
using System.Collections.Generic;

namespace RockPaperScissorsGame.UnitTests
{
    /// <summary>
    /// Unit tests for the <see cref="GetChoicesQueryHandler"/> class,
    /// which handles the retrieval of available game choices.
    /// </summary>
    public class GetChoicesQueryHandlerTests
    {
        private readonly Mock<IGameService> _mockGameService;
        private readonly GetChoicesQueryHandler _handler;
        private readonly GetChoicesQueryValidator _validator;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetChoicesQueryHandlerTests"/> class,
        /// setting up the necessary mocks and handler.
        /// </summary>
        public GetChoicesQueryHandlerTests()
        {
            // Mock the IGameService to simulate game choices retrieval.
            _mockGameService = new Mock<IGameService>();

            // Use the real validator for query validation.
            _validator = new GetChoicesQueryValidator();

            // Initialize the handler with the mocked IGameService and real validator.
            _handler = new GetChoicesQueryHandler(_mockGameService.Object, _validator);
        }

        /// <summary>
        /// Tests that <see cref="GetChoicesQueryHandler.Handle(GetChoicesQuery)"/>
        /// returns the correct list of choices provided by <see cref="IGameService"/>.
        /// </summary>
        [Fact]
        public void Handle_ShouldReturnListOfChoices()
        {
            // Arrange - Setup a list of sample choices for testing.
            var choices = new List<Choice>
            {
                new Choice { Id = 1, Name = "Rock" },
                new Choice { Id = 2, Name = "Paper" }
            };

            // Configure the mock to return the sample choices when GetChoices is called.
            _mockGameService.Setup(service => service.GetChoices()).Returns(choices);

            // Create a valid query object to pass to the handler.
            var query = new GetChoicesQuery();

            // Act - Call the handler's Handle method to get the result.
            var result = _handler.Handle(query);

            // Assert - Verify that the result matches the expected choices list.
            Assert.Equal(choices, result);

            // Confirm that GetChoices was called exactly once on the mock.
            _mockGameService.Verify(service => service.GetChoices(), Times.Once);
        }
    }
}
