using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Minigames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CustomFarmTypes.Mod;

namespace CustomFarmTypes
{
    class NewCharacterCustomizeMenu : IClickableMenu
    {
        /////////////////////////////////////////////////////////
        private int scroll = 0;
        private Rectangle scrollRect;
        /////////////////////////////////////////////////////////

        public List<ClickableComponent> labels = new List<ClickableComponent>();
        public List<ClickableComponent> leftSelectionButtons = new List<ClickableComponent>();
        public List<ClickableComponent> rightSelectionButtons = new List<ClickableComponent>();
        public List<ClickableComponent> genderButtons = new List<ClickableComponent>();
        public List<ClickableComponent> petButtons = new List<ClickableComponent>();
        public List<ClickableTextureComponent> farmTypeButtons = new List<ClickableTextureComponent>();
        public List<ClickableComponent> colorPickerCCs = new List<ClickableComponent>();
        public const int region_okbutton = 505;
        public const int region_skipIntroButton = 506;
        public const int region_randomButton = 507;
        public const int region_male = 508;
        public const int region_female = 509;
        public const int region_dog = 510;
        public const int region_cat = 511;
        public const int region_shirtLeft = 512;
        public const int region_shirtRight = 513;
        public const int region_hairLeft = 514;
        public const int region_hairRight = 515;
        public const int region_accLeft = 516;
        public const int region_accRight = 517;
        public const int region_skinLeft = 518;
        public const int region_skinRight = 519;
        public const int region_directionLeft = 520;
        public const int region_directionRight = 521;
        public const int region_colorPicker1 = 522;
        public const int region_colorPicker2 = 523;
        public const int region_colorPicker3 = 524;
        public const int region_colorPicker4 = 525;
        public const int region_colorPicker5 = 526;
        public const int region_colorPicker6 = 527;
        public const int region_colorPicker7 = 528;
        public const int region_colorPicker8 = 529;
        public const int region_colorPicker9 = 530;
        public const int region_farmSelection1 = 531;
        public const int region_farmSelection2 = 532;
        public const int region_farmSelection3 = 533;
        public const int region_farmSelection4 = 534;
        public const int region_farmSelection5 = 535;
        public const int region_nameBox = 536;
        public const int region_farmNameBox = 537;
        public const int region_favThingBox = 538;
        public const int colorPickerTimerDelay = 100;
        private List<int> shirtOptions;
        private List<int> hairStyleOptions;
        private List<int> accessoryOptions;
        private int currentShirt;
        private int currentHair;
        private int currentAccessory;
        private int colorPickerTimer;
        public ColorPicker pantsColorPicker;
        public ColorPicker hairColorPicker;
        public ColorPicker eyeColorPicker;
        public ClickableTextureComponent okButton;
        public ClickableTextureComponent skipIntroButton;
        public ClickableTextureComponent randomButton;
        private TextBox nameBox;
        private TextBox farmnameBox;
        private TextBox favThingBox;
        private bool skipIntro;
        private bool wizardSource;
        private string hoverText;
        private string hoverTitle;
        public ClickableComponent nameBoxCC;
        public ClickableComponent farmnameBoxCC;
        public ClickableComponent favThingBoxCC;
        public ClickableComponent backButton;
        private ClickableComponent nameLabel;
        private ClickableComponent farmLabel;
        private ClickableComponent favoriteLabel;
        private ClickableComponent shirtLabel;
        private ClickableComponent skinLabel;
        private ClickableComponent hairLabel;
        private ClickableComponent accLabel;
        private ColorPicker lastHeldColorPicker;
        private int timesRandom;

