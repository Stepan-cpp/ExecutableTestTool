using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json;
using ExecutableTestTool.Libs;
using ExecutableTestTool.Plugins;
using ExecutableTestTool.Plugins.Abstractions;
using ExecutableTestTool.Plugins.Reflection;
using ExecutableTestTool.ProcessTracking.Abstractions;
using ExecutableTestTool.ProcessTracking.Datastructures;
using ExecutableTestTool.Shell.Commands.Abstractions;
using ExecutableTestTool.Shell.Commands.Commands.ReflectionCommand;
using ExecutableTestTool.Shell.Commands.Results;
using ExecutableTestTool.Shell.Services.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using OfficeOpenXml.ConditionalFormatting.Contracts;
using static ExecutableTestTool.Shell.Commands.Results.CommandResults;

namespace ExecutableTestTool.Shell.Commands.Commands;

internal class Commands
{
   [Command(Name = "Help",
      Aliases = new[] {"help", "?"},
      Usage = "help [<commandName>]",
      HelpDescription =
         "Shows list of commands, or if the commandName attribute is specified, shows manual on command")]
   public static IEnumerable<CommandResult> Help(ICommandExecutionContext context, string[] args)
   {
      var commandProvider = context.Services.GetService<ICommandsProvider>()!;
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
         return
            $"Usage: \n{command.Usage}\nAliases:\n{command.Aliases.Aggregate((p, n) => p + ", " + n)}\nDescription:\n{command.HelpDescription}\n";
      }
   }

   [Command(Name = "Execute",
      Aliases = new[] {"exec", "exe", "run", "test", "execute"},
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

      yield return Result($"Running time: {(long) ps.Runtime.TotalSeconds}s {ps.Runtime.Milliseconds}ms");
      yield return Result($"Peak memory usage: {readableMem.scale:F}{readableMem.unit.ToString()}");

      var statsStorage = context.Services.GetService<IStatsStorage>();
      Debug.Assert(statsStorage != null, "statsStorage == null");
      statsStorage!.Add(ps);
      yield return Result($"Successfully added test results into stats storage");
   }

   [Command(Name = "storage",
      Aliases = new[] {"storage"},
      Usage = "storage {list/clear/save}",
      HelpDescription = "Manipulates stats storage")]
   public static CommandResult Storage(ICommandExecutionContext context, string[] args)
   {
      if (args.Length == 0)
      {
         return Error("Please, specify action");
      }

      var storage = context.Services.GetService<IStatsStorage>()!;
      var action = args[0];

      switch (action)
      {
         case "list":
            return Result($"There are {storage.Stored.Count()} saved entries");
         case "clear":
            storage.Clear();
            return Result("Successfully cleared storage");
         case "save":
            if (args.Length < 2)
            {
               return Error("Please, specify filepath to save results");
            }

            var path = args[^1];

            HashSet<string> options = new(args[1..^1]);
            var excelWritingService = context.Services.GetService<IExcelWriter>();
            if (excelWritingService == null)
            { 
               return Error("Sorry, currently excel writing service is unavailable");
            }

            bool success = true;
            var message = "";
            try
            {
               excelWritingService.WriteToFileAsync(path, storage.Stored);
            }
            catch (Exception ex)
            {
               success = false;
               message = ex.ToString();
            }

            if (!success)
            {
               return Error(message);
            }
            break;
         default:
            return Error($"Cannot recognize action {action}");
      }

      return Result("Successfully saved results to file");
   }
   
   [Command(Name = "pm",
      Aliases = new[] {"pm", "plugin"},
      Usage = "pm <assemblyPath>",
      HelpDescription = "Manipulates plugins")]
    [RequiresUnreferencedCode("Calls System.Reflection.Assembly.LoadFrom(String)")]
    public static CommandResult Plugins(ICommandExecutionContext context, string[] args)
   {
      if (args.Length != 1)
         return Error("Please, specify path");

      var path = args[0];
      var fileInfo = new FileInfo(path);
      if (!fileInfo.Exists)
         return Error("Provided file does not exist");

      var assembly = Assembly.LoadFrom(fileInfo.FullName)!;
      Debug.Assert(assembly != null, "assembly != null");
      var pluginDefType =
         assembly.GetTypes()
            .First(a => a.CustomAttributes.Any(b => b.AttributeType == typeof(PluginDefinitionAttribute)));
      var pluginDefCtor = pluginDefType.GetConstructor(Type.EmptyTypes)!;
      Debug.Assert(pluginDefCtor != null, "pluginDefCtor != null");
      var pluginDefinition = (IPluginDefinition) pluginDefCtor.Invoke(Array.Empty<object>());
      pluginDefinition.Register(context.Services, context.Services.GetService<ICommandsRegistry>()!);
      Debug.Assert(pluginDefinition != null, "pluginDefinition != null");
      return Result("Success");
   }
}