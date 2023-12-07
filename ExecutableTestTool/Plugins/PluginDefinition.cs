using ExecutableTestTool.Plugins.Abstractions;

namespace ExecutableTestTool.Plugins;

public interface IPluginDefinition
{
   public string Name { get; }
   public string Author { get; }
   public string Company { get; }
   public string Description { get; }
   public void Register(IServiceProvider services, ICommandsRegistry registry);
}