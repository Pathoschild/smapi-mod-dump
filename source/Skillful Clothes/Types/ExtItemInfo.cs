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
        public string NewItemDescription { get; private set; }

        /// <summary>
        /// If true, the player can no longer craft the clothing item on a sewing machine
        /// </summary>
        public bool IsCraftingDisabled { get; private set; }

        /// <summary>
        /// The items effect
        /// </summary>
        public IEffect Effect { get; private set; }

        internal ExtItemInfo(string newDescription, bool disableCrafting, IEffect effect)
        {
            NewItemDescription = newDescription;
            IsCraftingDisabled = disableCrafting;
            Effect = effect;
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

        public ExtendItem CannotCraft
        {
            get
            {
                cannotCraft = false;
                return this;
            }
        }

        public static implicit operator ExtItemInfo(ExtendItem item) => new ExtItemInfo(item.newItemDescription, item.cannotCraft, item.effect);        
    }
}
