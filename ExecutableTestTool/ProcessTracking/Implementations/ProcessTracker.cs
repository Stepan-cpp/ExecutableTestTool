using System.Diagnostics;
using System.Net;
using System.Runtime.ExceptionServices;
using ExecutableTestTool.ProcessTracking.Abstractions;
using ExecutableTestTool.ProcessTracking.Datastructures;

namespace ExecutableTestTool.ProcessTracking.Implementations;

public class ProcessTracker : IProcessTracker
{
   private Process? Process { get; set; }
   private CancellationTokenSource? Cts { get; set; }
   private bool IsTracking { get; set; }

   private ProcessStats ps;
   
   public void BeginTracking(Process process, CancellationToken ct = default)
   {
      Process = process;
      Cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
      Task.Run(Track, ct);
      ps = new();

      InitProcessStats();
      
      IsTracking = true;
   }

   private void InitProcessStats()
   {
      ps.StartTime = Process!.StartTime;
      ps.File = Process.MainModule!.FileName;
   }

   private void CompleteProcessStats()
   {
      if (Process == null)
         return;

      try
      {
         if (Process.HasExited)
            return;

         ps.MaximumMemoryAllocated = Process!.PeakWorkingSet64;
      }
      catch
      {
         // Simply not to save stats
      }
   }

   private void Track()
   {
      Debug.Assert(IsTracking != true, "IsTracking must be true!");
      Debug.Assert(Process != null, "Process == null");
      var token = Cts!.Token;
      while (!token.IsCancellationRequested)
      {
         CompleteProcessStats();
      }
   }
   
   public ProcessStats EndTracking()
   {
      if (!IsTracking)
         throw new InvalidOperationException("Cannot end tracking when it is not started");
      ps.ExitTime = Process!.ExitTime;
      Dispose();
      return ps;
   }

   private void Dispose()
   {
      Process!.Dispose();
      Process = null;
      Cts!.Cancel();
      Cts = null;
   }
}