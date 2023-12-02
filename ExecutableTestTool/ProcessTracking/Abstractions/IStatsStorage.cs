using ExecutableTestTool.ProcessTracking.Datastructures;

namespace ExecutableTestTool.ProcessTracking.Abstractions;

public interface IStatsStorage
{
   public void Add(ProcessStats stats);
   public void Clear();
   public IEnumerable<ProcessStats> Stored { get; }
}