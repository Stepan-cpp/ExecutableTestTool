using ExecutableTestTool.Shell.Commands.Abstractions;
using ExecutableTestTool.Shell.Interface.Abstractions;

namespace ExecutableTestTool.Shell;

public class ShellCommandExecutionContext : ICommandExecutionContext
{
   public ShellCommandExecutionContext(IServiceProvider services, IUserInterface @interface)
   {
      Services = services;
      Interface = @interface;
   }

   public IServiceProvider Services { get; }
   public IUserInterface Interface { get; }
}