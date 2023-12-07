using ExecutableTestTool.Plugins.Abstractions;
using ExecutableTestTool.Shell.Commands.Abstractions;

namespace ExecutableTestTool.Plugins.Implementations;

internal class CommandsRegistry(ICommandsProvider commandsProvider) : ICommandsRegistry
{
   public void Register(ICommand command)
   {
      foreach (var alias in command.Aliases)
      {
         commandsProvider.Commands.Add(alias, command);
      }
   }
}