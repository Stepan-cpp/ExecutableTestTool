using ExecutableTestTool.ProcessTracking.Datastructures;

namespace ExecutableTestTool.Shell.Services.Abstractions;

public interface IExcelWriter
{
   Task WriteToFileAsync(string fileName, IEnumerable<ProcessStats> stats);
}