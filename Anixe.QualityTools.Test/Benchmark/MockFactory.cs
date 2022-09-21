using BenchmarkDotNet.Attributes;
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

namespace Anixe.QualityTools.Test.Benchmark
{
  public static class MockFactory
  {
    public static Summary CreateSummary(Type benchmarkType, string[] result)
    {
      var runInfo = BenchmarkConverter.TypeToBenchmarks(benchmarkType);
      return new Summary(
          title: "MockSummary",
          reports: runInfo.BenchmarksCases.Select((benchmark, index) => CreateReport(benchmark, result[index])).ToImmutableArray(),
          hostEnvironmentInfo: new HostEnvironmentInfoBuilder().WithoutDotNetSdkVersion().Build(),
          resultsDirectoryPath: "",
          logFilePath: "",
          totalTime: TimeSpan.FromMinutes(1),
          cultureInfo: CultureInfo.InvariantCulture,
          validationErrors: ImmutableArray<ValidationError>.Empty,
          columnHidingRules: ImmutableArray<BenchmarkDotNet.Columns.IColumnHidingRule>.Empty);
    }

    private static BenchmarkReport CreateReport(BenchmarkCase benchmarkCase, string result)
    {
      var generateResult = GenerateResult.Success(ArtifactsPaths.Empty, Array.Empty<string>(), noAcknowledgments: true);
      var buildResult = BuildResult.Success(generateResult);
      var executeResult = new ExecuteResult(true, 0, 0, result.Replace("\r", "").Split('\n'), Array.Empty<string>(), launchIndex: 0);
      return new BenchmarkReport(
        true,
        benchmarkCase,
        generateResult,
        buildResult,
        new List<ExecuteResult> { executeResult },
        Array.Empty<Metric>());
    }

    [LongRunJob]
    public class MockBenchmarkClass
    {
      [Benchmark] public void Foo() { }

      [Benchmark] public void Bar() { }
    }
  }
}
