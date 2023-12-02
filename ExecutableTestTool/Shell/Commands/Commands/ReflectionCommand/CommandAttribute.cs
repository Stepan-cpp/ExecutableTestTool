using System.ComponentModel;
using ExecutableTestTool.Shell.Commands.Abstractions;

namespace ExecutableTestTool.Shell.Commands.Commands.ReflectionCommand;

[AttributeUsage(AttributeTargets.Method)]
public class CommandAttribute : Attribute
{
   public string Name { get; set; } = "";
   public string HelpDescription { get; set; } = "";
   public string[] Aliases { get; set; } = Array.Empty<string>();
   public string Usage { get; set; } = "";
}