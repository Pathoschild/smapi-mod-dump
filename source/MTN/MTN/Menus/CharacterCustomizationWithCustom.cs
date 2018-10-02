using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Minigames;

namespace MTN.Menus
{
    /// <summary>
    /// *Almost* a complete replica of CharacterCustomization. It was easier to replicate, instead of using 10+ Harmony patches.
    /// This replica allows more than 5 farm type buttons, No Debris options, and whatever else is needed in the future
    /// </summary>
    public class CharacterCustomizationWithCustom : IClickableMenu
    {
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
        public const int region_cabinsLeft = 621;
        public const int region_cabinsRight = 622;
        public const int region_cabinsClose = 623;
        public const int region_cabinsSeparate = 624;
        public const int region_coopHelp = 625;
        public const int region_coopHelpOK = 626;
        public const int region_difficultyLeft = 627;
        public const int region_difficultyRight = 628;
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
        public const int widthOfMultiplayerArea = 256;
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
        public List<ClickableComponent> labels = new List<ClickableComponent>();
        public List<ClickableComponent> leftSelectionButtons = new List<ClickableComponent>();
        public List<ClickableComponent> rightSelectionButtons = new List<ClickableComponent>();
        public List<ClickableComponent> genderButtons = new List<ClickableComponent>();
        public List<ClickableComponent> petButtons = new List<ClickableComponent>();
        public List<ClickableTextureComponent> farmTypeButtons = new List<ClickableTextureComponent>();
        public List<ClickableComponent> colorPickerCCs = new List<ClickableComponent>();
        public List<ClickableTextureComponent> cabinLayoutButtons = new List<ClickableTextureComponent>();
        public ClickableTextureComponent okButton;
        public ClickableTextureComponent skipIntroButton;
        public ClickableTextureComponent randomButton;
        public ClickableTextureComponent coopHelpButton;
        public ClickableTextureComponent coopHelpOkButton;

        //New Stuff//
        public List<ClickableTextureComponent> newFarmTypeButtons = new List<ClickableTextureComponent>();
        public ClickableTextureComponent noDebrisButton;
        public ClickableTextureComponent upArrow;
        public ClickableTextureComponent downArrow;
        public ClickableTextureComponent scrollBar;
        public Rectangle scrollBarRunner;
        public int currentItemIndex;
        public string lastClickedFarmTypeBtn = "Standard";
        public List<MenuProperty> menuProperties;
        public bool allowCabinsSeperate = true;
        public bool allowCabinsClose = true;
        public bool scrolling = false;
        //New Stuff//

        private TextBox nameBox;
        private TextBox farmnameBox;
        private TextBox favThingBox;
        private bool skipIntro;
        public bool showingCoopHelp;
        public CharacterCustomization.Source source;
        private string hoverText;
        private string hoverTitle;
        private string coopHelpString;
        private string noneString;
        private string normalDiffString;
        private string toughDiffString;
        private string hardDiffString;
        private string superDiffString;
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
        private ClickableComponent startingCabinsLabel;
        private ClickableComponent cabinLayoutLabel;
        private ClickableComponent difficultyModifierLabel;
        private ColorPicker lastHeldColorPicker;
        private int timesRandom;

        public CharacterCustomizationWithCustom(CharacterCustomization.Source source) : base(Game1.viewport.Width / 2 - (632 + IClickableMenu.borderWidth * 2) / 2, Game1.viewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2 - 64, 632 + IClickableMenu.borderWidth * 2, 600 + IClickableMenu.borderWidth * 2 + 64, false)
        {
            this.shirtOptions = new List<int>
            {
                0,
                1,
                2,
                3,
                4,
                5
            };
            this.hairStyleOptions = new List<int>
            {
                0,
                1,
                2,
                3,
                4,
                5
            };
            this.accessoryOptions = new List<int>
            {
                0,
                1,
                2,
                3,
                4,
                5
            };
            this.source = source;
            this.setUpPositions();
            Game1.player.faceDirection(2);
            Game1.player.FarmerSprite.StopAnimation();
        }

        // Token: 0x0600136F RID: 4975 RVA: 0x00132544 File Offset: 0x00130744
        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            base.gameWindowSizeChanged(oldBounds, newBounds);
            this.xPositionOnScreen = Game1.viewport.Width / 2 - (632 + IClickableMenu.borderWidth * 2) / 2;
            this.yPositionOnScreen = Game1.viewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2 - 64;
            this.setUpPositions();
        }

