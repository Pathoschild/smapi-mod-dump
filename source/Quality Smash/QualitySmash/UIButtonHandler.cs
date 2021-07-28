/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jn84/QualitySmash
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace QualitySmash
{
    internal class UiButtonHandler
    {
        private const int Length = 64;
        private const int PositionFromBottom = 3;
        private const int GapSize = 16;

        private readonly List<QSButton> qsButtons;
        private readonly ModEntry modEntry;

        public UiButtonHandler(ModEntry modEntry)
        {
            qsButtons = new List<QSButton>();

            this.modEntry = modEntry;
        }

        public void AddButton(ModEntry.SmashType smashType, Texture2D texture, Rectangle clickableArea)
        {
            // Make sure button doesn't already exist
            if (qsButtons.Any(button => button.smashType == smashType))
                return;

            QSButton newButton = new QSButton(smashType, texture, clickableArea);

            newButton.UpdateHoverText("");

            qsButtons.Add(newButton);
        }

        public void RemoveButton(ModEntry.SmashType smashType)
        {
            for (int i = 0; i < qsButtons.Count; i++)
            {
                if (qsButtons[i].smashType == smashType)
                {
                    qsButtons.RemoveAt(i);
                    return;
                }
            }
        }

        public void UpdateBounds(IClickableMenu menu)
        {
            var screenX = menu.xPositionOnScreen + menu.width + GapSize + Length;
            var screenY = menu.yPositionOnScreen + menu.height / 3 - (Length * PositionFromBottom) - (GapSize * (PositionFromBottom - 1));

            for (int i = 0; i < qsButtons.Count; i++)
                qsButtons[i].SetBounds(screenX, screenY + (i * (GapSize + Length)), Length);
        }

        public void TryHover(float x, float y)
        {
            foreach (QSButton button in qsButtons)
            {
                if (button.ContainsPoint((int) x, (int) y))
                {
                    button.UpdateHoverText(
                        modEntry.helper.Translation.Get(ModEntry.TranslationMapping[button.smashType]));
                }
                else
                    button.UpdateHoverText("");

                button.TryHover((int)x, (int)y);
            }
        }

        public void DrawButtons()
        {
            foreach (QSButton button in qsButtons)
            {
                button.DrawButton(Game1.spriteBatch);
            }

            // Redraw cursor so it doesn't hide under QS buttons
            Game1.spriteBatch.Draw(Game1.mouseCursors, new Vector2(Game1.getOldMouseX(), Game1.getOldMouseY()), 
                Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 0, 16, 16), Color.White, 0f, Vector2.Zero,
                4f + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 0);
        }

        public ModEntry.SmashType GetButtonClicked(int x, int y)
        {
            foreach (var button in qsButtons)
            {
                if (button.ContainsPoint(x, y))
                    return button.smashType;
            }

            return ModEntry.SmashType.None;
        }
    }
}
