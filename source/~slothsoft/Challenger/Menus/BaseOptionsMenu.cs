/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-challenger
**
*************************************************/

using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.Menus;

namespace Slothsoft.Challenger.Menus;

/// <summary>
/// A menu page that displays a selection of options of some kind.
/// This is a more general class to reuse (maybe). After extending this class you need to call
/// <see cref="AddPage"/> to add your page(s coming soon maybe).
/// </summary>
internal class BaseOptionsMenu : IClickableMenu {
    
    private BaseOptionsPage? _page;

    public BaseOptionsMenu()
        : base(Game1.viewport.Width / 2 - (800 + borderWidth * 2) / 2,
            Game1.viewport.Height / 2 - (600 + borderWidth * 2) / 2,
            800 + borderWidth * 2, 
            600 + borderWidth * 2, 
            true) {
        if (Game1.activeClickableMenu == null)
            Game1.playSound("bigSelect");
    }

    public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds) {
        var xDiff = xPositionOnScreen;
        var yDiff = yPositionOnScreen;
        
        base.gameWindowSizeChanged(oldBounds, newBounds);
        _page?.gameWindowSizeChanged(oldBounds, newBounds);
        
        xDiff -= xPositionOnScreen;
        yDiff -= yPositionOnScreen;
        
        // move all items on the menu the same amount as the entire page
        upperRightCloseButton.bounds.X -= xDiff;
        upperRightCloseButton.bounds.Y -= yDiff;
    }
    
    public void AddPage(BaseOptionsPage page) {
        _page = page;
        _page.populateClickableComponentList();
    }

    public override void setUpForGamePadMode() {
        base.setUpForGamePadMode();
        _page?.setUpForGamePadMode();
    }

    public override ClickableComponent? getCurrentlySnappedComponent() {
        return _page?.getCurrentlySnappedComponent();
    }

    public override void setCurrentlySnappedComponentTo(int id) {
        _page?.setCurrentlySnappedComponentTo(id);
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true) {
        base.receiveLeftClick(x, y, playSound);
        _page?.receiveLeftClick(x, y);
    }

    public override void receiveRightClick(int x, int y, bool playSound = true) {
        _page?.receiveRightClick(x, y, playSound);
    }

    public override void receiveScrollWheelAction(int direction) {
        base.receiveScrollWheelAction(direction);
        _page?.receiveScrollWheelAction(direction);
    }

    public override void performHoverAction(int x, int y) {
        base.performHoverAction(x, y);
        _page?.performHoverAction(x, y);
    }

    public override void releaseLeftClick(int x, int y) {
        base.releaseLeftClick(x, y);
        _page?.releaseLeftClick(x, y);
    }

    public override void leftClickHeld(int x, int y) {
        base.leftClickHeld(x, y);
        _page?.leftClickHeld(x, y);
    }

    public override bool readyToClose() {
        return _page?.readyToClose() ?? true;
    }

    public override void draw(SpriteBatch b) {
        if (_page == null) {
            return;
        }
        
        if (!Game1.options.showMenuBackground)
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.4f);
        
        Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, _page.width, _page.height, false, true);
        _page.draw(b);
        b.End();
        b.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, SamplerState.PointClamp);

        base.draw(b);
        if (Game1.options.hardwareCursor)
            return;
        b.Draw(Game1.mouseCursors, new Vector2(Game1.getOldMouseX(), Game1.getOldMouseY()),
            Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors,
                Game1.options.gamepadControls ? 44 : 0, 16, 16), Color.White, 0.0f, Vector2.Zero,
            Game1.pixelZoom + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 1f);
    }

    public override bool areGamePadControlsImplemented() {
        return false;
    }

    public override void receiveKeyPress(Keys key) {
        if (Game1.options.menuButton.Contains(new InputButton(key)) &&
            readyToClose()) {
            Game1.exitActiveMenu();
            Game1.playSound("bigDeSelect");
        }
        _page?.receiveKeyPress(key);
    }
}