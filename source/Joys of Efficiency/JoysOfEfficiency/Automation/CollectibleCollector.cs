/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/pomepome/JoysOfEfficiency
**
*************************************************/

using System;
using System.Collections.Generic;
using JoysOfEfficiency.Core;
using JoysOfEfficiency.Utils;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using SVObject = StardewValley.Object;

namespace JoysOfEfficiency.Automation
{
    internal class CollectibleCollector
    {
        private static Config Config => InstanceHolder.Config;
        private static readonly Logger Logger = new Logger("CollectibleCollector");

        public static void CollectNearbyCollectibles(GameLocation location)
        {
            int reach = Config.BalancedMode ? 1 : Config.AutoCollectRadius;
            foreach (SVObject obj in Util.GetObjectsWithin<SVObject>(reach))
                if (obj.IsSpawnedObject || obj.isAnimalProduct())
                    CollectObj(location, obj);

            Hoe hoe = Util.FindToolFromInventory<Hoe>(true);
            if (hoe == null) return;
            foreach (KeyValuePair<Vector2, HoeDirt> kv in Util.GetFeaturesWithin<HoeDirt>(reach))
                if (IsGinger(kv.Value.crop))
                    CollectGinger(location, kv.Key, kv.Value);
        }
        private static void CollectObj(GameLocation loc, SVObject obj)
        {
            Farmer who = Game1.player;

            Vector2 vector = Util.GetLocationOf(loc, obj);

            if ((int)vector.X == -1 && (int)vector.Y == -1) return;
            if (obj.questItem.Value) return;

            int quality = obj.Quality;
            Random random = new Random((int)Game1.uniqueIDForThisGame / 2 + (int)Game1.stats.DaysPlayed + (int)vector.X + (int)vector.Y * 777);

            if (who.professions.Contains(16) && obj.isForage(loc))
            {
                obj.Quality = 4;
            }

            else if (obj.isForage(loc))
            {
                if (random.NextDouble() < who.ForagingLevel / 30f)
                {
                    obj.Quality = 2;
                }
                else if (random.NextDouble() < who.ForagingLevel / 15f)
                {
                    obj.Quality = 1;
                }
            }

            if (who.couldInventoryAcceptThisItem(obj))
            {
                Logger.Log($"picked up {obj.DisplayName} at [{vector.X},{vector.Y}]");
                if (who.IsLocalPlayer)
                {
                    loc.localSound("pickUpItem");
                    DelayedAction.playSoundAfterDelay("coin", 300);
                }

                if (!who.isRidingHorse() && !who.ridingMineElevator)
                {
                    who.animateOnce(279 + who.FacingDirection);
                }

                if (!loc.isFarmBuildingInterior())
                {
                    if (obj.isForage(loc))
                    {
                        who.gainExperience(2, 7);
                    }
                }
                else
                {
                    who.gainExperience(0, 5);
                }

                who.addItemToInventoryBool(obj.getOne());
                Game1.stats.ItemsForaged++;
                if (who.professions.Contains(13) && random.NextDouble() < 0.2 && !obj.questItem.Value && who.couldInventoryAcceptThisItem(obj) && !loc.isFarmBuildingInterior())
                {
                    who.addItemToInventoryBool(obj.getOne());
                    who.gainExperience(2, 7);
                }
                loc.Objects.Remove(vector);
                return;
            }
            obj.Quality = quality;
        }

        private static bool IsGinger(Crop crop)
        {
            return crop != null && crop.forageCrop && crop.whichForageCrop == Crop.forageCrop_ginger;
        }

        private static void CollectGinger(GameLocation loc, Vector2 pos, HoeDirt dirt)
        {
            Farmer who = Game1.player;
            float stamina = 2 - who.FarmingLevel * 0.1f;
            if (who.Stamina < stamina)
            {
                return;
            }

            if (dirt.crop.hitWithHoe((int)pos.X, (int)pos.Y, loc,dirt))
            {
                who.Stamina -= stamina;
                dirt.destroyCrop(pos, true, loc);
            }
        }
    }
}
