using System.Text.Json.Serialization;

namespace RockPaperScissorsGame.Models
{
    /// <summary>
    /// Represents the response from an external service that provides a random number.
    /// </summary>
    public class RandomResponse
    {
        /// <summary>
        /// Gets or sets the random number received from the external service.
        /// </summary>
        /// <value>An integer representing the generated random number.</value>
        [JsonPropertyName("random_number")]
        public int RandomNumber { get; set; }
    }
}
