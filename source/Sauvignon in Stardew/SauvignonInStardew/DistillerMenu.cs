using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System.Reflection;
using Microsoft.Xna.Framework.Input;

namespace Sauvignon_in_Stardew
{
    class DistillerMenu : LevelUpMenu
    {
        private Color firstProfessionColor;
        private Color secondProfessionColor;
        private Color thirdProfessionColor;
        private Color textColor = Game1.textColor;

        private readonly List<string> firstProfessionDescription = new List<string>() { "Artisan", "Artisan goods (cheese, truffle oil, cloth, etc.) worth 40% more." };
        //"Artisan goods (wine, cheese, oil, etc.) worth 40% more."
        private readonly List<string> secondProfessionDescription = new List<string>() { "Agriculturist", "All crops grow 10% faster." };
        //"All crops grow 10% faster."
        private readonly List<string> thirdProfessionDescription = new List<string>() { "Distiller", "Alcohol (beer, wine, etc.) worth 40% more." };
        //"All alcohol (beer, wine, etc) worth 40% more."

        private readonly List<string> extraInfoForFarming = new List<string>() { "+1 Watering Can Proficiency", "+1 Hoe Proficiency" };
        //"+1 Watering Can Proficiency"
        //"+1 Hoe Proficiency"

        private readonly string title = "Level 10 Farming";

        private readonly string text = "Choose a profession:";
        //Level 10 Farming

        public ClickableComponent firstProfession;
        public ClickableComponent secondProfession;
        public ClickableComponent thirdProfession;

        private readonly List<int> professionsToChoose = new List<int>() { 4, 5, 77 };

        private readonly Rectangle sourceRectForLevelIcon = new Rectangle(0, 0, 16, 16);

        private readonly List<TemporaryAnimatedSprite> littleStars = new List<TemporaryAnimatedSprite>();

        private int timerBeforeStart;

        public Texture2D icon;


        public DistillerMenu()
        {

        }

        public DistillerMenu(int skill, int level, Texture2D icon)
        {
            this.icon = icon;
            this.timerBeforeStart = 250;
            this.isActive = true;
            this.height = 512;
            this.width = 1344;

            Game1.player.completelyStopAnimatingOrDoingAction();
            this.xPositionOnScreen = 100;
            this.yPositionOnScreen =  (int)(Game1.viewport.Height * (1.0 / Game1.options.zoomLevel)) / 4;

            this.firstProfession = new ClickableComponent(new Rectangle(this.xPositionOnScreen, this.yPositionOnScreen + 128, this.width / 3, this.height), "")
            {
                myID = 102,
                rightNeighborID = 103
            };

            this.secondProfession = new ClickableComponent(new Rectangle(this.width / 3 + this.xPositionOnScreen, this.yPositionOnScreen + 128, this.width / 3, this.height), "")
            {
                myID = 103,
                leftNeighborID = 102
            };


            this.thirdProfession = new ClickableComponent(new Rectangle((this.width / 3) * 2 + this.xPositionOnScreen, this.yPositionOnScreen + 128, this.width / 3, this.height), "")
            {
                myID = 104,
                leftNeighborID = 103
            };

            //this.populateClickableComponentList();
            this.allClickableComponents = new List<ClickableComponent> { this.firstProfession, this.secondProfession, this.thirdProfession };
            this.snapToDefaultClickableComponent();
        }

        public override void update(GameTime time)
        {
            if (!this.isActive)
            {
                this.exitThisMenu(true);
            }
            //ModEntry.monitor.Log($"Update 1 yPosition is " + this.yPositionOnScreen);
            //stars
            for (int index = this.littleStars.Count - 1; index >= 0; --index)
            {
                if (this.littleStars[index].update(time))
                    this.littleStars.RemoveAt(index);
            }
            if (Game1.random.NextDouble() < 0.03)
            {
                Vector2 position = new Vector2(0.0f, (float)(Game1.random.Next(this.yPositionOnScreen - 128, this.yPositionOnScreen - 4) / 20 * 4 * 5 + 32));
                position.X = Game1.random.NextDouble() >= 0.5 ? (float)Game1.random.Next(this.xPositionOnScreen + this.width / 2 + 116, this.xPositionOnScreen + this.width - 160) : (float)Game1.random.Next(this.xPositionOnScreen + this.width / 2 - 228, this.xPositionOnScreen + this.width / 2 - 132);
                if ((double)position.Y < (double)(this.yPositionOnScreen - 64 - 8))
                    position.X = (float)Game1.random.Next(this.xPositionOnScreen + this.width / 2 - 116, this.xPositionOnScreen + this.width / 2 + 116);
                position.X = (float)((double)position.X / 20.0 * 4.0 * 5.0);
                this.littleStars.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(364, 79, 5, 5), 80f, 7, 1, position, false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, false)
                {
                    local = true
                });
            }
            //end stars

