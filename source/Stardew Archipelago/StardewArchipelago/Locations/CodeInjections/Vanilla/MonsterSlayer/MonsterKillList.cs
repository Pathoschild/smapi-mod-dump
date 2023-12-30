/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Force.DeepCloner;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Constants;
using StardewValley;

namespace StardewArchipelago.Locations.CodeInjections.Vanilla.MonsterSlayer
{
    public class MonsterKillList
    {
        private const string MONSTER_LINE_FORMAT = "Strings\\Locations:AdventureGuild_KillList_{0}";
        private const string MONSTER_HEADER = "Strings\\Locations:AdventureGuild_KillList_Header";
        private const string MONSTER_FOOTER = "Strings\\Locations:AdventureGuild_KillList_Footer";
        
        private ArchipelagoClient _archipelago;

        public readonly Dictionary<string, string[]> MonstersByCategory = new()
        {
            { MonsterCategory.SLIMES, new[] { MonsterName.GREEN_SLIME, MonsterName.FROST_JELLY, MonsterName.SLUDGE, MonsterName.TIGER_SLIME } },
            { MonsterCategory.VOID_SPIRITS, new[] { MonsterName.SHADOW_SHAMAN, MonsterName.SHADOW_BRUTE, MonsterName.SHADOW_SNIPER } },
            { MonsterCategory.BATS, new[] { MonsterName.BAT, MonsterName.FROST_BAT, MonsterName.LAVA_BAT, MonsterName.IRIDIUM_BAT } },
            { MonsterCategory.SKELETONS, new[] { MonsterName.SKELETON, MonsterName.SKELETON_MAGE } },
            { MonsterCategory.CAVE_INSECTS, new[] { MonsterName.GRUB, MonsterName.FLY, MonsterName.BUG } },
            { MonsterCategory.DUGGIES, new[] { MonsterName.DUGGY, MonsterName.MAGMA_DUGGY } },
            { MonsterCategory.DUST_SPRITES, new[] { MonsterName.DUST_SPRITE } },
            { MonsterCategory.ROCK_CRABS, new[] { MonsterName.ROCK_CRAB, MonsterName.LAVA_CRAB, MonsterName.IRIDIUM_CRAB } },
            { MonsterCategory.MUMMIES, new[] { MonsterName.MUMMY } },
            { MonsterCategory.PEPPER_REX, new[] { MonsterName.PEPPER_REX } },
            { MonsterCategory.SERPENTS, new[] { MonsterName.SERPENT, MonsterName.ROYAL_SERPENT } },
            { MonsterCategory.MAGMA_SPRITES, new[] { MonsterName.MAGMA_SPRITE, MonsterName.MAGMA_SPARKER } },
        };
        public readonly Dictionary<string, int> DefaultMonsterGoals = new()
        {
            { MonsterCategory.SLIMES, 1000 },
            { MonsterCategory.VOID_SPIRITS, 150 },
            { MonsterCategory.BATS, 200 },
            { MonsterCategory.SKELETONS, 50 },
            { MonsterCategory.CAVE_INSECTS, 125 },
            { MonsterCategory.DUGGIES, 30 },
            { MonsterCategory.DUST_SPRITES, 500 },
            { MonsterCategory.ROCK_CRABS, 60 },
            { MonsterCategory.MUMMIES, 100 },
            { MonsterCategory.PEPPER_REX, 50 },
            { MonsterCategory.SERPENTS, 250 },
            { MonsterCategory.MAGMA_SPRITES, 150 },
        };

        public Dictionary<string, int> MonsterGoals { get; private set; }
        public Dictionary<string, string> MonsterCategories { get; private set; }

        public MonsterKillList(ArchipelagoClient archipelago)
        {
            _archipelago = archipelago;
            MonsterCategories = new Dictionary<string, string>();
            foreach (var (category, monsters) in MonstersByCategory)
            {
                foreach (var monster in monsters)
                {
                    MonsterCategories.Add(monster, category);
                }
            }
            GenerateGoals();
        }

