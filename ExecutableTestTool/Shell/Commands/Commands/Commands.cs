using System.Diagnostics;
using ExecutableTestTool.Shell.Commands.Abstractions;
using ExecutableTestTool.Shell.Commands.Commands.ReflectionCommand;
using ExecutableTestTool.Shell.Commands.Results;
using Microsoft.Extensions.DependencyInjection;
using static ExecutableTestTool.Shell.Commands.Results.CommandResults;

namespace ExecutableTestTool.Shell.Commands.Commands;

public class Commands
{
   [Command(Name = "Help", 
      Aliases = new [] {"help", "?"},
      Usage = "help [<commandName>]", 
      HelpDescription = "Shows list of commands, or if the commandName attribute is specified, shows manual on command")]
   public static IEnumerable<CommandResult> Help(ICommandExecutionContext context, string[] args)
   {
      var commandProvider = context.Services.GetService<ICommandProvider>()!;
      var commands = commandProvider.Commands;

      if (args.Length > 1)
      {
         yield return Error("Invalid usage");
         yield break;
      }

      if (args.Length == 1)
      {
         var exists = commands.TryGetValue(args[0], out var command);

         if (!exists)
         {
            yield return Error($"Can't recognize \"{args[0]}\" command");
            yield break;
         }

         yield return Result(BuildCommandHelpMessage(command!));
         yield break;
      }
       
      foreach (var (alias, command) in commands)
      {
         yield return Result($"{alias}\t{command.Name}");
      }

      string BuildCommandHelpMessage(ICommand command)
      {
         return $"Usage: \n{command.Usage}\nAliases:\n{command.Aliases.Aggregate((p, n) => p + ", " + n)}\nDescription:\n{command.HelpDescription}\n";
      }
   }

   [Command(Name = "Execute", 
      Aliases = new[]{"exec", "exe", "run", "test", "execute"}, 
      Usage = "execute <executablePath>",
      HelpDescription = "Executes a file")]
   public static IEnumerable<CommandResult> Execute(ICommandExecutionContext context, string[] args)
   {
      if (args.Length == 0)
      {
         yield return Error("Please, specify path to an executable");
         yield break;
      }

      string path = args[^1];
      FileInfo file = new FileInfo(path);
      if (!file.Exists)
      {
         yield return Error("Specified file does not exist");
         yield break;
      }
      
      Process process = Process.Start(file.FullName);
      yield return Info("Process started");
      process.Start();
      process.WaitForExit();
      yield return Result("Process finished");
   }
}