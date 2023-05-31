/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace TerrainFeatureRefresh;

public class TfrMainMenu : IClickableMenu
{
    private Button resetButton;
    private Texture2D boxTexture;
    // private Texture2D buttonPanelTexture;
    private Rectangle titleBounds;
    private Rectangle mainWindowBounds;
    private Rectangle buttonPanelBounds;

    private List<Checkbox> checkboxes;

    // Objects
    private Checkbox fences;
    private Checkbox weeds;
    private Checkbox twigs;
    private Checkbox stones;
    private Checkbox forage;

    // Terrain Features
    private Checkbox grass;
    private Checkbox wildTrees;
    private Checkbox fruitTrees;
    private Checkbox paths;
    private Checkbox hoeDirt;
    private Checkbox crops;
    private Checkbox bushes;

    // Resource Clumps
    private Checkbox stumps;
    private Checkbox logs;
    private Checkbox boulders;
    private Checkbox meteorites;

    private string resetButtonText;
    private string terrainFeatureHeader = "Terrain Features";
    private string objectHeader = "Objects";
    private string clumpHeader = "Resource Clumps";

    public TfrMainMenu(int screenX, int screenY, int width, int height)
        : base(screenX, screenY, width, height, true)
    {
        this.boxTexture = Game1.menuTexture;
        // this.buttonPanelTexture = Game1.content.Load<Texture2D>("Mods/DecidedlyHuman/TFR/ButtonPanel");
        this.resetButton = new Button(
            new Rectangle(
                this.xPositionOnScreen + 64,
                this.yPositionOnScreen + 64,
                0,
                0),
            "Reset Selected");
        this.checkboxes = new List<Checkbox>();
        this.allClickableComponents = new List<ClickableComponent>();

        // Objects
        this.checkboxes.Add(this.fences = new Checkbox(Rectangle.Empty, "Fences"));
        this.checkboxes.Add(this.weeds = new Checkbox(Rectangle.Empty, "Weeds"));
        this.checkboxes.Add(this.twigs = new Checkbox(Rectangle.Empty, "Twigs"));
        this.checkboxes.Add(this.stones = new Checkbox(Rectangle.Empty, "Stones"));
        this.checkboxes.Add(this.forage = new Checkbox(Rectangle.Empty, "Forage"));

        // Terrain Features
        this.checkboxes.Add(this.grass = new Checkbox(Rectangle.Empty, "Grass"));
        this.checkboxes.Add(this.wildTrees = new Checkbox(Rectangle.Empty, "Wild Trees"));
        this.checkboxes.Add(this.fruitTrees = new Checkbox(Rectangle.Empty, "Fruit Trees"));
        this.checkboxes.Add(this.paths = new Checkbox(Rectangle.Empty, "Paths"));
        this.checkboxes.Add(this.hoeDirt = new Checkbox(Rectangle.Empty, "Hoed Dirt"));
        this.checkboxes.Add(this.crops = new Checkbox(Rectangle.Empty, "Crops"));
        this.checkboxes.Add(this.bushes = new Checkbox(Rectangle.Empty, "Bushes"));

        // Resource Clumps
        this.checkboxes.Add(this.stumps = new Checkbox(Rectangle.Empty, "Stumps"));
        this.checkboxes.Add(this.logs = new Checkbox(Rectangle.Empty, "Logs"));
        this.checkboxes.Add(this.boulders = new Checkbox(Rectangle.Empty, "Boulders"));
        this.checkboxes.Add(this.meteorites = new Checkbox(Rectangle.Empty, "Spoiler Rocks"));

        foreach (Checkbox box in this.checkboxes)
        {
            this.allClickableComponents.Add(box);
        }

        this.allClickableComponents.Add(this.resetButton);

        this.UpdateBounds();
    }

