using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace walkthroughtrellis
{
    //Entry
    public class ModEntry : Mod, IAssetEditor
    {
        //Entry Method
        public override void Entry(IModHelper helper)
        {
            //implementing the event
            PlayerEvents.Warped += PlayerEvents_Warped;
            //was current location and now that's different?
        }
        //location change event
        public void PlayerEvents_Warped(object sender, EventArgs e)
        {
            /*Telling it for every HoeDirt type that is dirt in the locations gathered (Thanks PathosChild <3)
            with the given parameters, make the raised seed setting for the crops false. 
            */
            foreach (HoeDirt dirt in Game1.currentLocation.terrainFeatures.Values.OfType<HoeDirt>().Where(dirt => dirt.crop != null).ToArray())
            {
                dirt.crop.raisedSeeds.Value = false;
                //was dirt.crop.raisedSeeds?
            }
        }
        //Telling smapi that we can edit the assetinfo of the crops XNB in the data folder of SDV
        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals(@"Data\Crops");
        }
        //Telling it what we want to actually edit in SDV's crops XNB and how exactly we want to do that.
        public void Edit<T>(IAssetData asset)
        {
            asset
            .AsDictionary<int, string>()
            .Set((id, data) =>
            {
                //splits the fields in the crops XNB via the '/' which is how values are seperated in the xnb
                string[] fields = data.Split('/');
                //setting the field for raised seeds to false (This is for all crops, maybe in the future I'll be more explicit)
                fields[7] = "false";
                //stitching the fields in the XNB file back together for use.
                return string.Join("/", fields);
            });
        }
    }
}
