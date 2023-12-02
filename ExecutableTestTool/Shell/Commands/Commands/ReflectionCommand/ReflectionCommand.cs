using System.Net.Mime;
using System.Reflection;
using ExecutableTestTool.Shell.Commands.Abstractions;
using ExecutableTestTool.Shell.Commands.Results;

namespace ExecutableTestTool.Shell.Commands.Commands.ReflectionCommand;

public class ReflectionCommand : ICommand
{
   public string Name { get; }
   
   public string HelpDescription { get; }
   
   public IEnumerable<string> Aliases { get; }
   
   public string Usage { get; }

   private delegate IEnumerable<CommandResult> Command(ICommandExecutionContext context, string[] args);
   
   private Command CommandAction { get; }
   
   public IEnumerable<CommandResult> Invoke(ICommandExecutionContext context, string[] args)
   {
      return CommandAction.Invoke(context, args);
   }

   public ReflectionCommand(MethodInfo method, CommandAttribute attribute)
   {
      CommandAction = method.CreateDelegate<Command>();
      Name = attribute.Name;
      HelpDescription = attribute.HelpDescription;
      Aliases = attribute.Aliases;
      Usage = attribute.Usage;
   }
}