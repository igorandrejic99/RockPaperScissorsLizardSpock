using RockPaperScissorsGame.Models;
using RockPaperScissorsGame.Models.Commands;
using RockPaperScissorsGame.Validators;
using System;
using System.Threading.Tasks;

namespace RockPaperScissorsGame.Services
{
    /// <summary>
    /// Handler for processing the play game command by validating 
    /// the command and initiating a game round through the game service.
    /// </summary>
    public class PlayGameCommandHandler
    {
        private readonly IGameService _gameService;
        private readonly PlayGameCommandValidator _validator;

        /// <summary>
        /// Initializes a new instance of <see cref="PlayGameCommandHandler"/>.
        /// </summary>
        /// <param name="gameService">The game service used to execute game logic.</param>
        /// <param name="validator">The validator for verifying the command's validity.</param>
        public PlayGameCommandHandler(IGameService gameService, PlayGameCommandValidator validator)
        {
            _gameService = gameService;
            _validator = validator;
        }

        /// <summary>
        /// Processes the play game command by validating it and initiating 
        /// a game round through the game service.
        /// </summary>
        /// <param name="command">The command containing the player's choice for the game round.</param>
        /// <returns>A <see cref="GameResult"/> object containing the outcome of the game.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the command is null.</exception>
        /// <exception cref="ArgumentException">Thrown if the command contains invalid data.</exception>
        public async Task<GameResult> Handle(PlayGameCommand command)
        {
            // Validate the provided command; throws if invalid
            _validator.Validate(command);
            
            // Initiate the game using the player's choice from the command
            return await _gameService.PlayGameAsync(command.Player);
        }
    }
}
