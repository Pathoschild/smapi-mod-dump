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
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.GameData.Crafting;
using StardewValley.Objects;
using StardewModdingAPI;
using StardewValley.Menus;
using StardewValley;
using DynamicBodies.Framework;

namespace DynamicBodies.UI
{
    internal class ClothingModifier : MenuWithInventory
	{
		protected enum CraftState
		{
			MissingIngredients,
			Valid,
			ValidStainOnly
		}

		public const int region_tabmake = 50, region_tabdye = 52, region_tabedit = 51;
		public const int region_shirtSetting = 990;
		public const int region_bootsIngredient = 998, region_stainIngredient = 997, region_startButton = 996, region_resultItem = 995, region_stainBottle = 994;

		public ClickableComponent makeTab;
		public ClickableComponent dyeTab;
		public ClickableComponent editTab;

		private Point sleeve_label;
		public ClickableTextureComponent shirtSpot;
		public ClickableTextureComponent blankShirtSpot;
		public List<ClickableComponent> sleeveIcons = new List<ClickableComponent>();

		private Point extra_settings_label;
		private Checkbox useBathers;
		public ClickableTextureComponent useBathersCTC;
		private Checkbox useOverallColor;
		public ClickableTextureComponent useOverallColorCTC;

		private Point stain_label;
		public ClickableTextureComponent bootsSpot;
		public ClickableTextureComponent blankBootsSpot;

		public ClickableTextureComponent stainIngredientSpot;
		public ClickableTextureComponent blankStainIngredientSpot;

		public ClickableTextureComponent stainBottle;
		private Rectangle stainBottleRainbowBack;
		private string prismatic_stain = "";

		public ClickableTextureComponent craftResultDisplay;

		public const int region_shoes = 109;

		public List<ClickableComponent> equipmentIcons = new List<ClickableComponent>();

		public Texture2D tailoringTextures;
		public Texture2D moreTailoringTextures;

		protected Dictionary<Item, bool> _highlightDictionary;

		protected bool _isDyeCraft;

		protected string displayedDescription = "";

		protected CraftState _craftState;

		public Color bgColor;
		public Color darkText;

		private Texture2D stainBottleOverlay;
		private Color[] stainData;
		private Dictionary<int, Dictionary<int, Color>> stainColors = new Dictionary<int, Dictionary<int, Color>>();

		public ClothingModifier()
			: base(null, okButton: true, trashCan: true, 12, 132)
		{
			
			if (base.yPositionOnScreen == IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder)
			{
				base.movePosition(0, -IClickableMenu.spaceToClearTopBorder);
			}
			base.inventory.highlightMethod = HighlightItems;
			tailoringTextures = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\tailoring");
			moreTailoringTextures = Game1.temporaryContent.Load<Texture2D>("Mods/ribeena.dynamicbodies/assets/Interface/more_tailoring.png");

			makeTab = new ClickableComponent(new Rectangle(base.xPositionOnScreen + 64, base.yPositionOnScreen + IClickableMenu.tabYPositionRelativeToMenuY + 64, 64, 64), "Make", "Sewing Machine")
			{
				myID = region_tabmake,
				downNeighborID = -99998,
				leftNeighborID = -99998,
				rightNeighborID = -99998,
				tryDefaultIfNoDownNeighborExists = true,
				fullyImmutable = true
			};
			dyeTab = new ClickableComponent(new Rectangle(base.xPositionOnScreen + 128, base.yPositionOnScreen + IClickableMenu.tabYPositionRelativeToMenuY + 64, 64, 64), "Make", "Sewing Machine")
			{
				myID = region_tabdye,
				downNeighborID = -99998,
				leftNeighborID = -99998,
				rightNeighborID = -99998,
				tryDefaultIfNoDownNeighborExists = true,
				fullyImmutable = true
			};
			editTab = new ClickableComponent(new Rectangle(base.xPositionOnScreen + 192, base.yPositionOnScreen + IClickableMenu.tabYPositionRelativeToMenuY + 64, 64, 64), "Edit", "Adjusting Tools")
			{
				myID = region_tabedit,
				downNeighborID = -99998,
				leftNeighborID = -99998,
				rightNeighborID = -99998,
				tryDefaultIfNoDownNeighborExists = true,
				fullyImmutable = true
			};

			//Update the stain colors incase it's changed by JSONAssets
			ShoesPalette.LoadPalettes();

			stainBottleOverlay = new Texture2D(Game1.graphics.GraphicsDevice, 24, 24);
			stainData = new Color[24 * 24];
			moreTailoringTextures.GetData(0, new Rectangle(24, 64, 24, 24), stainData, 0, stainData.Length);
			stainBottleOverlay.SetData(stainData);
			//Build dye color structure for quick dying
			for(int i = 0; i < stainData.Length; i++)
            {
				if (stainData[i].A > 10)
				{
					if (stainData[i].R == 0)
					{
						if (!stainColors.ContainsKey(0))
						{
							stainColors[0] = new Dictionary<int, Color>();
						}
						stainColors[0].Add(i, stainData[i]);
					}
					if (stainData[i].R == 77)
					{
						if (!stainColors.ContainsKey(1))
						{
							stainColors[1] = new Dictionary<int, Color>();
						}
						stainColors[1].Add(i, stainData[i]);
					}
					if (stainData[i].R == 166)
					{
						if (!stainColors.ContainsKey(2))
						{
							stainColors[2] = new Dictionary<int, Color>();
						}
						stainColors[2].Add(i, stainData[i]);
					}
					if (stainData[i].R == 255)
					{
						if (!stainColors.ContainsKey(3))
						{
							stainColors[3] = new Dictionary<int, Color>();
						}
						stainColors[3].Add(i, stainData[i]);
					}
				}
			}

			Color[] data = new Color[moreTailoringTextures.Width * moreTailoringTextures.Height];
			moreTailoringTextures.GetData(data, 0, data.Length);
			//get the colour from the tab image rather than hardcode
			bgColor = data[moreTailoringTextures.Width * 2 + - 6];
			darkText = data[2];


			this._CreateButtons();
			if (base.trashCan != null)
			{
				base.trashCan.myID = 106;
			}
			if (base.okButton != null)
			{
				base.okButton.leftNeighborID = 11;
			}
			if (Game1.options.SnappyMenus)
			{
				base.populateClickableComponentList();
				this.snapToDefaultClickableComponent();
			}
			this._ValidateCraft();
		}

		//Make translation easier
		static public string T(string toTranslate)
		{
			return ModEntry.translate(toTranslate);
		}


