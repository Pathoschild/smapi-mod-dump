using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI;

namespace MailFrameworkMod
{
    public class MailController
    {
        public static readonly string CustomMailId = "MailFrameworkPlaceholderId";

        private static String _nextLetterId = "none";
        private static readonly List<Letter> Letters = new List<Letter>();
        private static Letter _shownLetter = null;
        private static IModEvents _events => MailFrameworkModEntry.ModHelper.Events;

        /// <summary>
        /// Call this method to update the mail box with new letters.
        /// </summary>
        public static void UpdateMailBox()
        {
            List<Letter> newLetters = MailDao.GetValidatedLetters();
            newLetters.RemoveAll((l)=>Letters.Contains(l));
            try
            {
                while (Game1.mailbox.Contains(null))
                {
                    Game1.mailbox.Remove(null);
                }
                if (newLetters.Count > 0) {
                    IList<string> mailbox = new List<string>(Game1.mailbox);
                    newLetters.ForEach((l) =>
                    {
                        mailbox.Insert(0, CustomMailId);
                        Letters.Add(l);
                    });
                    Game1.player.mailbox.Set(mailbox);
                }
            }
            finally
            {
                UpdateNextLetterId();
            }
        }


        /// <summary>
        /// Call this method to unload any new letters still on the mailbox.
        /// </summary>
        public static void UnloadMailBox()
        {
            List<String> tempMailBox = new List<string>();
            while (Game1.mailbox.Count > 0)
            {
                tempMailBox.Add(Game1.mailbox.First());
                Game1.mailbox.RemoveAt(0);
            }
            foreach (Letter letter in Letters)
            {
                tempMailBox.Remove(CustomMailId);
            }
            foreach (String mail in tempMailBox)
            {
                Game1.mailbox.Add(mail);
            }
            
            Letters.Clear();
        }

        /// <summary>
        /// If exists any custom mail to be delivered.
        /// </summary>
        /// <returns></returns>
        public static bool HasCustomMail()
        {
            return Letters.Count > 0;
        }

