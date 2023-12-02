using ExecutableTestTool.Shell.Commands.Datastructures;
using ExecutableTestTool.Shell.Commands.Parsing.Abstractions;

namespace ExecutableTestTool.Shell.Commands.Parsing.Implementation;

public class CommandParser : ICommandParser
{
   public CommandInvocation Parse(string str)
   {
      var tokens = str.Split(' ');
      return new CommandInvocation {Command = tokens[0].ToLower(), Arguments = tokens[1..]};
   }
}