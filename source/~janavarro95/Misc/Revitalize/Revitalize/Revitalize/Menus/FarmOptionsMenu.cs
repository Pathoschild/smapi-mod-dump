using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Revitalize.Resources.DataNodes;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Minigames;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using xTile;

namespace Revitalize.Menus
{
    //TODO: Check if persistent across saves
    public class FarmOptionsMenu : IClickableMenu
    {
        public const int colorPickerTimerDelay = 100;

        private List<int> shirtOptions;

        private List<int> hairStyleOptions;

        private List<int> accessoryOptions;

        private int currentShirt;

        private int currentHair;

        private int currentAccessory;

        private int colorPickerTimer;

        private ColorPicker pantsColorPicker;

        private ColorPicker hairColorPicker;

        private ColorPicker eyeColorPicker;

        private List<ClickableComponent> labels = new List<ClickableComponent>();

        private List<ClickableComponent> leftSelectionButtons = new List<ClickableComponent>();

        private List<ClickableComponent> rightSelectionButtons = new List<ClickableComponent>();

        //private List<ClickableComponent> genderButtons = new List<ClickableComponent>();

       // private List<ClickableComponent> petButtons = new List<ClickableComponent>();

        public List<ClickableTextureComponent> farmTypeButtons = new List<ClickableTextureComponent>();

        private ClickableTextureComponent okButton;

        private ClickableTextureComponent skipIntroButton;

        private ClickableTextureComponent randomButton;

        private TextBox nameBox;

        private TextBox farmnameBox;

        private TextBox favThingBox;

        private bool skipIntro;

        private bool wizardSource;

        private string hoverText;

        private string hoverTitle;

        public int whichFarm;
        private ColorPicker lastHeldColorPicker;

        private int timesRandom;

        public List<FarmOptionsDataNode> farmsToAdd;

        private int count;
        private ClickableComponent nameLabel;

        public FarmOptionsMenu(bool wizardSource = false) : base(Game1.viewport.Width / 2 - (632 + IClickableMenu.borderWidth * 2) / 2, Game1.viewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2 - Game1.tileSize, 632 + IClickableMenu.borderWidth * 2, 600 + IClickableMenu.borderWidth * 2 + Game1.tileSize, false)
        {
            farmsToAdd = new List<FarmOptionsDataNode>();
          //  getAllFarmMapsToAdd();
            this.wizardSource = wizardSource;
            this.setUpPositions();
            Game1.player.faceDirection(2);
            Game1.player.FarmerSprite.StopAnimation();
        }

        public FarmOptionsMenu(List<FarmOptionsDataNode> FarmsToAdd, bool wizardSource = false) : base(Game1.viewport.Width / 2 - (632 + IClickableMenu.borderWidth * 2) / 2, Game1.viewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2 - Game1.tileSize, 632 + IClickableMenu.borderWidth * 2, 600 + IClickableMenu.borderWidth * 2 + Game1.tileSize, false)
        {
            farmsToAdd = FarmsToAdd;
           // getAllFarmMapsToAdd();
            foreach(var v in farmsToAdd)
            {
                farmTypeButtons.Add(v.clicky);
            }

            this.wizardSource = wizardSource;
            this.setUpPositions();
            Game1.player.faceDirection(2);
            Game1.player.FarmerSprite.StopAnimation();
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            base.gameWindowSizeChanged(oldBounds, newBounds);
            this.xPositionOnScreen = Game1.viewport.Width / 2 - (632 + IClickableMenu.borderWidth * 2) / 2;
            this.yPositionOnScreen = Game1.viewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2 - Game1.tileSize;
            this.setUpPositions();
        }

