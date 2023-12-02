using System.Security.Cryptography;
using ExecutableTestTool.Shell.Commands.Abstractions;
using ExecutableTestTool.Shell.Commands.Parsing.Abstractions;
using ExecutableTestTool.Shell.Commands.Results;
using ExecutableTestTool.Shell.Interface.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace ExecutableTestTool.Shell;

public class Shell
{
   public IServiceProvider? ServiceProvider { get; set; }
   private IUserInterface ui;
   private ICommandParser commandParser;
   private ICommandProvider commands;
   
   public Shell(IUserInterface ui, ICommandParser commandParser, ICommandProvider commands)
   {
      this.ui = ui;
      this.commandParser = commandParser;
      this.commands = commands;
   }
   
   public async Task RunAsync(CancellationToken token = default)
   {
      if (ServiceProvider == null)
      {
         throw new InvalidOperationException("Service provider should be set before starting shell");
      }

      ICommandExecutionContext commandExecutionContext = new ShellCommandExecutionContext(ServiceProvider, ui);
      while (!token.IsCancellationRequested)
      {
         var commandInv = commandParser.Parse(await ui.ReadCommandSource(token));
         var commandExists = commands.Commands.TryGetValue(commandInv.Command, out var command);
         if (!commandExists)
         {
            ui.Error($"Cannot recognize \"{commandInv.Command}\" command");
            continue;
         }

         foreach (var result in command.Invoke(commandExecutionContext, commandInv.Arguments))
         {
            result.Output(ui);
         }
      }
   }
}