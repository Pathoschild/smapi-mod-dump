using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Minigames;
using System.Collections.Generic;
using System.Linq;
using System;
using Revitalize.Objects;
using StardewModdingAPI;
using System.Diagnostics;
using System.IO;
using System.Drawing.Imaging;

namespace Revitalize.Menus
{/// <summary>
/// TODO: Positioning, TOP/MID/Bottom Sections, Serialization, Color Picking Tool, Simple Presets, Possibly add in sprite preview when swapping pages, or an update button
/// </summary>
    public class Pixel
    {
        public Color color;
        public ClickableTextureComponent component;
        public Point position;

      public  Pixel(ClickableTextureComponent newComponent, Color newColor, Point newPosition)
        {
            component = newComponent;
            color = newColor;
            position = newPosition;
        }

        public Pixel(ClickableTextureComponent newComponent, Point newPosition, int Red = 255, int Green = 255, int Blue = 255, int Alpha = 255)
        {
            component = newComponent;
            color = new Color(Red, Green, Blue, Alpha);
            position = newPosition;
        }
    }


    public class PaintMenu : IClickableMenu
    {
        public const int colorPickerTimerDelay = 100;

        private int colorPickerTimer;

        private ColorPicker lightColorPicker;

        private List<ClickableComponent> labels = new List<ClickableComponent>();

        private List<ClickableComponent> leftSelectionButtons = new List<ClickableComponent>();

        private List<ClickableComponent> rightSelectionButtons = new List<ClickableComponent>();

        public List<Pixel> pixels = new List<Pixel>();


        public bool colorChanged;

        private ClickableTextureComponent okButton;

        private ClickableTextureComponent cancelButton;

        private ClickableTextureComponent randomButton;

        private TextBox nameBox;

        private TextBox farmnameBox;

        private TextBox favThingBox;

        private bool skipIntro;

        private bool wizardSource;

        private string hoverText;

        private string hoverTitle;

        private ColorPicker lastHeldColorPicker;

        public Canvas CanvasObject;

        private int timesRandom;

        public bool once;

        public bool clean;


        public TextBox numbersSelectBox;

        public PaintMenu(Canvas Obj, bool Clean=true) : base(Game1.viewport.Width / 2 - (632 + IClickableMenu.borderWidth * 2) / 2, Game1.viewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2 - Game1.tileSize, 632 + IClickableMenu.borderWidth * 2, 600 + IClickableMenu.borderWidth * 2 + Game1.tileSize, false)
        {
            clean = Clean;
            this.CanvasObject = Obj;
            this.setUpPositions(clean);
            colorChanged = false;
          //  this.height = this.height ;
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            base.gameWindowSizeChanged(oldBounds, newBounds);
            this.xPositionOnScreen = Game1.viewport.Width / 2 - (632 + IClickableMenu.borderWidth * 2) / 2;
            this.yPositionOnScreen = Game1.viewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2 - Game1.tileSize;
            this.setUpPositions(clean);
        }

        private void setUpPositions(bool Clean)
        {

            this.labels.Clear();

            this.leftSelectionButtons.Clear();
            this.rightSelectionButtons.Clear();
            this.pixels = new List<Pixel>();
           
            this.okButton = new ClickableTextureComponent("OK", new Rectangle(this.xPositionOnScreen + this.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - Game1.tileSize, (this.yPositionOnScreen + this.height - IClickableMenu.borderWidth - IClickableMenu.spaceToClearTopBorder + Game1.tileSize / 4) / 3, Game1.tileSize, Game1.tileSize), null, null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46, -1, -1), 1f, false);

