/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using DecidedlyShared.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace DecidedlyShared.Ui;

public class MenuBase : IClickableMenu
{
    // Every base menu needs a container, and only a container.
    internal ContainerElement uiContainer;
    private string menuName;
    private string openSound;
    private Logger logger;

    public string MenuName
    {
        get => this.menuName;
    }

    public MenuBase(ContainerElement uiContainer, string name, Logger logger, string openSound = "bigSelect")
    {
        this.uiContainer = uiContainer;
        this.xPositionOnScreen = 0;
        this.yPositionOnScreen = 0;
        this.width = Game1.uiViewport.Width;
        this.height = Game1.uiViewport.Height;
        this.uiContainer.textureTint = Color.White;
        this.menuName = name;
        this.openSound = openSound;
        this.logger = logger;

        this.UpdateCloseButton(this.uiContainer.TopRightCorner);
    }

    public void SetOpenSound(string openSound)
    {
        this.openSound = openSound;
    }

    public void MenuOpened()
    {
        if (!Utilities.Sound.TryPlaySound(this.openSound))
            this.logger.Error($"Oops! I failed while trying to play sound cue {this.openSound} in {Game1.currentLocation.Name}. Is it a valid cue?");
    }

    public override void draw(SpriteBatch b)
    {
        this.uiContainer.Draw(b);
        base.upperRightCloseButton.draw(b);
        this.drawMouse(b);
    }

    public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
    {
        this.xPositionOnScreen = 0;
        this.yPositionOnScreen = 0;
        this.width = Game1.uiViewport.Width;
        this.height = Game1.uiViewport.Height;
        this.uiContainer.OrganiseChildren();
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
        if (this.upperRightCloseButton.containsPoint(x, y))
            this.exitThisMenu();

        this.uiContainer.ReceiveLeftClick(x, y);
    }

    public override void receiveRightClick(int x, int y, bool playSound = true)
    {
        this.uiContainer.ReceiveRightClick(x, y);
    }

    public override void receiveScrollWheelAction(int direction)
    {
        this.uiContainer.ReceiveScrollWheel(direction);
    }

    public void UpdateCloseButton(Vector2 topRightCorner)
    {
        base.upperRightCloseButton = new ClickableTextureComponent(new Rectangle((int)topRightCorner.X, (int)topRightCorner.Y, 48, 48), Game1.mouseCursors, new Rectangle(337, 494, 12, 12), 4f, false);
    }

    public override void releaseLeftClick(int x, int y)
    {
        base.releaseLeftClick(x, y);
    }

    public override void emergencyShutDown()
    {
        base.emergencyShutDown();
    }
}
