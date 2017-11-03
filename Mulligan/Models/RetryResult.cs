using System;
using System.Collections.Generic;
using System.Linq;

namespace Mulligan.Models
{
    public sealed class RetryResult<TResult> : RetryResult
    {
        public TResult Result { get; set; }
    }
    
    public class RetryResult
    {
        public DateTime Start { get; set; }

        public DateTime Finish { get; set; }

        public TimeSpan Duration => Finish.Subtract(Start);

        public Exception Exception { get; set; }

        public bool Success { get; set; }
    }
}
