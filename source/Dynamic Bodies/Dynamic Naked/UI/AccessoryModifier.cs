/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ribeena/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.BellsAndWhistles;
using StardewValley.Characters;
using StardewValley.Objects;
using StardewValley.Menus;
using StardewValley;
using StardewModdingAPI;

using DynamicBodies.Data;
using HarmonyLib;

namespace DynamicBodies.UI
{
    internal class AccessoryModifier : IClickableMenu
	{
		private bool isWizardSubmenu = false;
		public const int region_okbutton = 505, region_backbutton = 81114, region_accLeft = 516, region_accRight = 517, region_directionLeft = 520, region_directionRight = 521;

		public const int region_colorPicker1 = 522, region_colorPicker2 = 523, region_colorPicker3 = 524;//primary
		public const int region_colorPicker4 = 525, region_colorPicker5 = 526, region_colorPicker6 = 527;//secondary

		public Farmer who;

		private PlayerBaseExtended pbe;

		private string hoverText;
		private string hoverTitle;

		public Dictionary<string, ClothingToggle> clothingToggles = new Dictionary<string, ClothingToggle>();
		public ClickableComponent hatButton;
		public ClickableComponent shirtButton;
		public ClickableComponent pantsButton;
		public ClickableComponent shoesButton;

		public List<ClickableComponent> labels = new List<ClickableComponent>();
		public List<ClickableComponent> leftSelectionButtons = new List<ClickableComponent>();
		public List<ClickableComponent> rightSelectionButtons = new List<ClickableComponent>();

		private TextBox nameBox;
		public ClickableTextureComponent okButton;
		public ClickableTextureComponent backButton;
		private ClickableComponent costLabel;
		public ClickableComponent layerLabel;
		public ClickableComponent trinketLabel;
		public ClickableComponent primaryLabel;
		public ClickableComponent secondaryLabel;

		public ClickableComponent accLabel;
		public List<int> accessoryOptions;

		private Dictionary<string, string> settingsBefore = new Dictionary<string, string>();

		public Rectangle CharacterBackgroundRect;

		public List<ClickableComponent> colorPickerCCs = new List<ClickableComponent>();
		public ColorPicker primaryColorPicker;
		private bool primaryColorActive = false;
		public ColorPicker secondaryColorPicker;
		private bool secondaryColorActive = false;
		private readonly Action _recolorPrimaryAction;
		private readonly Action _recolorSecondaryAction;
		private int colorPickerTimer;
		private ColorPicker _sliderOpTarget;
		private ColorPicker lastHeldColorPicker;
		private Action _sliderAction;
		public const int colorPickerTimerDelay = 100;

		private bool everythingOwned = true;
		private bool currentOwned = true;
		private List<String> ownedTrinkets;

		private int doubleBackTimer;

		protected Farmer _displayFarmer;
		public Rectangle portraitBox;

		protected Texture2D UItexture;

		int cost = 0;
		int costMultiplier = 1;
		public const int windowHeight = 508;

		int currentTrinket = 0;

		private static Color disabledColor = new Color(128, 128, 128, 128);
		private static Color purchase_selected_color = Color.Wheat;

		public AccessoryModifier(bool isWizardSubmenu = false) : base(Game1.uiViewport.Width / 2 - (632 + IClickableMenu.borderWidth* 2) / 2, Game1.uiViewport.Height / 2 - (windowHeight + IClickableMenu.borderWidth* 2) / 2 - 64, 632 + IClickableMenu.borderWidth* 2, windowHeight + IClickableMenu.borderWidth* 2 + 64)
		{
			CharacterBackgroundRect = new Rectangle(0, 80, 32, 48);

			who = Game1.player;

			this.isWizardSubmenu = isWizardSubmenu;

			if (ModEntry.Config.freecustomisation) costMultiplier = 0;

			this.pbe = PlayerBaseExtended.Get(who);
			if (pbe == null)
			{
				pbe = new PlayerBaseExtended(who, "Characters\\Farmer\\farmer_base");
			}

			//Store settings for resetting
			settingsBefore["acc"] = who.accessory.ToString();
            for (int i = 0; i < 5; i++)
            {
				if (who.modData.ContainsKey("DB.trinket" + i))
				{
					settingsBefore["DB.trinket" + i] = who.modData["DB.trinket" + i];
					if (who.modData.ContainsKey("DB.trinket" + i + "_c1"))
					{
						settingsBefore["DB.trinket" + i + "_c1"] = who.modData["DB.trinket" + i + "_c1"];
					}
					if (who.modData.ContainsKey("DB.trinket" + i + "_c2"))
					{
						settingsBefore["DB.trinket" + i + "_c2"] = who.modData["DB.trinket" + i + "_c2"];
					}
				} else
				{
                    settingsBefore["DB.trinket" + i] = "Default";
                }
			}
			currentTrinket = 0;

			if (!who.modData.ContainsKey("trinket_owned")) who.modData["trinket_owned"] = "Default";
            
			currentOwned = true;
			ownedTrinkets = who.modData["trinket_owned"].Split(",").ToList<string>();

			this.accessoryOptions = new List<int> { 0, 1, 2, 3, 4, 5 };
			this.UItexture = Game1.content.Load<Texture2D>("Mods/ribeena.dynamicbodies/assets/Interface/ui.png");

			clothingToggles["hat"] = new ClothingToggle() { offset = 16 };
			clothingToggles["hat"].active = clothingToggles["hat"].showing = who.hat.Value != null;
			clothingToggles["shirt"] = new ClothingToggle() { offset = 32 };
			clothingToggles["shirt"].active = clothingToggles["shirt"].showing = who.shirtItem.Value != null;
			clothingToggles["pants"] = new ClothingToggle() { offset = 48 };
			clothingToggles["pants"].active = clothingToggles["pants"].showing = who.pantsItem.Value != null;
			clothingToggles["shoes"] = new ClothingToggle() { offset = 64 };
			clothingToggles["shoes"].active = clothingToggles["shoes"].showing = who.boots.Value != null;

			this._recolorPrimaryAction = delegate
			{
				pbe.SetModData(who, "DB.trinket" + currentTrinket + "_c1", this.primaryColorPicker.getSelectedColor().PackedValue.ToString());
				pbe.trinkets[currentTrinket].texture = null;
				pbe.dirtyLayers["trinkets"] = true;
				pbe.UpdateTextures(who);
			};
			this._recolorSecondaryAction = delegate
			{
				pbe.SetModData(who, "DB.trinket" + currentTrinket + "_c2", this.secondaryColorPicker.getSelectedColor().PackedValue.ToString());
				pbe.trinkets[currentTrinket].texture = null;
				pbe.dirtyLayers["trinkets"] = true;
				pbe.UpdateTextures(who);
			};
			this._displayFarmer = this.GetOrCreateDisplayFarmer();

			setUpPositions();

			//Set up the colour pickers
			primaryColorPicker.setColor(WizardCharacterCharacterCustomization.RandomColor());
			secondaryColorPicker.setColor(WizardCharacterCharacterCustomization.RandomColor());
			if (who.modData.ContainsKey("DB.trinket" + currentTrinket))
			{
				Trinkets.ContentPackTrinketOption option = ModEntry.trinketOptions[0][currentTrinket] as Trinkets.ContentPackTrinketOption;
				primaryColorActive = option.settings.primaryColor;
				if (who.modData.ContainsKey("DB.trinket" + currentTrinket + "_c1"))
				{
					primaryColorPicker.setColor(new Color(uint.Parse(who.modData["DB.trinket" + currentTrinket + "_c1"])));
				}
				secondaryColorActive = option.settings.secondaryColor;
				if (who.modData.ContainsKey("DB.trinket" + currentTrinket + "_c2"))
				{
					secondaryColorPicker.setColor(new Color(uint.Parse(who.modData["DB.trinket" + currentTrinket + "_c2"])));
				}
			}

			//Set the colors if needed
			_recolorPrimaryAction();
			_recolorSecondaryAction();
		}

