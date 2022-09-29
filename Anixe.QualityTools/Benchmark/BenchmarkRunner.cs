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
    private readonly string? header;
    private readonly BenchmarkRunnerConfig? runnerConfig;

    public static IConfig DefaultConfig { get; } = ManualConfig.CreateEmpty()
      .AddColumnProvider(DefaultColumnProviders.Instance)
      .AddLogger(ConsoleLogger.Default)
      .AddDiagnoser(MemoryDiagnoser.Default)
      .AddAnalyser(EnvironmentAnalyser.Default,
            OutliersAnalyser.Default,
            MinIterationTimeAnalyser.Default,
            MultimodalDistributionAnalyzer.Default,
            RuntimeErrorAnalyser.Default)
      .AddValidator(BaselineValidator.FailOnError,
            SetupCleanupValidator.FailOnError,
            JitOptimizationsValidator.FailOnError,
            RunModeValidator.FailOnError)
      .AddJob(Job.Default.WithGcMode(new GcMode()
      {
        Force = false // tell BenchmarkDotNet not to force GC collections after every iteration
      }).WithEnvironmentVariables(new[] { new EnvironmentVariable("BENCHMARK", "1") }));

    public BenchmarkRunner(string? header = null, BenchmarkRunnerConfig? runnerConfig = null)
    {
      this.header = header;
      this.runnerConfig = runnerConfig;
    }

    /// <summary>Run console application with set environment variable BENCHMARK=1</summary>
    public void Run(string[]? args, IConfig? config = null)
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

      RunBenchmarkDotNetTests(args ?? Array.Empty<string>(), config);
#endif
    }

    private void RunBenchmarkDotNetTests(string[] args, IConfig? config)
    {
      var benchmarkClasses = FindClassesWithMethodAttribute();
      config ??= DefaultConfig;

      var menu = new ConsoleMenu(args, level: 1);
      menu.Add("All tests", () =>
      {
        var benchmarks = benchmarkClasses.Select(c => BenchmarkConverter.TypeToBenchmarks(c, config)).ToArray();
        BenchmarkDotNet.Running.BenchmarkRunner.Run(benchmarks);
        Environment.Exit(0);
      });

      if (this.runnerConfig?.DisplaySubmenuOfMethodsInClass == true)
      {
        foreach (var benchmarkClass in benchmarkClasses)
        {
          menu.Add(benchmarkClass.Name, CreateSubmenu(args, config, benchmarkClass).Show);
        }
      }
      else
      {
        foreach (var benchmarkClass in benchmarkClasses)
        {
          menu.Add(benchmarkClass.Name, () =>
          {
            BenchmarkDotNet.Running.BenchmarkRunner.Run(benchmarkClass, config);
            Environment.Exit(0);
          });
        }
      }
      menu.Add("Exit", ConsoleMenu.Close);
      ApplyConfiguration(menu, "Main menu");
      menu.Show();
    }

    private ConsoleMenu CreateSubmenu(string[] args, IConfig config, Type benchmarkClass)
    {
      var submenu = new ConsoleMenu(args, level: 2);
      submenu.Add("All", () =>
      {
        BenchmarkDotNet.Running.BenchmarkRunner.Run(benchmarkClass, config);
        Environment.Exit(0);
      });

      foreach (var method in GetBenchmarkMethods(benchmarkClass))
      {
        submenu.Add(method.Name, () =>
        {
          BenchmarkDotNet.Running.BenchmarkRunner.Run(benchmarkClass, new[] { method }, config);
          Environment.Exit(0);
        });
      }
      submenu.Add("Back", submenu.CloseMenu);
      ApplyConfiguration(submenu, benchmarkClass.Name);
      return submenu;
    }

    private void ApplyConfiguration(ConsoleMenu menu, string title)
    {
      menu.Configure(conf =>
      {
        conf.Title = title;
        conf.EnableFilter = true;
        conf.WriteHeaderAction = () => Console.WriteLine(this.header);
        conf.EnableBreadcrumb = true;
      });
    }

    private static Type[] FindClassesWithMethodAttribute()
    {
      return Assembly.GetEntryAssembly().GetTypes()
              .Where(HasAnyBenchmarkMethod)
              .OrderBy(c => c.Name)
              .ToArray();
    }

    private static MethodInfo[] GetBenchmarkMethods(Type t)
    {
      return Array.FindAll(t.GetMethods(), HasBenchmarkAttribute);
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
