/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Revitalize.Framework.Energy;
using Revitalize.Framework.Objects;
using Revitalize.Framework.Utilities;
using StardewValley;
using StardustCore.UIUtilities;
using StardustCore.UIUtilities.MenuComponents.ComponentsV2.Buttons;

namespace Revitalize.Framework.Menus.Machines
{
    /// <summary>
    /// TODO:
    /// Add in minutes remaining display
    /// Add in remaining inventory space display.
    /// Make crafting menu require the object passed in to count down before crafting the recipe.
    /// </summary>
    public class MachineSummaryMenu : IClickableMenuExtended
    {

        /// <summary>
        /// The custom object to be gathering information from.
        /// </summary>
        private CustomObject objectSource;
        /// <summary>
        /// The background color for the menu.
        /// </summary>
        private Color backgroundColor;
        /// <summary>
        /// The hover text to display for the menu.
        /// </summary>
        private string hoverText;


        private AnimatedButton batteryBackground;
        private AnimatedButton battergyEnergyGuage;
        private Vector2 energyPosition;
        private Texture2D energyTexture;
        private Vector2 itemDisplayOffset;


        private AnimatedButton clockSprite;
        private Vector2 timeDisplayLocation;
        private AnimatedButton energyRequiredButton;
        private Vector2 energyRequiredDisplayLocation;
        private AnimatedButton storageButton;
        private Vector2 storageRemainingDisplayLocation;


        private AnimatedButton inputFluidTank1Button;
        private AnimatedButton inputFluidTank2Button;
        private AnimatedButton outputFluidTankButton;
        private Vector2 fluidDisplayLocation;

        private int requiredEnergyPer10Min;

        private EnergyManager energy
        {
            get
            {
                return this.objectSource.GetEnergyManager();
            }
        }


        /// <summary>
        /// Should this menu draw the battery for the energy guage?
        /// </summary>
        private bool shouldDrawBattery
        {
            get
            {
                return this.energy.maxEnergy != 0 && ModCore.Configs.machinesConfig.doMachinesConsumeEnergy==true;
            }
        }

        public MachineSummaryMenu()
        {

        }

