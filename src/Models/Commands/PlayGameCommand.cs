namespace RockPaperScissorsGame.Models.Commands
{
    /// <summary>
    /// Represents the command to play a game round in the Rock-Paper-Scissors-Lizard-Spock game.
    /// </summary>
    public class PlayGameCommand
    {
        /// <summary>
        /// Gets or sets the player's choice for the game round.
        /// </summary>
        /// <value>
        /// An integer representing the player's choice.
        /// </value>
        /// <remarks>
        /// Valid values are between 1 and 5. The value is used to determine the result when compared to the computer's random choice.
        /// </remarks>
        public int Player { get; set; }
    }
}
