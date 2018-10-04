using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace SauvignonInStardew
{
    internal class DistillerMenu : LevelUpMenu
    {
        /*********
        ** Fields
        *********/
        /// <summary>Provides an API for checking and changing input state.</summary>
        private readonly IInputHelper Input;

        private Color FirstProfessionColor;
        private Color SecondProfessionColor;
        private Color ThirdProfessionColor;

        private readonly List<string> FirstProfessionDescription = new List<string>() { "Artisan", "Artisan goods (cheese, truffle oil, cloth, etc.) worth 40% more." };

        private readonly List<string> SecondProfessionDescription = new List<string>() { "Agriculturist", "All crops grow 10% faster." };

        private readonly List<string> ThirdProfessionDescription = new List<string>() { "Distiller", "Alcohol (beer, wine, etc.) worth 40% more." };

        private readonly string Title = "Level 10 Farming";

        private readonly string Text = "Choose a profession:";

        private readonly List<int> ProfessionsToChoose = new List<int> { 4, 5, 77 };

        private readonly Rectangle SourceRectForLevelIcon = new Rectangle(0, 0, 16, 16);

        private readonly List<TemporaryAnimatedSprite> LittleStars = new List<TemporaryAnimatedSprite>();

        private int TimerBeforeStart;

        private readonly Texture2D Icon;


        /*********
        ** Public methods
        *********/
        public DistillerMenu(Texture2D icon, IInputHelper input)
        {
            this.Icon = icon;
            this.Input = input;
            this.TimerBeforeStart = 250;
            this.isActive = true;
            this.height = 512;
            this.width = 1344;

            Game1.player.completelyStopAnimatingOrDoingAction();
            this.xPositionOnScreen = 100;
            this.yPositionOnScreen = (int)(Game1.viewport.Height * (1.0 / Game1.options.zoomLevel)) / 4;

            //this.populateClickableComponentList();
            this.allClickableComponents = new List<ClickableComponent>
            {
                // first profession
                new ClickableComponent(new Rectangle(this.xPositionOnScreen, this.yPositionOnScreen + 128, this.width / 3, this.height), "")
                {
                    myID = 102,
                    rightNeighborID = 103
                },

                // second profession
                new ClickableComponent(new Rectangle(this.width / 3 + this.xPositionOnScreen, this.yPositionOnScreen + 128, this.width / 3, this.height), "")
                {
                    myID = 103,
                    leftNeighborID = 102
                },

                // third profession
                new ClickableComponent(new Rectangle((this.width / 3) * 2 + this.xPositionOnScreen, this.yPositionOnScreen + 128, this.width / 3, this.height), "")
                {
                    myID = 104,
                    leftNeighborID = 103
                }
            };
            this.snapToDefaultClickableComponent();
        }

        public override void update(GameTime time)
        {
            if (!this.isActive)
            {
                this.exitThisMenu();
            }

            //stars
            for (int index = this.LittleStars.Count - 1; index >= 0; --index)
            {
                if (this.LittleStars[index].update(time))
                    this.LittleStars.RemoveAt(index);
            }
            if (Game1.random.NextDouble() < 0.03)
            {
                Vector2 position = new Vector2(0.0f, Game1.random.Next(this.yPositionOnScreen - 128, this.yPositionOnScreen - 4) / 20 * 4 * 5 + 32);
                position.X = Game1.random.NextDouble() >= 0.5 ? Game1.random.Next(this.xPositionOnScreen + this.width / 2 + 116, this.xPositionOnScreen + this.width - 160) : Game1.random.Next(this.xPositionOnScreen + this.width / 2 - 228, this.xPositionOnScreen + this.width / 2 - 132);
                if (position.Y < (double)(this.yPositionOnScreen - 64 - 8))
                    position.X = Game1.random.Next(this.xPositionOnScreen + this.width / 2 - 116, this.xPositionOnScreen + this.width / 2 + 116);
                position.X = (float)(position.X / 20.0 * 4.0 * 5.0);
                this.LittleStars.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(364, 79, 5, 5), 80f, 7, 1, position, false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
                {
                    local = true
                });
            }
            //end stars

            if (this.TimerBeforeStart > 0)
            {
                this.TimerBeforeStart -= time.ElapsedGameTime.Milliseconds;
            }
            else
            {
                this.FirstProfessionColor = Game1.textColor;
                this.SecondProfessionColor = Game1.textColor;
                this.ThirdProfessionColor = Game1.textColor;
                if (Game1.getMouseY() > this.yPositionOnScreen + 192 && Game1.getMouseY() < this.yPositionOnScreen + this.height)
                {
                    if (Game1.getMouseX() > this.xPositionOnScreen && Game1.getMouseX() < this.xPositionOnScreen + this.width / 3)
                    {
                        this.FirstProfessionColor = Color.Green;
                        //if ((Game1.input.GetMouseState().LeftButton == ButtonState.Pressed && this.oldMouseState.LeftButton == ButtonState.Released || Game1.options.gamepadControls && (Game1.input.GetGamePadState().IsButtonDown(Buttons.A) && !Game1.oldPadState.IsButtonDown(Buttons.A))) && this.readyToClose())
                        if ((this.Input.IsDown(SButton.MouseLeft)) || (Game1.options.gamepadControls && this.Input.IsDown(SButton.A)))
                        {
                            if (!Game1.player.professions.Contains(4))
                            {
                                Game1.player.professions.Add(this.ProfessionsToChoose[0]);
                                this.getImmediateProfessionPerk(this.ProfessionsToChoose[0]);
                                this.isActive = false;
                                this.informationUp = false;
                                this.isProfessionChooser = false;
                            }
                        }
                    }
                    else if (Game1.getMouseX() > this.xPositionOnScreen + this.width / 3 && Game1.getMouseX() < this.xPositionOnScreen + this.width / 3 + this.width / 3)
                    {
                        this.SecondProfessionColor = Color.Green;
                        if ((this.Input.IsDown(SButton.MouseLeft)) || (Game1.options.gamepadControls && this.Input.IsDown(SButton.A)))
                        {
                            if (!Game1.player.professions.Contains(5))
                            {
                                Game1.player.professions.Add(this.ProfessionsToChoose[1]);
                                this.getImmediateProfessionPerk(this.ProfessionsToChoose[1]);
                                this.isActive = false;
                                this.informationUp = false;
                                this.isProfessionChooser = false;
                            }
                        }
                    }
                    else if (Game1.getMouseX() > this.xPositionOnScreen + this.width / 3 + this.width / 3 && Game1.getMouseX() < this.xPositionOnScreen + this.width / 3 + this.width / 3 + this.width / 3)
                    {
                        this.ThirdProfessionColor = Color.Green;
                        if ((this.Input.IsDown(SButton.MouseLeft)) || (Game1.options.gamepadControls && this.Input.IsDown(SButton.A)))
                        {
                            if (!Game1.player.professions.Contains(77))
                            {
                                Game1.player.professions.Add(this.ProfessionsToChoose[2]);
                                this.getImmediateProfessionPerk(this.ProfessionsToChoose[2]);
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
            foreach (TemporaryAnimatedSprite littleStar in this.LittleStars)
                littleStar.draw(b);
            b.Draw(Game1.mouseCursors, new Vector2(this.xPositionOnScreen + this.width / 2 - 116, this.yPositionOnScreen - 32 + 12), new Rectangle(363, 87, 58, 22), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);

            //top info and actual menu
            Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, false, true);

            this.drawHorizontalPartition(b, this.yPositionOnScreen + 192);

            this.drawVerticalIntersectingPartition(b, this.xPositionOnScreen + this.width / 3 - 32, this.yPositionOnScreen + 192);
            this.drawVerticalIntersectingPartition(b, this.xPositionOnScreen + this.width / 3 - 32 + this.width / 3, this.yPositionOnScreen + 192);

            Utility.drawWithShadow(b, Game1.buffsIcons, new Vector2(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 16), this.SourceRectForLevelIcon, Color.White, 0.0f, Vector2.Zero, 4f, false, 0.88f);

            b.DrawString(Game1.dialogueFont, this.Title, new Vector2(this.xPositionOnScreen + this.width / 2 - Game1.dialogueFont.MeasureString(this.Title).X / 2f, this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 16), Game1.textColor);

            Utility.drawWithShadow(b, Game1.buffsIcons, new Vector2(this.xPositionOnScreen + this.width - IClickableMenu.spaceToClearSideBorder - IClickableMenu.borderWidth - 64, this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 16), this.SourceRectForLevelIcon, Color.White, 0.0f, Vector2.Zero, 4f, false, 0.88f);

            b.DrawString(Game1.smallFont, this.Text, new Vector2(this.xPositionOnScreen + this.width / 2 - Game1.smallFont.MeasureString(this.Text).X / 2f, this.yPositionOnScreen + 64 + IClickableMenu.spaceToClearTopBorder), Game1.textColor);

            //first profession
            //title
            b.DrawString(Game1.dialogueFont, this.FirstProfessionDescription[0], new Vector2(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 32, this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 160), this.FirstProfessionColor);

            //icon
            b.Draw(Game1.mouseCursors, new Vector2(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + this.width / 3 - 112, this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 160 - 16), new Rectangle(this.ProfessionsToChoose[0] % 6 * 16, 624 + this.ProfessionsToChoose[0] / 6 * 16, 16, 16), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);

            //description
            for (int index = 1; index < this.FirstProfessionDescription.Count; ++index)
            {
                b.DrawString(Game1.smallFont, Game1.parseText(this.FirstProfessionDescription[index], Game1.smallFont, this.width / 3 - 64), new Vector2(this.xPositionOnScreen - 4 + IClickableMenu.spaceToClearSideBorder + 32, this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 128 + 8 + 64 * (index + 1)), this.FirstProfessionColor);
            }

            //second profession
            //title
            b.DrawString(Game1.dialogueFont, this.SecondProfessionDescription[0], new Vector2(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + this.width / 3, this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 160), this.SecondProfessionColor);

            //icon
            b.Draw(Game1.mouseCursors, new Vector2(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + (this.width / 3) * 2 - 128, this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 160 - 16), new Rectangle(this.ProfessionsToChoose[1] % 6 * 16, 624 + this.ProfessionsToChoose[1] / 6 * 16, 16, 16), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);

            //description
            for (int index = 1; index < this.SecondProfessionDescription.Count; ++index)
            {
                b.DrawString(Game1.smallFont, Game1.parseText(this.SecondProfessionDescription[index], Game1.smallFont, this.width / 3 - 48), new Vector2(this.xPositionOnScreen - 4 + IClickableMenu.spaceToClearSideBorder + this.width / 3, this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 128 + 8 + 64 * (index + 1)), this.SecondProfessionColor);
            }

            //third profession
            //title
            b.DrawString(Game1.dialogueFont, this.ThirdProfessionDescription[0], new Vector2(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + this.width / 3 + this.width / 3, this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 160), this.ThirdProfessionColor);

            //icon
            b.Draw(this.Icon, new Vector2(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + (this.width / 3) * 3 - 128, this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 160 - 16), new Rectangle(0, 0, 16, 16), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);//this.professionsToChoose[2] % 6 * 16, 624 + this.professionsToChoose[2] / 6 * 16

            //description
            for (int index = 1; index < this.ThirdProfessionDescription.Count; ++index)
            {
                b.DrawString(Game1.smallFont, Game1.parseText(this.ThirdProfessionDescription[index], Game1.smallFont, (this.width / 3 - 48)), new Vector2(this.xPositionOnScreen - 4 + IClickableMenu.spaceToClearSideBorder + this.width / 3 + this.width / 3, this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 128 + 8 + 64 * (index + 1)), this.ThirdProfessionColor);
            }

            this.drawMouse(b);
        }
    }
}
