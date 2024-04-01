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
using DecidedlyShared.Logging;
using DecidedlyShared.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using TerrainFeatureRefresh.src.Framework;

namespace TerrainFeatureRefresh.Framework.Ui;

public class TfrMainMenu : IClickableMenu
{
    private Logger logger;
    private Button resetButton;
    private Button clearButton;
    private Button generateButton;
    private Texture2D boxTexture;
    private Texture2D buttonTexture;
    private Texture2D closeButtonTexture;
    private Texture2D checkboxTexture;
    private SpriteFont font;
    // private Texture2D buttonPanelTexture;
    private Rectangle titleBounds;
    private Rectangle mainWindowBounds;
    private Rectangle buttonPanelBounds;
    private TfrSettings settings;

    private List<Checkbox> checkboxes;

    private Checkbox affectAllLocations;

    // Objects
    private Checkbox fences;
    private Checkbox weeds;
    private Checkbox twigs;
    private Checkbox stones;
    private Checkbox forage;
    private Checkbox artifactSpots;

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

    public TfrMainMenu(int screenX, int screenY, int width, int height, Logger logger, IModHelper helper)
        : base(screenX, screenY, width, height, true)
    {
        this.logger = logger;
        this.font = helper.ModContent.Load<SpriteFont>("assets/DejaVu-20pt.xnb");
        this.buttonTexture = Game1.content.Load<Texture2D>("Mods/DecidedlyHuman/TFR/Button");
        this.boxTexture = Game1.content.Load<Texture2D>("Mods/DecidedlyHuman/TFR/WindowSkin");
        this.closeButtonTexture = Game1.content.Load<Texture2D>("Mods/DecidedlyHuman/TFR/CloseButton");
        this.checkboxTexture = Game1.content.Load<Texture2D>("Mods/DecidedlyHuman/TFR/Checkbox");
        this.upperRightCloseButton = new ClickableTextureComponent(
            new Rectangle(this.xPositionOnScreen - 84 + this.width, this.yPositionOnScreen + 52, 16 * 4, 14 * 4),
            this.closeButtonTexture,
            new Rectangle(0, 0, 64, 56),
            1f);
        this.checkboxes = new List<Checkbox>();
        this.allClickableComponents = new List<ClickableComponent>();

        this.settings = new TfrSettings();

        this.checkboxes.Add(this.affectAllLocations = new AllLocationsCheckbox(Rectangle.Empty, "All Maps", this.checkboxTexture, this.font, ref this.settings.AffectAllLocations));

        // Objects
        this.checkboxes.Add(this.fences = new TfrCheckbox(Rectangle.Empty, "Fences", this.checkboxTexture, this.font, ref this.settings.Fences));
        this.checkboxes.Add(this.weeds = new TfrCheckbox(Rectangle.Empty, "Weeds", this.checkboxTexture, this.font, ref this.settings.Weeds));
        this.checkboxes.Add(this.twigs = new TfrCheckbox(Rectangle.Empty, "Twigs", this.checkboxTexture, this.font, ref this.settings.Twigs));
        this.checkboxes.Add(this.stones = new TfrCheckbox(Rectangle.Empty, "Stones", this.checkboxTexture, this.font, ref this.settings.Stones));
        this.checkboxes.Add(this.forage = new TfrCheckbox(Rectangle.Empty, "Forage", this.checkboxTexture, this.font, ref this.settings.Forage));
        this.checkboxes.Add(this.artifactSpots = new TfrCheckbox(Rectangle.Empty, "Artifact Spot", this.checkboxTexture, this.font, ref this.settings.ArtifactSpots));

        // Terrain Features
        this.checkboxes.Add(this.grass = new TfrCheckbox(Rectangle.Empty, "Grass", this.checkboxTexture, this.font, ref this.settings.Grass));
        this.checkboxes.Add(this.wildTrees = new TfrCheckbox(Rectangle.Empty, "Wild Trees", this.checkboxTexture, this.font, ref this.settings.WildTrees));
        this.checkboxes.Add(this.fruitTrees = new TfrCheckbox(Rectangle.Empty, "Fruit Trees", this.checkboxTexture, this.font, ref this.settings.FruitTrees));
        this.checkboxes.Add(this.paths = new TfrCheckbox(Rectangle.Empty, "Paths", this.checkboxTexture, this.font, ref this.settings.Paths));
        this.checkboxes.Add(this.hoeDirt = new TfrCheckbox(Rectangle.Empty, "Hoed Dirt", this.checkboxTexture, this.font, ref this.settings.HoeDirt));
        this.checkboxes.Add(this.crops = new TfrCheckbox(Rectangle.Empty, "Crops", this.checkboxTexture, this.font, ref this.settings.Crops));
        this.checkboxes.Add(this.bushes = new TfrCheckbox(Rectangle.Empty, "Bushes", this.checkboxTexture, this.font, ref this.settings.Bushes));

        // Resource Clumps
        this.checkboxes.Add(this.stumps = new TfrCheckbox(Rectangle.Empty, "Stumps", this.checkboxTexture, this.font, ref this.settings.Stumps));
        this.checkboxes.Add(this.logs = new TfrCheckbox(Rectangle.Empty, "Logs", this.checkboxTexture, this.font, ref this.settings.Logs));
        this.checkboxes.Add(this.boulders = new TfrCheckbox(Rectangle.Empty, "Boulders", this.checkboxTexture, this.font, ref this.settings.Boulders));
        this.checkboxes.Add(this.meteorites = new TfrCheckbox(Rectangle.Empty, "Spoiler Rocks", this.checkboxTexture, this.font, ref this.settings.Meteorites));

        this.resetButton = new Button(Rectangle.Empty, "Reset Selected", this.buttonTexture, this.font,
        new Rectangle(0, 0, 16, 16));
        this.clearButton = new Button(Rectangle.Empty, "Clear Selected", this.buttonTexture, this.font,
        new Rectangle(0, 0, 16, 16));
        this.generateButton = new Button(Rectangle.Empty, "Generate Selected", this.buttonTexture, this.font,
            new Rectangle(0, 0, 16, 16));

        foreach (Checkbox box in this.checkboxes)
        {
            this.allClickableComponents.Add(box);
        }

        this.allClickableComponents.Add(this.resetButton);

        this.UpdateBounds();
    }

