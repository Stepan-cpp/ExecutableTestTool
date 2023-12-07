using ExecutableTestTool.Plugins;
using ExecutableTestTool.Plugins.Abstractions;
using ExecutableTestTool.Plugins.Helpers;
using ExecutableTestTool.Plugins.Reflection;
using TestPlugin.Commands;

namespace TestPlugin;

[PluginDefinition]
public class PluginDefinition : IPluginDefinition
{
   public string Name => "MySamplePlugin";
   public string Author => "Stepan Byhovtsov";
   public string Company => "No company";
   public string Description  => "Sample plugin";
   
   public void Register(IServiceProvider services, ICommandsRegistry r)
   {
      r.RegisterCommandsFrom<SampleCommands>();
   }
}