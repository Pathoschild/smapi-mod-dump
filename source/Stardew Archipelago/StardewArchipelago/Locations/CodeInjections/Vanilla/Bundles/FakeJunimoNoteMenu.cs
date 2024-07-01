/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Extensions;
using StardewValley.Locations;
using StardewValley.Menus;
// ReSharper disable InconsistentNaming
// ReSharper disable FieldCanBeMadeReadOnly.Local
// ReSharper disable VirtualMemberCallInConstructor
// ReSharper disable InvertIf
// ReSharper disable ConvertIfStatementToConditionalTernaryExpression
// ReSharper disable ParameterHidesMember

namespace StardewArchipelago.Locations.CodeInjections.Vanilla.Bundles
{
    public class FakeJunimoNoteMenu : IClickableMenu
    {
        public const int region_ingredientSlotModifier = 250;
        public const int region_ingredientListModifier = 1000;
        public const int region_bundleModifier = 5000;
        public const int region_areaNextButton = 101;
        public const int region_areaBackButton = 102;
        public const int region_backButton = 103;
        public const int region_purchaseButton = 104;
        public const int region_presentButton = 105;
        public const string noteTextureName = "LooseSprites\\JunimoNote";
        public Texture2D noteTexture;
        public bool specificBundlePage;
        public const int baseWidth = 320;
        public const int baseHeight = 180;
        public InventoryMenu inventory;
        public Item partialDonationItem;
        public List<Item> partialDonationComponents = new();
        public BundleIngredientDescription? currentPartialIngredientDescription;
        public int currentPartialIngredientDescriptionIndex = -1;
        public Item heldItem;
        public Item hoveredItem;
        public static bool canClick = true;
        public int whichArea;
        public int gameMenuTabToReturnTo = -1;
        public IClickableMenu menuToReturnTo;
        public bool bundlesChanged;
        public static ScreenSwipe screenSwipe;
        public static string hoverText = "";
        public List<Bundle> bundles = new();
        public static TemporaryAnimatedSpriteList tempSprites = new();
        public List<ClickableTextureComponent> ingredientSlots = new();
        public List<ClickableTextureComponent> ingredientList = new();
        public bool fromGameMenu;
        public bool fromThisMenu;
        public bool scrambledText;
        private bool singleBundleMenu;
        public ClickableTextureComponent backButton;
        public ClickableTextureComponent purchaseButton;
        public ClickableTextureComponent areaNextButton;
        public ClickableTextureComponent areaBackButton;
        public ClickableAnimatedComponent presentButton;
        public Action<int> onIngredientDeposit;
        public Action<FakeJunimoNoteMenu> onBundleComplete;
        public Action<FakeJunimoNoteMenu> onScreenSwipeFinished;
        public Bundle currentPageBundle;

        //public FakeJunimoNoteMenu(bool fromGameMenu, int area = 1, bool fromThisMenu = false)
        //  : base(Game1.uiViewport.Width / 2 - 640, Game1.uiViewport.Height / 2 - 360, 1280, 720, true)
        //{
        //    var communityCenter = Game1.RequireLocation<CommunityCenter>("CommunityCenter");
        //    if (fromGameMenu && !fromThisMenu)
        //    {
        //        for (var index = 0; index < communityCenter.areasComplete.Count; ++index)
        //        {
        //            if (communityCenter.shouldNoteAppearInArea(index) && !communityCenter.areasComplete[index])
        //            {
        //                area = index;
        //                this.whichArea = area;
        //                break;
        //            }
        //        }
        //        if (Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow("abandonedJojaMartAccessible") && !Game1.MasterPlayer.hasOrWillReceiveMail("ccMovieTheater"))
        //        {
        //            area = 6;
        //        }
        //    }
        //    this.setUpMenu(area, communityCenter.bundlesDict());
        //    Game1.player.forceCanMove();
        //    var textureComponent1 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width - 128, this.yPositionOnScreen, 48, 44), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 4f)
        //    {
        //        visible = false,
        //        myID = 101,
        //        leftNeighborID = 102,
        //        leftNeighborImmutable = true,
        //        downNeighborID = -99998,
        //    };
        //    this.areaNextButton = textureComponent1;
        //    var textureComponent2 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + 64, this.yPositionOnScreen, 48, 44), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), 4f)
        //    {
        //        visible = false,
        //        myID = 102,
        //        rightNeighborID = 101,
        //        rightNeighborImmutable = true,
        //        downNeighborID = -99998,
        //    };
        //    this.areaBackButton = textureComponent2;
        //    var num = 6;
        //    for (var area1 = 0; area1 < num; ++area1)
        //    {
        //        if (area1 != area && communityCenter.shouldNoteAppearInArea(area1))
        //        {
        //            this.areaNextButton.visible = true;
        //            this.areaBackButton.visible = true;
        //            break;
        //        }
        //    }
        //    this.fromGameMenu = fromGameMenu;
        //    this.fromThisMenu = fromThisMenu;
        //    foreach (var bundle in this.bundles)
        //    {
        //        bundle.depositsAllowed = false;
        //    }
        //    if (!Game1.options.SnappyMenus)
        //    {
        //        return;
        //    }
        //    this.populateClickableComponentList();
        //    this.snapToDefaultClickableComponent();
        //}

        //public FakeJunimoNoteMenu(int whichArea, Dictionary<int, bool[]> bundlesComplete)
        //  : base(Game1.uiViewport.Width / 2 - 640, Game1.uiViewport.Height / 2 - 360, 1280, 720, true)
        //{
        //    this.setUpMenu(whichArea, bundlesComplete);
        //    if (!Game1.options.SnappyMenus)
        //    {
        //        return;
        //    }
        //    this.populateClickableComponentList();
        //    this.snapToDefaultClickableComponent();
        //}

        public FakeJunimoNoteMenu(Bundle b, string noteTexturePath)
          : base(Game1.uiViewport.Width / 2 - 640, Game1.uiViewport.Height / 2 - 360, 1280, 720, true)
        {
            this.singleBundleMenu = true;
            this.whichArea = -1;
            this.noteTexture = Game1.temporaryContent.Load<Texture2D>(noteTexturePath);
            tempSprites.Clear();
            this.inventory = new InventoryMenu(this.xPositionOnScreen + 128, this.yPositionOnScreen + 140, true, highlightMethod: this.HighlightObjects, capacity: 36, rows: 6, horizontalGap: 8, verticalGap: 8, drawSlots: false)
            {
                capacity = 36
            };
            for (var index = 0; index < this.inventory.inventory.Count; ++index)
            {
                if (index >= this.inventory.actualInventory.Count)
                {
                    this.inventory.inventory[index].visible = false;
                }
            }
            foreach (var clickableComponent in this.inventory.GetBorder(InventoryMenu.BorderSide.Bottom))
            {
                clickableComponent.downNeighborID = -99998;
            }
            foreach (var clickableComponent in this.inventory.GetBorder(InventoryMenu.BorderSide.Right))
            {
                clickableComponent.rightNeighborID = -99998;
            }
            this.inventory.dropItemInvisibleButton.visible = false;
            canClick = true;
            this.setUpBundleSpecificPage(b);
            if (!Game1.options.SnappyMenus)
            {
                return;
            }
            this.populateClickableComponentList();
            this.snapToDefaultClickableComponent();
        }

        public override void snapToDefaultClickableComponent()
        {
            if (this.specificBundlePage)
            {
                this.currentlySnappedComponent = this.getComponentWithID(0);
            }
            else
            {
                this.currentlySnappedComponent = this.getComponentWithID(5000);
            }
            this.snapCursorToCurrentSnappedComponent();
        }

        protected override bool _ShouldAutoSnapPrioritizeAlignedElements() => !this.specificBundlePage;