		protected void _CreateButtons()
		{
			this.equipmentIcons = new List<ClickableComponent>();
			this.equipmentIcons.Add(new ClickableComponent(new Rectangle(0, 0, 64, 64), "Shirt")
			{
				myID = 100,
				leftNeighborID = -99998,
				downNeighborID = -99998,
				upNeighborID = -99998,
				rightNeighborID = -99998
			}); 
			this.equipmentIcons.Add(new ClickableComponent(new Rectangle(0, 0, 64, 64), "Shoes")
			{
				myID = 101,
				leftNeighborID = -99998,
				downNeighborID = -99998,
				upNeighborID = -99998,
				rightNeighborID = -99998
			});
			for (int i = 0; i < this.equipmentIcons.Count; i++)
			{
				this.equipmentIcons[i].bounds.X = base.xPositionOnScreen - 64 + 9;
				this.equipmentIcons[i].bounds.Y = base.yPositionOnScreen + 192 + i * 64;
			}


			sleeve_label = new Point(base.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth / 2 + 4+4, base.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 4);

			shirtSpot = new ClickableTextureComponent(new Rectangle(sleeve_label.X, sleeve_label.Y+36, 96, 96), this.moreTailoringTextures, new Rectangle(0, 64, 24, 24), 4f)
			{
				myID = region_shirtSetting,
				downNeighborID = region_resultItem,
				leftNeighborID = -99998,
				rightNeighborID = -99998,
				upNeighborID = region_tabmake,
				item = ((this.shirtSpot != null) ? this.shirtSpot.item : null)
			};
			this.blankShirtSpot = new ClickableTextureComponent(new Rectangle(shirtSpot.bounds.X, shirtSpot.bounds.Y, 96, 96), this.moreTailoringTextures, new Rectangle(24, 40, 24, 24), 4f);
			this.sleeveIcons = new List<ClickableComponent>();
			this.sleeveIcons.Add(new ClickableComponent(new Rectangle(0, 0, 64, 64), "long")
			{ myID = region_shirtSetting + 1, leftNeighborID = -99998, downNeighborID = -99998, upNeighborID = -99998, rightNeighborID = -99998 });
			this.sleeveIcons.Add(new ClickableComponent(new Rectangle(0, 0, 64, 64), "default")
			{ myID = region_shirtSetting + 2, leftNeighborID = -99998, downNeighborID = -99998, upNeighborID = -99998, rightNeighborID = -99998 });
			this.sleeveIcons.Add(new ClickableComponent(new Rectangle(0, 0, 64, 64), "short")
			{ myID = region_shirtSetting + 3, leftNeighborID = -99998, downNeighborID = -99998, upNeighborID = -99998, rightNeighborID = -99998 });
			this.sleeveIcons.Add(new ClickableComponent(new Rectangle(0, 0, 64, 64), "sleeveless")
			{ myID = region_shirtSetting + 4, leftNeighborID = -99998, downNeighborID = -99998, upNeighborID = -99998, rightNeighborID = -99998 });
			for (int i = 0; i < this.sleeveIcons.Count; i++)
			{
				this.sleeveIcons[i].bounds.X = shirtSpot.bounds.X+108 +i*68;
				this.sleeveIcons[i].bounds.Y = shirtSpot.bounds.Y+16;
			}
			
			//Extra settings
			extra_settings_label = new Point(sleeve_label.X, shirtSpot.bounds.Y+shirtSpot.bounds.Height+24);
			useBathers = new Checkbox("bathers", extra_settings_label.X, extra_settings_label.Y+36, T("bathers_setting"))
			{
				myID = region_shirtSetting+6,
				downNeighborID = -99998,
				leftNeighborID = -99998,
				upNeighborID = -99998,
				isChecked = (Game1.player.modData.ContainsKey("DB.bathers") ? Game1.player.modData["DB.bathers"]=="true" : false)
			};
			useBathersCTC = useBathers as ClickableTextureComponent;
			useOverallColor = new Checkbox("overalls", extra_settings_label.X, useBathers.bounds.Y + 48, T("pants_overlay_setting"))
			{
				myID = region_shirtSetting + 7,
				downNeighborID = -99998,
				leftNeighborID = -99998,
				upNeighborID = -99998,
				isChecked = (Game1.player.modData.ContainsKey("DB.overallColor") ? Game1.player.modData["DB.overallColor"] == "true" : false)
			};
			useOverallColorCTC = useOverallColor as ClickableTextureComponent;

			// Staining menu

			stain_label = new Point(base.xPositionOnScreen + base.width - (IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth / 2 + 4 + 96*2 + 8), sleeve_label.Y);
			
			this.stainIngredientSpot = new ClickableTextureComponent(new Rectangle(stain_label.X, stain_label.Y+36, 96, 96), this.moreTailoringTextures, new Rectangle(0, 16, 24, 24), 4f)
			{
				myID = region_stainIngredient,
				downNeighborID = region_bootsIngredient,
				leftNeighborID = -99998,
				rightNeighborID = -99998,
				upNeighborID = region_tabedit,
				item = ((this.stainIngredientSpot != null) ? this.stainIngredientSpot.item : null),
				fullyImmutable = true
			};
			this.blankStainIngredientSpot = new ClickableTextureComponent(new Rectangle(stainIngredientSpot.bounds.X, stainIngredientSpot.bounds.Y, 96, 96), this.moreTailoringTextures, new Rectangle(24, 40, 24, 24), 4f);

			this.stainBottle = new ClickableTextureComponent(new Rectangle(stainIngredientSpot.bounds.X+stainIngredientSpot.bounds.Width + 4, stainIngredientSpot.bounds.Y, 96, 96), this.moreTailoringTextures, new Rectangle(24, 16, 24, 24), 4f)
			{
				myID = region_stainBottle,
				downNeighborID = region_bootsIngredient,
				leftNeighborID = -99998,
				rightNeighborID = -99998,
				upNeighborID = region_tabedit,
				fullyImmutable = true
			};
			stainBottleRainbowBack = new Rectangle(0, 88, 24, 24);


			this.bootsSpot = new ClickableTextureComponent(new Rectangle(stainIngredientSpot.bounds.X+96/2+4, stainIngredientSpot.bounds.Y+96+8, 96, 96), this.moreTailoringTextures, new Rectangle(0, 40, 24, 24), 4f)
			{
				myID = region_bootsIngredient,
				downNeighborID = region_resultItem,
				leftNeighborID = -99998,
				rightNeighborID = -99998,
				upNeighborID = region_stainIngredient,
				item = ((this.bootsSpot != null) ? this.bootsSpot.item : null)
			};
			this.blankBootsSpot = new ClickableTextureComponent(new Rectangle(bootsSpot.bounds.X, bootsSpot.bounds.Y, 96, 96), this.moreTailoringTextures, new Rectangle(24, 40, 24, 24), 4f);


			this.craftResultDisplay = new ClickableTextureComponent(new Rectangle(bootsSpot.bounds.X + 16, bootsSpot.bounds.Y+96+8, 64, 64), this.moreTailoringTextures, new Rectangle(0, 0, 16, 16), 4f)
			{
				myID = region_resultItem,
				downNeighborID = -99998,
				leftNeighborID = -99998,
				upNeighborID = region_stainIngredient,
				item = ((this.craftResultDisplay != null) ? this.craftResultDisplay.item : null)
			};

			if (base.inventory.inventory != null && base.inventory.inventory.Count >= 12)
			{
				for (int j = 0; j < 12; j++)
				{
					if (base.inventory.inventory[j] != null)
					{
						base.inventory.inventory[j].upNeighborID = -99998;
					}
				}

				//Fix joypad navigation
				craftResultDisplay.downNeighborID = base.inventory.inventory[10].myID;
			}
		}

