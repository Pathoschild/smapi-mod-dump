using System.Reflection;
using Harmony;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace MoveFaster
{
    public class Main : Mod
    {
        internal static Config Config { get; private set; }

        public override void Entry(IModHelper helper)
        {
            base.Entry(helper);

            Config = helper.ReadConfig<Config>();
            helper.WriteConfig(Config);
            Monitor.Log(Config.FasterSpeed.ToString());

            SaveEvents.AfterLoad += this.SaveEvents_AfterLoad;
        }

        private void SaveEvents_AfterLoad(object sender, System.EventArgs e)
        {
            var originSpeed = StardewValley.Game1.player.getMovementSpeed();

            var harmony = HarmonyInstance.Create("io.github.hcoona.StardrewValleyMods.MoveFasterMod");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            var patchedSpeed = StardewValley.Game1.player.getMovementSpeed();

            Monitor.Log($"Player's movement speed raised from {originSpeed} to {patchedSpeed}");
        }
    }

    [HarmonyPatch(typeof(global::StardewValley.Farmer), "getMovementSpeed")]
    internal class PatchFarmerGetMovementSpeed
    {
        internal static void Postfix(ref float __result)
        {
            __result += Main.Config.FasterSpeed;
        }
    }
}
