using Common.StardewValley.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Translation = StardewMods.ArchaeologyHouseContentManagementHelper.Common.Translation;

namespace StardewMods.ArchaeologyHouseContentManagementHelper.Framework
{
    /// <summary>
    /// This class extends the in-game collections page and adds a [Lost Books] sidetab to the page.
    /// </summary>
    internal class CollectionsPageEx : CollectionsPage
    {
        public const int region_sideTabLostBooks = 7008;

        private const int LostBooksTab_ItemBaseId = 0x70080000;

        public const int lostBooksTab = 7;

        private readonly int lostBooksTabPageIndex;

        private const int sideTabs_FirstIndex = region_sideTabShipped;

        // The number of side tabs of the Collection Page.
        private readonly int numTabs = 8;

        // Indicates whether the [Secret Notes] sidetab is shown or not.
        private readonly bool showSecretNotesTab;

        // The length of the preview of a book's content.
        private const int BOOK_PREVIEW_LENGTH = 22;

        // The distance between two book entries in the [Lost Books] collection side-tab.
        private const int BOOK_OFFSET = 85;

        private static ClickableComponent snappedItem;

        private readonly IReflectedField<string> hoverTextRef;
        private readonly IReflectedField<string> descriptionTextRef;
        private readonly IReflectedField<int> valueRef;

        public CollectionsPageEx(int x, int y, int width, int height,
            int selectedTab = organicsTab) : base(x, y, width, height)
        {
            hoverTextRef = ModEntry.CommonServices.ReflectionHelper.GetField<string>(this, "hoverText");
            descriptionTextRef = ModEntry.CommonServices.ReflectionHelper.GetField<string>(this, "descriptionText");
            valueRef = ModEntry.CommonServices.ReflectionHelper.GetField<int>(this, "value");

            showSecretNotesTab = Game1.player.secretNotesSeen.Count > 0;
            lostBooksTabPageIndex = showSecretNotesTab ? lostBooksTab : lostBooksTab - 1;

            // Loads the [Lost Books] side-tab texture.
            Texture2D bookTabTexture = ModEntry.CommonServices.ContentHelper.Load<Texture2D>("Assets/CollectionTab_LostBook.png", ContentSource.ModFolder);

            // Adds the [Lost Books] side-tab to the Collections Page.
            ClickableTextureComponent stLostBooks = new ClickableTextureComponent(
                name: "", 
                bounds: new Rectangle(this.xPositionOnScreen - 48, this.yPositionOnScreen + (showSecretNotesTab ? 576 : 512), 64, 64), 
                label: "", 
                hoverText: ModEntry.CommonServices.TranslationHelper.Get(Translation.GAMEMENU_COLLECTIONSPAGE_TAB_LABEL_LOST_BOOKS), 
                texture: bookTabTexture, 
                sourceRect: new Rectangle(0, 0, 16, 16),
                scale: 4f, 
                drawShadow: false)
            {
                myID = region_sideTabLostBooks,
                upNeighborID = showSecretNotesTab ? region_sideTabSecretNotes : region_sideTabAchivements,
                rightNeighborID = 0
            };

            this.sideTabs.Add(stLostBooks);
            this.collections.Add(lostBooksTabPageIndex, new List<List<ClickableTextureComponent>>());

            var prevSideTab = this.sideTabs[showSecretNotesTab ? secretNotesTab : achievementsTab];
            prevSideTab.downNeighborID = region_sideTabLostBooks;

            // Fill [Lost Book] collection

            this.collections[lostBooksTabPageIndex].Add(new List<ClickableTextureComponent>());

            var lostBooksIndices = LibraryMuseumHelper.GetLostBookIndexList();

            // Add lost books to the [Lost Books] side tab.

            int booksInCurrentPage = 0;
            int startPosX = this.xPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearSideBorder;
            int startPosY = this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 16;
            int numBooksPerRow = 8; // The number of books per row

            for (int i = 0; i < LibraryMuseumHelper.TotalLibraryBooks; i++)
            {
                bool drawShadow = false;

                // If the current Lost Book has already been discovered by the player, "enable" it in the collection
                if (lostBooksIndices[i] <= LibraryMuseumHelper.LibraryBooks)
                {
                    drawShadow = true;
                }

                // Start a new page if the current page has already been filled completely
                int x1 = startPosX + booksInCurrentPage % numBooksPerRow * BOOK_OFFSET;
                int y1 = startPosY + booksInCurrentPage / numBooksPerRow * BOOK_OFFSET;
                if (y1 > this.yPositionOnScreen + height - 128)
                {
                    // Add a new page to the collection.
                    this.collections[lostBooksTabPageIndex].Add(new List<ClickableTextureComponent>());
                    booksInCurrentPage = 0;
                    x1 = startPosX;
                    y1 = startPosY;
                }

                // Add the current lost book to the current page of the [Lost Book] collection
                ClickableTextureComponent lostBookTextureObject = new ClickableTextureComponent(
                    name: StardewMods.Common.StardewValley.Constants.ID_GAME_OBJECT_LOST_BOOK.ToString() + " " + drawShadow.ToString() + " " + lostBooksIndices[i], 
                    bounds: new Rectangle(x1, y1, 64, 64), 
                    label: (string)null, 
                    hoverText: "", 
                    texture: Game1.objectSpriteSheet, 
                    sourceRect: Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, StardewMods.Common.StardewValley.Constants.ID_GAME_OBJECT_LOST_BOOK, 16, 16), 
                    scale: 4f, 
                    drawShadow: drawShadow)
                {
                    myID = LostBooksTab_ItemBaseId + i,

                    rightNeighborID = (this.collections[lostBooksTabPageIndex].Last().Count + 1) % numBooksPerRow == 0 
                        ? -1 
                        : LostBooksTab_ItemBaseId + i + 1,
                    leftNeighborID = this.collections[lostBooksTabPageIndex].Last().Count % numBooksPerRow == 0 
                        ? region_sideTabLostBooks 
                        : LostBooksTab_ItemBaseId + i - 1,
                    downNeighborID = y1 + BOOK_OFFSET > this.yPositionOnScreen + height - 128 
                        ? -7777 
                        : LostBooksTab_ItemBaseId + i + numBooksPerRow,
                    upNeighborID = this.collections[lostBooksTabPageIndex].Last().Count < numBooksPerRow 
                        ? 12345 
                        : LostBooksTab_ItemBaseId + i - numBooksPerRow,

                    fullyImmutable = true
                };

                this.collections[lostBooksTabPageIndex].Last().Add(lostBookTextureObject);
                ++booksInCurrentPage;
            }

