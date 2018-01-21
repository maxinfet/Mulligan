﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Mulligan.Tests
{
    [TestClass]
    public class While
    {
        [TestMethod]
        public void RetryWhileFalse()
        {
            int index = 0;
            List<int> list = new List<int>() {1, 2, 3};

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

            bool ShouldRetry(int @int) => @int != 3;

            RetryResults<int> results = Retry.While(ShouldRetry, Function, TimeSpan.FromSeconds(1));

            Assert.AreEqual(3, results.GetResult());
            Assert.AreEqual(3, results.GetRetryCount());
            Assert.IsTrue(results.SuccessResult.Success);
            Assert.IsTrue(results.FailureResults.All(f => !f.Success));
            Assert.IsTrue(results.Retries.All(r => r.Exception is null));
        }
    }
}
