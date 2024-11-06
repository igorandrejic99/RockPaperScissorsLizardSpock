using Xunit;
using RockPaperScissorsGame.Models.Queries;
using RockPaperScissorsGame.Validators;
using System;

namespace RockPaperScissorsGame.UnitTests
{
    /// <summary>
    /// Unit tests for the <see cref="GetChoicesQueryValidator"/> class, 
    /// which validates the <see cref="GetChoicesQuery"/> object.
    /// </summary>
    public class GetChoicesQueryValidatorTests
    {
        private readonly GetChoicesQueryValidator _validator;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetChoicesQueryValidatorTests"/> class
        /// and sets up an instance of <see cref="GetChoicesQueryValidator"/> for testing.
        /// </summary>
        public GetChoicesQueryValidatorTests()
        {
            // Instantiate the validator to be tested.
            _validator = new GetChoicesQueryValidator();
        }

        /// <summary>
        /// Tests that <see cref="GetChoicesQueryValidator.Validate(GetChoicesQuery)"/>
        /// throws an <see cref="ArgumentNullException"/> when the query is null.
        /// </summary>
        [Fact]
        public void Validate_ShouldThrowArgumentNullException_WhenQueryIsNull()
        {
            // Arrange - Set the query to null to simulate an invalid input.
            GetChoicesQuery query = null;

            // Act & Assert - Validate should throw an ArgumentNullException for null query.
            var exception = Assert.Throws<ArgumentNullException>(() => _validator.Validate(query));
            Assert.Equal("Query cannot be null. (Parameter 'query')", exception.Message);
        }

        /// <summary>
        /// Tests that <see cref="GetChoicesQueryValidator.Validate(GetChoicesQuery)"/>
        /// does not throw an exception when the query is valid.
        /// </summary>
        [Fact]
        public void Validate_ShouldNotThrowException_WhenQueryIsValid()
        {
            // Arrange - Create a valid GetChoicesQuery instance.
            var query = new GetChoicesQuery();

            // Act & Assert - Ensure that no exception is thrown when the query is valid.
            _validator.Validate(query); // Should not throw an exception.
        }
    }
}