            SetCurrentSidetab();

            void SetCurrentSidetab()
            {
                // Set the current side tab
                if (selectedTab < 0 || selectedTab >= numTabs)
                {
                    selectedTab = 0;
                }

                // [Lost Books] tab has a different Collection Page index depending on the visibility
                // of the [Secret Notes] tab.
                if (selectedTab == lostBooksTab)
                {
                    selectedTab = lostBooksTabPageIndex;
                    this.currentlySnappedComponent = snappedItem;
                    this.snapCursorToCurrentSnappedComponent();
                }

                ModEntry.CommonServices.ReflectionHelper.GetField<int>(this, "currentTab").SetValue(selectedTab);

                /* 
                 * On default creation, the selected side tab is the first side tab. Our custom CollectionsPage
                 * sometimes needs to set a different tab as the selected tab (i.e. when returning from reading a book).
                 */
                if (selectedTab != 0)
                {
                    this.sideTabs[0].bounds.X -= CollectionsPage.widthToMoveActiveTab;
                    this.sideTabs[selectedTab].bounds.X += CollectionsPage.widthToMoveActiveTab;
                }
            }
        }

        // Change: Calls moveCursorInDirectionNew()
        public override void applyMovementKey(int direction)
        {
            if (this.allClickableComponents == null)
                this.populateClickableComponentList();

            this.moveCursorInDirectionNew((CursorDirection)direction);
        }

