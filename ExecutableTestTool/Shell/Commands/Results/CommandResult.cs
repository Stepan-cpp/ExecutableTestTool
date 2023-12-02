using ExecutableTestTool.Shell.Interface.Abstractions;

namespace ExecutableTestTool.Shell.Commands.Results;

public class CommandResult
{
   public Action<IUserInterface> Output { get; }

   public CommandResult(Action<IUserInterface> output)
   {
      Output = output;
   }
}