        private void setUpPositions()
        {
            this.labels.Clear();
            this.leftSelectionButtons.Clear();
            this.rightSelectionButtons.Clear();
            this.farmTypeButtons.Clear();
            this.count = 4;
            this.okButton = new ClickableTextureComponent("OK", new Rectangle(this.xPositionOnScreen + this.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - Game1.tileSize, this.yPositionOnScreen + this.height - IClickableMenu.borderWidth - IClickableMenu.spaceToClearTopBorder + Game1.tileSize / 4, Game1.tileSize, Game1.tileSize), null, null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46, -1, -1), 1f, false);
            this.nameBox = new TextBox(Game1.content.Load<Texture2D>("LooseSprites\\textBox"), null, Game1.smallFont, Game1.textColor)
            {
                X = this.xPositionOnScreen + Game1.tileSize + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth - Game1.tileSize * 1,
                Y = this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 4,
                Text = ""
            };
            this.nameBox.Width = this.nameBox.Width * 3;
            this.labels.Add(this.nameLabel = new ClickableComponent(new Rectangle(this.xPositionOnScreen + Game1.tileSize / 6 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth - Game1.tileSize * 3 + 8, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 10, 1, 1), "Farm Name"));
            if (!this.wizardSource)
            {
                Point point = new Point((this.xPositionOnScreen + this.width + Game1.pixelZoom + Game1.tileSize / 8)/3, this.yPositionOnScreen + IClickableMenu.borderWidth * 2);
                this.farmTypeButtons.Add(new ClickableTextureComponent("Standard", new Rectangle((int)(point.X * .25f), point.Y + 22 * Game1.pixelZoom, 22 * Game1.pixelZoom, 20 * Game1.pixelZoom), null, Game1.content.LoadString("Strings\\UI:Character_FarmStandard", new object[0]), Game1.mouseCursors, new Rectangle(0, 324, 22, 20), (float)Game1.pixelZoom, false));
                this.farmTypeButtons.Add(new ClickableTextureComponent("Riverland", new Rectangle((int)(point.X *.25f), point.Y + 22 * Game1.pixelZoom * 2, 22 * Game1.pixelZoom, 20 * Game1.pixelZoom), null, Game1.content.LoadString("Strings\\UI:Character_FarmFishing", new object[0]), Game1.mouseCursors, new Rectangle(22, 324, 22, 20), (float)Game1.pixelZoom, false));
                this.farmTypeButtons.Add(new ClickableTextureComponent("Forest", new Rectangle((int)(point.X*.25f), point.Y + 22 * Game1.pixelZoom * 3, 22 * Game1.pixelZoom, 20 * Game1.pixelZoom), null, Game1.content.LoadString("Strings\\UI:Character_FarmForaging", new object[0]), Game1.mouseCursors, new Rectangle(44, 324, 22, 20), (float)Game1.pixelZoom, false));
                this.farmTypeButtons.Add(new ClickableTextureComponent("Hills", new Rectangle((int)(point.X*.25f), point.Y + 22 * Game1.pixelZoom * 4, 22 * Game1.pixelZoom, 20 * Game1.pixelZoom), null, Game1.content.LoadString("Strings\\UI:Character_FarmMining", new object[0]), Game1.mouseCursors, new Rectangle(66, 324, 22, 20), (float)Game1.pixelZoom, false));
                this.farmTypeButtons.Add(new ClickableTextureComponent("Wilderness", new Rectangle((int)(point.X*.25f), point.Y + 22 * Game1.pixelZoom * 5, 22 * Game1.pixelZoom, 20 * Game1.pixelZoom), null, Game1.content.LoadString("Strings\\UI:Character_FarmCombat", new object[0]), Game1.mouseCursors, new Rectangle(88, 324, 22, 20), (float)Game1.pixelZoom, false));
                this.getAllFarmMapsToAdd();
            }
          
        }

