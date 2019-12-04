using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewValley.Menus;
using StardewValley.Characters;
using StardewValley.Minigames;
using StardewValley.Objects;
using StardewValley;
using StardewModdingAPI;

namespace MTN2.Menus {
    public class CharacterCustomizationMTN2 : IClickableMenu {
        public List<ClickableComponent> labels = new List<ClickableComponent>();
        public List<ClickableComponent> leftSelectionButtons = new List<ClickableComponent>();
        public List<ClickableComponent> rightSelectionButtons = new List<ClickableComponent>();
        public List<ClickableComponent> genderButtons = new List<ClickableComponent>();
        public List<ClickableComponent> petButtons = new List<ClickableComponent>();
        public List<ClickableTextureComponent> farmTypeButtons = new List<ClickableTextureComponent>();
        public List<ClickableComponent> colorPickerCCs = new List<ClickableComponent>();
        public List<ClickableTextureComponent> cabinLayoutButtons = new List<ClickableTextureComponent>();
        protected bool _shouldShowBackButton = true;
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
        public const int region_petLeft = 627;
        public const int region_petRight = 628;
        public const int region_pantsLeft = 629;
        public const int region_pantsRight = 630;
        public const int region_walletsLeft = 631;
        public const int region_walletsRight = 632;
        public const int region_coopHelpRight = 633;
        public const int region_coopHelpLeft = 634;
        public const int region_coopHelpButtons = 635;
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
        public const int region_farmSelection6 = 545;
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
        private int currentPet;
        public ColorPicker pantsColorPicker;
        public ColorPicker hairColorPicker;
        public ColorPicker eyeColorPicker;
        public ClickableTextureComponent okButton;
        public ClickableTextureComponent skipIntroButton;
        public ClickableTextureComponent randomButton;
        public ClickableTextureComponent coopHelpButton;
        public ClickableTextureComponent coopHelpOkButton;
        public ClickableTextureComponent coopHelpRightButton;
        public ClickableTextureComponent coopHelpLeftButton;
        private TextBox nameBox;
        private TextBox farmnameBox;
        private TextBox favThingBox;
        private bool skipIntro;
        public bool isModifyingExistingPet;
        public bool showingCoopHelp;
        public int coopHelpScreen;
        public CharacterCustomization.Source source;
        private Vector2 helpStringSize;
        private string hoverText;
        private string hoverTitle;
        private string coopHelpString;
        private string noneString;
        private string normalDiffString;
        private string toughDiffString;
        private string hardDiffString;
        private string superDiffString;
        private string sharedWalletString;
        private string separateWalletString;
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
        private ClickableComponent pantsStyleLabel;
        private ClickableComponent startingCabinsLabel;
        private ClickableComponent cabinLayoutLabel;
        private ClickableComponent separateWalletLabel;
        private ClickableComponent difficultyModifierLabel;
        private ColorPicker _sliderOpTarget;
        private Action _sliderAction;
        private readonly Action _recolorEyesAction;
        private readonly Action _recolorPantsAction;
        private readonly Action _recolorHairAction;
        protected Clothing _itemToDye;
        protected bool _isDyeMenu;
        protected Farmer _displayFarmer;
        public Rectangle portraitBox;
        public Rectangle? petPortraitBox;
        private ColorPicker lastHeldColorPicker;
        private int timesRandom;

        // MTN Fields
        private readonly ICustomManager customManager;
        private readonly IMonitor monitor;
        private readonly Multiplayer multiplayer;

        protected List<ClickableTextureComponent> allFarmButtons = new List<ClickableTextureComponent>();

        public ClickableTextureComponent noDebrisButton;
        public ClickableTextureComponent upArrow;
        public ClickableTextureComponent downArrow;
        public ClickableTextureComponent scrollBar;

        public Rectangle scrollBarRunner;

        public int currentItemIndex;
        public int previousItemIndex;

        public string lastClickedFarmTypeBtn = "Standard";
        public bool allowCabinsSeperate = true;
        public bool allowCabinsClose = true;
        public bool scrolling = false;

        // End MTN Fields

        public CharacterCustomizationMTN2(Clothing item)
            : this(null, null, null, CharacterCustomization.Source.ClothesDye) {
            this._itemToDye = item;
            this.setUpPositions();
            this._recolorPantsAction = (Action)(() => this.DyeItem(this.pantsColorPicker.getSelectedColor()));
            if (this._itemToDye.clothesType.Value == 0)
                this._displayFarmer.shirtItem.Set(this._itemToDye);
            else if (this._itemToDye.clothesType.Value == 1)
                this._displayFarmer.pantsItem.Set(this._itemToDye);
            this._displayFarmer.UpdateClothing();
        }

        public void DyeItem(Color color) {
            if (this._itemToDye == null)
                return;
            this._itemToDye.Dye(color, 1f);
            this._displayFarmer.FarmerRenderer.MarkSpriteDirty();
        }

        public CharacterCustomizationMTN2(ICustomManager customManager, IMonitor monitor, Multiplayer multiplayer, CharacterCustomization.Source source)
            : base(Game1.viewport.Width / 2 - (632 + IClickableMenu.borderWidth * 2) / 2, Game1.viewport.Height / 2 - (648 + IClickableMenu.borderWidth * 2) / 2 - 64, 632 + IClickableMenu.borderWidth * 2, 648 + IClickableMenu.borderWidth * 2 + 64, false) {
            this.customManager = customManager;
            this.monitor = monitor;
            this.multiplayer = multiplayer;

            int num = 0;
            if (source == CharacterCustomization.Source.ClothesDye || source == CharacterCustomization.Source.DyePots) {
                this._isDyeMenu = true;
                switch (source) {
                    case CharacterCustomization.Source.ClothesDye:
                        num = 1;
                        break;
                    case CharacterCustomization.Source.DyePots:
                        if (Game1.player.pantsItem.Value != null && Game1.player.pantsItem.Value.dyeable.Value)
                            ++num;
                        if (Game1.player.shirtItem.Value != null && Game1.player.shirtItem.Value.dyeable.Value) {
                            ++num;
                            break;
                        }
                        break;
                }
                this.height = 308 + IClickableMenu.borderWidth * 2 + 64 + 72 * num - 4;
                this.xPositionOnScreen = Game1.viewport.Width / 2 - this.width / 2;
                this.yPositionOnScreen = Game1.viewport.Height / 2 - this.height / 2 - 64;
            }
            if (source == CharacterCustomization.Source.Wizard) {
                this.height = 540 + IClickableMenu.borderWidth * 2 + 64 + 72 * num - 4;
                this.yPositionOnScreen = Game1.viewport.Height / 2 - this.height / 2 - 64;
            }
            this.shirtOptions = new List<int>()
            {
                0,
                1,
                2,
                3,
                4,
                5
            };
            this.hairStyleOptions = new List<int>()
            {
                0,
                1,
                2,
                3,
                4,
                5
            };
            this.accessoryOptions = new List<int>()
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
            this._recolorEyesAction = (Action)(() => Game1.player.changeEyeColor(this.eyeColorPicker.getSelectedColor()));
            this._recolorPantsAction = (Action)(() => Game1.player.changePants(this.pantsColorPicker.getSelectedColor()));
            this._recolorHairAction = (Action)(() => Game1.player.changeHairColor(this.hairColorPicker.getSelectedColor()));
            if (source == CharacterCustomization.Source.DyePots) {
                this._recolorHairAction = (Action)(() => {
                    if (Game1.player.shirtItem.Value == null || !Game1.player.shirtItem.Value.dyeable.Value)
                        return;
                    Game1.player.shirtItem.Value.clothesColor.Value = this.hairColorPicker.getSelectedColor();
                    Game1.player.FarmerRenderer.MarkSpriteDirty();
                    this._displayFarmer.FarmerRenderer.MarkSpriteDirty();
                });
                this._recolorPantsAction = (Action)(() => {
                    if (Game1.player.pantsItem.Value == null || !Game1.player.pantsItem.Value.dyeable.Value)
                        return;
                    Game1.player.pantsItem.Value.clothesColor.Value = this.pantsColorPicker.getSelectedColor();
                    Game1.player.FarmerRenderer.MarkSpriteDirty();
                    this._displayFarmer.FarmerRenderer.MarkSpriteDirty();
                });
                this.favThingBoxCC.visible = false;
                this.nameBoxCC.visible = false;
                this.farmnameBoxCC.visible = false;
                this.favoriteLabel.visible = false;
                this.nameLabel.visible = false;
                this.farmLabel.visible = false;
            }
            this._displayFarmer = this.GetOrCreateDisplayFarmer();
        }

        public Farmer GetOrCreateDisplayFarmer() {
            if (this._displayFarmer == null) {
                this._displayFarmer = this.source == CharacterCustomization.Source.ClothesDye || this.source == CharacterCustomization.Source.DyePots ? Game1.player.CreateFakeEventFarmer() : Game1.player;
                if (this.source == CharacterCustomization.Source.NewFarmhand) {
                    if (this._displayFarmer.pants.Value == -1)
                        this._displayFarmer.pants.Value = this._displayFarmer.GetPantsIndex();
                    if (this._displayFarmer.shirt.Value == -1)
                        this._displayFarmer.shirt.Value = this._displayFarmer.GetShirtIndex();
                }
                this._displayFarmer.faceDirection(2);
                this._displayFarmer.FarmerSprite.StopAnimation();
            }
            return this._displayFarmer;
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds) {
            base.gameWindowSizeChanged(oldBounds, newBounds);
            if (this._isDyeMenu) {
                this.xPositionOnScreen = Game1.viewport.Width / 2 - this.width / 2;
                this.yPositionOnScreen = Game1.viewport.Height / 2 - this.height / 2 - 64;
            } else {
                this.xPositionOnScreen = Game1.viewport.Width / 2 - (632 + IClickableMenu.borderWidth * 2) / 2;
                this.yPositionOnScreen = Game1.viewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2 - 64;
            }
            this.setUpPositions();
        }

