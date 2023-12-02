using ExecutableTestTool.Shell.Commands.Datastructures;

namespace ExecutableTestTool.Shell.Commands.Parsing.Abstractions;

public interface ICommandParser
{
   CommandInvocation Parse(string str);
}