using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using System;
using System.Linq;

namespace DailyTasksReport.UI
{
    internal class Checkbox : OptionsElement
    {
        private readonly ModConfig _config;
        private readonly OptionsEnum _option;

        private readonly Rectangle _bubbleButtonSource;
        private readonly Rectangle _checkButtonSource;

        private bool _isChecked;
        private bool _isMouseOnBubbleButton;
        private bool _isMouseOnCheckbox;

        public Checkbox(string label, OptionsEnum whichOption, ModConfig config, int itemLevel = 0) :
            base(label, -1, -1, Game1.pixelZoom * 9, Game1.pixelZoom * 9)
        {
            _option = whichOption;
            _config = config;
            bounds.X += itemLevel * Game1.pixelZoom * 7;
            _checkButtonSource = bounds;

            if (whichOption == OptionsEnum.AllAnimalProducts || whichOption == OptionsEnum.AllMachines)
                this.whichOption = -1;
            else
                this.whichOption = (int)whichOption;

            // Load options
            RefreshStatus();
        }

        public Checkbox(string label, OptionsEnum whichOption, ModConfig config, bool hasBubbleButton, int slotWidth) :
            base(label, -1, -1, slotWidth, Game1.pixelZoom * 9, (int)whichOption)
        {
            _option = whichOption;
            _config = config;
            _checkButtonSource = new Rectangle(bounds.X, bounds.Y, Game1.pixelZoom * 9, Game1.pixelZoom * 9);

            if (hasBubbleButton)
                _bubbleButtonSource = new Rectangle(slotWidth - 20 * Game1.pixelZoom, bounds.Y,
                    Game1.pixelZoom * 12, Game1.pixelZoom * 17);

            RefreshStatus();
        }

        public override void receiveKeyPress(Keys key)
        {
            base.receiveKeyPress(key);
            if (!Game1.options.snappyMenus || !Game1.options.gamepadControls || _bubbleButtonSource.IsEmpty)
                return;
            if (Game1.options.doesInputListContain(Game1.options.moveRightButton, key))
            {
                _isMouseOnBubbleButton = true;
                _isMouseOnCheckbox = false;
            }
            else if (Game1.options.doesInputListContain(Game1.options.moveLeftButton, key))
            {
                _isMouseOnBubbleButton = false;
                _isMouseOnCheckbox = true;
            }
        }

        public void CursorAboveOption()
        {
            if (!Game1.options.snappyMenus || !Game1.options.gamepadControls)
                return;
            if (_isMouseOnCheckbox || _isMouseOnBubbleButton)
                return;
            _isMouseOnCheckbox = true;
            _isMouseOnBubbleButton = false;
        }

