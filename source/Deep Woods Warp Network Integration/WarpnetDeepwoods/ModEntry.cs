/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/WarpnetDeepwoods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Buildings;

namespace WarpnetDeepwoods
{
    public class ModEntry : Mod, IAssetLoader
    {
        internal static Config Config;
        private static IWarpNetAPI WarpApi = null;
        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<Config>();
            helper.Events.GameLoop.GameLaunched += SetupIntegrations;
        }
        private void SetupIntegrations(object sender, GameLaunchedEventArgs ev)
        {
            Config.RegisterModConfigMenu(Helper, ModManifest);
            WarpApi = Helper.ModRegistry.GetApi<IWarpNetAPI>("tlitookilakin.warpnetwork");
            WarpApi.AddCustomDestinationHandler("deepwoods", CanWarpToDeepWoods, GetWarpLabel, () => "Deepwoods", WarpToDeepWoods);
        }
        private string GetWarpLabel()
        {
            return Helper.Translation.Get("ui-label");
        }
        private static void WarpToDeepWoods()
        {
            Game1.exitActiveMenu();
            Game1.activeClickableMenu = new DeepWoodsMod.WoodsObeliskMenu();
        }
        private static bool CanWarpToDeepWoods()
        {
            if (!Config.AfterObelisk)
            {
                return true;
            }
            Farm farm = Game1.getFarm();
            if(farm is null)
            {
                return false;
            }
            foreach(Building building in farm.buildings)
            {
                if(building.buildingType.ToString() == "Woods Obelisk")
                {
                    return true;
                }
            }
            return false;
        }

        public bool CanLoad<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("Data/WarpNetwork/Icons/Deepwoods");
        }
        public T Load<T>(IAssetInfo asset)
        {
            return Helper.Content.Load<T>(PathUtilities.NormalizePath("assets/icon.png"));
        }
    }
}
