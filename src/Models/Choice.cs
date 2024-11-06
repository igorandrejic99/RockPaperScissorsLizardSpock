namespace RockPaperScissorsGame.Models
{
    /// <summary>
    /// Represents a game choice option in Rock-Paper-Scissors-Lizard-Spock.
    /// </summary>
    public class Choice
    {
        /// <summary>
        /// Gets or sets the unique identifier for the choice.
        /// </summary>
        /// <value>An integer representing the unique ID of the choice (e.g., 1 for Rock).</value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the choice.
        /// </summary>
        /// <value>A string representing the name of the choice (e.g., "Rock", "Paper").</value>
        public string Name { get; set; }
    }
}
