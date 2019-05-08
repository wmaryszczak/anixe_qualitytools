using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Anixe.QualityTools.Benchmark
{
  internal static class BenchmarkDataLoader
  {
    public static DataTable LoadDataIntoTable(string resultsDirectory)
    {
      var dt = new DataTable();
      dt.Columns.Add("Date", typeof(DateTime));

      var dir = new DirectoryInfo(resultsDirectory);
      var data = dir.GetFiles("*-report-*.json").Select(file => (file.Name, JObject.Parse(File.ReadAllText(file.FullName))));
      var columns = data.SelectMany(d => d.Item2["Benchmarks"]).Select(x => x["Method"].ToString().Replace("_Benchmark", "")).Distinct();

      foreach (var col in columns)
      {
        dt.Columns.Add(col, typeof(double));
      }

      foreach (var item in data)
      {
        var row = dt.NewRow();
        row["Date"] = DateTime.ParseExact(Path.GetFileNameWithoutExtension(item.Item1).Split('-').Last(), "yyyyMMddTHHmmss", CultureInfo.InvariantCulture);

        foreach (var benchmark in item.Item2["Benchmarks"])
        {
          var name = benchmark["Method"].ToString().Replace("_Benchmark", "");
          row[name] = benchmark["Statistics"]?["Percentiles"]?["P90"]?.Value<double>();
        }
        dt.Rows.Add(row);
      }

      dt.DefaultView.Sort = "Date ASC";

      return dt.DefaultView.ToTable();
    }
  }
}
