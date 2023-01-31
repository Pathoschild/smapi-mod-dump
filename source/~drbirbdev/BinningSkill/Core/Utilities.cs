/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System;
using BirbShared;
using Microsoft.Xna.Framework;
using SpaceCore;
using StardewValley;
using xTile.Dimensions;

namespace BinningSkill
{
    internal class Utilities
    {
        internal static void DoTrashCanCheck(string location, string whichCan, int debrisCount, Vector2 position)
        {
            if (Game1.currentLocation.debris is not null && Game1.currentLocation.debris.Count > debrisCount)
            {
                // something was already gotten from the trash, give standard binning experience and return
                Skills.AddExperience(Game1.player, "drbirbdev.Binning", ModEntry.Config.ExperienceFromTrashSuccess);
                return;

            }

            Item drop = GetBonusItem(location, whichCan, Game1.player, true);

            if (drop != null)
            {
                Game1.createItemDebris(drop, position, 2, Game1.currentLocation, (int)position.Y + 64);
            }
        }

        internal static Item GetBonusItem(string location, string whichCan, Farmer player, bool doSound)
        {
            int rarity = BirbShared.Utilities.GetRarity(GetBinningRarityLevels());

            if (rarity < 0)
            {
                // Give binning experience for not getting anything
                Skills.AddExperience(player, "drbirbdev.Binning", ModEntry.Config.ExperienceFromTrashFail);
                return null;
            }

            if (doSound && rarity == 2)
            {
                Game1.currentLocation.playSound("yoba");
            }
            else if (doSound && rarity == 3)
            {
                Game1.currentLocation.playSound("reward");
            }

            Skills.AddExperience(player, "drbirbdev.Binning", ModEntry.Config.ExperienceFromTrashBonus * (int)(Math.Pow(2, rarity)));

            string dropString = BirbShared.Utilities.GetRandomDropStringFromLootTable(ModEntry.Assets.TrashTable, location, whichCan, rarity.ToString());

            return BirbShared.Utilities.ParseDropString(dropString, ModEntry.JsonAssets, ModEntry.DynamicGameAssets);
        } 

        internal static Vector2 GetItemPosition(Vector2 tilePosition)
        {
            return new Vector2(tilePosition.X + 0.5f, tilePosition.Y - 1) * 64f;
        }

        internal static Vector2 GetItemPosition(Location tileLocation)
        {
            return new Vector2(tileLocation.X + 0.5f, tileLocation.Y - 1) * 64f;
        }

        internal static int[] GetBinningRarityLevels()
        {
            int level = Skills.GetSkillLevel(Game1.player, "drbirbdev.Binning");
            return new int[]
            {
                ModEntry.Config.PerLevelBonusDropChance * level,
                level >= 3 ? ModEntry.Config.RareDropChance : 0,
                level >= 6 ? ModEntry.Config.SuperRareDropChance : 0,
                level >= 9 ? ModEntry.Config.UltraRareDropChance : 0
            };
        }

        internal static int[] GetSalvagerRarityLevels()
        {
            if (!ModEntry.MargoLoaded || Game1.player.HasCustomPrestigeProfession(BinningSkill.Salvager))
            {
                return new int[]
                {
                    ModEntry.Config.SalvagerRareDropChance,
                    ModEntry.Config.SalvagerSuperRareDropChance
                };
            }
            else
            {
                return new int[]
                {
                    ModEntry.Config.SalvagerRareDropChance,
                    0,
                };
            }
        }

        internal static StardewValley.Object GetSalvagerUpgrade(StardewValley.Object original)
        {
            if (original == null)
            {
                Log.Error("Expected a value to already exist in the Recycling Machine but found null");
                return original;
            }
            if (!Game1.player.HasCustomProfession(BinningSkill.Salvager))
            {
                return original;
            }

            int rarity = BirbShared.Utilities.GetRarity(GetSalvagerRarityLevels());
            if (rarity < 0)
            {
                return original;
            }

            string dropString = BirbShared.Utilities.GetRandomDropStringFromLootTable(ModEntry.Assets.SalvagerTable, original.ParentSheetIndex.ToString(), "*", rarity.ToString());

            return (StardewValley.Object)BirbShared.Utilities.ParseDropString(dropString, ModEntry.JsonAssets, ModEntry.DynamicGameAssets);
        }
    }
}
