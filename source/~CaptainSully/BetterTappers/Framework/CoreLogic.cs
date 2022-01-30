/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/CaptainSully/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley.TerrainFeatures;

namespace BetterTappers
{
    internal class CoreLogic
    {
        private static readonly Log log = ModEntry.Instance.log;
        private static readonly ModConfig Config = ModEntry.Config;
        public const int LvlCap = 100;
        public const int formula = 0;

        /// <summary>Return number of minutes the tapper should take to produce.</summary>
		internal static int CalculateTapperMinutes(int treeType, int parentSheetIndex)
        {
            if (Config.DisableAllModEffects || !Config.ChangeTapperTimes)
            {
                return 0;
            }
            log.D("Calculating modded tapper minutes...", Config.DebugMode);

            float days_configured = 1f;
            float time_multiplier = 1f;
            int result;

            if (parentSheetIndex == 264)
            {
                time_multiplier = Config.HeavyTapperMultiplier;
            }
            log.D("Time multiplier: " + time_multiplier, Config.DebugMode);

            switch (treeType)
            {
                case 1:
                case 2:
                case 3:
                    days_configured = Config.DaysForSyrups;
                    break;
                case 7:
                    days_configured = Config.DaysForMushroom;
                    break;
                case 8:
                    days_configured = Config.DaysForSap;
                    break;
            }

            days_configured *= time_multiplier;
            log.D("Days calculated: " + days_configured, Config.DebugMode);
            if (days_configured < 1)
            {
                result = (int)MathHelper.Max(1440 * days_configured, 5);
            }
            else
            {
                result = Utility.CalculateMinutesUntilMorning(Game1.timeOfDay, (int)Math.Max(1f, days_configured));
            }
            log.D("Changing minutes until ready as per configs: " + result, Config.DebugMode);
            return result;
        }

        /// <summary>Return a quality level for tapper output.</summary>
		internal static int GetQualityLevel(Farmer who, int age, int timesHarvested)
        {
            log.D("Quality check requested...", Config.DebugMode);
            if (Config.DisableAllModEffects || !Config.TappersUseQuality)
            {
                log.D("Quality disabled", Config.DebugMode);
                return SObject.lowQuality;
            }
            if ((!Config.ForageLevelAffectsQuality || who is null) && (!Config.TreeAgeAffectsQuality || age < 1) &&
                (!Config.TimesHarvestedAffectsQuality || timesHarvested < 1))
            {
                log.D("Quality all types disabled", Config.DebugMode);
                return SObject.lowQuality;
            }

            if (Config.BotanistAffectsTappers)
            {
                if (who is not null && who.professions.Contains(Farmer.botanist))
                {
                    log.D("Botanist perk applied.", Config.DebugMode);
                    return SObject.bestQuality;
                }
            }
            int quality = DetermineQuality(who.foragingLevel.Value, age, timesHarvested);
            if (quality == 3)
            {
                return SObject.highQuality;
            }
            return quality;
        }

        /// <summary>Return a quality level based on what categories are enabled.</summary>
		private static int DetermineQuality(int foragingLevel, int age, int timesHarvested)
        {
            log.D("Determining quality...", Config.DebugMode);
            int n, FLQ, TAQ, THQ, t;
            n = FLQ = TAQ = THQ = 0;

            if (Config.ForageLevelAffectsQuality)
            {
                FLQ = GetQualityPart(foragingLevel);
                n++;
            }
            if (Config.TreeAgeAffectsQuality)
            {
                TAQ = GetQualityPart(age);
                n++;
            }
            if (Config.TimesHarvestedAffectsQuality)
            {
                THQ = GetQualityPart(timesHarvested);
                n++;
            }

            log.D("QualitiesActive: " + n + "    FLQ: " + FLQ + "    TAQ: " + TAQ + "    THQ: " + THQ, Config.DebugMode);
            t = (FLQ + TAQ + THQ);
            log.D("Sum of qualty pieces: " + t, Config.DebugMode);
            switch (n)
            {
                case 3:
                    if (t == 6)
                    {
                        return SObject.bestQuality;
                    }
                    return (int)Math.Floor(t * 0.5);

                case 2:
                    return (int)Math.Floor(t * 0.75);
                case 1:
                    return t;
                //these shouldn't happen, but if they do return low
                case 0:
                default:
                    log.D("Problem: shouldn't asking for quality when no quality types are enabled. Defaulted to low.", true);
                    return SObject.lowQuality;
            }
        }

        /// <summary>Calculate and return a quality level for one of the 3 quality categories.</summary>
        private static int GetQualityPart(int lvl)
        {
            log.D("Getting quality piece...", Config.DebugMode);
            if (lvl > 0)
            {
                double ran = Game1.random.NextDouble();
                switch (CoreLogic.formula)
                {
                    case 0:
                    default:
                        if (ran < (Math.Min(lvl, CoreLogic.LvlCap) / 30f))
                        {
                            return SObject.highQuality;
                        }
                        else if (ran < (Math.Min(lvl, CoreLogic.LvlCap) / 15f))
                        {
                            return SObject.medQuality;
                        }
                        break;
                }
            }
            return SObject.lowQuality;
        }

