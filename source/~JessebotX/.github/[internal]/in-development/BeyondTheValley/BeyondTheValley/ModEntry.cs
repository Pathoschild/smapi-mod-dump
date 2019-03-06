using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewModdingAPI;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Framework;
using StardewModdingAPI.Events;
using xTile;

namespace BeyondTheValley
{
    class ModEntry : Mod, IAssetLoader
    {
        public override void Entry(IModHelper helper)
        { 
            /* Helper Events */
            helper.Events.GameLoop.SaveLoaded += this.GameLoop_SaveLoaded;
        }

        private void GameLoop_SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            this.Helper.Content.Load<Map>("assets/Maps/FarmMaps/Farming_FarmShed.tbin", ContentSource.ModFolder);

            string FarmShedMapAssetKey = this.Helper.Content.GetActualAssetKey("assets/Maps/FarmMaps/Farming_FarmShed.tbin", ContentSource.ModFolder);

            GameLocation FarmShed = new GameLocation(FarmShedMapAssetKey, "FarmShed") { IsOutdoors = false, IsFarm = false };
            Game1.locations.Add(FarmShed);
        }

        public bool CanLoad<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("Maps/Farm");
        }

        public T Load<T>(IAssetInfo asset)
        {
            return this.Helper.Content.Load<T>("assets/Maps/FarmMaps/Farm.tbin");
        }
    }
}