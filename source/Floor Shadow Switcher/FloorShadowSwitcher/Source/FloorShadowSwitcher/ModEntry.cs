using System.Reflection;
using StardewModdingAPI;
using StardewValley;
using Harmony;

namespace FloorShadowSwitcher
{
    public class ModEntry : Mod
    {
        internal static ModEntry Instance;
        internal bool[] Enabled = new bool[10];
        public override void Entry(IModHelper helper)
        {
            ModEntry.Instance = this;
            ModConfig Config = helper.ReadConfig<ModConfig>();

            // There are probably better ways to do this, but I really want array-style access along with nice config names
            this.Enabled[0] = Config.EnableShadowsForType_0_WoodFloor;
            this.Enabled[1] = Config.EnableShadowsForType_1_StoneFloor;
            this.Enabled[2] = Config.EnableShadowsForType_2_WeatheredFloor;
            this.Enabled[3] = Config.EnableShadowsForType_3_CrystalFloor;
            this.Enabled[4] = Config.EnableShadowsForType_4_StrawFloor;
            this.Enabled[5] = Config.EnableShadowsForType_5_GravelPath;
            this.Enabled[6] = Config.EnableShadowsForType_6_WoodPath;
            this.Enabled[7] = Config.EnableShadowsForType_7_CrystalPath;
            this.Enabled[8] = Config.EnableShadowsForType_8_CobblestonePath;
            this.Enabled[9] = Config.EnableShadowsForType_9_SteppingStonePath;

            HarmonyInstance harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

    }
}