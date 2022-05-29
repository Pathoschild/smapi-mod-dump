/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Traktori7/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Locations;
using StardewValley.Menus;
using SObject = StardewValley.Object;


namespace EndgameBundles
{
	internal class EndgameBundleMenu : IClickableMenu
	{
		private readonly IModHelper helper;
		private readonly IMonitor monitor;
		private readonly List<EndgameBundleSet> bundlesets;

		// Which bundle is open
		private int whichBundleSet;
		public List<EndgameBundlePage> bundlePages = new List<EndgameBundlePage>();

		public EndgameBundleIngredient? currentPartialIngredient;

		// JunimoNoteMenu variables
		public const int region_ingredientSlotModifier = 250;
		public const int region_ingredientListModifier = 1000;
		public const int region_bundleModifier = 5000;
		public const int region_areaNextButton = 101;
		public const int region_areaBackButton = 102;
		public const int region_backButton = 103;
		public const int region_purchaseButton = 104;
		public const int region_presentButton = 105;
		public const string noteTextureName = "LooseSprites\\JunimoNote";
		public const int baseWidth = 320;
		public const int baseHeight = 180;

		public Texture2D noteTexture;
		private bool specificBundlePage;
		public InventoryMenu inventory;
		public Item? partialDonationItem;
		public List<Item> partialDonationComponents = new List<Item>();
		
		public int currentPartialIngredientIndex = -1;
		private Item? heldItem;
		private Item? hoveredItem;
		public static bool canClick = true;
		
		public bool bundlesChanged;
		public static ScreenSwipe? screenSwipe;
		public static string hoverText = string.Empty;
		
		public static List<TemporaryAnimatedSprite> tempSprites = new List<TemporaryAnimatedSprite>();
		public List<ClickableTextureComponent> ingredientSlots = new List<ClickableTextureComponent>();
		public List<ClickableTextureComponent> ingredientList = new List<ClickableTextureComponent>();
		public List<ClickableTextureComponent> otherClickableComponents = new List<ClickableTextureComponent>();
		public bool fromGameMenu;
		public bool fromThisMenu;
		//public bool scrambledText;
		public ClickableTextureComponent backButton;
		public ClickableTextureComponent? purchaseButton;
		public ClickableTextureComponent? areaNextButton;
		public ClickableTextureComponent? areaBackButton;
		public ClickableAnimatedComponent? presentButton;
		private EndgameBundlePage? currentPageBundle;


		public EndgameBundleMenu(IModHelper modHelper, IMonitor monitor, List<EndgameBundleSet> bundlesets)
			: base(Game1.uiViewport.Width / 2 - baseWidth * 2, Game1.uiViewport.Height / 2 - baseHeight * 2, baseWidth * 4, baseHeight * 4, showUpperRightCloseButton: true)
		{
			helper = modHelper;
			this.monitor = monitor;
			this.bundlesets = bundlesets;

			SetUpMenu();
		}


		/*public EndgameBundleMenu(IModHelper modHelper, bool fromGameMenu, int area = 1, bool fromThisMenu = false)
			: base(Game1.uiViewport.Width / 2 - baseWidth * 2, Game1.uiViewport.Height / 2 - baseHeight * 2, baseWidth * 4, baseHeight * 4, showUpperRightCloseButton: true)
		{
			helper = modHelper;

			CommunityCenter cc = (Game1.getLocationFromName("CommunityCenter") as CommunityCenter)!;

			/*if (fromGameMenu && !fromThisMenu)
			{
				for (int j = 0; j < cc.areasComplete.Count; j++)
				{
					if (cc.shouldNoteAppearInArea(j) && !cc.areasComplete[j])
					{
						area = j;
						whichArea = area;
						break;
					}
				}

				if (Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow("abandonedJojaMartAccessible") && !Game1.MasterPlayer.hasOrWillReceiveMail("ccMovieTheater"))
				{
					area = 6;
				}
			}

			SetUpMenu(area, cc.bundlesDict());
			Game1.player.forceCanMove();

			areaNextButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width - 128, yPositionOnScreen, 48, 44), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 4f)
			{
				visible = false,
				myID = region_areaNextButton,
				leftNeighborID = region_areaBackButton,
				leftNeighborImmutable = true,
				downNeighborID = -99998
			};

			areaBackButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + 64, yPositionOnScreen, 48, 44), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), 4f)
			{
				visible = false,
				myID = region_areaBackButton,
				rightNeighborID = region_areaNextButton,
				rightNeighborImmutable = true,
				downNeighborID = -99998
			};

			int area_count = 6;

			for (int i = 0; i < area_count; i++)
			{
				if (i != area && cc.shouldNoteAppearInArea(i))
				{
					areaNextButton.visible = true;
					areaBackButton.visible = true;
					break;
				}
			}

			this.fromGameMenu = fromGameMenu;
			this.fromThisMenu = fromThisMenu;

			foreach (EndgameBundlePage bundle in bundles)
			{
				bundle.depositsAllowed = false;
			}

			if (Game1.options.SnappyMenus)
			{
				populateClickableComponentList();
				snapToDefaultClickableComponent();
			}
		}*/

		/*public EndgameBundleMenu(IModHelper modHelper, int whichArea, Dictionary<int, bool[]> bundlesComplete)
			: base(Game1.uiViewport.Width / 2 - 640, Game1.uiViewport.Height / 2 - 360, 1280, 720, showUpperRightCloseButton: true)
		{
			helper = modHelper;

			SetUpMenu(whichArea, bundlesComplete);

			if (Game1.options.SnappyMenus)
			{
				populateClickableComponentList();
				snapToDefaultClickableComponent();
			}
		}*/

		public override void snapToDefaultClickableComponent()
		{
			if (specificBundlePage)
			{
				currentlySnappedComponent = getComponentWithID(0);
			}
			else
			{
				currentlySnappedComponent = getComponentWithID(region_bundleModifier);
			}

			snapCursorToCurrentSnappedComponent();
		}

		protected override bool _ShouldAutoSnapPrioritizeAlignedElements()
		{
			if (specificBundlePage)
			{
				return false;
			}

			return true;
		}

		protected override void customSnapBehavior(int direction, int oldRegion, int oldID)
		{
			if (!Game1.player.hasOrWillReceiveMail("canReadJunimoText") || oldID - region_bundleModifier < 0 || oldID - region_bundleModifier >= 10 || currentlySnappedComponent is null)
			{
				return;
			}

			int lowestScoreBundle = -1;
			int lowestScore = 999999;
			Point startingPosition = currentlySnappedComponent.bounds.Center;

			for (int i = 0; i < bundlePages.Count; i++)
			{
				if (bundlePages[i].myID == oldID)
				{
					continue;
				}

				int score = 999999;
				Point bundlePosition = bundlePages[i].bounds.Center;

				switch (direction)
				{
					case 3:
						if (bundlePosition.X < startingPosition.X)
						{
							score = startingPosition.X - bundlePosition.X + Math.Abs(startingPosition.Y - bundlePosition.Y) * 3;
						}
						break;
					case 0:
						if (bundlePosition.Y < startingPosition.Y)
						{
							score = startingPosition.Y - bundlePosition.Y + Math.Abs(startingPosition.X - bundlePosition.X) * 3;
						}
						break;
					case 1:
						if (bundlePosition.X > startingPosition.X)
						{
							score = bundlePosition.X - startingPosition.X + Math.Abs(startingPosition.Y - bundlePosition.Y) * 3;
						}
						break;
					case 2:
						if (bundlePosition.Y > startingPosition.Y)
						{
							score = bundlePosition.Y - startingPosition.Y + Math.Abs(startingPosition.X - bundlePosition.X) * 3;
						}
						break;
				}

				if (score < 10000 && score < lowestScore)
				{
					lowestScore = score;
					lowestScoreBundle = i;
				}
			}

			if (lowestScoreBundle != -1)
			{
				currentlySnappedComponent = getComponentWithID(lowestScoreBundle + region_bundleModifier);
				snapCursorToCurrentSnappedComponent();
				return;
			}

			switch (direction)
			{
				case 2:
					if (presentButton is not null)
					{
						currentlySnappedComponent = presentButton;
						snapCursorToCurrentSnappedComponent();
						presentButton.upNeighborID = oldID;
					}
					break;
				case 3:
					if (areaBackButton is not null && areaBackButton.visible)
					{
						currentlySnappedComponent = areaBackButton;
						snapCursorToCurrentSnappedComponent();
						areaBackButton.rightNeighborID = oldID;
					}
					break;
				case 1:
					if (areaNextButton is not null && areaNextButton.visible)
					{
						currentlySnappedComponent = areaNextButton;
						snapCursorToCurrentSnappedComponent();
						areaNextButton.leftNeighborID = oldID;
					}
					break;
			}
		}


