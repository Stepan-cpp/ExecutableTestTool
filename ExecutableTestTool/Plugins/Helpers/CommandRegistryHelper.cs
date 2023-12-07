using System.Diagnostics.CodeAnalysis;
using ExecutableTestTool.Plugins.Abstractions;
using ExecutableTestTool.Shell.Commands.Commands;

namespace ExecutableTestTool.Plugins.Helpers;

public static class CommandRegistryHelper
{
   public static void RegisterCommandsFrom<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)] T>(this ICommandsRegistry r)
   {
      RegisterCommandsFrom(r, typeof(T));
   }
   
   public static void RegisterCommandsFrom(this ICommandsRegistry r, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)] Type type)
   {
      var commands = SingleClassCommandsProvider.GetCommandsFromClass(type);
      foreach (var command in commands)
      {
         r.Register(command);
      }
   }
}