        internal void RefreshStatus()
        {
            switch (_option)
            {
                case OptionsEnum.ShowReportButton:
                    _isChecked = _config.DisplayReportButton;
                    break;

                case OptionsEnum.ShowDetailedInfo:
                    _isChecked = _config.ShowDetailedInfo;
                    break;

                case OptionsEnum.DisplayBubbles:
                    _isChecked = _config.DisplayBubbles;
                    break;

                case OptionsEnum.NewRecipeOnTv:
                    _isChecked = _config.NewRecipeOnTv;
                    break;

                case OptionsEnum.Birthdays:
                    _isChecked = _config.Birthdays;
                    break;

                case OptionsEnum.TravelingMerchant:
                    _isChecked = _config.TravelingMerchant;
                    break;

                case OptionsEnum.UnwateredCrops:
                    _isChecked = _config.UnwateredCrops;
                    break;

                case OptionsEnum.UnharvestedCrops:
                    _isChecked = _config.UnharvestedCrops;
                    break;

                case OptionsEnum.DeadCrops:
                    _isChecked = _config.DeadCrops;
                    break;

                case OptionsEnum.UnpettedPet:
                    _isChecked = _config.UnpettedPet;
                    break;

                case OptionsEnum.UnfilledPetBowl:
                    _isChecked = _config.UnfilledPetBowl;
                    break;

                case OptionsEnum.UnpettedAnimals:
                    _isChecked = _config.UnpettedAnimals;
                    break;

                case OptionsEnum.MissingHay:
                    _isChecked = _config.MissingHay;
                    break;

                case OptionsEnum.FarmCave:
                    _isChecked = _config.FarmCave;
                    break;

                case OptionsEnum.UncollectedCrabpots:
                    _isChecked = _config.UncollectedCrabpots;
                    break;

                case OptionsEnum.NotBaitedCrabpots:
                    _isChecked = _config.NotBaitedCrabpots;
                    break;

                // Animal products
                case OptionsEnum.AllAnimalProducts:
                    _isChecked = !_config.AnimalProducts.ContainsValue(false);
                    break;

                case OptionsEnum.CowMilk:
                    _isChecked = _config.AnimalProducts["Cow milk"];
                    break;

                case OptionsEnum.GoatMilk:
                    _isChecked = _config.AnimalProducts["Goat milk"];
                    break;

                case OptionsEnum.SheepWool:
                    _isChecked = _config.AnimalProducts["Sheep wool"];
                    break;

                case OptionsEnum.ChickenEgg:
                    _isChecked = _config.AnimalProducts["Chicken egg"];
                    break;

                case OptionsEnum.DinosaurEgg:
                    _isChecked = _config.AnimalProducts["Dinosaur egg"];
                    break;

                case OptionsEnum.DuckEgg:
                    _isChecked = _config.AnimalProducts["Duck egg"];
                    break;

                case OptionsEnum.DuckFeather:
                    _isChecked = _config.AnimalProducts["Duck feather"];
                    break;

                case OptionsEnum.RabbitsWool:
                    _isChecked = _config.AnimalProducts["Rabbit's wool"];
                    break;

                case OptionsEnum.RabbitsFoot:
                    _isChecked = _config.AnimalProducts["Rabbit's foot"];
                    break;

                case OptionsEnum.Truffle:
                    _isChecked = _config.AnimalProducts["Truffle"];
                    break;

                case OptionsEnum.SlimeBall:
                    _isChecked = _config.AnimalProducts["Slime ball"];
                    break;

                // Machines
                case OptionsEnum.AllMachines:
                    _isChecked = !_config.Machines.ContainsValue(false) && _config.Cask > 0;
                    break;

                case OptionsEnum.BeeHouse:
                    _isChecked = _config.Machines["Bee House"];
                    break;

                case OptionsEnum.CharcoalKiln:
                    _isChecked = _config.Machines["Charcoal Kiln"];
                    break;

                case OptionsEnum.CheesePress:
                    _isChecked = _config.Machines["Cheese Press"];
                    break;

                case OptionsEnum.Crystalarium:
                    _isChecked = _config.Machines["Crystalarium"];
                    break;

                case OptionsEnum.Furnace:
                    _isChecked = _config.Machines["Furnace"];
                    break;

                case OptionsEnum.Keg:
                    _isChecked = _config.Machines["Keg"];
                    break;

                case OptionsEnum.LightningRod:
                    _isChecked = _config.Machines["Lightning Rod"];
                    break;

                case OptionsEnum.Loom:
                    _isChecked = _config.Machines["Loom"];
                    break;

                case OptionsEnum.MayonnaiseMachine:
                    _isChecked = _config.Machines["Mayonnaise Machine"];
                    break;

                case OptionsEnum.OilMaker:
                    _isChecked = _config.Machines["Oil Maker"];
                    break;

                case OptionsEnum.PreservesJar:
                    _isChecked = _config.Machines["Preserves Jar"];
                    break;

                case OptionsEnum.RecyclingMachine:
                    _isChecked = _config.Machines["Recycling Machine"];
                    break;

                case OptionsEnum.SeedMaker:
                    _isChecked = _config.Machines["Seed Maker"];
                    break;

                case OptionsEnum.SlimeEggPress:
                    _isChecked = _config.Machines["Slime Egg-Press"];
                    break;

                case OptionsEnum.SodaMachine:
                    _isChecked = _config.Machines["Soda Machine"];
                    break;

                case OptionsEnum.StatueOfEndlessFortune:
                    _isChecked = _config.Machines["Statue Of Endless Fortune"];
                    break;

                case OptionsEnum.StatueOfPerfection:
                    _isChecked = _config.Machines["Statue Of Perfection"];
                    break;

                case OptionsEnum.Tapper:
                    _isChecked = _config.Machines["Tapper"];
                    break;

                case OptionsEnum.WormBin:
                    _isChecked = _config.Machines["Worm Bin"];
                    break;

                case OptionsEnum.DrawUnwateredCrops:
                    _isChecked = _config.DrawBubbleUnwateredCrops;
                    break;

                case OptionsEnum.DrawUnharvestedCrops:
                    _isChecked = _config.DrawBubbleUnharvestedCrops;
                    break;

                case OptionsEnum.DrawDeadCrops:
                    _isChecked = _config.DrawBubbleDeadCrops;
                    break;

                case OptionsEnum.DrawUnpettedPet:
                    _isChecked = _config.DrawBubbleUnpettedPet;
                    break;

                case OptionsEnum.DrawUnpettedAnimals:
                    _isChecked = _config.DrawBubbleUnpettedAnimals;
                    break;

                case OptionsEnum.DrawAnimalsWithProduce:
                    _isChecked = _config.DrawBubbleAnimalsWithProduce;
                    break;

                case OptionsEnum.DrawBuildingsWithProduce:
                    _isChecked = _config.DrawBubbleBuildingsWithProduce;
                    break;

                case OptionsEnum.DrawBuildingsMissingHay:
                    _isChecked = _config.DrawBubbleBuildingsMissingHay;
                    break;

                case OptionsEnum.DrawTruffles:
                    _isChecked = _config.DrawBubbleTruffles;
                    break;

                case OptionsEnum.DrawCrabpotsNotBaited:
                    _isChecked = _config.DrawBubbleCrabpotsNotBaited;
                    break;

                case OptionsEnum.DrawCask:
                    _isChecked = _config.DrawBubbleCask;
                    break;

                default:
                    throw new ArgumentOutOfRangeException($"Option {_option} is not possible on a checkbox.");
            }
        }

