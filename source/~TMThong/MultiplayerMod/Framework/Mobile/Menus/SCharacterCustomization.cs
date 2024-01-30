/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TMThong/Stardew-Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Characters;
using StardewValley.GameData;
using StardewValley.Menus;
using StardewValley.Minigames;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerMod.Framework.Mobile.Menus
{
    public class SCharacterCustomization : IClickableMenu
    {

        public static Multiplayer multiplayer
        {
            get
            {
                return ModUtilities.multiplayer;
            }
        }

        public SCharacterCustomization(Clothing item) : this(CharacterCustomization.Source.ClothesDye)
        {
            this._itemToDye = item;
            this.setUpPositions();
            if (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm)
            {
                Game1.spawnMonstersAtNight = false;
            }
            this._recolorPantsAction = delegate ()
            {
                this.DyeItem(this.pantsColorPicker.getSelectedColor());
            };
            if (this._itemToDye.clothesType.Value == 0)
            {
                this._displayFarmer.shirtItem.Set(this._itemToDye);
            }
            else if (this._itemToDye.clothesType.Value == 1)
            {
                this._displayFarmer.pantsItem.Set(this._itemToDye);
            }
            this._displayFarmer.UpdateClothing();
        }

        public void DyeItem(Color color)
        {
            if (this._itemToDye != null)
            {
                this._itemToDye.Dye(color, 1f);
                this._displayFarmer.FarmerRenderer.MarkSpriteDirty();
            }
        }

        public SCharacterCustomization(CharacterCustomization.Source source) : base(Game1.uiViewport.Width / 2 - (632 + IClickableMenu.borderWidth * 2) / 2, Game1.uiViewport.Height / 2 - (648 + IClickableMenu.borderWidth * 2) / 2 - 64, 632 + IClickableMenu.borderWidth * 2, 648 + IClickableMenu.borderWidth * 2 + 64, false)
        {
            this.LoadFarmTypeData();
            this.oldName = Game1.player.Name;
            int items_to_dye = 0;
            if (source == CharacterCustomization.Source.ClothesDye || source == CharacterCustomization.Source.DyePots)
            {
                this._isDyeMenu = true;
                if (source == CharacterCustomization.Source.ClothesDye)
                {
                    items_to_dye = 1;
                }
                else if (source == CharacterCustomization.Source.DyePots)
                {
                    if (Game1.player.pantsItem.Value != null && Game1.player.pantsItem.Value.dyeable.Value)
                    {
                        items_to_dye++;
                    }
                    if (Game1.player.shirtItem.Value != null && Game1.player.shirtItem.Value.dyeable.Value)
                    {
                        items_to_dye++;
                    }
                }
                this.height = 308 + IClickableMenu.borderWidth * 2 + 64 + 72 * items_to_dye - 4;
                this.xPositionOnScreen = Game1.uiViewport.Width / 2 - this.width / 2;
                this.yPositionOnScreen = Game1.uiViewport.Height / 2 - this.height / 2 - 64;
            }
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
            this._recolorEyesAction = delegate ()
            {
                Game1.player.changeEyeColor(this.eyeColorPicker.getSelectedColor());
            };
            this._recolorPantsAction = delegate ()
            {
                Game1.player.changePants(this.pantsColorPicker.getSelectedColor());
            };
            this._recolorHairAction = delegate ()
            {
                Game1.player.changeHairColor(this.hairColorPicker.getSelectedColor());
            };
            if (source == CharacterCustomization.Source.DyePots)
            {
                this._recolorHairAction = delegate ()
                {
                    if (Game1.player.shirtItem.Value != null && Game1.player.shirtItem.Value.dyeable.Value)
                    {
                        Game1.player.shirtItem.Value.clothesColor.Value = this.hairColorPicker.getSelectedColor();
                        Game1.player.FarmerRenderer.MarkSpriteDirty();
                        this._displayFarmer.FarmerRenderer.MarkSpriteDirty();
                    }
                };
                this._recolorPantsAction = delegate ()
                {
                    if (Game1.player.pantsItem.Value != null && Game1.player.pantsItem.Value.dyeable.Value)
                    {
                        Game1.player.pantsItem.Value.clothesColor.Value = this.pantsColorPicker.getSelectedColor();
                        Game1.player.FarmerRenderer.MarkSpriteDirty();
                        this._displayFarmer.FarmerRenderer.MarkSpriteDirty();
                    }
                };
                this.favThingBoxCC.visible = false;
                this.nameBoxCC.visible = false;
                this.farmnameBoxCC.visible = false;
                this.favoriteLabel.visible = false;
                this.nameLabel.visible = false;
                this.farmLabel.visible = false;
            }
            this._displayFarmer = this.GetOrCreateDisplayFarmer();
        }

        public Farmer GetOrCreateDisplayFarmer()
        {
            if (this._displayFarmer == null)
            {
                if (this.source == CharacterCustomization.Source.ClothesDye || this.source == CharacterCustomization.Source.DyePots)
                {
                    this._displayFarmer = Game1.player.CreateFakeEventFarmer();
                }
                else
                {
                    this._displayFarmer = Game1.player;
                }
                if (this.source == CharacterCustomization.Source.NewFarmhand)
                {
                    if (this._displayFarmer.pants.Value == -1)
                    {
                        this._displayFarmer.pants.Value = this._displayFarmer.GetPantsIndex();
                    }
                    if (this._displayFarmer.shirt.Value == -1)
                    {
                        this._displayFarmer.shirt.Value = this._displayFarmer.GetShirtIndex();
                    }
                }
                this._displayFarmer.faceDirection(2);
                this._displayFarmer.FarmerSprite.StopAnimation();
            }
            return this._displayFarmer;
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            base.gameWindowSizeChanged(oldBounds, newBounds);
            if (this._isDyeMenu)
            {
                this.xPositionOnScreen = Game1.uiViewport.Width / 2 - this.width / 2;
                this.yPositionOnScreen = Game1.uiViewport.Height / 2 - this.height / 2 - 64;
            }
            else
            {
                this.xPositionOnScreen = Game1.uiViewport.Width / 2 - (632 + IClickableMenu.borderWidth * 2) / 2;
                this.yPositionOnScreen = Game1.uiViewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2 - 64;
            }
            this.setUpPositions();
        }

        public void showAdvancedCharacterCreationHighlight()
        {
            this.advancedCCHighlightTimer = 4000f;
        }

        private void setUpPositions()
        {
            this.colorPickerCCs.Clear();
            if (this.source == CharacterCustomization.Source.ClothesDye && this._itemToDye == null)
            {
                return;
            }
            bool allow_accessory_changes = true;
            bool allow_clothing_changes = true;
            if (this.source == CharacterCustomization.Source.Wizard || this.source == CharacterCustomization.Source.ClothesDye || this.source == CharacterCustomization.Source.DyePots)
            {
                allow_clothing_changes = false;
            }
            if (this.source == CharacterCustomization.Source.ClothesDye || this.source == CharacterCustomization.Source.DyePots)
            {
                allow_accessory_changes = false;
            }
            this.labels.Clear();
            this.petButtons.Clear();
            this.genderButtons.Clear();
            this.cabinLayoutButtons.Clear();
            this.leftSelectionButtons.Clear();
            this.rightSelectionButtons.Clear();
            this.farmTypeButtons.Clear();
            if (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm)
            {
                this.advancedOptionsButton = new ClickableTextureComponent("Advanced", new Rectangle(this.xPositionOnScreen - 80, this.yPositionOnScreen + this.height - 80 - 16, 80, 80), null, null, Game1.mouseCursors2, new Rectangle(154, 154, 20, 20), 4f, false)
                {
                    myID = 636,
                    upNeighborID = -99998,
                    leftNeighborID = -99998,
                    rightNeighborID = -99998,
                    downNeighborID = -99998
                };
            }
            else
            {
                this.advancedOptionsButton = null;
            }
            this.okButton = new ClickableTextureComponent("OK", new Rectangle(this.xPositionOnScreen + this.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - 64, this.yPositionOnScreen + this.height - IClickableMenu.borderWidth - IClickableMenu.spaceToClearTopBorder + 16, 64, 64), null, null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46, -1, -1), 1f, false)
            {
                myID = 505,
                upNeighborID = -99998,
                leftNeighborID = -99998,
                rightNeighborID = -99998,
                downNeighborID = -99998
            };
            this.backButton = new ClickableComponent(new Rectangle(Game1.uiViewport.Width + -198 - 48, Game1.uiViewport.Height - 81 - 24, 198, 81), "")
            {
                myID = 81114,
                upNeighborID = -99998,
                leftNeighborID = -99998,
                rightNeighborID = -99998,
                downNeighborID = -99998
            };
            this.nameBox = new TextBox(Game1.content.Load<Texture2D>("LooseSprites\\textBox"), null, Game1.smallFont, Game1.textColor)
            {
                X = this.xPositionOnScreen + 64 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 256,
                Y = this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 16,
                Text = Game1.player.Name
            };
            this.nameBoxCC = new ClickableComponent(new Rectangle(this.xPositionOnScreen + 64 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 256, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 16, 192, 48), "")
            {
                myID = 536,
                upNeighborID = -99998,
                leftNeighborID = -99998,
                rightNeighborID = -99998,
                downNeighborID = -99998
            };
            int textBoxLabelsXOffset = (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ru || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.es || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.pt) ? -4 : 0;
            this.labels.Add(this.nameLabel = new ClickableComponent(new Rectangle(this.xPositionOnScreen + textBoxLabelsXOffset + 16 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 192 + 4, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 8, 1, 1), Game1.content.LoadString("Strings\\UI:Character_Name")));
            this.farmnameBox = new TextBox(Game1.content.Load<Texture2D>("LooseSprites\\textBox"), null, Game1.smallFont, Game1.textColor)
            {
                X = this.xPositionOnScreen + 64 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 256,
                Y = this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 16 + 64,
                Text = Game1.MasterPlayer.farmName
            };
            this.farmnameBoxCC = new ClickableComponent(new Rectangle(this.xPositionOnScreen + 64 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 256, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 16 + 64, 192, 48), "")
            {
                myID = 537,
                upNeighborID = -99998,
                leftNeighborID = -99998,
                rightNeighborID = -99998,
                downNeighborID = -99998
            };
            int farmLabelXOffset = (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko) ? -16 : 0;
            this.labels.Add(this.farmLabel = new ClickableComponent(new Rectangle(this.xPositionOnScreen + textBoxLabelsXOffset * 3 + 16 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 192 + 4 + farmLabelXOffset, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 16 + 64, 1, 1), Game1.content.LoadString("Strings\\UI:Character_Farm")));
            int favThingBoxXoffset = (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko) ? 48 : 0;
            this.favThingBox = new TextBox(Game1.content.Load<Texture2D>("LooseSprites\\textBox"), null, Game1.smallFont, Game1.textColor)
            {
                X = this.xPositionOnScreen + 64 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 256 + favThingBoxXoffset,
                Y = this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 16 + 128,
                Text = Game1.player.favoriteThing
            };
            this.favThingBoxCC = new ClickableComponent(new Rectangle(this.xPositionOnScreen + 64 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 256, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 16 + 128, 192, 48), "")
            {
                myID = 538,
                upNeighborID = -99998,
                leftNeighborID = -99998,
                rightNeighborID = -99998,
                downNeighborID = -99998
            };
            this.labels.Add(this.favoriteLabel = new ClickableComponent(new Rectangle(this.xPositionOnScreen + textBoxLabelsXOffset + 16 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 192 + 4, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 16 + 128, 1, 1), Game1.content.LoadString("Strings\\UI:Character_FavoriteThing")));
            this.randomButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + 48, this.yPositionOnScreen + 64 + 56, 40, 40), Game1.mouseCursors, new Rectangle(381, 361, 10, 10), 4f, false)
            {
                myID = 507,
                upNeighborID = -99998,
                leftNeighborImmutable = true,
                leftNeighborID = -99998,
                rightNeighborID = -99998,
                downNeighborID = -99998
            };
            if (this.source == CharacterCustomization.Source.DyePots || this.source == CharacterCustomization.Source.ClothesDye)
            {
                this.randomButton.visible = false;
            }
            this.portraitBox = new Rectangle(this.xPositionOnScreen + 64 + 42 - 2, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 16, 128, 192);
            if (this._isDyeMenu)
            {
                this.portraitBox.X = this.xPositionOnScreen + (this.width - this.portraitBox.Width) / 2;
                this.randomButton.bounds.X = this.portraitBox.X - 56;
            }
            int yOffset = 128;
            this.leftSelectionButtons.Add(new ClickableTextureComponent("Direction", new Rectangle(this.portraitBox.X - 32, this.portraitBox.Y + 144, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44, -1, -1), 1f, false)
            {
                myID = 520,
                upNeighborID = -99998,
                leftNeighborID = -99998,
                leftNeighborImmutable = true,
                rightNeighborID = -99998,
                downNeighborID = -99998
            });
            this.rightSelectionButtons.Add(new ClickableTextureComponent("Direction", new Rectangle(this.portraitBox.Right - 32, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33, -1, -1), 1f, false)
            {
                myID = 521,
                upNeighborID = -99998,
                leftNeighborID = -99998,
                rightNeighborID = -99998,
                downNeighborID = -99998
            });
            int leftSelectionXOffset = (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ru || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.es || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.pt) ? -20 : 0;
            this.isModifyingExistingPet = false;
            if (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm)
            {
                this.petPortraitBox = new Rectangle?(new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 448 - 16 + ((LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ru) ? 60 : 0), this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 192 - 16, 64, 64));
                this.labels.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + 16 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 192 + 8 + textBoxLabelsXOffset, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 8 + 192, 1, 1), Game1.content.LoadString("Strings\\UI:Character_Animal")));
            }
            if (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm || this.source == CharacterCustomization.Source.NewFarmhand || this.source == CharacterCustomization.Source.Wizard)
            {
                this.genderButtons.Add(new ClickableTextureComponent("Male", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 32 + 8, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 192, 64, 64), null, "Male", Game1.mouseCursors, new Rectangle(128, 192, 16, 16), 4f, false)
                {
                    myID = 508,
                    upNeighborID = -99998,
                    leftNeighborID = -99998,
                    rightNeighborID = -99998,
                    downNeighborID = -99998
                });
                this.genderButtons.Add(new ClickableTextureComponent("Female", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 32 + 64 + 24, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 192, 64, 64), null, "Female", Game1.mouseCursors, new Rectangle(144, 192, 16, 16), 4f, false)
                {
                    myID = 509,
                    upNeighborID = -99998,
                    leftNeighborID = -99998,
                    rightNeighborID = -99998,
                    downNeighborID = -99998
                });
                if (this.source == CharacterCustomization.Source.Wizard && this.genderButtons != null && this.genderButtons.Count > 0)
                {
                    int start_x = this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 320 + 16;
                    int start_y = this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 64 + 48;
                    for (int i = 0; i < this.genderButtons.Count; i++)
                    {
                        this.genderButtons[i].bounds.X = start_x + 80 * i;
                        this.genderButtons[i].bounds.Y = start_y;
                    }
                }
                yOffset = 256;
                if (this.source == CharacterCustomization.Source.Wizard)
                {
                    yOffset = 192;
                }
                leftSelectionXOffset = ((LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ru || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.es || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.pt || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.fr) ? -20 : 0);
                this.leftSelectionButtons.Add(new ClickableTextureComponent("Skin", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 16 + leftSelectionXOffset, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44, -1, -1), 1f, false)
                {
                    myID = 518,
                    upNeighborID = -99998,
                    leftNeighborID = -99998,
                    rightNeighborID = -99998,
                    downNeighborID = -99998
                });
                this.labels.Add(this.skinLabel = new ClickableComponent(new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 16 + 64 + 8 + leftSelectionXOffset / 2, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset + 16, 1, 1), Game1.content.LoadString("Strings\\UI:Character_Skin")));
                this.rightSelectionButtons.Add(new ClickableTextureComponent("Skin", new Rectangle(this.xPositionOnScreen + 16 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 128, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33, -1, -1), 1f, false)
                {
                    myID = 519,
                    upNeighborID = -99998,
                    leftNeighborID = -99998,
                    rightNeighborID = -99998,
                    downNeighborID = -99998
                });
            }
            if (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm)
            {
                Game1.startingCabins = 0;
                if (this.source == CharacterCustomization.Source.HostNewFarm)
                {
                    Game1.startingCabins = 1;
                }
                Game1.player.difficultyModifier = 1f;
                Game1.player.team.useSeparateWallets.Value = false;
                this.RefreshFarmTypeButtons();
            }
            if (this.source == CharacterCustomization.Source.HostNewFarm)
            {
                this.labels.Add(this.startingCabinsLabel = new ClickableComponent(new Rectangle(this.xPositionOnScreen - 21 - 128, this.yPositionOnScreen + IClickableMenu.borderWidth * 2 + 84, 1, 1), Game1.content.LoadString("Strings\\UI:Character_StartingCabins")));
                this.leftSelectionButtons.Add(new ClickableTextureComponent("Cabins", new Rectangle(this.xPositionOnScreen - 256 + IClickableMenu.borderWidth / 2 + 8, this.yPositionOnScreen + IClickableMenu.borderWidth * 2 + 108, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44, -1, -1), 1f, false)
                {
                    myID = 621,
                    upNeighborID = -99998,
                    leftNeighborID = -99998,
                    rightNeighborID = -99998,
                    downNeighborID = -99998
                });
                this.rightSelectionButtons.Add(new ClickableTextureComponent("Cabins", new Rectangle(this.xPositionOnScreen - 256 + IClickableMenu.borderWidth + 128 + 8, this.yPositionOnScreen + IClickableMenu.borderWidth * 2 + 108, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33, -1, -1), 1f, false)
                {
                    myID = 622,
                    upNeighborID = -99998,
                    leftNeighborID = -99998,
                    rightNeighborID = -99998,
                    downNeighborID = -99998
                });
                this.labels.Add(this.cabinLayoutLabel = new ClickableComponent(new Rectangle(this.xPositionOnScreen - 128 - (int)(Game1.smallFont.MeasureString(Game1.content.LoadString("Strings\\UI:Character_CabinLayout")).X / 2f), this.yPositionOnScreen + IClickableMenu.borderWidth * 2 + 120 + 64, 1, 1), Game1.content.LoadString("Strings\\UI:Character_CabinLayout")));
                this.cabinLayoutButtons.Add(new ClickableTextureComponent("Close", new Rectangle(this.xPositionOnScreen - 256 + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.borderWidth * 2 + 160 + 64, 64, 64), null, Game1.content.LoadString("Strings\\UI:Character_Close"), Game1.mouseCursors, new Rectangle(208, 192, 16, 16), 4f, false)
                {
                    myID = 623,
                    upNeighborID = -99998,
                    leftNeighborID = -99998,
                    rightNeighborID = -99998,
                    downNeighborID = -99998
                });
                this.cabinLayoutButtons.Add(new ClickableTextureComponent("Separate", new Rectangle(this.xPositionOnScreen - 256 + IClickableMenu.borderWidth + 128 - 8, this.yPositionOnScreen + IClickableMenu.borderWidth * 2 + 160 + 64, 64, 64), null, Game1.content.LoadString("Strings\\UI:Character_Separate"), Game1.mouseCursors, new Rectangle(224, 192, 16, 16), 4f, false)
                {
                    myID = 624,
                    upNeighborID = -99998,
                    leftNeighborID = -99998,
                    rightNeighborID = -99998,
                    downNeighborID = -99998
                });
                this.labels.Add(this.difficultyModifierLabel = new ClickableComponent(new Rectangle(this.xPositionOnScreen - 21 - 128, this.yPositionOnScreen + IClickableMenu.borderWidth * 2 + 256 + 56, 1, 1), Game1.content.LoadString("Strings\\UI:Character_Difficulty")));
                this.leftSelectionButtons.Add(new ClickableTextureComponent("Difficulty", new Rectangle(this.xPositionOnScreen - 256 + IClickableMenu.borderWidth / 2 - 4, this.yPositionOnScreen + IClickableMenu.borderWidth * 2 + 256 + 80, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44, -1, -1), 1f, false)
                {
                    myID = 627,
                    upNeighborID = -99998,
                    leftNeighborID = -99998,
                    rightNeighborID = -99998,
                    downNeighborID = -99998
                });
                this.rightSelectionButtons.Add(new ClickableTextureComponent("Difficulty", new Rectangle(this.xPositionOnScreen - 256 + IClickableMenu.borderWidth + 128 + 12, this.yPositionOnScreen + IClickableMenu.borderWidth * 2 + 256 + 80, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33, -1, -1), 1f, false)
                {
                    myID = 628,
                    upNeighborID = -99998,
                    leftNeighborID = -99998,
                    rightNeighborID = -99998,
                    downNeighborID = -99998
                });
                int walletY = this.yPositionOnScreen + IClickableMenu.borderWidth * 2 + 320 + 100;
                this.labels.Add(this.separateWalletLabel = new ClickableComponent(new Rectangle(this.xPositionOnScreen - 21 - 128, walletY - 24, 1, 1), Game1.content.LoadString("Strings\\UI:Character_Wallets")));
                this.leftSelectionButtons.Add(new ClickableTextureComponent("Wallets", new Rectangle(this.xPositionOnScreen - 256 + IClickableMenu.borderWidth / 2 - 4, walletY, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44, -1, -1), 1f, false)
                {
                    myID = 631,
                    upNeighborID = -99998,
                    leftNeighborID = -99998,
                    rightNeighborID = -99998,
                    downNeighborID = -99998
                });
                this.rightSelectionButtons.Add(new ClickableTextureComponent("Wallets", new Rectangle(this.xPositionOnScreen - 256 + IClickableMenu.borderWidth + 128 + 12, walletY, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33, -1, -1), 1f, false)
                {
                    myID = 632,
                    upNeighborID = -99998,
                    leftNeighborID = -99998,
                    rightNeighborID = -99998,
                    downNeighborID = -99998
                });
                this.coopHelpButton = new ClickableTextureComponent("CoopHelp", new Rectangle(this.xPositionOnScreen - 256 + IClickableMenu.borderWidth + 128 - 8, this.yPositionOnScreen + IClickableMenu.borderWidth * 2 + 448 + 40, 64, 64), null, Game1.content.LoadString("Strings\\UI:Character_CoopHelp"), Game1.mouseCursors, new Rectangle(240, 192, 16, 16), 4f, false)
                {
                    myID = 625,
                    upNeighborID = -99998,
                    leftNeighborID = -99998,
                    rightNeighborID = -99998,
                    downNeighborID = -99998
                };
                this.coopHelpOkButton = new ClickableTextureComponent("CoopHelpOK", new Rectangle(this.xPositionOnScreen - 256 - 12, this.yPositionOnScreen + this.height - 64, 64, 64), null, null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46, -1, -1), 1f, false)
                {
                    myID = 626,
                    region = 635,
                    upNeighborID = -99998,
                    leftNeighborID = -99998,
                    rightNeighborID = -99998,
                    downNeighborID = -99998
                };
                this.noneString = Game1.content.LoadString("Strings\\UI:Character_none");
                this.normalDiffString = Game1.content.LoadString("Strings\\UI:Character_Normal");
                this.toughDiffString = Game1.content.LoadString("Strings\\UI:Character_Tough");
                this.hardDiffString = Game1.content.LoadString("Strings\\UI:Character_Hard");
                this.superDiffString = Game1.content.LoadString("Strings\\UI:Character_Super");
                this.separateWalletString = Game1.content.LoadString("Strings\\UI:Character_SeparateWallet");
                this.sharedWalletString = Game1.content.LoadString("Strings\\UI:Character_SharedWallet");
                this.coopHelpRightButton = new ClickableTextureComponent("CoopHelpRight", new Rectangle(this.xPositionOnScreen + this.width, this.yPositionOnScreen + this.height, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33, -1, -1), 1f, false)
                {
                    myID = 633,
                    region = 635,
                    upNeighborID = -99998,
                    leftNeighborID = -99998,
                    rightNeighborID = -99998,
                    downNeighborID = -99998
                };
                this.coopHelpLeftButton = new ClickableTextureComponent("CoopHelpLeft", new Rectangle(this.xPositionOnScreen, this.yPositionOnScreen + this.height, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44, -1, -1), 1f, false)
                {
                    myID = 634,
                    region = 635,
                    upNeighborID = -99998,
                    leftNeighborID = -99998,
                    rightNeighborID = -99998,
                    downNeighborID = -99998
                };
            }
            Point top = new Point(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 320 + 48 + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset);
            int label_position = this.xPositionOnScreen + 16 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 192 + 8;
            if (this._isDyeMenu)
            {
                label_position = this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth;
            }
            if (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm || this.source == CharacterCustomization.Source.NewFarmhand || this.source == CharacterCustomization.Source.Wizard)
            {
                this.labels.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + 16 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 192 + 8, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset + 16, 1, 1), Game1.content.LoadString("Strings\\UI:Character_EyeColor")));
                this.eyeColorPicker = new ColorPicker("Eyes", top.X, top.Y);
                this.eyeColorPicker.setColor(Game1.player.newEyeColor);
                this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(top.X, top.Y, 128, 20), "")
                {
                    myID = 522,
                    downNeighborID = -99998,
                    upNeighborID = -99998,
                    leftNeighborImmutable = true,
                    rightNeighborImmutable = true
                });
                this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(top.X, top.Y + 20, 128, 20), "")
                {
                    myID = 523,
                    upNeighborID = -99998,
                    downNeighborID = -99998,
                    leftNeighborImmutable = true,
                    rightNeighborImmutable = true
                });
                this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(top.X, top.Y + 40, 128, 20), "")
                {
                    myID = 524,
                    upNeighborID = -99998,
                    downNeighborID = -99998,
                    leftNeighborImmutable = true,
                    rightNeighborImmutable = true
                });
                yOffset += 68;
                this.leftSelectionButtons.Add(new ClickableTextureComponent("Hair", new Rectangle(this.xPositionOnScreen + 16 + IClickableMenu.borderWidth + IClickableMenu.spaceToClearSideBorder + leftSelectionXOffset, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44, -1, -1), 1f, false)
                {
                    myID = 514,
                    upNeighborID = -99998,
                    leftNeighborID = -99998,
                    rightNeighborID = -99998,
                    downNeighborID = -99998
                });
                this.labels.Add(this.hairLabel = new ClickableComponent(new Rectangle(this.xPositionOnScreen + 16 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 64 + 8 + leftSelectionXOffset / 2, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset + 16, 1, 1), Game1.content.LoadString("Strings\\UI:Character_Hair")));
                this.rightSelectionButtons.Add(new ClickableTextureComponent("Hair", new Rectangle(this.xPositionOnScreen + 16 + IClickableMenu.spaceToClearSideBorder + 128 + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33, -1, -1), 1f, false)
                {
                    myID = 515,
                    upNeighborID = -99998,
                    leftNeighborID = -99998,
                    rightNeighborID = -99998,
                    downNeighborID = -99998
                });
            }
            top = new Point(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 320 + 48 + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset);
            if (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm || this.source == CharacterCustomization.Source.NewFarmhand || this.source == CharacterCustomization.Source.Wizard)
            {
                this.labels.Add(new ClickableComponent(new Rectangle(label_position, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset + 16, 1, 1), Game1.content.LoadString("Strings\\UI:Character_HairColor")));
                this.hairColorPicker = new ColorPicker("Hair", top.X, top.Y);
                this.hairColorPicker.setColor(Game1.player.hairstyleColor);
                this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(top.X, top.Y, 128, 20), "")
                {
                    myID = 525,
                    downNeighborID = -99998,
                    upNeighborID = -99998,
                    leftNeighborImmutable = true,
                    rightNeighborImmutable = true
                });
                this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(top.X, top.Y + 20, 128, 20), "")
                {
                    myID = 526,
                    upNeighborID = -99998,
                    downNeighborID = -99998,
                    leftNeighborImmutable = true,
                    rightNeighborImmutable = true
                });
                this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(top.X, top.Y + 40, 128, 20), "")
                {
                    myID = 527,
                    upNeighborID = -99998,
                    downNeighborID = -99998,
                    leftNeighborImmutable = true,
                    rightNeighborImmutable = true
                });
            }
            if (this.source == CharacterCustomization.Source.DyePots)
            {
                yOffset += 68;
                if (Game1.player.shirtItem.Value != null && Game1.player.shirtItem.Value.dyeable.Value)
                {
                    top = new Point(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 320 + 48 + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset);
                    top.X = this.xPositionOnScreen + this.width - IClickableMenu.spaceToClearSideBorder - IClickableMenu.borderWidth - 160;
                    this.labels.Add(new ClickableComponent(new Rectangle(label_position, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset + 16, 1, 1), Game1.content.LoadString("Strings\\UI:Character_ShirtColor")));
                    this.hairColorPicker = new ColorPicker("Hair", top.X, top.Y);
                    this.hairColorPicker.setColor(Game1.player.GetShirtColor());
                    this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(top.X, top.Y, 128, 20), "")
                    {
                        myID = 525,
                        downNeighborID = -99998,
                        upNeighborID = -99998,
                        leftNeighborImmutable = true,
                        rightNeighborImmutable = true
                    });
                    this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(top.X, top.Y + 20, 128, 20), "")
                    {
                        myID = 526,
                        upNeighborID = -99998,
                        downNeighborID = -99998,
                        leftNeighborImmutable = true,
                        rightNeighborImmutable = true
                    });
                    this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(top.X, top.Y + 40, 128, 20), "")
                    {
                        myID = 527,
                        upNeighborID = -99998,
                        downNeighborID = -99998,
                        leftNeighborImmutable = true,
                        rightNeighborImmutable = true
                    });
                    yOffset += 64;
                }
                if (Game1.player.pantsItem.Value != null && Game1.player.pantsItem.Value.dyeable.Value)
                {
                    top = new Point(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 320 + 48 + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset);
                    top.X = this.xPositionOnScreen + this.width - IClickableMenu.spaceToClearSideBorder - IClickableMenu.borderWidth - 160;
                    int pantsColorLabelYOffset = (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.tr) ? -16 : 0;
                    this.labels.Add(new ClickableComponent(new Rectangle(label_position, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset + 16 + pantsColorLabelYOffset, 1, 1), Game1.content.LoadString("Strings\\UI:Character_PantsColor")));
                    this.pantsColorPicker = new ColorPicker("Pants", top.X, top.Y);
                    this.pantsColorPicker.setColor(Game1.player.GetPantsColor());
                    this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(top.X, top.Y, 128, 20), "")
                    {
                        myID = 528,
                        downNeighborID = -99998,
                        upNeighborID = -99998,
                        rightNeighborImmutable = true,
                        leftNeighborImmutable = true
                    });
                    this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(top.X, top.Y + 20, 128, 20), "")
                    {
                        myID = 529,
                        downNeighborID = -99998,
                        upNeighborID = -99998,
                        rightNeighborImmutable = true,
                        leftNeighborImmutable = true
                    });
                    this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(top.X, top.Y + 40, 128, 20), "")
                    {
                        myID = 530,
                        downNeighborID = -99998,
                        upNeighborID = -99998,
                        rightNeighborImmutable = true,
                        leftNeighborImmutable = true
                    });
                }
            }
            else if (allow_clothing_changes)
            {
                yOffset += 68;
                int shirtArrowsExtraWidth = (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.tr) ? 8 : 0;
                this.leftSelectionButtons.Add(new ClickableTextureComponent("Shirt", new Rectangle(this.xPositionOnScreen + 16 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + leftSelectionXOffset - shirtArrowsExtraWidth, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44, -1, -1), 1f, false)
                {
                    myID = 512,
                    upNeighborID = -99998,
                    leftNeighborID = -99998,
                    rightNeighborID = -99998,
                    downNeighborID = -99998
                });
                this.labels.Add(this.shirtLabel = new ClickableComponent(new Rectangle(this.xPositionOnScreen + 16 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 64 + 8 + leftSelectionXOffset / 2, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset + 16, 1, 1), Game1.content.LoadString("Strings\\UI:Character_Shirt")));
                this.rightSelectionButtons.Add(new ClickableTextureComponent("Shirt", new Rectangle(this.xPositionOnScreen + 16 + IClickableMenu.spaceToClearSideBorder + 128 + IClickableMenu.borderWidth + shirtArrowsExtraWidth, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33, -1, -1), 1f, false)
                {
                    myID = 513,
                    upNeighborID = -99998,
                    leftNeighborID = -99998,
                    rightNeighborID = -99998,
                    downNeighborID = -99998
                });
                int pantsColorLabelYOffset2 = (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.tr) ? -16 : 0;
                this.labels.Add(new ClickableComponent(new Rectangle(label_position, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset + 16 + pantsColorLabelYOffset2, 1, 1), Game1.content.LoadString("Strings\\UI:Character_PantsColor")));
                top = new Point(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 320 + 48 + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset);
                this.pantsColorPicker = new ColorPicker("Pants", top.X, top.Y);
                this.pantsColorPicker.setColor(Game1.player.pantsColor);
                this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(top.X, top.Y, 128, 20), "")
                {
                    myID = 528,
                    downNeighborID = -99998,
                    upNeighborID = -99998,
                    rightNeighborImmutable = true,
                    leftNeighborImmutable = true
                });
                this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(top.X, top.Y + 20, 128, 20), "")
                {
                    myID = 529,
                    downNeighborID = -99998,
                    upNeighborID = -99998,
                    rightNeighborImmutable = true,
                    leftNeighborImmutable = true
                });
                this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(top.X, top.Y + 40, 128, 20), "")
                {
                    myID = 530,
                    downNeighborID = -99998,
                    upNeighborID = -99998,
                    rightNeighborImmutable = true,
                    leftNeighborImmutable = true
                });
            }
            else if (this.source == CharacterCustomization.Source.ClothesDye)
            {
                yOffset += 60;
                top = new Point(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 320 + 48 + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset);
                top.X = this.xPositionOnScreen + this.width - IClickableMenu.spaceToClearSideBorder - IClickableMenu.borderWidth - 160;
                this.labels.Add(new ClickableComponent(new Rectangle(label_position, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset + 16, 1, 1), Game1.content.LoadString("Strings\\UI:Character_DyeColor")));
                this.pantsColorPicker = new ColorPicker("Pants", top.X, top.Y);
                this.pantsColorPicker.setColor(this._itemToDye.clothesColor);
                this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(top.X, top.Y, 128, 20), "")
                {
                    myID = 528,
                    downNeighborID = -99998,
                    upNeighborID = -99998,
                    rightNeighborImmutable = true,
                    leftNeighborImmutable = true
                });
                this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(top.X, top.Y + 20, 128, 20), "")
                {
                    myID = 529,
                    downNeighborID = -99998,
                    upNeighborID = -99998,
                    rightNeighborImmutable = true,
                    leftNeighborImmutable = true
                });
                this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(top.X, top.Y + 40, 128, 20), "")
                {
                    myID = 530,
                    downNeighborID = -99998,
                    upNeighborID = -99998,
                    rightNeighborImmutable = true,
                    leftNeighborImmutable = true
                });
            }
            this.skipIntroButton = new ClickableTextureComponent("Skip Intro", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 320 - 48 + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset + 80, 36, 36), null, Game1.content.LoadString("Strings\\UI:Character_SkipIntro"), Game1.mouseCursors, new Rectangle(227, 425, 9, 9), 4f, false)
            {
                myID = 506,
                upNeighborID = 530,
                leftNeighborID = 517,
                rightNeighborID = 505
            };
            if (allow_clothing_changes)
            {
                yOffset += 68;
                this.leftSelectionButtons.Add(new ClickableTextureComponent("Pants Style", new Rectangle(this.xPositionOnScreen + 16 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + leftSelectionXOffset, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44, -1, -1), 1f, false)
                {
                    myID = 629,
                    upNeighborID = -99998,
                    leftNeighborID = -99998,
                    rightNeighborID = -99998,
                    downNeighborID = -99998
                });
                this.labels.Add(this.pantsStyleLabel = new ClickableComponent(new Rectangle(this.xPositionOnScreen + 16 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 64 + 8 + leftSelectionXOffset / 2, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset + 16, 1, 1), Game1.content.LoadString("Strings\\UI:Character_Pants")));
                this.rightSelectionButtons.Add(new ClickableTextureComponent("Pants Style", new Rectangle(this.xPositionOnScreen + 16 + IClickableMenu.spaceToClearSideBorder + 128 + IClickableMenu.borderWidth, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33, -1, -1), 1f, false)
                {
                    myID = 517,
                    upNeighborID = -99998,
                    leftNeighborID = -99998,
                    rightNeighborID = -99998,
                    downNeighborID = -99998
                });
            }
            yOffset += 68;
            if (allow_accessory_changes)
            {
                int accessoryArrowsExtraWidth = (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.tr) ? 32 : 0;
                this.leftSelectionButtons.Add(new ClickableTextureComponent("Acc", new Rectangle(this.xPositionOnScreen + 16 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + leftSelectionXOffset - accessoryArrowsExtraWidth, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44, -1, -1), 1f, false)
                {
                    myID = 516,
                    upNeighborID = -99998,
                    leftNeighborID = -99998,
                    rightNeighborID = -99998,
                    downNeighborID = -99998
                });
                this.labels.Add(this.accLabel = new ClickableComponent(new Rectangle(this.xPositionOnScreen + 16 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 64 + 8 + leftSelectionXOffset / 2, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset + 16, 1, 1), Game1.content.LoadString("Strings\\UI:Character_Accessory")));
                this.rightSelectionButtons.Add(new ClickableTextureComponent("Acc", new Rectangle(this.xPositionOnScreen + 16 + IClickableMenu.spaceToClearSideBorder + 128 + IClickableMenu.borderWidth + accessoryArrowsExtraWidth, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33, -1, -1), 1f, false)
                {
                    myID = 517,
                    upNeighborID = -99998,
                    leftNeighborID = -99998,
                    rightNeighborID = -99998,
                    downNeighborID = -99998
                });
            }
             
            if (this.petPortraitBox != null)
            {
                this.leftSelectionButtons.Add(new ClickableTextureComponent("Pet", new Rectangle(this.petPortraitBox.Value.Left - 64, this.petPortraitBox.Value.Top, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44, -1, -1), 1f, false)
                {
                    myID = 511,
                    upNeighborID = -99998,
                    leftNeighborID = -99998,
                    rightNeighborID = -99998,
                    downNeighborID = -99998
                });
                this.rightSelectionButtons.Add(new ClickableTextureComponent("Pet", new Rectangle(this.petPortraitBox.Value.Left + 64, this.petPortraitBox.Value.Top, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33, -1, -1), 1f, false)
                {
                    myID = 510,
                    upNeighborID = -99998,
                    leftNeighborID = -99998,
                    rightNeighborID = -99998,
                    downNeighborID = -99998
                });
                if (this.colorPickerCCs != null && this.colorPickerCCs.Count > 0)
                {
                    this.colorPickerCCs[0].upNeighborID = 511;
                    this.colorPickerCCs[0].upNeighborImmutable = true;
                }

            }
            this._shouldShowBackButton = true;
            if (this.source == CharacterCustomization.Source.Dresser || this.source == CharacterCustomization.Source.Wizard || this.source == CharacterCustomization.Source.ClothesDye)
            {
                this._shouldShowBackButton = false;
            }
            if (this.source == CharacterCustomization.Source.Dresser || this.source == CharacterCustomization.Source.Wizard || this._isDyeMenu)
            {
                this.nameBoxCC.visible = false;
                this.farmnameBoxCC.visible = false;
                this.favThingBoxCC.visible = false;
                this.farmLabel.visible = false;
                this.nameLabel.visible = false;
                this.favoriteLabel.visible = false;
            }
            if (this.source == CharacterCustomization.Source.Wizard)
            {
                this.nameLabel.visible = true;
                this.nameBoxCC.visible = true;
                this.favThingBoxCC.visible = true;
                this.favoriteLabel.visible = true;
                this.favThingBoxCC.bounds.Y = this.farmnameBoxCC.bounds.Y;
                this.favoriteLabel.bounds.Y = this.farmLabel.bounds.Y;
                this.favThingBox.Y = this.farmnameBox.Y;
            }
            if (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm)
            {
                this.skipIntroButton.visible = true;
            }
            else
            {
                this.skipIntroButton.visible = false;
            }
            if (Game1.options.snappyMenus && Game1.options.gamepadControls)
            {
                base.populateClickableComponentList();
                this.snapToDefaultClickableComponent();
            }
        }

        public virtual void LoadFarmTypeData()
        {
            List<ModFarmType> farm_types = Game1.content.Load<List<ModFarmType>>("Data\\AdditionalFarms");
            this.farmTypeButtonNames.Add("Standard");
            this.farmTypeButtonNames.Add("Riverland");
            this.farmTypeButtonNames.Add("Forest");
            this.farmTypeButtonNames.Add("Hills");
            this.farmTypeButtonNames.Add("Wilderness");
            this.farmTypeButtonNames.Add("Four Corners");
            this.farmTypeButtonNames.Add("Beach");
            this.farmTypeHoverText.Add(Game1.content.LoadString("Strings\\UI:Character_FarmStandard"));
            this.farmTypeHoverText.Add(Game1.content.LoadString("Strings\\UI:Character_FarmFishing"));
            this.farmTypeHoverText.Add(Game1.content.LoadString("Strings\\UI:Character_FarmForaging"));
            this.farmTypeHoverText.Add(Game1.content.LoadString("Strings\\UI:Character_FarmMining"));
            this.farmTypeHoverText.Add(Game1.content.LoadString("Strings\\UI:Character_FarmCombat"));
            this.farmTypeHoverText.Add(Game1.content.LoadString("Strings\\UI:Character_FarmFourCorners"));
            this.farmTypeHoverText.Add(Game1.content.LoadString("Strings\\UI:Character_FarmBeach"));
            this.farmTypeIcons.Add(new KeyValuePair<Texture2D, Rectangle>(Game1.mouseCursors, new Rectangle(0, 324, 22, 20)));
            this.farmTypeIcons.Add(new KeyValuePair<Texture2D, Rectangle>(Game1.mouseCursors, new Rectangle(22, 324, 22, 20)));
            this.farmTypeIcons.Add(new KeyValuePair<Texture2D, Rectangle>(Game1.mouseCursors, new Rectangle(44, 324, 22, 20)));
            this.farmTypeIcons.Add(new KeyValuePair<Texture2D, Rectangle>(Game1.mouseCursors, new Rectangle(66, 324, 22, 20)));
            this.farmTypeIcons.Add(new KeyValuePair<Texture2D, Rectangle>(Game1.mouseCursors, new Rectangle(88, 324, 22, 20)));
            this.farmTypeIcons.Add(new KeyValuePair<Texture2D, Rectangle>(Game1.mouseCursors, new Rectangle(0, 345, 22, 20)));
            this.farmTypeIcons.Add(new KeyValuePair<Texture2D, Rectangle>(Game1.mouseCursors, new Rectangle(22, 345, 22, 20)));
            if (farm_types != null)
            {
                foreach (ModFarmType farm_type in farm_types)
                {
                    this.farmTypeButtonNames.Add("ModFarm_" + farm_type.ID);
                    this.farmTypeHoverText.Add(Game1.content.LoadString(farm_type.TooltipStringPath));
                    if (farm_type.IconTexture != null)
                    {
                        Texture2D texture = Game1.content.Load<Texture2D>(farm_type.IconTexture);
                        this.farmTypeIcons.Add(new KeyValuePair<Texture2D, Rectangle>(texture, new Rectangle(0, 0, 22, 20)));
                    }
                    else
                    {
                        this.farmTypeIcons.Add(new KeyValuePair<Texture2D, Rectangle>(Game1.mouseCursors, new Rectangle(1, 324, 22, 20)));
                    }
                }
            }
            this._farmPages = 1;
            if (farm_types != null)
            {
                this._farmPages = (int)Math.Floor((double)((float)(this.farmTypeButtonNames.Count - 1) / 7f)) + 1;
            }
        }

        public virtual void RefreshFarmTypeButtons()
        {
            this.farmTypeButtons.Clear();
            Point baseFarmButton = new Point(this.xPositionOnScreen + this.width + 4 + 8, this.yPositionOnScreen + IClickableMenu.borderWidth);
            int index = this._currentFarmPage * 7;
            if (index < this.farmTypeButtonNames.Count)
            {
                this.farmTypeButtons.Add(new ClickableTextureComponent(this.farmTypeButtonNames[index], new Rectangle(baseFarmButton.X, baseFarmButton.Y + 88, 88, 80), null, this.farmTypeHoverText[index], this.farmTypeIcons[index].Key, this.farmTypeIcons[index].Value, 4f, false)
                {
                    myID = 531,
                    downNeighborID = -99998,
                    leftNeighborID = 537
                });
                index++;
            }
            if (index < this.farmTypeButtonNames.Count)
            {
                this.farmTypeButtons.Add(new ClickableTextureComponent(this.farmTypeButtonNames[index], new Rectangle(baseFarmButton.X, baseFarmButton.Y + 176, 88, 80), null, this.farmTypeHoverText[index], this.farmTypeIcons[index].Key, this.farmTypeIcons[index].Value, 4f, false)
                {
                    myID = 532,
                    upNeighborID = -99998,
                    leftNeighborID = -99998,
                    rightNeighborID = -99998,
                    downNeighborID = -99998
                });
                index++;
            }
            if (index < this.farmTypeButtonNames.Count)
            {
                this.farmTypeButtons.Add(new ClickableTextureComponent(this.farmTypeButtonNames[index], new Rectangle(baseFarmButton.X, baseFarmButton.Y + 264, 88, 80), null, this.farmTypeHoverText[index], this.farmTypeIcons[index].Key, this.farmTypeIcons[index].Value, 4f, false)
                {
                    myID = 533,
                    upNeighborID = -99998,
                    leftNeighborID = -99998,
                    rightNeighborID = -99998,
                    downNeighborID = -99998
                });
                index++;
            }
            if (index < this.farmTypeButtonNames.Count)
            {
                this.farmTypeButtons.Add(new ClickableTextureComponent(this.farmTypeButtonNames[index], new Rectangle(baseFarmButton.X, baseFarmButton.Y + 352, 88, 80), null, this.farmTypeHoverText[index], this.farmTypeIcons[index].Key, this.farmTypeIcons[index].Value, 4f, false)
                {
                    myID = 534,
                    upNeighborID = -99998,
                    leftNeighborID = -99998,
                    rightNeighborID = -99998,
                    downNeighborID = -99998
                });
                index++;
            }
            if (index < this.farmTypeButtonNames.Count)
            {
                this.farmTypeButtons.Add(new ClickableTextureComponent(this.farmTypeButtonNames[index], new Rectangle(baseFarmButton.X, baseFarmButton.Y + 440, 88, 80), null, this.farmTypeHoverText[index], this.farmTypeIcons[index].Key, this.farmTypeIcons[index].Value, 4f, false)
                {
                    myID = 535,
                    upNeighborID = -99998,
                    leftNeighborID = -99998,
                    rightNeighborID = -99998,
                    downNeighborID = -99998
                });
                index++;
            }
            if (index < this.farmTypeButtonNames.Count)
            {
                this.farmTypeButtons.Add(new ClickableTextureComponent(this.farmTypeButtonNames[index], new Rectangle(baseFarmButton.X, baseFarmButton.Y + 528, 88, 80), null, this.farmTypeHoverText[index], this.farmTypeIcons[index].Key, this.farmTypeIcons[index].Value, 4f, false)
                {
                    myID = 545,
                    upNeighborID = -99998,
                    leftNeighborID = -99998,
                    rightNeighborID = -99998,
                    downNeighborID = -99998
                });
                index++;
            }
            if (index < this.farmTypeButtonNames.Count)
            {
                this.farmTypeButtons.Add(new ClickableTextureComponent(this.farmTypeButtonNames[index], new Rectangle(baseFarmButton.X, baseFarmButton.Y + 616, 88, 80), null, this.farmTypeHoverText[index], this.farmTypeIcons[index].Key, this.farmTypeIcons[index].Value, 4f, false)
                {
                    myID = 546,
                    upNeighborID = -99998,
                    leftNeighborID = -99998,
                    rightNeighborID = -99998,
                    downNeighborID = -99998
                });
                index++;
            }
            this.farmTypePreviousPageButton = null;
            this.farmTypeNextPageButton = null;
            if (this._currentFarmPage > 0)
            {
                this.farmTypePreviousPageButton = new ClickableTextureComponent("", new Rectangle(baseFarmButton.X - 64 + 16, baseFarmButton.Y + 352 + 12, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44, -1, -1), 1f, false)
                {
                    myID = 547,
                    upNeighborID = -99998,
                    leftNeighborID = -99998,
                    rightNeighborID = -99998,
                    downNeighborID = -99998
                };
            }
            if (this._currentFarmPage < this._farmPages - 1)
            {
                this.farmTypeNextPageButton = new ClickableTextureComponent("", new Rectangle(baseFarmButton.X + 64 + 8, baseFarmButton.Y + 352 + 12, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33, -1, -1), 1f, false)
                {
                    myID = 547,
                    upNeighborID = -99998,
                    leftNeighborID = -99998,
                    rightNeighborID = -99998,
                    downNeighborID = -99998
                };
            }
        }

        public override void snapToDefaultClickableComponent()
        {
            if (this.showingCoopHelp)
            {
                this.currentlySnappedComponent = base.getComponentWithID(626);
            }
            else
            {
                this.currentlySnappedComponent = base.getComponentWithID(521);
            }
            this.snapCursorToCurrentSnappedComponent();
        }

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
                else if (b == Buttons.LeftThumbstickLeft || b == Buttons.DPadLeft)
                {
                    switch (this.currentlySnappedComponent.myID)
                    {
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
                            break;
                        default:
                            return;
                    }
                }
            }
        }

        public override void receiveGamePadButton(Buttons b)
        {
            base.receiveGamePadButton(b);
            if (this.currentlySnappedComponent != null)
            {
                if (b == Buttons.RightTrigger)
                {
                    int myID = this.currentlySnappedComponent.myID;
                    if (myID - 512 <= 9)
                    {
                        this.selectionClick(this.currentlySnappedComponent.name, 1);
                        return;
                    }
                }
                else if (b == Buttons.LeftTrigger)
                {
                    int myID = this.currentlySnappedComponent.myID;
                    if (myID - 512 <= 9)
                    {
                        this.selectionClick(this.currentlySnappedComponent.name, -1);
                        return;
                    }
                }
                else if (b == Buttons.B && this.showingCoopHelp)
                {
                    this.receiveLeftClick(this.coopHelpOkButton.bounds.Center.X, this.coopHelpOkButton.bounds.Center.Y, true);
                }
            }
        }

        private void optionButtonClick(string name)
        {
            if (name.StartsWith("ModFarm_"))
            {
                if (this.source != CharacterCustomization.Source.NewGame && this.source != CharacterCustomization.Source.HostNewFarm)
                {
                    goto IL_855;
                }
                List<ModFarmType> list = Game1.content.Load<List<ModFarmType>>("Data\\AdditionalFarms");
                string farm_id = name.Substring("ModFarm_".Length);
                using (List<ModFarmType>.Enumerator enumerator = list.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        ModFarmType farm_type = enumerator.Current;
                        if (farm_type.ID == farm_id)
                        {
                            Game1.whichFarm = 7;
                            Game1.whichModFarm = farm_type;
                            Game1.spawnMonstersAtNight = true;
                            break;
                        }
                    }
                    goto IL_855;
                }
            }
            uint num = ComputeStringHash(name);
            if (num <= 2246359087U)
            {
                if (num <= 1216165616U)
                {
                    if (num != 1485672U)
                    {
                        if (num != 989237149U)
                        {
                            if (num == 1216165616U)
                            {
                                if (name == "Male")
                                {
                                    Game1.player.changeGender(true);
                                    if (this.source != CharacterCustomization.Source.Wizard)
                                    {
                                        Game1.player.changeHairStyle(0);
                                    }
                                }
                            }
                        }
                        else if (name == "Wilderness")
                        {
                            if (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm)
                            {
                                Game1.whichFarm = 4;
                                Game1.spawnMonstersAtNight = true;
                            }
                        }
                    }
                    else if (name == "Standard")
                    {
                        if (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm)
                        {
                            Game1.whichFarm = 0;
                            Game1.spawnMonstersAtNight = false;
                        }
                    }
                }
                else if (num <= 1367651536U)
                {
                    if (num != 1265483177U)
                    {
                        if (num == 1367651536U)
                        {
                            if (name == "Riverland")
                            {
                                if (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm)
                                {
                                    Game1.whichFarm = 1;
                                    Game1.spawnMonstersAtNight = false;
                                }
                            }
                        }
                    }
                    else if (name == "Dog")
                    {
                        if (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm)
                        {
                            Game1.player.catPerson = false;
                        }
                    }
                }
                else if (num != 1761538983U)
                {
                    if (num == 2246359087U)
                    {
                        if (name == "OK")
                        {
                            if (!this.canLeaveMenu())
                            {
                                return;
                            }
                            if (this._itemToDye != null)
                            {
                                if (!Game1.player.IsEquippedItem(this._itemToDye))
                                {
                                    Utility.CollectOrDrop(this._itemToDye);
                                }
                                this._itemToDye = null;
                            }
                            if (this.source == CharacterCustomization.Source.ClothesDye)
                            {
                                Game1.exitActiveMenu();
                            }
                            else
                            {
                                Game1.player.Name = this.nameBox.Text.Trim();
                                Game1.player.displayName = Game1.player.Name;
                                Game1.player.favoriteThing.Value = this.favThingBox.Text.Trim();
                                Game1.player.isCustomized.Value = true;
                                Game1.player.ConvertClothingOverrideToClothesItems();
                                if (this.source == CharacterCustomization.Source.HostNewFarm)
                                {
                                    Game1.multiplayerMode = 2;
                                }
                                try
                                {
                                    if (Game1.player.Name != this.oldName && Game1.player.Name.IndexOf("[") != -1 && Game1.player.Name.IndexOf("]") != -1)
                                    {
                                        int start = Game1.player.Name.IndexOf("[");
                                        int end = Game1.player.Name.IndexOf("]");
                                        if (end > start)
                                        {
                                            string s = Game1.player.Name.Substring(start + 1, end - start - 1);
                                            int item_index = -1;
                                            if (int.TryParse(s, out item_index))
                                            {
                                                string itemName = Game1.objectInformation[item_index].Split('/', StringSplitOptions.None)[0];
                                                switch (Game1.random.Next(5))
                                                {
                                                    case 0:
                                                        Game1.chatBox.addMessage(Game1.content.LoadString("Strings\\UI:NameChange_EasterEgg1"), new Color(104, 214, 255));
                                                        break;
                                                    case 1:
                                                        Game1.chatBox.addMessage(Game1.content.LoadString("Strings\\UI:NameChange_EasterEgg2", Lexicon.makePlural(itemName, false)), new Color(100, 50, 255));
                                                        break;
                                                    case 2:
                                                        Game1.chatBox.addMessage(Game1.content.LoadString("Strings\\UI:NameChange_EasterEgg3", Lexicon.makePlural(itemName, false)), new Color(0, 220, 40));
                                                        break;
                                                    case 3:
                                                        Game1.chatBox.addMessage(Game1.content.LoadString("Strings\\UI:NameChange_EasterEgg4"), new Color(0, 220, 40));
                                                        DelayedAction.functionAfterDelay(delegate
                                                        {
                                                            Game1.chatBox.addMessage(Game1.content.LoadString("Strings\\UI:NameChange_EasterEgg5"), new Color(104, 214, 255));
                                                        }, 12000);
                                                        break;
                                                    case 4:
                                                        Game1.chatBox.addMessage(Game1.content.LoadString("Strings\\UI:NameChange_EasterEgg6", Lexicon.getProperArticleForWord(itemName), itemName), new Color(100, 120, 255));
                                                        break;
                                                }
                                            }
                                        }
                                    }
                                }
                                catch (Exception)
                                {
                                }
                                string changed_pet_name = null;
                               

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
                                        if (changed_pet_name != null)
                                        {
                                            multiplayer.globalChatInfoMessage("Makeover_Pet", new string[]
                                            {
                                                Game1.player.Name,
                                                changed_pet_name
                                            });
                                        }
                                        else
                                        {
                                            multiplayer.globalChatInfoMessage("Makeover", new string[]
                                            {
                                                Game1.player.Name
                                            });
                                        }
                                        Game1.flashAlpha = 1f;
                                        Game1.playSound("yoba");
                                    }
                                    else if (this.source == CharacterCustomization.Source.ClothesDye)
                                    {
                                        Game1.playSound("yoba");
                                    }
                                }
                            }
                        }
                    }
                }
                else if (name == "Cat")
                {
                    if (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm)
                    {
                        Game1.player.catPerson = true;
                    }
                }
            }
            else if (num <= 2773244447U)
            {
                if (num != 2503779456U)
                {
                    if (num != 2508411131U)
                    {
                        if (num == 2773244447U)
                        {
                            if (name == "Four Corners")
                            {
                                if (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm)
                                {
                                    Game1.whichFarm = 5;
                                    Game1.spawnMonstersAtNight = false;
                                }
                            }
                        }
                    }
                    else if (name == "Hills")
                    {
                        if (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm)
                        {
                            Game1.whichFarm = 3;
                            Game1.spawnMonstersAtNight = false;
                        }
                    }
                }
                else if (name == "Forest")
                {
                    if (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm)
                    {
                        Game1.whichFarm = 2;
                        Game1.spawnMonstersAtNight = false;
                    }
                }
            }
            else if (num <= 3333348840U)
            {
                if (num != 2877142840U)
                {
                    if (num == 3333348840U)
                    {
                        if (name == "Beach")
                        {
                            if (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm)
                            {
                                Game1.whichFarm = 6;
                                Game1.spawnMonstersAtNight = false;
                            }
                        }
                    }
                }
                else if (name == "Separate")
                {
                    Game1.cabinsSeparate = true;
                }
            }
            else if (num != 3448155331U)
            {
                if (num == 3634523321U)
                {
                    if (name == "Female")
                    {
                        Game1.player.changeGender(false);
                        if (this.source != CharacterCustomization.Source.Wizard)
                        {
                            Game1.player.changeHairStyle(16);
                        }
                    }
                }
            }
            else if (name == "Close")
            {
                Game1.cabinsSeparate = false;
            }
        IL_855:
            Game1.playSound("coin");
        }

        public bool petHasChanges(Pet pet)
        {
            return (Game1.player.catPerson && pet == null) || Game1.player.whichPetBreed != pet.whichBreed.Value;
        }
        internal static uint ComputeStringHash(string s)
        {
            uint num = 0;
            if (s != null)
            {
                num = 2166136261U;
                for (int i = 0; i < s.Length; i++)
                {
                    num = ((uint)s[i] ^ num) * 16777619U;
                }
            }
            return num;
        }
        private void selectionClick(string name, int change)
        {
            uint num = ComputeStringHash(name);
            if (num > 1644100618U)
            {
                if (num <= 2765422138U)
                {
                    if (num != 2233508368U)
                    {
                        if (num != 2765422138U)
                        {
                            return;
                        }
                        if (!(name == "Acc"))
                        {
                            return;
                        }
                        Game1.player.changeAccessory(Game1.player.accessory + change);
                        Game1.playSound("purchase");
                        return;
                    }
                    else
                    {
                        if (!(name == "Difficulty"))
                        {
                            return;
                        }
                        if (Game1.player.difficultyModifier < 1f && change < 0)
                        {
                            Game1.playSound("breathout");
                            Game1.player.difficultyModifier += 0.25f;
                            return;
                        }
                        if (Game1.player.difficultyModifier > 0.25f && change > 0)
                        {
                            Game1.playSound("batFlap");
                            Game1.player.difficultyModifier -= 0.25f;
                            return;
                        }
                    }
                }
                else if (num != 3013063128U)
                {
                    if (num != 3461384990U)
                    {
                        if (num != 3919500963U)
                        {
                            return;
                        }
                        if (!(name == "Shirt"))
                        {
                            return;
                        }
                        Game1.player.changeShirt(Game1.player.shirt + change, true);
                        Game1.playSound("coin");
                        return;
                    }
                    else
                    {
                        if (!(name == "Pet"))
                        {
                            return;
                        }
                        Game1.player.whichPetBreed += change;
                        if (Game1.player.whichPetBreed >= 3)
                        {
                            Game1.player.whichPetBreed = 0;
                            if (!this.isModifyingExistingPet)
                            {
                                Game1.player.catPerson = !Game1.player.catPerson;
                            }
                        }
                        else if (Game1.player.whichPetBreed < 0)
                        {
                            Game1.player.whichPetBreed = 2;
                            if (!this.isModifyingExistingPet)
                            {
                                Game1.player.catPerson = !Game1.player.catPerson;
                            }
                        }
                        Game1.playSound("coin");
                    }
                }
                else
                {
                    if (!(name == "Skin"))
                    {
                        return;
                    }
                    Game1.player.changeSkinColor(Game1.player.skin + change, false);
                    Game1.playSound("skeletonStep");
                    return;
                }
                return;
            }
            if (num <= 521700525U)
            {
                if (num != 482321747U)
                {
                    if (num != 521700525U)
                    {
                        return;
                    }
                    if (!(name == "Wallets"))
                    {
                        return;
                    }
                    if (Game1.player.team.useSeparateWallets)
                    {
                        Game1.playSound("coin");
                        Game1.player.team.useSeparateWallets.Value = false;
                        return;
                    }
                    Game1.playSound("coin");
                    Game1.player.team.useSeparateWallets.Value = true;
                    return;
                }
                else
                {
                    if (!(name == "Cabins"))
                    {
                        return;
                    }
                    if ((Game1.startingCabins != 0 || change >= 0) && (Game1.startingCabins != 3 || change <= 0))
                    {
                        Game1.playSound("axchop");
                    }
                    Game1.startingCabins += change;
                    Game1.startingCabins = Math.Max(0, Math.Min(3, Game1.startingCabins));
                    return;
                }
            }
            else if (num != 952529348U)
            {
                if (num != 1424250329U)
                {
                    if (num != 1644100618U)
                    {
                        return;
                    }
                    if (!(name == "Direction"))
                    {
                        return;
                    }
                    this._displayFarmer.faceDirection((this._displayFarmer.FacingDirection - change + 4) % 4);
                    this._displayFarmer.FarmerSprite.StopAnimation();
                    this._displayFarmer.completelyStopAnimatingOrDoingAction();
                    Game1.playSound("pickUpItem");
                    return;
                }
                else
                {
                    if (!(name == "Hair"))
                    {
                        return;
                    }
                    List<int> all_hairs = Farmer.GetAllHairstyleIndices();
                    int current_index = all_hairs.IndexOf(Game1.player.hair);
                    current_index += change;
                    if (current_index >= all_hairs.Count)
                    {
                        current_index = 0;
                    }
                    else if (current_index < 0)
                    {
                        current_index = all_hairs.Count<int>() - 1;
                    }
                    Game1.player.changeHairStyle(all_hairs[current_index]);
                    Game1.playSound("grassyStep");
                    return;
                }
            }
            else
            {
                if (!(name == "Pants Style"))
                {
                    return;
                }
                Game1.player.changePantStyle(Game1.player.pants + change, true);
                Game1.playSound("coin");
                return;
            }
        }

        public void ShowAdvancedOptions()
        {
            base.AddDependency();
            (TitleMenu.subMenu = new AdvancedGameOptions()).exitFunction = delegate ()
            {
                TitleMenu.subMenu = this;
                base.RemoveDependency();
                base.populateClickableComponentList();
                if (Game1.options.SnappyMenus)
                {
                    this.setCurrentlySnappedComponentTo(636);
                    this.snapCursorToCurrentSnappedComponent();
                }
            };
        }

        public override bool readyToClose()
        {
            if (this.showingCoopHelp)
            {
                return false;
            }
            if (Game1.lastCursorMotionWasMouse)
            {
                using (List<ClickableTextureComponent>.Enumerator enumerator = this.farmTypeButtons.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        if (enumerator.Current.containsPoint(Game1.getMouseX(true), Game1.getMouseY(true)))
                        {
                            return false;
                        }
                    }
                }
            }
            return base.readyToClose();
        }

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
                if (this.coopHelpScreen == 0 && this.coopHelpRightButton != null && this.coopHelpRightButton.containsPoint(x, y))
                {
                    this.coopHelpScreen++;
                    this.coopHelpString = Game1.parseText(Game1.content.LoadString("Strings\\UI:Character_CoopHelpString2").Replace("^", Environment.NewLine), Game1.dialogueFont, this.width + 384 - IClickableMenu.borderWidth * 2);
                    Game1.playSound("shwip");
                }
                if (this.coopHelpScreen == 1 && this.coopHelpLeftButton != null && this.coopHelpLeftButton.containsPoint(x, y))
                {
                    this.coopHelpScreen--;
                    this.coopHelpString = Game1.parseText(Game1.content.LoadString("Strings\\UI:Character_CoopHelpString").Replace("^", Environment.NewLine), Game1.dialogueFont, this.width + 384 - IClickableMenu.borderWidth * 2);
                    Game1.playSound("shwip");
                }
                return;
            }
            if (this.genderButtons.Count > 0)
            {
                foreach (ClickableComponent c in this.genderButtons)
                {
                    if (c.containsPoint(x, y))
                    {
                        this.optionButtonClick(c.name);
                        c.scale -= 0.5f;
                        c.scale = Math.Max(3.5f, c.scale);
                    }
                }
            }
            if (this.farmTypeNextPageButton != null && this.farmTypeNextPageButton.containsPoint(x, y))
            {
                Game1.playSound("shwip");
                this._currentFarmPage++;
                this.RefreshFarmTypeButtons();
            }
            else if (this.farmTypePreviousPageButton != null && this.farmTypePreviousPageButton.containsPoint(x, y))
            {
                Game1.playSound("shwip");
                this._currentFarmPage--;
                this.RefreshFarmTypeButtons();
            }
            else if (this.farmTypeButtons.Count > 0)
            {
                foreach (ClickableComponent c2 in this.farmTypeButtons)
                {
                    if (c2.containsPoint(x, y) && !c2.name.Contains("Gray"))
                    {
                        this.optionButtonClick(c2.name);
                        c2.scale -= 0.5f;
                        c2.scale = Math.Max(3.5f, c2.scale);
                    }
                }
            }
            if (this.petButtons.Count > 0)
            {
                foreach (ClickableComponent c3 in this.petButtons)
                {
                    if (c3.containsPoint(x, y))
                    {
                        this.optionButtonClick(c3.name);
                        c3.scale -= 0.5f;
                        c3.scale = Math.Max(3.5f, c3.scale);
                    }
                }
            }
            if (this.cabinLayoutButtons.Count > 0)
            {
                foreach (ClickableComponent c4 in this.cabinLayoutButtons)
                {
                    if (Game1.startingCabins > 0 && c4.containsPoint(x, y))
                    {
                        this.optionButtonClick(c4.name);
                        c4.scale -= 0.5f;
                        c4.scale = Math.Max(3.5f, c4.scale);
                    }
                }
            }
            if (this.leftSelectionButtons.Count > 0)
            {
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
            }
            if (this.rightSelectionButtons.Count > 0)
            {
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
            }
            if (this.okButton.containsPoint(x, y) && this.canLeaveMenu())
            {
                this.optionButtonClick(this.okButton.name);
                this.okButton.scale -= 0.25f;
                this.okButton.scale = Math.Max(0.75f, this.okButton.scale);
            }
            if (this.hairColorPicker != null && this.hairColorPicker.containsPoint(x, y))
            {
                Color color = this.hairColorPicker.click(x, y);
                if (this.source == CharacterCustomization.Source.DyePots)
                {
                    if (Game1.player.shirtItem.Value != null && Game1.player.shirtItem.Value.dyeable.Value)
                    {
                        Game1.player.shirtItem.Value.clothesColor.Value = color;
                        Game1.player.FarmerRenderer.MarkSpriteDirty();
                        this._displayFarmer.FarmerRenderer.MarkSpriteDirty();
                    }
                }
                else
                {
                    Game1.player.changeHairColor(color);
                }
                this.lastHeldColorPicker = this.hairColorPicker;
            }
            else if (this.pantsColorPicker != null && this.pantsColorPicker.containsPoint(x, y))
            {
                Color color2 = this.pantsColorPicker.click(x, y);
                if (this.source == CharacterCustomization.Source.DyePots)
                {
                    if (Game1.player.pantsItem.Value != null && Game1.player.pantsItem.Value.dyeable.Value)
                    {
                        Game1.player.pantsItem.Value.clothesColor.Value = color2;
                        Game1.player.FarmerRenderer.MarkSpriteDirty();
                        this._displayFarmer.FarmerRenderer.MarkSpriteDirty();
                    }
                }
                else if (this.source == CharacterCustomization.Source.ClothesDye)
                {
                    this.DyeItem(color2);
                }
                else
                {
                    Game1.player.changePants(color2);
                }
                this.lastHeldColorPicker = this.pantsColorPicker;
            }
            else if (this.eyeColorPicker != null && this.eyeColorPicker.containsPoint(x, y))
            {
                Game1.player.changeEyeColor(this.eyeColorPicker.click(x, y));
                this.lastHeldColorPicker = this.eyeColorPicker;
            }
            if (this.source != CharacterCustomization.Source.Dresser && this.source != CharacterCustomization.Source.ClothesDye && this.source != CharacterCustomization.Source.DyePots)
            {
                this.nameBox.Update();
                if (this.source == CharacterCustomization.Source.NewGame || this.source == CharacterCustomization.Source.HostNewFarm)
                {
                    this.farmnameBox.Update();
                }
                else
                {
                    this.farmnameBox.Text = Game1.MasterPlayer.farmName.Value;
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
                this.coopHelpScreen = 0;
                this.coopHelpString = Game1.parseText(Game1.content.LoadString("Strings\\UI:Character_CoopHelpString").Replace("^", Environment.NewLine), Game1.dialogueFont, this.width + 384 - IClickableMenu.borderWidth * 2);
                this.helpStringSize = Game1.dialogueFont.MeasureString(this.coopHelpString);
                this.coopHelpRightButton.bounds.Y = this.yPositionOnScreen + (int)this.helpStringSize.Y + IClickableMenu.borderWidth * 2 - 4;
                this.coopHelpRightButton.bounds.X = this.xPositionOnScreen + (int)this.helpStringSize.X - IClickableMenu.borderWidth * 5;
                this.coopHelpLeftButton.bounds.Y = this.yPositionOnScreen + (int)this.helpStringSize.Y + IClickableMenu.borderWidth * 2 - 4;
                this.coopHelpLeftButton.bounds.X = this.xPositionOnScreen - IClickableMenu.borderWidth * 4;
            }
            if (this.advancedOptionsButton != null && this.advancedOptionsButton.containsPoint(x, y))
            {
                Game1.playSound("drumkit6");
                this.ShowAdvancedOptions();
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
                if (this.accLabel != null && this.accLabel.visible)
                {
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
                }
                if (this.hairLabel != null && this.hairLabel.visible)
                {
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
                    this.hairColorPicker.setColor(hairColor);
                }
                if (this.shirtLabel != null && this.shirtLabel.visible)
                {
                    Game1.player.changeShirt(Game1.random.Next(112), false);
                }
                if (this.skinLabel != null && this.skinLabel.visible)
                {
                    Game1.player.changeSkinColor(Game1.random.Next(6), false);
                    if (Game1.random.NextDouble() < 0.25)
                    {
                        Game1.player.changeSkinColor(Game1.random.Next(24), false);
                    }
                }
                if (this.pantsStyleLabel != null && this.pantsStyleLabel.visible)
                {
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
                    this.pantsColorPicker.setColor(Game1.player.pantsColor);
                }
                if (this.eyeColorPicker != null)
                {
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
                    this.eyeColorPicker.setColor(Game1.player.newEyeColor);
                }
                this.randomButton.scale = 3.5f;
            }
        }

        public override void leftClickHeld(int x, int y)
        {
            this.colorPickerTimer -= Game1.currentGameTime.ElapsedGameTime.Milliseconds;
            if (this.colorPickerTimer <= 0)
            {
                if (this.lastHeldColorPicker != null && !Game1.options.SnappyMenus)
                {
                    if (this.lastHeldColorPicker.Equals(this.hairColorPicker))
                    {
                        Color color = this.hairColorPicker.clickHeld(x, y);
                        if (this.source == CharacterCustomization.Source.DyePots)
                        {
                            if (Game1.player.shirtItem.Value != null && Game1.player.shirtItem.Value.dyeable.Value)
                            {
                                Game1.player.shirtItem.Value.clothesColor.Value = color;
                                Game1.player.FarmerRenderer.MarkSpriteDirty();
                                this._displayFarmer.FarmerRenderer.MarkSpriteDirty();
                            }
                        }
                        else
                        {
                            Game1.player.changeHairColor(color);
                        }
                    }
                    if (this.lastHeldColorPicker.Equals(this.pantsColorPicker))
                    {
                        Color color2 = this.pantsColorPicker.clickHeld(x, y);
                        if (this.source == CharacterCustomization.Source.DyePots)
                        {
                            if (Game1.player.pantsItem.Value != null && Game1.player.pantsItem.Value.dyeable.Value)
                            {
                                Game1.player.pantsItem.Value.clothesColor.Value = color2;
                                Game1.player.FarmerRenderer.MarkSpriteDirty();
                                this._displayFarmer.FarmerRenderer.MarkSpriteDirty();
                            }
                        }
                        else if (this.source == CharacterCustomization.Source.ClothesDye)
                        {
                            this.DyeItem(color2);
                        }
                        else
                        {
                            Game1.player.changePants(color2);
                        }
                    }
                    if (this.lastHeldColorPicker.Equals(this.eyeColorPicker))
                    {
                        Game1.player.changeEyeColor(this.eyeColorPicker.clickHeld(x, y));
                    }
                }
                this.colorPickerTimer = 100;
            }
        }

        public override void releaseLeftClick(int x, int y)
        {
            if (this.hairColorPicker != null)
            {
                this.hairColorPicker.releaseClick();
            }
            if (this.pantsColorPicker != null)
            {
                this.pantsColorPicker.releaseClick();
            }
            if (this.eyeColorPicker != null)
            {
                this.eyeColorPicker.releaseClick();
            }
            this.lastHeldColorPicker = null;
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
        }

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
                        if (c3.hoverText.Contains('_'))
                        {
                            this.hoverTitle = c3.hoverText.Split('_', StringSplitOptions.None)[0];
                            this.hoverText = c3.hoverText.Split('_', StringSplitOptions.None)[1];
                        }
                        else
                        {
                            this.hoverTitle = null;
                            this.hoverText = c3.hoverText;
                        }
                    }
                    else
                    {
                        c3.scale = Math.Max(c3.scale - 0.02f, c3.baseScale);
                        if (c3.name.Contains("Gray") && c3.containsPoint(x, y))
                        {
                            this.hoverText = "Reach level 10 " + Game1.content.LoadString("Strings\\UI:Character_" + c3.name.Split('_', StringSplitOptions.None)[1]) + " to unlock.";
                        }
                    }
                }
            }
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
            if (this.coopHelpRightButton != null)
            {
                if (this.coopHelpRightButton.containsPoint(x, y))
                {
                    this.coopHelpRightButton.scale = Math.Min(this.coopHelpRightButton.scale + 0.025f, this.coopHelpRightButton.baseScale + 0.2f);
                }
                else
                {
                    this.coopHelpRightButton.scale = Math.Max(this.coopHelpRightButton.scale - 0.025f, this.coopHelpRightButton.baseScale);
                }
            }
            if (this.coopHelpLeftButton != null)
            {
                if (this.coopHelpLeftButton.containsPoint(x, y))
                {
                    this.coopHelpLeftButton.scale = Math.Min(this.coopHelpLeftButton.scale + 0.025f, this.coopHelpLeftButton.baseScale + 0.2f);
                }
                else
                {
                    this.coopHelpLeftButton.scale = Math.Max(this.coopHelpLeftButton.scale - 0.025f, this.coopHelpLeftButton.baseScale);
                }
            }
            if (this.advancedOptionsButton != null)
            {
                this.advancedOptionsButton.tryHover(x, y, 0.1f);
            }
            if (this.farmTypeNextPageButton != null)
            {
                this.farmTypeNextPageButton.tryHover(x, y, 0.1f);
            }
            if (this.farmTypePreviousPageButton != null)
            {
                this.farmTypePreviousPageButton.tryHover(x, y, 0.1f);
            }
            this.randomButton.tryHover(x, y, 0.25f);
            this.randomButton.tryHover(x, y, 0.25f);
            if ((this.hairColorPicker != null && this.hairColorPicker.containsPoint(x, y)) || (this.pantsColorPicker != null && this.pantsColorPicker.containsPoint(x, y)) || (this.eyeColorPicker != null && this.eyeColorPicker.containsPoint(x, y)))
            {
                Game1.SetFreeCursorDrag();
            }
            this.nameBox.Hover(x, y);
            this.farmnameBox.Hover(x, y);
            this.favThingBox.Hover(x, y);
            this.skipIntroButton.tryHover(x, y, 0.1f);
        }

        public bool canLeaveMenu()
        {
            return this.source == CharacterCustomization.Source.ClothesDye || this.source == CharacterCustomization.Source.DyePots || (Game1.player.Name.Length > 0 && Game1.player.farmName.Length > 0 && Game1.player.favoriteThing.Length > 0);
        }

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

        public override void draw(SpriteBatch b)
        {
            bool ignoreTitleSafe = true;
            if (this.showingCoopHelp)
            {
                IClickableMenu.drawTextureBox(b, this.xPositionOnScreen - 192, this.yPositionOnScreen + 64, (int)this.helpStringSize.X + IClickableMenu.borderWidth * 2, (int)this.helpStringSize.Y + IClickableMenu.borderWidth * 2, Color.White);
                Utility.drawTextWithShadow(b, this.coopHelpString, Game1.dialogueFont, new Vector2((float)(this.xPositionOnScreen + IClickableMenu.borderWidth - 192), (float)(this.yPositionOnScreen + IClickableMenu.borderWidth + 64)), Game1.textColor, 1f, -1f, -1, -1, 1f, 3);
                if (this.coopHelpOkButton != null)
                {
                    this.coopHelpOkButton.draw(b, Color.White, 0.95f, 0);
                }
                if (this.coopHelpRightButton != null)
                {
                    this.coopHelpRightButton.draw(b, Color.White, 0.95f, 0);
                }
                if (this.coopHelpLeftButton != null)
                {
                    this.coopHelpLeftButton.draw(b, Color.White, 0.95f, 0);
                }
                base.drawMouse(b, false, -1);
                return;
            }
            Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, false, true, null, false, ignoreTitleSafe, -1, -1, -1);
            if (this.source == CharacterCustomization.Source.HostNewFarm)
            {
                IClickableMenu.drawTextureBox(b, this.xPositionOnScreen - 256 + 4 - ((LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko) ? 25 : 0), this.yPositionOnScreen + IClickableMenu.borderWidth * 2 + 68, (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko) ? 320 : 256, 512, Color.White);
                foreach (ClickableTextureComponent c in this.cabinLayoutButtons)
                {
                    c.draw(b, Color.White * ((Game1.startingCabins > 0) ? 1f : 0.5f), 0.9f, 0);
                    if (Game1.startingCabins > 0 && ((c.name.Equals("Close") && !Game1.cabinsSeparate) || (c.name.Equals("Separate") && Game1.cabinsSeparate)))
                    {
                        b.Draw(Game1.mouseCursors, c.bounds, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 34, -1, -1)), Color.White);
                    }
                }
            }
            b.Draw(Game1.daybg, new Vector2((float)this.portraitBox.X, (float)this.portraitBox.Y), Color.White);
            foreach (ClickableComponent clickableComponent in this.genderButtons)
            {
                ClickableTextureComponent c2 = (ClickableTextureComponent)clickableComponent;
                if (c2.visible)
                {
                    c2.draw(b);
                    if ((c2.name.Equals("Male") && Game1.player.IsMale) || (c2.name.Equals("Female") && !Game1.player.IsMale))
                    {
                        b.Draw(Game1.mouseCursors, c2.bounds, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 34, -1, -1)), Color.White);
                    }
                }
            }
            foreach (ClickableComponent clickableComponent2 in this.petButtons)
            {
                ClickableTextureComponent c3 = (ClickableTextureComponent)clickableComponent2;
                if (c3.visible)
                {
                    c3.draw(b);
                    if ((c3.name.Equals("Cat") && Game1.player.catPerson) || (c3.name.Equals("Dog") && !Game1.player.catPerson))
                    {
                        b.Draw(Game1.mouseCursors, c3.bounds, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 34, -1, -1)), Color.White);
                    }
                }
            }
            if (this.nameBoxCC.visible)
            {
                Game1.player.Name = this.nameBox.Text;
            }
            if (this.favThingBoxCC.visible)
            {
                Game1.player.favoriteThing.Value = this.favThingBox.Text;
            }
            if (this.farmnameBoxCC.visible)
            {
                Game1.player.farmName.Value = this.farmnameBox.Text;
            }
            if (this.source == CharacterCustomization.Source.NewFarmhand)
            {
                Game1.player.farmName.Value = Game1.MasterPlayer.farmName.Value;
            }
            foreach (ClickableComponent clickableComponent3 in this.leftSelectionButtons)
            {
                ((ClickableTextureComponent)clickableComponent3).draw(b);
            }
            foreach (ClickableComponent c4 in this.labels)
            {
                if (c4.visible)
                {
                    string sub = "";
                    float offset = 0f;
                    float subYOffset = 0f;
                    Color color = Game1.textColor;
                    if (c4 == this.nameLabel)
                    {
                        color = ((Game1.player.Name != null && Game1.player.Name.Length < 1) ? Color.Red : Game1.textColor);
                    }
                    else if (c4 == this.farmLabel)
                    {
                        color = ((Game1.player.farmName.Value != null && Game1.player.farmName.Length < 1) ? Color.Red : Game1.textColor);
                    }
                    else if (c4 == this.favoriteLabel)
                    {
                        color = ((Game1.player.favoriteThing.Value != null && Game1.player.favoriteThing.Length < 1) ? Color.Red : Game1.textColor);
                    }
                    else if (c4 == this.shirtLabel)
                    {
                        offset = 21f - Game1.smallFont.MeasureString(c4.name).X / 2f;
                        sub = ((Game1.player.shirt + 1).ToString() ?? "");
                    }
                    else if (c4 == this.skinLabel)
                    {
                        offset = 21f - Game1.smallFont.MeasureString(c4.name).X / 2f;
                        sub = ((Game1.player.skin + 1).ToString() ?? "");
                    }
                    else if (c4 == this.hairLabel)
                    {
                        offset = 21f - Game1.smallFont.MeasureString(c4.name).X / 2f;
                        if (!c4.name.Contains("Color"))
                        {
                            sub = ((Farmer.GetAllHairstyleIndices().IndexOf(Game1.player.hair) + 1).ToString() ?? "");
                        }
                    }
                    else if (c4 == this.accLabel)
                    {
                        offset = 21f - Game1.smallFont.MeasureString(c4.name).X / 2f;
                        sub = ((Game1.player.accessory + 2).ToString() ?? "");
                    }
                    else if (c4 == this.pantsStyleLabel)
                    {
                        offset = 21f - Game1.smallFont.MeasureString(c4.name).X / 2f;
                        sub = ((Game1.player.pants + 1).ToString() ?? "");
                    }
                    else if (c4 == this.startingCabinsLabel)
                    {
                        offset = 21f - Game1.smallFont.MeasureString(c4.name).X / 2f;
                        sub = ((Game1.startingCabins == 0 && this.noneString != null) ? this.noneString : (Game1.startingCabins.ToString() ?? ""));
                        subYOffset = 4f;
                    }
                    else if (c4 == this.difficultyModifierLabel)
                    {
                        offset = 21f - Game1.smallFont.MeasureString(c4.name).X / 2f;
                        subYOffset = 4f;
                        sub = this.getNameOfDifficulty();
                    }
                    else if (c4 == this.separateWalletLabel)
                    {
                        offset = 21f - Game1.smallFont.MeasureString(c4.name).X / 2f;
                        subYOffset = 4f;
                        sub = (Game1.player.team.useSeparateWallets ? this.separateWalletString : this.sharedWalletString);
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
            }
            foreach (ClickableComponent clickableComponent4 in this.rightSelectionButtons)
            {
                ((ClickableTextureComponent)clickableComponent4).draw(b);
            }
            if (this.farmTypeButtons.Count > 0)
            {
                IClickableMenu.drawTextureBox(b, this.farmTypeButtons[0].bounds.X - 16, this.farmTypeButtons[0].bounds.Y - 20, 120, 652, Color.White);
                for (int i = 0; i < this.farmTypeButtons.Count; i++)
                {
                    this.farmTypeButtons[i].draw(b, this.farmTypeButtons[i].name.Contains("Gray") ? (Color.Black * 0.5f) : Color.White, 0.88f, 0);
                    if (this.farmTypeButtons[i].name.Contains("Gray"))
                    {
                        b.Draw(Game1.mouseCursors, new Vector2((float)(this.farmTypeButtons[i].bounds.Center.X - 12), (float)(this.farmTypeButtons[i].bounds.Center.Y - 8)), new Rectangle?(new Rectangle(107, 442, 7, 8)), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.89f);
                    }
                    bool farm_is_selected = false;
                    int index = i + this._currentFarmPage * 7;
                    if (Game1.whichFarm == 7)
                    {
                        if ("ModFarm_" + Game1.whichModFarm.ID == this.farmTypeButtonNames[index])
                        {
                            farm_is_selected = true;
                        }
                    }
                    else if (Game1.whichFarm == index)
                    {
                        farm_is_selected = true;
                    }
                    if (farm_is_selected)
                    {
                        IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(375, 357, 3, 3), this.farmTypeButtons[i].bounds.X, this.farmTypeButtons[i].bounds.Y - 4, this.farmTypeButtons[i].bounds.Width, this.farmTypeButtons[i].bounds.Height + 8, Color.White, 4f, false, -1f);
                    }
                }
                if (this.farmTypeNextPageButton != null)
                {
                    this.farmTypeNextPageButton.draw(b);
                }
                if (this.farmTypePreviousPageButton != null)
                {
                    this.farmTypePreviousPageButton.draw(b);
                }
            }
            if (this.petPortraitBox != null)
            {
                b.Draw(Game1.mouseCursors, this.petPortraitBox.Value, new Rectangle?(new Rectangle(160 + (Game1.player.catPerson ? 0 : 48) + Game1.player.whichPetBreed * 16, 208, 16, 16)), Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.89f);
            }
            if (this.advancedOptionsButton != null)
            {
                this.advancedOptionsButton.draw(b);
            }
            if (this.canLeaveMenu())
            {
                this.okButton.draw(b, Color.White, 0.75f, 0);
            }
            else
            {
                this.okButton.draw(b, Color.White, 0.75f, 0);
                this.okButton.draw(b, Color.Black * 0.5f, 0.751f, 0);
            }
            if (this.coopHelpButton != null)
            {
                this.coopHelpButton.draw(b, Color.White, 0.75f, 0);
            }
            if (this.hairColorPicker != null)
            {
                this.hairColorPicker.draw(b);
            }
            if (this.pantsColorPicker != null)
            {
                this.pantsColorPicker.draw(b);
            }
            if (this.eyeColorPicker != null)
            {
                this.eyeColorPicker.draw(b);
            }
            if (this.source != CharacterCustomization.Source.Dresser && this.source != CharacterCustomization.Source.DyePots && this.source != CharacterCustomization.Source.ClothesDye)
            {
                this.nameBox.Draw(b, true);
                this.favThingBox.Draw(b, true);
            }
            if (this.farmnameBoxCC.visible)
            {
                this.farmnameBox.Draw(b, true);
                Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\UI:Character_FarmNameSuffix"), Game1.smallFont, new Vector2((float)(this.farmnameBox.X + this.farmnameBox.Width + 8), (float)(this.farmnameBox.Y + 12)), Game1.textColor, 1f, -1f, -1, -1, 1f, 3);
            }
            if (this.skipIntroButton != null && this.skipIntroButton.visible)
            {
                this.skipIntroButton.draw(b);
                Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\UI:Character_SkipIntro"), Game1.smallFont, new Vector2((float)(this.skipIntroButton.bounds.X + this.skipIntroButton.bounds.Width + 8), (float)(this.skipIntroButton.bounds.Y + 8)), Game1.textColor, 1f, -1f, -1, -1, 1f, 3);
            }
            if (this.advancedCCHighlightTimer > 0f)
            {
                b.Draw(Game1.mouseCursors, this.advancedOptionsButton.getVector2() + new Vector2(4f, 84f), new Rectangle?(new Rectangle(128 + ((this.advancedCCHighlightTimer % 500f < 250f) ? 16 : 0), 208, 16, 16)), Color.White * Math.Min(1f, this.advancedCCHighlightTimer / 500f), 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.5f);
            }
            this.randomButton.draw(b);
            b.End();
            b.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);
            this._displayFarmer.FarmerRenderer.draw(b, this._displayFarmer.FarmerSprite.CurrentAnimationFrame, this._displayFarmer.FarmerSprite.CurrentFrame, this._displayFarmer.FarmerSprite.SourceRect, new Vector2((float)(this.portraitBox.Center.X - 32), (float)(this.portraitBox.Bottom - 160)), Vector2.Zero, 0.8f, Color.White, 0f, 1f, this._displayFarmer);
            b.End();
            b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);
            if (this.hoverText != null && this.hoverText.Count<char>() > 0)
            {
                IClickableMenu.drawHoverText(b, Game1.parseText(this.hoverText, Game1.smallFont, 256), Game1.smallFont, 0, 0, -1, this.hoverTitle, -1, null, null, 0, -1, -1, -1, -1, 1f, null, null);
            }
            base.drawMouse(b, false, -1);
        }

        public override void emergencyShutDown()
        {
            if (this._itemToDye != null)
            {
                if (!Game1.player.IsEquippedItem(this._itemToDye))
                {
                    Utility.CollectOrDrop(this._itemToDye);
                }
                this._itemToDye = null;
            }
            base.emergencyShutDown();
        }

        public override bool IsAutomaticSnapValid(int direction, ClickableComponent a, ClickableComponent b)
        {
            if (a.region != b.region)
            {
                return false;
            }
            if (this.advancedOptionsButton != null && this.backButton != null && a == this.advancedOptionsButton && b == this.backButton)
            {
                return false;
            }
            if (this.source == CharacterCustomization.Source.Wizard)
            {
                if (a == this.favThingBoxCC && b.myID >= 522 && b.myID <= 530)
                {
                    return false;
                }
                if (b == this.favThingBoxCC && a.myID >= 522 && a.myID <= 530)
                {
                    return false;
                }
            }
            if (this.source == CharacterCustomization.Source.Wizard)
            {
                if (a.name == "Direction" && b.name == "Pet")
                {
                    return false;
                }
                if (b.name == "Direction" && a.name == "Pet")
                {
                    return false;
                }
            }
            if (this.randomButton != null)
            {
                if (direction != 0)
                {
                    if (direction == 3)
                    {
                        if (b == this.randomButton && a.name == "Direction")
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (a == this.randomButton && b.name != "Direction")
                        {
                            return false;
                        }
                        if (b == this.randomButton && a.name != "Direction")
                        {
                            return false;
                        }
                    }
                }
                if (a.myID == 622 && direction == 1 && (b == this.nameBoxCC || b == this.favThingBoxCC || b == this.farmnameBoxCC))
                {
                    return false;
                }
            }
            return base.IsAutomaticSnapValid(direction, a, b);
        }

        public override void update(GameTime time)
        {
            base.update(time);
            if (this.showingCoopHelp)
            {
                this.backButton.visible = false;
                if (this.coopHelpScreen == 0)
                {
                    this.coopHelpRightButton.visible = true;
                    this.coopHelpLeftButton.visible = false;
                }
                else if (this.coopHelpScreen == 1)
                {
                    this.coopHelpRightButton.visible = false;
                    this.coopHelpLeftButton.visible = true;
                }
            }
            else
            {
                this.backButton.visible = this._shouldShowBackButton;
            }
            if (this._sliderOpTarget != null)
            {
                Color col = this._sliderOpTarget.getSelectedColor();
                if (this._sliderOpTarget.Dirty && this._sliderOpTarget.LastColor == col)
                {
                    this._sliderAction();
                    this._sliderOpTarget.LastColor = this._sliderOpTarget.getSelectedColor();
                    this._sliderOpTarget.Dirty = false;
                    this._sliderOpTarget = null;
                }
                else
                {
                    this._sliderOpTarget.LastColor = col;
                }
            }
            if (this.advancedCCHighlightTimer > 0f)
            {
                this.advancedCCHighlightTimer -= (float)time.ElapsedGameTime.TotalMilliseconds;
            }
        }

        protected override bool _ShouldAutoSnapPrioritizeAlignedElements()
        {
            return true;
        }

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

        public const int region_advancedOptions = 636;

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

        public const int region_farmSelection7 = 546;

        public const int region_farmSelectionLeft = 547;

        public const int region_farmSelectionRight = 548;

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

        public List<ClickableComponent> labels = new List<ClickableComponent>();

        public List<ClickableComponent> leftSelectionButtons = new List<ClickableComponent>();

        public List<ClickableComponent> rightSelectionButtons = new List<ClickableComponent>();

        public List<ClickableComponent> genderButtons = new List<ClickableComponent>();

        public List<ClickableComponent> petButtons = new List<ClickableComponent>();

        public List<ClickableTextureComponent> farmTypeButtons = new List<ClickableTextureComponent>();

        public ClickableTextureComponent farmTypeNextPageButton;

        public ClickableTextureComponent farmTypePreviousPageButton;

        private List<string> farmTypeButtonNames = new List<string>();

        private List<string> farmTypeHoverText = new List<string>();

        private List<KeyValuePair<Texture2D, Rectangle>> farmTypeIcons = new List<KeyValuePair<Texture2D, Rectangle>>();

        protected int _currentFarmPage;

        protected int _farmPages;

        public List<ClickableComponent> colorPickerCCs = new List<ClickableComponent>();

        public List<ClickableTextureComponent> cabinLayoutButtons = new List<ClickableTextureComponent>();

        public ClickableTextureComponent okButton;

        public ClickableTextureComponent skipIntroButton;

        public ClickableTextureComponent randomButton;

        public ClickableTextureComponent coopHelpButton;

        public ClickableTextureComponent coopHelpOkButton;

        public ClickableTextureComponent coopHelpRightButton;

        public ClickableTextureComponent coopHelpLeftButton;

        public ClickableTextureComponent advancedOptionsButton;

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

        protected bool _shouldShowBackButton = true;

        protected bool _isDyeMenu;

        protected Farmer _displayFarmer;

        public Rectangle portraitBox;

        public Rectangle? petPortraitBox;

        public string oldName = "";

        private float advancedCCHighlightTimer;

        private ColorPicker lastHeldColorPicker;

        private int timesRandom;
    }
}