    private void UpdateBounds()
    {
        int checkboxSpacing = 6;
        int buttonSpacing = 8;

        this.mainWindowBounds = new Rectangle(
            this.xPositionOnScreen,
            this.yPositionOnScreen + 32,
            this.width,
            this.height - 16);

        this.affectAllLocations.bounds = new Rectangle(
            this.mainWindowBounds.Right - this.affectAllLocations.bounds.Width - 32,
            this.mainWindowBounds.Bottom - this.resetButton.bounds.Height * 4 - 8 * 3,
            this.fences.bounds.Width,
            this.fences.bounds.Height);

        int buttonY = this.mainWindowBounds.Bottom - 64;

        this.generateButton.bounds = new Rectangle(
            this.mainWindowBounds.Right - this.generateButton.bounds.Width - 16,
            buttonY,
            this.generateButton.bounds.Width,
            this.generateButton.bounds.Height);
        new Vector2(this.xPositionOnScreen + 16, this.yPositionOnScreen + 64 - 8);

        buttonY -= this.clearButton.bounds.Height + buttonSpacing;

        this.resetButton.bounds = new Rectangle(
            this.mainWindowBounds.Right - this.resetButton.bounds.Width - 16,
            buttonY,
            this.resetButton.bounds.Width,
            this.resetButton.bounds.Height);
        new Vector2(this.xPositionOnScreen + 16, this.yPositionOnScreen + 64 - 8);

        buttonY -= this.resetButton.bounds.Height + buttonSpacing;

        this.clearButton.bounds = new Rectangle(
            this.mainWindowBounds.Right - this.clearButton.bounds.Width - 16,
            buttonY,
            this.clearButton.bounds.Width,
            this.clearButton.bounds.Height);
        new Vector2(this.xPositionOnScreen + 16, this.yPositionOnScreen + 64 - 8);

        #region Objects

        int objectY = this.yPositionOnScreen + 64 + 24 + 32 + 32;

        this.fences.bounds = new Rectangle(
            this.xPositionOnScreen + 256 + 16,
            objectY,
            this.fences.bounds.Width,
            this.fences.bounds.Height);

        objectY += this.fences.bounds.Height + checkboxSpacing;

        this.weeds.bounds = new Rectangle(
            this.xPositionOnScreen + 256 + 16,
            objectY,
            this.fences.bounds.Width,
            this.fences.bounds.Height);

        objectY += this.fences.bounds.Height + checkboxSpacing;

        this.stones.bounds = new Rectangle(
            this.xPositionOnScreen + 256 + 16,
            objectY,
            this.fences.bounds.Width,
            this.fences.bounds.Height);

        objectY += this.fences.bounds.Height + checkboxSpacing;

        this.twigs.bounds = new Rectangle(
            this.xPositionOnScreen + 256 + 16,
            objectY,
            this.fences.bounds.Width,
            this.fences.bounds.Height);

        objectY += this.fences.bounds.Height + checkboxSpacing;

        this.forage.bounds = new Rectangle(
            this.xPositionOnScreen + 256 + 16,
            objectY,
            this.fences.bounds.Width,
            this.fences.bounds.Height);

        objectY += this.fences.bounds.Height + checkboxSpacing;

        this.artifactSpots.bounds = new Rectangle(
            this.xPositionOnScreen + 256 + 16,
            objectY,
            this.artifactSpots.bounds.Width,
            this.artifactSpots.bounds.Height);

        #endregion

        #region TerrainFeatures

        int terrainY = this.yPositionOnScreen + 64 + 24 + 32 + 32;

        this.grass.bounds = new Rectangle(
            this.xPositionOnScreen + 16,
            terrainY,
            this.grass.bounds.Width,
            this.grass.bounds.Height);

        terrainY += this.grass.bounds.Height + checkboxSpacing;

        this.wildTrees.bounds = new Rectangle(
            this.xPositionOnScreen + 16,
            terrainY,
            this.grass.bounds.Width,
            this.grass.bounds.Height);

        terrainY += this.grass.bounds.Height + checkboxSpacing;

        this.fruitTrees.bounds = new Rectangle(
            this.xPositionOnScreen + 16,
            terrainY,
            this.grass.bounds.Width,
            this.grass.bounds.Height);

        terrainY += this.grass.bounds.Height + checkboxSpacing;

        this.hoeDirt.bounds = new Rectangle(
            this.xPositionOnScreen + 16,
            terrainY,
            this.grass.bounds.Width,
            this.grass.bounds.Height);

        terrainY += this.grass.bounds.Height + checkboxSpacing;

        this.crops.bounds = new Rectangle(
            this.xPositionOnScreen + 16,
            terrainY,
            this.grass.bounds.Width,
            this.grass.bounds.Height);

        terrainY += this.grass.bounds.Height + checkboxSpacing;

        this.bushes.bounds = new Rectangle(
            this.xPositionOnScreen + 16,
            terrainY,
            this.grass.bounds.Width,
            this.grass.bounds.Height);

        terrainY += this.grass.bounds.Height + checkboxSpacing;

        this.paths.bounds = new Rectangle(
            this.xPositionOnScreen + 16,
            terrainY,
            this.grass.bounds.Width,
            this.grass.bounds.Height);

        #endregion

        #region ResourceClumps

        int clumpY = this.yPositionOnScreen + 64 + 24 + 32 + 32;

        this.stumps.bounds = new Rectangle(
            this.xPositionOnScreen + 256 + 128 + 32,
            clumpY,
            this.stumps.bounds.Width,
            this.stumps.bounds.Height);

        clumpY += this.stumps.bounds.Height + checkboxSpacing;

        this.logs.bounds = new Rectangle(
            this.xPositionOnScreen + 256 + 128 + 32,
            clumpY,
            this.stumps.bounds.Width,
            this.stumps.bounds.Height);

        clumpY += this.stumps.bounds.Height + checkboxSpacing;

        this.boulders.bounds = new Rectangle(
            this.xPositionOnScreen + 256 + 128 + 32,
            clumpY,
            this.stumps.bounds.Width,
            this.stumps.bounds.Height);

        clumpY += this.stumps.bounds.Height + checkboxSpacing;

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

        // DecidedlyShared.Ui.Utils.DrawBox(
        //     b,
        //     this.boxTexture,
        //     new Rectangle(0, 0, 60, 60),
        //     this.titleBounds);

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
            new Rectangle(0, 0, 9 * 4, 24 * 4),
            this.mainWindowBounds,
            21 * 4,
            3 * 4,
            3 * 4,
            2 * 4
            );