        private void moveCursorInDirectionNew(CursorDirection direction)
        {
            if (this.currentlySnappedComponent == null && this.allClickableComponents != null && this.allClickableComponents.Count<ClickableComponent>() > 0)
            {
                this.snapToDefaultClickableComponent();
                if (this.currentlySnappedComponent == null)
                    this.currentlySnappedComponent = this.allClickableComponents.First<ClickableComponent>();
            }
            if (this.currentlySnappedComponent == null)
                return;

            ClickableComponent snappedComponent = this.currentlySnappedComponent;
            switch (direction)
            {
                case CursorDirection.North:
                    if (this.currentlySnappedComponent.upNeighborID == -99999)
                        this.snapToDefaultClickableComponent();
                    else if (this.currentlySnappedComponent.upNeighborID == -7777)
                        this.customSnapBehavior(0, this.currentlySnappedComponent.region, this.currentlySnappedComponent.myID);
                    else
                        this.currentlySnappedComponent = this.getComponentWithID(this.currentlySnappedComponent.upNeighborID);
                    if (this.currentlySnappedComponent != null && (snappedComponent == null || snappedComponent.upNeighborID != -7777) && (!this.currentlySnappedComponent.downNeighborImmutable && !this.currentlySnappedComponent.fullyImmutable))
                        this.currentlySnappedComponent.downNeighborID = snappedComponent.myID;
                    if (this.currentlySnappedComponent == null)
                    {
                        this.noSnappedComponentFound(0, snappedComponent.region, snappedComponent.myID);
                        break;
                    }
                    break;
                case CursorDirection.Right:
                    if (this.currentlySnappedComponent.rightNeighborID == -99999)
                        this.snapToDefaultClickableComponent();
                    else if (this.currentlySnappedComponent.rightNeighborID == -7777)
                        this.customSnapBehavior(1, this.currentlySnappedComponent.region, this.currentlySnappedComponent.myID);

                    // Code added: Sets the correct snapped component (first lost book) when the [Lost Books] sidetab is selected
                    // and the player presses the [Right] button to snap to a book item. Previously, the snapped component was set to
                    // the component with ID = 0 in the collections page. The first item with ID = 0 is the first item in the [Shipped] tab
                    // and has the wrong item offset. As a result, continous right-snapping or below snapping would fail to snap to the correct
                    // [Lost Book].
                    else if (this.currentlySnappedComponent.myID >= region_sideTabShipped
                        && this.currentlySnappedComponent.myID <= region_sideTabLostBooks
                        && ModEntry.CommonServices.ReflectionHelper.GetField<int>(this, "currentTab").GetValue() == lostBooksTabPageIndex)
                    {
                        this.currentlySnappedComponent = this.getComponentWithID(LostBooksTab_ItemBaseId);
                    }

                    // Code added: Sets the correct snapped component (first lost book) when a sidetab other than [Lost Books] is selected
                    // and the player presses the [Right] button to snap to an item while the [Lost Books] item is snapped. 
                    // Without this code the snapped item will be treated as a [Lost Book] (with its non-standard item offset).
                    // Since the item offset is the same for every sidetab other than [Lost Books], we simply set the currently snapped item
                    // to the first item with ID = 0 which is the first item for every original sidetab.
                    else if (this.currentlySnappedComponent.myID == region_sideTabLostBooks 
                        && ModEntry.CommonServices.ReflectionHelper.GetField<int>(this, "currentTab").GetValue() != lostBooksTabPageIndex)
                    {
                        this.currentlySnappedComponent = this.getComponentWithID(0);
                    }

                    else
                        this.currentlySnappedComponent = this.getComponentWithID(this.currentlySnappedComponent.rightNeighborID);
                    if (this.currentlySnappedComponent != null && (snappedComponent == null || snappedComponent.rightNeighborID != -7777) && (!this.currentlySnappedComponent.leftNeighborImmutable && !this.currentlySnappedComponent.fullyImmutable))
                        this.currentlySnappedComponent.leftNeighborID = snappedComponent.myID;
                    if (this.currentlySnappedComponent == null && snappedComponent.tryDefaultIfNoRightNeighborExists)
                    {
                        this.snapToDefaultClickableComponent();
                        break;
                    }
                    if (this.currentlySnappedComponent == null)
                    {
                        this.noSnappedComponentFound(1, snappedComponent.region, snappedComponent.myID);
                        break;
                    }
                    break;
                case CursorDirection.South:
                    if (this.currentlySnappedComponent.downNeighborID == -99999)
                        this.snapToDefaultClickableComponent();
                    else if (this.currentlySnappedComponent.downNeighborID == -7777)
                        this.customSnapBehavior(2, this.currentlySnappedComponent.region, this.currentlySnappedComponent.myID);
                    else
                        this.currentlySnappedComponent = this.getComponentWithID(this.currentlySnappedComponent.downNeighborID);
                    if (this.currentlySnappedComponent != null && (snappedComponent == null || snappedComponent.downNeighborID != -7777) && (!this.currentlySnappedComponent.upNeighborImmutable && !this.currentlySnappedComponent.fullyImmutable))
                        this.currentlySnappedComponent.upNeighborID = snappedComponent.myID;
                    if (this.currentlySnappedComponent == null && snappedComponent.tryDefaultIfNoDownNeighborExists)
                    {
                        this.snapToDefaultClickableComponent();
                        break;
                    }
                    if (this.currentlySnappedComponent == null)
                    {
                        this.noSnappedComponentFound(2, snappedComponent.region, snappedComponent.myID);
                        break;
                    }
                    break;
                case CursorDirection.Left:
                    if (this.currentlySnappedComponent.leftNeighborID == -99999)
                        this.snapToDefaultClickableComponent();
                    else if (this.currentlySnappedComponent.leftNeighborID == -7777)
                        this.customSnapBehavior(3, this.currentlySnappedComponent.region, this.currentlySnappedComponent.myID);
                    else
                    {
                        //this.currentlySnappedComponent = this.getComponentWithID(this.currentlySnappedComponent.leftNeighborID);

                        // Addd code: This addition makes sure that the cursor will snap to the selected sidetab (and not the first tab)
                        // when the player presses [Left] when on the leftmost column of a collections page.
                        var currentTab = ModEntry.CommonServices.ReflectionHelper.GetField<int>(this, "currentTab").GetValue();
                        if (this.currentlySnappedComponent.leftNeighborID >= sideTabs_FirstIndex 
                            && this.currentlySnappedComponent.leftNeighborID <= sideTabs_FirstIndex + secretNotesTab
                            && this.currentlySnappedComponent.leftNeighborID != sideTabs_FirstIndex + currentTab)
                        {
                            //var currentTab = ModEntry.CommonServices.ReflectionHelper.GetField<int>(this, "currentTab").GetValue();

                            this.currentlySnappedComponent.leftNeighborID = sideTabs_FirstIndex + currentTab;
                            this.currentlySnappedComponent = this.getComponentWithID(sideTabs_FirstIndex + currentTab);
                        }
                        else
                        {
                            this.currentlySnappedComponent = this.getComponentWithID(this.currentlySnappedComponent.leftNeighborID);
                        }
                    }

                    if (this.currentlySnappedComponent != null && (snappedComponent == null || snappedComponent.leftNeighborID != -7777) && (!this.currentlySnappedComponent.rightNeighborImmutable && !this.currentlySnappedComponent.fullyImmutable))
                        this.currentlySnappedComponent.rightNeighborID = snappedComponent.myID;
                    if (this.currentlySnappedComponent == null)
                    {
                        this.noSnappedComponentFound(3, snappedComponent.region, snappedComponent.myID);
                        break;
                    }
                    break;
            }

            if (this.currentlySnappedComponent != null && snappedComponent != null && this.currentlySnappedComponent.region != snappedComponent.region)
            {
                this.actionOnRegionChange(snappedComponent.region, this.currentlySnappedComponent.region);
            }

            if (this.currentlySnappedComponent == null)
            {
                this.currentlySnappedComponent = snappedComponent;
            }

            this.snapCursorToCurrentSnappedComponent();
            Game1.playSound("shiny4");
        }

