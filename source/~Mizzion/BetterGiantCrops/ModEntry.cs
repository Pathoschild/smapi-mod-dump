/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace BetterGiantCrops
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += OnButtonPressed;
        }

        //Private methods
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;
            if (e.IsDown(SButton.NumPad5))
            {
                Monitor.Log("Started running mod.");
                TryNewGiantCrop(Game1.getFarm());
            }
        }

        //Custom Methods
        private void TryNewGiantCrop(GameLocation loc)
        {
            Monitor.Log($"There is {GetValidCrops().Count} items in the list.");
            foreach (Tuple<Vector2, Crop> cropTuple in GetValidCrops())
            {
                int xCoord = (int)cropTuple.Item1.X;
                int yCoord = (int) cropTuple.Item1.Y;
                Crop selCrop = cropTuple.Item2;
                bool doGiantCrop = true;
                Monitor.Log($"{selCrop.indexOfHarvest.Value} x: {xCoord}, y:{yCoord}");
                if (selCrop.currentPhase.Value == selCrop.phaseDays.Count - 1)
                {
                    for (int x = xCoord - 1; x <= xCoord + 1; x++)
                    {
                        for (int y = yCoord - 1; y <= yCoord + 1; y++)
                        {
                            Vector2 newLoc = new Vector2(x, y);

                            if (!loc.terrainFeatures.ContainsKey(newLoc) ||
                                !(loc.terrainFeatures[newLoc] is HoeDirt) ||
                                (loc.terrainFeatures[newLoc] as HoeDirt).crop == null ||
                                (loc.terrainFeatures[newLoc] as HoeDirt).crop.indexOfHarvest != selCrop.indexOfHarvest)
                            {
                                doGiantCrop = false;
                                break;
                            }
                        }
                        if (!doGiantCrop)
                            break;
                    }
                    if (!doGiantCrop)
                        continue;

                    for (int x = xCoord - 1; x <= xCoord + 1; x++)
                    {
                        for (int y = yCoord - 1; y <= yCoord + 1; y++)
                        {
                            Vector2 newLoc = new Vector2(x, y);
                            ((HoeDirt) loc.terrainFeatures[newLoc]).crop = null;
                        };
                    }
                    (loc as Farm)?.resourceClumps.Add(new DoGiantCrop(selCrop.indexOfHarvest.Value, new Vector2(xCoord - 1, yCoord + 1)));
                }
            }
        }

        //Get crops code taken from Cats Giant Crop Ring mod.
        private List<Tuple<Vector2, Crop>> GetValidCrops()
        {
            return Game1.locations.Where(gl => gl is Farm).SelectMany(gl => (gl as Farm)?.terrainFeatures.Pairs.Where(
                    tf =>
                        tf.Value is HoeDirt hd && hd.crop != null
                                               && hd.state.Value == 1).Select(hd =>
                    new Tuple<Vector2, Crop>(hd.Key, (hd.Value as HoeDirt)?.crop))
                .Where(c => !(c.Item2.dead.Value || !c.Item2.seasonsToGrowIn.Contains(Game1.currentSeason)))).ToList();
        }
    }
}
