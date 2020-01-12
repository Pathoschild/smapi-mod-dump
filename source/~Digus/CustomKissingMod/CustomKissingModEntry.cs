using System;
using System.Linq;
using CustomKissingMod.Api;
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
        internal static IMonitor ModMonitor;
        internal DataLoader DataLoader;
        public override void Entry(IModHelper helper)
        {
            ModHelper = helper;
            ModMonitor = Monitor;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            DataLoader = new DataLoader(ModHelper);
            DataLoader.LoadContentPacks();

            try
            {
                var harmony = HarmonyInstance.Create("Digus.CustomKissingMod");
                harmony.Patch(
                    original: AccessTools.Method(typeof(NPC), nameof(NPC.checkAction)),
                    postfix: new HarmonyMethod(typeof(NPCOverrides), nameof(NPCOverrides.checkAction))
                );
            }
            catch (Exception ex)
            {
                Monitor.Log("Error while trying to apply harmony patch. This mod won't work.",LogLevel.Error);
                Monitor.Log($"{ex.Message}\n{ex.StackTrace}");
            }
        }

        public override object GetApi()
        {
            return new CustomKissingModApi();
        }
    }
}
