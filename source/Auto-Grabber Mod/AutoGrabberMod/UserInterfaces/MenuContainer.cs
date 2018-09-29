using System;
using System.Collections.Generic;
using System.Linq;
using AutoGrabberMod.Features;
using AutoGrabberMod.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace AutoGrabberMod.UserInterfaces
{
    public class MenuContainer : IClickableMenu
    {
        public const int itemsPerPage = 7;

        public const int indexOfGraphicsPage = 6;                

        public List<ClickableComponent> optionSlots = new List<ClickableComponent>();

        public int currentItemIndex;

        private ClickableTextureComponent upArrow;

        private ClickableTextureComponent downArrow;

        private ClickableTextureComponent scrollBar;

        private readonly ClickableComponent Title;

        private bool scrolling;

        private List<OptionsElement> options = new List<OptionsElement>();

        private Rectangle scrollBarRunner;

        private int optionsSlotHeld = -1;

        private readonly ModConfig Config;
        private bool CanClose;

        public MenuContainer(AutoGrabber[] grabbers, int indexGrabberSettings, ModConfig config) : base(Game1.viewport.Width / 2 - (600 + IClickableMenu.borderWidth * 2) / 2, Game1.viewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2, 800 + IClickableMenu.borderWidth * 2, 600 + IClickableMenu.borderWidth * 2)
        {
            this.Config = config;
            this.Title = new ClickableComponent(new Rectangle(this.xPositionOnScreen + this.width / 2, this.yPositionOnScreen, Game1.tileSize * 4, Game1.tileSize), "Auto-Grabber Mod");
            this.upArrow = new ClickableTextureComponent(new Rectangle(base.xPositionOnScreen + width + 16, base.yPositionOnScreen + 64, 44, 48), Game1.mouseCursors, new Rectangle(421, 459, 11, 12), 4f, false);
            this.downArrow = new ClickableTextureComponent(new Rectangle(base.xPositionOnScreen + width + 16, base.yPositionOnScreen + height - 64, 44, 48), Game1.mouseCursors, new Rectangle(421, 472, 11, 12), 4f, false);
            this.scrollBar = new ClickableTextureComponent(new Rectangle(this.upArrow.bounds.X + 12, this.upArrow.bounds.Y + this.upArrow.bounds.Height + 4, 24, 40), Game1.mouseCursors, new Rectangle(435, 463, 6, 10), 4f, false);
            this.scrollBarRunner = new Rectangle(this.scrollBar.bounds.X, this.upArrow.bounds.Y + this.upArrow.bounds.Height + 4, this.scrollBar.bounds.Width, height - 128 - this.upArrow.bounds.Height - 8);
            for (int i = 0; i < 7; i++)
            {
                this.optionSlots.Add(new ClickableComponent(new Rectangle(base.xPositionOnScreen + 16, base.yPositionOnScreen + 80 + 4 + i * ((height - 128) / 7), width - 32, (height - 128) / 7 + 4), string.Concat(i)));
            }
            var dropDownItems = new List<string>();
            foreach (AutoGrabber grabber in grabbers) dropDownItems.Add($"{grabber.InstanceName}");
            dropDownItems.Add("Global Options");
            this.options.Add(new DropDown("", dropDownItems, value =>
            {
                if (value != indexGrabberSettings)
                {
                    Game1.activeClickableMenu = new MenuContainer(grabbers, value, config);
                }
            }, indexGrabberSettings, x: 40, y: 40));

            if (grabbers.Length < (indexGrabberSettings + 1))
            {
                Utilities.Monitor.Log($"  Viewing settings for: Global");
                this.options.Add(new OptionsCheckbox("Show All Range Grids", Config.ShowAllRangeGrids, value =>
                {
                    Config.ShowAllRangeGrids = value;
                }));

                this.options.Add(new OptionsCheckbox("Show Range Grid When Mouse Over", Config.ShowRangeGridMouseOver, value =>
                {
                    Config.ShowRangeGridMouseOver = value;
                }));

                this.options.Add(new OptionsSlider("Max Range", Config.MaxRange, 1, 100, value =>
                {
                    if (value == 0) value = 1;
                    Config.MaxRange = value;
                    foreach (var g in grabbers.Where(g => g.Range > value).ToArray())
                    {
                        g.Range = value;
                        g.Update();
                    }
                }));

                this.options.Add(new OptionsCheckbox("Allow Auto Harvesting", Config.AllowAutoHarvest, value =>
                {
                    Config.AllowAutoHarvest = value;
                    if (!value)
                    {
                        Utilities.Monitor.Log("Value is false for harvesting setting false to all");
                        foreach (var g in grabbers.Where(g => (bool)g.FeatureType<Harvest>().Value).ToArray())
                        {
                            g.FeatureType<Harvest>().Value = value;
                            g.Update();
                        }
                    }
                }));
                this.options.Add(new OptionsCheckbox("Allow Auto Hoe Tiles", Config.AllowAutoHoe, value =>
                {
                    Config.AllowAutoHoe = value;
                    if (!value)
                    {
                        foreach (var g in grabbers.Where(g => (bool)g.FeatureType<HoeTiles>().Value).ToArray())
                        {
                            g.FeatureType<HoeTiles>().Value = value;
                            g.Update();
                        }
                    }
                }));
                this.options.Add(new OptionsCheckbox("Allow Auto Fertilize Soil", Config.AllowAutoFertilize, value =>
                {
                    Config.AllowAutoFertilize = value;
                    if (!value)
                    {
                        foreach (var g in grabbers.Where(g => (bool)g.FeatureType<Fertilize>().Value).ToArray())
                        {
                            g.FeatureType<Fertilize>().Value = value;
                            g.Update();
                        }
                    }
                }));
                this.options.Add(new OptionsCheckbox("Allow Auto Seeding", Config.AllowAutoSeed, value =>
                {
                    Config.AllowAutoSeed = value;
                    if (!value)
                    {
                        foreach (var g in grabbers.Where(g => (bool)g.FeatureType<PlantSeeds>().Value).ToArray())
                        {
                            g.FeatureType<PlantSeeds>().Value = value;
                            g.Update();
                        }
                    }
                }));
                this.options.Add(new OptionsCheckbox("Allow Auto Watering", Config.AllowAutoWater, value =>
                {
                    Config.AllowAutoWater = value;
                    if (!value)
                    {
                        foreach (var g in grabbers.Where(g => (bool)g.FeatureType<WaterFields>().Value).ToArray())
                        {
                            g.FeatureType<WaterFields>().Value = value;
                            g.Update();
                        }
                    }
                }));
                this.options.Add(new OptionsCheckbox("Allow Auto Forage", Config.AllowAutoForage, value =>
                {
                    Config.AllowAutoForage = value;
                    if (!value)
                    {
                        foreach (var g in grabbers.Where(g => (bool)g.FeatureType<Forage>().Value).ToArray())
                        {
                            g.FeatureType<Forage>().Value = value;
                            g.Update();
                        }
                    }
                }));
                this.options.Add(new OptionsCheckbox("Allow Auto Dig Artifacts", Config.AllowAutoDig, value =>
                {
                    Config.AllowAutoDig = value;
                    if (!value)
                    {
                        foreach (var g in grabbers.Where(g => (bool)g.FeatureType<DigArtifacts>().Value).ToArray())
                        {
                            g.FeatureType<DigArtifacts>().Value = value;
                            g.Update();
                        }
                    }
                }));
                this.options.Add(new OptionsCheckbox("Allow Auto Pet", Config.AllowAutoPet, value =>
                {
                    Config.AllowAutoPet = value;
                    if (!value)
                    {
                        foreach (var g in grabbers.Where(g => (bool)g.FeatureType<PetAnimals>().Value).ToArray())
                        {
                            g.FeatureType<PetAnimals>().Value = value;
                            g.Update();
                        }
                    }
                }));
            }
            else
            {
                AutoGrabber selectedGrabber = grabbers[indexGrabberSettings];
                //Utilities.Monitor.Log($"  Viewing settings for: {selectedGrabber.InstanceName}");                

                //Range options
                var grid = new OptionsCheckbox("Show Grid", selectedGrabber.ShowRange, value =>
                {
                    selectedGrabber.ShowRange = value;
                    selectedGrabber.Update();
                });
                grid.greyedOut = selectedGrabber.RangeEntireMap;

                this.options.Add(new OptionsCheckbox("Use Entire Map", selectedGrabber.RangeEntireMap, value =>
                {
                    selectedGrabber.RangeEntireMap = value;
                    selectedGrabber.Update();
                    grid.greyedOut = selectedGrabber.RangeEntireMap;
                }));
                this.options.Add(new OptionsSlider("Range", selectedGrabber.Range, 1, config.MaxRange, value =>
                {
                    if (value == 0) value = 1;
                    selectedGrabber.Range = value;
                    selectedGrabber.Update();
                }, () => selectedGrabber.RangeEntireMap));

                this.options.Add(grid);

                foreach (var feature in selectedGrabber.FeaturesConfig)
                {
                    if (feature.IsAllowed)
                    {
                        this.options.AddRange(feature.InterfaceElement());
                    }
                }               

                this.options.Add(new OptionsCheckbox("Gain Experience", selectedGrabber.GainExperience, value =>
                {
                    selectedGrabber.GainExperience = value;
                    selectedGrabber.Update();
                }));
            }

        }

        public override void receiveKeyPress(Keys key)
        {
            bool isExitKey = Game1.options.menuButton.Contains(new InputButton(key)) || (this.Config.OpenMenuKey.TryGetKeyboard(out Keys exitKey) && key == exitKey);
            if (isExitKey && this.readyToClose() && this.CanClose)
            {
                Game1.exitActiveMenu();
                Game1.soundBank.PlayCue("bigDeSelect");
                return;
            }
            this.CanClose = true;         
            base.receiveKeyPress(key);
        }

        private void setScrollBarToCurrentIndex()
        {
            if (this.options.Count > 0)
            {
                this.scrollBar.bounds.Y = this.scrollBarRunner.Height / Math.Max(1, this.options.Count - 7 + 1) * this.currentItemIndex + this.upArrow.bounds.Bottom + 4;
                if (this.currentItemIndex == this.options.Count - 7)
                {
                    this.scrollBar.bounds.Y = this.downArrow.bounds.Y - this.scrollBar.bounds.Height - 4;
                }
            }
        }

        public override void leftClickHeld(int x, int y)
        {
            if (!GameMenu.forcePreventClose)
            {
                base.leftClickHeld(x, y);
                if (this.scrolling)
                {
                    int y2 = this.scrollBar.bounds.Y;
                    this.scrollBar.bounds.Y = Math.Min(base.yPositionOnScreen + base.height - 64 - 12 - this.scrollBar.bounds.Height, Math.Max(y, base.yPositionOnScreen + this.upArrow.bounds.Height + 20));
                    float num = (float)(y - this.scrollBarRunner.Y) / (float)this.scrollBarRunner.Height;
                    this.currentItemIndex = Math.Min(this.options.Count - 7, Math.Max(0, (int)((float)this.options.Count * num)));
                    this.setScrollBarToCurrentIndex();
                    if (y2 != this.scrollBar.bounds.Y)
                    {
                        Game1.playSound("shiny4");
                    }
                }
                else if (this.optionsSlotHeld != -1 && this.optionsSlotHeld + this.currentItemIndex < this.options.Count)
                {
                    this.options[this.currentItemIndex + this.optionsSlotHeld].leftClickHeld(x - this.optionSlots[this.optionsSlotHeld].bounds.X, y - this.optionSlots[this.optionsSlotHeld].bounds.Y);
                }
            }
        } 

        public override void receiveScrollWheelAction(int direction)
        {
            if (!GameMenu.forcePreventClose)
            {
                base.receiveScrollWheelAction(direction);
                if (direction > 0 && this.currentItemIndex > 0)
                {
                    this.upArrowPressed();
                    Game1.playSound("shiny4");
                }
                else if (direction < 0 && this.currentItemIndex < Math.Max(0, this.options.Count - 7))
                {
                    this.downArrowPressed();
                    Game1.playSound("shiny4");
                }
            }
        }

        public override void releaseLeftClick(int x, int y)
        {
            if (!GameMenu.forcePreventClose)
            {
                base.releaseLeftClick(x, y);
                if (this.optionsSlotHeld != -1 && this.optionsSlotHeld + this.currentItemIndex < this.options.Count)
                {
                    this.options[this.currentItemIndex + this.optionsSlotHeld].leftClickReleased(x - this.optionSlots[this.optionsSlotHeld].bounds.X, y - this.optionSlots[this.optionsSlotHeld].bounds.Y);
                }
                this.optionsSlotHeld = -1;
                this.scrolling = false;
            }
        }

        private void downArrowPressed()
        {
            this.downArrow.scale = this.downArrow.baseScale;
            this.currentItemIndex++;
            this.setScrollBarToCurrentIndex();
        }

        private void upArrowPressed()
        {
            this.upArrow.scale = this.upArrow.baseScale;
            this.currentItemIndex--;
            this.setScrollBarToCurrentIndex();
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (!GameMenu.forcePreventClose)
            {
                if (this.downArrow.containsPoint(x, y) && this.currentItemIndex < Math.Max(0, this.options.Count - 7))
                {
                    this.downArrowPressed();
                    Game1.playSound("shwip");
                }
                else if (this.upArrow.containsPoint(x, y) && this.currentItemIndex > 0)
                {
                    this.upArrowPressed();
                    Game1.playSound("shwip");
                }
                else if (this.scrollBar.containsPoint(x, y))
                {
                    this.scrolling = true;
                }
                else if (!this.downArrow.containsPoint(x, y) && x > base.xPositionOnScreen + base.width && x < base.xPositionOnScreen + base.width + 128 && y > base.yPositionOnScreen && y < base.yPositionOnScreen + base.height)
                {
                    this.scrolling = true;
                    this.leftClickHeld(x, y);
                    this.releaseLeftClick(x, y);
                }
                this.currentItemIndex = Math.Max(0, Math.Min(this.options.Count - 7, this.currentItemIndex));
                int num = 0;
                while (true)
                {
                    if (num < this.optionSlots.Count)
                    {
                        if (this.optionSlots[num].bounds.Contains(x, y) && this.currentItemIndex + num < this.options.Count && this.options[this.currentItemIndex + num].bounds.Contains(x - this.optionSlots[num].bounds.X, y - this.optionSlots[num].bounds.Y))
                        {
                            break;
                        }
                        num++;
                        continue;
                    }
                    return;
                }
                this.options[this.currentItemIndex + num].receiveLeftClick(x - this.optionSlots[num].bounds.X, y - this.optionSlots[num].bounds.Y);
                this.optionsSlotHeld = num;
            }
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
        }       

        public override void draw(SpriteBatch spriteBatch)
        {
            if (!Game1.options.showMenuBackground)
                spriteBatch.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.4f);

            Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, false, true);
            Utilities.DrawTextBox(this.Title.bounds.X, this.Title.bounds.Y, Game1.dialogueFont, this.Title.name, 1);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null);
            for (int i = 0; i < this.optionSlots.Count; i++)
            {
                if (this.currentItemIndex >= 0 && this.currentItemIndex + i < this.options.Count)
                {
                    this.options[this.currentItemIndex + i].draw(spriteBatch, this.optionSlots[i].bounds.X, this.optionSlots[i].bounds.Y);
                }
            }
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null);
            if (!GameMenu.forcePreventClose)
            {
                this.upArrow.draw(spriteBatch);
                this.downArrow.draw(spriteBatch);
                if (this.options.Count > 7)
                {
                    IClickableMenu.drawTextureBox(spriteBatch, Game1.mouseCursors, new Rectangle(403, 383, 6, 6), this.scrollBarRunner.X, this.scrollBarRunner.Y, this.scrollBarRunner.Width, this.scrollBarRunner.Height, Color.White, 4f, false);
                    this.scrollBar.draw(spriteBatch);
                }
            }        

            if (!Game1.options.hardwareCursor)
                Game1.spriteBatch.Draw(Game1.mouseCursors, new Vector2(Game1.getMouseX(), Game1.getMouseY()), Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, Game1.options.SnappyMenus ? 44 : 0, 16, 16), Color.White * Game1.mouseCursorTransparency, 0.0f, Vector2.Zero, Game1.pixelZoom + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 1f);
        }
    }
}
