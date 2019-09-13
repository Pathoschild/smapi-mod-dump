using FelixDev.StardewMods.FeTK.Framework.Data.Parsers;
using FelixDev.StardewMods.FeTK.Framework.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FelixDev.StardewMods.FeTK.Framework.UI
{
    /// <summary>
    /// This class extends the <see cref="LetterViewerMenu"/> class with additional functionality such as:
    ///         - Text Coloring support
    ///         - attached item draw bug fixed
    /// </summary>
    internal class LetterViewerMenuEx : LetterViewerMenu
    {
        /// <summary>Provides access to the <see cref="IReflectionHelper"/> API provided by SMAPI.</summary>
        private static readonly IReflectionHelper reflectionHelper = ToolkitMod.ModHelper.Reflection;

        /// <summary>Indicates that no quest is attached to the mail.</summary>
        protected const int QUEST_ID_NO_QUEST = -1;

        #region LetterViewerMenu Reflection Fields

        private IReflectedField<float> scaleRef;
        private IReflectedField<int> whichBGRef;

        private IReflectedField<int> moneyIncludedRef;
        private IReflectedField<int> questIdRef;

        private IReflectedField<List<string>> mailMessageRef;
        private IReflectedField<int> pageRef;

        private IReflectedField<string> learnedRecipeRef;
        private IReflectedField<string> cookingOrCraftingRef;
        
        private IReflectedField<int> secretNoteImageRef;

        private IReflectedMethod getTextColorRef;

        #endregion // LetterViewerMenu Reflection Fields

        /// <summary>The money included in the mail.</summary>
        protected int MoneyIncluded
        {
            get => moneyIncludedRef.GetValue();
            set => moneyIncludedRef.SetValue(value);
        }

        /// <summary>The ID of the quest attached to the mail.</summary>
        /// <remarks>Once the quest is accepted by the player, this ID is set to <see cref="QUEST_ID_NO_QUEST"/>.</remarks>
        protected int QuestId
        {
            get => questIdRef.GetValue();
            set => questIdRef.SetValue(value);
        }

        /// <summary>The name of the attached recipe.</summary>
        protected string LearnedRecipe
        {
            get => learnedRecipeRef.GetValue();
            set => learnedRecipeRef.SetValue(value);
        }

        /// <summary>The text stating the type of the recipe (i.e. cooking/crafting/...).</summary>
        protected string CookingOrCrafting
        {
            get => cookingOrCraftingRef.GetValue();
            set => cookingOrCraftingRef.SetValue(value);
        }

        /// <summary>Contains a collection of <see cref="TextColorInfo"/> objects, if any, for the specified mail content.</summary>
        private List<List<TextColorInfo>> textColorDataPerPage;

        /// <summary>The default text color to use for the mail's text content.</summary>
        private Color textColor;

        /// <summary>
        /// Create a new instance of the <see cref="LetterViewerMenuEx"/> class.
        /// </summary>
        /// <param name="text">The text content of the mail.</param>
        public LetterViewerMenuEx(string text)
            : base(text)
        {
            SetupReflectionAndParseContent();
        }

        /// <summary>
        /// Create a new instance of the <see cref="LetterViewerMenuEx"/> class.
        /// </summary>
        /// <param name="mailId">The ID of the mail.</param>
        /// <param name="content">The content of the mail. Can include text, attached items, attached money,....</param>
        public LetterViewerMenuEx(string mailId, string content)
            : base(content, mailId)
        {
            SetupReflectionAndParseContent();
        }

        /// <summary>
        /// Setup reflection fields and parse the mail's text content for use of the Text Coloring API.
        /// </summary>
        private void SetupReflectionAndParseContent()
        {
            #region Setup Reflection

            // private fields

            scaleRef = reflectionHelper
                .GetField<float>(this, "scale");

            whichBGRef = reflectionHelper
                .GetField<int>(this, "whichBG");

            mailMessageRef = reflectionHelper
                .GetField<List<string>>(this, "mailMessage");

            pageRef = reflectionHelper
                .GetField<int>(this, "page");

            moneyIncludedRef = reflectionHelper
                .GetField<int>(this, "moneyIncluded");

            learnedRecipeRef = reflectionHelper
                    .GetField<string>(this, "learnedRecipe");

            cookingOrCraftingRef = reflectionHelper
                    .GetField<string>(this, "cookingOrCrafting");

            questIdRef = reflectionHelper
                    .GetField<int>(this, "questID");

            secretNoteImageRef = reflectionHelper
                    .GetField<int>(this, "secretNoteImage");

            // private methods

            getTextColorRef = reflectionHelper
                .GetMethod(this, "getTextColor");

            #endregion // Reflection
           
            string content;
            List<string> mailMessage = mailMessageRef.GetValue();

            // Retrieve the game-parsed mail content if content is not set. Since the mail content has already
            // been potentially sliced up into multiple pages, we need to combine those pages again to get the
            // complete mail content so we can successfully parse the content for the text coloring API.
            if (mailMessage.Count > 1)
            {
                StringBuilder contentBuilder = new StringBuilder();
                mailMessage.ForEach(page => contentBuilder.Append(page));

                content = contentBuilder.ToString();
            }
            else
            {
                content = mailMessage[0];
            }

            // Check if the mail content uses the text coloring API and parse it accordingly.
            bool couldParse = StringColorParser.TryParse(content, SpriteTextHelper.GetColorFromIndex(getTextColorRef.Invoke<int>()), out List<TextColorInfo> textColorData);
            if (couldParse)
            {
                // Construct the new mail content with all <color> tags removed.
                StringBuilder parsedStringBuilder = new StringBuilder();
                textColorData.ForEach(mapping => parsedStringBuilder.Append(mapping.Text));

                var parsedString = parsedStringBuilder.ToString();

                // The mail content was parsed successfully. The original mail content might have contained pairs of <color></color> tags 
                // which then were removed in the resulting parsed string output. Hence the length of the resulting parsed string and the
                // length of the original mail content string might differ (the former being shorter) which requires a new run to break up
                // the resulting mail content into different mail pages. The previous run worked on the original mail message whch might have
                // contained now removed <color> tags.
                if (parsedString.Length < content.Length)
                {
                    mailMessageRef.SetValue(SpriteText.getStringBrokenIntoSectionsOfHeight(parsedString, this.width - 64, this.height - 128));
                }

                mailMessage = mailMessageRef.GetValue();

                // If the mail content did not contain any <color> tags, we set the default mail-content text color 
                // based on the mail's background.
                if (parsedString.Length == content.Length)
                {
                    textColor = SpriteTextHelper.GetColorFromIndex(getTextColorRef.Invoke<int>());
                    return;
                }

                // If the mail content contained a single pair of <color> tags which enclosed the entire actual 
                // mail content, the entire mail content will be drawn in the same color, that is the color specified
                // by the color tag.
                // Example: <color=#0000FF>mail content</color>
                else if (textColorData.Count == 1)
                {
                    textColor = textColorData[0].Color;
                }

                // If the mail content is to be drawn in at least two different colors and the mail content has been sliced up
                // into multiple pages, we also need to "slice up" our TextColorInfo data so that each content page will only 
                // contain the TextColorInfo data relevant to it.
                else
                {
                    textColorDataPerPage = new List<List<TextColorInfo>>(mailMessage.Count);
                    for (int i = 0; i < mailMessage.Count; i++)
                    {
                        textColorDataPerPage.Add(new List<TextColorInfo>());
                    }

                    int currentBlockIndex = 0; // The current TextColorInfo block we are assigning to a content page.
                    int currentIndexInBlock = 0; // The current index into the current TextColorInfo block.

                    for (int i = 0; i < mailMessage.Count; i++)
                    {
                        // As long as there are still page characters left which have not yet been assigned a TextColorInfo (sub)block to,
                        // we continue to assign TextColorInfo blocks to the page content.
                        int remainingCharsPerPage = mailMessage[i].Length;
                        while (remainingCharsPerPage > 0)
                        {
                            // A TextColorInfo block can contain a string which is shorter, of same length or longer than the remaining 
                            // unassigned content of the current mail page. In the first two cases (shorter or of same length) we assign 
                            // all the unassigned content of the current TextColorInfo block (some of its text data could have already  
                            // been assigned to a page -- see second case below) to the current page.
                            //
                            // In the second case, the current TextColorInfo block spans multiple content pages and we thus have to split it 
                            // up into multiple sub TextColorInfo blocks (one block per page the TextColorInfo block is spanning). 
                            // Splitting up means that one part of the TextColorInfo block will be assigned to a different page than the rest of 
                            // the TextColorInfo block. We keep track of the TextColorInfo parts which are unassgined yet using "currentIndexInBlock".

                            // First case, the unassigned part of the current TextColorInfo block fits into the remaining content
                            // of the current page.
                            // Note: Since a TextColorInfo block can potentially span more than two pages, we also have to make 
                            // sure to ignore any already assigned parts of the current TextColorInfo block.
                            if (textColorData[currentBlockIndex].Text.Length - currentIndexInBlock <= remainingCharsPerPage)
                            {
                                string blockText = (currentIndexInBlock > 0)
                                    ? textColorData[currentBlockIndex].Text.Substring(currentIndexInBlock)
                                    : textColorData[currentBlockIndex].Text;

                                textColorDataPerPage[i].Add(new TextColorInfo(blockText, textColorData[currentBlockIndex].Color));

                                remainingCharsPerPage -= textColorData[currentBlockIndex].Text.Length - currentIndexInBlock;

                                currentBlockIndex++;
                                currentIndexInBlock = 0;
                            }

                            // Second case, the unassigned part of the current TextColorInfo block spans at least two pages:
                            // Split it up into an unassigned part for the current page and a remaining unassigned part for
                            // the next page(s). Then assign the first part to the current page.
                            // Note: Since a TextColorInfo block can potentially span more than two pages, we also have to make 
                            // sure to ignore any already assigned parts of the current TextColorInfo block.
                            else
                            {
                                string splitBlockText = textColorData[currentBlockIndex].Text.Substring(currentIndexInBlock, remainingCharsPerPage);

                                textColorDataPerPage[i].Add(new TextColorInfo(splitBlockText, textColorData[currentBlockIndex].Color));

                                currentIndexInBlock += remainingCharsPerPage;
                                remainingCharsPerPage = 0;
                            }
                        }
                    }
                }

                // We potentially changed the number of pages the mail content has been split up
                // after the content was parsed by the text coloring parser. Hence we might need to
                // update the [Back Button] and [Forward Button] settings. 
                if (Game1.options.SnappyMenus && mailMessage?.Count <= 1)
                {
                    this.backButton.myID = -100;
                    this.forwardButton.myID = -100;
                }
            }
        }

        /// <summary>
        /// Draw the letter menu. Our implementation fixes a bug in the game where only the last item
        /// of the attached mail items is always drawn.
        /// </summary>
        /// <param name="b"></param>
        public override void draw(SpriteBatch b)
        {
            #region Setup local variables with reflection

            int whichBG = whichBGRef.GetValue();
            float scale = scaleRef.GetValue();

            int page = pageRef.GetValue();
            List<string> mailMessage = mailMessageRef.GetValue();

            int secretNoteImage = secretNoteImageRef.GetValue();

            #endregion // Setup local variables with reflection

            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.4f);

            // Draw the letter background.
            b.Draw(letterTexture, new Vector2((float)(xPositionOnScreen + this.width / 2), (float)(this.yPositionOnScreen + this.height / 2)), new Rectangle?(new Rectangle(whichBG * 320, 0, 320, 180)), Color.White, 0.0f, new Vector2(160f, 90f), 4f * scale, SpriteEffects.None, 0.86f);

            if (scale == 1.0)
            {
                // Draw the secret note image.
                if (secretNoteImage != -1)
                {
                    b.Draw(this.secretNoteImageTexture, new Vector2((float)(this.xPositionOnScreen + this.width / 2 - 128 - 4), (float)(this.yPositionOnScreen + this.height / 2 - 128 + 8)), new Rectangle?(new Rectangle(secretNoteImage * 64 % this.secretNoteImageTexture.Width, secretNoteImage * 64 / this.secretNoteImageTexture.Width * 64, 64, 64)), Color.Black * 0.4f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.865f);
                    b.Draw(this.secretNoteImageTexture, new Vector2((float)(this.xPositionOnScreen + this.width / 2 - 128), (float)(this.yPositionOnScreen + this.height / 2 - 128)), new Rectangle?(new Rectangle(secretNoteImage * 64 % this.secretNoteImageTexture.Width, secretNoteImage * 64 / this.secretNoteImageTexture.Width * 64, 64, 64)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.865f);
                    b.Draw(this.secretNoteImageTexture, new Vector2((float)(this.xPositionOnScreen + this.width / 2 - 40), (float)(this.yPositionOnScreen + this.height / 2 - 192)), new Rectangle?(new Rectangle(193, 65, 14, 21)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.867f);
                }

                // Draw the mail content for the current mail page.
                else
                {                    
                    SpriteTextHelper.DrawString(b: b, s: mailMessage[page], x: this.xPositionOnScreen + 32, y: this.yPositionOnScreen + 32,
                        color: textColor, characterPosition: 999999,
                        width: this.width - 64, height: 999999, alpha: 0.75f, layerDepth: 0.865f,
                        drawBGScroll: -1, placeHolderScrollWidthText: "", textColorDataPerPage?[page]);
                }

                // Draw the attached items, if any.
                foreach (ClickableComponent clickableComponent in this.itemsToGrab)
                {
                    b.Draw(this.letterTexture, clickableComponent.bounds, new Rectangle?(new Rectangle(whichBG * 24, 180, 24, 24)), Color.White);
                    if (clickableComponent.item != null)
                    {
                        Vector2 itemMailLocation = new Vector2(clickableComponent.bounds.X + 16, clickableComponent.bounds.Y + 16);
                        clickableComponent.item.drawInMenu(b, itemMailLocation, clickableComponent.scale);

                        // Missing "break" in original game code (at least up to version 1.3.36). Without it, attached items will overdraw each other 
                        // from first to last, resulting in only the last attached item to be visible in the mail as long as there are any remaining
                        // attached items.
                        break;
                    }
                }

                // Draw the amount of attached money, if any.
                if (this.MoneyIncluded > 0)
                {
                    DrawMoney(b);
                }

                // Draw the attached recipe, if any. 
                else if (this.LearnedRecipe != null && this.LearnedRecipe.Length > 0)
                {
                    string s = Game1.content.LoadString("Strings\\UI:LetterViewer_LearnedRecipe", this.CookingOrCrafting);
                    SpriteText.drawStringHorizontallyCenteredAt(b, s, this.xPositionOnScreen + this.width / 2, this.yPositionOnScreen + this.height - 32 - SpriteText.getHeightOfString(s, 999999) * 2, 999999, this.width - 64, 9999, 0.65f, 0.865f, false, -1, 99999);
                    SpriteText.drawStringHorizontallyCenteredAt(b, Game1.content.LoadString("Strings\\UI:LetterViewer_LearnedRecipeName", this.LearnedRecipe), this.xPositionOnScreen + this.width / 2, this.yPositionOnScreen + this.height - 32 - SpriteText.getHeightOfString("t", 999999), 999999, this.width - 64, 9999, 0.9f, 0.865f, false, -1, 99999);
                }

                //base.draw(b);

                /// The <see cref="IClickableMenu.draw(SpriteBatch)"/> function.
                void BaseDraw(SpriteBatch b2)
                {
                    this.upperRightCloseButton?.draw(b2);
                }
                BaseDraw(b);

                if (page < mailMessage.Count - 1)
                    this.forwardButton.draw(b);

                if (page > 0)
                    this.backButton.draw(b);

                // Draw the [Accept Quest] button if this is a quest mail.
                if (this.QuestId != QUEST_ID_NO_QUEST)
                {
                    IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 373, 9, 9), this.acceptQuestButton.bounds.X, this.acceptQuestButton.bounds.Y, this.acceptQuestButton.bounds.Width, this.acceptQuestButton.bounds.Height, (double)this.acceptQuestButton.scale > 1.0 ? Color.LightPink : Color.White, 4f * this.acceptQuestButton.scale, true);
                    Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\UI:AcceptQuest"), Game1.dialogueFont, new Vector2((float)(this.acceptQuestButton.bounds.X + 12), (float)(this.acceptQuestButton.bounds.Y + (LocalizedContentManager.CurrentLanguageLatin ? 16 : 12))), Game1.textColor, 1f, -1f, -1, -1, 1f, 3);
                }
            }

            if (Game1.options.hardwareCursor)
                return;

            // Draw the mouse cursor.
            b.Draw(Game1.mouseCursors, new Vector2(Game1.getMouseX(), Game1.getMouseY()),
                new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 0, 16, 16)),
                Color.White, 0.0f, Vector2.Zero, (float)(4.0 + Game1.dialogueButtonScale / 150.0),
                SpriteEffects.None, 1f);
        }

        /// <summary>
        /// Draws the attached money to the screen.
        /// </summary>
        /// <param name="b">The sprite batch used to draw to the screen.</param>
        protected virtual void DrawMoney(SpriteBatch b)
        {
            string s = Game1.content.LoadString("Strings\\UI:LetterViewer_MoneyIncluded", this.MoneyIncluded);
            SpriteText.drawString(b, s, this.xPositionOnScreen + this.width / 2 - SpriteText.getWidthOfString(s, 999999) / 2, this.yPositionOnScreen + this.height - 96, 999999, -1, 9999, 0.75f, 0.865f, false, -1, "", -1);
        }
    }
}
