using ExecutableTestTool.Shell.Interface.Datastructures;

namespace ExecutableTestTool.Shell.Interface.Abstractions;

public interface IUserInterface
{
   public void Info(string str, InfoType infoType = InfoType.ShellInfo);
   public void Success(string str);
   public void Error(string str);
   public Task<string> ReadCommandSource(CancellationToken ct);
}