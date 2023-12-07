using ExecutableTestTool.Shell.Commands.Abstractions;
using ExecutableTestTool.Shell.Commands.Commands.ReflectionCommand;
using ExecutableTestTool.Shell.Commands.Results;
using static ExecutableTestTool.Shell.Commands.Results.CommandResults;

namespace TestPlugin.Commands;

public class SampleCommands
{
   [Command(Name = "Hello", Aliases = new[]{"hello"})]
   public static CommandResult Hello(ICommandExecutionContext executionContext, string[] args)
   {
      return Result("Hello world!");
   }
   
   [Command(Name = "Knock-Knock", Aliases = new[]{"knock"})]
   public static IEnumerable<CommandResult> KnockKnock(ICommandExecutionContext executionContext, string[] args)
   {
      for (int i = 1; i <= 5; i++)
      {
         yield return Info($"Knock {i}!");
      }

      yield return Result("Are you alright?");
      var input = executionContext.Interface.ReadInput();
      if (input.ToLower().Contains("yes"))
      {
         yield return Result("Hooray!");
         yield break;
      }

      if (input.Trim().Length == 0)
      {
         yield return Error("Are you dead there?");
      }
      
      yield return Result("Calling 911...");
   }
}