        // Token: 0x06001370 RID: 4976 RVA: 0x001325A8 File Offset: 0x001307A8
        private void setUpPositions()
        {
            this.labels.Clear();
            this.petButtons.Clear();
            this.genderButtons.Clear();
            this.cabinLayoutButtons.Clear();
            this.leftSelectionButtons.Clear();
            this.rightSelectionButtons.Clear();
            this.farmTypeButtons.Clear();
            newFarmTypeButtons.Clear();
            this.okButton = new ClickableTextureComponent("OK", new Rectangle(this.xPositionOnScreen + this.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - 64, this.yPositionOnScreen + this.height - IClickableMenu.borderWidth - IClickableMenu.spaceToClearTopBorder + 16, 64, 64), null, null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46, -1, -1), 1f, false)
            {
                myID = 505,
                upNeighborID = 530,
                leftNeighborID = 506,
                rightNeighborID = 535,
                downNeighborID = ((this.source != CharacterCustomization.Source.NewGame) ? -1 : 81114)
            };
            this.nameBox = new TextBox(Game1.content.Load<Texture2D>("LooseSprites\\textBox"), null, Game1.smallFont, Game1.textColor)
            {
                X = this.xPositionOnScreen + 64 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 256,
                Y = this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 16,
                Text = Game1.player.Name
            };
            this.backButton = new ClickableComponent(new Rectangle(Game1.viewport.Width + -198 - 48, Game1.viewport.Height - 81 - 24, 198, 81), "")
            {
                myID = 81114,
                leftNeighborID = 535
            };
            this.nameBoxCC = new ClickableComponent(new Rectangle(this.xPositionOnScreen + 64 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 256, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 16, 192, 48), "")
            {
                myID = 536,
                leftNeighborID = 507,
                downNeighborID = 537,
                rightNeighborID = 531
            };
            int textBoxLabelsXOffset = (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ru || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.es || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.pt) ? -4 : 0;
            this.labels.Add(this.nameLabel = new ClickableComponent(new Rectangle(this.xPositionOnScreen + textBoxLabelsXOffset + 16 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 192 + 4, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 8, 1, 1), Game1.content.LoadString("Strings\\UI:Character_Name")));
            this.farmnameBox = new TextBox(Game1.content.Load<Texture2D>("LooseSprites\\textBox"), null, Game1.smallFont, Game1.textColor)
            {
                X = this.xPositionOnScreen + 64 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 256,
                Y = this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 16 + 64,
                Text = Game1.player.farmName
            };
            this.farmnameBoxCC = new ClickableComponent(new Rectangle(this.xPositionOnScreen + 64 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 256, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 16 + 64, 192, 48), "")
            {
                myID = 537,
                leftNeighborID = 507,
                downNeighborID = 538,
                upNeighborID = 536,
                rightNeighborID = 531
            };
            this.labels.Add(this.farmLabel = new ClickableComponent(new Rectangle(this.xPositionOnScreen + textBoxLabelsXOffset * 3 + 16 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 192 + 4, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 16 + 64, 1, 1), Game1.content.LoadString("Strings\\UI:Character_Farm")));
            this.favThingBox = new TextBox(Game1.content.Load<Texture2D>("LooseSprites\\textBox"), null, Game1.smallFont, Game1.textColor)
            {
                X = this.xPositionOnScreen + 64 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 256,
                Y = this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 16 + 128,
                Text = Game1.player.favoriteThing
            };
            this.favThingBoxCC = new ClickableComponent(new Rectangle(this.xPositionOnScreen + 64 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 256, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 16 + 128, 192, 48), "")
            {
                myID = 538,
                leftNeighborID = 521,
                downNeighborID = 511,
                upNeighborID = 537,
                rightNeighborID = 531
            };
            this.labels.Add(this.favoriteLabel = new ClickableComponent(new Rectangle(this.xPositionOnScreen + textBoxLabelsXOffset + 16 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 192 + 4, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 16 + 128, 1, 1), Game1.content.LoadString("Strings\\UI:Character_FavoriteThing")));
            this.randomButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + 48, this.yPositionOnScreen + 64 + 56, 40, 40), Game1.mouseCursors, new Rectangle(381, 361, 10, 10), 4f, false)
            {
                myID = 507,
                rightNeighborID = 536,
                downNeighborID = 520,
                leftNeighborID = 622
            };
            int yOffset = 128;
            this.leftSelectionButtons.Add(new ClickableTextureComponent("Direction", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 16, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44, -1, -1), 1f, false)
            {
                myID = 520,
                rightNeighborID = 521,
                upNeighborID = 507,
                downNeighborID = 508,
                leftNeighborID = 622
            });
            this.rightSelectionButtons.Add(new ClickableTextureComponent("Direction", new Rectangle(this.xPositionOnScreen + 16 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 128, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33, -1, -1), 1f, false)
            {
                myID = 521,
                leftNeighborID = 520,
                downNeighborID = 509,
                upNeighborID = 507,
                rightNeighborID = 538
            });
            if (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm)
            {
                this.labels.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + 16 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 192 + 8 + textBoxLabelsXOffset, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 8 + 192, 1, 1), Game1.content.LoadString("Strings\\UI:Character_Animal")));
            }
            int petButtonXOffset = (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ru) ? 60 : 0;
            this.petButtons.Add(new ClickableTextureComponent("Cat", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 384 - 16 + petButtonXOffset, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 192 - 16, 64, 64), null, "Cat", Game1.mouseCursors, new Rectangle(160, 192, 16, 16), 4f, false)
            {
                myID = 511,
                rightNeighborID = 510,
                leftNeighborID = 509,
                downNeighborID = 522,
                upNeighborID = 538
            });
            this.petButtons.Add(new ClickableTextureComponent("Dog", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 448 - 16 + petButtonXOffset, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 192 - 16, 64, 64), null, "Dog", Game1.mouseCursors, new Rectangle(176, 192, 16, 16), 4f, false)
            {
                myID = 510,
                leftNeighborID = 511,
                downNeighborID = 522,
                rightNeighborID = 532,
                upNeighborID = 538
            });
            this.genderButtons.Add(new ClickableTextureComponent("Male", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 32 + 8, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 192, 64, 64), null, "Male", Game1.mouseCursors, new Rectangle(128, 192, 16, 16), 4f, false)
            {
                myID = 508,
                rightNeighborID = 509,
                downNeighborID = 518,
                upNeighborID = 520,
                leftNeighborID = 624
            });
            this.genderButtons.Add(new ClickableTextureComponent("Female", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 32 + 64 + 8, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 192, 64, 64), null, "Female", Game1.mouseCursors, new Rectangle(144, 192, 16, 16), 4f, false)
            {
                myID = 509,
                leftNeighborID = 508,
                downNeighborID = 519,
                rightNeighborID = 511,
                upNeighborID = 521
            });
            yOffset = 264;
            int leftSelectionXOffset = (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ru || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.es || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.pt) ? -20 : 0;
            this.leftSelectionButtons.Add(new ClickableTextureComponent("Skin", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 16 + leftSelectionXOffset, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44, -1, -1), 1f, false)
            {
                myID = 518,
                rightNeighborID = 519,
                downNeighborID = 514,
                upNeighborID = 508,
                leftNeighborID = 628
            });
            this.labels.Add(this.skinLabel = new ClickableComponent(new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 16 + 64 + 8 + leftSelectionXOffset / 2, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset + 16, 1, 1), Game1.content.LoadString("Strings\\UI:Character_Skin")));
            this.rightSelectionButtons.Add(new ClickableTextureComponent("Skin", new Rectangle(this.xPositionOnScreen + 16 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 128, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33, -1, -1), 1f, false)
            {
                myID = 519,
                leftNeighborID = 518,
                downNeighborID = 515,
                upNeighborID = 509,
                rightNeighborID = 522
            });

            /////////////////////////
            //POINT OF INTERCEPTION//
            /////////////////////////
            //Farm Type/Map Buttons
            if (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm)
            {

                Game1.startingCabins = 0;
                Game1.player.difficultyModifier = 1f;
                Point baseFarmButton = new Point(this.xPositionOnScreen + this.width + 4 + 8, this.yPositionOnScreen + IClickableMenu.borderWidth * 2);
                this.farmTypeButtons.Add(new ClickableTextureComponent("Standard", new Rectangle(baseFarmButton.X, baseFarmButton.Y + 88, 88, 80), null, Game1.content.LoadString("Strings\\UI:Character_FarmStandard"), Game1.mouseCursors, new Rectangle(0, 324, 22, 20), 4f, false)
                {
                    myID = 531,
                    downNeighborID = 532,
                    leftNeighborID = 537
                });
                this.farmTypeButtons.Add(new ClickableTextureComponent("Riverland", new Rectangle(baseFarmButton.X, baseFarmButton.Y + 176, 88, 80), null, Game1.content.LoadString("Strings\\UI:Character_FarmFishing"), Game1.mouseCursors, new Rectangle(22, 324, 22, 20), 4f, false)
                {
                    myID = 532,
                    downNeighborID = 533,
                    upNeighborID = 531,
                    leftNeighborID = 510,
                    rightNeighborID = 81114
                });
                this.farmTypeButtons.Add(new ClickableTextureComponent("Forest", new Rectangle(baseFarmButton.X, baseFarmButton.Y + 264, 88, 80), null, Game1.content.LoadString("Strings\\UI:Character_FarmForaging"), Game1.mouseCursors, new Rectangle(44, 324, 22, 20), 4f, false)
                {
                    myID = 533,
                    downNeighborID = 534,
                    upNeighborID = 532,
                    leftNeighborID = 522,
                    rightNeighborID = 81114
                });
                this.farmTypeButtons.Add(new ClickableTextureComponent("Hills", new Rectangle(baseFarmButton.X, baseFarmButton.Y + 352, 88, 80), null, Game1.content.LoadString("Strings\\UI:Character_FarmMining"), Game1.mouseCursors, new Rectangle(66, 324, 22, 20), 4f, false)
                {
                    myID = 534,
                    downNeighborID = 535,
                    upNeighborID = 533,
                    leftNeighborID = 525,
                    rightNeighborID = 81114
                });
                this.farmTypeButtons.Add(new ClickableTextureComponent("Wilderness", new Rectangle(baseFarmButton.X, baseFarmButton.Y + 440, 88, 80), null, Game1.content.LoadString("Strings\\UI:Character_FarmCombat"), Game1.mouseCursors, new Rectangle(88, 324, 22, 20), 4f, false)
                {
                    myID = 535,
                    upNeighborID = 534,
                    leftNeighborID = 528,
                    downNeighborID = 505,
                    rightNeighborID = 81114
                });

                //Copy them over.
                for (int k = 0; k < farmTypeButtons.Count; k++)
                {
                    newFarmTypeButtons.Add(farmTypeButtons[k]);
                }

                //Grab the custom farms. Add to farmTypeButtons. Handle the 5+ case in draw.
                for (int k = 5; k < Memory.customFarms.Count; k++)
                {
                    newFarmTypeButtons.Add(new ClickableTextureComponent(Memory.customFarms[k].Name, new Rectangle(baseFarmButton.X, baseFarmButton.Y + 440, 88, 80), null, Memory.customFarms[k].Description, Memory.customFarms[k].IconSource, new Rectangle(0, 0, 22, 20), 4f, false));
                }

                //Set up menu properties for scrolling
                menuProperties = new List<MenuProperty> {
                    new MenuProperty(88, 531, 532, 0, 537, 0),
                    new MenuProperty(176, 532, 533, 531, 510, 81114),
                    new MenuProperty(264, 533, 534, 532, 522, 81114),
                    new MenuProperty(352, 534, 535, 533, 525, 81114),
                    new MenuProperty(440, 535, 505, 534, 528, 81114)
                };

                //Set up scroll bar / arrow buttons
                upArrow = new ClickableTextureComponent(new Rectangle(baseFarmButton.X + 115, baseFarmButton.Y + 75, 44, 48), Game1.mouseCursors, new Rectangle(421, 459, 11, 12), 4f, false);
                downArrow = new ClickableTextureComponent(new Rectangle(baseFarmButton.X + 115, baseFarmButton.Y + 500, 44, 48), Game1.mouseCursors, new Rectangle(421, 472, 11, 12), 4f, false);
                scrollBar = new ClickableTextureComponent(new Rectangle(upArrow.bounds.X + 11, upArrow.bounds.Y + upArrow.bounds.Height + 4, 24, 20), Game1.mouseCursors, new Rectangle(435, 463, 6, 10), 4f, false);
                scrollBarRunner = new Rectangle(scrollBar.bounds.X, upArrow.bounds.Y + upArrow.bounds.Height + 4, scrollBar.bounds.Width, this.height - upArrow.bounds.Height - 332);
            }
            //End of Interception

            if (this.source == CharacterCustomization.Source.HostNewFarm)
            {
                this.labels.Add(this.startingCabinsLabel = new ClickableComponent(new Rectangle(this.xPositionOnScreen - 21 - 128, this.yPositionOnScreen + IClickableMenu.borderWidth * 2 + 84, 1, 1), Game1.content.LoadString("Strings\\UI:Character_StartingCabins")));
                this.leftSelectionButtons.Add(new ClickableTextureComponent("Cabins", new Rectangle(this.xPositionOnScreen - 256 + IClickableMenu.borderWidth / 2 + 8, this.yPositionOnScreen + IClickableMenu.borderWidth * 2 + 108, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44, -1, -1), 1f, false)
                {
                    myID = 621,
                    rightNeighborID = 622,
                    downNeighborID = 623
                });
                this.rightSelectionButtons.Add(new ClickableTextureComponent("Cabins", new Rectangle(this.xPositionOnScreen - 256 + IClickableMenu.borderWidth + 128 + 8, this.yPositionOnScreen + IClickableMenu.borderWidth * 2 + 108, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33, -1, -1), 1f, false)
                {
                    myID = 622,
                    leftNeighborID = 621,
                    rightNeighborID = 507,
                    downNeighborID = 624
                });
                this.labels.Add(this.cabinLayoutLabel = new ClickableComponent(new Rectangle(this.xPositionOnScreen - 128 - (int)(Game1.smallFont.MeasureString(Game1.content.LoadString("Strings\\UI:Character_CabinLayout")).X / 2f), this.yPositionOnScreen + IClickableMenu.borderWidth * 2 + 120 + 64, 1, 1), Game1.content.LoadString("Strings\\UI:Character_CabinLayout")));
                this.cabinLayoutButtons.Add(new ClickableTextureComponent("Close", new Rectangle(this.xPositionOnScreen - 256 + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.borderWidth * 2 + 160 + 64, 64, 64), null, Game1.content.LoadString("Strings\\UI:Character_Close"), Game1.mouseCursors, new Rectangle(208, 192, 16, 16), 4f, false)
                {
                    myID = 623,
                    upNeighborID = 621,
                    rightNeighborID = 624,
                    downNeighborID = 627
                });
                this.cabinLayoutButtons.Add(new ClickableTextureComponent("Separate", new Rectangle(this.xPositionOnScreen - 256 + IClickableMenu.borderWidth + 128 - 8, this.yPositionOnScreen + IClickableMenu.borderWidth * 2 + 160 + 64, 64, 64), null, Game1.content.LoadString("Strings\\UI:Character_Separate"), Game1.mouseCursors, new Rectangle(224, 192, 16, 16), 4f, false)
                {
                    myID = 624,
                    upNeighborID = 622,
                    leftNeighborID = 623,
                    rightNeighborID = 508,
                    downNeighborID = 628
                });
                this.labels.Add(this.difficultyModifierLabel = new ClickableComponent(new Rectangle(this.xPositionOnScreen - 21 - 128, this.yPositionOnScreen + IClickableMenu.borderWidth * 2 + 256 + 56, 1, 1), Game1.content.LoadString("Strings\\UI:Character_Difficulty")));
                this.leftSelectionButtons.Add(new ClickableTextureComponent("Difficulty", new Rectangle(this.xPositionOnScreen - 256 + IClickableMenu.borderWidth / 2 - 4, this.yPositionOnScreen + IClickableMenu.borderWidth * 2 + 256 + 80, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44, -1, -1), 1f, false)
                {
                    myID = 627,
                    rightNeighborID = 628,
                    downNeighborID = 625,
                    upNeighborID = 623
                });
                this.rightSelectionButtons.Add(new ClickableTextureComponent("Difficulty", new Rectangle(this.xPositionOnScreen - 256 + IClickableMenu.borderWidth + 128 + 12, this.yPositionOnScreen + IClickableMenu.borderWidth * 2 + 256 + 80, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33, -1, -1), 1f, false)
                {
                    myID = 628,
                    leftNeighborID = 627,
                    rightNeighborID = 518,
                    downNeighborID = 625,
                    upNeighborID = 624
                });
                this.coopHelpButton = new ClickableTextureComponent("CoopHelp", new Rectangle(this.xPositionOnScreen - 256 + IClickableMenu.borderWidth + 128 - 8, this.yPositionOnScreen + IClickableMenu.borderWidth * 2 + 384 + 40, 64, 64), null, Game1.content.LoadString("Strings\\UI:Character_CoopHelp"), Game1.mouseCursors, new Rectangle(240, 192, 16, 16), 4f, false)
                {
                    myID = 625,
                    rightNeighborID = 512,
                    upNeighborID = 628
                };
                this.coopHelpOkButton = new ClickableTextureComponent("CoopHelpOK", new Rectangle(this.xPositionOnScreen - 256 - 12, this.yPositionOnScreen + this.height - 64, 64, 64), null, null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46, -1, -1), 1f, false)
                {
                    myID = 626
                };
                this.noneString = Game1.content.LoadString("Strings\\UI:Character_none");
                this.normalDiffString = Game1.content.LoadString("Strings\\UI:Character_Normal");
                this.toughDiffString = Game1.content.LoadString("Strings\\UI:Character_Tough");
                this.hardDiffString = Game1.content.LoadString("Strings\\UI:Character_Hard");
                this.superDiffString = Game1.content.LoadString("Strings\\UI:Character_Super");
            }
            this.labels.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + 16 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 192 + 8, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset + 16, 1, 1), Game1.content.LoadString("Strings\\UI:Character_EyeColor")));
            Point top = new Point(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 320 + 48 + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset);
            this.eyeColorPicker = new ColorPicker(top.X, top.Y);
            this.eyeColorPicker.setColor(Game1.player.newEyeColor);
            this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(top.X, top.Y, 128, 20), "")
            {
                myID = 522,
                downNeighborID = 523,
                upNeighborID = 511,
                leftNeighborImmutable = true,
                rightNeighborImmutable = true
            });
            this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(top.X, top.Y + 20, 128, 20), "")
            {
                myID = 523,
                upNeighborID = 522,
                downNeighborID = 524,
                leftNeighborImmutable = true,
                rightNeighborImmutable = true
            });
            this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(top.X, top.Y + 40, 128, 20), "")
            {
                myID = 524,
                upNeighborID = 523,
                downNeighborID = 525,
                leftNeighborImmutable = true,
                rightNeighborImmutable = true
            });
            yOffset += 72;
            this.leftSelectionButtons.Add(new ClickableTextureComponent("Hair", new Rectangle(this.xPositionOnScreen + 16 + IClickableMenu.borderWidth + IClickableMenu.spaceToClearSideBorder + leftSelectionXOffset, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44, -1, -1), 1f, false)
            {
                myID = 514,
                rightNeighborID = 515,
                downNeighborID = 512,
                upNeighborID = 518,
                leftNeighborID = 625
            });
            this.labels.Add(this.hairLabel = new ClickableComponent(new Rectangle(this.xPositionOnScreen + 16 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 64 + 8 + leftSelectionXOffset / 2, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset + 16, 1, 1), Game1.content.LoadString("Strings\\UI:Character_Hair")));
            this.rightSelectionButtons.Add(new ClickableTextureComponent("Hair", new Rectangle(this.xPositionOnScreen + 16 + IClickableMenu.spaceToClearSideBorder + 128 + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33, -1, -1), 1f, false)
            {
                myID = 515,
                leftNeighborID = 514,
                downNeighborID = 513,
                upNeighborID = 519,
                rightNeighborID = 525
            });
            this.labels.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + 16 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 192 + 8, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset + 16, 1, 1), Game1.content.LoadString("Strings\\UI:Character_HairColor")));
            top = new Point(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 320 + 48 + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset);
            this.hairColorPicker = new ColorPicker(top.X, top.Y);
            this.hairColorPicker.setColor(Game1.player.hairstyleColor);
            this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(top.X, top.Y, 128, 20), "")
            {
                myID = 525,
                downNeighborID = 526,
                upNeighborID = 524,
                leftNeighborImmutable = true,
                rightNeighborImmutable = true
            });
            this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(top.X, top.Y + 20, 128, 20), "")
            {
                myID = 526,
                upNeighborID = 525,
                downNeighborID = 527,
                leftNeighborImmutable = true,
                rightNeighborImmutable = true
            });
            this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(top.X, top.Y + 40, 128, 20), "")
            {
                myID = 527,
                upNeighborID = 526,
                downNeighborID = 528,
                leftNeighborImmutable = true,
                rightNeighborImmutable = true
            });
            yOffset += 72;
            this.leftSelectionButtons.Add(new ClickableTextureComponent("Shirt", new Rectangle(this.xPositionOnScreen + 16 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + leftSelectionXOffset, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44, -1, -1), 1f, false)
            {
                myID = 512,
                rightNeighborID = 513,
                downNeighborID = 516,
                upNeighborID = 514,
                leftNeighborID = 625
            });
            this.labels.Add(this.shirtLabel = new ClickableComponent(new Rectangle(this.xPositionOnScreen + 16 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 64 + 8 + leftSelectionXOffset / 2, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset + 16, 1, 1), Game1.content.LoadString("Strings\\UI:Character_Shirt")));
            this.rightSelectionButtons.Add(new ClickableTextureComponent("Shirt", new Rectangle(this.xPositionOnScreen + 16 + IClickableMenu.spaceToClearSideBorder + 128 + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33, -1, -1), 1f, false)
            {
                myID = 513,
                leftNeighborID = 512,
                downNeighborID = 517,
                upNeighborID = 515,
                rightNeighborID = 528
            });
            this.labels.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + 16 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 192 + 8, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset + 16, 1, 1), Game1.content.LoadString("Strings\\UI:Character_PantsColor")));
            top = new Point(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 320 + 48 + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset);
            this.pantsColorPicker = new ColorPicker(top.X, top.Y);
            this.pantsColorPicker.setColor(Game1.player.pantsColor);
            this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(top.X, top.Y, 128, 20), "")
            {
                myID = 528,
                downNeighborID = 529,
                upNeighborID = 527,
                leftNeighborImmutable = true,
                rightNeighborImmutable = true
            });
            this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(top.X, top.Y + 20, 128, 20), "")
            {
                myID = 529,
                upNeighborID = 528,
                downNeighborID = 530,
                leftNeighborImmutable = true,
                rightNeighborImmutable = true
            });
            this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(top.X, top.Y + 40, 128, 20), "")
            {
                myID = 530,
                upNeighborID = 529,
                downNeighborID = 506,
                leftNeighborImmutable = true,
                rightNeighborImmutable = true
            });
            this.skipIntroButton = new ClickableTextureComponent("Skip Intro", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 320 - 48 + IClickableMenu.borderWidth - 20, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset + 70, 36, 36), null, Game1.content.LoadString("Strings\\UI:Character_SkipIntro"), Game1.mouseCursors, new Rectangle(227, 425, 9, 9), 4f, false)
            {
                myID = 506,
                upNeighborID = 530,
                leftNeighborID = 517,
                rightNeighborID = 505
            };

            //No Debris Button
            noDebrisButton = new ClickableTextureComponent("No Debris", new Rectangle(skipIntroButton.bounds.X, skipIntroButton.bounds.Y + 45, 64, 64), null, "Start off with a bare farm. No trees, twigs, rocks or weeds.", Game1.mouseCursors, new Rectangle(227, 425, 9, 9), 4f, false);

            yOffset += 72;
            this.leftSelectionButtons.Add(new ClickableTextureComponent("Acc", new Rectangle(this.xPositionOnScreen + 16 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + leftSelectionXOffset, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44, -1, -1), 1f, false)
            {
                myID = 516,
                rightNeighborID = 517,
                upNeighborID = 512,
                leftNeighborID = 625
            });
            this.labels.Add(this.accLabel = new ClickableComponent(new Rectangle(this.xPositionOnScreen + 16 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 64 + 8 + leftSelectionXOffset / 2, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset + 16, 1, 1), Game1.content.LoadString("Strings\\UI:Character_Accessory")));
            this.rightSelectionButtons.Add(new ClickableTextureComponent("Acc", new Rectangle(this.xPositionOnScreen + 16 + IClickableMenu.spaceToClearSideBorder + 128 + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33, -1, -1), 1f, false)
            {
                myID = 517,
                leftNeighborID = 516,
                upNeighborID = 513,
                rightNeighborID = 528
            });
            if (Game1.options.snappyMenus && Game1.options.gamepadControls)
            {
                base.populateClickableComponentList();
                this.snapToDefaultClickableComponent();
            }
        }

        // Token: 0x06001371 RID: 4977 RVA: 0x001346B2 File Offset: 0x001328B2
        public override void snapToDefaultClickableComponent()
        {
            this.currentlySnappedComponent = base.getComponentWithID(521);
            this.snapCursorToCurrentSnappedComponent();
        }

        // Token: 0x06001372 RID: 4978 RVA: 0x001346CC File Offset: 0x001328CC
        public override void gamePadButtonHeld(Buttons b)
        {
            base.gamePadButtonHeld(b);
            if (this.currentlySnappedComponent != null)
            {
                if (b == Buttons.LeftThumbstickRight || b == Buttons.DPadRight)
                {
                    switch (this.currentlySnappedComponent.myID)
                    {
                        case 522:
                            this.eyeColorPicker.changeHue(1);
                            Game1.player.changeEyeColor(this.eyeColorPicker.getSelectedColor());
                            this.lastHeldColorPicker = this.eyeColorPicker;
                            return;
                        case 523:
                            this.eyeColorPicker.changeSaturation(1);
                            Game1.player.changeEyeColor(this.eyeColorPicker.getSelectedColor());
                            this.lastHeldColorPicker = this.eyeColorPicker;
                            return;
                        case 524:
                            this.eyeColorPicker.changeValue(1);
                            Game1.player.changeEyeColor(this.eyeColorPicker.getSelectedColor());
                            this.lastHeldColorPicker = this.eyeColorPicker;
                            return;
                        case 525:
                            this.hairColorPicker.changeHue(1);
                            Game1.player.changeHairColor(this.hairColorPicker.getSelectedColor());
                            this.lastHeldColorPicker = this.hairColorPicker;
                            return;
                        case 526:
                            this.hairColorPicker.changeSaturation(1);
                            Game1.player.changeHairColor(this.hairColorPicker.getSelectedColor());
                            this.lastHeldColorPicker = this.hairColorPicker;
                            return;
                        case 527:
                            this.hairColorPicker.changeValue(1);
                            Game1.player.changeHairColor(this.hairColorPicker.getSelectedColor());
                            this.lastHeldColorPicker = this.hairColorPicker;
                            return;
                        case 528:
                            this.pantsColorPicker.changeHue(1);
                            Game1.player.changePants(this.pantsColorPicker.getSelectedColor());
                            this.lastHeldColorPicker = this.pantsColorPicker;
                            return;
                        case 529:
                            this.pantsColorPicker.changeSaturation(1);
                            Game1.player.changePants(this.pantsColorPicker.getSelectedColor());
                            this.lastHeldColorPicker = this.pantsColorPicker;
                            return;
                        case 530:
                            this.pantsColorPicker.changeValue(1);
                            Game1.player.changePants(this.pantsColorPicker.getSelectedColor());
                            this.lastHeldColorPicker = this.pantsColorPicker;
                            return;
                        default:
                            return;
                    }
                }
                else if (b == Buttons.LeftThumbstickLeft || b == Buttons.DPadLeft)
                {
                    switch (this.currentlySnappedComponent.myID)
                    {
                        case 522:
                            this.eyeColorPicker.changeHue(-1);
                            Game1.player.changeEyeColor(this.eyeColorPicker.getSelectedColor());
                            this.lastHeldColorPicker = this.eyeColorPicker;
                            return;
                        case 523:
                            this.eyeColorPicker.changeSaturation(-1);
                            Game1.player.changeEyeColor(this.eyeColorPicker.getSelectedColor());
                            this.lastHeldColorPicker = this.eyeColorPicker;
                            return;
                        case 524:
                            this.eyeColorPicker.changeValue(-1);
                            Game1.player.changeEyeColor(this.eyeColorPicker.getSelectedColor());
                            this.lastHeldColorPicker = this.eyeColorPicker;
                            return;
                        case 525:
                            this.hairColorPicker.changeHue(-1);
                            Game1.player.changeHairColor(this.hairColorPicker.getSelectedColor());
                            this.lastHeldColorPicker = this.hairColorPicker;
                            return;
                        case 526:
                            this.hairColorPicker.changeSaturation(-1);
                            Game1.player.changeHairColor(this.hairColorPicker.getSelectedColor());
                            this.lastHeldColorPicker = this.hairColorPicker;
                            return;
                        case 527:
                            this.hairColorPicker.changeValue(-1);
                            Game1.player.changeHairColor(this.hairColorPicker.getSelectedColor());
                            this.lastHeldColorPicker = this.hairColorPicker;
                            return;
                        case 528:
                            this.pantsColorPicker.changeHue(-1);
                            Game1.player.changePants(this.pantsColorPicker.getSelectedColor());
                            this.lastHeldColorPicker = this.pantsColorPicker;
                            return;
                        case 529:
                            this.pantsColorPicker.changeSaturation(-1);
                            Game1.player.changePants(this.pantsColorPicker.getSelectedColor());
                            this.lastHeldColorPicker = this.pantsColorPicker;
                            return;
                        case 530:
                            this.pantsColorPicker.changeValue(-1);
                            Game1.player.changePants(this.pantsColorPicker.getSelectedColor());
                            this.lastHeldColorPicker = this.pantsColorPicker;
                            break;
                        default:
                            return;
                    }
                }
            }
        }

        // Token: 0x06001373 RID: 4979 RVA: 0x00134AC0 File Offset: 0x00132CC0
        public override void receiveGamePadButton(Buttons b)
        {
            base.receiveGamePadButton(b);
            if (this.currentlySnappedComponent != null)
            {
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
                            return;
                        default:
                            return;
                    }
                }
                else if (b == Buttons.LeftTrigger)
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
                            this.selectionClick(this.currentlySnappedComponent.name, -1);
                            return;
                        default:
                            return;
                    }
                }
                else if (b == Buttons.B && this.showingCoopHelp)
                {
                    this.receiveLeftClick(this.coopHelpOkButton.bounds.Center.X, this.coopHelpOkButton.bounds.Center.Y, true);
                }
            }
        }

        ////////////////
        //Needs fixing//
        ////////////////
        private void optionButtonClick(string name)
        {
            ///
            if (name.StartsWith("MTN_"))
            {
                if (source == CharacterCustomization.Source.NewGame || source == CharacterCustomization.Source.HostNewFarm)
                {
                    Game1.whichFarm = Memory.getFarmIdByName(name);
                    Game1.spawnMonstersAtNight = false;
                    adjustWhichFarmType(name);
                }
            }
            ///
            else if (name == "Male")
            {
                if (this.source != CharacterCustomization.Source.Wizard)
                {
                    Game1.player.changeGender(true);
                    Game1.player.changeHairStyle(0);
                }
            }
            else if (name == "Wilderness")
            {
                if (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm)
                {
                    Game1.whichFarm = 4;
                    Game1.spawnMonstersAtNight = true;
                    adjustWhichFarmType(name);
                }
            }
            else if (name == "Standard")
            {
                if (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm)
                {
                    Game1.whichFarm = 0;
                    Game1.spawnMonstersAtNight = false;
                    adjustWhichFarmType(name);
                }
            }
            else if (name == "Cat")
            {
                if (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm)
                {
                    Game1.player.catPerson = true;
                }
            }
            else if (name == "Riverland")
            {
                if (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm)
                {
                    Game1.whichFarm = 1;
                    Game1.spawnMonstersAtNight = false;
                    adjustWhichFarmType(name);
                }
            }
            else if (name == "Dog")
            {
                if (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm)
                {
                    Game1.player.catPerson = false;
                }
            }
            else if (name == "Hills")
            {
                if (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm)
                {
                    Game1.whichFarm = 3;
                    Game1.spawnMonstersAtNight = false;
                    adjustWhichFarmType(name);
                }
            }
            else if (name == "Forest")
            {
                if (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm)
                {
                    Game1.whichFarm = 2;
                    Game1.spawnMonstersAtNight = false;
                    adjustWhichFarmType(name);
                }
            }
            else if (name == "OK")
            {
                if (!this.canLeaveMenu())
                {
                    return;
                }
                Game1.player.Name = this.nameBox.Text.Trim();
                Game1.player.displayName = Game1.player.Name;
                Game1.player.favoriteThing.Value = this.favThingBox.Text.Trim();
                Game1.player.isCustomized.Value = true;
                //
                Memory.loadCustomFarmType(Game1.whichFarm);
                //
                if (this.source == CharacterCustomization.Source.HostNewFarm)
                {
                    Game1.multiplayerMode = 2;
                }
                if (Game1.activeClickableMenu is TitleMenu)
                {
                    (Game1.activeClickableMenu as TitleMenu).createdNewCharacter(this.skipIntro);
                }
                else
                {
                    Game1.exitActiveMenu();
                    if (Game1.currentMinigame != null && Game1.currentMinigame is Intro)
                    {
                        (Game1.currentMinigame as Intro).doneCreatingCharacter();
                    }
                    else if (this.source == CharacterCustomization.Source.Wizard)
                    {
                        Game1.flashAlpha = 1f;
                        Game1.playSound("yoba");
                    }
                }
            }
            else if (name == "Female")
            {
                if (this.source != CharacterCustomization.Source.Wizard)
                {
                    Game1.player.changeGender(false);
                    Game1.player.changeHairStyle(16);
                }
            }
            else if (name == "Close")
            {
                Game1.cabinsSeparate = false;
            }
            else if (name == "Separate")
            {
                Game1.cabinsSeparate = true;
            }
            Game1.playSound("coin");
        }

        //More support. ZA WURLDO
        private void adjustWhichFarmType(string name)
        {
            lastClickedFarmTypeBtn = name;
            Memory.updateSelectedFarm(name);
            adjustCabinSettings();
        }

        //Added for support. REEEEE
        private void adjustCabinSettings()
        {
            if (Memory.selectedFarm.cabinCapacity == 0)
            {
                Game1.startingCabins = 0;
                return;
            }
            else if (Memory.selectedFarm.cabinCapacity < Game1.startingCabins)
            {
                Game1.startingCabins = Memory.selectedFarm.cabinCapacity;
            }

            allowCabinsClose = Memory.selectedFarm.allowClose;
            allowCabinsSeperate = Memory.selectedFarm.allowSeperate;
            //Memory.selectedFarm.cabinCapacity;
        }

        ////////////////
        //Needs Fixing//
        ////////////////
        private void selectionClick(string name, int change)
        {
            switch (name)
            {
                case "Cabins":
                    if ((Game1.startingCabins != 0 || change >= 0) && (Game1.startingCabins != Memory.selectedFarm.cabinCapacity || change <= 0))
                        Game1.playSound("axchop");
                    Game1.startingCabins += change;
                    Game1.startingCabins = Math.Max(0, Math.Min(Memory.selectedFarm.cabinCapacity, Game1.startingCabins));
                    break;
                case "Hair":
                    Game1.player.changeHairStyle((int)((NetFieldBase<int, NetInt>)Game1.player.hair) + change);
                    Game1.playSound("grassyStep");
                    break;
                case "Direction":
                    Game1.player.faceDirection((Game1.player.FacingDirection - change + 4) % 4);
                    Game1.player.FarmerSprite.StopAnimation();
                    Game1.player.completelyStopAnimatingOrDoingAction();
                    Game1.playSound("pickUpItem");
                    break;
                case "Difficulty":
                    if (Game1.player.difficultyModifier < 1.0 && change < 0)
                    {
                        Game1.playSound("breathout");
                        Game1.player.difficultyModifier += 0.25f;
                        break;
                    }
                    if (Game1.player.difficultyModifier <= 0.25 || change <= 0)
                        break;
                    Game1.playSound("batFlap");
                    Game1.player.difficultyModifier -= 0.25f;
                    break;
                case "Acc":
                    Game1.player.changeAccessory((int)((NetFieldBase<int, NetInt>)Game1.player.accessory) + change);
                    Game1.playSound("purchase");
                    break;
                case "Skin":
                    Game1.player.changeSkinColor((int)((NetFieldBase<int, NetInt>)Game1.player.skin) + change);
                    Game1.playSound("skeletonStep");
                    break;
                case "Shirt":
                    Game1.player.changeShirt((int)((NetFieldBase<int, NetInt>)Game1.player.shirt) + change);
                    Game1.playSound("coin");
                    break;
            }
        }

        /////////////////////////
        //POINT OF INTERCEPTION//
        /////////////////////////
        //Blow this shit up
        //Need to rework a bit for additional inputs
        //and buttons (Farm Types)
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (this.showingCoopHelp)
            {
                if (this.coopHelpOkButton != null && this.coopHelpOkButton.containsPoint(x, y))
                {
                    this.showingCoopHelp = false;
                    Game1.playSound("bigDeSelect");
                    if (Game1.options.SnappyMenus)
                    {
                        this.currentlySnappedComponent = this.coopHelpButton;
                        this.snapCursorToCurrentSnappedComponent();
                    }
                }
                return;
            }
            foreach (ClickableComponent c in this.genderButtons)
            {
                if (c.containsPoint(x, y))
                {
                    this.optionButtonClick(c.name);
                    c.scale -= 0.5f;
                    c.scale = Math.Max(3.5f, c.scale);
                }
            }
            foreach (ClickableComponent c2 in this.farmTypeButtons)
            {
                if (c2.containsPoint(x, y) && !c2.name.Contains("Gray"))
                {
                    this.optionButtonClick(c2.name);
                    c2.scale -= 0.5f;
                    c2.scale = Math.Max(3.5f, c2.scale);
                }
            }
            foreach (ClickableComponent c3 in this.petButtons)
            {
                if (c3.containsPoint(x, y))
                {
                    this.optionButtonClick(c3.name);
                    c3.scale -= 0.5f;
                    c3.scale = Math.Max(3.5f, c3.scale);
                }
            }
            foreach (ClickableComponent c4 in this.cabinLayoutButtons)
            {
                if (Game1.startingCabins > 0 && c4.containsPoint(x, y))
                {
                    this.optionButtonClick(c4.name);
                    c4.scale -= 0.5f;
                    c4.scale = Math.Max(3.5f, c4.scale);
                }
            }
            foreach (ClickableComponent c5 in this.leftSelectionButtons)
            {
                if (c5.containsPoint(x, y))
                {
                    this.selectionClick(c5.name, -1);
                    if (c5.scale != 0f)
                    {
                        c5.scale -= 0.25f;
                        c5.scale = Math.Max(0.75f, c5.scale);
                    }
                }
            }
            foreach (ClickableComponent c6 in this.rightSelectionButtons)
            {
                if (c6.containsPoint(x, y))
                {
                    this.selectionClick(c6.name, 1);
                    if (c6.scale != 0f)
                    {
                        c6.scale -= 0.25f;
                        c6.scale = Math.Max(0.75f, c6.scale);
                    }
                }
            }

            //Up/Down Arrow Button & NoDebris Button
            if (downArrow.containsPoint(x, y) && currentItemIndex < Math.Max(0, newFarmTypeButtons.Count - 5))
            {
                downArrowPressed();
            }
            else if (upArrow.containsPoint(x, y) && currentItemIndex > 0)
            {
                upArrowPressed();
            } 
            else if (noDebrisButton.containsPoint(x, y))
            {
                Game1.playSound("drumkit6");
                noDebrisButton.sourceRect.X = (noDebrisButton.sourceRect.X == 227) ? 236 : 227;
                Memory.noDebrisRequested = !Memory.noDebrisRequested;
            }
            //Holding the scroll bar
            if (scrollBar.containsPoint(x, y))
            {
                scrolling = true;
            }
            //

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
            if (this.source != CharacterCustomization.Source.Wizard)
            {
                this.nameBox.Update();
                if (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm)
                {
                    this.farmnameBox.Update();
                }
                this.favThingBox.Update();
                if ((this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm) && this.skipIntroButton.containsPoint(x, y))
                {
                    Game1.playSound("drumkit6");
                    this.skipIntroButton.sourceRect.X = ((this.skipIntroButton.sourceRect.X == 227) ? 236 : 227);
                    this.skipIntro = !this.skipIntro;
                }
            }
            if (this.coopHelpButton != null && this.coopHelpButton.containsPoint(x, y))
            {
                if (Game1.options.SnappyMenus)
                {
                    this.currentlySnappedComponent = this.coopHelpOkButton;
                    this.snapCursorToCurrentSnappedComponent();
                }
                Game1.playSound("bigSelect");
                this.showingCoopHelp = true;
                this.coopHelpString = Game1.parseText(Game1.content.LoadString("Strings\\UI:Character_CoopHelpString").Replace("^", Environment.NewLine), Game1.dialogueFont, this.width + 384 - IClickableMenu.borderWidth * 2);
            }
            if (this.randomButton.containsPoint(x, y))
            {
                string sound = "drumkit6";
                if (this.timesRandom > 0)
                {
                    switch (Game1.random.Next(15))
                    {
                        case 0:
                            sound = "drumkit1";
                            break;
                        case 1:
                            sound = "dirtyHit";
                            break;
                        case 2:
                            sound = "axchop";
                            break;
                        case 3:
                            sound = "hoeHit";
                            break;
                        case 4:
                            sound = "fishSlap";
                            break;
                        case 5:
                            sound = "drumkit6";
                            break;
                        case 6:
                            sound = "drumkit5";
                            break;
                        case 7:
                            sound = "drumkit6";
                            break;
                        case 8:
                            sound = "junimoMeep1";
                            break;
                        case 9:
                            sound = "coin";
                            break;
                        case 10:
                            sound = "axe";
                            break;
                        case 11:
                            sound = "hammer";
                            break;
                        case 12:
                            sound = "drumkit2";
                            break;
                        case 13:
                            sound = "drumkit4";
                            break;
                        case 14:
                            sound = "drumkit3";
                            break;
                    }
                }
                Game1.playSound(sound);
                this.timesRandom++;
                if (Game1.random.NextDouble() < 0.33)
                {
                    if (Game1.player.IsMale)
                    {
                        Game1.player.changeAccessory(Game1.random.Next(19));
                    }
                    else
                    {
                        Game1.player.changeAccessory(Game1.random.Next(6, 19));
                    }
                }
                else
                {
                    Game1.player.changeAccessory(-1);
                }
                if (Game1.player.IsMale)
                {
                    Game1.player.changeHairStyle(Game1.random.Next(16));
                }
                else
                {
                    Game1.player.changeHairStyle(Game1.random.Next(16, 32));
                }
                Color hairColor = new Color(Game1.random.Next(25, 254), Game1.random.Next(25, 254), Game1.random.Next(25, 254));
                if (Game1.random.NextDouble() < 0.5)
                {
                    hairColor.R /= 2;
                    hairColor.G /= 2;
                    hairColor.B /= 2;
                }
                if (Game1.random.NextDouble() < 0.5)
                {
                    hairColor.R = (byte)Game1.random.Next(15, 50);
                }
                if (Game1.random.NextDouble() < 0.5)
                {
                    hairColor.G = (byte)Game1.random.Next(15, 50);
                }
                if (Game1.random.NextDouble() < 0.5)
                {
                    hairColor.B = (byte)Game1.random.Next(15, 50);
                }
                Game1.player.changeHairColor(hairColor);
                Game1.player.changeShirt(Game1.random.Next(112));
                Game1.player.changeSkinColor(Game1.random.Next(6));
                if (Game1.random.NextDouble() < 0.25)
                {
                    Game1.player.changeSkinColor(Game1.random.Next(24));
                }
                Color pantsColor = new Color(Game1.random.Next(25, 254), Game1.random.Next(25, 254), Game1.random.Next(25, 254));
                if (Game1.random.NextDouble() < 0.5)
                {
                    pantsColor.R /= 2;
                    pantsColor.G /= 2;
                    pantsColor.B /= 2;
                }
                if (Game1.random.NextDouble() < 0.5)
                {
                    pantsColor.R = (byte)Game1.random.Next(15, 50);
                }
                if (Game1.random.NextDouble() < 0.5)
                {
                    pantsColor.G = (byte)Game1.random.Next(15, 50);
                }
                if (Game1.random.NextDouble() < 0.5)
                {
                    pantsColor.B = (byte)Game1.random.Next(15, 50);
                }
                Game1.player.changePants(pantsColor);
                Color eyeColor = new Color(Game1.random.Next(25, 254), Game1.random.Next(25, 254), Game1.random.Next(25, 254));
                eyeColor.R /= 2;
                eyeColor.G /= 2;
                eyeColor.B /= 2;
                if (Game1.random.NextDouble() < 0.5)
                {
                    eyeColor.R = (byte)Game1.random.Next(15, 50);
                }
                if (Game1.random.NextDouble() < 0.5)
                {
                    eyeColor.G = (byte)Game1.random.Next(15, 50);
                }
                if (Game1.random.NextDouble() < 0.5)
                {
                    eyeColor.B = (byte)Game1.random.Next(15, 50);
                }
                Game1.player.changeEyeColor(eyeColor);
                this.randomButton.scale = 3.5f;
                this.pantsColorPicker.setColor(Game1.player.pantsColor);
                this.eyeColorPicker.setColor(Game1.player.newEyeColor);
                this.hairColorPicker.setColor(Game1.player.hairstyleColor);
            }
        }
        //

        // Token: 0x06001377 RID: 4983 RVA: 0x00135D6C File Offset: 0x00133F6C
        public override void leftClickHeld(int x, int y)
        {
            base.leftClickHeld(x, y);
            //Scrolly bar thing
            if (scrolling)
            {
                int y2 = scrollBar.bounds.Y;
                scrollBar.bounds.Y = Math.Min(yPositionOnScreen + height - 64 - 12 - scrollBar.bounds.Height, Math.Max(y, yPositionOnScreen + upArrow.bounds.Height + 20));
                float percentage = (y - scrollBarRunner.Y) / scrollBarRunner.Height;
                currentItemIndex = Math.Min(newFarmTypeButtons.Count - 5, Math.Max(0, (int)(newFarmTypeButtons.Count * percentage)));
                setScrollBarToCurrentIndex();
                if (y2 != scrollBar.bounds.Y)
                {
                    Game1.playSound("shiny4");
                }
            }

            this.colorPickerTimer -= Game1.currentGameTime.ElapsedGameTime.Milliseconds;
            if (this.colorPickerTimer <= 0)
            {
                if (this.lastHeldColorPicker != null && !Game1.options.SnappyMenus)
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

        // Token: 0x06001378 RID: 4984 RVA: 0x00135E41 File Offset: 0x00134041
        public override void releaseLeftClick(int x, int y)
        {
            base.releaseLeftClick(x, y);
            this.hairColorPicker.releaseClick();
            this.pantsColorPicker.releaseClick();
            this.eyeColorPicker.releaseClick();
            this.lastHeldColorPicker = null;
            scrolling = false;
        }

        // Token: 0x06001379 RID: 4985 RVA: 0x00002A64 File Offset: 0x00000C64
        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
        }

        // Token: 0x0600137A RID: 4986 RVA: 0x00135E6C File Offset: 0x0013406C
        public override void receiveKeyPress(Keys key)
        {
            if (key == Keys.Tab)
            {
                if (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm)
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
                else if (this.source == CharacterCustomization.Source.NewFarmhand)
                {
                    if (this.nameBox.Selected)
                    {
                        this.favThingBox.SelectMe();
                        this.nameBox.Selected = false;
                    }
                    else
                    {
                        this.favThingBox.Selected = false;
                        this.nameBox.SelectMe();
                    }
                }
            }
            if (Game1.options.SnappyMenus && !Game1.options.doesInputListContain(Game1.options.menuButton, key) && Game1.GetKeyboardState().GetPressedKeys().Count<Keys>() == 0)
            {
                base.receiveKeyPress(key);
            }
        }

        // Token: 0x0600137B RID: 4987 RVA: 0x00135F80 File Offset: 0x00134180
        public override void performHoverAction(int x, int y)
        {
            this.hoverText = "";
            this.hoverTitle = "";
            foreach (ClickableComponent clickableComponent in this.leftSelectionButtons)
            {
                ClickableTextureComponent c = (ClickableTextureComponent)clickableComponent;
                if (c.containsPoint(x, y))
                {
                    c.scale = Math.Min(c.scale + 0.02f, c.baseScale + 0.1f);
                }
                else
                {
                    c.scale = Math.Max(c.scale - 0.02f, c.baseScale);
                }
                if (c.name.Equals("Cabins") && Game1.startingCabins == 0)
                {
                    c.scale = 0f;
                }
            }
            foreach (ClickableComponent clickableComponent2 in this.rightSelectionButtons)
            {
                ClickableTextureComponent c2 = (ClickableTextureComponent)clickableComponent2;
                if (c2.containsPoint(x, y))
                {
                    c2.scale = Math.Min(c2.scale + 0.02f, c2.baseScale + 0.1f);
                }
                else
                {
                    c2.scale = Math.Max(c2.scale - 0.02f, c2.baseScale);
                }
                if (c2.name.Equals("Cabins") && Game1.startingCabins == 3)
                {
                    c2.scale = 0f;
                }
            }
            if (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm)
            {
                foreach (ClickableTextureComponent c3 in this.farmTypeButtons)
                {
                    if (c3.containsPoint(x, y) && !c3.name.Contains("Gray"))
                    {
                        c3.scale = Math.Min(c3.scale + 0.02f, c3.baseScale + 0.1f);
                        this.hoverTitle = c3.hoverText.Split(new char[]
                        {
                            '_'
                        })[0];
                        this.hoverText = c3.hoverText.Split(new char[]
                        {
                            '_'
                        })[1];
                    }
                    else
                    {
                        c3.scale = Math.Max(c3.scale - 0.02f, c3.baseScale);
                        if (c3.name.Contains("Gray") && c3.containsPoint(x, y))
                        {
                            this.hoverText = "Reach level 10 " + Game1.content.LoadString("Strings\\UI:Character_" + c3.name.Split(new char[]
                            {
                                '_'
                            })[1]) + " to unlock.";
                        }
                    }
                }
            }
            if (this.source != CharacterCustomization.Source.Wizard)
            {
                foreach (ClickableComponent clickableComponent3 in this.genderButtons)
                {
                    ClickableTextureComponent c4 = (ClickableTextureComponent)clickableComponent3;
                    if (c4.containsPoint(x, y))
                    {
                        c4.scale = Math.Min(c4.scale + 0.05f, c4.baseScale + 0.5f);
                    }
                    else
                    {
                        c4.scale = Math.Max(c4.scale - 0.05f, c4.baseScale);
                    }
                }
            }
            if (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm)
            {
                foreach (ClickableComponent clickableComponent4 in this.petButtons)
                {
                    ClickableTextureComponent c5 = (ClickableTextureComponent)clickableComponent4;
                    if (c5.containsPoint(x, y))
                    {
                        c5.scale = Math.Min(c5.scale + 0.05f, c5.baseScale + 0.5f);
                    }
                    else
                    {
                        c5.scale = Math.Max(c5.scale - 0.05f, c5.baseScale);
                    }
                }
                foreach (ClickableTextureComponent c6 in this.cabinLayoutButtons)
                {
                    if (Game1.startingCabins > 0 && c6.containsPoint(x, y))
                    {
                        c6.scale = Math.Min(c6.scale + 0.05f, c6.baseScale + 0.5f);
                        this.hoverText = c6.hoverText;
                    }
                    else
                    {
                        c6.scale = Math.Max(c6.scale - 0.05f, c6.baseScale);
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
            if (this.coopHelpButton != null)
            {
                if (this.coopHelpButton.containsPoint(x, y))
                {
                    this.coopHelpButton.scale = Math.Min(this.coopHelpButton.scale + 0.05f, this.coopHelpButton.baseScale + 0.5f);
                    this.hoverText = this.coopHelpButton.hoverText;
                }
                else
                {
                    this.coopHelpButton.scale = Math.Max(this.coopHelpButton.scale - 0.05f, this.coopHelpButton.baseScale);
                }
            }
            if (this.coopHelpOkButton != null)
            {
                if (this.coopHelpOkButton.containsPoint(x, y))
                {
                    this.coopHelpOkButton.scale = Math.Min(this.coopHelpOkButton.scale + 0.025f, this.coopHelpOkButton.baseScale + 0.2f);
                }
                else
                {
                    this.coopHelpOkButton.scale = Math.Max(this.coopHelpOkButton.scale - 0.025f, this.coopHelpOkButton.baseScale);
                }
            }
            this.randomButton.tryHover(x, y, 0.25f);
            this.randomButton.tryHover(x, y, 0.25f);
            if (this.hairColorPicker.containsPoint(x, y) || this.pantsColorPicker.containsPoint(x, y) || this.eyeColorPicker.containsPoint(x, y))
            {
                Game1.SetFreeCursorDrag();
            }
            this.nameBox.Hover(x, y);
            this.farmnameBox.Hover(x, y);
            this.favThingBox.Hover(x, y);
            this.skipIntroButton.tryHover(x, y, 0.1f);
        }

        // Token: 0x0600137C RID: 4988 RVA: 0x001366A8 File Offset: 0x001348A8
        public bool canLeaveMenu()
        {
            return this.source == CharacterCustomization.Source.Wizard || (Game1.player.Name.Length > 0 && Game1.player.farmName.Length > 0 && Game1.player.favoriteThing.Length > 0);
        }

        // Token: 0x0600137D RID: 4989 RVA: 0x001366F8 File Offset: 0x001348F8
        private string getNameOfDifficulty()
        {
            if (Game1.player.difficultyModifier < 0.5f)
            {
                return this.superDiffString;
            }
            if (Game1.player.difficultyModifier < 0.75f)
            {
                return this.hardDiffString;
            }
            if (Game1.player.difficultyModifier < 1f)
            {
                return this.toughDiffString;
            }
            return this.normalDiffString;
        }

        /////////////////////////
        //POINT OF INTERCEPTION//
        /////////////////////////
        public override void draw(SpriteBatch b)
        {
            if (this.showingCoopHelp)
            {
                IClickableMenu.drawTextureBox(b, this.xPositionOnScreen - 192, this.yPositionOnScreen + 64, (int)Game1.dialogueFont.MeasureString(this.coopHelpString).X + IClickableMenu.borderWidth * 2, (int)Game1.dialogueFont.MeasureString(this.coopHelpString).Y + IClickableMenu.borderWidth * 2, Color.White);
                Utility.drawTextWithShadow(b, this.coopHelpString, Game1.dialogueFont, new Vector2((float)(this.xPositionOnScreen + IClickableMenu.borderWidth - 192), (float)(this.yPositionOnScreen + IClickableMenu.borderWidth + 64)), Game1.textColor, 1f, -1f, -1, -1, 1f, 3);
                if (this.coopHelpOkButton != null)
                {
                    this.coopHelpOkButton.draw(b, Color.White, 0.95f);
                }
                base.drawMouse(b);
                return;
            }
            Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, false, true, null, false);
            if (this.source == CharacterCustomization.Source.HostNewFarm)
            {
                IClickableMenu.drawTextureBox(b, this.xPositionOnScreen - 256 + 4, this.yPositionOnScreen + IClickableMenu.borderWidth * 2 + 68, 256, 448, Color.White);
                foreach (ClickableTextureComponent c in this.cabinLayoutButtons)
                {
                    //Modified
                    //c.draw(b, Color.White * ((Game1.startingCabins > 0) ? 1f : 0.5f), 0.9f);
                    Single opacity = 0f;
                    if (c.name.Equals("Close"))
                    {
                        opacity = (Game1.startingCabins > 0 && allowCabinsClose) ? 1f : 0.5f;
                    }
                    else
                    {
                        opacity = (Game1.startingCabins > 0 && allowCabinsSeperate) ? 1f : 0.5f;
                    }

                    c.draw(b, Color.White * opacity, 0.9f);

                    if (Game1.startingCabins > 0 && ((c.name.Equals("Close") && !Game1.cabinsSeparate) || (c.name.Equals("Separate") && Game1.cabinsSeparate)))
                    {
                        b.Draw(Game1.mouseCursors, c.bounds, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 34, -1, -1)), Color.White);
                    }
                }
            }
            b.Draw(Game1.daybg, new Vector2((float)(this.xPositionOnScreen + 64 + 42 - 2), (float)(this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 16)), Color.White);
            Game1.player.FarmerRenderer.draw(b, Game1.player.FarmerSprite.CurrentAnimationFrame, Game1.player.FarmerSprite.CurrentFrame, Game1.player.FarmerSprite.SourceRect, new Vector2((float)(this.xPositionOnScreen - 2 + 42 + 128 - 32), (float)(this.yPositionOnScreen + IClickableMenu.borderWidth - 16 + IClickableMenu.spaceToClearTopBorder + 32)), Vector2.Zero, 0.8f, Color.White, 0f, 1f, Game1.player);
            if (this.source != CharacterCustomization.Source.Wizard)
            {
                foreach (ClickableComponent clickableComponent in this.genderButtons)
                {
                    ClickableTextureComponent c2 = (ClickableTextureComponent)clickableComponent;
                    c2.draw(b);
                    if ((c2.name.Equals("Male") && Game1.player.IsMale) || (c2.name.Equals("Female") && !Game1.player.IsMale))
                    {
                        b.Draw(Game1.mouseCursors, c2.bounds, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 34, -1, -1)), Color.White);
                    }
                }
            }
            if (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm)
            {
                foreach (ClickableComponent clickableComponent2 in this.petButtons)
                {
                    ClickableTextureComponent c3 = (ClickableTextureComponent)clickableComponent2;
                    c3.draw(b);
                    if ((c3.name.Equals("Cat") && Game1.player.catPerson) || (c3.name.Equals("Dog") && !Game1.player.catPerson))
                    {
                        b.Draw(Game1.mouseCursors, c3.bounds, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 34, -1, -1)), Color.White);
                    }
                }
            }
            if (this.source != CharacterCustomization.Source.Wizard)
            {
                Game1.player.Name = this.nameBox.Text;
                Game1.player.favoriteThing.Value = this.favThingBox.Text;
            }
            if (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm)
            {
                Game1.player.farmName.Value = this.farmnameBox.Text;
            }
            foreach (ClickableComponent clickableComponent3 in this.leftSelectionButtons)
            {
                ((ClickableTextureComponent)clickableComponent3).draw(b);
            }
            foreach (ClickableComponent c4 in this.labels)
            {
                string sub = "";
                float offset = 0f;
                float subYOffset = 0f;
                Color color = Game1.textColor;
                if (c4 == this.nameLabel)
                {
                    color = ((Game1.player.Name.Length < 1) ? Color.Red : Game1.textColor);
                    if (this.source == CharacterCustomization.Source.Wizard)
                    {
                        continue;
                    }
                }
                else if (c4 == this.farmLabel)
                {
                    color = ((Game1.player.farmName.Length < 1) ? Color.Red : Game1.textColor);
                    if (this.source != CharacterCustomization.Source.NewGame && this.source != CharacterCustomization.Source.HostNewFarm)
                    {
                        continue;
                    }
                }
                else if (c4 == this.favoriteLabel)
                {
                    color = ((Game1.player.favoriteThing.Length < 1) ? Color.Red : Game1.textColor);
                    if (this.source == CharacterCustomization.Source.Wizard)
                    {
                        continue;
                    }
                }
                else if (c4 == this.shirtLabel)
                {
                    offset = 21f - Game1.smallFont.MeasureString(c4.name).X / 2f;
                    sub = string.Concat(Game1.player.shirt + 1);
                }
                else if (c4 == this.skinLabel)
                {
                    offset = 21f - Game1.smallFont.MeasureString(c4.name).X / 2f;
                    sub = string.Concat(Game1.player.skin + 1);
                }
                else if (c4 == this.hairLabel)
                {
                    offset = 21f - Game1.smallFont.MeasureString(c4.name).X / 2f;
                    if (!c4.name.Contains("Color"))
                    {
                        sub = string.Concat(Game1.player.hair + 1);
                    }
                }
                else if (c4 == this.accLabel)
                {
                    offset = 21f - Game1.smallFont.MeasureString(c4.name).X / 2f;
                    sub = string.Concat(Game1.player.accessory + 2);
                }
                else if (c4 == this.startingCabinsLabel)
                {
                    offset = 21f - Game1.smallFont.MeasureString(c4.name).X / 2f;
                    sub = ((Game1.startingCabins == 0 && this.noneString != null) ? this.noneString : string.Concat(Game1.startingCabins));
                    subYOffset = 4f;
                }
                else if (c4 == this.difficultyModifierLabel)
                {
                    offset = 21f - Game1.smallFont.MeasureString(c4.name).X / 2f;
                    subYOffset = 4f;
                    sub = this.getNameOfDifficulty();
                }
                else
                {
                    color = Game1.textColor;
                }
                Utility.drawTextWithShadow(b, c4.name, Game1.smallFont, new Vector2((float)c4.bounds.X + offset, (float)c4.bounds.Y), color, 1f, -1f, -1, -1, 1f, 3);
                if (sub.Length > 0)
                {
                    Utility.drawTextWithShadow(b, sub, Game1.smallFont, new Vector2((float)(c4.bounds.X + 21) - Game1.smallFont.MeasureString(sub).X / 2f, (float)(c4.bounds.Y + 32) + subYOffset), color, 1f, -1f, -1, -1, 1f, 3);
                }
            }
            foreach (ClickableComponent clickableComponent4 in this.rightSelectionButtons)
            {
                ((ClickableTextureComponent)clickableComponent4).draw(b);
            }

            //MODIFICATIONS MADE HERE
            if (source == CharacterCustomization.Source.NewGame || source == CharacterCustomization.Source.HostNewFarm)
            {
                int count = 0;
                Point baseFarmButton = new Point(this.xPositionOnScreen + this.width + 4 + 8, this.yPositionOnScreen + IClickableMenu.borderWidth * 2);
                IClickableMenu.drawTextureBox(b, this.farmTypeButtons[0].bounds.X - 16, this.farmTypeButtons[0].bounds.Y - 20, 120, 476, Color.White);

                farmTypeButtons.Clear();
                while (count < 5)
                {
                    farmTypeButtons.Add(newFarmTypeButtons[currentItemIndex + count]);
                    farmTypeButtons[count].bounds = new Rectangle(baseFarmButton.X, baseFarmButton.Y + menuProperties[count].yOffSet, 88, 80);
                    farmTypeButtons[count].myID = menuProperties[count].ID;
                    farmTypeButtons[count].downNeighborID = menuProperties[count].downNeighborID;
                    farmTypeButtons[count].upNeighborID = menuProperties[count].upNeightborID;
                    farmTypeButtons[count].leftNeighborID = menuProperties[count].leftNeighborID;
                    farmTypeButtons[count].rightNeighborID = menuProperties[count].rightNeighborID;
                    count++;
                }

                for (int i = 0; i < 5; i++)
                {
                    this.farmTypeButtons[i].draw(b, this.farmTypeButtons[i].name.Contains("Gray") ? (Color.Black * 0.5f) : Color.White, 0.88f);
                    if (this.farmTypeButtons[i].name.Contains("Gray"))
                    {
                        b.Draw(Game1.mouseCursors, new Vector2((float)(this.farmTypeButtons[i].bounds.Center.X - 12), (float)(this.farmTypeButtons[i].bounds.Center.Y - 8)), new Rectangle?(new Rectangle(107, 442, 7, 8)), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.89f);
                    }
                    if (farmTypeButtons[i].name == lastClickedFarmTypeBtn)
                    {
                        IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(375, 357, 3, 3), this.farmTypeButtons[i].bounds.X, this.farmTypeButtons[i].bounds.Y - 4, this.farmTypeButtons[i].bounds.Width, this.farmTypeButtons[i].bounds.Height + 8, Color.White, 4f, false);
                    }
                }
            }
            //

            //Scroll Bar and Up/Down Buttons
            upArrow.draw(b);
            downArrow.draw(b);
            scrollBar.draw(b);
            if (newFarmTypeButtons.Count > 5)
            {
                drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 383, 6, 6), scrollBarRunner.X, scrollBarRunner.Y, scrollBarRunner.Width, scrollBarRunner.Height, Color.White, 4f, false);
                scrollBar.draw(b);
            }

            setScrollBarToCurrentIndex();
            //

            //No Debris Button
            //IClickableMenu.drawTextureBox(b, farmTypeButtons[0].bounds.X - 16, farmTypeButtons[4].bounds.Y + 110, 250, 80, Color.White);
            noDebrisButton.draw(b);
            Utility.drawTextWithShadow(b, "No Debris", Game1.smallFont, new Vector2(noDebrisButton.bounds.X + noDebrisButton.bounds.Width - 20, noDebrisButton.bounds.Y + 8), Game1.textColor, 1f, -1f, -1, -1, 1f, 3);
            //

            if (this.canLeaveMenu())
            {
                this.okButton.draw(b, Color.White, 0.75f);
            }
            else
            {
                this.okButton.draw(b, Color.White, 0.75f);
                this.okButton.draw(b, Color.Black * 0.5f, 0.751f);
            }
            if (this.coopHelpButton != null)
            {
                this.coopHelpButton.draw(b, Color.White, 0.75f);
            }
            this.hairColorPicker.draw(b);
            this.pantsColorPicker.draw(b);
            this.eyeColorPicker.draw(b);
            if (this.source != CharacterCustomization.Source.Wizard)
            {
                this.nameBox.Draw(b, true);
                this.favThingBox.Draw(b, true);
            }
            if (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm)
            {
                this.farmnameBox.Draw(b, true);
                if (this.skipIntroButton != null)
                {
                    this.skipIntroButton.draw(b);
                    Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\UI:Character_SkipIntro"), Game1.smallFont, new Vector2((float)(this.skipIntroButton.bounds.X + this.skipIntroButton.bounds.Width + 8), (float)(this.skipIntroButton.bounds.Y + 8)), Game1.textColor, 1f, -1f, -1, -1, 1f, 3);
                }
                Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\UI:Character_FarmNameSuffix"), Game1.smallFont, new Vector2((float)(this.farmnameBox.X + this.farmnameBox.Width + 8), (float)(this.farmnameBox.Y + 12)), Game1.textColor, 1f, -1f, -1, -1, 1f, 3);
            }
            if (this.hoverText != null && this.hoverTitle != null && this.hoverText.Count<char>() > 0)
            {
                IClickableMenu.drawHoverText(b, Game1.parseText(this.hoverText, Game1.smallFont, 256), Game1.smallFont, 0, 0, -1, this.hoverTitle, -1, null, null, 0, -1, -1, -1, -1, 1f, null);
            }
            this.randomButton.draw(b);
            base.drawMouse(b);
        }

        public void setScrollBarToCurrentIndex()
        {
            scrollBar.bounds.Y = scrollBarRunner.Height / Math.Max(1, newFarmTypeButtons.Count - 5 + 1) * currentItemIndex + upArrow.bounds.Bottom + 5;
            if (currentItemIndex != newFarmTypeButtons.Count - 5)
                return;
            scrollBar.bounds.Y = downArrow.bounds.Y - scrollBar.bounds.Height - 30;
        }

        public override void receiveScrollWheelAction(int direction)
        {
            base.receiveScrollWheelAction(direction);
            if (direction > 0 && currentItemIndex > 0)
            {
                upArrowPressed();
                return;
            }
            if (direction < 0 && currentItemIndex < Math.Max(0, newFarmTypeButtons.Count - 5))
            {
                downArrowPressed();
                return;
            }
        }

        private void upArrowPressed()
        {
            upArrow.scale = upArrow.baseScale;
            currentItemIndex--;
            Game1.playSound("shwip");
            setScrollBarToCurrentIndex();
        }

        private void downArrowPressed()
        {
            downArrow.scale = downArrow.baseScale;
            currentItemIndex++;
            Game1.playSound("shwip");
            setScrollBarToCurrentIndex();
        }
    }
}
