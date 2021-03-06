/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
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
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;

namespace MailFrameworkMod
{
    public class LetterViewerMenuExtended : LetterViewerMenu
    {
        public int? TextColor { get; set; }

        public LetterViewerMenuExtended(string text) : base(text) {}

        public LetterViewerMenuExtended(int secretNoteIndex) : base(secretNoteIndex) {}

        public LetterViewerMenuExtended(string mail, string mailTitle, bool fromCollection = false) : base(mail, mailTitle, fromCollection) {}

        public override int getTextColor()
        {
            return TextColor ?? base.getTextColor();
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if ((double)MailFrameworkModEntry.ModHelper.Reflection.GetField<float>(this, "scale").GetValue() < 1.0)
                return;

            if (this.upperRightCloseButton == null || !this.readyToClose() ||
                !this.upperRightCloseButton.containsPoint(x, y))
            {
                if (Game1.activeClickableMenu != null || Game1.currentMinigame != null)
                {
                    if (this.itemsToGrab.Count > 0)
                    {
                        ClickableComponent clickableComponent = this.itemsToGrab.Last();
                        if (clickableComponent.containsPoint(x, y) && clickableComponent.item != null)
                        {
                            Game1.playSound("coin");
                            Game1.player.addItemByMenuIfNecessary(clickableComponent.item, null);
                            if (Game1.activeClickableMenu != null && Game1.activeClickableMenu != this)
                            {
                                Game1.activeClickableMenu.exitFunction = (IClickableMenu.onExit)Delegate.Combine(Game1.activeClickableMenu.exitFunction, new IClickableMenu.onExit(() => Game1.activeClickableMenu = this));
                            }
                            clickableComponent.item = (Item)null;
                            if (this.itemsToGrab.Count > 1)
                            {
                                this.itemsToGrab.Remove(clickableComponent);
                            }
                            return;
                        }
                    }
                }
            }

            base.receiveLeftClick(x, y, playSound);
        }
    }
}