		public override void snapToDefaultClickableComponent()
		{
			base.currentlySnappedComponent = base.getComponentWithID(0);
			this.snapCursorToCurrentSnappedComponent();
		}

		public override bool readyToClose()
		{
			if (base.readyToClose() && base.heldItem == null)
			{
				return true;
			}
			return false;
		}

		public bool HighlightItems(Item i)
		{
			if (i == null)
			{
				return false;
			}
			if (i != null && !this.IsValidCraftIngredient(i))
			{
				return false;
			}
			if (this._highlightDictionary == null)
			{
				this.GenerateHighlightDictionary();
			}
			if (!this._highlightDictionary.ContainsKey(i))
			{
				this._highlightDictionary = null;
				this.GenerateHighlightDictionary();
			}
			return this._highlightDictionary[i];
		}

		public void GenerateHighlightDictionary()
		{
			this._highlightDictionary = new Dictionary<Item, bool>();
			List<Item> item_list = new List<Item>(base.inventory.actualInventory);
			if (Game1.player.boots.Value != null)
			{
				item_list.Add(Game1.player.boots.Value);
			}
			if (Game1.player.shirtItem.Value != null)
			{
				item_list.Add(Game1.player.shirtItem.Value);
			}
			foreach (Item item in item_list)
			{
				if (item != null)
				{
					if (this.shirtSpot.item != null){
						_highlightDictionary[item] = IsShirt(item);
					}
					else if (this.bootsSpot.item == null && this.stainIngredientSpot.item == null)
					{
						this._highlightDictionary[item] = true;
					}
					else if (this.bootsSpot.item != null && this.stainIngredientSpot.item != null)
					{
						this._highlightDictionary[item] = false;
					}
					else if (this.bootsSpot.item != null)
					{
						this._highlightDictionary[item] = this.IsValidCraft(this.bootsSpot.item, item);
					}
					else
					{
						this._highlightDictionary[item] = this.IsValidCraft(item, this.stainIngredientSpot.item);
					}
				}
			}
		}

		private void _bootsSpotClicked()
		{
			Item old_item = this.bootsSpot.item;
            _DGABootFix(base.heldItem);
            if (base.heldItem == null || this.IsValidCraftIngredient(base.heldItem))
			{
				Game1.playSound("stoneStep");
				this.bootsSpot.item = base.heldItem;
				base.heldItem = old_item;
				this._highlightDictionary = null;
				this._ValidateCraft();
				ModEntry.MakePlayerDirty();
			}
		}

		private void _shirtSpotClicked()
		{
			Item old_item = this.shirtSpot.item;
			if (base.heldItem == null || IsShirt(base.heldItem))
			{
				Game1.playSound("stoneStep");
				this.shirtSpot.item = base.heldItem;
				base.heldItem = old_item;
				this._highlightDictionary = null;
				ModEntry.MakePlayerDirty();
			}
		}

		private void _DGABootFix(Item item)
		{
            if (item is Boots)  ModEntry.debugmsg("boots palette is "+ (item as Boots).indexInColorSheet, LogLevel.Debug);

            if (ModEntry.dga == null) return;
            if (item == null) return;

			if (item is Boots)
			{
				if (ModEntry.dga.GetDGAItemId(item) == null)
				{
                    //Not a DGA boot
                    item.modData["DGA.FarmerColors"] = (item as Boots).indexInColorSheet+"";
                    ModEntry.debugmsg("Vanilla-ish boots palette stored on item", LogLevel.Debug);
                }
				else if (!item.modData.ContainsKey("DGA.FarmerColors"))
				{
					item.modData["DGA.FarmerColors"] = ModEntry.dga.GetDGAItemId(item);
					ModEntry.debugmsg("DGA boots palette stored on item", LogLevel.Debug);
				}
			}
        }

		public bool IsValidCraftIngredient(Item item)
		{
			if (item.HasContextTag("item_lucky_purple_shorts"))
			{
				return true;
			}
			if (!item.canBeTrashed())
			{
				return false;
			}
			return true;
		}

		public bool IsShirt(Item item)
        {
			if(item is Clothing)
            {
				return ((Clothing)item).clothesType.Value == 0;//1 for pants

			} else
            {
				return false;
            }
        }

		private void _stainIngredientSpotClicked()
		{
			Item old_item = this.stainIngredientSpot.item;
            _DGABootFix(base.heldItem);
            if (base.heldItem == null || this.IsValidCraftIngredient(base.heldItem))
			{
				if (base.heldItem != null && base.heldItem.HasContextTag("color_prismatic"))
				{
					if (bootsSpot.item == null)
					{
						prismatic_stain = ShoesPalette.First();
					} else
                    {
						if (bootsSpot.item.modData.ContainsKey("DGA.FarmerColors")) {
							prismatic_stain = bootsSpot.item.modData["DGA.FarmerColors"];
                        }
						else
						{
							prismatic_stain = (bootsSpot.item as Boots).indexInColorSheet + "";
						}
                    }
				}
				else
				{
					prismatic_stain = "";
				}

				Game1.playSound("stoneStep");
				this.stainIngredientSpot.item = base.heldItem;
				base.heldItem = old_item;
				this._highlightDictionary = null;
				this._ValidateCraft();
                
			}
		}

		private void _applyStainBottleColor(int stain_id)
        {
			Texture2D shoesTex = Game1.content.Load<Texture2D>("Characters\\Farmer\\shoeColors");
			Color[] data = new Color[shoesTex.Width * shoesTex.Height];
			shoesTex.GetData(data, 0, data.Length);

			ModEntry.debugmsg($"Stain id is {stain_id}", LogLevel.Debug);

			foreach (KeyValuePair<int, Dictionary<int, Color>> kvp in stainColors)
			{
				Color change = data[stain_id * 4 + kvp.Key];
				foreach(KeyValuePair<int, Color> stainPixel in kvp.Value)
                {
					stainData[stainPixel.Key] = change;
				}
			}
			stainBottleOverlay.SetData(stainData);
		}

        private void _applyStainBottleColor(string stain_id)
        {
            ModEntry.debugmsg($"Stain id is {stain_id}", LogLevel.Debug);

            foreach (KeyValuePair<int, Dictionary<int, Color>> kvp in stainColors)
            {
                Color change = ShoesPalette.GetColors(stain_id)[kvp.Key];
                foreach (KeyValuePair<int, Color> stainPixel in kvp.Value)
                {
                    stainData[stainPixel.Key] = change;
                }
            }
            stainBottleOverlay.SetData(stainData);
        }

        public override void receiveKeyPress(Keys key)
		{
			if (key == Keys.Delete)
			{
				if (base.heldItem != null && this.IsValidCraftIngredient(base.heldItem))
				{
					Utility.trashItem(base.heldItem);
					base.heldItem = null;
				}
			}
			else
			{
				base.receiveKeyPress(key);
			}
		}

