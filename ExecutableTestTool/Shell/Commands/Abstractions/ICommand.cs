using ExecutableTestTool.Shell.Commands.Results;

namespace ExecutableTestTool.Shell.Commands.Abstractions;

public interface ICommand
{
   string Name { get; }

   string HelpDescription { get; }

   IEnumerable<string> Aliases { get; }

   string Usage { get; }

   IEnumerable<CommandResult> Invoke(ICommandExecutionContext context, string[] args);
}