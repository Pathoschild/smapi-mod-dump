using Harmony;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Objects;

namespace CustomCaskMod
{
    public class CustomCaskModEntry : Mod
    {
        internal IMonitor ModMonitor { get; set; }

        public override void Entry(IModHelper helper)
        {
            ModMonitor = Monitor;
            new DataLoader(helper);

            var harmony = HarmonyInstance.Create("Digus.CustomCaskMod");

            var caskPerformObjectDropInAction = typeof(Cask).GetMethod("performObjectDropInAction");
            var caskOverridesPerformObjectDropInAction = typeof(CaskOverrides).GetMethod("PerformObjectDropInAction");
            harmony.Patch(caskPerformObjectDropInAction, new HarmonyMethod(caskOverridesPerformObjectDropInAction), null);
        }
    }
}
