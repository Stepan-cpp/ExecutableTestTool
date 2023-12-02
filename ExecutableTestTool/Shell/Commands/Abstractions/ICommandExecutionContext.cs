using ExecutableTestTool.Shell.Interface.Abstractions;

namespace ExecutableTestTool.Shell.Commands.Abstractions;

public interface ICommandExecutionContext
{
   public IServiceProvider Services { get; }
   public IUserInterface Interface { get; }
}