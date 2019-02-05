using BenchmarkDotNet.Analysers;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters.Json;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Validators;
using ConsoleTools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Anixe.QualityTools.Benchmark
{
  public class BenchmarkRunner
  {
    private static readonly string PerfResultsDirectory = Path.Combine("..", "..", "PerfResults");
    private static readonly string ResultsDirectory = Path.Combine(PerfResultsDirectory, "results");
    private readonly string header;

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
        RunBenchmarkDotNetTests(args);
        return;
      }

      Directory.CreateDirectory(ResultsDirectory);
      RunBenchmarkDotNetTests(args, AfterTestAction, config);
#endif
    }

    private void RunBenchmarkDotNetTests(string[] args, Action afterTestAction = null, IConfig config = null)
    {
      var benchmarkClasses = FindClassesWithMethodAttribute();
      config = config ?? ManualConfig.CreateEmpty().WithArtifactsPath(PerfResultsDirectory)
                    .With(DefaultColumnProviders.Instance)
                    .With(new JsonExporter(DateTime.Now.ToString("-yyyyMMddTHHmmss"), true, true))
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

      var menu = new ConsoleMenu(args, level: 1);
      menu.Add("All tests", () =>
      {
        var benchmarks = benchmarkClasses.Select(c => BenchmarkConverter.TypeToBenchmarks(c, config)).ToArray();
        BenchmarkDotNet.Running.BenchmarkRunner.Run(benchmarks, config);
        afterTestAction?.Invoke();
        Environment.Exit(0);
      });
      foreach (var benchmarkClass in benchmarkClasses)
      {
        menu.Add(benchmarkClass.Name, () =>
        {
          BenchmarkDotNet.Running.BenchmarkRunner.Run(benchmarkClass, config);
          afterTestAction?.Invoke();
          Environment.Exit(0);
        });
      }
      menu.Add("Exit", ConsoleMenu.Close);
      menu.Configure(conf => { conf.EnableFilter = true; conf.WriteHeaderAction = () => { Console.WriteLine(this.header); }; });
      menu.Show();
    }

    private static void GenerateAnalyzeFile()
    {
      try
      {
        var data = BenchmarkDataLoader.LoadDataIntoTable(ResultsDirectory);
        var excelPackage = AnalyzeFileGenerator.GenerateExcelAnalyzeFile(ResultsDirectory, data);
        string path = Path.GetFullPath(Path.Combine(PerfResultsDirectory, "analyse.xlsx"));
        excelPackage.SaveAs(new FileInfo(path));
        Console.WriteLine($"Analyse saved into `{path}`");
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
      }
    }

    private static void AfterTestAction()
    {
      GenerateAnalyzeFile();
    }

    private static Type[] FindClassesWithMethodAttribute()
    {
      return Assembly.GetEntryAssembly().GetTypes()
              .Where(t => SelectBenchmarkMethods(t).Any())
              .OrderBy(c => c.Name)
              .ToArray();
    }

    private static IEnumerable<MethodInfo> SelectBenchmarkMethods(Type benchmarkClass)
    {
      return benchmarkClass.GetMethods().Where(m => m.GetCustomAttributes(typeof(BenchmarkDotNet.Attributes.BenchmarkAttribute), false).Length > 0);
    }
  }
}
