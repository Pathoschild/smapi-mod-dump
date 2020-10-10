/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/cilekli-link/SDVMods
**
*************************************************/

using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using StardewValley.Quests;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DarkUI
{
    [HarmonyPatch(typeof(QuestLog))]
    [HarmonyPatch("draw")]
    class QuestLogPatch
    {
#pragma warning disable AvoidImplicitNetFieldCast // Netcode types shouldn't be implicitly converted
        static bool Prefix(SpriteBatch b, QuestLog __instance, List<List<Quest>> ___pages, int ___currentPage, int ___questPage = -1, string ___hoverText = "")
        {
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            SpriteText.drawStringWithScrollCenteredAt(b, Game1.content.LoadString("Strings\\StringsFromCSFiles:QuestLog.cs.11373"), __instance.xPositionOnScreen + __instance.width / 2, __instance.yPositionOnScreen - 64, "", 1f, -1, 0, 0.88f, false);
            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 373, 18, 18), __instance.xPositionOnScreen, __instance.yPositionOnScreen, __instance.width, __instance.height, Color.White, 4f, true);
            if (___questPage == -1)
            {
                for (int index = 0; index < __instance.questLogButtons.Count; ++index)
                {
                    if (___pages.Count() > 0 && ___pages[___currentPage].Count() > index)
                    {
                        IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 396, 15, 15), __instance.questLogButtons[index].bounds.X, __instance.questLogButtons[index].bounds.Y, __instance.questLogButtons[index].bounds.Width, __instance.questLogButtons[index].bounds.Height, __instance.questLogButtons[index].containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY()) ? Color.Wheat : Color.White, 4f, false);
                        if ((bool)((NetFieldBase<bool, NetBool>)___pages[___currentPage][index].showNew) || (bool)((NetFieldBase<bool, NetBool>)___pages[___currentPage][index].completed))
                        {
                            Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2(__instance.questLogButtons[index].bounds.X + 64 + 4, __instance.questLogButtons[index].bounds.Y + 44), new Rectangle((bool)((NetFieldBase<bool, NetBool>)___pages[___currentPage][index].completed) ? 341 : 317, 410, 23, 9), Color.White, 0.0f, new Vector2(11f, 4f), (float)(4.0 + Game1.dialogueButtonScale * 10.0 / 250.0), false, 0.99f, -1, -1, 0.35f);
                        }
                        else
                        {
                            Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2(__instance.questLogButtons[index].bounds.X + 32, __instance.questLogButtons[index].bounds.Y + 28), (bool)((NetFieldBase<bool, NetBool>)___pages[___currentPage][index].dailyQuest) ? new Rectangle(410, 501, 9, 9) : new Rectangle(395 + ((bool)((NetFieldBase<bool, NetBool>)___pages[___currentPage][index].dailyQuest) ? 3 : 0), 497, 3, 8), Color.White, 0.0f, Vector2.Zero, 4f, false, 0.99f, -1, -1, 0.35f);
                        }

                        int num = (bool)((NetFieldBase<bool, NetBool>)___pages[___currentPage][index].dailyQuest) ? 1 : 0;
                        SpriteText.drawString(b, ___pages[___currentPage][index].questTitle, __instance.questLogButtons[index].bounds.X + 128 + 4, __instance.questLogButtons[index].bounds.Y + 20, 999999, -1, 999999, 1f, 0.88f, false, -1, "", -1);
                    }
                }
            }
            else
            {
                SpriteText.drawStringHorizontallyCenteredAt(b, ___pages[___currentPage][___questPage].questTitle, __instance.xPositionOnScreen + __instance.width / 2 + (!(bool)((NetFieldBase<bool, NetBool>)___pages[___currentPage][___questPage].dailyQuest) || (int)((NetFieldBase<int, NetInt>)___pages[___currentPage][___questPage].daysLeft) <= 0 ? 0 : Math.Max(32, SpriteText.getWidthOfString(___pages[___currentPage][___questPage].questTitle, 999999) / 3) - 32), __instance.yPositionOnScreen + 32, 999999, -1, 999999, 1f, 0.88f, false, -1, 99999);
                if ((bool)((NetFieldBase<bool, NetBool>)___pages[___currentPage][___questPage].dailyQuest) && (int)((NetFieldBase<int, NetInt>)___pages[___currentPage][___questPage].daysLeft) > 0)
                {
                    Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2(__instance.xPositionOnScreen + 32, __instance.yPositionOnScreen + 48 - 8), new Rectangle(410, 501, 9, 9), Color.White, 0.0f, Vector2.Zero, 4f, false, 0.99f, -1, -1, 0.35f);
                    Utility.drawTextWithShadow(b, Game1.parseText((int)((NetFieldBase<int, NetInt>)___pages[___currentPage][___questPage].daysLeft) > 1 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:QuestLog.cs.11374", ___pages[___currentPage][___questPage].daysLeft) : Game1.content.LoadString("Strings\\StringsFromCSFiles:QuestLog.cs.11375", ___pages[___currentPage][___questPage].daysLeft), Game1.dialogueFont, __instance.width - 128), Game1.dialogueFont, new Vector2(__instance.xPositionOnScreen + 80, __instance.yPositionOnScreen + 48 - 8), Game1.textColor, 1f, -1f, -1, -1, 1f, 3);
                }
                Utility.drawTextWithShadow(b, Game1.parseText(___pages[___currentPage][___questPage].questDescription, Game1.dialogueFont, __instance.width - 128), Game1.dialogueFont, new Vector2(__instance.xPositionOnScreen + 64, __instance.yPositionOnScreen + 96), Game1.textColor, 1f, -1f, -1, -1, 1f, 3);
                float y = (float)(__instance.yPositionOnScreen + 96 + (double)Game1.dialogueFont.MeasureString(Game1.parseText(___pages[___currentPage][___questPage].questDescription, Game1.dialogueFont, __instance.width - 128)).Y + 32.0);
                if ((bool)((NetFieldBase<bool, NetBool>)___pages[___currentPage][___questPage].completed))
                {
                    SpriteText.drawString(b, Game1.content.LoadString("Strings\\StringsFromCSFiles:QuestLog.cs.11376"), __instance.xPositionOnScreen + 32 + 4, __instance.rewardBox.bounds.Y + 21 + 4, 999999, -1, 999999, 1f, 0.88f, false, -1, "", -1);
                    __instance.rewardBox.draw(b);
                    if ((int)((NetFieldBase<int, NetInt>)___pages[___currentPage][___questPage].moneyReward) > 0)
                    {
                        b.Draw(Game1.mouseCursors, new Vector2(__instance.rewardBox.bounds.X + 16, __instance.rewardBox.bounds.Y + 16 - Game1.dialogueButtonScale / 2f), new Rectangle?(new Rectangle(280, 410, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
                        SpriteText.drawString(b, Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11020", ___pages[___currentPage][___questPage].moneyReward), __instance.xPositionOnScreen + 448, __instance.rewardBox.bounds.Y + 21 + 4, 999999, -1, 999999, 1f, 0.88f, false, -1, "", -1);
                    }
                }
                else
                {
                    Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2(__instance.xPositionOnScreen + 96 + (float)(8.0 * Game1.dialogueButtonScale / 10.0), y), new Rectangle(412, 495, 5, 4), Color.White, 1.570796f, Vector2.Zero, -1f, false, -1f, -1, -1, 0.35f);
                    Utility.drawTextWithShadow(b, Game1.parseText(___pages[___currentPage][___questPage].currentObjective, Game1.dialogueFont, __instance.width - 256), Game1.dialogueFont, new Vector2(__instance.xPositionOnScreen + 128, y - 8f), Color.Yellow, 1f, -1f, -1, -1, 1f, 3);
                    if ((bool)((NetFieldBase<bool, NetBool>)___pages[___currentPage][___questPage].canBeCancelled))
                    {
                        __instance.cancelQuestButton.draw(b);
                    }
                }
            }
            if (___currentPage < ___pages.Count - 1 && ___questPage == -1)
            {
                __instance.forwardButton.draw(b);
            }

            if (___currentPage > 0 || ___questPage != -1)
            {
                __instance.backButton.draw(b);
            }

            //__instance.draw(b);
            Game1.mouseCursorTransparency = 1f;
            __instance.drawMouse(b);
            if (___hoverText.Length <= 0)
            {
                return false;
            }

            IClickableMenu.drawHoverText(b, ___hoverText, Game1.dialogueFont, 0, 0, -1, null, -1, null, null, 0, -1, -1, -1, -1, 1f, null);
            return false;
        }
    }
}
