using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using ExecutableTestTool.Shell.Commands.Abstractions;
using ExecutableTestTool.Shell.Commands.Commands.ReflectionCommand;
using ExecutableTestTool.Shell.Commands.Results;

namespace ExecutableTestTool.Shell.Commands.Commands;

public class SingleClassCommandProvider : ICommandProvider
{
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)]
    public static readonly Type CommandsSource = typeof(Commands);

    public IDictionary<string, ICommand> Commands { get; }

   public SingleClassCommandProvider()
   {
      Commands = new Dictionary<string, ICommand>();
      AddCommandsFromClass(CommandsSource, Commands);
   }

   private static void AddCommandsFromClass([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)] Type type, IDictionary<string, ICommand> commands)
   {
      bool ParametersMatch(ParameterInfo[] p)
      {
         if (p.Length != 2)
            return false;
         if (p[0].ParameterType != typeof(ICommandExecutionContext))
            return false;
         if (p[1].ParameterType != typeof(string[]))
            return false;

         return true;
      }

      var methods = type.GetMethods().Where(c =>
         (c.ReturnType == typeof(IEnumerable<CommandResult>) /* || c.ReturnType == typeof(CommandResult)*/) &&
         ParametersMatch(c.GetParameters()));
      
      foreach (var method in methods)
      {
         var attr = method.GetCustomAttribute<CommandAttribute>();
         if (attr == null) 
            continue;
         
         var command = new ReflectionCommand.ReflectionCommand(method, attr);
         foreach (var alias in command.Aliases)
         {
            commands.Add(alias, command);
         }
      }
   }
}