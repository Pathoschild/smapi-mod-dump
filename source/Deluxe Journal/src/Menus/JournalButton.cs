/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace DeluxeJournal.Menus
{
    /// <summary>The replacement journal button for when there are no more active quests.</summary>
    public class JournalButton : IClickableMenu
    {
        public readonly ClickableTextureComponent taskButton;

        private string _hoverText;

        internal JournalButton() : base(Game1.uiViewport.Width - 88, 248, 44, 46)
        {
            _hoverText = string.Empty;

            taskButton = new ClickableTextureComponent(
                new Rectangle(xPositionOnScreen, yPositionOnScreen, width, height),
                DeluxeJournalMod.UiTexture,
                new Rectangle(0, 16, 11, 14),
                4f);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (!Game1.player.hasVisibleQuests && taskButton.containsPoint(x, y) &&
                Game1.player.CanMove && !Game1.dialogueUp && !Game1.eventUp && Game1.farmEvent == null)
            {
                Game1.activeClickableMenu = new QuestLog();
            }
        }

        public override void performHoverAction(int x, int y)
        {
            if (!Game1.player.hasVisibleQuests && taskButton.containsPoint(x, y))
            {
                _hoverText = string.Format(Game1.content.LoadString("Strings\\UI:QuestButton_Hover", Game1.options.journalButton[0].ToString()));
            }
            else
            {
                _hoverText = string.Empty;
            }
        }

        public override void draw(SpriteBatch b)
        {
            if (!Game1.player.hasVisibleQuests)
            {
                Vector2 position = new Vector2(Game1.uiViewport.Width - 88, 248);

                if (Game1.isOutdoorMapSmallerThanViewport())
                {
                    position.X = Math.Min(position.X, Game1.currentLocation.map.Layers[0].LayerWidth * 64 - Game1.uiViewport.X - 88);
                }

                Utility.makeSafe(ref position, width, height);
                xPositionOnScreen = (int)position.X;
                yPositionOnScreen = (int)position.Y;
                taskButton.bounds = new Rectangle(xPositionOnScreen, yPositionOnScreen, width, height);

                taskButton.draw(b);

                if (_hoverText.Length > 0 && isWithinBounds(Game1.getOldMouseX(), Game1.getOldMouseY()))
                {
                    drawHoverText(b, _hoverText, Game1.dialogueFont);
                }
            }
        }
    }
}
