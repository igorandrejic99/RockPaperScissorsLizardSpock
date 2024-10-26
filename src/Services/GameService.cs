using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RockPaperScissorsGame.Models;
using RockPaperScissorsGame.Infrastructure;
using Microsoft.Extensions.Logging;

namespace RockPaperScissorsGame.Services
{
    /// <summary>
    /// Service that handles the core game logic for Rock-Paper-Scissors-Lizard-Spock.
    /// Provides methods to retrieve available choices, generate a random choice, and play a game round.
    /// </summary>
    public class GameService : IGameService
    {
        private readonly RandomNumberService _randomNumberService;
        private readonly ILogger<GameService> _logger;

        /// <summary>
        /// List of available choices in the game.
        /// </summary>
        private static readonly List<Choice> Choices = new List<Choice>
        {
            new Choice { Id = 1, Name = "Rock" },
            new Choice { Id = 2, Name = "Paper" },
            new Choice { Id = 3, Name = "Scissors" },
            new Choice { Id = 4, Name = "Lizard" },
            new Choice { Id = 5, Name = "Spock" }
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="GameService"/> class.
        /// </summary>
        /// <param name="randomNumberService">Service for generating random numbers, used for computer choice.</param>
        /// <param name="logger">Logger for logging game events and results.</param>
        public GameService(RandomNumberService randomNumberService, ILogger<GameService> logger)
        {
            _randomNumberService = randomNumberService;
            _logger = logger;
        }

        /// <summary>
        /// Gets the list of available choices in the game.
        /// </summary>
        /// <returns>A list of <see cref="Choice"/> objects representing each choice option.</returns>
        public List<Choice> GetChoices()
        {
            return Choices;
        }

        /// <summary>
        /// Gets a random choice for the computer by calling the random number service.
        /// </summary>
        /// <returns>A <see cref="Choice"/> object representing the randomly chosen game choice.</returns>
        /// <exception cref="Exception">Thrown when the random number service returns an error.</exception>
        public async Task<Choice> GetRandomChoice()
        {
            // Retrieve a random number from the service
            var result = await _randomNumberService.GetRandomNumberAsync();

            if (!result.IsSuccess)
            {
                _logger.LogError(result.Error);
                // Handle error by throwing an exception with the result's error message
                throw new Exception(result.Error);
            }

            // Map the random number to a choice ID within the range of 1 to 5
            int choiceIndex = result.Value % 5 + 1;
            return GetChoices().First(c => c.Id == choiceIndex);
        }

        /// <summary>
        /// Plays a game round by comparing the player's choice with a randomly generated computer choice.
        /// </summary>
        /// <param name="playerChoice">The player's choice represented as an integer (1-5).</param>
        /// <returns>A <see cref="GameResult"/> containing the outcome of the game.</returns>
        public async Task<GameResult> PlayGameAsync(int playerChoice)
        {
            // Generate a random number for the computer's choice
            var result = await _randomNumberService.GetRandomNumberAsync();
            int computerChoice = result.Value % 5 + 1;

            // Determine game outcome based on player and computer choices
            var results = DetermineResult(playerChoice, computerChoice);
            _logger.LogInformation($"Player choice: {playerChoice}, Computer choice: {computerChoice}, Result: {results}");

            return new GameResult
            {
                Player = playerChoice,
                Computer = computerChoice,
                Results = results
            };
        }

        /// <summary>
        /// Determines the result of the game based on player and computer choices.
        /// </summary>
        /// <param name="player">The player's choice as an integer (1-5).</param>
        /// <param name="computer">The computer's choice as an integer (1-5).</param>
        /// <returns>A string representing the result ("win", "lose", or "tie").</returns>
        private string DetermineResult(int player, int computer)
        {
            // Return "tie" if both choices are the same
            if (player == computer) return "tie";

            // Define the conditions under which each choice wins against others
            var winConditions = new Dictionary<int, List<int>>
            {
                { 1, new List<int> { 3, 4 } }, // Rock beats Scissors, Lizard
                { 2, new List<int> { 1, 5 } }, // Paper beats Rock, Spock
                { 3, new List<int> { 2, 4 } }, // Scissors beats Paper, Lizard
                { 4, new List<int> { 5, 2 } }, // Lizard beats Spock, Paper
                { 5, new List<int> { 3, 1 } }  // Spock beats Scissors, Rock
            };

            // Check if the player’s choice beats the computer’s choice, otherwise return "lose"
            return winConditions[player].Contains(computer) ? "win" : "lose";
        }
    }
}
