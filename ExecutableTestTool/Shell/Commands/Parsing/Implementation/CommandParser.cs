using System.Reflection.Metadata.Ecma335;
using System.Text;
using ExecutableTestTool.Shell.Commands.Datastructures;
using ExecutableTestTool.Shell.Commands.Parsing.Abstractions;

namespace ExecutableTestTool.Shell.Commands.Parsing.Implementation;

public class CommandParser : ICommandParser
{
   public CommandInvocation Parse(string str)
   {
      LinkedList<string> strings = new();

      StringBuilder word = new();
      bool isEscaped = false;
      bool isString = false;
      for (var i = 0; i < str.Length; i++)
      {
         if (isEscaped)
         {
            isEscaped = false;
            word.Append(str[i]);
            continue;
         }
         
         if (str[i] == '"')
         {
            isString = !isString;
            continue;
         }

         if (isString)
         {
            word.Append(str[i]);
            continue;
         }
         
         if (str[i] == '\\')
         {
            isEscaped = true;
            continue;
         }

         if (char.IsWhiteSpace(str[i]))
         {
            if (word.Length != 0)
            {
               strings.AddLast(word.ToString());
               word.Clear();
            }
            continue;
         }
         
         word.Append(str[i]);
      }

      if (word.Length != 0)
      {
         strings.AddLast(word.ToString());
      }

      if (strings.Count == 0)
         return new CommandInvocation() {Command = "", Arguments = Array.Empty<string>()};
      
      if (strings.Count == 1)
         return new CommandInvocation() {Command = strings.First!.Value, Arguments = Array.Empty<string>()};
      
      return new CommandInvocation {Command = strings.First!.Value.ToLower(), Arguments = strings.ToArray()[1..]};
   }
}