/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/unidarkshin/Stardew-Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Tools;
using StardewValley.Menus;
using StardewValley.BellsAndWhistles;
using StardewValley.Objects;
using System.Timers;
using StardewValley.TerrainFeatures;
using StardewValley.Monsters;
using StardewValley.Locations;
using System.Collections;

namespace AdditionalSkillsBase
{
    /// <summary>The mod entry point.</summary>
    /// 
    public class ModEntry : Mod
    {
        public static Random rnd;
        public static Mod instance;

        public static Thieving th;
        public bool th_check = false;

        public static Magic mg;
        public bool mg_check = false;

        public bool shouldDraw = false;
        public string shouldDraw2;

        public int[] skillIndex = { 1, 2, 3, 4, 5 };

        public ModEntry()
        {
            instance = this;


        }

        public override void Entry(IModHelper helper)
        {
            rnd = new Random();
            shouldDraw2 = "";

            helper.Events.Display.Rendered += GraphicsEvents_OnPostRenderEvent;
            helper.Events.GameLoop.SaveLoaded += SaveEvents_AfterLoad;
            helper.Events.Input.ButtonPressed += InputEvents_ButtonPressed;


        }

        private void InputEvents_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (Context.IsWorldReady && Game1.activeClickableMenu is GameMenu)
            {
                List<IClickableMenu> tabs = this.Helper.Reflection.GetField<List<IClickableMenu>>(Game1.activeClickableMenu, "pages").GetValue();
                IClickableMenu curTab = tabs[(Game1.activeClickableMenu as GameMenu).currentTab];

                if (curTab is SkillsPage && e.Button == SButton.S)
                {
                    if (!shouldDraw)
                        shouldDraw = true;
                    else
                        shouldDraw = false;
                }
                else if (curTab is SkillsPage && e.Button == SButton.MouseLeft)
                {
                    if (shouldDraw && Game1.getMouseY() >= curTab.yPositionOnScreen + (100 * skillIndex[0]) && Game1.getMouseY() <= curTab.yPositionOnScreen + (100 * skillIndex[0]) + 100)
                    {
                        shouldDraw2 = "th";
                        shouldDraw = false;
                    }
                    else if (!shouldDraw && shouldDraw2 != "")
                    {
                        shouldDraw2 = "";
                        shouldDraw = true;
                    }
                }
            }
        }

        private void SaveEvents_AfterLoad(object sender, EventArgs e)
        {
            th = new Thieving(instance);
            mg = new Magic(instance);
            mg_check = true;
            th_check = true;
        }

        private void GraphicsEvents_OnPostRenderEvent(object sender, EventArgs e)
        {
            if (Context.IsWorldReady && Game1.activeClickableMenu is GameMenu)
            {
                List<IClickableMenu> tabs = this.Helper.Reflection.GetField<List<IClickableMenu>>(Game1.activeClickableMenu, "pages").GetValue();
                IClickableMenu curTab = tabs[(Game1.activeClickableMenu as GameMenu).currentTab];

                if (curTab is SkillsPage && shouldDraw)
                {
                    Game1.drawDialogueBox(curTab.xPositionOnScreen, curTab.yPositionOnScreen, curTab.width, curTab.height, false, true, (string)null, false);

                    if (th != null)
                    {
                        Game1.spriteBatch.DrawString(Game1.smallFont, $"Thieving: {th.level}", new Vector2(curTab.xPositionOnScreen + 40, curTab.yPositionOnScreen + (100 * skillIndex[0])), Color.Black, 0.0f, Vector2.Zero, (float)(Game1.pixelZoom / 2), SpriteEffects.None, 0.5f);
                        Game1.spriteBatch.Draw(Game1.staminaRect, new Rectangle(curTab.xPositionOnScreen + 40, curTab.yPositionOnScreen + (100 * skillIndex[0]) + 65, 300, 40), Color.Black);
                        Game1.spriteBatch.Draw(Game1.staminaRect, new Rectangle(curTab.xPositionOnScreen + 43, curTab.yPositionOnScreen + (100 * skillIndex[0]) + 68, (int)((th.xp / (double)((th.level * th.level * th.level) + 30)) * 294.0), 34), Color.LightBlue);


                    }
                    Game1.spriteBatch.Draw(Game1.mouseCursors, new Vector2(Game1.getMouseX(), Game1.getMouseY()), Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, Game1.options.SnappyMenus ? 44 : 0, 16, 16), Color.White * Game1.mouseCursorTransparency, 0.0f, Vector2.Zero, Game1.pixelZoom + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 0.1f);
                }
                else if (curTab is SkillsPage && shouldDraw2 != "")
                {
                    Game1.drawDialogueBox(curTab.xPositionOnScreen - 400, curTab.yPositionOnScreen, curTab.width + 800, curTab.height, false, true, (string)null, false);

                    if (shouldDraw2 == "th")
                    {
                        Game1.spriteBatch.DrawString(Game1.smallFont, $"Thieving Perks:", new Vector2(curTab.xPositionOnScreen + (int)(curTab.width / 2.0) - 90, curTab.yPositionOnScreen + 100), Color.Black, 0.0f, Vector2.Zero, (float)(Game1.pixelZoom / 3), SpriteEffects.None, 0.5f);
                        for (int i = 1; i < 11; i++)
                        {
                            Game1.spriteBatch.DrawString(Game1.smallFont, $"{th.perksInfo[i - 1]}", new Vector2(curTab.xPositionOnScreen + 40, curTab.yPositionOnScreen + (50 * (i + 2))), Color.DarkViolet * (float)(20.0f/Math.Abs((float)(curTab.yPositionOnScreen + (50 * (i + 2)) + 5) - (float)Game1.getMouseY())), 0.0f, Vector2.Zero, (float)(Game1.pixelZoom / 3), SpriteEffects.None, 0.5f);
                        }
                    }

                    Game1.spriteBatch.Draw(Game1.mouseCursors, new Vector2(Game1.getMouseX(), Game1.getMouseY()), Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, Game1.options.SnappyMenus ? 44 : 0, 16, 16), Color.White * Game1.mouseCursorTransparency, 0.0f, Vector2.Zero, Game1.pixelZoom + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 0.1f);
                }
            }
        }

    }
}
