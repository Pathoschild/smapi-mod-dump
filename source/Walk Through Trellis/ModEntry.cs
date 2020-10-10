/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/OfficialTab/walk-through-trellis
**
*************************************************/

using System;
using System.Linq;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewModdingAPI;

namespace walkthroughtrellis
{
    //Entry
    public class ModEntry : Mod
    {
        //Entry Method
        public override void Entry(IModHelper helper)
        {
            //implementing the event
            helper.Events.Player.Warped += cropUpdate;
            helper.Events.Player.InventoryChanged += cropUpdate;
            //was current location and now that's different?
        }
        //location change event
        public void cropUpdate(object sender, EventArgs e)
        {
            /*Telling it for every HoeDirt type that is dirt in the locations gathered (Thanks PathosChild <3)
            with the given parameters, make the raised seed setting for the crops false. 
            */
            foreach (HoeDirt dirt in Game1.currentLocation.terrainFeatures.Values.OfType<HoeDirt>().Where(dirt => dirt.crop != null).ToArray())
            {
                dirt.crop.raisedSeeds.Value = false;
            }
        }
        //Decided directly editing the file using code was kinda stupid considering I had to edit the properties of existing crops live anyways.
    }
}
