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
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace MailFrameworkMod
{
    /// <summary>
    /// Represents a custom letter that needs to be delivered.
    /// </summary>
    public class Letter
    {
        private string _id;
        /// <summary>
        /// this letter unique id
        /// </summary>
        public string Id { get => _id; private set => _id = value.Replace(" ",""); }
        /// <summary>
        /// this letter group id. Letter with the same group id are never delivered in the same day.
        /// Letters registered first have priority, unless they have the suffix ".Random", in that case a random letter will be chosen.
        /// </summary>
        public string GroupId;
        /// <summary>
        /// this letter title to be show in the collections menu. Use a translation key if you provide a ITranslationHelper
        /// </summary>
        public string Title;
        /// <summary>
        /// translated title
        /// </summary>
        public string TranslatedTitle => I18N != null ? I18N.Get(Title) : Title;
        /// <summary>
        /// text to be show on the letter menu. You can use @ to put the players name and ^ for line breaks. Use a translation key if you provide a ITranslationHelper
        /// </summary>
        public string Text { get; private set; }
        /// <summary>
        /// translated text
        /// </summary>
        public string TranslatedText => I18N != null ? I18N.Get(Text) : Text;
        /// <summary>
        /// list of static items to be added in the letter. If null or empty, no item is added
        /// There is a bug with the game when adding more than one object. It draws one over the other, and when clicked the one showing is the last one to be picked.
        /// </summary>
        public List<Item> Items { get; private set; }
        /// <summary>
        /// a function that will be called to get items to be added to the letter. If null, or empty, no items is added
        /// </summary>
        public Func<Letter,List<Item>> DynamicItems { get; set; }
        /// <summary>
        /// name of the recipe to be learned with the letter
        /// </summary>
        public string Recipe { get; private set;}
        /// <summary>
        /// a function that will be called to check if the letter is ready to be placed in the mailbox
        /// </summary>
        public Func<Letter,bool> Condition { get; private set; }
        /// <summary>
        /// a action that will be called after the letter was seen by the player
        /// </summary>
        public Action<Letter> Callback { get; set; }
        /// <summary>
        /// the id of the letter background. 0 = classic, 1 = notepad, 2 = pyramids
        /// </summary>
        public int WhichBG;
        /// <summary>
        /// custom texture to replace the game default. must follow the same proportions and image structure
        /// </summary>
        public Texture2D LetterTexture;
        /// <summary>
        /// number of the text color. -1 = Dark Red, 0 = Black, 1 = Sky Blue, 2 = Red, 3 = Blue Violet, 4 = White, 5 = Orange Red, 6 = Lime Green, 7 = Cyan, 8 = Darkest Gray
        /// </summary>
        public int? TextColor;
        /// <summary>
        /// custom texture to replace the game default close button. must follow the same proportions
        /// </summary>
        public Texture2D UpperRightCloseButtonTexture;
        /// <summary>
        /// when conditions are met any recipe will be learned and the callback will be called. the letter won't show in the mailbox
        /// </summary>
        public bool AutoOpen;
        /// <summary>
        /// translation helper of the content pack or mod
        /// </summary>
        public ITranslationHelper I18N;

        /// <summary>
        /// Creates a letter.
        /// </summary>
        /// <param name="id">this letter unique id</param>
        /// <param name="text">text to be show on the letter menu. You can use @ to put the players name and ^ for line breakes</param>
        /// <param name="condition">a function that will be called to check if the letter is ready to be placed in the mailbox</param>
        /// <param name="callback">a action that will be called after the letter was seen by the player</param>
        /// <param name="whichBG">the id of the letter background. 0 = classic, 1 = notepad, 2 = piramds</param>
        public Letter(string id, string text, Func<Letter, bool> condition, Action<Letter> callback = null, int whichBG = 0) : this(id, text, null, null, condition, callback, whichBG) { }

        /// <summary>
        /// Creates a letter.
        /// </summary>
        /// <param name="id">this letter unique id</param>
        /// <param name="text">text to be show on the letter menu. You can use @ to put the players name and ^ for line breakes</param>
        /// <param name="items">list of items to be added in the letter. If null or empty, no item is added</param>
        /// <param name="condition">a function that will be called to check if the letter is ready to be placed in the mailbox</param>
        /// <param name="callback">a action that will be called after the letter was seen by the player</param>
        /// <param name="whichBG">the id of the letter background. 0 = classic, 1 = notepad, 2 = piramds</param>
        public Letter(string id, string text, List<Item> items, Func<Letter,bool> condition, Action<Letter> callback = null, int whichBG = 0) : this(id, text, items, null, condition, callback, whichBG) { }

        /// <summary>
        /// Creates a letter.
        /// </summary>
        /// <param name="id">this letter unique id</param>
        /// <param name="text">text to be show on the letter menu. You can use @ to put the players name and ^ for line breakes</param>
        /// <param name="recipe">name of the recipe to be learned with the letter</param>
        /// <param name="condition">a function that will be called to check if the letter is ready to be placed in the mailbox</param>
        /// <param name="callback">a action that will be called after the letter was seen by the player</param>
        /// <param name="whichBG">the id of the letter background. 0 = classic, 1 = notepad, 2 = piramds</param>
        public Letter(string id, string text, string recipe, Func<Letter,bool> condition, Action<Letter> callback = null, int whichBG = 0) : this(id, text, null, recipe, condition, callback, whichBG) { }

        private Letter(string id, string text, List<Item> items, string recipe, Func<Letter,bool> condition, Action<Letter> callback, int whichBG)
        {
            Id = id;
            Text = text;
            Items = items;
            Recipe = recipe;
            Condition = condition;
            Callback = callback;
            WhichBG = whichBG;
        }

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Title)}: {TranslatedTitle}, { nameof(Text)}: {TranslatedText}";
        }

        public string ToMailDataString()
        {
            return TranslatedText + (TranslatedTitle != null ? $"[#]{TranslatedTitle}" : "");
        }
    }
}
