using System.Collections.Generic;
using System.Linq;
using JoysOfEfficiency.OptionsElements;
using JoysOfEfficiency.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace JoysOfEfficiency
{
    internal class JoeMenu : IClickableMenu
    {
        private readonly ModEntry _mod;

        private readonly List<MenuTab> _tabs = new List<MenuTab>();

        private readonly ClickableTextureComponent _upCursor;
        private readonly ClickableTextureComponent _downCursor;

        private Rectangle _tabAutomation;
        private Rectangle _tabUIs;
        private Rectangle _tabMisc;
        private Rectangle _tabControls;

        private bool _isScrolling;

        private Rectangle _scrollBarRunner;
        private readonly ClickableTextureComponent _scrollBar;

        private int _tabIndex;
        private int _firstIndex;

        private readonly SpriteFont _font = Game1.smallFont;
        private readonly string _tabAutomationString;
        private readonly string _tabUIsString;
        private readonly string _tabMiscString;
        private readonly string _tabControlsString;

        private bool _isListening;
        private bool _isListeningClick;

        private bool _isFirstTime;

        private ModifiedInputListener _listener;
        private ModifiedClickListener _clickListener;

        internal JoeMenu(int width, int height, ModEntry mod) : base(Game1.viewport.Width / 2 - width / 2, Game1.viewport.Height / 2 - height / 2, width, height, true)
        {
            _mod = mod;
            ITranslationHelper translation = mod.Helper.Translation;
            _upCursor = new ClickableTextureComponent("up-arrow", new Rectangle(xPositionOnScreen + this.width + Game1.tileSize / 4, yPositionOnScreen + Game1.tileSize, 11 * Game1.pixelZoom, 12 * Game1.pixelZoom), "", "", Game1.mouseCursors, new Rectangle(421, 459, 11, 12), Game1.pixelZoom);
            _downCursor = new ClickableTextureComponent("down-arrow", new Rectangle(xPositionOnScreen + this.width + Game1.tileSize / 4, yPositionOnScreen + this.height - Game1.tileSize, 11 * Game1.pixelZoom, 12 * Game1.pixelZoom), "", "", Game1.mouseCursors, new Rectangle(421, 472, 11, 12), Game1.pixelZoom);

            _scrollBar = new ClickableTextureComponent(new Rectangle(_upCursor.bounds.X + 12, _upCursor.bounds.Y + _upCursor.bounds.Height + 4, 24, 40), Game1.mouseCursors, new Rectangle(435, 463, 6, 10), 4f);
            _scrollBarRunner = new Rectangle(_scrollBar.bounds.X, _upCursor.bounds.Y + _upCursor.bounds.Height + 4, _scrollBar.bounds.Width, height - 128 - _upCursor.bounds.Height - 8);

            _tabAutomationString = translation.Get("tab.automation");
            Vector2 size = _font.MeasureString(_tabAutomationString);
            _tabAutomation = new Rectangle(xPositionOnScreen - (int)size.X - 20, yPositionOnScreen, (int)size.X + 32, 64);

            _tabUIsString = translation.Get("tab.UIs");
            size = _font.MeasureString(_tabUIsString);
            _tabUIs = new Rectangle(xPositionOnScreen - (int)size.X - 20, yPositionOnScreen + 68, (int)size.X + 32, 64);

            _tabMiscString = translation.Get("tab.misc");
            size = _font.MeasureString(_tabMiscString);
            _tabMisc = new Rectangle(xPositionOnScreen - (int)size.X - 20, yPositionOnScreen + 68*2, (int)size.X + 32, 64);

            _tabControlsString = translation.Get("tab.controls");
            size = _font.MeasureString(_tabControlsString);
            _tabControls = new Rectangle(xPositionOnScreen - (int)size.X - 20, yPositionOnScreen + 68*3, (int)size.X + 32, 64);

            {
                //Automation Tab
                MenuTab tab = new MenuTab();
                tab.AddOptionsElement(new EmptyLabel());
                tab.AddOptionsElement(new LabelComponent("Balanced Mode"));
                tab.AddOptionsElement(new ModifiedCheckBox("BalancedMode", 20, ModEntry.Conf.BalancedMode, OnCheckboxValueChanged));

                tab.AddOptionsElement(new EmptyLabel());
                tab.AddOptionsElement(new LabelComponent("Auto Water Nearby Crops"));
                tab.AddOptionsElement(new ModifiedCheckBox("AutoWaterNearbyCrops", 2, ModEntry.Conf.AutoWaterNearbyCrops, OnCheckboxValueChanged));
                tab.AddOptionsElement(new ModifiedSlider("AutoWaterRadius", 3, ModEntry.Conf.AutoWaterRadius, 1, 3, OnSliderValueChanged, () => !ModEntry.Conf.AutoWaterNearbyCrops || ModEntry.Conf.BalancedMode));
                tab.AddOptionsElement(new ModifiedCheckBox("FindCanFromInventory", 16, ModEntry.Conf.FindCanFromInventory, OnCheckboxValueChanged, i => !(ModEntry.Conf.AutoWaterNearbyCrops || ModEntry.Conf.AutoRefillWateringCan)));

                tab.AddOptionsElement(new EmptyLabel());
                tab.AddOptionsElement(new LabelComponent("Auto Pet Nearby Animals/Pets"));
                tab.AddOptionsElement(new ModifiedCheckBox("AutoPetNearbyAnimals", 3, ModEntry.Conf.AutoPetNearbyAnimals, OnCheckboxValueChanged));
                tab.AddOptionsElement(new ModifiedCheckBox("AutoPetNearbyPets", 24, ModEntry.Conf.AutoPetNearbyPets, OnCheckboxValueChanged));
                tab.AddOptionsElement(new ModifiedSlider("AutoPetRadius", 4, ModEntry.Conf.AutoPetRadius, 1, 3, OnSliderValueChanged, () => !ModEntry.Conf.AutoPetNearbyAnimals || ModEntry.Conf.BalancedMode));

                tab.AddOptionsElement(new EmptyLabel());
                tab.AddOptionsElement(new LabelComponent("Auto Animal Door"));
                tab.AddOptionsElement(new ModifiedCheckBox("AutoAnimalDoor", 4, ModEntry.Conf.AutoAnimalDoor, OnCheckboxValueChanged));

                tab.AddOptionsElement(new EmptyLabel());
                tab.AddOptionsElement(new LabelComponent("Auto Fishing"));
                tab.AddOptionsElement(new ModifiedCheckBox("AutoFishing", 5, ModEntry.Conf.AutoFishing, OnCheckboxValueChanged));
                tab.AddOptionsElement(new ModifiedSlider("CPUThresholdFishing", 0, (int)(ModEntry.Conf.CpuThresholdFishing * 10), 0, 5, OnSliderValueChanged, () => !ModEntry.Conf.AutoFishing, Format));

                tab.AddOptionsElement(new EmptyLabel());
                tab.AddOptionsElement(new LabelComponent("Fishing Tweaks"));
                tab.AddOptionsElement(new ModifiedCheckBox("AutoReelRod", 6, ModEntry.Conf.AutoReelRod, OnCheckboxValueChanged));

                tab.AddOptionsElement(new EmptyLabel());
                tab.AddOptionsElement(new LabelComponent("Auto Gate"));
                tab.AddOptionsElement(new ModifiedCheckBox("AutoGate", 9, ModEntry.Conf.AutoGate, OnCheckboxValueChanged));

                tab.AddOptionsElement(new EmptyLabel());
                tab.AddOptionsElement(new LabelComponent("Auto Eat"));
                tab.AddOptionsElement(new ModifiedCheckBox("AutoEat", 10, ModEntry.Conf.AutoEat, OnCheckboxValueChanged));
                tab.AddOptionsElement(new ModifiedSlider("StaminaToEatRatio", 1, (int)(ModEntry.Conf.StaminaToEatRatio * 10), 1, 8, OnSliderValueChanged, () => !ModEntry.Conf.AutoEat, Format));
                tab.AddOptionsElement(new ModifiedSlider("HealthToEatRatio", 2, (int)(ModEntry.Conf.HealthToEatRatio * 10), 1, 8, OnSliderValueChanged, () => !ModEntry.Conf.AutoEat, Format));

                tab.AddOptionsElement(new EmptyLabel());
                tab.AddOptionsElement(new LabelComponent("Auto Harvest"));
                tab.AddOptionsElement(new ModifiedCheckBox("AutoHarvest", 11, ModEntry.Conf.AutoHarvest, OnCheckboxValueChanged));
                tab.AddOptionsElement(new ModifiedSlider("AutoHarvestRadius", 5, ModEntry.Conf.AutoHarvestRadius, 1, 3, OnSliderValueChanged, () => !ModEntry.Conf.AutoHarvest || ModEntry.Conf.BalancedMode));
                tab.AddOptionsElement(new ModifiedCheckBox("ProtectNectarProducingFlower", 25, ModEntry.Conf.ProtectNectarProducingFlower, OnCheckboxValueChanged, i => !ModEntry.Conf.AutoHarvest));

                tab.AddOptionsElement(new EmptyLabel());
                tab.AddOptionsElement(new LabelComponent("Auto Destroy Dead Crops"));
                tab.AddOptionsElement(new ModifiedCheckBox("AutoDestroyDeadCrops", 12, ModEntry.Conf.AutoDestroyDeadCrops, OnCheckboxValueChanged));

                tab.AddOptionsElement(new EmptyLabel());
                tab.AddOptionsElement(new LabelComponent("Auto Refill Watering Can"));
                tab.AddOptionsElement(new ModifiedCheckBox("AutoRefillWateringCan", 13, ModEntry.Conf.AutoRefillWateringCan, OnCheckboxValueChanged));

                tab.AddOptionsElement(new EmptyLabel());
                tab.AddOptionsElement(new LabelComponent("Auto Collect Collectibles"));
                tab.AddOptionsElement(new ModifiedCheckBox("AutoCollectCollectibles", 14, ModEntry.Conf.AutoCollectCollectibles, OnCheckboxValueChanged));
                tab.AddOptionsElement(new ModifiedSlider("AutoCollectRadius", 6, ModEntry.Conf.AutoCollectRadius, 1, 3, OnSliderValueChanged, () => !ModEntry.Conf.AutoCollectCollectibles || ModEntry.Conf.BalancedMode));

                tab.AddOptionsElement(new EmptyLabel());
                tab.AddOptionsElement(new LabelComponent("Auto Shake Fruited Plants"));
                tab.AddOptionsElement(new ModifiedCheckBox("AutoShakeFruitedPlants", 15, ModEntry.Conf.AutoShakeFruitedPlants, OnCheckboxValueChanged));
                tab.AddOptionsElement(new ModifiedSlider("AutoShakeRadius", 7, ModEntry.Conf.AutoShakeRadius, 1, 3, OnSliderValueChanged, () => !ModEntry.Conf.AutoShakeFruitedPlants || ModEntry.Conf.BalancedMode));

                tab.AddOptionsElement(new EmptyLabel());
                tab.AddOptionsElement(new LabelComponent("Auto Dig Artifact Spot"));
                tab.AddOptionsElement(new ModifiedCheckBox("AutoDigArtifactSpot", 17, ModEntry.Conf.AutoDigArtifactSpot, OnCheckboxValueChanged));
                tab.AddOptionsElement(new ModifiedSlider("AutoDigRadius", 8, ModEntry.Conf.AutoDigRadius, 1, 3, OnSliderValueChanged, () => !ModEntry.Conf.AutoDigArtifactSpot || ModEntry.Conf.BalancedMode));
                tab.AddOptionsElement(new ModifiedCheckBox("FindHoeFromInventory", 18, ModEntry.Conf.FindHoeFromInventory, OnCheckboxValueChanged, i => !ModEntry.Conf.AutoDigArtifactSpot));

                tab.AddOptionsElement(new EmptyLabel());
                tab.AddOptionsElement(new LabelComponent("Auto Deposit/Pull Machines"));
                tab.AddOptionsElement(new ModifiedCheckBox("AutoDepositIngredient", 22, ModEntry.Conf.AutoDepositIngredient, OnCheckboxValueChanged));
                tab.AddOptionsElement(new ModifiedCheckBox("AutoPullMachineResult", 23, ModEntry.Conf.AutoPullMachineResult, OnCheckboxValueChanged));
                tab.AddOptionsElement(new ModifiedSlider("MachineRadius", 10, ModEntry.Conf.MachineRadius, 1, 3, OnSliderValueChanged, () => !(ModEntry.Conf.AutoPullMachineResult || ModEntry.Conf.AutoDepositIngredient) || ModEntry.Conf.BalancedMode));

                tab.AddOptionsElement(new EmptyLabel());
                tab.AddOptionsElement(new LabelComponent("Auto Loot Treasures"));
                tab.AddOptionsElement(new ModifiedCheckBox("AutoLootTreasures", 30, ModEntry.Conf.AutoLootTreasures, OnCheckboxValueChanged));
                tab.AddOptionsElement(new ModifiedCheckBox("CloseTreasureWhenAllLooted", 31, ModEntry.Conf.CloseTreasureWhenAllLooted, OnCheckboxValueChanged));


                tab.AddOptionsElement(new EmptyLabel());
                tab.AddOptionsElement(new LabelComponent("Auto Pick Up Trash"));
                tab.AddOptionsElement(new ModifiedCheckBox("AutoPickUpTrash", 34, ModEntry.Conf.AutoPickUpTrash, OnCheckboxValueChanged));
                tab.AddOptionsElement(new ModifiedSlider("ScavengingRadius", 13, ModEntry.Conf.ScavengingRadius, 1, 3, OnSliderValueChanged, () => !ModEntry.Conf.AutoPickUpTrash || ModEntry.Conf.BalancedMode));

                tab.AddOptionsElement(new EmptyLabel());
                tab.AddOptionsElement(new LabelComponent("Auto Shearing and Milking"));
                tab.AddOptionsElement(new ModifiedCheckBox("AutoShearingAndMilking", 35, ModEntry.Conf.AutoShearingAndMilking, OnCheckboxValueChanged));
                tab.AddOptionsElement(new ModifiedSlider("AnimalHarvestRadius", 14, ModEntry.Conf.AnimalHarvestRadius, 1, 3, OnSliderValueChanged, () => !ModEntry.Conf.AutoShearingAndMilking || ModEntry.Conf.BalancedMode));
                
                tab.AddOptionsElement(new EmptyLabel());
                tab.AddOptionsElement(new LabelComponent("Collect Letter Attachments And Quests"));
                tab.AddOptionsElement(new ModifiedCheckBox("CollectLetterAttachmentsAndQuests", 36, ModEntry.Conf.CollectLetterAttachmentsAndQuests, OnCheckboxValueChanged));

                tab.AddOptionsElement(new EmptyLabel());
                _tabs.Add(tab);
            }
            {
                //UIs Tab
                MenuTab tab = new MenuTab();

                tab.AddOptionsElement(new EmptyLabel());
                tab.AddOptionsElement(new LabelComponent("Config Menu"));
                tab.AddOptionsElement(new ModifiedCheckBox("FilterBackgroundInMenu", 32, ModEntry.Conf.FilterBackgroundInMenu, OnCheckboxValueChanged));

                tab.AddOptionsElement(new EmptyLabel());
                tab.AddOptionsElement(new LabelComponent("Mine Info GUI"));
                tab.AddOptionsElement(new ModifiedCheckBox("MineInfoGUI", 0, ModEntry.Conf.MineInfoGui, OnCheckboxValueChanged));

                tab.AddOptionsElement(new EmptyLabel());
                tab.AddOptionsElement(new LabelComponent("Gift Information Tooltip"));
                tab.AddOptionsElement(new ModifiedCheckBox("GiftInformation", 1, ModEntry.Conf.GiftInformation, OnCheckboxValueChanged));

                tab.AddOptionsElement(new EmptyLabel());
                tab.AddOptionsElement(new LabelComponent("Fishing Info"));
                tab.AddOptionsElement(new ModifiedCheckBox("FishingInfo", 8, ModEntry.Conf.FishingInfo, OnCheckboxValueChanged));

                tab.AddOptionsElement(new EmptyLabel());
                tab.AddOptionsElement(new LabelComponent("Fishing Probabilities Information"));
                tab.AddOptionsElement(new ModifiedCheckBox("FishingProbabilitiesInfo", 26, ModEntry.Conf.FishingProbabilitiesInfo, OnCheckboxValueChanged));
                tab.AddOptionsElement(new ModifiedClickListener(this, "ProbBoxLocation", 0, ModEntry.Conf.ProbBoxX, ModEntry.Conf.ProbBoxY, translation, OnSomewhereClicked, OnStartListeningClick));

                tab.AddOptionsElement(new EmptyLabel());
                tab.AddOptionsElement(new LabelComponent("Show Shipping Price"));
                tab.AddOptionsElement(new ModifiedCheckBox("EstimateShippingPrice", 28, ModEntry.Conf.EstimateShippingPrice, OnCheckboxValueChanged));

                tab.AddOptionsElement(new EmptyLabel());
                _tabs.Add(tab);
            }
            {
                //Misc Tab
                MenuTab tab = new MenuTab();
                
                tab.AddOptionsElement(new EmptyLabel());
                tab.AddOptionsElement(new LabelComponent("Crafting From Chests"));
                tab.AddOptionsElement(new ModifiedCheckBox("CraftingFromChests", 27, ModEntry.Conf.CraftingFromChests, OnCheckboxValueChanged, i => ModEntry.IsCCOn));
                tab.AddOptionsElement(new ModifiedSlider("RadiusCraftingFromChests", 11, ModEntry.Conf.RadiusCraftingFromChests, 1, 5, OnSliderValueChanged, () => !ModEntry.Conf.CraftingFromChests || ModEntry.Conf.BalancedMode));

                tab.AddOptionsElement(new EmptyLabel());
                tab.AddOptionsElement(new LabelComponent("Unify Flower Colors"));
                tab.AddOptionsElement(new ModifiedCheckBox("UnifyFlowerColors", 29, ModEntry.Conf.UnifyFlowerColors, OnCheckboxValueChanged));

                tab.AddOptionsElement(new EmptyLabel());
                tab.AddOptionsElement(new LabelComponent("Pause When Idle"));
                tab.AddOptionsElement(new ModifiedCheckBox("PauseWhenIdle", 33, ModEntry.Conf.PauseWhenIdle, OnCheckboxValueChanged));
                tab.AddOptionsElement(new ModifiedSlider("IdleTimeout", 12, ModEntry.Conf.IdleTimeout, 1, 300, OnSliderValueChanged, () => !ModEntry.Conf.PauseWhenIdle, (which, value) => value + "s"));

                tab.AddOptionsElement(new EmptyLabel());
                _tabs.Add(tab);
            }
            {
                //Controls Tab
                MenuTab tab = new MenuTab();

                tab.AddOptionsElement(new EmptyLabel());
                tab.AddOptionsElement(new LabelComponent("Config Menu"));
                tab.AddOptionsElement(new ModifiedInputListener(this, "KeyShowMenu", 0, ModEntry.Conf.ButtonShowMenu, translation, OnInputListnerChanged, OnStartListening));

                tab.AddOptionsElement(new EmptyLabel());
                tab.AddOptionsElement(new LabelComponent("Auto Harvest"));
                tab.AddOptionsElement(new ModifiedInputListener(this, "KeyToggleBlackList", 1, ModEntry.Conf.ButtonToggleBlackList, translation, OnInputListnerChanged, OnStartListening));

                tab.AddOptionsElement(new EmptyLabel());
                _tabs.Add(tab);
            }
        }
        private void OnStartListening(int i, ModifiedInputListener option)
        {
            _isListening = true;
            _listener = option;
        }

        private void OnStartListeningClick(int i, ModifiedClickListener option)
        {
            _isListeningClick = true;
            _clickListener = option;
        }

        private void OnSomewhereClicked(int index, Point point)
        {
            _isListeningClick = false;
            _clickListener = null;
            switch (index)
            {
                case 0:
                    ModEntry.Conf.ProbBoxX = point.X;
                    ModEntry.Conf.ProbBoxY = point.Y;
                    break;
                default: return;
            }
            _mod.WriteConfig();
        }
        private void OnInputListnerChanged(int index, SButton value)
        {
            switch (index)
            {
                case 0:
                    ModEntry.Conf.ButtonShowMenu = value; break;
                case 1:
                    ModEntry.Conf.ButtonToggleBlackList = value; break;
                default: return;
            }
            _mod.WriteConfig();
            _isListening = false;
            _listener = null;
        }
        private void OnCheckboxValueChanged(int index, bool value)
        {
            switch (index)
            {
                case 0: ModEntry.Conf.MineInfoGui = value; break;
                case 1: ModEntry.Conf.GiftInformation = value; break;
                case 2: ModEntry.Conf.AutoWaterNearbyCrops = value; break;
                case 3: ModEntry.Conf.AutoPetNearbyAnimals = value; break;
                case 4: ModEntry.Conf.AutoAnimalDoor = value; break;
                case 5: ModEntry.Conf.AutoFishing = value; break;
                case 6: ModEntry.Conf.AutoReelRod = value; break;
                case 8: ModEntry.Conf.FishingInfo = value; break;
                case 9: ModEntry.Conf.AutoGate = value; break;
                case 10: ModEntry.Conf.AutoEat = value; break;
                case 11: ModEntry.Conf.AutoHarvest = value; break;
                case 12: ModEntry.Conf.AutoDestroyDeadCrops = value; break;
                case 13: ModEntry.Conf.AutoRefillWateringCan = value; break;
                case 14: ModEntry.Conf.AutoCollectCollectibles = value; break;
                case 15: ModEntry.Conf.AutoShakeFruitedPlants = value; break;
                case 16: ModEntry.Conf.FindCanFromInventory = value; break;
                case 17: ModEntry.Conf.AutoDigArtifactSpot = value; break;
                case 18: ModEntry.Conf.FindHoeFromInventory = value; break;
                case 20: ModEntry.Conf.BalancedMode = value; break;
                case 22: ModEntry.Conf.AutoDepositIngredient = value; break;
                case 23: ModEntry.Conf.AutoPullMachineResult = value; break;
                case 24: ModEntry.Conf.AutoPetNearbyPets = value; break;
                case 25: ModEntry.Conf.ProtectNectarProducingFlower = value; break;
                case 26: ModEntry.Conf.FishingProbabilitiesInfo = value; break;
                case 27: ModEntry.Conf.CraftingFromChests = value; break;
                case 28: ModEntry.Conf.EstimateShippingPrice = value; break;
                case 29: ModEntry.Conf.UnifyFlowerColors = value; break;
                case 30: ModEntry.Conf.AutoLootTreasures = value; break;
                case 31: ModEntry.Conf.CloseTreasureWhenAllLooted = value; break;
                case 32: ModEntry.Conf.FilterBackgroundInMenu = value; break;
                case 33: ModEntry.Conf.PauseWhenIdle = value; break;
                case 34: ModEntry.Conf.AutoPickUpTrash = value; break;
                case 35: ModEntry.Conf.AutoShearingAndMilking = value; break;
                case 36: ModEntry.Conf.CollectLetterAttachmentsAndQuests = value; break;
                default: return;
            }
            _mod.WriteConfig();
        }
        private void OnSliderValueChanged(int index, int value)
        {
            switch (index)
            {
                case 0: ModEntry.Conf.CpuThresholdFishing = value / 10.0f; break;
                case 1: ModEntry.Conf.StaminaToEatRatio = value / 10.0f; break;
                case 2: ModEntry.Conf.HealthToEatRatio = value / 10.0f; break;
                case 3: ModEntry.Conf.AutoWaterRadius = value; break;
                case 4: ModEntry.Conf.AutoPetRadius = value; break;
                case 5: ModEntry.Conf.AutoHarvestRadius = value; break;
                case 6: ModEntry.Conf.AutoCollectRadius = value; break;
                case 7: ModEntry.Conf.AutoShakeRadius = value; break;
                case 8: ModEntry.Conf.AutoDigRadius = value; break;
                case 10: ModEntry.Conf.MachineRadius = value; break;
                case 11: ModEntry.Conf.RadiusCraftingFromChests = value; break;
                case 12: ModEntry.Conf.IdleTimeout = value; break;
                case 13: ModEntry.Conf.ScavengingRadius = value; break;
                case 14: ModEntry.Conf.AnimalHarvestRadius = value; break;
                default: return;
            }

            _mod.WriteConfig();
        }

        private static string Format(int id, int value)
        {
            if (id >= 0 && id <= 2)
            {
                return $"{value / 10f:f1}";
            }
            return value + "";
        }

        public override void update(GameTime time)
        {
            base.update(time);
            _upCursor.visible = _firstIndex > 0;
            _downCursor.visible = _firstIndex < GetLastViewableIndex();
        }

        public override void gamePadButtonHeld(Buttons b)
        {
            if (_isListening || _isListeningClick)
            {
                return;
            }
            if (b.HasFlag(Buttons.RightThumbstickUp) && _upCursor.visible)
            {
                UpCursor();
            }
            else if (b.HasFlag(Buttons.RightThumbstickDown) && _downCursor.visible)
            {
                DownCursor();
            }
        }

        public override void receiveGamePadButton(Buttons b)
        {
            if (_isListeningClick)
            {
                return;
            }
            if (_isListening)
            {
                foreach (ModifiedInputListener element in _tabs[_tabIndex].GetElements().OfType<ModifiedInputListener>())
                {
                    element.receiveButtonPress(b);
                }
                return;
            }
            if (b.HasFlag(Buttons.DPadUp) && _upCursor.visible)
            {
                UpCursor();
            }
            else if (b.HasFlag(Buttons.DPadDown) && _downCursor.visible)
            {
                DownCursor();
            }
            else if(b.HasFlag(Buttons.B))
            {
                CloseMenu();
            }
            else if (b.HasFlag(Buttons.LeftShoulder))
            {
                Game1.playSound("shwip");
                _firstIndex = 0;
                _tabIndex--;
                if (_tabIndex == -1)
                {
                    _tabIndex = _tabs.Count - 1;
                }
            }
            else if (b.HasFlag(Buttons.RightShoulder))
            {
                Game1.playSound("shwip");
                _firstIndex = 0;
                _tabIndex++;
                if (_tabIndex >= _tabs.Count)
                {
                    _tabIndex = 0;
                }
            }
        }

        private void DownCursor()
        {
            Game1.playSound("shwip");
            _firstIndex++;
            ChengeIndexOfScrollBar(_firstIndex);
        }

        private void UpCursor()
        {
            Game1.playSound("shwip");
            _firstIndex--;
            ChengeIndexOfScrollBar(_firstIndex);
        }

        public override void receiveScrollWheelAction(int direction)
        {
            if (_isListening || _isListeningClick)
            {
                return;
            }
            //Scroll with Mouse Wheel
            if (direction > 0 && _upCursor.visible)
            {
                UpCursor();
            }
            else if (direction < 0 && _downCursor.visible)
            {
                DownCursor();
            }
        }

        public override void draw(SpriteBatch b)
        {
            if (ModEntry.Conf.FilterBackgroundInMenu)
            {
                b.Draw(Game1.fadeToBlackRect, new Rectangle(0, 0, Game1.viewport.Width, Game1.viewport.Height), Color.Black * 0.5f);
            }

            const int x = 16;
            int y = 16;

            drawTextureBox(b, _tabAutomation.Left, _tabAutomation.Top, _tabAutomation.Width, _tabAutomation.Height, Color.White * (_tabIndex == 0 ? 1.0f : 0.6f));
            b.DrawString(Game1.smallFont, _tabAutomationString, new Vector2(_tabAutomation.Left + 16, _tabAutomation.Top + (_tabAutomation.Height - _font.MeasureString(_tabAutomationString).Y) / 2), Color.Black * (_tabIndex == 0 ? 1.0f : 0.6f));

            drawTextureBox(b, _tabUIs.Left, _tabUIs.Top, _tabUIs.Width, _tabUIs.Height, Color.White * (_tabIndex == 1 ? 1.0f : 0.6f));
            b.DrawString(Game1.smallFont, _tabUIsString, new Vector2(_tabUIs.Left + 16, _tabUIs.Top + (_tabUIs.Height - _font.MeasureString(_tabUIsString).Y) / 2), Color.Black * (_tabIndex == 1 ? 1.0f : 0.6f));

            drawTextureBox(b, _tabMisc.Left, _tabMisc.Top, _tabMisc.Width, _tabMisc.Height, Color.White * (_tabIndex == 2 ? 1.0f : 0.6f));
            b.DrawString(Game1.smallFont, _tabMiscString, new Vector2(_tabMisc.Left + 16, _tabMisc.Top + (_tabMisc.Height - _font.MeasureString(_tabMiscString).Y) / 2), Color.Black * (_tabIndex == 2 ? 1.0f : 0.6f));

            drawTextureBox(b, _tabControls.Left, _tabControls.Top, _tabControls.Width, _tabControls.Height, Color.White * (_tabIndex == 3 ? 1.0f : 0.6f));
            b.DrawString(Game1.smallFont, _tabControlsString, new Vector2(_tabControls.Left + 16, _tabControls.Top + (_tabControls.Height - _font.MeasureString(_tabControlsString).Y) / 2), Color.Black * (_tabIndex == 3 ? 1.0f : 0.6f));

            drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), xPositionOnScreen, yPositionOnScreen, width, height, Color.White, 1.0f, false);
            base.draw(b);

            if(!CanDrawAll(0))
            {
                _scrollBar.visible = true;
                drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 383, 6, 6), _scrollBarRunner.X, _scrollBarRunner.Y, _scrollBarRunner.Width, _scrollBarRunner.Height, Color.White, 4f, false);
                _scrollBar.draw(b);
            }
            else
            {
                _scrollBar.visible = false;
            }

            {
                int x2 = (Game1.viewport.Width - 400) / 2;
                drawTextureBox(b, x2, yPositionOnScreen - 108, 400, 100, Color.White);

                const string str = "JoE Settings";
                Vector2 size = Game1.dialogueFont.MeasureString(str) * 1.1f;

                Utility.drawTextWithShadow(b, str, Game1.dialogueFont, new Vector2((Game1.viewport.Width - size.X) / 2, yPositionOnScreen - 50 - (int)size.Y / 2), Color.Black, 1.1f);
            }

            foreach (OptionsElement element in GetElementsToShow())
            {
                element.draw(b, x + xPositionOnScreen, y + yPositionOnScreen);
                y += element.bounds.Height + 16;
            }
            _upCursor.draw(b);
            _downCursor.draw(b);

            if (_isListening)
            {
                Point size = _listener.GetListeningMessageWindowSize();
                drawTextureBox(b, (Game1.viewport.Width - size.X) / 2, (Game1.viewport.Height - size.Y) / 2, size.X, size.Y, Color.White);
                _listener.DrawStrings(b, (Game1.viewport.Width - size.X) / 2, (Game1.viewport.Height - size.Y) / 2);
            }

            if (_isListeningClick)
            {
                Point size = _clickListener.GetListeningMessageWindowSize();
                drawTextureBox(b, (Game1.viewport.Width - size.X) / 2, (Game1.viewport.Height - size.Y) / 2, size.X, size.Y, Color.White);
                _clickListener.DrawStrings(b, (Game1.viewport.Width - size.X) / 2, (Game1.viewport.Height - size.Y) / 2);
            }

            Util.DrawCursor();
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            _upCursor.tryHover(x, y);
            _downCursor.tryHover(x, y);
        }

        private void ChengeIndexOfScrollBar(int index)
        {
            int maxIndex = GetLastViewableIndex();
            _firstIndex = index;
            _scrollBar.bounds.Y = _scrollBarRunner.Top + (int)(index * ((float)(_scrollBarRunner.Height - _scrollBar.bounds.Height) / maxIndex));
            if (index == maxIndex)
            {
                _scrollBar.bounds.Y = _scrollBarRunner.Bottom - _scrollBar.bounds.Height;
            }
        }

        public override void leftClickHeld(int x, int y)
        {
            if (_isScrolling && _scrollBar.visible && _scrollBarRunner.Contains(x, y))
            {
                int maxIndex = GetLastViewableIndex();
                int index = (int)((y - _scrollBarRunner.Top - _scrollBar.bounds.Height / 2) / ((double)(_scrollBarRunner.Height - _scrollBar.bounds.Height) / maxIndex) * 1.025);
                index = (int)Util.Cap(index, 0, maxIndex);
                if (_firstIndex != index)
                {
                    Game1.playSound("shwip");
                    ChengeIndexOfScrollBar(index);
                }
            }
            if (_isListening)
            {
                return;
            }
            base.leftClickHeld(x, y);
            foreach (OptionsElement element in GetElementsToShow())
            {
                if (element.bounds.Contains(x - xPositionOnScreen - element.bounds.X / 2, y - yPositionOnScreen - element.bounds.Y / 2))
                {
                    element.leftClickHeld(x - element.bounds.X - xPositionOnScreen, y - element.bounds.Y - yPositionOnScreen);
                }
                y -= element.bounds.Height + 16;
            }
        }

        public override void receiveKeyPress(Keys key)
        {
            base.receiveKeyPress(key);
            if (_isListening)
            {
                foreach (OptionsElement element in _tabs[_tabIndex].GetElements())
                {
                    element.receiveKeyPress(key);
                }
            }
            else if (key == Keys.Escape)
            {
                CloseMenu();
            }
            else if (key.ToSButton() == ModEntry.Conf.ButtonShowMenu)
            {
                if (!_isFirstTime)
                {
                    _isFirstTime = true;
                    return;
                }
                CloseMenu();
            }
        }

        private static void CloseMenu()
        {
            if (Game1.activeClickableMenu is JoeMenu)
            {
                Game1.playSound("bigDeSelect");
                Game1.exitActiveMenu();
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);
            if (_isListening)
            {
                return;
            }
            if (_isListeningClick)
            {
                foreach (ModifiedClickListener listener in _tabs[_tabIndex].GetElements().OfType<ModifiedClickListener>())
                {
                    listener.receiveLeftClick(x, y);
                }
                return;
            }
            if (_scrollBar.visible && _scrollBarRunner.Contains(x, y))
            {
                _isScrolling = true;
            }
            if (_tabAutomation.Contains(x, y))
            {
                TryToChangeTab(0);
                return;
            }
            if (_tabUIs.Contains(x, y))
            {
                TryToChangeTab(1);
                return;
            }

            if (_tabMisc.Contains(x, y))
            {
                TryToChangeTab(2);
                return;
            }

            if (_tabControls.Contains(x, y))
            {
                TryToChangeTab(3);
                return;
            }
            if (_upCursor.bounds.Contains(x, y) && _upCursor.visible)
            {
                UpCursor();
                return;
            }
            if (_downCursor.bounds.Contains(x, y) && _downCursor.visible)
            {
                DownCursor();
                return;
            }
            foreach (OptionsElement element in GetElementsToShow())
            {
                if (element.bounds.Contains(x - xPositionOnScreen - element.bounds.X / 2, y - yPositionOnScreen - element.bounds.Y / 2))
                {
                    element.receiveLeftClick(x - element.bounds.X - xPositionOnScreen, y - element.bounds.Y - yPositionOnScreen);
                }
                y -= element.bounds.Height + 16;
            }
        }

        public override void releaseLeftClick(int x, int y)
        {
            _isScrolling = false;
            base.releaseLeftClick(x, y);
            if (_isListeningClick)
            {
                foreach (ModifiedClickListener listener in _tabs[_tabIndex].GetElements().OfType<ModifiedClickListener>())
                {
                    listener.leftClickReleased(x, y);
                }
                return;
            }
            if (_isListening)
            {
                return;
            }
            foreach (OptionsElement element in GetElementsToShow())
            {
                if (element.bounds.Contains(x - xPositionOnScreen - element.bounds.X / 2, y - yPositionOnScreen - element.bounds.Y / 2))
                {
                    element.leftClickReleased(x - element.bounds.X - xPositionOnScreen, y - element.bounds.Y - yPositionOnScreen);
                }
                y -= element.bounds.Height + 16;
            }
        }

        public List<OptionsElement> GetElementsToShow()
        {
            List<OptionsElement> menuElements = _tabs[_tabIndex].GetElements();
            List<OptionsElement> elements = new List<OptionsElement>();
            int y = 16;
            for (int i = _firstIndex; i < menuElements.Count; i++)
            {
                OptionsElement element = menuElements[i];
                int hElem = element is ModifiedSlider ? element.bounds.Height + 4 : element.bounds.Height;
                if (y + hElem < height)
                {
                    y += hElem + 16;
                    elements.Add(element);
                }
                else
                {
                    break;
                }
            }
            return elements;
        }

        public bool CanDrawAll(int firstIndex = -1)
        {
            if (firstIndex < 0)
            {
                firstIndex = _firstIndex;
            }
            List<OptionsElement> menuElements = _tabs[_tabIndex].GetElements();
            int y = 16;
            for (int i = firstIndex; i < menuElements.Count; i++)
            {
                OptionsElement element = menuElements[i];
                int hElem = element is ModifiedSlider ? element.bounds.Height + 4 : element.bounds.Height;
                if (y + hElem < height)
                {
                    y += hElem + 16;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        private int GetLastViewableIndex()
        {
            int index = 0;
            while (!CanDrawAll(index))
            {
                index++;
            }
            return index;
        }

        private void TryToChangeTab(int which)
        {
            if (_tabIndex != which)
            {
                _tabIndex = which;
                _firstIndex = 0;
                ChengeIndexOfScrollBar(0);
                Game1.playSound("drumkit6");
            }
        }
    }
}