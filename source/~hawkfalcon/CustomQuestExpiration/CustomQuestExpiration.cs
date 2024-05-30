/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/hawkfalcon/Stardew-Mods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Quests;
using System.Collections.Generic;
using System.Linq;

namespace CustomQuestExpiration {
    public class CustomQuestExpiration : Mod {
        private const int InfiniteDays = 100000;
        private ModConfig Config;
        private List<Quest> dailyQuests = new List<Quest>();

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper) {
            Config = Helper.ReadConfig<ModConfig>();

            helper.Events.Display.MenuChanged += OnMenuChanged;
        }

        /// <summary>Raised after a game menu is opened, closed, or replaced.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnMenuChanged(object sender, MenuChangedEventArgs e) {
            if (e.OldMenu is StardewValley.Menus.Billboard) {
                updateQuest();
            }

            if (Config.NeverExpires && (e.OldMenu is StardewValley.Menus.QuestLog || e.NewMenu is StardewValley.Menus.QuestLog)) {
                updateQuestIcons();
            }
        }

        // Update quests when leaving the billboard
        private void updateQuest() {
            Quest currentDailyQuest = Game1.player.questLog.FirstOrDefault(quest =>
                quest.dailyQuest.Value && quest.Equals(Game1.questOfTheDay)
            );
            if (currentDailyQuest != null) {
                int daysLeft = getDaysLeft(currentDailyQuest);
                currentDailyQuest.daysLeft.Value = daysLeft;
            }
        }

        // Make all quests not daily temporarily to set the icon to (!)
        private void updateQuestIcons() {
            foreach (Quest quest in Game1.player.questLog) {
                if (quest.dailyQuest.Value) {
                    dailyQuests.Add(quest);
                    quest.dailyQuest.Value = false;
                }
            }
        }

        private void resetQuestIcons() {
            foreach (Quest quest in dailyQuests) {
                quest.dailyQuest.Value = true;
            }
            dailyQuests.Clear();
        }

        private int getDaysLeft(Quest quest) {
            if (Config.NeverExpires)
                return InfiniteDays;
            if (!Config.UsesQuestCategoryExpiration)
                return Config.DaysToExpiration;

            // daily quest types
            // https://stardewvalleywiki.com/Quests#Types
            switch (quest.questType.Value) {
            case Quest.type_itemDelivery:
                return Config.CategoryExpiration.ItemDelivery;
            case Quest.type_resource:
                return Config.CategoryExpiration.Gathering;
            case Quest.type_fishing:
                return Config.CategoryExpiration.Fishing;
            case Quest.type_monster:
                return Config.CategoryExpiration.SlayMonsters;
            default:
                return Config.DaysToExpiration;
            }
        }
    }
}