		public bool IsHoldingEquippedItem()
		{
			if (base.heldItem == null)
			{
				return false;
			}
			if (!Game1.player.IsEquippedItem(base.heldItem))
			{
				return Game1.player.IsEquippedItem(Utility.PerformSpecialItemGrabReplacement(base.heldItem));
			}
			return true;
		}

		public override void receiveLeftClick(int x, int y, bool playSound = true)
		{
			//Add tab action
			if (makeTab.containsPoint(x, y))
			{
				//return items etc
				cleanupBeforeExit();

				Game1.playSound("smallSelect");
				Game1.activeClickableMenu = new TabbedTailoringMenu();
				return;
			}

			//Add tab action
			if (dyeTab.containsPoint(x, y))
			{
				//return items etc
				cleanupBeforeExit();

				Game1.playSound("smallSelect");
				Game1.activeClickableMenu = new TabbedDyeMenu();
				return;
			}

			Item oldHeldItem = base.heldItem;
			bool num = Game1.player.IsEquippedItem(oldHeldItem);
			base.receiveLeftClick(x, y);
			if (num && base.heldItem != oldHeldItem)
			{
				if (oldHeldItem == Game1.player.boots.Value)
				{
					Game1.player.boots.Value = null;
					this._highlightDictionary = null;
				}
				if (num && base.heldItem != oldHeldItem)
				{
					/*if (oldHeldItem == Game1.player.hat.Value)
					{
						Game1.player.hat.Value = null;
						this._highlightDictionary = null;
					}
					else*/ if (oldHeldItem == Game1.player.shirtItem.Value)
					{
						Game1.player.shirtItem.Value = null;
						this._highlightDictionary = null;
					}
					/*else if (oldHeldItem == Game1.player.pantsItem.Value)
					{
						Game1.player.pantsItem.Value = null;
						this._highlightDictionary = null;
					}*/
				}
			}
			foreach (ClickableComponent c in this.equipmentIcons)
			{
				if (!c.containsPoint(x, y))
				{
					continue;
				}
				switch (c.name)
				{
					case "Shirt":
						{
							Item item_to_place2 = Utility.PerformSpecialItemPlaceReplacement(base.heldItem);
							if (base.heldItem == null)
							{
								if (this.HighlightItems((Clothing)Game1.player.shirtItem))
								{
									base.heldItem = Utility.PerformSpecialItemGrabReplacement((Clothing)Game1.player.shirtItem);
									Game1.playSound("dwop");
									if (!(base.heldItem is Clothing))
									{
										Game1.player.shirtItem.Set(null);
									}
									this._highlightDictionary = null;
									this._ValidateCraft();
									ModEntry.MakePlayerDirty();
								}
							}
							else if (base.heldItem is Clothing && (base.heldItem as Clothing).clothesType.Value == 0)
							{
								Item old_item2 = (Clothing)Game1.player.shirtItem;
								old_item2 = Utility.PerformSpecialItemGrabReplacement(old_item2);
								if (old_item2 == base.heldItem)
								{
									old_item2 = null;
								}
								Game1.player.shirtItem.Set(item_to_place2 as Clothing);
								base.heldItem = old_item2;
								Game1.playSound("sandyStep");
								this._highlightDictionary = null;
								this._ValidateCraft();
								ModEntry.MakePlayerDirty();
							}
							break;
						}
					case "Shoes":
						{
							Item item_to_place3 = Utility.PerformSpecialItemPlaceReplacement(base.heldItem);
							if (base.heldItem == null)
							{
								if (this.HighlightItems((Item)Game1.player.boots))
								{
									base.heldItem = Utility.PerformSpecialItemGrabReplacement((Item)Game1.player.boots);
									if (!(base.heldItem is Boots))
									{
										Game1.player.boots.Set(null);
										Game1.player.changeShoeColor(12);
									}
									Game1.playSound("dwop");
									this._highlightDictionary = null;
									this._ValidateCraft();
									ModEntry.MakePlayerDirty();
								}
							}
							else if (item_to_place3 is Boots)
							{
								Item old_item3 = Game1.player.boots.Value;
								old_item3 = Utility.PerformSpecialItemGrabReplacement(old_item3);
								if (old_item3 == base.heldItem)
								{
									old_item3 = null;
								}
								Game1.player.boots.Set(item_to_place3 as Boots);
								Game1.player.changeShoeColor((item_to_place3 as Boots).indexInColorSheet.Value);
								base.heldItem = old_item3;
								Game1.playSound("sandyStep");
								this._highlightDictionary = null;
								this._ValidateCraft();
								ModEntry.MakePlayerDirty();
							}
							break;
						}
				}
				return;
			}

			//Change the shirt sleeves
			if (shirtSpot.item != null && !(shirtSpot.item as Clothing).GetOtherData().Contains("Sleeveless"))
			{
				foreach (ClickableComponent c in this.sleeveIcons)
				{
					if (!c.containsPoint(x, y)) continue;
					switch (c.name)
					{
						case "long":
							shirtSpot.item.modData[ModEntry.sleeveSetting] = "Long";
							Game1.playSound("scissors");
							break;
						case "short":
							shirtSpot.item.modData[ModEntry.sleeveSetting] = "Short";
							Game1.playSound("scissors");
							break;
						case "default":
							shirtSpot.item.modData[ModEntry.sleeveSetting] = "Normal";
							Game1.playSound("scissors");
							break;
						case "sleeveless":
							shirtSpot.item.modData[ModEntry.sleeveSetting] = "Sleeveless";
							Game1.playSound("scissors");
							break;
					}
				}
			}

			if (Game1.GetKeyboardState().IsKeyDown(Keys.LeftShift) && oldHeldItem != base.heldItem && base.heldItem != null)
			{
				if (base.heldItem is Boots)
				{
					this._bootsSpotClicked();
				} else if (IsShirt(base.heldItem))
				{
					this._shirtSpotClicked();
				}
				else
				{
					this._stainIngredientSpotClicked();
				}
			}
			if (this.bootsSpot.containsPoint(x, y))
			{
				this._bootsSpotClicked();
				if (Game1.GetKeyboardState().IsKeyDown(Keys.LeftShift) && base.heldItem != null)
				{
					if (Game1.player.IsEquippedItem(base.heldItem))
					{
						base.heldItem = null;
					}
					else
					{
						base.heldItem = base.inventory.tryToAddItem(base.heldItem, "");
					}
				}
			}
			else if (this.stainIngredientSpot.containsPoint(x, y))
			{
				this._stainIngredientSpotClicked();
				if (Game1.GetKeyboardState().IsKeyDown(Keys.LeftShift) && base.heldItem != null)
				{
					if (Game1.player.IsEquippedItem(base.heldItem))
					{
						base.heldItem = null;
					}
					else
					{
						base.heldItem = base.inventory.tryToAddItem(base.heldItem, "");
					}
				}
			}
			else if (this.craftResultDisplay.containsPoint(x, y))
			{
				//Grab the crafted item
				if (base.heldItem == null)
				{
					bool fail = false;
					if (!this.CanFitCraftedItem())
					{
						Game1.playSound("cancel");
						Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
						fail = true;
					}
					if (!fail && this.IsValidCraft(this.bootsSpot.item, this.stainIngredientSpot.item))
					{
						Game1.playSound("bigSelect");
						ColorBoot();
						this._UpdateDescriptionText();
						prismatic_stain = "";
					}
					else
					{
						Game1.playSound("sell");
					}
				}
				else
				{
					Game1.playSound("sell");
				}
			}
			else if (this.stainBottle.containsPoint(x, y))
			{
				//Change the stain
				if (prismatic_stain != "")
				{
					prismatic_stain = ShoesPalette.Next(prismatic_stain);
					_ValidateCraft();
					Game1.playSound("bigSelect");
				}
			}


			if (this.shirtSpot.containsPoint(x, y))
			{
				this._shirtSpotClicked();
				if (Game1.GetKeyboardState().IsKeyDown(Keys.LeftShift) && base.heldItem != null)
				{
					if (Game1.player.IsEquippedItem(base.heldItem))
					{
						base.heldItem = null;
					}
					else
					{
						base.heldItem = base.inventory.tryToAddItem(base.heldItem, "");
					}
				}
			}

			if(useBathers.receiveLeftClick(x, y))
            {
				Game1.player.modData["DB.bathers"] = useBathers.isChecked ? "true" : "false";
            }
			if(useOverallColor.receiveLeftClick(x, y))
            {
				Game1.player.modData["DB.overallColor"] = useOverallColor.isChecked ? "true" : "false";
			}

			if (base.heldItem == null || this.isWithinBounds(x, y) || !base.heldItem.canBeTrashed())
			{
				return;
			}
			if (Game1.player.IsEquippedItem(base.heldItem))
			{
				if (base.heldItem == Game1.player.boots.Value)
				{
					Game1.player.boots.Set(null);
				}
			}
			Game1.playSound("throwDownITem");
			Game1.createItemDebris(base.heldItem, Game1.player.getStandingPosition(), Game1.player.FacingDirection);
			base.heldItem = null;
		}

