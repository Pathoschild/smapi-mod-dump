/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/HeyImAmethyst/Ars-Venefici
**
*************************************************/

using ArsVenefici.Framework.GUI.DragNDrop;
using ArsVenefici.Framework.Interfaces;
using ArsVenefici.Framework.Interfaces.Spells;
using ArsVenefici.Framework.Spells;
using ArsVenefici.Framework.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using SpaceCore.Content;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using xTile;
using static System.Net.Mime.MediaTypeNames;

namespace ArsVenefici.Framework.GUI.Menus
{
    public class SpellBookMenu : ModdedClickableMenu
    {

        public ModEntry modEntry;

        /// <summary>The areas to draw.</summary>
        private List<DragArea<SpellPartDraggable>> dragAreas = new List<DragArea<SpellPartDraggable>>();

        SpellBook spellBook;

        private SpellPartSourceArea sourceArea;
        private SpellGrammarArea spellGrammarArea;
        private ShapeGroupListArea shapeGroupArea;

        private SpellPartDraggable dragged;
        DragArea<SpellPartDraggable> hoveredArea;
        SpellPartDraggable hoveredPart;

        public List<ClickableTextureComponent> buttons = new List<ClickableTextureComponent>();

        private TextBox nameBox;
        public ClickableComponent nameBoxCC;

        /// <summary>The labels to draw.</summary>
        private readonly List<ClickableComponent> labels = new List<ClickableComponent>();

        private readonly List<ClickableComponent> clickables = new List<ClickableComponent>();

        ClickableComponent PageNumberLable;
        ClickableComponent TotalManaCostLable;

        private const int BasePlayId = 2000;

        private const int ScrollDownId = 6487; //This was just the first number that popped into my head
        private const int ScrollUpId = 7846; //Why think of another one when I can reverse the first
        private const int ExitId = 1;
        private const int ArrowUpId = 2;
        private const int ArrowDownId = 3;
        private const int ScrollThumbId = 4;

        private Rectangle sourceAreaScrollBarTrack;
        private ClickableTextureComponent sourceAreaScrollBarThumb;
        private ClickableTextureComponent sourceAreaArrowUp;
        private ClickableTextureComponent sourceAreaArrowDown;

        private int CurrentOffset = 0;
        private bool Scrolling;

        public static int windowWidth = 220 + borderWidth * 2;
        public static int windowHeight = 252 + borderWidth * 2 + Game1.tileSize;

        int currentPageIndex = 0;

        public SpellBookMenu(ModEntry modEntry)
            : base((int)GetAppropriateMenuPosition().X, (int)GetAppropriateMenuPosition().Y, windowWidth, windowHeight, true)
        {
            this.modEntry = modEntry;
            spellBook = Game1.player.GetSpellBook();

            exitFunction = () =>
            {
                SaveSpellBook(modEntry);

                string filePath = Path.Combine(modEntry.Helper.DirectoryPath + "/Saves", $"{Constants.SaveFolderName}_spellbook_data.json");

                if (File.Exists(filePath))
                {
                    Game1.player.GetSpellBook().SyncSpellBook(modEntry);
                }

                spellBook.CreateSpells(modEntry);
            };

            UpdateMenu();
        }

        public static Vector2 GetAppropriateMenuPosition()
        {

            int x = Game1.viewport.Size.Width / 2 - windowWidth / 2;
            int y = Game1.viewport.Size.Height / 2 - windowHeight / 2;

            Vector2 defaultPosition = new Vector2(x, y);

            defaultPosition = defaultPosition * Game1.options.zoomLevel;

            return defaultPosition;

        }

        /// <summary>The method called when the game window changes size.</summary>
        /// <param name="oldBounds">The former viewport.</param>
        /// <param name="newBounds">The new viewport.</param>
        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            base.gameWindowSizeChanged(oldBounds, newBounds);

            SetUpPositions();
        }

