using System;

namespace Mulligan.Models
{
    public sealed class RetryResult<TResult> : RetryResult
    {
        public TResult Result { get; set; }
    }
    
    public class RetryResult
    {
        /// <summary>
        /// Start time of the retry
        /// </summary>
        public DateTime Start { get; set; }

        /// <summary>
        /// Finish time of the retry
        /// </summary>
        public DateTime Finish { get; set; }

        /// <summary>
        /// Duration of the retry
        /// </summary>
        public TimeSpan Duration => Finish.Subtract(Start);

        /// <summary>
        /// Exception if any exception was thrown during the retry
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// Whether the retry completed successfully
        /// </summary>
        public bool IsCompletedSuccessfully { get; set; }
    }
}
