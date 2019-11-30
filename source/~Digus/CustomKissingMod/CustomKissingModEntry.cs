using System.Linq;
using Harmony;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace CustomKissingMod
{
    public class CustomKissingModEntry : Mod
    {
        public const string MessageType = "Kissing";
        internal IModHelper ModHelper;
        internal IMonitor monitor;
        internal DataLoader DataLoader;
        public override void Entry(IModHelper helper)
        {
            ModHelper = helper;
            monitor = Monitor;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            DataLoader = new DataLoader(ModHelper);

            var harmony = HarmonyInstance.Create("Digus.CustomKissingMod");
            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), nameof(NPC.checkAction)),
                postfix: new HarmonyMethod(typeof(NPCOverrides), nameof(NPCOverrides.checkAction))
            );
        }
    }
}
