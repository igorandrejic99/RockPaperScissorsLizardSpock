namespace RockPaperScissorsGame.Models
{
    /// <summary>
    /// Represents the result of an operation, encapsulating a value if successful, or an error message if not.
    /// </summary>
    /// <typeparam name="T">The type of the value contained in the result.</typeparam>
    public class Result<T>
    {
        /// <summary>
        /// Gets the result value if the operation was successful; otherwise, the default value of <typeparamref name="T"/>.
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// Gets the error message if the operation failed; otherwise, <c>null</c>.
        /// </summary>
        public string Error { get; }

        /// <summary>
        /// Gets a value indicating whether the operation was successful.
        /// </summary>
        /// <value><c>true</c> if successful; otherwise, <c>false</c>.</value>
        public bool IsSuccess => Error == null;

        /// <summary>
        /// Initializes a new instance of the <see cref="Result{T}"/> class with the specified value and error message.
        /// </summary>
        /// <param name="value">The value of the result if successful.</param>
        /// <param name="error">The error message if the operation failed.</param>
        protected Result(T value, string error)
        {
            Value = value;
            Error = error;
        }

        /// <summary>
        /// Creates a successful result with the specified value.
        /// </summary>
        /// <param name="value">The value of the successful result.</param>
        /// <returns>A successful <see cref="Result{T}"/> instance containing the value.</returns>
        public static Result<T> Success(T value) => new Result<T>(value, null);

        /// <summary>
        /// Creates a failed result with the specified error message.
        /// </summary>
        /// <param name="error">The error message for the failed result.</param>
        /// <returns>A failed <see cref="Result{T}"/> instance containing the error message.</returns>
        public static Result<T> Failure(string error) => new Result<T>(default, error);
    }
}
