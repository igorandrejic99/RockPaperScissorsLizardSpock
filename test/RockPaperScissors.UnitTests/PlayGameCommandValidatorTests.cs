using Xunit;
using RockPaperScissorsGame.Models.Commands;
using RockPaperScissorsGame.Validators;
using System;

namespace RockPaperScissorsGame.UnitTests
{
    /// <summary>
    /// Unit tests for the <see cref="PlayGameCommandValidator"/> class, 
    /// which validates <see cref="PlayGameCommand"/> instances.
    /// </summary>
    public class PlayGameCommandValidatorTests
    {
        private readonly PlayGameCommandValidator _validator;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayGameCommandValidatorTests"/> class,
        /// setting up the validator instance for testing.
        /// </summary>
        public PlayGameCommandValidatorTests()
        {
            _validator = new PlayGameCommandValidator();
        }

        /// <summary>
        /// Tests that <see cref="PlayGameCommandValidator.Validate"/> throws 
        /// an <see cref="ArgumentNullException"/> when the command is null.
        /// </summary>
        [Fact]
        public void Validate_ShouldThrowArgumentNullException_WhenCommandIsNull()
        {
            // Arrange
            PlayGameCommand command = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => _validator.Validate(command));
            Assert.Equal("Command cannot be null. (Parameter 'command')", exception.Message);
        }

        /// <summary>
        /// Tests that <see cref="PlayGameCommandValidator.Validate"/> throws 
        /// an <see cref="ArgumentException"/> when the Player property is set to its default value.
        /// </summary>
        [Fact]
        public void Validate_ShouldThrowArgumentException_WhenPlayerIsDefault()
        {
            // Arrange
            var command = new PlayGameCommand { Player = 0 }; // Default for int is 0.

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => _validator.Validate(command));
            Assert.Equal("Format of request has to be: {\"Player\":[int]}. (Parameter 'Player')", exception.Message);
        }

        /// <summary>
        /// Tests that <see cref="PlayGameCommandValidator.Validate"/> throws 
        /// an <see cref="ArgumentOutOfRangeException"/> when the Player property is out of the allowed range (1-5).
        /// </summary>
        /// <param name="playerChoice">Invalid player choice outside the 1-5 range.</param>
        [Theory]
        [InlineData(-1)]
        [InlineData(6)]
        [InlineData(54)]
        public void Validate_ShouldThrowArgumentOutOfRangeException_WhenPlayerIsOutOfRange(int playerChoice)
        {
            // Arrange
            var command = new PlayGameCommand { Player = playerChoice };

            // Act & Assert
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _validator.Validate(command));
            Assert.Equal("Player choice must be between 1 and 5. (Parameter 'Player')", exception.Message);
        }

        /// <summary>
        /// Tests that <see cref="PlayGameCommandValidator.Validate"/> does not throw an exception 
        /// when the Player property is within the valid range of 1 to 5.
        /// </summary>
        /// <param name="playerChoice">Valid player choice within the 1-5 range.</param>
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public void Validate_ShouldNotThrowException_WhenPlayerIsInRange(int playerChoice)
        {
            // Arrange
            var command = new PlayGameCommand { Player = playerChoice };

            // Act & Assert
            _validator.Validate(command); // Should not throw an exception for valid input.
        }
    }
}
