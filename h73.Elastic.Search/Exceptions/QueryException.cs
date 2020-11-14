using System;

namespace h73.Elastic.Search.Exceptions
{
    /// <summary>
    /// Exceptions for Query
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class QueryException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryException"/> class.
        /// </summary>
        public QueryException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public QueryException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public QueryException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Missings the body.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="ex">The ex.</param>
        /// <returns>MissingBody QueryException</returns>
        public static QueryException MissingBody(string message, Exception ex)
        {
            return new QueryException(message, ex);
        }
    }
}