        /// <summary>
        /// Shows any custom letter waiting in the mail box.
        /// Don't do anything if there is already a letter being shown.
        /// </summary>
        public static void ShowLetter()
        {
            if (_shownLetter == null)
            {
                if (Letters.Count > 0 && _nextLetterId == CustomMailId)
                {
                    _shownLetter = Letters.First();
                    var activeClickableMenu = new LetterViewerMenuExtended(_shownLetter.Text.Replace("@", Game1.player.Name),_shownLetter.Id);
                    MailFrameworkModEntry.ModHelper.Reflection.GetField<int>(activeClickableMenu,"whichBG").SetValue(_shownLetter.WhichBG);
                    if (_shownLetter.LetterTexture != null)
                    {
                        MailFrameworkModEntry.ModHelper.Reflection.GetField<Texture2D>(activeClickableMenu, "letterTexture").SetValue(_shownLetter.LetterTexture);
                    }
                    activeClickableMenu.TextColor = _shownLetter.TextColor;
                    
                    Game1.activeClickableMenu = activeClickableMenu;
                    _shownLetter.Items?.ForEach(
                        (i) =>
                        {
                            var item = i.getOne();
                            item.Stack = i.Stack;
                            activeClickableMenu.itemsToGrab.Add
                            (
                                new ClickableComponent
                                (
                                    new Rectangle
                                    (
                                        activeClickableMenu.xPositionOnScreen + activeClickableMenu.width / 2 -
                                        12 * Game1.pixelZoom
                                        , activeClickableMenu.yPositionOnScreen + activeClickableMenu.height -
                                          Game1.tileSize / 2 - 24 * Game1.pixelZoom
                                        , 24 * Game1.pixelZoom
                                        , 24 * Game1.pixelZoom
                                    )
                                    , item
                                )
                                {
                                    myID = 104,
                                    leftNeighborID = 101,
                                    rightNeighborID = 102
                                }
                            );
                            activeClickableMenu.backButton.rightNeighborID = 104;
                            activeClickableMenu.forwardButton.leftNeighborID = 104;
                            activeClickableMenu.populateClickableComponentList();
                            activeClickableMenu.snapToDefaultClickableComponent();
                        });
                    if (_shownLetter.Recipe != null)
                    {
                        string recipe = _shownLetter.Recipe;
                        Dictionary<string, string> cookingData = MailFrameworkModEntry.ModHelper.Content.Load<Dictionary<string, string>>("Data\\CookingRecipes", ContentSource.GameContent);
                        Dictionary<string, string> craftingData = MailFrameworkModEntry.ModHelper.Content.Load<Dictionary<string, string>>("Data\\CraftingRecipes", ContentSource.GameContent);
                        string recipeString = null;
                        int dataArrayI18NSize = 0;
                        string cookingOrCraftingText = null;
                        if (cookingData.ContainsKey(recipe))
                        {
                            if (!Game1.player.cookingRecipes.ContainsKey(recipe))
                            {
                                Game1.player.cookingRecipes.Add(recipe, 0);
                            }
                            recipeString = cookingData[recipe];
                            dataArrayI18NSize = 5;
                            cookingOrCraftingText = Game1.content.LoadString("Strings\\UI:LearnedRecipe_cooking");
                        }
                        else if (craftingData.ContainsKey(recipe))
                        {
                            if (!Game1.player.craftingRecipes.ContainsKey(recipe))
                            {
                                Game1.player.craftingRecipes.Add(recipe, 0);
                            }
                            recipeString = craftingData[recipe];
                            dataArrayI18NSize = 6;
                            cookingOrCraftingText = Game1.content.LoadString("Strings\\UI:LearnedRecipe_crafting");
                        }
                        else
                        {
                            MailFrameworkModEntry.ModMonitor.Log($"The recipe '{recipe}' was not found. The mail will ignore it.", LogLevel.Warn);
                        }

                        if (recipeString != null)
                        {
                            string learnedRecipe = recipe;
                            if (LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.en)
                            {
                                string[] strArray = recipeString.Split('/');
                                if (strArray.Length < dataArrayI18NSize)
                                {
                                    MailFrameworkModEntry.ModMonitor.Log($"The recipe '{recipe}' does not have a internationalized name. The default name will be used.", LogLevel.Warn);
                                }
                                else
                                {
                                    learnedRecipe = strArray[strArray.Length - 1];
                                }
                            }

                            if (MailFrameworkModEntry.ModHelper.Reflection.GetMethod(activeClickableMenu, "getTextColor").Invoke<int>() == -1)
                            {
                                MailFrameworkModEntry.ModHelper.Reflection
                                    .GetField<String>(activeClickableMenu, "cookingOrCrafting").SetValue(cookingOrCraftingText);
                                MailFrameworkModEntry.ModHelper.Reflection
                                    .GetField<String>(activeClickableMenu, "learnedRecipe").SetValue(learnedRecipe);
                            }
                            else
                            {
                                activeClickableMenu.CookingOrCrafting = cookingOrCraftingText;
                                activeClickableMenu.LearnedRecipe = learnedRecipe;
                            }
                        }
                    }

                    _events.Display.MenuChanged += OnMenuChanged;
                }
                else
                {
                    UpdateNextLetterId();
                }
            }
        }

        /// <summary>
        /// Raised after a game menu is opened, closed, or replaced.
        /// Remove the showed letter from the list and calls the callback function.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private static void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu != null)
                return;

            if (_shownLetter != null)
            { 
                Letters.Remove(_shownLetter);
                _shownLetter.Callback?.Invoke(_shownLetter);
                _shownLetter = null;
            }
            UpdateNextLetterId();
            _events.Display.MenuChanged -= OnMenuChanged;
        }

        /// <summary>
        /// Sees if there is a new letter on the mailbox and saves its id on the class.
        /// </summary>
        private static void UpdateNextLetterId()
        {
            if (Game1.mailbox.Count > 0)
            {
                _nextLetterId = Game1.mailbox.First();
            }
            else
            {
                _nextLetterId = "none";
            }
        }

        public static bool mailbox(GameLocation __instance)
        {
            if (Game1.mailbox.Count > 0 && Game1.player.ActiveObject == null)
            {
                if (Game1.mailbox.First<string>() == null)
                {
                    Game1.mailbox.RemoveAt(0);
                    return false;
                } else if (Game1.mailbox.First<string>().Contains(CustomMailId))
                {
                    Game1.mailbox.RemoveAt(0);
                    MailController.ShowLetter();
                    return false;
                }
                else
                {
                    _events.Display.MenuChanged += OnMenuChanged;
                }
            }
            return true;
        }
    }
}
