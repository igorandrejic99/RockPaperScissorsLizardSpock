namespace RockPaperScissorsGame.Models
{
    /// <summary>
    /// Represents a request to initiate a game round in Rock-Paper-Scissors-Lizard-Spock.
    /// </summary>
    public class PlayRequest
    {
        /// <summary>
        /// Gets or sets the player's choice for the game round.
        /// </summary>
        /// <value>An integer indicating the player's selected option.</value>
        public int Player { get; set; }
    }
}
