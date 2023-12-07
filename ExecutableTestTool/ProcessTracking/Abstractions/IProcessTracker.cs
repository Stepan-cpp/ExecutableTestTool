using System.Diagnostics;
using ExecutableTestTool.ProcessTracking.Datastructures;

namespace ExecutableTestTool.ProcessTracking.Abstractions;

public interface IProcessTracker
{
   public void BeginTracking(Process process, CancellationToken ct = default);
   public ProcessStats EndTracking();
}