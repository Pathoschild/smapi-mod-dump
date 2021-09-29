/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/joisse1101/InstantAnimals
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using xTile.Dimensions;
using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;

namespace InstantAnimals
{
	public class InstantPurchaseAnimalsMenu : IClickableMenu
	{
		public const int region_okButton = 101;

		public const int region_doneNamingButton = 102;

		public const int region_randomButton = 103;

		public const int region_namingBox = 104;

		public static int menuHeight = 320;

		public static int menuWidth = 448;

		public List<List<ClickableTextureComponent>> animalsToPurchase = new List<List<ClickableTextureComponent>>();

		public ClickableTextureComponent okButton;

		public ClickableTextureComponent doneNamingButton;

		public ClickableTextureComponent randomButton;

		public ClickableTextureComponent hovered;

		public ClickableComponent textBoxCC;

		private ClickableTextureComponent upButton;

		private ClickableTextureComponent downButton;

		private bool onFarm;

		private bool namingAnimal;

		private bool freeze;

		private FarmAnimal animalBeingPurchased;

		private TextBox textBox;

		private TextBoxEvent e;

		private Building newAnimalHome;

		private int priceOfAnimal;

		public bool readOnly;

		public ulong latestID;

		public static IDictionary<string, string> strings;

		public static IDictionary<string, Texture2D> textures;

		public static IDictionary<string, string> data;

		private static int[] okBounds;

		private static int pageNum = 0;

		private static int pageMax;

		public static bool adults;

		public List<StardewValley.Object> stock;

		public static int friendship = 0;		
		public void setAdult (bool x)
        {

			adults = x;
        }
		public InstantPurchaseAnimalsMenu((List<StardewValley.Object>, IDictionary<string, string>, IDictionary<string, Texture2D>, IDictionary<string, string>) p, bool xadults)
			: base(Game1.uiViewport.Width / 2 - InstantPurchaseAnimalsMenu.menuWidth / 2 - IClickableMenu.borderWidth * 2, (Game1.uiViewport.Height - InstantPurchaseAnimalsMenu.menuHeight - IClickableMenu.borderWidth * 2) / 4, InstantPurchaseAnimalsMenu.menuWidth + IClickableMenu.borderWidth * 2, InstantPurchaseAnimalsMenu.menuHeight + IClickableMenu.borderWidth)
		{
			(List<StardewValley.Object> stock, IDictionary<string, string> xstrings, IDictionary<string, Texture2D> xtextures, IDictionary<string, string> xdata) = p;
			textures = xtextures;
			strings = xstrings;
			data = xdata;
			adults = xadults;
			base.height += 128;
			pageMax = stock.Count / 9;
			for (int j = 0; j <= pageMax; j++)
			{
				List<ClickableTextureComponent> page = new List<ClickableTextureComponent>();
				for (int i = 0 + j * 9 ; i < Math.Min(stock.Count, (j+1) * 9); i++)
				{
					page.Add(new ClickableTextureComponent(string.Concat(stock[i].salePrice()), new Microsoft.Xna.Framework.Rectangle(base.xPositionOnScreen + IClickableMenu.borderWidth * 2 + i % 3 * 75 * 2, base.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth/2 + (i%9) / 3 * 90, 64, 64), null, stock[i].Name, textures[stock[i].Name], new Microsoft.Xna.Framework.Rectangle(1, 1, Convert.ToInt32(data[stock[i].Name].Split('/')[16])-1, Convert.ToInt32(data[stock[i].Name].Split('/')[17])-1), (Convert.ToInt32(data[stock[i].Name].Split('/')[16]) < 17) ? 4f : 2f, stock[i].Type == null)
					{
						item = stock[i],
						myID = i % 9,
						rightNeighborID = (((i % 9) % 3 == 2) ? (-1) : ((i % 9) + 1)),
						leftNeighborID = (((i % 9) % 3 == 0) ? (-1) : ((i % 9) - 1)),
						downNeighborID = (i % 9) + 3,
						upNeighborID = (i % 9) - 3
					});
				}
				animalsToPurchase.Add(page);
			}

			this.upButton = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(base.xPositionOnScreen + base.width, base.yPositionOnScreen + IClickableMenu.borderWidth/2, 44, 48), Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(421, 459, 11, 12), 4f);
			this.downButton = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(base.xPositionOnScreen + base.width, base.yPositionOnScreen + base.height - 4 - IClickableMenu.borderWidth / 2, 44, 48), Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(421, 472, 11, 12), 4f);
			
