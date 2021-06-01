/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LunaticShade/StardewValley.SkillfulClothes
**
*************************************************/

using SkillfulClothes.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillfulClothes.Types
{
    /// <summary>
    /// Holds information on patches which should be applied to items
    /// Comes with a fluent configuration
    /// </summary>
    public class ExtItemInfo
    {
        public bool ShouldDescriptionBePatched => !String.IsNullOrEmpty(NewItemDescription);
        
        /// <summary>
        /// Replaces the item's original description with the given text
        /// </summary>
        public string NewItemDescription { get; }

        /// <summary>
        /// If true, the player can no longer craft the clothing item on a sewing machine
        /// </summary>
        public bool IsCraftingDisabled { get; }

        public Shop SoldBy { get; }

        /// <summary>
        /// The price the shop sells this item for
        /// </summary>
        public int Price { get; }

        /// <summary>
        /// The condition(s) under which the item gets listed at the shop
        /// </summary>
        public SellingCondition SellingCondition { get; }

        /// <summary>
        /// The items effect
        /// </summary>
        public IEffect Effect { get; private set; }

        internal ExtItemInfo(string newDescription, bool disableCrafting, IEffect effect, Shop soldBy, int price, SellingCondition sellingCondition)
        {
            NewItemDescription = newDescription;
            IsCraftingDisabled = disableCrafting;
            Effect = effect;
            SoldBy = soldBy;
            Price = price;
            SellingCondition = sellingCondition;
        }
    }

    /// <summary>
    /// Fluent builder for ExtendedItemInfo
    /// </summary>
    public class ExtendItem
    {
        bool cannotCraft = false;
        string newItemDescription;
        IEffect effect;

        Shop soldBy = Shop.None;
        int price = 0;
        SellingCondition sellingCondition;

        public static ExtendItem With => new ExtendItem();

        public ExtendItem And => this;

        public ExtendItem Description(string newDescription)
        {
            newItemDescription = newDescription;
            return this;
        }

        public ExtendItem Effect(params IEffect[] effects)
        {
            if (effects.Length == 1)
            {
                this.effect = effects[0];
            } else if (effects.Length > 1)
            {
                this.effect = EffectSet.Of(effects);
            } else
            {
                this.effect = null;
            }

            return this;
        }

        public ExtendItem SoldBy(Shop shop, int price, SellingCondition sellingCondition = SellingCondition.None)
        {
            soldBy = shop;
            this.price = price;
            this.sellingCondition = sellingCondition;
            return this;
        }

        public ExtendItem CannotCraft
        {
            get
            {
                cannotCraft = true;
                return this;
            }
        }

        public static implicit operator ExtItemInfo(ExtendItem item) => new ExtItemInfo(item.newItemDescription, item.cannotCraft, item.effect, item.soldBy, item.price, item.sellingCondition);        
    }
}
