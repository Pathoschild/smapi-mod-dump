using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using System;
using System.Collections.Generic;

namespace MultipleBedLocations
{
    public class ModDialogueBox : IClickableMenu
    {
        private List<string> dialogues = new List<string>();

        private Dialogue characterDialogue;

        private Stack<string> characterDialoguesBrokenUp = new Stack<string>();

        private List<Response> responses = new List<Response>();

        public const int portraitBoxSize = 74;

        public const int nameTagWidth = 102;

        public const int nameTagHeight = 18;

        public const int portraitPlateWidth = 115;

        public const int nameTagSideMargin = 5;

        public const float transitionRate = 3f;

        public const int characterAdvanceDelay = 30;

        public const int safetyDelay = 750;

        public static int questionFinishPauseTimer;

        private Rectangle friendshipJewel = Rectangle.Empty;

        private bool activatedByGamePad;

        private int x;

        private int y;

        private int transitionX = -1;

        private int transitionY;

        private int transitionWidth;

        private int transitionHeight;

        private int characterAdvanceTimer;

        private int characterIndexInDialogue;

        private int safetyTimer = 750;

        private int heightForQuestions;

        private int selectedResponse = -1;

        private int newPortaitShakeTimer;

        private int gamePadIntroTimer;

        private bool transitioning = true;

        private bool transitioningBigger = true;

        private bool dialogueContinuedOnNextPage;

        private bool dialogueFinished;

        private bool isQuestion;

        private TemporaryAnimatedSprite dialogueIcon;

        private string hoverText = "";

        public ModDialogueBox(int x, int y, int width, int height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.activatedByGamePad = Game1.isAnyGamePadButtonBeingPressed();
            this.gamePadIntroTimer = 1000;
        }

        public ModDialogueBox(string dialogue)
        {
            this.activatedByGamePad = Game1.isAnyGamePadButtonBeingPressed();
            if (this.activatedByGamePad)
            {
                Game1.mouseCursorTransparency = 0f;
                this.gamePadIntroTimer = 1000;
            }
            this.dialogues.AddRange(dialogue.Split(new char[]
            {
                '#'
            }));
            this.width = Math.Min(1200, SpriteText.getWidthOfString(dialogue) + Game1.tileSize);
            this.height = SpriteText.getHeightOfString(dialogue, this.width - Game1.pixelZoom * 5) + Game1.pixelZoom;
            this.x = (int)Utility.getTopLeftPositionForCenteringOnScreen(this.width, this.height, 0, 0).X;
            this.y = Game1.viewport.Height - this.height - Game1.tileSize;
            this.setUpIcons();
        }

        public ModDialogueBox(string dialogue, List<Response> responses)
        {
            this.activatedByGamePad = Game1.isAnyGamePadButtonBeingPressed();
            this.gamePadIntroTimer = 1000;
            this.dialogues.Add(dialogue);
            this.responses = responses;
            this.isQuestion = true;
            this.width = 1200;
            this.setUpQuestions();
            this.height = this.heightForQuestions;
            this.x = (int)Utility.getTopLeftPositionForCenteringOnScreen(this.width, this.height, 0, 0).X;
            this.y = Game1.viewport.Height - this.height - Game1.tileSize;
            this.setUpIcons();
            this.characterIndexInDialogue = dialogue.Length - 1;
        }

        public ModDialogueBox(Dialogue dialogue)
        {
            this.characterDialogue = dialogue;
            this.activatedByGamePad = Game1.isAnyGamePadButtonBeingPressed();
            if (this.activatedByGamePad)
            {
                Game1.mouseCursorTransparency = 0f;
                this.gamePadIntroTimer = 1000;
            }
            this.width = 1200;
            this.height = 6 * Game1.tileSize;
            this.x = (int)Utility.getTopLeftPositionForCenteringOnScreen(this.width, this.height, 0, 0).X;
            this.y = Game1.viewport.Height - this.height - Game1.tileSize;
            this.friendshipJewel = new Rectangle(this.x + this.width - Game1.tileSize, this.y + Game1.tileSize * 4, 11 * Game1.pixelZoom, 11 * Game1.pixelZoom);
            this.characterDialoguesBrokenUp.Push(dialogue.getCurrentDialogue());
            this.checkDialogue(dialogue);
            this.newPortaitShakeTimer = ((this.characterDialogue.getPortraitIndex() == 1) ? 250 : 0);
            this.setUpForGamePadMode();
        }

