using Xunit;
using RockPaperScissorsGame.Models.Queries;
using RockPaperScissorsGame.Validators;
using System;

namespace RockPaperScissorsGame.UnitTests
{
    /// <summary>
    /// Unit tests for the <see cref="GetRandomChoiceQueryValidator"/> class, 
    /// which validates <see cref="GetRandomChoiceQuery"/> instances.
    /// </summary>
    public class GetRandomChoiceQueryValidatorTests
    {
        private readonly GetRandomChoiceQueryValidator _validator;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetRandomChoiceQueryValidatorTests"/> class,
        /// setting up the validator instance used for testing.
        /// </summary>
        public GetRandomChoiceQueryValidatorTests()
        {
            // Instantiate the validator to be used in all test cases.
            _validator = new GetRandomChoiceQueryValidator();
        }

        /// <summary>
        /// Tests that <see cref="GetRandomChoiceQueryValidator.Validate(GetRandomChoiceQuery)"/> 
        /// throws an <see cref="ArgumentNullException"/> when the query parameter is null.
        /// </summary>
        [Fact]
        public void Validate_ShouldThrowArgumentNullException_WhenQueryIsNull()
        {
            // Arrange - Set up a null query to simulate an invalid input scenario.
            GetRandomChoiceQuery query = null;

            // Act & Assert - Verify that an ArgumentNullException is thrown.
            var exception = Assert.Throws<ArgumentNullException>(() => _validator.Validate(query));
            Assert.Equal("Query cannot be null. (Parameter 'query')", exception.Message);
        }

        /// <summary>
        /// Tests that <see cref="GetRandomChoiceQueryValidator.Validate(GetRandomChoiceQuery)"/> 
        /// does not throw an exception when the query parameter is valid.
        /// </summary>
        [Fact]
        public void Validate_ShouldNotThrowException_WhenQueryIsValid()
        {
            // Arrange - Create a valid query instance.
            var query = new GetRandomChoiceQuery();

            // Act & Assert - Ensure that no exception is thrown for a valid query.
            _validator.Validate(query); // Should not throw an exception.
        }
    }
}
