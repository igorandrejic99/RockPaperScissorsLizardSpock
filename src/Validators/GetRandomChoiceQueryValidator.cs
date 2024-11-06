using RockPaperScissorsGame.Models.Queries;
using System;

namespace RockPaperScissorsGame.Validators
{
    /// <summary>
    /// Validator for the <see cref="GetRandomChoiceQuery"/> to ensure the query is valid.
    /// </summary>
    public class GetRandomChoiceQueryValidator : IValidator<GetRandomChoiceQuery>
    {
        /// <summary>
        /// Validates the provided <see cref="GetRandomChoiceQuery"/> instance.
        /// </summary>
        /// <param name="query">The query object to be validated.</param>
        /// <exception cref="ArgumentNullException">Thrown if the query is null.</exception>
        public void Validate(GetRandomChoiceQuery query)
        {
            // Check if the query object is null and throw an exception if it is
            if (query == null)
                throw new ArgumentNullException(nameof(query), "Query cannot be null.");
        }
    }
}