		private void ColorBoot()
        {
			Item crafted_item = this.CraftItem(this.bootsSpot.item, this.stainIngredientSpot.item);
			
			if (!Utility.canItemBeAddedToThisInventoryList(crafted_item, base.inventory.actualInventory))
			{
				Game1.playSound("cancel");
				Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
				return;
			}
			if (this.bootsSpot.item == crafted_item)
			{
				this.bootsSpot.item = null;
			}
			else
			{
				this.SpendLeftItem();
			}
			//Always destroy the coloring item
			this.SpendRightItem();
			Game1.playSound("coin");
			base.heldItem = crafted_item;

			ModEntry.debugmsg($"Took a stained shoe with index [{(((Boots)crafted_item).indexInColorSheet.Value)}]", LogLevel.Debug);

			this._ValidateCraft();
			ModEntry.MakePlayerDirty();
		}

		protected virtual bool CheckHeldItem(Func<Item, bool> f = null)
		{
			return f?.Invoke(base.heldItem) ?? (base.heldItem != null);
		}

		protected void _ValidateCraft()
		{
			Item boot_item = this.bootsSpot.item;
			Item stain_item = this.stainIngredientSpot.item;
			if (stain_item != null && GetStainIndex(stain_item) != "" && boot_item == null)
			{
				this._craftState = CraftState.ValidStainOnly;
				//Update dye bottle
				if (stain_item.HasContextTag("color_prismatic"))
				{
					_applyStainBottleColor(prismatic_stain);
				}
				else
				{
					if (stain_item is Boots && stain_item.modData.ContainsKey("DGA.FarmerColors"))
					{
						_applyStainBottleColor(stain_item.modData["DGA.FarmerColors"]);
                    }
					else
					{
						_applyStainBottleColor(GetStainIndex(stain_item));
					}
				}
			}
			else if (boot_item == null || (boot_item == null && stain_item == null))
			{
				this._craftState = CraftState.MissingIngredients;
			} 
			else if (this.IsValidCraft(boot_item, stain_item))
			{
				this._craftState = CraftState.Valid;
				Item boot_clone = boot_item.getOne();
				this.craftResultDisplay.item = this.CraftItem(boot_clone, stain_item.getOne());
				if (stain_item.HasContextTag("color_prismatic"))
				{
					_applyStainBottleColor(prismatic_stain);
				}
				else
				{
                    if (boot_clone.modData.ContainsKey("DGA.FarmerColors")) 
                    {
						ModEntry.debugmsg("DGA.FarmerColors is " + boot_clone.modData["DGA.FarmerColors"], LogLevel.Debug);
						_applyStainBottleColor(boot_clone.modData["DGA.FarmerColors"]);
                    }
                    else
                    {
                        //dye bottle matches the base boot
                        _applyStainBottleColor(((Boots)boot_clone).indexInColorSheet);
                    }
				}
				if (this.craftResultDisplay.item == boot_clone)
				{
					this._isDyeCraft = true;
				}
				else
				{
					this._isDyeCraft = false;
				}
			}
			this._UpdateDescriptionText();
		}

		protected void _UpdateDescriptionText()
		{
			if (this._craftState == CraftState.MissingIngredients)
			{
				this.displayedDescription = Game1.content.LoadString("Strings\\UI:Tailor_MissingIngredients");
			}
			else if (this._craftState == CraftState.Valid)
			{
				if (!this.CanFitCraftedItem())
				{
					this.displayedDescription = Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588");
				}
				else
				{
					this.displayedDescription = Game1.content.LoadString("Strings\\UI:Tailor_Valid");
				}
			}
			else
			{
				this.displayedDescription = "";
			}
		}