        public override void draw(SpriteBatch b, int slotX, int slotY)
        {
            if (whichOption == -1)
            {
                b.Draw(Game1.mouseCursors,
                    new Vector2(slotX + _checkButtonSource.X, slotY + _checkButtonSource.Y + Game1.pixelZoom),
                    _isChecked ? OptionsCheckbox.sourceRectChecked : OptionsCheckbox.sourceRectUnchecked,
                    Color.White * (greyedOut ? 0.33f : 1f), 0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None,
                    0.4f);
                SpriteText.drawString(b, label, slotX + bounds.X * 2 + Game1.pixelZoom * 4, slotY + bounds.Y, 999, -1,
                    999, 1f, 0.1f);
            }
            else
            {
                b.Draw(Game1.mouseCursors, new Vector2(slotX + bounds.X, slotY + bounds.Y),
                    _isChecked ? OptionsCheckbox.sourceRectChecked : OptionsCheckbox.sourceRectUnchecked,
                    Color.White * (greyedOut ? 0.33f : 1f), 0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None,
                    0.4f);
                Utility.drawTextWithShadow(b, label, Game1.dialogueFont,
                    new Vector2(slotX + bounds.X + _checkButtonSource.Width + Game1.pixelZoom * 2, slotY + bounds.Y),
                    greyedOut ? Game1.textColor * 0.33f : Game1.textColor, 1f, 0.1f);
                if (!_bubbleButtonSource.IsEmpty)
                    Utility.drawWithShadow(b, Game1.mouseCursors,
                        new Vector2(slotX + _bubbleButtonSource.X, slotY + _bubbleButtonSource.Y),
                        new Rectangle(66, 4, 14, 12), Color.White, 0f, Vector2.Zero, 3.5f, false, 0.4f);
                if (Game1.options.snappyMenus && Game1.options.gamepadControls)
                    if (_isMouseOnCheckbox)
                    {
                        Game1.setMousePosition(slotX + _checkButtonSource.Center.X,
                            slotY + _checkButtonSource.Center.Y);
                        _isMouseOnCheckbox = false;
                    }
                    else if (_isMouseOnBubbleButton)
                    {
                        Game1.setMousePosition(slotX + _bubbleButtonSource.Center.X,
                            slotY + _bubbleButtonSource.Center.Y);
                        _isMouseOnBubbleButton = false;
                    }
            }
        }

