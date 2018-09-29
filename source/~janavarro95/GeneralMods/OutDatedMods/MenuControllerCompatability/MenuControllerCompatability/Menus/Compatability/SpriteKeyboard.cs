using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Minigames;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MenuControllerCompatability
{
    public class SpriteKeyboard : IClickableMenu
    {


        private List<ClickableComponent> labels = new List<ClickableComponent>();

        public List<ClickableTextureComponent> keyboardButtonsLowerCase;
        public List<ClickableTextureComponent> keyboardButtonsUpperCase;
        public List<Color> randomList;
        bool upperCase;

        private ClickableTextureComponent okButton;

        private TextBox nameBox;



        private string hoverText;

        private string hoverTitle;

        private ClickableComponent nameLabel;

        public static float textureScale = 4.0f;


        public SpriteKeyboard() : base(Game1.viewport.Width / 2 - (632 + IClickableMenu.borderWidth * 2) / 2, Game1.viewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2 - Game1.tileSize, 632 + IClickableMenu.borderWidth * 2, 600 + IClickableMenu.borderWidth * 2 + Game1.tileSize, false)
        {
            keyboardButtonsLowerCase = new List<ClickableTextureComponent>();
            keyboardButtonsUpperCase = new List<ClickableTextureComponent>();
            randomList = new List<Color>();
            this.setUpPositions();
           // Game1.player.faceDirection(2);
           // Game1.player.FarmerSprite.StopAnimation();
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
           

            this.keyboardButtonsLowerCase.Clear();
            this.keyboardButtonsUpperCase.Clear();
            upperCase = false;

            this.okButton = new ClickableTextureComponent("OK", new Rectangle(this.xPositionOnScreen + this.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - Game1.tileSize, this.yPositionOnScreen + this.height - IClickableMenu.borderWidth - IClickableMenu.spaceToClearTopBorder + Game1.tileSize / 4, Game1.tileSize, Game1.tileSize), null, null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46, -1, -1), 1f, false);
            this.nameBox = new TextBox(Game1.content.Load<Texture2D>("LooseSprites\\textBox"), null, Game1.smallFont, Game1.textColor)
            {
                X = this.xPositionOnScreen + Game1.tileSize + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 4,
                Y = this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 4,
                Text = Game1.player.name
            };
            this.labels.Add(this.nameLabel = new ClickableComponent(new Rectangle(this.xPositionOnScreen + Game1.tileSize / 4 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 3 + 8, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 8, 1, 1), Game1.content.LoadString("Strings\\UI:Character_Name", new object[0])));


            int row1 = 4;
            this.keyboardButtonsLowerCase.Add(new ClickableTextureComponent("1", new Rectangle(xPositionOnScreen + (Game1.tileSize * 1), this.yPositionOnScreen + (Game1.tileSize * row1), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("1"), new Rectangle(0, 0, 16, 16), textureScale, false));
            this.keyboardButtonsLowerCase.Add(new ClickableTextureComponent("2", new Rectangle(xPositionOnScreen + (Game1.tileSize * 2), this.yPositionOnScreen + (Game1.tileSize * row1), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("2"), new Rectangle(0, 0, 16, 16), textureScale, false));
            this.keyboardButtonsLowerCase.Add(new ClickableTextureComponent("3", new Rectangle(xPositionOnScreen + (Game1.tileSize * 3), this.yPositionOnScreen + (Game1.tileSize * row1), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("3"), new Rectangle(0, 0, 16, 16), textureScale, false));
            this.keyboardButtonsLowerCase.Add(new ClickableTextureComponent("4", new Rectangle(xPositionOnScreen + (Game1.tileSize * 4), this.yPositionOnScreen + (Game1.tileSize * row1), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("4"), new Rectangle(0, 0, 16, 16), textureScale, false));
            this.keyboardButtonsLowerCase.Add(new ClickableTextureComponent("5", new Rectangle(xPositionOnScreen + (Game1.tileSize * 5), this.yPositionOnScreen + (Game1.tileSize * row1), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("5"), new Rectangle(0, 0, 16, 16), textureScale, false));
            this.keyboardButtonsLowerCase.Add(new ClickableTextureComponent("6", new Rectangle(xPositionOnScreen + (Game1.tileSize * 6), this.yPositionOnScreen + (Game1.tileSize * row1), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("6"), new Rectangle(0, 0, 16, 16), textureScale, false));
            this.keyboardButtonsLowerCase.Add(new ClickableTextureComponent("7", new Rectangle(xPositionOnScreen + (Game1.tileSize * 7), this.yPositionOnScreen + (Game1.tileSize * row1), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("7"), new Rectangle(0, 0, 16, 16), textureScale, false));
            this.keyboardButtonsLowerCase.Add(new ClickableTextureComponent("8", new Rectangle(xPositionOnScreen + (Game1.tileSize * 8), this.yPositionOnScreen + (Game1.tileSize * row1), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("8"), new Rectangle(0, 0, 16, 16), textureScale, false));
            this.keyboardButtonsLowerCase.Add(new ClickableTextureComponent("9", new Rectangle(xPositionOnScreen + (Game1.tileSize * 9), this.yPositionOnScreen + (Game1.tileSize * row1), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("9"), new Rectangle(0, 0, 16, 16), textureScale, false));

            this.keyboardButtonsUpperCase.Add(new ClickableTextureComponent("1", new Rectangle(xPositionOnScreen + (Game1.tileSize * 1), this.yPositionOnScreen + (Game1.tileSize * row1), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("1"), new Rectangle(0, 0, 16, 16), textureScale, false));
            this.keyboardButtonsUpperCase.Add(new ClickableTextureComponent("2", new Rectangle(xPositionOnScreen + (Game1.tileSize * 2), this.yPositionOnScreen + (Game1.tileSize * row1), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("2"), new Rectangle(0, 0, 16, 16), textureScale, false));
            this.keyboardButtonsUpperCase.Add(new ClickableTextureComponent("3", new Rectangle(xPositionOnScreen + (Game1.tileSize * 3), this.yPositionOnScreen + (Game1.tileSize * row1), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("3"), new Rectangle(0, 0, 16, 16), textureScale, false));
            this.keyboardButtonsUpperCase.Add(new ClickableTextureComponent("4", new Rectangle(xPositionOnScreen + (Game1.tileSize * 4), this.yPositionOnScreen + (Game1.tileSize * row1), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("4"), new Rectangle(0, 0, 16, 16), textureScale, false));
            this.keyboardButtonsUpperCase.Add(new ClickableTextureComponent("5", new Rectangle(xPositionOnScreen + (Game1.tileSize * 5), this.yPositionOnScreen + (Game1.tileSize * row1), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("5"), new Rectangle(0, 0, 16, 16), textureScale, false));
            this.keyboardButtonsUpperCase.Add(new ClickableTextureComponent("6", new Rectangle(xPositionOnScreen + (Game1.tileSize * 6), this.yPositionOnScreen + (Game1.tileSize * row1), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("6"), new Rectangle(0, 0, 16, 16), textureScale, false));
            this.keyboardButtonsUpperCase.Add(new ClickableTextureComponent("7", new Rectangle(xPositionOnScreen + (Game1.tileSize * 7), this.yPositionOnScreen + (Game1.tileSize * row1), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("7"), new Rectangle(0, 0, 16, 16), textureScale, false));
            this.keyboardButtonsUpperCase.Add(new ClickableTextureComponent("8", new Rectangle(xPositionOnScreen + (Game1.tileSize * 8), this.yPositionOnScreen + (Game1.tileSize * row1), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("8"), new Rectangle(0, 0, 16, 16), textureScale, false));
            this.keyboardButtonsUpperCase.Add(new ClickableTextureComponent("9", new Rectangle(xPositionOnScreen + (Game1.tileSize * 9), this.yPositionOnScreen + (Game1.tileSize * row1), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("9"), new Rectangle(0, 0, 16, 16), textureScale, false));


            int row2 = 5;
            this.keyboardButtonsLowerCase.Add(new ClickableTextureComponent("q", new Rectangle(xPositionOnScreen + (Game1.tileSize * 1), this.yPositionOnScreen + (Game1.tileSize * row2), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("lowercaseQ"), new Rectangle(0, 0, 16, 16), textureScale, false));
            this.keyboardButtonsLowerCase.Add(new ClickableTextureComponent("w", new Rectangle(xPositionOnScreen + (Game1.tileSize * 2), this.yPositionOnScreen + (Game1.tileSize * row2), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("lowercaseW"), new Rectangle(0, 0, 16, 16), textureScale, false));
            this.keyboardButtonsLowerCase.Add(new ClickableTextureComponent("e", new Rectangle(xPositionOnScreen + (Game1.tileSize * 3), this.yPositionOnScreen + (Game1.tileSize * row2), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("lowercaseE"), new Rectangle(0, 0, 16, 16), textureScale, false));
            this.keyboardButtonsLowerCase.Add(new ClickableTextureComponent("r", new Rectangle(xPositionOnScreen + (Game1.tileSize * 4), this.yPositionOnScreen + (Game1.tileSize * row2), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("lowercaseR"), new Rectangle(0, 0, 16, 16), textureScale, false));
            this.keyboardButtonsLowerCase.Add(new ClickableTextureComponent("t", new Rectangle(xPositionOnScreen + (Game1.tileSize * 5), this.yPositionOnScreen + (Game1.tileSize * row2), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("lowercaseT"), new Rectangle(0, 0, 16, 16), textureScale, false));
            this.keyboardButtonsLowerCase.Add(new ClickableTextureComponent("y", new Rectangle(xPositionOnScreen + (Game1.tileSize * 6), this.yPositionOnScreen + (Game1.tileSize * row2), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("lowercaseY"), new Rectangle(0, 0, 16, 16), textureScale, false));
            this.keyboardButtonsLowerCase.Add(new ClickableTextureComponent("u", new Rectangle(xPositionOnScreen + (Game1.tileSize * 7), this.yPositionOnScreen + (Game1.tileSize * row2), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("lowercaseU"), new Rectangle(0, 0, 16, 16), textureScale, false));
            this.keyboardButtonsLowerCase.Add(new ClickableTextureComponent("i", new Rectangle(xPositionOnScreen + (Game1.tileSize * 8), this.yPositionOnScreen + (Game1.tileSize * row2), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("lowercaseI"), new Rectangle(0, 0, 16, 16), textureScale, false));
            this.keyboardButtonsLowerCase.Add(new ClickableTextureComponent("o", new Rectangle(xPositionOnScreen + (Game1.tileSize * 9), this.yPositionOnScreen + (Game1.tileSize * row2), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("lowercaseO"), new Rectangle(0, 0, 16, 16), textureScale, false));
            this.keyboardButtonsLowerCase.Add(new ClickableTextureComponent("p", new Rectangle(xPositionOnScreen + (Game1.tileSize * 10), this.yPositionOnScreen + (Game1.tileSize * row2), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("lowercaseP"), new Rectangle(0, 0, 16, 16), textureScale, false));

            this.keyboardButtonsUpperCase.Add(new ClickableTextureComponent("Q", new Rectangle(xPositionOnScreen + (Game1.tileSize * 1), this.yPositionOnScreen + (Game1.tileSize * row2), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("capitalQ"), new Rectangle(0, 0, 16, 16), textureScale, false));
            this.keyboardButtonsUpperCase.Add(new ClickableTextureComponent("W", new Rectangle(xPositionOnScreen + (Game1.tileSize * 2), this.yPositionOnScreen + (Game1.tileSize * row2), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("capitalW"), new Rectangle(0, 0, 16, 16), textureScale, false));
            this.keyboardButtonsUpperCase.Add(new ClickableTextureComponent("E", new Rectangle(xPositionOnScreen + (Game1.tileSize * 3), this.yPositionOnScreen + (Game1.tileSize * row2), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("capitalE"), new Rectangle(0, 0, 16, 16), textureScale, false));
            this.keyboardButtonsUpperCase.Add(new ClickableTextureComponent("R", new Rectangle(xPositionOnScreen + (Game1.tileSize * 4), this.yPositionOnScreen + (Game1.tileSize * row2), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("capitalR"), new Rectangle(0, 0, 16, 16), textureScale, false));
            this.keyboardButtonsUpperCase.Add(new ClickableTextureComponent("T", new Rectangle(xPositionOnScreen + (Game1.tileSize * 5), this.yPositionOnScreen + (Game1.tileSize * row2), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("capitalT"), new Rectangle(0, 0, 16, 16), textureScale, false));
            this.keyboardButtonsUpperCase.Add(new ClickableTextureComponent("Y", new Rectangle(xPositionOnScreen + (Game1.tileSize * 6), this.yPositionOnScreen + (Game1.tileSize * row2), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("capitalY"), new Rectangle(0, 0, 16, 16), textureScale, false));
            this.keyboardButtonsUpperCase.Add(new ClickableTextureComponent("U", new Rectangle(xPositionOnScreen + (Game1.tileSize * 7), this.yPositionOnScreen + (Game1.tileSize * row2), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("capitalU"), new Rectangle(0, 0, 16, 16), textureScale, false));
            this.keyboardButtonsUpperCase.Add(new ClickableTextureComponent("I", new Rectangle(xPositionOnScreen + (Game1.tileSize * 8), this.yPositionOnScreen + (Game1.tileSize * row2), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("capitalI"), new Rectangle(0, 0, 16, 16), textureScale, false));
            this.keyboardButtonsUpperCase.Add(new ClickableTextureComponent("O", new Rectangle(xPositionOnScreen + (Game1.tileSize * 9), this.yPositionOnScreen + (Game1.tileSize * row2), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("capitalO"), new Rectangle(0, 0, 16, 16), textureScale, false));
            this.keyboardButtonsUpperCase.Add(new ClickableTextureComponent("P", new Rectangle(xPositionOnScreen + (Game1.tileSize * 10), this.yPositionOnScreen + (Game1.tileSize * row2), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("capitalP"), new Rectangle(0, 0, 16, 16), textureScale, false));


            int row3 = 6;
            this.keyboardButtonsLowerCase.Add(new ClickableTextureComponent("a", new Rectangle(xPositionOnScreen + (Game1.tileSize * 1), this.yPositionOnScreen + (Game1.tileSize * row3), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("lowercaseA"), new Rectangle(0, 0, 16, 16), textureScale, false));
            this.keyboardButtonsLowerCase.Add(new ClickableTextureComponent("s", new Rectangle(xPositionOnScreen + (Game1.tileSize * 2), this.yPositionOnScreen + (Game1.tileSize * row3), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("lowercaseS"), new Rectangle(0, 0, 16, 16), textureScale, false));
            this.keyboardButtonsLowerCase.Add(new ClickableTextureComponent("d", new Rectangle(xPositionOnScreen + (Game1.tileSize * 3), this.yPositionOnScreen + (Game1.tileSize * row3), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("lowercaseD"), new Rectangle(0, 0, 16, 16), textureScale, false));
            this.keyboardButtonsLowerCase.Add(new ClickableTextureComponent("f", new Rectangle(xPositionOnScreen + (Game1.tileSize * 4), this.yPositionOnScreen + (Game1.tileSize * row3), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("lowercaseF"), new Rectangle(0, 0, 16, 16), textureScale, false));
            this.keyboardButtonsLowerCase.Add(new ClickableTextureComponent("g", new Rectangle(xPositionOnScreen + (Game1.tileSize * 5), this.yPositionOnScreen + (Game1.tileSize * row3), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("lowercaseG"), new Rectangle(0, 0, 16, 16), textureScale, false));
            this.keyboardButtonsLowerCase.Add(new ClickableTextureComponent("h", new Rectangle(xPositionOnScreen + (Game1.tileSize * 6), this.yPositionOnScreen + (Game1.tileSize * row3), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("lowercaseH"), new Rectangle(0, 0, 16, 16), textureScale, false));
            this.keyboardButtonsLowerCase.Add(new ClickableTextureComponent("j", new Rectangle(xPositionOnScreen + (Game1.tileSize * 7), this.yPositionOnScreen + (Game1.tileSize * row3), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("lowercaseJ"), new Rectangle(0, 0, 16, 16), textureScale, false));
            this.keyboardButtonsLowerCase.Add(new ClickableTextureComponent("k", new Rectangle(xPositionOnScreen + (Game1.tileSize * 8), this.yPositionOnScreen + (Game1.tileSize * row3), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("lowercaseK"), new Rectangle(0, 0, 16, 16), textureScale, false));
            this.keyboardButtonsLowerCase.Add(new ClickableTextureComponent("l", new Rectangle(xPositionOnScreen + (Game1.tileSize * 9), this.yPositionOnScreen + (Game1.tileSize * row3), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("lowercaseL"), new Rectangle(0, 0, 16, 16), textureScale, false));

            this.keyboardButtonsUpperCase.Add(new ClickableTextureComponent("A", new Rectangle(xPositionOnScreen + (Game1.tileSize * 1), this.yPositionOnScreen + (Game1.tileSize * row3), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("capitalA"), new Rectangle(0, 0, 16, 16), textureScale, false));
            this.keyboardButtonsUpperCase.Add(new ClickableTextureComponent("S", new Rectangle(xPositionOnScreen + (Game1.tileSize * 2), this.yPositionOnScreen + (Game1.tileSize * row3), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("capitalS"), new Rectangle(0, 0, 16, 16), textureScale, false));
            this.keyboardButtonsUpperCase.Add(new ClickableTextureComponent("D", new Rectangle(xPositionOnScreen + (Game1.tileSize * 3), this.yPositionOnScreen + (Game1.tileSize * row3), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("capitalD"), new Rectangle(0, 0, 16, 16), textureScale, false));
            this.keyboardButtonsUpperCase.Add(new ClickableTextureComponent("F", new Rectangle(xPositionOnScreen + (Game1.tileSize * 4), this.yPositionOnScreen + (Game1.tileSize * row3), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("capitalF"), new Rectangle(0, 0, 16, 16), textureScale, false));
            this.keyboardButtonsUpperCase.Add(new ClickableTextureComponent("G", new Rectangle(xPositionOnScreen + (Game1.tileSize * 5), this.yPositionOnScreen + (Game1.tileSize * row3), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("capitalG"), new Rectangle(0, 0, 16, 16), textureScale, false));
            this.keyboardButtonsUpperCase.Add(new ClickableTextureComponent("H", new Rectangle(xPositionOnScreen + (Game1.tileSize * 6), this.yPositionOnScreen + (Game1.tileSize * row3), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("capitalH"), new Rectangle(0, 0, 16, 16), textureScale, false));
            this.keyboardButtonsUpperCase.Add(new ClickableTextureComponent("J", new Rectangle(xPositionOnScreen + (Game1.tileSize * 7), this.yPositionOnScreen + (Game1.tileSize * row3), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("capitalJ"), new Rectangle(0, 0, 16, 16), textureScale, false));
            this.keyboardButtonsUpperCase.Add(new ClickableTextureComponent("K", new Rectangle(xPositionOnScreen + (Game1.tileSize * 8), this.yPositionOnScreen + (Game1.tileSize * row3), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("capitalK"), new Rectangle(0, 0, 16, 16), textureScale, false));
            this.keyboardButtonsUpperCase.Add(new ClickableTextureComponent("L", new Rectangle(xPositionOnScreen + (Game1.tileSize * 9), this.yPositionOnScreen + (Game1.tileSize * row3), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("capitalL"), new Rectangle(0, 0, 16, 16), textureScale, false));


            int row4 = 7;
            this.keyboardButtonsLowerCase.Add(new ClickableTextureComponent("z", new Rectangle(xPositionOnScreen + (Game1.tileSize * 1), this.yPositionOnScreen + (Game1.tileSize * row4), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("lowercaseZ"), new Rectangle(0, 0, 16, 16), textureScale, false));
            this.keyboardButtonsLowerCase.Add(new ClickableTextureComponent("x", new Rectangle(xPositionOnScreen + (Game1.tileSize * 2), this.yPositionOnScreen + (Game1.tileSize * row4), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("lowercaseX"), new Rectangle(0, 0, 16, 16), textureScale, false));
            this.keyboardButtonsLowerCase.Add(new ClickableTextureComponent("c", new Rectangle(xPositionOnScreen + (Game1.tileSize * 3), this.yPositionOnScreen + (Game1.tileSize * row4), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("lowercaseC"), new Rectangle(0, 0, 16, 16), textureScale, false));
            this.keyboardButtonsLowerCase.Add(new ClickableTextureComponent("v", new Rectangle(xPositionOnScreen + (Game1.tileSize * 4), this.yPositionOnScreen + (Game1.tileSize * row4), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("lowercaseV"), new Rectangle(0, 0, 16, 16), textureScale, false));
            this.keyboardButtonsLowerCase.Add(new ClickableTextureComponent("b", new Rectangle(xPositionOnScreen + (Game1.tileSize * 5), this.yPositionOnScreen + (Game1.tileSize * row4), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("lowercaseB"), new Rectangle(0, 0, 16, 16), textureScale, false));
            this.keyboardButtonsLowerCase.Add(new ClickableTextureComponent("n", new Rectangle(xPositionOnScreen + (Game1.tileSize * 6), this.yPositionOnScreen + (Game1.tileSize * row4), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("lowercaseN"), new Rectangle(0, 0, 16, 16), textureScale, false));
            this.keyboardButtonsLowerCase.Add(new ClickableTextureComponent("m", new Rectangle(xPositionOnScreen + (Game1.tileSize * 7), this.yPositionOnScreen + (Game1.tileSize * row4), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("lowercaseM"), new Rectangle(0, 0, 16, 16), textureScale, false));

            this.keyboardButtonsUpperCase.Add(new ClickableTextureComponent("Z", new Rectangle(xPositionOnScreen + (Game1.tileSize * 1), this.yPositionOnScreen + (Game1.tileSize * row4), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("capitalZ"), new Rectangle(0, 0, 16, 16), textureScale, false));
            this.keyboardButtonsUpperCase.Add(new ClickableTextureComponent("X", new Rectangle(xPositionOnScreen + (Game1.tileSize * 2), this.yPositionOnScreen + (Game1.tileSize * row4), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("capitalX"), new Rectangle(0, 0, 16, 16), textureScale, false));
            this.keyboardButtonsUpperCase.Add(new ClickableTextureComponent("C", new Rectangle(xPositionOnScreen + (Game1.tileSize * 3), this.yPositionOnScreen + (Game1.tileSize * row4), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("capitalC"), new Rectangle(0, 0, 16, 16), textureScale, false));
            this.keyboardButtonsUpperCase.Add(new ClickableTextureComponent("V", new Rectangle(xPositionOnScreen + (Game1.tileSize * 4), this.yPositionOnScreen + (Game1.tileSize * row4), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("capitalV"), new Rectangle(0, 0, 16, 16), textureScale, false));
            this.keyboardButtonsUpperCase.Add(new ClickableTextureComponent("B", new Rectangle(xPositionOnScreen + (Game1.tileSize * 5), this.yPositionOnScreen + (Game1.tileSize * row4), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("capitalB"), new Rectangle(0, 0, 16, 16), textureScale, false));
            this.keyboardButtonsUpperCase.Add(new ClickableTextureComponent("N", new Rectangle(xPositionOnScreen + (Game1.tileSize * 6), this.yPositionOnScreen + (Game1.tileSize * row4), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("capitalN"), new Rectangle(0, 0, 16, 16), textureScale, false));
            this.keyboardButtonsUpperCase.Add(new ClickableTextureComponent("M", new Rectangle(xPositionOnScreen + (Game1.tileSize * 7), this.yPositionOnScreen + (Game1.tileSize * row4), Game1.tileSize, Game1.tileSize), null, null, Class1.loadTexture("capitalM"), new Rectangle(0, 0, 16, 16), textureScale, false));
            Random r;
            try
            {
                int s = 0;
                List<string> nameList = new List<string>();
                foreach (var v in Game1.locations)
                {
                    foreach (var c in v.characters)
                    {
                        nameList.Add(c.name);
                    }
                }
                foreach (var v in nameList)
                {
                    try
                    {
                        s += Game1.player.getFriendshipLevelForNPC(v);
                    }
                    catch(Exception err)
                    {
                        continue;
                    }
                }

                 r = new Random(Game1.player.money + (int)Game1.player.totalMoneyEarned * Game1.timeOfDay + s+Game1.random.Next());
            }
            catch(Exception e)
            {
                r = new Random(Game1.weather_debris + Game1.realMilliSecondsPerGameTenMinutes+Game1.random.Next());
            }

            foreach (var v in keyboardButtonsLowerCase)
            {
                int R = r.Next(0, 255);
                int G = r.Next(0, 255);
                int B = r.Next(0, 255);
                int A = 255;
                Color c = new Color(R, G, B, A);

                randomList.Add(c);
            }

        }

        private void optionButtonClick(string name)
        {
            Log.AsyncC(name);

            if (name == "OK")
            {
                if (!this.canLeaveMenu())
                {
                    return;
                }
                Game1.player.Name = this.nameBox.Text.Trim();
                   // Game1.exitActiveMenu();
                if (Game1.currentMinigame != null && Game1.currentMinigame is Intro)
                {
                    (Game1.currentMinigame as Intro).doneCreatingCharacter();
                }        
            }
            else
            {
                if(name!="BackSpace" || name != "Back" ||name!="Shift"||name!="Caps")
                {
                    this.nameBox.Text += name;
                }
            }


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


            foreach (ClickableComponent current4 in this.keyboardButtonsLowerCase)
            {
                if (current4.containsPoint(x, y))
                {
                    optionButtonClick(current4.name);
                }
            }

            if (this.okButton.containsPoint(x, y) && this.canLeaveMenu())
            {
                this.optionButtonClick(this.okButton.name);
                this.okButton.scale -= 0.25f;
                this.okButton.scale = Math.Max(0.75f, this.okButton.scale);
            }

        
                this.nameBox.Update();

            
        }

        public override void leftClickHeld(int x, int y)
        {
           
        }

        public override void releaseLeftClick(int x, int y)
        {
            
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
        }

        public override void receiveKeyPress(Keys key)
        {
            if (key == Keys.Tab)
            {
                if (this.nameBox.Selected)
                {
                    this.nameBox.Selected = false;
                    return;
                }

                this.nameBox.SelectMe();
            }
        }

        public override void performHoverAction(int x, int y)
        {
            this.hoverText = "";
            this.hoverTitle = "";
            /*
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
            */


            /*
            if (!this.wizardSource)
            {
                using (List<ClickableComponent>.Enumerator enumerator = this.genderButtons.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        ClickableTextureComponent clickableTextureComponent3 = (ClickableTextureComponent)enumerator.Current;
                        if (clickableTextureComponent3.containsPoint(x, y))
                        {
                            clickableTextureComponent3.scale = Math.Min(clickableTextureComponent3.scale + 0.02f, clickableTextureComponent3.baseScale + 0.1f);
                        }
                        else
                        {
                            clickableTextureComponent3.scale = Math.Max(clickableTextureComponent3.scale - 0.02f, clickableTextureComponent3.baseScale);
                        }
                    }
                }
                using (List<ClickableComponent>.Enumerator enumerator = this.petButtons.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        ClickableTextureComponent clickableTextureComponent4 = (ClickableTextureComponent)enumerator.Current;
                        if (clickableTextureComponent4.containsPoint(x, y))
                        {
                            clickableTextureComponent4.scale = Math.Min(clickableTextureComponent4.scale + 0.02f, clickableTextureComponent4.baseScale + 0.1f);
                        }
                        else
                        {
                            clickableTextureComponent4.scale = Math.Max(clickableTextureComponent4.scale - 0.02f, clickableTextureComponent4.baseScale);
                        }
-                    }
                }
            }
            */
            if (this.okButton.containsPoint(x, y) && this.canLeaveMenu())
            {
                this.okButton.scale = Math.Min(this.okButton.scale + 0.02f, this.okButton.baseScale + 0.1f);
            }
            else
            {
                this.okButton.scale = Math.Max(this.okButton.scale - 0.02f, this.okButton.baseScale);
            }

        }

        public bool canLeaveMenu()
        {
            return true;
        }

        public override void draw(SpriteBatch b)
        {
            Game1.drawDialogueBox(this.xPositionOnScreen/2, this.yPositionOnScreen, (int)(this.width*1.5f), this.height, false, true, null, false);
           Game1.player.name = this.nameBox.Text;
            foreach (ClickableComponent current in this.labels)
            {
                string text = "";
                Color color = Game1.textColor;
                if (current == this.nameLabel)
                {
                    color = ((Game1.player.name.Length < 1) ? Color.Red : Game1.textColor);
                    
                }
                else
                {
                    color = Game1.textColor;
                }
                Utility.drawTextWithShadow(b, current.name, Game1.smallFont, new Vector2((float)current.bounds.X, (float)current.bounds.Y), color, 1f, -1f, -1, -1, 1f, 3);
                if (text.Length > 0)
                {
                    Utility.drawTextWithShadow(b, text, Game1.smallFont, new Vector2((float)(current.bounds.X + Game1.tileSize / 3) - Game1.smallFont.MeasureString(text).X / 2f, (float)(current.bounds.Y + Game1.tileSize / 2)), color, 1f, -1f, -1, -1, 1f, 3);
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

         
                this.nameBox.Draw(b);
            
            if (this.hoverText != null && this.hoverTitle != null && this.hoverText.Count<char>() > 0)
            {
                IClickableMenu.drawHoverText(b, Game1.parseText(this.hoverText, Game1.smallFont, Game1.tileSize * 4), Game1.smallFont, 0, 0, -1, this.hoverTitle, -1, null, null, 0, -1, -1, -1, -1, 1f, null);
            }
            if (upperCase == false)
            {
               for(int i=0; i < keyboardButtonsLowerCase.Count; i++)
                {
                    keyboardButtonsLowerCase[i].draw(b, randomList[i], 0);
                }
            }
            else
            {
                for (int i = 0; i < keyboardButtonsUpperCase.Count; i++)
                {
                    keyboardButtonsUpperCase[i].draw(b, randomList[i], 0);
                }
            }
            base.drawMouse(b);
        }
    }
}
