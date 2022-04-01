/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/adverserath/StardewValley-CasualLifeMod
**
*************************************************/

using System;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;

namespace CasualLife
{
    public class ModEntry : Mod
    {
        private ModConfig Config;
        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();
            Game1Patches.DoLighting = Config.ControlDayLightLevels;
            DayTimeMoneyBoxPatch.Is24Hour = Config.Is24HourDefault;

            Harmony harmony = new Harmony(this.ModManifest.UniqueID);
            harmony.Patch(
               original: AccessTools.Method(typeof(Game1), nameof(Game1.performTenMinuteClockUpdate)),
               prefix: new HarmonyMethod(typeof(Game1Patches), nameof(Game1Patches.performTenMinuteClockUpdate))
            );


            harmony.Patch(
               original: AccessTools.Method(typeof(Game1), nameof(Game1.UpdateGameClock)),
               prefix: new HarmonyMethod(typeof(Game1Patches), nameof(Game1Patches.UpdateGameClock))
            );


            harmony.Patch(
                original: AccessTools.Method(typeof(DayTimeMoneyBox), "draw", new Type[] { typeof(SpriteBatch) }, null),
                prefix: new HarmonyMethod(typeof(DayTimeMoneyBoxPatch), nameof(DayTimeMoneyBoxPatch.drawFromDecom))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(DayTimeMoneyBox), "receiveRightClick"),
                prefix: new HarmonyMethod(typeof(DayTimeMoneyBoxPatch), nameof(DayTimeMoneyBoxPatch.receiveRightClick))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(MineShaft), "getExtraMillisecondsPerInGameMinuteForThisLocation"),
                prefix: new HarmonyMethod(typeof(Game1Patches), nameof(Game1Patches.getExtraMillisecondsPerInGameMinuteForThisLocation))
            );
            if (Config.ControlDayWithKeys)
            {
                helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            }
        }
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;


            if (e.IsDown(SButton.LeftControl))
                {
                if (e.Button == SButton.Left)
                {
                        Game1.timeOfDay -= 100;
                    return;
                }

                if (e.Button == SButton.Right)
                {
                    Game1.timeOfDay += 100;

                    return;

                }

                if (e.Button == SButton.Up)
                {
                    if (Game1.timeOfDay % 100 >= 59)
                    {
                        Game1.timeOfDay += 41;
                    }
                    else
                    {
                        Game1.timeOfDay += 1;
                    }
                    return;
                }
            
            if (e.Button == SButton.Down)
            {
                    if (Game1.timeOfDay % 100 <= 0)
                    {
                        Game1.timeOfDay -= 41;
                        Game1.ticks = 0;
                    }
                    else
                    {
                        Game1.timeOfDay -= 1;
                        Game1.ticks = 0;
                    }
                    return;
            }
        }

            if (e.Button == SButton.Right)
            {
                if (Game1.dayOfMonth < 28)
                {
                    Game1.dayOfMonth++;
                }
                else if (Game1.dayOfMonth == 28)
                {
                    Game1.dayOfMonth = 1;
                    ShiftSeasonUp();
                }
            }

            if (e.Button == SButton.Up)
            {
                ShiftSeasonUp();
            }
            if (e.Button == SButton.Down)
            {
                ShiftSeasonDown();
            }

            if (e.IsDown(SButton.LeftControl) && e.IsDown(SButton.LeftShift))
            {
                switch (e.Button)
                {
                    case SButton.F1:
                        Game1.timeOfDay = 1300;
                        break;
                    case SButton.F2:
                        Game1.timeOfDay = 1400;
                        break;
                    case SButton.F3:
                        Game1.timeOfDay = 1500;
                        break;
                    case SButton.F4:
                        Game1.timeOfDay = 1600;
                        break;
                    case SButton.F5:
                        Game1.timeOfDay = 1700;
                        break;
                    case SButton.F6:
                        Game1.timeOfDay = 1800;
                        break;
                    case SButton.F7:
                        Game1.timeOfDay = 1900;
                        break;
                    case SButton.F8:
                        Game1.timeOfDay = 2000;
                        break;
                    case SButton.F9:
                        Game1.timeOfDay = 2100;
                        break;
                    case SButton.F10:
                        Game1.timeOfDay = 2200;
                        break;
                    case SButton.F11:
                        Game1.timeOfDay = 2300;
                        break;
                    case SButton.F12:
                        Game1.timeOfDay = 0;
                        break;
                }
                if (e.Button == SButton.NumPad9)
                {
                    Game1.timeOfDay = 2100;
                }
            }
            else if (e.IsDown(SButton.LeftControl))
            {
                switch (e.Button)
                {
                    case SButton.F1:
                        Game1.timeOfDay = 100;
                        break;
                    case SButton.F2:
                        Game1.timeOfDay = 200;
                        break;
                    case SButton.F3:
                        Game1.timeOfDay = 300;
                        break;
                    case SButton.F4:
                        Game1.timeOfDay = 400;
                        break;
                    case SButton.F5:
                        Game1.timeOfDay = 500;
                        break;
                    case SButton.F6:
                        Game1.timeOfDay = 600;
                        break;
                    case SButton.F7:
                        Game1.timeOfDay = 700;
                        break;
                    case SButton.F8:
                        Game1.timeOfDay = 800;
                        break;
                    case SButton.F9:
                        Game1.timeOfDay = 900;
                        break;
                    case SButton.F10:
                        Game1.timeOfDay = 1000;
                        break;
                    case SButton.F11:
                        Game1.timeOfDay = 1100;
                        break;
                    case SButton.F12:
                        Game1.timeOfDay = 1200;
                        break;
                }
            }

            this.Monitor.Log($"{Game1.currentSeason}", LogLevel.Debug);
        }

        private void ShiftSeasonUp()
        {
            if (Game1.currentSeason == "spring")
            {
                Game1.currentSeason = "summer";
            }
            else if (Game1.currentSeason == "summer")
            {
                Game1.currentSeason = "fall";
            }
            else if (Game1.currentSeason == "fall")
            {
                Game1.currentSeason = "winter";
            }
            else if (Game1.currentSeason == "winter")
            {
                Game1.currentSeason = "spring";
            }
        }

        private void ShiftSeasonDown()
        {
            if (Game1.currentSeason == "spring")
            {
                Game1.currentSeason = "winter";
            }
            else if (Game1.currentSeason == "summer")
            {
                Game1.currentSeason = "spring";
            }
            else if (Game1.currentSeason == "fall")
            {
                Game1.currentSeason = "summer";
            }
            else if (Game1.currentSeason == "winter")
            {
                Game1.currentSeason = "fall";
            }
        }
    }
}