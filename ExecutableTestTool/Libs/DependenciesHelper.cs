using ExecutableTestTool.Plugins.Abstractions;
using ExecutableTestTool.Plugins.Implementations;
using ExecutableTestTool.ProcessTracking.Abstractions;
using ExecutableTestTool.ProcessTracking.Implementations;
using ExecutableTestTool.Shell.Commands.Abstractions;
using ExecutableTestTool.Shell.Commands.Commands;
using ExecutableTestTool.Shell.Commands.Parsing.Abstractions;
using ExecutableTestTool.Shell.Commands.Parsing.Implementation;
using ExecutableTestTool.Shell.Interface.Abstractions;
using ExecutableTestTool.Shell.Interface.Implementations;
using ExecutableTestTool.Shell.Services.Abstractions;
using ExecutableTestTool.Shell.Services.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace ExecutableTestTool.Libs;

internal static class DependenciesHelper
{
   public static IServiceCollection AddShell(this IServiceCollection services)
   {
      services.AddSingleton<IUserInterface, ConsoleUserInterface>();
      services.AddSingleton<ICommandsRegistry, CommandsRegistry>();
      services.AddSingleton<ICommandParser, CommandParser>();
      services.AddSingleton<ICommandsProvider, SingleClassCommandsProvider>();
      services.AddSingleton<IStatsStorage, MemoryStatsStorage>();
      services.AddSingleton<IExcelWriter, EpPlusExcelWriter>();
      services.AddTransient<IProcessTracker, ProcessTracker>();
      return services;
   }
   
   public static Shell.Shell BuildShell(this IServiceCollection services)
   {
      services.AddSingleton<Shell.Shell>();
      var serviceProvider = services.BuildServiceProvider();
      var shell = serviceProvider.GetService<Shell.Shell>()!;
      shell.ServiceProvider = serviceProvider;
      return shell;
   }
} 