        public ModDialogueBox(List<string> dialogues)
        {
            this.activatedByGamePad = Game1.isAnyGamePadButtonBeingPressed();
            if (this.activatedByGamePad)
            {
                Game1.mouseCursorTransparency = 0f;
                this.gamePadIntroTimer = 1000;
            }
            this.dialogues = dialogues;
            this.width = Math.Min(1200, SpriteText.getWidthOfString(dialogues[0]) + Game1.tileSize);
            this.height = SpriteText.getHeightOfString(dialogues[0], this.width - Game1.pixelZoom * 4);
            this.x = (int)Utility.getTopLeftPositionForCenteringOnScreen(this.width, this.height, 0, 0).X;
            this.y = Game1.viewport.Height - this.height - Game1.tileSize;
            this.setUpIcons();
        }

        public override bool autoCenterMouseCursorForGamepad()
        {
            return false;
        }

        private void playOpeningSound()
        {
            Game1.playSound("breathin");
        }

        public override void setUpForGamePadMode()
        {
            if (Game1.options.gamepadControls && (this.activatedByGamePad || !Game1.lastCursorMotionWasMouse))
            {
                this.gamePadControlsImplemented = true;
                if (this.isQuestion)
                {
                    int num = 0;
                    string currentString = this.getCurrentString();
                    if (currentString != null && currentString.Length > 0)
                    {
                        num = SpriteText.getHeightOfString(currentString, 999999);
                    }
                    Game1.setMousePosition(this.x + this.width - Game1.tileSize * 2, this.y + num + Game1.tileSize);
                    return;
                }
                Game1.mouseCursorTransparency = 0f;
            }
        }

        public void closeDialogue()
        {
            if (Game1.activeClickableMenu.Equals(this))
            {
                Game1.exitActiveMenu();
                Game1.dialogueUp = false;
                if (this.characterDialogue != null && this.characterDialogue.speaker != null && this.characterDialogue.speaker.CurrentDialogue.Count > 0 && this.dialogueFinished && this.characterDialogue.speaker.CurrentDialogue.Count > 0)
                {
                    this.characterDialogue.speaker.CurrentDialogue.Pop();
                }
                if (Game1.messagePause)
                {
                    Game1.pauseTime = 500f;
                }
                if (Game1.currentObjectDialogue.Count > 0)
                {
                    Game1.currentObjectDialogue.Dequeue();
                }
                Game1.currentDialogueCharacterIndex = 0;
                if (Game1.currentObjectDialogue.Count > 0)
                {
                    Game1.dialogueUp = true;
                    Game1.questionChoices.Clear();
                    Game1.dialogueTyping = true;
                }
                Game1.tvStation = -1;
                if (this.characterDialogue != null && this.characterDialogue.speaker != null && !this.characterDialogue.speaker.name.Equals("Gunther") && !Game1.eventUp && !this.characterDialogue.speaker.doingEndOfRouteAnimation)
                {
                    this.characterDialogue.speaker.doneFacingPlayer(Game1.player);
                }
                Game1.currentSpeaker = null;
                if (!Game1.eventUp)
                {
                    Game1.player.CanMove = true;
                    Game1.player.movementDirections.Clear();
                }
                else if (Game1.currentLocation.currentEvent.CurrentCommand > 0 || Game1.currentLocation.currentEvent.specialEventVariable1)
                {
                    if (!Game1.isFestival() || !Game1.currentLocation.currentEvent.canMoveAfterDialogue())
                    {
                        Event expr_1A3 = Game1.currentLocation.currentEvent;
                        int currentCommand = expr_1A3.CurrentCommand;
                        expr_1A3.CurrentCommand = currentCommand + 1;
                    }
                    else
                    {
                        Game1.player.CanMove = true;
                    }
                }
                Game1.questionChoices.Clear();
            }
            if (Game1.afterDialogues != null)
            {
                Game1.afterFadeFunction arg_1DB_0 = Game1.afterDialogues;
                Game1.afterDialogues = null;
                arg_1DB_0();
            }
        }

        public void finishTyping()
        {
            this.characterIndexInDialogue = this.getCurrentString().Length - 1;
        }

        public void beginOutro()
        {
            this.transitioning = true;
            this.transitioningBigger = false;
            Game1.playSound("breathout");
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            this.receiveLeftClick(x, y, playSound);
        }

        private void tryOutro()
        {
            if (Game1.activeClickableMenu != null && Game1.activeClickableMenu.Equals(this))
            {
                this.beginOutro();
            }
        }

