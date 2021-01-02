/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ha1fdaew/PlayerIncomeStats
**
*************************************************/

using Microsoft.Xna.Framework;
using PlayerIncomeStats.Core;
using PlayerIncomeStats.Utils;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;
using System.Linq;

namespace PlayerIncomeStats.Client.UI
{
    public class CustomPanel
    {
        public Rectangle transform;
        private readonly IModHelper helper = ModEntry.instance.Helper;
        private float entryHeight = 0;
        private float incomeXOffset = 180;
        private float maxIncomeSWidth;
        private float maxTempIncomeSWidth;
        private Point oldMousePos;
        private bool show = true;
        private string tempIncomeSumS;
        private float tempIncomeXOffset = 20;
        private bool translateMode;
        private List<DataBlock> notEmptyEntries;

        public CustomPanel(Rectangle transform)
        {
            this.transform = transform;
        }

        private List<DataBlock> Entries => ModEntry.instance.networkManager.entries;
        private float TempIncomeX => transform.X + 10 + incomeXOffset + maxIncomeSWidth + tempIncomeXOffset;
        private float TempIncomeXRelative => TempIncomeX - transform.X;

        public void Draw()
        {
            if (HotkeyPressed())
                show = !show;
            if (!show) return;

            IClickableMenu.drawTextureBox(
                Game1.spriteBatch,
                Game1.mouseCursors,
                new Rectangle(293, 360, 24, 24),
                transform.X, transform.Y, transform.Width, transform.Height,
                OnMouseColor(Color.Wheat, Color.White)
            );
            if (notEmptyEntries != null && notEmptyEntries.Count != 0)
            {
                for (int i = 0; i < notEmptyEntries.Count; i++)
                {
                    if (notEmptyEntries[i].justChangedTimer != 0)
                        notEmptyEntries[i].justChangedTimer--;
                    DrawField(notEmptyEntries[i].nameS, 0, i);
                    DrawField(notEmptyEntries[i].overallIncomeS, incomeXOffset, i);
                    if (notEmptyEntries[i].justChangedTimer != 0)
                        IClickableMenu.drawTextureBox(
                            Game1.spriteBatch,
                            Game1.mouseCursors,
                            new Rectangle(293, 360, 24, 24),
                            (int)(transform.X + 10 + incomeXOffset + maxIncomeSWidth + 20),
                            transform.Y + 10 + i * ((int)entryHeight + 5),
                            (int)notEmptyEntries[i].tempIncomeSWidth, (int)entryHeight,
                            notEmptyEntries[i].justChangedColor,
                            drawShadow: false
                        );
                    DrawField(notEmptyEntries[i].tempIncomeS, tempIncomeXOffset, i, true);
                }
                DrawField(tempIncomeSumS, TempIncomeXRelative - 10, notEmptyEntries.Count);
            }
            DrawThink();
        }

        public void DrawThink()
        {
            translateMode = Game1.mouseClickPolling != 0 && (translateMode || MouseHovered());
            if (!translateMode) oldMousePos = new Point(-1, -1);
            Translate();
        }

        public bool HotkeyPressed()
            => helper.Input.GetState(SButton.LeftShift) == SButtonState.Pressed && helper.Input.GetState(SButton.V) == SButtonState.Pressed
            || helper.Input.IsDown(SButton.LeftShift) && helper.Input.GetState(SButton.V) == SButtonState.Pressed;

        public void OnEntriesChanged()
        {
            try
            {
                notEmptyEntries = Entries.Where(db => !string.IsNullOrEmpty(db.Name)).ToList();
                tempIncomeSumS = "(+" + notEmptyEntries.Sum(db => db.MoneyMadeTemp).ToString() + ")";
                maxIncomeSWidth = notEmptyEntries.Max(db => db.overallIncomeSWidth);
                maxTempIncomeSWidth = System.Math.Max(notEmptyEntries.Max(db => db.tempIncomeSWidth), Game1.smallFont.MeasureString(tempIncomeSumS).X);
                entryHeight = Game1.smallFont.MeasureString("TEXT").Y;
                transform.Height = (int)(entryHeight * (notEmptyEntries.Count + 1) + 20 + 5 * notEmptyEntries.Count);
                transform.Width = (int)(TempIncomeXRelative + maxTempIncomeSWidth + 10);
            }
            catch { ModEntry.LogDebug("There was an error updating GUI"); }
        }

        public void Translate()
        {
            if (!translateMode) return;
            if (oldMousePos.X == -1 || oldMousePos.Y == -1)
            {
                oldMousePos = Game1.getMousePosition();
                return;
            }
            Point newMousePos = Game1.getMousePosition();
            Point delta = new Point(newMousePos.X - oldMousePos.X, newMousePos.Y - oldMousePos.Y);
            transform.X += delta.X;
            transform.Y += delta.Y;
            oldMousePos = newMousePos;
        }

        private void DrawField(string fieldValue, float xOffset, int num, bool tempIncome = false)
        {
            Game1.spriteBatch.DrawString(
                Game1.smallFont,
                fieldValue,
                new Vector2(
                    tempIncome ? TempIncomeX : transform.X + 10 + xOffset,
                    transform.Y + 10 + num * (entryHeight + 5)
                ),
                tempIncome && notEmptyEntries[num].justChangedTimer != 0 ? Color.White : Color.Black
            );
        }

        private bool MouseHovered()
        {
            int x = Game1.getMouseX();
            int y = Game1.getMouseY();
            return
                x.OnIntervalStrict(transform.X, transform.X + transform.Width)
                && y.OnIntervalStrict(transform.Y, transform.Y + transform.Height);
        }

        private Color OnMouseColor(Color def, Color onMouse)
                    => MouseHovered() ? onMouse : def;
    }
}