using ExecutableTestTool.ProcessTracking.Abstractions;
using ExecutableTestTool.ProcessTracking.Datastructures;

namespace ExecutableTestTool.ProcessTracking.Implementations;

public class MemoryStatsStorage : IStatsStorage
{
   private LinkedList<ProcessStats> stats = new();
   
   public void Add(ProcessStats ps)
   {
      stats.AddLast(ps);
   }

   public void Clear()
   {
      stats.Clear();
   }

   public IEnumerable<ProcessStats> Stored => stats;
}