		//Make translation easier
		static public string T(string toTranslate)
		{
			return ModEntry.translate(toTranslate);
		}

		//Run actions needed based on what is happening
		public override void update(GameTime time)
		{
			base.update(time);

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
		}
		//farmer for rendering
		public Farmer GetOrCreateDisplayFarmer()
		{
			if (this._displayFarmer == null)
			{
				this._displayFarmer = who;
				this._displayFarmer.faceDirection(2);
				this._displayFarmer.FarmerSprite.StopAnimation();
			}
			return this._displayFarmer;
		}

		public void setUpPositions()
		{
			int arrow_size = 64;

			this.labels.Clear();
			this.leftSelectionButtons.Clear();
			this.rightSelectionButtons.Clear();

			this.okButton = new ClickableTextureComponent("OK", new Rectangle(base.xPositionOnScreen + base.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - 64, base.yPositionOnScreen + base.height - IClickableMenu.borderWidth - IClickableMenu.spaceToClearTopBorder + 16, 64, 64), null, null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f)
			{
				myID = 505,
				upNeighborID = -99998,
				leftNeighborID = -99998,
				rightNeighborID = -99998,
				downNeighborID = -99998
			};
			this.backButton = new ClickableTextureComponent("Back", new Rectangle(base.xPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearSideBorder, base.yPositionOnScreen + base.height - IClickableMenu.borderWidth - IClickableMenu.spaceToClearTopBorder + 16, 64, 64), null, null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 47), 1f)
			{
				myID = region_backbutton,
				upNeighborID = -99998,
				leftNeighborID = -99998,
				rightNeighborID = -99998,
				downNeighborID = -99998
			};

			

			//Draw the farmer
			this.portraitBox = new Rectangle(base.xPositionOnScreen + 64 + 4, base.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 16, 128, 192);

			//Arrows to change farmers direction
			this.leftSelectionButtons.Add(new ClickableTextureComponent("Direction", new Rectangle(this.portraitBox.X - arrow_size / 2, this.portraitBox.Y + 144, arrow_size, arrow_size), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44), 1f)
			{
				myID = region_directionLeft,
				upNeighborID = -99998,
				leftNeighborID = -99998,
				leftNeighborImmutable = true,
				rightNeighborID = -99998,
				downNeighborID = -99998
			});