		[MemberNotNull(nameof(noteTexture), nameof(backButton), nameof(inventory))]
		public void SetUpMenu(int whichBundleSet = 0)
		{
			noteTexture = Game1.temporaryContent.Load<Texture2D>(noteTextureName);

			/*if (!Game1.player.hasOrWillReceiveMail("seenJunimoNote"))
			{
				Game1.player.removeQuest(26);
				Game1.player.mailReceived.Add("seenJunimoNote");
			}

			if (!Game1.player.hasOrWillReceiveMail("wizardJunimoNote"))
			{
				Game1.addMailForTomorrow("wizardJunimoNote");
			}

			if (!Game1.player.hasOrWillReceiveMail("hasSeenAbandonedJunimoNote") && whichArea == 6)
			{
				Game1.player.mailReceived.Add("hasSeenAbandonedJunimoNote");
			}*/

			//scrambledText = !Game1.player.hasOrWillReceiveMail("canReadJunimoText");

			tempSprites.Clear();
			this.whichBundleSet = whichBundleSet;

			inventory = new InventoryMenu(xPositionOnScreen + 128, yPositionOnScreen + 140, playerInventory: true, null, HighlightObjects, 36, 6, 8, 8, drawSlots: false)
			{
				// TODO: support for bigger backpacks? Or is it handled by the next part?
				capacity = 36
			};

			for (int i = 0; i < inventory.inventory.Count; i++)
			{
				if (i >= inventory.actualInventory.Count)
				{
					inventory.inventory[i].visible = false;
				}
			}

			foreach (ClickableComponent item in inventory.GetBorder(InventoryMenu.BorderSide.Bottom))
			{
				item.downNeighborID = -99998;
			}

			foreach (ClickableComponent item2 in inventory.GetBorder(InventoryMenu.BorderSide.Right))
			{
				item2.rightNeighborID = -99998;
			}

			inventory.dropItemInvisibleButton.visible = false;

			//Dictionary<string, string> bundlesInfo = Game1.netWorldState.Value.BundleData;
			//string areaName = CommunityCenter.getAreaNameFromNumber(whichBundle);

			int bundlesAdded = 0;

			EndgameBundleSet? bundleSet = null;

			try
			{
				bundleSet = bundlesets[whichBundleSet];
			}
			catch (Exception ex)
			{
				monitor.Log($"Failed getting the correct bundle set for bundle index {whichBundleSet}", LogLevel.Error);
				monitor.Log(ex.ToString(), LogLevel.Error);
			}

			if (bundleSet is not null)
			{
				for (int i = 0; i < bundleSet.Bundles.Count; i++)
				{
					bundlePages.Add(new EndgameBundlePage(i, bundleSet.Bundles[i], GetBundleLocationFromNumber(i), noteTextureName, this)
					{
						// TODO: Figure out what these do and what to change
						myID = bundlesAdded + region_bundleModifier,
						rightNeighborID = -7777,
						leftNeighborID = -7777,
						upNeighborID = -7777,
						downNeighborID = -7777,
						fullyImmutable = true
					});

					bundlesAdded++;
				}
			}

			/*foreach (string j in bundlesInfo.Keys)
			{
				if (j.Contains(areaName))
				{
					int bundleIndex = Convert.ToInt32(j.Split('/')[1]);
					
					bundlePages.Add(new EndgameBundlePage(bundleIndex, bundlesInfo[j], bundlesComplete[bundleIndex], GetBundleLocationFromNumber(bundlesAdded), "LooseSprites\\JunimoNote", this)
					{
						myID = bundlesAdded + region_bundleModifier,
						rightNeighborID = -7777,
						leftNeighborID = -7777,
						upNeighborID = -7777,
						downNeighborID = -7777,
						fullyImmutable = true
					});

					bundlesAdded++;
				}
			}*/

			backButton = new ClickableTextureComponent("Back", new Rectangle(xPositionOnScreen + borderWidth * 2 + 8, yPositionOnScreen + borderWidth * 2 + 4, 64, 64), null, null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44), 1f)
			{
				myID = region_backButton
			};

			CheckForRewards();
			canClick = true;
			Game1.playSound("shwip");
			bool isOneIncomplete = false;

			foreach (EndgameBundlePage b in bundlePages)
			{
				if (!b.complete && !b.Equals(currentPageBundle))
				{
					isOneIncomplete = true;
					break;
				}
			}

			if (!isOneIncomplete)
			{
				//((CommunityCenter)Game1.getLocationFromName("CommunityCenter")).markAreaAsComplete(whichBundle);
				exitFunction = RestoreAreaOnExit;
				//((CommunityCenter)Game1.getLocationFromName("CommunityCenter")).areaCompleteReward(whichBundle);
			}
		}

		public virtual bool HighlightObjects(Item item)
		{
			if (partialDonationItem is not null && currentPageBundle is not null && currentPartialIngredientIndex >= 0)
			{
				return currentPageBundle.IsValidItemForTheBundle(item, currentPageBundle.bundle.Ingredients[currentPartialIngredientIndex]);
			}

			return Utility.highlightSmallObjects(item);
		}

		public override bool readyToClose()
		{
			if (!specificBundlePage)
			{
				return IsReadyToCloseMenuOrBundle();
			}

			return false;
		}

