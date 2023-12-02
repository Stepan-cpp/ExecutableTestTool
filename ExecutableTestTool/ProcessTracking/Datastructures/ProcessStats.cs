namespace ExecutableTestTool.ProcessTracking.Datastructures;

public struct ProcessStats
{
   public string File { get; set; }
   public long MaximumMemoryAllocated { get; set; }
   public DateTime StartTime { get; set; }
   public DateTime ExitTime { get; set; }
   
   public TimeSpan RunningTime => ExitTime - StartTime;
}