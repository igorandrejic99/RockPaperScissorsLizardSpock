namespace RockPaperScissorsGame.Configuration
{
    /// <summary>
    /// Configuration settings for the Random Number API.
    /// </summary>
    public class RandomNumberSettings
    {
        /// <summary>
        /// Gets or sets the URL endpoint for the external random number API.
        /// </summary>
        /// <remarks>
        /// This URL is used by the application to fetch random numbers for gameplay purposes.
        /// </remarks>
        public required string ApiUrl { get; set; }
    }
}
