using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnostics.dotTrace;

namespace BenchmarkDotNet.Samples
{
    [DotTraceDiagnoser]
    public class IntroDotTraceDiagnoser
    {
        [Benchmark]
        public void Fibonacci() => Fibonacci(30);

        private static int Fibonacci(int n)
        {
            return n <= 1 ? n : Fibonacci(n - 1) + Fibonacci(n - 2);
        }
    }
}