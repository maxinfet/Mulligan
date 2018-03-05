using System;

namespace Mulligan.Models
{
    public sealed class RetryResult<TResult> : RetryResult
    {
        public TResult Result { get; internal set; }
    }
    
    public class RetryResult
    {
        /// <summary>
        /// Start time of the retry
        /// </summary>
        public DateTime Start { get; internal set; }

        /// <summary>
        /// Finish time of the retry
        /// </summary>
        public DateTime Finish { get; internal set; }

        /// <summary>
        /// Duration of the retry
        /// </summary>
        public TimeSpan Duration => Finish.Subtract(Start);

        /// <summary>
        /// Exception if any exception was thrown during the retry
        /// </summary>
        public Exception Exception { get; internal set; }

        /// <summary>
        /// Gets whether the retry completed successfully
        /// </summary>
        public bool IsCompletedSuccessfully { get; internal set; }

        /// <summary>
        /// Gets whether the retry completed due to an unhandled exception
        /// </summary>
        public bool IsFaulted { get; internal set; }
    }
}
