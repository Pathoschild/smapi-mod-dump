/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ferdaber/sdv-mods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SObject = StardewValley.Object;

namespace DeluxeGrabberRedux.Grabbers
{
    class BerryBushGrabber : TerrainFeaturesMapGrabber
    {
        private readonly Func<SObject, Vector2, GameLocation, KeyValuePair<SObject, int>> GetBerryBushHarvest;

        public BerryBushGrabber(ModEntry mod, GameLocation location) : base(mod, location)
        {
            GetBerryBushHarvest = Mod.Api.GetBerryBushHarvest ?? DefaultGetBerryBushHarvest;
        }

        private KeyValuePair<SObject, int> DefaultGetBerryBushHarvest(SObject berry, Vector2 bushTile, GameLocation location)
        {
            if (berry.ParentSheetIndex == ItemIds.TeaLeaves)
            {
                berry.Quality = SObject.lowQuality;
            }
            else
            {
                berry.Quality = Player.professions.Contains(Farmer.botanist) ? SObject.bestQuality : SObject.lowQuality;
            }
            return new KeyValuePair<SObject, int>(berry, 0);
        }

        public override bool GrabFeature(Vector2 tile, TerrainFeature feature)
        {
            if (Config.bushes && IsForageableBush(feature, out Bush bush))
            {
                // impl @ StardewValley::Bush::shake
                List<SObject> items = new List<SObject>();
                var season = (bush.overrideSeason.Value == -1) ? Game1.GetSeasonForLocation(Location) : Utility.getSeasonNameFromNumber(bush.overrideSeason.Value);
                int shakeOff = -1;
                int expGain = 0;
                Random r = new Random((int)tile.X + (int)tile.Y * 5000 + (int)Game1.uniqueIDForThisGame + (int)Game1.stats.DaysPlayed);

                if (season == "spring") shakeOff = ItemIds.Salmonberry;
                else if (season == "fall") shakeOff = ItemIds.Blackberry;
                if (bush.size.Value == 3) shakeOff = ItemIds.TeaLeaves;
                else if (bush.size.Value == 4) shakeOff = ItemIds.GoldenWalnut;

                if (shakeOff != -1 && shakeOff != ItemIds.GoldenWalnut)
                {
                    if (bush.size.Value == 3 || bush.size.Value == 4) // Tea Leaves or Golden Walnut bush
                    {
                        var pair = GetBerryBushHarvest(new SObject(shakeOff, 1), tile, Location);
                        items.Add(pair.Key);
                        expGain = pair.Value;
                    }
                    else // berry bush
                    {
                        var numShakes = r.Next(1, 2) + Player.ForagingLevel / 4;
                        for (int i = 0; i < numShakes; i++)
                        {
                            var pair = GetBerryBushHarvest(new SObject(shakeOff, 1), tile, Location);
                            items.Add(pair.Key);
                            if (i == 0) expGain = pair.Value;
                        }
                    }
                }

                if (TryAddItems(items))
                {
                    bush.tileSheetOffset.Value = 0;
                    bush.setUpSourceRect();
                    GainExperience(Skills.Foraging, expGain);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private bool IsForageableBush(TerrainFeature feature, out Bush bush)
        {
            // impl @ StardewValley::Bush::shake
            bush = null;
            if (feature is Bush outBush)
            {
                bush = outBush;
                return !bush.townBush.Value && bush.tileSheetOffset.Value == 1 && bush.inBloom(Game1.GetSeasonForLocation(Location), Game1.dayOfMonth);
            }
            return false;
        }
    }
}
