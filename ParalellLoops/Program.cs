using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ParalellLoops
{
    class Program
    {
        public static void func(int x)
        {
            Console.WriteLine("Enter");
            Console.WriteLine(x);
        }
        static void Main(string[] args)
        {
            ParallelOptions options = new ParallelOptions();
            options.MaxDegreeOfParallelism = Environment.ProcessorCount * 2;
            var tokenSource = new CancellationTokenSource();
            options.CancellationToken = tokenSource.Token;
            tokenSource.Cancel(); // Cancel

            int[] arr = new int[5] { 1, 2, 3, 4, 5 };
            //  ParalellClass.ParallelFor(2, 12, func);
            //  ParalellClass.ParallelForEach(arr, func);
            ParalellClass.ParallelForEachWithOptions(arr, options, func);
            Console.WriteLine("Finish main ");
        }
    }
}
