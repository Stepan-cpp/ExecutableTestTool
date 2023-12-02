using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices.JavaScript;
using System.Security.AccessControl;
using System.Text.Json;
using ExecutableTestTool.Common;
using ExecutableTestTool.ProcessTracking.Abstractions;
using ExecutableTestTool.ProcessTracking.Datastructures;
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

      var processTracker = context.Services.GetService<IProcessTracker>()!;
      Process process = Process.Start(file.FullName);
      yield return Info("Process started");
      processTracker.BeginTracking(process);
      process.WaitForExit();
      yield return Result("Process finished");
      
      var ps = processTracker.EndTracking();
      var readableMem = DataAmountConverter.Minimize(ps.MaximumMemoryAllocated);
      
      yield return Result($"Running time: {(long) ps.RunningTime.TotalSeconds}s {ps.RunningTime.Milliseconds}ms");
      yield return Result($"Peak memory usage: {readableMem.scale:F}{readableMem.unit.ToString()}");

      var statsStorage = context.Services.GetService<IStatsStorage>();
      Debug.Assert(statsStorage != null, "statsStorage == null");
      statsStorage!.Add(ps);
      yield return Result($"Successfully added test results into stats storage");
   }
   
   [Command(Name = "storage", 
      Aliases = new[]{"storage"}, 
      Usage = "storage {list/clear/save}",
      HelpDescription = "Manipulates stats storage")]
    public static IEnumerable<CommandResult> Storage(ICommandExecutionContext context, string[] args)
   {
      if (args.Length == 0)
      {
         yield return Error("Please, specify action");
      }

      var storage = context.Services.GetService<IStatsStorage>()!;
      var action = args[0];

      switch (action)
      {
         case "list":
            yield return Result($"There are {storage.Stored.Count()} saved entries");
            yield break;
         case "clear":
            storage.Clear();
            yield return Result("Successfully cleared storage");
            yield break;
         case "save":
            if (args.Length < 2)
            {
               yield return Error("Please, specify filepath to save results");
               yield break;
            }

            var path = args[^1];
            HashSet<string> options = new (args[1..^1]);
            if (options.Contains("--excel"))
            {
               yield return Info("Sorry, currently excel export is not supported. It will be saved commonly"); // TODO
            }

            using (var file = File.OpenWrite(path))
            {
               JsonSerializerOptions jsonOptions = new() {WriteIndented = true};
               JsonSerializer.Serialize(file, storage.Stored, typeof(IEnumerable<ProcessStats>), jsonOptions); // TODO
            }

            yield return Result("Successfully saved results to file");
            yield break;
         default:
            yield return Error($"Cannot recognize action {action}");
            yield break;
      }
   }
}