    private void UpdateBounds()
    {
        this.titleBounds = new Rectangle(
            this.xPositionOnScreen,
            this.yPositionOnScreen - 64 + 32 + 16 + 8,
            this.width - 128,
            128);

        this.mainWindowBounds = new Rectangle(
            this.xPositionOnScreen,
            this.yPositionOnScreen + 32,
            this.width,
            this.height - 32);

        this.buttonPanelBounds = new Rectangle(
            this.xPositionOnScreen + this.width - 256 + 128 + 32 + 8,
            this.yPositionOnScreen + this.height - 104,
            this.width - 256 - 32,
            128
            );

        this.resetButton.bounds = new Rectangle(
            this.mainWindowBounds.Right - this.resetButton.bounds.Width - 16,
            this.mainWindowBounds.Bottom - this.resetButton.bounds.Height - 16,
            this.resetButton.bounds.Width,
            this.resetButton.bounds.Height);
        new Vector2(this.xPositionOnScreen + 16, this.yPositionOnScreen + 64 - 8);

        #region Objects

        int objectY = this.yPositionOnScreen + 64 + 24;

        this.fences.bounds = new Rectangle(
            this.xPositionOnScreen + 256 + 16,
            objectY,
            this.fences.bounds.Width,
            this.fences.bounds.Height);

        objectY += this.fences.bounds.Height + 6;

        this.weeds.bounds = new Rectangle(
            this.xPositionOnScreen + 256 + 16,
            objectY,
            this.fences.bounds.Width,
            this.fences.bounds.Height);

        objectY += this.fences.bounds.Height + 6;

        this.stones.bounds = new Rectangle(
            this.xPositionOnScreen + 256 + 16,
            objectY,
            this.fences.bounds.Width,
            this.fences.bounds.Height);

        objectY += this.fences.bounds.Height + 6;

        this.twigs.bounds = new Rectangle(
            this.xPositionOnScreen + 256 + 16,
            objectY,
            this.fences.bounds.Width,
            this.fences.bounds.Height);

        objectY += this.fences.bounds.Height + 6;

        this.forage.bounds = new Rectangle(
            this.xPositionOnScreen + 256 + 16,
            objectY,
            this.fences.bounds.Width,
            this.fences.bounds.Height);

        #endregion

        #region TerrainFeatures

        int terrainY = this.yPositionOnScreen + 64 + 24;

        this.grass.bounds = new Rectangle(
            this.xPositionOnScreen + 16,
            terrainY,
            this.grass.bounds.Width,
            this.grass.bounds.Height);

        terrainY += this.grass.bounds.Height + 6;

        this.wildTrees.bounds = new Rectangle(
            this.xPositionOnScreen + 16,
            terrainY,
            this.grass.bounds.Width,
            this.grass.bounds.Height);

        terrainY += this.grass.bounds.Height + 6;

        this.fruitTrees.bounds = new Rectangle(
            this.xPositionOnScreen + 16,
            terrainY,
            this.grass.bounds.Width,
            this.grass.bounds.Height);

        terrainY += this.grass.bounds.Height + 6;

        this.hoeDirt.bounds = new Rectangle(
            this.xPositionOnScreen + 16,
            terrainY,
            this.grass.bounds.Width,
            this.grass.bounds.Height);

        terrainY += this.grass.bounds.Height + 6;

        this.crops.bounds = new Rectangle(
            this.xPositionOnScreen + 16,
            terrainY,
            this.grass.bounds.Width,
            this.grass.bounds.Height);

        terrainY += this.grass.bounds.Height + 6;

        this.bushes.bounds = new Rectangle(
            this.xPositionOnScreen + 16,
            terrainY,
            this.grass.bounds.Width,
            this.grass.bounds.Height);

        terrainY += this.grass.bounds.Height + 6;

        this.paths.bounds = new Rectangle(
            this.xPositionOnScreen + 16,
            terrainY,
            this.grass.bounds.Width,
            this.grass.bounds.Height);

        #endregion

        #region ResourceClumps

        int clumpY = this.yPositionOnScreen + 64 + 24;

        this.stumps.bounds = new Rectangle(
            this.xPositionOnScreen + 256 + 128 + 32,
            clumpY,
            this.stumps.bounds.Width,
            this.stumps.bounds.Height);

        clumpY += this.stumps.bounds.Height + 6;

        this.logs.bounds = new Rectangle(
            this.xPositionOnScreen + 256 + 128 + 32,
            clumpY,
            this.stumps.bounds.Width,
            this.stumps.bounds.Height);

        clumpY += this.stumps.bounds.Height + 6;

        this.boulders.bounds = new Rectangle(
            this.xPositionOnScreen + 256 + 128 + 32,
            clumpY,
            this.stumps.bounds.Width,
            this.stumps.bounds.Height);

        clumpY += this.stumps.bounds.Height + 6;

        this.meteorites.bounds = new Rectangle(
            this.xPositionOnScreen + 256 + 128 + 32,
            clumpY,
            this.stumps.bounds.Width,
            this.stumps.bounds.Height);

        #endregion
    }

