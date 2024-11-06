using RockPaperScissorsGame.Models;
using RockPaperScissorsGame.Models.Queries;
using RockPaperScissorsGame.Validators;
using System.Threading.Tasks;

namespace RockPaperScissorsGame.Services
{
    /// <summary>
    /// Handles requests for retrieving a random choice in the game.
    /// Validates the request query and retrieves a random choice from the game service.
    /// </summary>
    public class GetRandomChoiceQueryHandler
    {
        private readonly IGameService _gameService;
        private readonly GetRandomChoiceQueryValidator _validator;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetRandomChoiceQueryHandler"/> class.
        /// </summary>
        /// <param name="gameService">Service that provides game-related operations.</param>
        /// <param name="validator">Validator for validating the random choice query request.</param>
        public GetRandomChoiceQueryHandler(IGameService gameService, GetRandomChoiceQueryValidator validator)
        {
            _gameService = gameService;
            _validator = validator;
        }

        /// <summary>
        /// Handles the request to retrieve a random choice in the game asynchronously.
        /// </summary>
        /// <param name="query">The query request containing parameters for retrieving a random choice.</param>
        /// <returns>A <see cref="Choice"/> object representing a random choice in the game.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the query is null.</exception>
        public async Task<Choice> Handle(GetRandomChoiceQuery query)
        {
            // Validate the query to ensure it's not null or invalid.
            _validator.Validate(query);

            // Retrieve and return a random choice from the game service asynchronously.
            return await _gameService.GetRandomChoice();
        }
    }
}
