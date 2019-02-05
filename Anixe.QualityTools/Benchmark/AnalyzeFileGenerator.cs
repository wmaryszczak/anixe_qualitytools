using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using System;
using System.Collections.Generic;
using System.Data;

namespace Anixe.QualityTools.Benchmark
{
  internal static class AnalyzeFileGenerator
  {
    public static ExcelPackage GenerateExcelAnalyzeFile(string resultsDirectory, DataTable data)
    {
      var package = new ExcelPackage();
      var sheet = package.Workbook.Worksheets.Add("Graph");
      var dataSheet = package.Workbook.Worksheets.Add("Data");
      OfficeOpenXml.Table.ExcelTable table = AddTableWithBenchData(dataSheet, data);

      var pivotTable = sheet.PivotTables.Add(sheet.Cells["A1"], dataSheet.Cells[table.Address.Address], "Pivotname");

      pivotTable.RowFields.Add(pivotTable.Fields["Date"]);
      pivotTable.DataOnRows = false;
      pivotTable.GrandTotalCaption = "Average";

      for (int i = 1; i < table.Columns.Count; i++)
      {
        var name = table.Columns[i].Name;
        var field = pivotTable.DataFields.Add(pivotTable.Fields[name]);
        field.Name = name;
        field.Function = OfficeOpenXml.Table.PivotTable.DataFieldFunctions.Average;
      }

      var pivotChart = sheet.Drawings.AddChart("ResultsChart", eChartType.LineMarkers, pivotTable);
      pivotChart.DisplayBlanksAs = eDisplayBlanksAs.Span;
      pivotChart.SetSize(700, 300);
      pivotChart.SetPosition(50, 50);

      return package;
    }

    private static OfficeOpenXml.Table.ExcelTable AddTableWithBenchData(ExcelWorksheet dataSheet, DataTable dt)
    {
      dataSheet.Cells["A1"].LoadFromDataTable(dt, PrintHeaders: true);
      dataSheet.Cells["A:A"].Style.Numberformat.Format = "dd-MM-yyyy HH:mm:ss";

      return dataSheet.Tables.Add(dataSheet.Cells[1, 1, dt.Rows.Count + 1, dt.Columns.Count], "dataTable");
    }
  }
}
