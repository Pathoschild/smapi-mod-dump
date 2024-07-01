/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewDruid.Cast;
using StardewDruid.Event;
using StardewValley;
using StardewValley.Characters;
using StardewValley.GameData.Crops;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Timers;

namespace StardewDruid.Cast.Effect
{
    public class Harvest : EventHandle
    {

        public Dictionary<Vector2, HarvestTarget> harvesters = new();

        public Harvest()
        {

        }


        public virtual void AddTarget(GameLocation location, Vector2 tile)
        {

            if (harvesters.ContainsKey(tile))
            {
                return;
            }

            harvesters.Add(tile, new(location, tile));


            activeLimit = eventCounter + 8;


        }

        public override void EventDecimal()
        {

            // -------------------------------------------------
            // Crops
            // -------------------------------------------------

            for(int h = harvesters.Count - 1; h >= 0; h--)
            {

                KeyValuePair<Vector2, HarvestTarget> toHarvest = harvesters.ElementAt(h);

                if ((toHarvest.Value.counter <= 0))
                {

                    harvesters.Remove(toHarvest.Key);

                }

                toHarvest.Value.counter--;

                int targetRadius = toHarvest.Value.limit - toHarvest.Value.counter;

                List<Vector2> tileVectors = ModUtility.GetTilesWithinRadius(location, toHarvest.Value.tile, targetRadius);

                if(targetRadius == 1)
                {

                    tileVectors.Add(toHarvest.Value.tile);

                }

                List<HoeDirt> hoeDirts = new();

                foreach (Vector2 tileVector in tileVectors)
                {

                    if (toHarvest.Value.location.terrainFeatures.ContainsKey(tileVector))
                    {

                        if (toHarvest.Value.location.terrainFeatures[tileVector] is HoeDirt hoeDirt)
                        {

                            hoeDirts.Add(hoeDirt);

                        }

                    }

                    if (toHarvest.Value.location.objects.ContainsKey(tileVector))
                    {

                        if (toHarvest.Value.location.objects[tileVector] is IndoorPot indoorPot)
                        {

                            if(indoorPot.hoeDirt.Value != null)
                            {
                                hoeDirts.Add(indoorPot.hoeDirt.Value);

                            }

                            if(indoorPot.bush.Value != null)
                            {

                                if(indoorPot.bush.Value.size.Value == 3)
                                {
                                    
                                    if (indoorPot.bush.Value.shakeTimer == 0)
                                    {
                                        
                                        indoorPot.bush.Value.shake(tileVector, doEvenIfStillShaking: false);

                                    }

                                }

                            }

                        }

                    }

                }

                foreach(HoeDirt hoeDirt in hoeDirts)
                {

                    if (hoeDirt.crop != null)
                    {
                        if (
                            hoeDirt.crop.currentPhase.Value >= hoeDirt.crop.phaseDays.Count - 1 &&
                            (!hoeDirt.crop.fullyGrown.Value || hoeDirt.crop.dayOfCurrentPhase.Value <= 0)
                            && !hoeDirt.crop.dead.Value
                            && hoeDirt.crop.indexOfHarvest.Value != null)
                        {

                            List<StardewValley.Object> extracts = ModUtility.ExtractCrop(hoeDirt, hoeDirt.crop, hoeDirt.Tile);

                            foreach (StardewValley.Object extract in extracts)
                            {

                                ThrowHandle throwObject = new(hoeDirt.Tile * 64, toHarvest.Value.tile * 64, extract);

                                throwObject.pocket = true;

                                throwObject.register();

                            }

                        }

                    }

                }

            }

        }

    }

    public class HarvestTarget
    {

        public Vector2 tile;

        public GameLocation location;

        public int counter;

        public int limit;

        public HarvestTarget(GameLocation Location, Vector2 Tile)
        {

            tile = Tile;

            counter = 8;

            limit = 8;

            location = Location;

        }

    }

}
