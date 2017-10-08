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
        public static void While(Action action, TimeSpan timeout, TimeSpan? retryInterval = null)
        {
            DateTime startTime = DateTime.Now;

            while (true)
            {
                try
                {
                    action();
                    return;
                }
                catch (Exception exception)
                {
                    if (IsTimedOut(startTime, timeout))
                        throw new TimeoutException("Timeout occured while retrying", exception);

                    Thread.Sleep(retryInterval ?? DefaultRetryInterval);
                }
            }
        }

        /// <summary>
        /// Retries a function until the function succeeds or until timeout is reached.
        /// </summary>
        /// <typeparam name="T">Return type of the function</typeparam>
        /// <param name="function">Function that will be retried</param>
        /// <param name="timeout">Time the action will be retried</param>
        /// <param name="retryInterval">Interval between retries</param>
        /// <returns>Return of the function</returns>
        public static T While<T>(Func<T> function, TimeSpan timeout, TimeSpan? retryInterval = null)
        {
            DateTime startTime = DateTime.Now;

            while (true)
            {
                try
                {
                    return function();
                }
                catch (Exception exception)
                {
                    Thread.Sleep(retryInterval ?? DefaultRetryInterval);

                    if (IsTimedOut(startTime, timeout))
                        throw new TimeoutException("Timeout occured while retrying", exception);
                }
            }
        }

        /// <summary>
        /// Retries a function until the predicate evaluates false and the function succeeds or until timeout is reached.
        /// </summary>
        /// <typeparam name="T">Return type of the function</typeparam>
        /// <param name="predicate">Predicate that evaluates the results of the function</param>
        /// <param name="function">Function that will be retried</param>
        /// <param name="timeout">Time the action will be retried</param>
        /// <param name="retryInterval">Interval between retries</param>
        /// <returns>Return of the function</returns>
        public static T While<T>(Predicate<T> predicate, Func<T> function, TimeSpan timeout, TimeSpan? retryInterval = null)
        {
            DateTime startTime = DateTime.Now;

            while (true)
            {
                try
                {
                    T result = function();
                    if (!predicate(result))
                        return result;

                    if (IsTimedOut(startTime, timeout))
                        return result;
                }
                catch (Exception exception)
                {
                    Thread.Sleep(retryInterval ?? DefaultRetryInterval);

                    if (IsTimedOut(startTime, timeout))
                        throw new TimeoutException("Timeout occured while retrying", exception);
                }
            }
        }

        /// <summary>
        /// Retries a function until the predicate evaluates false and the function succeeds or until timeout is reached.
        /// </summary>
        /// <typeparam name="T">Return type of the function</typeparam>
        /// <param name="predicate">Predicate that evaluates the results of the function</param>
        /// <param name="function">Function that will be retried</param>
        /// <param name="tryCatchHandler">Custom exception handling for function</param>
        /// <param name="timeout">Time the action will be retried</param>
        /// <param name="retryInterval">Interval between retries</param>
        /// <returns>Return of the function</returns>
        public static T While<T>(Predicate<T> predicate, Func<T> function, Func<Func<T>, T> tryCatchHandler, TimeSpan timeout, TimeSpan? retryInterval = null)
        {
            DateTime startTime = DateTime.Now;

            while (true)
            {
                T result = tryCatchHandler(function);
                if (!predicate(result))
                    return result;

                if (IsTimedOut(startTime, timeout))
                    return result;
            }
        }

        private static bool IsTimedOut(DateTime startTime, TimeSpan timeout)
        {
            // Check for infinite timeout
            if (timeout.TotalMilliseconds < 0)
            {
                return false;
            }
            return DateTime.Now.Subtract(startTime) >= timeout;
        }
    }
}