		public static Color? GetItemColor(Item dye_object)
		{
			if (dye_object != null)
			{
				if (dye_object.HasContextTag("color_prismatic"))
				{
					return Color.White;
				}
				if (dye_object is ColoredObject)
				{
					return (dye_object as ColoredObject).color;
				}
				Dictionary<string, Color> color_dictionary = new Dictionary<string, Color>();
				color_dictionary["black"] = new Color(45, 45, 45);
				color_dictionary["gray"] = Color.Gray;
				color_dictionary["white"] = Color.White;
				color_dictionary["pink"] = new Color(255, 163, 186);
				color_dictionary["red"] = new Color(220, 0, 0);
				color_dictionary["orange"] = new Color(255, 128, 0);
				color_dictionary["yellow"] = new Color(255, 230, 0);
				color_dictionary["green"] = new Color(10, 143, 0);
				color_dictionary["blue"] = new Color(46, 85, 183);
				color_dictionary["purple"] = new Color(115, 41, 181);
				color_dictionary["brown"] = new Color(130, 73, 37);
				color_dictionary["light_cyan"] = new Color(180, 255, 255);
				color_dictionary["cyan"] = Color.Cyan;
				color_dictionary["aquamarine"] = Color.Aquamarine;
				color_dictionary["sea_green"] = Color.SeaGreen;
				color_dictionary["lime"] = Color.Lime;
				color_dictionary["yellow_green"] = Color.GreenYellow;
				color_dictionary["pale_violet_red"] = Color.PaleVioletRed;
				color_dictionary["salmon"] = new Color(255, 85, 95);
				color_dictionary["jade"] = new Color(130, 158, 93);
				color_dictionary["sand"] = Color.NavajoWhite;
				color_dictionary["poppyseed"] = new Color(82, 47, 153);
				color_dictionary["dark_red"] = Color.DarkRed;
				color_dictionary["dark_orange"] = Color.DarkOrange;
				color_dictionary["dark_yellow"] = Color.DarkGoldenrod;
				color_dictionary["dark_green"] = Color.DarkGreen;
				color_dictionary["dark_blue"] = Color.DarkBlue;
				color_dictionary["dark_purple"] = Color.DarkViolet;
				color_dictionary["dark_pink"] = Color.DeepPink;
				color_dictionary["dark_cyan"] = Color.DarkCyan;
				color_dictionary["dark_gray"] = Color.DarkGray;
				color_dictionary["dark_brown"] = Color.SaddleBrown;
				color_dictionary["gold"] = Color.Gold;
				color_dictionary["copper"] = new Color(179, 85, 0);
				color_dictionary["iron"] = new Color(197, 213, 224);
				color_dictionary["iridium"] = new Color(105, 15, 255);
				foreach (string key in color_dictionary.Keys)
				{
					if (dye_object.HasContextTag("color_" + key))
					{
						return color_dictionary[key];
					}
				}
			}
			return null;
		}

		public bool DyeItems(Boots boot, Item dye_object)
		{
			ModEntry.debugmsg("Trying stain", LogLevel.Debug);
			string stain = GetStainIndex(dye_object);

			if(stain != "")
            {
				ModEntry.debugmsg($"Found a stain at [{stain}]", LogLevel.Debug);
				
				int stainColorSheet = 1;
				try
				{
                    //Attempt setting the correct color index
                    stainColorSheet = int.Parse(stain);
				} catch { }
				boot.indexInColorSheet.Set(stainColorSheet);

                if (ModEntry.dga != null)
                {
                    boot.modData["DGA.FarmerColors"] = stain;
                }
                return true;
			}
            else
            {
				return false;
            }
		}

		public string GetStainIndex(Item dye_object)
        {
            if (dye_object.HasContextTag("color_prismatic"))
            {
				return prismatic_stain;
            }

			if (dye_object.modData.ContainsKey("DGA.FarmerColors")) return dye_object.modData["DGA.FarmerColors"];
			if (dye_object is Boots) return (dye_object as Boots).indexInColorSheet + "";

			string currentIndex = "";

			Color? dye_color = ClothingModifier.GetItemColor(dye_object);
			if (dye_color.HasValue)
			{
                currentIndex = ShoesPalette.FindClosestColor(dye_color.Value, 2);
			}

			ModEntry.debugmsg($"Stain found for {dye_object.Name}: {currentIndex}", LogLevel.Debug);
			return currentIndex;
		}

		public bool IsValidCraft(Item left_item, Item right_item)
		{
			if (left_item == null || right_item == null)
			{
				return false;
			}
			if (left_item is Boots)
			{
				if (ClothingModifier.GetItemColor(right_item).HasValue)
				{
					return true;
				}

				if(right_item.HasContextTag("color_prismatic"))
                {
					return true;
                }

				if(right_item is Boots)
                {
					return true;
                }
			}
			return false;
		}

		public Item CraftItem(Item boot_item, Item stain_item)
		{
			if (boot_item == null || stain_item == null)
			{
				return null;
			}

			if (boot_item is Boots)
			{
				if (stain_item.HasContextTag("color_prismatic"))
				{

                    int stainColorSheet = 1;
                    try
                    {
                        //Attempt setting the correct color index
                        stainColorSheet = int.Parse(prismatic_stain);
					}
					catch { }
                    (boot_item as Boots).indexInColorSheet.Set(stainColorSheet);

                    if (ModEntry.dga != null)
					{
                        boot_item.modData["DGA.FarmerColors"] = prismatic_stain;
                    }

					return boot_item;
				}
				if (this.DyeItems(boot_item as Boots, stain_item))
				{
					return boot_item;
				}
			}
			return null;
		}

		public void SpendRightItem()
		{
			if (this.stainIngredientSpot.item != null)
			{
				this.stainIngredientSpot.item.Stack--;
				if (this.stainIngredientSpot.item.Stack <= 0 || this.stainIngredientSpot.item.maximumStackSize() == 1)
				{
					this.stainIngredientSpot.item = null;
				}
			}
		}

		public void SpendLeftItem()
		{
			if (this.bootsSpot.item != null)
			{
				this.bootsSpot.item.Stack--;
				if (this.bootsSpot.item.Stack <= 0)
				{
					this.bootsSpot.item = null;
				}
			}
		}

		public override void receiveRightClick(int x, int y, bool playSound = true)
		{
			base.receiveRightClick(x, y);
		}

		public override void performHoverAction(int x, int y)
		{
			base.hoveredItem = null;
			base.performHoverAction(x, y);
			base.hoverText = "";
			for (int i = 0; i < this.equipmentIcons.Count; i++)
			{
				if (this.equipmentIcons[i].containsPoint(x, y))
				{
					if (this.equipmentIcons[i].name == "Boots")
					{
						base.hoveredItem = Game1.player.boots.Value;
					}
					if (this.equipmentIcons[i].name == "Shirt")
					{
						base.hoveredItem = Game1.player.shirtItem.Value;
					}
				}
			}
			foreach (ClickableComponent c in this.sleeveIcons)
			{
				if (!c.containsPoint(x, y)) continue;
				base.hoverText = T("set_sleeves") +" " + T("sleeve_"+c.name);
			}
			if (this.craftResultDisplay.visible && this.craftResultDisplay.containsPoint(x, y) && this.craftResultDisplay.item != null)
			{
				if (this._isDyeCraft || Game1.player.HasTailoredThisItem(this.craftResultDisplay.item))
				{
					base.hoveredItem = this.craftResultDisplay.item;
				}
				else
				{
					base.hoverText = Game1.content.LoadString("Strings\\UI:Tailor_MakeResultUnknown");
				}
			}
			if (this.bootsSpot.containsPoint(x, y))
			{
				if (this.bootsSpot.item != null)
				{
					base.hoveredItem = this.bootsSpot.item;
				}
				else
				{
					base.hoverText = "Boots to stain";
				}
			}
			if (this.stainBottle.containsPoint(x, y))
			{
				if (prismatic_stain != "")
				{
					base.hoverText = T("boot_change_color");
				}
				else
				{
					base.hoverText = T("boot_stain_color");
				}
			}
			if (this.stainIngredientSpot.containsPoint(x, y) && this.stainIngredientSpot.item == null)
			{
				base.hoverText = T("stain_ingredient");
			}
			this.stainIngredientSpot.tryHover(x, y);
			this.bootsSpot.tryHover(x, y);
		}