			okBounds = new[] { base.xPositionOnScreen + base.width + 64, base.yPositionOnScreen + base.height - 4 - IClickableMenu.borderWidth };
			this.okButton = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(okBounds[0], okBounds[1], 64, 64), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 47), 1f)
			{
				myID = 101,
				upNeighborID = 103,
				leftNeighborID = 103
			};
			this.randomButton = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(base.xPositionOnScreen + base.width + 51 + 64, Game1.uiViewport.Height / 2, 64, 64), Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(381, 361, 10, 10), 4f)
			{
				myID = 103,
				downNeighborID = 101,
				rightNeighborID = 101
			};
			
			InstantPurchaseAnimalsMenu.menuHeight = 320;
			InstantPurchaseAnimalsMenu.menuWidth = 448;
			this.textBox = new TextBox(null, null, Game1.dialogueFont, Game1.textColor);
			this.textBox.X = Game1.uiViewport.Width / 2 - 192;
			this.textBox.Y = Game1.uiViewport.Height / 2;
			this.textBox.Width = 256;
			this.textBox.Height = 192;
			this.e = textBoxEnter;
			this.textBoxCC = new ClickableComponent(new Microsoft.Xna.Framework.Rectangle(this.textBox.X, this.textBox.Y, 192, 48), "")
			{
				myID = 104,
				rightNeighborID = 102,
				downNeighborID = 101
			};
			this.randomButton = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(this.textBox.X + this.textBox.Width + 64 + 48 - 8, Game1.uiViewport.Height / 2 + 4, 64, 64), Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(381, 361, 10, 10), 4f)
			{
				myID = 103,
				leftNeighborID = 102,
				downNeighborID = 101,
				rightNeighborID = 101
			};
			this.doneNamingButton = new ClickableTextureComponent(new Microsoft.Xna.Framework.Rectangle(this.textBox.X + this.textBox.Width + 32 + 4, Game1.uiViewport.Height / 2 - 8, 64, 64), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f)
			{
				myID = 102,
				rightNeighborID = 103,
				leftNeighborID = 104,
				downNeighborID = 101
			};
			if (Game1.options.SnappyMenus)
			{
				base.populateClickableComponentList();
				this.snapToDefaultClickableComponent();
			}
		}

        public override bool shouldClampGamePadCursor()
		{
			return this.onFarm;
		}

		public override void snapToDefaultClickableComponent()
		{
			base.currentlySnappedComponent = base.getComponentWithID(0);
			this.snapCursorToCurrentSnappedComponent();
		}

		public void textBoxEnter(TextBox sender)
		{
			if (!this.namingAnimal)
			{
				return;
			}
			if (Game1.activeClickableMenu == null || !(Game1.activeClickableMenu is InstantPurchaseAnimalsMenu))
			{
				this.textBox.OnEnterPressed -= this.e;
			}
			else if (sender.Text.Length >= 1)
			{
				if (Utility.areThereAnyOtherAnimalsWithThisName(sender.Text))
				{
					Game1.showRedMessage(strings["PurchaseAnimalsMenu.cs.11308"]);
					return;
				}
				this.textBox.OnEnterPressed -= this.e;
				this.animalBeingPurchased.Name = sender.Text;
				this.animalBeingPurchased.displayName = sender.Text;
				this.animalBeingPurchased.displayType = this.animalBeingPurchased.Name;
				this.animalBeingPurchased.home = this.newAnimalHome;
				this.animalBeingPurchased.homeLocation.Value = new Vector2((int)this.newAnimalHome.tileX, (int)this.newAnimalHome.tileY);
				this.animalBeingPurchased.setRandomPosition(this.animalBeingPurchased.home.indoors);
				(this.newAnimalHome.indoors.Value as AnimalHouse).animals.Add(this.animalBeingPurchased.myID, this.animalBeingPurchased);
				(this.newAnimalHome.indoors.Value as AnimalHouse).animalsThatLiveHere.Add(this.animalBeingPurchased.myID);
				this.newAnimalHome = null;
				this.namingAnimal = false;
				Game1.player.Money -= this.priceOfAnimal;
				this.setUpForReturnAfterPurchasingAnimal();
			}
		}

		public void setUpForReturnAfterPurchasingAnimal()
		{
			//LocationRequest locationRequest = Game1.getLocationRequest("AnimalShop");
			//locationRequest.OnWarp += delegate
			{
				///this.onFarm = false;
				//Game1.player.viewingLocation.Value = null;
				this.okButton.bounds.X = base.xPositionOnScreen + base.width + 4;
				Game1.displayHUD = true;
				Game1.displayFarmer = true;
				this.freeze = false;
				this.textBox.OnEnterPressed -= this.e;
				this.textBox.Selected = false;
				Game1.viewportFreeze = false;
				base.exitThisMenu();
				Game1.player.forceCanMove();
				this.freeze = false;
			};
			//Game1.warpFarmer(locationRequest, Game1.player.getTileX(), Game1.player.getTileY(), Game1.player.facingDirection);
		}

		public void setUpForAnimalPlacement()
		{
			Game1.currentLocation.cleanupBeforePlayerExit();
			Game1.displayFarmer = false;
			Game1.currentLocation.resetForPlayerEntry();
			Game1.currentLocation.cleanupBeforePlayerExit();
			this.onFarm = true;
			this.freeze = false;
			this.okButton.bounds.X = Game1.uiViewport.Width - 128;
			this.okButton.bounds.Y = Game1.uiViewport.Height - 128;
			Game1.displayHUD = false;
			Game1.viewportFreeze = true;
			Game1.viewport.Location = new Location((Game1.player.getTileX() * Game1.tileSize) - (Game1.viewport.Width / 2), (Game1.player.getTileY() * Game1.tileSize) - (Game1.viewport.Height / 2));
			Game1.panScreen(0, 0);
		}

		public void setUpForReturnToShopMenu()
		{
			this.freeze = false;
			Game1.displayFarmer = true;
			{
				this.onFarm = false;
				Game1.player.viewingLocation.Value = null;
				this.okButton.bounds.X = okBounds[0];
				this.okButton.bounds.Y = okBounds[1];
				Game1.displayHUD = true;
				Game1.viewportFreeze = false;
				this.namingAnimal = false;
				this.textBox.OnEnterPressed -= this.e;
				this.textBox.Selected = false;
				if (Game1.options.SnappyMenus)
				{
					this.snapToDefaultClickableComponent();
				}
			};
		}

		public override void receiveLeftClick(int x, int y, bool playSound = true)
		{
			if (Game1.IsFading() || this.freeze)
			{
				return;
			}
			if (this.okButton != null && this.okButton.containsPoint(x, y) && this.readyToClose())
			{
				if (this.onFarm)
				{
					this.setUpForReturnToShopMenu();
					Game1.playSound("smallSelect");
				}
				else
				{
					Game1.exitActiveMenu();
					Game1.playSound("bigDeSelect");
				}
			}
			if (this.onFarm)
			{
				Vector2 clickTile = new Vector2((int)((Utility.ModifyCoordinateFromUIScale(x) + (float)Game1.viewport.X) / 64f), (int)((Utility.ModifyCoordinateFromUIScale(y) + (float)Game1.viewport.Y) / 64f));
				Building selection = (Game1.getLocationFromName("Farm") as Farm).getBuildingAt(clickTile);
				if (selection != null && !this.namingAnimal)
				{
					if (selection.buildingType.Value.Contains(this.animalBeingPurchased.buildingTypeILiveIn.Value))
					{
						if ((selection.indoors.Value as AnimalHouse).isFull())
						{
							Game1.showRedMessage(strings["PurchaseAnimalsMenu.cs.11321"]);
						}
						else if ((byte)this.animalBeingPurchased.harvestType != 2)
						{
							this.namingAnimal = true;
							this.newAnimalHome = selection;
							if (this.animalBeingPurchased.sound.Value != null && Game1.soundBank != null)
							{
								ICue cue = Game1.soundBank.GetCue(this.animalBeingPurchased.sound.Value);
								cue.SetVariable("Pitch", 1200 + Game1.random.Next(-200, 201));
								cue.Play();
							}
							this.textBox.OnEnterPressed += this.e;
							this.textBox.Text = this.animalBeingPurchased.displayName;
							Game1.keyboardDispatcher.Subscriber = this.textBox;
							if (Game1.options.SnappyMenus)
							{
								base.currentlySnappedComponent = base.getComponentWithID(104);
								this.snapCursorToCurrentSnappedComponent();
							}
						}
						else if (Game1.player.Money >= this.priceOfAnimal)
						{
							this.newAnimalHome = selection;
							this.animalBeingPurchased.home = this.newAnimalHome;
							this.animalBeingPurchased.homeLocation.Value = new Vector2((int)this.newAnimalHome.tileX, (int)this.newAnimalHome.tileY);
							this.animalBeingPurchased.setRandomPosition(this.animalBeingPurchased.home.indoors);
							(this.newAnimalHome.indoors.Value as AnimalHouse).animals.Add(this.animalBeingPurchased.myID, this.animalBeingPurchased);
							(this.newAnimalHome.indoors.Value as AnimalHouse).animalsThatLiveHere.Add(this.animalBeingPurchased.myID);
							this.newAnimalHome = null;
							this.namingAnimal = false;
							if (this.animalBeingPurchased.sound.Value != null && Game1.soundBank != null)
							{
								ICue cue2 = Game1.soundBank.GetCue(this.animalBeingPurchased.sound.Value);
								cue2.SetVariable("Pitch", 1200 + Game1.random.Next(-200, 201));
								cue2.Play();
							}
							Game1.player.Money -= this.priceOfAnimal;
							Game1.addHUDMessage(new HUDMessage(String.Format(strings["PurchaseAnimalsMenu.cs.11324"], this.animalBeingPurchased.displayType), Color.LimeGreen, 3500f));
						}
						else if (Game1.player.Money < this.priceOfAnimal)
						{
							Game1.dayTimeMoneyBox.moneyShakeTimer = 1000;
						}
					}
					else
					{
						Game1.showRedMessage(String.Format(strings["PurchaseAnimalsMenu.cs.11326"], this.animalBeingPurchased.displayType));
					}
				}
				if (this.namingAnimal)
				{
					if (this.doneNamingButton.containsPoint(x, y))
					{
						this.textBoxEnter(this.textBox);
						Game1.playSound("smallSelect");
					}
					else if (this.namingAnimal && this.randomButton.containsPoint(x, y))
					{
						this.animalBeingPurchased.Name = Dialogue.randomName();
						this.animalBeingPurchased.displayName = this.animalBeingPurchased.Name;
						this.textBox.Text = this.animalBeingPurchased.displayName;
						this.randomButton.scale = this.randomButton.baseScale;
						Game1.playSound("drumkit6");
					}
					this.textBox.Update();
				}
				return;
			}
			if (this.upButton.containsPoint(x, y) && pageNum > 0)
			{
				pageNum--;
				if (Game1.options.SnappyMenus)
				{
					base.populateClickableComponentList();
					this.snapToDefaultClickableComponent();
				}

			}
			if (this.downButton.containsPoint(x, y) && pageNum < pageMax)
			{
				pageNum++;
				if (Game1.options.SnappyMenus)
				{
					base.populateClickableComponentList();
					this.snapToDefaultClickableComponent();
				}

			}
			foreach (ClickableTextureComponent c in this.animalsToPurchase[pageNum])
			{
				if (!this.readOnly && c.containsPoint(x, y) && (c.item as StardewValley.Object).Type == null)
				{
					int price = c.item.salePrice();
					if (Game1.player.Money >= price)
					{
						setUpForAnimalPlacement();
						Game1.playSound("smallSelect");
						this.onFarm = true;
						while(this.animalBeingPurchased == null || !this.animalBeingPurchased.type.Value.Equals(c.hoverText)) { this.animalBeingPurchased = new FarmAnimal(c.hoverText, getNewID(), Game1.player.UniqueMultiplayerID); }
						if (adults) { this.makeAdult(this.animalBeingPurchased); }
						this.animalBeingPurchased.friendshipTowardFarmer.Value = friendship * 200;
						this.priceOfAnimal = price;
					}
					else
					{
						Game1.addHUDMessage(new HUDMessage(strings["PurchaseAnimalsMenu.cs.11325"], Color.Red, 3500f));
					}
				}
			}
		}

        public override void receiveScrollWheelAction(int direction)
        {
			if (!this.onFarm && !Game1.dialogueUp && !Game1.IsFading())
            {
				base.receiveScrollWheelAction(direction);
				if (direction > 0 && friendship < 5) { friendship++; }
				else if (direction < 0 && friendship > 0 ) { friendship--;  }
			}
        }

        public override bool overrideSnappyMenuCursorMovementBan()
		{
			if (this.onFarm)
			{
				return !this.namingAnimal;
			}
			return false;
		}

		public override void receiveGamePadButton(Buttons b)
		{
			base.receiveGamePadButton(b);
			if (b == Buttons.B && !Game1.globalFade && this.onFarm && this.namingAnimal)
			{
				this.setUpForReturnToShopMenu();
				Game1.playSound("smallSelect");
			}
		}

		public override void receiveKeyPress(Keys key)
		{
			if (Game1.globalFade || this.freeze)
			{
				return;
			}
			if (!Game1.globalFade && this.onFarm)
			{
				if (!this.namingAnimal)
				{
					if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && this.readyToClose() && !Game1.IsFading())
					{
						this.setUpForReturnToShopMenu();
					}
					else if (!Game1.options.SnappyMenus)
					{
						if (Game1.options.doesInputListContain(Game1.options.moveDownButton, key))
						{
							Game1.panScreen(0, 4);
						}
						else if (Game1.options.doesInputListContain(Game1.options.moveRightButton, key))
						{
							Game1.panScreen(4, 0);
						}
						else if (Game1.options.doesInputListContain(Game1.options.moveUpButton, key))
						{
							Game1.panScreen(0, -4);
						}
						else if (Game1.options.doesInputListContain(Game1.options.moveLeftButton, key))
						{
							Game1.panScreen(-4, 0);
						}
					}
				}
				else if (Game1.options.SnappyMenus)
				{
					if (!this.textBox.Selected && Game1.options.doesInputListContain(Game1.options.menuButton, key))
					{
						this.setUpForReturnToShopMenu();
						Game1.playSound("smallSelect");
					}
					else if (!this.textBox.Selected || !Game1.options.doesInputListContain(Game1.options.menuButton, key))
					{
						base.receiveKeyPress(key);
					}
				}
			}
			else if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && !Game1.IsFading())
			{
				if (this.readyToClose())
				{
					Game1.player.forceCanMove();
					Game1.exitActiveMenu();
					Game1.playSound("bigDeSelect");
				}
			}
			else if (Game1.options.SnappyMenus)
			{
				base.receiveKeyPress(key);
			}
		}

		public override void update(GameTime time)
		{
			base.update(time);
			if (this.onFarm && !this.namingAnimal)
			{
				int mouseX = Game1.getOldMouseX(ui_scale: false) + Game1.viewport.X;
				int mouseY = Game1.getOldMouseY(ui_scale: false) + Game1.viewport.Y;
				if (mouseX - Game1.viewport.X < 64)
				{
					Game1.panScreen(-8, 0);
				}
				else if (mouseX - (Game1.viewport.X + Game1.viewport.Width) >= -64)
				{
					Game1.panScreen(8, 0);
				}
				if (mouseY - Game1.viewport.Y < 64)
				{
					Game1.panScreen(0, -8);
				}
				else if (mouseY - (Game1.viewport.Y + Game1.viewport.Height) >= -64)
				{
					Game1.panScreen(0, 8);
				}
				Keys[] pressedKeys = Game1.oldKBState.GetPressedKeys();
				foreach (Keys key in pressedKeys)
				{
					this.receiveKeyPress(key);
				}
			}
		}

		public override void receiveRightClick(int x, int y, bool playSound = true)
		{
		}

		public override void performHoverAction(int x, int y)
		{
			this.hovered = null;
			if (Game1.IsFading() || this.freeze)
			{
				return;
			}
			if (this.upButton.containsPoint(x, y))
			{
				this.upButton.scale = Math.Min(4.5f, this.upButton.scale + 0.25f);
			}
			else
			{
				this.upButton.scale = Math.Max(4f, this.upButton.scale - 0.25f);
			}
			if (this.downButton.containsPoint(x, y))
			{
				this.downButton.scale = Math.Min(4.5f, this.downButton.scale + 0.25f);
			}
			else
			{
				this.downButton.scale = Math.Max(4f, this.downButton.scale - 0.25f);
			}
			if (this.okButton != null)
			{
				if (this.okButton.containsPoint(x, y))
				{
					this.okButton.scale = Math.Min(1.1f, this.okButton.scale + 0.05f);
				}
				else
				{
					this.okButton.scale = Math.Max(1f, this.okButton.scale - 0.05f);
				}
			}
			if (this.onFarm)
			{
				if (!this.namingAnimal)
				{
					Vector2 clickTile = new Vector2((int)((Utility.ModifyCoordinateFromUIScale(x) + (float)Game1.viewport.X) / 64f), (int)((Utility.ModifyCoordinateFromUIScale(y) + (float)Game1.viewport.Y) / 64f));
					Farm f = Game1.getLocationFromName("Farm") as Farm;
					foreach (Building building in f.buildings)
					{
						building.color.Value = Color.White;
					}
					Building selection = f.getBuildingAt(clickTile);
					if (selection != null)
					{
						if (selection.buildingType.Value.Contains(this.animalBeingPurchased.buildingTypeILiveIn.Value) && !(selection.indoors.Value as AnimalHouse).isFull())
						{
							selection.color.Value = Color.LightGreen * 0.8f;
						}
						else
						{
							selection.color.Value = Color.Red * 0.8f;
						}
					}
				}
				if (this.doneNamingButton != null)
				{
					if (this.doneNamingButton.containsPoint(x, y))
					{
						this.doneNamingButton.scale = Math.Min(1.1f, this.doneNamingButton.scale + 0.05f);
					}
					else
					{
						this.doneNamingButton.scale = Math.Max(1f, this.doneNamingButton.scale - 0.05f);
					}
				}
				this.randomButton.tryHover(x, y, 0.5f);
				return;
			}
			foreach (ClickableTextureComponent c in this.animalsToPurchase[pageNum])
			{
				if (c.containsPoint(x, y))
				{
					c.scale = Math.Min(c.scale + 0.05f, c.baseScale < 3 ? 3.1f : 4.1f);
					
					this.hovered = c;
				}
				else
				{
					c.scale = Math.Max(c.baseScale < 3 ? 3f : 4f, c.scale - 0.025f);
				}
			}
		}

		public static string getAnimalTitle(string name)
		{
			return name;
		}

		public static string getAnimalDescription(string name, IModHelper helper = null)
		{	
			switch (name)
            {
				case "White Chicken":
				case "Brown Chicken":
				case "Blue Chicken":
				case "Void Chicken":
				case "Golden Chicken":
					return strings["PurchaseAnimalsMenu.cs.11334"] + Environment.NewLine + strings["PurchaseAnimalsMenu.cs.11335"];
				case "Duck": 
					return strings["PurchaseAnimalsMenu.cs.11337"] + Environment.NewLine + strings["PurchaseAnimalsMenu.cs.11335"];
				case "Rabbit":
					return strings["PurchaseAnimalsMenu.cs.11340"] + Environment.NewLine + strings["PurchaseAnimalsMenu.cs.11335"];
				case "White Cow":
					case "Brown Cow":
					return strings["PurchaseAnimalsMenu.cs.11343"] + Environment.NewLine + strings["PurchaseAnimalsMenu.cs.11344"];
				case "Pig":
					return strings["PurchaseAnimalsMenu.cs.11346"] + Environment.NewLine + strings["PurchaseAnimalsMenu.cs.11344"];
				case "Goat":
					return strings["PurchaseAnimalsMenu.cs.11349"] + Environment.NewLine + strings["PurchaseAnimalsMenu.cs.11344"];
				case "Sheep":
					return strings["PurchaseAnimalsMenu.cs.11352"] + Environment.NewLine + strings["PurchaseAnimalsMenu.cs.11344"];
				case "Ostrich":
					return strings["Ostrich_Description"] + Environment.NewLine + strings["PurchaseAnimalsMenu.cs.11344"];
				case "Dinosaur":
					return "Adult dinosaurs will lay a very large egg once a week." + Environment.NewLine + "Baby dinosaurs not sold for fear of non-fingers.";
				default:
					return "";
            }
		}

		public override void draw(SpriteBatch b)
		{
			if (!this.onFarm && !Game1.dialogueUp && !Game1.IsFading())
			{
				b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
				if (adults) { SpriteText.drawStringWithScrollBackground(b, "Adult " + strings["PurchaseAnimalsMenu.cs.11354"], base.xPositionOnScreen + 96, base.yPositionOnScreen); }
				else { SpriteText.drawStringWithScrollBackground(b, "Baby " + strings["PurchaseAnimalsMenu.cs.11354"], base.xPositionOnScreen + 96, base.yPositionOnScreen); }
				Game1.drawDialogueBox(base.xPositionOnScreen, base.yPositionOnScreen, base.width, base.height, speaker: false, drawOnlyBox: true);
				Game1.dayTimeMoneyBox.drawMoneyBox(b);
				foreach (ClickableTextureComponent c in this.animalsToPurchase[pageNum])
				{
					c.draw(b, ((c.item as StardewValley.Object).Type != null) ? (Color.Black * 0.4f) : Color.White, 0.87f);
				}
				if (pageNum != 0)
				{
					this.upButton.draw(b);
				}
				if (pageMax > 0 && pageNum < pageMax)
				{
					this.downButton.draw(b);
				}
				for (int hearts = 0; hearts < 5; hearts++)
				{
					b.Draw(Game1.mouseCursors, new Vector2(base.xPositionOnScreen + IClickableMenu.borderWidth * 2 + hearts * 64, base.yPositionOnScreen + base.height - 64 - IClickableMenu.borderWidth / 2), new Microsoft.Xna.Framework.Rectangle(211, 428, 7, 6), (friendship <= hearts) ? (Color.Black * 0.35f) : Color.White, 0f, Vector2.Zero, 6f, SpriteEffects.None, 0.88f);
				}
			}
			else if (!Game1.IsFading() && this.onFarm)
			{
				string s = String.Format(strings["PurchaseAnimalsMenu.cs.11355"], this.animalBeingPurchased.displayHouse, this.animalBeingPurchased.displayType);
				SpriteText.drawStringWithScrollBackground(b, s, Game1.uiViewport.Width / 2 - SpriteText.getWidthOfString(s) / 2, 16);
				if (this.namingAnimal)
				{
					b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
					Game1.drawDialogueBox(Game1.uiViewport.Width / 2 - 256, Game1.uiViewport.Height / 2 - 192 - 32, 512, 192, speaker: false, drawOnlyBox: true);
					Utility.drawTextWithShadow(b, strings["PurchaseAnimalsMenu.cs.11357"], Game1.dialogueFont, new Vector2(Game1.uiViewport.Width / 2 - 256 + 32 + 8, Game1.uiViewport.Height / 2 - 128 + 8), Game1.textColor);
					this.textBox.Draw(b);
					this.doneNamingButton.draw(b);
					this.randomButton.draw(b);
				}
			}
			if (!Game1.IsFading() && this.okButton != null)
			{
				this.okButton.draw(b);
			}
			if (this.hovered != null)
			{
				if ((this.hovered.item as StardewValley.Object).Type != null)
				{
					IClickableMenu.drawHoverText(b, Game1.parseText((this.hovered.item as StardewValley.Object).Type, Game1.dialogueFont, 320), Game1.dialogueFont);
				}
				else
				{
					string displayName = InstantPurchaseAnimalsMenu.getAnimalTitle(this.hovered.hoverText);
					SpriteText.drawStringWithScrollBackground(b, displayName, base.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 64, base.yPositionOnScreen + base.height + -32 + IClickableMenu.spaceToClearTopBorder / 2 + 8, "Truffle Pig");
					SpriteText.drawStringWithScrollBackground(b, "$" + String.Format(strings["LoadGameMenu.cs.11020"], this.hovered.item.salePrice()), base.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 128, base.yPositionOnScreen + base.height + 64 + IClickableMenu.spaceToClearTopBorder / 2 + 8, "$99999999g", (Game1.player.Money >= this.hovered.item.salePrice()) ? 1f : 0.5f);
					string description = InstantPurchaseAnimalsMenu.getAnimalDescription(this.hovered.hoverText);
					IClickableMenu.drawHoverText(b, Game1.parseText(description, Game1.smallFont, 320), Game1.smallFont, 0, 0, -1, displayName);
				}
			}
			Game1.mouseCursorTransparency = 1f;
			base.drawMouse(b);
		}
		public virtual long getNewID()
		{
			ulong seqNum = ((this.latestID & 0xFF) + 1) & 0xFF;
			ulong nodeID = (ulong)Game1.player.UniqueMultiplayerID;
			nodeID = (nodeID >> 32) ^ (nodeID & 0xFFFFFFFFu);
			nodeID = ((nodeID >> 16) ^ (nodeID & 0xFFFF)) & 0xFFFF;
			ulong timestamp = (ulong)(DateTime.Now.Ticks / 10000);
			this.latestID = (timestamp << 24) | (nodeID << 8) | seqNum;
			return (long)this.latestID;
		}

		public void reloadStock((List<StardewValley.Object> stock, IDictionary<string, string> xstrings, IDictionary<string, Texture2D> textures, IDictionary<string, string> data)p)
        {
			(List<StardewValley.Object> stock, IDictionary<string, string> xstrings, IDictionary<string, Texture2D> textures, IDictionary<string, string> data) = p;
			strings = xstrings;
			pageMax = stock.Count / 9;
			animalsToPurchase.Clear();
			for (int j = 0; j <= pageMax; j++)
			{
				List<ClickableTextureComponent> page = new List<ClickableTextureComponent>();
				for (int i = 0 + j * 9; i < Math.Min(stock.Count, (j + 1) * 9); i++)
				{
					page.Add(new ClickableTextureComponent(string.Concat(stock[i].salePrice()), new Microsoft.Xna.Framework.Rectangle(base.xPositionOnScreen + IClickableMenu.borderWidth * 2 + i % 3 * 75 * 2, base.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth / 2 + (i % 9) / 3 * 90, 64, 64), null, stock[i].Name, textures[stock[i].Name], new Microsoft.Xna.Framework.Rectangle(1, 1, Convert.ToInt32(data[stock[i].Name].Split('/')[16]) - 1, Convert.ToInt32(data[stock[i].Name].Split('/')[17]) - 1), (Convert.ToInt32(data[stock[i].Name].Split('/')[16]) < 17) ? 4f : 2f, stock[i].Type == null)
					{
						item = stock[i],
						myID = i % 9,
						rightNeighborID = (((i % 9) % 3 == 2) ? (-1) : ((i % 9) + 1)),
						leftNeighborID = (((i % 9) % 3 == 0) ? (-1) : ((i % 9) - 1)),
						downNeighborID = (i % 9) + 3,
						upNeighborID = (i % 9) - 3
					});
				}
				animalsToPurchase.Add(page);
			}
			if (Game1.options.SnappyMenus)
			{
				base.populateClickableComponentList();
				this.snapToDefaultClickableComponent();
			}

		}

		public void makeAdult(FarmAnimal animal)
        {
			animal.age.Value = animal.ageWhenMature.Value;
			animal.Sprite.LoadTexture("Animals\\" + animal.type.Value);
			if (animal.type.Value.Contains("Sheep"))
			{
				animal.currentProduce.Value = animal.defaultProduceIndex;
			}
			animal.daysSinceLastLay.Value = 99;
			animal.reload(animal.home);
		}
	}
}
