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
using Netcode;

namespace walkthroughtrellis
{
    //Entry
    public class ModEntry : Mod
    {
        //Documentation on Netcode seems to be far and few between. A lot of this was banging my head against a wall and seeing if the dent looked right.
        //In other words, if this looks crude to someone who actually knows Netcode and how it works, then I am sorry, but not really.
        NetInt[] cropKeys =
        {
            new NetInt(473),
            new NetInt(301),
            new NetInt(302)
            //473, 301, 302
        };
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
            foreach (HoeDirt dirt in Game1.currentLocation.terrainFeatures.Values.OfType<HoeDirt>().Where(dirt => dirt.crop != null).ToArray())
            {
               //obtaining the actual crop ID using netcode.
               NetInt isCorrectCrop = this.Helper.Reflection.GetField<NetInt>(dirt.crop, "netSeedIndex").GetValue();

                //This was for debugging:
                //string logMsg = "The crop is this value: " + isCorrectCrop;
                //this.Monitor.Log(logMsg, LogLevel.Debug);

                foreach (int value in cropKeys)
                {
                    if(isCorrectCrop == value)
                    {
                        dirt.crop.raisedSeeds.Value = false;
                    }
                }
            }
        }
    }
}