        base.draw(b);

        Vector2 stringWidth = this.font.MeasureString("Terrain Feature Refresh");

        Drawing.DrawStringWithShadow(
            b,
            this.font,
            "Terrain Feature Refresh",
            new Vector2(this.xPositionOnScreen + 32, this.yPositionOnScreen + 64 - 4),
            Color.White,
            Color.Transparent);

        // Utility.drawTextWithShadow(
        //     b,
        //     "Terrain Feature Refresh",
        //     Game1.smallFont,
        //     new Vector2(this.xPositionOnScreen + stringWidth.X / 2 - 32, this.yPositionOnScreen + 4),
        //     Game1.textColor);

        Drawing.DrawStringWithShadow(
            b,
            this.font,
            this.terrainFeatureHeader,
            new Vector2(this.xPositionOnScreen + 16, this.yPositionOnScreen + 32 + 64 + 20),
            Color.Black,
            Color.Gray);

        // Utility.drawTextWithShadow(
        //     b,
        //     this.terrainFeatureHeader,
        //     Game1.smallFont,
        //     new Vector2(this.xPositionOnScreen + 16, this.yPositionOnScreen + 64 - 8),
        //     Game1.textColor);

        Drawing.DrawStringWithShadow(
            b,
            this.font,
            this.objectHeader,
            new Vector2(this.xPositionOnScreen + 256 + 16, this.yPositionOnScreen + 32 + 64 + 20),
            Color.Black,
            Color.Gray);

