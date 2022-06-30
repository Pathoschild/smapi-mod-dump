/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/SolidFoundations
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SolidFoundations.Framework.Models.Backport;
using SolidFoundations.Framework.Models.ContentPack;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidFoundations.Framework.Utilities.Backport
{
    public class BuildingSkinMenu : IClickableMenu
    {
        public const int region_okButton = 101;

        public const int region_nextSkin = 102;

        public const int region_prevSkin = 103;

        public static int WINDOW_WIDTH = 576;

        public static int WINDOW_HEIGHT = 576;

        public int maxWidthOfBuildingViewer = 448;

        public int maxHeightOfBuildingViewer = 512;

        public Rectangle previewPane;

        public ClickableTextureComponent okButton;

        private string hoverText = "";

        public GenericBuilding building;

        public string buildingType = "";

        public ClickableTextureComponent nextSkinButton;

        public ClickableTextureComponent previousSkinButton;

        public string buildingDisplayName;

        public string buildingDescription;

        public List<string> skinIDs;

        public List<string> skinNames;

        public List<string> skinDescriptions;

        public int selectedSkin;

        public BuildingSkinMenu(GenericBuilding target_building)
            : base(Game1.uiViewport.Width / 2 - BuildingSkinMenu.WINDOW_WIDTH / 2, Game1.uiViewport.Height / 2 - BuildingSkinMenu.WINDOW_HEIGHT / 2, BuildingSkinMenu.WINDOW_WIDTH, BuildingSkinMenu.WINDOW_HEIGHT)
        {
            Game1.player.Halt();
            this.building = target_building;
            this.buildingDisplayName = TextParser.ParseText(this.building.Model.Name);
            this.buildingDescription = TextParser.ParseText(this.building.Model.Description);
            this.skinIDs = new List<string>();
            this.skinNames = new List<string>();
            this.skinDescriptions = new List<string>();
            this.skinIDs.Add(null);
            this.skinNames.Add(this.buildingDisplayName);
            this.skinDescriptions.Add(this.buildingDescription);
            if (this.building.Model.Skins != null)
            {
                foreach (BuildingSkin skin in this.building.Model.Skins)
                {
                    if (skin.Name != null)
                    {
                        this.skinIDs.Add(skin.ID);
                        this.skinNames.Add(TextParser.ParseText(building.Model.GetTranslation(skin.Name)));
                        this.skinDescriptions.Add(TextParser.ParseText(building.Model.GetTranslation(skin.Description)));
                    }
                }
            }
            this.RepositionElements();
            this.selectedSkin = this.skinIDs.IndexOf(this.building.skinID.Value);
            if (this.selectedSkin < 0)
            {
                this.selectedSkin = 0;
            }
            this.SetSkin(this.selectedSkin);
            base.populateClickableComponentList();
            if (Game1.options.SnappyMenus)
            {
                this.snapToDefaultClickableComponent();
            }
        }

        public override void snapToDefaultClickableComponent()
        {
            base.currentlySnappedComponent = base.getComponentWithID(101);
            this.snapCursorToCurrentSnappedComponent();
        }

        public override void receiveGamePadButton(Buttons b)
        {
            if ((int)b == 4194304)
            {
                Game1.playSound("shwip");
                this.SetSkin(this.selectedSkin + 1);
            }
            else if ((int)b == 8388608)
            {
                Game1.playSound("shwip");
                this.SetSkin(this.selectedSkin - 1);
            }
            base.receiveGamePadButton(b);
        }

        public override void update(GameTime time)
        {
            base.update(time);
        }

        public override void releaseLeftClick(int x, int y)
        {
            base.releaseLeftClick(x, y);
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            base.receiveRightClick(x, y, playSound);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (this.okButton.containsPoint(x, y))
            {
                base.exitThisMenu(playSound);
            }
            else if (this.previousSkinButton.containsPoint(x, y))
            {
                Game1.playSound("shwip");
                this.SetSkin(this.selectedSkin - 1);
            }
            else if (this.nextSkinButton.containsPoint(x, y))
            {
                this.SetSkin(this.selectedSkin + 1);
                Game1.playSound("shwip");
            }
            else
            {
                base.receiveLeftClick(x, y, playSound);
            }
        }

        public virtual void SetSkin(int skin_index)
        {
            if (skin_index >= this.skinIDs.Count)
            {
                skin_index = 0;
            }
            if (skin_index < 0)
            {
                skin_index = this.skinIDs.Count - 1;
            }
            this.selectedSkin = skin_index;
            if (this.skinIDs.Count > 0 && this.building.skinID.Value != this.skinIDs[this.selectedSkin])
            {
                this.building.skinID.Value = this.skinIDs[this.selectedSkin];
                this.building.netBuildingPaintColor.Value.Color1Default.Value = true;
                this.building.netBuildingPaintColor.Value.Color2Default.Value = true;
                this.building.netBuildingPaintColor.Value.Color3Default.Value = true;
            }

            this.building.resetTexture();
        }

        public override bool overrideSnappyMenuCursorMovementBan()
        {
            return false;
        }

        public override bool readyToClose()
        {
            return true;
        }

        public override void performHoverAction(int x, int y)
        {
            this.hoverText = "";
            this.okButton.tryHover(x, y);
            this.previousSkinButton.tryHover(x, y);
            this.nextSkinButton.tryHover(x, y);
        }

        public virtual void RepositionElements()
        {
            this.previewPane.Y = base.yPositionOnScreen + 48;
            this.previewPane.Width = 576;
            this.previewPane.Height = 576;
            this.previewPane.X = base.xPositionOnScreen + base.width / 2 - this.previewPane.Width / 2;
            Rectangle panel_rectangle = this.previewPane;
            panel_rectangle.Inflate(-16, -16);
            this.previousSkinButton = new ClickableTextureComponent(new Rectangle(panel_rectangle.Left, panel_rectangle.Center.Y - 32, 64, 64), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44), 1f)
            {
                myID = 103,
                leftNeighborID = -99998,
                rightNeighborID = -99998,
                downNeighborID = 101,
                upNeighborID = -99998,
                fullyImmutable = true
            };
            this.nextSkinButton = new ClickableTextureComponent(new Rectangle(panel_rectangle.Right - 64, panel_rectangle.Center.Y - 32, 64, 64), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33), 1f)
            {
                myID = 102,
                leftNeighborID = -99998,
                rightNeighborID = -99998,
                downNeighborID = 101,
                upNeighborID = -99998,
                fullyImmutable = true
            };
            panel_rectangle.Y += 64;
            panel_rectangle.Height = 0;
            _ = panel_rectangle.Left;
            panel_rectangle.Y += 80;
            panel_rectangle.Y += 64;
            this.okButton = new ClickableTextureComponent(new Rectangle(this.previewPane.Right - 64 - 16, this.previewPane.Bottom - 64 - 16, 64, 64), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f)
            {
                myID = 101,
                upNeighborID = 102
            };
            if (this.skinIDs.Count == 0)
            {
                this.nextSkinButton.visible = false;
                this.previousSkinButton.visible = false;
            }
            base.populateClickableComponentList();
        }

        public override bool IsAutomaticSnapValid(int direction, ClickableComponent a, ClickableComponent b)
        {
            return base.IsAutomaticSnapValid(direction, a, b);
        }

        public virtual bool SaveColor()
        {
            return true;
        }

        public virtual void SetRegion(int new_region)
        {
            this.RepositionElements();
        }

        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            Game1.DrawBox(this.previewPane.X, this.previewPane.Y, this.previewPane.Width, this.previewPane.Height);
            Rectangle rectangle = this.previewPane;
            rectangle.Inflate(0, 0);
            b.End();
            b.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, Utility.ScissorEnabled);
            b.GraphicsDevice.ScissorRectangle = rectangle;
            Vector2 building_draw_center = new Vector2(this.previewPane.X + this.previewPane.Width / 2, this.previewPane.Y + this.previewPane.Height / 2 - 16);
            if (this.building != null)
            {
                this.building.drawInMenu(b, (int)building_draw_center.X - (int)((float)(int)this.building.tilesWide / 2f * 64f), (int)building_draw_center.Y - this.building.getSourceRectForMenu().Height * 4 / 2);
            }
            b.End();
            b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
            SpriteText.drawStringWithScrollCenteredAt(b, $"Select an appearance for your {this.buildingDisplayName}.", base.xPositionOnScreen + base.width / 2, this.previewPane.Top - 96);
            this.okButton.draw(b);
            this.nextSkinButton.draw(b);
            this.previousSkinButton.draw(b);
            base.drawMouse(b);
        }
    }
}