        protected override void SetUpPositions()
        {
            if (modEntry != null)
            {
                if (spellBook == null)
                    return;

                dragAreas.Clear();

                buttons.Clear();
                labels.Clear();

                const int Offset = Game1.tileSize / 4;

                sourceArea = new SpellPartSourceArea(new Rectangle(xPositionOnScreen - 142, yPositionOnScreen - 100, 530, 190), modEntry, modEntry.Helper.Translation.Get("ui.spell_book.source_area.name"));
                shapeGroupArea = new ShapeGroupListArea(xPositionOnScreen - 200, yPositionOnScreen + 150, this, (part, i, j) => OnPartDropped(part), modEntry.Helper.Translation.Get("ui.spell_book.shape_group_area.name"));
                spellGrammarArea = new SpellGrammarArea(new Rectangle(xPositionOnScreen - 142, yPositionOnScreen + 344, 436, 70), (part, i) => OnPartDropped(part), modEntry.Helper.Translation.Get("ui.spell_book.spell_grammar_area.name"));

                dragAreas.Add(sourceArea);
                dragAreas.Add(spellGrammarArea);
                dragAreas.Add(shapeGroupArea);

                sourceAreaArrowUp = new ClickableTextureComponent(new(xPositionOnScreen + 400 + Offset, yPositionOnScreen - 235 + Game1.tileSize, Game1.pixelZoom * 11, Game1.pixelZoom * 12), Game1.mouseCursors, new(421, 459, 11, 12), Game1.pixelZoom)
                {
                    myID = ArrowUpId,
                    leftNeighborID = BasePlayId + 1,
                    rightNeighborID = -7777,
                    downNeighborID = ScrollThumbId,
                    upNeighborID = ExitId
                };

                sourceAreaArrowDown = new ClickableTextureComponent(new(xPositionOnScreen + 400 + Offset, yPositionOnScreen - 235 + height - Game1.tileSize, Game1.pixelZoom * 11, Game1.pixelZoom * 12), Game1.mouseCursors, new(421, 472, 11, 12), Game1.pixelZoom)
                {
                    myID = ArrowDownId,
                    leftNeighborID = BasePlayId + 7,
                    rightNeighborID = -7777,
                    downNeighborID = ScrollDownId,
                    upNeighborID = ScrollThumbId
                };

                sourceAreaScrollBarThumb = new ClickableTextureComponent(new(sourceAreaArrowUp.bounds.X + Game1.pixelZoom * 3, sourceAreaArrowUp.bounds.Y + sourceAreaArrowUp.bounds.Height + Game1.pixelZoom, 24, 40), Game1.mouseCursors, new(435, 463, 6, 10), Game1.pixelZoom)
                {
                    myID = ScrollThumbId,
                    leftNeighborID = BasePlayId + 4,
                    rightNeighborID = -7777,
                    downNeighborID = ArrowDownId,
                    upNeighborID = ArrowUpId
                };

                sourceAreaScrollBarTrack = new(sourceAreaScrollBarThumb.bounds.X, sourceAreaArrowUp.bounds.Y + sourceAreaArrowUp.bounds.Height + Game1.pixelZoom, sourceAreaScrollBarThumb.bounds.Width, height - Game1.tileSize * 2 - sourceAreaArrowUp.bounds.Height - Game1.pixelZoom * 2);
                SetScrollBarToCurrentIndex();

                ClickableTextureComponent leftPageButton =
                    new ClickableTextureComponent("LeftPage", new Rectangle(xPositionOnScreen - 250, yPositionOnScreen + 450, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44, -1, -1), 1f, false);

                leftPageButton.myID = 629;
                leftPageButton.upNeighborID = -99998;
                leftPageButton.leftNeighborID = -99998;
                leftPageButton.rightNeighborID = -99998;
                leftPageButton.downNeighborID = -99998;

                ClickableTextureComponent rightPageButton =
                    new ClickableTextureComponent("RightPage", new Rectangle(xPositionOnScreen + 350, yPositionOnScreen + 450, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33, -1, -1), 1f, false);

                rightPageButton.myID = 630;
                rightPageButton.upNeighborID = -99998;
                rightPageButton.leftNeighborID = -99998;
                rightPageButton.rightNeighborID = -99998;
                rightPageButton.downNeighborID = -99998;

                nameBox = new TextBox(Game1.content.Load<Texture2D>("LooseSprites\\textBox"), null, Game1.smallFont, Game1.textColor)
                {
                    X = xPositionOnScreen + 300,
                    Y = yPositionOnScreen + 360,
                    textLimit = 20
                };

                nameBoxCC = new ClickableComponent(new Rectangle(xPositionOnScreen + 64 + spaceToClearSideBorder + borderWidth + 256, yPositionOnScreen + borderWidth + spaceToClearTopBorder - 16, 192, 48), "")
                {
                    myID = 536,
                    upNeighborID = -99998,
                    leftNeighborID = -99998,
                    rightNeighborID = -99998,
                    downNeighborID = -99998
                };

                //this.nameLabel = new ClickableComponent(new Rectangle(this.xPositionOnScreen + num1 + 16 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 192 + 4, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 8, 1, 1), "Spell Name");

                upperRightCloseButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + 500, yPositionOnScreen - 150, 48, 48), Game1.mouseCursors, new Rectangle(337, 494, 12, 12), 4f)
                {
                    myID = 9175502
                };