        // Change: Handles the default item for the [Lost Book] side-tab.
        public override void snapToDefaultClickableComponent()
        {
            base.snapToDefaultClickableComponent();

            int currentTab = ModEntry.CommonServices.ReflectionHelper.GetField<int>(this, "currentTab").GetValue();

            if (currentTab == lostBooksTabPageIndex)
            {
                this.currentlySnappedComponent = this.getComponentWithID(LostBooksTab_ItemBaseId);
            }
            else
            {
                this.currentlySnappedComponent = this.getComponentWithID(0);
            }

            this.snapCursorToCurrentSnappedComponent();
        }

        // Change: Added functionality to show a preview of a lost book.
        public override void performHoverAction(int x, int y)
        {
            IReflectedField<int> secretNoteImageRef = ModEntry.CommonServices.ReflectionHelper.GetField<int>(this, "secretNoteImage");

            int currentTab = ModEntry.CommonServices.ReflectionHelper.GetField<int>(this, "currentTab").GetValue();
            int currentPage = ModEntry.CommonServices.ReflectionHelper.GetField<int>(this, "currentPage").GetValue();

            descriptionTextRef.SetValue("");
            hoverTextRef.SetValue("");
            valueRef.SetValue(-1);
            secretNoteImageRef.SetValue(-1);

            foreach (ClickableTextureComponent sideTab in this.sideTabs)
            {
                if (sideTab.containsPoint(x, y))
                {
                    hoverTextRef.SetValue(sideTab.hoverText);
                    return;
                }
            }

            foreach (ClickableTextureComponent textureComponent in this.collections[currentTab][currentPage])
            {
                if (textureComponent.containsPoint(x, y))
                {
                    textureComponent.scale = Math.Min(textureComponent.scale + 0.02f, textureComponent.baseScale + 0.1f);

                    if (currentTab != achievementsTab)
                    {
                        // Draw [unknown] tooltip if item hasn't been encountered yet
                        if (!Convert.ToBoolean(textureComponent.name.Split(' ')[1]))
                        {
                            hoverTextRef.SetValue("???");
                            continue;
                        }

                        // Book has already been found -> show book preview
                        if (currentTab == lostBooksTabPageIndex)
                        {                            
                            string index = textureComponent.name.Split(' ')[2];
                            string message = Game1.content.LoadString("Strings\\Notes:" + index).Replace('\n', '^');

                            string title = message.Split('^')[0].Trim();
                            if (title.Length > BOOK_PREVIEW_LENGTH)
                            {
                                title = title.Substring(0, BOOK_PREVIEW_LENGTH) + "...";
                            }

                            // Set hover text to book content preview.
                            hoverTextRef.SetValue(title);
                            continue;
                        }
                    }

                    hoverTextRef.SetValue(this.createDescription(Convert.ToInt32(textureComponent.name.Split(' ')[0])));
                }
                else
                {
                    textureComponent.scale = Math.Max(textureComponent.scale - 0.02f, textureComponent.baseScale);
                }
            }

            this.forwardButton.tryHover(x, y, 0.5f);
            this.backButton.tryHover(x, y, 0.5f);
        }

