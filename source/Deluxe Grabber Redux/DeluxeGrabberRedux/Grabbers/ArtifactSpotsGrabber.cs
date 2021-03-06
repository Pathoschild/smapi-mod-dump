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
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SObject = StardewValley.Object;

namespace DeluxeGrabberRedux.Grabbers
{
    class ArtifactSpotsGrabber : ObjectsMapGrabber
    {
        public ArtifactSpotsGrabber(ModEntry mod, GameLocation location) : base(mod, location)
        {
        }

        public override bool GrabObject(Vector2 tile, SObject obj)
        {
            if (Config.artifactSpots && obj.ParentSheetIndex == ItemIds.ArtifactSpot)
            {
                var artifacts = GetForagedArtifactsFromArtifactSpot(Location, tile);
                if (TryAddItems(artifacts))
                {
                    Location.Objects.Remove(tile);
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

        private List<SObject> GetForagedArtifactsFromArtifactSpot(GameLocation location, Vector2 tile)
        {
            // impl @ StardewValley::GameLocation::digUpArtifactSpot
            var artifacts = new List<SObject>();
            var r = new Random((int)tile.X * 2000 + (int)tile.Y + (int)Game1.uniqueIDForThisGame / 2 + (int)Game1.stats.DaysPlayed);
            var hoe = Player.getToolFromName(Tools.Hoe);
            var archaeologyEnchant = hoe != null && hoe is Hoe && hoe.hasEnchantmentOfType<ArchaeologistEnchantment>();
            var generousEnchant = hoe != null && hoe is Hoe && hoe.hasEnchantmentOfType<GenerousEnchantment>();
            var artifactId = -1;
            string[] split;
            foreach (var pair in Game1.objectInformation)
            {
                split = pair.Value.Split('/');
                if (split[3].Contains("Arch"))
                {
                    string[] archSplit = split[6].Split(' ');
                    for (int j = 0; j < archSplit.Length; j += 2)
                    {
                        if (archSplit[j].Equals(location.Name) && r.NextDouble() < (double)((!archaeologyEnchant) ? 1 : 2) * Convert.ToDouble(archSplit[j + 1], CultureInfo.InvariantCulture))
                        {
                            artifactId = pair.Key;
                            break;
                        }
                    }
                }
                if (artifactId != -1) break;
            }
            if (r.NextDouble() < 0.2 && !(location is Farm)) artifactId = ItemIds.LostBook;

            if (artifactId == ItemIds.LostBook && Game1.netWorldState.Value.LostBooksFound.Value >= 21)
            {
                artifactId = ItemIds.MixedSeeds;
            }

            if (artifactId != -1)
            {
                artifacts.Add(new SObject(artifactId, 1));
                return artifacts;
            }

            var season = Game1.GetSeasonForLocation(location);
            bool extraHarvest() => generousEnchant && r.NextDouble() < 0.5f;
            if (season == "winter" && r.NextDouble() < 0.5 && !(location is Desert))
            {
                artifacts.Add(new SObject(r.NextDouble() < 0.4 ? ItemIds.SnowYam : ItemIds.WinterRoot, extraHarvest() ? 2 : 1));
                return artifacts;
            }

            if (Game1.random.NextDouble() <= 0.25 && Player.team.SpecialOrderRuleActive("DROP_QI_BEANS"))
            {
                artifacts.Add(new SObject(ItemIds.QiBeans, r.Next(2, 6)));
            }

            if (season == "spring" && r.NextDouble() < 0.0625 && !(location is Desert) && !(location is Beach))
            {
                var extra = extraHarvest() ? r.Next(2, 6) : 0;
                artifacts.Add(new SObject(ItemIds.RiceShoot, extra + r.Next(2, 6)));
                return artifacts;
            }

            var hasGuntherQuest = Game1.MasterPlayer.mailReceived.Contains("guntherBones")
                || Player.team.specialOrders.Any(order => order.questKey.Value == "Gunther");
            if (Game1.random.NextDouble() <= 0.2 && hasGuntherQuest)
            {
                artifacts.Add(new SObject(ItemIds.GuntherBones, r.Next(2, 6)));
            }

            var locationData = Game1.content.Load<Dictionary<string, string>>("Data\\Locations");
            if (!locationData.ContainsKey(location.Name)) return null;
            var rawLocationData = locationData[location.Name].Split('/')[8].Split(' ');
            if (rawLocationData.Length == 0 || rawLocationData[0] == "-1") return null;

            for (int i = 0; i < rawLocationData.Length; i += 2)
            {
                if (!(r.NextDouble() <= Convert.ToDouble(rawLocationData[i + 1]))) continue;

                artifactId = Convert.ToInt32(rawLocationData[i]);
                if (Game1.objectInformation.ContainsKey(artifactId) && (Game1.objectInformation[artifactId].Split('/').Contains("Arch") || artifactId == ItemIds.LostBook))
                {
                    if (artifactId == ItemIds.LostBook && Game1.netWorldState.Value.LostBooksFound.Value >= 21) artifactId = ItemIds.MixedSeeds;
                    artifacts.Add(new SObject(artifactId, 1));
                    return artifacts;
                }
                if (artifactId == ItemIds.Clay && location.HasUnlockedAreaSecretNotes(Player) && Game1.random.NextDouble() < 0.11)
                {
                    var secretNote = location.tryToCreateUnseenSecretNote(Player);
                    if (secretNote != null)
                    {
                        artifacts.Add(secretNote);
                        return artifacts;
                    }
                }
                else if (artifactId == ItemIds.Clay && Game1.stats.daysPlayed > 28 && Game1.random.NextDouble() < 0.1)
                {
                    artifacts.Add(new SObject(ItemIds.FarmTotem + Game1.random.Next(3), 1));
                }
                var extra = extraHarvest() ? r.Next(1, 4) : 0;
                artifacts.Add(new SObject(artifactId, extra + r.Next(1, 4)));
                return artifacts;
            }
            return artifacts;
        }
    }
}