                buttons.Add(leftPageButton);
                buttons.Add(rightPageButton);

                clickables.Add(nameBoxCC);

                //this.labels.Add(this.nameLabel);
                TotalManaCostLable = new ClickableComponent(new Rectangle(xPositionOnScreen + 50, yPositionOnScreen + 430, 64, 64), "Total Mana Cost");
                labels.Add(TotalManaCostLable);

                PageNumberLable = new ClickableComponent(new Rectangle(xPositionOnScreen + 50, yPositionOnScreen + 480, 64, 64), "Page Number");
                labels.Add(PageNumberLable);

                currentPageIndex = spellBook.GetCurrentSpellPageIndex();

                //string filePath = Path.Combine(modEntry.helper.DirectoryPath + "/Saves", $"{new DirectoryInfo(Game1.player.slotName)}_spellbook_data.json");

                string filePath = Path.Combine(modEntry.Helper.DirectoryPath + "/Saves", $"{Constants.SaveFolderName}_spellbook_data.json");

                if (File.Exists(filePath))
                {
                    Game1.player.GetSpellBook().SyncSpellBook(modEntry);
                }

                if (spellBook.GetCurrentSpellPage() != null)
                {
                    if (spellBook.GetCurrentSpellPage().GetSpellShapeAreas() != null && spellBook.GetCurrentSpellPage().GetSpellGrammerList() != null)
                    {
                        foreach (DragArea<SpellPartDraggable> area in dragAreas)
                        {
                            GetSpellPage(area, currentPageIndex);
                        }
                    }
                }

