using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamMerge.Tests.Utils
{
    [TestClass]
    public class QueuingTaskTests
    {
        [TestMethod]
        public async Task QueuingTask_WhenAll_WhenCalled_ThenOnlyStartsFourTasksAtTheSameTime()
        {
            var totalTasksDone = new List<int> { 0, 4, 8 };
            var startDate = DateTime.Now;
            var list = new List<int> { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            var totalDone = 0;

            var result = await QueuingTask.WhenAll(list, async x =>
            {
                Assert.IsTrue(totalTasksDone.Contains(totalDone));

                await Task.Delay(2000);

                totalDone++;

                return new List<int> { x };
            });

            Assert.AreEqual(list.Count, result.Count());
            Assert.IsTrue(DateTime.Now.Ticks >= startDate.Ticks + 6000);
        }
    }
}