    public override void draw(SpriteBatch b)
    {
        this.UpdateBounds();

        DecidedlyShared.Ui.Utils.DrawBox(
            b,
            this.boxTexture,
            new Rectangle(0, 256, 60, 60),
            this.titleBounds);

        // DecidedlyShared.Ui.Utils.DrawBox(
        //     b,
        //     this.buttonPanelTexture,
        //     new Rectangle(0, 0, 84, 108),
        //     this.buttonPanelBounds,
        //     Color.White,
        //     40,
        //     12,
        //     40,
        //     40);

        DecidedlyShared.Ui.Utils.DrawBox(
            b,
            this.boxTexture,
            new Rectangle(0, 256, 60, 60),
            this.mainWindowBounds);

        base.draw(b);

        Vector2 stringWidth = Game1.smallFont.MeasureString("Terrain Feature Refresh");

        Utility.drawTextWithShadow(
            b,
            "Terrain Feature Refresh",
            Game1.smallFont,
            new Vector2(this.xPositionOnScreen + stringWidth.X / 2 - 32, this.yPositionOnScreen + 4),
            Game1.textColor);

        Utility.drawTextWithShadow(
            b,
            this.terrainFeatureHeader,
            Game1.smallFont,
            new Vector2(this.xPositionOnScreen + 16, this.yPositionOnScreen + 64 - 8),
            Game1.textColor);

        Utility.drawTextWithShadow(
            b,
            this.objectHeader,
            Game1.smallFont,
            new Vector2(this.xPositionOnScreen + 256 + 16, this.yPositionOnScreen + 64 - 8),
            Game1.textColor);

        Utility.drawTextWithShadow(
            b,
            this.clumpHeader,
            Game1.smallFont,
            new Vector2(this.xPositionOnScreen + 256 + 128 + 32, this.yPositionOnScreen + 64 - 8),
            Game1.textColor);

        this.resetButton.Draw(b);

        // Objects
        this.fences.Draw(b);
        this.weeds.Draw(b);
        this.twigs.Draw(b);
        this.stones.Draw(b);
        this.forage.Draw(b);

        // Terrain Features
        this.wildTrees.Draw(b);
        this.fruitTrees.Draw(b);
        this.paths.Draw(b);
        this.hoeDirt.Draw(b);
        this.crops.Draw(b);
        this.bushes.Draw(b);
        this.grass.Draw(b);

        // Resource Clumps
        this.stumps.Draw(b);
        this.logs.Draw(b);
        this.boulders.Draw(b);
        this.meteorites.Draw(b);

        base.drawMouse(b);
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
        foreach (Checkbox box in this.checkboxes)
        {
            if (box.containsPoint(x, y))
                box.ReceiveLeftClick();
        }
    }

    public override void performHoverAction(int x, int y)
    {
        this.resetButton.DoHover(x, y);

        // This call is required for the close button hover to work.
        base.performHoverAction(x, y);
    }

    public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
    {
        base.gameWindowSizeChanged(oldBounds, newBounds);
        this.UpdateBounds();
        this.upperRightCloseButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width - 36, this.yPositionOnScreen - 8, 48, 48), Game1.mouseCursors, new Rectangle(337, 494, 12, 12), 4f);
    }
}
