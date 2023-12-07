using ExecutableTestTool.Shell.Interface.Abstractions;
using ExecutableTestTool.Shell.Interface.Datastructures;

namespace ExecutableTestTool.Shell.Interface.Implementations;

internal class ConsoleUserInterface : IUserInterface
{
   private readonly ConsoleColor errorColor = ConsoleColor.Red;
   private readonly ConsoleColor successColor = ConsoleColor.Green;

   public void Info(string str, InfoType infoType = InfoType.ShellInfo)
   {
      Write(str + '\n');
   }

   public void Success(string str)
   {
      WriteColored(str + '\n', successColor);
   }

   public void Error(string str)
   {
      WriteColored(str + '\n', errorColor);
   }

   private void WriteColored(string str, ConsoleColor color)
   {
      var defaultColor = Console.ForegroundColor;
      Console.ForegroundColor = color;
      Write(str);
      Console.ForegroundColor = defaultColor;
   }
   
   private void Write(string str)
   {
      Console.Write(str);
   }


   public async Task<string> ReadInputAsync(CancellationToken ct)
   {
      Write(" < ");
      var result = await Console.In.ReadLineAsync(ct);
      return result ?? "";
   }
}