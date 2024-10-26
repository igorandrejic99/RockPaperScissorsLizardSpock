using RockPaperScissorsGame.Models;
using RockPaperScissorsGame.Models.Queries;
using RockPaperScissorsGame.Validators;
using System.Collections.Generic;

namespace RockPaperScissorsGame.Services
{
    /// <summary>
    /// Handles requests for retrieving the available choices in the game.
    /// Validates the request query and retrieves choices from the game service.
    /// </summary>
    public class GetChoicesQueryHandler
    {
        private readonly IGameService _gameService;
        private readonly GetChoicesQueryValidator _validator;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetChoicesQueryHandler"/> class.
        /// </summary>
        /// <param name="gameService">Service that provides game-related operations.</param>
        /// <param name="validator">Validator for validating the query request.</param>
        public GetChoicesQueryHandler(IGameService gameService, GetChoicesQueryValidator validator)
        {
            _gameService = gameService;
            _validator = validator;
        }

        /// <summary>
        /// Handles the request to retrieve a list of available choices in the game.
        /// </summary>
        /// <param name="query">The query request containing the parameters for retrieving choices.</param>
        /// <returns>A list of <see cref="Choice"/> objects representing the game's available choices.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the query is null.</exception>
        public List<Choice> Handle(GetChoicesQuery query)
        {
            // Validate the query to ensure it's not null or invalid.
            _validator.Validate(query);

            // Retrieve and return the list of available choices from the game service.
            return _gameService.GetChoices();
        }
    }
}