		public override void receiveLeftClick(int x, int y, bool playSound = true)
		{
			if (!canClick)
			{
				return;
			}

			base.receiveLeftClick(x, y, playSound);

			// TODO: is this null check correct?
			if (specificBundlePage && currentPageBundle is not null)
			{
				if (!currentPageBundle.complete && currentPageBundle.completionTimer <= 0)
				{
					heldItem = inventory.leftClick(x, y, heldItem);
				}

				if (backButton.containsPoint(x, y) && heldItem is null)
				{
					CloseBundlePage();
				}

				if (partialDonationItem is not null)
				{
					if (heldItem is not null && helper.Input.IsDown(SButton.LeftShift))
					{
						for (int i = 0; i < ingredientSlots.Count; i++)
						{
							if (ingredientSlots[i].item == partialDonationItem)
							{
								HandlePartialDonation(heldItem, ingredientSlots[i]);
							}
						}
					}
					else
					{
						for (int l = 0; l < ingredientSlots.Count; l++)
						{
							if (ingredientSlots[l].containsPoint(x, y) && ingredientSlots[l].item == partialDonationItem)
							{
								if (heldItem is not null)
								{
									HandlePartialDonation(heldItem, ingredientSlots[l]);
									return;
								}

								bool return_to_inventory = helper.Input.IsDown(SButton.LeftShift);
								ReturnPartialDonations(!return_to_inventory);

								return;
							}
						}
					}
				}
				else if (heldItem is not null)
				{
					if (helper.Input.IsDown(SButton.LeftShift))
					{
						for (int k = 0; k < ingredientSlots.Count; k++)
						{
							if (currentPageBundle.CanAcceptThisItem(heldItem, ingredientSlots[k]))
							{
								if (ingredientSlots[k].item is null)
								{
									heldItem = currentPageBundle.TryToDepositThisItem(heldItem, ingredientSlots[k], "LooseSprites\\JunimoNote");
									CheckIfBundleIsComplete();

									return;
								}
							}
							else if (ingredientSlots[k].item is null)
							{
								HandlePartialDonation(heldItem, ingredientSlots[k]);
							}
						}
					}
					for (int j = 0; j < ingredientSlots.Count; j++)
					{
						if (ingredientSlots[j].containsPoint(x, y))
						{
							if (currentPageBundle.CanAcceptThisItem(heldItem, ingredientSlots[j]))
							{
								heldItem = currentPageBundle.TryToDepositThisItem(heldItem, ingredientSlots[j], "LooseSprites\\JunimoNote");
								CheckIfBundleIsComplete();
							}
							else if (ingredientSlots[j].item is null)
							{
								HandlePartialDonation(heldItem, ingredientSlots[j]);
							}
						}
					}
				}

				// TODO: Purchase button
				/*if (purchaseButton is not null && purchaseButton.containsPoint(x, y))
				{
					int moneyRequired = currentPageBundle.ingredients.Last().stack;

					if (Game1.player.Money >= moneyRequired)
					{
						Game1.player.Money -= moneyRequired;
						Game1.playSound("select");
						currentPageBundle.CompletionAnimation(this);

						if (purchaseButton is not null)
						{
							purchaseButton.scale = purchaseButton.baseScale * 0.75f;
						}

						((CommunityCenter)Game1.getLocationFromName("CommunityCenter")).bundleRewards[currentPageBundle.bundleIndex] = true;
						(Game1.getLocationFromName("CommunityCenter") as CommunityCenter)!.bundles.FieldDict[currentPageBundle.bundleIndex][0] = true;
						CheckForRewards();
						bool isOneIncomplete = false;

						foreach (EndgameBundlePage b2 in bundlePages)
						{
							if (!b2.complete && !b2.Equals(currentPageBundle))
							{
								isOneIncomplete = true;
								break;
							}
						}

						if (!isOneIncomplete)
						{
							((CommunityCenter)Game1.getLocationFromName("CommunityCenter")).markAreaAsComplete(whichBundle);
							exitFunction = RestoreAreaOnExit;
							((CommunityCenter)Game1.getLocationFromName("CommunityCenter")).areaCompleteReward(whichBundle);
						}
						else
						{
							((CommunityCenter)Game1.getLocationFromName("CommunityCenter")).getJunimoForArea(whichBundle)?.bringBundleBackToHut(EndgameBundlePage.GetColorFromColorIndex(currentPageBundle.bundleColor), Game1.getLocationFromName("CommunityCenter"));
						}

						//Game1.multiplayer.globalChatInfoMessage("Bundle");
					}
					else
					{
						Game1.dayTimeMoneyBox.moneyShakeTimer = 600;
					}
				}*/
				if (upperRightCloseButton is not null && IsReadyToCloseMenuOrBundle() && upperRightCloseButton.containsPoint(x, y))
				{
					CloseBundlePage();

					return;
				}
			}
			else
			{
				foreach (EndgameBundlePage b in bundlePages)
				{
					if (b.CanBeClicked() && b.containsPoint(x, y))
					{
						SetUpBundleSpecificPage(b);
						Game1.playSound("shwip");

						return;
					}
				}

				if (presentButton is not null && presentButton.containsPoint(x, y) && !fromGameMenu && !fromThisMenu)
				{
					OpenRewardsMenu();
				}

				if (fromGameMenu)
				{
					//Game1.getLocationFromName("CommunityCenter");

					if (areaNextButton is not null && areaNextButton.containsPoint(x, y))
					{
						SwapPage(1);
					}
					else if (areaBackButton is not null && areaBackButton.containsPoint(x, y))
					{
						SwapPage(-1);
					}
				}
			}

			if (heldItem is not null && !isWithinBounds(x, y) && heldItem.canBeTrashed())
			{
				Game1.playSound("throwDownITem");
				Game1.createItemDebris(heldItem, Game1.player.getStandingPosition(), Game1.player.FacingDirection);
				heldItem = null;
			}
		}

		public virtual void ReturnPartialDonation(Item item, bool play_sound = true)
		{
			List<Item> affected_items = new List<Item>();
			Item remainder = Game1.player.addItemToInventory(item, affected_items);

			foreach (Item affected_item in affected_items)
			{
				inventory.ShakeItem(affected_item);
			}

			if (remainder is not null)
			{
				Utility.CollectOrDrop(remainder);
				inventory.ShakeItem(remainder);
			}

			if (play_sound)
			{
				Game1.playSound("coin");
			}
		}

		public virtual void ReturnPartialDonations(bool to_hand = true)
		{
			if (partialDonationComponents.Count > 0)
			{
				bool play_sound = true;

				foreach (Item item in partialDonationComponents)
				{
					if (heldItem is null && to_hand)
					{
						Game1.playSound("dwop");
						heldItem = item;
					}
					else
					{
						ReturnPartialDonation(item, play_sound);
						play_sound = false;
					}
				}
			}

			ResetPartialDonation();
		}

		public virtual void ResetPartialDonation()
		{
			partialDonationComponents.Clear();
			currentPartialIngredient = null;
			currentPartialIngredientIndex = -1;

			foreach (ClickableTextureComponent slot in ingredientSlots)
			{
				if (slot.item == partialDonationItem)
				{
					slot.item = null;
				}
			}

			partialDonationItem = null;
		}


		public virtual bool CanBePartiallyOrFullyDonated([NotNullWhen(true)] Item? item)
		{
			if (currentPageBundle is null || item is null)
			{
				return false;
			}

			int index = currentPageBundle.GetBundleIngredientIndexForItem(item);

			if (index < 0)
			{
				return false;
			}

			//BundleIngredientDescription description = currentPageBundle.ingredients[index];
			EndgameBundleIngredient ingredient = currentPageBundle.bundle.Ingredients[index];
			int count = 0;

			if (currentPageBundle.IsValidItemForTheBundle(item, ingredient))
			{
				count += item.Stack;
			}

			foreach (Item inventory_item in Game1.player.Items)
			{
				if (currentPageBundle.IsValidItemForTheBundle(inventory_item, ingredient))
				{
					count += inventory_item.Stack;
				}
			}

			if (index == currentPartialIngredientIndex && partialDonationItem is not null)
			{
				count += partialDonationItem.Stack;
			}

			return count >= ingredient.Amount;
		}

		public virtual void HandlePartialDonation(Item? item, ClickableTextureComponent slot)
		{
			if (currentPageBundle is not null || (partialDonationItem is not null && slot.item != partialDonationItem) || !CanBePartiallyOrFullyDonated(item))
			{
				return;
			}

			if (currentPartialIngredient is null)
			{
				currentPartialIngredientIndex = currentPageBundle.GetBundleIngredientIndexForItem(item);

				if (currentPartialIngredientIndex != -1)
				{
					currentPartialIngredient = currentPageBundle.bundle.Ingredients[currentPartialIngredientIndex];
				}
			}

			if (currentPartialIngredient is null || !currentPageBundle.IsValidItemForTheBundle(item, currentPartialIngredient))
			{
				return;
			}

			bool play_sound = true;
			int amount_to_donate = 0;

			if (slot.item is null)
			{
				Game1.playSound("sell");
				play_sound = false;
				partialDonationItem = item.getOne();
				amount_to_donate = Math.Min(currentPartialIngredient.Amount, item.Stack);
				partialDonationItem.Stack = amount_to_donate;
				item.Stack -= amount_to_donate;

				if (partialDonationItem is SObject o)
				{
					o.Quality = currentPartialIngredient.Quality;
				}

				slot.item = partialDonationItem;
				slot.sourceRect.X = 512;
				slot.sourceRect.Y = 244;
			}
			else
			{
				amount_to_donate = Math.Min(currentPartialIngredient.Amount - partialDonationItem.Stack, item.Stack);
				partialDonationItem.Stack += amount_to_donate;
				item.Stack -= amount_to_donate;
			}

			if (amount_to_donate > 0)
			{
				Item donated_item = heldItem.getOne();
				donated_item.Stack = amount_to_donate;

				foreach (Item contributed_item in partialDonationComponents)
				{
					if (contributed_item.canStackWith(heldItem))
					{
						donated_item.Stack = contributed_item.addToStack(donated_item);
					}
				}

				if (donated_item.Stack > 0)
				{
					partialDonationComponents.Add(donated_item);
				}

				partialDonationComponents.Sort((Item a, Item b) => b.Stack.CompareTo(a.Stack));
			}

			if (item.Stack <= 0 && item == heldItem)
			{
				heldItem = null;
			}

			if (partialDonationItem.Stack >= currentPartialIngredient.Amount)
			{
				slot.item = null;
				partialDonationItem = currentPageBundle.TryToDepositThisItem(partialDonationItem, slot, noteTextureName);

				if (partialDonationItem is not null && partialDonationItem.Stack > 0)
				{
					ReturnPartialDonation(partialDonationItem);
				}

				partialDonationItem = null;
				ResetPartialDonation();
				CheckIfBundleIsComplete();
			}
			else if (amount_to_donate > 0 && play_sound)
			{
				Game1.playSound("sell");
			}
		}

