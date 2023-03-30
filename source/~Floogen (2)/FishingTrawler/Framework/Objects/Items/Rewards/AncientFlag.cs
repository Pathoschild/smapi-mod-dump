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
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using System;

namespace FishingTrawler.Framework.Objects.Items.Rewards
{
    public enum FlagType
    {
        Unknown,
        Parley,
        JollyRoger,
        GamblersCrest,
        MermaidsBlessing,
        PatronSaint,
        SharksFin,
        Worldly,
        SlimeKing,
        KingCrab,
        EternalFlame,
        SwiftWinds
    }

    public class AncientFlag
    {
        private const int PIRATE_FLAG_BASE_ID = 1900;

        public static Furniture CreateInstance(FlagType flagType = FlagType.Unknown)
        {
            var flag = new Furniture(PIRATE_FLAG_BASE_ID, Vector2.Zero);
            flag.modData[ModDataKeys.ANCIENT_FLAG_KEY] = flagType.ToString();

            return flag;
        }

        public static FlagType GetFlagType(Item item)
        {
            if (item is Furniture furniture && furniture is not null && furniture.modData.ContainsKey(ModDataKeys.ANCIENT_FLAG_KEY) && Enum.TryParse(furniture.modData[ModDataKeys.ANCIENT_FLAG_KEY], out FlagType flagType))
            {
                return flagType;
            }

            return FlagType.Unknown;
        }

        internal static string GetFlagName(FlagType flagType)
        {
            switch (flagType)
            {
                case FlagType.Parley:
                    return FishingTrawler.i18n.Get("item.ancient_flag.parely.name");
                case FlagType.JollyRoger:
                    return FishingTrawler.i18n.Get("item.ancient_flag.jolly_roger.name");
                case FlagType.GamblersCrest:
                    return FishingTrawler.i18n.Get("item.ancient_flag.gamblers_crest.name");
                case FlagType.MermaidsBlessing:
                    return FishingTrawler.i18n.Get("item.ancient_flag.mermaids_blessing.name");
                case FlagType.PatronSaint:
                    return FishingTrawler.i18n.Get("item.ancient_flag.patron_saint.name");
                case FlagType.SharksFin:
                    return FishingTrawler.i18n.Get("item.ancient_flag.sharks_fin.name");
                case FlagType.Worldly:
                    return FishingTrawler.i18n.Get("item.ancient_flag.worldly_flag.name");
                case FlagType.SlimeKing:
                    return FishingTrawler.i18n.Get("item.ancient_flag.slime_king.name");
                case FlagType.KingCrab:
                    return FishingTrawler.i18n.Get("item.ancient_flag.king_crab.name");
                case FlagType.EternalFlame:
                    return FishingTrawler.i18n.Get("item.ancient_flag.eternal_flame.name");
                case FlagType.SwiftWinds:
                    return FishingTrawler.i18n.Get("item.ancient_flag.swift_winds.name");
                default:
                    return FishingTrawler.i18n.Get("item.ancient_flag.unknown.name");
            }
        }

        internal static string GetFlagDescription(FlagType flagType)
        {
            switch (flagType)
            {
                case FlagType.Parley:
                    return FishingTrawler.i18n.Get("item.ancient_flag.parely.description");
                case FlagType.JollyRoger:
                    return FishingTrawler.i18n.Get("item.ancient_flag.jolly_roger.description");
                case FlagType.GamblersCrest:
                    return FishingTrawler.i18n.Get("item.ancient_flag.gamblers_crest.description");
                case FlagType.MermaidsBlessing:
                    return FishingTrawler.i18n.Get("item.ancient_flag.mermaids_blessing.description");
                case FlagType.PatronSaint:
                    return FishingTrawler.i18n.Get("item.ancient_flag.patron_saint.description");
                case FlagType.SharksFin:
                    return FishingTrawler.i18n.Get("item.ancient_flag.sharks_fin.description");
                case FlagType.Worldly:
                    return FishingTrawler.i18n.Get("item.ancient_flag.worldly_flag.description");
                case FlagType.SlimeKing:
                    return FishingTrawler.i18n.Get("item.ancient_flag.slime_king.description");
                case FlagType.KingCrab:
                    return FishingTrawler.i18n.Get("item.ancient_flag.king_crab.description");
                case FlagType.EternalFlame:
                    return FishingTrawler.i18n.Get("item.ancient_flag.eternal_flame.description");
                case FlagType.SwiftWinds:
                    return FishingTrawler.i18n.Get("item.ancient_flag.swift_winds.description");
                default:
                    return FishingTrawler.i18n.Get("item.ancient_flag.unknown.description");
            }
        }

        internal static bool IsFlag(Item item)
        {
            if (item is Furniture furniture && furniture.modData.ContainsKey(ModDataKeys.ANCIENT_FLAG_KEY) is true)
            {
                return true;
            }

            return false;
        }
    }
}
