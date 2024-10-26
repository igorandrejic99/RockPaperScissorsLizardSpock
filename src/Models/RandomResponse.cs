using System.Text.Json.Serialization;

namespace RockPaperScissorsGame.Models
{
    public class RandomResponse
    {
        [JsonPropertyName("random_number")]
        public int RandomNumber { get; set; }
    }
}
