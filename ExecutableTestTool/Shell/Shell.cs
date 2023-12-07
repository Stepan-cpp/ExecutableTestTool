using System.Diagnostics;
using System.Security.Cryptography;
using ExecutableTestTool.Shell.Commands.Abstractions;
using ExecutableTestTool.Shell.Commands.Parsing.Abstractions;
using ExecutableTestTool.Shell.Commands.Results;
using ExecutableTestTool.Shell.Interface.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace ExecutableTestTool.Shell;

public class Shell(IUserInterface ui, ICommandParser commandParser, ICommandsProvider commandses)
{
   public IServiceProvider? ServiceProvider { get; set; }

   public async Task RunAsync(CancellationToken token = default)
   {
      CheckServiceProviderNotNull();
      
      ICommandExecutionContext commandExecutionContext = new ShellCommandExecutionContext(ServiceProvider!, ui);
      while (!token.IsCancellationRequested)
      {
         await Tick(commandExecutionContext, token);
      }
   }

   private void CheckServiceProviderNotNull()
   {
      if (ServiceProvider == null)
      {
         throw new InvalidOperationException("Service provider should be set before starting shell");
      }
   }

   private async Task Tick(ICommandExecutionContext commandExecutionContext, CancellationToken token = default)
   {
      var commandInv = commandParser.Parse(await ui.ReadInputAsync(token));
      ICommand? command;
      var commandExists = commandses.Commands.TryGetValue(commandInv.Command, out command);
      if (!commandExists)
      {
         ui.Error($"Cannot recognize \"{commandInv.Command}\" command");
         return;
      }

      Debug.Assert(command != null, "command != null");
      
      try
      {
         foreach (var result in command.Invoke(commandExecutionContext, commandInv.Arguments))
         {
            result.Output(ui);
         }
      }
      catch (Exception ex)
      {
         ui.Error($"During the invocation of command, the unexpected exception was thrown: {ex}");
      }
   }
}