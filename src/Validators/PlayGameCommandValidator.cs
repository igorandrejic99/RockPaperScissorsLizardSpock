using RockPaperScissorsGame.Models.Commands;
using System;

namespace RockPaperScissorsGame.Validators
{
    /// <summary>
    /// Validates the <see cref="PlayGameCommand"/> to ensure it meets required constraints.
    /// </summary>
    public class PlayGameCommandValidator : IValidator<PlayGameCommand>
    {
        /// <summary>
        /// Validates the provided <see cref="PlayGameCommand"/>.
        /// </summary>
        /// <param name="command">The <see cref="PlayGameCommand"/> to validate.</param>
        /// <exception cref="ArgumentNullException">Thrown if the command is null.</exception>
        /// <exception cref="ArgumentException">Thrown if the Player property is not set.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the Player choice is outside the allowed range of 1 to 5.</exception>
        public void Validate(PlayGameCommand command)
        {
            // Ensure the command is not null
            if (command == null)
                throw new ArgumentNullException(nameof(command), "Command cannot be null.");

            // Check if the Player property is set (default for int is 0, which is invalid)
            if (command.Player == default) 
                throw new ArgumentException("Format of request has to be: {\"Player\":[int]}.", nameof(command.Player));

            // Ensure the Player choice is within the valid range (1 to 5)
            if (command.Player <= 0 || command.Player > 5)
                throw new ArgumentOutOfRangeException(nameof(command.Player), "Player choice must be between 1 and 5.");
        }
    }
}