        protected override void customSnapBehavior(int direction, int oldRegion, int oldID)
        {
            if (!Game1.player.hasOrWillReceiveMail("canReadJunimoText") || oldID - 5000 < 0 || oldID - 5000 >= 10 || this.currentlySnappedComponent == null)
            {
                return;
            }
            var num1 = -1;
            var num2 = 999999;
            var center1 = this.currentlySnappedComponent.bounds.Center;
            for (var index = 0; index < this.bundles.Count; ++index)
            {
                if (this.bundles[index].myID != oldID)
                {
                    var num3 = 999999;
                    var center2 = this.bundles[index].bounds.Center;
                    switch (direction)
                    {
                        case 0:
                            if (center2.Y < center1.Y)
                            {
                                num3 = center1.Y - center2.Y + Math.Abs(center1.X - center2.X) * 3;
                            }
                            break;
                        case 1:
                            if (center2.X > center1.X)
                            {
                                num3 = center2.X - center1.X + Math.Abs(center1.Y - center2.Y) * 3;
                            }
                            break;
                        case 2:
                            if (center2.Y > center1.Y)
                            {
                                num3 = center2.Y - center1.Y + Math.Abs(center1.X - center2.X) * 3;
                            }
                            break;
                        case 3:
                            if (center2.X < center1.X)
                            {
                                num3 = center1.X - center2.X + Math.Abs(center1.Y - center2.Y) * 3;
                            }
                            break;
                    }
                    if (num3 < 10000 && num3 < num2)
                    {
                        num2 = num3;
                        num1 = index;
                    }
                }
            }
            if (num1 != -1)
            {
                this.currentlySnappedComponent = this.getComponentWithID(num1 + 5000);
                this.snapCursorToCurrentSnappedComponent();
            }
            else
            {
                switch (direction)
                {
                    case 1:
                        if (this.areaNextButton == null || !this.areaNextButton.visible)
                        {
                            break;
                        }
                        this.currentlySnappedComponent = this.areaNextButton;
                        this.snapCursorToCurrentSnappedComponent();
                        this.areaNextButton.leftNeighborID = oldID;
                        break;
                    case 2:
                        if (this.presentButton == null)
                        {
                            break;
                        }
                        this.currentlySnappedComponent = this.presentButton;
                        this.snapCursorToCurrentSnappedComponent();
                        this.presentButton.upNeighborID = oldID;
                        break;
                    case 3:
                        if (this.areaBackButton == null || !this.areaBackButton.visible)
                        {
                            break;
                        }
                        this.currentlySnappedComponent = this.areaBackButton;
                        this.snapCursorToCurrentSnappedComponent();
                        this.areaBackButton.rightNeighborID = oldID;
                        break;
                }
            }
        }

        //public void setUpMenu(int whichArea, Dictionary<int, bool[]> bundlesComplete)
        //{
        //    this.noteTexture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\JunimoNote");
        //    if (!Game1.player.hasOrWillReceiveMail("seenJunimoNote"))
        //    {
        //        Game1.player.removeQuest("26");
        //        Game1.player.mailReceived.Add("seenJunimoNote");
        //    }
        //    if (!Game1.player.hasOrWillReceiveMail("wizardJunimoNote"))
        //    {
        //        Game1.addMailForTomorrow("wizardJunimoNote");
        //    }
        //    if (!Game1.player.hasOrWillReceiveMail("hasSeenAbandonedJunimoNote") && whichArea == 6)
        //    {
        //        Game1.player.mailReceived.Add("hasSeenAbandonedJunimoNote");
        //    }
        //    this.scrambledText = !Game1.player.hasOrWillReceiveMail("canReadJunimoText");
        //    tempSprites.Clear();
        //    this.whichArea = whichArea;
        //    this.inventory = new InventoryMenu(this.xPositionOnScreen + 128, this.yPositionOnScreen + 140, true, highlightMethod: this.HighlightObjects, capacity: 36, rows: 6, horizontalGap: 8, verticalGap: 8, drawSlots: false)
        //    {
        //        capacity = 36
        //    };
        //    for (var index = 0; index < this.inventory.inventory.Count; ++index)
        //    {
        //        if (index >= this.inventory.actualInventory.Count)
        //        {
        //            this.inventory.inventory[index].visible = false;
        //        }
        //    }
        //    foreach (var clickableComponent in this.inventory.GetBorder(InventoryMenu.BorderSide.Bottom))
        //    {
        //        clickableComponent.downNeighborID = -99998;
        //    }
        //    foreach (var clickableComponent in this.inventory.GetBorder(InventoryMenu.BorderSide.Right))
        //    {
        //        clickableComponent.rightNeighborID = -99998;
        //    }
        //    this.inventory.dropItemInvisibleButton.visible = false;
        //    var bundleData = Game1.netWorldState.Value.BundleData;
        //    var areaNameFromNumber = CommunityCenter.getAreaNameFromNumber(whichArea);
        //    var whichBundle = 0;
        //    foreach (var key in bundleData.Keys)
        //    {
        //        if (key.Contains(areaNameFromNumber))
        //        {
        //            var int32 = Convert.ToInt32(key.Split('/')[1]);
        //            var bundles = this.bundles;
        //            var bundle = new Bundle(int32, bundleData[key], bundlesComplete[int32], this.getBundleLocationFromNumber(whichBundle), "LooseSprites\\JunimoNote", this)
        //            {
        //                myID = whichBundle + 5000,
        //                rightNeighborID = -7777,
        //                leftNeighborID = -7777,
        //                upNeighborID = -7777,
        //                downNeighborID = -7777,
        //                fullyImmutable = true,
        //            };
        //            bundles.Add(bundle);
        //            ++whichBundle;
        //        }
        //    }
        //    var textureComponent = new ClickableTextureComponent("Back", new Rectangle(this.xPositionOnScreen + borderWidth * 2 + 8, this.yPositionOnScreen + borderWidth * 2 + 4, 64, 64), null, null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44), 1f)
        //    {
        //        myID = 103,
        //    };
        //    this.backButton = textureComponent;
        //    this.checkForRewards();
        //    canClick = true;
        //    Game1.playSound("shwip");
        //    var flag = false;
        //    foreach (var bundle in this.bundles)
        //    {
        //        if (!bundle.complete && !bundle.Equals(this.currentPageBundle))
        //        {
        //            flag = true;
        //            break;
        //        }
        //    }
        //    if (flag)
        //    {
        //        return;
        //    }
        //    var communityCenter = Game1.RequireLocation<CommunityCenter>("CommunityCenter");
        //    communityCenter.markAreaAsComplete(whichArea);
        //    this.exitFunction = this.restoreAreaOnExit;
        //    communityCenter.areaCompleteReward(whichArea);
        //}

