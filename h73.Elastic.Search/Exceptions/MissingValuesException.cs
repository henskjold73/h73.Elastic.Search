using System;

namespace h73.Elastic.Search.Exceptions
{
    /// <summary>
    /// Missing values exception
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class MissingValuesException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingValuesException"/> class.
        /// </summary>
        public MissingValuesException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MissingValuesException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public MissingValuesException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MissingValuesException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public MissingValuesException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
