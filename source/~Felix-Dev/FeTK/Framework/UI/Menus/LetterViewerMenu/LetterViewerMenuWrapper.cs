using FelixDev.StardewMods.FeTK.Framework.Data.Parsers;
using FelixDev.StardewMods.FeTK.Framework.Services;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using FelixDev.StardewMods.Common.StardewValley;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.BellsAndWhistles;

namespace FelixDev.StardewMods.FeTK.Framework.UI
{
    /// <summary>
    /// This class is a wrapper around the <see cref="LetterViewerMenu"/> class to provide an extended API 
    /// such as:
    ///     - A <see cref="MenuClosed"/> event
    ///     - Programmatically settable attached items
    ///     - A text coloring API - see <see cref="StringColorParser"/>
    ///     
    /// It also fixes an in-game bug (up to 1.3.36 at least) which only displays the last attached item 
    /// in a collection of attached items.
    /// </summary>
    public class LetterViewerMenuWrapper
    {
        /// <summary>Provides access to the <see cref="IReflectionHelper"/> API provided by SMAPI.</summary>
        private static readonly IReflectionHelper reflectionHelper = ToolkitMod.ModHelper.Reflection;

        /// <summary>The <see cref="LetterViewerMenuEx2"/> instance used to display the mail.</summary>
        private readonly LetterViewerMenuEx2 letterMenu;

        /// <summary>Raised when the letter viewer menu has been closed.</summary>
        public event EventHandler<LetterViewerMenuClosedEventArgs> MenuClosed;

        /// <summary>
        /// Create a new instance of the <see cref="LetterViewerMenuWrapper"/> class.
        /// </summary>
        /// <param name="mail">The mail to create the menu for.</param>
        /// <exception cref="ArgumentNullException">The specified <paramref name="mail"/> is <c>null</c>.</exception>
        public LetterViewerMenuWrapper(Mail mail)
        {
            if (mail == null)
            {
                throw new ArgumentNullException(nameof(mail));
            }

            string textContent = mail.Text.Equals("") ? " " : mail.Text;

            switch (mail)
            {
                case ItemMail itemMail:
                    this.letterMenu =  LetterViewerMenuEx2.CreateItemMailMenu(itemMail.Id, textContent, itemMail.AttachedItems);
                    break;
                case MoneyMail moneyMail:
                    this.letterMenu = LetterViewerMenuEx2.CreateMoneyMailMenu(moneyMail.Id, textContent, moneyMail.AttachedMoney, moneyMail.Currency);
                    break;
                case RecipeMail recipeMail:
                    this.letterMenu = LetterViewerMenuEx2.CreateRecipeMailMenu(recipeMail.Id, textContent, recipeMail.Recipe);
                    break;
                case QuestMail questMail:
                    this.letterMenu = LetterViewerMenuEx2.CreateQuestMailMenu(questMail.Id, textContent, questMail.QuestId, questMail.IsAutomaticallyAccepted);
                    break;
                default:
                    this.letterMenu = new LetterViewerMenuEx2(mail.Id, textContent);
                    break;
            }

            this.letterMenu.exitFunction = new IClickableMenu.onExit(OnExit);
        }

        /// <summary>
        /// Display the menu.
        /// </summary>
        public void Show()
        {
            Game1.activeClickableMenu = letterMenu;
        }

        /// <summary>
        /// Called when the letter viewer menu is closed. Raises the <see cref="MenuClosed"/> event./>
        /// </summary>
        private void OnExit()
        {
            MenuClosed?.Invoke(this, new LetterViewerMenuClosedEventArgs(letterMenu.MailId, letterMenu.InteractionRecord));
        }

        /// <summary>
        /// This class extends the <see cref="LetterViewerMenuEx"/> class with additional functionality such as 
        ///         - keeping track of user interaction with the mail's content
        /// </summary>
        private class LetterViewerMenuEx2 : LetterViewerMenuEx
        {
            /// <summary>Provides access to the <see cref="IMonitor"/> API provided by SMAPI.</summary>
            private static readonly IMonitor monitor = ToolkitMod._Monitor;