        public virtual bool HighlightObjects(Item item)
        {
            if (this.currentPageBundle != null)
            {
                if (this.partialDonationItem != null && this.currentPartialIngredientDescriptionIndex >= 0)
                {
                    return this.currentPageBundle.IsValidItemForThisIngredientDescription(item, this.currentPageBundle.ingredients[this.currentPartialIngredientDescriptionIndex]);
                }
                foreach (var ingredient in this.currentPageBundle.ingredients)
                {
                    if (this.currentPageBundle.IsValidItemForThisIngredientDescription(item, ingredient))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override bool readyToClose() => (!this.specificBundlePage || this.singleBundleMenu) && this.isReadyToCloseMenuOrBundle();

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (!canClick)
            {
                return;
            }
            base.receiveLeftClick(x, y, playSound);
            if (this.scrambledText)
            {
                return;
            }
            if (this.specificBundlePage)
            {
                if (!this.currentPageBundle.complete && this.currentPageBundle.completionTimer <= 0)
                {
                    this.heldItem = this.inventory.leftClick(x, y, this.heldItem);
                }
                if (this.backButton != null && this.backButton.containsPoint(x, y) && this.heldItem == null)
                {
                    this.closeBundlePage();
                }
                if (this.partialDonationItem != null)
                {
                    if (this.heldItem != null && Game1.oldKBState.IsKeyDown(Keys.LeftShift))
                    {
                        for (var index = 0; index < this.ingredientSlots.Count; ++index)
                        {
                            if (this.ingredientSlots[index].item == this.partialDonationItem)
                            {
                                this.HandlePartialDonation(this.heldItem, this.ingredientSlots[index]);
                            }
                        }
                    }
                    else
                    {
                        for (var index = 0; index < this.ingredientSlots.Count; ++index)
                        {
                            if (this.ingredientSlots[index].containsPoint(x, y) && this.ingredientSlots[index].item == this.partialDonationItem)
                            {
                                if (this.heldItem != null)
                                {
                                    this.HandlePartialDonation(this.heldItem, this.ingredientSlots[index]);
                                    return;
                                }
                                this.ReturnPartialDonations(!Game1.oldKBState.IsKeyDown(Keys.LeftShift));
                                return;
                            }
                        }
                    }
                }
                else if (this.heldItem != null)
                {
                    if (Game1.oldKBState.IsKeyDown(Keys.LeftShift))
                    {
                        for (var index = 0; index < this.ingredientSlots.Count; ++index)
                        {
                            if (this.currentPageBundle.canAcceptThisItem(this.heldItem, this.ingredientSlots[index]))
                            {
                                if (this.ingredientSlots[index].item == null)
                                {
                                    // this.heldItem = this.currentPageBundle.tryToDepositThisItem(this.heldItem, this.ingredientSlots[index], "LooseSprites\\JunimoNote", this);
                                    this.checkIfBundleIsComplete();
                                    return;
                                }
                            }
                            else if (this.ingredientSlots[index].item == null)
                            {
                                this.HandlePartialDonation(this.heldItem, this.ingredientSlots[index]);
                            }
                        }
                    }
                    for (var index = 0; index < this.ingredientSlots.Count; ++index)
                    {
                        if (this.ingredientSlots[index].containsPoint(x, y))
                        {
                            if (this.currentPageBundle.canAcceptThisItem(this.heldItem, this.ingredientSlots[index]))
                            {
                                // this.heldItem = this.currentPageBundle.tryToDepositThisItem(this.heldItem, this.ingredientSlots[index], "LooseSprites\\JunimoNote", this);
                                this.checkIfBundleIsComplete();
                            }
                            else if (this.ingredientSlots[index].item == null)
                            {
                                this.HandlePartialDonation(this.heldItem, this.ingredientSlots[index]);
                            }
                        }
                    }
                }
                if (this.purchaseButton != null && this.purchaseButton.containsPoint(x, y))
                {
                    var stack = this.currentPageBundle.ingredients.Last<BundleIngredientDescription>().stack;
                    if (Game1.player.Money >= stack)
                    {
                        Game1.player.Money -= stack;
                        Game1.playSound("select");
                        // this.currentPageBundle.completionAnimation(this);
                        if (this.purchaseButton != null)
                        {
                            this.purchaseButton.scale = this.purchaseButton.baseScale * 0.75f;
                        }
                        var communityCenter = Game1.RequireLocation<CommunityCenter>("CommunityCenter");
                        communityCenter.bundleRewards[this.currentPageBundle.bundleIndex] = true;
                        communityCenter.bundles.FieldDict[this.currentPageBundle.bundleIndex][0] = true;
                        this.checkForRewards();
                        var flag = false;
                        foreach (var bundle in this.bundles)
                        {
                            if (!bundle.complete && !bundle.Equals(this.currentPageBundle))
                            {
                                flag = true;
                                break;
                            }
                        }
                        if (!flag)
                        {
                            communityCenter.markAreaAsComplete(this.whichArea);
                            this.exitFunction = this.restoreAreaOnExit;
                            communityCenter.areaCompleteReward(this.whichArea);
                        }
                        else
                        {
                            communityCenter.getJunimoForArea(this.whichArea)?.bringBundleBackToHut(Bundle.getColorFromColorIndex(this.currentPageBundle.bundleColor), Game1.RequireLocation("CommunityCenter"));
                        }
                        Game1.Multiplayer.globalChatInfoMessage("Bundle");
                    }
                    else
                    {
                        Game1.dayTimeMoneyBox.moneyShakeTimer = 600;
                    }
                }
                if (this.upperRightCloseButton != null && this.isReadyToCloseMenuOrBundle() && this.upperRightCloseButton.containsPoint(x, y))
                {
                    this.closeBundlePage();
                    return;
                }
            }
            else
            {
                foreach (var bundle in this.bundles)
                {
                    if (bundle.canBeClicked() && bundle.containsPoint(x, y))
                    {
                        this.setUpBundleSpecificPage(bundle);
                        Game1.playSound("shwip");
                        return;
                    }
                }
                if (this.presentButton != null && this.presentButton.containsPoint(x, y) && !this.fromGameMenu && !this.fromThisMenu)
                {
                    //this.openRewardsMenu();
                }
                if (this.fromGameMenu)
                {
                    //if (this.areaNextButton.containsPoint(x, y))
                    //{
                    //    this.SwapPage(1);
                    //}
                    //else if (this.areaBackButton.containsPoint(x, y))
                    //{
                    //    this.SwapPage(-1);
                    //}
                }
            }
            if (this.heldItem == null || this.isWithinBounds(x, y) || !this.heldItem.canBeTrashed())
            {
                return;
            }
            Game1.playSound("throwDownITem");
            Game1.createItemDebris(this.heldItem, Game1.player.getStandingPosition(), Game1.player.FacingDirection);
            this.heldItem = null;
        }

        public virtual void ReturnPartialDonation(Item item, bool play_sound = true)
        {
            var affected_items_list = new List<Item>();
            var inventory = Game1.player.addItemToInventory(item, affected_items_list);
            foreach (var obj in affected_items_list)
            {
                this.inventory.ShakeItem(obj);
            }
            if (inventory != null)
            {
                Utility.CollectOrDrop(inventory);
                this.inventory.ShakeItem(inventory);
            }
            if (!play_sound)
            {
                return;
            }
            Game1.playSound("coin");
        }

        public virtual void ReturnPartialDonations(bool to_hand = true)
        {
            if (this.partialDonationComponents.Count > 0)
            {
                var play_sound = true;
                foreach (var donationComponent in this.partialDonationComponents)
                {
                    if (this.heldItem == null & to_hand)
                    {
                        Game1.playSound("dwop");
                        this.heldItem = donationComponent;
                    }
                    else
                    {
                        this.ReturnPartialDonation(donationComponent, play_sound);
                        play_sound = false;
                    }
                }
            }
            this.ResetPartialDonation();
        }

        public virtual void ResetPartialDonation()
        {
            this.partialDonationComponents.Clear();
            this.currentPartialIngredientDescription = new BundleIngredientDescription?();
            this.currentPartialIngredientDescriptionIndex = -1;
            foreach (var ingredientSlot in this.ingredientSlots)
            {
                if (ingredientSlot.item == this.partialDonationItem)
                {
                    ingredientSlot.item = null;
                }
            }
            this.partialDonationItem = null;
        }

        public virtual bool CanBePartiallyOrFullyDonated(Item item)
        {
            if (this.currentPageBundle == null)
            {
                return false;
            }
            var descriptionIndexForItem = this.currentPageBundle.GetBundleIngredientDescriptionIndexForItem(item);
            if (descriptionIndexForItem < 0)
            {
                return false;
            }
            var ingredient = this.currentPageBundle.ingredients[descriptionIndexForItem];
            var num = 0;
            if (this.currentPageBundle.IsValidItemForThisIngredientDescription(item, ingredient))
            {
                num += item.Stack;
            }
            foreach (var obj in Game1.player.Items)
            {
                if (this.currentPageBundle.IsValidItemForThisIngredientDescription(obj, ingredient))
                {
                    num += obj.Stack;
                }
            }
            if (descriptionIndexForItem == this.currentPartialIngredientDescriptionIndex && this.partialDonationItem != null)
            {
                num += this.partialDonationItem.Stack;
            }
            return num >= ingredient.stack;
        }

        public virtual void HandlePartialDonation(Item item, ClickableTextureComponent slot)
        {
            if (this.currentPageBundle != null && !this.currentPageBundle.depositsAllowed || this.partialDonationItem != null && slot.item != this.partialDonationItem || !this.CanBePartiallyOrFullyDonated(item))
            {
                return;
            }
            if (!this.currentPartialIngredientDescription.HasValue)
            {
                this.currentPartialIngredientDescriptionIndex = this.currentPageBundle.GetBundleIngredientDescriptionIndexForItem(item);
                if (this.currentPartialIngredientDescriptionIndex != -1)
                {
                    this.currentPartialIngredientDescription = new BundleIngredientDescription?(this.currentPageBundle.ingredients[this.currentPartialIngredientDescriptionIndex]);
                }
            }
            if (!this.currentPartialIngredientDescription.HasValue || !this.currentPageBundle.IsValidItemForThisIngredientDescription(item, this.currentPartialIngredientDescription.Value))
            {
                return;
            }
            var flag = true;
            int num;
            if (slot.item == null)
            {
                Game1.playSound("sell");
                flag = false;
                this.partialDonationItem = item.getOne();
                num = Math.Min(this.currentPartialIngredientDescription.Value.stack, item.Stack);
                this.partialDonationItem.Stack = num;
                item.Stack -= num;
                this.partialDonationItem.Quality = this.currentPartialIngredientDescription.Value.quality;
                slot.item = this.partialDonationItem;
                slot.sourceRect.X = 512;
                slot.sourceRect.Y = 244;
            }
            else
            {
                num = Math.Min(this.currentPartialIngredientDescription.Value.stack - this.partialDonationItem.Stack, item.Stack);
                this.partialDonationItem.Stack += num;
                item.Stack -= num;
            }
            if (num > 0)
            {
                var one = this.heldItem.getOne();
                one.Stack = num;
                foreach (var donationComponent in this.partialDonationComponents)
                {
                    if (donationComponent.canStackWith(this.heldItem))
                    {
                        one.Stack = donationComponent.addToStack(one);
                    }
                }
                if (one.Stack > 0)
                {
                    this.partialDonationComponents.Add(one);
                }
                this.partialDonationComponents.Sort((Comparison<Item>)((a, b) => b.Stack.CompareTo(a.Stack)));
            }
            if (item.Stack <= 0 && item == this.heldItem)
            {
                this.heldItem = null;
            }
            if (this.partialDonationItem.Stack >= this.currentPartialIngredientDescription.Value.stack)
            {
                slot.item = null;
                // this.partialDonationItem = this.currentPageBundle.tryToDepositThisItem(this.partialDonationItem, slot, "LooseSprites\\JunimoNote", this);
                var partialDonationItem = this.partialDonationItem;
                if ((partialDonationItem != null ? (partialDonationItem.Stack > 0 ? 1 : 0) : 0) != 0)
                {
                    this.ReturnPartialDonation(this.partialDonationItem);
                }
                this.partialDonationItem = null;
                this.ResetPartialDonation();
                this.checkIfBundleIsComplete();
            }
            else
            {
                if (num <= 0 || !flag)
                {
                    return;
                }
                Game1.playSound("sell");
            }
        }

        public bool isReadyToCloseMenuOrBundle()
        {
            if (this.specificBundlePage)
            {
                var currentPageBundle = this.currentPageBundle;
                if ((currentPageBundle != null ? (currentPageBundle.completionTimer > 0 ? 1 : 0) : 0) != 0)
                {
                    return false;
                }
            }
            return this.heldItem == null;
        }

        public override void receiveGamePadButton(Buttons b)
        {
            base.receiveGamePadButton(b);
            if (this.fromGameMenu && !this.specificBundlePage)
            {
                if (b != Buttons.RightTrigger)
                {
                    if (b != Buttons.LeftTrigger)
                    {
                        return;
                    }
                    //this.SwapPage(-1);
                }
                else
                {
                    //this.SwapPage(1);
                }
            }
            else
            {
                if (!this.specificBundlePage)
                {
                    return;
                }
                switch (b)
                {
                    case Buttons.RightTrigger:
                        if (this.currentlySnappedComponent == null || this.currentlySnappedComponent.myID >= 50)
                        {
                            break;
                        }
                        var id = 250;
                        foreach (var ingredientSlot in this.ingredientSlots)
                        {
                            if (ingredientSlot.item == null)
                            {
                                id = ingredientSlot.myID;
                                break;
                            }
                        }
                        this.setCurrentlySnappedComponentTo(id);
                        this.snapCursorToCurrentSnappedComponent();
                        break;
                    case Buttons.LeftTrigger:
                        if (this.currentlySnappedComponent == null || this.currentlySnappedComponent.myID < 250)
                        {
                            break;
                        }
                        this.setCurrentlySnappedComponentTo(0);
                        this.snapCursorToCurrentSnappedComponent();
                        break;
                }
            }
        }

        //public void SwapPage(int direction)
        //{
        //    if (direction > 0 && !this.areaNextButton.visible || direction < 0 && !this.areaBackButton.visible)
        //    {
        //        return;
        //    }
        //    var communityCenter = Game1.RequireLocation<CommunityCenter>("CommunityCenter");
        //    var whichArea = this.whichArea;
        //    var num1 = 6;
        //    for (var index = 0; index < num1; ++index)
        //    {
        //        whichArea += direction;
        //        if (whichArea < 0)
        //        {
        //            whichArea += num1;
        //        }
        //        if (whichArea >= num1)
        //        {
        //            whichArea -= num1;
        //        }
        //        if (communityCenter.shouldNoteAppearInArea(whichArea))
        //        {
        //            var num2 = -1;
        //            if (this.currentlySnappedComponent != null && (this.currentlySnappedComponent.myID >= 5000 || this.currentlySnappedComponent.myID == 101 || this.currentlySnappedComponent.myID == 102))
        //            {
        //                num2 = this.currentlySnappedComponent.myID;
        //            }
        //            var junimoNoteMenu = new FakeJunimoNoteMenu(true, whichArea, true)
        //            {
        //                gameMenuTabToReturnTo = this.gameMenuTabToReturnTo
        //            };
        //            Game1.activeClickableMenu = junimoNoteMenu;
        //            if (num2 >= 0)
        //            {
        //                junimoNoteMenu.currentlySnappedComponent = junimoNoteMenu.getComponentWithID(this.currentlySnappedComponent.myID);
        //                junimoNoteMenu.snapCursorToCurrentSnappedComponent();
        //            }
        //            if (junimoNoteMenu.getComponentWithID(this.areaNextButton.leftNeighborID) != null)
        //            {
        //                junimoNoteMenu.areaNextButton.leftNeighborID = this.areaNextButton.leftNeighborID;
        //            }
        //            else
        //            {
        //                junimoNoteMenu.areaNextButton.leftNeighborID = junimoNoteMenu.areaBackButton.myID;
        //            }
        //            junimoNoteMenu.areaNextButton.rightNeighborID = this.areaNextButton.rightNeighborID;
        //            junimoNoteMenu.areaNextButton.upNeighborID = this.areaNextButton.upNeighborID;
        //            junimoNoteMenu.areaNextButton.downNeighborID = this.areaNextButton.downNeighborID;
        //            if (junimoNoteMenu.getComponentWithID(this.areaBackButton.rightNeighborID) != null)
        //            {
        //                junimoNoteMenu.areaBackButton.leftNeighborID = this.areaBackButton.leftNeighborID;
        //            }
        //            else
        //            {
        //                junimoNoteMenu.areaBackButton.leftNeighborID = junimoNoteMenu.areaNextButton.myID;
        //            }
        //            junimoNoteMenu.areaBackButton.rightNeighborID = this.areaBackButton.rightNeighborID;
        //            junimoNoteMenu.areaBackButton.upNeighborID = this.areaBackButton.upNeighborID;
        //            junimoNoteMenu.areaBackButton.downNeighborID = this.areaBackButton.downNeighborID;
        //            break;
        //        }
        //    }
        //}

        public override void receiveKeyPress(Keys key)
        {
            if (this.gameMenuTabToReturnTo != -1)
            {
                this.closeSound = "shwip";
            }
            base.receiveKeyPress(key);
            if (key.Equals(Keys.Delete) && this.heldItem != null && this.heldItem.canBeTrashed())
            {
                Utility.trashItem(this.heldItem);
                this.heldItem = null;
            }
            if (!Game1.options.doesInputListContain(Game1.options.menuButton, key) || !this.isReadyToCloseMenuOrBundle())
            {
                return;
            }
            if (this.singleBundleMenu)
            {
                this.exitThisMenu(this.gameMenuTabToReturnTo == -1);
            }
            this.closeBundlePage();
        }

        protected override void cleanupBeforeExit()
        {
            base.cleanupBeforeExit();
            if (this.gameMenuTabToReturnTo != -1)
            {
                Game1.activeClickableMenu = new GameMenu(this.gameMenuTabToReturnTo, playOpeningSound: false);
            }
            else
            {
                if (this.menuToReturnTo == null)
                {
                    return;
                }
                Game1.activeClickableMenu = this.menuToReturnTo;
            }
        }

        private void closeBundlePage()
        {
            if (this.partialDonationItem != null)
            {
                this.ReturnPartialDonations(false);
            }
            else
            {
                if (!this.specificBundlePage)
                {
                    return;
                }
                this.hoveredItem = null;
                this.inventory.descriptionText = "";
                if (this.heldItem == null)
                {
                    this.takeDownBundleSpecificPage();
                    Game1.playSound("shwip");
                }
                else
                {
                    this.heldItem = this.inventory.tryToAddItem(this.heldItem);
                }
            }
        }

        //private void reOpenThisMenu()
        //{
        //    var num = this.specificBundlePage ? 1 : 0;
        //    FakeJunimoNoteMenu junimoNoteMenu;
        //    if (this.fromGameMenu || this.fromThisMenu)
        //    {
        //        junimoNoteMenu = new FakeJunimoNoteMenu(this.fromGameMenu, this.whichArea, this.fromThisMenu)
        //        {
        //            gameMenuTabToReturnTo = this.gameMenuTabToReturnTo,
        //            menuToReturnTo = this.menuToReturnTo
        //        };
        //    }
        //    else
        //    {
        //        junimoNoteMenu = new FakeJunimoNoteMenu(this.whichArea, Game1.RequireLocation<CommunityCenter>("CommunityCenter").bundlesDict())
        //        {
        //            gameMenuTabToReturnTo = this.gameMenuTabToReturnTo,
        //            menuToReturnTo = this.menuToReturnTo
        //        };
        //    }
        //    if (num != 0)
        //    {
        //        foreach (var bundle in junimoNoteMenu.bundles)
        //        {
        //            if (bundle.bundleIndex == this.currentPageBundle.bundleIndex)
        //            {
        //                junimoNoteMenu.setUpBundleSpecificPage(bundle);
        //                break;
        //            }
        //        }
        //    }
        //    Game1.activeClickableMenu = junimoNoteMenu;
        //}

        private void updateIngredientSlots()
        {
            var index = 0;
            foreach (var ingredient in this.currentPageBundle.ingredients)
            {
                if (ingredient.completed && index < this.ingredientSlots.Count)
                {
                    var representativeItemId = GetRepresentativeItemId(ingredient);
                    if (ingredient.preservesId != null)
                    {
                        this.ingredientSlots[index].item = Utility.CreateFlavoredItem(representativeItemId, ingredient.preservesId, ingredient.quality, ingredient.stack);
                    }
                    else
                    {
                        this.ingredientSlots[index].item = ItemRegistry.Create(representativeItemId, ingredient.stack, ingredient.quality);
                    }
                    this.currentPageBundle.ingredientDepositAnimation(this.ingredientSlots[index], "LooseSprites\\JunimoNote", true);
                    ++index;
                }
            }
        }

        /// <summary>Get the qualified item ID to draw in the bundle UI for an ingredient.</summary>
        /// <param name="ingredient">The ingredient to represent.</param>
        public static string GetRepresentativeItemId(BundleIngredientDescription ingredient)
        {
            if (!ingredient.category.HasValue)
            {
                return ingredient.id;
            }
            foreach (var parsedItemData in ItemRegistry.GetObjectTypeDefinition().GetAllData())
            {
                var category1 = parsedItemData.Category;
                var category2 = ingredient.category;
                var valueOrDefault = category2.GetValueOrDefault();
                if (category1 == valueOrDefault & category2.HasValue)
                {
                    return parsedItemData.QualifiedItemId;
                }
            }
            return "0";
        }

        public static void GetBundleRewards(int area, List<Item> rewards)
        {
            var communityCenter = Game1.RequireLocation<CommunityCenter>("CommunityCenter");
            var bundleData = Game1.netWorldState.Value.BundleData;
            foreach (var key in bundleData.Keys)
            {
                if (key.Contains(CommunityCenter.getAreaNameFromNumber(area)))
                {
                    var int32 = Convert.ToInt32(key.Split('/')[1]);
                    if (communityCenter.bundleRewards[int32])
                    {
                        var standardTextDescription = Utility.getItemFromStandardTextDescription(bundleData[key].Split('/')[1], Game1.player);
                        standardTextDescription.SpecialVariable = int32;
                        rewards.Add(standardTextDescription);
                    }
                }
            }
        }

        //private void openRewardsMenu()
        //{
        //    Game1.playSound("smallSelect");
        //    var objList = new List<Item>();
        //    GetBundleRewards(this.whichArea, objList);
        //    Game1.activeClickableMenu = new ItemGrabMenu(objList, false, true, null, null, null, this.rewardGrabbed, canBeExitedWithKey: true, context: this);
        //    Game1.activeClickableMenu.exitFunction = this.exitFunction != null ? this.exitFunction : this.reOpenThisMenu;
        //}

        private void rewardGrabbed(Item item, Farmer who) => Game1.RequireLocation<CommunityCenter>("CommunityCenter").bundleRewards[item.SpecialVariable] = false;

        private void checkIfBundleIsComplete()
        {
            this.ReturnPartialDonations();
            if (!this.specificBundlePage || this.currentPageBundle == null)
            {
                return;
            }
            var num = 0;
            foreach (var ingredientSlot in this.ingredientSlots)
            {
                if (ingredientSlot.item != null && ingredientSlot.item != this.partialDonationItem)
                {
                    ++num;
                }
            }
            if (num < this.currentPageBundle.numberOfIngredientSlots)
            {
                return;
            }
            if (this.heldItem != null)
            {
                Game1.player.addItemToInventory(this.heldItem);
                this.heldItem = null;
            }
            if (!this.singleBundleMenu)
            {
                var location = Game1.RequireLocation<CommunityCenter>("CommunityCenter");
                for (var index = 0; index < location.bundles[this.currentPageBundle.bundleIndex].Length; ++index)
                    location.bundles.FieldDict[this.currentPageBundle.bundleIndex][index] = true;
                location.checkForNewJunimoNotes();
                screenSwipe = new ScreenSwipe(0);
                // this.currentPageBundle.completionAnimation(this, delay: 400);
                canClick = false;
                location.bundleRewards[this.currentPageBundle.bundleIndex] = true;
                Game1.Multiplayer.globalChatInfoMessage("Bundle");
                var flag = false;
                foreach (var bundle in this.bundles)
                {
                    if (!bundle.complete && !bundle.Equals(this.currentPageBundle))
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    if (this.whichArea == 6)
                    {
                        this.exitFunction = this.restoreaAreaOnExit_AbandonedJojaMart;
                    }
                    else
                    {
                        location.markAreaAsComplete(this.whichArea);
                        this.exitFunction = this.restoreAreaOnExit;
                        location.areaCompleteReward(this.whichArea);
                    }
                }
                else
                {
                    location.getJunimoForArea(this.whichArea)?.bringBundleBackToHut(Bundle.getColorFromColorIndex(this.currentPageBundle.bundleColor), location);
                }
                this.checkForRewards();
            }
            else
            {
                if (this.onBundleComplete == null)
                {
                    return;
                }
                this.onBundleComplete(this);
            }
        }

        private void restoreaAreaOnExit_AbandonedJojaMart() => Game1.RequireLocation<AbandonedJojaMart>("AbandonedJojaMart").restoreAreaCutscene();

        private void restoreAreaOnExit()
        {
            if (this.fromGameMenu)
            {
                return;
            }
            Game1.RequireLocation<CommunityCenter>("CommunityCenter").restoreAreaCutscene(this.whichArea);
        }

        public void checkForRewards()
        {
            var bundleData = Game1.netWorldState.Value.BundleData;
            foreach (var key in bundleData.Keys)
            {
                if (key.Contains(CommunityCenter.getAreaNameFromNumber(this.whichArea)) && bundleData[key].Split('/')[1].Length > 1)
                {
                    var int32 = Convert.ToInt32(key.Split('/')[1]);
                    if (Game1.RequireLocation<CommunityCenter>("CommunityCenter").bundleRewards[int32])
                    {
                        this.presentButton = new ClickableAnimatedComponent(new Rectangle(this.xPositionOnScreen + 592, this.yPositionOnScreen + 512, 72, 72), "", Game1.content.LoadString("Strings\\StringsFromCSFiles:JunimoNoteMenu.cs.10783"), new TemporaryAnimatedSprite("LooseSprites\\JunimoNote", new Rectangle(548, 262, 18, 20), 70f, 4, 99999, new Vector2(-64f, -64f), false, false, 0.5f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true));
                        break;
                    }
                }
            }
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            if (!canClick)
            {
                return;
            }
            if (this.specificBundlePage)
            {
                this.heldItem = this.inventory.rightClick(x, y, this.heldItem);
                if (this.partialDonationItem != null)
                {
                    for (var index = 0; index < this.ingredientSlots.Count; ++index)
                    {
                        if (this.ingredientSlots[index].containsPoint(x, y) && this.ingredientSlots[index].item == this.partialDonationItem)
                        {
                            if (this.partialDonationComponents.Count > 0)
                            {
                                var one = this.partialDonationComponents[0].getOne();
                                var flag = false;
                                if (this.heldItem == null)
                                {
                                    this.heldItem = one;
                                    Game1.playSound("dwop");
                                    flag = true;
                                }
                                else if (this.heldItem.canStackWith(one))
                                {
                                    this.heldItem.addToStack(one);
                                    Game1.playSound("dwop");
                                    flag = true;
                                }
                                if (flag)
                                {
                                    --this.partialDonationComponents[0].Stack;
                                    if (this.partialDonationComponents[0].Stack <= 0)
                                    {
                                        this.partialDonationComponents.RemoveAt(0);
                                    }
                                    var num = 0;
                                    foreach (var donationComponent in this.partialDonationComponents)
                                    {
                                        num += donationComponent.Stack;
                                    }
                                    if (this.partialDonationItem != null)
                                    {
                                        this.partialDonationItem.Stack = num;
                                    }
                                    if (this.partialDonationComponents.Count == 0)
                                    {
                                        this.ResetPartialDonation();
                                    }
                                }
                            }
                            break;
                        }
                    }
                }
            }
            if (this.specificBundlePage || !this.isReadyToCloseMenuOrBundle())
            {
                return;
            }
            this.exitThisMenu(this.gameMenuTabToReturnTo == -1);
        }

        public override void update(GameTime time)
        {
            if (this.specificBundlePage && this.currentPageBundle != null && this.currentPageBundle.completionTimer <= 0 && this.isReadyToCloseMenuOrBundle() && this.currentPageBundle.complete)
            {
                this.takeDownBundleSpecificPage();
            }
            foreach (var bundle in this.bundles)
            {
                bundle.update(time);
            }
            for (var index = tempSprites.Count - 1; index >= 0; --index)
            {
                if (tempSprites[index].update(time))
                {
                    tempSprites.RemoveAt(index);
                }
            }
            this.presentButton?.update(time);
            if (screenSwipe != null)
            {
                canClick = false;
                if (screenSwipe.update(time))
                {
                    screenSwipe = null;
                    canClick = true;
                    var screenSwipeFinished = this.onScreenSwipeFinished;
                    if (screenSwipeFinished != null)
                    {
                        screenSwipeFinished(this);
                    }
                }
            }
            if (!this.bundlesChanged || !this.fromGameMenu)
            {
                return;
            }
            //this.reOpenThisMenu();
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            if (this.scrambledText)
            {
                return;
            }
            hoverText = "";
            if (this.specificBundlePage)
            {
                this.backButton?.tryHover(x, y);
                this.hoveredItem = this.currentPageBundle.complete || this.currentPageBundle.completionTimer > 0 ? null : this.inventory.hover(x, y, this.heldItem);
                foreach (var ingredient in this.ingredientList)
                {
                    if (ingredient.bounds.Contains(x, y))
                    {
                        hoverText = ingredient.hoverText;
                        break;
                    }
                }
                if (this.heldItem != null)
                {
                    foreach (var ingredientSlot in this.ingredientSlots)
                    {
                        if (ingredientSlot.bounds.Contains(x, y) && this.CanBePartiallyOrFullyDonated(this.heldItem) && (this.partialDonationItem == null || ingredientSlot.item == this.partialDonationItem))
                        {
                            ingredientSlot.sourceRect.X = 530;
                            ingredientSlot.sourceRect.Y = 262;
                        }
                        else
                        {
                            ingredientSlot.sourceRect.X = 512;
                            ingredientSlot.sourceRect.Y = 244;
                        }
                    }
                }
                this.purchaseButton?.tryHover(x, y);
            }
            else
            {
                if (this.presentButton != null)
                {
                    hoverText = this.presentButton.tryHover(x, y);
                }
                foreach (var bundle in this.bundles)
                {
                    bundle.tryHoverAction(x, y);
                }
                if (!this.fromGameMenu)
                {
                    return;
                }
                this.areaNextButton.tryHover(x, y);
                this.areaBackButton.tryHover(x, y);
            }
        }

        public override void draw(SpriteBatch b)
        {
            if (Game1.options.showMenuBackground)
            {
                this.drawBackground(b);
            }
            else if (!Game1.options.showClearBackgrounds)
            {
                b.Draw(Game1.fadeToBlackRect, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), Color.Black * 0.5f);
            }
            if (!this.specificBundlePage)
            {
                b.Draw(this.noteTexture, new Vector2(this.xPositionOnScreen, this.yPositionOnScreen), new Rectangle?(new Rectangle(0, 0, 320, 180)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.1f);
                SpriteText.drawStringHorizontallyCenteredAt(b, this.scrambledText ? CommunityCenter.getAreaEnglishDisplayNameFromNumber(this.whichArea) : CommunityCenter.getAreaDisplayNameFromNumber(this.whichArea), this.xPositionOnScreen + this.width / 2 + 16, this.yPositionOnScreen + 12, height: 99999, alpha: 0.88f, junimoText: this.scrambledText);
                if (this.scrambledText)
                {
                    SpriteText.drawString(b, LocalizedContentManager.CurrentLanguageLatin ? Game1.content.LoadString("Strings\\StringsFromCSFiles:JunimoNoteMenu.cs.10786") : Game1.content.LoadBaseString("Strings\\StringsFromCSFiles:JunimoNoteMenu.cs.10786"), this.xPositionOnScreen + 96, this.yPositionOnScreen + 96, width: (this.width - 192), height: 99999, alpha: 0.88f, junimoText: true);
                    base.draw(b);
                    if (Game1.options.SnappyMenus || !canClick)
                    {
                        return;
                    }
                    this.drawMouse(b);
                    return;
                }
                foreach (var bundle in this.bundles)
                {
                    bundle.draw(b);
                }
                this.presentButton?.draw(b);
                foreach (var tempSprite in tempSprites)
                {
                    tempSprite.draw(b, true);
                }
                if (this.fromGameMenu)
                {
                    if (this.areaNextButton.visible)
                    {
                        this.areaNextButton.draw(b);
                    }
                    if (this.areaBackButton.visible)
                    {
                        this.areaBackButton.draw(b);
                    }
                }
            }
            else
            {
                b.Draw(this.noteTexture, new Vector2(this.xPositionOnScreen, this.yPositionOnScreen), new Rectangle?(new Rectangle(320, 0, 320, 180)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.1f);
                if (this.currentPageBundle != null)
                {
                    var num1 = this.currentPageBundle.bundleIndex;
                    var texture = this.noteTexture;
                    var num2 = 180;
                    if (this.currentPageBundle.bundleTextureIndexOverride >= 0)
                    {
                        num1 = this.currentPageBundle.bundleTextureIndexOverride;
                    }
                    if (this.currentPageBundle.bundleTextureOverride != null)
                    {
                        texture = this.currentPageBundle.bundleTextureOverride;
                        num2 = 0;
                    }
                    b.Draw(texture, new Vector2(this.xPositionOnScreen + 872, this.yPositionOnScreen + 88), new Rectangle?(new Rectangle(num1 * 16 * 2 % texture.Width, num2 + 32 * (num1 * 16 * 2 / texture.Width), 32, 32)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.15f);
                    if (this.currentPageBundle.label != null)
                    {
                        var x = Game1.dialogueFont.MeasureString(!Game1.player.hasOrWillReceiveMail("canReadJunimoText") ? "???" : Game1.content.LoadString("Strings\\UI:JunimoNote_BundleName", this.currentPageBundle.label)).X;
                        b.Draw(this.noteTexture, new Vector2(this.xPositionOnScreen + 936 - (int)x / 2 - 16, this.yPositionOnScreen + 228), new Rectangle?(new Rectangle(517, 266, 4, 17)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.1f);
                        b.Draw(this.noteTexture, new Rectangle(this.xPositionOnScreen + 936 - (int)x / 2, this.yPositionOnScreen + 228, (int)x, 68), new Rectangle?(new Rectangle(520, 266, 1, 17)), Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.1f);
                        b.Draw(this.noteTexture, new Vector2(this.xPositionOnScreen + 936 + (int)x / 2, this.yPositionOnScreen + 228), new Rectangle?(new Rectangle(524, 266, 4, 17)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.1f);
                        b.DrawString(Game1.dialogueFont, !Game1.player.hasOrWillReceiveMail("canReadJunimoText") ? "???" : Game1.content.LoadString("Strings\\UI:JunimoNote_BundleName", this.currentPageBundle.label), new Vector2(this.xPositionOnScreen + 936 - x / 2f, this.yPositionOnScreen + 236) + new Vector2(2f, 2f), Game1.textShadowColor);
                        b.DrawString(Game1.dialogueFont, !Game1.player.hasOrWillReceiveMail("canReadJunimoText") ? "???" : Game1.content.LoadString("Strings\\UI:JunimoNote_BundleName", this.currentPageBundle.label), new Vector2(this.xPositionOnScreen + 936 - x / 2f, this.yPositionOnScreen + 236) + new Vector2(0.0f, 2f), Game1.textShadowColor);
                        b.DrawString(Game1.dialogueFont, !Game1.player.hasOrWillReceiveMail("canReadJunimoText") ? "???" : Game1.content.LoadString("Strings\\UI:JunimoNote_BundleName", this.currentPageBundle.label), new Vector2(this.xPositionOnScreen + 936 - x / 2f, this.yPositionOnScreen + 236) + new Vector2(2f, 0.0f), Game1.textShadowColor);
                        b.DrawString(Game1.dialogueFont, !Game1.player.hasOrWillReceiveMail("canReadJunimoText") ? "???" : Game1.content.LoadString("Strings\\UI:JunimoNote_BundleName", this.currentPageBundle.label), new Vector2(this.xPositionOnScreen + 936 - x / 2f, this.yPositionOnScreen + 236), Game1.textColor * 0.9f);
                    }
                }
                if (this.backButton != null)
                {
                    this.backButton.draw(b);
                }
                if (this.purchaseButton != null)
                {
                    this.purchaseButton.draw(b);
                    Game1.dayTimeMoneyBox.drawMoneyBox(b);
                }
                var extraAlpha = 1f;
                if (this.partialDonationItem != null)
                {
                    extraAlpha = 0.25f;
                }
                foreach (var tempSprite in tempSprites)
                {
                    tempSprite.draw(b, true, extraAlpha: extraAlpha);
                }
                foreach (var ingredientSlot in this.ingredientSlots)
                {
                    var alpha = 1f;
                    if (this.partialDonationItem != null && ingredientSlot.item != this.partialDonationItem)
                    {
                        alpha = 0.25f;
                    }
                    if (ingredientSlot.item == null || this.partialDonationItem != null && ingredientSlot.item == this.partialDonationItem)
                    {
                        ingredientSlot.draw(b, (this.fromGameMenu ? Color.LightGray * 0.5f : Color.White) * alpha, 0.89f);
                    }
                    ingredientSlot.drawItem(b, 4, 4, alpha);
                }
                for (var index = 0; index < this.ingredientList.Count; ++index)
                {
                    var num3 = 1f;
                    if (this.currentPartialIngredientDescriptionIndex >= 0 && this.currentPartialIngredientDescriptionIndex != index)
                    {
                        num3 = 0.25f;
                    }
                    var ingredient = this.ingredientList[index];
                    var flag = false;
                    var num4 = index;
                    var count = this.currentPageBundle?.ingredients?.Count;
                    var valueOrDefault = count.GetValueOrDefault();
                    if (num4 < valueOrDefault & count.HasValue && this.currentPageBundle.ingredients[index].completed)
                    {
                        flag = true;
                    }
                    if (!flag)
                    {
                        b.Draw(Game1.shadowTexture, new Vector2(ingredient.bounds.Center.X - Game1.shadowTexture.Bounds.Width * 4 / 2 - 4, ingredient.bounds.Center.Y + 4), new Rectangle?(Game1.shadowTexture.Bounds), Color.White * num3, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.1f);
                    }
                    if (ingredient.item != null && ingredient.visible)
                    {
                        ingredient.item.drawInMenu(b, new Vector2(ingredient.bounds.X, ingredient.bounds.Y), ingredient.scale / 4f, 1f, 0.9f, StackDrawType.Draw, Color.White * (flag ? 0.25f : num3), false);
                    }
                }
                this.inventory.draw(b);
            }
            if (this.getRewardNameForArea(this.whichArea) != "")
            {
                SpriteText.drawStringWithScrollCenteredAt(b, this.getRewardNameForArea(this.whichArea), this.xPositionOnScreen + this.width / 2, Math.Min(this.yPositionOnScreen + this.height + 20, Game1.uiViewport.Height - 64 - 8));
            }
            base.draw(b);
            Game1.mouseCursorTransparency = 1f;
            if (canClick)
            {
                this.drawMouse(b);
            }
            this.heldItem?.drawInMenu(b, new Vector2(Game1.getOldMouseX() + 16, Game1.getOldMouseY() + 16), 1f);
            if (this.inventory.descriptionText.Length > 0)
            {
                if (this.hoveredItem != null)
                {
                    drawToolTip(b, this.hoveredItem.getDescription(), this.hoveredItem.DisplayName, this.hoveredItem);
                }
            }
            else
            {
                drawHoverText(b, this.singleBundleMenu || Game1.player.hasOrWillReceiveMail("canReadJunimoText") || hoverText.Length <= 0 ? hoverText : "???", Game1.dialogueFont);
            }
            screenSwipe?.draw(b);
        }

        public string getRewardNameForArea(int whichArea)
        {
            switch (whichArea)
            {
                case -1:
                    return "";
                case 0:
                    return Game1.content.LoadString("Strings\\UI:JunimoNote_RewardPantry");
                case 1:
                    return Game1.content.LoadString("Strings\\UI:JunimoNote_RewardCrafts");
                case 2:
                    return Game1.content.LoadString("Strings\\UI:JunimoNote_RewardFishTank");
                case 3:
                    return Game1.content.LoadString("Strings\\UI:JunimoNote_RewardBoiler");
                case 4:
                    return Game1.content.LoadString("Strings\\UI:JunimoNote_RewardVault");
                case 5:
                    return Game1.content.LoadString("Strings\\UI:JunimoNote_RewardBulletin");
                default:
                    return "???";
            }
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            base.gameWindowSizeChanged(oldBounds, newBounds);
            tempSprites.Clear();
            this.xPositionOnScreen = Game1.uiViewport.Width / 2 - 640;
            this.yPositionOnScreen = Game1.uiViewport.Height / 2 - 360;
            this.backButton = new ClickableTextureComponent("Back", new Rectangle(this.xPositionOnScreen + borderWidth * 2 + 8, this.yPositionOnScreen + borderWidth * 2 + 4, 64, 64), null, null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44), 1f);
            if (this.fromGameMenu)
            {
                var textureComponent1 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width - 128, this.yPositionOnScreen, 48, 44), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 4f)
                {
                    visible = false,
                };
                this.areaNextButton = textureComponent1;
                var textureComponent2 = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + 64, this.yPositionOnScreen, 48, 44), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), 4f)
                {
                    visible = false,
                };
                this.areaBackButton = textureComponent2;
            }
            this.inventory = new InventoryMenu(this.xPositionOnScreen + 128, this.yPositionOnScreen + 140, true, highlightMethod: this.HighlightObjects, capacity: ((int)Game1.player.maxItems), rows: 6, horizontalGap: 8, verticalGap: 8, drawSlots: false);
            for (var index = 0; index < this.inventory.inventory.Count; ++index)
            {
                if (index >= this.inventory.actualInventory.Count)
                {
                    this.inventory.inventory[index].visible = false;
                }
            }
            for (var index = 0; index < this.bundles.Count; ++index)
            {
                var locationFromNumber = this.getBundleLocationFromNumber(index);
                this.bundles[index].bounds.X = locationFromNumber.X;
                this.bundles[index].bounds.Y = locationFromNumber.Y;
                this.bundles[index].sprite.position = new Vector2(locationFromNumber.X, locationFromNumber.Y);
            }
            if (!this.specificBundlePage)
            {
                return;
            }
            var ofIngredientSlots = this.currentPageBundle.numberOfIngredientSlots;
            var toAddTo1 = new List<Rectangle>();
            this.addRectangleRowsToList(toAddTo1, ofIngredientSlots, 932, 540);
            this.ingredientSlots.Clear();
            for (var index = 0; index < toAddTo1.Count; ++index)
                this.ingredientSlots.Add(new ClickableTextureComponent(toAddTo1[index], this.noteTexture, new Rectangle(512, 244, 18, 18), 4f));
            var toAddTo2 = new List<Rectangle>();
            this.ingredientList.Clear();
            this.addRectangleRowsToList(toAddTo2, this.currentPageBundle.ingredients.Count, 932, 364);
            for (var index = 0; index < toAddTo2.Count; ++index)
            {
                var ingredient = this.currentPageBundle.ingredients[index];
                var metadata = ItemRegistry.GetMetadata(ingredient.id);
                if (metadata?.TypeIdentifier == "(O)")
                {
                    var parsedOrErrorData = metadata.GetParsedOrErrorData();
                    var texture = parsedOrErrorData.GetTexture();
                    var sourceRect = parsedOrErrorData.GetSourceRect();
                    var obj = ingredient.preservesId != null ? Utility.CreateFlavoredItem(ingredient.id, ingredient.preservesId, ingredient.quality, ingredient.stack) : ItemRegistry.Create(ingredient.id, ingredient.stack, ingredient.quality);
                    var ingredientList = this.ingredientList;
                    var textureComponent = new ClickableTextureComponent("", toAddTo2[index], "", obj.DisplayName, texture, sourceRect, 4f)
                    {
                        myID = index + 1000,
                        item = obj,
                        upNeighborID = -99998,
                        rightNeighborID = -99998,
                        leftNeighborID = -99998,
                        downNeighborID = -99998,
                    };
                    ingredientList.Add(textureComponent);
                }
            }
            this.updateIngredientSlots();
        }

