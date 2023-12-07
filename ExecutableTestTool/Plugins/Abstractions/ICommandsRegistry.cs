using ExecutableTestTool.Shell.Commands.Abstractions;

namespace ExecutableTestTool.Plugins.Abstractions;

public interface ICommandsRegistry
{
   public void Register(ICommand command);
}