using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;
using System.Net.Sockets;

namespace Anixe.QualityTools.Benchmark
{
  public class GraylogExporter : IExporter
  {
    private readonly UdpClient udpClient;
    private readonly string env;
    private readonly string platform;
    public TimeSpan Timeout = new TimeSpan(0, 0, 10);

    public string Name => nameof(GraylogExporter);

    public GraylogExporter(string platform, UdpClient udpClient)
    {
      this.udpClient = udpClient;
      this.env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "development";
      this.platform = platform;
    }

    public GraylogExporter(string platform, string host, int port)
    : this(platform, new UdpClient(host, port))
    { }

    public IEnumerable<string> ExportToFiles(Summary summary, ILogger consoleLogger)
    {
      ExportToLog(summary, consoleLogger);
      return Array.Empty<string>();
    }

    public void ExportToLog(Summary summary, ILogger logger)
    {
      var serializer = new JsonSerializer();
      if (summary.Reports.Length > 0)
      {
        foreach (var r in summary.Reports)
        {
          var log = new Dictionary<string, object>
          {
            { "short_message", r.BenchmarkCase.DisplayInfo },
            { "_time_taken_median_ns", r.ResultStatistics.Median },
            { "_time_taken_mean_ns", r.ResultStatistics.Mean },
            { "_bytes_allocated", r.GcStats.BytesAllocatedPerOperation },
            { "_gen0_collections", r.GcStats.Gen0Collections },
            { "_gen1_collections", r.GcStats.Gen1Collections },
            { "_gen2_collections", r.GcStats.Gen2Collections },
            { "_total_operations", r.GcStats.TotalOperations },
            { "_env", this.env },
            { "_platform", this.platform },
            { "_facility", "benchmark" },
          };

          var inputString = JsonConvert.SerializeObject(log);
          logger.WriteLine(inputString);

          if (udpClient != null)
          {
            var bytes = Encoding.UTF8.GetBytes(inputString);
            this.udpClient.Send(bytes, bytes.Length);
          }
        }
      }
    }
  }
}
