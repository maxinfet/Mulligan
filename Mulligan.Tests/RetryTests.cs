using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Mulligan.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Mulligan.Tests
{
   [TestClass]
   public class RetryTests
   {
      [TestMethod]
      public void RetryWhile_NotFive()
      {
         int index = 0;
         List<int> list = new List<int>() { 1, 2, 3, 5 };

         int Function()
         {
            try
            {
               return list[index];
            }
            finally
            {
               index++;
            }
         }

         bool ShouldRetry(int @int) => @int != 5;

         RetryResults<int> results = Retry.While(ShouldRetry, Function, TimeSpan.FromSeconds(1));

         Assert.AreEqual(1, results.Retries[0].Value);
         Assert.AreEqual(2, results.Retries[1].Value);
         Assert.AreEqual(3, results.Retries[2].Value);
         Assert.AreEqual(5, results.Retries[3].Value);
         Assert.AreEqual(5, results.Result.Value);
         Assert.AreEqual(4, results.Count);
         Assert.IsTrue(results.IsCompletedSuccessfully);
         Assert.IsTrue(results.Failures.All(f => !f.IsCompletedSuccessfully));
         Assert.IsTrue(results.Retries.All(r => r.Exception is null));
      }

      [TestMethod]
      public void RetryWhile_NotIsCompletedSuccessfully()
      {
         int index = 0;
         List<int> list = new List<int>() { 1, 2, 3, 5 };

         int Function()
         {
            try
            {
               return list[index];
            }
            finally
            {
               index++;
            }
         }

         //4 does not exist in the collection so we should fail to find it
         bool ShouldRetry(int @int) => @int != 4;

         RetryResults<int> results = Retry.While(ShouldRetry, Function, TimeSpan.FromSeconds(1));

         Assert.AreEqual(1, results.Retries[0].Value);
         Assert.AreEqual(2, results.Retries[1].Value);
         Assert.AreEqual(3, results.Retries[2].Value);
         Assert.AreEqual(5, results.Retries[3].Value);
         Assert.IsNotNull(results.Result);
         Assert.IsFalse(results.IsCompletedSuccessfully);
         Assert.IsTrue(results.Failures.All(f => !f.IsCompletedSuccessfully));
         Assert.IsTrue(results.Retries.Count(r => r.Exception != null) >= 1);
      }

      [TestMethod]
      public void RetryWhile_IsException_NotOne()
      {
         int index = 0;
         List<Func<int>> tasks = new List<Func<int>>
            {
                () => throw new Exception("Exception 1"),
                () => throw new Exception("Exception 2"),
                () => throw new Exception("Exception 3"),
                () => 1
            };

         int Function()
         {
            try
            {
               return tasks[index]();
            }
            finally
            {
               index++;
            }
         }

         bool ShouldRetry(int @int) => @int != 1;

         RetryResults<int> results = Retry.While(ShouldRetry, Function, TimeSpan.FromSeconds(1));

         Assert.IsNotNull(results.Retries[0].Exception);
         Assert.IsNotNull(results.Retries[1].Exception);
         Assert.IsNotNull(results.Retries[2].Exception);
         Assert.IsNull(results.Retries[3].Exception);
         Assert.AreEqual(1, results.Retries[3].Value);
         Assert.AreEqual(1, results.Result.Value);
         Assert.AreEqual(tasks.Count, results.Count);
         Assert.IsTrue(results.IsCompletedSuccessfully);
         Assert.IsTrue(results.Failures.All(f => !f.IsCompletedSuccessfully));
      }

      [TestMethod]
      public void RetryWhile_IsException_NoPredicate()
      {
         int index = 0;
         List<Func<int>> tasks = new List<Func<int>>
            {
                () => throw new Exception("Exception 1"),
                () => throw new Exception("Exception 2"),
                () => throw new Exception("Exception 3"),
                () => 1
            };

         int Function()
         {
            try
            {
               return tasks[index]();
            }
            finally
            {
               index++;
            }
         }

         RetryResults<int> results = Retry.While(Function, TimeSpan.FromSeconds(1));

         Assert.IsNotNull(results.Retries[0].Exception);
         Assert.IsNotNull(results.Retries[1].Exception);
         Assert.IsNotNull(results.Retries[2].Exception);
         Assert.IsNull(results.Retries[3].Exception);
         Assert.AreEqual(1, results.Retries[3].Value);
         Assert.AreEqual(1, results.Result.Value);
         Assert.AreEqual(tasks.Count, results.Count);
         Assert.IsTrue(results.IsCompletedSuccessfully);
         Assert.IsTrue(results.Failures.All(f => !f.IsCompletedSuccessfully));
      }

      [TestMethod]
      public void RetryWhile_IsException_NoResult()
      {
         int index = 0;
         List<Action> tasks = new List<Action>
            {
                () => throw new Exception("Exception 1"),
                () => throw new Exception("Exception 2"),
                () => throw new Exception("Exception 3"),
                () => { }
            };

         void Action()
         {
            try
            {
               tasks[index]();
            }
            finally
            {
               index++;
            }
         }

         RetryResults results = Retry.While(Action, TimeSpan.FromSeconds(1));

         Assert.IsNotNull(results.Retries[0].Exception);
         Assert.IsNotNull(results.Retries[1].Exception);
         Assert.IsNotNull(results.Retries[2].Exception);
         Assert.IsNull(results.Retries[3].Exception);
         Assert.AreEqual(tasks.Count, results.Count);
         Assert.IsTrue(results.IsCompletedSuccessfully);
         Assert.IsTrue(results.Failures.All(f => !f.IsCompletedSuccessfully));
      }

      [TestMethod]
      public void RetryWhile_CancelRetry_Action()
      {
         CancellationTokenSource tokenSource = new CancellationTokenSource();

         int index = 0;

         List<Action> tasks = new List<Action>
            {
               () => throw new Exception("Exception 1"),
               () => throw new Exception("Exception 2"),
               () =>
               {
                  tokenSource.Cancel();
                  throw new Exception("Exception 3");
               },
               () => { }
            };

         void Action()
         {
            try
            {
               tasks[index]();
            }
            finally
            {
               index++;
            }
         }

         RetryResults results = Retry.While(Action, TimeSpan.FromSeconds(1), null, tokenSource.Token);

         Assert.IsTrue(results.IsCanceled);
         Assert.IsFalse(results.IsCompletedSuccessfully);
         Assert.IsNotNull(results.Result.Exception);
         Assert.IsTrue(results.Result.Exception is OperationCanceledException);
         Assert.IsTrue(results.GetDuration < TimeSpan.FromSeconds(1));
      }

      [TestMethod]
      public void RetryWhile_CancelRetry_NoPredicate()
      {
         CancellationTokenSource tokenSource = new CancellationTokenSource();

         int index = 0;
         List<Func<int>> tasks = new List<Func<int>>
            {
                () => throw new Exception("Exception 1"),
                () => throw new Exception("Exception 2"),
                () =>
                {
                   tokenSource.Cancel();
                   throw new Exception("Exception 3");
                },
                () => 1
            };

         int Function()
         {
            try
            {
               return tasks[index]();
            }
            finally
            {
               index++;
            }
         }

         RetryResults<int> results = Retry.While(Function, TimeSpan.FromSeconds(1), null, tokenSource.Token);

         Assert.IsTrue(results.IsCanceled);
         Assert.IsFalse(results.IsCompletedSuccessfully);
         Assert.IsNotNull(results.Result.Exception);
         Assert.IsTrue(results.Result.Exception is OperationCanceledException);
         Assert.IsTrue(results.GetDuration < TimeSpan.FromSeconds(1));
      }

      [TestMethod]
      public void RetryWhile_CancelRetry_Predicate()
      {
         CancellationTokenSource tokenSource = new CancellationTokenSource();

         int index = 0;
         List<Func<bool>> tasks = new List<Func<bool>>
            {
               () => false,
               () => false,
               () =>
               {
                  tokenSource.Cancel();
                  return false;
               },
               () => true
            };

         bool Function()
         {
            try
            {
               return tasks[index]();
            }
            finally
            {
               index++;
            }
         }

         bool ShouldRetry(bool @bool) => !@bool;

         RetryResults<bool> results = Retry.While(ShouldRetry, Function, TimeSpan.FromSeconds(1), null, tokenSource.Token);

         Assert.IsTrue(results.IsCanceled);
         Assert.IsFalse(results.IsCompletedSuccessfully);
         Assert.IsNotNull(results.Result.Exception);
         Assert.IsTrue(results.Result.Exception is OperationCanceledException);
         Assert.IsTrue(results.GetDuration < TimeSpan.FromSeconds(1));
      }
   }
}
