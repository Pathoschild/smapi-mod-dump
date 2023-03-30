/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/FishingTrawler
**
*************************************************/

using FishingTrawler.Framework.Utilities;
using FishingTrawler.GameLocations;
using StardewValley;
using StardewValley.Tools;

namespace FishingTrawler.Framework.Objects.Items.Tools
{
    internal class BailingBucket
    {
        public float Scale { get; set; }
        public bool ContainsWater { get; set; }
        public bool IsValid { get; }

        private Tool _tool;

        public BailingBucket()
        {
            Scale = 0;
            ContainsWater = false;
            IsValid = true;
        }

        public BailingBucket(Tool tool)
        {
            if (tool is null || tool.modData.ContainsKey(ModDataKeys.BAILING_BUCKET_KEY) is false)
            {
                IsValid = false;
                return;
            }
            IsValid = true;

            Scale = 0;
            if (tool.modData.ContainsKey(ModDataKeys.BAILING_BUCKET_SCALE) is true && float.TryParse(tool.modData[ModDataKeys.BAILING_BUCKET_SCALE], out var scale))
            {
                Scale = scale;
            }

            ContainsWater = false;
            if (tool.modData.ContainsKey(ModDataKeys.BAILING_BUCKET_CONTAINS_WATER) is true && bool.TryParse(tool.modData[ModDataKeys.BAILING_BUCKET_CONTAINS_WATER], out var containsWater))
            {
                ContainsWater = containsWater;
            }

            // Set the tool
            _tool = tool;
        }

        public bool Use(GameLocation location, int x, int y, Farmer who)
        {
            if (FishingTrawler.IsPlayerOnTrawler() is false || who is null || (who is not null && Game1.player.Equals(who) is false))
            {
                who.forceCanMove();
                return false;
            }

            if (location is TrawlerHull trawlerHull)
            {
                if (ContainsWater)
                {
                    Game1.addHUDMessage(new HUDMessage(FishingTrawler.i18n.Get("game_message.bailing_bucket.empty_into_sea"), 3) { timeLeft = 1250f });
                }
                else if (trawlerHull.IsFlooding())
                {
                    ContainsWater = true;
                    Scale = 0.5f;

                    trawlerHull.ChangeWaterLevel(-5);
                    trawlerHull.localSound("slosh");
                    FishingTrawler.SyncTrawler(Messages.SyncType.WaterLevel, trawlerHull.GetWaterLevel(), FishingTrawler.GetFarmersOnTrawler());
                }
                else
                {
                    Game1.addHUDMessage(new HUDMessage(FishingTrawler.i18n.Get("game_message.bailing_bucket.no_water_to_bail"), 3) { timeLeft = 1250f });
                }
            }
            else if (location is TrawlerSurface trawlerSurface && ContainsWater)
            {
                if (trawlerSurface.IsPlayerByBoatEdge(who))
                {
                    ContainsWater = false;
                    Scale = 0.5f;

                    who.currentLocation.localSound("waterSlosh");
                }
                else
                {
                    Game1.addHUDMessage(new HUDMessage(FishingTrawler.i18n.Get("game_message.bailing_bucket.stand_closer_to_edge"), 3) { timeLeft = 1250f });
                }
            }
            else
            {
                Game1.addHUDMessage(new HUDMessage(FishingTrawler.i18n.Get("game_message.bailing_bucket.bail_from_hull"), 3) { timeLeft = 1250f });
            }
            SaveData();

            who.CanMove = true;
            who.UsingTool = false;
            return false;
        }

        public void SaveData()
        {
            if (IsValid is false || _tool is null)
            {
                return;
            }

            _tool.modData[ModDataKeys.BAILING_BUCKET_SCALE] = Scale.ToString();
            _tool.modData[ModDataKeys.BAILING_BUCKET_CONTAINS_WATER] = ContainsWater.ToString();
        }

        public static GenericTool CreateInstance()
        {
            var bucket = new GenericTool(string.Empty, string.Empty, -1, 6, 6);
            bucket.modData[ModDataKeys.BAILING_BUCKET_KEY] = true.ToString();

            return bucket;
        }
    }
}
