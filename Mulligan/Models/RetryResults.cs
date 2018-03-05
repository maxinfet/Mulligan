using System;
using System.Collections.Generic;
using System.Linq;

namespace Mulligan.Models
{
    public sealed class RetryResults<TResult> : RetryResults
    {
        /// <inheritdoc />
        public override TimeSpan GetDuration => new TimeSpan(Retries.Sum(r => r.Duration.Ticks));

        /// <inheritdoc />
        public override int Count => Retries.Count();

        /// <inheritdoc />
        public override bool IsCompletedSuccessfully => Result?.IsCompletedSuccessfully ?? false;

        /// <summary>
        /// Returns a new List object that contains all the RetryResult with a IsCompleteSuccessfully of false
        /// </summary>
        public new List<RetryResult<TResult>> Failures => Retries.Where(r => r.IsCompletedSuccessfully == false).ToList();

        /// <summary>
        /// Returns the only RetryResult that was successful or null if no results were successful
        /// </summary>
        public new RetryResult<TResult> Result => Retries.SingleOrDefault(r => r.IsCompletedSuccessfully);

        /// <summary>
        /// Returns a new List object that contains all the RetryResult
        /// </summary>
        public new List<RetryResult<TResult>> Retries { get; } = new List<RetryResult<TResult>>();
    }

    public class RetryResults
    {
        /// <summary>
        /// Returns a new TimeSpan object whose value is the sum of the TimeSpans of all the RetryResult
        /// </summary>
        public virtual TimeSpan GetDuration => new TimeSpan(Retries.Sum(r => r.Duration.Ticks));

        /// <summary>
        /// Gets the number of Retries contained in the RetryResults
        /// </summary>
        public virtual int Count => Retries.Count();

        /// <summary>
        /// Gets whether the last result has completed successfully
        /// </summary>
        public virtual bool IsCompletedSuccessfully => Result?.IsCompletedSuccessfully ?? false;

        /// <summary>
        /// Gets whether the last result has completed due to an unhandled exception
        /// </summary>
        public virtual bool IsFaulted => Result?.IsFaulted ?? false;

        /// <summary>
        /// Returns a new List object that contains all the RetryResult with a IsCompleteSuccessfully of false
        /// </summary>
        public List<RetryResult> Failures => Retries.Where(r => r.IsCompletedSuccessfully == false).ToList();

        /// <summary>
        /// Returns the only RetryResult that was successful or null if no results were successful
        /// </summary>
        public RetryResult Result => Retries.SingleOrDefault(r => r.IsCompletedSuccessfully);

        /// <summary>
        /// Returns a new List object that contains all the RetryResult
        /// </summary>
        public List<RetryResult> Retries { get; } = new List<RetryResult>();
    }
}
