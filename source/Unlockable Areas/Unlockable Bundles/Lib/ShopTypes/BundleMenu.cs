/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-unlockable-areas
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewValley.Objects;
using StardewValley.BellsAndWhistles;
using Unlockable_Bundles.Lib.Enums;
using Unlockable_Bundles.Lib.AdvancedPricing;
using static Unlockable_Bundles.ModEntry;

namespace Unlockable_Bundles.Lib.ShopTypes
{
    //CCBundleMenu
    internal class BundleMenu : IClickableMenu
    {
        public Unlockable Unlockable;
        public Farmer Who;

        public bool CanClick = true;
        private bool ClickableComponentsDirty = false;
        public ScreenSwipe ScreenSwipe;
        public List<TemporaryAnimatedSprite> TempSprites = new();

        public string HoverText = "";
        private ClickableRequirementTexture HoveredComponent = null;
        private Item HoveredItem = null;
        private Item HeldItem = null;
        public int CompletionTimer;
        public bool Complete;
        private int Cursor;

        public InventoryMenu InventoryMenu;
        public Item PartialDonationItem;
        public List<Item> PartialDonationComponents = new();
        public KeyValuePair<string, int> CurrentPartialRequirement { get => Unlockable._price.Pairs.ElementAtOrDefault(CurrentPartialRequirementIndex); }
        public int CurrentPartialRequirementIndex = -1;

        public Texture2D BackgroundTexture;
        public Texture2D JunimoTexture;
        public Texture2D IconTexture;
        public static Texture2D MoneyTexture;

        public List<ClickableRequirementTexture> RequirementSlots = new();
        public List<ClickableRequirementTexture> AlreadyPaidSlots = new();

        public static void Initialize()
        {
            Helper.Events.GameLoop.GameLaunched += delegate {
                MoneyTexture = UtilityMisc.createSubTexture(Game1.mouseCursors, new Rectangle(280, 412, 15, 14));
            };
        }

        public BundleMenu(Farmer who, Unlockable unlockable, ShopType shopType)
            : base(Game1.uiViewport.Width / 2 - 640, Game1.uiViewport.Height / 2 - 360, 1280, 720, showUpperRightCloseButton: true)
        {
            Game1.playSound("bigSelect");
            Game1.dialogueUp = true;
            Game1.player.CanMove = false;

            Unlockable = unlockable;
            Who = who;

            JunimoTexture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\JunimoNote");

            if (Unlockable.JunimoNoteTexture == "")
                BackgroundTexture = JunimoTexture;
            else
                BackgroundTexture = Helper.GameContent.Load<Texture2D>(Unlockable.JunimoNoteTexture);

            resetUI();
        }

        public void customPopulateClickableComponentList()
        {
            resetSnappyInventory();

            allClickableComponents = new List<ClickableComponent>();
            allClickableComponents.AddRange(InventoryMenu.inventory);
            allClickableComponents.AddRange(RequirementSlots);
            allClickableComponents.AddRange(AlreadyPaidSlots);

            if (Game1.options.SnappyMenus)
                snapToDefaultClickableComponent();
        }
        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds) => resetUI();

