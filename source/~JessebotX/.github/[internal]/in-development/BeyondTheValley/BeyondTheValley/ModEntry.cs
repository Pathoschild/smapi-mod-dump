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
using System.IO;

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
            /* Farm Shed */
            this.Helper.Content.Load<Map>("assets/Maps/FarmMaps/Farming_FarmShed.tbin", ContentSource.ModFolder);

            string FarmShedMapAssetKey = this.Helper.Content.GetActualAssetKey("assets/Maps/FarmMaps/Farming_FarmShed.tbin", ContentSource.ModFolder);

            GameLocation FarmShed = new GameLocation(FarmShedMapAssetKey, "FarmShed") { IsOutdoors = false, IsFarm = false };
            Game1.locations.Add(FarmShed);
            //-----------//
        }

        public bool CanLoad<T>(IAssetInfo asset)
        {
            //Standard Farm
            if (asset.AssetNameEquals("Maps/Farm"))
                return true;

            //Forest Farm
            else if (asset.AssetNameEquals("Maps/Farm_Foraging"))
                return true;

            //Cindersnap Forest
            else
                return asset.AssetNameEquals("Maps/Forest");
        }

        public T Load<T>(IAssetInfo asset)
        {
            //Standard Farm
            if (asset.AssetNameEquals("Maps/Farm"))
                return this.Helper.Content.Load<T>("assets/Maps/FarmMaps/Farm.tbin");

            else
                return this.Helper.Content.Load<T>("assets/Maps/FarmMaps/Farm_Foraging.tbin");
        }
    }
}