        public NewCharacterCustomizeMenu(List<int> shirtOptions, List<int> hairStyleOptions, List<int> accessoryOptions, bool wizardSource = false)
        : base(Game1.viewport.Width / 2 - (632 + IClickableMenu.borderWidth * 2) / 2, Game1.viewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2 - Game1.tileSize, 632 + IClickableMenu.borderWidth * 2, 600 + IClickableMenu.borderWidth * 2 + Game1.tileSize, false)
        {
            this.shirtOptions = shirtOptions;
            this.hairStyleOptions = hairStyleOptions;
            this.accessoryOptions = accessoryOptions;
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
            this.petButtons.Clear();
            this.genderButtons.Clear();
            this.leftSelectionButtons.Clear();
            this.rightSelectionButtons.Clear();
            this.farmTypeButtons.Clear();
            ClickableTextureComponent textureComponent1 = new ClickableTextureComponent("OK", new Rectangle(this.xPositionOnScreen + this.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - Game1.tileSize, this.yPositionOnScreen + this.height - IClickableMenu.borderWidth - IClickableMenu.spaceToClearTopBorder + Game1.tileSize / 4, Game1.tileSize, Game1.tileSize), (string)null, (string)null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46, -1, -1), 1f, false);
            int num1 = 505;
            textureComponent1.myID = num1;
            int num2 = 530;
            textureComponent1.upNeighborID = num2;
            int num3 = 506;
            textureComponent1.leftNeighborID = num3;
            int num4 = 535;
            textureComponent1.rightNeighborID = num4;
            int num5 = this.wizardSource ? -1 : 81114;
            textureComponent1.downNeighborID = num5;
            this.okButton = textureComponent1;
            this.nameBox = new TextBox(Game1.content.Load<Texture2D>("LooseSprites\\textBox"), (Texture2D)null, Game1.smallFont, Game1.textColor)
            {
                X = this.xPositionOnScreen + Game1.tileSize + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 4,
                Y = this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 4,
                Text = Game1.player.name
            };
            this.backButton = new ClickableComponent(new Rectangle(Game1.viewport.Width - 198 - 48, Game1.viewport.Height - 81 - 24, 198, 81), "")
            {
                myID = 81114,
                leftNeighborID = 535
            };
            this.nameBoxCC = new ClickableComponent(new Rectangle(this.xPositionOnScreen + Game1.tileSize + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 4, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 4, 192, 48), "")
            {
                myID = 536,
                leftNeighborID = 507,
                downNeighborID = 537,
                rightNeighborID = 531
            };
            int num6 = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ru || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.es || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.pt ? -Game1.pixelZoom : 0;
            this.labels.Add(this.nameLabel = new ClickableComponent(new Rectangle(this.xPositionOnScreen + num6 + Game1.tileSize / 4 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 3 + 4, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 8, 1, 1), Game1.content.LoadString("Strings\\UI:Character_Name")));
            this.farmnameBox = new TextBox(Game1.content.Load<Texture2D>("LooseSprites\\textBox"), (Texture2D)null, Game1.smallFont, Game1.textColor)
            {
                X = this.xPositionOnScreen + Game1.tileSize + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 4,
                Y = this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 4 + Game1.tileSize,
                Text = Game1.player.farmName
            };
            this.farmnameBoxCC = new ClickableComponent(new Rectangle(this.xPositionOnScreen + Game1.tileSize + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 4, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 4 + Game1.tileSize, 192, 48), "")
            {
                myID = 537,
                leftNeighborID = 507,
                downNeighborID = 538,
                upNeighborID = 536,
                rightNeighborID = 531
            };
            this.labels.Add(this.farmLabel = new ClickableComponent(new Rectangle(this.xPositionOnScreen + num6 * 3 + Game1.tileSize / 4 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 3 + 4, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 4 + Game1.tileSize, 1, 1), Game1.content.LoadString("Strings\\UI:Character_Farm")));
            this.favThingBox = new TextBox(Game1.content.Load<Texture2D>("LooseSprites\\textBox"), (Texture2D)null, Game1.smallFont, Game1.textColor)
            {
                X = this.xPositionOnScreen + Game1.tileSize + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 4,
                Y = this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 4 + Game1.tileSize * 2,
                Text = Game1.player.favoriteThing
            };
            this.favThingBoxCC = new ClickableComponent(new Rectangle(this.xPositionOnScreen + Game1.tileSize + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 4, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 4 + Game1.tileSize * 2, 192, 48), "")
            {
                myID = 538,
                leftNeighborID = 521,
                downNeighborID = 511,
                upNeighborID = 537,
                rightNeighborID = 531
            };
            this.labels.Add(this.favoriteLabel = new ClickableComponent(new Rectangle(this.xPositionOnScreen + num6 + Game1.tileSize / 4 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 3 + 4, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 4 + Game1.tileSize * 2, 1, 1), Game1.content.LoadString("Strings\\UI:Character_FavoriteThing")));
            ClickableTextureComponent textureComponent2 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + Game1.pixelZoom * 12, this.yPositionOnScreen + Game1.tileSize + Game1.pixelZoom * 14, Game1.pixelZoom * 10, Game1.pixelZoom * 10), Game1.mouseCursors, new Rectangle(381, 361, 10, 10), (float)Game1.pixelZoom, false);
            int num7 = 507;
            textureComponent2.myID = num7;
            int num8 = 536;
            textureComponent2.rightNeighborID = num8;
            int num9 = 520;
            textureComponent2.downNeighborID = num9;
            this.randomButton = textureComponent2;
            int num10 = Game1.tileSize * 2;
            List<ClickableComponent> selectionButtons1 = this.leftSelectionButtons;
            ClickableTextureComponent textureComponent3 = new ClickableTextureComponent("Direction", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize / 4, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num10, Game1.tileSize, Game1.tileSize), (string)null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44, -1, -1), 1f, false);
            int num11 = 520;
            textureComponent3.myID = num11;
            int num12 = 521;
            textureComponent3.rightNeighborID = num12;
            int num13 = 507;
            textureComponent3.upNeighborID = num13;
            int num14 = 508;
            textureComponent3.downNeighborID = num14;
            selectionButtons1.Add((ClickableComponent)textureComponent3);
            List<ClickableComponent> selectionButtons2 = this.rightSelectionButtons;
            ClickableTextureComponent textureComponent4 = new ClickableTextureComponent("Direction", new Rectangle(this.xPositionOnScreen + Game1.tileSize / 4 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 2, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num10, Game1.tileSize, Game1.tileSize), (string)null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33, -1, -1), 1f, false);
            int num15 = 521;
            textureComponent4.myID = num15;
            int num16 = 520;
            textureComponent4.leftNeighborID = num16;
            int num17 = 509;
            textureComponent4.downNeighborID = num17;
            int num18 = 507;
            textureComponent4.upNeighborID = num18;
            int num19 = 538;
            textureComponent4.rightNeighborID = num19;
            selectionButtons2.Add((ClickableComponent)textureComponent4);
            if (!this.wizardSource)
                this.labels.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + Game1.tileSize / 4 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 3 + 8 + num6, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 8 + Game1.tileSize * 3, 1, 1), Game1.content.LoadString("Strings\\UI:Character_Animal")));
            int num20 = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ru ? Game1.pixelZoom * 15 : 0;
            List<ClickableComponent> petButtons1 = this.petButtons;
            ClickableTextureComponent textureComponent5 = new ClickableTextureComponent("Cat", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 6 - Game1.tileSize / 4 + num20, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 3 - Game1.tileSize / 4, Game1.tileSize, Game1.tileSize), (string)null, "Cat", Game1.mouseCursors, new Rectangle(160, 192, 16, 16), (float)Game1.pixelZoom, false);
            int num21 = 511;
            textureComponent5.myID = num21;
            int num22 = 510;
            textureComponent5.rightNeighborID = num22;
            int num23 = 509;
            textureComponent5.leftNeighborID = num23;
            int num24 = 522;
            textureComponent5.downNeighborID = num24;
            int num25 = 538;
            textureComponent5.upNeighborID = num25;
            petButtons1.Add((ClickableComponent)textureComponent5);
            List<ClickableComponent> petButtons2 = this.petButtons;
            ClickableTextureComponent textureComponent6 = new ClickableTextureComponent("Dog", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 7 - Game1.tileSize / 4 + num20, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 3 - Game1.tileSize / 4, Game1.tileSize, Game1.tileSize), (string)null, "Dog", Game1.mouseCursors, new Rectangle(176, 192, 16, 16), (float)Game1.pixelZoom, false);
            int num26 = 510;
            textureComponent6.myID = num26;
            int num27 = 511;
            textureComponent6.leftNeighborID = num27;
            int num28 = 522;
            textureComponent6.downNeighborID = num28;
            int num29 = 532;
            textureComponent6.rightNeighborID = num29;
            int num30 = 538;
            textureComponent6.upNeighborID = num30;
            petButtons2.Add((ClickableComponent)textureComponent6);
            List<ClickableComponent> genderButtons1 = this.genderButtons;
            ClickableTextureComponent textureComponent7 = new ClickableTextureComponent("Male", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize / 2 + 8, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 3, Game1.tileSize, Game1.tileSize), (string)null, "Male", Game1.mouseCursors, new Rectangle(128, 192, 16, 16), (float)Game1.pixelZoom, false);
            int num31 = 508;
            textureComponent7.myID = num31;
            int num32 = 509;
            textureComponent7.rightNeighborID = num32;
            int num33 = 518;
            textureComponent7.downNeighborID = num33;
            int num34 = 520;
            textureComponent7.upNeighborID = num34;
            genderButtons1.Add((ClickableComponent)textureComponent7);
            List<ClickableComponent> genderButtons2 = this.genderButtons;
            ClickableTextureComponent textureComponent8 = new ClickableTextureComponent("Female", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize / 2 + Game1.tileSize + 8, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 3, Game1.tileSize, Game1.tileSize), (string)null, "Female", Game1.mouseCursors, new Rectangle(144, 192, 16, 16), (float)Game1.pixelZoom, false);
            int num35 = 509;
            textureComponent8.myID = num35;
            int num36 = 508;
            textureComponent8.leftNeighborID = num36;
            int num37 = 519;
            textureComponent8.downNeighborID = num37;
            int num38 = 511;
            textureComponent8.rightNeighborID = num38;
            int num39 = 521;
            textureComponent8.upNeighborID = num39;
            genderButtons2.Add((ClickableComponent)textureComponent8);
            int num40 = Game1.tileSize * 4 + 8;
            int num41 = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ru || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.es || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.pt ? -Game1.pixelZoom * 5 : 0;
            List<ClickableComponent> selectionButtons3 = this.leftSelectionButtons;
            ClickableTextureComponent textureComponent9 = new ClickableTextureComponent("Skin", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize / 4 + num41, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num40, Game1.tileSize, Game1.tileSize), (string)null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44, -1, -1), 1f, false);
            int num42 = 518;
            textureComponent9.myID = num42;
            int num43 = 519;
            textureComponent9.rightNeighborID = num43;
            int num44 = 514;
            textureComponent9.downNeighborID = num44;
            int num45 = 508;
            textureComponent9.upNeighborID = num45;
            selectionButtons3.Add((ClickableComponent)textureComponent9);
            this.labels.Add(this.skinLabel = new ClickableComponent(new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize / 4 + Game1.tileSize + 8 + num41 / 2, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num40 + 16, 1, 1), Game1.content.LoadString("Strings\\UI:Character_Skin")));
            List<ClickableComponent> selectionButtons4 = this.rightSelectionButtons;
            ClickableTextureComponent textureComponent10 = new ClickableTextureComponent("Skin", new Rectangle(this.xPositionOnScreen + Game1.tileSize / 4 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 2, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num40, Game1.tileSize, Game1.tileSize), (string)null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33, -1, -1), 1f, false);
            int num46 = 519;
            textureComponent10.myID = num46;
            int num47 = 518;
            textureComponent10.leftNeighborID = num47;
            int num48 = 515;
            textureComponent10.downNeighborID = num48;
            int num49 = 509;
            textureComponent10.upNeighborID = num49;
            int num50 = 522;
            textureComponent10.rightNeighborID = num50;
            selectionButtons4.Add((ClickableComponent)textureComponent10);
            if (!this.wizardSource)
            {
                Point point = new Point(this.xPositionOnScreen + this.width + Game1.pixelZoom + Game1.tileSize / 8, this.yPositionOnScreen + IClickableMenu.borderWidth * 2);
                List<ClickableTextureComponent> farmTypeButtons1 = this.farmTypeButtons;
                ClickableTextureComponent textureComponent11 = new ClickableTextureComponent("Standard", new Rectangle(point.X, point.Y + 22 * Game1.pixelZoom, 22 * Game1.pixelZoom, 20 * Game1.pixelZoom), (string)null, Game1.content.LoadString("Strings\\UI:Character_FarmStandard"), Game1.mouseCursors, new Rectangle(0, 324, 22, 20), (float)Game1.pixelZoom, false);
                int num51 = 531;
                textureComponent11.myID = num51;
                int num52 = 532;
                textureComponent11.downNeighborID = num52;
                int num53 = 537;
                textureComponent11.leftNeighborID = num53;
                farmTypeButtons1.Add(textureComponent11);
                List<ClickableTextureComponent> farmTypeButtons2 = this.farmTypeButtons;
                ClickableTextureComponent textureComponent12 = new ClickableTextureComponent("Riverland", new Rectangle(point.X, point.Y + 22 * Game1.pixelZoom * 2, 22 * Game1.pixelZoom, 20 * Game1.pixelZoom), (string)null, Game1.content.LoadString("Strings\\UI:Character_FarmFishing"), Game1.mouseCursors, new Rectangle(22, 324, 22, 20), (float)Game1.pixelZoom, false);
                int num54 = 532;
                textureComponent12.myID = num54;
                int num55 = 533;
                textureComponent12.downNeighborID = num55;
                int num56 = 531;
                textureComponent12.upNeighborID = num56;
                int num57 = 510;
                textureComponent12.leftNeighborID = num57;
                int num58 = 81114;
                textureComponent12.rightNeighborID = num58;
                farmTypeButtons2.Add(textureComponent12);
                List<ClickableTextureComponent> farmTypeButtons3 = this.farmTypeButtons;
                ClickableTextureComponent textureComponent13 = new ClickableTextureComponent("Forest", new Rectangle(point.X, point.Y + 22 * Game1.pixelZoom * 3, 22 * Game1.pixelZoom, 20 * Game1.pixelZoom), (string)null, Game1.content.LoadString("Strings\\UI:Character_FarmForaging"), Game1.mouseCursors, new Rectangle(44, 324, 22, 20), (float)Game1.pixelZoom, false);
                int num59 = 533;
                textureComponent13.myID = num59;
                int num60 = 534;
                textureComponent13.downNeighborID = num60;
                int num61 = 532;
                textureComponent13.upNeighborID = num61;
                int num62 = 522;
                textureComponent13.leftNeighborID = num62;
                int num63 = 81114;
                textureComponent13.rightNeighborID = num63;
                farmTypeButtons3.Add(textureComponent13);
                List<ClickableTextureComponent> farmTypeButtons4 = this.farmTypeButtons;
                ClickableTextureComponent textureComponent14 = new ClickableTextureComponent("Hills", new Rectangle(point.X, point.Y + 22 * Game1.pixelZoom * 4, 22 * Game1.pixelZoom, 20 * Game1.pixelZoom), (string)null, Game1.content.LoadString("Strings\\UI:Character_FarmMining"), Game1.mouseCursors, new Rectangle(66, 324, 22, 20), (float)Game1.pixelZoom, false);
                int num64 = 534;
                textureComponent14.myID = num64;
                int num65 = 535;
                textureComponent14.downNeighborID = num65;
                int num66 = 533;
                textureComponent14.upNeighborID = num66;
                int num67 = 525;
                textureComponent14.leftNeighborID = num67;
                int num68 = 81114;
                textureComponent14.rightNeighborID = num68;
                farmTypeButtons4.Add(textureComponent14);
                List<ClickableTextureComponent> farmTypeButtons5 = this.farmTypeButtons;
                ClickableTextureComponent textureComponent15 = new ClickableTextureComponent("Wilderness", new Rectangle(point.X, point.Y + 22 * Game1.pixelZoom * 5, 22 * Game1.pixelZoom, 20 * Game1.pixelZoom), (string)null, Game1.content.LoadString("Strings\\UI:Character_FarmCombat"), Game1.mouseCursors, new Rectangle(88, 324, 22, 20), (float)Game1.pixelZoom, false);
                int num69 = 535;
                textureComponent15.myID = num69;
                int num70 = 534;
                textureComponent15.upNeighborID = num70;
                int num71 = 528;
                textureComponent15.leftNeighborID = num71;
                int num72 = 505;
                textureComponent15.downNeighborID = num72;
                int num73 = 81114;
                textureComponent15.rightNeighborID = num73;
                farmTypeButtons5.Add(textureComponent15);
                /////////////////////////////////////////////////////////
                int prevId = num69;
                int myId = 100000;
                int i = 6;
                foreach (var type in FarmType.getTypes())
                {
                    Texture2D icon = type.Icon;
                    ClickableTextureComponent texComp = new ClickableTextureComponent("Custom_" + type.ID, new Rectangle(point.X, point.Y + 22 * Game1.pixelZoom * i, 22 * Game1.pixelZoom, 20 * Game1.pixelZoom), (string)null, type.Name, icon, new Rectangle(0, 0, icon.Width, icon.Height), (float)Game1.pixelZoom, false);
                    texComp.myID = myId;
                    texComp.upNeighborID = prevId;
                    texComp.leftNeighborID = region_colorPicker7;
                    texComp.downNeighborID = region_okbutton;
                    texComp.rightNeighborID = 81114;
                    farmTypeButtons.Add(texComp);

                    prevId = myId++;
                    i++;
                }
                /////////////////////////////////////////////////////////

            }
            this.labels.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + Game1.tileSize / 4 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 3 + 8, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num40 + 16, 1, 1), Game1.content.LoadString("Strings\\UI:Character_EyeColor")));
            Point point1 = new Point(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + Game1.tileSize * 5 + Game1.tileSize * 3 / 4 + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num40);
            this.eyeColorPicker = new ColorPicker(point1.X, point1.Y);
            this.eyeColorPicker.setColor(Game1.player.newEyeColor);
            this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(point1.X, point1.Y, Game1.tileSize * 2, 20), "")
            {
                myID = 522,
                downNeighborID = 523,
                upNeighborID = 511,
                leftNeighborImmutable = true,
                rightNeighborImmutable = true
            });
            this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(point1.X, point1.Y + 20, Game1.tileSize * 2, 20), "")
            {
                myID = 523,
                upNeighborID = 522,
                downNeighborID = 524,
                leftNeighborImmutable = true,
                rightNeighborImmutable = true
            });
            this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(point1.X, point1.Y + 40, Game1.tileSize * 2, 20), "")
            {
                myID = 524,
                upNeighborID = 523,
                downNeighborID = 525,
                leftNeighborImmutable = true,
                rightNeighborImmutable = true
            });
            int num74 = num40 + (Game1.tileSize + 8);
            List<ClickableComponent> selectionButtons5 = this.leftSelectionButtons;
            ClickableTextureComponent textureComponent16 = new ClickableTextureComponent("Hair", new Rectangle(this.xPositionOnScreen + Game1.tileSize / 4 + IClickableMenu.borderWidth + IClickableMenu.spaceToClearSideBorder + num41, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num74, Game1.tileSize, Game1.tileSize), (string)null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44, -1, -1), 1f, false);
            int num75 = 514;
            textureComponent16.myID = num75;
            int num76 = 515;
            textureComponent16.rightNeighborID = num76;
            int num77 = 512;
            textureComponent16.downNeighborID = num77;
            int num78 = 518;
            textureComponent16.upNeighborID = num78;
            selectionButtons5.Add((ClickableComponent)textureComponent16);
            this.labels.Add(this.hairLabel = new ClickableComponent(new Rectangle(this.xPositionOnScreen + Game1.tileSize / 4 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize + 8 + num41 / 2, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num74 + 16, 1, 1), Game1.content.LoadString("Strings\\UI:Character_Hair")));
            List<ClickableComponent> selectionButtons6 = this.rightSelectionButtons;
            ClickableTextureComponent textureComponent17 = new ClickableTextureComponent("Hair", new Rectangle(this.xPositionOnScreen + Game1.tileSize / 4 + IClickableMenu.spaceToClearSideBorder + Game1.tileSize * 2 + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num74, Game1.tileSize, Game1.tileSize), (string)null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33, -1, -1), 1f, false);
            int num79 = 515;
            textureComponent17.myID = num79;
            int num80 = 514;
            textureComponent17.leftNeighborID = num80;
            int num81 = 513;
            textureComponent17.downNeighborID = num81;
            int num82 = 519;
            textureComponent17.upNeighborID = num82;
            int num83 = 525;
            textureComponent17.rightNeighborID = num83;
            selectionButtons6.Add((ClickableComponent)textureComponent17);
            this.labels.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + Game1.tileSize / 4 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 3 + 8, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num74 + 16, 1, 1), Game1.content.LoadString("Strings\\UI:Character_HairColor")));
            point1 = new Point(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + Game1.tileSize * 5 + Game1.tileSize * 3 / 4 + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num74);
            this.hairColorPicker = new ColorPicker(point1.X, point1.Y);
            this.hairColorPicker.setColor(Game1.player.hairstyleColor);
            this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(point1.X, point1.Y, Game1.tileSize * 2, 20), "")
            {
                myID = 525,
                downNeighborID = 526,
                upNeighborID = 524,
                leftNeighborImmutable = true,
                rightNeighborImmutable = true
            });
            this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(point1.X, point1.Y + 20, Game1.tileSize * 2, 20), "")
            {
                myID = 526,
                upNeighborID = 525,
                downNeighborID = 527,
                leftNeighborImmutable = true,
                rightNeighborImmutable = true
            });
            this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(point1.X, point1.Y + 40, Game1.tileSize * 2, 20), "")
            {
                myID = 527,
                upNeighborID = 526,
                downNeighborID = 528,
                leftNeighborImmutable = true,
                rightNeighborImmutable = true
            });
            int num84 = num74 + (Game1.tileSize + 8);
            List<ClickableComponent> selectionButtons7 = this.leftSelectionButtons;
            ClickableTextureComponent textureComponent18 = new ClickableTextureComponent("Shirt", new Rectangle(this.xPositionOnScreen + Game1.tileSize / 4 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + num41, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num84, Game1.tileSize, Game1.tileSize), (string)null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44, -1, -1), 1f, false);
            int num85 = 512;
            textureComponent18.myID = num85;
            int num86 = 513;
            textureComponent18.rightNeighborID = num86;
            int num87 = 516;
            textureComponent18.downNeighborID = num87;
            int num88 = 514;
            textureComponent18.upNeighborID = num88;
            selectionButtons7.Add((ClickableComponent)textureComponent18);
            this.labels.Add(this.shirtLabel = new ClickableComponent(new Rectangle(this.xPositionOnScreen + Game1.tileSize / 4 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize + 8 + num41 / 2, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num84 + 16, 1, 1), Game1.content.LoadString("Strings\\UI:Character_Shirt")));
            List<ClickableComponent> selectionButtons8 = this.rightSelectionButtons;
            ClickableTextureComponent textureComponent19 = new ClickableTextureComponent("Shirt", new Rectangle(this.xPositionOnScreen + Game1.tileSize / 4 + IClickableMenu.spaceToClearSideBorder + Game1.tileSize * 2 + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num84, Game1.tileSize, Game1.tileSize), (string)null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33, -1, -1), 1f, false);
            int num89 = 513;
            textureComponent19.myID = num89;
            int num90 = 512;
            textureComponent19.leftNeighborID = num90;
            int num91 = 517;
            textureComponent19.downNeighborID = num91;
            int num92 = 515;
            textureComponent19.upNeighborID = num92;
            int num93 = 528;
            textureComponent19.rightNeighborID = num93;
            selectionButtons8.Add((ClickableComponent)textureComponent19);
            this.labels.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + Game1.tileSize / 4 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 3 + 8, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num84 + 16, 1, 1), Game1.content.LoadString("Strings\\UI:Character_PantsColor")));
            point1 = new Point(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + Game1.tileSize * 5 + Game1.tileSize * 3 / 4 + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num84);
            this.pantsColorPicker = new ColorPicker(point1.X, point1.Y);
            this.pantsColorPicker.setColor(Game1.player.pantsColor);
            this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(point1.X, point1.Y, Game1.tileSize * 2, 20), "")
            {
                myID = 528,
                downNeighborID = 529,
                upNeighborID = 527,
                leftNeighborImmutable = true,
                rightNeighborImmutable = true
            });
            this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(point1.X, point1.Y + 20, Game1.tileSize * 2, 20), "")
            {
                myID = 529,
                upNeighborID = 528,
                downNeighborID = 530,
                leftNeighborImmutable = true,
                rightNeighborImmutable = true
            });
            this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(point1.X, point1.Y + 40, Game1.tileSize * 2, 20), "")
            {
                myID = 530,
                upNeighborID = 529,
                downNeighborID = 506,
                leftNeighborImmutable = true,
                rightNeighborImmutable = true
            });
            ClickableTextureComponent textureComponent20 = new ClickableTextureComponent("Skip Intro", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + Game1.tileSize * 5 - Game1.tileSize * 3 / 4 + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num84 + Game1.tileSize * 5 / 4, Game1.pixelZoom * 9, Game1.pixelZoom * 9), (string)null, Game1.content.LoadString("Strings\\UI:Character_SkipIntro"), Game1.mouseCursors, new Rectangle(227, 425, 9, 9), (float)Game1.pixelZoom, false);
            int num94 = 506;
            textureComponent20.myID = num94;
            int num95 = 530;
            textureComponent20.upNeighborID = num95;
            int num96 = 517;
            textureComponent20.leftNeighborID = num96;
            int num97 = 505;
            textureComponent20.rightNeighborID = num97;
            this.skipIntroButton = textureComponent20;
            int num98 = num84 + (Game1.tileSize + 8);
            List<ClickableComponent> selectionButtons9 = this.leftSelectionButtons;
            ClickableTextureComponent textureComponent21 = new ClickableTextureComponent("Acc", new Rectangle(this.xPositionOnScreen + Game1.tileSize / 4 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + num41, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num98, Game1.tileSize, Game1.tileSize), (string)null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44, -1, -1), 1f, false);
            int num99 = 516;
            textureComponent21.myID = num99;
            int num100 = 517;
            textureComponent21.rightNeighborID = num100;
            int num101 = 512;
            textureComponent21.upNeighborID = num101;
            selectionButtons9.Add((ClickableComponent)textureComponent21);
            this.labels.Add(this.accLabel = new ClickableComponent(new Rectangle(this.xPositionOnScreen + Game1.tileSize / 4 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize + 8 + num41 / 2, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num98 + 16, 1, 1), Game1.content.LoadString("Strings\\UI:Character_Accessory")));
            List<ClickableComponent> selectionButtons10 = this.rightSelectionButtons;
            ClickableTextureComponent textureComponent22 = new ClickableTextureComponent("Acc", new Rectangle(this.xPositionOnScreen + Game1.tileSize / 4 + IClickableMenu.spaceToClearSideBorder + Game1.tileSize * 2 + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num98, Game1.tileSize, Game1.tileSize), (string)null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33, -1, -1), 1f, false);
            int num102 = 517;
            textureComponent22.myID = num102;
            int num103 = 516;
            textureComponent22.leftNeighborID = num103;
            int num104 = 513;
            textureComponent22.upNeighborID = num104;
            int num105 = 528;
            textureComponent22.rightNeighborID = num105;
            selectionButtons10.Add((ClickableComponent)textureComponent22);
            if (!Game1.options.snappyMenus || !Game1.options.gamepadControls)
                return;
            this.populateClickableComponentList();
            this.snapToDefaultClickableComponent();
        }

        public override void snapToDefaultClickableComponent()
        {
            this.currentlySnappedComponent = this.getComponentWithID(521);
            this.snapCursorToCurrentSnappedComponent();
        }

        public override void gamePadButtonHeld(Buttons b)
        {
            base.gamePadButtonHeld(b);
            if (this.currentlySnappedComponent == null)
                return;
            if (b == Buttons.LeftThumbstickRight || b == Buttons.DPadRight)
            {
                switch (this.currentlySnappedComponent.myID)
                {
                    case 522:
                        this.eyeColorPicker.changeHue(1);
                        Game1.player.changeEyeColor(this.eyeColorPicker.getSelectedColor());
                        this.lastHeldColorPicker = this.eyeColorPicker;
                        break;
                    case 523:
                        this.eyeColorPicker.changeSaturation(1);
                        Game1.player.changeEyeColor(this.eyeColorPicker.getSelectedColor());
                        this.lastHeldColorPicker = this.eyeColorPicker;
                        break;
                    case 524:
                        this.eyeColorPicker.changeValue(1);
                        Game1.player.changeEyeColor(this.eyeColorPicker.getSelectedColor());
                        this.lastHeldColorPicker = this.eyeColorPicker;
                        break;
                    case 525:
                        this.hairColorPicker.changeHue(1);
                        Game1.player.changeHairColor(this.hairColorPicker.getSelectedColor());
                        this.lastHeldColorPicker = this.hairColorPicker;
                        break;
                    case 526:
                        this.hairColorPicker.changeSaturation(1);
                        Game1.player.changeHairColor(this.hairColorPicker.getSelectedColor());
                        this.lastHeldColorPicker = this.hairColorPicker;
                        break;
                    case 527:
                        this.hairColorPicker.changeValue(1);
                        Game1.player.changeHairColor(this.hairColorPicker.getSelectedColor());
                        this.lastHeldColorPicker = this.hairColorPicker;
                        break;
                    case 528:
                        this.pantsColorPicker.changeHue(1);
                        Game1.player.changePants(this.pantsColorPicker.getSelectedColor());
                        this.lastHeldColorPicker = this.pantsColorPicker;
                        break;
                    case 529:
                        this.pantsColorPicker.changeSaturation(1);
                        Game1.player.changePants(this.pantsColorPicker.getSelectedColor());
                        this.lastHeldColorPicker = this.pantsColorPicker;
                        break;
                    case 530:
                        this.pantsColorPicker.changeValue(1);
                        Game1.player.changePants(this.pantsColorPicker.getSelectedColor());
                        this.lastHeldColorPicker = this.pantsColorPicker;
                        break;
                }
            }
            else
            {
                if (b != Buttons.LeftThumbstickLeft && b != Buttons.DPadLeft)
                    return;
                switch (this.currentlySnappedComponent.myID)
                {
                    case 522:
                        this.eyeColorPicker.changeHue(-1);
                        Game1.player.changeEyeColor(this.eyeColorPicker.getSelectedColor());
                        this.lastHeldColorPicker = this.eyeColorPicker;
                        break;
                    case 523:
                        this.eyeColorPicker.changeSaturation(-1);
                        Game1.player.changeEyeColor(this.eyeColorPicker.getSelectedColor());
                        this.lastHeldColorPicker = this.eyeColorPicker;
                        break;
                    case 524:
                        this.eyeColorPicker.changeValue(-1);
                        Game1.player.changeEyeColor(this.eyeColorPicker.getSelectedColor());
                        this.lastHeldColorPicker = this.eyeColorPicker;
                        break;
                    case 525:
                        this.hairColorPicker.changeHue(-1);
                        Game1.player.changeHairColor(this.hairColorPicker.getSelectedColor());
                        this.lastHeldColorPicker = this.hairColorPicker;
                        break;
                    case 526:
                        this.hairColorPicker.changeSaturation(-1);
                        Game1.player.changeHairColor(this.hairColorPicker.getSelectedColor());
                        this.lastHeldColorPicker = this.hairColorPicker;
                        break;
                    case 527:
                        this.hairColorPicker.changeValue(-1);
                        Game1.player.changeHairColor(this.hairColorPicker.getSelectedColor());
                        this.lastHeldColorPicker = this.hairColorPicker;
                        break;
                    case 528:
                        this.pantsColorPicker.changeHue(-1);
                        Game1.player.changePants(this.pantsColorPicker.getSelectedColor());
                        this.lastHeldColorPicker = this.pantsColorPicker;
                        break;
                    case 529:
                        this.pantsColorPicker.changeSaturation(-1);
                        Game1.player.changePants(this.pantsColorPicker.getSelectedColor());
                        this.lastHeldColorPicker = this.pantsColorPicker;
                        break;
                    case 530:
                        this.pantsColorPicker.changeValue(-1);
                        Game1.player.changePants(this.pantsColorPicker.getSelectedColor());
                        this.lastHeldColorPicker = this.pantsColorPicker;
                        break;
                }
            }
        }

        public override void receiveGamePadButton(Buttons b)
        {
            base.receiveGamePadButton(b);
            if (this.currentlySnappedComponent == null)
                return;
            if (b == Buttons.RightTrigger)
            {
                switch (this.currentlySnappedComponent.myID)
                {
                    case 512:
                    case 513:
                    case 514:
                    case 515:
                    case 516:
                    case 517:
                    case 518:
                    case 519:
                    case 520:
                    case 521:
                        this.selectionClick(this.currentlySnappedComponent.name, 1);
                        break;
                }
            }
            else
            {
                if (b != Buttons.LeftTrigger)
                    return;
                switch (this.currentlySnappedComponent.myID)
                {
                    case 512:
                    case 513:
                    case 514:
                    case 515:
                    case 516:
                    case 517:
                    case 518:
                    case 519:
                    case 520:
                    case 521:
                        this.selectionClick(this.currentlySnappedComponent.name, -1);
                        break;
                }
            }
        }

        private void optionButtonClick(string name)
        {
            /////////////////////////////////////////////////////////
            if (name.StartsWith("Custom_"))
            {
                FarmType type = FarmType.getType(name.Substring(7));
                if (type == null)
                {
                    Log.warn("Clicked on a nonexistant option!?!?!?");
                    return;
                }

                Game1.whichFarm = 0;
                Game1.spawnMonstersAtNight = type.Behavior.SpawnMonsters;
                Data.FarmTypes["Farm"] = type.ID;
                return;
            }
            /////////////////////////////////////////////////////////
            switch (name)
            {
                case "Wilderness":
                    if (!this.wizardSource)
                    {
                        Game1.whichFarm = 4;
                        Game1.spawnMonstersAtNight = true;
                        /////////////////////////////////////////////////////////
                        if ( Data.FarmTypes.ContainsKey("Farm"))
                            Data.FarmTypes.Remove("Farm");
                        /////////////////////////////////////////////////////////
                    }
                    break;

                case "Standard":
                    if (!this.wizardSource)
                    {
                        Game1.whichFarm = 0;
                        Game1.spawnMonstersAtNight = false;
                        /////////////////////////////////////////////////////////
                        if (Data.FarmTypes.ContainsKey("Farm"))
                            Data.FarmTypes.Remove("Farm");
                        /////////////////////////////////////////////////////////
                    }
                    break;

                case "Riverland":
                    if (!this.wizardSource)
                    {
                        Game1.whichFarm = 1;
                        Game1.spawnMonstersAtNight = false;
                        /////////////////////////////////////////////////////////
                        if (Data.FarmTypes.ContainsKey("Farm"))
                            Data.FarmTypes.Remove("Farm");
                        /////////////////////////////////////////////////////////
                    }
                    break;

                case "Dog":
                    if (!this.wizardSource)
                        Game1.player.catPerson = false;
                    break;

                case "Male":
                    if (!this.wizardSource)
                    {
                        Game1.player.changeGender(true);
                        Game1.player.changeHairStyle(0);
                    }
                    break;

                case "OK":
                    if (!this.canLeaveMenu())
                        return;
                    Game1.player.Name = this.nameBox.Text.Trim();
                    Game1.player.displayName = Game1.player.Name;
                    Game1.player.favoriteThing = this.favThingBox.Text.Trim();
                    if (Game1.activeClickableMenu is TitleMenu)
                    {
                        (Game1.activeClickableMenu as TitleMenu).createdNewCharacter(this.skipIntro);
                    }
                    else
                    {
                        Game1.exitActiveMenu();
                        if (Game1.currentMinigame != null && Game1.currentMinigame is Intro)
                            (Game1.currentMinigame as Intro).doneCreatingCharacter();
                        else if (this.wizardSource)
                        {
                            Game1.flashAlpha = 1f;
                            Game1.playSound("yoba");
                        }
                    }
                    break;

                case "Cat":
                    if (!this.wizardSource)
                        Game1.player.catPerson = true;
                    break;

                case "Female":
                    if (!this.wizardSource)
                    {
                        Game1.player.changeGender(false);
                        Game1.player.changeHairStyle(16);
                    }
                    break;

                case "Hills":
                    if (!this.wizardSource)
                    {
                        Game1.whichFarm = 3;
                        Game1.spawnMonstersAtNight = false;
                        /////////////////////////////////////////////////////////
                        if (Data.FarmTypes.ContainsKey("Farm"))
                            Data.FarmTypes.Remove("Farm");
                        /////////////////////////////////////////////////////////
                    }
                    break;

                case "Forest":
                    if (!this.wizardSource)
                    {
                        Game1.whichFarm = 2;
                        Game1.spawnMonstersAtNight = false;
                        /////////////////////////////////////////////////////////
                        if (Data.FarmTypes.ContainsKey("Farm"))
                            Data.FarmTypes.Remove("Farm");
                        /////////////////////////////////////////////////////////
                    }
                    break;
            }
            Game1.playSound("coin");
        }

        private void selectionClick(string name, int change)
        {
            if (!(name == "Skin"))
            {
                if (!(name == "Hair"))
                {
                    if (!(name == "Shirt"))
                    {
                        if (!(name == "Acc"))
                        {
                            if (!(name == "Direction"))
                                return;
                            Game1.player.faceDirection((Game1.player.facingDirection - change + 4) % 4);
                            Game1.player.FarmerSprite.StopAnimation();
                            Game1.player.completelyStopAnimatingOrDoingAction();
                            Game1.playSound("pickUpItem");
                        }
                        else
                        {
                            Game1.player.changeAccessory(Game1.player.accessory + change);
                            Game1.playSound("purchase");
                        }
                    }
                    else
                    {
                        Game1.player.changeShirt(Game1.player.shirt + change);
                        Game1.playSound("coin");
                    }
                }
                else
                {
                    Game1.player.changeHairStyle(Game1.player.hair + change);
                    Game1.playSound("grassyStep");
                }
            }
            else
            {
                Game1.player.changeSkinColor(Game1.player.skin + change);
                Game1.playSound("skeletonStep");
            }
        }

        /////////////////////////////////////////////////////////
        public override void receiveScrollWheelAction(int dir)
        {
            scroll += dir;
            if (scroll > 0) scroll = 0;

            int cap = (farmTypeButtons[0].bounds.Height + Game1.pixelZoom * 3) * farmTypeButtons.Count - scrollRect.Height;
            if (scroll <= -cap) scroll = -cap;
        }
        /////////////////////////////////////////////////////////

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            foreach (ClickableComponent genderButton in this.genderButtons)
            {
                if (genderButton.containsPoint(x, y))
                {
                    this.optionButtonClick(genderButton.name);
                    genderButton.scale -= 0.5f;
                    genderButton.scale = Math.Max(3.5f, genderButton.scale);
                }
            }
            foreach (ClickableComponent farmTypeButton in this.farmTypeButtons)
            {
                //if (farmTypeButton.containsPoint(x, y) && !farmTypeButton.name.Contains("Gray"))
                /////////////////////////////////////////////////////////
                if (farmTypeButton.containsPoint(x, y - scroll) && !farmTypeButton.name.Contains("Gray"))
                /////////////////////////////////////////////////////////
                {
                    this.optionButtonClick(farmTypeButton.name);
                    farmTypeButton.scale -= 0.5f;
                    farmTypeButton.scale = Math.Max(3.5f, farmTypeButton.scale);
                }
            }
            foreach (ClickableComponent petButton in this.petButtons)
            {
                if (petButton.containsPoint(x, y))
                {
                    this.optionButtonClick(petButton.name);
                    petButton.scale -= 0.5f;
                    petButton.scale = Math.Max(3.5f, petButton.scale);
                }
            }
            foreach (ClickableComponent leftSelectionButton in this.leftSelectionButtons)
            {
                if (leftSelectionButton.containsPoint(x, y))
                {
                    this.selectionClick(leftSelectionButton.name, -1);
                    leftSelectionButton.scale -= 0.25f;
                    leftSelectionButton.scale = Math.Max(0.75f, leftSelectionButton.scale);
                }
            }
            foreach (ClickableComponent rightSelectionButton in this.rightSelectionButtons)
            {
                if (rightSelectionButton.containsPoint(x, y))
                {
                    this.selectionClick(rightSelectionButton.name, 1);
                    rightSelectionButton.scale -= 0.25f;
                    rightSelectionButton.scale = Math.Max(0.75f, rightSelectionButton.scale);
                }
            }
            if (this.okButton.containsPoint(x, y) && this.canLeaveMenu())
            {
                this.optionButtonClick(this.okButton.name);
                this.okButton.scale -= 0.25f;
                this.okButton.scale = Math.Max(0.75f, this.okButton.scale);
            }
            if (this.hairColorPicker.containsPoint(x, y))
            {
                Game1.player.changeHairColor(this.hairColorPicker.click(x, y));
                this.lastHeldColorPicker = this.hairColorPicker;
            }
            else if (this.pantsColorPicker.containsPoint(x, y))
            {
                Game1.player.changePants(this.pantsColorPicker.click(x, y));
                this.lastHeldColorPicker = this.pantsColorPicker;
            }
            else if (this.eyeColorPicker.containsPoint(x, y))
            {
                Game1.player.changeEyeColor(this.eyeColorPicker.click(x, y));
                this.lastHeldColorPicker = this.eyeColorPicker;
            }
            if (!this.wizardSource)
            {
                this.nameBox.Update();
                this.farmnameBox.Update();
                this.favThingBox.Update();
                if (this.skipIntroButton.containsPoint(x, y))
                {
                    Game1.playSound("drumkit6");
                    this.skipIntroButton.sourceRect.X = this.skipIntroButton.sourceRect.X == 227 ? 236 : 227;
                    this.skipIntro = !this.skipIntro;
                }
            }
            if (!this.randomButton.containsPoint(x, y))
                return;
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
            this.timesRandom = this.timesRandom + 1;
            if (Game1.random.NextDouble() < 0.33)
            {
                if (Game1.player.isMale)
                    Game1.player.changeAccessory(Game1.random.Next(19));
                else
                    Game1.player.changeAccessory(Game1.random.Next(6, 19));
            }
            else
                Game1.player.changeAccessory(-1);
            if (Game1.player.isMale)
                Game1.player.changeHairStyle(Game1.random.Next(16));
            else
                Game1.player.changeHairStyle(Game1.random.Next(16, 32));
            Color c1 = new Color(Game1.random.Next(25, 254), Game1.random.Next(25, 254), Game1.random.Next(25, 254));
            if (Game1.random.NextDouble() < 0.5)
            {
                c1.R = (byte)((uint)c1.R / 2U);
                c1.G = (byte)((uint)c1.G / 2U);
                c1.B = (byte)((uint)c1.B / 2U);
            }
            if (Game1.random.NextDouble() < 0.5)
                c1.R = (byte)Game1.random.Next(15, 50);
            if (Game1.random.NextDouble() < 0.5)
                c1.G = (byte)Game1.random.Next(15, 50);
            if (Game1.random.NextDouble() < 0.5)
                c1.B = (byte)Game1.random.Next(15, 50);
            Game1.player.changeHairColor(c1);
            Game1.player.changeShirt(Game1.random.Next(112));
            Game1.player.changeSkinColor(Game1.random.Next(6));
            if (Game1.random.NextDouble() < 0.25)
                Game1.player.changeSkinColor(Game1.random.Next(24));
            Color color = new Color(Game1.random.Next(25, 254), Game1.random.Next(25, 254), Game1.random.Next(25, 254));
            if (Game1.random.NextDouble() < 0.5)
            {
                color.R = (byte)((uint)color.R / 2U);
                color.G = (byte)((uint)color.G / 2U);
                color.B = (byte)((uint)color.B / 2U);
            }
            if (Game1.random.NextDouble() < 0.5)
                color.R = (byte)Game1.random.Next(15, 50);
            if (Game1.random.NextDouble() < 0.5)
                color.G = (byte)Game1.random.Next(15, 50);
            if (Game1.random.NextDouble() < 0.5)
                color.B = (byte)Game1.random.Next(15, 50);
            Game1.player.changePants(color);
            Color c2 = new Color(Game1.random.Next(25, 254), Game1.random.Next(25, 254), Game1.random.Next(25, 254));
            c2.R = (byte)((uint)c2.R / 2U);
            c2.G = (byte)((uint)c2.G / 2U);
            c2.B = (byte)((uint)c2.B / 2U);
            if (Game1.random.NextDouble() < 0.5)
                c2.R = (byte)Game1.random.Next(15, 50);
            if (Game1.random.NextDouble() < 0.5)
                c2.G = (byte)Game1.random.Next(15, 50);
            if (Game1.random.NextDouble() < 0.5)
                c2.B = (byte)Game1.random.Next(15, 50);
            Game1.player.changeEyeColor(c2);
            this.randomButton.scale = (float)Game1.pixelZoom - 0.5f;
            this.pantsColorPicker.setColor(Game1.player.pantsColor);
            this.eyeColorPicker.setColor(Game1.player.newEyeColor);
            this.hairColorPicker.setColor(Game1.player.hairstyleColor);
        }

        public override void leftClickHeld(int x, int y)
        {
            this.colorPickerTimer = this.colorPickerTimer - Game1.currentGameTime.ElapsedGameTime.Milliseconds;
            if (this.colorPickerTimer > 0)
                return;
            if (this.lastHeldColorPicker != null && !Game1.options.SnappyMenus)
            {
                if (this.lastHeldColorPicker.Equals((object)this.hairColorPicker))
                    Game1.player.changeHairColor(this.hairColorPicker.clickHeld(x, y));
                if (this.lastHeldColorPicker.Equals((object)this.pantsColorPicker))
                    Game1.player.changePants(this.pantsColorPicker.clickHeld(x, y));
                if (this.lastHeldColorPicker.Equals((object)this.eyeColorPicker))
                    Game1.player.changeEyeColor(this.eyeColorPicker.clickHeld(x, y));
            }
            this.colorPickerTimer = 100;
        }

        public override void releaseLeftClick(int x, int y)
        {
            this.hairColorPicker.releaseClick();
            this.pantsColorPicker.releaseClick();
            this.eyeColorPicker.releaseClick();
            this.lastHeldColorPicker = (ColorPicker)null;
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
                }
                else if (this.farmnameBox.Selected)
                {
                    this.farmnameBox.Selected = false;
                    this.favThingBox.SelectMe();
                }
                else
                {
                    this.favThingBox.Selected = false;
                    this.nameBox.SelectMe();
                }
            }
            if (!Game1.options.SnappyMenus || Game1.options.doesInputListContain(Game1.options.menuButton, key) || ((IEnumerable<Keys>)Keyboard.GetState().GetPressedKeys()).Count<Keys>() != 0)
                return;
            base.receiveKeyPress(key);
        }

        public override void performHoverAction(int x, int y)
        {
            this.hoverText = "";
            this.hoverTitle = "";
            foreach (ClickableTextureComponent leftSelectionButton in this.leftSelectionButtons)
            {
                if (leftSelectionButton.containsPoint(x, y))
                    leftSelectionButton.scale = Math.Min(leftSelectionButton.scale + 0.02f, leftSelectionButton.baseScale + 0.1f);
                else
                    leftSelectionButton.scale = Math.Max(leftSelectionButton.scale - 0.02f, leftSelectionButton.baseScale);
            }
            foreach (ClickableTextureComponent rightSelectionButton in this.rightSelectionButtons)
            {
                if (rightSelectionButton.containsPoint(x, y))
                    rightSelectionButton.scale = Math.Min(rightSelectionButton.scale + 0.02f, rightSelectionButton.baseScale + 0.1f);
                else
                    rightSelectionButton.scale = Math.Max(rightSelectionButton.scale - 0.02f, rightSelectionButton.baseScale);
            }
            if (!this.wizardSource)
            {
                foreach (ClickableTextureComponent farmTypeButton in this.farmTypeButtons)
                {
                    //if (farmTypeButton.containsPoint(x, y) && !farmTypeButton.name.Contains("Gray"))
                    /////////////////////////////////////////////////////////
                    if (farmTypeButton.containsPoint(x, y - scroll) && !farmTypeButton.name.Contains("Gray"))
                    /////////////////////////////////////////////////////////
                    {
                        farmTypeButton.scale = Math.Min(farmTypeButton.scale + 0.02f, farmTypeButton.baseScale + 0.1f);
                        this.hoverTitle = farmTypeButton.hoverText.Split('_')[0];
                        /////////////////////////////////////////////////////////
                        if ( farmTypeButton.name.StartsWith( "Custom_" ) )
                        {
                            FarmType type = FarmType.getType(farmTypeButton.name.Substring(7));
                            if (type == null)
                                continue;
                            this.hoverText = type.Description;
                        }
                        else
                        /////////////////////////////////////////////////////////
                        this.hoverText = farmTypeButton.hoverText.Split('_')[1];
                    }
                    else
                    {
                        farmTypeButton.scale = Math.Max(farmTypeButton.scale - 0.02f, farmTypeButton.baseScale);
                        if (farmTypeButton.name.Contains("Gray") && farmTypeButton.containsPoint(x, y))
                            this.hoverText = "Reach level 10 " + Game1.content.LoadString("Strings\\UI:Character_" + farmTypeButton.name.Split('_')[1]) + " to unlock.";
                    }
                }
            }
            if (!this.wizardSource)
            {
                foreach (ClickableTextureComponent genderButton in this.genderButtons)
                {
                    if (genderButton.containsPoint(x, y))
                        genderButton.scale = Math.Min(genderButton.scale + 0.02f, genderButton.baseScale + 0.1f);
                    else
                        genderButton.scale = Math.Max(genderButton.scale - 0.02f, genderButton.baseScale);
                }
                foreach (ClickableTextureComponent petButton in this.petButtons)
                {
                    if (petButton.containsPoint(x, y))
                        petButton.scale = Math.Min(petButton.scale + 0.02f, petButton.baseScale + 0.1f);
                    else
                        petButton.scale = Math.Max(petButton.scale - 0.02f, petButton.baseScale);
                }
            }
            if (this.okButton.containsPoint(x, y) && this.canLeaveMenu())
                this.okButton.scale = Math.Min(this.okButton.scale + 0.02f, this.okButton.baseScale + 0.1f);
            else
                this.okButton.scale = Math.Max(this.okButton.scale - 0.02f, this.okButton.baseScale);
            this.randomButton.tryHover(x, y, 0.25f);
            this.randomButton.tryHover(x, y, 0.25f);
            if (this.hairColorPicker.containsPoint(x, y) || this.pantsColorPicker.containsPoint(x, y) || this.eyeColorPicker.containsPoint(x, y))
                Game1.SetFreeCursorDrag();
            this.nameBox.Hover(x, y);
            this.farmnameBox.Hover(x, y);
            this.favThingBox.Hover(x, y);
            this.skipIntroButton.tryHover(x, y, 0.1f);
        }

        public bool canLeaveMenu()
        {
            if (this.wizardSource)
                return true;
            if (Game1.player.name.Length > 0 && Game1.player.farmName.Length > 0)
                return Game1.player.favoriteThing.Length > 0;
            return false;
        }

        public override void draw(SpriteBatch b)
        {
            /////////////////////////////////////////////////////////
            var keyState = Keyboard.GetState();
            if (keyState.IsKeyDown(Keys.Up))
                scroll += 5;
            if (keyState.IsKeyDown(Keys.Down))
                scroll -= 5;

            if (scroll > 0) scroll = 0;

            int cap = (farmTypeButtons[0].bounds.Height + Game1.pixelZoom * 3) * farmTypeButtons.Count - scrollRect.Height;
            if (scroll <= -cap) scroll = -cap;
            /////////////////////////////////////////////////////////
            Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, false, true, (string)null, false);
            b.Draw(Game1.daybg, new Vector2((float)(this.xPositionOnScreen + Game1.tileSize + Game1.tileSize * 2 / 3 - 2), (float)(this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 4)), Color.White);
            Game1.player.FarmerRenderer.draw(b, Game1.player.FarmerSprite.CurrentAnimationFrame, Game1.player.FarmerSprite.CurrentFrame, Game1.player.FarmerSprite.SourceRect, new Vector2((float)(this.xPositionOnScreen - 2 + Game1.tileSize * 2 / 3 + Game1.tileSize * 2 - Game1.tileSize / 2), (float)(this.yPositionOnScreen + IClickableMenu.borderWidth - Game1.tileSize / 4 + IClickableMenu.spaceToClearTopBorder + Game1.tileSize / 2)), Vector2.Zero, 0.8f, Color.White, 0.0f, 1f, Game1.player);
            if (!this.wizardSource)
            {
                foreach (ClickableTextureComponent genderButton in this.genderButtons)
                {
                    genderButton.draw(b);
                    if (genderButton.name.Equals("Male") && Game1.player.isMale || genderButton.name.Equals("Female") && !Game1.player.isMale)
                        b.Draw(Game1.mouseCursors, genderButton.bounds, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 34, -1, -1)), Color.White);
                }
                foreach (ClickableTextureComponent petButton in this.petButtons)
                {
                    petButton.draw(b);
                    if (petButton.name.Equals("Cat") && Game1.player.catPerson || petButton.name.Equals("Dog") && !Game1.player.catPerson)
                        b.Draw(Game1.mouseCursors, petButton.bounds, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 34, -1, -1)), Color.White);
                }
                Game1.player.name = this.nameBox.Text;
                Game1.player.favoriteThing = this.favThingBox.Text;
                Game1.player.farmName = this.farmnameBox.Text;
            }
            foreach (ClickableTextureComponent leftSelectionButton in this.leftSelectionButtons)
                leftSelectionButton.draw(b);
            foreach (ClickableComponent label in this.labels)
            {
                string text = "";
                float num = 0.0f;
                Color color = Game1.textColor;
                if (label == this.nameLabel)
                {
                    color = Game1.player.name.Length < 1 ? Color.Red : Game1.textColor;
                    if (this.wizardSource)
                        continue;
                }
                else if (label == this.farmLabel)
                {
                    color = Game1.player.farmName.Length < 1 ? Color.Red : Game1.textColor;
                    if (this.wizardSource)
                        continue;
                }
                else if (label == this.favoriteLabel)
                {
                    color = Game1.player.favoriteThing.Length < 1 ? Color.Red : Game1.textColor;
                    if (this.wizardSource)
                        continue;
                }
                else if (label == this.shirtLabel)
                {
                    num = (float)(Game1.tileSize / 3) - Game1.smallFont.MeasureString(label.name).X / 2f;
                    text = string.Concat((object)(Game1.player.shirt + 1));
                }
                else if (label == this.skinLabel)
                {
                    num = (float)(Game1.tileSize / 3) - Game1.smallFont.MeasureString(label.name).X / 2f;
                    text = string.Concat((object)(Game1.player.skin + 1));
                }
                else if (label == this.hairLabel)
                {
                    num = (float)(Game1.tileSize / 3) - Game1.smallFont.MeasureString(label.name).X / 2f;
                    if (!label.name.Contains("Color"))
                        text = string.Concat((object)(Game1.player.hair + 1));
                }
                else if (label == this.accLabel)
                {
                    num = (float)(Game1.tileSize / 3) - Game1.smallFont.MeasureString(label.name).X / 2f;
                    text = string.Concat((object)(Game1.player.accessory + 2));
                }
                else
                    color = Game1.textColor;
                Utility.drawTextWithShadow(b, label.name, Game1.smallFont, new Vector2((float)label.bounds.X + num, (float)label.bounds.Y), color, 1f, -1f, -1, -1, 1f, 3);
                if (text.Length > 0)
                    Utility.drawTextWithShadow(b, text, Game1.smallFont, new Vector2((float)(label.bounds.X + Game1.tileSize / 3) - Game1.smallFont.MeasureString(text).X / 2f, (float)(label.bounds.Y + Game1.tileSize / 2)), color, 1f, -1f, -1, -1, 1f, 3);
            }
            foreach (ClickableTextureComponent rightSelectionButton in this.rightSelectionButtons)
                rightSelectionButton.draw(b);
            if (!this.wizardSource)
            {
                IClickableMenu.drawTextureBox(b, this.farmTypeButtons[0].bounds.X - Game1.pixelZoom * 4, this.farmTypeButtons[0].bounds.Y - Game1.pixelZoom * 5, 30 * Game1.pixelZoom, 110 * Game1.pixelZoom + Game1.pixelZoom * 9, Color.White);
                /////////////////////////////////////////////////////////
                b.End();
                RasterizerState state = new RasterizerState();
                state.ScissorTestEnable = true;
                Matrix transform = Matrix.CreateTranslation(0, scroll, 0);
                b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, state, null, transform);
                b.GraphicsDevice.ScissorRectangle = scrollRect = new Rectangle(this.farmTypeButtons[0].bounds.X - Game1.pixelZoom * 4, this.farmTypeButtons[0].bounds.Y - Game1.pixelZoom * 5, 30 * Game1.pixelZoom, 110 * Game1.pixelZoom + Game1.pixelZoom * 9);
                /////////////////////////////////////////////////////////
                for (int index = 0; index < this.farmTypeButtons.Count; ++index)
                {
                    this.farmTypeButtons[index].draw(b, this.farmTypeButtons[index].name.Contains("Gray") ? Color.Black * 0.5f : Color.White, 0.88f);
                    if (this.farmTypeButtons[index].name.Contains("Gray"))
                        b.Draw(Game1.mouseCursors, new Vector2((float)(this.farmTypeButtons[index].bounds.Center.X - Game1.pixelZoom * 3), (float)(this.farmTypeButtons[index].bounds.Center.Y - Game1.pixelZoom * 2)), new Rectangle?(new Rectangle(107, 442, 7, 8)), Color.White, 0.0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, 0.89f);
                    //if (index == Game1.whichFarm)
                    /////////////////////////////////////////////////////////
                    if (index == Game1.whichFarm && !Data.FarmTypes.ContainsKey("Farm") || (Data.FarmTypes.ContainsKey("Farm") && farmTypeButtons[index].name.StartsWith("Custom_") && farmTypeButtons[index].name.Substring(7) == Data.FarmTypes["Farm"]))
                    /////////////////////////////////////////////////////////
                        IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(375, 357, 3, 3), this.farmTypeButtons[index].bounds.X, this.farmTypeButtons[index].bounds.Y - Game1.pixelZoom, this.farmTypeButtons[index].bounds.Width, this.farmTypeButtons[index].bounds.Height + Game1.pixelZoom * 2, Color.White, (float)Game1.pixelZoom, false);
                }
                /////////////////////////////////////////////////////////
                b.End();
                b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
                /////////////////////////////////////////////////////////
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
            this.hairColorPicker.draw(b);
            this.pantsColorPicker.draw(b);
            this.eyeColorPicker.draw(b);
            if (!this.wizardSource)
            {
                this.nameBox.Draw(b);
                this.farmnameBox.Draw(b);
                if (this.skipIntroButton != null)
                {
                    this.skipIntroButton.draw(b);
                    Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\UI:Character_SkipIntro"), Game1.smallFont, new Vector2((float)(this.skipIntroButton.bounds.X + this.skipIntroButton.bounds.Width + Game1.pixelZoom * 2), (float)(this.skipIntroButton.bounds.Y + Game1.pixelZoom * 2)), Game1.textColor, 1f, -1f, -1, -1, 1f, 3);
                }
                Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\UI:Character_FarmNameSuffix"), Game1.smallFont, new Vector2((float)(this.farmnameBox.X + this.farmnameBox.Width + Game1.pixelZoom * 2), (float)(this.farmnameBox.Y + Game1.pixelZoom * 3)), Game1.textColor, 1f, -1f, -1, -1, 1f, 3);
                this.favThingBox.Draw(b);
            }
            if (this.hoverText != null && this.hoverTitle != null && this.hoverText.Count<char>() > 0)
                IClickableMenu.drawHoverText(b, Game1.parseText(this.hoverText, Game1.smallFont, Game1.tileSize * 4), Game1.smallFont, 0, 0, -1, this.hoverTitle, -1, (string[])null, (Item)null, 0, -1, -1, -1, -1, 1f, (CraftingRecipe)null);
            this.randomButton.draw(b);
            this.drawMouse(b);
        }
    }
}