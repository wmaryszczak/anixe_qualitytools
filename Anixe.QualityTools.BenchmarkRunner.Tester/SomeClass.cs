#pragma warning disable CA1822 // Mark members as static
using BenchmarkDotNet.Attributes;

namespace Anixe.QualityTools.BenchmarkRunner.Tester
{
  public class SomeClass
  {
    [Benchmark]
    public void SomeTestCase1()
    {
      // this is an example benchmark method that BenchmarkRunner should call
    }

    [Benchmark]
    public void SomeTestCase2()
    {
      // this is an example benchmark method that BenchmarkRunner should call
    }
  }
}
