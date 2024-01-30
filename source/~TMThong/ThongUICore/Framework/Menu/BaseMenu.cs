/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TMThong/Stardew-Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThongUICore.Framework.Menu
{
    public class BaseMenu : IClickableMenu
    {

        public string MenuId { get; set; }

        



        public override void applyMovementKey(int direction)
        {
            base.applyMovementKey(direction);
        }

        public override bool areGamePadControlsImplemented()
        {
            return base.areGamePadControlsImplemented();
        }

        public override bool autoCenterMouseCursorForGamepad()
        {
            return base.autoCenterMouseCursorForGamepad();
        }

        public override void automaticSnapBehavior(int direction, int oldRegion, int oldID)
        {
            base.automaticSnapBehavior(direction, oldRegion, oldID);
        }

        public override void clickAway()
        {
            base.clickAway();
        }

        public override void draw(SpriteBatch b, int red = -1, int green = -1, int blue = -1)
        {
            base.draw(b, red, green, blue);
        }

        public override void draw(SpriteBatch b)
        {
            base.draw(b);
        }

        public override void drawBackground(SpriteBatch b)
        {
            base.drawBackground(b);
        }

        public override void emergencyShutDown()
        {
            base.emergencyShutDown();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override void gamePadButtonHeld(Buttons b)
        {
            base.gamePadButtonHeld(b);
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            base.gameWindowSizeChanged(oldBounds, newBounds);
        }

        public override ClickableComponent getCurrentlySnappedComponent()
        {
            return base.getCurrentlySnappedComponent();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool HasFocus()
        {
            return base.HasFocus();
        }

        public override bool IsActive()
        {
            return base.IsActive();
        }

        public override bool IsAutomaticSnapValid(int direction, ClickableComponent a, ClickableComponent b)
        {
            return base.IsAutomaticSnapValid(direction, a, b);
        }

        public override bool isWithinBounds(int x, int y)
        {
            return base.isWithinBounds(x, y);
        }

        public override void leftClickHeld(int x, int y)
        {
            base.leftClickHeld(x, y);
        }

        public override bool overrideSnappyMenuCursorMovementBan()
        {
            return base.overrideSnappyMenuCursorMovementBan();
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
        }

        public override bool readyToClose()
        {
            return base.readyToClose();
        }

        public override void receiveGamePadButton(Buttons b)
        {
            base.receiveGamePadButton(b);
        }

        public override void receiveKeyPress(Keys key)
        {
            base.receiveKeyPress(key);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            base.receiveRightClick(x, y, playSound);
        }

        public override void receiveScrollWheelAction(int direction)
        {
            base.receiveScrollWheelAction(direction);
        }

        public override void releaseLeftClick(int x, int y)
        {
            base.releaseLeftClick(x, y);
        }

        public override void setCurrentlySnappedComponentTo(int id)
        {
            base.setCurrentlySnappedComponentTo(id);
        }

        public override void setUpForGamePadMode()
        {
            base.setUpForGamePadMode();
        }

        public override bool shouldClampGamePadCursor()
        {
            return base.shouldClampGamePadCursor();
        }

        public override bool shouldDrawCloseButton()
        {
            return base.shouldDrawCloseButton();
        }

        public override bool showWithoutTransparencyIfOptionIsSet()
        {
            return base.showWithoutTransparencyIfOptionIsSet();
        }

        public override void snapCursorToCurrentSnappedComponent()
        {
            base.snapCursorToCurrentSnappedComponent();
        }

        public override void snapToDefaultClickableComponent()
        {
            base.snapToDefaultClickableComponent();
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override void update(GameTime time)
        {
            base.update(time);
        }

        protected override void actionOnRegionChange(int oldRegion, int newRegion)
        {
            base.actionOnRegionChange(oldRegion, newRegion);
        }

        protected override void cleanupBeforeExit()
        {
            base.cleanupBeforeExit();
        }

        protected override void customSnapBehavior(int direction, int oldRegion, int oldID)
        {
            base.customSnapBehavior(direction, oldRegion, oldID);
        }

        protected override void noSnappedComponentFound(int direction, int oldRegion, int oldID)
        {
            base.noSnappedComponentFound(direction, oldRegion, oldID);
        }

        protected override bool _ShouldAutoSnapPrioritizeAlignedElements()
        {
            return base._ShouldAutoSnapPrioritizeAlignedElements();
        }
    }
}
