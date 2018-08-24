using Mulligan.Models;
using System;
using System.Threading;

namespace Mulligan
{
   public static class Retry
   {
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
            RetryResult result = new RetryResult();

            try
            {
               action();

               result.IsCompletedSuccessfully = true;
            }
            catch (Exception exception)
            {
               result.Exception = exception;
               result.IsCompletedSuccessfully = false;
            }
            finally
            {
               result.Start = retryStart;
               result.Finish = DateTime.Now;
               results.Retries.Add(result);
            }

            if (result.IsCompletedSuccessfully)
               return results;

            if (IsTimedOut(start, timeout))
               return results;

            Thread.Sleep(retryInterval ?? DefaultRetryInterval);
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
            RetryResult<TResult> result = new RetryResult<TResult>();

            try
            {
               result.Value = function();
               result.IsCompletedSuccessfully = true;
            }
            catch (Exception exception)
            {
               result.Exception = exception;
               result.IsCompletedSuccessfully = false;
            }
            finally
            {
               result.Start = retryStart;
               result.Finish = DateTime.Now;
               results.Retries.Add(result);
            }

            if (result.IsCompletedSuccessfully)
               return results;

            if (IsTimedOut(start, timeout))
               return results;

            Thread.Sleep(retryInterval ?? DefaultRetryInterval);
         }
      }

      /// <summary>
      /// Retries a function until the predicate evaluates false and the function succeeds or until timeout is reached.
      /// </summary>
      /// <typeparam name="TResult">Return type of the function</typeparam>
      /// <param name="shouldRetry">Predicate that evaluates the results of the function</param>
      /// <param name="function">Function that will be retried</param>
      /// <param name="timeout">Time the action will be retried</param>
      /// <param name="retryInterval">Interval between retries</param>
      /// <returns>Return of the function</returns>
      public static RetryResults<TResult> While<TResult>(Predicate<TResult> shouldRetry, Func<TResult> function, TimeSpan timeout, TimeSpan? retryInterval = null)
      {
         DateTime start = DateTime.Now;
         RetryResults<TResult> results = new RetryResults<TResult>();

         while (true)
         {
            DateTime retryStart = DateTime.Now;
            RetryResult<TResult> result = new RetryResult<TResult>();

            try
            {
               result.Value = function();
               if (!shouldRetry(result.Value))
                  result.IsCompletedSuccessfully = true;
            }
            catch (Exception exception)
            {
               result.Exception = exception;
               result.IsCompletedSuccessfully = false;
            }
            finally
            {
               result.Start = retryStart;
               result.Finish = DateTime.Now;
               results.Retries.Add(result);
            }

            if (result.IsCompletedSuccessfully)
               return results;

            if (IsTimedOut(start, timeout))
               return results;

            Thread.Sleep(retryInterval ?? DefaultRetryInterval);
         }
      }

      #region Private Helpers
      private static bool IsTimedOut(DateTime startTime, TimeSpan timeout)
      {
         // Check for infinite timeout
         if (timeout == TimeSpan.MaxValue)
            return false;

         return DateTime.Now.Subtract(startTime) >= timeout;
      }
      #endregion
   }
}