			this.rightSelectionButtons.Add(new ClickableTextureComponent("Direction", new Rectangle(this.portraitBox.Right - arrow_size / 2, this.portraitBox.Y + 144, arrow_size, arrow_size), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33), 1f)
			{
				myID = region_directionRight,
				upNeighborID = -99998,
				leftNeighborID = -99998,
				rightNeighborID = -99998,
				downNeighborID = -99998
			});

			//Items next to portrait box
			this.nameBox = new TextBox(Game1.content.Load<Texture2D>("LooseSprites\\textBox"), null, Game1.smallFont, Game1.textColor)
			{
				X = portraitBox.X + portraitBox.Width + 10 * 4,
				Y = base.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 16,
				Width = 104 * 4,
				Text = T("trinkets")
			};

			int yOffset = 32;
			int label_col2_position = portraitBox.X + portraitBox.Width + 12 * 4;
			//Clothing toggle buttons
			foreach (KeyValuePair<string, ClothingToggle> ctkv in clothingToggles)
			{
				ctkv.Value.clickable = new ClickableComponent(new Rectangle(label_col2_position + (ctkv.Value.offset - 16) * 2 + (ctkv.Value.offset / 16) * 2, base.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset, 16 * 2, 16 * 2), ctkv.Key)
				{
					myID = 600 + ctkv.Value.offset / 16,
					upNeighborID = -99998,
					leftNeighborID = -99998,
					rightNeighborID = -99998,
					downNeighborID = -99998
				};
				switch (ctkv.Key)
				{
					case "hat":
						hatButton = ctkv.Value.clickable;
						break;
					case "shirt":
						shirtButton = ctkv.Value.clickable;
						break;
					case "pants":
						pantsButton = ctkv.Value.clickable;
						break;
					case "shoes":
						shoesButton = ctkv.Value.clickable;
						break;
				}
			}

			int leftPadding = 64 + 4;
			yOffset = 32;
			int label_col1_width = 42 * 4;

			int leftSelectionXOffset = ((LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ru || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.es || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.pt) ? (-20) : 0);


			label_col2_position = portraitBox.X + portraitBox.Width + 12 * 4;
			int label_col2_width = 40 * 4;

			//Items next to portrait box

			//Next line
			yOffset += 32;
			Point top = new Point(label_col2_position + label_col2_width, base.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset);

			//Vanilla accessories
			this.leftSelectionButtons.Add(new ClickableTextureComponent("Acc", new Rectangle(label_col2_position, top.Y, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44), 1f)
			{
				myID = region_accLeft,
				upNeighborID = -99998,
				leftNeighborID = -99998,
				rightNeighborID = -99998,
				downNeighborID = -99998
			});
			this.labels.Add(this.accLabel = new ClickableComponent(new Rectangle(label_col2_position + arrow_size / 2 + label_col1_width / 2, top.Y + 16, 1, 1), Game1.content.LoadString("Strings\\UI:Character_Accessory")));
			this.rightSelectionButtons.Add(new ClickableTextureComponent("Acc", new Rectangle(label_col2_position + label_col1_width + arrow_size / 2, top.Y, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33), 1f)
			{
				myID = region_accRight,
				upNeighborID = -99998,
				leftNeighborID = -99998,
				rightNeighborID = -99998,
				downNeighborID = -99998
			});


			//After portraitbox
			top.X = this.portraitBox.X - arrow_size / 2;
			top.Y = base.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 176;

			//Wider selections
			label_col1_width += 48;

			//Layer selector
			this.leftSelectionButtons.Add(new ClickableTextureComponent("Layer", new Rectangle(top.X, top.Y, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44), 1f)
			{
				myID = region_accLeft,
				upNeighborID = -99998,
				leftNeighborID = -99998,
				rightNeighborID = -99998,
				downNeighborID = -99998
			});
			this.labels.Add(this.layerLabel = new ClickableComponent(new Rectangle(top.X + arrow_size / 2 + label_col1_width / 2, top.Y + 16, 1, 1), T("trinket")));
			this.rightSelectionButtons.Add(new ClickableTextureComponent("Layer", new Rectangle(top.X + label_col1_width + arrow_size / 2, top.Y, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33), 1f)
			{
				myID = region_accRight,
				upNeighborID = -99998,
				leftNeighborID = -99998,
				rightNeighborID = -99998,
				downNeighborID = -99998
			});

			//New row
			top.Y += 60;
			//Trinket selector
			this.leftSelectionButtons.Add(new ClickableTextureComponent("Trinket", new Rectangle(top.X, top.Y, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44), 1f)
			{
				myID = region_accLeft,
				upNeighborID = -99998,
				leftNeighborID = -99998,
				rightNeighborID = -99998,
				downNeighborID = -99998
			});
			this.labels.Add(this.trinketLabel = new ClickableComponent(new Rectangle(top.X + arrow_size / 2 + label_col1_width / 2, top.Y + 16, 1, 1), T("trinket_style")));
			this.rightSelectionButtons.Add(new ClickableTextureComponent("Trinket", new Rectangle(top.X + label_col1_width + arrow_size / 2, top.Y, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33), 1f)
			{
				myID = region_accRight,
				upNeighborID = -99998,
				leftNeighborID = -99998,
				rightNeighborID = -99998,
				downNeighborID = -99998
			});

			//side column
			label_col2_position = top.X + arrow_size + label_col1_width + 48;
			label_col2_width = 30 * 4;

			this.labels.Add(new ClickableComponent(new Rectangle(label_col2_position, top.Y, 1, 1), T("primary_color")));

			this.primaryColorPicker = new ColorPicker("Primary", label_col2_position + label_col2_width, top.Y);
			this.primaryColorPicker.setColor(Color.Red);
			this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(label_col2_position + label_col2_width, top.Y, 128, 20), "")
			{
				myID = region_colorPicker1,
				downNeighborID = -99998,
				upNeighborID = -99998,
				leftNeighborImmutable = true,
				rightNeighborImmutable = true
			});
			this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(label_col2_position + label_col2_width, top.Y + 20, 128, 20), "")
			{
				myID = region_colorPicker2,
				upNeighborID = -99998,
				downNeighborID = -99998,
				leftNeighborImmutable = true,
				rightNeighborImmutable = true
			});
			this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(label_col2_position + label_col2_width, top.Y + 40, 128, 20), "")
			{
				myID = region_colorPicker3,
				upNeighborID = -99998,
				downNeighborID = -99998,
				leftNeighborImmutable = true,
				rightNeighborImmutable = true
			});

			//New row
			top.Y += 72;

			this.costLabel = new ClickableComponent(new Rectangle(top.X + (int)(arrow_size*0.75f) + label_col1_width / 2, top.Y+16, label_col1_width+ arrow_size / 2, 60), T("owned"));

			this.labels.Add(costLabel);

			this.labels.Add(new ClickableComponent(new Rectangle(label_col2_position, top.Y, 1, 1), T("secondary_color")));

			this.secondaryColorPicker = new ColorPicker("Secondary", label_col2_position + label_col2_width, top.Y);
			this.secondaryColorPicker.setColor(Color.Blue);
			this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(label_col2_position + label_col2_width, top.Y, 128, 20), "")
			{
				myID = region_colorPicker4,
				downNeighborID = -99998,
				upNeighborID = -99998,
				leftNeighborImmutable = true,
				rightNeighborImmutable = true
			});
			this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(label_col2_position + label_col2_width, top.Y + 20, 128, 20), "")
			{
				myID = region_colorPicker5,
				upNeighborID = -99998,
				downNeighborID = -99998,
				leftNeighborImmutable = true,
				rightNeighborImmutable = true
			});
			this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(label_col2_position + label_col2_width, top.Y + 40, 128, 20), "")
			{
				myID = region_colorPicker6,
				upNeighborID = -99998,
				downNeighborID = -99998,
				leftNeighborImmutable = true,
				rightNeighborImmutable = true
			});

			if (Game1.options.snappyMenus && Game1.options.gamepadControls)
			{
				base.populateClickableComponentList();
				this.snapToDefaultClickableComponent();
			}
		}

		//Close the window
		public override bool readyToClose()
		{
			RevertClothing();
			//Add close prevention
			return base.readyToClose();
		}

		private void RevertChanges()
		{
			PlayerBaseExtended pbe = PlayerBaseExtended.Get(who);
			who.accessory.Set(Int32.Parse(settingsBefore["acc"]));

			currentTrinket = 0;
			while (settingsBefore.ContainsKey("DB.trinket" + currentTrinket) && currentTrinket < 5)
			{
				who.modData["DB.trinket" + currentTrinket] = settingsBefore["DB.trinket" + currentTrinket];
				if (settingsBefore.ContainsKey("DB.trinket" + currentTrinket + "_c1"))
				{
					who.modData["DB.trinket" + currentTrinket + "_c1"] = settingsBefore["DB.trinket" + currentTrinket + "_c1"];
				}
				if (settingsBefore.ContainsKey("DB.trinket" + currentTrinket + "_c2"))
				{
					who.modData["DB.trinket" + currentTrinket + "_c2"] = settingsBefore["DB.trinket" + currentTrinket + "_c2"];
				}
				currentTrinket++;
			}
			pbe.dirtyLayers["trinkets"] = true;

			pbe.dirty = true;
		}

		public void RevertClothing()
		{
			if (!clothingToggles["pants"].showing)
			{
				who.pantsItem.Set(clothingToggles["pants"].storeItem as Clothing);

				clothingToggles["pants"].showing = true;
			}

			if (!clothingToggles["shoes"].showing)
			{
				who.boots.Set(clothingToggles["shoes"].storeItem as Boots);

				clothingToggles["shoes"].showing = true;
			}

			if (!clothingToggles["shirt"].showing)
			{
				who.shirtItem.Set(clothingToggles["shirt"].storeItem as Clothing);

				clothingToggles["shirt"].showing = true;
			}

			if (!clothingToggles["hat"].showing)
			{
				who.hat.Set(clothingToggles["hat"].storeItem as Hat);

				clothingToggles["hat"].showing = true;
			}
		}
		private void CheckOwnership()
        {
			CheckOwnershipAll();

			if (!who.modData.ContainsKey("DB.trinket" + currentTrinket))//Has to be default
			{
				currentOwned = true;
				return;
			}
			currentOwned = ownedTrinkets.Contains(who.modData["DB.trinket" + currentTrinket]);
			if(who.modData["DB.trinket" + currentTrinket] == "Default")
            {
				currentOwned = true;
            }
        }

		private void CheckOwnershipAll()
		{
			everythingOwned = true;
			for (int i = 0; i < 5; i++)
			{
				if (who.modData.ContainsKey("DB.trinket" + i))//Has to be non-default
				{

					if (!ownedTrinkets.Contains(who.modData["DB.trinket" + i]))
					{
						everythingOwned = false; //Not owned
						return;
					}
				}
			}
		}
		public override void emergencyShutDown()
		{
			RevertClothing();
			//Handle any reverts as required
			base.emergencyShutDown();
		}


		protected override void cleanupBeforeExit()
		{
			RevertClothing();
		}

		public void cancelAndExit()
		{
			RevertClothing();
			RevertChanges();
			if (isWizardSubmenu)
			{
				Game1.activeClickableMenu = new WizardCharacterCharacterCustomization();
				Game1.playSound("shwip");
			}
			else
			{
				this.exitThisMenu(false);
				Game1.playSound("cancel");
			}
		}

		public override void snapToDefaultClickableComponent()
		{
			base.currentlySnappedComponent = base.getComponentWithID(region_directionRight);
			this.snapCursorToCurrentSnappedComponent();
		}
		private void optionButtonClick(string name)
		{

			switch (name)
			{
				case "OK":
					{
						RevertClothing();
						who.isCustomized.Value = true;
						if (everythingOwned)
						{
                            //Set the new items ownership
                            who.modData["trinket_owned"] = string.Join(",", ownedTrinkets);

                            if (isWizardSubmenu)
							{
								Game1.activeClickableMenu = new WizardCharacterCharacterCustomization();
								Game1.playSound("shwip");
							}
							else
							{
								this.exitThisMenu(false);
								Game1.playSound("shwip");
							}
						} else
                        {
							Game1.playSound("cancel");
						}
						break;
					}
				case "Back":
					{

						cancelAndExit();
						break;
					}

			}

		}
		//Handle click particular items
		private void selectionClick(string name, int change)
		{
			List<string> all_trinkets;

			switch (name)
			{
				case "Acc":
					int newacc = (int)who.accessory.Value;
					//Skip the beards
					if (newacc >= 0 && newacc <= 6 && change < 0)
					{
						newacc = -1;
					}
					else if (newacc >= 18 && change > 0)
					{
						newacc = -1;
					}
					else if (newacc == -1 && change > 0)
					{
						newacc = 6;
					}
					else
					{
						newacc += change;
					}
					who.changeAccessory(newacc);
					Game1.playSound("purchase");
					break;
				case "Direction":
					this._displayFarmer.faceDirection((this._displayFarmer.FacingDirection - change + 4) % 4);
					this._displayFarmer.FarmerSprite.StopAnimation();
					this._displayFarmer.completelyStopAnimatingOrDoingAction();
					Game1.playSound("pickUpItem");
					break;
				case "Layer":
					currentTrinket = (currentTrinket + change) % 5;
					if (currentTrinket < 0) currentTrinket += 5;
					//Set up controls etc
					all_trinkets = ModEntry.getContentPackOptions(ModEntry.trinketOptions[currentTrinket]).ToList();
					if (all_trinkets.Count() > 0)
					{
						int current_index = all_trinkets.IndexOf((who.modData.ContainsKey("DB.trinket" + currentTrinket)) ? who.modData["DB.trinket" + currentTrinket] : "Default");
						if (current_index > -1)
						{
							CheckOwnership();
							Trinkets.ContentPackTrinketOption option = ModEntry.trinketOptions[currentTrinket][current_index] as Trinkets.ContentPackTrinketOption;
							primaryColorActive = option.settings.primaryColor;
							secondaryColorActive = option.settings.secondaryColor;
							if (currentOwned)
							{
								cost = 0;
								costLabel.name = T("owned");
							}
							else
							{
								cost = option.settings.cost * costMultiplier;
								if (cost > 0)
								{
									costLabel.name = T("buy_for") + " " + option.settings.cost;
								}
								else
								{
									costLabel.name = T("buy_for") + " " + T("free");
								}
							}
						}
						else
						{
							currentOwned = true;
							primaryColorActive = false;
							secondaryColorActive = false;
							cost = 0;
							costLabel.name = T("owned");
						}
						pbe.dirtyLayers["trinkets"] = true;
					}
					CheckOwnershipAll();

					Game1.playSound("pickUpItem");
					break;
				case "Trinket":
					{
						all_trinkets = ModEntry.getContentPackOptions(ModEntry.trinketOptions[currentTrinket]).ToList();
						if (all_trinkets.Count() > 0)
						{
							int current_index = all_trinkets.IndexOf((who.modData.ContainsKey("DB.trinket" + currentTrinket)) ? who.modData["DB.trinket" + currentTrinket] : "Default");
							current_index += change;
							if (current_index >= all_trinkets.Count)
							{
								current_index = -1;
							}
							else if (current_index < -1)
							{
								current_index = all_trinkets.Count() - 1;
							}
							
							//Apply controls etc
							if (current_index > -1)
							{
								pbe.SetModData(who, "DB.trinket" + currentTrinket, all_trinkets[current_index]);
								CheckOwnership();
								Trinkets.ContentPackTrinketOption option = ModEntry.trinketOptions[currentTrinket][current_index] as Trinkets.ContentPackTrinketOption;
								primaryColorActive = option.settings.primaryColor;
								secondaryColorActive = option.settings.secondaryColor;
								if (currentOwned)
								{
									cost = 0;
									costLabel.name = T("owned");
								}
								else
								{
									cost = option.settings.cost*costMultiplier;
									if (cost > 0)
									{
										costLabel.name = T("buy_for") + " " + option.settings.cost;
									} else
                                    {
										costLabel.name = T("buy_for") + " " + T("free");
									}
								}
							} else
                            {
								pbe.SetModData(who, "DB.trinket" + currentTrinket, "Default");
								currentOwned = true;
								primaryColorActive = false;
								secondaryColorActive = false;
								cost = 0;
								costLabel.name = T("owned");
							}

							pbe.dirtyLayers["trinkets"] = true;
							Game1.playSound("grassyStep");
						} else
                        {
							pbe.SetModData(who, "DB.trinket" + currentTrinket, "Default");
						}
						CheckOwnershipAll();
					}
					break;
			}
		}

		//Handle gamepad pressed
		public override void gamePadButtonHeld(Buttons b)
		{
			base.gamePadButtonHeld(b);
			if (base.currentlySnappedComponent == null)
			{
				return;
			}
			switch (b)
			{
				case Buttons.DPadRight:
				case Buttons.LeftThumbstickRight:
					switch (base.currentlySnappedComponent.myID)
					{
						case region_colorPicker1:
							this.primaryColorPicker.LastColor = this.primaryColorPicker.getSelectedColor();
							this.primaryColorPicker.changeHue(1);
							this.primaryColorPicker.Dirty = true;
							this._sliderOpTarget = this.primaryColorPicker;
							this._sliderAction = this._recolorPrimaryAction;
							break;
						case region_colorPicker2:
							this.primaryColorPicker.LastColor = this.primaryColorPicker.getSelectedColor();
							this.primaryColorPicker.changeSaturation(1);
							this.primaryColorPicker.Dirty = true;
							this._sliderOpTarget = this.primaryColorPicker;
							this._sliderAction = this._recolorPrimaryAction;
							break;
						case region_colorPicker3:
							this.primaryColorPicker.LastColor = this.primaryColorPicker.getSelectedColor();
							this.primaryColorPicker.changeValue(1);
							this.primaryColorPicker.Dirty = true;
							this._sliderOpTarget = this.primaryColorPicker;
							this._sliderAction = this._recolorPrimaryAction;
							break;
						case region_colorPicker4:
							this.secondaryColorPicker.LastColor = this.secondaryColorPicker.getSelectedColor();
							this.secondaryColorPicker.changeHue(1);
							this.secondaryColorPicker.Dirty = true;
							this._sliderOpTarget = this.secondaryColorPicker;
							this._sliderAction = this._recolorSecondaryAction;
							break;
						case region_colorPicker5:
							this.secondaryColorPicker.LastColor = this.secondaryColorPicker.getSelectedColor();
							this.secondaryColorPicker.changeSaturation(1);
							this.secondaryColorPicker.Dirty = true;
							this._sliderOpTarget = this.secondaryColorPicker;
							this._sliderAction = this._recolorSecondaryAction;
							break;
						case region_colorPicker6:
							this.secondaryColorPicker.LastColor = this.secondaryColorPicker.getSelectedColor();
							this.secondaryColorPicker.changeValue(1);
							this.secondaryColorPicker.Dirty = true;
							this._sliderOpTarget = this.secondaryColorPicker;
							this._sliderAction = this._recolorSecondaryAction;
							break;
					}
					break;
				case Buttons.DPadLeft:
				case Buttons.LeftThumbstickLeft:
					switch (base.currentlySnappedComponent.myID)
					{
						case region_colorPicker1:
							this.primaryColorPicker.LastColor = this.primaryColorPicker.getSelectedColor();
							this.primaryColorPicker.changeHue(-1);
							this.primaryColorPicker.Dirty = true;
							this._sliderOpTarget = this.primaryColorPicker;
							this._sliderAction = this._recolorPrimaryAction;
							break;
						case region_colorPicker2:
							this.primaryColorPicker.LastColor = this.primaryColorPicker.getSelectedColor();
							this.primaryColorPicker.changeSaturation(-1);
							this.primaryColorPicker.Dirty = true;
							this._sliderOpTarget = this.primaryColorPicker;
							this._sliderAction = this._recolorPrimaryAction;
							break;
						case region_colorPicker3:
							this.primaryColorPicker.LastColor = this.primaryColorPicker.getSelectedColor();
							this.primaryColorPicker.changeValue(-1);
							this.primaryColorPicker.Dirty = true;
							this._sliderOpTarget = this.primaryColorPicker;
							this._sliderAction = this._recolorPrimaryAction;
							break;
						case region_colorPicker4:
							this.secondaryColorPicker.LastColor = this.secondaryColorPicker.getSelectedColor();
							this.secondaryColorPicker.changeHue(-1);
							this.secondaryColorPicker.Dirty = true;
							this._sliderOpTarget = this.secondaryColorPicker;
							this._sliderAction = this._recolorSecondaryAction;
							break;
						case region_colorPicker5:
							this.secondaryColorPicker.LastColor = this.secondaryColorPicker.getSelectedColor();
							this.secondaryColorPicker.changeSaturation(-1);
							this.secondaryColorPicker.Dirty = true;
							this._sliderOpTarget = this.secondaryColorPicker;
							this._sliderAction = this._recolorSecondaryAction;
							break;
						case region_colorPicker6:
							this.secondaryColorPicker.LastColor = this.secondaryColorPicker.getSelectedColor();
							this.secondaryColorPicker.changeValue(-1);
							this.secondaryColorPicker.Dirty = true;
							this._sliderOpTarget = this.secondaryColorPicker;
							this._sliderAction = this._recolorSecondaryAction;
							break;
					}
					break;
				//Jumpt to the exit button when pressing b
				case Buttons.B:
					if (base.currentlySnappedComponent.myID == region_backbutton)
					{
						this.doubleBackTimer -= Game1.currentGameTime.ElapsedGameTime.Milliseconds;
						if (this.doubleBackTimer <= 0)
						{
							cancelAndExit();
						}
					}
					else
					{
						doubleBackTimer = Game1.currentGameTime.ElapsedGameTime.Milliseconds + 200;
						base.currentlySnappedComponent = base.getComponentWithID(region_backbutton);
						this.snapCursorToCurrentSnappedComponent();
					}
					break;
			}
		}
		//Handle gamepad input
		public override void receiveGamePadButton(Buttons b)
		{
			base.receiveGamePadButton(b);
			if (base.currentlySnappedComponent == null)
			{
				return;
			}
			switch (b)
			{
				case Buttons.RightTrigger:
					{
						int myID = base.currentlySnappedComponent.myID;
						if ((uint)(myID - 512) <= 9u)
						{
							this.selectionClick(base.currentlySnappedComponent.name, 1);
						}
						break;
					}
				case Buttons.LeftTrigger:
					{
						int myID = base.currentlySnappedComponent.myID;
						if ((uint)(myID - 512) <= 9u)
						{
							this.selectionClick(base.currentlySnappedComponent.name, -1);
						}
						break;
					}
			}
		}
		//Handle mouse down event
		public override void receiveLeftClick(int x, int y, bool playSound = true)
		{
			if (this.leftSelectionButtons.Count > 0)
			{
				foreach (ClickableComponent c2 in this.leftSelectionButtons)
				{
					if (c2.containsPoint(x, y))
					{
						this.selectionClick(c2.name, -1);
						if (c2.scale != 0f)
						{
							c2.scale -= 0.25f;
							c2.scale = Math.Max(0.75f, c2.scale);
						}
					}
				}
			}
			if (this.rightSelectionButtons.Count > 0)
			{
				foreach (ClickableComponent c in this.rightSelectionButtons)
				{
					if (c.containsPoint(x, y))
					{
						this.selectionClick(c.name, 1);
						if (c.scale != 0f)
						{
							c.scale -= 0.25f;
							c.scale = Math.Max(0.75f, c.scale);
						}
					}
				}
			}

			if (this.okButton.containsPoint(x, y))// && canPay())
			{
				this.optionButtonClick(this.okButton.name);
				this.okButton.scale -= 0.25f;
				this.okButton.scale = Math.Max(0.75f, this.okButton.scale);
			}

			if (this.backButton.containsPoint(x, y))
			{
				this.optionButtonClick(this.backButton.name);
				this.backButton.scale -= 0.25f;
				this.backButton.scale = Math.Max(0.75f, this.backButton.scale);
			}

			if (this.primaryColorPicker != null && this.primaryColorPicker.containsPoint(x, y))
			{
				Color color2 = this.primaryColorPicker.click(x, y);
				who.modData["DB.trinket" + currentTrinket + "_c1"] = color2.PackedValue.ToString();
				pbe.dirtyLayers["trinkets"] = true;
				pbe.UpdateTextures(who);
				this.lastHeldColorPicker = this.primaryColorPicker;
			}
			else if (this.secondaryColorPicker != null && this.secondaryColorPicker.containsPoint(x, y))
			{
				Color color2 = this.secondaryColorPicker.click(x, y);
				who.modData["DB.trinket" + currentTrinket + "_c2"] = color2.PackedValue.ToString();
				pbe.dirtyLayers["trinkets"] = true;
				pbe.UpdateTextures(who);
				this.lastHeldColorPicker = this.secondaryColorPicker;
			}

			if (this.costLabel.containsPoint(x + costLabel.bounds.Width / 2, y))
			{
				if (currentOwned) { Game1.playSound("cancel"); }
				else
				{
					if (who.Money >= cost)
					{
						ownedTrinkets.Add(who.modData["DB.trinket" + currentTrinket]);
						who.Money -= cost;
						currentOwned = true;
						costLabel.name = T("owned");
						CheckOwnershipAll(); //Check everything bought now.
						Game1.playSound("purchase");
					}
					else
					{
						Game1.playSound("cancel");
					}
				}
			}

			if (clothingToggles["pants"].active && clothingToggles["pants"].clickable.containsPoint(x, y))
			{
				if (clothingToggles["pants"].showing)
				{
					clothingToggles["pants"].storeItem = who.pantsItem.Value as Item;
					who.pantsItem.Set(null);
					Game1.playSound("scissors");
				}
				else
				{
					who.pantsItem.Set(clothingToggles["pants"].storeItem as Clothing);
					Game1.playSound("pickUpItem");
				}
				clothingToggles["pants"].showing = !clothingToggles["pants"].showing;

			}

			if (clothingToggles["shoes"].active && clothingToggles["shoes"].clickable.containsPoint(x, y))
			{
				if (clothingToggles["shoes"].showing)
				{
					clothingToggles["shoes"].storeItem = who.boots.Get() as Item;
					who.boots.Set(null);

					Game1.playSound("scissors");
				}
				else
				{
					who.boots.Set(clothingToggles["shoes"].storeItem as Boots);
					Game1.playSound("pickUpItem");
				}
				clothingToggles["shoes"].showing = !clothingToggles["shoes"].showing;

			}

			if (clothingToggles["shirt"].active && clothingToggles["shirt"].clickable.containsPoint(x, y))
			{
				if (clothingToggles["shirt"].showing)
				{
					clothingToggles["shirt"].storeItem = who.shirtItem.Get() as Item;
					who.shirtItem.Set(null);
					Game1.playSound("scissors");
				}
				else
				{
					who.shirtItem.Set(clothingToggles["shirt"].storeItem as Clothing);
					Game1.playSound("pickUpItem");
				}
				clothingToggles["shirt"].showing = !clothingToggles["shirt"].showing;

			}

			if (clothingToggles["hat"].active && clothingToggles["hat"].clickable.containsPoint(x, y))
			{
				if (clothingToggles["hat"].showing)
				{
					clothingToggles["hat"].storeItem = who.hat.Get() as Item;
					who.hat.Set(null);
					Game1.playSound("scissors");
				}
				else
				{
					who.hat.Set(clothingToggles["hat"].storeItem as Hat);
					Game1.playSound("pickUpItem");
				}
				clothingToggles["hat"].showing = !clothingToggles["hat"].showing;

			}

			//string sound = "drumkit6";
			//Game1.playSound(sound);
		}

		//Handle the mouse down-held event on the colour picker
		public override void leftClickHeld(int x, int y)
		{
			this.colorPickerTimer -= Game1.currentGameTime.ElapsedGameTime.Milliseconds;
			if (this.colorPickerTimer > 0)
			{
				return;
			}
			if (this.lastHeldColorPicker != null && !Game1.options.SnappyMenus)
			{
				if (this.lastHeldColorPicker.Equals(this.primaryColorPicker))
				{
					Color color2 = this.primaryColorPicker.clickHeld(x, y);
					who.modData["DB.trinket" + currentTrinket + "_c1"] = color2.PackedValue.ToString();
					pbe.trinkets[currentTrinket].texture = null;
					pbe.dirtyLayers["trinkets"] = true;
					pbe.UpdateTextures(who);
				}

				if (this.lastHeldColorPicker.Equals(this.secondaryColorPicker))
				{
					Color color2 = this.secondaryColorPicker.clickHeld(x, y);
					who.modData["DB.trinket" + currentTrinket + "_c2"] = color2.PackedValue.ToString();
					pbe.trinkets[currentTrinket].texture = null;
					pbe.dirtyLayers["trinkets"] = true;
					pbe.UpdateTextures(who);
				}
			}
			this.colorPickerTimer = 100;
		}

		//Handle mouse up event on the colour picker
		public override void releaseLeftClick(int x, int y)
		{
			if (this.primaryColorPicker != null)
			{
				this.primaryColorPicker.releaseClick();
			}

			if (this.secondaryColorPicker != null)
			{
				this.secondaryColorPicker.releaseClick();
			}

			this.lastHeldColorPicker = null;
		}

		//Provide tooltip information
		public override void performHoverAction(int x, int y)
		{
			this.hoverText = "";
			this.hoverTitle = "";

			foreach (ClickableTextureComponent c6 in this.leftSelectionButtons)
			{
				if (c6.containsPoint(x, y))
				{
					c6.scale = Math.Min(c6.scale + 0.02f, c6.baseScale + 0.1f);
				}
				else
				{
					c6.scale = Math.Max(c6.scale - 0.02f, c6.baseScale);
				}
			}

			foreach (ClickableTextureComponent c5 in this.rightSelectionButtons)
			{
				if (c5.containsPoint(x, y))
				{
					c5.scale = Math.Min(c5.scale + 0.02f, c5.baseScale + 0.1f);
				}
				else
				{
					c5.scale = Math.Max(c5.scale - 0.02f, c5.baseScale);
				}
			}

			if (this.okButton.containsPoint(x, y))// && this.canPay())
			{
				if (everythingOwned)
				{
					this.okButton.scale = Math.Min(this.okButton.scale + 0.02f, this.okButton.baseScale + 0.1f);
					hoverText = "";
				} else
                {
					hoverText = T("own_all");
                }
			}
			else
			{
				this.okButton.scale = Math.Max(this.okButton.scale - 0.02f, this.okButton.baseScale);
			}

			if (this.backButton.containsPoint(x, y))
			{
				this.backButton.scale = Math.Min(this.backButton.scale + 0.02f, this.backButton.baseScale + 0.1f);
			}
			else
			{
				this.backButton.scale = Math.Max(this.backButton.scale - 0.02f, this.backButton.baseScale);
			}

			if ((this.primaryColorPicker != null && this.primaryColorPicker.containsPoint(x, y))
				|| (this.secondaryColorPicker != null && this.secondaryColorPicker.containsPoint(x, y)))
			{
				Game1.SetFreeCursorDrag();
			}

			foreach (KeyValuePair<string, ClothingToggle> ctkv in clothingToggles)
			{
				ctkv.Value.hover = ctkv.Value.clickable.containsPoint(x, y) && ctkv.Value.active;
				if (ctkv.Value.hover)
				{
					hoverText = T("toggle_show") + " " + T(ctkv.Key);
				}
			}
		}

		//Draw the menu and its various buttons
		public override void draw(SpriteBatch b)
		{
			bool ignoreTitleSafe = true;
			Game1.drawDialogueBox(base.xPositionOnScreen, base.yPositionOnScreen, base.width, base.height, speaker: false, drawOnlyBox: true, null, objectDialogueWithPortrait: false, ignoreTitleSafe);
			//b.Draw(Game1.daybg, new Vector2(this.portraitBox.X, this.portraitBox.Y), Color.White);
			
			//Draw Haley back
			b.Draw(UItexture, new Vector2(this.portraitBox.X, this.portraitBox.Y), CharacterBackgroundRect, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.8f);
			//Draw frame over
			b.Draw(UItexture, new Vector2(this.portraitBox.X, this.portraitBox.Y), new Rectangle(0, 32, 32, 48), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.8f);


			nameBox.Draw(b);

			foreach (KeyValuePair<string, ClothingToggle> ctkv in clothingToggles)
			{
				Color tint = Color.White;
				if (!ctkv.Value.active)
				{
					tint.A = 128;
				}
				b.Draw(UItexture, new Vector2(ctkv.Value.clickable.bounds.X, ctkv.Value.clickable.bounds.Y), new Rectangle(0, ctkv.Value.hover ? 16 : 0, 16, 16), Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0.8f);
				b.Draw(UItexture, new Vector2(ctkv.Value.clickable.bounds.X, ctkv.Value.clickable.bounds.Y), new Rectangle(ctkv.Value.offset, ctkv.Value.showing ? 0 : 16, 16, 16), tint, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0.8f);
			}


			if (currentOwned)
			{
				//Draw purchased background border
				IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 396, 15, 15), costLabel.bounds.X - costLabel.bounds.Width / 2, costLabel.bounds.Y, costLabel.bounds.Width, costLabel.bounds.Height, disabledColor, 4f, drawShadow: false);
			}
			else
			{
				//Draw purchase background border
				IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 396, 15, 15), costLabel.bounds.X - costLabel.bounds.Width / 2, costLabel.bounds.Y, costLabel.bounds.Width, costLabel.bounds.Height, (costLabel.containsPoint(Game1.getOldMouseX() + costLabel.bounds.Width / 2, Game1.getOldMouseY())) ? purchase_selected_color : Color.White, 4f, drawShadow: false);

				//Draw the coin icon
				b.Draw(Game1.mouseCursors, new Vector2(costLabel.bounds.X + Game1.smallFont.MeasureString(costLabel.name).X / 2f+4f, costLabel.bounds.Y + 24), new Rectangle(193, 373, 9, 9), Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0.8f);
			}

			foreach (ClickableTextureComponent leftSelectionButton in this.leftSelectionButtons)
			{
				leftSelectionButton.draw(b);
			}

			foreach (ClickableComponent c3 in this.labels)
			{
				if (!c3.visible)
				{
					continue;
				}
				string sub = "";//Line below the label
				string tiny_sub = "";//Smaller line below label
				float offset = 0f;
				float offsety = 0f;
				float subYOffset = 0f;
				Color color = Game1.textColor;

				//Colour and size the strings
				if (c3 == this.accLabel)
				{
					offset = 21f - Game1.smallFont.MeasureString(c3.name).X / 2f;
					sub = ((int)who.accessory.Value + 2).ToString() ?? "";
					if (who.accessory.Value == -1)
					{
						sub = T("none");
					}
				}
				else if(c3 == this.layerLabel)
				{
					c3.name = T("trinket") + " " + (currentTrinket + 1);
					offset = 21f - Game1.smallFont.MeasureString(c3.name).X / 2f;
					offsety = 8f;
				}
				if (c3 == this.trinketLabel)
				{
					offset = 21f - Game1.smallFont.MeasureString(c3.name).X / 2f;
					tiny_sub = (who.modData.ContainsKey("DB.trinket"+currentTrinket)) ? who.modData["DB.trinket" + currentTrinket] : T("none");
					if(tiny_sub == "Default") tiny_sub = T("none");
				}
				else if (c3 == this.costLabel)
				{
					offset = -Game1.smallFont.MeasureString(c3.name).X/2f;
					offsety = 16f;
					color = new Color(86, 22, 12);
				}
				else
				{
					color = Game1.textColor;
				}

				if(!primaryColorActive && c3 == this.primaryLabel)
                {
					color = disabledColor;
				}

				if (!secondaryColorActive && c3 == this.secondaryLabel)
				{
					color = disabledColor;
				}

				if (tiny_sub == "Default") tiny_sub = T("default");

				Utility.drawTextWithShadow(b, c3.name, Game1.smallFont, new Vector2((float)c3.bounds.X + offset, c3.bounds.Y + offsety), color);
				if (sub.Length > 0)
				{
					Utility.drawTextWithShadow(b, sub, Game1.smallFont, new Vector2((float)(c3.bounds.X + 21) - Game1.smallFont.MeasureString(sub).X / 2f, (float)(c3.bounds.Y + 32) + subYOffset), color);
				}
				if (tiny_sub.Length > 0)
				{
					Utility.drawTextWithShadow(b, tiny_sub, Game1.dialogueFont, new Vector2((float)(c3.bounds.X + 21) - (Game1.dialogueFont.MeasureString(tiny_sub).X / 2f) / 2f, (float)(c3.bounds.Y + 32) + subYOffset), color, 0.5f);
				}
			}
			foreach (ClickableTextureComponent rightSelectionButton in this.rightSelectionButtons)
			{
				rightSelectionButton.draw(b);
			}

			if (everythingOwned)
			{
				this.okButton.draw(b, Color.White, 0.75f);
			}
			else
			{
				this.okButton.draw(b, disabledColor, 0.75f);
			}


			this.backButton.draw(b, Color.White, 0.75f);

			if (this.primaryColorPicker != null)
			{
				if (primaryColorActive)
				{
					this.primaryColorPicker.draw(b);
				} else
                {
					DrawInactive(primaryColorPicker, b, colorPickerCCs[0].bounds);
                }
			}

			if (this.secondaryColorPicker != null)
			{
				if (secondaryColorActive)
				{
					this.secondaryColorPicker.draw(b);
				}
				else
				{
					DrawInactive(secondaryColorPicker, b, colorPickerCCs[3].bounds);
				}
			}

			b.End();
			b.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp);
			this._displayFarmer.FarmerRenderer.draw(b, this._displayFarmer.FarmerSprite.CurrentAnimationFrame, this._displayFarmer.FarmerSprite.CurrentFrame, this._displayFarmer.FarmerSprite.SourceRect, new Vector2(this.portraitBox.Center.X - 32, this.portraitBox.Bottom - 160), Vector2.Zero, 0.8f, Color.White, 0f, 1f, this._displayFarmer);
			b.End();
			b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
			if (this.hoverText != null && this.hoverText.Count() > 0)
			{
				IClickableMenu.drawHoverText(b, Game1.parseText(this.hoverText, Game1.smallFont, 256), Game1.smallFont, 0, 0, -1, this.hoverTitle);
			}
			base.drawMouse(b);
		}

		public static void DrawInactive(ColorPicker cp, SpriteBatch b, Rectangle bounds)
		{
			b.Draw(Game1.staminaRect, new Rectangle(bounds.X, bounds.Y + cp.hueBar.bounds.Center.Y - 2, cp.hueBar.bounds.Width, 4), disabledColor);
			b.Draw(Game1.mouseCursors, new Vector2(bounds.X + (int)((float)cp.hueBar.value / 100f * (float)cp.hueBar.bounds.Width), bounds.Y + cp.hueBar.bounds.Center.Y), new Rectangle(64, 256, 32, 32), disabledColor, 0f, new Vector2(16f, 9f), 1f, SpriteEffects.None, 0.86f);
			
			b.Draw(Game1.staminaRect, new Rectangle(bounds.X, bounds.Y + cp.saturationBar.bounds.Center.Y - 2, cp.saturationBar.bounds.Width, 4), disabledColor);
			b.Draw(Game1.mouseCursors, new Vector2(bounds.X + (int)((float)cp.saturationBar.value / 100f * (float)cp.saturationBar.bounds.Width), bounds.Y + cp.saturationBar.bounds.Center.Y), new Rectangle(64, 256, 32, 32), disabledColor, 0f, new Vector2(16f, 9f), 1f, SpriteEffects.None, 0.87f);
			
			b.Draw(Game1.staminaRect, new Rectangle(bounds.X, bounds.Y + cp.valueBar.bounds.Center.Y - 2, cp.valueBar.bounds.Width, 4), disabledColor);
			b.Draw(Game1.mouseCursors, new Vector2(bounds.X + (int)((float)cp.valueBar.value / 100f * (float)cp.valueBar.bounds.Width), bounds.Y + cp.valueBar.bounds.Center.Y), new Rectangle(64, 256, 32, 32), disabledColor, 0f, new Vector2(16f, 9f), 1f, SpriteEffects.None, 0.86f);
			
		}
	}
}