        private void optionButtonClick(string name)
        {

              //Log.AsyncC(name);
            if (name == "Wilderness")
            {
                if (!this.wizardSource)
                {
                    string g = Path.Combine("Maps", "Farm_Combat");
                    Utilities.MapUtilities.removeAllWaterTilesFromMap(Game1.getLocationFromName("Farm"));
                    Game1.getLocationFromName("Farm").map = Game1.game1.xTileContent.Load<Map>(g);
                    whichFarm = 4;
                    //Log.AsyncG("MAP SWAP!");
                    Class1.persistentMapSwap.mapPath = Path.Combine(Game1.content.RootDirectory, g);
                    //Log.AsyncG(Class1.persistentMapSwap.mapPath);
                    Serialize.serializeMapSwapData(Class1.persistentMapSwap);


                    Game1.whichFarm = 4;
                    Game1.spawnMonstersAtNight = true;
                }
            }


            else if (name == "Standard")
            {
                if (!this.wizardSource)
                {
                    string g = Path.Combine("Maps", "Farm");
                    Utilities.MapUtilities.removeAllWaterTilesFromMap(Game1.getLocationFromName("Farm"));
                    Game1.getLocationFromName("Farm").map = Game1.game1.xTileContent.Load<Map>(g);
                    //Log.AsyncG("MAP SWAP!");
                    Class1.persistentMapSwap.mapPath = Path.Combine(Game1.content.RootDirectory, g);
                    //Log.AsyncG(Class1.persistentMapSwap.mapPath);
                    Serialize.serializeMapSwapData(Class1.persistentMapSwap);


                    Game1.whichFarm = 0;
                    whichFarm = 0;
                    Game1.spawnMonstersAtNight = false;
                }
            }
            else if (name == "Riverland")
            {
                if (!this.wizardSource)
                {
                    string g = Path.Combine("Maps", "Farm_Fishing");
                    Utilities.MapUtilities.removeAllWaterTilesFromMap(Game1.getLocationFromName("Farm"));
                    Game1.getLocationFromName("Farm").map = Game1.game1.xTileContent.Load<Map>(g);
                    //Log.AsyncG("MAP SWAP!");
                    Class1.persistentMapSwap.mapPath = Path.Combine(Game1.content.RootDirectory, g);
                    //Log.AsyncG(Class1.persistentMapSwap.mapPath);
                    Serialize.serializeMapSwapData(Class1.persistentMapSwap);

                    Game1.whichFarm = 1;
                    whichFarm = 1;
                    Game1.spawnMonstersAtNight = false;
                }
            }
            else if (name == "OK")
            {
                if (!this.canLeaveMenu())
                {
                    return;
                }
                //        Game1.player.Name = this.nameBox.Text.Trim();
                //        Game1.player.favoriteThing = this.favThingBox.Text.Trim();
                Game1.exitActiveMenu();
                if (Game1.currentMinigame != null && Game1.currentMinigame is Intro)
                {
                    (Game1.currentMinigame as Intro).doneCreatingCharacter();
                }
                else if (this.wizardSource)
                {
                    Game1.flashAlpha = 1f;
                    Game1.playSound("yoba");
                }
            }
            else if (name == "Hills")
            {
                if (!this.wizardSource)
                {
                    string g = Path.Combine("Maps", "Farm_Mining");
                    Utilities.MapUtilities.removeAllWaterTilesFromMap(Game1.getLocationFromName("Farm"));
                    Game1.getLocationFromName("Farm").map = Game1.game1.xTileContent.Load<Map>(g);
                    //Log.AsyncG("MAP SWAP!");
                    Class1.persistentMapSwap.mapPath = Path.Combine(Game1.content.RootDirectory, g);
                    //Log.AsyncG(Class1.persistentMapSwap.mapPath);
                    Serialize.serializeMapSwapData(Class1.persistentMapSwap);

                    Game1.whichFarm = 3;
                    whichFarm = 3;
                    Game1.spawnMonstersAtNight = false;
                }
            }

            else if (name == "Forest")
            {
                if (!this.wizardSource)
                {
                    string g = Path.Combine("Maps", "Farm_Foraging");
                    Utilities.MapUtilities.removeAllWaterTilesFromMap(Game1.getLocationFromName("Farm"));
                    Game1.getLocationFromName("Farm").map = Game1.game1.xTileContent.Load<Map>(g);
                    //Log.AsyncG("MAP SWAP!");
                    Class1.persistentMapSwap.mapPath = Path.Combine(Game1.content.RootDirectory, g);
                    //Log.AsyncG(Class1.persistentMapSwap.mapPath);
                    Serialize.serializeMapSwapData(Class1.persistentMapSwap);


                    Game1.whichFarm = 2;
                    whichFarm = 2;
                    Game1.spawnMonstersAtNight = false;
                }
            }


            int count = 4;
                    foreach(var v in farmsToAdd)
                    {
                        count++;
                        if (v.clicky.name == name)
                        {
                            foreach(var c in Game1.locations)
                            {
                                if (c.name == "Farm")
                                {

                             Utilities.MapUtilities.removeAllWaterTilesFromMap(c);

                            Vector2 oldDimenstions = Utilities.MapUtilities.getMapDimensions(c);
                            bool[,] oldWaterTiles = c.waterTiles;
                                c.map = v.map;
                                //
                                    whichFarm = count;
                                    //Log.AsyncG("MAP SWAP!");
                         Class1.persistentMapSwap.mapPath= Path.Combine(Game1.content.RootDirectory,"Maps","Farms", v.clicky.name,v.clicky.name);
                            Class1.persistentMapSwap.folderPath = Path.Combine(Game1.content.RootDirectory, "Maps", "Farms", v.clicky.name);
                            //Log.AsyncG(Class1.persistentMapSwap.mapPath);
                            //Game1.getLocationFromName("Farm").map = Game1.game1.xTileContent.Load<Map>(Class1.persistentMapSwap.mapPath);
                            Serialize.serializeMapSwapData(Class1.persistentMapSwap);
                            // Util.removeAllWaterTilesFromMap(c);
                            Utilities.MapUtilities.transferWaterTiles(c, oldDimenstions, oldWaterTiles);
                            Utilities.MapUtilities.parseWarpsFromFile(c);

                            
                        }
                            }
                        }

                }
            this.nameBox.Text = name;
            Game1.playSound("coin");
          
        }