        // Utility.drawTextWithShadow(
        //     b,
        //     this.objectHeader,
        //     Game1.smallFont,
        //     new Vector2(this.xPositionOnScreen + 256 + 16, this.yPositionOnScreen + 64 - 8),
        //     Game1.textColor);

        Drawing.DrawStringWithShadow(
            b,
            this.font,
            this.clumpHeader,
            new Vector2(this.xPositionOnScreen + 256 + 128 + 32, this.yPositionOnScreen + 32 + 64 + 20),
            Color.Black,
            Color.Gray);

        // Utility.drawTextWithShadow(
        //     b,
        //     this.clumpHeader,
        //     Game1.smallFont,
        //     new Vector2(this.xPositionOnScreen + 256 + 128 + 32, this.yPositionOnScreen + 64 - 8),
        //     Game1.textColor);

        this.generateButton.Draw(b);
        this.resetButton.Draw(b);
        this.clearButton.Draw(b);
        this.affectAllLocations.Draw(b);

        // Objects
        this.fences.Draw(b);
        this.weeds.Draw(b);
        this.twigs.Draw(b);
        this.stones.Draw(b);
        this.forage.Draw(b);
        this.artifactSpots.Draw(b);

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
        // this.logger.Log($"Before clicking: {this.settings}", LogLevel.Info);
        foreach (Checkbox box in this.checkboxes)
        {
            if (box.containsPoint(x, y))
                box.ReceiveLeftClick();
        }
        // this.logger.Log($"After clicking: {this.settings}", LogLevel.Info);

        if (this.resetButton.containsPoint(x, y))
        {
            // Do the clicky.
            FeatureProcessor processor = new FeatureProcessor(this.settings, ProcessorAction.Regenerate, this.logger);
            processor.Execute();
        }
        else if (this.clearButton.containsPoint(x, y))
        {
            // Do the clicky.
            FeatureProcessor processor = new FeatureProcessor(this.settings, ProcessorAction.ClearOnly, this.logger);
            processor.Execute();
        }
        else if (this.generateButton.containsPoint(x, y))
        {
            // Do the clicky.
            FeatureProcessor processor = new FeatureProcessor(this.settings, ProcessorAction.Generate, this.logger);
            processor.Execute();
        }

        // TODO: Rig up close button click!
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
        this.upperRightCloseButton.bounds = new Rectangle(this.xPositionOnScreen - 84 + this.width,
            this.yPositionOnScreen + 52, 16 * 4, 14 * 4);
    }
}
