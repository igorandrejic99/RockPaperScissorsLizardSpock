namespace RockPaperScissorsGame.Validators
{
    /// <summary>
    /// Defines a contract for validating entities of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the entity to be validated.</typeparam>
    public interface IValidator<T>
    {
        /// <summary>
        /// Validates the specified entity of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="entity">The entity to validate.</param>
        /// <exception cref="ArgumentNullException">Thrown if the entity is null.</exception>
        /// <exception cref="ArgumentException">Thrown if the entity is invalid or contains invalid properties.</exception>
        void Validate(T entity); // Validates the entity to ensure it meets required conditions
    }
}
