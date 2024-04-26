/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewDruid.Data;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using StardewValley.Quests;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace StardewDruid.Journal
{
    internal class Druid : IClickableMenu
    {

        public bool active;

        public bool reverse;

        public bool quests;

        public bool effects;

        public const int region_forwardButton = 101;

        public const int region_backButton = 102;
 
        public List<List<string>> pages;

        public List<ClickableComponent> questLogButtons;

        private int currentPage;

        private int questPage = -1;
 
        public ClickableTextureComponent forwardButton;
        public ClickableTextureComponent endButton;
        public ClickableTextureComponent backButton;
        public ClickableTextureComponent startButton;
        public ClickableTextureComponent activeButton;
        public ClickableTextureComponent reverseButton;
        public ClickableTextureComponent questsButton;
        public ClickableTextureComponent effectsButton;

        //protected Page _shownPage;

        protected float _contentHeight;
 
        protected float _scissorRectHeight;
 
        public float scrollAmount;
  
        public ClickableTextureComponent upArrow;

        public ClickableTextureComponent downArrow;

        public ClickableTextureComponent scrollBar;
 
        private bool scrolling;

        public Rectangle scrollBarBounds;

        private string hoverText = "";

        public Druid()
          : base(0, 0, 0, 0, true)
        {

            reverse = Mod.instance.Config.reverseJournal;

            active = Mod.instance.Config.activeJournal;

            Game1.playSound("bigSelect");
            
            setupPages();
            
            width = 832;
            
            height = 576;

            Vector2 centeringOnScreen = Utility.getTopLeftPositionForCenteringOnScreen(width, height, 0, 0);
           
            xPositionOnScreen = (int)centeringOnScreen.X;
            
            yPositionOnScreen = (int)centeringOnScreen.Y + 32;
            
            questLogButtons = new List<ClickableComponent>();
            
            for (int index = 0; index < 6; ++index)
                questLogButtons.Add(new ClickableComponent(new Rectangle(xPositionOnScreen + 16, yPositionOnScreen + 16 + index * ((height - 32) / 6), width - 32, (height - 32) / 6 + 4), index.ToString() ?? "")
                {
                    myID = index,
                    downNeighborID = -7777,
                    upNeighborID = index > 0 ? index - 1 : -1,
                    rightNeighborID = -7777,
                    leftNeighborID = -7777,
                    fullyImmutable = true
                });
            upperRightCloseButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width - 20, yPositionOnScreen - 8, 48, 48), Game1.mouseCursors, new Rectangle(337, 494, 12, 12), 4f, false);
            
            ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(new Rectangle(xPositionOnScreen - 72, yPositionOnScreen + 8, 48, 48), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), 3f, false);
            textureComponent1.myID = 102;
            textureComponent1.rightNeighborID = -7777;
            backButton = textureComponent1;
            
            ClickableTextureComponent textureComponent2 = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width, yPositionOnScreen + height - 48, 48, 48), Game1.mouseCursors, new Rectangle(365, 495, 12, 11),3f, false);
            textureComponent2.myID = 101;
            forwardButton = textureComponent2;
            
            ClickableTextureComponent textureComponent3 = new ClickableTextureComponent(new Rectangle(xPositionOnScreen - 72, yPositionOnScreen + 72, 48, 48), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 3f, false);
            textureComponent3.myID = 103;
            startButton = textureComponent3;
            
            ClickableTextureComponent textureComponent4 = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width, yPositionOnScreen + height - 100, 48, 48), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 3f, false);
            textureComponent4.myID = 104;
            endButton = textureComponent4;
            
            ClickableTextureComponent textureComponent5 = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + 16, yPositionOnScreen - 64, 48, 48), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 3f, false);
            textureComponent5.myID = 105;
            activeButton = textureComponent5;
            
            ClickableTextureComponent textureComponent6 = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + 72, yPositionOnScreen - 64, 48, 48), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 3f, false);
            textureComponent6.myID = 106;
            reverseButton = textureComponent6;

            ClickableTextureComponent textureComponent7= new ClickableTextureComponent(new Rectangle(xPositionOnScreen + 72 + 56, yPositionOnScreen - 64, 48, 48), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 3f, false);
            textureComponent7.myID = 107;
            questsButton = textureComponent7;

            ClickableTextureComponent textureComponent8 = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + 72 + 56 + 56, yPositionOnScreen - 64, 48, 48), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 3f, false);
            textureComponent8.myID = 108;
            effectsButton = textureComponent8;

            int num = xPositionOnScreen + width + 16;
            
            upArrow = new ClickableTextureComponent(new Rectangle(num, yPositionOnScreen + 96, 44, 48), Game1.mouseCursors, new Rectangle(421, 459, 11, 12), 4f, false);
            
            downArrow = new ClickableTextureComponent(new Rectangle(num, yPositionOnScreen + height - 64, 44, 48), Game1.mouseCursors, new Rectangle(421, 472, 11, 12), 4f, false);
            
            scrollBarBounds = new Rectangle();
            
            scrollBarBounds.X = upArrow.bounds.X + 12;
           
            scrollBarBounds.Width = 24;
            
            scrollBarBounds.Y = upArrow.bounds.Y + upArrow.bounds.Height + 4;
            
            scrollBarBounds.Height = downArrow.bounds.Y - 4 - scrollBarBounds.Y;
            
            scrollBar = new ClickableTextureComponent(new Rectangle(scrollBarBounds.X, scrollBarBounds.Y, 24, 40), Game1.mouseCursors, new Rectangle(435, 463, 6, 10), 4f, false);
            
            if (!Game1.options.SnappyMenus)
                return;
            
            populateClickableComponentList();
            
            base.snapToDefaultClickableComponent();
            
        }

        public void setupPages()
        {

            if (quests)
            {

                pages = Mod.instance.questHandle.OrganiseQuests(active, reverse);

            }
            else
            {

                pages = Mod.instance.questHandle.OrganiseEffects(reverse);

            }

        }

        protected override void customSnapBehavior(int direction, int oldRegion, int oldID)
        {
            if (oldID >= 0 && oldID < 6 && questPage == -1)
            {
                switch (direction)
                {
                    case 1:
                        if (currentPage < pages.Count - 1)
                        {
                            currentlySnappedComponent = getComponentWithID(101);
                            currentlySnappedComponent.leftNeighborID = oldID;
                            break;
                        }
                        break;
                    case 2:
                        if (oldID < 5 && pages[currentPage].Count - 1 > oldID)
                        {
                            currentlySnappedComponent = getComponentWithID(oldID + 1);
                            break;
                        }
                        break;
                    case 3:
                        if (currentPage > 0)
                        {
                            currentlySnappedComponent = getComponentWithID(102);
                            currentlySnappedComponent.rightNeighborID = oldID;
                            break;
                        }
                        break;
                }
            }
            else if (oldID == 102)
            {
                if (questPage != -1)
                    return;
                currentlySnappedComponent = getComponentWithID(0);
            }
            snapCursorToCurrentSnappedComponent();
        }

        public override void snapToDefaultClickableComponent()
        {
 
            currentlySnappedComponent = getComponentWithID(0);
 
            snapCursorToCurrentSnappedComponent();

        }

        public override void receiveGamePadButton(Buttons b)
        {
 
            if (b == Buttons.RightTrigger && questPage == -1 && currentPage < pages.Count - 1)
            {
 
                nonQuestPageForwardButton();
 
            }
            else if (b == Buttons.LeftTrigger && questPage == -1)
            {

                if (currentPage > 0)
                {
                    nonQuestPageBackButton();

                }
                else
                {
                    pageEndButton();
                }

            }

        }

        public bool NeedsScroll()
        {
 
            return questPage != -1 && _contentHeight > (double)_scissorRectHeight;

        }

        public override void receiveScrollWheelAction(int direction)
        {
  
            if (NeedsScroll())
            {
  
                float num = scrollAmount - Math.Sign(direction) * 64 / 2;
 
                if ((double)num < 0.0)
                {
                    num = 0.0f;
                }
                    
  
                if ((double)num > _contentHeight - (double)_scissorRectHeight)
                {
                    num = _contentHeight - _scissorRectHeight;

                }

                if (scrollAmount != (double)num)
                {
                    scrollAmount = num;
                    Game1.playSound("shiny4");
                    SetScrollBarFromAmount();
                }
 
            }

            base.receiveScrollWheelAction(direction);
 
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
        }

        public override void performHoverAction(int x, int y)
        {
            hoverText = "";
            base.performHoverAction(x, y);
            forwardButton.tryHover(x, y, 0.2f);
            backButton.tryHover(x, y, 0.2f);
            endButton.tryHover(x, y, 0.2f);
            startButton.tryHover(x, y, 0.2f);
            activeButton.tryHover(x, y, 0.2f);
            reverseButton.tryHover(x, y, 0.2f);
            questsButton.tryHover(x, y, 0.2f);
            effectsButton.tryHover(x, y, 0.2f);
            if (!NeedsScroll())
                return;
            upArrow.tryHover(x, y, 0.1f);
            downArrow.tryHover(x, y, 0.1f);
            scrollBar.tryHover(x, y, 0.1f);
            int num = scrolling ? 1 : 0;
        }

        public override void receiveKeyPress(Keys key)
        {
            if (Game1.isAnyGamePadButtonBeingPressed() && questPage != -1 && Game1.options.doesInputListContain(Game1.options.menuButton, key))
                exitQuestPage();
            else
                base.receiveKeyPress(key);
            if (Game1.options.doesInputListContain(Game1.options.journalButton, key) && readyToClose())
            {
                Game1.exitActiveMenu();
                Game1.playSound("bigDeSelect");
            }
            if (!Mod.instance.RiteButtonPressed())
                return;
            Game1.exitActiveMenu();
        }

        private void nonQuestPageForwardButton()
        {
            ++currentPage;
            Game1.playSound("shwip");
            if (!Game1.options.SnappyMenus || currentPage != pages.Count - 1)
                return;
            currentlySnappedComponent = getComponentWithID(0);
            snapCursorToCurrentSnappedComponent();
        }

        private void nonQuestPageBackButton()
        {
            --currentPage;
            Game1.playSound("shwip");
            if (!Game1.options.SnappyMenus || currentPage != 0)
                return;
            currentlySnappedComponent = getComponentWithID(0);
            snapCursorToCurrentSnappedComponent();
        }

        private void pageEndButton()
        {
            currentPage = pages.Count - 1;
            Game1.playSound("shwip");
            if (!Game1.options.SnappyMenus || currentPage != 0)
                return;
            currentlySnappedComponent = getComponentWithID(0);
            snapCursorToCurrentSnappedComponent();
        }

        private void pageStartButton()
        {
            currentPage = 0;
            Game1.playSound("shwip");
            if (!Game1.options.SnappyMenus || currentPage != 0)
                return;
            currentlySnappedComponent = getComponentWithID(0);
            snapCursorToCurrentSnappedComponent();
        }

        public override void leftClickHeld(int x, int y)
        {
            if (GameMenu.forcePreventClose)
                return;
            base.leftClickHeld(x, y);
            if (scrolling)
                SetScrollFromY(y);
        }

        public override void releaseLeftClick(int x, int y)
        {
            if (GameMenu.forcePreventClose)
                return;
            base.releaseLeftClick(x, y);
            scrolling = false;
        }

        public void SetScrollFromY(int y)
        {
            int y1 = scrollBar.bounds.Y;
            scrollAmount = Utility.Clamp((y - scrollBarBounds.Y) / (float)(scrollBarBounds.Height - scrollBar.bounds.Height), 0.0f, 1f) * (_contentHeight - _scissorRectHeight);
            SetScrollBarFromAmount();
            if (y1 == scrollBar.bounds.Y)
                return;
            Game1.playSound("shiny4");
        }

        public void UpArrowPressed()
        {
            upArrow.scale = upArrow.baseScale;
            scrollAmount -= 64f;
            if (scrollAmount < 0.0)
                scrollAmount = 0.0f;
            SetScrollBarFromAmount();
        }

        public void DownArrowPressed()
        {
            downArrow.scale = downArrow.baseScale;
            scrollAmount += 64f;
            if (scrollAmount > _contentHeight - (double)_scissorRectHeight)
                scrollAmount = _contentHeight - _scissorRectHeight;
            SetScrollBarFromAmount();
        }

        private void SetScrollBarFromAmount()
        {
            if (!NeedsScroll())
            {
                scrollAmount = 0.0f;
            }
            else
            {
                if (scrollAmount < 8.0)
                    scrollAmount = 0.0f;
                if (scrollAmount > _contentHeight - (double)_scissorRectHeight - 8.0)
                    scrollAmount = _contentHeight - _scissorRectHeight;
                scrollBar.bounds.Y = (int)(scrollBarBounds.Y + (scrollBarBounds.Height - scrollBar.bounds.Height) / (double)Math.Max(1f, _contentHeight - _scissorRectHeight) * scrollAmount);
            }
        }

        public override void applyMovementKey(int direction)
        {
            base.applyMovementKey(direction);
            if (!NeedsScroll())
                return;
            switch (direction)
            {
                case 0:
                    UpArrowPressed();
                    break;
                case 2:
                    DownArrowPressed();
                    break;
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);
            if (Game1.activeClickableMenu == null)
                return;
            if (questPage == -1)
            {
                for (int index = 0; index < questLogButtons.Count; ++index)
                {
                    if (pages.Count > 0 && pages[currentPage].Count > index && questLogButtons[index].containsPoint(x, y))
                    {
                        Game1.playSound("smallSelect");
                        questPage = index;
                        //_shownPage = pages[currentPage][index];
                        //_objectiveText = _shownPage.objectives;
                        //_transcriptText = _shownPage.transcript;
                        scrollAmount = 0.0f;
                        SetScrollBarFromAmount();
                        if (!Game1.options.SnappyMenus)
                            return;
                        currentlySnappedComponent = getComponentWithID(102);
                        currentlySnappedComponent.rightNeighborID = -7777;
                        currentlySnappedComponent.downNeighborID = 104;
                        snapCursorToCurrentSnappedComponent();
                        return;
                    }
                }
                if (currentPage == 0 && backButton.containsPoint(x, y))
                    exitThisMenu(true);
                else if (currentPage < pages.Count - 1 && forwardButton.containsPoint(x, y))
                    nonQuestPageForwardButton();
                else if (currentPage > 0 && backButton.containsPoint(x, y))
                    nonQuestPageBackButton();
                else if (currentPage > 0 && startButton.containsPoint(x, y))
                    pageStartButton();
                else if (currentPage < pages.Count - 1 && endButton.containsPoint(x, y))
                    pageEndButton();
                else if (reverseButton.containsPoint(x, y))
                { reverse = reverse ? false : true; setupPages(); }
                else if (activeButton.containsPoint(x, y))
                { active = active ? false : true; setupPages(); }
                else if (questsButton.containsPoint(x, y))
                { quests = true; effects = false; setupPages(); }
                else if (effectsButton.containsPoint(x, y))
                { quests = false; effects = true; setupPages(); }
                else
                    exitThisMenu(true);
            }
            else
            {
                if (!NeedsScroll() || backButton.containsPoint(x, y))
                    exitQuestPage();
                if (!NeedsScroll())
                    return;
                if (downArrow.containsPoint(x, y) && scrollAmount < _contentHeight - (double)_scissorRectHeight)
                {
                    DownArrowPressed();
                    Game1.playSound("shwip");
                }
                else if (upArrow.containsPoint(x, y) && scrollAmount > 0.0)
                {
                    UpArrowPressed();
                    Game1.playSound("shwip");
                }
                else if (scrollBar.containsPoint(x, y))
                    scrolling = true;
                else if (scrollBarBounds.Contains(x, y))
                    scrolling = true;
                else if (!downArrow.containsPoint(x, y) && x > xPositionOnScreen + width && x < xPositionOnScreen + width + 128 && y > yPositionOnScreen && y < yPositionOnScreen + height)
                {
                    scrolling = true;
                    base.leftClickHeld(x, y);
                    base.releaseLeftClick(x, y);
                }
            }
        }

        public void exitQuestPage()
        {
            questPage = -1;
            setupPages();
            Game1.playSound("shwip");
            if (!Game1.options.SnappyMenus)
                return;
            base.snapToDefaultClickableComponent();
        }

        public override void update(GameTime time) => base.update(time);

        public override void draw(SpriteBatch b)
        {
            Texture2D iconTexture = Mod.instance.iconData.displayTexture;

            SpriteBatch spriteBatch1 = b;
            
            Texture2D fadeToBlackRect = Game1.fadeToBlackRect;
            
            Viewport viewport = Game1.graphics.GraphicsDevice.Viewport;
            
            Rectangle bounds = viewport.Bounds;
            
            Color color = Color.Black * 0.75f;
            
            spriteBatch1.Draw(fadeToBlackRect, bounds, color);
            
            SpriteText.drawStringWithScrollCenteredAt(b, "Stardew Druid", xPositionOnScreen + width / 2, yPositionOnScreen - 64);

            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 373, 18, 18), xPositionOnScreen, yPositionOnScreen, width, height, Color.White, 4f, true, -1f);
            
            if (questPage == -1)
            {

                for (int index = 0; index < questLogButtons.Count; ++index)
                {

                    if (pages.Count() > 0 && pages[currentPage].Count() > index)
                    {

                        IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 396, 15, 15), questLogButtons[index].bounds.X, questLogButtons[index].bounds.Y, questLogButtons[index].bounds.Width, questLogButtons[index].bounds.Height, questLogButtons[index].containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY()) ? Color.Wheat : Color.White, 4f, false, -1f);


                        IconData.displays entryIcon;

                        string entryText;

                        if (quests)
                        {
                            string questId = pages[currentPage][index];

                            bool active = Mod.instance.save.progress[questId].status == 1;

                            if (active)
                            {
                                b.Draw(iconTexture, new Vector2(questLogButtons[index].bounds.Right - 80, questLogButtons[index].bounds.Y + 20), Mod.instance.iconData.DisplayRect(Data.IconData.displays.active), Color.White * 1f, 0f, Vector2.Zero, 3f, 0, 999f);

                            }

                            entryIcon = Mod.instance.questHandle.quests[questId].icon;

                            entryText = Mod.instance.questHandle.quests[questId].title;
                        
                        }
                        else
                        {

                            string[] effectIds = pages[currentPage][index].Split("|");

                            entryIcon = Mod.instance.questHandle.effects[effectIds[0]][Convert.ToInt32(effectIds[1])].icon;

                            entryText = Mod.instance.questHandle.effects[effectIds[0]][Convert.ToInt32(effectIds[1])].title;

                        }

                        SpriteText.drawString(b, entryText, questLogButtons[index].bounds.X + 100, questLogButtons[index].bounds.Y + 24, 999999, -1, 999999, 1f, 0.88f, false, -1, "", null, 0);

                        Utility.drawWithShadow(b, iconTexture, new Vector2(questLogButtons[index].bounds.X + 28, questLogButtons[index].bounds.Y + 24), Mod.instance.iconData.DisplayRect(entryIcon), Color.White, 0.0f, Vector2.Zero, 3f, false, 0.99f, -1, -1, 0.35f);

                    }

                }

            }
            else
            {

                string title;

                string description;

                string explanation;

                List<string> objectives = new();

                List<string> transcripts = new();

                if (quests)
                {
                    string questId = pages[currentPage][questPage];

                    bool active = Mod.instance.save.progress[questId].status == 1;

                    title = Mod.instance.questHandle.quests[questId].title;

                    description = Mod.instance.questHandle.quests[questId].description;

                    if(!active && Mod.instance.questHandle.quests[questId].explanation != null)
                    {
                        
                        explanation = Mod.instance.questHandle.quests[questId].explanation;

                    }
                    else
                    {

                        explanation = Mod.instance.questHandle.quests[questId].instruction;

                    }

                    if (active)
                    {

                        if (Mod.instance.questHandle.quests[questId].type == Quest.questTypes.lesson)
                        {

                            objectives.Add(Mod.instance.questHandle.quests[questId].requirement.ToString() + " " + Mod.instance.questHandle.quests[questId].progression);

                            if (Mod.instance.questHandle.quests[questId].reward > 0)
                            {

                                objectives.Add("Reward: " + Mod.instance.questHandle.quests[questId].reward.ToString() + "g");

                            }

                        }
                        else if (Mod.instance.questHandle.quests[questId].reward > 0)
                        {

                            objectives.Add("Bounty " + Mod.instance.questHandle.quests[questId].reward.ToString() + "g");

                        }

                    }
                    else
                    {

                        objectives = Mod.instance.questHandle.quests[questId].details;


                    }

                    if (Mod.instance.questHandle.quests[questId].type == Quest.questTypes.challenge)
                    {

                        Dictionary<int, Dictionary<int, string>> dialogueScene = DialogueData.DialogueScene(questId);

                        if (dialogueScene.Count > 0)
                        {

                            Dictionary<int, Dialogue.Narrator> narrator = DialogueData.DialogueNarrator(questId);

                            foreach (KeyValuePair<int, Dialogue.Narrator> sceneNarrator in narrator)
                            {

                                transcripts.Add("(transcript) " + sceneNarrator.Value.name);

                                foreach (KeyValuePair<int, Dictionary<int, string>> sceneEntry in dialogueScene)
                                {

                                    if (sceneEntry.Key > 900)
                                    {
                                        continue;
                                    }

                                    if (sceneEntry.Value.ContainsKey(sceneNarrator.Key))
                                    {

                                        transcripts.Add(sceneEntry.Value[sceneNarrator.Key]);

                                    }

                                }

                            }


                        }

                    }

                }
                else
                {

                    string[] effectParts = pages[currentPage][questPage].Split("|");

                    title = Mod.instance.questHandle.effects[effectParts[0]][Convert.ToInt32(effectParts[1])].title;

                    description = Mod.instance.questHandle.effects[effectParts[0]][Convert.ToInt32(effectParts[1])].description;

                    explanation = Mod.instance.questHandle.effects[effectParts[0]][Convert.ToInt32(effectParts[1])].instruction;

                    objectives = Mod.instance.questHandle.effects[effectParts[0]][Convert.ToInt32(effectParts[1])].details;

                }

                SpriteText.drawStringHorizontallyCenteredAt(b, title, xPositionOnScreen + width / 2, yPositionOnScreen + 32, 999999, -1, 999999, 1f, 0.88f, false, null, 99999);

                string text1 = Game1.parseText(description, Game1.dialogueFont, width - 128);

                Rectangle scissorRectangle = b.GraphicsDevice.ScissorRectangle;

                Vector2 vector2 = Game1.dialogueFont.MeasureString(text1);

                Rectangle rectangle = new Rectangle()
                {
                    X = xPositionOnScreen + 32,
                    Y = yPositionOnScreen + 96
                };

                rectangle.Height = yPositionOnScreen + height - 32 - rectangle.Y;

                rectangle.Width = width - 64;

                _scissorRectHeight = rectangle.Height;

                Rectangle screen = Utility.ConstrainScissorRectToScreen(rectangle);

                b.End();

                SpriteBatch spriteBatch2 = b;

                BlendState alphaBlend = BlendState.AlphaBlend;

                SamplerState pointClamp = SamplerState.PointClamp;

                RasterizerState rasterizerState = new RasterizerState();

                rasterizerState.ScissorTestEnable = true;

                Matrix? nullable = new Matrix?();

                spriteBatch2.Begin(0, alphaBlend, pointClamp, null, rasterizerState, null, nullable);

                Game1.graphics.GraphicsDevice.ScissorRectangle = screen;

                Utility.drawTextWithShadow(b, text1, Game1.dialogueFont, new Vector2(xPositionOnScreen + 64, (float)(yPositionOnScreen - (double)scrollAmount + 96.0)), Game1.textColor, 1f, -1f, -1, -1, 1f, 3);

                float textHeight = (float)(yPositionOnScreen + 96 + (double)vector2.Y + 32.0) - scrollAmount;

                // -------------------------------------------------------
                // instruction / explanation

                int num2 = width - 128;

                SpriteFont dialogueFont = Game1.dialogueFont;

                int num3 = num2;

                string text2 = Game1.parseText(explanation, dialogueFont, num3);

                Utility.drawTextWithShadow(b, text2, Game1.dialogueFont, new Vector2(xPositionOnScreen + 64, textHeight - 8f), Game1.textColor, 1f, -1f, -1, -1, 1f, 3);

                textHeight += Game1.dialogueFont.MeasureString(text2).Y;

                _contentHeight = textHeight + scrollAmount - screen.Y;

                // ------------------------------------------------------
                // extra details

                for (int index = 0; index < objectives.Count; ++index)
                {

                    string objectiveParse = Game1.parseText(objectives[index], dialogueFont, width - 128);

                    Color darkBlue = Color.DarkBlue;

                    Utility.drawTextWithShadow(b, objectiveParse, Game1.dialogueFont, new Vector2(xPositionOnScreen + 64, textHeight - 8f), darkBlue, 1f, -1f, -1, -1, 1f, 3);

                    textHeight += Game1.dialogueFont.MeasureString(objectiveParse).Y;

                    _contentHeight = textHeight + scrollAmount - screen.Y;

                }

                // ------------------------------------------------------
                // transcripts

                if (transcripts.Count > 0) {


                    textHeight += 16;

                    for (int index = 0; index < transcripts.Count; ++index)
                    {

                        string transcriptParse = Game1.parseText(transcripts[index], dialogueFont, width - 128);

                        Utility.drawTextWithShadow(b, transcriptParse, Game1.dialogueFont, new Vector2(xPositionOnScreen + 64, textHeight - 8f), Game1.textColor, 1f, -1f, -1, -1, 1f, 3);

                        textHeight += Game1.dialogueFont.MeasureString(transcriptParse).Y;

                        _contentHeight = textHeight + scrollAmount - screen.Y;

                    }

                }

                b.End();

                b.GraphicsDevice.ScissorRectangle = scissorRectangle;

                b.Begin(0, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, new Matrix?());

                if (NeedsScroll())
                {

                    if (scrollAmount > 0.0)
                    {
                        b.Draw(Game1.staminaRect, new Rectangle(screen.X, screen.Top, screen.Width, 4), Color.Black * 0.15f);
                    }

                    if (scrollAmount < _contentHeight - (double)_scissorRectHeight)
                    {
                        b.Draw(Game1.staminaRect, new Rectangle(screen.X, screen.Bottom - 4, screen.Width, 4), Color.Black * 0.15f);
                    }

                }

            }

            if (NeedsScroll())
            {

                upArrow.draw(b);

                downArrow.draw(b);

                scrollBar.draw(b);

            }

            if (currentPage < pages.Count - 1 && questPage == -1)
            {

                b.Draw(iconTexture, new Vector2(forwardButton.bounds.X, forwardButton.bounds.Y) - ((forwardButton.scale - 4f) * new Vector2(16, 16)), Mod.instance.iconData.DisplayRect(Data.IconData.displays.forward), Color.White, 0f, Vector2.Zero, forwardButton.scale, 0, 999f);

                b.Draw(iconTexture, new Vector2(endButton.bounds.X,endButton.bounds.Y) - ((endButton.scale - 4f) * new Vector2(16, 16)), Mod.instance.iconData.DisplayRect(Data.IconData.displays.end), Color.White, 0f, Vector2.Zero, endButton.scale, 0, 999f);

            }

            if (currentPage > 0 && questPage == -1)
            {

                b.Draw(iconTexture, new Vector2(startButton.bounds.X, startButton.bounds.Y) - ((startButton.scale - 4f) * new Vector2(16, 16)), Mod.instance.iconData.DisplayRect(Data.IconData.displays.end), Color.White, 0f, Vector2.Zero, startButton.scale, SpriteEffects.FlipHorizontally, 999f);

            }

            //back
            b.Draw(iconTexture, new Vector2(backButton.bounds.X, backButton.bounds.Y) - ((backButton.scale - 4f) * new Vector2(16,16)), Mod.instance.iconData.DisplayRect(Data.IconData.displays.forward), Color.White, 0f, Vector2.Zero, backButton.scale, SpriteEffects.FlipHorizontally, 999f);

            // reverse
            b.Draw(iconTexture, new Vector2(reverseButton.bounds.X, reverseButton.bounds.Y) - ((reverseButton.scale - 4f) * new Vector2(16, 16)), Mod.instance.iconData.DisplayRect(Data.IconData.displays.reverse), Color.White * (reverse ? 1f : 0.65f), 0f, Vector2.Zero, reverseButton.scale, 0, 999f);

            // active
            b.Draw(iconTexture, new Vector2(activeButton.bounds.X, activeButton.bounds.Y) - ((activeButton.scale - 4f) * new Vector2(16, 16)), Mod.instance.iconData.DisplayRect(Data.IconData.displays.active), Color.White * (active ? 1f : 0.65f), 0f, Vector2.Zero, activeButton.scale, 0, 999f);

            // quests
            b.Draw(iconTexture, new Vector2(questsButton.bounds.X, questsButton.bounds.Y) - ((questsButton.scale - 4f) * new Vector2(16, 16)), Mod.instance.iconData.DisplayRect(Data.IconData.displays.quest), Color.White * (quests ? 1f : 0.65f), 0f, Vector2.Zero, questsButton.scale, 0, 999f);

            // effects
            b.Draw(iconTexture, new Vector2(effectsButton.bounds.X, effectsButton.bounds.Y) - ((effectsButton.scale - 4f) * new Vector2(16, 16)), Mod.instance.iconData.DisplayRect(Data.IconData.displays.effect), Color.White * (effects ? 1f : 0.65f), 0f, Vector2.Zero, effectsButton.scale, 0, 999f);

            if (upperRightCloseButton != null && shouldDrawCloseButton())
            {
                b.Draw(iconTexture, new Vector2(upperRightCloseButton.bounds.X, upperRightCloseButton.bounds.Y) - ((upperRightCloseButton.scale - 4f) * new Vector2(16, 16)), Mod.instance.iconData.DisplayRect(Data.IconData.displays.exit), Color.White, 0f, Vector2.Zero, upperRightCloseButton.scale, 0, 999f);

            }

            Game1.mouseCursorTransparency = 1f;
 
            drawMouse(b, false, -1);

            if (hoverText.Length <= 0)
            {
                return;
            }

            IClickableMenu.drawHoverText(b, hoverText, Game1.dialogueFont, 0, 0, -1, null, -1, null, null, 0, null, -1, -1, -1, 1f, null, null);
        
        }
    
    }

}
