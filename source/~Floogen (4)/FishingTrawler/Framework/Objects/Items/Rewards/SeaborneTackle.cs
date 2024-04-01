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
using StardewValley;
using System;

namespace FishingTrawler.Framework.Objects.Items.Rewards
{
    public enum TackleType
    {
        Unknown,
        GuidingNarwhal,
        RustyCage,
        AlluringJellyfish,
        WeightedTreasure,
        StarryBobber
    }

    public class SeaborneTackle
    {
        private const int DRESSED_SPINNER_TACKLE_BASE_ID = 687;
        private const int TRAP_BOBBER_TACKLE_BASE_ID = 694;
        private const int CORK_BOBBER_TACKLE_BASE_ID = 695;
        private const int TREASURE_HUNTER_TACKLE_BASE_ID = 693;
        private const int QUALITY_BOBBER_TACKLE_BASE_ID = 877;

        public static Item CreateInstance(TackleType tackleType = TackleType.Unknown)
        {
            var tackle = new StardewValley.Object($"{GetBaseIdByType(tackleType)}", 1);
            tackle.modData[ModDataKeys.SEABORNE_TACKLE_KEY] = tackleType.ToString();
            tackle.uses.Value = int.MinValue;

            return tackle;
        }

        private static int GetBaseIdByType(TackleType tackleType)
        {
            switch (tackleType)
            {
                case TackleType.GuidingNarwhal:
                    return DRESSED_SPINNER_TACKLE_BASE_ID;
                case TackleType.RustyCage:
                    return TRAP_BOBBER_TACKLE_BASE_ID;
                case TackleType.AlluringJellyfish:
                    return CORK_BOBBER_TACKLE_BASE_ID;
                case TackleType.WeightedTreasure:
                    return TREASURE_HUNTER_TACKLE_BASE_ID;
                case TackleType.StarryBobber:
                    return QUALITY_BOBBER_TACKLE_BASE_ID;
                default:
                    return -1;
            }
        }

        public static TackleType GetTackleType(Item item)
        {
            if (item is not null && item.modData.ContainsKey(ModDataKeys.SEABORNE_TACKLE_KEY) && Enum.TryParse(item.modData[ModDataKeys.SEABORNE_TACKLE_KEY], out TackleType tackleType))
            {
                return tackleType;
            }

            return TackleType.Unknown;
        }

        public static string GetName(Item item)
        {
            switch (GetTackleType(item))
            {
                case TackleType.GuidingNarwhal:
                    return FishingTrawler.i18n.Get("item.seaborne_tackle.guiding_narwhal.name");
                case TackleType.RustyCage:
                    return FishingTrawler.i18n.Get("item.seaborne_tackle.rusty_cage.name");
                case TackleType.AlluringJellyfish:
                    return FishingTrawler.i18n.Get("item.seaborne_tackle.alluring_jellyfish.name");
                case TackleType.WeightedTreasure:
                    return FishingTrawler.i18n.Get("item.seaborne_tackle.weighted_treasure.name");
                case TackleType.StarryBobber:
                    return FishingTrawler.i18n.Get("item.seaborne_tackle.starry_bobber.name");
                default:
                    return string.Empty;
            }
        }

        public static string GetDescription(Item item)
        {
            switch (GetTackleType(item))
            {
                case TackleType.GuidingNarwhal:
                    return FishingTrawler.i18n.Get("item.seaborne_tackle.guiding_narwhal.description");
                case TackleType.RustyCage:
                    return FishingTrawler.i18n.Get("item.seaborne_tackle.rusty_cage.description");
                case TackleType.AlluringJellyfish:
                    return FishingTrawler.i18n.Get("item.seaborne_tackle.alluring_jellyfish.description");
                case TackleType.WeightedTreasure:
                    return FishingTrawler.i18n.Get("item.seaborne_tackle.weighted_treasure.description");
                case TackleType.StarryBobber:
                    return FishingTrawler.i18n.Get("item.seaborne_tackle.starry_bobber.description");
                default:
                    return string.Empty;
            }
        }

        public static bool IsValid(Item item)
        {
            if (item is not null && item.modData.ContainsKey(ModDataKeys.SEABORNE_TACKLE_KEY))
            {
                return true;
            }

            return false;
        }
    }
}