/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;
using SObject = StardewValley.Object;

namespace BetterMeteorites
{
    internal class Mod : StardewModdingAPI.Mod
    {
        public static Mod Instance;

        public override void Entry(IModHelper helper)
        {
            Mod.Instance = this;
            SpaceShared.Log.Monitor = this.Monitor;

            helper.Events.GameLoop.SaveCreated += this.OnSaveCreated;
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
        }

        private void OnSaveCreated(object sender, SaveCreatedEventArgs e)
        {
            Game1.getFarm().resourceClumps.OnValueRemoved += this.OnClumpRemoved;
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            Game1.getFarm().resourceClumps.OnValueRemoved += this.OnClumpRemoved;
        }

        private void OnClumpRemoved(ResourceClump value)
        {
            if (value.parentSheetIndex.Value == ResourceClump.meteoriteIndex)
            {
                Random r = new Random((int)value.tile.X * 1000 + (int)value.tile.Y);
                Game1.createMultipleObjectDebris(SObject.stone, (int)value.tile.X, (int)value.tile.Y, 75 + r.Next(175));
                Game1.createMultipleObjectDebris(SObject.coal, (int)value.tile.X, (int)value.tile.Y, 20 + r.Next(55));
                Game1.createMultipleObjectDebris(SObject.iridium, (int)value.tile.X, (int)value.tile.Y, 50 + r.Next(100));
                Game1.createMultipleObjectDebris(535, (int)value.tile.X, (int)value.tile.Y, 7 + r.Next(15));
                Game1.createMultipleObjectDebris(536, (int)value.tile.X, (int)value.tile.Y, 7 + r.Next(15));
                Game1.createMultipleObjectDebris(537, (int)value.tile.X, (int)value.tile.Y, 7 + r.Next(15));
                Game1.createMultipleObjectDebris(749, (int)value.tile.X, (int)value.tile.Y, 3 + r.Next(9));
            }
        }
    }
}