                SetDragged(null);
            }
        }

        /// <summary>Draw the menu to the screen.</summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        public override void draw(SpriteBatch spriteBatch)
        {
            if (spellBook == null)
                return;

            base.draw(spriteBatch);

            int pMouseX = Game1.getOldMouseX();
            int pMouseY = Game1.getOldMouseY();

            //int pMouseX = Game1.getMouseX();
            //int pMouseY = Game1.getMouseY();

            //pMouseX = Game1.getMouseX();
            //pMouseY = Game1.getMouseY();

            int x = xPositionOnScreen;
            int y = yPositionOnScreen;

            foreach (DragArea<SpellPartDraggable> area in dragAreas)
            {
                if (area != null)
                {
                    area.Draw(spriteBatch, x, y, 0);
                }
            }

            // draw labels
            foreach (DragArea<SpellPartDraggable> area in dragAreas)
            {
                Color color = Color.Violet;

                drawTextureBox(spriteBatch, area.bounds.X, area.bounds.Y - 50, area.name.Length + 280, 45, Color.White);
                Utility.drawTextWithShadow(spriteBatch, area.name, Game1.smallFont, new Vector2(area.bounds.X + 10, area.bounds.Y - 45), color);
            }

            foreach (DragArea<SpellPartDraggable> area in dragAreas)
            {
                string text = "";
                Color color = Game1.textColor;
                //Color color = Color.Purple;

                Utility.drawTextWithShadow(spriteBatch, area.name, Game1.smallFont, new Vector2(area.bounds.X + 10, area.bounds.Y - 45), color);
                //Utility.drawBoldText(spriteBatch, area.name, Game1.smallFont, new Vector2(area.bounds.X + 10, area.bounds.Y - 45), color);

                if (text.Length > 0)
                    Utility.drawTextWithShadow(spriteBatch, text, Game1.smallFont, new Vector2(area.bounds.X + Game1.tileSize / 3 - Game1.smallFont.MeasureString(text).X / 2f, area.bounds.Y + Game1.tileSize / 2), color);
            }

            sourceAreaArrowUp.draw(spriteBatch);
            sourceAreaArrowDown.draw(spriteBatch);
            drawTextureBox(spriteBatch, Game1.mouseCursors, new Rectangle(403, 383, 6, 6), sourceAreaScrollBarTrack.X, sourceAreaScrollBarTrack.Y, sourceAreaScrollBarTrack.Width, sourceAreaScrollBarTrack.Height, Color.White, Game1.pixelZoom);
            sourceAreaScrollBarThumb.draw(spriteBatch);

            foreach (ClickableComponent label in labels)
            {

                if (label == PageNumberLable)
                {
                    string text = "";
                    Color color = Game1.textColor;

                    int pageNumber = spellBook.GetPages().IndexOf(spellBook.GetCurrentSpellPage()) + 1;

                    drawTextureBox(spriteBatch, PageNumberLable.bounds.X, PageNumberLable.bounds.Y - 30, PageNumberLable.bounds.Width + 15, PageNumberLable.bounds.Height, Color.White);

                    Utility.drawTextWithShadow(spriteBatch, pageNumber.ToString(), Game1.smallFont, new Vector2(PageNumberLable.bounds.X + 23, PageNumberLable.bounds.Y - 10), color);

                    if (text.Length > 0)
                        Utility.drawTextWithShadow(spriteBatch, text, Game1.smallFont, new Vector2(PageNumberLable.bounds.X + Game1.tileSize / 3 - Game1.smallFont.MeasureString(text).X / 2f, PageNumberLable.bounds.Y + Game1.tileSize / 2), color);
                }

                if(label == TotalManaCostLable)
                {
                    //Color color = Game1.textColor;
                    //Color color = Color.White;

                    //Spell spell = spellBook.CreateSpell(modEntry, currentPageIndex);

                    //if(spell != null)
                    //{
                    //    string text = "Total Mana Cost: " + spell.Mana().ToString();

                    //    Utility.drawTextWithColoredShadow(spriteBatch, text, Game1.smallFont, new Vector2(PageNumberLable.bounds.X - 70, PageNumberLable.bounds.Y + 45), color, Color.Gray);
                    //}
                }
                
            }

            // draw buttons
            foreach (ClickableTextureComponent button in buttons)
                button.draw(spriteBatch);

            //draw textbox
            nameBox.Draw(spriteBatch);

            spellBook.GetCurrentSpellPage().SetName(nameBox.Text);

            string spellPartNameText = null;
            string spellPartDescriptionText = null;

            if (dragged != null)
            {
                dragged.Draw(spriteBatch, pMouseX - SpellPartDraggable.SIZE / 2, pMouseY - SpellPartDraggable.SIZE / 2, 0);
            }
            else
            {
                if (hoveredPart != null)
                {

                    spellPartNameText = hoveredPart.GetNameTranslationKey();
                    spellPartDescriptionText = hoveredPart.GetDescriptionTranslationKey();

                    int val1 = 272;
                    if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.fr)
                        val1 = 384;
                    if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.tr)
                        val1 = 336;

                    int value = Math.Max(val1, (int)Game1.dialogueFont.MeasureString(spellPartNameText == null ? "" : spellPartNameText).X);

                    StringBuilder s = new StringBuilder();
                    s.AppendLine(Game1.parseText(spellPartDescriptionText, Game1.smallFont, value));
                    s.AppendLine("");
                    s.Append(modEntry.Helper.Translation.Get("ui.mana_cost.name") + ": " + hoveredPart.GetPart().ManaCost());

                    if (spellPartNameText != null && spellPartDescriptionText != null)
                        drawToolTip(spriteBatch, s.ToString(), spellPartNameText, null);
                }
            }

            // draw cursor
            drawMouse(spriteBatch);
        }

        public void SaveSpellBook(ModEntry modEntry)
        {
            if (spellBook == null)
                return;

            spellBook.Mutate(_ => spellBook.SaveSpellBook(modEntry));
        }

        public void GetSpellPage(DragArea<SpellPartDraggable> area, int spellPageIndex)
        {
            if (spellBook == null)
                return;

            if (area is ShapeGroupListArea)
            {
                int index = 0;

                foreach (ShapeGroupArea shapeGroupArea in ((ShapeGroupListArea)area).shapeGroups)
                {
                    //area.setAll(index, spellBook.pages[spellPageIndex].GetSpellShapes()[index].getAll());

                    if (spellBook.GetPages()[spellPageIndex].GetSpellShapeAreas() != null && spellBook.GetPages()[spellPageIndex].GetSpellShapeAreas().Length > 0)
                        shapeGroupArea.SetAll(spellBook.GetPages()[spellPageIndex].GetSpellShapeAreas()[index].GetAll());

                    index++;
                }
            }
            else if (area is SpellGrammarArea)
            {
                area.SetAll(spellBook.GetPages()[spellPageIndex].GetSpellGrammerList());
            }

            nameBox.Text = spellBook.GetPages()[spellPageIndex].GetName();
        }

        private void LogSavedSpellPages(SavedSpellPage[] spellPages)
        {
            modEntry.Monitor.Log("Logging saved spell pages for list " + spellPages, LogLevel.Info);

            for (int i = 0; i < spellPages.Length; i++)
            {
                SavedSpellPage page = spellPages[i];

                //modEntry.Monitor.Log("Listing saved shape group areas in spell page " + i, LogLevel.Info);

                for (int j = 0; j < page.GetSpellShapes().Length; j++)
                {
                    SavedShapeGroupArea<SpellPartDraggable> area = page.GetSpellShapes()[j];

                    modEntry.Monitor.Log("Listing spell parts in shape group area " + j, LogLevel.Info);

                    List<SpellPartDraggable> draggedShapes = page.GetSpellShapes()[j].GetAll();

                    foreach (SpellPartDraggable item in draggedShapes)
                    {
                        modEntry.Monitor.Log(item.GetPart().GetId(), LogLevel.Info);
                    }
                }
            }
        }

        private void LogSpellPages(List<SpellPage> spellPages)
        {
            modEntry.Monitor.Log("Logging spell pages for list " + spellPages, LogLevel.Info);

            for (int i = 0; i < spellPages.Count; i++)
            {
                SpellPage page = spellPages[i];

                //modEntry.Monitor.Log("Listing saved shape group areas in spell page " + i, LogLevel.Info);

                for (int j = 0; j < page.GetSpellShapeAreas().Length; j++)
                {
                    ShapeGroupArea area = page.GetSpellShapeAreas()[j];

                    modEntry.Monitor.Log("Listing spell parts in shape group area " + j, LogLevel.Info);

                    List<SpellPartDraggable> draggedShapes = page.GetSpellShapeAreas()[j].GetAll();

                    foreach (SpellPartDraggable item in draggedShapes)
                    {
                        modEntry.Monitor.Log(item.GetPart().GetId(), LogLevel.Info);
                    }
                }
            }
        }

        /// <summary>Handle a button click.</summary>
        /// <param name="name">The button name that was clicked.</param>
        private void HandleButtonClick(string name)
        {
            if (name == null || spellBook == null)
                return;

            switch (name)
            {
                case "LeftPage":
                    spellBook.SetCurrentSpellPageIndex(spellBook.GetCurrentSpellPageIndex() - 1);
                    spellBook.TurnToSpellPage();
                    SaveSpellBook(modEntry);
                    UpdateMenu();
                    break;
                case "RightPage":
                    spellBook.SetCurrentSpellPageIndex(spellBook.GetCurrentSpellPageIndex() + 1);
                    spellBook.TurnToSpellPage();
                    SaveSpellBook(modEntry);
                    UpdateMenu();
                    break;

                case "OK":

                    break;
            }

            Game1.playSound("grassyStep");
        }

        /// <summary>The method invoked when the player left-clicks on the menu.</summary>
        /// <param name="x">The X-position of the cursor.</param>
        /// <param name="y">The Y-position of the cursor.</param>
        /// <param name="playSound">Whether to enable sound.</param>
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (spellBook == null)
                return;

            if (hoveredPart == null)
            {
                base.receiveLeftClick(x, y, playSound);
            }

            int mouseX = x;
            int mouseY = y;

            if (dragged == null)
            {
                if (hoveredArea != null && hoveredPart != null && hoveredArea.CanPick(hoveredPart, mouseX, mouseY))
                {
                    hoveredArea.Pick(hoveredPart, mouseX, mouseY);
                    SetDragged(hoveredPart);
                }

                foreach (ClickableTextureComponent button in buttons.ToList())
                {
                    if (button.containsPoint(mouseX, mouseY))
                    {
                        HandleButtonClick(button.name);
                        button.scale -= 0.5f;
                        button.scale = Math.Max(3.5f, button.scale);
                    }
                }
            }
            else if (dragged != null)
            {
                if (hoveredArea != null && hoveredArea.CanDrop(dragged, mouseX, mouseY))
                {
                    hoveredArea.Drop(dragged, mouseX, mouseY);
                    SetDragged(null);
                }
                else
                {
                    SetDragged(null);
                }
            }

            if (sourceAreaArrowUp.containsPoint(x, y))
                ArrowUpPressed();
            else if (sourceAreaArrowDown.containsPoint(x, y))
                ArrowDownPressed();
            else if (sourceAreaScrollBarThumb.containsPoint(x, y))
                Scrolling = true;
            else if (sourceAreaScrollBarTrack.Contains(x, y))
            {
                Scrolling = true;
                leftClickHeld(x, y);
                releaseLeftClick(x, y);
            }

            nameBox.Update();

            //modEntry.Monitor.Log("Mouse clicked at " + x + " , " + y, LogLevel.Info);
            SaveSpellBook(modEntry);
        }

        public override void leftClickHeld(int x, int y)
        {
            if (Scrolling)
            {
                int initialY = y - (sourceAreaScrollBarTrack.Y + sourceAreaScrollBarThumb.bounds.Height / 2);
                int scrollHeight = sourceAreaScrollBarTrack.Height - sourceAreaScrollBarThumb.bounds.Height;
                double ratio = Math.Min(Math.Max(initialY * 1.0 / scrollHeight, 0), 1);
                int newOffset = (int)Math.Round(ratio * (sourceArea.GetAll().Count - (SpellPartSourceArea.COLUMNS * SpellPartSourceArea.ROWS)));
                if (newOffset != CurrentOffset)
                {
                    CurrentOffset = newOffset;

                    sourceArea.SetCurrentOffset(CurrentOffset);
                    SetScrollBarToCurrentIndex();
                    sourceArea.UpdateVisibility();
                }
            }

            base.leftClickHeld(x, y);
        }

        public override void releaseLeftClick(int x, int y)
        {
            base.releaseLeftClick(x, y);
            Scrolling = false;
        }

        /// <summary>The method invoked when the player right-clicks on the lookup UI.</summary>
        /// <param name="x">The X-position of the cursor.</param>
        /// <param name="y">The Y-position of the cursor.</param>
        /// <param name="playSound">Whether to enable sound.</param>
        public override void receiveRightClick(int x, int y, bool playSound = true)
        {

        }

        public override void receiveScrollWheelAction(int direction)
        {

            if(hoveredArea != null)
            {
                if(hoveredArea is SpellPartSourceArea)
                {
                    if (direction > 0)
                        ArrowUpPressed();
                    else if (direction < 0)
                        ArrowDownPressed();

                    base.receiveScrollWheelAction(direction);
                }
            }
        }

        /// <summary>The method invoked when the player hovers the cursor over the menu.</summary>
        /// <param name="x">The X-position of the cursor.</param>
        /// <param name="y">The Y-position of the cursor.</param>
        public override void performHoverAction(int x, int y)
        {
            if (spellBook == null)
                return;

            hoveredArea = GetHoveredArea(x, y);
            hoveredPart = GetHoveredElement(x, y);

            foreach (ClickableTextureComponent button in buttons)
                ChangeHoverActionScale(button, x, y, 0.01f, 0.1f);

            ChangeHoverActionScale(upperRightCloseButton, x, y, 0.01f, 0.1f);

            nameBox.Hover(x, y);

            //if (hoveredArea != null)
            //    modEntry.Monitor.Log("Mouse hovering over " + hoveredArea.name, LogLevel.Info);

            //if (hoveredPart != null)
            //    modEntry.Monitor.Log("Mouse hovering over spell part " + hoveredPart.getPart().GetId(), LogLevel.Info);
        }

        private void SetScrollBarToCurrentIndex()
        {
            sourceAreaScrollBarThumb.bounds.Y = (int)((sourceAreaScrollBarTrack.Height - sourceAreaScrollBarThumb.bounds.Height) * 1.0 * CurrentOffset / (sourceArea.GetAll().Count - (SpellPartSourceArea.COLUMNS * SpellPartSourceArea.ROWS)));
            sourceAreaScrollBarThumb.bounds.Y += sourceAreaScrollBarTrack.Y;
            Game1.playSound("shiny4");
        }

        private void ArrowUpPressed()
        {
            if (CurrentOffset > 0)
            {
                CurrentOffset--;
                sourceArea.SetCurrentOffset(CurrentOffset);
                SetScrollBarToCurrentIndex();
                sourceArea.UpdateVisibility();
            }
        }

        private void ArrowDownPressed()
        {
            if (CurrentOffset < sourceArea.GetAll().Count - (SpellPartSourceArea.COLUMNS * SpellPartSourceArea.ROWS))
            {
                CurrentOffset++;
                sourceArea.SetCurrentOffset(CurrentOffset);
                SetScrollBarToCurrentIndex();
                sourceArea.UpdateVisibility();
            }
        }


        public override void receiveKeyPress(Keys key)
        {
            if (key != 0)
            {
                if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && readyToClose())
                {
                    if (key != Keys.E)
                        exitThisMenu();
                }
                else if (Game1.options.snappyMenus && Game1.options.gamepadControls && !overrideSnappyMenuCursorMovementBan())
                {
                    applyMovementKey(key);
                }
            }
        }

        /// <summary>
        /// Changes Scale When A Component Is Hovered.
        /// </summary>
        /// <param name="component">Current Component</param>
        /// <param name="x">X Position Of The Mouse</param>
        /// <param name="y">Y Position Of The Mouse</param>
        /// <param name="min">The Minimum Scale</param>
        /// <param name="max">The Maximum Scale</param>
        public void ChangeHoverActionScale(ClickableTextureComponent component, int x, int y, float min, float max)
        {
            if (component.containsPoint(x, y))
                component.scale = Math.Min(component.scale + min, component.baseScale + max);
            else
                component.scale = Math.Max(component.scale - min, component.baseScale);
        }

        private void SetDragged(SpellPartDraggable dragged)
        {
            this.dragged = dragged;
            sourceArea?.SetTypeFilter(shapeGroupArea.CanStore(), spellGrammarArea.CanStore(), spellGrammarArea.GetAll().Any() && spellGrammarArea.GetAll()[0].GetPart().GetType() == SpellPartType.COMPONENT || shapeGroupArea.GetAll().Any() && shapeGroupArea.GetAll()[0].GetPart().GetType() == SpellPartType.SHAPE);
        }

        private DragArea<SpellPartDraggable> GetHoveredArea(int mouseX, int mouseY)
        {
            foreach (DragArea<SpellPartDraggable> area in dragAreas)
            {
                if (area.bounds.Contains(mouseX, mouseY))
                    return area;
            }

            return null;
        }

        private SpellPartDraggable GetHoveredElement(int mouseX, int mouseY)
        {
            DragArea<SpellPartDraggable> area = GetHoveredArea(mouseX, mouseY);
            return area == null ? null : area.ElementAt(mouseX, mouseY);
        }

        public int AllowedShapeGroups()
        {
            return 5;
        }

        private void OnPartDropped(SpellPartDraggable part)
        {
            ISpellPart spellPart = part.GetPart();

            //if (spellPart == AMSpellParts.COLOR.get())
            //{
            //    openColorPicker(part);
            //}
        }
    }
}
