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

namespace CustomCrystalariumMod.integrations
{
    public class ApiLetter : ILetter
    {
        /// <summary>
        /// this letter unique id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// text to be show on the letter menu. You can use @ to put the players name and ^ for line breaks. Use a translation key if you provide a ITranslationHelper
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// this letter group id. Letter with the same group id are never delivered in the same day.
        /// Letters registered first have priority, unless they have the suffix ".Random", in that case a random letter will be chosen.
        /// </summary>
        public string GroupId { get; set; }

        /// <summary>
        /// this letter title to be show in the collections menu. Use a translation key if you provide a ITranslationHelper
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// list of static items to be added in the letter. If null or empty, no item is added
        /// There is a bug with the game when adding more than one object. It draws one over the other, and when clicked the one showing is the last one to be picked.
        /// </summary>
        public List<Item> Items { get; set; }

        /// <summary>
        /// name of the recipe to be learned with the letter
        /// </summary>
        public string Recipe { get; set; }

        /// <summary>
        /// the id of the letter background. 0 = classic, 1 = notepad, 2 = pyramids
        /// </summary>
        public int WhichBG { get; set; }

        /// <summary>
        /// custom texture to replace the game default. must follow the same proportions and image structure
        /// </summary>
        public Texture2D LetterTexture { get; set; }

        /// <summary>
        /// number of the text color. -1 = Dark Red, 0 = Black, 1 = Sky Blue, 2 = Red, 3 = Blue Violet, 4 = White, 5 = Orange Red, 6 = Lime Green, 7 = Cyan, 8 = Darkest Gray
        /// </summary>
        public int? TextColor { get; set; }

        /// <summary>
        /// custom texture to replace the game default close button. must follow the same proportions
        /// </summary>
        public Texture2D UpperRightCloseButtonTexture { get; set; }

        /// <summary>
        /// when conditions are met any recipe will be learned and the callback will be called. the letter won't show in the mailbox
        /// </summary>
        public bool AutoOpen { get; set; }

        /// <summary>
        /// translation helper of the content pack or mod
        /// </summary>
        public ITranslationHelper I18N { get; set; }
    }
}
