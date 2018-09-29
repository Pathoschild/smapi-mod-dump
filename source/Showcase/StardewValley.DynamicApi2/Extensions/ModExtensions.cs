using System.Linq;
using Igorious.StardewValley.DynamicApi2.Utils;
using StardewModdingAPI;

namespace Igorious.StardewValley.DynamicApi2.Extensions
{
    public static class ModExtensions
    {
        public static void RegisterCommands(this Mod mod)
        {
            var commands = mod.GetType().Assembly.GetTypes()
                .Where(typeof(ConsoleCommand).IsAssignableFrom)
                .Select(t => new Constructor<IMonitor, ConsoleCommand>(t))
                .Select(c => c.Invoke(mod.Monitor));
            foreach (var command in commands)
            {
                command.Register();
            }
        }
    }
}