using System;
using System.Collections.Generic;
using System.Linq;

namespace Mulligan.Models
{
    public sealed class RetryResults<TResult> : RetryResults
    {
        /// <summary>
        /// Returns the Result by getting the value of the last Retry.
        /// </summary>
        /// <returns>Returns the value of the result, if all retries failed the result will be the default value for the type.</returns>
        public TResult GetResult() => Retries.Last().Result;

        /// <summary>
        /// The total amount of retries attempted
        /// </summary>
        /// <returns>The total amount of retries attempted</returns>
        public override int Count() => Retries.Count();

        /// <summary>
        /// Checks that the last result is a success
        /// </summary>
        public override bool IsCompletedSuccessfully => Result?.IsCompletedSuccessfully ?? false;

        /// <summary>
        /// Collection of all retries that failed
        /// </summary>
        public new List<RetryResult<TResult>> Failures => Retries.Where(r => r.IsCompletedSuccessfully == false).ToList();

        /// <summary>
        /// Result of the retry if successful otherwise returns null
        /// </summary>
        public new RetryResult<TResult> Result => Retries.SingleOrDefault(r => r.IsCompletedSuccessfully);

        /// <summary>
        /// Collection of all retries including success and failures
        /// </summary>
        public new List<RetryResult<TResult>> Retries { get; } = new List<RetryResult<TResult>>();
    }

    public class RetryResults
    {
        /// <summary>
        /// The sum of all the retries
        /// </summary>
        public TimeSpan GetDuration => new TimeSpan(Retries.Sum(r => r.Duration.Ticks));

        /// <summary>
        /// The total amount of retries attempted
        /// </summary>
        /// <returns>The total amount of retries attempted</returns>
        public virtual int Count() => Retries.Count();

        /// <summary>
        /// Checks that the last result is a success
        /// </summary>
        public virtual bool IsCompletedSuccessfully => Result?.IsCompletedSuccessfully ?? false;

        /// <summary>
        /// Collection of all retries that failed
        /// </summary>
        public List<RetryResult> Failures => Retries.Where(r => r.IsCompletedSuccessfully == false).ToList();

        /// <summary>
        /// Result of the retry if successful otherwise returns null
        /// </summary>
        public RetryResult Result => Retries.SingleOrDefault(r => r.IsCompletedSuccessfully);

        /// <summary>
        /// Collection of all retries including success and failures
        /// </summary>
        public List<RetryResult> Retries { get; } = new List<RetryResult>();
    }
}
