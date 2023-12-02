using ExecutableTestTool.ProcessTracking.Abstractions;
using ExecutableTestTool.ProcessTracking.Implementations;
using ExecutableTestTool.Shell;
using ExecutableTestTool.Shell.Commands.Abstractions;
using ExecutableTestTool.Shell.Commands.Commands;
using ExecutableTestTool.Shell.Commands.Parsing.Abstractions;
using ExecutableTestTool.Shell.Commands.Parsing.Implementation;
using ExecutableTestTool.Shell.Interface.Abstractions;
using ExecutableTestTool.Shell.Interface.Implementations;
using Microsoft.Extensions.DependencyInjection;

ServiceCollection services = new ServiceCollection();

services.AddSingleton<IUserInterface, ConsoleUserInterface>();
services.AddSingleton<ICommandParser, CommandParser>();
services.AddSingleton<ICommandProvider, SingleClassCommandProvider>();
services.AddSingleton<IStatsStorage, MemoryStatsStorage>();
services.AddTransient<IProcessTracker, ProcessTracker>();

services.AddSingleton<Shell>();

var serviceProvider = services.BuildServiceProvider();

var shell = serviceProvider.GetService<Shell>()!;

shell.ServiceProvider = serviceProvider;

using var cts = new CancellationTokenSource();
await shell.RunAsync(cts.Token);
cts.Cancel();