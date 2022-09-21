using System.IO;
using System.Text;
using Xunit;
using Anixe.QualityTools.Benchmark;
using BenchmarkDotNet.Loggers;

namespace Anixe.QualityTools.Test.Benchmark
{
  public class GraylogExporterTest
  {
    [Fact]
    public void Should_Export()
    {
      //Arrange
      const string fooBenchmarkResult = @"// AfterActualRun
WorkloadResult   1: 536870912 op, 22366700.00 ns, 0.0417 ns/op
WorkloadResult   2: 536870912 op, 2826400.00 ns, 0.0053 ns/op
WorkloadResult   3: 536870912 op, 45811700.00 ns, 0.0853 ns/op
WorkloadResult   4: 536870912 op, 29350900.00 ns, 0.0547 ns/op
WorkloadResult   5: 536870912 op, 36907300.00 ns, 0.0687 ns/op
WorkloadResult   6: 536870912 op, 22826300.00 ns, 0.0425 ns/op
WorkloadResult   7: 536870912 op, 3750100.00 ns, 0.0070 ns/op
WorkloadResult   8: 536870912 op, 26188500.00 ns, 0.0488 ns/op
WorkloadResult   9: 536870912 op, 20779000.00 ns, 0.0387 ns/op
WorkloadResult  10: 536870912 op, 3224900.00 ns, 0.0060 ns/op
WorkloadResult  11: 536870912 op, 18848000.00 ns, 0.0351 ns/op
WorkloadResult  12: 536870912 op, 3215100.00 ns, 0.0060 ns/op
WorkloadResult  13: 536870912 op, 4805000.00 ns, 0.0090 ns/op
WorkloadResult  14: 536870912 op, 10553200.00 ns, 0.0197 ns/op
WorkloadResult  15: 536870912 op, 0.00 ns, 0.0000 ns/op
GC:  100 10 1 528 50
Threading:  0 0 536870912";

      const string barBenchmarkResult = @"// AfterActualRun
WorkloadResult   1: 536870912 op, 10285200.00 ns, 0.0192 ns/op
WorkloadResult   2: 536870912 op, 53770800.00 ns, 0.1002 ns/op
WorkloadResult   3: 536870912 op, 36945300.00 ns, 0.0688 ns/op
WorkloadResult   4: 536870912 op, 0.00 ns, 0.0000 ns/op
WorkloadResult   5: 536870912 op, 4792100.00 ns, 0.0089 ns/op
WorkloadResult   6: 536870912 op, 2627800.00 ns, 0.0049 ns/op
WorkloadResult   7: 536870912 op, 0.00 ns, 0.0000 ns/op
WorkloadResult   8: 536870912 op, 14942900.00 ns, 0.0278 ns/op
WorkloadResult   9: 536870912 op, 25713000.00 ns, 0.0479 ns/op
WorkloadResult  10: 536870912 op, 40912500.00 ns, 0.0762 ns/op
WorkloadResult  11: 536870912 op, 11266000.00 ns, 0.0210 ns/op
WorkloadResult  12: 536870912 op, 0.00 ns, 0.0000 ns/op
WorkloadResult  13: 536870912 op, 0.00 ns, 0.0000 ns/op
WorkloadResult  14: 536870912 op, 3069800.00 ns, 0.0057 ns/op
WorkloadResult  15: 536870912 op, 8841100.00 ns, 0.0165 ns/op
GC:  200 20 2 528 60
Threading:  0 0 536870912";

      var benchmarkResults = new[] { fooBenchmarkResult, barBenchmarkResult };
      var consoleLogger = new SimpleConsoleLogger();
      var summary = MockFactory.CreateSummary(typeof(MockFactory.MockBenchmarkClass), benchmarkResults);
      var subject = new GraylogExporter("some_product", "some_app", "localhost", 5558) { HostName = "SomeHost", Enabled = false };

      // Act
      subject.ExportToFiles(summary, consoleLogger);

      // Assert
      Assert.Equal(File.ReadAllText("Benchmark/expected_console_output.txt"), consoleLogger.GetOutput(), ignoreLineEndingDifferences: true);
    }

    private class SimpleConsoleLogger : ILogger
    {
      private readonly StringBuilder output;

      public string Id => nameof(SimpleConsoleLogger);

      public int Priority => 0;

      public SimpleConsoleLogger()
      {
        this.output = new StringBuilder();
      }

      public void Write(LogKind logKind, string text)
      {
        this.output.Append(text);
      }

      public void Flush() { }

      public void WriteLine() => output.AppendLine();

      public void WriteLine(LogKind logKind, string text) => output.AppendLine(text);

      public string GetOutput() => this.output.ToString();
    }
  }
}