		public bool CanFitCraftedItem()
		{
			if (this.craftResultDisplay.item != null && !Utility.canItemBeAddedToThisInventoryList(this.craftResultDisplay.item, base.inventory.actualInventory))
			{
				return false;
			}
			return true;
		}

		public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
		{
			string[] obj = new string[10] { "meow:", null, null, null, null, null, null, null, null, null };
			Rectangle rectangle = oldBounds;
			obj[1] = rectangle.ToString();
			obj[2] = " ";
			rectangle = newBounds;
			obj[3] = rectangle.ToString();
			obj[4] = " ";
			obj[5] = base.width.ToString();
			obj[6] = " ";
			obj[7] = base.height.ToString();
			obj[8] = " ";
			obj[9] = base.yPositionOnScreen.ToString();
			Console.WriteLine(string.Concat(obj));
			base.gameWindowSizeChanged(oldBounds, newBounds);
			Console.WriteLine("meow2:" + base.yPositionOnScreen);
			int yPositionForInventory = base.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth + 192 - 16 + 4;
			base.inventory = new InventoryMenu(base.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth / 2 + 12, yPositionForInventory, playerInventory: false, null, base.inventory.highlightMethod);
			this._CreateButtons();
		}

		public override void emergencyShutDown()
		{
			this._OnCloseMenu();
			base.emergencyShutDown();
		}

		public override void update(GameTime time)
		{
			base.update(time);
			base.descriptionText = this.displayedDescription;
			bool can_fit_crafted_item = this.CanFitCraftedItem();
			//Can take the coloured boots
			if (this._craftState == CraftState.Valid && can_fit_crafted_item)
			{
				this.craftResultDisplay.visible = true;
			}
			else
			{
				this.craftResultDisplay.visible = false;
			}

			if(prismatic_stain != "")
            {
				//stainBottleRainbowBack = new Rectangle(0, 88, 24, 24);
				int frame = (int)(time.TotalGameTime.TotalMilliseconds % 600.0 / 100.0);
				stainBottleRainbowBack.Y = 88 + (int)(frame / 2) * 24;
				stainBottleRainbowBack.X = (frame%2)*24;
			}
		}