            /// <summary>Gender switch character.</summary>
            private const string SPECIAL_TOKEN_GENDER_SWITCH = "|";

            /// <summary>Player name character.</summary>
            private const string SPECIAL_TOKEN_PLAYER_NAME = "@";

            /// <summary>Random NPC name command.</summary>
            private const string SPECIAL_COMMAND_RANDOM_NPC_NAME = "%secretsanta";

            /// <summary>The type of the mail visualized by this menu.</summary>
            private MailType mailType;

            /// <summary>The currency of the monetary value included in the mail.</summary>
            private Currency currency;

            /// <summary>The recipe attached to the mail.</summary>
            private RecipeData recipe;

            /// <summary>The ID of the quest included in the mail.</summary>
            /// <remarks>
            /// This differs from <see cref="LetterViewerMenuEx.QuestId"/> in that it will always hold the ID of the quest
            /// this menu was created with.
            /// </remarks>
            private int attachedQuestId = QUEST_ID_NO_QUEST;

            /// <summary>Indicates whether the quest included in the mail was accepted or not.</summary>
            private bool questAccepted = false;

            /// <summary>Contains the attached items which were selected by the player.</summary>
            private List<Item> selectedItems;

            /// <summary>
            /// Create a new instance of the <see cref="LetterViewerMenuEx2"/> class.
            /// </summary>
            /// <param name="id">The ID of the mail.</param>
            /// <param name="text">The text content of the mail.</param>
            /// <param name="attachedItems">The items attached to the mail. Can be <c>null</c>.</param>
            /// <returns>The created <see cref="LetterViewerMenuEx2"/> instance.</returns>
            public static LetterViewerMenuEx2 CreateItemMailMenu(string id, string text, IList<Item> attachedItems)
            {
                var menu = new LetterViewerMenuEx2(id, text)
                {
                    selectedItems = new List<Item>(),
                    mailType = MailType.ItemMail
                };

                // If the mail has attached items, add them to the LetterViewerMenu so they will be shown when the
                // mail is drawn to the screen.
                if (attachedItems?.Count > 0)
                {
                    foreach (var item in attachedItems)
                    {
                        var attachedItemComponent = new ClickableComponent(
                            new Rectangle(menu.xPositionOnScreen + menu.width / 2 - 48, menu.yPositionOnScreen + menu.height - 32 - 96, 96, 96),
                            item)
                        {
                            myID = region_itemGrabButton,
                            leftNeighborID = region_backButton,
                            rightNeighborID = region_forwardButton
                        };

                        menu.itemsToGrab.Add(attachedItemComponent);
                    }

                    menu.backButton.rightNeighborID = region_itemGrabButton;
                    menu.forwardButton.leftNeighborID = region_itemGrabButton;

                    if (!Game1.options.SnappyMenus)
                        return menu;

                    menu.populateClickableComponentList();
                    menu.snapToDefaultClickableComponent();
                }

                return menu;
            }

            /// <summary>
            /// Create a new instance of the <see cref="LetterViewerMenuEx2"/> class.
            /// </summary>
            /// <param name="id">The ID of the mail.</param>
            /// <param name="text">The text content of the mail.</param>
            /// <param name="money">The monetaray value attached to the mail.</param>
            /// <param name="currency">The currency of the specified <paramref name="money"/>.</param>
            /// <returns>The created <see cref="LetterViewerMenuEx2"/> instance.</returns>
            public static LetterViewerMenuEx2 CreateMoneyMailMenu(string id, string text, int money, Currency currency)
            {
                var menu = new LetterViewerMenuEx2(id, text)
                {
                    mailType = MailType.MoneyMail,

                    MoneyIncluded = money,
                    currency = currency
                };

                // Add mentary value to the player's account.
                switch (currency)
                {
                    case Currency.Money:
                        Game1.player.Money += money;
                        break;
                    case Currency.QiCoins:
                        Game1.player.clubCoins += money;
                        break;
                    case Currency.StarTokens:
                        Game1.player.festivalScore += money;
                        break;
                }

                return menu;
            }