        /// <summary>Return number of items to add to a stack based on gatherer perk.</summary>
		public static int TriggerGathererPerk(Farmer who)
        {
            if (!Config.DebugMode && Config.GathererAffectsTappers && who.professions.Contains(Farmer.gatherer) && Game1.random.NextDouble() < 0.2)
            {
                log.D("Gatherer perk applied", Config.DebugMode);
                return 1;
            }
            return 0;
        }

        public static int GetTimesHarvested(SObject tapper)
        {
            if (tapper is Tapper t && IsVanillaTapper(tapper))
            {
                return t.TimesHarvested;
            }

            tapper.modData.TryGetValue($"{ModEntry.UID}/timesHarvested", out string data);

            if (!string.IsNullOrEmpty(data))
            {
                return int.Parse(data);
            }
            log.D("Could not get times harvested.", true);
            return 0;
        }

        internal static void IncreaseTimesHarvested(SObject tapper)
        {
            if (tapper is Tapper t && IsVanillaTapper(tapper))
            {
                t.TimesHarvested++;
                return;
            }

            tapper.modData.TryGetValue($"{ModEntry.UID}/timesHarvested", out string data);

            if (!string.IsNullOrEmpty(data))
            {
                tapper.modData[$"{ModEntry.UID}/timesHarvested"] = (int.Parse(data) + 1).ToString();
            }
            else
            {
                tapper.modData[$"{ModEntry.UID}/timesHarvested"] = "1";
            }
        }

        internal static void SetTimesHarvested(SObject tapper, int num)
        {
            tapper.modData[$"{ModEntry.UID}/timesHarvested"] = num.ToString();
        }

        internal static ModData GetData()
        {
            ModData model = ModEntry.Instance.Helper.Data.ReadSaveData<ModData>($"{ModEntry.UID}.moddata");
            return model;
        }

        internal static void SaveData(ModData model)
        {
            ModEntry.Instance.Helper.Data.WriteSaveData($"{ModEntry.UID}.moddata", model);
        }

        internal static SObject ConvertToNormalTappers(SObject tapper, GameLocation location)
        {
            if (tapper is Tapper)
            {
                log.D("Attempting to convert tapper back to normal", Config.DebugMode);
                SObject o = new(tapper.TileLocation, tapper.ParentSheetIndex);
                o.owner.Value = tapper.owner.Value;
                o.heldObject.Value = tapper.heldObject.Value;
                o.MinutesUntilReady = tapper.MinutesUntilReady;
                SetTimesHarvested(o, GetTimesHarvested(tapper));

                location.objects.Remove(tapper.TileLocation);
                location.objects.Add(o.TileLocation, o);

                return o;
            }
            return tapper;
        }

        internal static long GetTmpUMID(SObject tapper)
        {
            tapper.modData.TryGetValue($"{ModEntry.UID}/tmpUMID", out string data);

            if (!string.IsNullOrEmpty(data))
            {
                return long.Parse(data);
            }
            log.D("Could not get tmpUMID.", true);
            return -1;
        }

        internal static void SetTmpUMID(SObject tapper, long UMID)
        {
            tapper.modData[$"{ModEntry.UID}/tmpUMID"] = UMID.ToString();
        }

        public static int GetTreeAgeMonths(Tree tree)
        {
            return (int)Math.Floor(GetTreeAge(tree)/28f);
        }

        public static int GetTreeAge(Tree tree)
        {
            tree.modData.TryGetValue($"{ModEntry.UID}/treeAge", out string data);

            if (!string.IsNullOrEmpty(data))
            {
                return int.Parse(data);
            }
            log.D("Could not get tree age.", true);
            return 0;
        }

        internal static void IncreaseTreeAges()
        {
            if (!Context.IsMainPlayer)
            {
                return;
            }

            log.T("Increasing the age of trees.");

            foreach (var location in Game1.locations)
            {
                foreach (var terrainfeature in location.terrainFeatures.Pairs)
                {
                    if (terrainfeature.Value is Tree tree)
                    {
                        IncreaseTreeAge(tree);
                    }
                }
            }
        }

        internal static void IncreaseTreeAge(Tree tree)
        {
            tree.modData.TryGetValue($"{ModEntry.UID}/treeAge", out string data);

            if (!string.IsNullOrEmpty(data))
            {
                tree.modData[$"{ModEntry.UID}/treeAge"] = (int.Parse(data) + 1).ToString();
            }
            else
            {
                tree.modData[$"{ModEntry.UID}/treeAge"] = "1";
            }
        }

        public static bool IsAnyTapper(SObject o)
        {
            return o is not null && o.bigCraftable.Value && (o.ParentSheetIndex == 105 || o.ParentSheetIndex == 264);
        }

        public static bool IsCustomTapper(SObject o)
        {
            return (o is Tapper) && !IsVanillaTapper(o);
        }

        public static bool IsVanillaTapper(SObject o)
        {
            return IsTapper(o) || IsHeavyTapper(o);
        }

        public static bool IsTapper(SObject o)
        {
            return o is not null && o.bigCraftable.Value && o.ParentSheetIndex == 105;
        }

        public static bool IsHeavyTapper(SObject o)
        {
            return o is not null && o.bigCraftable.Value && o.ParentSheetIndex == 264;
        }
    }
}