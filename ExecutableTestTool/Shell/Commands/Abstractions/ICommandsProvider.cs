namespace ExecutableTestTool.Shell.Commands.Abstractions;

public interface ICommandsProvider
{
   /// <summary>
   /// Key of dictionary is alias,
   /// Value is command description
   /// </summary>
   public IDictionary<string, ICommand> Commands { get; }
}