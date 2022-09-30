/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace Omegasis.HappyBirthday.Framework.Utilities
{
    public static class RenderUtilities
    {
        /// <summary>Raised after drawing the HUD (item toolbar, clock, etc) to the sprite batch, but before it's rendered to the screen.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        public static void OnRenderedHud(object sender, RenderedHudEventArgs e)
        {
            if (Game1.activeClickableMenu == null || HappyBirthdayModCore.Instance.birthdayManager.playerBirthdayData?.BirthdaySeason?.ToLower() != Game1.currentSeason.ToLower())
                return;

            if (Game1.activeClickableMenu is Billboard billboard)
            {
                if (MenuUtilities.IsDailyQuestBoard || billboard.calendarDays == null)
                    return;

                string hoverText = "";
                List<string> texts = new List<string>();

                foreach (var clicky in billboard.calendarDays)
                {
                    if (clicky.containsPoint(Game1.getMouseX(), Game1.getMouseY()))
                    {
                        if (!string.IsNullOrEmpty(clicky.hoverText))
                            texts.Add(clicky.hoverText); //catches npc birhday names.
                        else if (!string.IsNullOrEmpty(clicky.name))
                            texts.Add(clicky.name); //catches festival dates.
                    }
                }

                for (int i = 0; i < texts.Count; i++)
                {
                    hoverText += texts[i]; //Append text.
                    if (i != texts.Count - 1)
                        hoverText += Environment.NewLine; //Append new line.
                }

                if (!string.IsNullOrEmpty(hoverText))
                {
                    var oldText = HappyBirthdayModCore.Instance.Helper.Reflection.GetField<string>(Game1.activeClickableMenu, "hoverText");
                    oldText.SetValue(hoverText);
                }
            }
        }

        /// <summary>When a menu is open (<see cref="Game1.activeClickableMenu"/> isn't null), raised after that menu is drawn to the sprite batch but before it's rendered to the screen.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        public static void OnRenderedActiveMenu(object sender, RenderedActiveMenuEventArgs e)
        {
            if (Game1.activeClickableMenu == null || MenuUtilities.IsDailyQuestBoard)
                return;

            //Don't do anything if birthday has not been chosen yet.
            if (HappyBirthdayModCore.Instance.birthdayManager.playerBirthdayData == null)
                return;

            if (Game1.activeClickableMenu is Billboard)
            {
                if (!string.IsNullOrEmpty(HappyBirthdayModCore.Instance.birthdayManager.playerBirthdayData.BirthdaySeason))
                {
                    if (HappyBirthdayModCore.Instance.birthdayManager.playerBirthdayData.BirthdaySeason.ToLower() == Game1.currentSeason.ToLower())
                    {
                        int index = HappyBirthdayModCore.Instance.birthdayManager.playerBirthdayData.BirthdayDay;
                        Game1.player.FarmerRenderer.drawMiniPortrat(Game1.spriteBatch, new Vector2(Game1.activeClickableMenu.xPositionOnScreen + 152 + (index - 1) % 7 * 32 * 4, Game1.activeClickableMenu.yPositionOnScreen + 230 + (index - 1) / 7 * 32 * 4), 0.5f, 4f, 2, Game1.player);
                        (Game1.activeClickableMenu as Billboard).drawMouse(e.SpriteBatch);

                        string hoverText = HappyBirthdayModCore.Instance.Helper.Reflection.GetField<string>((Game1.activeClickableMenu as Billboard), "hoverText", true).GetValue();
                        if (hoverText.Length > 0)
                        {
                            IClickableMenu.drawHoverText(Game1.spriteBatch, hoverText, Game1.dialogueFont, 0, 0, -1, (string)null, -1, (string[])null, (Item)null, 0, -1, -1, -1, -1, 1f, (CraftingRecipe)null);
                        }
                    }
                }

                foreach (var pair in HappyBirthdayModCore.Instance.birthdayManager.othersBirthdays)
                {
                    int index = pair.Value.BirthdayDay;
                    if (pair.Value.BirthdaySeason != Game1.currentSeason.ToLower()) continue; //Hide out of season birthdays.
                    index = pair.Value.BirthdayDay;
                    Game1.player.FarmerRenderer.drawMiniPortrat(Game1.spriteBatch, new Vector2(Game1.activeClickableMenu.xPositionOnScreen + 152 + (index - 1) % 7 * 32 * 4, Game1.activeClickableMenu.yPositionOnScreen + 230 + (index - 1) / 7 * 32 * 4), 0.5f, 4f, 2, Game1.getFarmer(pair.Key));
                    (Game1.activeClickableMenu as Billboard).drawMouse(e.SpriteBatch);

                    string hoverText = HappyBirthdayModCore.Instance.Helper.Reflection.GetField<string>((Game1.activeClickableMenu as Billboard), "hoverText", true).GetValue();
                    if (hoverText.Length > 0)
                    {
                        IClickableMenu.drawHoverText(Game1.spriteBatch, hoverText, Game1.dialogueFont, 0, 0, -1, (string)null, -1, (string[])null, (Item)null, 0, -1, -1, -1, -1, 1f, (CraftingRecipe)null);
                    }
                }
                (Game1.activeClickableMenu).drawMouse(e.SpriteBatch);

            }
        }

    }
}