        private void GenerateGoals()
        {
            switch (_archipelago.SlotData.Monstersanity)
            {
                case Monstersanity.None:
                    MonsterGoals = DefaultMonsterGoals.DeepClone();
                    break;
                case Monstersanity.OnePerCategory:
                    MonsterGoals = DefaultMonsterGoals.ToDictionary(x => x.Key, _ => 1);
                    break;
                case Monstersanity.OnePerMonster:
                    MonsterGoals = MonstersByCategory.SelectMany(x => x.Value).ToDictionary(x => x, _ => 1);
                    break;
                case Monstersanity.Goals:
                    MonsterGoals = DefaultMonsterGoals.DeepClone();
                    break;
                case Monstersanity.ShortGoals:
                    MonsterGoals = DefaultMonsterGoals.ToDictionary(x => x.Key, x => (int)(x.Value * 0.4));
                    break;
                case Monstersanity.VeryShortGoals:
                    MonsterGoals = DefaultMonsterGoals.ToDictionary(x => x.Key, x => (int)(x.Value * 0.1));
                    break;
                case Monstersanity.ProgressiveGoals:
                    MonsterGoals = DefaultMonsterGoals.DeepClone();
                    break;
                case Monstersanity.SplitGoals:
                    MonsterGoals = GenerateSplitGoals();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (!_archipelago.SlotData.ExcludeGingerIsland)
            {
                return;
            }

            MonsterGoals.Remove(MonsterCategory.MAGMA_SPRITES);
            MonsterGoals.Remove(MonsterName.MAGMA_SPRITE);
            MonsterGoals.Remove(MonsterName.MAGMA_SPARKER);
            MonsterGoals.Remove(MonsterName.ROYAL_SERPENT);
            MonsterGoals.Remove(MonsterName.SKELETON_MAGE);
            MonsterGoals.Remove(MonsterName.SHADOW_SNIPER);
            MonsterGoals.Remove(MonsterName.TIGER_SLIME);
        }

        private Dictionary<string, int> GenerateSplitGoals()
        {
            var goals = new Dictionary<string, int>();
            foreach (var (category, killsRequired) in DefaultMonsterGoals)
            {
                var monstersInCategory = MonstersByCategory[category];
                var killsPerMonster = killsRequired / monstersInCategory.Length;
                foreach (var monster in monstersInCategory)
                {
                    goals.Add(monster, killsPerMonster);
                }
            }

            return goals;
        }

        public string GetKillListLetterContent()
        {
            var header = Game1.content.LoadString(MONSTER_HEADER).Replace('\n', '^') + "^";
            var footer = Game1.content.LoadString(MONSTER_FOOTER).Replace('\n', '^');

            var stringBuilder = new StringBuilder();
            stringBuilder.Append(header);

            foreach (var (monster, killsRequired) in MonsterGoals)
            {
                var killCount = MonstersByCategory.ContainsKey(monster) ? GetMonstersKilledInCategory(monster) : GetMonstersKilled(monster);
                var killListLine = GetKillListLine(monster, killCount, killsRequired);
                stringBuilder.Append(killListLine);
            }

            stringBuilder.Append(footer);
            return stringBuilder.ToString();
        }

        public int GetMonstersKilledInCategory(string category)
        {
            return MonstersByCategory[category].Sum(GetMonstersKilled);
        }

        public int GetMonstersKilled(string monster)
        {
            return Game1.stats.getMonstersKilled(monster);
        }

        private string GetKillListLine(string monsterType, int killCount, int target)
        {
            var lineFormat = Game1.content.LoadString("Strings\\Locations:AdventureGuild_KillList_LineFormat");
            if (killCount <= 0)
            {
                lineFormat = Game1.content.LoadString("Strings\\Locations:AdventureGuild_KillList_LineFormat_None");
            }

            if (killCount >= target)
            {
                lineFormat = Game1.content.LoadString("Strings\\Locations:AdventureGuild_KillList_LineFormat_OverTarget");
            }
            
            var line = string.Format(lineFormat, killCount, target, monsterType) + "^";
            return line;
        }
    }
}
