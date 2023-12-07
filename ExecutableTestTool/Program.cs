using ExecutableTestTool.Libs;
using Microsoft.Extensions.DependencyInjection;

ServiceCollection services = new ServiceCollection();
services.AddShell();

var shell = services.BuildShell();
using var cts = new CancellationTokenSource();

await shell.RunAsync(cts.Token);

cts.Cancel();