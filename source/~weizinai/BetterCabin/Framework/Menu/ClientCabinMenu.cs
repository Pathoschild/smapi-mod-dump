/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using StardewValley.Menus;
using xTile.Dimensions;
using static StardewValley.Menus.BuildingSkinMenu;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace weizinai.StardewValleyMod.BetterCabin.Framework.Menu;

internal class ClientCabinMenu : IClickableMenu
{
    private const int WindowWidth = 576;
    private const int WindowHeight = 576;
    
    private readonly Building building;
    private SkinEntry currentSkin = null!;
    private readonly List<SkinEntry> skins = new();

    private ClickableTextureComponent moveButton = null!;
    private ClickableTextureComponent okButton = null!;
    private ClickableTextureComponent previousSkinButton = null!;
    private ClickableTextureComponent nextSkinButton = null!;

    private bool isMoving;
    private Building? buildingToMove;
    private string hoverText = "";

    private readonly GameLocation originLocation;
    private readonly Location originViewport;

    private Rectangle Bound => new(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height);
    private GameLocation TargetLocation => this.building.GetParentLocation();

    public ClientCabinMenu(Building targetBuilding)
        : base(Game1.uiViewport.Width / 2 - WindowWidth / 2, Game1.uiViewport.Height / 2 - WindowHeight / 2, WindowWidth, WindowHeight)
    {
        this.originLocation = Game1.player.currentLocation;
        this.originViewport = Game1.viewport.Location;
        
        this.building = targetBuilding;

        var buildingData = targetBuilding.GetData();
        var index = 0;
        this.skins.Add(new SkinEntry(index++, null, "", ""));
        if (buildingData.Skins != null)
        {
            foreach (var skin in buildingData.Skins)
            {
                if (GameStateQuery.CheckConditions(skin.Condition, this.building.GetParentLocation()))
                {
                    this.skins.Add(new SkinEntry(index++, skin));
                }
            }
        }

        this.InitButton();
        this.SetSkin(Math.Max(this.skins.FindIndex(skin => skin.Id == this.building.skinId.Value), 0));
    }

    public override void performHoverAction(int x, int y)
    {
        if (!this.isMoving)
        {
            this.okButton.tryHover(x, y);
            this.moveButton.tryHover(x, y);
            this.previousSkinButton.tryHover(x, y);
            this.nextSkinButton.tryHover(x, y);

            if (this.moveButton.containsPoint(x, y))
                this.hoverText = Game1.content.LoadString("Strings\\UI:Carpenter_MoveBuildings");
            else
                this.hoverText = "";
        }
        else
        {
            this.building.color = Color.White;
            var b = this.TargetLocation.getBuildingAt(PositionHelper.GetTilePositionFromMousePosition());
            if (this.building.Equals(b)) b.color = Color.Lime * 0.8f;
        }
    }

