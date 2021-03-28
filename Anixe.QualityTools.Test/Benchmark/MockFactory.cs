using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains;
using BenchmarkDotNet.Toolchains.Results;
using BenchmarkDotNet.Validators;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Anixe.QualityTools.Test.Benchmark
{
  public static class MockFactory
  {
    public static Summary CreateSummary(Type benchmarkType)
    {
      var runInfo = BenchmarkConverter.TypeToBenchmarks(benchmarkType);
      return new Summary(
          title: "MockSummary",
          reports: runInfo.BenchmarksCases.Select((benchmark, index) => CreateReport(benchmark, 5, (index + 1) * 100)).ToImmutableArray(),
          hostEnvironmentInfo: new HostEnvironmentInfoBuilder().WithoutDotNetSdkVersion().Build(),
          resultsDirectoryPath: "",
          logFilePath: "",
          totalTime: TimeSpan.FromMinutes(1),
          cultureInfo: CultureInfo.InvariantCulture,
          validationErrors: ImmutableArray<ValidationError>.Empty);
    }

    public static Summary CreateSummary(IConfig config)
    {
      return new Summary(
          "MockSummary",
          CreateReports(config),
          new HostEnvironmentInfoBuilder().WithoutDotNetSdkVersion().Build(),
          "",
          "",
          TimeSpan.FromMinutes(1),
          CultureInfo.InvariantCulture,
          ImmutableArray<ValidationError>.Empty);
    }

    private static ImmutableArray<BenchmarkReport> CreateReports(IConfig config)
        => CreateBenchmarks(config).Select(CreateSimpleReport).ToImmutableArray();

    private static BenchmarkCase[] CreateBenchmarks(IConfig config)
        => BenchmarkConverter.TypeToBenchmarks(typeof(MockBenchmarkClass), config).BenchmarksCases;

    private static BenchmarkReport CreateSimpleReport(BenchmarkCase benchmarkCase) => CreateReport(benchmarkCase, 1, 1);

    private static BenchmarkReport CreateReport(BenchmarkCase benchmarkCase, int n, double nanoseconds)
    {
      var buildResult = BuildResult.Success(GenerateResult.Success(ArtifactsPaths.Empty, Array.Empty<string>()));
      var executeResult = new ExecuteResult(true, 0, 0, Array.Empty<string>(), new[] { $"// Runtime=extra output line" });
      var measurements = Enumerable.Range(0, n)
          .Select(index => new Measurement(1, IterationMode.Workload, IterationStage.Result, index + 1, 1, nanoseconds + index))
          .ToList();
      var gcStats = GcStats.Parse("GC: 100 10 1 666666 888");
      return new BenchmarkReport(true, benchmarkCase, buildResult, buildResult, new List<ExecuteResult> { executeResult }, measurements, gcStats, Array.Empty<Metric>());
    }

    [LongRunJob]
    public class MockBenchmarkClass
    {
      [Benchmark] public void Foo() { }

      [Benchmark] public void Bar() { }
    }
  }
}
