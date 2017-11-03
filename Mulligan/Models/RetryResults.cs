using Mulligan.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mulligan
{
    public sealed class RetryResults<TResult> : RetryResults
    {
        public TResult GetResult() => Retries.Last().Result;

        public override int GetRetryCount() => Retries.Count();

        public new List<RetryResult<TResult>> FailureResults => Retries.Where(r => r.Success == false).ToList();

        public new RetryResult<TResult> SuccessResult => Retries.Single(r => r.Success);

        public new List<RetryResult<TResult>> Retries { get; private set; } = new List<RetryResult<TResult>>();
    }

    public class RetryResults
    {
        public TimeSpan GetDuration => new TimeSpan(Retries.Sum(r => r.Duration.Ticks));

        public virtual int GetRetryCount() => Retries.Count();

        public List<RetryResult> FailureResults => Retries.Where(r => r.Success == false).ToList();

        public RetryResult SuccessResult => Retries.Single(r => r.Success);

        public List<RetryResult> Retries { get; private set; } = new List<RetryResult>();
    }
}
