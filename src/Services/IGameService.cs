using RockPaperScissorsGame.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RockPaperScissorsGame.Services
{
    /// <summary>
    /// Interface for game-related services, defining core methods
    /// for fetching choices, retrieving a random choice, and playing a game round.
    /// </summary>
    public interface IGameService
    {
        /// <summary>
        /// Retrieves the list of available game choices.
        /// </summary>
        /// <returns>A list of <see cref="Choice"/> objects representing each game choice.</returns>
        List<Choice> GetChoices();

        /// <summary>
        /// Retrieves a random game choice asynchronously.
        /// </summary>
        /// <returns>A <see cref="Choice"/> object representing a randomly selected choice.</returns>
        Task<Choice> GetRandomChoice();

        /// <summary>
        /// Plays a game round between the player's choice and a random computer choice.
        /// </summary>
        /// <param name="playerChoice">An integer representing the player's choice.</param>
        /// <returns>A <see cref="GameResult"/> object containing the game outcome.</returns>
        Task<GameResult> PlayGameAsync(int playerChoice);
    }
}
