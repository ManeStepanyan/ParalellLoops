using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ParalellLoops
{ /// <summary>
/// Implementing Paralell For and Foreach
/// </summary>
    public class ParalellClass
    { /// <summary>
      /// Paralell for iteration
      /// </summary>
      /// <param name="fromInclusive">start Index of iteration </param>
      /// <param name="toExclusive">last index of iteration</param>
      /// <param name="body"> delegate which holds the method to execute in every iteration </param>
        public static void ParallelFor(int fromInclusive, int toExclusive, Action<int> body)
        {
            if (body == null)
            {
                throw new ArgumentNullException("No action");
            }
            int j = 0;
            Task[] tasks = new Task[toExclusive - fromInclusive];
            for (int i = fromInclusive; i < toExclusive; i++)
            {
                int temp = i;
                tasks[j] = new Task(() => { body(temp); });
                tasks[j++].Start();
            }
            Task.WaitAll(tasks);
        }/// <summary>
         /// Paralell ForEach
         /// </summary>
         /// <typeparam name="TSource"> Type of items to iterate </typeparam>
         /// <param name="source"> Collection to iterate </param>
         /// <param name="body"> delegate for holding the method to be called in every iteration </param>
        public static void ParallelForEach<TSource>(IEnumerable<TSource> source, Action<TSource> body)
        {
            if (source == null)
            {
                throw new ArgumentNullException("No source");
            }

            if (body == null)
            {
                throw new ArgumentNullException("No action");
            }
            int j = 0;
            Task[] tasks = new Task[source.Count()];
            foreach (var item in source)
            {
                tasks[j] = new Task(() => body(item));
                tasks[j++].Start();
            }
            Task.WaitAll(tasks);

        }/// <summary>
         /// 
         /// </summary>
         /// <typeparam name="TSource"> Type of items of collection </typeparam>
         /// <param name="source"> Source collection to iterate through</param>
         /// <param name="parallelOptions">object to configure behaviour of operation </param>
         /// <param name="body"> delegate for method to be called in every iteration </param>
        public static void ParallelForEachWithOptions<TSource>(IEnumerable<TSource> source, ParallelOptions parallelOptions, Action<TSource> body)
        {

            if (source == null)
            {
                throw new ArgumentNullException("No source");
            }

            if (body == null)
            {
                throw new ArgumentNullException("No action");
            }
            if (parallelOptions == null)
            {
                throw new ArgumentNullException("No options");
            }
            if (parallelOptions.MaxDegreeOfParallelism == -1)
            {
                try
                {
                    parallelOptions.CancellationToken.ThrowIfCancellationRequested();
                    ParallelForEach(source, body);
                }
                catch (OperationCanceledException) { Console.WriteLine("Cancelled!"); }
            }
            else
            {
                int j = 0;
                List<Task> tasks = new List<Task>();

                foreach (var item in source)
                {
                    try
                    {
                        parallelOptions.CancellationToken.ThrowIfCancellationRequested();
                        tasks.Add(new Task(() => body(item)));
                        tasks[j++].Start();
                        if (tasks.Count() > parallelOptions.MaxDegreeOfParallelism)
                        {
                            while (tasks.Count() > parallelOptions.MaxDegreeOfParallelism)
                            {
                                for (int i = 0; i < parallelOptions.MaxDegreeOfParallelism; i++)
                                {
                                    if (tasks[i].IsCompleted)
                                    {
                                        tasks.Remove(tasks[i]);
                                    }
                                }

                            }
                        }                       
                    }  
                    catch (OperationCanceledException) { Console.WriteLine("Cancelled!"); return; }              
                }
                Task.WaitAll(tasks.ToArray());
            }
        }
    }
}
