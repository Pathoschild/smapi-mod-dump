/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using xTile;

namespace MapExampleRF1
{
    public class Class1 : Mod
    {
        public override void Entry(IModHelper helper)
        {
            // the game clears locations when loading the save, so do it after the save loads
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs args)
        {
            // load the map file from your mod folder
            this.Helper.Content.Load<Map>("assets/RF1Farm.tbin", ContentSource.ModFolder);

            // get the internal asset key for the map file
            string mapAssetKey = this.Helper.Content.GetActualAssetKey("assets/RF1Farm.tbin", ContentSource.ModFolder);

            // add the new location
            GameLocation location = new GameLocation(mapAssetKey, "RF1Farm") { IsOutdoors = false, IsFarm = false };
            Game1.locations.Add(location);
        }
    }
}
