/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/ShroomSpotterRedux
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace ShroomSpotter
{
    public class ModEntry : Mod
    {
        private ModConfig Config;

        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();
            Helper.Events.Input.ButtonPressed += OnButtonPressed;
            Helper.Events.Display.RenderingActiveMenu += OnRenderingActiveMenu;
            Helper.Events.Display.RenderedActiveMenu += OnRenderedActiveMenu;
        }

        #region Events

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button != Config.GetShroomLevels) return;

            var shroomLevels = FindShroomLevels(out int daysTilShroom);
            DisplayShroomMessage(shroomLevels, daysTilShroom);
        }

        private void OnRenderingActiveMenu(object sender, RenderingActiveMenuEventArgs e)
        {
            if (!(Game1.activeClickableMenu is Billboard menu)) return;

            var calendarDays = GetCalendarDays(menu);
            if (calendarDays == null) return;

            var hoverField = Helper.Reflection.GetField<string>(menu, "hoverText");
            var hoverText = hoverField.GetValue();

            if (hoverText.ToLower().Contains("shrooms")) return;

            hoverText = UpdateHoverText(hoverText, calendarDays);

            hoverField.SetValue(hoverText);
        }

        private void OnRenderedActiveMenu(object sender, RenderedActiveMenuEventArgs e)
        {
            if (!(Game1.activeClickableMenu is Billboard menu)) return;

            var calendarDays = GetCalendarDays(menu);
            if (calendarDays == null) return;

            var hoverText = Helper.Reflection.GetField<string>(menu, "hoverText").GetValue();
            DrawShroomsOnCalendar(calendarDays, e.SpriteBatch);

            IClickableMenu.drawHoverText(e.SpriteBatch, hoverText, Game1.dialogueFont);
        }

        #endregion

        #region Helper Methods

        private static List<int> FindShroomLevels(out int daysTilShroom)
        {
            var shroomLevels = new List<int>();
            daysTilShroom = -1;

            while (shroomLevels.Count == 0 && ++daysTilShroom < 50)
            {
                shroomLevels = GetShroomLayers(daysTilShroom);
            }

            return shroomLevels;
        }

        private static void DisplayShroomMessage(List<int> shroomLevels, int daysTilShroom)
        {
            if (shroomLevels.Count > 0)
            {
                string message = daysTilShroom == 0
                    ? $"Shroom layers will spawn on these mine levels: {string.Join(", ", shroomLevels)}"
                    : $"Shrooms will spawn in {daysTilShroom} day(s) on these mine levels: {string.Join(", ", shroomLevels)}";
                Game1.showGlobalMessage(message);
            }
            else
            {
                Game1.showGlobalMessage("No shroom layers will spawn in the next 50 days!");
            }
        }

        private List<ClickableTextureComponent> GetCalendarDays(Billboard menu)
        {
            var calendarDaysField = Helper.Reflection.GetField<List<ClickableTextureComponent>>(menu, nameof(Billboard.calendarDays));
            return calendarDaysField?.GetValue();
        }

        private static string UpdateHoverText(string hoverText, List<ClickableTextureComponent> calendarDays)
        {
            var mouseX = Game1.getMouseX();
            var mouseY = Game1.getMouseY();
            var day = Game1.dayOfMonth;

            var component = calendarDays[day - 1];
            if (component.bounds.Contains(mouseX, mouseY))
            {
                var shrooms = GetShroomLayers(day - Game1.dayOfMonth);
                hoverText += hoverText.Length > 0 ? "\n" : string.Empty;
                hoverText += shrooms.Count > 0 ? $"Shrooms: {string.Join(", ", shrooms)}" : "No shrooms";
            }

            return hoverText;
        }

        private static void DrawShroomsOnCalendar(List<ClickableTextureComponent> calendarDays, SpriteBatch spriteBatch)
        {
            var day = Game1.dayOfMonth;
            var component = calendarDays[day - 1];
            var shrooms = GetShroomLayers(day - Game1.dayOfMonth);

            if (shrooms.Count > 0)
            {
                var source = GameLocation.getSourceRectForObject(422);
                var dest = new Vector2(component.bounds.Right - 8f * Game1.pixelZoom, component.bounds.Y);
                spriteBatch.Draw(Game1.objectSpriteSheet, dest, source, Color.White, 0.0f, Vector2.Zero, Game1.pixelZoom / 2f, SpriteEffects.None, 1f);
            }
        }

        private static List<int> GetShroomLayers(int v)
        {
            var shroomLevels = new List<int>();

            for (var mineLevel = 1; mineLevel < 120; mineLevel++)
            {
                var random = Utility.CreateDaySaveRandom(Game1.stats.DaysPlayed, mineLevel, 4 * mineLevel);

                if (random.NextDouble() < 0.3 && mineLevel > 2) random.NextDouble();
                random.NextDouble();

                if (random.NextDouble() < 0.035 && mineLevel > 80 && mineLevel % 5 != 0)
                {
                    shroomLevels.Add(mineLevel);
                }
            }

            return shroomLevels;
        }

        #endregion
    }
}