        public override void receiveKeyPress(Keys key)
        {
            if (Game1.options.doesInputListContain(Game1.options.actionButton, key))
            {
                this.receiveLeftClick(0, 0, true);
                return;
            }
            if (this.isQuestion && !Game1.eventUp && this.characterDialogue == null)
            {
                if (Game1.options.doesInputListContain(Game1.options.menuButton, key))
                {
                    if (this.responses != null && this.responses.Count > 0 && Question.answerDialogueWrapper(Game1.currentLocation,this.responses[this.responses.Count - 1]))
                    {
                        Game1.playSound("smallSelect");
                    }
                    this.selectedResponse = -1;
                    this.tryOutro();
                    return;
                }
                if (key == Keys.Y && this.responses != null && this.responses.Count > 0 && this.responses[0].responseKey.Equals("Yes") && Question.answerDialogueWrapper(Game1.currentLocation,this.responses[0]))
                {
                    Game1.playSound("smallSelect");
                    this.selectedResponse = -1;
                    this.tryOutro();
                    return;
                }
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (!this.transitioning)
            {
                if (this.characterIndexInDialogue < this.getCurrentString().Length - 1)
                {
                    this.characterIndexInDialogue = this.getCurrentString().Length - 1;
                    return;
                }
                if (this.safetyTimer > 0)
                {
                    return;
                }
                if (this.isQuestion)
                {
                    if (this.selectedResponse == -1)
                    {
                        return;
                    }
                    DialogueBox.questionFinishPauseTimer = (Game1.eventUp ? 600 : 200);
                    this.transitioning = true;
                    this.transitionX = -1;
                    this.transitioningBigger = true;
                    if (this.characterDialogue != null)
                    {
                        this.characterDialoguesBrokenUp.Pop();
                        this.characterDialogue.chooseResponse(this.responses[this.selectedResponse]);
                        this.characterDialoguesBrokenUp.Push("");
                        Game1.playSound("smallSelect");
                    }
                    else
                    {
                        Game1.dialogueUp = false;
                        if (Game1.eventUp)
                        {
                            Game1.playSound("smallSelect");
                            Game1.currentLocation.currentEvent.answerDialogue(Game1.currentLocation.lastQuestionKey, this.selectedResponse);
                            this.selectedResponse = -1;
                            this.tryOutro();
                            return;
                        }
                        if (Question.answerDialogue(this.responses[this.selectedResponse]))
                        {
                            Game1.playSound("smallSelect");
                        }
                        this.selectedResponse = -1;
                        this.tryOutro();
                        return;
                    }
                }
                else if (this.characterDialogue == null)
                {
                    this.dialogues.RemoveAt(0);
                    if (this.dialogues.Count == 0)
                    {
                        this.closeDialogue();
                    }
                    else
                    {
                        this.width = Math.Min(1200, SpriteText.getWidthOfString(this.dialogues[0]) + Game1.tileSize);
                        this.height = SpriteText.getHeightOfString(this.dialogues[0], this.width - Game1.pixelZoom * 4);
                        this.x = (int)Utility.getTopLeftPositionForCenteringOnScreen(this.width, this.height, 0, 0).X;
                        this.y = Game1.viewport.Height - this.height - Game1.tileSize * 2;
                        this.xPositionOnScreen = x;
                        this.yPositionOnScreen = y;
                        this.setUpIcons();
                    }
                }
                this.characterIndexInDialogue = 0;
                if (this.characterDialogue != null)
                {
                    int portraitIndex = this.characterDialogue.getPortraitIndex();
                    if (this.characterDialoguesBrokenUp.Count == 0)
                    {
                        this.beginOutro();
                        return;
                    }
                    this.characterDialoguesBrokenUp.Pop();
                    if (this.characterDialoguesBrokenUp.Count == 0)
                    {
                        if (!this.characterDialogue.isCurrentStringContinuedOnNextScreen)
                        {
                            this.beginOutro();
                        }
                        this.characterDialogue.exitCurrentDialogue();
                    }
                    if (!this.characterDialogue.isDialogueFinished() && this.characterDialogue.getCurrentDialogue().Length > 0 && this.characterDialoguesBrokenUp.Count == 0)
                    {
                        this.characterDialoguesBrokenUp.Push(this.characterDialogue.getCurrentDialogue());
                    }
                    this.checkDialogue(this.characterDialogue);
                    if (this.characterDialogue.getPortraitIndex() != portraitIndex)
                    {
                        this.newPortaitShakeTimer = ((this.characterDialogue.getPortraitIndex() == 1) ? 250 : 50);
                    }
                }
                if (!this.transitioning)
                {
                    Game1.playSound("smallSelect");
                }
                this.setUpIcons();
                this.safetyTimer = 750;
                if (this.getCurrentString() != null && this.getCurrentString().Length <= 20)
                {
                    this.safetyTimer -= 200;
                }
            }
        }

        private void setUpIcons()
        {
            this.dialogueIcon = null;
            if (this.isQuestion)
            {
                this.setUpQuestionIcon();
            }
            else if (this.characterDialogue != null && (this.characterDialogue.isCurrentStringContinuedOnNextScreen || this.characterDialoguesBrokenUp.Count > 1))
            {
                this.setUpNextPageIcon();
            }
            else if (this.dialogues != null && this.dialogues.Count > 1)
            {
                this.setUpNextPageIcon();
            }
            else
            {
                this.setUpCloseDialogueIcon();
            }
            this.setUpForGamePadMode();
            if (this.getCurrentString() != null && this.getCurrentString().Length <= 20)
            {
                this.safetyTimer -= 200;
            }
        }

        public override void performHoverAction(int mouseX, int mouseY)
        {
            this.hoverText = "";
            if (!this.transitioning && this.characterIndexInDialogue >= this.getCurrentString().Length - 1)
            {
                base.performHoverAction(mouseX, mouseY);
                if (this.isQuestion)
                {
                    int num = this.selectedResponse;
                    int num2 = this.y - (this.heightForQuestions - this.height) + SpriteText.getHeightOfString(this.getCurrentString(), this.width) + Game1.pixelZoom * 12;
                    for (int i = 0; i < this.responses.Count; i++)
                    {
                        SpriteText.getHeightOfString(this.responses[i].responseText, this.width);
                        if (mouseY >= num2 && mouseY < num2 + SpriteText.getHeightOfString(this.responses[i].responseText, this.width))
                        {
                            this.selectedResponse = i;
                            break;
                        }
                        num2 += SpriteText.getHeightOfString(this.responses[i].responseText, this.width) + Game1.pixelZoom * 4;
                    }
                    if (this.selectedResponse != num)
                    {
                        Game1.playSound("Cowboy_gunshot");
                    }
                }
            }
            if (!Game1.eventUp && !this.friendshipJewel.Equals(Rectangle.Empty) && this.friendshipJewel.Contains(mouseX, mouseY) && this.characterDialogue != null && this.characterDialogue.speaker != null && Game1.player.friendships.ContainsKey(this.characterDialogue.speaker.name))
            {
                this.hoverText = string.Concat(new object[]
                {
                    Game1.player.getFriendshipHeartLevelForNPC(this.characterDialogue.speaker.name),
                    "/",
                    this.characterDialogue.speaker.name.Equals(Game1.player.spouse) ? "12" : "10",
                    "<"
                });
            }
        }

        private void setUpQuestionIcon()
        {
            Vector2 position = new Vector2((float)(this.x + this.width - 10 * Game1.pixelZoom), (float)(this.y + this.height - 11 * Game1.pixelZoom));
            this.dialogueIcon = new TemporaryAnimatedSprite(Game1.mouseCursors, new Rectangle(330, 357, 7, 13), 100f, 6, 999999, position, false, false, 0.89f, 0f, Color.White, (float)Game1.pixelZoom, 0f, 0f, 0f, true)
            {
                yPeriodic = true,
                yPeriodicLoopTime = 1500f,
                yPeriodicRange = (float)(Game1.tileSize / 8)
            };
        }

        private void setUpCloseDialogueIcon()
        {
            Vector2 position = new Vector2((float)(this.x + this.width - 10 * Game1.pixelZoom), (float)(this.y + this.height - 11 * Game1.pixelZoom));
            if (this.isPortraitBox())
            {
                position.X -= (float)(115 * Game1.pixelZoom + 8 * Game1.pixelZoom);
            }
            this.dialogueIcon = new TemporaryAnimatedSprite(Game1.mouseCursors, new Rectangle(289, 342, 11, 12), 80f, 11, 999999, position, false, false, 0.89f, 0f, Color.White, (float)Game1.pixelZoom, 0f, 0f, 0f, true);
        }

        private void setUpNextPageIcon()
        {
            Vector2 position = new Vector2((float)(this.x + this.width - 10 * Game1.pixelZoom), (float)(this.y + this.height - 10 * Game1.pixelZoom));
            if (this.isPortraitBox())
            {
                position.X -= (float)(115 * Game1.pixelZoom + 8 * Game1.pixelZoom);
            }
            this.dialogueIcon = new TemporaryAnimatedSprite(Game1.mouseCursors, new Rectangle(232, 346, 9, 9), 90f, 6, 999999, position, false, false, 0.89f, 0f, Color.White, (float)Game1.pixelZoom, 0f, 0f, 0f, true)
            {
                yPeriodic = true,
                yPeriodicLoopTime = 1500f,
                yPeriodicRange = (float)(Game1.tileSize / 8)
            };
        }

        private void checkDialogue(Dialogue d)
        {
            this.isQuestion = false;
            string text = "";
            if (this.characterDialoguesBrokenUp.Count == 1)
            {
                text = SpriteText.getSubstringBeyondHeight(this.characterDialoguesBrokenUp.Peek(), this.width - 115 * Game1.pixelZoom - 5 * Game1.pixelZoom, this.height - Game1.pixelZoom * 4);
            }
            if (text.Length > 0)
            {
                string text2 = this.characterDialoguesBrokenUp.Pop().Replace(Environment.NewLine, "");
                this.characterDialoguesBrokenUp.Push(text.Trim());
                this.characterDialoguesBrokenUp.Push(text2.Substring(0, text2.Length - text.Length + 1).Trim());
            }
            if (d.getCurrentDialogue().Length == 0)
            {
                this.dialogueFinished = true;
            }
            if (d.isCurrentStringContinuedOnNextScreen || this.characterDialoguesBrokenUp.Count > 1)
            {
                this.dialogueContinuedOnNextPage = true;
            }
            else if (d.getCurrentDialogue().Length == 0)
            {
                this.beginOutro();
            }
            if (d.isCurrentDialogueAQuestion())
            {
                this.responses = d.getResponseOptions();
                this.isQuestion = true;
                this.setUpQuestions();
            }
        }

        private void setUpQuestions()
        {
            int widthConstraint = this.width - Game1.pixelZoom * 4;
            this.heightForQuestions = SpriteText.getHeightOfString(this.getCurrentString(), widthConstraint);
            foreach (Response current in this.responses)
            {
                this.heightForQuestions += SpriteText.getHeightOfString(current.responseText, widthConstraint) + Game1.pixelZoom * 4;
            }
            this.heightForQuestions += Game1.pixelZoom * 10;
        }

        public bool isPortraitBox()
        {
            return this.characterDialogue != null && this.characterDialogue.speaker != null && this.characterDialogue.speaker.Portrait != null && this.characterDialogue.showPortrait && Game1.options.showPortraits;
        }

        public void drawBox(SpriteBatch b, int xPos, int yPos, int boxWidth, int boxHeight)
        {
            if (xPos > 0)
            {
                b.Draw(Game1.mouseCursors, new Rectangle(xPos, yPos, boxWidth, boxHeight), new Rectangle?(new Rectangle(306, 320, 16, 16)), Color.White);
                b.Draw(Game1.mouseCursors, new Rectangle(xPos, yPos - 5 * Game1.pixelZoom, boxWidth, 6 * Game1.pixelZoom), new Rectangle?(new Rectangle(275, 313, 1, 6)), Color.White);
                b.Draw(Game1.mouseCursors, new Rectangle(xPos + 3 * Game1.pixelZoom, yPos + boxHeight, boxWidth - 5 * Game1.pixelZoom, 8 * Game1.pixelZoom), new Rectangle?(new Rectangle(275, 328, 1, 8)), Color.White);
                b.Draw(Game1.mouseCursors, new Rectangle(xPos - 8 * Game1.pixelZoom, yPos + 6 * Game1.pixelZoom, 8 * Game1.pixelZoom, boxHeight - 7 * Game1.pixelZoom), new Rectangle?(new Rectangle(264, 325, 8, 1)), Color.White);
                b.Draw(Game1.mouseCursors, new Rectangle(xPos + boxWidth, yPos, 7 * Game1.pixelZoom, boxHeight), new Rectangle?(new Rectangle(293, 324, 7, 1)), Color.White);
                b.Draw(Game1.mouseCursors, new Vector2((float)(xPos - 11 * Game1.pixelZoom), (float)(yPos - 7 * Game1.pixelZoom)), new Rectangle?(new Rectangle(261, 311, 14, 13)), Color.White, 0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, 0.87f);
                b.Draw(Game1.mouseCursors, new Vector2((float)(xPos + boxWidth - Game1.pixelZoom * 2), (float)(yPos - 7 * Game1.pixelZoom)), new Rectangle?(new Rectangle(291, 311, 12, 11)), Color.White, 0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, 0.87f);
                b.Draw(Game1.mouseCursors, new Vector2((float)(xPos + boxWidth - Game1.pixelZoom * 2), (float)(yPos + boxHeight - 2 * Game1.pixelZoom)), new Rectangle?(new Rectangle(291, 326, 12, 12)), Color.White, 0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, 0.87f);
                b.Draw(Game1.mouseCursors, new Vector2((float)(xPos - 11 * Game1.pixelZoom), (float)(yPos + boxHeight - Game1.pixelZoom)), new Rectangle?(new Rectangle(261, 327, 14, 11)), Color.White, 0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, 0.87f);
            }
        }

        private bool shouldPortraitShake(Dialogue d)
        {
            int portraitIndex = d.getPortraitIndex();
            return (d.speaker.name.Equals("Pam") && portraitIndex == 3) || (d.speaker.name.Equals("Abigail") && portraitIndex == 7) || (d.speaker.name.Equals("Haley") && portraitIndex == 5) || (d.speaker.name.Equals("Maru") && portraitIndex == 9) || this.newPortaitShakeTimer > 0;
        }

        public void drawPortrait(SpriteBatch b)
        {
            if (this.width >= 107 * Game1.pixelZoom * 3 / 2)
            {
                int num = this.x + this.width - 112 * Game1.pixelZoom + Game1.pixelZoom;
                int num2 = this.x + this.width - num;
                b.Draw(Game1.mouseCursors, new Rectangle(num - 10 * Game1.pixelZoom, this.y, 9 * Game1.pixelZoom, this.height), new Rectangle?(new Rectangle(278, 324, 9, 1)), Color.White);
                b.Draw(Game1.mouseCursors, new Vector2((float)(num - 10 * Game1.pixelZoom), (float)(this.y - 5 * Game1.pixelZoom)), new Rectangle?(new Rectangle(278, 313, 10, 7)), Color.White, 0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, 0.88f);
                b.Draw(Game1.mouseCursors, new Vector2((float)(num - 10 * Game1.pixelZoom), (float)(this.y + this.height)), new Rectangle?(new Rectangle(278, 328, 10, 8)), Color.White, 0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, 0.88f);
                int num3 = num + Game1.pixelZoom * 19;
                int num4 = this.y + this.height / 2 - 74 * Game1.pixelZoom / 2 - 18 * Game1.pixelZoom / 2;
                b.Draw(Game1.mouseCursors, new Vector2((float)(num - 2 * Game1.pixelZoom), (float)this.y), new Rectangle?(new Rectangle(583, 411, 115, 97)), Color.White, 0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, 0.88f);
                Rectangle sourceRectForStandardTileSheet = Game1.getSourceRectForStandardTileSheet(this.characterDialogue.speaker.Portrait, this.characterDialogue.getPortraitIndex(), 64, 64);
                if (!this.characterDialogue.speaker.Portrait.Bounds.Contains(sourceRectForStandardTileSheet))
                {
                    sourceRectForStandardTileSheet = new Rectangle(0, 0, 64, 64);
                }
                int num5 = this.shouldPortraitShake(this.characterDialogue) ? Game1.random.Next(-1, 2) : 0;
                b.Draw(this.characterDialogue.speaker.Portrait, new Vector2((float)(num3 + 4 * Game1.pixelZoom + num5), (float)(num4 + 6 * Game1.pixelZoom)), new Rectangle?(sourceRectForStandardTileSheet), Color.White, 0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, 0.88f);
                SpriteText.drawStringHorizontallyCenteredAt(b, this.characterDialogue.speaker.getName(), num + num2 / 2, num4 + 74 * Game1.pixelZoom + 4 * Game1.pixelZoom, 999999, -1, 999999, 1f, 0.88f, false, -1);
                if (!Game1.eventUp && !this.friendshipJewel.Equals(Rectangle.Empty) && this.characterDialogue != null && this.characterDialogue.speaker != null && Game1.player.friendships.ContainsKey(this.characterDialogue.speaker.name))
                {
                    b.Draw(Game1.mouseCursors, new Vector2((float)this.friendshipJewel.X, (float)this.friendshipJewel.Y), new Rectangle?((Game1.player.getFriendshipHeartLevelForNPC(this.characterDialogue.speaker.name) >= 10) ? new Rectangle(269, 494, 11, 11) : new Rectangle(Math.Max(140, 140 + (int)(Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 1000.0 / 250.0) * 11), Math.Max(532, 532 + Game1.player.getFriendshipHeartLevelForNPC(this.characterDialogue.speaker.name) / 2 * 11), 11, 11)), Color.White, 0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, 0.88f);
                }
            }
        }

        public string getCurrentString()
        {
            if (this.characterDialogue != null)
            {
                string text;
                if (this.characterDialoguesBrokenUp.Count > 0)
                {
                    text = this.characterDialoguesBrokenUp.Peek().Trim().Replace(Environment.NewLine, "");
                }
                else
                {
                    text = this.characterDialogue.getCurrentDialogue().Trim().Replace(Environment.NewLine, "");
                }
                if (!Game1.options.showPortraits)
                {
                    text = this.characterDialogue.speaker.getName() + ": " + text;
                }
                return text;
            }
            if (this.dialogues.Count > 0)
            {
                return this.dialogues[0].Trim().Replace(Environment.NewLine, "");
            }
            return "";
        }

        public override void update(GameTime time)
        {
            base.update(time);
            if (!Game1.lastCursorMotionWasMouse && !this.isQuestion)
            {
                Game1.mouseCursorTransparency = 0f;
            }
            else
            {
                Game1.mouseCursorTransparency = 1f;
            }
            if (this.gamePadIntroTimer > 0 && !this.isQuestion)
            {
                Game1.mouseCursorTransparency = 0f;
                this.gamePadIntroTimer -= time.ElapsedGameTime.Milliseconds;
            }
            if (this.safetyTimer > 0)
            {
                this.safetyTimer -= time.ElapsedGameTime.Milliseconds;
            }
            if (DialogueBox.questionFinishPauseTimer > 0)
            {
                DialogueBox.questionFinishPauseTimer -= time.ElapsedGameTime.Milliseconds;
                return;
            }
            if (this.transitioning)
            {
                if (this.transitionX == -1)
                {
                    this.transitionX = this.x + this.width / 2;
                    this.transitionY = this.y + this.height / 2;
                    this.transitionWidth = 0;
                    this.transitionHeight = 0;
                }
                if (this.transitioningBigger)
                {
                    bool arg_267_0 = this.transitionWidth != 0;
                    this.transitionX -= (int)((float)time.ElapsedGameTime.Milliseconds * 3f);
                    this.transitionY -= (int)((float)time.ElapsedGameTime.Milliseconds * 3f * ((float)(this.isQuestion ? this.heightForQuestions : this.height) / (float)this.width));
                    this.transitionX = Math.Max(this.x, this.transitionX);
                    this.transitionY = Math.Max(this.isQuestion ? (this.y + this.height - this.heightForQuestions) : this.y, this.transitionY);
                    this.transitionWidth += (int)((float)time.ElapsedGameTime.Milliseconds * 3f * 2f);
                    this.transitionHeight += (int)((float)time.ElapsedGameTime.Milliseconds * 3f * ((float)(this.isQuestion ? this.heightForQuestions : this.height) / (float)this.width) * 2f);
                    this.transitionWidth = Math.Min(this.width, this.transitionWidth);
                    this.transitionHeight = Math.Min(this.isQuestion ? this.heightForQuestions : this.height, this.transitionHeight);
                    if (!arg_267_0 && this.transitionWidth > 0)
                    {
                        this.playOpeningSound();
                    }
                    if (this.transitionX == this.x && this.transitionY == (this.isQuestion ? (this.y + this.height - this.heightForQuestions) : this.y))
                    {
                        this.transitioning = false;
                        this.characterAdvanceTimer = 90;
                        this.setUpIcons();
                        this.transitionX = this.x;
                        this.transitionY = this.y;
                        this.transitionWidth = this.width;
                        this.transitionHeight = this.height;
                    }
                }
                else
                {
                    this.transitionX += (int)((float)time.ElapsedGameTime.Milliseconds * 3f);
                    this.transitionY += (int)((float)time.ElapsedGameTime.Milliseconds * 3f * ((float)this.height / (float)this.width));
                    this.transitionX = Math.Min(this.x + this.width / 2, this.transitionX);
                    this.transitionY = Math.Min(this.y + this.height / 2, this.transitionY);
                    this.transitionWidth -= (int)((float)time.ElapsedGameTime.Milliseconds * 3f * 2f);
                    this.transitionHeight -= (int)((float)time.ElapsedGameTime.Milliseconds * 3f * ((float)this.height / (float)this.width) * 2f);
                    this.transitionWidth = Math.Max(0, this.transitionWidth);
                    this.transitionHeight = Math.Max(0, this.transitionHeight);
                    if (this.transitionWidth == 0 && this.transitionHeight == 0)
                    {
                        this.closeDialogue();
                    }
                }
            }
            if (!this.transitioning && this.characterIndexInDialogue < this.getCurrentString().Length)
            {
                this.characterAdvanceTimer -= time.ElapsedGameTime.Milliseconds;
                if (this.characterAdvanceTimer <= 0)
                {
                    this.characterAdvanceTimer = 30;
                    int num = this.characterIndexInDialogue;
                    this.characterIndexInDialogue = Math.Min(this.characterIndexInDialogue + 1, this.getCurrentString().Length);
                    if (this.characterIndexInDialogue != num && this.characterIndexInDialogue == this.getCurrentString().Length)
                    {
                        Game1.playSound("dialogueCharacterClose");
                    }
                    if (this.characterIndexInDialogue > 1 && this.characterIndexInDialogue < this.getCurrentString().Length && Game1.options.dialogueTyping)
                    {
                        Game1.playSound("dialogueCharacter");
                    }
                }
            }
            if (!this.transitioning && this.dialogueIcon != null)
            {
                this.dialogueIcon.update(time);
            }
            if (!this.transitioning && this.newPortaitShakeTimer > 0)
            {
                this.newPortaitShakeTimer -= time.ElapsedGameTime.Milliseconds;
            }
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            this.width = 1200;
            this.height = 6 * Game1.tileSize;
            this.x = (int)Utility.getTopLeftPositionForCenteringOnScreen(this.width, this.height, 0, 0).X;
            this.y = Game1.viewport.Height - this.height - Game1.tileSize;
            this.friendshipJewel = new Rectangle(this.x + this.width - Game1.tileSize, this.y + Game1.tileSize * 4, 11 * Game1.pixelZoom, 11 * Game1.pixelZoom);
            this.setUpIcons();
        }

        public override void draw(SpriteBatch b)
        {
            if (this.width < Game1.tileSize / 4 || this.height < Game1.tileSize / 4)
            {
                return;
            }
            if (this.transitioning)
            {
                this.drawBox(b, this.transitionX, this.transitionY, this.transitionWidth, this.transitionHeight);
                if ((!this.activatedByGamePad || Game1.lastCursorMotionWasMouse || this.isQuestion || Game1.isGamePadThumbstickInMotion()) && (Game1.getMouseX() != 0 || Game1.getMouseY() != 0))
                {
                    base.drawMouse(b);
                }
                return;
            }
            if (this.isQuestion)
            {
                this.drawBox(b, this.x, this.y - (this.heightForQuestions - this.height), this.width, this.heightForQuestions);
                SpriteText.drawString(b, this.getCurrentString(), this.x + Game1.pixelZoom * 2, this.y + Game1.pixelZoom * 3 - (this.heightForQuestions - this.height), this.characterIndexInDialogue, this.width - Game1.pixelZoom * 4, 999999, 1f, 0.88f, false, -1, "", -1);
                if (this.characterIndexInDialogue >= this.getCurrentString().Length - 1)
                {
                    int num = this.y - (this.heightForQuestions - this.height) + SpriteText.getHeightOfString(this.getCurrentString(), this.width - Game1.pixelZoom * 4) + Game1.pixelZoom * 12;
                    for (int i = 0; i < this.responses.Count; i++)
                    {
                        if (i == this.selectedResponse)
                        {
                            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(375, 357, 3, 3), this.x + Game1.pixelZoom, num - Game1.pixelZoom * 2, this.width - Game1.pixelZoom * 2, SpriteText.getHeightOfString(this.responses[i].responseText, this.width - Game1.pixelZoom * 4) + Game1.pixelZoom * 4, Color.White, (float)Game1.pixelZoom, false);
                        }
                        SpriteText.drawString(b, this.responses[i].responseText, this.x + Game1.pixelZoom * 2, num, 999999, this.width, 999999, (this.selectedResponse == i) ? 1f : 0.6f, 0.88f, false, -1, "", -1);
                        num += SpriteText.getHeightOfString(this.responses[i].responseText, this.width) + Game1.pixelZoom * 4;
                    }
                }
            }
            else
            {
                this.drawBox(b, this.x, this.y, this.width, this.height);
                if (!this.isPortraitBox() && !this.isQuestion)
                {
                    SpriteText.drawString(b, this.getCurrentString(), this.x + Game1.pixelZoom * 2, this.y + Game1.pixelZoom * 2, this.characterIndexInDialogue, this.width, 999999, 1f, 0.88f, false, -1, "", -1);
                }
            }
            if (this.isPortraitBox() && !this.isQuestion)
            {
                this.drawPortrait(b);
                if (!this.isQuestion)
                {
                    SpriteText.drawString(b, this.getCurrentString(), this.x + Game1.pixelZoom * 2, this.y + Game1.pixelZoom * 2, this.characterIndexInDialogue, this.width - 115 * Game1.pixelZoom - 5 * Game1.pixelZoom, 999999, 1f, 0.88f, false, -1, "", -1);
                }
            }
            if (this.dialogueIcon != null && this.characterIndexInDialogue >= this.getCurrentString().Length - 1)
            {
                this.dialogueIcon.draw(b, true, 0, 0);
            }
            if ((!this.activatedByGamePad || Game1.lastCursorMotionWasMouse || this.isQuestion || Game1.isGamePadThumbstickInMotion()) && (Game1.getMouseX() != 0 || Game1.getMouseY() != 0))
            {
                base.drawMouse(b);
            }
            if (this.hoverText.Length > 0)
            {
                SpriteText.drawStringWithScrollBackground(b, this.hoverText, this.friendshipJewel.Center.X - SpriteText.getWidthOfString(this.hoverText) / 2, this.friendshipJewel.Y - Game1.tileSize, "", 1f, -1);
            }
        }
    }
}
