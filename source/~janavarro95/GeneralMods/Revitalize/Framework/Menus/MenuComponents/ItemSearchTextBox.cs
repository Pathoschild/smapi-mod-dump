/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Omegasis.Revitalize.Framework.Utilities.JsonContentLoading;
using Omegasis.Revitalize.Framework.World.Objects;
using Omegasis.Revitalize.Framework.World.Objects.Interfaces;
using StardewValley;
using StardewValley.Menus;

namespace Omegasis.Revitalize.Framework.Menus.MenuComponents
{
    /// <summary>
    /// A search text box that is specially tailored towards searching out which items are valid or not.
    /// </summary>
    public class ItemSearchTextBox : SearchTextBox
    {
        public enum ItemSearchMode
        {
            DisplayName,
            Id,
            Category,
            Description,
            /// <summary>
            /// Used to match any singular field from above to do a mass text search.
            /// </summary>
            Any
        }


        public ItemSearchMode currentSearchMode = ItemSearchMode.DisplayName;

        public ItemSearchTextBox(Texture2D textBoxTexture, Texture2D caretTexture, SpriteFont font, Color textColor, Rectangle bounds,ItemSearchMode CurrentSearchMode = ItemSearchMode.DisplayName) : base(textBoxTexture, caretTexture, font, textColor,bounds)
        {
            this.currentSearchMode = CurrentSearchMode;
        }

        /// <summary>
        /// Cycles the search mode through the various modes.
        /// </summary>
        public virtual void cycleSearchMode()
        {
            if (this.currentSearchMode == ItemSearchMode.Any)
            {
                this.currentSearchMode = ItemSearchMode.DisplayName;
                return;
            }
            this.currentSearchMode=(ItemSearchMode)((int)this.currentSearchMode)+1;
        }

        /// <summary>
        /// Gets the current display string for the current search mode.
        /// </summary>
        /// <returns></returns>
        public virtual string getCurrentSearchModeDisplayString()
        {
            Dictionary<string, string> dict = JsonContentPackUtilities.LoadStringDictionaryFile(Path.Combine(Revitalize.Framework.Constants.PathConstants.StringsPaths.MenuComponents, "ItemSearchTextBox.json"));
            return string.Format(dict["CurrentSearchMode"],dict[Enum.GetName(this.currentSearchMode)]);
        }

        /// <summary>
        /// Gets the valid items that are filtered through this search text box.
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public virtual List<Item> getValidItems(IList<Item> items)
        {
            List<Item> validItems = new List<Item>();
            if (string.IsNullOrEmpty(this.Text)) return items.ToList();
            foreach (Item item in items)
            {
                if (this.currentSearchMode != ItemSearchMode.Any)
                {
                    if (this.getComparingString(item).ToLowerInvariant().Contains(this.Text.ToLowerInvariant()))
                        validItems.Add(item);
                }
                else
                {
                    List<string> massSearchStrings = new List<string>();

                    foreach (ItemSearchMode mode in Enum.GetValues<ItemSearchMode>())
                    {
                        if (mode == ItemSearchMode.Any) continue;
                        if (this.getComparingString(item, mode).ToLowerInvariant().Contains(this.Text.ToLowerInvariant()))
                        {
                            validItems.Add(item);
                            break;
                        }
                    }
                }
            }
            return validItems;
        }

        /// <summary>
        /// Gets which string from the item to compare to the text field based on
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual string getComparingString(Item item)
        {
            return this.getComparingString(item, this.currentSearchMode);
        }

        public virtual string getComparingString(Item item, ItemSearchMode itemSearchMode)
        {
            string searchString = "";
            if (itemSearchMode == ItemSearchMode.DisplayName)
            {
                searchString = item.DisplayName;
            }
            else if (itemSearchMode == ItemSearchMode.Id)
            {
                if (item is IBasicItemInfoProvider)
                {
                    return ((item as IBasicItemInfoProvider).Id);
                }
                else
                {
                    //This should be the fully qualified item id in SDV 1.6
                    searchString = item.ParentSheetIndex.ToString();
                }
            }
            else if (itemSearchMode == ItemSearchMode.Category)
            {
                searchString = item.getCategoryName();
            }
            else if (itemSearchMode == ItemSearchMode.Description)
            {
                searchString = item.getDescription();
            }
            return searchString;
        }
    }
}