            /// <summary>
            /// Create a new instance of the <see cref="LetterViewerMenuEx2"/> class.
            /// </summary>
            /// <param name="id">The ID of the mail.</param>
            /// <param name="text">The text content of the mail.</param>
            /// <param name="questId">The ID of the quest included in the mail.</param>
            /// <param name="isAutomaticallyAccepted">
            /// Indicates whether the included quest is automatically accepted when the mail is opened or if the 
            /// player needs to manually accept it.
            /// </param>
            /// <returns>The created <see cref="LetterViewerMenuEx2"/> instance.</returns>
            public static LetterViewerMenuEx2 CreateQuestMailMenu(string id, string text, int questId, bool isAutomaticallyAccepted)
            {
                var menu = new LetterViewerMenuEx2(id, text)
                {
                    mailType = MailType.QuestMail,
                    attachedQuestId = questId < 1 ? QUEST_ID_NO_QUEST : questId,
                };

                // If the ID does not represent an existing quest, we don't include it in the mail.
                if (menu.attachedQuestId == QUEST_ID_NO_QUEST)
                {
                    menu.QuestId = QUEST_ID_NO_QUEST;
                    return menu;
                }

                // Add the quest to the player's quest log if it is an automatically accepted quest.
                if (isAutomaticallyAccepted)
                {
                    Game1.player.addQuest(questId);

                    menu.questAccepted = true;
                    menu.QuestId = QUEST_ID_NO_QUEST;

                    return menu;
                }

                // Specified quest has to be manually accepted by the player -> setup [quest accept] button in the menu.

                menu.QuestId = questId;

                string label = Game1.content.LoadString("Strings\\UI:AcceptQuest");
                menu.acceptQuestButton = new ClickableComponent(
                    new Rectangle(menu.xPositionOnScreen + menu.width / 2 - 128, menu.yPositionOnScreen + menu.height - 128,
                                 (int)Game1.dialogueFont.MeasureString(label).X + 24,
                                 (int)Game1.dialogueFont.MeasureString(label).Y + 24),
                                 "")
                {
                    myID = region_acceptQuestButton,
                    rightNeighborID = region_forwardButton,
                    leftNeighborID = region_backButton
                };

                menu.backButton.rightNeighborID = region_acceptQuestButton;
                menu.forwardButton.leftNeighborID = region_acceptQuestButton;

                if (!Game1.options.SnappyMenus)
                    return menu;

                menu.populateClickableComponentList();
                menu.snapToDefaultClickableComponent();

                return menu;
            }

