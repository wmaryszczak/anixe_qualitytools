using BenchmarkDotNet.Analysers;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Validators;
using ConsoleTools;
using System;
using System.Linq;
using System.Reflection;

namespace Anixe.QualityTools.Benchmark
{
  public class BenchmarkRunner
  {
    private readonly string header;

    public static IConfig DefaultConfig { get; } = ManualConfig.CreateEmpty()
      .With(DefaultColumnProviders.Instance)
      .With(ConsoleLogger.Default)
      .With(MemoryDiagnoser.Default)
      .With(EnvironmentAnalyser.Default,
            OutliersAnalyser.Default,
            MinIterationTimeAnalyser.Default,
            MultimodalDistributionAnalyzer.Default,
            RuntimeErrorAnalyser.Default)
      .With(BaselineValidator.FailOnError,
            SetupCleanupValidator.FailOnError,
            JitOptimizationsValidator.FailOnError,
            RunModeValidator.FailOnError)
      .With(Job.Core.With(new GcMode()
      {
        Force = false // tell BenchmarkDotNet not to force GC collections after every iteration
      }).With(new[] { new EnvironmentVariable("BENCHMARK", "1") }));

    public BenchmarkRunner(string header = null)
    {
      this.header = header;
    }

    /// <summary>Run console application with set environment variable BENCHMARK=1</summary>
    public void Run(string[] args, IConfig config = null)
    {
#if DEBUG
      Console.WriteLine("You are trying to run performance tests using DEBUG mode. Use `dotnet run -c Release`");
#else

      if (args?.Length > 0 && args[0] == "all")
      {
        args[0] = "--menu-select=BenchmarkDotNet.All tests";
        RunBenchmarkDotNetTests(args, config);
        return;
      }
      
      RunBenchmarkDotNetTests(args, config);
#endif
    }

    private void RunBenchmarkDotNetTests(string[] args, IConfig config)
    {
      var benchmarkClasses = FindClassesWithMethodAttribute();
      config = config ?? DefaultConfig;

      var menu = new ConsoleMenu(args, level: 1);
      menu.Add("All tests", () =>
      {
        var benchmarks = benchmarkClasses.Select(c => BenchmarkConverter.TypeToBenchmarks(c, config)).ToArray();
        BenchmarkDotNet.Running.BenchmarkRunner.Run(benchmarks);
        Environment.Exit(0);
      });
      foreach (var benchmarkClass in benchmarkClasses)
      {
        menu.Add(benchmarkClass.Name, () =>
        {
          BenchmarkDotNet.Running.BenchmarkRunner.Run(benchmarkClass, config);
          Environment.Exit(0);
        });
      }
      menu.Add("Exit", ConsoleMenu.Close);
      menu.Configure(conf => { conf.EnableFilter = true; conf.WriteHeaderAction = () => { Console.WriteLine(this.header); }; });
      menu.Show();
    }

    private static Type[] FindClassesWithMethodAttribute()
    {
      return Assembly.GetEntryAssembly().GetTypes()
              .Where(HasAnyBenchmarkMethod)
              .OrderBy(c => c.Name)
              .ToArray();
    }

    private static bool HasAnyBenchmarkMethod(Type t)
    {
      return Array.Exists(t.GetMethods(), HasBenchmarkAttribute);
    }

    private static bool HasBenchmarkAttribute(MethodInfo m)
    {
      return m.GetCustomAttributes(typeof(BenchmarkDotNet.Attributes.BenchmarkAttribute), false).Length > 0;
    }
  }
}
