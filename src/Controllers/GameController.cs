using Microsoft.AspNetCore.Mvc;
using RockPaperScissorsGame.Models.Commands;
using RockPaperScissorsGame.Models.Queries;
using RockPaperScissorsGame.Services;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace RockPaperScissorsGame.Controllers
{
    /// <summary>
    /// Controller responsible for handling game actions, including fetching game choices, retrieving random choices,
    /// and playing a game round.
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Route("[controller]")]
    [ApiVersion("1.0")]
    public class GameController : ControllerBase
    {
        private readonly PlayGameCommandHandler _playGameCommandHandler;
        private readonly GetChoicesQueryHandler _getChoicesQueryHandler;
        private readonly GetRandomChoiceQueryHandler _getRandomChoiceQueryHandler;
        private readonly ILogger<GameController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameController"/> class with required dependencies.
        /// </summary>
        /// <param name="playGameCommandHandler">Handles playing a game round.</param>
        /// <param name="getChoicesQueryHandler">Handles fetching all available choices.</param>
        /// <param name="getRandomChoiceQueryHandler">Handles fetching a random choice.</param>
        /// <param name="logger">Logger for logging information, warnings, and errors.</param>
        public GameController(
            PlayGameCommandHandler playGameCommandHandler,
            GetChoicesQueryHandler getChoicesQueryHandler,
            GetRandomChoiceQueryHandler getRandomChoiceQueryHandler,
            ILogger<GameController> logger)
        {
            _playGameCommandHandler = playGameCommandHandler;
            _getChoicesQueryHandler = getChoicesQueryHandler;
            _getRandomChoiceQueryHandler = getRandomChoiceQueryHandler;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves the list of all game choices.
        /// </summary>
        /// <remarks>
        /// This endpoint allows clients to obtain the complete list of choices available in the game.
        /// </remarks>
        /// <returns>Returns a list of choices with details such as ID and name.</returns>
        /// <response code="200">Returns a list of available choices.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [HttpGet("choices")]
        public IActionResult GetChoices()
        {
            try
            {
                _logger.LogInformation("Fetching available choices.");
                
                // Execute the query to retrieve choices
                var choices = _getChoicesQueryHandler.Handle(new GetChoicesQuery());
                
                _logger.LogInformation($"Successfully retrieved {choices.Count} choices.");
                return Ok(choices);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching choices.");
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }

        /// <summary>
        /// Retrieves a random game choice.
        /// </summary>
        /// <remarks>
        /// Useful for simulating a choice by the computer in a game round.
        /// </remarks>
        /// <returns>Returns a randomly selected choice.</returns>
        /// <response code="200">Returns a random choice.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [HttpGet("choice")]
        public async Task<IActionResult> GetRandomChoice()
        {
            try
            {
                _logger.LogInformation("Fetching a random choice.");
                
                // Asynchronously execute the query to retrieve a random choice
                var choice = await _getRandomChoiceQueryHandler.Handle(new GetRandomChoiceQuery());
                
                _logger.LogInformation("Successfully retrieved a random choice.");
                return Ok(choice);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching a random choice.");
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }

        /// <summary>
        /// Plays a round of the game with the specified player choice.
        /// </summary>
        /// <remarks>
        /// Clients send the player's choice, and the API responds with the result of the round.
        /// </remarks>
        /// <param name="command">The command containing the player's choice (1-Rock, 2-Paper ...).</param>
        /// <returns>Returns the game result including both player and computer choices and the outcome.</returns>
        /// <response code="200">Returns the game result indicating win, lose, or tie.</response>
        /// <response code="400">If the input is invalid (e.g., choice is out of range or null).</response>
        /// <response code="500">If an internal server error occurs.</response>
        [HttpPost("play")]
        public async Task<IActionResult> Play([FromBody] PlayGameCommand command)
        {
            // Validate that the command is not null before processing
            if (command == null)
            {
                _logger.LogWarning("Received null command for Play endpoint.");
                return BadRequest("Command cannot be null.");
            }

            try
            {
                _logger.LogInformation($"Playing game with player choice: {command.Player}.");
                
                // Asynchronously execute the command to play the game
                var gameResult = await _playGameCommandHandler.Handle(command);
                
                _logger.LogInformation($"Game played successfully. Result: {gameResult.Results}.");
                return Ok(gameResult);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogWarning(ex, "Invalid request: {Message}", ex.Message);
                return BadRequest($"Invalid request: {ex.Message}");
            }
            catch (ArgumentException ex)
            {
                // Handle cases where the player choice is invalid
                _logger.LogWarning(ex, "Invalid input: {Message}", ex.Message);
                return BadRequest($"Invalid input: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while playing the game.");
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }
    }
}