        public override void receiveLeftClick(int x, int y)
        {
            if (greyedOut)
                return;

            if (_bubbleButtonSource.Contains(x, y))
                BubblesMenu.OpenMenu(_config);

            if (!_checkButtonSource.Contains(x, y)) return;

            Game1.playSound("drumkit6");
            _isChecked = !_isChecked;

            // Change options
            switch (_option)
            {
                case OptionsEnum.ShowReportButton:
                    _config.DisplayReportButton = _isChecked;
                    return;

                case OptionsEnum.ShowDetailedInfo:
                    _config.ShowDetailedInfo = _isChecked;
                    break;

                case OptionsEnum.DisplayBubbles:
                    _config.DisplayBubbles = _isChecked;
                    return;

                case OptionsEnum.NewRecipeOnTv:
                    _config.NewRecipeOnTv = _isChecked;
                    break;

                case OptionsEnum.Birthdays:
                    _config.Birthdays = _isChecked;
                    break;

                case OptionsEnum.TravelingMerchant:
                    _config.TravelingMerchant = _isChecked;
                    break;

                case OptionsEnum.UnwateredCrops:
                    _config.UnwateredCrops = _isChecked;
                    break;

                case OptionsEnum.UnharvestedCrops:
                    _config.UnharvestedCrops = _isChecked;
                    break;

                case OptionsEnum.DeadCrops:
                    _config.DeadCrops = _isChecked;
                    break;

                case OptionsEnum.UnpettedPet:
                    _config.UnpettedPet = _isChecked;
                    break;

                case OptionsEnum.UnfilledPetBowl:
                    _config.UnfilledPetBowl = _isChecked;
                    break;

                case OptionsEnum.UnpettedAnimals:
                    _config.UnpettedAnimals = _isChecked;
                    break;

                case OptionsEnum.MissingHay:
                    _config.MissingHay = _isChecked;
                    break;

                case OptionsEnum.FarmCave:
                    _config.FarmCave = _isChecked;
                    break;

                case OptionsEnum.UncollectedCrabpots:
                    _config.UncollectedCrabpots = _isChecked;
                    break;

                case OptionsEnum.NotBaitedCrabpots:
                    _config.NotBaitedCrabpots = _isChecked;
                    break;

                // Animal products
                case OptionsEnum.AllAnimalProducts:
                    foreach (var key in _config.AnimalProducts.Keys.ToList())
                        _config.AnimalProducts[key] = _isChecked;
                    break;

                case OptionsEnum.CowMilk:
                    _config.AnimalProducts["Cow milk"] = _isChecked;
                    break;

                case OptionsEnum.GoatMilk:
                    _config.AnimalProducts["Goat milk"] = _isChecked;
                    break;

                case OptionsEnum.SheepWool:
                    _config.AnimalProducts["Sheep wool"] = _isChecked;
                    break;

                case OptionsEnum.ChickenEgg:
                    _config.AnimalProducts["Chicken egg"] = _isChecked;
                    break;

                case OptionsEnum.DinosaurEgg:
                    _config.AnimalProducts["Dinosaur egg"] = _isChecked;
                    break;

                case OptionsEnum.DuckEgg:
                    _config.AnimalProducts["Duck egg"] = _isChecked;
                    break;

                case OptionsEnum.DuckFeather:
                    _config.AnimalProducts["Duck feather"] = _isChecked;
                    break;

                case OptionsEnum.RabbitsWool:
                    _config.AnimalProducts["Rabbit's wool"] = _isChecked;
                    break;

                case OptionsEnum.RabbitsFoot:
                    _config.AnimalProducts["Rabbit's foot"] = _isChecked;
                    break;

                case OptionsEnum.Truffle:
                    _config.AnimalProducts["Truffle"] = _isChecked;
                    break;

                case OptionsEnum.SlimeBall:
                    _config.AnimalProducts["Slime ball"] = _isChecked;
                    break;

                // Machines
                case OptionsEnum.AllMachines:
                    foreach (var key in _config.Machines.Keys.ToList())
                        _config.Machines[key] = _isChecked;
                    if (!_isChecked)
                        _config.Cask = 0;
                    else if (_config.Cask == 0)
                        _config.Cask = 4;
                    break;

                case OptionsEnum.BeeHouse:
                    _config.Machines["Bee House"] = _isChecked;
                    break;

                case OptionsEnum.CharcoalKiln:
                    _config.Machines["Charcoal Kiln"] = _isChecked;
                    break;

                case OptionsEnum.CheesePress:
                    _config.Machines["Cheese Press"] = _isChecked;
                    break;

                case OptionsEnum.Crystalarium:
                    _config.Machines["Crystalarium"] = _isChecked;
                    break;

                case OptionsEnum.Furnace:
                    _config.Machines["Furnace"] = _isChecked;
                    break;

                case OptionsEnum.Keg:
                    _config.Machines["Keg"] = _isChecked;
                    break;

                case OptionsEnum.LightningRod:
                    _config.Machines["Lightning Rod"] = _isChecked;
                    break;

                case OptionsEnum.Loom:
                    _config.Machines["Loom"] = _isChecked;
                    break;

                case OptionsEnum.MayonnaiseMachine:
                    _config.Machines["Mayonnaise Machine"] = _isChecked;
                    break;

                case OptionsEnum.OilMaker:
                    _config.Machines["Oil Maker"] = _isChecked;
                    break;

                case OptionsEnum.PreservesJar:
                    _config.Machines["Preserves Jar"] = _isChecked;
                    break;

                case OptionsEnum.RecyclingMachine:
                    _config.Machines["Recycling Machine"] = _isChecked;
                    break;

                case OptionsEnum.SeedMaker:
                    _config.Machines["Seed Maker"] = _isChecked;
                    break;

                case OptionsEnum.SlimeEggPress:
                    _config.Machines["Slime Egg-Press"] = _isChecked;
                    break;

                case OptionsEnum.SodaMachine:
                    _config.Machines["Soda Machine"] = _isChecked;
                    break;

                case OptionsEnum.StatueOfEndlessFortune:
                    _config.Machines["Statue Of Endless Fortune"] = _isChecked;
                    break;

                case OptionsEnum.StatueOfPerfection:
                    _config.Machines["Statue Of Perfection"] = _isChecked;
                    break;

                case OptionsEnum.Tapper:
                    _config.Machines["Tapper"] = _isChecked;
                    break;

                case OptionsEnum.WormBin:
                    _config.Machines["Worm Bin"] = _isChecked;
                    break;

                case OptionsEnum.DrawUnwateredCrops:
                    _config.DrawBubbleUnwateredCrops = _isChecked;
                    break;

                case OptionsEnum.DrawUnharvestedCrops:
                    _config.DrawBubbleUnharvestedCrops = _isChecked;
                    break;

                case OptionsEnum.DrawDeadCrops:
                    _config.DrawBubbleDeadCrops = _isChecked;
                    break;

                case OptionsEnum.DrawUnpettedPet:
                    _config.DrawBubbleUnpettedPet = _isChecked;
                    break;

                case OptionsEnum.DrawUnpettedAnimals:
                    _config.DrawBubbleUnpettedAnimals = _isChecked;
                    break;

                case OptionsEnum.DrawAnimalsWithProduce:
                    _config.DrawBubbleAnimalsWithProduce = _isChecked;
                    break;

                case OptionsEnum.DrawBuildingsWithProduce:
                    _config.DrawBubbleBuildingsWithProduce = _isChecked;
                    break;

                case OptionsEnum.DrawBuildingsMissingHay:
                    _config.DrawBubbleBuildingsMissingHay = _isChecked;
                    break;

                case OptionsEnum.DrawTruffles:
                    _config.DrawBubbleTruffles = _isChecked;
                    break;

                case OptionsEnum.DrawCrabpotsNotBaited:
                    _config.DrawBubbleCrabpotsNotBaited = _isChecked;
                    break;

                case OptionsEnum.DrawCask:
                    _config.DrawBubbleCask = _isChecked;
                    break;

                default:
                    throw new ArgumentOutOfRangeException($"Option {_option} is not possible on a checkbox.");
            }
            SettingsMenu.RaiseReportConfigChanged();
        }
    }
}