/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

using StardewValley;
using StardewValley.Objects;
using System;
using StardewObject = StardewValley.Object;

namespace CrabPotQuality
{
    internal class CrabPotQualityLogic
    {
        internal static CrabPotQualityConfig Config { get; set; }

        internal const string wildBaitQID = "(O)774";
        internal const string magicBaitQID = "(O)908";
        internal const string challengeBaitQID = "(O)ChallengeBait";
        internal const string deluxeBaitQID = "(O)DeluxeBait";
        internal const string magnetBaitQID = "(O)703";

        internal const string defaultBaitQID = "(O)685";

        internal static readonly string chancesAlreadyCheckedKey = $"{CrabPotQuality.Manifest?.UniqueID}/chancesAlreadyChecked";

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0066:Convert switch statement to expression", Justification = "Less readable")]
        internal static bool IsTrash(StardewObject o)
        {
            switch (o.QualifiedItemId)
            {
                case "(O)168":
                case "(O)169":
                case "(O)170":
                case "(O)171":
                case "(O)172":
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Checks the bait and config to replace the crabpot item with a new item and checks whether it should get quality
        /// </summary>
        /// <returns> True when the item was changed to an item that should not have quality, else False </returns>
        internal static bool ReplaceWithSpecialItem(CrabPot pot)
        {
            if (pot?.bait.Value == null)
            {
                return false;
            }

            return (pot.bait.Value.QualifiedItemId) switch
            {
                wildBaitQID => EvaluateBaitSpecialItemConfig(pot, Config.EnableWildBaitEffect, Config.WildBaitSpecialItemChance, Config.WildBaitSpecialItem, Config.WildBaitSpecialItemHasNoQuality),
                magicBaitQID => EvaluateBaitSpecialItemConfig(pot, Config.EnableMagicBaitEffect, Config.MagicBaitSpecialItemChance, Config.MagicBaitSpecialItem, Config.ChallengeBaitSpecialItemHasNoQuality),
                challengeBaitQID => EvaluateBaitSpecialItemConfig(pot, Config.EnableChallengeBaitEffect, Config.ChallengeBaitSpecialItemChance, Config.ChallengeBaitSpecialItem, Config.ChallengeBaitSpecialItemHasNoQuality),
                deluxeBaitQID => EvaluateBaitSpecialItemConfig(pot, Config.EnableDeluxeBaitEffect, Config.DeluxeBaitSpecialItemChance, Config.DeluxeBaitSpecialItem, Config.DeluxeBaitSpecialItemHasNoQuality),
                magnetBaitQID => EvaluateBaitSpecialItemConfig(pot, Config.EnableMagnetBaitEffect, Config.MagnetBaitSpecialItemChance, Config.MagnetBaitSpecialItem, Config.MagnetBaitSpecialItemHasNoQuality),
                defaultBaitQID => false,
                _ => EvaluateBaitSpecialItemConfig(pot, Config.EnableCustomModdedBaitEffect, Config.CustomModdedBaitSpecialItemChance, Config.CustomModdedBaitSpecialItem, Config.CustomModdedBaitSpecialItemHasNoQuality),
            };
        }

        internal static bool EvaluateBaitSpecialItemConfig(CrabPot pot, bool enabledConfig, int specialItemChanceConfig, string specialItem, bool specialItemNoQuality)
        {
            if (!enabledConfig)
            {
                return false;
            }

            Random r = Utility.CreateDaySaveRandom(pot.TileLocation.X * 777f, pot.TileLocation.Y * 77f);
            if (r.NextDouble() < Math.Clamp(specialItemChanceConfig / 100f, 0f, 1f))
            {
                if (string.IsNullOrWhiteSpace(specialItem) || !ItemRegistry.IsQualifiedItemId(specialItem.Trim()))
                {
                    return false;
                }

                pot.heldObject.Value = ItemRegistry.Create(specialItem.Trim()) as StardewObject;

                return specialItemNoQuality;
            }

            return false;
        }

        internal static float GetBaitDoubleItemAmountChance(CrabPot pot)
        {
            if (pot?.bait.Value == null)
            {
                return 0f;
            }

            return (pot?.bait.Value.QualifiedItemId) switch
            {
                wildBaitQID => EvaluateBaitDoubleItemAmountChanceConfig(Config.EnableWildBaitEffect, Config.WildBaitDoubleItemAmountChance),
                magicBaitQID => EvaluateBaitDoubleItemAmountChanceConfig(Config.EnableMagicBaitEffect, Config.MagicBaitDoubleItemAmountChance),
                challengeBaitQID => EvaluateBaitDoubleItemAmountChanceConfig(Config.EnableChallengeBaitEffect, Config.ChallengeBaitDoubleItemAmountChance),
                deluxeBaitQID => EvaluateBaitDoubleItemAmountChanceConfig(Config.EnableDeluxeBaitEffect, Config.DeluxeBaitDoubleItemAmountChance),
                magnetBaitQID => EvaluateBaitDoubleItemAmountChanceConfig(Config.EnableMagnetBaitEffect, Config.MagnetBaitDoubleItemAmountChance),
                defaultBaitQID => 0f,
                _ => EvaluateBaitDoubleItemAmountChanceConfig(Config.EnableCustomModdedBaitEffect, Config.CustomModdedBaitDoubleItemAmountChance),
            };
        }

        internal static float EvaluateBaitDoubleItemAmountChanceConfig(bool enabledConfig, float baitQualityModifierConfig)
        {
            return !enabledConfig ? 0f : Math.Clamp(baitQualityModifierConfig / 100f, 0f, 1f);
        }

        internal static float GetBaitQualityModifier(CrabPot pot)
        {
            if (pot?.bait.Value == null)
            {
                return 1f;
            }

            return (pot?.bait.Value.QualifiedItemId) switch
            {
                wildBaitQID => EvaluateBaitQualifyModifierConfig(Config.EnableWildBaitEffect, Config.WildBaitQualityChanceModifier),
                magicBaitQID => EvaluateBaitQualifyModifierConfig(Config.EnableMagicBaitEffect, Config.MagicBaitQualityChanceModifier),
                challengeBaitQID => EvaluateBaitQualifyModifierConfig(Config.EnableChallengeBaitEffect, Config.ChallengeBaitQualityChanceModifier),
                deluxeBaitQID => EvaluateBaitQualifyModifierConfig(Config.EnableDeluxeBaitEffect, Config.DeluxeBaitQualityChanceModifier),
                magnetBaitQID => EvaluateBaitQualifyModifierConfig(Config.EnableMagnetBaitEffect, Config.MagnetBaitQualityChanceModifier),
                defaultBaitQID => 1f,
                _ => EvaluateBaitQualifyModifierConfig(Config.EnableCustomModdedBaitEffect, Config.CustomModdedBaitQualityChanceModifier),
            };
        }

        internal static float EvaluateBaitQualifyModifierConfig(bool enabledConfig, float baitQualityModifierConfig)
        {
            return !enabledConfig ? 1f : Math.Max(0f, baitQualityModifierConfig);
        }
    }
}