        private void setUpBundleSpecificPage(Bundle b)
        {
            tempSprites.Clear();
            this.currentPageBundle = b;
            this.specificBundlePage = true;
            if (this.whichArea == 4)
            {
                if (this.fromGameMenu)
                {
                    return;
                }
                var textureComponent = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + 800, this.yPositionOnScreen + 504, 260, 72), this.noteTexture, new Rectangle(517, 286, 65, 20), 4f)
                {
                    myID = 797,
                    leftNeighborID = 103,
                };
                this.purchaseButton = textureComponent;
                if (!Game1.options.SnappyMenus)
                {
                    return;
                }
                this.currentlySnappedComponent = this.purchaseButton;
                this.snapCursorToCurrentSnappedComponent();
            }
            else
            {
                var ofIngredientSlots = b.numberOfIngredientSlots;
                var toAddTo1 = new List<Rectangle>();
                this.addRectangleRowsToList(toAddTo1, ofIngredientSlots, 932, 540);
                for (var index = 0; index < toAddTo1.Count; ++index)
                {
                    var ingredientSlots = this.ingredientSlots;
                    var textureComponent = new ClickableTextureComponent(toAddTo1[index], this.noteTexture, new Rectangle(512, 244, 18, 18), 4f)
                    {
                        myID = index + 250,
                        upNeighborID = -99998,
                        rightNeighborID = -99998,
                        leftNeighborID = -99998,
                        downNeighborID = -99998,
                    };
                    ingredientSlots.Add(textureComponent);
                }
                var toAddTo2 = new List<Rectangle>();
                this.addRectangleRowsToList(toAddTo2, b.ingredients.Count, 932, 364);
                for (var index = 0; index < toAddTo2.Count; ++index)
                {
                    var ingredient = b.ingredients[index];
                    var representativeItemId = GetRepresentativeItemId(ingredient);
                    var dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(representativeItemId);
                    if (dataOrErrorItem.HasTypeObject())
                    {
                        var category = ingredient.category;
                        string hoverText;
                        if (category.HasValue)
                        {
                            switch (category.GetValueOrDefault())
                            {
                                case -75:
                                    hoverText = Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.570");
                                    goto label_18;
                                case -6:
                                    hoverText = Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.573");
                                    goto label_18;
                                case -5:
                                    hoverText = Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.572");
                                    goto label_18;
                                case -4:
                                    hoverText = Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.571");
                                    goto label_18;
                                case -2:
                                    hoverText = Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.569");
                                    goto label_18;
                            }
                        }
                        hoverText = dataOrErrorItem.DisplayName;
                    label_18:
                        Item flavoredItem;
                        if (ingredient.preservesId != null)
                        {
                            flavoredItem = Utility.CreateFlavoredItem(ingredient.id, ingredient.preservesId, ingredient.quality, ingredient.stack);
                            hoverText = flavoredItem.DisplayName;
                        }
                        else
                        {
                            flavoredItem = ItemRegistry.Create(representativeItemId, ingredient.stack, ingredient.quality);
                        }
                        var texture = dataOrErrorItem.GetTexture();
                        var sourceRect = dataOrErrorItem.GetSourceRect();
                        var ingredientList = this.ingredientList;
                        var textureComponent = new ClickableTextureComponent("ingredient_list_slot", toAddTo2[index], "", hoverText, texture, sourceRect, 4f)
                        {
                            myID = index + 1000,
                            item = flavoredItem,
                            upNeighborID = -99998,
                            rightNeighborID = -99998,
                            leftNeighborID = -99998,
                            downNeighborID = -99998,
                        };
                        ingredientList.Add(textureComponent);
                    }
                }
                this.updateIngredientSlots();
                if (!Game1.options.SnappyMenus)
                {
                    return;
                }
                this.populateClickableComponentList();
                if (this.inventory?.inventory != null)
                {
                    for (var index = 0; index < this.inventory.inventory.Count; ++index)
                    {
                        if (this.inventory.inventory[index] != null)
                        {
                            if (this.inventory.inventory[index].downNeighborID == 101)
                            {
                                this.inventory.inventory[index].downNeighborID = -1;
                            }
                            if (this.inventory.inventory[index].leftNeighborID == -1)
                            {
                                this.inventory.inventory[index].leftNeighborID = 103;
                            }
                            if (this.inventory.inventory[index].upNeighborID >= 1000)
                            {
                                this.inventory.inventory[index].upNeighborID = 103;
                            }
                        }
                    }
                }
                this.currentlySnappedComponent = this.getComponentWithID(0);
                this.snapCursorToCurrentSnappedComponent();
            }
        }

        public override bool IsAutomaticSnapValid(
          int direction,
          ClickableComponent a,
          ClickableComponent b)
        {
            return (this.currentPartialIngredientDescriptionIndex < 0 || (!this.ingredientSlots.Contains<ClickableComponent>(b) || b.item == this.partialDonationItem) && (!this.ingredientList.Contains<ClickableComponent>(b) || this.ingredientList.IndexOf(b as ClickableTextureComponent) == this.currentPartialIngredientDescriptionIndex)) && (a.myID >= 5000 || a.myID == 101 ? 1 : (a.myID == 102 ? 1 : 0)) == (b.myID >= 5000 || b.myID == 101 ? 1 : (b.myID == 102 ? 1 : 0));
        }

        private void addRectangleRowsToList(
          List<Rectangle> toAddTo,
          int numberOfItems,
          int centerX,
          int centerY)
        {
            switch (numberOfItems)
            {
                case 1:
                    toAddTo.AddRange(this.createRowOfBoxesCenteredAt(this.xPositionOnScreen + centerX, this.yPositionOnScreen + centerY, 1, 72, 72, 12));
                    break;
                case 2:
                    toAddTo.AddRange(this.createRowOfBoxesCenteredAt(this.xPositionOnScreen + centerX, this.yPositionOnScreen + centerY, 2, 72, 72, 12));
                    break;
                case 3:
                    toAddTo.AddRange(this.createRowOfBoxesCenteredAt(this.xPositionOnScreen + centerX, this.yPositionOnScreen + centerY, 3, 72, 72, 12));
                    break;
                case 4:
                    toAddTo.AddRange(this.createRowOfBoxesCenteredAt(this.xPositionOnScreen + centerX, this.yPositionOnScreen + centerY, 4, 72, 72, 12));
                    break;
                case 5:
                    toAddTo.AddRange(this.createRowOfBoxesCenteredAt(this.xPositionOnScreen + centerX, this.yPositionOnScreen + centerY - 36, 3, 72, 72, 12));
                    toAddTo.AddRange(this.createRowOfBoxesCenteredAt(this.xPositionOnScreen + centerX, this.yPositionOnScreen + centerY + 40, 2, 72, 72, 12));
                    break;
                case 6:
                    toAddTo.AddRange(this.createRowOfBoxesCenteredAt(this.xPositionOnScreen + centerX, this.yPositionOnScreen + centerY - 36, 3, 72, 72, 12));
                    toAddTo.AddRange(this.createRowOfBoxesCenteredAt(this.xPositionOnScreen + centerX, this.yPositionOnScreen + centerY + 40, 3, 72, 72, 12));
                    break;
                case 7:
                    toAddTo.AddRange(this.createRowOfBoxesCenteredAt(this.xPositionOnScreen + centerX, this.yPositionOnScreen + centerY - 36, 4, 72, 72, 12));
                    toAddTo.AddRange(this.createRowOfBoxesCenteredAt(this.xPositionOnScreen + centerX, this.yPositionOnScreen + centerY + 40, 3, 72, 72, 12));
                    break;
                case 8:
                    toAddTo.AddRange(this.createRowOfBoxesCenteredAt(this.xPositionOnScreen + centerX, this.yPositionOnScreen + centerY - 36, 4, 72, 72, 12));
                    toAddTo.AddRange(this.createRowOfBoxesCenteredAt(this.xPositionOnScreen + centerX, this.yPositionOnScreen + centerY + 40, 4, 72, 72, 12));
                    break;
                case 9:
                    toAddTo.AddRange(this.createRowOfBoxesCenteredAt(this.xPositionOnScreen + centerX, this.yPositionOnScreen + centerY - 36, 5, 72, 72, 12));
                    toAddTo.AddRange(this.createRowOfBoxesCenteredAt(this.xPositionOnScreen + centerX, this.yPositionOnScreen + centerY + 40, 4, 72, 72, 12));
                    break;
                case 10:
                    toAddTo.AddRange(this.createRowOfBoxesCenteredAt(this.xPositionOnScreen + centerX, this.yPositionOnScreen + centerY - 36, 5, 72, 72, 12));
                    toAddTo.AddRange(this.createRowOfBoxesCenteredAt(this.xPositionOnScreen + centerX, this.yPositionOnScreen + centerY + 40, 5, 72, 72, 12));
                    break;
                case 11:
                    toAddTo.AddRange(this.createRowOfBoxesCenteredAt(this.xPositionOnScreen + centerX, this.yPositionOnScreen + centerY - 36, 6, 72, 72, 12));
                    toAddTo.AddRange(this.createRowOfBoxesCenteredAt(this.xPositionOnScreen + centerX, this.yPositionOnScreen + centerY + 40, 5, 72, 72, 12));
                    break;
                case 12:
                    toAddTo.AddRange(this.createRowOfBoxesCenteredAt(this.xPositionOnScreen + centerX, this.yPositionOnScreen + centerY - 36, 6, 72, 72, 12));
                    toAddTo.AddRange(this.createRowOfBoxesCenteredAt(this.xPositionOnScreen + centerX, this.yPositionOnScreen + centerY + 40, 6, 72, 72, 12));
                    break;
            }
        }

        private List<Rectangle> createRowOfBoxesCenteredAt(
          int xStart,
          int yStart,
          int numBoxes,
          int boxWidth,
          int boxHeight,
          int horizontalGap)
        {
            var ofBoxesCenteredAt = new List<Rectangle>();
            var num = xStart - numBoxes * (boxWidth + horizontalGap) / 2;
            var y = yStart - boxHeight / 2;
            for (var index = 0; index < numBoxes; ++index)
                ofBoxesCenteredAt.Add(new Rectangle(num + index * (boxWidth + horizontalGap), y, boxWidth, boxHeight));
            return ofBoxesCenteredAt;
        }

        public void takeDownBundleSpecificPage()
        {
            if (!this.isReadyToCloseMenuOrBundle())
            {
                return;
            }
            this.ReturnPartialDonations(false);
            this.hoveredItem = null;
            if (!this.specificBundlePage)
            {
                return;
            }
            this.specificBundlePage = false;
            this.ingredientSlots.Clear();
            this.ingredientList.Clear();
            tempSprites.Clear();
            this.purchaseButton = null;
            if (!Game1.options.SnappyMenus)
            {
                return;
            }
            if (this.currentPageBundle != null)
            {
                this.currentlySnappedComponent = this.currentPageBundle;
                this.snapCursorToCurrentSnappedComponent();
            }
            else
            {
                this.snapToDefaultClickableComponent();
            }
        }

        private Point getBundleLocationFromNumber(int whichBundle)
        {
            var locationFromNumber = new Point(this.xPositionOnScreen, this.yPositionOnScreen);
            switch (whichBundle)
            {
                case 0:
                    locationFromNumber.X += 592;
                    locationFromNumber.Y += 136;
                    break;
                case 1:
                    locationFromNumber.X += 392;
                    locationFromNumber.Y += 384;
                    break;
                case 2:
                    locationFromNumber.X += 784;
                    locationFromNumber.Y += 388;
                    break;
                case 3:
                    locationFromNumber.X += 304;
                    locationFromNumber.Y += 252;
                    break;
                case 4:
                    locationFromNumber.X += 892;
                    locationFromNumber.Y += 252;
                    break;
                case 5:
                    locationFromNumber.X += 588;
                    locationFromNumber.Y += 276;
                    break;
                case 6:
                    locationFromNumber.X += 588;
                    locationFromNumber.Y += 380;
                    break;
                case 7:
                    locationFromNumber.X += 440;
                    locationFromNumber.Y += 164;
                    break;
                case 8:
                    locationFromNumber.X += 776;
                    locationFromNumber.Y += 164;
                    break;
            }
            return locationFromNumber;
        }
    }
}