		public override void draw(SpriteBatch b)
		{
			b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.6f);
			//Draw trees in Haley/Emily's place
			if (Game1.currentLocation.Name == "HaleyHouse")
			{
				b.Draw(this.tailoringTextures, new Vector2((float)base.xPositionOnScreen + 96f, base.yPositionOnScreen - 64), new Rectangle(101, 80, 41, 36), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.FlipHorizontally, 0.87f);
				b.Draw(this.tailoringTextures, new Vector2((float)base.xPositionOnScreen + 352f, base.yPositionOnScreen - 64), new Rectangle(101, 80, 41, 36), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
				b.Draw(this.tailoringTextures, new Vector2((float)base.xPositionOnScreen + 608f, base.yPositionOnScreen - 64), new Rectangle(101, 80, 41, 36), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
				b.Draw(this.tailoringTextures, new Vector2((float)base.xPositionOnScreen + 256f, base.yPositionOnScreen), new Rectangle(79, 97, 22, 20), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
				b.Draw(this.tailoringTextures, new Vector2((float)base.xPositionOnScreen + 512f, base.yPositionOnScreen), new Rectangle(79, 97, 22, 20), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
				b.Draw(this.tailoringTextures, new Vector2((float)base.xPositionOnScreen + 32f, base.yPositionOnScreen + 44), new Rectangle(81, 81, 16, 9), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
				b.Draw(this.tailoringTextures, new Vector2((float)base.xPositionOnScreen + 768f, base.yPositionOnScreen + 44), new Rectangle(81, 81, 16, 9), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
			}

			
			//Background colour for equipped items
			Game1.DrawBox(base.xPositionOnScreen - 64, base.yPositionOnScreen + 128, 94, 74 + equipmentIcons.Count * 64, bgColor);
			Game1.player.FarmerRenderer.drawMiniPortrat(b, new Vector2((float)(base.xPositionOnScreen - 64) + 9.6f, base.yPositionOnScreen + 128), 0.87f, 4f, 2, Game1.player);
			
			//draw top half
			base.draw(b, drawUpperPortion: true, drawDescriptionArea: false, bgColor.R, bgColor.G, bgColor.B);

			//tab for making
			b.Draw(moreTailoringTextures, new Vector2(makeTab.bounds.X, makeTab.bounds.Y), new Rectangle(64, 0, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0001f);
			//tab for dying
			b.Draw(moreTailoringTextures, new Vector2(dyeTab.bounds.X, dyeTab.bounds.Y), new Rectangle(64, 32, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0001f);
			//tab for this
			b.Draw(moreTailoringTextures, new Vector2(editTab.bounds.X, editTab.bounds.Y + 8), new Rectangle(64, 16, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0001f);


			//Sleeve section
			Utility.drawTextWithColoredShadow(b, T("sleeve_tailoring"), Game1.smallFont, sleeve_label.ToVector2(), Color.White, darkText);

			//Draw shirt sleeve settings
			if (this.shirtSpot.item != null && !(this.shirtSpot.item as Clothing).GetOtherData().Contains("Sleeveless"))
			{
				this.blankShirtSpot.draw(b);

				string setting = ModEntry.AssignShirtLength(shirtSpot.item as Clothing, Game1.player.IsMale);

				foreach (ClickableComponent c in this.sleeveIcons)
				{
					switch (c.name) {
						case "long":
							if (setting == "Long")
							{
								//Selected
								b.Draw(this.moreTailoringTextures, c.bounds, new Rectangle(48, 0, 16, 16), Color.White);
							}
							else
							{
								b.Draw(this.moreTailoringTextures, c.bounds, new Rectangle(32, 0, 16, 16), Color.White);
							}
							break;
						case "short":
							if (setting == "Short")
							{
								//Selected
								b.Draw(this.moreTailoringTextures, c.bounds, new Rectangle(48, 0, 16, 16), Color.White);
							}
							else
							{
								b.Draw(this.moreTailoringTextures, c.bounds, new Rectangle(32, 0, 16, 16), Color.White);
							}
							break;
						case "default":
							if (setting == "Normal")
							{
								//Selected
								b.Draw(this.moreTailoringTextures, c.bounds, new Rectangle(48, 0, 16, 16), Color.White);
							}
							else
							{
								b.Draw(this.moreTailoringTextures, c.bounds, new Rectangle(32, 0, 16, 16), Color.White);
							}
							break;
						case "sleeveless":
							if (setting == "Sleeveless")
							{
								//Selected
								b.Draw(this.moreTailoringTextures, c.bounds, new Rectangle(48, 0, 16, 16), Color.White);
							}
							else
							{
								b.Draw(this.moreTailoringTextures, c.bounds, new Rectangle(32, 0, 16, 16), Color.White);
							}
							break;
					}
					//Draw icon
					b.Draw(this.moreTailoringTextures, c.bounds, new Rectangle(48, (c.myID - region_shirtSetting) * 16, 16, 16), Color.White);
				}
			}
			else
			{
				this.shirtSpot.draw(b, Color.White, 0.87f);

				foreach (ClickableComponent c in this.sleeveIcons)
				{
					//Draw bg non selected
					b.Draw(this.moreTailoringTextures, c.bounds, new Rectangle(32, 0, 16, 16), Color.White);
					//Draw icon
					b.Draw(this.moreTailoringTextures, c.bounds, new Rectangle(48, (c.myID-region_shirtSetting)*16, 16, 16), Color.White);
				}
			}
			this.shirtSpot.drawItem(b, 4 * 4, 4 * 4);

			//extra setting section
			Utility.drawTextWithColoredShadow(b, T("other_clothing_options"), Game1.smallFont, extra_settings_label.ToVector2(), Color.White, darkText);
			useBathers.draw(b, Color.White, 0.75f);
			useOverallColor.draw(b, Color.White, 0.75f);


			//Staining section
			Utility.drawTextWithColoredShadow(b, T("boots_stainer"), Game1.smallFont, stain_label.ToVector2(), Color.White, darkText);

			if (this.bootsSpot.item != null)
			{
				this.blankBootsSpot.draw(b);
			}
			else
			{
				this.bootsSpot.draw(b, Color.White, 0.87f);
			}
			this.bootsSpot.drawItem(b, 4 * 4, 4 * 4);
			
			if (this.craftResultDisplay.visible)
			{
				this.craftResultDisplay.draw(b);
				if (this.craftResultDisplay.item != null)
				{
					if (this._isDyeCraft || Game1.player.HasTailoredThisItem(this.craftResultDisplay.item))
					{
						this.craftResultDisplay.drawItem(b);
					}
				}
			}
            else
            {
				b.Draw(moreTailoringTextures, new Vector2(craftResultDisplay.bounds.X, craftResultDisplay.bounds.Y), new Rectangle(0, 0, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.8f);
			}

			foreach (ClickableComponent c in this.equipmentIcons)
			{
				switch (c.name)
				{
					case "Shirt":
						if (Game1.player.shirtItem.Value != null)
						{
							//draw bounds of square only
							b.Draw(this.moreTailoringTextures, c.bounds, new Rectangle(0, 0, 16, 16), Color.White);
							float transparency3 = 1f;
							if (!this.HighlightItems(Game1.player.shirtItem))
							{
								transparency3 = 0.5f;
							}
							if (Game1.player.shirtItem.Value == base.heldItem)
							{
								transparency3 = 0.5f;
							}
							Game1.player.shirtItem.Value.drawInMenu(b, new Vector2(c.bounds.X, c.bounds.Y), c.scale, transparency3, 0.866f);
						}
						else
						{
							//Draw bounds with shirt placeholder
							b.Draw(this.moreTailoringTextures, c.bounds, new Rectangle(48, 80, 16, 16), Color.White);
						}
						break;
					case "Shoes":
						if (Game1.player.boots.Value != null)
						{
							//draw bounds of square only
							b.Draw(this.moreTailoringTextures, c.bounds, new Rectangle(0, 0, 16, 16), Color.White);
							float transparency3 = 1f;
							if (!this.HighlightItems((Item)Game1.player.boots))
							{
								transparency3 = 0.5f;
							}
							if (Game1.player.boots.Value == base.heldItem)
							{
								transparency3 = 0.5f;
							}
							Game1.player.boots.Value.drawInMenu(b, new Vector2(c.bounds.X, c.bounds.Y), c.scale, transparency3, 0.866f);
						}
						else
						{
							//Draw bounds with boots placeholder
							b.Draw(this.moreTailoringTextures, c.bounds, new Rectangle(16, 0, 16, 16), Color.White);
						}
						break;
				}
			}
			if (this.stainIngredientSpot.item != null)
			{
				this.blankStainIngredientSpot.draw(b);
			}
			else
			{
				//Glow the spot
				this.stainIngredientSpot.draw(b, Color.White, 0.87f);
			}
			this.stainIngredientSpot.drawItem(b, 16, 4 * 4);

			if (prismatic_stain != "") {
				b.Draw(this.moreTailoringTextures, stainBottle.bounds, stainBottleRainbowBack, Color.White);
			}

			this.stainBottle.draw(b);
			if(this._craftState == CraftState.ValidStainOnly || this._craftState == CraftState.Valid)
            {
				b.Draw(stainBottleOverlay, new Vector2(stainBottle.bounds.X, stainIngredientSpot.bounds.Y), new Rectangle(0, 0, 24, 24), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.8f);
			}
			//Partition for the staining section
			base.drawVerticalUpperIntersectingPartition(b, stain_label.X - 48, 328, bgColor.R, bgColor.G, bgColor.B);

			if (!base.hoverText.Equals(""))
			{
				IClickableMenu.drawHoverText(b, base.hoverText, Game1.smallFont, (base.heldItem != null) ? 32 : 0, (base.heldItem != null) ? 32 : 0);
			}
			else if (base.hoveredItem != null)
			{
				IClickableMenu.drawToolTip(b, base.hoveredItem.getDescription(), base.hoveredItem.DisplayName, base.hoveredItem, base.heldItem != null);
			}
			
			if (base.heldItem != null)
			{
				base.heldItem.drawInMenu(b, new Vector2(Game1.getOldMouseX() + 8, Game1.getOldMouseY() + 8), 1f);
			}
			
			if (!Game1.options.hardwareCursor)
			{
				base.drawMouse(b);
			}
		}

		protected override void cleanupBeforeExit()
		{
			this._OnCloseMenu();
		}

		public void cancelAndExit()
		{
			cleanupBeforeExit();
			this.exitThisMenu(false);
			Game1.playSound("cancel");
		}

		protected void _OnCloseMenu()
		{
			if (!Game1.player.IsEquippedItem(base.heldItem))
			{
				Utility.CollectOrDrop(base.heldItem);
			}
			if (!Game1.player.IsEquippedItem(this.shirtSpot.item))
			{
				Utility.CollectOrDrop(this.shirtSpot.item);
			}
			if (!Game1.player.IsEquippedItem(this.bootsSpot.item))
			{
				Utility.CollectOrDrop(this.bootsSpot.item);
			}
			if (!Game1.player.IsEquippedItem(this.stainIngredientSpot.item))
			{
				Utility.CollectOrDrop(this.stainIngredientSpot.item);
			}
			//Fix for no shoes on when exiting
			foreach (ClickableComponent c in this.equipmentIcons)
			{
				switch (c.name)
				{
					case "Shoes":
						if(c.item == null)
                        {
							Game1.player.changeShoeColor(12);
							ModEntry.MakePlayerDirty();
                        }
						break;
				}
			}
			base.heldItem = null;
			this.shirtSpot.item = null;
			this.bootsSpot.item = null;
			this.stainIngredientSpot.item = null;
		}

	}
}
