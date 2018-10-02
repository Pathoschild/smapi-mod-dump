using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN.Menus
{
    /// <summary>
    /// This class is nothing more than a replica of CarpenterMenu, except portions have been redesigned for displaying
    /// the farm house (which is not a Building object, but just a plain Texture2D. I know right?) to allow players to
    /// change the way the farm house exterior looks like, without reloading the game, doing it in an immersive experience.
    /// This class is still a WIP!
    /// </summary>
    public class CarpenterMenuHouseDesign : IClickableMenu
    {
        public int maxWidthOfBuildingViewer = 448;
        public int maxHeightOfBuildingViewer = 512;
        public int maxWidthOfDescription = 384;

        private List<HouseExteriorDesign> blueprints;
        private int currentBlueprintIndex = 0;

        public ClickableTextureComponent okButton;
        public ClickableTextureComponent cancelButton;
        public ClickableTextureComponent backButton;
        public ClickableTextureComponent forwardButton;

        private string hoverText = "";

        private int price;
        private string designName;
        private string designDescription;
        private Texture2D currentDesign;
        Rectangle area;

        public HouseExteriorDesign CurrentBlueprint
        {
            get
            {
                return blueprints[currentBlueprintIndex];
            }
        }

        /// <summary>
        /// Constructor. Grabs all available buildings and creates a HouseExteriorDesign object for each.
        /// </summary>
        public CarpenterMenuHouseDesign()
        {
            Game1.player.forceCanMove();
            resetBounds();
            blueprints = new List<HouseExteriorDesign>();
            area = new Rectangle(0, 144 * ((int)((NetFieldBase<int, NetInt>)Game1.MasterPlayer.houseUpgradeLevel) == 3 ? 2 : (int)((NetFieldBase<int, NetInt>)Game1.MasterPlayer.houseUpgradeLevel)), 160, 144);

            string pathToDirectory = Path.Combine(new string[] {
                    Path.Combine(Path.Combine(Path.Combine(Directory.GetCurrentDirectory(), "Mods"), "MTN"), "Building")
                });

            if (Directory.Exists(pathToDirectory))
            {
                foreach (string dir in Directory.GetDirectories(pathToDirectory))
                {
                    //I'm a jackass and my comments are shit.
                    string[] rememberMe = dir.Split('\\');
                    blueprints.Add(new HouseExteriorDesign(rememberMe[rememberMe.Length-1], rememberMe[rememberMe.Length - 1] + " AIDS"));

                    /*
                    Memory.instance.Monitor.Log("Attempting to change farm house look...");

                    Texture2D newHouseTexture = Memory.instance.Helper.Content.Load<Texture2D>(Path.Combine("Building", "HVF_Stonybrook", "houses.png"), ContentSource.ModFolder);
                    Traverse.Create(theFarmz).Field("houseTextures").SetValue(newHouseTexture);

                    Memory.instance.Monitor.Log("Done... ?");
                    */
                }
            }
            setNewActiveBlueprint();
        }

        /// <summary>
        /// Resets the bounderies of the menu, so it looks centered.
        /// </summary>
        private void resetBounds()
        {
            this.xPositionOnScreen = Game1.viewport.Width / 2 - this.maxWidthOfBuildingViewer - IClickableMenu.spaceToClearSideBorder;
            this.yPositionOnScreen = Game1.viewport.Height / 2 - this.maxHeightOfBuildingViewer / 2 - IClickableMenu.spaceToClearTopBorder + 32;
            this.width = this.maxWidthOfBuildingViewer + this.maxWidthOfDescription + IClickableMenu.spaceToClearSideBorder * 2 + 64;
            this.height = this.maxHeightOfBuildingViewer + IClickableMenu.spaceToClearTopBorder;
            this.initialize(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, true);
            ClickableTextureComponent textureComponent1 = new ClickableTextureComponent("OK", new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + this.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - 192 - 12, this.yPositionOnScreen + this.maxHeightOfBuildingViewer + 64, 64, 64), (string)null, (string)null, Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(366, 373, 16, 16), 4f, false);
            textureComponent1.myID = 106;
            textureComponent1.rightNeighborID = 104;
            textureComponent1.leftNeighborID = 105;
            this.okButton = textureComponent1;
            ClickableTextureComponent textureComponent2 = new ClickableTextureComponent("OK", new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + this.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - 64, this.yPositionOnScreen + this.maxHeightOfBuildingViewer + 64, 64, 64), (string)null, (string)null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 47, -1, -1), 1f, false);
            textureComponent2.myID = 107;
            textureComponent2.leftNeighborID = 104;
            this.cancelButton = textureComponent2;
            ClickableTextureComponent textureComponent3 = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + 64, this.yPositionOnScreen + this.maxHeightOfBuildingViewer + 64, 48, 44), Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(352, 495, 12, 11), 4f, false);
            textureComponent3.myID = 101;
            textureComponent3.rightNeighborID = 102;
            this.backButton = textureComponent3;
            ClickableTextureComponent textureComponent4 = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen + this.maxWidthOfBuildingViewer - 256 + 16, this.yPositionOnScreen + this.maxHeightOfBuildingViewer + 64, 48, 44), Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(365, 495, 12, 11), 4f, false);
            textureComponent4.myID = 102;
            textureComponent4.leftNeighborID = 101;
            textureComponent4.rightNeighborID = 105;
            this.forwardButton = textureComponent4;
        }

        /// <summary>
        /// If user brings up the menu, clicks Left Arrow or Right arrow, this will recalibrate what they're looking at.
        /// </summary>
        public void setNewActiveBlueprint()
        {
            currentDesign = blueprints[currentBlueprintIndex].texture;
            price = blueprints[currentBlueprintIndex].moneyRequired;
            designDescription = this.blueprints[this.currentBlueprintIndex].description;
            designName = this.blueprints[this.currentBlueprintIndex].name;
        }

        /// <summary>
        /// Detects if mouse is over a particular clickable component that has hover text.
        /// </summary>
        /// <param name="x">X coordinate in pixels</param>
        /// <param name="y">Y coordinate in pixels</param>
        public override void performHoverAction(int x, int y)
        {
            cancelButton.tryHover(x, y, 0.1f);
            base.performHoverAction(x, y);
            backButton.tryHover(x, y, 1f);
            forwardButton.tryHover(x, y, 1f);
            okButton.tryHover(x, y, 0.1f);
            if (okButton.containsPoint(x, y) && CurrentBlueprint.doesFarmerHaveEnoughResourcesToBuild())
            {
                hoverText = Game1.content.LoadString("Strings\\UI:Carpenter_Build");
            }
            else
            {
                hoverText = "";
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (cancelButton.containsPoint(x, y))
            {
                this.exitThisMenu(true);
                Game1.player.forceCanMove();
                Game1.playSound("bigDeSelect");
            }
            else if (backButton.containsPoint(x, y))
            {
                currentBlueprintIndex--;
                if (currentBlueprintIndex < 0) currentBlueprintIndex = blueprints.Count - 1;
                setNewActiveBlueprint();
                Game1.playSound("shwip");
                backButton.scale = backButton.baseScale;
            }
            else if (forwardButton.containsPoint(x, y))
            {
                currentBlueprintIndex = (currentBlueprintIndex + 1) % blueprints.Count;
                setNewActiveBlueprint();
                backButton.scale = backButton.baseScale;
                Game1.playSound("shwip");
            }
            else if (okButton.containsPoint(x, y) && Game1.player.money >= price && blueprints[currentBlueprintIndex].doesFarmerHaveEnoughResourcesToBuild())
            {
                exitFunction = new IClickableMenu.onExit(setFarmHouseDesign);
                this.exitThisMenu(true);
            }
        }

        public void setFarmHouseDesign()
        {
            Farm theFarm = Game1.getFarm();
            Traverse.Create(theFarm).Field("houseTextures").SetValue(currentDesign);
            //Game1.drawDialogue(Game1.getCharacterFromName("Robin", false), "I would say it's going to take 2 days, but I'm just gonna use mod magic for now. P.S. hawkfalcon? More like cockfalcon!");
        }

        /// <summary>
        /// Executed when window is resized.
        /// </summary>
        /// <param name="oldBounds"></param>
        /// <param name="newBounds"></param>
        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            resetBounds();
        }

        /// <summary>
        /// The draw routine.
        /// </summary>
        /// <param name="b">SpriteBatch for XNA</param>
        public override void draw(SpriteBatch b)
        {
            //Shade the background a bit
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.5f);

            //Base drawing
            base.draw(b);

            //
            IClickableMenu.drawTextureBox(b, this.xPositionOnScreen - 96, this.yPositionOnScreen - 16, this.maxWidthOfBuildingViewer + 64, this.maxHeightOfBuildingViewer + 64, Color.White);

            //Actual Farm House
            b.Draw(currentDesign, new Vector2(this.xPositionOnScreen + this.maxWidthOfBuildingViewer / 2 - 5 * 64 / 2 - 64, yPositionOnScreen + maxHeightOfBuildingViewer / 2 - 7 * 4 / 2), new Rectangle?(area), Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0.5f);

            //Header with Building Name
            string str = "Are Traps Gay???";
            SpriteText.drawStringWithScrollBackground(b, designName, this.xPositionOnScreen + this.maxWidthOfBuildingViewer - IClickableMenu.spaceToClearSideBorder - 16 + 64 + ((this.width - (this.maxWidthOfBuildingViewer + 128)) / 2 - SpriteText.getWidthOfString(str) / 2), this.yPositionOnScreen, str, 1f, -1);

            IClickableMenu.drawTextureBox(b, this.xPositionOnScreen + this.maxWidthOfBuildingViewer - 16, this.yPositionOnScreen + 80, this.maxWidthOfDescription + 64, this.maxWidthOfDescription + 96, Color.White);

            Utility.drawTextWithShadow(b, Game1.parseText(designDescription, Game1.dialogueFont, this.maxWidthOfDescription + 32), Game1.dialogueFont, new Vector2((float)(this.xPositionOnScreen + this.maxWidthOfDescription + 64), (float)(this.yPositionOnScreen + 80 + 16)), Game1.textColor, 1f, -1f, -1, -1, 0.25f, 3);

            //Resource listing
            Vector2 location = new Vector2((float)(this.xPositionOnScreen + this.maxWidthOfDescription + 16 + 64), (float)(this.yPositionOnScreen + 256 + 32));

            //The price is right
            SpriteText.drawString(b, "$", (int)location.X, (int)location.Y, 999999, -1, 999999, 1f, 0.88f, false, -1, "", -1);

            Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11020", (object)this.price), Game1.dialogueFont, new Vector2((float)((double)location.X + 64.0 + 4.0), location.Y + 4f), Game1.player.money >= this.price ? Game1.textColor : Color.Red, 1f, -1f, -1, -1, 0.25f, 3);

            location.X -= 16f;
            location.Y -= 21f;

            //Resources go here, later on. Fuck nuts

            //Buttons and mouse
            this.backButton.draw(b);
            this.forwardButton.draw(b);
            this.okButton.draw(b, this.blueprints[this.currentBlueprintIndex].doesFarmerHaveEnoughResourcesToBuild() ? Color.White : Color.Gray * 0.8f, 0.88f);
            cancelButton.draw(b);
            drawMouse(b);

            //Hover text
            if (hoverText.Length <= 0)
                return;
            IClickableMenu.drawHoverText(b, hoverText, Game1.dialogueFont, 0, 0, -1, (string)null, -1, (string[])null, (Item)null, 0, -1, -1, -1, -1, 1f, (CraftingRecipe)null);
        }
    }
}
