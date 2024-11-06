namespace RockPaperScissorsGame.Models
{
    /// <summary>
    /// Represents the result of a game round in Rock-Paper-Scissors-Lizard-Spock.
    /// </summary>
    public class GameResult
    {
        /// <summary>
        /// Gets or sets the outcome of the game.
        /// </summary>
        /// <value>A string indicating the result, such as "win," "lose," or "tie."</value>
        public string Results { get; set; }

        /// <summary>
        /// Gets or sets the player's choice in the game.
        /// </summary>
        /// <value>An integer representing the player's choice, mapped to a specific game option.</value>
        public int Player { get; set; }

        /// <summary>
        /// Gets or sets the computer's choice in the game.
        /// </summary>
        /// <value>An integer representing the computer's choice, mapped to a specific game option.</value>
        public int Computer { get; set; }
    }
}
