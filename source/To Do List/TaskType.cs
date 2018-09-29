using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace ToDoMod
{
    class TaskType : IClickableMenu
    {
        public TextBox textBox;
        public ClickableComponent textBoxCC;
        private TextBoxEvent e;
        
        public const int region_doneNamingButton = 202;

        public ClickableTextureComponent doneNamingButton;

        public TaskType(SpriteFont fontToUse)
        {
            this.textBox = new TextBox((Texture2D)null, (Texture2D)null, fontToUse, Game1.textColor)
            {
                /* Positioning stuff */
                Width = Game1.tileSize * 13 - (IClickableMenu.borderWidth * 2),
                Height = Game1.tileSize * 3
            };
            Vector2 centeringOnScreen = Utility.getTopLeftPositionForCenteringOnScreen(this.textBox.Width, this.textBox.Height, 0, 0);
            this.textBox.X = (int)centeringOnScreen.X - 2;
            this.textBox.Y = Game1.viewport.Height / 2 + 200;
           
            this.textBox.OnEnterPressed += this.e;
            Game1.keyboardDispatcher.Subscriber = (IKeyboardSubscriber)this.textBox;
            this.textBox.Text = "";
            this.textBox.Selected = true;
            this.textBox.limitWidth = true;

            ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(new Rectangle(this.textBox.X + this.textBox.Width + Game1.tileSize + Game1.tileSize * 3 / 4 - Game1.pixelZoom * 2, Game1.viewport.Height / 2 + Game1.pixelZoom, Game1.tileSize, Game1.tileSize), Game1.mouseCursors, new Rectangle(381, 361, 10, 10), (float)Game1.pixelZoom, false);
            int num1 = 203;
            textureComponent1.myID = num1;
            int num2 = 202;
            textureComponent1.leftNeighborID = num2;

            ClickableTextureComponent textureComponent2 = new ClickableTextureComponent(new Rectangle(this.textBox.X + this.textBox.Width + Game1.tileSize / 2 + Game1.pixelZoom, Game1.viewport.Height / 2 - Game1.pixelZoom * 2 + 200, Game1.tileSize, Game1.tileSize), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46, -1, -1), 1f, false);
            int num3 = 202;
            textureComponent2.myID = num3;
            int num4 = 203;
            textureComponent2.rightNeighborID = num4;
            int num5 = 204;
            textureComponent2.leftNeighborID = num5;

            this.doneNamingButton = textureComponent2;
            this.textBoxCC = new ClickableComponent(new Rectangle(this.textBox.X, this.textBox.Y, this.textBox.Width, this.textBox.Height), "")
            {
                myID = 204,
                rightNeighborID = 202
            };
            


        }

        /// <summary>
        /// Take a key press - implemented to accept typing in the text box.
        /// </summary>
        public override void receiveKeyPress(Keys key)
        {
            if (this.textBox.Selected || Game1.options.doesInputListContain(Game1.options.menuButton, key))
                return;
            base.receiveKeyPress(key);
        }


        /// <summary>
        /// Draw the components to the screen.
        /// </summary>
        public override void draw(SpriteBatch batch)
        {
            base.draw(batch);
            this.textBox.Draw(batch);
            this.doneNamingButton.draw(batch);
        }

        public bool Selected { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <summary>
        /// Not used, but needs implementing
        /// </summary>
        public override void receiveRightClick(int x, int y, bool playSound = true)
        {

        }
    }





}