            /// <summary>
            /// Create a new instance of the <see cref="LetterViewerMenuEx2"/> class.
            /// </summary>
            /// <param name="id">The ID of the mail.</param>
            /// <param name="text">The text content of the mail.</param>
            /// <param name="recipe">The recipe attached to the mail.</param>
            /// <returns>The created <see cref="LetterViewerMenuEx2"/> instance.</returns>
            /// <exception cref="InvalidOperationException">If the specified recipe was not found.</exception>
            public static LetterViewerMenuEx2 CreateRecipeMailMenu(string id, string text, RecipeData recipe)
            {
                var menu = new LetterViewerMenuEx2(id, text)
                {
                    mailType = MailType.RecipeMail,
                };

                // If there is no recipe attached to the mail -> we are done.
                if (recipe == null)
                {
                    return menu;
                }

                menu.recipe = recipe;

                // Load the relevant recipe game asset to obtain the recipe's data.
                Dictionary<string, string> recipes = null;
                switch (recipe.Type)
                {
                    case RecipeType.Cooking:
                        // If the player already received the recipe -> don't attach the recipe to the mail.
                        if (Game1.player.cookingRecipes.ContainsKey(recipe.Name))
                        {
                            monitor.Log($"The player already learned the recipe \"{recipe.Name}\"!");
                            return menu;
                        }

                        recipes = Game1.content.Load<Dictionary<string, string>>("Data\\CookingRecipes");
                        break;
                    case RecipeType.Crafting:
                        // If the player already received the recipe -> don't attach the recipe to the mail.
                        if (Game1.player.craftingRecipes.ContainsKey(recipe.Name))
                        {
                            monitor.Log($"The player already learned the recipe \"{recipe.Name}\"!");
                            return menu;
                        }

                        recipes = Game1.content.Load<Dictionary<string, string>>("Data\\CraftingRecipes");
                        break;
                }

                // If the specified recipe is not available in the loaded recipe game asset, we throw an error
                if (!recipes.TryGetValue(recipe.Name, out string recipeData))
                {
                    monitor.Log($"A recipe with the name \"{recipe.Name}\" was not found!", LogLevel.Warn);
                    throw new InvalidOperationException($"Could not find a recipe with the specified name \"{recipe.Name}\"!");
                }

                // Add the recipe to the recipes the player already obtained.

                int translatedNameIndex;
                if (recipe.Type == RecipeType.Cooking)
                {
                    translatedNameIndex = 4; // See Data/CookingRecipes{.lg-LG}.xnb files
                    menu.CookingOrCrafting = Game1.content.LoadString("Strings\\UI:LearnedRecipe_cooking");
                    Game1.player.cookingRecipes.Add(recipe.Name, 0);
                }
                else
                {
                    translatedNameIndex = 5; // See Data/CraftingRecipes{.lg-LG}.xnb files
                    menu.CookingOrCrafting = Game1.content.LoadString("Strings\\UI:LearnedRecipe_crafting");
                    Game1.player.craftingRecipes.Add(recipe.Name, 0);
                }

                // Set the name of the recipe depending on the currently selected display language.
                string[] recipeParams = recipeData.Split('/');
                if (LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.en)
                {
                    if (recipeParams.Length < translatedNameIndex + 1)
                    {
                        menu.LearnedRecipe = recipe.Name;
                        monitor.Log($"There is no translated name for the recipe \"{recipe.Name}\" available! Using the recipe name as a fallback name.",
                            LogLevel.Warn);
                    }
                    else
                    {
                        // Read the translated name field from the recipe asset.
                        menu.LearnedRecipe = recipeParams[translatedNameIndex];
                    }
                }
                else
                {
                    // Language is English -> use the supplied recipe name.
                    menu.LearnedRecipe = recipe.Name;
                }

                return menu;
            }

            /// <summary>
            /// Create a new instance of the <see cref="LetterViewerMenuEx2"/> class.
            /// </summary>
            /// <param name="id">The ID of the mail.</param>
            /// <param name="text">The text content of the mail.</param>
            public LetterViewerMenuEx2(string id, string text)
                : base(PreParseTextContent(text))
            {
                reflectionHelper
                    .GetField<bool>(this, "isMail")
                    .SetValue(true);
                reflectionHelper
                    .GetField<string>(this, "mailTitle")
                    .SetValue(id);

                MailId = id;

                this.mailType = MailType.PlainMail;
            }

            /// <summary>
            /// The ID of the mail.
            /// </summary>
            public string MailId { get; private set; }

            /// <summary>
            /// Contains information about how the player interacted with the content of the mail.
            /// </summary>
            public MailInteractionRecord InteractionRecord { get; private set; }

            /// <summary>
            /// Add functionality to add a clicked item to the <see cref="selectedItems"/> list or 
            /// to accept a quest./>
            /// </summary>
            /// <param name="x">X-coordinate of the click.</param>
            /// <param name="y">Y-coordinate of the click.</param>
            /// <param name="playSound">Not used.</param>
            public override void receiveLeftClick(int x, int y, bool playSound = true)
            {
                // Handle [attached item] click
                foreach (ClickableComponent clickableComponent in this.itemsToGrab)
                {
                    if (clickableComponent.containsPoint(x, y) && clickableComponent.item != null)
                    {
                        // Add the clicked item to the list of user-selected items.
                        selectedItems.Add(clickableComponent.item);

                        Game1.playSound("coin");
                        clickableComponent.item = null;

                        return;
                    }
                }

                // Handle [quest accept button] click
                if (this.QuestId != QUEST_ID_NO_QUEST && this.acceptQuestButton.containsPoint(x, y))
                {
                    Game1.player.addQuest(this.QuestId);

                    this.questAccepted = true;
                    this.QuestId = QUEST_ID_NO_QUEST;

                    Game1.playSound("newArtifact");
                    return;
                }

                base.receiveLeftClick(x, y, playSound);
            }

