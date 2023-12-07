using ExecutableTestTool.ProcessTracking.Datastructures;
using ExecutableTestTool.Shell.Services.Abstractions;
using OfficeOpenXml;

namespace ExecutableTestTool.Shell.Services.Implementations;

internal class EpPlusExcelWriter : IExcelWriter
{
   public EpPlusExcelWriter()
   {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
   }

   public async Task WriteToFileAsync(string fileName, IEnumerable<ProcessStats> stats)
   {
      var package = new ExcelPackage(fileName);
      
      // TODO Beautify this stuff
      
      var sheet = package.Workbook.Worksheets.Add("Testing results");
      sheet.Cells[1, 1].Value = "Test date";
      sheet.Cells[1, 2].Value = "Runtime";
      sheet.Cells[1, 3].Value = "Memory usage";
      var row = 2;
      foreach (var stat in stats)
      {
         sheet.Cells[row, 1].Value = stat.StartTime;
         sheet.Cells[row, 2].Value = stat.Runtime;
         sheet.Cells[row, 3].Value = stat.ExitTime;
         row++;
      }
      sheet.Cells.AutoFitColumns();
      await package.SaveAsync();
   }
}