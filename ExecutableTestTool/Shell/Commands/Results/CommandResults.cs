namespace ExecutableTestTool.Shell.Commands.Results;

public static class CommandResults
{
   public static CommandResult Error(string message)
   {
      return new CommandResult(c => c.Error(message));
   }

   public static CommandResult Result(string result)
   {
      return new CommandResult(c => c.Success(result));
   }
   
   public static CommandResult Info(string result)
   {
      return new CommandResult(c => c.Info(result));
   }
}