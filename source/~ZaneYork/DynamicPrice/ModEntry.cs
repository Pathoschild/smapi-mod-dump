/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ZaneYork/SDV_Mods
**
*************************************************/

using DynamicPrice.Rewrites;
using Harmony;
using StardewModdingAPI;
using SObject = StardewValley.Object;

namespace DynamicPrice
{
    public class ModEntry : Mod
    {
        public static ModConfig ModConfig;
        public static IReflectionHelper reflection;
        public static CustomCropsDecay.IApi ccdApi;
        public override void Entry(IModHelper helper)
        {
            ModConfig = helper.ReadConfig<ModConfig>();
            if(ModConfig.ChangeRateMultiplier >= 1)
            {
                ModConfig.ChangeRateMultiplier = 1;
            }
            helper.WriteConfig(ModConfig);
            reflection = helper.Reflection;
            helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
            HarmonyInstance harmony = HarmonyInstance.Create("zaneyork.DynamicPrice");
            harmony.Patch(
                original: AccessTools.DeclaredMethod(typeof(SObject), "sellToStorePrice"),
                prefix: new HarmonyMethod(typeof(ObjectRewrites.SellToStorePriceRewrite), nameof(ObjectRewrites.SellToStorePriceRewrite.Prefix))
            );
        }

        private void GameLoop_GameLaunched(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        {
            ccdApi = this.Helper.ModRegistry.GetApi<CustomCropsDecay.IApi>("ZaneYork.CustomCropsDecay");
        }
    }
}
