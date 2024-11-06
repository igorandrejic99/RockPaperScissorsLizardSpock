using Polly;
using Polly.Retry;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using RockPaperScissorsGame.Models;
using RockPaperScissorsGame.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace RockPaperScissorsGame.Infrastructure
{
    /// <summary>
    /// Service for fetching a random number from an external API with retry policies.
    /// Includes logging and a fallback mechanism.
    /// </summary>
    public class RandomNumberService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<RandomNumberService> _logger;
        private readonly RandomNumberSettings _settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="RandomNumberService"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client used to make API requests.</param>
        /// <param name="logger">Logger for logging warnings, errors, and informational messages.</param>
        /// <param name="settings">Configuration settings for accessing the random number API.</param>
        public RandomNumberService(HttpClient httpClient, ILogger<RandomNumberService> logger, IOptions<RandomNumberSettings> settings)
        {
            _httpClient = httpClient;
            _logger = logger;
            _settings = settings.Value;
        }

        /// <summary>
        /// Fetches a random number from the external API.
        /// Implements a retry policy in case of network or JSON deserialization errors.
        /// </summary>
        /// <returns>
        /// A <see cref="Result{T}"/> containing the random number if successful, 
        /// or an error message if unsuccessful.
        /// </returns>
        public async Task<Result<int>> GetRandomNumberAsync()
        {
            // Define a retry policy: Retries up to 3 times on HttpRequestException or JsonException
            var retryPolicy = Policy<Result<int>>.Handle<HttpRequestException>()
                .Or<JsonException>()
                .RetryAsync(3, onRetry: (result, retryCount) =>
                {
                    var exception = result.Exception;
                    _logger.LogWarning($"Retry {retryCount} due to error: {exception.Message}");
                });

            // Attempt to fetch the random number with the retry policy in place
            try
            {
                return await retryPolicy.ExecuteAsync(async () =>
                {
                    // Send the HTTP GET request
                    var response = await _httpClient.GetAsync(_settings.ApiUrl);
                    _logger.LogInformation($"Received HTTP response with status code: {response.StatusCode}");

                    // Ensure the response was successful; throws an exception if not
                    response.EnsureSuccessStatusCode();

                    // Read and parse the response content
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var randomResponse = JsonSerializer.Deserialize<RandomResponse>(jsonString);

                    // Check that the random number is valid and greater than zero
                    if (randomResponse != null && randomResponse.RandomNumber > 0)
                    {
                        _logger.LogInformation($"Successfully retrieved random number: {randomResponse.RandomNumber}");
                        return Result<int>.Success(randomResponse.RandomNumber);
                    }

                    // Return failure result if the number is invalid
                    return Result<int>.Failure("Invalid random number received.");
                });
            }
            catch (Exception ex)
            {
                // Log the exception after all retry attempts have been exhausted
                _logger.LogError(ex, "Unexpected error while fetching random number.");
                _logger.LogWarning($"All retries failed. Returning a random number between 1 and 100 as fallback due to: {ex.Message}");
                
                // Return a fallback random number in the range [1, 100]
                return Result<int>.Success(new Random().Next(1, 101));
            }
        }
    }
}
