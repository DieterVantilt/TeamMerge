using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shared.Utils
{
    public static class QueuingTask
    {
        private static int MAX_NUMBER_OF_TASKS = 4;

        public static async Task<IEnumerable<TResult>> WhenAll<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, Task<List<TResult>>> createTask)
        {
            var resultList = new List<TResult>();
            var tasksToExecute = new List<Task<List<TResult>>>();

            for (var i = 0; i < source.Count(); i += MAX_NUMBER_OF_TASKS)
            {
                foreach (var item in source.Skip(i).Take(MAX_NUMBER_OF_TASKS))
                {
                    tasksToExecute.Add(createTask(item));
                }

                await Task.WhenAll(tasksToExecute);

                resultList.AddRange(tasksToExecute.SelectMany(x => x.Result));

                tasksToExecute.Clear();
            }

            return resultList.ToList();
        }
    }
}