        public void resetUI()
        {
            xPositionOnScreen = Game1.uiViewport.Width / 2 - 640;
            yPositionOnScreen = Game1.uiViewport.Height / 2 - 360;

            InventoryMenu = new InventoryMenu(xPositionOnScreen + 128, yPositionOnScreen + 140, playerInventory: true, null, HighlightObjects, 36, 6, 8, 8, drawSlots: false);
            upperRightCloseButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width - 36, yPositionOnScreen - 8, 48, 48), Game1.mouseCursors, new Rectangle(337, 494, 12, 12), 4f);

            ReturnPartialDonations();

            List<Rectangle> ingredientSlotRectangles = new List<Rectangle>();
            addRectangleRowsToList(ingredientSlotRectangles, Unlockable.BundleSlots, xPositionOnScreen + 932, yPositionOnScreen + 540);
            AlreadyPaidSlots.Clear();
            for (int j = 0; j < ingredientSlotRectangles.Count; j++)
                AlreadyPaidSlots.Add(new ClickableRequirementTexture(ingredientSlotRectangles[j], BackgroundTexture, new Rectangle(512, 244, 18, 18), 4f) {
                    myID = j + 250,
                    upNeighborID = -99998,
                    rightNeighborID = -99998,
                    leftNeighborID = -99998,
                    downNeighborID = -99998
                });

            RequirementSlots = createRequirementTextures(xPositionOnScreen + 932, yPositionOnScreen + 364, Unlockable);
            customPopulateClickableComponentList();

            updateAlreadyPaidSlots();
        }

        public static List<ClickableRequirementTexture> createRequirementTextures(int x, int y, Unlockable unlockable, int page = 0, int maxPerPage = 12)
        {
            List<ClickableRequirementTexture> requirementSlots = new();
            List<Rectangle> ingredientListRectangles = new List<Rectangle>();
            addRectangleRowsToList(ingredientListRectangles, Math.Min(unlockable._price.Count() - maxPerPage * page, maxPerPage), x, y);
            for (int i = 0; i < ingredientListRectangles.Count; i++) {
                var paginatedIndex = page * maxPerPage + i;
                var requirement = unlockable._price.Pairs.ElementAt(paginatedIndex);
                var id = Unlockable.getFirstIDFromReqKey(requirement.Key);

                if (id == "money") {
                    requirementSlots.Add(new ClickableRequirementTexture("", ingredientListRectangles[i], "", "", MoneyTexture, new Rectangle(0, 0, 15, 14), 4f) {
                        ReqKey = requirement.Key,
                        ReqValue = requirement.Value,
                        ReqItemId = id,
                        myID = i + 1000,
                        hoverText = requirement.Value.ToString("# ### ##0") + "g  ",
                        upNeighborID = -99998,
                        rightNeighborID = -99998,
                        leftNeighborID = -99998,
                        downNeighborID = -99998
                    });
                    continue;
                }

                var obj = Unlockable.parseItem(id, unlockable._price.Pairs.ElementAt(paginatedIndex).Value, Unlockable.getFirstQualityFromReqKey(requirement.Key));

                requirementSlots.Add(new ClickableRequirementTexture("", ingredientListRectangles[i], "", obj.DisplayName, Game1.objectSpriteSheet, Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, obj.ParentSheetIndex, 16, 16), 4f) {
                    ReqKey = requirement.Key,
                    ReqValue = requirement.Value,
                    ReqItemId = id,
                    myID = i + 1000,
                    item = obj,
                    upNeighborID = -99998,
                    rightNeighborID = -99998,
                    leftNeighborID = -99998,
                    downNeighborID = -99998
                });
            }

            return requirementSlots;
        }

        private void resetSnappyInventory()
        {
            foreach (var inv in InventoryMenu.inventory) {
                inv.upNeighborID = -99998;
                inv.rightNeighborID = -99998;
                inv.leftNeighborID = -99998;
                inv.downNeighborID = -99998;
            }
        }

        private void updateAlreadyPaidSlots()
        {
            TempSprites.Clear();
            for (int i = 0; i < Unlockable._price.Pairs.Count(); i++) {
                var req = Unlockable._price.Pairs.ElementAt(i);

                if (Unlockable._alreadyPaid.ContainsKey(req.Key) /*&& slotNumber < AlreadyPaidSlots.Count*/) {
                    string id = Unlockable.getFirstIDFromReqKey(req.Key);
                    var slot = AlreadyPaidSlots[Unlockable._alreadyPaidIndex[req.Key]];

                    ingredientDepositAnimation(slot, skipAnimation: true);

                    slot.ReqKey = req.Key;
                    slot.ReqValue = req.Value;
                    slot.ReqItemId = id;

                    if (id == "money")
                        continue;

                    var obj = Unlockable.parseItem(id, req.Value, Unlockable.getFirstQualityFromReqKey(req.Key));

                    slot.item = obj;
                }
            }
        }

        private static void addRectangleRowsToList(List<Rectangle> toAddTo, int numberOfItems, int x, int y)
        {
            switch (numberOfItems) {
                case 1:
                    toAddTo.AddRange(createRowOfBoxesCenteredAt(x, y, 1, 72, 72, 12));
                    break;
                case 2:
                    toAddTo.AddRange(createRowOfBoxesCenteredAt(x, y, 2, 72, 72, 12));
                    break;
                case 3:
                    toAddTo.AddRange(createRowOfBoxesCenteredAt(x, y, 3, 72, 72, 12));
                    break;
                case 4:
                    toAddTo.AddRange(createRowOfBoxesCenteredAt(x, y, 4, 72, 72, 12));
                    break;
                case 5:
                    toAddTo.AddRange(createRowOfBoxesCenteredAt(x, y - 36, 3, 72, 72, 12));
                    toAddTo.AddRange(createRowOfBoxesCenteredAt(x, y + 40, 2, 72, 72, 12));
                    break;
                case 6:
                    toAddTo.AddRange(createRowOfBoxesCenteredAt(x, y - 36, 3, 72, 72, 12));
                    toAddTo.AddRange(createRowOfBoxesCenteredAt(x, y + 40, 3, 72, 72, 12));
                    break;
                case 7:
                    toAddTo.AddRange(createRowOfBoxesCenteredAt(x, y - 36, 4, 72, 72, 12));
                    toAddTo.AddRange(createRowOfBoxesCenteredAt(x, y + 40, 3, 72, 72, 12));
                    break;
                case 8:
                    toAddTo.AddRange(createRowOfBoxesCenteredAt(x, y - 36, 4, 72, 72, 12));
                    toAddTo.AddRange(createRowOfBoxesCenteredAt(x, y + 40, 4, 72, 72, 12));
                    break;
                case 9:
                    toAddTo.AddRange(createRowOfBoxesCenteredAt(x, y - 36, 5, 72, 72, 12));
                    toAddTo.AddRange(createRowOfBoxesCenteredAt(x, y + 40, 4, 72, 72, 12));
                    break;
                case 10:
                    toAddTo.AddRange(createRowOfBoxesCenteredAt(x, y - 36, 5, 72, 72, 12));
                    toAddTo.AddRange(createRowOfBoxesCenteredAt(x, y + 40, 5, 72, 72, 12));
                    break;
                case 11:
                    toAddTo.AddRange(createRowOfBoxesCenteredAt(x, y - 36, 6, 72, 72, 12));
                    toAddTo.AddRange(createRowOfBoxesCenteredAt(x, y + 40, 5, 72, 72, 12));
                    break;
                case 12:
                    toAddTo.AddRange(createRowOfBoxesCenteredAt(x, y - 36, 6, 72, 72, 12));
                    toAddTo.AddRange(createRowOfBoxesCenteredAt(x, y + 40, 6, 72, 72, 12));
                    break;
            }
        }

        private static List<Rectangle> createRowOfBoxesCenteredAt(int xStart, int yStart, int numBoxes, int boxWidth, int boxHeight, int horizontalGap)
        {
            List<Rectangle> rectangles = new List<Rectangle>();
            int actualXStart = xStart - numBoxes * (boxWidth + horizontalGap) / 2;
            int actualYStart = yStart - boxHeight / 2;
            for (int i = 0; i < numBoxes; i++)
                rectangles.Add(new Rectangle(actualXStart + i * (boxWidth + horizontalGap), actualYStart, boxWidth, boxHeight));

            return rectangles;
        }

        public virtual bool HighlightObjects(Item item)
        {
            if (PartialDonationItem != null && CurrentPartialRequirementIndex >= 0)
                return IsValidItemForThisRequirement(item, CurrentPartialRequirement);

            return RequirementSlots.Any(el => reqContainsType(el.ReqKey, item.TypeDefinitionId) && !Unlockable._alreadyPaid.ContainsKey(el.ReqKey));
        }

        public bool reqContainsType(string reqKey, string type)
        {
            var items = Unlockable.getRequiredItems(reqKey);
            return items.Any(
                el => el.TypeDefinitionId == type
                || (el is AdvancedPricingItem apItem && apItem.ItemTypes.Contains(type)));
        }

        private bool IsValidItemForThisRequirement(Item comparedItem, KeyValuePair<string, int> requirement)
        {
            if (Unlockable._alreadyPaid.ContainsKey(requirement.Key))
                return false;

            var items = Unlockable.getRequiredItems(requirement.Key);
            return items.Any(el => Inventory.isItemValid(el, comparedItem));
        }

        private static bool isExceptionItem(string id) => Unlockable.isExceptionItem(id);

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            if (!CanClick)
                return;

            HeldItem = InventoryMenu.rightClick(x, y, HeldItem);

            if (PartialDonationItem == null)
                return;

            for (int i = 0; i < AlreadyPaidSlots.Count; i++) {
                if (!AlreadyPaidSlots[i].containsPoint(x, y) || AlreadyPaidSlots[i].item != PartialDonationItem)
                    continue;

                if (PartialDonationComponents.Count <= 0)
                    break;

                Item item = PartialDonationComponents[0].getOne();
                bool valid = false;
                if (HeldItem == null) {
                    HeldItem = item;
                    Game1.playSound("dwop");
                    valid = true;
                } else if (HeldItem.canStackWith(item)) {
                    HeldItem.addToStack(item);
                    Game1.playSound("dwop");
                    valid = true;
                }
                if (!valid) {
                    break;
                }

                PartialDonationComponents[0].Stack--;
                if (PartialDonationComponents[0].Stack <= 0) {
                    PartialDonationComponents.RemoveAt(0);
                }
                int count = PartialDonationComponents.Sum(e => e.Stack);

                if (PartialDonationItem != null)
                    PartialDonationItem.Stack = count;

                if (PartialDonationComponents.Count == 0)
                    ResetPartialDonation();
                break;
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (!CanClick)
                return;

            if (CompletionTimer <= 0)
                HeldItem = InventoryMenu.leftClick(x, y, HeldItem);

            if (upperRightCloseButton.containsPoint(x, y) && isReadyToClose())
                exitThisMenu();

            if (PartialDonationItem != null)
                partialDonationLeftClick(x, y);
            else if (HeldItem != null)
                heldItemLeftClick(x, y);

            if (HoveredComponent != null
                && isExceptionItem(HoveredComponent.ReqItemId)
                && HoveredComponent.bounds.Contains(x, y)
                && PartialDonationItem == null
                && !Unlockable._alreadyPaid.ContainsKey(HoveredComponent.ReqKey)
                && !Complete
                && !Unlockable.allRequirementsPaid())
                tryPayExceptionItem(HoveredComponent);
        }

        public void tryPayExceptionItem(ClickableRequirementTexture c)
        {
            var requirement = new KeyValuePair<string, int>(c.ReqKey, c.ReqValue);

            if (!Inventory.hasEnoughItems(Who, requirement)) {
                Game1.playSound("bigDeSelect");
                return;
            }

            for (int i = 0; i < Unlockable._price.Pairs.Count(); i++) {
                if (AlreadyPaidSlots[i].ReqKey != null)
                    continue;

                AlreadyPaidSlots[i].ReqItemId = c.ReqItemId;
                AlreadyPaidSlots[i].ReqKey = c.ReqKey;
                AlreadyPaidSlots[i].ReqValue = c.ReqValue;

                Inventory.removeItemsOfRequirement(Who, requirement);
                Unlockable.processContribution(new KeyValuePair<string, int>(c.ReqKey, c.ReqValue), i);

                if (c.ReqItemId != "money") {
                    var obj = Unlockable.parseItem(c.ReqItemId, c.ReqValue);

                    AlreadyPaidSlots[i].item = obj;
                }

                announceDonation(AlreadyPaidSlots[i]);
                checkIfBundleIsComplete();
                break;
            }

        }

        private void heldItemLeftClick(int x, int y)
        {
            var shiftDown = isShiftDown();

            for (int k = 0; k < AlreadyPaidSlots.Count; k++)
                if (AlreadyPaidSlots[k].containsPoint(x, y) || shiftDown)
                    if (canAcceptThisItem(HeldItem, AlreadyPaidSlots[k])) {
                        HeldItem = tryToDepositThisItem(HeldItem, AlreadyPaidSlots[k]);

                        if (shiftDown && HeldItem is not null) {
                            Game1.player.addItemToInventory(HeldItem);
                            HeldItem = null;
                        }

                        checkIfBundleIsComplete();
                        return;

                    } else if (AlreadyPaidSlots[k].ReqKey == null)
                        HandlePartialDonation(HeldItem, AlreadyPaidSlots[k]);
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);

            HoverText = "";
            HoveredItem = null;
            HoveredComponent = null;

            if (!Complete && CompletionTimer <= 0)
                HoveredItem = InventoryMenu.hover(x, y, HeldItem);

            foreach (ClickableRequirementTexture c in RequirementSlots) {
                if (isExceptionItem(c.ReqItemId) && !Unlockable._alreadyPaid.ContainsKey(c.ReqKey))
                    c.tryHover(x, y, 0.2f);

                if (c.bounds.Contains(x, y))
                    HoveredComponent = c;
            }

            if (HeldItem == null)
                return;

            foreach (ClickableTextureComponent c in AlreadyPaidSlots)
                if (c.bounds.Contains(x, y) && CanBePartiallyOrFullyDonated(HeldItem) && (PartialDonationItem == null || c.item == PartialDonationItem)) {
                    c.sourceRect.X = 530;
                    c.sourceRect.Y = 262;
                } else {
                    c.sourceRect.X = 512;
                    c.sourceRect.Y = 244;
                }
        }

        public override void snapToDefaultClickableComponent()
        {
            base.currentlySnappedComponent = base.getComponentWithID(0);
            this.snapCursorToCurrentSnappedComponent();
        }

        protected override bool _ShouldAutoSnapPrioritizeAlignedElements() => false;

        public override void setUpForGamePadMode() => ClickableComponentsDirty = true;

        public override void automaticSnapBehavior(int direction, int oldRegion, int oldID)
        {
            if (ClickableComponentsDirty) {
                ClickableComponentsDirty = false;
                customPopulateClickableComponentList();
            }
            base.automaticSnapBehavior(direction, oldRegion, oldID);
        }
        private bool isReadyToClose() => CompletionTimer <= 0; //&& HeldItem == null;

        public override void update(GameTime time)
        {
            if (isReadyToClose() && Complete)
                ReturnPartialDonations(to_hand: false);

            if (CompletionTimer > 0 && ScreenSwipe == null) {
                CompletionTimer -= time.ElapsedGameTime.Milliseconds;
                if (CompletionTimer <= 0)
                    Unlockable.processShopEvent();
            }

            for (int i = TempSprites.Count - 1; i >= 0; i--)
                if (TempSprites[i].update(time))
                    TempSprites.RemoveAt(i);

            if (ScreenSwipe != null) {
                CanClick = false;
                if (ScreenSwipe.update(time)) {
                    ScreenSwipe = null;
                    CanClick = true;
                }
            }
        }

        private bool isShiftDown() => Game1.oldKBState.IsKeyDown(Keys.LeftShift) || Game1.oldPadState.IsButtonDown(Buttons.RightTrigger) || Game1.oldPadState.IsButtonDown(Buttons.LeftTrigger);

        private void partialDonationLeftClick(int x, int y)
        {
            if (HeldItem != null && isShiftDown()) {
                for (int i = 0; i < AlreadyPaidSlots.Count; i++) {
                    if (AlreadyPaidSlots[i].item == PartialDonationItem) {
                        HandlePartialDonation(HeldItem, AlreadyPaidSlots[i]);
                        if (HeldItem != null)
                            Game1.player.addItemToInventory(HeldItem);
                        HeldItem = null;
                        return;
                    }
                }
            } else {
                for (int l = 0; l < this.AlreadyPaidSlots.Count; l++) {
                    if (AlreadyPaidSlots[l].containsPoint(x, y) && AlreadyPaidSlots[l].item == PartialDonationItem) {
                        if (HeldItem != null) {
                            HandlePartialDonation(HeldItem, AlreadyPaidSlots[l]);
                            return;
                        }
                        bool return_to_inventory = isShiftDown();
                        this.ReturnPartialDonations(!return_to_inventory);
                        return;
                    }
                }
            }
        }

        public bool canAcceptThisItem(Item item, ClickableRequirementTexture slot) => canAcceptThisItem(item, slot, ignore_stack_count: false);

        public bool canAcceptThisItem(Item item, ClickableRequirementTexture slot, bool ignore_stack_count = false)
        {
            if (item is null)
                return false;

            for (int i = 0; i < Unlockable._price.Count(); i++)
                if (IsValidItemForThisRequirement(item, Unlockable._price.Pairs.ElementAt(i))
                    && (ignore_stack_count || Unlockable._price.Pairs.ElementAt(i).Value <= item.Stack)
                    && (slot == null || slot.ReqKey == null))
                    return true;

            return false;
        }

        public virtual void HandlePartialDonation(Item heldItem, ClickableRequirementTexture slot)
        {
            if ((PartialDonationItem != null && slot.item != PartialDonationItem) || !CanBePartiallyOrFullyDonated(heldItem))
                return;

            if (CurrentPartialRequirement.Equals(default(KeyValuePair<string, int>)))
                CurrentPartialRequirementIndex = GetIngredientIndexForItem(heldItem);

            if (CurrentPartialRequirement.Equals(default(KeyValuePair<string, int>)) || !IsValidItemForThisRequirement(heldItem, CurrentPartialRequirement))
                return;

            if (slot.ReqKey == null) {
                PartialDonationItem = Unlockable.parseItem(Unlockable.getFirstIDFromReqKey(CurrentPartialRequirement.Key), 0, Unlockable.getFirstQualityFromReqKey(CurrentPartialRequirement.Key));

                slot.ReqItemId = Unlockable.getFirstIDFromReqKey(CurrentPartialRequirement.Key);
                slot.ReqKey = CurrentPartialRequirement.Key;
                slot.ReqValue = CurrentPartialRequirement.Value;
                slot.item = PartialDonationItem;

                slot.sourceRect.X = 512;
                slot.sourceRect.Y = 244;
            }

            int amount_to_donate = Math.Min(CurrentPartialRequirement.Value - PartialDonationItem.Stack, heldItem.Stack);
            PartialDonationItem.Stack += amount_to_donate;
            heldItem.Stack -= amount_to_donate;

            if (amount_to_donate > 0) {
                Item donated_item = this.HeldItem.getOne();
                donated_item.Stack = amount_to_donate;
                foreach (Item contributed_item in PartialDonationComponents)
                    if (contributed_item.canStackWith(HeldItem))
                        donated_item.Stack = contributed_item.addToStack(donated_item);

                if (donated_item.Stack > 0)
                    PartialDonationComponents.Add(donated_item);

                PartialDonationComponents.Sort((Item a, Item b) => b.Stack.CompareTo(a.Stack));
            }

            if (heldItem.Stack <= 0 && heldItem == HeldItem)
                HeldItem = null;

            if (PartialDonationItem.Stack >= CurrentPartialRequirement.Value) {
                resetSlot(slot);

                PartialDonationItem = tryToDepositThisItem(PartialDonationItem, slot);
                if (PartialDonationItem != null && PartialDonationItem.Stack > 0) {
                    ReturnPartialDonation(PartialDonationItem);
                }
                PartialDonationItem = null;
                ResetPartialDonation();
                checkIfBundleIsComplete();
            } else if (amount_to_donate > 0) {
                Game1.playSound("sell");
            }
        }

        public virtual void ReturnPartialDonations(bool to_hand = true, bool includeHeldItem = false)
        {
            bool play_sound = true;
            foreach (Item item in PartialDonationComponents) {
                if (HeldItem == null && to_hand) {
                    Game1.playSound("dwop");
                    HeldItem = item;
                } else {
                    ReturnPartialDonation(item, play_sound);
                    play_sound = false;
                }
            }

            if (includeHeldItem && HeldItem is not null) {
                ReturnPartialDonation(HeldItem, play_sound);
                HeldItem = null;
            }

            ResetPartialDonation();
        }

        private void checkIfBundleIsComplete()
        {
            ReturnPartialDonations();

            if (!Unlockable.allRequirementsPaid())
                return;

            if (HeldItem != null) {
                Game1.player.addItemToInventory(HeldItem);
                HeldItem = null;
            }

            Unlockable.processPurchase();

            ScreenSwipe = new ScreenSwipe(0);
            CompletionTimer = 800;
            Complete = true;
            CanClick = false;
        }


        public virtual void ResetPartialDonation()
        {
            PartialDonationComponents.Clear();
            CurrentPartialRequirementIndex = -1;
            foreach (ClickableRequirementTexture slot in AlreadyPaidSlots)
                if (slot.item == PartialDonationItem && slot.item != null)
                    resetSlot(slot);


            PartialDonationItem = null;
        }

        public void resetSlot(ClickableRequirementTexture slot)
        {
            slot.item = null;
            slot.ReqKey = null;
            slot.ReqItemId = null;
            slot.ReqValue = 0;
        }


        public virtual void ReturnPartialDonation(Item item, bool play_sound = true)
        {
            List<Item> affected_items = new List<Item>();
            Item remainder = Game1.player.addItemToInventory(item, affected_items);
            foreach (Item affected_item in affected_items) {
                InventoryMenu.ShakeItem(affected_item);
            }
            if (remainder != null) {
                Utility.CollectOrDrop(remainder);
                InventoryMenu.ShakeItem(remainder);
            }
            if (play_sound) {
                Game1.playSound("coin");
            }
        }

        public Item tryToDepositThisItem(Item item, ClickableRequirementTexture slot)
        {
            for (int i = 0; i < Unlockable._price.Pairs.Count(); i++) {
                var requirement = Unlockable._price.Pairs.ElementAt(i);

                if (IsValidItemForThisRequirement(item, requirement) && slot.ReqKey == null) {
                    item.Stack -= requirement.Value;
                    Unlockable.processContribution(requirement, AlreadyPaidSlots.IndexOf(slot));

                    string id = Unlockable.getFirstIDFromReqKey(requirement.Key);

                    slot.ReqItemId = id;
                    slot.ReqKey = requirement.Key;
                    slot.ReqValue = requirement.Value;

                    var obj = Unlockable.parseItem(id, requirement.Value, Unlockable.getFirstQualityFromReqKey(requirement.Key));

                    slot.item = obj;

                    announceDonation(slot);


                    break;
                }
            }
            if (item.Stack > 0)
                return item;

            return null;
        }

        private void announceDonation(ClickableRequirementTexture slot)
        {
            ingredientDepositAnimation(slot);
            Game1.playSound("newArtifact");
            slot.sourceRect.X = 512;
            slot.sourceRect.Y = 244;
            var displayName = slot.ReqItemId == "money" ? slot.ReqValue.ToString("# ### ##0") + "g" : slot.item.DisplayName;
            Helper.Reflection.GetField<StardewValley.Multiplayer>(typeof(Game1), "multiplayer").GetValue().globalChatInfoMessage("BundleDonate", Game1.player.displayName, displayName);
        }

        public void ingredientDepositAnimation(ClickableTextureComponent slot, bool skipAnimation = false)
        {
            TemporaryAnimatedSprite t = new TemporaryAnimatedSprite(null, new Rectangle(530, 244, 18, 18), 50f, 6, 1, new Vector2(slot.bounds.X, slot.bounds.Y), flicker: false, flipped: false, 0.88f, 0f, Color.White, 4f, 0f, 0f, 0f, local: true) {
                holdLastFrame = true,
                endSound = "cowboy_monsterhit",
                texture = BackgroundTexture,
            };
            if (skipAnimation) {
                t.sourceRect.Offset(t.sourceRect.Width * 5, 0);
                t.sourceRectStartingPos = new Vector2(t.sourceRect.X, t.sourceRect.Y);
                t.animationLength = 1;
            }
            TempSprites.Add(t);
        }

        public virtual bool CanBePartiallyOrFullyDonated(Item item)
        {
            int index = GetIngredientIndexForItem(item);
            if (index < 0) {
                return false;
            }
            var requirement = Unlockable._price.Pairs.ElementAt(index);
            int count = 0;
            if (IsValidItemForThisRequirement(item, requirement))
                count += item.Stack;

            foreach (Item inventory_item in Game1.player.Items.Where(el => el is not null))
                if (IsValidItemForThisRequirement(inventory_item, requirement))
                    count += inventory_item.Stack;

            if (index == CurrentPartialRequirementIndex && PartialDonationItem != null) {
                count += PartialDonationItem.Stack;
            }
            return count >= requirement.Value;
        }

        public int GetIngredientIndexForItem(Item item)
        {
            for (int i = 0; i < Unlockable._price.Count(); i++)
                if (IsValidItemForThisRequirement(item, Unlockable._price.Pairs.ElementAt(i)))
                    return i;

            return -1;
        }

        public override bool readyToClose()
        {
            if (!isReadyToClose())
                return false;

            GamePadState currentPadState = Game1.input.GetGamePadState();
            KeyboardState keyState = Game1.GetKeyboardState();

            if (((currentPadState.IsButtonDown(Buttons.Start) && !Game1.oldPadState.IsButtonDown(Buttons.Start)) || (currentPadState.IsButtonDown(Buttons.B) && !Game1.oldPadState.IsButtonDown(Buttons.B))))
                exitThisMenu();

            if (keyState.IsKeyDown(Keys.Escape))
                exitThisMenu();

            if (Game1.options.menuButton.Any(e => keyState.IsKeyDown(e.key)))
                exitThisMenu();

            return false;
        }

        public new void exitThisMenu(bool playSound = true)
        {
            ReturnPartialDonations(false, true);
            Game1.dialogueUp = false;
            Game1.player.CanMove = true;
            base.exitThisMenu(playSound);
        }

        public Texture2D getIconTexture()
        {
            if (IconTexture != null)
                return IconTexture;



            if (Unlockable.BundleIconAsset != "") {
                IconTexture = Helper.GameContent.Load<Texture2D>(Unlockable.BundleIconAsset);
                return IconTexture;
            }

            int index = Unlockable.BundleIcon switch {
                BundleIconType.Spring_Crops => 0,
                BundleIconType.Summer_Crops => 1,
                BundleIconType.Fall_Crops => 2,
                BundleIconType.Quality_Crops => 3,
                BundleIconType.Animal => 4,
                BundleIconType.Artisan => 5,

                BundleIconType.River_Fish => 6,
                BundleIconType.Lake_Fish => 7,
                BundleIconType.Ocean_Fish => 8,
                BundleIconType.Night_Fishing => 9,
                BundleIconType.Specialty_Fish => 10,
                BundleIconType.Crab_Pot => 11,
                BundleIconType.Master_Fisher => 12,

                BundleIconType.Spring_Foraging => 13,
                BundleIconType.Summer_Foraging => 14,
                BundleIconType.Fall_Foraging => 15,
                BundleIconType.Winter_Foraging => 16,
                BundleIconType.Construction => 17,
                BundleIconType.Fruit_Tree => 18,
                BundleIconType.Exotic_Foraging => 19,

                BundleIconType.Blacksmith => 20,
                BundleIconType.Geologist => 21,
                BundleIconType.Adventurer => 22,

                BundleIconType.Small_Money => 23,
                BundleIconType.Medium_Money => 24,
                BundleIconType.Large_Money => 25,
                BundleIconType.Extra_Large_Money => 26,

                BundleIconType.Spring_Fishing => 27,
                BundleIconType.Summer_Fishing => 28,
                BundleIconType.Fall_Fishing => 29,
                BundleIconType.Winter_Fishing => 30,

                BundleIconType.Chef => 31,
                BundleIconType.Field_Research => 32,
                BundleIconType.Enchanter => 33,
                BundleIconType.Dye => 34,
                BundleIconType.Fodder => 35,

                BundleIconType.Junimo => 36,

                _ => -1
            };

            var xOffset = index >= 20 ? 32 * (index - 20) : 32 * index;
            var yOffset = index >= 20 ? 32 : 0;

            if (index != -1) {
                IconTexture = UtilityMisc.createSubTexture(JunimoTexture, new Rectangle(xOffset, 180 + yOffset, 32, 32));
                return IconTexture;
            }

            index = Unlockable.BundleIcon switch {
                BundleIconType.Children => 0,
                BundleIconType.Treasure_Hunter => 1,
                BundleIconType.Brewer => 2,
                BundleIconType.Rare_Crops => 3,
                BundleIconType.Quality_Fish => 4,
                BundleIconType.Forager => 5,
                BundleIconType.Home_Cook => 6,
                BundleIconType.Garden => 7,
                BundleIconType.Fish_Farmer => 8,
                BundleIconType.Wild_Medicine => 9,
                BundleIconType.Sticky => 10,
                BundleIconType.Volcano => 11,
                _ => 2,
            };

            var bundleSprites = Helper.GameContent.Load<Texture2D>("LooseSprites\\BundleSprites");
            xOffset = index >= 20 ? 32 * (index - 20) : 32 * index;
            yOffset = index >= 20 ? 32 : 0;
            IconTexture = UtilityMisc.createSubTexture(bundleSprites, new Rectangle(xOffset, yOffset, 32, 32));
            return IconTexture;
        }

        public override void draw(SpriteBatch b)
        {
            Cursor = -1;
            b.Draw(Game1.fadeToBlackRect, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), Color.Black * 0.5f);

            drawMenu(b);
            drawTemporarySprites(b);
            drawAlreadyPaidSlots(b);
            drawRequirementSlots(b);

            drawDescription(b);

            upperRightCloseButton.draw(b);
            InventoryMenu.draw(b);

            if (CanClick)
                drawMouse(b, cursor: Cursor);

            if (HeldItem != null)
                HeldItem.drawInMenu(b, new Vector2(Game1.getOldMouseX() + 16, Game1.getOldMouseY() + 16), 1f);

            if (InventoryMenu.descriptionText.Length > 0 && HoveredItem != null)
                drawToolTip(b, HoveredItem.getDescription(), HoveredItem.DisplayName, HoveredItem);
            else if (HoveredComponent != null && HoveredComponent.item != null)
                drawToolTip(b, HoveredComponent.item.getDescription(), HoveredComponent.item.DisplayName, HoveredComponent.item);
            else if (HoveredComponent != null)
                drawHoverText(b, HoveredComponent.hoverText, Game1.dialogueFont);
            else
                drawHoverText(b, HoverText, Game1.dialogueFont);

            if (ScreenSwipe != null)
                ScreenSwipe.draw(b);
        }

        public void drawDescription(SpriteBatch b)
        {
            if (Unlockable.BundleDescription != "")
                SpriteText.drawStringWithScrollCenteredAt(b, Unlockable.BundleDescription, base.xPositionOnScreen + base.width / 2, Math.Min(base.yPositionOnScreen + base.height + 20, Game1.uiViewport.Height - 64 - 8));
        }

        public void drawAlreadyPaidSlots(SpriteBatch b)
        {
            foreach (ClickableRequirementTexture c in AlreadyPaidSlots) {
                float alpha_mult = 1f;

                if (PartialDonationItem != null && c.item != PartialDonationItem)
                    alpha_mult = 0.25f;

                if (c.ReqKey == null || (PartialDonationItem != null && c.item == PartialDonationItem))
                    c.draw(b, Color.White * alpha_mult, 0.89f);

                if (c.ReqItemId == "money") {
                    b.Draw(MoneyTexture, new Vector2(c.bounds.X + 4, c.bounds.Y + 4), new Rectangle(0, 0, 15, 14), Color.White * alpha_mult, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.9f);
                    UtilityMisc.drawKiloFormat(b, c.ReqValue, c.bounds.X, c.bounds.Y, Color.White);
                }

                c.drawItem(b, 4, 4, alpha_mult);
            }
        }

        public void drawRequirementSlots(SpriteBatch b)
        {
            for (int i = 0; i < RequirementSlots.Count; i++) {
                float alpha_mult = 1f;

                if (CurrentPartialRequirementIndex >= 0 && CurrentPartialRequirementIndex != i)
                    alpha_mult = 0.25f;

                ClickableRequirementTexture c = RequirementSlots[i];
                bool completed = Unlockable._alreadyPaid.ContainsKey(c.ReqKey);

                //Background box on exception items indicating a possible purchase
                if (c == HoveredComponent && isExceptionItem(c.ReqItemId) && CurrentPartialRequirement.Key == null && !completed) {
                    b.Draw(BackgroundTexture, new Rectangle(c.bounds.X - 12, c.bounds.Y - 8, c.bounds.Width + 12, c.bounds.Height + 8), new Rectangle(530, 262, 18, 18), Color.White);
                    Cursor = 44;
                }

                if (!completed)
                    b.Draw(Game1.shadowTexture, new Vector2(c.bounds.Center.X - Game1.shadowTexture.Bounds.Width * 4 / 2 - 4, c.bounds.Center.Y + 4), Game1.shadowTexture.Bounds, Color.White * alpha_mult, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.1f);

                if (c.ReqItemId == "money")
                    c.draw(b, Color.White * (completed ? 0.25f : alpha_mult), 0.89f);
                else if (c.item != null && c.visible)
                    c.item.drawInMenu(b, new Vector2(c.bounds.X, c.bounds.Y), c.scale / 4f, 1f, 0.9f, StackDrawType.HideButShowQuality, Color.White * (completed ? 0.25f : alpha_mult), drawShadow: false);

                UtilityMisc.drawKiloFormat(b, c.ReqValue, c.bounds.X + 2, c.bounds.Y + 2, Color.White * (completed ? 0.25f : alpha_mult));
            }
        }

        public void drawMenu(SpriteBatch b)
        {
            b.Draw(BackgroundTexture, new Vector2(xPositionOnScreen, yPositionOnScreen), new Rectangle(320, 0, 320, 180), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.1f);

            Texture2D Icon = getIconTexture();
            b.Draw(Icon, new Vector2(xPositionOnScreen + 872, yPositionOnScreen + 88), new Rectangle(0, 0, Icon.Width, Icon.Height), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.15f);
            if (Unlockable.BundleName != "") {
                float textX = Game1.dialogueFont.MeasureString(Unlockable.BundleName).X;
                b.Draw(BackgroundTexture, new Vector2(xPositionOnScreen + 936 - (int)textX / 2 - 16, yPositionOnScreen + 228), new Rectangle(517, 266, 4, 17), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.1f);
                b.Draw(BackgroundTexture, new Rectangle(xPositionOnScreen + 936 - (int)textX / 2, yPositionOnScreen + 228, (int)textX, 68), new Rectangle(520, 266, 1, 17), Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.1f);
                b.Draw(BackgroundTexture, new Vector2(xPositionOnScreen + 936 + (int)textX / 2, yPositionOnScreen + 228), new Rectangle(524, 266, 4, 17), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.1f);
                b.DrawString(Game1.dialogueFont, Unlockable.BundleName, new Vector2(xPositionOnScreen + 936 - textX / 2f, yPositionOnScreen + 236) + new Vector2(2f, 2f), Game1.textShadowColor);
                b.DrawString(Game1.dialogueFont, Unlockable.BundleName, new Vector2(xPositionOnScreen + 936 - textX / 2f, yPositionOnScreen + 236) + new Vector2(0f, 2f), Game1.textShadowColor);
                b.DrawString(Game1.dialogueFont, Unlockable.BundleName, new Vector2(xPositionOnScreen + 936 - textX / 2f, yPositionOnScreen + 236) + new Vector2(2f, 0f), Game1.textShadowColor);
                b.DrawString(Game1.dialogueFont, Unlockable.BundleName, new Vector2(xPositionOnScreen + 936 - textX / 2f, yPositionOnScreen + 236), Game1.textColor * 0.9f);
            }
        }

        public void drawTemporarySprites(SpriteBatch b)
        {
            float completed_slot_alpha = 1f;
            if (PartialDonationItem != null)
                completed_slot_alpha = 0.25f;

            foreach (var temp in TempSprites)
                temp.draw(b, localPosition: true, 0, 0, completed_slot_alpha);
        }
    }
}