            if (this.timerBeforeStart > 0)
            {
                this.timerBeforeStart -= time.ElapsedGameTime.Milliseconds;
            }
            else
            {
                //ModEntry.monitor.Log($"Update 2 yPosition is " + this.yPositionOnScreen);
                this.firstProfessionColor = Game1.textColor;
                this.secondProfessionColor = Game1.textColor;
                this.thirdProfessionColor = Game1.textColor;
                if (Game1.getMouseY() > this.yPositionOnScreen + 192 && Game1.getMouseY() < this.yPositionOnScreen + this.height)
                {
                    //ModEntry.monitor.Log($"Update 3 yPosition is " + this.yPositionOnScreen);
                    if (Game1.getMouseX() > this.xPositionOnScreen && Game1.getMouseX() < this.xPositionOnScreen + this.width / 3)
                    {
                        this.firstProfessionColor = Color.Green;
                        //if ((Game1.input.GetMouseState().LeftButton == ButtonState.Pressed && this.oldMouseState.LeftButton == ButtonState.Released || Game1.options.gamepadControls && (Game1.input.GetGamePadState().IsButtonDown(Buttons.A) && !Game1.oldPadState.IsButtonDown(Buttons.A))) && this.readyToClose())
                        if ( (ModEntry.helper.Input.IsDown(SButton.MouseLeft)) || (Game1.options.gamepadControls && ModEntry.helper.Input.IsDown(SButton.A)) )
                        {
                            if (!Game1.player.professions.Contains(4))
                            {
                                Game1.player.professions.Add(this.professionsToChoose[0]);
                                this.getImmediateProfessionPerk(this.professionsToChoose[0]);
                                this.isActive = false;
                                this.informationUp = false;
                                this.isProfessionChooser = false;
                            }
                        }
                    }
                    else if (Game1.getMouseX() > this.xPositionOnScreen + this.width / 3 && Game1.getMouseX() < this.xPositionOnScreen + this.width / 3 + this.width / 3)
                    {
                        this.secondProfessionColor = Color.Green;
                        if((ModEntry.helper.Input.IsDown(SButton.MouseLeft)) || (Game1.options.gamepadControls && ModEntry.helper.Input.IsDown(SButton.A)))
                        {
                            if (!Game1.player.professions.Contains(5))
                            {
                                Game1.player.professions.Add(this.professionsToChoose[1]);
                                this.getImmediateProfessionPerk(this.professionsToChoose[1]);
                                this.isActive = false;
                                this.informationUp = false;
                                this.isProfessionChooser = false;
                            }
                        }
                    }
                    else if (Game1.getMouseX() > this.xPositionOnScreen + this.width / 3 + this.width / 3 && Game1.getMouseX() < this.xPositionOnScreen + this.width / 3 + this.width / 3 + this.width / 3)
                    {
                        this.thirdProfessionColor = Color.Green;
                        if((ModEntry.helper.Input.IsDown(SButton.MouseLeft)) || (Game1.options.gamepadControls && ModEntry.helper.Input.IsDown(SButton.A)))
                        {
                            if (!Game1.player.professions.Contains(77))
                            {
                                Game1.player.professions.Add(this.professionsToChoose[2]);
                                this.getImmediateProfessionPerk(this.professionsToChoose[2]);
                                this.isActive = false;
                                this.informationUp = false;
                                this.isProfessionChooser = false;
                            }
                        }
                    }
                }
            }
        }

        public override void draw(SpriteBatch b)
        {
            //stars
            b.Draw(Game1.fadeToBlackRect, new Rectangle(0, 0, Game1.viewport.Width, Game1.viewport.Height), Color.Black * 0.5f);
            foreach (TemporaryAnimatedSprite littleStar in this.littleStars)
                littleStar.draw(b, false, 0, 0, 1f);
            b.Draw(Game1.mouseCursors, new Vector2((float)(this.xPositionOnScreen + this.width / 2 - 116), (float)(this.yPositionOnScreen - 32 + 12)), new Rectangle?(new Rectangle(363, 87, 58, 22)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);

            //top info and actual menu
            Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, false, true, (string)null, false);

            this.drawHorizontalPartition(b, this.yPositionOnScreen + 192, false);

            this.drawVerticalIntersectingPartition(b, this.xPositionOnScreen + this.width / 3 - 32, this.yPositionOnScreen + 192);
            this.drawVerticalIntersectingPartition(b, this.xPositionOnScreen + this.width / 3 - 32 + this.width / 3, this.yPositionOnScreen + 192);

            Utility.drawWithShadow(b, Game1.buffsIcons, new Vector2((float)(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth), (float)(this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 16)), this.sourceRectForLevelIcon, Color.White, 0.0f, Vector2.Zero, 4f, false, 0.88f, -1, -1, 0.35f);

            b.DrawString(Game1.dialogueFont, this.title, new Vector2((float)(this.xPositionOnScreen + this.width / 2) - Game1.dialogueFont.MeasureString(this.title).X / 2f, (float)(this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 16)), Game1.textColor);

            Utility.drawWithShadow(b, Game1.buffsIcons, new Vector2((float)(this.xPositionOnScreen + this.width - IClickableMenu.spaceToClearSideBorder - IClickableMenu.borderWidth - 64), (float)(this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 16)), this.sourceRectForLevelIcon, Color.White, 0.0f, Vector2.Zero, 4f, false, 0.88f, -1, -1, 0.35f);

            b.DrawString(Game1.smallFont, text, new Vector2((float)(this.xPositionOnScreen + this.width / 2) - Game1.smallFont.MeasureString(text).X / 2f, (float)(this.yPositionOnScreen + 64 + IClickableMenu.spaceToClearTopBorder)), Game1.textColor);

            //first profession   
                //title
            b.DrawString(Game1.dialogueFont, this.firstProfessionDescription[0], new Vector2((float)(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 32), (float)(this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 160)), this.firstProfessionColor);

                //icon
            b.Draw(Game1.mouseCursors, new Vector2((float)(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + this.width / 3 - 112), (float)(this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 160 - 16)), new Rectangle?(new Rectangle(this.professionsToChoose[0] % 6 * 16, 624 + this.professionsToChoose[0] / 6 * 16, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);

                //description
            for (int index = 1; index < this.firstProfessionDescription.Count; ++index)
            {
                b.DrawString(Game1.smallFont, Game1.parseText(this.firstProfessionDescription[index], Game1.smallFont, this.width / 3 - 64), new Vector2((float)(this.xPositionOnScreen - 4 + IClickableMenu.spaceToClearSideBorder + 32), (float)(this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 128 + 8 + 64 * (index + 1))), this.firstProfessionColor);
            }

            //second profession     
                //title
            b.DrawString(Game1.dialogueFont, this.secondProfessionDescription[0], new Vector2((float)(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + this.width / 3), (float)(this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 160)), this.secondProfessionColor);

                //icon
            b.Draw(Game1.mouseCursors, new Vector2((float)(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + (this.width / 3) * 2 - 128), (float)(this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 160 - 16)), new Rectangle?(new Rectangle(this.professionsToChoose[1] % 6 * 16, 624 + this.professionsToChoose[1] / 6 * 16, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);

                //description
            for (int index = 1; index < this.secondProfessionDescription.Count; ++index)
            {
                b.DrawString(Game1.smallFont, Game1.parseText(this.secondProfessionDescription[index], Game1.smallFont, this.width / 3 - 48), new Vector2((float)(this.xPositionOnScreen - 4 + IClickableMenu.spaceToClearSideBorder + this.width / 3), (float)(this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 128 + 8 + 64 * (index + 1))), this.secondProfessionColor);
            }

            //third profession
                //title
            b.DrawString(Game1.dialogueFont, this.thirdProfessionDescription[0], new Vector2((float)(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + this.width / 3 + this.width / 3), (float)(this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 160)), this.thirdProfessionColor);

                //icon
            b.Draw(this.icon, new Vector2((float)(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + (this.width / 3) * 3 - 128), (float)(this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 160 - 16)), new Rectangle?(new Rectangle(0, 0, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);//this.professionsToChoose[2] % 6 * 16, 624 + this.professionsToChoose[2] / 6 * 16

            //description
            for (int index = 1; index < this.thirdProfessionDescription.Count; ++index)
            {
                b.DrawString(Game1.smallFont, Game1.parseText(this.thirdProfessionDescription[index], Game1.smallFont, (this.width / 3 - 48)), new Vector2((float)(this.xPositionOnScreen - 4 + IClickableMenu.spaceToClearSideBorder + this.width / 3 + this.width / 3), (float)(this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 128 + 8 + 64 * (index + 1))), this.thirdProfessionColor);
            }

            this.drawMouse(b);
        }
    }
}