            /// <summary>
            /// Preparses the mail's text content.
            /// </summary>
            /// <param name="text">The text content to parse.</param>
            /// <returns>The parsed text content.</returns>
            /// <remarks>
            /// In our creation of the <see cref="LetterViewerMenu"/> for the mail, we skip the game's mail content parsing.
            /// We are not interested in special commands such as "%item" but we are interested in commands/tokens which change
            /// the mail's text content. These tokens/commands are handled in this function.
            /// </remarks>
            private static string PreParseTextContent(string text)
            {
                if (text.Contains(SPECIAL_TOKEN_GENDER_SWITCH))
                {
                    text = Game1.player.IsMale
                        ? text.Substring(0, text.IndexOf(SPECIAL_TOKEN_GENDER_SWITCH))
                        : text.Substring(text.IndexOf(SPECIAL_TOKEN_GENDER_SWITCH) + 1);
                }

                text = text.Replace(SPECIAL_TOKEN_PLAYER_NAME, Game1.player.Name);

                Random r = new Random((int)(Game1.uniqueIDForThisGame / 2UL) ^ Game1.year ^ (int)Game1.player.UniqueMultiplayerID);
                text = text.Replace(SPECIAL_COMMAND_RANDOM_NPC_NAME, Utility.getRandomTownNPC(r).displayName);

                return text;
            }

            /// <summary>
            /// Called before the menu is exited. We use this function to setup any <see cref="MailInteractionRecord"/> 
            /// data we might need later.
            /// </summary>
            protected override void cleanupBeforeExit()
            {
                switch (mailType)
                {                      
                    case MailType.ItemMail:
                        // Grab all attached items which weren't selected by the player.
                        var unselectedItems = this.itemsToGrab.Where(component => component.item != null).Select(component => component.item).ToList();

                        InteractionRecord = new ItemMailInteractionRecord(this.selectedItems, unselectedItems);
                        break;
                    case MailType.MoneyMail:
                        InteractionRecord = new MoneyMailInteractionRecord(this.MoneyIncluded, this.currency);
                        break;
                    case MailType.RecipeMail:
                        InteractionRecord = new RecipeMailInteractionRecord(this.recipe);
                        break;
                    case MailType.QuestMail:
                        InteractionRecord = new QuestMailInteractionRecord(this.attachedQuestId, this.questAccepted);
                        break;
                    default:
                        InteractionRecord = new MailInteractionRecord();
                        break;
                }

                base.cleanupBeforeExit();
            }

            /// <summary>
            /// Draw the attached monetary value to the screen. This adds support for drawning different
            /// currencies.
            /// </summary>
            /// <param name="b">The sprite batch used to draw to the screen.</param>
            protected override void DrawMoney(SpriteBatch b)
            {
                string s = this.MoneyIncluded.ToString();

                int x = this.xPositionOnScreen + this.width / 2 - SpriteText.getWidthOfString(s, 999999) / 2;
                int y = this.yPositionOnScreen + this.height - 96;
                int trailingX = x + SpriteText.getWidthOfString(s, 999999);

                SpriteText.drawString(b, s, x, y, 999999, -1, 9999, 0.75f, 0.865f, false, -1, "", -1);

                Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2(trailingX + 4, y), new Rectangle(193 + (int)this.currency * 9, 373, 9, 10), Color.White, 0.0f, Vector2.Zero, 4f, false, 1f, -1, -1, 0.35f);
            }
        }
    }
}
