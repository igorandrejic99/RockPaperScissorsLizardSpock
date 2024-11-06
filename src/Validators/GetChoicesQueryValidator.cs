using RockPaperScissorsGame.Models.Queries;
using System;

namespace RockPaperScissorsGame.Validators
{
    /// <summary>
    /// Validator for the <see cref="GetChoicesQuery"/> to ensure the query is not null.
    /// </summary>
    public class GetChoicesQueryValidator : IValidator<GetChoicesQuery>
    {
        /// <summary>
        /// Validates the provided <see cref="GetChoicesQuery"/> instance.
        /// </summary>
        /// <param name="query">The query object to be validated.</param>
        /// <exception cref="ArgumentNullException">Thrown if the query is null.</exception>
        public void Validate(GetChoicesQuery query)
        {
            // Check if the query is null and throw an exception if it is
            if (query == null)
                throw new ArgumentNullException(nameof(query), "Query cannot be null.");
        }
    }
}