    public override void receiveKeyPress(Keys key)
    {
        if (!this.isMoving)
        {
            base.receiveKeyPress(key);
        }
        else
        {
            if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && this.readyToClose() && Game1.locationRequest == null)
            {
                this.ReturnToCarpentryMenu();
            }
            else
            {
                if (Game1.options.doesInputListContain(Game1.options.moveDownButton, key))
                    Game1.panScreen(0, 32);
                else if (Game1.options.doesInputListContain(Game1.options.moveRightButton, key))
                    Game1.panScreen(32, 0);
                else if (Game1.options.doesInputListContain(Game1.options.moveUpButton, key))
                    Game1.panScreen(0, -32);
                else if (Game1.options.doesInputListContain(Game1.options.moveLeftButton, key))
                    Game1.panScreen(-32, 0);
            }
        }
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
        if (!this.isMoving)
        {
            if (this.okButton.containsPoint(x, y))
            {
                this.exitThisMenu(playSound);
            }
            else if (this.previousSkinButton.containsPoint(x, y))
            {
                Game1.playSound("shwip");
                this.SetSkin(this.currentSkin.Index - 1);
            }
            else if (this.nextSkinButton.containsPoint(x, y))
            {
                Game1.playSound("shwip");
                this.SetSkin(this.currentSkin.Index + 1);
            }
            else if (this.moveButton.containsPoint(x, y))
            {
                Game1.globalFadeToBlack(this.SetUpForBuildingPlacement);
                Game1.playSound("smallSelect");
                this.isMoving = true;
            }
            else
            {
                base.receiveLeftClick(x, y, playSound);
            }
        }
        else
        {
            if (this.buildingToMove is null)
            {
                this.buildingToMove = this.TargetLocation.getBuildingAt(PositionHelper.GetTilePositionFromMousePosition());
                if (this.building.Equals(this.buildingToMove))
                {
                    this.buildingToMove.isMoving = true;
                    Game1.playSound("axchop");
                }
                else
                {
                    this.buildingToMove = null;
                }

                return;
            }

            var buildingPosition = PositionHelper.GetTilePositionFromMousePosition();
            if (this.TargetLocation.buildStructure(this.buildingToMove, buildingPosition, Game1.player))
            {
                this.buildingToMove.isMoving = false;
                this.buildingToMove = null;
                Game1.playSound("axchop");
                DelayedAction.playSoundAfterDelay("dirtyHit", 50);
                DelayedAction.playSoundAfterDelay("dirtyHit", 150);
            }
            else
            {
                Game1.playSound("cancel");
            }
        }
    }

    public override void update(GameTime time)
    {
        if (!this.isMoving) return;

        var mouseX = Game1.getOldMouseX(false);
        if (mouseX < 64)
            Game1.panScreen(-32, 0);
        else if (mouseX - Game1.viewport.Width >= -64)
            Game1.panScreen(32, 0);

        var mouseY = Game1.getOldMouseY(false);
        if (mouseY < 64)
            Game1.panScreen(0, -32);
        else if (mouseY - Game1.viewport.Height >= -64)
            Game1.panScreen(0, 32);

        var pressedKeys = Game1.oldKBState.GetPressedKeys();
        foreach (var key in pressedKeys) this.receiveKeyPress(key);
    }

    public override void draw(SpriteBatch b)
    {
        if (!this.isMoving)
        {
            if (!Game1.options.showClearBackgrounds) b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);

            Game1.DrawBox(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height);

            var sourceRect = this.building.getSourceRect();
            this.building.drawInMenu(b, this.Bound.Center.X - sourceRect.Width * 4 / 2, this.Bound.Center.Y - sourceRect.Height * 4 / 2 - 16);

            SpriteText.drawStringWithScrollCenteredAt(b, I18n.UI_ClientCabinMenu_ChooseSkin(), this.Bound.Center.X, this.yPositionOnScreen - 96);

            this.okButton.draw(b);
            this.moveButton.draw(b);
            this.nextSkinButton.draw(b);
            this.previousSkinButton.draw(b);
        }
        else
        {
            Game1.StartWorldDrawInUI(b);
            if (this.buildingToMove is not null)
            {
                var mouseTilePosition = PositionHelper.GetTilePositionFromMousePosition();
                for (var y = 0; y < this.buildingToMove.tilesHigh.Value; y++)
                {
                    for (var x = 0; x < this.buildingToMove.tilesWide.Value; x++)
                    {
                        var sheetIndex = this.buildingToMove.getTileSheetIndexForStructurePlacementTile(x, y);
                        var currentGlobalTilePosition = new Vector2(mouseTilePosition.X + x, mouseTilePosition.Y + y);
                        if (!Game1.currentLocation.isBuildable(currentGlobalTilePosition)) sheetIndex++;
                        b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, currentGlobalTilePosition * 64f),
                            new Rectangle(194 + sheetIndex * 16, 388, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.999f);
                    }
                }
            }

            Game1.EndWorldDrawInUI(b);
        }

        this.drawMouse(b);
        if (this.hoverText.Length > 0) drawHoverText(b, this.hoverText, Game1.dialogueFont);
    }

    private void InitButton()
    {
        this.previousSkinButton = new ClickableTextureComponent(new Rectangle(this.Bound.Left, this.Bound.Center.Y - 32, 64, 64),
            Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44), 1f);
        this.nextSkinButton = new ClickableTextureComponent(new Rectangle(this.Bound.Right - 64, this.Bound.Center.Y - 32, 64, 64),
            Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33), 1f);

        this.moveButton = new ClickableTextureComponent(new Rectangle(this.Bound.Right - 64 * 2 - 8, this.Bound.Bottom + 16, 64, 64),
            Game1.mouseCursors, new Rectangle(257, 284, 16, 16), 4f);
        this.okButton = new ClickableTextureComponent(new Rectangle(this.Bound.Right - 64, this.Bound.Bottom + 16, 64, 64),
            Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f);

        if (this.skins.Count == 0)
        {
            this.nextSkinButton.visible = false;
            this.previousSkinButton.visible = false;
        }
    }

    private void SetSkin(int index)
    {
        index %= this.skins.Count;
        if (index < 0) index = this.skins.Count + index;
        this.SetSkin(this.skins[index]);
    }

    private void SetSkin(SkinEntry skin)
    {
        this.currentSkin = skin;
        if (this.building.skinId.Value != skin.Id)
        {
            this.building.skinId.Value = skin.Id;
            this.building.netBuildingPaintColor.Value.Color1Default.Value = true;
            this.building.netBuildingPaintColor.Value.Color2Default.Value = true;
            this.building.netBuildingPaintColor.Value.Color3Default.Value = true;
        }
    }

    private void SetUpForBuildingPlacement()
    {
        this.hoverText = "";

        Game1.currentLocation.cleanupBeforePlayerExit();
        Game1.currentLocation = this.TargetLocation;
        Game1.currentLocation.resetForPlayerEntry();
        Game1.globalFadeToClear();

        Game1.displayHUD = false;
        Game1.displayFarmer = false;

        Game1.player.viewingLocation.Value = this.TargetLocation.NameOrUniqueName;
        Game1.viewportFreeze = true;
        var position = PositionHelper.GetAbsolutePositionFromTilePosition(new Vector2(this.building.tileX.Value, this.building.tileY.Value));
        Game1.viewport.Location = new Location((int)position.X - Game1.viewport.Width / 2, (int)position.Y - Game1.viewport.Height / 2);
        Game1.panScreen(0, 0);
    }

    private void ReturnToCarpentryMenu()
    {
        var locationRequest = Game1.getLocationRequest(this.originLocation.NameOrUniqueName);
        locationRequest.OnWarp += delegate
        {
            this.isMoving = false;
            this.buildingToMove = null;

            Game1.displayHUD = true;
            Game1.displayFarmer = true;

            Game1.player.viewingLocation.Value = null;
            Game1.viewportFreeze = false;
            Game1.viewport.Location = this.originViewport;
        };
        Game1.warpFarmer(locationRequest, Game1.player.TilePoint.X, Game1.player.TilePoint.Y, Game1.player.FacingDirection);
    }
}