        private void selectionClick(string name, int change)
        {
            if (name == "Skin")
            {
                Game1.player.changeSkinColor(Game1.player.skin + change);
                Game1.playSound("skeletonStep");
                return;
            }
            if (name == "Hair")
            {
                Game1.player.changeHairStyle(Game1.player.hair + change);
                Game1.playSound("grassyStep");
                return;
            }
            if (name == "Shirt")
            {
                Game1.player.changeShirt(Game1.player.shirt + change);
                Game1.playSound("coin");
                return;
            }
            if (name == "Acc")
            {
                Game1.player.changeAccessory(Game1.player.accessory + change);
                Game1.playSound("purchase");
                return;
            }
            if (!(name == "Direction"))
            {
                return;
            }
            Game1.player.faceDirection((Game1.player.facingDirection - change + 4) % 4);
            Game1.player.FarmerSprite.StopAnimation();
            Game1.player.completelyStopAnimatingOrDoingAction();
            Game1.playSound("pickUpItem");
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {

            foreach (ClickableComponent current2 in this.farmTypeButtons)
            {
                if (current2.containsPoint(x, y) && !current2.name.Contains("Gray"))
                {
                    this.optionButtonClick(current2.name);
                    current2.scale -= 0.5f;
                    current2.scale = Math.Max(3.5f, current2.scale);
                }
            }

            foreach (ClickableComponent current4 in this.leftSelectionButtons)
            {
                if (current4.containsPoint(x, y))
                {
                    this.selectionClick(current4.name, -1);
                    current4.scale -= 0.25f;
                    current4.scale = Math.Max(0.75f, current4.scale);
                }
            }
            foreach (ClickableComponent current5 in this.rightSelectionButtons)
            {
                if (current5.containsPoint(x, y))
                {
                    this.selectionClick(current5.name, 1);
                    current5.scale -= 0.25f;
                    current5.scale = Math.Max(0.75f, current5.scale);
                }
            }
            if (this.okButton.containsPoint(x, y) && this.canLeaveMenu())
            {
                this.optionButtonClick(this.okButton.name);
                this.okButton.scale -= 0.25f;
                this.okButton.scale = Math.Max(0.75f, this.okButton.scale);
            }

        }

        public override void leftClickHeld(int x, int y)
        {
            this.colorPickerTimer -= Game1.currentGameTime.ElapsedGameTime.Milliseconds;
            if (this.colorPickerTimer <= 0)
            {
                if (this.lastHeldColorPicker != null)
                {
                    if (this.lastHeldColorPicker.Equals(this.hairColorPicker))
                    {
                        Game1.player.changeHairColor(this.hairColorPicker.clickHeld(x, y));
                    }
                    if (this.lastHeldColorPicker.Equals(this.pantsColorPicker))
                    {
                        Game1.player.changePants(this.pantsColorPicker.clickHeld(x, y));
                    }
                    if (this.lastHeldColorPicker.Equals(this.eyeColorPicker))
                    {
                        Game1.player.changeEyeColor(this.eyeColorPicker.clickHeld(x, y));
                    }
                }
                this.colorPickerTimer = 100;
            }
        }

        public override void releaseLeftClick(int x, int y)
        {
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
        }

        public override void receiveKeyPress(Keys key)
        {
            if (!this.wizardSource && key == Keys.Tab)
            {
                if (this.nameBox.Selected)
                {
                    this.farmnameBox.SelectMe();
                    this.nameBox.Selected = false;
                    return;
                }
                if (this.farmnameBox.Selected)
                {
                    this.farmnameBox.Selected = false;
                    this.favThingBox.SelectMe();
                    return;
                }
                this.favThingBox.Selected = false;
                this.nameBox.SelectMe();
            }
        }

        public override void performHoverAction(int x, int y)
        {
            this.hoverText = "";
            this.hoverTitle = "";
            using (List<ClickableComponent>.Enumerator enumerator = this.leftSelectionButtons.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    ClickableTextureComponent clickableTextureComponent = (ClickableTextureComponent)enumerator.Current;
                    if (clickableTextureComponent.containsPoint(x, y))
                    {
                        clickableTextureComponent.scale = Math.Min(clickableTextureComponent.scale + 0.02f, clickableTextureComponent.baseScale + 0.1f);
                    }
                    else
                    {
                        clickableTextureComponent.scale = Math.Max(clickableTextureComponent.scale - 0.02f, clickableTextureComponent.baseScale);
                    }
                }
            }
            using (List<ClickableComponent>.Enumerator enumerator = this.rightSelectionButtons.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    ClickableTextureComponent clickableTextureComponent2 = (ClickableTextureComponent)enumerator.Current;
                    if (clickableTextureComponent2.containsPoint(x, y))
                    {
                        clickableTextureComponent2.scale = Math.Min(clickableTextureComponent2.scale + 0.02f, clickableTextureComponent2.baseScale + 0.1f);
                    }
                    else
                    {
                        clickableTextureComponent2.scale = Math.Max(clickableTextureComponent2.scale - 0.02f, clickableTextureComponent2.baseScale);
                    }
                }
            }
            if (!this.wizardSource)
            {
                foreach (ClickableTextureComponent current in this.farmTypeButtons)
                {
                    if (current.containsPoint(x, y) && !current.name.Contains("Gray"))
                    {
                        try
                        {
                            current.scale = Math.Min(current.scale + 0.02f, current.baseScale + 0.1f);
                            this.hoverTitle = current.hoverText.Split(new char[]
                            {
                            '_'
                            })[0];
                            this.hoverText = current.hoverText.Split(new char[]
                            {
                            '_'
                            })[1];
                        }
                        catch(Exception e)
                        {
                            this.hoverTitle = current.name;
                            this.hoverText = current.hoverText;
                        }
                    }
                    else
                    {
                        current.scale = Math.Max(current.scale - 0.02f, current.baseScale);
                        if (current.name.Contains("Gray") && current.containsPoint(x, y))
                        {
                            this.hoverText = "Reach level 10 " + Game1.content.LoadString("Strings\\UI:Character_" + current.name.Split(new char[]
                            {
                                '_'
                            })[1], new object[0]) + " to unlock.";
                        }
                    }
                }
            }
            if (!this.wizardSource)
            {

            }
            if (this.okButton.containsPoint(x, y) && this.canLeaveMenu())
            {
                this.okButton.scale = Math.Min(this.okButton.scale + 0.02f, this.okButton.baseScale + 0.1f);
            }
            else
            {
                this.okButton.scale = Math.Max(this.okButton.scale - 0.02f, this.okButton.baseScale);
            }
            //this.randomButton.tryHover(x, y, 0.25f);
            //this.randomButton.tryHover(x, y, 0.25f);
        }

        public bool canLeaveMenu()
        {
            return true;
        }

        public override void draw(SpriteBatch b)
        {
            Game1.drawDialogueBox(this.xPositionOnScreen-this.xPositionOnScreen, this.yPositionOnScreen, this.width+this.xPositionOnScreen, this.height, false, true, null, false);
          //  b.Draw(Game1.daybg, new Vector2((float)(this.xPositionOnScreen + Game1.tileSize + Game1.tileSize * 2 / 3 - 2), (float)(this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 4)), Color.White);
           // Game1.player.FarmerRenderer.draw(b, Game1.player.FarmerSprite.CurrentAnimationFrame, Game1.player.FarmerSprite.CurrentFrame, Game1.player.FarmerSprite.SourceRect, new Vector2((float)(this.xPositionOnScreen - 2 + Game1.tileSize * 2 / 3 + Game1.tileSize * 2 - Game1.tileSize / 2), (float)(this.yPositionOnScreen + IClickableMenu.borderWidth - Game1.tileSize / 4 + IClickableMenu.spaceToClearTopBorder + Game1.tileSize / 2)), Vector2.Zero, 0.8f, Color.White, 0f, 1f, Game1.player);
            if (!this.wizardSource)
            {
                /*
                using (List<ClickableComponent>.Enumerator enumerator = this.genderButtons.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        ClickableTextureComponent clickableTextureComponent = (ClickableTextureComponent)enumerator.Current;
                        clickableTextureComponent.draw(b);
                        if ((clickableTextureComponent.name.Equals("Male") && Game1.player.isMale) || (clickableTextureComponent.name.Equals("Female") && !Game1.player.isMale))
                        {
                            b.Draw(Game1.mouseCursors, clickableTextureComponent.bounds, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 34, -1, -1)), Color.White);
                        }
                    }
                }
                using (List<ClickableComponent>.Enumerator enumerator = this.petButtons.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        ClickableTextureComponent clickableTextureComponent2 = (ClickableTextureComponent)enumerator.Current;
                        clickableTextureComponent2.draw(b);
                        if ((clickableTextureComponent2.name.Equals("Cat") && Game1.player.catPerson) || (clickableTextureComponent2.name.Equals("Dog") && !Game1.player.catPerson))
                        {
                            b.Draw(Game1.mouseCursors, clickableTextureComponent2.bounds, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 34, -1, -1)), Color.White);
                        }
                    }
                }
                Game1.player.name = this.nameBox.Text;
                Game1.player.favoriteThing = this.favThingBox.Text;
                Game1.player.farmName = this.farmnameBox.Text;
            }
            using (List<ClickableComponent>.Enumerator enumerator = this.leftSelectionButtons.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    ((ClickableTextureComponent)enumerator.Current).draw(b);
                }
            }
    */
    
                foreach (ClickableComponent current in this.labels)
                {
                    string text = "";
                    Color color = Game1.textColor;
                    if (current == this.nameLabel)
                    {
                        color = ((Game1.player.name.Length < 1) ? Color.Red : Game1.textColor);
                        if (this.wizardSource)
                        {
                            continue;
                        }
                    }
                    color = Game1.textColor;
                    
                    Utility.drawTextWithShadow(b, current.name, Game1.smallFont, new Vector2((float)current.bounds.X, (float)current.bounds.Y), color, 1f, -1f, -1, -1, 1f, 3);
                    if (text.Length > 0)
                    {
                        Utility.drawTextWithShadow(b, text, Game1.smallFont, new Vector2((float)(current.bounds.X + Game1.tileSize / 3) - Game1.smallFont.MeasureString(text).X / 2f, (float)(current.bounds.Y + Game1.tileSize / 2)), color, 1f, -1f, -1, -1, 1f, 3);
                    }
                }
                
                
                using (List<ClickableComponent>.Enumerator enumerator = this.rightSelectionButtons.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        ((ClickableTextureComponent)enumerator.Current).draw(b);
                    }
                }
                
            }
            
            if (!this.wizardSource)
            {
               // IClickableMenu.drawTextureBox(b, (this.farmTypeButtons[0].bounds.X - Game1.pixelZoom * 4)/3, this.farmTypeButtons[0].bounds.Y - Game1.pixelZoom * 5, 30 * Game1.pixelZoom, 110 * Game1.pixelZoom + Game1.pixelZoom * 9, Color.White);
                for (int i = 0; i < this.farmTypeButtons.Count; i++)
                {
                    this.farmTypeButtons[i].draw(b, this.farmTypeButtons[i].name.Contains("Gray") ? (Color.Black * 0.5f) : Color.White, 0.88f);
                    if (this.farmTypeButtons[i].name.Contains("Gray"))
                    {
                        b.Draw(Game1.mouseCursors, new Vector2((float)(this.farmTypeButtons[i].bounds.Center.X - Game1.pixelZoom * 3), (float)(this.farmTypeButtons[i].bounds.Center.Y - Game1.pixelZoom * 2)), new Rectangle?(new Rectangle(107, 442, 7, 8)), Color.White, 0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, 0.89f);
                    }
                    if (i == whichFarm)
                    {
                        IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(375, 357, 3, 3), this.farmTypeButtons[i].bounds.X, this.farmTypeButtons[i].bounds.Y - Game1.pixelZoom, this.farmTypeButtons[i].bounds.Width, this.farmTypeButtons[i].bounds.Height + Game1.pixelZoom * 2, Color.White, (float)Game1.pixelZoom, false);
                    }
                }
            }
            if (this.canLeaveMenu())
            {
                this.okButton.draw(b, Color.White, 0.75f);
            }
            else
            {
                this.okButton.draw(b, Color.White, 0.75f);
                this.okButton.draw(b, Color.Black * 0.5f, 0.751f);
            }
            //this.hairColorPicker.draw(b);
            //this.pantsColorPicker.draw(b);
            //this.eyeColorPicker.draw(b);
            if (!this.wizardSource)
            {
                this.nameBox.Draw(b);
                //this.farmnameBox.Draw(b);
                if (this.skipIntroButton != null)
                {
                    //this.skipIntroButton.draw(b);
                   // Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\UI:Character_SkipIntro", new object[0]), Game1.smallFont, new Vector2((float)(this.skipIntroButton.bounds.X + this.skipIntroButton.bounds.Width + Game1.pixelZoom * 2), (float)(this.skipIntroButton.bounds.Y + Game1.pixelZoom * 2)), Game1.textColor, 1f, -1f, -1, -1, 1f, 3);
                }
            //    Utility.drawTextWithShadow(b, , Game1.smallFont, new Vector2((float)(this.farmnameBox.X + this.farmnameBox.Width + Game1.pixelZoom * 2), (float)(this.farmnameBox.Y + Game1.pixelZoom * 3)), Game1.textColor, 1f, -1f, -1, -1, 1f, 3);
                //this.favThingBox.Draw(b);
            }
            if (this.hoverText != null && this.hoverTitle != null && this.hoverText.Count<char>() > 0)
            {
                IClickableMenu.drawHoverText(b, Game1.parseText(this.hoverText, Game1.smallFont, Game1.tileSize * 4), Game1.smallFont, 0, 0, -1, this.hoverTitle, -1, null, null, 0, -1, -1, -1, -1, 1f, null);
            }
            //this.randomButton.draw(b);
            base.drawMouse(b);
        }
        public void getAllFarmMapsToAdd()
        {
            Point point = new Point((this.xPositionOnScreen + this.width + Game1.pixelZoom + Game1.tileSize / 8) / 3, this.yPositionOnScreen + IClickableMenu.borderWidth * 2);
            string[] dir = Directory.GetDirectories(Path.Combine(Game1.content.RootDirectory, "Maps", "Farms"));
            string[] fi = Directory.GetFiles(Path.Combine(Game1.content.RootDirectory, "Maps", "Farms"));
            List<string> dir2 = new List<string>();
            foreach (var v in dir)
            {
                string path = "";
                path = Path.GetDirectoryName(v);
                dir2.Add(path);
              
                string[] spliiter=v.Split('\\');
                string fileName = spliiter.ElementAt(spliiter.Length-1);
                string s = fileName;
                //Log.AsyncC(v);
               fileName= Path.Combine(v.Remove(0, 8),s);
                try
                {
                    if (v.ToString().Contains("FarmIcons")) continue;
                    Rectangle r = new Rectangle();
                    if (this.getFarmIcon(v) != Game1.mouseCursors) r = new Microsoft.Xna.Framework.Rectangle(0, 0, 22, 20);
                    else r= new Microsoft.Xna.Framework.Rectangle(0, 324, 22, 20);

                    farmsToAdd.Add(new FarmOptionsDataNode(new ClickableTextureComponent(s, new Microsoft.Xna.Framework.Rectangle((int)(point.X * (.25f * ((farmsToAdd.Count / 5) + 2))), point.Y + 22 * Game1.pixelZoom * 1 * ((farmsToAdd.Count % 5) + 1), 22 * Game1.pixelZoom, 20 * Game1.pixelZoom), null, this.getFarmDescription(v), this.getFarmIcon(v), r, (float)Game1.pixelZoom, false), (Game1.content.Load<Map>(fileName))));
                }
                catch(Exception e)
                {
                    if (e.ToString().Contains("FarmIcons")) continue;
                    //Log.AsyncR(e);
                }
                }

            foreach (var v in fi)
            {
                dir2.Add(Path.GetFileNameWithoutExtension(v));
                //Log.AsyncC(v);
                string f=Path.GetDirectoryName(v);
                string[] spliiter = f.Split('\\');
                string fileName = spliiter.ElementAt(spliiter.Length-1);
                fileName = Path.Combine(f.Remove(0,8), Path.GetFileNameWithoutExtension(v));
                try
                {
                    Rectangle r = new Rectangle();
                    if (this.getFarmIcon(v) != Game1.mouseCursors) r = new Microsoft.Xna.Framework.Rectangle(0, 0, 22, 20);
                    else r =new Microsoft.Xna.Framework.Rectangle(0, 324, 22, 20);
                    farmsToAdd.Add(new FarmOptionsDataNode(new ClickableTextureComponent(Path.GetFileNameWithoutExtension(v), new Microsoft.Xna.Framework.Rectangle((int)(point.X * (.25f * ((farmsToAdd.Count / 5) + 2))), point.Y + 22 * Game1.pixelZoom * 1 * ((farmsToAdd.Count % 5) + 1), 22 * Game1.pixelZoom, 20 * Game1.pixelZoom), null, "A custom farm map.", Game1.mouseCursors, r, (float)Game1.pixelZoom, false), (Game1.content.Load<Map>(fileName))));
                    //compile data node
                }
                catch(Exception e)
                {
                    //Log.AsyncR(e);
                }
            }


            foreach(var k in farmsToAdd)
            {
               // Log.AsyncC("BOO");
                farmTypeButtons.Add(k.clicky);
                //Log.AsyncC(k.clicky.name);
            }
            //TODO: CHECK THE DIRECTORY AND ADD ALL DATA NODES TO THIS LIST

            return;
        }

        public string getFarmDescription(string s)
        {
            try
            {
                return File.ReadAllText(Path.Combine(s, "description.txt"));
            }
            catch (Exception e)
            {
                return "A custom farm map.";
            }
        }

        public Texture2D getFarmIcon(string s)
        {
            string y = s.Remove(0, 8);
            try
            {
              
                return Game1.content.Load<Texture2D>(Path.Combine(y,"icon"));//File.ReadAllText(Path.Combine(s, "description.txt");
            }

            catch(Exception e)
            {
                //Log.AsyncO(e);
                return Game1.mouseCursors;
            }

        }
    }




}