		public bool IsReadyToCloseMenuOrBundle()
		{
			if (specificBundlePage && currentPageBundle is not null && currentPageBundle.completionTimer > 0)
			{
				return false;
			}

			if (heldItem is not null)
			{
				return false;
			}

			return true;
		}

		public override void receiveGamePadButton(Buttons b)
		{
			base.receiveGamePadButton(b);

			if (fromGameMenu && !specificBundlePage)
			{
				//Game1.getLocationFromName("CommunityCenter");

				switch (b)
				{
					case Buttons.RightTrigger:
						SwapPage(1);
						break;
					case Buttons.LeftTrigger:
						SwapPage(-1);
						break;
				}
			}
		}

		public void SwapPage(int direction)
		{
			/*
			if ((direction > 0 && !areaNextButton.visible) || (direction < 0 && !areaBackButton.visible))
			{
				return;
			}

			CommunityCenter cc = (Game1.getLocationFromName("CommunityCenter") as CommunityCenter)!;
			int area = whichArea;
			int area_count = 6;

			for (int i = 0; i < area_count; i++)
			{
				area += direction;

				if (area < 0)
				{
					area += area_count;
				}

				if (area >= area_count)
				{
					area -= area_count;
				}

				if (cc.shouldNoteAppearInArea(area))
				{
					int selected_id = -1;

					if (currentlySnappedComponent is not null && (currentlySnappedComponent.myID >= region_bundleModifier || currentlySnappedComponent.myID == region_areaNextButton || currentlySnappedComponent.myID == region_areaBackButton))
					{
						selected_id = currentlySnappedComponent.myID;
					}

					EndgameBundleMenu new_menu = (EndgameBundleMenu)(Game1.activeClickableMenu = new EndgameBundleMenu(helper, fromGameMenu: true, area, fromThisMenu: true));
					
					if (selected_id >= 0)
					{
						new_menu.currentlySnappedComponent = new_menu.getComponentWithID(currentlySnappedComponent.myID);
						new_menu.snapCursorToCurrentSnappedComponent();
					}

					if (new_menu.getComponentWithID(areaNextButton.leftNeighborID) is not null)
					{
						new_menu.areaNextButton.leftNeighborID = areaNextButton.leftNeighborID;
					}
					else
					{
						new_menu.areaNextButton.leftNeighborID = new_menu.areaBackButton.myID;
					}

					new_menu.areaNextButton.rightNeighborID = areaNextButton.rightNeighborID;
					new_menu.areaNextButton.upNeighborID = areaNextButton.upNeighborID;
					new_menu.areaNextButton.downNeighborID = areaNextButton.downNeighborID;
					
					if (new_menu.getComponentWithID(areaBackButton.rightNeighborID) is not null)
					{
						new_menu.areaBackButton.leftNeighborID = areaBackButton.leftNeighborID;
					}
					else
					{
						new_menu.areaBackButton.leftNeighborID = new_menu.areaNextButton.myID;
					}

					new_menu.areaBackButton.rightNeighborID = areaBackButton.rightNeighborID;
					new_menu.areaBackButton.upNeighborID = areaBackButton.upNeighborID;
					new_menu.areaBackButton.downNeighborID = areaBackButton.downNeighborID;

					break;
				}
			}*/
		}