        public MachineSummaryMenu(int x, int y, int width, int height, Color BackgroundColor, CustomObject SourceObject,int RequiredEnergyPer10Min) : base(x, y, width, height, false)
        {

            this.objectSource = SourceObject;
            this.backgroundColor = BackgroundColor;
            this.energyTexture = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
            this.energyMeterColorSwap();

            this.timeDisplayLocation = new Vector2(this.xPositionOnScreen + (this.width * .1f), this.yPositionOnScreen + (this.height * .25f));
            this.energyRequiredDisplayLocation = this.timeDisplayLocation + new Vector2(0, 64);
            this.storageRemainingDisplayLocation = this.energyRequiredDisplayLocation + new Vector2(0, 64);
            this.fluidDisplayLocation = this.storageRemainingDisplayLocation + new Vector2(0, 64);

            this.energyPosition = new Vector2(this.xPositionOnScreen + this.width - 128, this.yPositionOnScreen + this.height - 72 * 4);
            this.batteryBackground = new AnimatedButton(new StardustCore.Animations.AnimatedSprite("BatteryFrame", this.energyPosition, new StardustCore.Animations.AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Menus.EnergyMenu", "BatteryFrame"), new StardustCore.Animations.Animation(0, 0, 32, 64)), Color.White), new Rectangle(0, 0, 32, 64), 4f);
            this.battergyEnergyGuage = new AnimatedButton(new StardustCore.Animations.AnimatedSprite("BatteryEnergyGuage", this.energyPosition, new StardustCore.Animations.AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Menus.EnergyMenu", "BatteryEnergyGuage"), new StardustCore.Animations.Animation(0, 0, 32, 64)), Color.White), new Rectangle(0, 0, 32, 64), 4f);

            this.itemDisplayOffset = ObjectUtilities.GetDimensionOffsetFromItem(this.objectSource);
            this.clockSprite= new AnimatedButton(new StardustCore.Animations.AnimatedSprite("Time Remaining",this.timeDisplayLocation, new StardustCore.Animations.AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Menus", "Clock"), new StardustCore.Animations.Animation(0, 0, 18, 18)), Color.White), new Rectangle(0, 0, 18, 18), 2f);
            this.energyRequiredButton=new AnimatedButton(new StardustCore.Animations.AnimatedSprite("Energy Required", this.energyRequiredDisplayLocation, new StardustCore.Animations.AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Menus.EnergyMenu", "LightningBolt"), new StardustCore.Animations.Animation(0, 0, 16, 16)), Color.White), new Rectangle(0, 0, 16, 16), 2f);
            this.storageButton = new AnimatedButton(new StardustCore.Animations.AnimatedSprite("Storage Remaining", this.storageRemainingDisplayLocation, new StardustCore.Animations.AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Menus", "Chest"), new StardustCore.Animations.Animation(0, 0, 16, 32)), Color.White), new Rectangle(0, 0, 16, 32), 1f);


            if (this.objectSource.GetFluidManager().inputTank1.capacity > 0)
            {
                this.inputFluidTank1Button = new AnimatedButton(new StardustCore.Animations.AnimatedSprite("Input 1 fluid:", this.fluidDisplayLocation, new StardustCore.Animations.AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Menus.EnergyMenu", this.objectSource.info.fluidManager.inputTank1.fluid != null ? "DropletColored" : "DropletOutline"), new StardustCore.Animations.Animation(0, 0, 16, 16)), this.objectSource.info.fluidManager.inputTank1.fluid != null ? this.objectSource.info.fluidManager.inputTank1.fluid.color : Color.White), new Rectangle(0, 0, 16, 16), 2f);
            }
            if (this.objectSource.GetFluidManager().inputTank2.capacity > 0)
            {
                this.inputFluidTank2Button = new AnimatedButton(new StardustCore.Animations.AnimatedSprite("Input 2 fluid:", this.fluidDisplayLocation + new Vector2(0, 64), new StardustCore.Animations.AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Menus.EnergyMenu", this.objectSource.info.fluidManager.inputTank2.fluid != null ? "DropletColored" : "DropletOutline"), new StardustCore.Animations.Animation(0, 0, 16, 16)), this.objectSource.info.fluidManager.inputTank2.fluid != null ? this.objectSource.info.fluidManager.inputTank2.fluid.color : Color.White), new Rectangle(0, 0, 16, 16), 2f);
            }
            //ModCore.log(this.objectSource.info.fluidManager.outputTank.fluid != null ? "Color of fluid:" + this.objectSource.info.fluidManager.outputTank.fluid.color.ToString() : "Color is null!");
            if (this.objectSource.GetFluidManager().outputTank.capacity > 0)
            {
                this.outputFluidTankButton = new AnimatedButton(new StardustCore.Animations.AnimatedSprite("Output fluid:", this.fluidDisplayLocation + new Vector2(0, 128), new StardustCore.Animations.AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Menus.EnergyMenu", this.objectSource.info.fluidManager.outputTank.fluid != null ? "DropletColored" : "DropletOutline"), new StardustCore.Animations.Animation(0, 0, 16, 16)), this.objectSource.info.fluidManager.outputTank.fluid != null ? this.objectSource.info.fluidManager.outputTank.fluid.color : Color.White), new Rectangle(0, 0, 16, 16), 2f);
            }
            this.requiredEnergyPer10Min = RequiredEnergyPer10Min;
        }

        public override void performHoverAction(int x, int y)
        {
            bool hovered = false;
            if (this.batteryBackground.containsPoint(x, y) && this.shouldDrawBattery)
            {
                this.hoverText = "Energy: " + this.energy.energyDisplayString;
                hovered = true;
            }
            if (this.clockSprite.containsPoint(x, y))
            {
                this.hoverText = "Time Remaining: " + System.Environment.NewLine + TimeUtilities.GetVerboseTimeString(this.objectSource.MinutesUntilReady);
                hovered = true;
            }
            if (this.energyRequiredButton.containsPoint(x, y))
            {
                this.hoverText = this.getProperEnergyDescriptionString(this.requiredEnergyPer10Min);
                hovered = true;
            }

            if (this.objectSource.info.fluidManager.InteractsWithFluids)
            {
                if (this.inputFluidTank1Button != null)
                {
                    if (this.inputFluidTank1Button.containsPoint(x, y))
                    {
                        if (this.objectSource.info.fluidManager.inputTank1.capacity > 0)
                        {
                            this.hoverText = "Input Tank 1: " + this.objectSource.info.fluidManager.inputTank1.getFluidDisplayString();
                            hovered = true;
                        }
                    }
                }

                if (this.inputFluidTank2Button != null)
                {
                    if (this.inputFluidTank2Button.containsPoint(x, y))
                    {
                        if (this.objectSource.info.fluidManager.inputTank2.capacity > 0)
                        {
                            this.hoverText = "Input Tank 2: " + this.objectSource.info.fluidManager.inputTank2.getFluidDisplayString();
                            hovered = true;
                        }
                    }

                }

                if (this.outputFluidTankButton != null)
                {
                    if (this.outputFluidTankButton.containsPoint(x, y))
                    {
                        if (this.objectSource.info.fluidManager.outputTank.capacity > 0)
                        {
                            this.hoverText = "Output Tank: " + this.objectSource.info.fluidManager.outputTank.getFluidDisplayString();
                            hovered = true;
                        }
                    }
                }
            }

            if (hovered == false)
            {
                this.hoverText = "";
            }
        }

        protected virtual string getProperEnergyDescriptionString(int EnergyAmount)
        {
            if (this.energy.consumesEnergy)
            {
                return "Energy required per 10 minutes to run: " + System.Environment.NewLine + EnergyAmount;

            }
            else if (this.energy.producesEnergy)
            {
                return "Produces " + EnergyAmount + " energy per 10 minutes.";
            }
            else return "";
        }

        /// <summary>
        /// Draws the menu to the screen.
        /// </summary>
        /// <param name="b"></param>
        public override void draw(SpriteBatch b)
        {
            this.drawDialogueBoxBackground(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, this.backgroundColor);

            this.clockSprite.draw(b);
            b.DrawString(Game1.smallFont, TimeUtilities.GetTimeString(this.objectSource.MinutesUntilReady), this.timeDisplayLocation + new Vector2(0, 36f), Color.Black);


            //Draw the energy on the screen.

            if (this.shouldDrawBattery)
            {
                this.batteryBackground.draw(b, 1f, 1f);
                this.energyMeterColorSwap();
                b.Draw(this.energyTexture, new Rectangle((int)this.energyPosition.X + (int)(11 * this.batteryBackground.scale), (int)this.energyPosition.Y + (int)(18 * this.batteryBackground.scale) + (int)(46 * this.batteryBackground.scale), (int)((9 * this.batteryBackground.scale)), (int)(46 * this.batteryBackground.scale * this.energy.energyPercentRemaining)), new Rectangle(0, 0, 1, 1), Color.White, 0f, new Vector2(0f, 1f), SpriteEffects.None, 0.2f);
                this.battergyEnergyGuage.draw(b, 1f, 1f);

                this.energyRequiredButton.draw(b);
                b.DrawString(Game1.smallFont, this.requiredEnergyPer10Min + " E/10m", this.energyRequiredDisplayLocation + new Vector2(0, 36f), Color.Black);
            }

            if (this.objectSource.info.inventory.HasInventory)
            {
                this.storageButton.draw(b);
                b.DrawString(Game1.smallFont, "Storage remaining: " + (this.objectSource.info.inventory.capacity - this.objectSource.info.inventory.ItemCount) + "/" + this.objectSource.info.inventory.capacity, this.storageRemainingDisplayLocation + new Vector2(0, 32f), Color.Black);
            }

            if (this.objectSource.info.fluidManager.InteractsWithFluids)
            {
                if (this.objectSource.info.fluidManager.inputTank1.capacity > 0)
                {
                    this.inputFluidTank1Button.draw(b);
                    b.DrawString(Game1.smallFont, this.objectSource.info.fluidManager.inputTank1.getFluidDisplayString(), this.fluidDisplayLocation + new Vector2(32, 0f), Color.Black);
                }
                if (this.objectSource.info.fluidManager.inputTank2.capacity > 0)
                {
                    this.inputFluidTank2Button.draw(b);
                    b.DrawString(Game1.smallFont, this.objectSource.info.fluidManager.inputTank2.getFluidDisplayString(), this.fluidDisplayLocation + new Vector2(32, 64f), Color.Black);
                }
                if (this.objectSource.info.fluidManager.outputTank.capacity > 0)
                {
                    //ModCore.log("Color:" + this.objectSource.GetFluidManager().outputTank.fluid.color);
                    this.outputFluidTankButton.draw(b);
                    b.DrawString(Game1.smallFont, this.objectSource.info.fluidManager.outputTank.getFluidDisplayString(), this.fluidDisplayLocation + new Vector2(32, 128f), Color.Black);
                }
            }

            this.objectSource.drawFullyInMenu(b, new Vector2((int)(this.xPositionOnScreen + (this.width / 2) - (this.itemDisplayOffset.X / 2)), (int)(this.yPositionOnScreen + 128f)), .24f);
            Vector2 nameOffset = Game1.dialogueFont.MeasureString(this.objectSource.DisplayName);

            b.DrawString(Game1.dialogueFont, this.objectSource.DisplayName, new Vector2(this.xPositionOnScreen + (this.width / 2) - nameOffset.X / 2, (this.yPositionOnScreen + 150f)) + new Vector2(0, ObjectUtilities.GetHeightOffsetFromItem(this.objectSource)), Color.Black);

            if (string.IsNullOrEmpty(this.hoverText) == false)
            {
                IClickableMenuExtended.drawHoverText(b, this.hoverText, Game1.dialogueFont);
            }


            this.drawMouse(b);


        }
        /// <summary>
        /// Swaps the color for the energy bar meter depending on how much energy is left.
        /// </summary>

        private void energyMeterColorSwap()
        {
            Color col = new Color();
            //ModCore.log("Energy is: " + this.energy.energyPercentRemaining);
            if (this.energy.energyPercentRemaining > .75d)
            {
                col = Color.Green;
            }
            else if (this.energy.energyPercentRemaining > .5d && this.energy.energyPercentRemaining <= .75d)
            {
                col = Color.GreenYellow;
            }
            else if (this.energy.energyPercentRemaining > .25d && this.energy.energyPercentRemaining <= .5d)
            {
                col = Color.Yellow;
            }
            else if (this.energy.energyPercentRemaining > .10d && this.energy.energyPercentRemaining <= .25d)
            {
                col = Color.Orange;
            }
            else
            {
                col = Color.Red;
            }

            Color[] color = new Color[1]
            {
                col
            };
            this.energyTexture.SetData<Color>(color);
        }

        
    }
}