        // Change: Added functionality to read a lost book.
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            var currentTabRef = ModEntry.CommonServices.ReflectionHelper.GetField<int>(this, "currentTab");
            var currentPageRef = ModEntry.CommonServices.ReflectionHelper.GetField<int>(this, "currentPage");

            int currentTab = currentTabRef.GetValue();
            int currentPage = currentPageRef.GetValue();

            for (int index = 0; index < this.sideTabs.Count; ++index)
            {
                if (this.sideTabs[index].containsPoint(x, y) && currentTab != index)
                {
                    Game1.playSound("smallSelect");
                    this.sideTabs[currentTab].bounds.X -= CollectionsPage.widthToMoveActiveTab;

                    currentTabRef.SetValue(index);
                    currentPageRef.SetValue(0);
                    currentTab = index;
                    currentPage = 0;

                    this.sideTabs[index].bounds.X += CollectionsPage.widthToMoveActiveTab;

                    // On tab switch, we set the currently snapped element to the tab element
                    currentlySnappedComponent = this.sideTabs[index];

                    if (index == lostBooksTabPageIndex)
                    {
                        currentlySnappedComponent.rightNeighborID = LostBooksTab_ItemBaseId;
                    }
                    else
                    {
                        currentlySnappedComponent.rightNeighborID = 0;
                    }

                    return;
                }
            }

            // Open a book when it has been clicked by the player.
            if (currentTab == lostBooksTabPageIndex)
            {
                foreach (ClickableTextureComponent textureComponent in collections[lostBooksTabPageIndex][currentPage])
                {
                    if (textureComponent.containsPoint(x, y))
                    {
                        // Only open a book if it has already been found by the player.
                        if (int.TryParse(textureComponent.name.Split(' ')[2], out int index)
                            && index <= LibraryMuseumHelper.LibraryBooks)
                        {
                            string message = Game1.content.LoadString("Strings\\Notes:" + index).Replace('\n', '^');
                            Game1.drawLetterMessage(message);

                            // Preserve the currently clicked book so it can be re-set as the snapped item 
                            snappedItem = textureComponent;
                        }
                        return;
                    }
                }
            }

            if (currentPage > 0 && this.backButton.containsPoint(x, y))
            {
                --currentPage;
                currentPageRef.SetValue(currentPage);

                Game1.playSound("shwip");
                this.backButton.scale = this.backButton.baseScale;
                if (Game1.options.snappyMenus && Game1.options.gamepadControls && currentPage == 0)
                {
                    this.currentlySnappedComponent = (ClickableComponent)this.forwardButton;
                    Game1.setMousePosition(this.currentlySnappedComponent.bounds.Center);
                }
            }

            if (currentPage >= this.collections[currentTab].Count - 1 || !this.forwardButton.containsPoint(x, y))
            {
                return;
            }

            ++currentPage;
            currentPageRef.SetValue(currentPage);

            Game1.playSound("shwip");
            this.forwardButton.scale = this.forwardButton.baseScale;
            if (!Game1.options.snappyMenus || !Game1.options.gamepadControls || currentPage != this.collections[currentTab].Count - 1)
            {
                return;
            }

            this.currentlySnappedComponent = (ClickableComponent)this.backButton;
            Game1.setMousePosition(this.currentlySnappedComponent.bounds.Center);
        }

        public override void draw(SpriteBatch b)
        {
            if (snappedItem != null)
            {
                this.currentlySnappedComponent = snappedItem;
                this.snapCursorToCurrentSnappedComponent();

                snappedItem = null;
            }

            base.draw(b);
        }
    }
}