            this.cancelButton = new ClickableTextureComponent("Cancel", new Rectangle(this.xPositionOnScreen + this.width / 4 - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - Game1.tileSize, (this.yPositionOnScreen + this.height - IClickableMenu.borderWidth - IClickableMenu.spaceToClearTopBorder + Game1.tileSize / 4) / 3, Game1.tileSize, Game1.tileSize), null, null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 47, -1, -1), 1f, false);
            this.randomButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + Game1.pixelZoom * 12, this.yPositionOnScreen + Game1.tileSize + Game1.pixelZoom * 14, Game1.pixelZoom * 10, Game1.pixelZoom * 10), Game1.mouseCursors, new Rectangle(381, 361, 10, 10), (float)Game1.pixelZoom, false);
            int num = Game1.tileSize * 2;
            this.leftSelectionButtons.Add(new ClickableTextureComponent("Direction", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize / 4, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num, Game1.tileSize, Game1.tileSize), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44, -1, -1), 1f, false));
            this.rightSelectionButtons.Add(new ClickableTextureComponent("Direction", new Rectangle(this.xPositionOnScreen + Game1.tileSize / 4 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 2, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num, Game1.tileSize, Game1.tileSize), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33, -1, -1), 1f, false));
            if (!this.wizardSource)
            {
                //this.labels.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + Game1.tileSize / 4 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 3 + 8, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 8 + Game1.tileSize * 3, 1, 1), Class1.modContent.LoadString("Strings\\UI:Character_Animal", new object[0])));
            }
            this.labels.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + Game1.tileSize / 4 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 3 + 8, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 16, 1, 1), "Color"));
            this.lightColorPicker = new ColorPicker(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + Game1.tileSize * 5 + Game1.tileSize * 3 / 4 + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder);
            num += Game1.tileSize + 8;
            if (Clean == true)
            {
                for (int x = 1; x <= 16; x++)
                {
                    for (int y = 1; y <= 16; y++)
                    {
                        this.pixels.Add(new Pixel(new ClickableTextureComponent("pixel", new Rectangle((int)(this.xPositionOnScreen * 1.2f) + ((Game1.tileSize / 2) * x), (int)(this.yPositionOnScreen * -2.0f) + ((Game1.tileSize / 2) * y)+Game1.tileSize*1, Game1.tileSize / 2, Game1.tileSize / 2), null, null, Game1.content.Load<Texture2D>(Canvas.whiteTexture), new Rectangle(0, 0, 8, 8), 4f, false), Color.White, new Point(x - 1, y - 1)));
                    }

                }
                CanvasObject.pixels = this.pixels;
            }
            else
            {
                if (this.pixels == null) //Log.AsyncC("this pixels null");
                if (CanvasObject == null) //Log.AsyncC("cnvas object is null");
                if (CanvasObject.pixels == null) //Log.AsyncC("this canvas object ==null");
                this.pixels = CanvasObject.pixels;
            }

            once = false;
            this.lightColorPicker.setColor(Color.White);

            this.nameBox = new TextBox(Game1.content.Load<Texture2D>("LooseSprites\\textBox"), null, Game1.smallFont, Game1.textColor)
            {
                X = this.xPositionOnScreen + Game1.tileSize + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 1,
                Y = this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 4,
                Text = "Untitled"
            };
            this.nameBox.Text = CanvasObject.name;


            this.numbersSelectBox = new TextBox(Game1.content.Load<Texture2D>("LooseSprites\\textBox"), null, Game1.smallFont, Game1.textColor)
            {
                X = this.xPositionOnScreen,
                Y = this.yPositionOnScreen + IClickableMenu.borderWidth + this.height / 2,
                Text = 255.ToString(),
                numbersOnly = true,
                textLimit = string.Concat(255).Length
            };
        }

        private void optionButtonClick(string name)
        {
          
            if (name == "Cancel")
            {
                Game1.exitActiveMenu();
                return;
            }

            if (name == "OK")
            {
                //  StardewModdingAPI.Log.AsyncC(this.LightObject.lightColor);

                this.lightColorPicker.setColor(CanvasObject.drawColor);

                //  StardewModdingAPI.Log.AsyncC(this.LightObject.lightColor);


                //UTIL FUNCTION TO GET CORRECT COLOR
                CanvasObject.drawColor = this.lightColorPicker.getSelectedColor();
                //LightObject.lightColor = Util.invertColor(LightObject.lightColor);

                //Game1.player.Name = this.nameBox.Text.Trim();
                //	Game1.player.favoriteThing = this.favThingBox.Text.Trim();
 
                this.CanvasObject.isPainted = true;
                this.CanvasObject.pixels = this.pixels;

                this.compileImage(this.nameBox.Text);

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
        
            Game1.playSound("coin");
        }

   

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            this.nameBox.Update();
            this.numbersSelectBox.Update();
            if (Convert.ToInt32(this.numbersSelectBox.Text)> 255){
                this.numbersSelectBox.Text = 255.ToString();
            }
            if (Convert.ToInt32(this.numbersSelectBox.Text) < 0)
            {
                this.numbersSelectBox.Text = 0.ToString();
            }
            this.numbersSelectBox.Update();
            
            using (List<Pixel>.Enumerator enumerator = pixels.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    ClickableTextureComponent clickableTextureComponent3 = (ClickableTextureComponent)enumerator.Current.component;
                    if (clickableTextureComponent3.containsPoint(x, y))
                    {
                        if (enumerator.Current.color.A == 0)
                        {
                            enumerator.Current.color.B = 0;
                            enumerator.Current.color.G = 0;
                            enumerator.Current.color.R = 0;
                        }
                        else
                        {
                            enumerator.Current.color.B = ((Byte)(enumerator.Current.color.B / (float)(255 / enumerator.Current.color.A)));
                            enumerator.Current.color.R = ((Byte)(enumerator.Current.color.R / (float)(255 / enumerator.Current.color.A)));
                            enumerator.Current.color.G = ((Byte)(enumerator.Current.color.G / (float)(255 / enumerator.Current.color.A)));
                        }
                      //  Log.AsyncC(enumerator.Current.color);
                        // Log.AsyncM("WOOOOOO");
                        //  clickableTextureComponent3.scale = Math.Min(clickableTextureComponent3.scale + 0.02f, clickableTextureComponent3.baseScale + 0.1f);
                    }
                    else
                    {

                        //  clickableTextureComponent3.scale = Math.Max(clickableTextureComponent3.scale - 0.02f, clickableTextureComponent3.baseScale);
                    }
                }
            }

            if (this.okButton.containsPoint(x, y) && this.canLeaveMenu())
            {
                this.optionButtonClick(this.okButton.name);
                this.okButton.scale -= 0.25f;
                this.okButton.scale = Math.Max(0.75f, this.okButton.scale);
            }

            if (this.cancelButton.containsPoint(x, y))
            {
                this.optionButtonClick(this.cancelButton.name);
                this.cancelButton.scale -= 0.25f;
                this.cancelButton.scale = Math.Max(0.75f, this.cancelButton.scale);
            }

            else if (this.lightColorPicker.containsPoint(x, y))
            {

                CanvasObject.drawColor = this.lightColorPicker.click(x, y);
                CanvasObject.drawColor = Util.invertColor(CanvasObject.drawColor, Convert.ToInt32(this.numbersSelectBox.Text));
                // LightObject.lightColor = Util.invertColor(LightObject.lightColor);
                this.lastHeldColorPicker = this.lightColorPicker;
                colorChanged = true;
            }

            if (this.randomButton.containsPoint(x, y))
            {
                string cueName = "drumkit6";
                if (this.timesRandom > 0)
                {
                    switch (Game1.random.Next(15))
                    {
                        case 0:
                            cueName = "drumkit1";
                            break;
                        case 1:
                            cueName = "dirtyHit";
                            break;
                        case 2:
                            cueName = "axchop";
                            break;
                        case 3:
                            cueName = "hoeHit";
                            break;
                        case 4:
                            cueName = "fishSlap";
                            break;
                        case 5:
                            cueName = "drumkit6";
                            break;
                        case 6:
                            cueName = "drumkit5";
                            break;
                        case 7:
                            cueName = "drumkit6";
                            break;
                        case 8:
                            cueName = "junimoMeep1";
                            break;
                        case 9:
                            cueName = "coin";
                            break;
                        case 10:
                            cueName = "axe";
                            break;
                        case 11:
                            cueName = "hammer";
                            break;
                        case 12:
                            cueName = "drumkit2";
                            break;
                        case 13:
                            cueName = "drumkit4";
                            break;
                        case 14:
                            cueName = "drumkit3";
                            break;
                    }
                }
                Game1.playSound(cueName);
                this.timesRandom++;
                if (Game1.random.NextDouble() < 0.33)
                {
                    if (Game1.player.isMale)
                    {
                        //	Game1.player.changeAccessory(Game1.random.Next(19));
                    }
                    else
                    {
                        //	Game1.player.changeAccessory(Game1.random.Next(6, 19));
                    }
                }
                else
                {
                    //	Game1.player.changeAccessory(-1);
                }
                if (Game1.player.isMale)
                {
                    //Game1.player.changeHairStyle(Game1.random.Next(16));
                }
                else
                {
                    //	Game1.player.changeHairStyle(Game1.random.Next(16, 32));
                }
                Color c = new Color(Game1.random.Next(25, 254), Game1.random.Next(25, 254), Game1.random.Next(25, 254));
                if (Game1.random.NextDouble() < 0.5)
                {
                    c.R /= 2;
                    c.G /= 2;
                    c.B /= 2;
                }
                if (Game1.random.NextDouble() < 0.5)
                {
                    c.R = (byte)Game1.random.Next(15, 50);
                }
                if (Game1.random.NextDouble() < 0.5)
                {
                    c.G = (byte)Game1.random.Next(15, 50);
                }
                if (Game1.random.NextDouble() < 0.5)
                {
                    c.B = (byte)Game1.random.Next(15, 50);
                }

                if (Game1.random.NextDouble() < 0.25)
                {
                    //Game1.player.changeSkinColor(Game1.random.Next(24));
                }
                Color color = new Color(Game1.random.Next(25, 254), Game1.random.Next(25, 254), Game1.random.Next(25, 254));
                if (Game1.random.NextDouble() < 0.5)
                {
                    color.R /= 2;
                    color.G /= 2;
                    color.B /= 2;
                }
                if (Game1.random.NextDouble() < 0.5)
                {
                    color.R = (byte)Game1.random.Next(15, 50);
                }
                if (Game1.random.NextDouble() < 0.5)
                {
                    color.G = (byte)Game1.random.Next(15, 50);
                }
                if (Game1.random.NextDouble() < 0.5)
                {
                    color.B = (byte)Game1.random.Next(15, 50);
                }
                //Game1.player.changePants(color);
                Color c2 = new Color(Game1.random.Next(25, 254), Game1.random.Next(25, 254), Game1.random.Next(25, 254));
                c2.R /= 2;
                c2.G /= 2;
                c2.B /= 2;
                if (Game1.random.NextDouble() < 0.5)
                {
                    c2.R = (byte)Game1.random.Next(15, 50);
                }
                if (Game1.random.NextDouble() < 0.5)
                {
                    c2.G = (byte)Game1.random.Next(15, 50);
                }
                if (Game1.random.NextDouble() < 0.5)
                {
                    c2.B = (byte)Game1.random.Next(15, 50);
                }
                //Game1.player.changeEyeColor(c2);
                this.randomButton.scale = (float)Game1.pixelZoom - 0.5f;


                // c2 = Util.invertColor(c2);
                colorChanged = true;
                this.lightColorPicker.setColor(c2);
                this.CanvasObject.drawColor = Util.invertColor(c2, Convert.ToInt32(this.numbersSelectBox.Text));

            }
        }

        public override void leftClickHeld(int x, int y)
        {
            this.colorPickerTimer -= Game1.currentGameTime.ElapsedGameTime.Milliseconds;
            if (this.colorPickerTimer <= 0)
            {
                if (this.lastHeldColorPicker != null)
                {

                    if (this.lastHeldColorPicker.Equals(this.lightColorPicker))
                    {
                        colorChanged = true;
                        this.CanvasObject.drawColor = Util.invertColor(this.lightColorPicker.clickHeld(x, y),Convert.ToInt32(this.numbersSelectBox.Text));
                        
                    }
                }
                this.colorPickerTimer = 100;
            }
            //THIS MIGHT CRASH SOME STUFF
            using (List<Pixel>.Enumerator enumerator = pixels.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    ClickableTextureComponent clickableTextureComponent3 = (ClickableTextureComponent)enumerator.Current.component;
                    if (clickableTextureComponent3.containsPoint(x, y))
                    {
                        enumerator.Current.color = lightColorPicker.getSelectedColor();
                        enumerator.Current.color.A =(byte) Convert.ToInt32(this.numbersSelectBox.Text);
                        if (enumerator.Current.color.A == 0)
                        {
                            enumerator.Current.color.B = 0;
                            enumerator.Current.color.G = 0;
                            enumerator.Current.color.R = 0;
                        }
                        else
                        {
                            enumerator.Current.color.B = ((Byte)(enumerator.Current.color.B / (float)(255 / enumerator.Current.color.A)));
                            enumerator.Current.color.R = ((Byte)(enumerator.Current.color.R / (float)(255 / enumerator.Current.color.A)));
                            enumerator.Current.color.G = ((Byte)(enumerator.Current.color.G / (float)(255 / enumerator.Current.color.A)));
                        }
                        //Log.AsyncM(enumerator.Current.color);
                        // Log.AsyncM("WOOOOOO");
                        //  clickableTextureComponent3.scale = Math.Min(clickableTextureComponent3.scale + 0.02f, clickableTextureComponent3.baseScale + 0.1f);
                    }
                    else
                    {

                        //  clickableTextureComponent3.scale = Math.Max(clickableTextureComponent3.scale - 0.02f, clickableTextureComponent3.baseScale);
                    }
                }
            }
        }

        public override void releaseLeftClick(int x, int y)
        {

            this.lightColorPicker.releaseClick();
            this.lastHeldColorPicker = null;
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
          

            if (this.okButton.containsPoint(x, y) && this.canLeaveMenu())
            {
                this.okButton.scale = Math.Min(this.okButton.scale + 0.02f, this.okButton.baseScale + 0.1f);
            }
            else
            {
                this.okButton.scale = Math.Max(this.okButton.scale - 0.02f, this.okButton.baseScale);
            }


            if (this.cancelButton.containsPoint(x, y))
            {
                this.cancelButton.scale = Math.Min(this.cancelButton.scale + 0.02f, this.cancelButton.baseScale + 0.1f);
            }
            else
            {
                this.cancelButton.scale = Math.Max(this.cancelButton.scale - 0.02f, this.cancelButton.baseScale);
            }
            this.randomButton.tryHover(x, y, 0.25f);
            this.randomButton.tryHover(x, y, 0.25f);
        }

        public bool canLeaveMenu()
        {
            return this.wizardSource || (Game1.player.name.Length > 0 && Game1.player.farmName.Length > 0 && Game1.player.favoriteThing.Length > 0);
        }

        public override void draw(SpriteBatch b)
        {
           // Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, false, true, null, false);
            foreach (ClickableComponent current in this.labels)
            {
                string text = "";
                Color color = Game1.textColor;

                Utility.drawTextWithShadow(b, current.name, Game1.smallFont, new Vector2((float)current.bounds.X, (float)current.bounds.Y), color, 1f, -1f, -1, -1, 1f, 3);
                if (text.Length > 0)
                {
                    Utility.drawTextWithShadow(b, text, Game1.smallFont, new Vector2((float)(current.bounds.X + Game1.tileSize / 3) - Game1.smallFont.MeasureString(text).X / 2f, (float)(current.bounds.Y + Game1.tileSize / 2)), color, 1f, -1f, -1, -1, 1f, 3);
                }
            }

            foreach(var v in this.pixels)
            {
                v.component.draw(b,v.color,1f);
            }
            this.cancelButton.draw(b, Color.White, 0.75f);

            if (this.canLeaveMenu())
            {
                this.okButton.draw(b, Color.White, 0.75f);
            }
            else
            {
                this.okButton.draw(b, Color.White, 0.75f);
                this.okButton.draw(b, Color.Black * 0.5f, 0.751f);
            }

            this.lightColorPicker.draw(b);

            if (this.hoverText != null && this.hoverTitle != null && this.hoverText.Count<char>() > 0)
            {
                IClickableMenu.drawHoverText(b, Game1.parseText(this.hoverText, Game1.smallFont, Game1.tileSize * 4), Game1.smallFont, 0, 0, -1, this.hoverTitle, -1, null, null, 0, -1, -1, -1, -1, 1f, null);
            }
            this.randomButton.draw(b);

            if (once == false)
            {
                Color c = Util.invertColor(CanvasObject.drawColor,Convert.ToInt32(this.numbersSelectBox.Text));

                this.lightColorPicker.setColor(c);
                once = true;
            }
            this.nameBox.Draw(b);
            this.numbersSelectBox.Draw(b);
            base.drawMouse(b);
        }


        public void compileImage(string s)
        {
            string a1 = Path.Combine(Class1.contentPath, Game1.content.RootDirectory, "Revitalize", "Paint", "decompiled");
            string b1 = Path.Combine(Class1.contentPath, Game1.content.RootDirectory, "Revitalize", "Paint", "compiled");

            string decompiled = Path.Combine(Class1.contentPath,Game1.content.RootDirectory,"Revitalize", "Paint", "decompiled",s);
            string compiled = Path.Combine(Class1.contentPath,Game1.content.RootDirectory, "Revitalize", "Paint", "compiled",s);
            string hate = Path.Combine("Revitalize", "Paint", "compiled", s);

            string arguments = "pack " + "\""+a1+"\"" +" " +"\""+b1+"\"";
            if(File.Exists(decompiled + ".yaml"))
            {
                File.Delete(decompiled + ".yaml");
             //   Log.AsyncC("DELETE THE YAML");
            }
            if (File.Exists(decompiled + ".png"))
            {
                File.Delete(decompiled + ".png");
            //    Log.AsyncC("DELETE THE PNG");
            }
            if (File.Exists(compiled + ".xnb"))
            {
                File.Delete(compiled + ".xnb");
              //  Log.AsyncC("DELETE THE XNB");
            }

            List<string> failure = new List<string>();
            foreach(string s22 in Directory.GetFiles(b1))
            {
              //  Log.AsyncC(s22);
                failure.Add(s22);
            }
            foreach(var v in failure)
            {
                while (File.Exists(v))
                {
                    File.Delete(v);
                }
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();
            /*
            if (File.Exists(compiled + ".xnb"))
            {
                int i = 0;
                List<string> fileList = new List<string>();
                //fileList.Add(Path.Combine(Class1.contentPath, Game1.content.RootDirectory, CanvasObject.texturePath));
                while (File.Exists(compiled + i + ".xnb"))
                {
                    fileList.Add(compiled + i + ".xnb");
                    fileList.Add(decompiled + i + ".yaml");
                    fileList.Add(decompiled + i + ".png");
                    i++;

                    Log.AsyncG("INTERESTING");
                }
                foreach(var v in fileList)
                {
                    File.Delete(v);
                    Log.AsyncM("DELETING THE THING");
                    Log.AsyncO(v);
                }
                hate += i;
                a1 += i;
                b1 += i;
                decompiled += i;
               // File.Delete(compiled + ".xnb");
                //Log.AsyncC("DELETE");
            }
            */
            File.Copy(Path.Combine(Game1.content.RootDirectory, "Revitalize", "Paint", "cleanYAML.yaml"), decompiled+".yaml");
            using (System.Drawing.Bitmap b = new System.Drawing.Bitmap(16, 16))
            {
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(b))
                {
                    int i = 16 * 16;
                    int j = 0;
                  //  Log.AsyncY(i);

                   
                    foreach(var v in pixels)
                    {
                        Color r = Util.invertColor(v.color,v.color.A);
                        r = Util.invertColor(r,r.A);
                       // Log.AsyncM(r + " THIS IS MY TRUE COLOR POWER. FEAR ME");
                        j++;
                      //  Log.AsyncM(j);
                        System.Drawing.Color c= System.Drawing.Color.FromArgb(r.A, r.R, r.G, r.B);
                        b.SetPixel(v.position.X, v.position.Y, c);
                    }
                    //    g.Clear(System.Drawing.Color.Green);
                }
                if (File.Exists(decompiled + ".png"))
                {
                    File.Delete(decompiled + ".png");
                }
                b.Save(decompiled+".png", ImageFormat.Png);
            }

            ProcessStartInfo start = new ProcessStartInfo();

           
            // Enter in the command line arguments, everything you would enter after the executable name itself
            start.Arguments = arguments;
          //  Log.AsyncC(arguments);
            // Enter the executable to run, including the complete path
            start.FileName = "xnb_node.cmd";

          //  Log.AsyncM(start.FileName + " I HATE THIS STUPID GARBAGE");

            if(File.Exists(Path.Combine(Class1.path, "xnb_node.cmd")))
            {
               // Log.AsyncG("YAY");
            }
            else
            {
              //  Log.AsyncM("NOOOO");
            }
            // Do you want to show a console window?
            start.RedirectStandardOutput = true;
            start.WindowStyle = ProcessWindowStyle.Normal;
            start.CreateNoWindow =false;
            start.UseShellExecute = false;
            int exitCode;



            // Run the external process & wait for it to finish
            using (Process proc = Process.Start(start))
            {
                while (!proc.StandardOutput.EndOfStream)
                {
                    string line = proc.StandardOutput.ReadLine();
                    // do something with line
                  //  Log.AsyncY(line);
                }

                proc.WaitForExit();

                // Retrieve the app's exit code
                exitCode = proc.ExitCode;
            }
          //  Log.AsyncM(hate);

            //  CanvasObject.TextureSheet.Dispose();
            CanvasObject.contentManager.Unload();
          //  Class1.modContent.Dispose();
            CanvasObject.TextureSheet = CanvasObject.contentManager.Load<Texture2D>(hate);
  
            CanvasObject.texturePath = hate;
            CanvasObject.name = this.nameBox.Text;
            CanvasObject.isPainted = true;
        }
    }
}