		public override void receiveKeyPress(Keys key)
		{
			base.receiveKeyPress(key);

			if (key.Equals(Keys.Delete) && heldItem is not null && heldItem.canBeTrashed())
			{
				Utility.trashItem(heldItem);
				heldItem = null;
			}

			if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && IsReadyToCloseMenuOrBundle())
			{
				CloseBundlePage();
			}
		}

		private void CloseBundlePage()
		{
			if (partialDonationItem is not null)
			{
				ReturnPartialDonations(to_hand: false);
			}
			else if (specificBundlePage)
			{
				hoveredItem = null;
				inventory.descriptionText = "";

				if (heldItem is null)
				{
					TakeDownBundleSpecificPage(currentPageBundle);
					Game1.playSound("shwip");
				}
				else
				{
					heldItem = inventory.tryToAddItem(heldItem);
				}
			}
		}

		private void ReOpenThisMenu()
		{
			/*bool num = specificBundlePage;
			EndgameBundleMenu newMenu = (!fromGameMenu && !fromThisMenu)
				? new EndgameBundleMenu(helper, whichArea, ((CommunityCenter)Game1.getLocationFromName("CommunityCenter")).bundlesDict())
				: new EndgameBundleMenu(helper, fromGameMenu, whichArea, fromThisMenu);

			if (num)
			{
				foreach (EndgameBundlePage bundle in newMenu.bundles)
				{
					if (bundle.bundleIndex == currentPageBundle?.bundleIndex)
					{
						newMenu.SetUpBundleSpecificPage(bundle);
						break;
					}
				}
			}

			Game1.activeClickableMenu = newMenu;*/
		}

		private void UpdateIngredientSlots()
		{
			//new Dictionary<string, string>();
			int slotNumber = 0;

			for (int i = 0; i < currentPageBundle?.bundle.Ingredients.Count; i++)
			{
				if (currentPageBundle.bundle.Ingredients[i].Completed && slotNumber < ingredientSlots.Count)
				{
					int index = currentPageBundle.bundle.Ingredients[i].ItemID;

					if (index < 0)
					{
						index = GetObjectOrCategoryIndex(index);
					}

					ingredientSlots[slotNumber].item = new SObject(index, currentPageBundle.bundle.Ingredients[i].Amount, isRecipe: false, -1, currentPageBundle.bundle.Ingredients[i].Quality);
					currentPageBundle.IngredientDepositAnimation(ingredientSlots[slotNumber], "LooseSprites\\JunimoNote", skipAnimation: true);
					slotNumber++;
				}
			}
		}

		public static int GetObjectOrCategoryIndex(int category)
		{
			if (category < 0)
			{
				foreach (int key in Game1.objectInformation.Keys)
				{
					string item_data = Game1.objectInformation[key];

					if (item_data is not null)
					{
						string[] data = item_data.Split('/');

						if (data.Length > 3 && data[3].EndsWith(category.ToString()))
						{
							return key;
						}
					}
				}

				return category;
			}

			return category;
		}

		public static void GetBundleRewards(int area, List<Item> rewards)
		{
			Dictionary<string, string> bundlesInfo = Game1.netWorldState.Value.BundleData;

			foreach (string j in bundlesInfo.Keys)
			{
				if (j.Contains(CommunityCenter.getAreaNameFromNumber(area)))
				{
					int bundleIndex = Convert.ToInt32(j.Split('/')[1]);

					if (((CommunityCenter)Game1.getLocationFromName("CommunityCenter")).bundleRewards[bundleIndex])
					{
						Item i = Utility.getItemFromStandardTextDescription(bundlesInfo[j].Split('/')[1], Game1.player);
						i.SpecialVariable = bundleIndex;
						rewards.Add(i);
					}
				}
			}
		}

		private void OpenRewardsMenu()
		{
			Game1.playSound("smallSelect");
			List<Item> rewards = new List<Item>();
			GetBundleRewards(whichBundleSet, rewards);

			Game1.activeClickableMenu = new ItemGrabMenu(rewards, reverseGrab: false, showReceivingMenu: true, null, null, null, RewardGrabbed, snapToBottom: false, canBeExitedWithKey: true, playRightClickSound: true, allowRightClick: true, showOrganizeButton: false, 0, null, -1, this)
			{
				exitFunction = exitFunction ?? new onExit(ReOpenThisMenu)
			};
		}

		private void RewardGrabbed(Item item, Farmer who)
		{
			((CommunityCenter)Game1.getLocationFromName("CommunityCenter")).bundleRewards[item.SpecialVariable] = false;
		}


		private void CheckIfBundleIsComplete()
		{
			ReturnPartialDonations();

			if (!specificBundlePage || currentPageBundle is null)
			{
				return;
			}

			int numberOfFilledSlots = 0;

			foreach (ClickableTextureComponent c in ingredientSlots)
			{
				if (c.item is not null && c.item != partialDonationItem)
				{
					numberOfFilledSlots++;
				}
			}

			if (numberOfFilledSlots < currentPageBundle.numberOfIngredientSlots)
			{
				return;
			}

			if (heldItem is not null)
			{
				Game1.player.addItemToInventory(heldItem);
				heldItem = null;
			}

			for (int i = 0; i < ((CommunityCenter)Game1.getLocationFromName("CommunityCenter")).bundles[currentPageBundle.bundleIndex].Length; i++)
			{
				((CommunityCenter)Game1.getLocationFromName("CommunityCenter")).bundles.FieldDict[currentPageBundle.bundleIndex][i] = true;
			}

			((CommunityCenter)Game1.getLocationFromName("CommunityCenter")).checkForNewJunimoNotes();
			screenSwipe = new ScreenSwipe(0);
			currentPageBundle.CompletionAnimation(this, playSound: true, 400);
			canClick = false;
			((CommunityCenter)Game1.getLocationFromName("CommunityCenter")).bundleRewards[currentPageBundle.bundleIndex] = true;
			//Game1.multiplayer.globalChatInfoMessage("Bundle");
			bool isOneIncomplete = false;

			foreach (EndgameBundlePage b in bundlePages)
			{
				if (!b.complete && !b.Equals(currentPageBundle))
				{
					isOneIncomplete = true;
					break;
				}
			}

			if (!isOneIncomplete)
			{
				if (whichBundleSet == 6)
				{
					exitFunction = RestoreaAreaOnExit_AbandonedJojaMart;
				}
				else
				{
					//((CommunityCenter)Game1.getLocationFromName("CommunityCenter")).markAreaAsComplete(whichBundle);
					exitFunction = RestoreAreaOnExit;
					//((CommunityCenter)Game1.getLocationFromName("CommunityCenter")).areaCompleteReward(whichBundle);
				}
			}
			else
			{
				//((CommunityCenter)Game1.getLocationFromName("CommunityCenter")).getJunimoForArea(whichBundle)?.bringBundleBackToHut(EndgameBundlePage.GetColorFromColorIndex(currentPageBundle.bundleColor), Game1.getLocationFromName("CommunityCenter"));
			}

			CheckForRewards();
		}

		private void RestoreaAreaOnExit_AbandonedJojaMart()
		{
			//((AbandonedJojaMart)Game1.getLocationFromName("AbandonedJojaMart")).restoreAreaCutscene();
		}

		private void RestoreAreaOnExit()
		{
			if (!fromGameMenu)
			{
				//((CommunityCenter)Game1.getLocationFromName("CommunityCenter")).restoreAreaCutscene(whichBundle);
			}
		}

		public void CheckForRewards()
		{
			/*Dictionary<string, string> bundlesInfo = Game1.netWorldState.Value.BundleData;
			foreach (string i in bundlesInfo.Keys)
			{
				if (i.Contains(CommunityCenter.getAreaNameFromNumber(whichBundleSet)) && bundlesInfo[i].Split('/')[1].Length > 1)
				{
					int bundleIndex = Convert.ToInt32(i.Split('/')[1]);
					if (((CommunityCenter)Game1.getLocationFromName("CommunityCenter")).bundleRewards[bundleIndex])
					{
						presentButton = new ClickableAnimatedComponent(new Rectangle(xPositionOnScreen + 592, yPositionOnScreen + 512, 72, 72), "", Game1.content.LoadString("Strings\\StringsFromCSFiles:JunimoNoteMenu.cs.10783"), new TemporaryAnimatedSprite("LooseSprites\\JunimoNote", new Rectangle(548, 262, 18, 20), 70f, 4, 99999, new Vector2(-64f, -64f), flicker: false, flipped: false, 0.5f, 0f, Color.White, 4f, 0f, 0f, 0f, local: true));
						break;
					}
				}
			}*/

			// TODO: Not sure if this actually works how it should
			if (bundlesets[whichBundleSet].UnclaimedRewards.Count > 0)
			{
				presentButton = new ClickableAnimatedComponent(new Rectangle(xPositionOnScreen + 592, yPositionOnScreen + 512, 72, 72),
					"",
					Game1.content.LoadString("Strings\\StringsFromCSFiles:JunimoNoteMenu.cs.10783"),
					new TemporaryAnimatedSprite("LooseSprites\\JunimoNote", new Rectangle(548, 262, 18, 20),
					70f, 4, 99999, new Vector2(-64f, -64f), flicker: false, flipped: false, 0.5f, 0f, Color.White, 4f, 0f, 0f, 0f, local: true));
			}
		}

		public override void receiveRightClick(int x, int y, bool playSound = true)
		{
			if (!canClick)
			{
				return;
			}
			if (specificBundlePage)
			{
				heldItem = inventory.rightClick(x, y, heldItem);
				if (partialDonationItem is not null)
				{
					for (int i = 0; i < ingredientSlots.Count; i++)
					{
						if (!ingredientSlots[i].containsPoint(x, y) || ingredientSlots[i].item != partialDonationItem)
						{
							continue;
						}
						if (partialDonationComponents.Count <= 0)
						{
							break;
						}
						Item item = partialDonationComponents[0].getOne();
						bool valid = false;
						if (heldItem is null)
						{
							heldItem = item;
							Game1.playSound("dwop");
							valid = true;
						}
						else if (heldItem.canStackWith(item))
						{
							heldItem.addToStack(item);
							Game1.playSound("dwop");
							valid = true;
						}
						if (!valid)
						{
							break;
						}
						partialDonationComponents[0].Stack--;
						if (partialDonationComponents[0].Stack <= 0)
						{
							partialDonationComponents.RemoveAt(0);
						}
						int count = 0;
						foreach (Item contributed_item in partialDonationComponents)
						{
							count += contributed_item.Stack;
						}
						if (partialDonationItem is not null)
						{
							partialDonationItem.Stack = count;
						}
						if (partialDonationComponents.Count == 0)
						{
							ResetPartialDonation();
						}
						break;
					}
				}
			}
			if (!specificBundlePage && IsReadyToCloseMenuOrBundle())
			{
				exitThisMenu();
			}
		}

		public override void update(GameTime time)
		{
			if (specificBundlePage && currentPageBundle is not null && currentPageBundle.completionTimer <= 0 && IsReadyToCloseMenuOrBundle() && currentPageBundle.complete)
			{
				TakeDownBundleSpecificPage(currentPageBundle);
			}
			foreach (EndgameBundlePage bundle in bundlePages)
			{
				bundle.Update(time);
			}
			for (int i = tempSprites.Count - 1; i >= 0; i--)
			{
				if (tempSprites[i].update(time))
				{
					tempSprites.RemoveAt(i);
				}
			}
			if (presentButton is not null)
			{
				presentButton.update(time);
			}
			if (screenSwipe is not null)
			{
				canClick = false;
				if (screenSwipe.update(time))
				{
					screenSwipe = null;
					canClick = true;
				}
			}
			if (bundlesChanged && fromGameMenu)
			{
				ReOpenThisMenu();
			}
		}

		public override void performHoverAction(int x, int y)
		{
			base.performHoverAction(x, y);

			hoverText = "";

			if (specificBundlePage)
			{
				backButton.tryHover(x, y);

				if (currentPageBundle is not null && !currentPageBundle.complete && currentPageBundle.completionTimer <= 0)
				{
					hoveredItem = inventory.hover(x, y, heldItem);
				}
				else
				{
					hoveredItem = null;
				}

				foreach (ClickableTextureComponent c2 in ingredientList)
				{
					if (c2.bounds.Contains(x, y))
					{
						hoverText = c2.hoverText;
						break;
					}
				}

				if (heldItem is not null)
				{
					foreach (ClickableTextureComponent c in ingredientSlots)
					{
						if (c.bounds.Contains(x, y) && CanBePartiallyOrFullyDonated(heldItem) && (partialDonationItem is null || c.item == partialDonationItem))
						{
							c.sourceRect.X = 530;
							c.sourceRect.Y = 262;
						}
						else
						{
							c.sourceRect.X = 512;
							c.sourceRect.Y = 244;
						}
					}
				}

				if (purchaseButton is not null)
				{
					purchaseButton.tryHover(x, y);
				}

				return;
			}

			if (presentButton is not null)
			{
				hoverText = presentButton.tryHover(x, y);
			}

			foreach (EndgameBundlePage bundle in bundlePages)
			{
				bundle.TryHoverAction(x, y);
			}

			if (fromGameMenu)
			{
				//Game1.getLocationFromName("CommunityCenter");
				areaNextButton?.tryHover(x, y);
				areaBackButton?.tryHover(x, y);
			}
		}

		public override void draw(SpriteBatch b)
		{
			if (Game1.options.showMenuBackground)
			{
				base.drawBackground(b);
			}
			else
			{
				b.Draw(Game1.fadeToBlackRect, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), Color.Black * 0.5f);
			}

			if (!specificBundlePage)
			{
				// TODO: sort this out
				string someSortOfAreaDisplayName = CommunityCenter.getAreaEnglishDisplayNameFromNumber(whichBundleSet);

				b.Draw(noteTexture, new Vector2(xPositionOnScreen, yPositionOnScreen), new Rectangle(0, 0, 320, 180), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.1f);
				SpriteText.drawStringHorizontallyCenteredAt(b, someSortOfAreaDisplayName, xPositionOnScreen + width / 2 + 16, yPositionOnScreen + 12,alpha: 0.88f,layerDepth: 0.88f);
				
				/*if (scrambledText)
				{
					SpriteText.drawString(b, LocalizedContentManager.CurrentLanguageLatin ? Game1.content.LoadString("Strings\\StringsFromCSFiles:JunimoNoteMenu.cs.10786") : Game1.content.LoadBaseString("Strings\\StringsFromCSFiles:JunimoNoteMenu.cs.10786"), xPositionOnScreen + 96, yPositionOnScreen + 96, 999999, width - 192, 99999, 0.88f, 0.88f, junimoText: true);
					base.draw(b);

					if (!Game1.options.SnappyMenus && canClick)
					{
						drawMouse(b);
					}

					return;
				}*/

				foreach (EndgameBundlePage bundle in bundlePages)
				{
					bundle.Draw(b);
				}

				if (presentButton is not null)
				{
					presentButton.draw(b);
				}

				foreach (TemporaryAnimatedSprite tempSprite in tempSprites)
				{
					tempSprite.draw(b, localPosition: true);
				}

				if (fromGameMenu)
				{
					if (areaNextButton is not null && areaNextButton.visible)
					{
						areaNextButton.draw(b);
					}

					if (areaBackButton is not null && areaBackButton.visible)
					{
						areaBackButton.draw(b);
					}
				}
			}
			else
			{
				b.Draw(noteTexture, new Vector2(xPositionOnScreen, yPositionOnScreen), new Rectangle(320, 0, 320, 180), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.1f);
				
				if (currentPageBundle is not null)
				{
					int bundle_index = currentPageBundle.bundleIndex;
					Texture2D bundle_texture = noteTexture;
					int y_offset = 180;

					if (currentPageBundle.bundleTextureIndexOverride >= 0)
					{
						bundle_index = currentPageBundle.bundleTextureIndexOverride;
					}

					if (currentPageBundle.bundleTextureOverride is not null)
					{
						bundle_texture = currentPageBundle.bundleTextureOverride;
						y_offset = 0;
					}

					b.Draw(bundle_texture, new Vector2(xPositionOnScreen + 872, yPositionOnScreen + 88), new Rectangle(bundle_index * 16 * 2 % bundle_texture.Width, y_offset + 32 * (bundle_index * 16 * 2 / bundle_texture.Width), 32, 32), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.15f);
					float textX = Game1.dialogueFont.MeasureString(Game1.content.LoadString("Strings\\UI:JunimoNote_BundleName", currentPageBundle.label)).X;
					b.Draw(noteTexture, new Vector2(xPositionOnScreen + 936 - (int)textX / 2 - 16, yPositionOnScreen + 228), new Rectangle(517, 266, 4, 17), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.1f);
					b.Draw(noteTexture, new Rectangle(xPositionOnScreen + 936 - (int)textX / 2, yPositionOnScreen + 228, (int)textX, 68), new Rectangle(520, 266, 1, 17), Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.1f);
					b.Draw(noteTexture, new Vector2(xPositionOnScreen + 936 + (int)textX / 2, yPositionOnScreen + 228), new Rectangle(524, 266, 4, 17), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.1f);
					b.DrawString(Game1.dialogueFont, Game1.content.LoadString("Strings\\UI:JunimoNote_BundleName", currentPageBundle.label), new Vector2(xPositionOnScreen + 936 - textX / 2f, yPositionOnScreen + 236) + new Vector2(2f, 2f), Game1.textShadowColor);
					b.DrawString(Game1.dialogueFont, Game1.content.LoadString("Strings\\UI:JunimoNote_BundleName", currentPageBundle.label), new Vector2(xPositionOnScreen + 936 - textX / 2f, yPositionOnScreen + 236) + new Vector2(0f, 2f), Game1.textShadowColor);
					b.DrawString(Game1.dialogueFont, Game1.content.LoadString("Strings\\UI:JunimoNote_BundleName", currentPageBundle.label), new Vector2(xPositionOnScreen + 936 - textX / 2f, yPositionOnScreen + 236) + new Vector2(2f, 0f), Game1.textShadowColor);
					b.DrawString(Game1.dialogueFont, Game1.content.LoadString("Strings\\UI:JunimoNote_BundleName", currentPageBundle.label), new Vector2(xPositionOnScreen + 936 - textX / 2f, yPositionOnScreen + 236), Game1.textColor * 0.9f);
				}

				backButton.draw(b);

				if (purchaseButton is not null)
				{
					purchaseButton.draw(b);
					Game1.dayTimeMoneyBox.drawMoneyBox(b);
				}

				float completed_slot_alpha = 1f;

				if (partialDonationItem is not null)
				{
					completed_slot_alpha = 0.25f;
				}

				foreach (TemporaryAnimatedSprite tempSprite2 in tempSprites)
				{
					tempSprite2.draw(b, localPosition: true, 0, 0, completed_slot_alpha);
				}

				foreach (ClickableTextureComponent c2 in ingredientSlots)
				{
					float alpha_mult2 = 1f;

					if (partialDonationItem is not null && c2.item != partialDonationItem)
					{
						alpha_mult2 = 0.25f;
					}

					if (c2.item is null || (partialDonationItem is not null && c2.item == partialDonationItem))
					{
						c2.draw(b, (fromGameMenu ? (Color.LightGray * 0.5f) : Color.White) * alpha_mult2, 0.89f);
					}

					c2.drawItem(b, 4, 4, alpha_mult2);
				}

				for (int i = 0; i < ingredientList.Count; i++)
				{
					float alpha_mult = 1f;

					if (currentPartialIngredientIndex >= 0 && currentPartialIngredientIndex != i)
					{
						alpha_mult = 0.25f;
					}

					ClickableTextureComponent c = ingredientList[i];
					bool completed = false;
					//_ = currentPageBundle.bundleColor;

					if (currentPageBundle is not null && currentPageBundle.bundle.Ingredients is not null && i < currentPageBundle.bundle.Ingredients.Count && currentPageBundle.bundle.Ingredients[i].Completed)
					{
						completed = true;
					}

					if (!completed)
					{
						b.Draw(Game1.shadowTexture, new Vector2(c.bounds.Center.X - Game1.shadowTexture.Bounds.Width * 4 / 2 - 4, c.bounds.Center.Y + 4), Game1.shadowTexture.Bounds, Color.White * alpha_mult, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.1f);
					}

					if (c.item is not null && c.visible)
					{
						c.item.drawInMenu(b, new Vector2(c.bounds.X, c.bounds.Y), c.scale / 4f, 1f, 0.9f, StackDrawType.Draw, Color.White * (completed ? 0.25f : alpha_mult), drawShadow: false);
					}
				}

				inventory.draw(b);
			}

			SpriteText.drawStringWithScrollCenteredAt(b, GetRewardNameForArea(whichBundleSet), xPositionOnScreen + width / 2, Math.Min(yPositionOnScreen + height + 20, Game1.uiViewport.Height - 64 - 8));
			base.draw(b);
			Game1.mouseCursorTransparency = 1f;

			if (canClick)
			{
				drawMouse(b);
			}

			if (heldItem is not null)
			{
				heldItem.drawInMenu(b, new Vector2(Game1.getOldMouseX() + 16, Game1.getOldMouseY() + 16), 1f);
			}

			if (inventory.descriptionText.Length > 0)
			{
				if (hoveredItem is not null)
				{
					drawToolTip(b, hoveredItem.getDescription(), hoveredItem.DisplayName, hoveredItem);
				}
			}
			else
			{
				drawHoverText(b, (hoverText.Length > 0) ? "???" : hoverText, Game1.dialogueFont);
			}

			if (screenSwipe is not null)
			{
				screenSwipe.draw(b);
			}
		}

		public string GetRewardNameForArea(int whichArea)
		{
			return whichArea switch
			{
				3 => Game1.content.LoadString("Strings\\UI:JunimoNote_RewardBoiler"),
				5 => Game1.content.LoadString("Strings\\UI:JunimoNote_RewardBulletin"),
				1 => Game1.content.LoadString("Strings\\UI:JunimoNote_RewardCrafts"),
				0 => Game1.content.LoadString("Strings\\UI:JunimoNote_RewardPantry"),
				4 => Game1.content.LoadString("Strings\\UI:JunimoNote_RewardVault"),
				2 => Game1.content.LoadString("Strings\\UI:JunimoNote_RewardFishTank"),
				_ => "???",
			};
		}

		public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
		{
			base.gameWindowSizeChanged(oldBounds, newBounds);
			xPositionOnScreen = Game1.uiViewport.Width / 2 - 640;
			yPositionOnScreen = Game1.uiViewport.Height / 2 - 360;
			backButton = new ClickableTextureComponent("Back", new Rectangle(xPositionOnScreen + borderWidth * 2 + 8, yPositionOnScreen + borderWidth * 2 + 4, 64, 64), null, null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44), 1f);
			
			if (fromGameMenu)
			{
				areaNextButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width - 128, yPositionOnScreen, 48, 44), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 4f)
				{
					visible = false
				};
				areaBackButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + 64, yPositionOnScreen, 48, 44), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), 4f)
				{
					visible = false
				};
			}

			inventory = new InventoryMenu(xPositionOnScreen + 128, yPositionOnScreen + 140, playerInventory: true, null, HighlightObjects, Game1.player.maxItems.Value, 6, 8, 8, drawSlots: false);
			
			for (int l = 0; l < inventory.inventory.Count; l++)
			{
				if (l >= inventory.actualInventory.Count)
				{
					inventory.inventory[l].visible = false;
				}
			}

			for (int k = 0; k < bundlePages.Count; k++)
			{
				Point p = GetBundleLocationFromNumber(k);
				bundlePages[k].bounds.X = p.X;
				bundlePages[k].bounds.Y = p.Y;
				bundlePages[k].sprite.position = new Vector2(p.X, p.Y);
			}

			if (!specificBundlePage || currentPageBundle is null)
			{
				return;
			}

			int numberOfIngredientSlots = currentPageBundle?.numberOfIngredientSlots ?? 0;
			List<Rectangle> ingredientSlotRectangles = new List<Rectangle>();
			AddRectangleRowsToList(ingredientSlotRectangles, numberOfIngredientSlots, 932, 540);
			ingredientSlots.Clear();

			for (int j = 0; j < ingredientSlotRectangles.Count; j++)
			{
				ingredientSlots.Add(new ClickableTextureComponent(ingredientSlotRectangles[j], noteTexture, new Rectangle(512, 244, 18, 18), 4f));
			}

			List<Rectangle> ingredientListRectangles = new List<Rectangle>();
			ingredientList.Clear();
			AddRectangleRowsToList(ingredientListRectangles, currentPageBundle?.bundle.Ingredients.Count ?? 0, 932, 364);

			for (int i = 0; i < ingredientListRectangles.Count; i++)
			{
				if (Game1.objectInformation.ContainsKey(currentPageBundle.bundle.Ingredients[i].ItemID))
				{
					ingredientList.Add(new ClickableTextureComponent("", ingredientListRectangles[i], "", Game1.objectInformation[currentPageBundle.bundle.Ingredients[i].ItemID].Split('/')[0], Game1.objectSpriteSheet, Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, currentPageBundle.bundle.Ingredients[i].ItemID, 16, 16), 4f)
					{
						myID = i + region_ingredientListModifier,
						item = new SObject(currentPageBundle.bundle.Ingredients[i].ItemID, currentPageBundle.bundle.Ingredients[i].Amount, isRecipe: false, -1, currentPageBundle.bundle.Ingredients[i].Quality),
						upNeighborID = -99998,
						rightNeighborID = -99998,
						leftNeighborID = -99998,
						downNeighborID = -99998
					});
				}
			}

			UpdateIngredientSlots();
		}


		[MemberNotNull(nameof(currentPageBundle))]
		private void SetUpBundleSpecificPage(EndgameBundlePage b)
		{
			tempSprites.Clear();
			currentPageBundle = b;
			specificBundlePage = true;

			if (whichBundleSet == 4)
			{
				if (!fromGameMenu)
				{
					purchaseButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + 800, yPositionOnScreen + 504, 260, 72), noteTexture, new Rectangle(517, 286, 65, 20), 4f)
					{
						myID = 797,
						leftNeighborID = region_backButton
					};

					if (Game1.options.SnappyMenus)
					{
						currentlySnappedComponent = purchaseButton;
						snapCursorToCurrentSnappedComponent();
					}
				}

				return;
			}

			int numberOfIngredientSlots = b.numberOfIngredientSlots;
			List<Rectangle> ingredientSlotRectangles = new List<Rectangle>();
			AddRectangleRowsToList(ingredientSlotRectangles, numberOfIngredientSlots, 932, 540);

			for (int k = 0; k < ingredientSlotRectangles.Count; k++)
			{
				ingredientSlots.Add(new ClickableTextureComponent(ingredientSlotRectangles[k], noteTexture, new Rectangle(512, 244, 18, 18), 4f)
				{
					myID = k + region_ingredientSlotModifier,
					upNeighborID = -99998,
					rightNeighborID = -99998,
					leftNeighborID = -99998,
					downNeighborID = -99998
				});
			}

			List<Rectangle> ingredientListRectangles = new List<Rectangle>();
			AddRectangleRowsToList(ingredientListRectangles, b.bundle.Ingredients.Count, 932, 364);

			for (int j = 0; j < ingredientListRectangles.Count; j++)
			{
				int itemID = b.bundle.Ingredients[j].ItemID;

				if (itemID < 0)
				{
					itemID = GetObjectOrCategoryIndex(itemID);
				}

				if (!Game1.objectInformation.ContainsKey(itemID))
				{
					continue;
				}

				string displayName = Game1.objectInformation[itemID].Split('/')[4];

				// TODO: Use the same thing used in Categories in Recipes to support other categories easily
				if (itemID < 0)
				{
					if (itemID == -2)
					{
						displayName = Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.569");
					}
					else if (itemID == -75)
					{
						displayName = Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.570");
					}
					else if (itemID == -4)
					{
						displayName = Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.571");
					}
					else if (itemID == -5)
					{
						displayName = Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.572");
					}
					else if (itemID == -6)
					{
						displayName = Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.573");
					}
				}

				ingredientList.Add(new ClickableTextureComponent("ingredient_list_slot", ingredientListRectangles[j], "", displayName, Game1.objectSpriteSheet, Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, b.bundle.Ingredients[j].ItemID, 16, 16), 4f)
				{
					myID = j + region_ingredientListModifier,
					item = new SObject(itemID, b.bundle.Ingredients[j].Amount, isRecipe: false, -1, b.bundle.Ingredients[j].Quality),
					upNeighborID = -99998,
					rightNeighborID = -99998,
					leftNeighborID = -99998,
					downNeighborID = -99998
				});
			}

			UpdateIngredientSlots();

			if (!Game1.options.SnappyMenus)
			{
				return;
			}

			populateClickableComponentList();

			if (inventory is not null && inventory.inventory is not null)
			{
				for (int i = 0; i < inventory.inventory.Count; i++)
				{
					if (inventory.inventory[i] is not null)
					{
						if (inventory.inventory[i].downNeighborID == region_areaNextButton)
						{
							inventory.inventory[i].downNeighborID = -1;
						}

						if (inventory.inventory[i].leftNeighborID == -1)
						{
							inventory.inventory[i].leftNeighborID = region_backButton;
						}

						if (inventory.inventory[i].upNeighborID >= region_ingredientListModifier)
						{
							inventory.inventory[i].upNeighborID = region_backButton;
						}
					}
				}
			}

			currentlySnappedComponent = getComponentWithID(0);
			snapCursorToCurrentSnappedComponent();
		}

		public override bool IsAutomaticSnapValid(int direction, ClickableComponent a, ClickableComponent b)
		{
			if (currentPartialIngredientIndex >= 0)
			{
				if (ingredientSlots.Contains(b) && b.item != partialDonationItem)
				{
					return false;
				}

				if (ingredientList.Contains(b) && ingredientList.IndexOf((b as ClickableTextureComponent)!) != currentPartialIngredientIndex)
				{
					return false;
				}
			}

			return (a.myID >= region_bundleModifier || a.myID == region_areaNextButton || a.myID == region_areaBackButton) == (b.myID >= region_bundleModifier || b.myID == region_areaNextButton || b.myID == region_areaBackButton);
		}

		private void AddRectangleRowsToList(List<Rectangle> toAddTo, int numberOfItems, int centerX, int centerY)
		{
			switch (numberOfItems)
			{
				case 1:
					toAddTo.AddRange(CreateRowOfBoxesCenteredAt(xPositionOnScreen + centerX, yPositionOnScreen + centerY, 1, 72, 72, 12));
					break;
				case 2:
					toAddTo.AddRange(CreateRowOfBoxesCenteredAt(xPositionOnScreen + centerX, yPositionOnScreen + centerY, 2, 72, 72, 12));
					break;
				case 3:
					toAddTo.AddRange(CreateRowOfBoxesCenteredAt(xPositionOnScreen + centerX, yPositionOnScreen + centerY, 3, 72, 72, 12));
					break;
				case 4:
					toAddTo.AddRange(CreateRowOfBoxesCenteredAt(xPositionOnScreen + centerX, yPositionOnScreen + centerY, 4, 72, 72, 12));
					break;
				case 5:
					toAddTo.AddRange(CreateRowOfBoxesCenteredAt(xPositionOnScreen + centerX, yPositionOnScreen + centerY - 36, 3, 72, 72, 12));
					toAddTo.AddRange(CreateRowOfBoxesCenteredAt(xPositionOnScreen + centerX, yPositionOnScreen + centerY + 40, 2, 72, 72, 12));
					break;
				case 6:
					toAddTo.AddRange(CreateRowOfBoxesCenteredAt(xPositionOnScreen + centerX, yPositionOnScreen + centerY - 36, 3, 72, 72, 12));
					toAddTo.AddRange(CreateRowOfBoxesCenteredAt(xPositionOnScreen + centerX, yPositionOnScreen + centerY + 40, 3, 72, 72, 12));
					break;
				case 7:
					toAddTo.AddRange(CreateRowOfBoxesCenteredAt(xPositionOnScreen + centerX, yPositionOnScreen + centerY - 36, 4, 72, 72, 12));
					toAddTo.AddRange(CreateRowOfBoxesCenteredAt(xPositionOnScreen + centerX, yPositionOnScreen + centerY + 40, 3, 72, 72, 12));
					break;
				case 8:
					toAddTo.AddRange(CreateRowOfBoxesCenteredAt(xPositionOnScreen + centerX, yPositionOnScreen + centerY - 36, 4, 72, 72, 12));
					toAddTo.AddRange(CreateRowOfBoxesCenteredAt(xPositionOnScreen + centerX, yPositionOnScreen + centerY + 40, 4, 72, 72, 12));
					break;
				case 9:
					toAddTo.AddRange(CreateRowOfBoxesCenteredAt(xPositionOnScreen + centerX, yPositionOnScreen + centerY - 36, 5, 72, 72, 12));
					toAddTo.AddRange(CreateRowOfBoxesCenteredAt(xPositionOnScreen + centerX, yPositionOnScreen + centerY + 40, 4, 72, 72, 12));
					break;
				case 10:
					toAddTo.AddRange(CreateRowOfBoxesCenteredAt(xPositionOnScreen + centerX, yPositionOnScreen + centerY - 36, 5, 72, 72, 12));
					toAddTo.AddRange(CreateRowOfBoxesCenteredAt(xPositionOnScreen + centerX, yPositionOnScreen + centerY + 40, 5, 72, 72, 12));
					break;
				case 11:
					toAddTo.AddRange(CreateRowOfBoxesCenteredAt(xPositionOnScreen + centerX, yPositionOnScreen + centerY - 36, 6, 72, 72, 12));
					toAddTo.AddRange(CreateRowOfBoxesCenteredAt(xPositionOnScreen + centerX, yPositionOnScreen + centerY + 40, 5, 72, 72, 12));
					break;
				case 12:
					toAddTo.AddRange(CreateRowOfBoxesCenteredAt(xPositionOnScreen + centerX, yPositionOnScreen + centerY - 36, 6, 72, 72, 12));
					toAddTo.AddRange(CreateRowOfBoxesCenteredAt(xPositionOnScreen + centerX, yPositionOnScreen + centerY + 40, 6, 72, 72, 12));
					break;
			}
		}

		private List<Rectangle> CreateRowOfBoxesCenteredAt(int xStart, int yStart, int numBoxes, int boxWidth, int boxHeight, int horizontalGap)
		{
			List<Rectangle> rectangles = new List<Rectangle>();
			int actualXStart = xStart - numBoxes * (boxWidth + horizontalGap) / 2;
			int actualYStart = yStart - boxHeight / 2;

			for (int i = 0; i < numBoxes; i++)
			{
				rectangles.Add(new Rectangle(actualXStart + i * (boxWidth + horizontalGap), actualYStart, boxWidth, boxHeight));
			}

			return rectangles;
		}

		public void TakeDownBundleSpecificPage(EndgameBundlePage? b = null)
		{
			if (!IsReadyToCloseMenuOrBundle())
			{
				return;
			}

			ReturnPartialDonations(to_hand: false);
			hoveredItem = null;

			if (!specificBundlePage)
			{
				return;
			}

			if (b is null)
			{
				b = currentPageBundle;
			}

			specificBundlePage = false;
			ingredientSlots.Clear();
			ingredientList.Clear();
			tempSprites.Clear();
			purchaseButton = null;

			if (Game1.options.SnappyMenus)
			{
				if (currentPageBundle is not null)
				{
					currentlySnappedComponent = currentPageBundle;
					snapCursorToCurrentSnappedComponent();
				}
				else
				{
					snapToDefaultClickableComponent();
				}
			}
		}

		private Point GetBundleLocationFromNumber(int whichBundle)
		{
			Point location = new Point(xPositionOnScreen, yPositionOnScreen);

			switch (whichBundle)
			{
				case 0:
					location.X += 592;
					location.Y += 136;
					break;
				case 1:
					location.X += 392;
					location.Y += 384;
					break;
				case 2:
					location.X += 784;
					location.Y += 388;
					break;
				case 5:
					location.X += 588;
					location.Y += 276;
					break;
				case 6:
					location.X += 588;
					location.Y += 380;
					break;
				case 3:
					location.X += 304;
					location.Y += 252;
					break;
				case 4:
					location.X += 892;
					location.Y += 252;
					break;
				case 7:
					location.X += 440;
					location.Y += 164;
					break;
				case 8:
					location.X += 776;
					location.Y += 164;
					break;
			}

			return location;
		}
	}
}
