/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

using SatoCore;
using SatoCore.Attributes;

namespace MasterFisher.Models
{
    /// <summary>Represents a category a fish can be in.</summary>
    public class FishCategory : ModelBase
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The name of the category.</summary>
        [Identifier]
        public string Name { get; set; }

        /// <summary>The maximum number of fish allowed in a pond in the category.</summary>
        [Editable]
        [DefaultValue(10)]
        public int? FishPondCap { get; set; }

        /// <summary>The path to the sprite to use in the fishing mini game when a fish in this category is hooked.</summary>
        [Editable]
        public string MinigameSprite { get; set; }
    }
}
