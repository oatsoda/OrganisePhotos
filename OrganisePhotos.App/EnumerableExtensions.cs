using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OrganisePhotos.App
{
    public static class EnumerableExtensions
    {
        public static Task RunInParallel<T>(this T[] sourceItems, Func<T, Task> job, int maxConcurrency)
        {
            return Task.Run(() =>
                            {

                                using var concurrencySemaphore = new SemaphoreSlim(maxConcurrency);

                                var tasks = new List<Task>();
                                foreach (var sourceItem in sourceItems)
                                {
                                    concurrencySemaphore.Wait();

                                    var t = Task.Run(async () =>
                                                     {
                                                         try
                                                         {
                                                             await job(sourceItem);
                                                         }
                                                         finally
                                                         {
                                                             // ReSharper disable once AccessToDisposedClosure - Task.WaitAll ensures all complete before disposing Semaphore
                                                             concurrencySemaphore.Release();
                                                         }
                                                     });

                                    tasks.Add(t);
                                }

                                Task.WaitAll(tasks.ToArray());
                            });
        }
    }
}