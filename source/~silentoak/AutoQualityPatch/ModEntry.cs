/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/silentoak/StardewMods
**
*************************************************/

using SilentOak.AutoQualityPatch.Patches.AutomateCompat;
using SilentOak.AutoQualityPatch.Utils;
using SilentOak.Patching;
using SilentOak.QualityProducts;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace SilentOak.AutoQualityPatch
{
    internal class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            Util.Init(Helper, Monitor);

            Helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
        }

        private void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            IQualityProductsAPI qualityProductsAPI = Helper.ModRegistry.GetApi<IQualityProductsAPI>("SilentOak.QualityProducts");
            if (qualityProductsAPI == null)
            {
                Monitor.Log("Could not find Quality Products' API. This mod will be disabled.", LogLevel.Error);
                return;
            }

            RecipeManager.Init(qualityProductsAPI);

            PatchManager.Apply(typeof(MachineGenericPullRecipePatch));
        }
    }
}
