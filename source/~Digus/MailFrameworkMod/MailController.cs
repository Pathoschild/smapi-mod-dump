/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace MailFrameworkMod
{
    public class MailController
    {
        public static readonly string CustomMailId = "MailFrameworkPlaceholderId";

        private static readonly PerScreen<string> NextLetterId = new PerScreen<string>(createNewState: () => "none");
        private static readonly PerScreen<List<Letter>> Letters = new PerScreen<List<Letter>>(createNewState: () => new List<Letter>());
        private static readonly PerScreen<Letter> ShownLetter = new PerScreen<Letter>();
        private static IModEvents _events => MailFrameworkModEntry.ModHelper.Events;

        /// <summary>
        /// Call this method to update the mail box with new letters.
        /// </summary>
        public static void UpdateMailBox()
        {
            List<Letter> newLetters = MailDao.GetValidatedLetters();
            newLetters.RemoveAll((l)=>Letters.Value.Contains(l));
            try
            {
                while (Game1.player.mailbox.Contains(null))
                {
                    Game1.player.mailbox.Remove(null);
                }
                while (Game1.player.mailbox.Contains(CustomMailId))
                {
                    Game1.player.mailbox.Remove(CustomMailId);
                }
                IList<string> mailbox = new List<string>(Game1.player.mailbox);
                newLetters.ForEach((l) =>
                {
                    Letters.Value.Add(l);
                });
                Letters.Value.ForEach((l) =>
                {
                    mailbox.Insert(0, CustomMailId);
                });
                Game1.player.mailbox.Set(mailbox);
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
            List<string> tempMailBox = new List<string>();
            while (Game1.player.mailbox.Count > 0)
            {
                tempMailBox.Add(Game1.player.mailbox.First());
                Game1.player.mailbox.RemoveAt(0);
            }
            foreach (Letter letter in Letters.Value)
            {
                tempMailBox.Remove(CustomMailId);
            }
            foreach (string mail in tempMailBox)
            {
                Game1.player.mailbox.Add(mail);
            }
            
            Letters.Value.Clear();
        }

        /// <summary>
        /// Call this method to unload letters with the specified id from the mailbox. 
        /// </summary>
        /// <param name="id">the letter id</param>
        public static void UnloadLetterMailbox(string id)
        {
            if (Letters.Value.Any(l => l.Id == id))
            {
                List<string> tempMailBox = new List<string>();
                while (Game1.player.mailbox.Count > 0)
                {
                    tempMailBox.Add(Game1.player.mailbox.First());
                    Game1.player.mailbox.RemoveAt(0);
                }
                foreach (var l in Letters.Value.Where(l => l.Id == id))
                {
                    tempMailBox.Remove(CustomMailId);
                }
                foreach (string mail in tempMailBox)
                {
                    Game1.player.mailbox.Add(mail);
                }
                Letters.Value.RemoveAll(l => l.Id == id);
            }
        }

        /// <summary>
        /// If exists any custom mail to be delivered.
        /// </summary>
        /// <returns></returns>
        public static bool HasCustomMail()
        {
            return Letters.Value.Count > 0;
        }

        /// <summary>
        /// Shows any custom letter waiting in the mail box.
        /// Don't do anything if there is already a letter being shown.
        /// </summary>
        public static void ShowLetter()
        {
            if (ShownLetter.Value != null)
            {
                IList<string> mailbox = new List<string>(Game1.player.mailbox);
                mailbox.Insert(0, CustomMailId);
                Game1.player.mailbox.Set(mailbox);
            }
            else
            {
                if (Letters.Value.Count > 0 && NextLetterId.Value == CustomMailId)
                {
                    ShownLetter.Value = Letters.Value.First();
                }
            }

            if (ShownLetter.Value != null)
            {
                var activeClickableMenu = new LetterViewerMenuExtended(ShownLetter.Value.Text.Replace("@", Game1.player.Name),ShownLetter.Value.Id);
                MailFrameworkModEntry.ModHelper.Reflection.GetField<int>(activeClickableMenu,"whichBG").SetValue(ShownLetter.Value.WhichBG);
                if (ShownLetter.Value.LetterTexture != null)
                {
                    activeClickableMenu.letterTexture = ShownLetter.Value.LetterTexture;
                }
                activeClickableMenu.TextColor = ShownLetter.Value.TextColor;
                if (ShownLetter.Value.UpperRightCloseButtonTexture != null &&
                    activeClickableMenu.upperRightCloseButton != null)
                {
                    activeClickableMenu.upperRightCloseButton = new ClickableTextureComponent(new Rectangle(activeClickableMenu.xPositionOnScreen + activeClickableMenu.width - 36, activeClickableMenu.yPositionOnScreen - 8, 48, 48), ShownLetter.Value.UpperRightCloseButtonTexture, new Rectangle(0, 0, 12, 12), 4f, false);
                }

                Game1.activeClickableMenu = activeClickableMenu;
                List<Item> attachments = new List<Item>();
                if (ShownLetter.Value.Items != null)
                {
                    attachments.AddRange(ShownLetter.Value.Items);
                }

                List<Item> dynamicItems = ShownLetter.Value.DynamicItems?.Invoke(ShownLetter.Value);
                if (dynamicItems != null)
                {
                    attachments.AddRange(dynamicItems);
                }
                attachments.ForEach(
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
                if (ShownLetter.Value.Recipe != null)
                {
                    string recipe = ShownLetter.Value.Recipe;
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
                            MailFrameworkModEntry.ModHelper.Reflection.GetField<String>(activeClickableMenu, "cookingOrCrafting").SetValue(cookingOrCraftingText);
                            MailFrameworkModEntry.ModHelper.Reflection.GetField<String>(activeClickableMenu, "learnedRecipe").SetValue(learnedRecipe);
                        }
                        else
                        {
                            activeClickableMenu.CookingOrCrafting = cookingOrCraftingText;
                            activeClickableMenu.LearnedRecipe = learnedRecipe;
                        }
                    }
                }
                activeClickableMenu.exitFunction = (IClickableMenu.onExit)Delegate.Combine(activeClickableMenu.exitFunction, (IClickableMenu.onExit)delegate
                {
                    OnMenuClose(ShownLetter.Value);
                });
            }
            else
            {
                UpdateNextLetterId();
            }
        }

        /// <summary>
        /// Remove the showed letter from the list and calls the callback function.
        /// </summary>
        private static void OnMenuClose(Letter letter)
        {
            if (letter != null)
            {
                Letters.Value.Remove(letter);
                letter.Callback?.Invoke(letter);
                ShownLetter.Value = null;
            }
            UpdateNextLetterId();
        }

        /// <summary>
        /// Sees if there is a new letter on the mailbox and saves its id on the class.
        /// </summary>
        private static void UpdateNextLetterId()
        {
            if (Game1.player.mailbox.Count > 0)
            {
                NextLetterId.Value = Game1.player.mailbox.First();
            }
            else
            {
                NextLetterId.Value = "none";
            }
        }

        public static bool mailbox_prefix(GameLocation __instance)
        {
            if (Game1.player.mailbox.Count > 0)
            {
                if (Game1.player.mailbox.First<string>() == null)
                {
                    Game1.player.mailbox.RemoveAt(0);
                    return false;
                } else if (Game1.player.mailbox.First<string>().Contains(CustomMailId))
                {
                    Game1.player.mailbox.RemoveAt(0);
                    MailController.ShowLetter();
                    return false;
                }
            } 
            return true;
        }

        public static void mailbox_postfix()
        {
            if (Game1.activeClickableMenu is LetterViewerMenu letterViewerMenu)
            {
                letterViewerMenu.exitFunction = (IClickableMenu.onExit)Delegate.Combine(letterViewerMenu.exitFunction, (IClickableMenu.onExit)delegate
                {
                    OnMenuClose(null);
                });
            }
        }

        public static bool receiveLeftClick(CollectionsPage __instance, int x, int y)
        {
            int currentTab = MailFrameworkModEntry.ModHelper.Reflection.GetField<int>(__instance, "currentTab").GetValue();

            if (__instance.letterviewerSubMenu == null && currentTab == 7)
            {
                int currentPage = MailFrameworkModEntry.ModHelper.Reflection.GetField<int>(__instance, "currentPage").GetValue();
                foreach (ClickableTextureComponent clickableComponent in __instance.collections[currentTab][currentPage])
                {
                    if (clickableComponent.containsPoint(x, y))
                    {
                        Letter letter = MailDao.FindLetter(clickableComponent.name.Split(' ')[0]);
                        if (letter != null)
                        {
                            LetterViewerMenuExtended letterViewerMenu = new LetterViewerMenuExtended(letter.Text.Replace("@", Game1.player.Name), letter.Id, true);
                            MailFrameworkModEntry.ModHelper.Reflection.GetField<int>(letterViewerMenu, "whichBG").SetValue(letter.WhichBG);
                            if (letter.LetterTexture != null)
                            {
                                letterViewerMenu.letterTexture = letter.LetterTexture;
                            }
                            letterViewerMenu.TextColor = letter.TextColor;
                            __instance.letterviewerSubMenu = letterViewerMenu;
                            return false;
                        }
                    }
                }
            }
            return true;
        }
    }
}
