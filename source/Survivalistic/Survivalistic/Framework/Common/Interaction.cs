/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ophaneom/Survivalistic
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewValley;
using Survivalistic.Framework.Bars;
using Survivalistic.Framework.Common.Affection;
using Survivalistic.Framework.Databases;

namespace Survivalistic.Framework.Common
{
    public class Interaction
    {
        private static bool already_eating = false;
        private static bool already_using_tool = false;

        private static string item_eaten_name;

        private static string tool_used_name;

        private static bool getting_tick_information = true;

        public static void EatingCheck()
        {
            if (!Context.IsWorldReady) return;

            if (Game1.player.isEating)
            {
                item_eaten_name = Game1.player.itemToEat.Name;

                already_eating = true;
            }
            else
            {
                if (already_eating)
                {
                    already_eating = false;
                    IncreaseStatus(item_eaten_name, Game1.player.itemToEat.staminaRecoveredOnConsumption());
                }
            }
        }

        public static void UsingToolCheck()
        {
            if (!Context.IsWorldReady) return;

            if (Game1.player.UsingTool)
            {
                tool_used_name = Game1.player.CurrentTool.BaseName;
                already_using_tool = true;
            }
            else
            {
                if (already_using_tool)
                {
                    already_using_tool = false;
                    DecreaseStatus(tool_used_name);
                }
            }
        }

        private static void IncreaseStatus(string food_eated, int recover)
        {
            float last_hunger = ModEntry.data.actual_hunger;
            float last_thirst = ModEntry.data.actual_thirst;

            if (Foods.foodDatabase.TryGetValue(food_eated, out string food_status_string))
            {
                List<string> food_status = food_status_string.Split('/').ToList();

                if (ModEntry.data.actual_hunger < ModEntry.data.max_hunger) ModEntry.data.actual_hunger += Int32.Parse(food_status[0]);
                if (ModEntry.data.actual_thirst < ModEntry.data.max_thirst) ModEntry.data.actual_thirst += Int32.Parse(food_status[1]);

                BarsInformations.NormalizeStatus();

                float hunger_diff = ModEntry.data.actual_hunger - last_hunger;
                float thirst_diff = ModEntry.data.actual_thirst - last_thirst;

                if (hunger_diff > 0) Game1.addHUDMessage(new HUDMessage(string.Format(ModEntry.instance.Helper.Translation.Get("info-fullness"), (int)hunger_diff), 4));
                if (thirst_diff > 0) Game1.addHUDMessage(new HUDMessage(string.Format(ModEntry.instance.Helper.Translation.Get("info-thirsty"), (int)thirst_diff), 4));
            }

            else if (ModEntry.config.non_supported_food)
            {
                if (ModEntry.data.actual_hunger < ModEntry.data.max_hunger) ModEntry.data.actual_hunger += recover * new Random().Next(1, 3);
                if (ModEntry.data.actual_thirst < ModEntry.data.max_thirst) ModEntry.data.actual_thirst += recover * new Random().Next(1, 3);

                BarsInformations.NormalizeStatus();

                float hunger_diff = ModEntry.data.actual_hunger - last_hunger;
                float thirst_diff = ModEntry.data.actual_thirst - last_thirst;

                if (hunger_diff > 0) Game1.addHUDMessage(new HUDMessage(string.Format(ModEntry.instance.Helper.Translation.Get("info-fullness"), (int)hunger_diff), 4));
                if (thirst_diff > 0) Game1.addHUDMessage(new HUDMessage(string.Format(ModEntry.instance.Helper.Translation.Get("info-thirsty"), (int)thirst_diff), 4));
            }
        }

        private static void DecreaseStatus(string tool_used)
        {
            if (Tools.GetToolDatabase().TryGetValue(tool_used, out string tool_status_string))
            {
                List<string> tool_status = tool_status_string.Split('/').ToList();

                if (ModEntry.data.actual_hunger >= 0) 
                    ModEntry.data.actual_hunger -= float.Parse(tool_status[0]) * (BarsDatabase.tool_use_multiplier * ModEntry.config.hunger_action_multiplier);

                if (ModEntry.data.actual_thirst >= 0) 
                    ModEntry.data.actual_thirst -= float.Parse(tool_status[1]) * (BarsDatabase.tool_use_multiplier * ModEntry.config.thirst_action_multiplier);

                if (!Benefits.VerifyBenefits())
                    Penalty.VerifyPenalty();

                BarsInformations.NormalizeStatus();
                BarsWarnings.VerifyStatus();
            }
        }

        public static void Awake()
        {
            ModEntry.data.initial_hunger = ModEntry.data.actual_hunger;
            ModEntry.data.initial_thirst = ModEntry.data.actual_thirst;
            ModEntry.data.actual_day = Game1.Date.DayOfMonth;
            ModEntry.data.actual_season = Game1.Date.SeasonIndex;
            ModEntry.data.actual_year = Game1.Date.Year;
        }

        public static void ReceiveAwakeInfos()
        {
            if (Game1.IsMultiplayer)
            {
                if (ModEntry.data.actual_day != Game1.Date.DayOfMonth ||
                        ModEntry.data.actual_season != Game1.Date.SeasonIndex ||
                        ModEntry.data.actual_year != Game1.Date.Year ||
                        ModEntry.data.actual_tick < Game1.ticks)
                {
                    ModEntry.data.actual_hunger = ModEntry.data.initial_hunger;
                    ModEntry.data.actual_thirst = ModEntry.data.initial_thirst;
                }
            }
            else
            {
                ModEntry.data.actual_hunger = ModEntry.data.initial_hunger;
                ModEntry.data.actual_thirst = ModEntry.data.initial_thirst;
            }
            getting_tick_information = false;
        }

        public static void UpdateTickInformation()
        {
            if (!getting_tick_information)
            {
                ModEntry.data.actual_tick = Game1.ticks;
            }
        }
    }
}