        private void setUpPositions() {
            if (this.source == CharacterCustomization.Source.ClothesDye && this._itemToDye == null)
                return;
            bool flag1 = true;
            bool flag2 = true;
            if (this.source == CharacterCustomization.Source.Wizard || this.source == CharacterCustomization.Source.ClothesDye || this.source == CharacterCustomization.Source.DyePots)
                flag2 = false;
            if (this.source == CharacterCustomization.Source.ClothesDye || this.source == CharacterCustomization.Source.DyePots)
                flag1 = false;
            this.labels.Clear();
            this.petButtons.Clear();
            this.genderButtons.Clear();
            this.cabinLayoutButtons.Clear();
            this.leftSelectionButtons.Clear();
            this.rightSelectionButtons.Clear();
            this.farmTypeButtons.Clear();
            this.allFarmButtons.Clear();

            ClickableTextureComponent textureComponent1 = new ClickableTextureComponent("OK", new Rectangle(this.xPositionOnScreen + this.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - 64, this.yPositionOnScreen + this.height - IClickableMenu.borderWidth - IClickableMenu.spaceToClearTopBorder + 16, 64, 64), (string)null, (string)null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46, -1, -1), 1f, false);
            textureComponent1.myID = 505;
            textureComponent1.upNeighborID = -99998;
            textureComponent1.leftNeighborID = -99998;
            textureComponent1.rightNeighborID = -99998;
            textureComponent1.downNeighborID = -99998;
            this.okButton = textureComponent1;
            this.backButton = new ClickableComponent(new Rectangle(Game1.viewport.Width - 198 - 48, Game1.viewport.Height - 81 - 24, 198, 81), "") {
                myID = 81114,
                upNeighborID = -99998,
                leftNeighborID = -99998,
                rightNeighborID = -99998,
                downNeighborID = -99998
            };
            this.nameBox = new TextBox(Game1.content.Load<Texture2D>("LooseSprites\\textBox"), (Texture2D)null, Game1.smallFont, Game1.textColor) {
                X = this.xPositionOnScreen + 64 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 256,
                Y = this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 16,
                Text = Game1.player.Name
            };
            this.nameBoxCC = new ClickableComponent(new Rectangle(this.xPositionOnScreen + 64 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 256, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 16, 192, 48), "") {
                myID = 536,
                upNeighborID = -99998,
                leftNeighborID = -99998,
                rightNeighborID = -99998,
                downNeighborID = -99998
            };
            int num1 = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ru || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.es || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.pt ? -4 : 0;
            this.labels.Add(this.nameLabel = new ClickableComponent(new Rectangle(this.xPositionOnScreen + num1 + 16 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 192 + 4, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 8, 1, 1), Game1.content.LoadString("Strings\\UI:Character_Name")));
            this.farmnameBox = new TextBox(Game1.content.Load<Texture2D>("LooseSprites\\textBox"), (Texture2D)null, Game1.smallFont, Game1.textColor) {
                X = this.xPositionOnScreen + 64 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 256,
                Y = this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 16 + 64,
                Text = (string)((NetFieldBase<string, NetString>)Game1.MasterPlayer.farmName)
            };
            this.farmnameBoxCC = new ClickableComponent(new Rectangle(this.xPositionOnScreen + 64 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 256, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 16 + 64, 192, 48), "") {
                myID = 537,
                upNeighborID = -99998,
                leftNeighborID = -99998,
                rightNeighborID = -99998,
                downNeighborID = -99998
            };
            int num2 = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko ? -16 : 0;
            this.labels.Add(this.farmLabel = new ClickableComponent(new Rectangle(this.xPositionOnScreen + num1 * 3 + 16 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 192 + 4 + num2, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 16 + 64, 1, 1), Game1.content.LoadString("Strings\\UI:Character_Farm")));
            int num3 = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko ? 48 : 0;
            this.favThingBox = new TextBox(Game1.content.Load<Texture2D>("LooseSprites\\textBox"), (Texture2D)null, Game1.smallFont, Game1.textColor) {
                X = this.xPositionOnScreen + 64 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 256 + num3,
                Y = this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 16 + 128,
                Text = (string)((NetFieldBase<string, NetString>)Game1.player.favoriteThing)
            };
            this.favThingBoxCC = new ClickableComponent(new Rectangle(this.xPositionOnScreen + 64 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 256, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 16 + 128, 192, 48), "") {
                myID = 538,
                upNeighborID = -99998,
                leftNeighborID = -99998,
                rightNeighborID = -99998,
                downNeighborID = -99998
            };
            this.labels.Add(this.favoriteLabel = new ClickableComponent(new Rectangle(this.xPositionOnScreen + num1 + 16 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 192 + 4, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 16 + 128, 1, 1), Game1.content.LoadString("Strings\\UI:Character_FavoriteThing")));
            ClickableTextureComponent textureComponent2 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + 48, this.yPositionOnScreen + 64 + 56, 40, 40), Game1.mouseCursors, new Rectangle(381, 361, 10, 10), 4f, false);
            textureComponent2.myID = 507;
            textureComponent2.upNeighborID = -99998;
            textureComponent2.leftNeighborID = -99998;
            textureComponent2.rightNeighborID = -99998;
            textureComponent2.downNeighborID = -99998;
            this.randomButton = textureComponent2;
            if (this.source == CharacterCustomization.Source.DyePots || this.source == CharacterCustomization.Source.ClothesDye)
                this.randomButton.visible = false;
            this.portraitBox = new Rectangle(this.xPositionOnScreen + 64 + 42 - 2, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 16, 128, 192);
            if (this._isDyeMenu || this.source == CharacterCustomization.Source.Wizard) {
                this.portraitBox.X = this.xPositionOnScreen + (this.width - this.portraitBox.Width) / 2;
                this.randomButton.bounds.X = this.portraitBox.X - 56;
            }
            int num4 = 128;
            List<ClickableComponent> selectionButtons1 = this.leftSelectionButtons;
            ClickableTextureComponent textureComponent3 = new ClickableTextureComponent("Direction", new Rectangle(this.portraitBox.X - 32, this.portraitBox.Y + 144, 64, 64), (string)null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44, -1, -1), 1f, false);
            textureComponent3.myID = 520;
            textureComponent3.upNeighborID = -99998;
            textureComponent3.leftNeighborID = -99998;
            textureComponent3.rightNeighborID = -99998;
            textureComponent3.downNeighborID = -99998;
            selectionButtons1.Add((ClickableComponent)textureComponent3);
            List<ClickableComponent> selectionButtons2 = this.rightSelectionButtons;
            ClickableTextureComponent textureComponent4 = new ClickableTextureComponent("Direction", new Rectangle(this.portraitBox.Right - 32, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num4, 64, 64), (string)null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33, -1, -1), 1f, false);
            textureComponent4.myID = 521;
            textureComponent4.upNeighborID = -99998;
            textureComponent4.leftNeighborID = -99998;
            textureComponent4.rightNeighborID = -99998;
            textureComponent4.downNeighborID = -99998;
            selectionButtons2.Add((ClickableComponent)textureComponent4);
            int num5 = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ru || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.es || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.pt ? -20 : 0;
            this.isModifyingExistingPet = false;
            if (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm) {
                this.petPortraitBox = new Rectangle?(new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 448 - 16 + (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ru ? 60 : 0), this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 192 - 16, 64, 64));
                this.labels.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + 16 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 192 + 8 + num1, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 8 + 192, 1, 1), Game1.content.LoadString("Strings\\UI:Character_Animal")));
            }
            if (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm || (this.source == CharacterCustomization.Source.NewFarmhand || this.source == CharacterCustomization.Source.Wizard)) {
                if (this.source != CharacterCustomization.Source.Wizard) {
                    List<ClickableComponent> genderButtons1 = this.genderButtons;
                    ClickableTextureComponent textureComponent5 = new ClickableTextureComponent("Male", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 32 + 8, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 192, 64, 64), (string)null, "Male", Game1.mouseCursors, new Rectangle(128, 192, 16, 16), 4f, false);
                    textureComponent5.myID = 508;
                    textureComponent5.upNeighborID = -99998;
                    textureComponent5.leftNeighborID = -99998;
                    textureComponent5.rightNeighborID = -99998;
                    textureComponent5.downNeighborID = -99998;
                    genderButtons1.Add((ClickableComponent)textureComponent5);
                    List<ClickableComponent> genderButtons2 = this.genderButtons;
                    ClickableTextureComponent textureComponent6 = new ClickableTextureComponent("Female", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 32 + 64 + 24, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 192, 64, 64), (string)null, "Female", Game1.mouseCursors, new Rectangle(144, 192, 16, 16), 4f, false);
                    textureComponent6.myID = 509;
                    textureComponent6.upNeighborID = -99998;
                    textureComponent6.leftNeighborID = -99998;
                    textureComponent6.rightNeighborID = -99998;
                    textureComponent6.downNeighborID = -99998;
                    genderButtons2.Add((ClickableComponent)textureComponent6);
                }
                num4 = 256;
                if (this.source == CharacterCustomization.Source.Wizard)
                    num4 = 192;
                num5 = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ru || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.es || (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.pt || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.fr) ? -20 : 0;
                List<ClickableComponent> selectionButtons3 = this.leftSelectionButtons;
                ClickableTextureComponent textureComponent7 = new ClickableTextureComponent("Skin", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 16 + num5, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num4, 64, 64), (string)null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44, -1, -1), 1f, false);
                textureComponent7.myID = 518;
                textureComponent7.upNeighborID = -99998;
                textureComponent7.leftNeighborID = -99998;
                textureComponent7.rightNeighborID = -99998;
                textureComponent7.downNeighborID = -99998;
                selectionButtons3.Add((ClickableComponent)textureComponent7);
                this.labels.Add(this.skinLabel = new ClickableComponent(new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 16 + 64 + 8 + num5 / 2, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num4 + 16, 1, 1), Game1.content.LoadString("Strings\\UI:Character_Skin")));
                List<ClickableComponent> selectionButtons4 = this.rightSelectionButtons;
                ClickableTextureComponent textureComponent8 = new ClickableTextureComponent("Skin", new Rectangle(this.xPositionOnScreen + 16 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 128, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num4, 64, 64), (string)null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33, -1, -1), 1f, false);
                textureComponent8.myID = 519;
                textureComponent8.upNeighborID = -99998;
                textureComponent8.leftNeighborID = -99998;
                textureComponent8.rightNeighborID = -99998;
                textureComponent8.downNeighborID = -99998;
                selectionButtons4.Add((ClickableComponent)textureComponent8);
            }

            // Farm Types/Map Buttons
            // MTN customs are generated within this if statements.
            if (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm) {
                Game1.startingCabins = 0;
                if (this.source == CharacterCustomization.Source.HostNewFarm)
                    Game1.startingCabins = 1;
                Game1.player.difficultyModifier = 1f;
                Game1.player.team.useSeparateWallets.Value = false;
                Point point = new Point(this.xPositionOnScreen + this.width + 4 + 8, this.yPositionOnScreen + IClickableMenu.borderWidth);
                List<ClickableTextureComponent> farmTypeButtons1 = this.farmTypeButtons;
                ClickableTextureComponent textureComponent5 = new ClickableTextureComponent("Standard", new Rectangle(point.X, point.Y + 88, 88, 80), (string)null, Game1.content.LoadString("Strings\\UI:Character_FarmStandard"), Game1.mouseCursors, new Rectangle(0, 324, 22, 20), 4f, false);
                textureComponent5.myID = 531;
                textureComponent5.downNeighborID = 532;
                textureComponent5.leftNeighborID = 537;
                farmTypeButtons1.Add(textureComponent5);
                List<ClickableTextureComponent> farmTypeButtons2 = this.farmTypeButtons;
                ClickableTextureComponent textureComponent6 = new ClickableTextureComponent("Riverland", new Rectangle(point.X, point.Y + 176, 88, 80), (string)null, Game1.content.LoadString("Strings\\UI:Character_FarmFishing"), Game1.mouseCursors, new Rectangle(22, 324, 22, 20), 4f, false);
                textureComponent6.myID = 532;
                textureComponent6.upNeighborID = -99998;
                textureComponent6.leftNeighborID = -99998;
                textureComponent6.rightNeighborID = -99998;
                textureComponent6.downNeighborID = -99998;
                farmTypeButtons2.Add(textureComponent6);
                List<ClickableTextureComponent> farmTypeButtons3 = this.farmTypeButtons;
                ClickableTextureComponent textureComponent7 = new ClickableTextureComponent("Forest", new Rectangle(point.X, point.Y + 264, 88, 80), (string)null, Game1.content.LoadString("Strings\\UI:Character_FarmForaging"), Game1.mouseCursors, new Rectangle(44, 324, 22, 20), 4f, false);
                textureComponent7.myID = 533;
                textureComponent7.upNeighborID = -99998;
                textureComponent7.leftNeighborID = -99998;
                textureComponent7.rightNeighborID = -99998;
                textureComponent7.downNeighborID = -99998;
                farmTypeButtons3.Add(textureComponent7);
                List<ClickableTextureComponent> farmTypeButtons4 = this.farmTypeButtons;
                ClickableTextureComponent textureComponent8 = new ClickableTextureComponent("Hills", new Rectangle(point.X, point.Y + 352, 88, 80), (string)null, Game1.content.LoadString("Strings\\UI:Character_FarmMining"), Game1.mouseCursors, new Rectangle(66, 324, 22, 20), 4f, false);
                textureComponent8.myID = 534;
                textureComponent8.upNeighborID = -99998;
                textureComponent8.leftNeighborID = -99998;
                textureComponent8.rightNeighborID = -99998;
                textureComponent8.downNeighborID = -99998;
                farmTypeButtons4.Add(textureComponent8);
                List<ClickableTextureComponent> farmTypeButtons5 = this.farmTypeButtons;
                ClickableTextureComponent textureComponent9 = new ClickableTextureComponent("Wilderness", new Rectangle(point.X, point.Y + 440, 88, 80), (string)null, Game1.content.LoadString("Strings\\UI:Character_FarmCombat"), Game1.mouseCursors, new Rectangle(88, 324, 22, 20), 4f, false);
                textureComponent9.myID = 535;
                textureComponent9.upNeighborID = -99998;
                textureComponent9.leftNeighborID = -99998;
                textureComponent9.rightNeighborID = -99998;
                textureComponent9.downNeighborID = -99998;
                farmTypeButtons5.Add(textureComponent9);
                List<ClickableTextureComponent> farmTypeButtons6 = this.farmTypeButtons;
                ClickableTextureComponent textureComponent10 = new ClickableTextureComponent("Four Corners", new Rectangle(point.X, point.Y + 528, 88, 80), (string)null, Game1.content.LoadString("Strings\\UI:Character_FarmFourCorners"), Game1.mouseCursors, new Rectangle(0, 345, 22, 20), 4f, false);
                textureComponent10.myID = 545;
                textureComponent10.upNeighborID = -99998;
                textureComponent10.leftNeighborID = -99998;
                textureComponent10.rightNeighborID = -99998;
                textureComponent10.downNeighborID = -99998;
                farmTypeButtons6.Add(textureComponent10);

                // The Custom Farm buttons. Added to farmTypeButtons
                foreach (ClickableTextureComponent farmButton in this.farmTypeButtons) {
                    this.allFarmButtons.Add(farmButton);
                }

                foreach (CustomFarm customFarm in customManager?.FarmList) {
                    this.farmTypeButtons.Add(new ClickableTextureComponent("MTN_" + customFarm.Name, new Rectangle(point.X, point.Y + 440, 88, 80), null,
                        customFarm.DescriptionName + "_" + customFarm.DescriptionDetails, customFarm.IconSource, new Rectangle(0, 0, 22, 20), 4f, false));
                }

                // Set up scroll bar / arrow buttons
                this.upArrow = new ClickableTextureComponent(new Rectangle(point.X + 115, point.Y + 75, 44, 48), Game1.mouseCursors, new Rectangle(421, 459, 11, 12), 4f, false);
                this.downArrow = new ClickableTextureComponent(new Rectangle(point.X + 115, point.Y + 500, 44, 48), Game1.mouseCursors, new Rectangle(421, 472, 11, 12), 4f, false);
                this.scrollBar = new ClickableTextureComponent(new Rectangle(upArrow.bounds.X + 11, upArrow.bounds.Y + upArrow.bounds.Height + 4, 24, 20), Game1.mouseCursors, new Rectangle(435, 463, 6, 10), 4f, false);
                this.scrollBarRunner = new Rectangle(scrollBar.bounds.X, upArrow.bounds.Y + upArrow.bounds.Height + 4, scrollBar.bounds.Width, height - upArrow.bounds.Height - 332);
            }
            if (this.source == CharacterCustomization.Source.HostNewFarm) {
                this.labels.Add(this.startingCabinsLabel = new ClickableComponent(new Rectangle(this.xPositionOnScreen - 21 - 128, this.yPositionOnScreen + IClickableMenu.borderWidth * 2 + 84, 1, 1), Game1.content.LoadString("Strings\\UI:Character_StartingCabins")));
                List<ClickableComponent> selectionButtons3 = this.leftSelectionButtons;
                ClickableTextureComponent textureComponent5 = new ClickableTextureComponent("Cabins", new Rectangle(this.xPositionOnScreen - 256 + IClickableMenu.borderWidth / 2 + 8, this.yPositionOnScreen + IClickableMenu.borderWidth * 2 + 108, 64, 64), (string)null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44, -1, -1), 1f, false);
                textureComponent5.myID = 621;
                textureComponent5.upNeighborID = -99998;
                textureComponent5.leftNeighborID = -99998;
                textureComponent5.rightNeighborID = -99998;
                textureComponent5.downNeighborID = -99998;
                selectionButtons3.Add((ClickableComponent)textureComponent5);
                List<ClickableComponent> selectionButtons4 = this.rightSelectionButtons;
                ClickableTextureComponent textureComponent6 = new ClickableTextureComponent("Cabins", new Rectangle(this.xPositionOnScreen - 256 + IClickableMenu.borderWidth + 128 + 8, this.yPositionOnScreen + IClickableMenu.borderWidth * 2 + 108, 64, 64), (string)null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33, -1, -1), 1f, false);
                textureComponent6.myID = 622;
                textureComponent6.upNeighborID = -99998;
                textureComponent6.leftNeighborID = -99998;
                textureComponent6.rightNeighborID = -99998;
                textureComponent6.downNeighborID = -99998;
                selectionButtons4.Add((ClickableComponent)textureComponent6);
                this.labels.Add(this.cabinLayoutLabel = new ClickableComponent(new Rectangle(this.xPositionOnScreen - 128 - (int)((double)Game1.smallFont.MeasureString(Game1.content.LoadString("Strings\\UI:Character_CabinLayout")).X / 2.0), this.yPositionOnScreen + IClickableMenu.borderWidth * 2 + 120 + 64, 1, 1), Game1.content.LoadString("Strings\\UI:Character_CabinLayout")));
                List<ClickableTextureComponent> cabinLayoutButtons1 = this.cabinLayoutButtons;
                ClickableTextureComponent textureComponent7 = new ClickableTextureComponent("Close", new Rectangle(this.xPositionOnScreen - 256 + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.borderWidth * 2 + 160 + 64, 64, 64), (string)null, Game1.content.LoadString("Strings\\UI:Character_Close"), Game1.mouseCursors, new Rectangle(208, 192, 16, 16), 4f, false);
                textureComponent7.myID = 623;
                textureComponent7.upNeighborID = -99998;
                textureComponent7.leftNeighborID = -99998;
                textureComponent7.rightNeighborID = -99998;
                textureComponent7.downNeighborID = -99998;
                cabinLayoutButtons1.Add(textureComponent7);
                List<ClickableTextureComponent> cabinLayoutButtons2 = this.cabinLayoutButtons;
                ClickableTextureComponent textureComponent8 = new ClickableTextureComponent("Separate", new Rectangle(this.xPositionOnScreen - 256 + IClickableMenu.borderWidth + 128 - 8, this.yPositionOnScreen + IClickableMenu.borderWidth * 2 + 160 + 64, 64, 64), (string)null, Game1.content.LoadString("Strings\\UI:Character_Separate"), Game1.mouseCursors, new Rectangle(224, 192, 16, 16), 4f, false);
                textureComponent8.myID = 624;
                textureComponent8.upNeighborID = -99998;
                textureComponent8.leftNeighborID = -99998;
                textureComponent8.rightNeighborID = -99998;
                textureComponent8.downNeighborID = -99998;
                cabinLayoutButtons2.Add(textureComponent8);
                this.labels.Add(this.difficultyModifierLabel = new ClickableComponent(new Rectangle(this.xPositionOnScreen - 21 - 128, this.yPositionOnScreen + IClickableMenu.borderWidth * 2 + 256 + 56, 1, 1), Game1.content.LoadString("Strings\\UI:Character_Difficulty")));
                List<ClickableComponent> selectionButtons5 = this.leftSelectionButtons;
                ClickableTextureComponent textureComponent9 = new ClickableTextureComponent("Difficulty", new Rectangle(this.xPositionOnScreen - 256 + IClickableMenu.borderWidth / 2 - 4, this.yPositionOnScreen + IClickableMenu.borderWidth * 2 + 256 + 80, 64, 64), (string)null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44, -1, -1), 1f, false);
                textureComponent9.myID = 627;
                textureComponent9.upNeighborID = -99998;
                textureComponent9.leftNeighborID = -99998;
                textureComponent9.rightNeighborID = -99998;
                textureComponent9.downNeighborID = -99998;
                selectionButtons5.Add((ClickableComponent)textureComponent9);
                List<ClickableComponent> selectionButtons6 = this.rightSelectionButtons;
                ClickableTextureComponent textureComponent10 = new ClickableTextureComponent("Difficulty", new Rectangle(this.xPositionOnScreen - 256 + IClickableMenu.borderWidth + 128 + 12, this.yPositionOnScreen + IClickableMenu.borderWidth * 2 + 256 + 80, 64, 64), (string)null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33, -1, -1), 1f, false);
                textureComponent10.myID = 628;
                textureComponent10.upNeighborID = -99998;
                textureComponent10.leftNeighborID = -99998;
                textureComponent10.rightNeighborID = -99998;
                textureComponent10.downNeighborID = -99998;
                selectionButtons6.Add((ClickableComponent)textureComponent10);
                int y = this.yPositionOnScreen + IClickableMenu.borderWidth * 2 + 320 + 100;
                this.labels.Add(this.separateWalletLabel = new ClickableComponent(new Rectangle(this.xPositionOnScreen - 21 - 128, y - 24, 1, 1), Game1.content.LoadString("Strings\\UI:Character_Wallets")));
                List<ClickableComponent> selectionButtons7 = this.leftSelectionButtons;
                ClickableTextureComponent textureComponent11 = new ClickableTextureComponent("Wallets", new Rectangle(this.xPositionOnScreen - 256 + IClickableMenu.borderWidth / 2 - 4, y, 64, 64), (string)null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44, -1, -1), 1f, false);
                textureComponent11.myID = 631;
                textureComponent11.upNeighborID = -99998;
                textureComponent11.leftNeighborID = -99998;
                textureComponent11.rightNeighborID = -99998;
                textureComponent11.downNeighborID = -99998;
                selectionButtons7.Add((ClickableComponent)textureComponent11);
                List<ClickableComponent> selectionButtons8 = this.rightSelectionButtons;
                ClickableTextureComponent textureComponent12 = new ClickableTextureComponent("Wallets", new Rectangle(this.xPositionOnScreen - 256 + IClickableMenu.borderWidth + 128 + 12, y, 64, 64), (string)null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33, -1, -1), 1f, false);
                textureComponent12.myID = 632;
                textureComponent12.upNeighborID = -99998;
                textureComponent12.leftNeighborID = -99998;
                textureComponent12.rightNeighborID = -99998;
                textureComponent12.downNeighborID = -99998;
                selectionButtons8.Add((ClickableComponent)textureComponent12);
                ClickableTextureComponent textureComponent13 = new ClickableTextureComponent("CoopHelp", new Rectangle(this.xPositionOnScreen - 256 + IClickableMenu.borderWidth + 128 - 8, this.yPositionOnScreen + IClickableMenu.borderWidth * 2 + 448 + 40, 64, 64), (string)null, Game1.content.LoadString("Strings\\UI:Character_CoopHelp"), Game1.mouseCursors, new Rectangle(240, 192, 16, 16), 4f, false);
                textureComponent13.myID = 625;
                textureComponent13.upNeighborID = -99998;
                textureComponent13.leftNeighborID = -99998;
                textureComponent13.rightNeighborID = -99998;
                textureComponent13.downNeighborID = -99998;
                this.coopHelpButton = textureComponent13;
                ClickableTextureComponent textureComponent14 = new ClickableTextureComponent("CoopHelpOK", new Rectangle(this.xPositionOnScreen - 256 - 12, this.yPositionOnScreen + this.height - 64, 64, 64), (string)null, (string)null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46, -1, -1), 1f, false);
                textureComponent14.myID = 626;
                textureComponent14.region = 635;
                textureComponent14.upNeighborID = -99998;
                textureComponent14.leftNeighborID = -99998;
                textureComponent14.rightNeighborID = -99998;
                textureComponent14.downNeighborID = -99998;
                this.coopHelpOkButton = textureComponent14;
                this.noneString = Game1.content.LoadString("Strings\\UI:Character_none");
                this.normalDiffString = Game1.content.LoadString("Strings\\UI:Character_Normal");
                this.toughDiffString = Game1.content.LoadString("Strings\\UI:Character_Tough");
                this.hardDiffString = Game1.content.LoadString("Strings\\UI:Character_Hard");
                this.superDiffString = Game1.content.LoadString("Strings\\UI:Character_Super");
                this.separateWalletString = Game1.content.LoadString("Strings\\UI:Character_SeparateWallet");
                this.sharedWalletString = Game1.content.LoadString("Strings\\UI:Character_SharedWallet");
                ClickableTextureComponent textureComponent15 = new ClickableTextureComponent("CoopHelpRight", new Rectangle(this.xPositionOnScreen + this.width, this.yPositionOnScreen + this.height, 64, 64), (string)null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33, -1, -1), 1f, false);
                textureComponent15.myID = 633;
                textureComponent15.region = 635;
                textureComponent15.upNeighborID = -99998;
                textureComponent15.leftNeighborID = -99998;
                textureComponent15.rightNeighborID = -99998;
                textureComponent15.downNeighborID = -99998;
                this.coopHelpRightButton = textureComponent15;
                ClickableTextureComponent textureComponent16 = new ClickableTextureComponent("CoopHelpLeft", new Rectangle(this.xPositionOnScreen, this.yPositionOnScreen + this.height, 64, 64), (string)null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44, -1, -1), 1f, false);
                textureComponent16.myID = 634;
                textureComponent16.region = 635;
                textureComponent16.upNeighborID = -99998;
                textureComponent16.leftNeighborID = -99998;
                textureComponent16.rightNeighborID = -99998;
                textureComponent16.downNeighborID = -99998;
                this.coopHelpLeftButton = textureComponent16;
            }
            Point point1 = new Point(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 320 + 48 + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num4);
            int x1 = this.xPositionOnScreen + 16 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 192 + 8;
            if (this._isDyeMenu)
                x1 = this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth;
            if (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm || (this.source == CharacterCustomization.Source.NewFarmhand || this.source == CharacterCustomization.Source.Wizard)) {
                this.labels.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + 16 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 192 + 8, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num4 + 16, 1, 1), Game1.content.LoadString("Strings\\UI:Character_EyeColor")));
                this.eyeColorPicker = new ColorPicker("Eyes", point1.X, point1.Y);
                this.eyeColorPicker.setColor((Color)((NetFieldBase<Color, NetColor>)Game1.player.newEyeColor));
                this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(point1.X, point1.Y, 128, 20), "") {
                    myID = 522,
                    downNeighborID = -99998,
                    upNeighborID = -99998,
                    leftNeighborImmutable = true,
                    rightNeighborImmutable = true
                });
                this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(point1.X, point1.Y + 20, 128, 20), "") {
                    myID = 523,
                    upNeighborID = -99998,
                    downNeighborID = -99998,
                    leftNeighborImmutable = true,
                    rightNeighborImmutable = true
                });
                this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(point1.X, point1.Y + 40, 128, 20), "") {
                    myID = 524,
                    upNeighborID = -99998,
                    downNeighborID = -99998,
                    leftNeighborImmutable = true,
                    rightNeighborImmutable = true
                });
                num4 += 68;
                List<ClickableComponent> selectionButtons3 = this.leftSelectionButtons;
                ClickableTextureComponent textureComponent5 = new ClickableTextureComponent("Hair", new Rectangle(this.xPositionOnScreen + 16 + IClickableMenu.borderWidth + IClickableMenu.spaceToClearSideBorder + num5, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num4, 64, 64), (string)null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44, -1, -1), 1f, false);
                textureComponent5.myID = 514;
                textureComponent5.upNeighborID = -99998;
                textureComponent5.leftNeighborID = -99998;
                textureComponent5.rightNeighborID = -99998;
                textureComponent5.downNeighborID = -99998;
                selectionButtons3.Add((ClickableComponent)textureComponent5);
                this.labels.Add(this.hairLabel = new ClickableComponent(new Rectangle(this.xPositionOnScreen + 16 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 64 + 8 + num5 / 2, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num4 + 16, 1, 1), Game1.content.LoadString("Strings\\UI:Character_Hair")));
                List<ClickableComponent> selectionButtons4 = this.rightSelectionButtons;
                ClickableTextureComponent textureComponent6 = new ClickableTextureComponent("Hair", new Rectangle(this.xPositionOnScreen + 16 + IClickableMenu.spaceToClearSideBorder + 128 + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num4, 64, 64), (string)null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33, -1, -1), 1f, false);
                textureComponent6.myID = 515;
                textureComponent6.upNeighborID = -99998;
                textureComponent6.leftNeighborID = -99998;
                textureComponent6.rightNeighborID = -99998;
                textureComponent6.downNeighborID = -99998;
                selectionButtons4.Add((ClickableComponent)textureComponent6);
            }
            point1 = new Point(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 320 + 48 + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num4);
            if (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm || (this.source == CharacterCustomization.Source.NewFarmhand || this.source == CharacterCustomization.Source.Wizard)) {
                this.labels.Add(new ClickableComponent(new Rectangle(x1, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num4 + 16, 1, 1), Game1.content.LoadString("Strings\\UI:Character_HairColor")));
                this.hairColorPicker = new ColorPicker("Hair", point1.X, point1.Y);
                this.hairColorPicker.setColor((Color)((NetFieldBase<Color, NetColor>)Game1.player.hairstyleColor));
                this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(point1.X, point1.Y, 128, 20), "") {
                    myID = 525,
                    downNeighborID = -99998,
                    upNeighborID = -99998,
                    leftNeighborImmutable = true,
                    rightNeighborImmutable = true
                });
                this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(point1.X, point1.Y + 20, 128, 20), "") {
                    myID = 526,
                    upNeighborID = -99998,
                    downNeighborID = -99998,
                    leftNeighborImmutable = true,
                    rightNeighborImmutable = true
                });
                this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(point1.X, point1.Y + 40, 128, 20), "") {
                    myID = 527,
                    upNeighborID = -99998,
                    downNeighborID = -99998,
                    leftNeighborImmutable = true,
                    rightNeighborImmutable = true
                });
            }
            if (this.source == CharacterCustomization.Source.DyePots) {
                num4 += 68;
                if (Game1.player.shirtItem.Value != null && Game1.player.shirtItem.Value.dyeable.Value) {
                    point1 = new Point(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 320 + 48 + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num4);
                    point1.X = this.xPositionOnScreen + this.width - IClickableMenu.spaceToClearSideBorder - IClickableMenu.borderWidth - 160;
                    this.labels.Add(new ClickableComponent(new Rectangle(x1, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num4 + 16, 1, 1), Game1.content.LoadString("Strings\\UI:Character_ShirtColor")));
                    this.hairColorPicker = new ColorPicker("Hair", point1.X, point1.Y);
                    this.hairColorPicker.setColor(Game1.player.GetShirtColor());
                    this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(point1.X, point1.Y, 128, 20), "") {
                        myID = 525,
                        downNeighborID = -99998,
                        upNeighborID = -99998,
                        leftNeighborImmutable = true,
                        rightNeighborImmutable = true
                    });
                    this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(point1.X, point1.Y + 20, 128, 20), "") {
                        myID = 526,
                        upNeighborID = -99998,
                        downNeighborID = -99998,
                        leftNeighborImmutable = true,
                        rightNeighborImmutable = true
                    });
                    this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(point1.X, point1.Y + 40, 128, 20), "") {
                        myID = 527,
                        upNeighborID = -99998,
                        downNeighborID = -99998,
                        leftNeighborImmutable = true,
                        rightNeighborImmutable = true
                    });
                    num4 += 64;
                }
                if (Game1.player.pantsItem.Value != null && Game1.player.pantsItem.Value.dyeable.Value) {
                    point1 = new Point(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 320 + 48 + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num4);
                    point1.X = this.xPositionOnScreen + this.width - IClickableMenu.spaceToClearSideBorder - IClickableMenu.borderWidth - 160;
                    int num6 = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.tr ? -16 : 0;
                    this.labels.Add(new ClickableComponent(new Rectangle(x1, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num4 + 16 + num6, 1, 1), Game1.content.LoadString("Strings\\UI:Character_PantsColor")));
                    this.pantsColorPicker = new ColorPicker("Pants", point1.X, point1.Y);
                    this.pantsColorPicker.setColor(Game1.player.GetPantsColor());
                    this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(point1.X, point1.Y, 128, 20), "") {
                        myID = 528,
                        downNeighborID = -99998,
                        upNeighborID = -99998,
                        rightNeighborImmutable = true,
                        leftNeighborImmutable = true
                    });
                    this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(point1.X, point1.Y + 20, 128, 20), "") {
                        myID = 529,
                        downNeighborID = -99998,
                        upNeighborID = -99998,
                        rightNeighborImmutable = true,
                        leftNeighborImmutable = true
                    });
                    this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(point1.X, point1.Y + 40, 128, 20), "") {
                        myID = 530,
                        downNeighborID = -99998,
                        upNeighborID = -99998,
                        rightNeighborImmutable = true,
                        leftNeighborImmutable = true
                    });
                }
            } else if (flag2) {
                num4 += 68;
                int num6 = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.tr ? 8 : 0;
                List<ClickableComponent> selectionButtons3 = this.leftSelectionButtons;
                ClickableTextureComponent textureComponent5 = new ClickableTextureComponent("Shirt", new Rectangle(this.xPositionOnScreen + 16 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + num5 - num6, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num4, 64, 64), (string)null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44, -1, -1), 1f, false);
                textureComponent5.myID = 512;
                textureComponent5.upNeighborID = -99998;
                textureComponent5.leftNeighborID = -99998;
                textureComponent5.rightNeighborID = -99998;
                textureComponent5.downNeighborID = -99998;
                selectionButtons3.Add((ClickableComponent)textureComponent5);
                this.labels.Add(this.shirtLabel = new ClickableComponent(new Rectangle(this.xPositionOnScreen + 16 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 64 + 8 + num5 / 2, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num4 + 16, 1, 1), Game1.content.LoadString("Strings\\UI:Character_Shirt")));
                List<ClickableComponent> selectionButtons4 = this.rightSelectionButtons;
                ClickableTextureComponent textureComponent6 = new ClickableTextureComponent("Shirt", new Rectangle(this.xPositionOnScreen + 16 + IClickableMenu.spaceToClearSideBorder + 128 + IClickableMenu.borderWidth + num6, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num4, 64, 64), (string)null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33, -1, -1), 1f, false);
                textureComponent6.myID = 513;
                textureComponent6.upNeighborID = -99998;
                textureComponent6.leftNeighborID = -99998;
                textureComponent6.rightNeighborID = -99998;
                textureComponent6.downNeighborID = -99998;
                selectionButtons4.Add((ClickableComponent)textureComponent6);
                int num7 = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.tr ? -16 : 0;
                this.labels.Add(new ClickableComponent(new Rectangle(x1, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num4 + 16 + num7, 1, 1), Game1.content.LoadString("Strings\\UI:Character_PantsColor")));
                point1 = new Point(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 320 + 48 + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num4);
                this.pantsColorPicker = new ColorPicker("Pants", point1.X, point1.Y);
                this.pantsColorPicker.setColor((Color)((NetFieldBase<Color, NetColor>)Game1.player.pantsColor));
                this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(point1.X, point1.Y, 128, 20), "") {
                    myID = 528,
                    downNeighborID = -99998,
                    upNeighborID = -99998,
                    rightNeighborImmutable = true,
                    leftNeighborImmutable = true
                });
                this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(point1.X, point1.Y + 20, 128, 20), "") {
                    myID = 529,
                    downNeighborID = -99998,
                    upNeighborID = -99998,
                    rightNeighborImmutable = true,
                    leftNeighborImmutable = true
                });
                this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(point1.X, point1.Y + 40, 128, 20), "") {
                    myID = 530,
                    downNeighborID = -99998,
                    upNeighborID = -99998,
                    rightNeighborImmutable = true,
                    leftNeighborImmutable = true
                });
            } else if (this.source == CharacterCustomization.Source.ClothesDye) {
                num4 += 60;
                point1 = new Point(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 320 + 48 + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num4);
                point1.X = this.xPositionOnScreen + this.width - IClickableMenu.spaceToClearSideBorder - IClickableMenu.borderWidth - 160;
                this.labels.Add(new ClickableComponent(new Rectangle(x1, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num4 + 16, 1, 1), Game1.content.LoadString("Strings\\UI:Character_DyeColor")));
                this.pantsColorPicker = new ColorPicker("Pants", point1.X, point1.Y);
                this.pantsColorPicker.setColor((Color)((NetFieldBase<Color, NetColor>)this._itemToDye.clothesColor));
                this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(point1.X, point1.Y, 128, 20), "") {
                    myID = 528,
                    downNeighborID = -99998,
                    upNeighborID = -99998,
                    rightNeighborImmutable = true,
                    leftNeighborImmutable = true
                });
                this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(point1.X, point1.Y + 20, 128, 20), "") {
                    myID = 529,
                    downNeighborID = -99998,
                    upNeighborID = -99998,
                    rightNeighborImmutable = true,
                    leftNeighborImmutable = true
                });
                this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(point1.X, point1.Y + 40, 128, 20), "") {
                    myID = 530,
                    downNeighborID = -99998,
                    upNeighborID = -99998,
                    rightNeighborImmutable = true,
                    leftNeighborImmutable = true
                });
            }
            ClickableTextureComponent textureComponent17 = new ClickableTextureComponent("Skip Intro", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 320 - 48 + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num4 + 80, 36, 36), (string)null, Game1.content.LoadString("Strings\\UI:Character_SkipIntro"), Game1.mouseCursors, new Rectangle(227, 425, 9, 9), 4f, false);
            textureComponent17.myID = 506;
            textureComponent17.upNeighborID = 530;
            textureComponent17.leftNeighborID = 517;
            textureComponent17.rightNeighborID = 505;
            this.skipIntroButton = textureComponent17;
            if (flag2) {
                num4 += 68;
                List<ClickableComponent> selectionButtons3 = this.leftSelectionButtons;
                ClickableTextureComponent textureComponent5 = new ClickableTextureComponent("Pants Style", new Rectangle(this.xPositionOnScreen + 16 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + num5, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num4, 64, 64), (string)null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44, -1, -1), 1f, false);
                textureComponent5.myID = 629;
                textureComponent5.upNeighborID = -99998;
                textureComponent5.leftNeighborID = -99998;
                textureComponent5.rightNeighborID = -99998;
                textureComponent5.downNeighborID = -99998;
                selectionButtons3.Add((ClickableComponent)textureComponent5);
                this.labels.Add(this.pantsStyleLabel = new ClickableComponent(new Rectangle(this.xPositionOnScreen + 16 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 64 + 8 + num5 / 2, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num4 + 16, 1, 1), Game1.content.LoadString("Strings\\UI:Character_Pants")));
                List<ClickableComponent> selectionButtons4 = this.rightSelectionButtons;
                ClickableTextureComponent textureComponent6 = new ClickableTextureComponent("Pants Style", new Rectangle(this.xPositionOnScreen + 16 + IClickableMenu.spaceToClearSideBorder + 128 + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num4, 64, 64), (string)null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33, -1, -1), 1f, false);
                textureComponent6.myID = 517;
                textureComponent6.upNeighborID = -99998;
                textureComponent6.leftNeighborID = -99998;
                textureComponent6.rightNeighborID = -99998;
                textureComponent6.downNeighborID = -99998;
                selectionButtons4.Add((ClickableComponent)textureComponent6);
            }
            int num8 = num4 + 68;
            if (flag1) {
                int num6 = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.tr ? 32 : 0;
                List<ClickableComponent> selectionButtons3 = this.leftSelectionButtons;
                ClickableTextureComponent textureComponent5 = new ClickableTextureComponent("Acc", new Rectangle(this.xPositionOnScreen + 16 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + num5 - num6, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num8, 64, 64), (string)null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44, -1, -1), 1f, false);
                textureComponent5.myID = 516;
                textureComponent5.upNeighborID = -99998;
                textureComponent5.leftNeighborID = -99998;
                textureComponent5.rightNeighborID = -99998;
                textureComponent5.downNeighborID = -99998;
                selectionButtons3.Add((ClickableComponent)textureComponent5);
                this.labels.Add(this.accLabel = new ClickableComponent(new Rectangle(this.xPositionOnScreen + 16 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 64 + 8 + num5 / 2, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num8 + 16, 1, 1), Game1.content.LoadString("Strings\\UI:Character_Accessory")));
                List<ClickableComponent> selectionButtons4 = this.rightSelectionButtons;
                ClickableTextureComponent textureComponent6 = new ClickableTextureComponent("Acc", new Rectangle(this.xPositionOnScreen + 16 + IClickableMenu.spaceToClearSideBorder + 128 + IClickableMenu.borderWidth + num6, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num8, 64, 64), (string)null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33, -1, -1), 1f, false);
                textureComponent6.myID = 517;
                textureComponent6.upNeighborID = -99998;
                textureComponent6.leftNeighborID = -99998;
                textureComponent6.rightNeighborID = -99998;
                textureComponent6.downNeighborID = -99998;
                selectionButtons4.Add((ClickableComponent)textureComponent6);
            }
            if (Game1.gameMode == (byte)3 && Game1.locations != null && this.source == CharacterCustomization.Source.Wizard) {
                Pet characterFromName = Game1.getCharacterFromName<Pet>(Game1.player.getPetName(), false);
                if (characterFromName != null) {
                    Game1.player.whichPetBreed = (int)((NetFieldBase<int, NetInt>)characterFromName.whichBreed);
                    Game1.player.catPerson = characterFromName is Cat;
                    this.isModifyingExistingPet = true;
                    int num6 = num8 + 60;
                    this.labels.Add(new ClickableComponent(new Rectangle((int)((double)(this.xPositionOnScreen + this.width / 2) - (double)Game1.smallFont.MeasureString((string)((NetFieldBase<string, NetString>)characterFromName.name)).X / 2.0), this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num6 + 16, 1, 1), characterFromName.Name));
                    point1 = new Point(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 320 + 48 + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num6);
                    point1.X = this.xPositionOnScreen + this.width - IClickableMenu.spaceToClearSideBorder - IClickableMenu.borderWidth - 128;
                    int num7 = num6 + 42;
                    point1 = new Point(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 320 + 48 + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num7);
                    point1.X = this.xPositionOnScreen + this.width - IClickableMenu.spaceToClearSideBorder - IClickableMenu.borderWidth - 128;
                    this.petPortraitBox = new Rectangle?(new Rectangle(this.xPositionOnScreen + this.width / 2 - 32, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + num7, 64, 64));
                }
            }
            if (this.petPortraitBox.HasValue) {
                List<ClickableComponent> selectionButtons3 = this.leftSelectionButtons;
                Rectangle rectangle = this.petPortraitBox.Value;
                int x2 = rectangle.Left - 64;
                rectangle = this.petPortraitBox.Value;
                int top1 = rectangle.Top;
                ClickableTextureComponent textureComponent5 = new ClickableTextureComponent("Pet", new Rectangle(x2, top1, 64, 64), (string)null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44, -1, -1), 1f, false);
                textureComponent5.myID = 511;
                textureComponent5.upNeighborID = -99998;
                textureComponent5.leftNeighborID = -99998;
                textureComponent5.rightNeighborID = -99998;
                textureComponent5.downNeighborID = -99998;
                selectionButtons3.Add((ClickableComponent)textureComponent5);
                List<ClickableComponent> selectionButtons4 = this.rightSelectionButtons;
                rectangle = this.petPortraitBox.Value;
                int x3 = rectangle.Left + 64;
                rectangle = this.petPortraitBox.Value;
                int top2 = rectangle.Top;
                ClickableTextureComponent textureComponent6 = new ClickableTextureComponent("Pet", new Rectangle(x3, top2, 64, 64), (string)null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33, -1, -1), 1f, false);
                textureComponent6.myID = 510;
                textureComponent6.upNeighborID = -99998;
                textureComponent6.leftNeighborID = -99998;
                textureComponent6.rightNeighborID = -99998;
                textureComponent6.downNeighborID = -99998;
                selectionButtons4.Add((ClickableComponent)textureComponent6);
            }
            this._shouldShowBackButton = true;
            if (this.source == CharacterCustomization.Source.Dresser || this.source == CharacterCustomization.Source.Wizard || this.source == CharacterCustomization.Source.ClothesDye)
                this._shouldShowBackButton = false;
            if (this.source == CharacterCustomization.Source.Dresser || this.source == CharacterCustomization.Source.Wizard || this._isDyeMenu) {
                this.nameBoxCC.visible = false;
                this.farmnameBoxCC.visible = false;
                this.favThingBoxCC.visible = false;
                this.farmLabel.visible = false;
                this.nameLabel.visible = false;
                this.favoriteLabel.visible = false;
            }
            if (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm)
                this.skipIntroButton.visible = true;
            else
                this.skipIntroButton.visible = false;
            if (!Game1.options.snappyMenus || !Game1.options.gamepadControls)
                return;
            this.populateClickableComponentList();
            this.snapToDefaultClickableComponent();
        }

        public override void snapToDefaultClickableComponent() {
            if (this.showingCoopHelp)
                this.currentlySnappedComponent = this.getComponentWithID(626);
            else
                this.currentlySnappedComponent = this.getComponentWithID(521);
            this.snapCursorToCurrentSnappedComponent();
        }

        public override void gamePadButtonHeld(Buttons b) {
            base.gamePadButtonHeld(b);
            if (this.currentlySnappedComponent == null)
                return;
            switch (b) {
                case Buttons.DPadLeft:
                case Buttons.LeftThumbstickLeft:
                    switch (this.currentlySnappedComponent.myID) {
                        case 522:
                            this.eyeColorPicker.LastColor = this.eyeColorPicker.getSelectedColor();
                            this.eyeColorPicker.changeHue(-1);
                            this.eyeColorPicker.Dirty = true;
                            this._sliderOpTarget = this.eyeColorPicker;
                            this._sliderAction = this._recolorEyesAction;
                            return;
                        case 523:
                            this.eyeColorPicker.LastColor = this.eyeColorPicker.getSelectedColor();
                            this.eyeColorPicker.changeSaturation(-1);
                            this.eyeColorPicker.Dirty = true;
                            this._sliderOpTarget = this.eyeColorPicker;
                            this._sliderAction = this._recolorEyesAction;
                            return;
                        case 524:
                            this.eyeColorPicker.LastColor = this.eyeColorPicker.getSelectedColor();
                            this.eyeColorPicker.changeValue(-1);
                            this.eyeColorPicker.Dirty = true;
                            this._sliderOpTarget = this.eyeColorPicker;
                            this._sliderAction = this._recolorEyesAction;
                            return;
                        case 525:
                            this.hairColorPicker.LastColor = this.hairColorPicker.getSelectedColor();
                            this.hairColorPicker.changeHue(-1);
                            this.hairColorPicker.Dirty = true;
                            this._sliderOpTarget = this.hairColorPicker;
                            this._sliderAction = this._recolorHairAction;
                            return;
                        case 526:
                            this.hairColorPicker.LastColor = this.hairColorPicker.getSelectedColor();
                            this.hairColorPicker.changeSaturation(-1);
                            this.hairColorPicker.Dirty = true;
                            this._sliderOpTarget = this.hairColorPicker;
                            this._sliderAction = this._recolorHairAction;
                            return;
                        case 527:
                            this.hairColorPicker.LastColor = this.hairColorPicker.getSelectedColor();
                            this.hairColorPicker.changeValue(-1);
                            this.hairColorPicker.Dirty = true;
                            this._sliderOpTarget = this.hairColorPicker;
                            this._sliderAction = this._recolorHairAction;
                            return;
                        case 528:
                            this.pantsColorPicker.LastColor = this.pantsColorPicker.getSelectedColor();
                            this.pantsColorPicker.changeHue(-1);
                            this.pantsColorPicker.Dirty = true;
                            this._sliderOpTarget = this.pantsColorPicker;
                            this._sliderAction = this._recolorPantsAction;
                            return;
                        case 529:
                            this.pantsColorPicker.LastColor = this.pantsColorPicker.getSelectedColor();
                            this.pantsColorPicker.changeSaturation(-1);
                            this.pantsColorPicker.Dirty = true;
                            this._sliderOpTarget = this.pantsColorPicker;
                            this._sliderAction = this._recolorPantsAction;
                            return;
                        case 530:
                            this.pantsColorPicker.LastColor = this.pantsColorPicker.getSelectedColor();
                            this.pantsColorPicker.changeValue(-1);
                            this.pantsColorPicker.Dirty = true;
                            this._sliderOpTarget = this.pantsColorPicker;
                            this._sliderAction = this._recolorPantsAction;
                            return;
                        default:
                            return;
                    }
                case Buttons.DPadRight:
                case Buttons.LeftThumbstickRight:
                    switch (this.currentlySnappedComponent.myID) {
                        case 522:
                            this.eyeColorPicker.LastColor = this.eyeColorPicker.getSelectedColor();
                            this.eyeColorPicker.changeHue(1);
                            this.eyeColorPicker.Dirty = true;
                            this._sliderOpTarget = this.eyeColorPicker;
                            this._sliderAction = this._recolorEyesAction;
                            return;
                        case 523:
                            this.eyeColorPicker.LastColor = this.eyeColorPicker.getSelectedColor();
                            this.eyeColorPicker.changeSaturation(1);
                            this.eyeColorPicker.Dirty = true;
                            this._sliderOpTarget = this.eyeColorPicker;
                            this._sliderAction = this._recolorEyesAction;
                            return;
                        case 524:
                            this.eyeColorPicker.LastColor = this.eyeColorPicker.getSelectedColor();
                            this.eyeColorPicker.changeValue(1);
                            this.eyeColorPicker.Dirty = true;
                            this._sliderOpTarget = this.eyeColorPicker;
                            this._sliderAction = this._recolorEyesAction;
                            return;
                        case 525:
                            this.hairColorPicker.LastColor = this.hairColorPicker.getSelectedColor();
                            this.hairColorPicker.changeHue(1);
                            this.hairColorPicker.Dirty = true;
                            this._sliderOpTarget = this.hairColorPicker;
                            this._sliderAction = this._recolorHairAction;
                            return;
                        case 526:
                            this.hairColorPicker.LastColor = this.hairColorPicker.getSelectedColor();
                            this.hairColorPicker.changeSaturation(1);
                            this.hairColorPicker.Dirty = true;
                            this._sliderOpTarget = this.hairColorPicker;
                            this._sliderAction = this._recolorHairAction;
                            return;
                        case 527:
                            this.hairColorPicker.LastColor = this.hairColorPicker.getSelectedColor();
                            this.hairColorPicker.changeValue(1);
                            this.hairColorPicker.Dirty = true;
                            this._sliderOpTarget = this.hairColorPicker;
                            this._sliderAction = this._recolorHairAction;
                            return;
                        case 528:
                            this.pantsColorPicker.LastColor = this.pantsColorPicker.getSelectedColor();
                            this.pantsColorPicker.changeHue(1);
                            this.pantsColorPicker.Dirty = true;
                            this._sliderOpTarget = this.pantsColorPicker;
                            this._sliderAction = this._recolorPantsAction;
                            return;
                        case 529:
                            this.pantsColorPicker.LastColor = this.pantsColorPicker.getSelectedColor();
                            this.pantsColorPicker.changeSaturation(1);
                            this.pantsColorPicker.Dirty = true;
                            this._sliderOpTarget = this.pantsColorPicker;
                            this._sliderAction = this._recolorPantsAction;
                            return;
                        case 530:
                            this.pantsColorPicker.LastColor = this.pantsColorPicker.getSelectedColor();
                            this.pantsColorPicker.changeValue(1);
                            this.pantsColorPicker.Dirty = true;
                            this._sliderOpTarget = this.pantsColorPicker;
                            this._sliderAction = this._recolorPantsAction;
                            return;
                        default:
                            return;
                    }
            }
        }

        public override void receiveGamePadButton(Buttons b) {
            base.receiveGamePadButton(b);
            if (this.currentlySnappedComponent == null)
                return;
            switch (b) {
                case Buttons.B:
                    if (!this.showingCoopHelp)
                        break;
                    this.receiveLeftClick(this.coopHelpOkButton.bounds.Center.X, this.coopHelpOkButton.bounds.Center.Y, true);
                    break;
                case Buttons.RightTrigger:
                    if ((uint)(this.currentlySnappedComponent.myID - 512) > 9U)
                        break;
                    this.selectionClick(this.currentlySnappedComponent.name, 1);
                    break;
                case Buttons.LeftTrigger:
                    if ((uint)(this.currentlySnappedComponent.myID - 512) > 9U)
                        break;
                    this.selectionClick(this.currentlySnappedComponent.name, -1);
                    break;
            }
        }

        // MTN Customized
        private void optionButtonClick(string name) {
            if (name.StartsWith("MTN_")) {
                if (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm) {
                    Game1.whichFarm = 200;
                    Game1.spawnMonstersAtNight = false;
                    adjustWhichFarmType(name);
                }
            } else {
                this.vanillaOptionButtonClick(name);
            }

            Game1.playSound("coin");
        }

        // Vanilla cases
        private void vanillaOptionButtonClick(string name) {
            switch (name) {
                case "Cat":
                    if (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm) {
                        Game1.player.catPerson = true;
                        break;
                    }
                    break;
                case "Close":
                    Game1.cabinsSeparate = false;
                    break;
                case "Dog":
                    if (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm) {
                        Game1.player.catPerson = false;
                        break;
                    }
                    break;
                case "Female":
                    if (this.source != CharacterCustomization.Source.Wizard) {
                        Game1.player.changeGender(false);
                        Game1.player.changeHairStyle(16);
                        break;
                    }
                    break;
                case "Forest":
                    if (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm) {
                        Game1.whichFarm = 2;
                        Game1.spawnMonstersAtNight = false;
                        adjustWhichFarmType(name);
                        break;
                    }
                    break;
                case "Four Corners":
                    if (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm) {
                        Game1.whichFarm = 5;
                        Game1.spawnMonstersAtNight = false;
                        adjustWhichFarmType(name);
                        break;
                    }
                    break;
                case "Hills":
                    if (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm) {
                        Game1.whichFarm = 3;
                        Game1.spawnMonstersAtNight = false;
                        adjustWhichFarmType(name);
                        break;
                    }
                    break;
                case "Male":
                    if (this.source != CharacterCustomization.Source.Wizard) {
                        Game1.player.changeGender(true);
                        Game1.player.changeHairStyle(0);
                        break;
                    }
                    break;
                case "OK":
                    if (!this.canLeaveMenu())
                        return;
                    if (this._itemToDye != null) {
                        if (!Game1.player.IsEquippedItem((Item)this._itemToDye))
                            Utility.CollectOrDrop((Item)this._itemToDye);
                        this._itemToDye = (Clothing)null;
                    }
                    if (this.source == CharacterCustomization.Source.ClothesDye) {
                        Game1.exitActiveMenu();
                        break;
                    }
                    Game1.player.Name = this.nameBox.Text.Trim();
                    Game1.player.displayName = Game1.player.Name;
                    Game1.player.favoriteThing.Value = this.favThingBox.Text.Trim();
                    Game1.player.isCustomized.Value = true;
                    Game1.player.ConvertClothingOverrideToClothesItems();
                    if (this.source == CharacterCustomization.Source.HostNewFarm)
                        Game1.multiplayerMode = (byte)2;
                    string str = (string)null;
                    if (this.petPortraitBox.HasValue && Game1.gameMode == (byte)3 && Game1.locations != null) {
                        Pet characterFromName = Game1.getCharacterFromName<Pet>(Game1.player.getPetName(), false);
                        if (characterFromName != null && this.petHasChanges(characterFromName)) {
                            characterFromName.whichBreed.Value = Game1.player.whichPetBreed;
                            str = characterFromName.getName();
                        }
                    }
                    if (Game1.activeClickableMenu is TitleMenu) {
                        (Game1.activeClickableMenu as TitleMenu).createdNewCharacter(this.skipIntro);
                        break;
                    }
                    Game1.exitActiveMenu();
                    if (Game1.currentMinigame != null && Game1.currentMinigame is Intro) {
                        (Game1.currentMinigame as Intro).doneCreatingCharacter();
                        break;
                    }
                    if (this.source == CharacterCustomization.Source.Wizard) {
                        if (str != null)
                            this.multiplayer.globalChatInfoMessage("Makeover_Pet", Game1.player.Name, str);
                        else
                            this.multiplayer.globalChatInfoMessage("Makeover", Game1.player.Name);
                        Game1.flashAlpha = 1f;
                        Game1.playSound("yoba");
                        break;
                    }
                    if (this.source == CharacterCustomization.Source.ClothesDye) {
                        Game1.playSound("yoba");
                        break;
                    }
                    break;
                case "Riverland":
                    if (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm) {
                        Game1.whichFarm = 1;
                        Game1.spawnMonstersAtNight = false;
                        adjustWhichFarmType(name);
                        break;
                    }
                    break;
                case "Separate":
                    Game1.cabinsSeparate = true;
                    break;
                case "Standard":
                    if (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm) {
                        Game1.whichFarm = 0;
                        Game1.spawnMonstersAtNight = false;
                        adjustWhichFarmType(name);
                        break;
                    }
                    break;
                case "Wilderness":
                    if (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm) {
                        Game1.whichFarm = 4;
                        Game1.spawnMonstersAtNight = true;
                        adjustWhichFarmType(name);
                        break;
                    }
                    break;
            }
        }

        private void adjustWhichFarmType(string name) {
            lastClickedFarmTypeBtn = name;
            customManager.UpdateSelectedFarm(name);
            if (!customManager.Canon) Game1.whichFarm = customManager.SelectedFarm.ID;
            adjustCabinSettings();
        }

        private void adjustCabinSettings() {
            if (customManager.Canon) {
                Game1.startingCabins = 0;
                allowCabinsClose = true;
                allowCabinsSeperate = true;
                return;
            }
            if (customManager.SelectedFarm.CabinCapacity == 0) {
                Game1.startingCabins = 0;
                return;
            } else if (customManager.SelectedFarm.CabinCapacity < Game1.startingCabins) {
                Game1.startingCabins = customManager.SelectedFarm.CabinCapacity;
            }

            allowCabinsClose = customManager.SelectedFarm.AllowClose;
            allowCabinsSeperate = customManager.SelectedFarm.AllowSeperate;
        }

        public bool petHasChanges(Pet pet) {
            return Game1.player.catPerson && pet == null || Game1.player.whichPetBreed != pet.whichBreed.Value;
        }

        private void selectionClick(string name, int change) {
            if (name == null) {
                monitor.Log($"name is null in selectionClick. Discarding click request. If the issue persist, inform SgtPickles in SDV community discord.", LogLevel.Warn);
                return;
            }

            switch (name) {
                case "Cabins":
                    if ((Game1.startingCabins != 0 || change >= 0) && (Game1.startingCabins != 3 || change <= 0))
                        Game1.playSound("axchop");
                    Game1.startingCabins += change;
                    Game1.startingCabins = Math.Max(0, Math.Min(3, Game1.startingCabins));
                    break;
                case "Wallets":
                    if ((bool)((NetFieldBase<bool, NetBool>)Game1.player.team.useSeparateWallets)) {
                        Game1.playSound("coin");
                        Game1.player.team.useSeparateWallets.Value = false;
                        break;
                    }
                    Game1.playSound("coin");
                    Game1.player.team.useSeparateWallets.Value = true;
                    break;
                case "Pants Style":
                    Game1.player.changePantStyle((int)((NetFieldBase<int, NetInt>)Game1.player.pants) + change, true);
                    Game1.playSound("coin");
                    break;
                case "Hair":
                    Game1.player.changeHairStyle((int)((NetFieldBase<int, NetInt>)Game1.player.hair) + change);
                    Game1.playSound("grassyStep");
                    break;
                case "Direction":
                    this._displayFarmer.faceDirection((this._displayFarmer.FacingDirection - change + 4) % 4);
                    this._displayFarmer.FarmerSprite.StopAnimation();
                    this._displayFarmer.completelyStopAnimatingOrDoingAction();
                    Game1.playSound("pickUpItem");
                    break;
                case "Difficulty":
                    if ((double)Game1.player.difficultyModifier < 1.0 && change < 0) {
                        Game1.playSound("breathout");
                        Game1.player.difficultyModifier += 0.25f;
                        break;
                    }
                    if ((double)Game1.player.difficultyModifier <= 0.25 || change <= 0)
                        break;
                    Game1.playSound("batFlap");
                    Game1.player.difficultyModifier -= 0.25f;
                    break;
                case "Acc":
                    Game1.player.changeAccessory((int)((NetFieldBase<int, NetInt>)Game1.player.accessory) + change);
                    Game1.playSound("purchase");
                    break;
                case "Skin":
                    Game1.player.changeSkinColor((int)((NetFieldBase<int, NetInt>)Game1.player.skin) + change, false);
                    Game1.playSound("skeletonStep");
                    break;
                case "Pet":
                    Game1.player.whichPetBreed += change;
                    if (Game1.player.whichPetBreed >= 3) {
                        Game1.player.whichPetBreed = 0;
                        if (!this.isModifyingExistingPet)
                            Game1.player.catPerson = !Game1.player.catPerson;
                    } else if (Game1.player.whichPetBreed < 0) {
                        Game1.player.whichPetBreed = 2;
                        if (!this.isModifyingExistingPet)
                            Game1.player.catPerson = !Game1.player.catPerson;
                    }
                    Game1.playSound("coin");
                    break;
                case "Shirt":
                    Game1.player.changeShirt((int)((NetFieldBase<int, NetInt>)Game1.player.shirt) + change, true);
                    Game1.playSound("coin");
                    break;
            }
        }

        public override bool readyToClose() {
            if (this.showingCoopHelp)
                return false;
            return base.readyToClose();
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true) {
            if (this.showingCoopHelp) {
                if (this.coopHelpOkButton != null && this.coopHelpOkButton.containsPoint(x, y)) {
                    this.showingCoopHelp = false;
                    Game1.playSound("bigDeSelect");
                    if (Game1.options.SnappyMenus) {
                        this.currentlySnappedComponent = (ClickableComponent)this.coopHelpButton;
                        this.snapCursorToCurrentSnappedComponent();
                    }
                }
                if (this.coopHelpScreen == 0 && this.coopHelpRightButton != null && this.coopHelpRightButton.containsPoint(x, y)) {
                    ++this.coopHelpScreen;
                    this.coopHelpString = Game1.parseText(Game1.content.LoadString("Strings\\UI:Character_CoopHelpString2").Replace("^", Environment.NewLine), Game1.dialogueFont, this.width + 384 - IClickableMenu.borderWidth * 2);
                    Game1.playSound("shwip");
                }
                if (this.coopHelpScreen != 1 || this.coopHelpLeftButton == null || !this.coopHelpLeftButton.containsPoint(x, y))
                    return;
                --this.coopHelpScreen;
                this.coopHelpString = Game1.parseText(Game1.content.LoadString("Strings\\UI:Character_CoopHelpString").Replace("^", Environment.NewLine), Game1.dialogueFont, this.width + 384 - IClickableMenu.borderWidth * 2);
                Game1.playSound("shwip");
            } else {
                if (this.genderButtons.Count > 0) {
                    foreach (ClickableComponent genderButton in this.genderButtons) {
                        if (genderButton.containsPoint(x, y)) {
                            this.optionButtonClick(genderButton.name);
                            genderButton.scale -= 0.5f;
                            genderButton.scale = Math.Max(3.5f, genderButton.scale);
                        }
                    }
                }
                if (this.farmTypeButtons.Count > 0) {
                    foreach (ClickableComponent farmTypeButton in this.farmTypeButtons) {
                        if (farmTypeButton.containsPoint(x, y) && !farmTypeButton.name.Contains("Gray")) {
                            this.optionButtonClick(farmTypeButton.name);
                            farmTypeButton.scale -= 0.5f;
                            farmTypeButton.scale = Math.Max(3.5f, farmTypeButton.scale);
                        }
                    }
                }
                if (this.petButtons.Count > 0) {
                    foreach (ClickableComponent petButton in this.petButtons) {
                        if (petButton.containsPoint(x, y)) {
                            this.optionButtonClick(petButton.name);
                            petButton.scale -= 0.5f;
                            petButton.scale = Math.Max(3.5f, petButton.scale);
                        }
                    }
                }
                if (this.cabinLayoutButtons.Count > 0) {
                    foreach (ClickableComponent cabinLayoutButton in this.cabinLayoutButtons) {
                        if (Game1.startingCabins > 0 && cabinLayoutButton.containsPoint(x, y)) {
                            this.optionButtonClick(cabinLayoutButton.name);
                            cabinLayoutButton.scale -= 0.5f;
                            cabinLayoutButton.scale = Math.Max(3.5f, cabinLayoutButton.scale);
                        }
                    }
                }
                if (this.leftSelectionButtons.Count > 0) {
                    foreach (ClickableComponent leftSelectionButton in this.leftSelectionButtons) {
                        if (leftSelectionButton.containsPoint(x, y)) {
                            this.selectionClick(leftSelectionButton.name, -1);
                            if ((double)leftSelectionButton.scale != 0.0) {
                                leftSelectionButton.scale -= 0.25f;
                                leftSelectionButton.scale = Math.Max(0.75f, leftSelectionButton.scale);
                            }
                        }
                    }
                }
                if (this.rightSelectionButtons.Count > 0) {
                    foreach (ClickableComponent rightSelectionButton in this.rightSelectionButtons) {
                        if (rightSelectionButton.containsPoint(x, y)) {
                            this.selectionClick(rightSelectionButton.name, 1);
                            if ((double)rightSelectionButton.scale != 0.0) {
                                rightSelectionButton.scale -= 0.25f;
                                rightSelectionButton.scale = Math.Max(0.75f, rightSelectionButton.scale);
                            }
                        }
                    }
                }

                // Up/Down Arrow Button & NoDebris Button
                if (downArrow.containsPoint(x, y) && currentItemIndex < Math.Max(0, allFarmButtons.Count - 6)) {
                    downArrowPressed();
                } else if (upArrow.containsPoint(x, y) && currentItemIndex > 0) {
                    upArrowPressed();
                } else if (noDebrisButton.containsPoint(x, y)) {
                    Game1.playSound("drumkit6");
                    noDebrisButton.sourceRect.X = (noDebrisButton.sourceRect.X == 227) ? 236 : 227;
                    customManager.NoDebris = !customManager.NoDebris;
                }

                // Holding the Scroll Bar
                if (scrollBar.containsPoint(x, y)) {
                    scrolling = true;
                }

                if (this.okButton.containsPoint(x, y) && this.canLeaveMenu()) {
                    this.optionButtonClick(this.okButton.name);
                    this.okButton.scale -= 0.25f;
                    this.okButton.scale = Math.Max(0.75f, this.okButton.scale);
                }
                if (this.hairColorPicker != null && this.hairColorPicker.containsPoint(x, y)) {
                    Color c = this.hairColorPicker.click(x, y);
                    if (this.source == CharacterCustomization.Source.DyePots) {
                        if (Game1.player.shirtItem.Value != null && Game1.player.shirtItem.Value.dyeable.Value) {
                            Game1.player.shirtItem.Value.clothesColor.Value = c;
                            Game1.player.FarmerRenderer.MarkSpriteDirty();
                            this._displayFarmer.FarmerRenderer.MarkSpriteDirty();
                        }
                    } else
                        Game1.player.changeHairColor(c);
                    this.lastHeldColorPicker = this.hairColorPicker;
                } else if (this.pantsColorPicker != null && this.pantsColorPicker.containsPoint(x, y)) {
                    Color color = this.pantsColorPicker.click(x, y);
                    if (this.source == CharacterCustomization.Source.DyePots) {
                        if (Game1.player.pantsItem.Value != null && Game1.player.pantsItem.Value.dyeable.Value) {
                            Game1.player.pantsItem.Value.clothesColor.Value = color;
                            Game1.player.FarmerRenderer.MarkSpriteDirty();
                            this._displayFarmer.FarmerRenderer.MarkSpriteDirty();
                        }
                    } else if (this.source == CharacterCustomization.Source.ClothesDye)
                        this.DyeItem(color);
                    else
                        Game1.player.changePants(color);
                    this.lastHeldColorPicker = this.pantsColorPicker;
                } else if (this.eyeColorPicker != null && this.eyeColorPicker.containsPoint(x, y)) {
                    Game1.player.changeEyeColor(this.eyeColorPicker.click(x, y));
                    this.lastHeldColorPicker = this.eyeColorPicker;
                }
                if (this.source != CharacterCustomization.Source.Wizard && this.source != CharacterCustomization.Source.Dresser && (this.source != CharacterCustomization.Source.ClothesDye && this.source != CharacterCustomization.Source.DyePots)) {
                    this.nameBox.Update();
                    if (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm)
                        this.farmnameBox.Update();
                    else
                        this.farmnameBox.Text = Game1.MasterPlayer.farmName.Value;
                    this.favThingBox.Update();
                    if ((this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm) && this.skipIntroButton.containsPoint(x, y)) {
                        Game1.playSound("drumkit6");
                        this.skipIntroButton.sourceRect.X = this.skipIntroButton.sourceRect.X == 227 ? 236 : 227;
                        this.skipIntro = !this.skipIntro;
                    }
                }
                if (this.coopHelpButton != null && this.coopHelpButton.containsPoint(x, y)) {
                    if (Game1.options.SnappyMenus) {
                        this.currentlySnappedComponent = (ClickableComponent)this.coopHelpOkButton;
                        this.snapCursorToCurrentSnappedComponent();
                    }
                    Game1.playSound("bigSelect");
                    this.showingCoopHelp = true;
                    this.coopHelpScreen = 0;
                    this.coopHelpString = Game1.parseText(Game1.content.LoadString("Strings\\UI:Character_CoopHelpString").Replace("^", Environment.NewLine), Game1.dialogueFont, this.width + 384 - IClickableMenu.borderWidth * 2);
                    this.helpStringSize = Game1.dialogueFont.MeasureString(this.coopHelpString);
                    this.coopHelpRightButton.bounds.Y = this.yPositionOnScreen + (int)this.helpStringSize.Y + IClickableMenu.borderWidth * 2 - 4;
                    this.coopHelpRightButton.bounds.X = this.xPositionOnScreen + (int)this.helpStringSize.X - IClickableMenu.borderWidth * 5;
                    this.coopHelpLeftButton.bounds.Y = this.yPositionOnScreen + (int)this.helpStringSize.Y + IClickableMenu.borderWidth * 2 - 4;
                    this.coopHelpLeftButton.bounds.X = this.xPositionOnScreen - IClickableMenu.borderWidth * 4;
                }
                if (!this.randomButton.containsPoint(x, y))
                    return;
                string cueName = "drumkit6";
                if (this.timesRandom > 0) {
                    switch (Game1.random.Next(15)) {
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
                ++this.timesRandom;
                if (this.accLabel != null && this.accLabel.visible) {
                    if (Game1.random.NextDouble() < 0.33) {
                        if (Game1.player.IsMale)
                            Game1.player.changeAccessory(Game1.random.Next(19));
                        else
                            Game1.player.changeAccessory(Game1.random.Next(6, 19));
                    } else
                        Game1.player.changeAccessory(-1);
                }
                if (this.hairLabel != null && this.hairLabel.visible) {
                    if (Game1.player.IsMale)
                        Game1.player.changeHairStyle(Game1.random.Next(16));
                    else
                        Game1.player.changeHairStyle(Game1.random.Next(16, 32));
                    Color c = new Color(Game1.random.Next(25, 254), Game1.random.Next(25, 254), Game1.random.Next(25, 254));
                    if (Game1.random.NextDouble() < 0.5) {
                        c.R /= (byte)2;
                        c.G /= (byte)2;
                        c.B /= (byte)2;
                    }
                    if (Game1.random.NextDouble() < 0.5)
                        c.R = (byte)Game1.random.Next(15, 50);
                    if (Game1.random.NextDouble() < 0.5)
                        c.G = (byte)Game1.random.Next(15, 50);
                    if (Game1.random.NextDouble() < 0.5)
                        c.B = (byte)Game1.random.Next(15, 50);
                    Game1.player.changeHairColor(c);
                }
                if (this.shirtLabel != null && this.shirtLabel.visible)
                    Game1.player.changeShirt(Game1.random.Next(112), false);
                if (this.skinLabel != null && this.skinLabel.visible) {
                    Game1.player.changeSkinColor(Game1.random.Next(6), false);
                    if (Game1.random.NextDouble() < 0.25)
                        Game1.player.changeSkinColor(Game1.random.Next(24), false);
                }
                if (this.pantsStyleLabel != null && this.pantsStyleLabel.visible) {
                    Color color = new Color(Game1.random.Next(25, 254), Game1.random.Next(25, 254), Game1.random.Next(25, 254));
                    if (Game1.random.NextDouble() < 0.5) {
                        color.R /= (byte)2;
                        color.G /= (byte)2;
                        color.B /= (byte)2;
                    }
                    if (Game1.random.NextDouble() < 0.5)
                        color.R = (byte)Game1.random.Next(15, 50);
                    if (Game1.random.NextDouble() < 0.5)
                        color.G = (byte)Game1.random.Next(15, 50);
                    if (Game1.random.NextDouble() < 0.5)
                        color.B = (byte)Game1.random.Next(15, 50);
                    Game1.player.changePants(color);
                    this.pantsColorPicker.setColor((Color)((NetFieldBase<Color, NetColor>)Game1.player.pantsColor));
                }
                if (this.eyeColorPicker != null) {
                    Color c = new Color(Game1.random.Next(25, 254), Game1.random.Next(25, 254), Game1.random.Next(25, 254));
                    c.R /= (byte)2;
                    c.G /= (byte)2;
                    c.B /= (byte)2;
                    if (Game1.random.NextDouble() < 0.5)
                        c.R = (byte)Game1.random.Next(15, 50);
                    if (Game1.random.NextDouble() < 0.5)
                        c.G = (byte)Game1.random.Next(15, 50);
                    if (Game1.random.NextDouble() < 0.5)
                        c.B = (byte)Game1.random.Next(15, 50);
                    Game1.player.changeEyeColor(c);
                    this.eyeColorPicker.setColor((Color)((NetFieldBase<Color, NetColor>)Game1.player.newEyeColor));
                }
                this.randomButton.scale = 3.5f;
            }
        }

        public void setScrollBarToCurrentIndex() {
            scrollBar.bounds.Y = scrollBarRunner.Height / Math.Max(1, allFarmButtons.Count - 5 + 1) * currentItemIndex + upArrow.bounds.Bottom + 5;
            if (currentItemIndex != allFarmButtons.Count - 5)
                return;
            scrollBar.bounds.Y = downArrow.bounds.Y - scrollBar.bounds.Height - 30;
        }

        public override void receiveScrollWheelAction(int direction) {
            base.receiveScrollWheelAction(direction);
            if (direction > 0 && currentItemIndex > 0) {
                upArrowPressed();
                return;
            }
            if (direction < 0 && currentItemIndex < Math.Max(0, allFarmButtons.Count - 5)) {
                downArrowPressed();
                return;
            }
        }

        private void upArrowPressed() {
            upArrow.scale = upArrow.baseScale;
            currentItemIndex--;
            Game1.playSound("shwip");
            setScrollBarToCurrentIndex();
        }

        private void downArrowPressed() {
            downArrow.scale = downArrow.baseScale;
            currentItemIndex++;
            Game1.playSound("shwip");
            setScrollBarToCurrentIndex();
        }

        public override void leftClickHeld(int x, int y) {
            // Scroll Bar
            if (scrolling) {
                int y2 = scrollBar.bounds.Y;
                scrollBar.bounds.Y = Math.Min(yPositionOnScreen + height - 64 - 12 - scrollBar.bounds.Height, Math.Max(y, yPositionOnScreen + upArrow.bounds.Height + 20));
                float percentage = (y - scrollBarRunner.Y) / scrollBarRunner.Height;
                currentItemIndex = Math.Min(farmTypeButtons.Count - 5, Math.Max(0, (int)(farmTypeButtons.Count * percentage)));
                setScrollBarToCurrentIndex();
                if (y2 != scrollBar.bounds.Y) {
                    Game1.playSound("shiny4");
                }
            }

            this.colorPickerTimer -= Game1.currentGameTime.ElapsedGameTime.Milliseconds;
            if (this.colorPickerTimer > 0)
                return;
            if (this.lastHeldColorPicker != null && !Game1.options.SnappyMenus) {
                if (this.lastHeldColorPicker.Equals((object)this.hairColorPicker)) {
                    Color c = this.hairColorPicker.clickHeld(x, y);
                    if (this.source == CharacterCustomization.Source.DyePots) {
                        if (Game1.player.shirtItem.Value != null && Game1.player.shirtItem.Value.dyeable.Value) {
                            Game1.player.shirtItem.Value.clothesColor.Value = c;
                            Game1.player.FarmerRenderer.MarkSpriteDirty();
                            this._displayFarmer.FarmerRenderer.MarkSpriteDirty();
                        }
                    } else
                        Game1.player.changeHairColor(c);
                }
                if (this.lastHeldColorPicker.Equals((object)this.pantsColorPicker)) {
                    Color color = this.pantsColorPicker.clickHeld(x, y);
                    if (this.source == CharacterCustomization.Source.DyePots) {
                        if (Game1.player.pantsItem.Value != null && Game1.player.pantsItem.Value.dyeable.Value) {
                            Game1.player.pantsItem.Value.clothesColor.Value = color;
                            Game1.player.FarmerRenderer.MarkSpriteDirty();
                            this._displayFarmer.FarmerRenderer.MarkSpriteDirty();
                        }
                    } else if (this.source == CharacterCustomization.Source.ClothesDye)
                        this.DyeItem(color);
                    else
                        Game1.player.changePants(color);
                }
                if (this.lastHeldColorPicker.Equals((object)this.eyeColorPicker))
                    Game1.player.changeEyeColor(this.eyeColorPicker.clickHeld(x, y));
            }
            this.colorPickerTimer = 100;
        }

        public override void releaseLeftClick(int x, int y) {
            if (this.hairColorPicker != null)
                this.hairColorPicker.releaseClick();
            if (this.pantsColorPicker != null)
                this.pantsColorPicker.releaseClick();
            if (this.eyeColorPicker != null)
                this.eyeColorPicker.releaseClick();
            this.lastHeldColorPicker = (ColorPicker)null;
        }

        public override void receiveRightClick(int x, int y, bool playSound = true) {
        }

        public override void receiveKeyPress(Keys key) {
            if (key == Keys.Tab) {
                if (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm) {
                    if (this.nameBox.Selected) {
                        this.farmnameBox.SelectMe();
                        this.nameBox.Selected = false;
                    } else if (this.farmnameBox.Selected) {
                        this.farmnameBox.Selected = false;
                        this.favThingBox.SelectMe();
                    } else {
                        this.favThingBox.Selected = false;
                        this.nameBox.SelectMe();
                    }
                } else if (this.source == CharacterCustomization.Source.NewFarmhand) {
                    if (this.nameBox.Selected) {
                        this.favThingBox.SelectMe();
                        this.nameBox.Selected = false;
                    } else {
                        this.favThingBox.Selected = false;
                        this.nameBox.SelectMe();
                    }
                }
            }
            if (!Game1.options.SnappyMenus || Game1.options.doesInputListContain(Game1.options.menuButton, key) || ((IEnumerable<Keys>)Game1.GetKeyboardState().GetPressedKeys()).Count<Keys>() != 0)
                return;
            base.receiveKeyPress(key);
        }

        public override void performHoverAction(int x, int y) {
            this.hoverText = "";
            this.hoverTitle = "";
            foreach (ClickableTextureComponent leftSelectionButton in this.leftSelectionButtons) {
                if (leftSelectionButton.containsPoint(x, y))
                    leftSelectionButton.scale = Math.Min(leftSelectionButton.scale + 0.02f, leftSelectionButton.baseScale + 0.1f);
                else
                    leftSelectionButton.scale = Math.Max(leftSelectionButton.scale - 0.02f, leftSelectionButton.baseScale);
                if (leftSelectionButton.name.Equals("Cabins") && Game1.startingCabins == 0)
                    leftSelectionButton.scale = 0.0f;
            }
            foreach (ClickableTextureComponent rightSelectionButton in this.rightSelectionButtons) {
                if (rightSelectionButton.containsPoint(x, y))
                    rightSelectionButton.scale = Math.Min(rightSelectionButton.scale + 0.02f, rightSelectionButton.baseScale + 0.1f);
                else
                    rightSelectionButton.scale = Math.Max(rightSelectionButton.scale - 0.02f, rightSelectionButton.baseScale);
                if (rightSelectionButton.name.Equals("Cabins") && Game1.startingCabins == 3)
                    rightSelectionButton.scale = 0.0f;
            }
            if (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm) {
                foreach (ClickableTextureComponent farmTypeButton in this.farmTypeButtons) {
                    if (farmTypeButton.containsPoint(x, y) && !farmTypeButton.name.Contains("Gray")) {
                        farmTypeButton.scale = Math.Min(farmTypeButton.scale + 0.02f, farmTypeButton.baseScale + 0.1f);
                        this.hoverTitle = farmTypeButton.hoverText.Split('_')[0];
                        this.hoverText = farmTypeButton.hoverText.Split('_')[1];
                    } else {
                        farmTypeButton.scale = Math.Max(farmTypeButton.scale - 0.02f, farmTypeButton.baseScale);
                        if (farmTypeButton.name.Contains("Gray") && farmTypeButton.containsPoint(x, y))
                            this.hoverText = "Reach level 10 " + Game1.content.LoadString("Strings\\UI:Character_" + farmTypeButton.name.Split('_')[1]) + " to unlock.";
                    }
                }
            }
            foreach (ClickableTextureComponent genderButton in this.genderButtons) {
                if (genderButton.containsPoint(x, y))
                    genderButton.scale = Math.Min(genderButton.scale + 0.05f, genderButton.baseScale + 0.5f);
                else
                    genderButton.scale = Math.Max(genderButton.scale - 0.05f, genderButton.baseScale);
            }
            if (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm) {
                foreach (ClickableTextureComponent petButton in this.petButtons) {
                    if (petButton.containsPoint(x, y))
                        petButton.scale = Math.Min(petButton.scale + 0.05f, petButton.baseScale + 0.5f);
                    else
                        petButton.scale = Math.Max(petButton.scale - 0.05f, petButton.baseScale);
                }
                foreach (ClickableTextureComponent cabinLayoutButton in this.cabinLayoutButtons) {
                    if (Game1.startingCabins > 0 && cabinLayoutButton.containsPoint(x, y)) {
                        cabinLayoutButton.scale = Math.Min(cabinLayoutButton.scale + 0.05f, cabinLayoutButton.baseScale + 0.5f);
                        this.hoverText = cabinLayoutButton.hoverText;
                    } else
                        cabinLayoutButton.scale = Math.Max(cabinLayoutButton.scale - 0.05f, cabinLayoutButton.baseScale);
                }
            }
            if (this.okButton.containsPoint(x, y) && this.canLeaveMenu())
                this.okButton.scale = Math.Min(this.okButton.scale + 0.02f, this.okButton.baseScale + 0.1f);
            else
                this.okButton.scale = Math.Max(this.okButton.scale - 0.02f, this.okButton.baseScale);
            if (this.coopHelpButton != null) {
                if (this.coopHelpButton.containsPoint(x, y)) {
                    this.coopHelpButton.scale = Math.Min(this.coopHelpButton.scale + 0.05f, this.coopHelpButton.baseScale + 0.5f);
                    this.hoverText = this.coopHelpButton.hoverText;
                } else
                    this.coopHelpButton.scale = Math.Max(this.coopHelpButton.scale - 0.05f, this.coopHelpButton.baseScale);
            }
            if (this.coopHelpOkButton != null) {
                if (this.coopHelpOkButton.containsPoint(x, y))
                    this.coopHelpOkButton.scale = Math.Min(this.coopHelpOkButton.scale + 0.025f, this.coopHelpOkButton.baseScale + 0.2f);
                else
                    this.coopHelpOkButton.scale = Math.Max(this.coopHelpOkButton.scale - 0.025f, this.coopHelpOkButton.baseScale);
            }
            if (this.coopHelpRightButton != null) {
                if (this.coopHelpRightButton.containsPoint(x, y))
                    this.coopHelpRightButton.scale = Math.Min(this.coopHelpRightButton.scale + 0.025f, this.coopHelpRightButton.baseScale + 0.2f);
                else
                    this.coopHelpRightButton.scale = Math.Max(this.coopHelpRightButton.scale - 0.025f, this.coopHelpRightButton.baseScale);
            }
            if (this.coopHelpLeftButton != null) {
                if (this.coopHelpLeftButton.containsPoint(x, y))
                    this.coopHelpLeftButton.scale = Math.Min(this.coopHelpLeftButton.scale + 0.025f, this.coopHelpLeftButton.baseScale + 0.2f);
                else
                    this.coopHelpLeftButton.scale = Math.Max(this.coopHelpLeftButton.scale - 0.025f, this.coopHelpLeftButton.baseScale);
            }
            this.randomButton.tryHover(x, y, 0.25f);
            this.randomButton.tryHover(x, y, 0.25f);
            if (this.hairColorPicker != null && this.hairColorPicker.containsPoint(x, y) || this.pantsColorPicker != null && this.pantsColorPicker.containsPoint(x, y) || this.eyeColorPicker != null && this.eyeColorPicker.containsPoint(x, y))
                Game1.SetFreeCursorDrag();
            this.nameBox.Hover(x, y);
            this.farmnameBox.Hover(x, y);
            this.favThingBox.Hover(x, y);
            this.skipIntroButton.tryHover(x, y, 0.1f);
        }

        public bool canLeaveMenu() {
            if (this.source == CharacterCustomization.Source.ClothesDye || this.source == CharacterCustomization.Source.DyePots || this.source == CharacterCustomization.Source.Wizard)
                return true;
            if (Game1.player.Name.Length > 0 && Game1.player.farmName.Length > 0)
                return Game1.player.favoriteThing.Length > 0;
            return false;
        }

        private string getNameOfDifficulty() {
            if ((double)Game1.player.difficultyModifier < 0.5)
                return this.superDiffString;
            if ((double)Game1.player.difficultyModifier < 0.75)
                return this.hardDiffString;
            if ((double)Game1.player.difficultyModifier < 1.0)
                return this.toughDiffString;
            return this.normalDiffString;
        }

        public override void draw(SpriteBatch b) {
            bool ignoreTitleSafe = true;
            if (this.showingCoopHelp) {
                IClickableMenu.drawTextureBox(b, this.xPositionOnScreen - 192, this.yPositionOnScreen + 64, (int)this.helpStringSize.X + IClickableMenu.borderWidth * 2, (int)this.helpStringSize.Y + IClickableMenu.borderWidth * 2, Color.White);
                Utility.drawTextWithShadow(b, this.coopHelpString, Game1.dialogueFont, new Vector2((float)(this.xPositionOnScreen + IClickableMenu.borderWidth - 192), (float)(this.yPositionOnScreen + IClickableMenu.borderWidth + 64)), Game1.textColor, 1f, -1f, -1, -1, 1f, 3);
                if (this.coopHelpOkButton != null)
                    this.coopHelpOkButton.draw(b, Color.White, 0.95f, 0);
                if (this.coopHelpRightButton != null)
                    this.coopHelpRightButton.draw(b, Color.White, 0.95f, 0);
                if (this.coopHelpLeftButton != null)
                    this.coopHelpLeftButton.draw(b, Color.White, 0.95f, 0);
                this.drawMouse(b);
            } else {
                Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, false, true, (string)null, false, ignoreTitleSafe, -1, -1, -1);
                if (this.source == CharacterCustomization.Source.HostNewFarm) {
                    IClickableMenu.drawTextureBox(b, this.xPositionOnScreen - 256 + 4 - (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko ? 25 : 0), this.yPositionOnScreen + IClickableMenu.borderWidth * 2 + 68, LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko ? 320 : 256, 512, Color.White);
                    foreach (ClickableTextureComponent cabinLayoutButton in this.cabinLayoutButtons) {
                        cabinLayoutButton.draw(b, Color.White * (Game1.startingCabins > 0 ? 1f : 0.5f), 0.9f, 0);
                        if (Game1.startingCabins > 0 && (cabinLayoutButton.name.Equals("Close") && !Game1.cabinsSeparate || cabinLayoutButton.name.Equals("Separate") && Game1.cabinsSeparate))
                            b.Draw(Game1.mouseCursors, cabinLayoutButton.bounds, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 34, -1, -1)), Color.White);
                    }
                }
                b.Draw(Game1.daybg, new Vector2((float)this.portraitBox.X, (float)this.portraitBox.Y), Color.White);
                foreach (ClickableTextureComponent genderButton in this.genderButtons) {
                    if (genderButton.visible) {
                        genderButton.draw(b);
                        if (genderButton.name.Equals("Male") && Game1.player.IsMale || genderButton.name.Equals("Female") && !Game1.player.IsMale)
                            b.Draw(Game1.mouseCursors, genderButton.bounds, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 34, -1, -1)), Color.White);
                    }
                }
                foreach (ClickableTextureComponent petButton in this.petButtons) {
                    if (petButton.visible) {
                        petButton.draw(b);
                        if (petButton.name.Equals("Cat") && Game1.player.catPerson || petButton.name.Equals("Dog") && !Game1.player.catPerson)
                            b.Draw(Game1.mouseCursors, petButton.bounds, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 34, -1, -1)), Color.White);
                    }
                }
                if (this.nameBoxCC.visible)
                    Game1.player.Name = this.nameBox.Text;
                if (this.favThingBoxCC.visible)
                    Game1.player.favoriteThing.Value = this.favThingBox.Text;
                if (this.farmnameBoxCC.visible)
                    Game1.player.farmName.Value = this.farmnameBox.Text;
                if (this.source == CharacterCustomization.Source.NewFarmhand)
                    Game1.player.farmName.Value = Game1.MasterPlayer.farmName.Value;
                foreach (ClickableTextureComponent leftSelectionButton in this.leftSelectionButtons)
                    leftSelectionButton.draw(b);
                foreach (ClickableComponent label in this.labels) {
                    if (label.visible) {
                        string text = "";
                        float num1 = 0.0f;
                        float num2 = 0.0f;
                        Color color = Game1.textColor;
                        if (label == this.nameLabel)
                            color = Game1.player.Name == null || Game1.player.Name.Length >= 1 ? Game1.textColor : Color.Red;
                        else if (label == this.farmLabel)
                            color = Game1.player.farmName.Value == null || Game1.player.farmName.Length >= 1 ? Game1.textColor : Color.Red;
                        else if (label == this.favoriteLabel)
                            color = Game1.player.favoriteThing.Value == null || Game1.player.favoriteThing.Length >= 1 ? Game1.textColor : Color.Red;
                        else if (label == this.shirtLabel) {
                            num1 = (float)(21.0 - (double)Game1.smallFont.MeasureString(label.name).X / 2.0);
                            text = string.Concat((object)((int)((NetFieldBase<int, NetInt>)Game1.player.shirt) + 1));
                        } else if (label == this.skinLabel) {
                            num1 = (float)(21.0 - (double)Game1.smallFont.MeasureString(label.name).X / 2.0);
                            text = string.Concat((object)((int)((NetFieldBase<int, NetInt>)Game1.player.skin) + 1));
                        } else if (label == this.hairLabel) {
                            num1 = (float)(21.0 - (double)Game1.smallFont.MeasureString(label.name).X / 2.0);
                            if (!label.name.Contains("Color"))
                                text = string.Concat((object)((int)((NetFieldBase<int, NetInt>)Game1.player.hair) + 1));
                        } else if (label == this.accLabel) {
                            num1 = (float)(21.0 - (double)Game1.smallFont.MeasureString(label.name).X / 2.0);
                            text = string.Concat((object)((int)((NetFieldBase<int, NetInt>)Game1.player.accessory) + 2));
                        } else if (label == this.pantsStyleLabel) {
                            num1 = (float)(21.0 - (double)Game1.smallFont.MeasureString(label.name).X / 2.0);
                            text = string.Concat((object)((int)((NetFieldBase<int, NetInt>)Game1.player.pants) + 1));
                        } else if (label == this.startingCabinsLabel) {
                            num1 = (float)(21.0 - (double)Game1.smallFont.MeasureString(label.name).X / 2.0);
                            text = Game1.startingCabins != 0 || this.noneString == null ? string.Concat((object)Game1.startingCabins) : this.noneString;
                            num2 = 4f;
                        } else if (label == this.difficultyModifierLabel) {
                            num1 = (float)(21.0 - (double)Game1.smallFont.MeasureString(label.name).X / 2.0);
                            num2 = 4f;
                            text = this.getNameOfDifficulty();
                        } else if (label == this.separateWalletLabel) {
                            num1 = (float)(21.0 - (double)Game1.smallFont.MeasureString(label.name).X / 2.0);
                            num2 = 4f;
                            text = (bool)((NetFieldBase<bool, NetBool>)Game1.player.team.useSeparateWallets) ? this.separateWalletString : this.sharedWalletString;
                        } else
                            color = Game1.textColor;
                        Utility.drawTextWithShadow(b, label.name, Game1.smallFont, new Vector2((float)label.bounds.X + num1, (float)label.bounds.Y), color, 1f, -1f, -1, -1, 1f, 3);
                        if (text.Length > 0)
                            Utility.drawTextWithShadow(b, text, Game1.smallFont, new Vector2((float)(label.bounds.X + 21) - Game1.smallFont.MeasureString(text).X / 2f, (float)(label.bounds.Y + 32) + num2), color, 1f, -1f, -1, -1, 1f, 3);
                    }
                }
                foreach (ClickableTextureComponent rightSelectionButton in this.rightSelectionButtons)
                    rightSelectionButton.draw(b);


                //if (this.farmTypeButtons.Count > 0) {
                //    IClickableMenu.drawTextureBox(b, this.farmTypeButtons[0].bounds.X - 16, this.farmTypeButtons[0].bounds.Y - 20, 120, 564, Color.White);
                //    for (int index = 0; index < this.farmTypeButtons.Count; ++index) {
                //        this.farmTypeButtons[index].draw(b, this.farmTypeButtons[index].name.Contains("Gray") ? Color.Black * 0.5f : Color.White, 0.88f, 0);
                //        if (this.farmTypeButtons[index].name.Contains("Gray"))
                //            b.Draw(Game1.mouseCursors, new Vector2((float)(this.farmTypeButtons[index].bounds.Center.X - 12), (float)(this.farmTypeButtons[index].bounds.Center.Y - 8)), new Rectangle?(new Rectangle(107, 442, 7, 8)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.89f);
                //        if (index == Game1.whichFarm)
                //            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(375, 357, 3, 3), this.farmTypeButtons[index].bounds.X, this.farmTypeButtons[index].bounds.Y - 4, this.farmTypeButtons[index].bounds.Width, this.farmTypeButtons[index].bounds.Height + 8, Color.White, 4f, false);
                //    }
                //}

                // Farm Buttons
                if (this.allFarmButtons.Count > 0) {
                    Point baseFarmButton = new Point(this.xPositionOnScreen + this.width + 4 + 8, this.yPositionOnScreen + IClickableMenu.borderWidth * 2);
                    IClickableMenu.drawTextureBox(b, this.farmTypeButtons[0].bounds.X - 16, this.farmTypeButtons[0].bounds.Y - 20, 120, 476, Color.White);

                    if (previousItemIndex != currentItemIndex) {
                        int count = 0;
                        farmTypeButtons.Clear();
                        while (count < 6) {
                            farmTypeButtons.Add(allFarmButtons[currentItemIndex + count]);
                            farmTypeButtons[count].bounds = new Rectangle(baseFarmButton.X, baseFarmButton.Y + (88 * (1 + count)), 88, 80);
                            farmTypeButtons[count].myID = 531 + count;
                            farmTypeButtons[count].downNeighborID = (count == 4) ? 505 : 532 + count;
                            farmTypeButtons[count].upNeighborID = (count == 0) ? 0 : 531 + (count - 1);
                            farmTypeButtons[count].leftNeighborID = (count == 0) ? 537 : (count == 1) ? 510 : (count == 2) ? 522 : (count == 3) ? 525 : 528;
                            farmTypeButtons[count].rightNeighborID = (count == 0) ? 0 : 81114;
                            count++;
                        }
                    }
                    previousItemIndex = currentItemIndex;

                    for (int i = 0; i < 6; i++) {
                        this.farmTypeButtons[i].draw(b, this.farmTypeButtons[i].name.Contains("Gray") ? (Color.Black * 0.5f) : Color.White, 0.88f);
                        if (this.farmTypeButtons[i].name.Contains("Gray")) {
                            b.Draw(Game1.mouseCursors, new Vector2((float)(this.farmTypeButtons[i].bounds.Center.X - 12), (float)(this.farmTypeButtons[i].bounds.Center.Y - 8)), new Rectangle?(new Rectangle(107, 442, 7, 8)), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.89f);
                        }
                        if (farmTypeButtons[i].name == lastClickedFarmTypeBtn) {
                            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(375, 357, 3, 3), this.farmTypeButtons[i].bounds.X, this.farmTypeButtons[i].bounds.Y - 4, this.farmTypeButtons[i].bounds.Width, this.farmTypeButtons[i].bounds.Height + 8, Color.White, 4f, false);
                        }
                    }
                }

                // Scroll Bar and Up/Down Arrows
                upArrow.draw(b);
                downArrow.draw(b);
                scrollBar.draw(b);
                if (allFarmButtons.Count > 5) {
                    drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 383, 6, 6), scrollBarRunner.X, scrollBarRunner.Y, scrollBarRunner.Width, scrollBarRunner.Height, Color.White, 4f, false);
                    scrollBar.draw(b);
                }
                setScrollBarToCurrentIndex();

                // No Debris Button
                noDebrisButton.draw(b);
                Utility.drawTextWithShadow(b, "No Debris", Game1.smallFont, new Vector2(noDebrisButton.bounds.X + noDebrisButton.bounds.Width - 20, noDebrisButton.bounds.Y + 8), Game1.textColor, 1f, -1f, -1, -1, 1f, 3);

                if (this.petPortraitBox.HasValue)
                    b.Draw(Game1.mouseCursors, this.petPortraitBox.Value, new Rectangle?(new Rectangle(160 + (Game1.player.catPerson ? 0 : 48) + Game1.player.whichPetBreed * 16, 208, 16, 16)), Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.89f);
                if (this.canLeaveMenu()) {
                    this.okButton.draw(b, Color.White, 0.75f, 0);
                } else {
                    this.okButton.draw(b, Color.White, 0.75f, 0);
                    this.okButton.draw(b, Color.Black * 0.5f, 0.751f, 0);
                }
                if (this.coopHelpButton != null)
                    this.coopHelpButton.draw(b, Color.White, 0.75f, 0);
                if (this.hairColorPicker != null)
                    this.hairColorPicker.draw(b);
                if (this.pantsColorPicker != null)
                    this.pantsColorPicker.draw(b);
                if (this.eyeColorPicker != null)
                    this.eyeColorPicker.draw(b);
                if (this.source != CharacterCustomization.Source.Wizard && this.source != CharacterCustomization.Source.Dresser && (this.source != CharacterCustomization.Source.DyePots && this.source != CharacterCustomization.Source.ClothesDye)) {
                    this.nameBox.Draw(b, true);
                    this.favThingBox.Draw(b, true);
                }
                if (this.farmnameBoxCC.visible) {
                    this.farmnameBox.Draw(b, true);
                    Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\UI:Character_FarmNameSuffix"), Game1.smallFont, new Vector2((float)(this.farmnameBox.X + this.farmnameBox.Width + 8), (float)(this.farmnameBox.Y + 12)), Game1.textColor, 1f, -1f, -1, -1, 1f, 3);
                }
                if (this.skipIntroButton != null && this.skipIntroButton.visible) {
                    this.skipIntroButton.draw(b);
                    Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\UI:Character_SkipIntro"), Game1.smallFont, new Vector2((float)(this.skipIntroButton.bounds.X + this.skipIntroButton.bounds.Width + 8), (float)(this.skipIntroButton.bounds.Y + 8)), Game1.textColor, 1f, -1f, -1, -1, 1f, 3);
                }
                this.randomButton.draw(b);
                b.End();
                b.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, (DepthStencilState)null, (RasterizerState)null);
                this._displayFarmer.FarmerRenderer.draw(b, this._displayFarmer.FarmerSprite.CurrentAnimationFrame, this._displayFarmer.FarmerSprite.CurrentFrame, this._displayFarmer.FarmerSprite.SourceRect, new Vector2((float)(this.portraitBox.Center.X - 32), (float)(this.portraitBox.Bottom - 160)), Vector2.Zero, 0.8f, Color.White, 0.0f, 1f, this._displayFarmer);
                b.End();
                b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, (DepthStencilState)null, (RasterizerState)null);
                if (this.hoverText != null && this.hoverTitle != null && this.hoverText.Count<char>() > 0)
                    IClickableMenu.drawHoverText(b, Game1.parseText(this.hoverText, Game1.smallFont, 256), Game1.smallFont, 0, 0, -1, this.hoverTitle, -1, (string[])null, (Item)null, 0, -1, -1, -1, -1, 1f, (CraftingRecipe)null, (IList<Item>)null);
                this.drawMouse(b);
            }
        }

        public override void emergencyShutDown() {
            if (this._itemToDye != null) {
                if (!Game1.player.IsEquippedItem((Item)this._itemToDye))
                    Utility.CollectOrDrop((Item)this._itemToDye);
                this._itemToDye = (Clothing)null;
            }
            base.emergencyShutDown();
        }

        public override bool IsAutomaticSnapValid(
            int direction,
            ClickableComponent a,
            ClickableComponent b) {
            if (a.region != b.region || this.source == CharacterCustomization.Source.Wizard && (a.name == "Direction" && b.name == "Pet" || b.name == "Direction" && a.name == "Pet") || this.randomButton != null && (a == this.randomButton && b.name != "Direction" || b == this.randomButton && a.name != "Direction"))
                return false;
            return base.IsAutomaticSnapValid(direction, a, b);
        }

        public override void update(GameTime time) {
            base.update(time);
            if (this.showingCoopHelp) {
                this.backButton.visible = false;
                if (this.coopHelpScreen == 0) {
                    this.coopHelpRightButton.visible = true;
                    this.coopHelpLeftButton.visible = false;
                } else if (this.coopHelpScreen == 1) {
                    this.coopHelpRightButton.visible = false;
                    this.coopHelpLeftButton.visible = true;
                }
            } else
                this.backButton.visible = this._shouldShowBackButton;
            if (this._sliderOpTarget == null)
                return;
            Color selectedColor = this._sliderOpTarget.getSelectedColor();
            if (this._sliderOpTarget.Dirty && this._sliderOpTarget.LastColor == selectedColor) {
                this._sliderAction();
                this._sliderOpTarget.LastColor = this._sliderOpTarget.getSelectedColor();
                this._sliderOpTarget.Dirty = false;
                this._sliderOpTarget = (ColorPicker)null;
            } else
                this._sliderOpTarget.LastColor = selectedColor;
        }

        protected override bool _ShouldAutoSnapPrioritizeAlignedElements() {
            return true;
        }
    }
}