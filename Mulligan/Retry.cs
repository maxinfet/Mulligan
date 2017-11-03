using Mulligan.Models;
using System;
using System.Threading;

namespace Mulligan
{
    public class Retry
    {
        public static readonly TimeSpan DefaultRetryFor = TimeSpan.FromMilliseconds(1000);
        private static readonly TimeSpan DefaultRetryInterval = TimeSpan.FromMilliseconds(200);

        /// <summary>
        /// Retries a action until the action succeeds or until timeout is reached.
        /// </summary>
        /// <param name="action">Action that will be retried</param>
        /// <param name="timeout">Time the action will be retried</param>
        /// <param name="retryInterval">Interval between retries</param>
        public static RetryResults While(Action action, TimeSpan timeout, TimeSpan? retryInterval = null)
        {
            DateTime start = DateTime.Now;
            RetryResults results = new RetryResults();

            while (true)
            {
                DateTime retryStart = DateTime.Now;

                try
                {
                    action();

                    results.Retries.Add(new RetryResult()
                    {
                        Start = retryStart,
                        Finish = DateTime.Now,
                        Success = true
                    });

                    return results;
                }
                catch (Exception exception)
                {
                    results.Retries.Add(new RetryResult()
                    {
                        Start = retryStart,
                        Finish = DateTime.Now,
                        Exception = exception,
                        Success = false
                    });

                    if (IsTimedOut(start, timeout))
                        return results;
                }
                finally
                {
                    Thread.Sleep(retryInterval ?? DefaultRetryInterval);
                }
            }
        }

        /// <summary>
        /// Retries a function until the function succeeds or until timeout is reached.
        /// </summary>
        /// <typeparam name="TResult">Return type of the function</typeparam>
        /// <param name="function">Function that will be retried</param>
        /// <param name="timeout">Time the action will be retried</param>
        /// <param name="retryInterval">Interval between retries</param>
        /// <returns>Return of the function</returns>
        public static RetryResults<TResult> While<TResult>(Func<TResult> function, TimeSpan timeout, TimeSpan? retryInterval = null)
        {
            DateTime start = DateTime.Now;
            RetryResults<TResult> results = new RetryResults<TResult>();

            while (true)
            {
                DateTime retryStart = DateTime.Now;

                try
                {
                    results.Retries.Add(new RetryResult<TResult>()
                    {
                        Start = retryStart,
                        Finish = DateTime.Now,
                        Result = function(),
                        Success = true
                    });

                    return results;
                }
                catch (Exception exception)
                {
                    results.Retries.Add(new RetryResult<TResult>()
                    {
                        Start = retryStart,
                        Finish = DateTime.Now,
                        Exception = exception,
                        Success = true
                    });

                    if (IsTimedOut(start, timeout))
                        return results;
                }
                finally
                {
                    Thread.Sleep(retryInterval ?? DefaultRetryInterval);
                }
            }
        }

        /// <summary>
        /// Retries a function until the predicate evaluates false and the function succeeds or until timeout is reached.
        /// </summary>
        /// <typeparam name="TResult">Return type of the function</typeparam>
        /// <param name="predicate">Predicate that evaluates the results of the function</param>
        /// <param name="function">Function that will be retried</param>
        /// <param name="timeout">Time the action will be retried</param>
        /// <param name="retryInterval">Interval between retries</param>
        /// <returns>Return of the function</returns>
        public static RetryResults<TResult> While<TResult>(Predicate<TResult> predicate, Func<TResult> function, TimeSpan timeout, TimeSpan? retryInterval = null)
        {
            DateTime start = DateTime.Now;
            RetryResults<TResult> results = new RetryResults<TResult>();

            while (true)
            {
                DateTime retryStart = DateTime.Now;

                try
                {
                    TResult result = function();

                    results.Retries.Add(new RetryResult<TResult>()
                    {
                        Start = retryStart,
                        Finish = DateTime.Now,
                        Result = result,
                        Success = true
                    });

                    if (!predicate(result))
                        return results;

                    if (IsTimedOut(start, timeout))
                        return results;
                }
                catch (Exception exception)
                {
                    results.Retries.Add(new RetryResult<TResult>()
                    {
                        Start = retryStart,
                        Finish = DateTime.Now,
                        Exception = exception,
                        Success = false
                    });

                    if (IsTimedOut(start, timeout))
                        return results;
                }
                finally
                {
                    Thread.Sleep(retryInterval ?? DefaultRetryInterval);
                }
            }
        }

        /// <summary>
        /// Retries a function until the predicate evaluates false and the function succeeds or until timeout is reached.
        /// </summary>
        /// <typeparam name="TResult">Return type of the function</typeparam>
        /// <param name="predicate">Predicate that evaluates the results of the function</param>
        /// <param name="function">Function that will be retried</param>
        /// <param name="tryCatchHandler">Custom exception handling for function</param>
        /// <param name="timeout">Time the action will be retried</param>
        /// <param name="retryInterval">Interval between retries</param>
        /// <returns>Return of the function</returns>
        public static RetryResults<TResult> While<TResult>(Predicate<TResult> predicate, Func<TResult> function, Func<Func<TResult>, RetryResult<TResult>> tryCatchHandler, TimeSpan timeout, TimeSpan? retryInterval = null)
        {
            DateTime start = DateTime.Now;
            RetryResults<TResult> results = new RetryResults<TResult>();

            while (true)
            {
                DateTime retryStart = DateTime.Now;

                RetryResult<TResult> result = tryCatchHandler(function);

                results.Retries.Add(result);

                if (!predicate(result.Result))
                    return results;

                if (IsTimedOut(start, timeout))
                    return results;

                Thread.Sleep(retryInterval ?? DefaultRetryInterval);
            }
        }

        private static bool IsTimedOut(DateTime startTime, TimeSpan timeout)
        {
            // Check for infinite timeout
            if (timeout == TimeSpan.MaxValue)
                return false;
            
            return DateTime.Now.Subtract(startTime) >= timeout;
        }
    }
}
