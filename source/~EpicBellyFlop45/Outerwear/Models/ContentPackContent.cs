/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/EpicBellyFlop45/StardewMods
**
*************************************************/

namespace Outerwear.Models
{
    /// <summary>Represents a content.json file in a content pack.</summary>
    public class ContentPackContent
    {
        /// <summary>The display name of the outerwear.</summary>
        public string DisplayName { get; set; }

        /// <summary>The description of the outerwear.</summary>
        public string Description { get; set; }

        /// <summary>Where you can buy the outerwear (NPC name).</summary>
        public string BuyFrom { get; set; } = null;

        /// <summary>The amount the outwear costs to buy.</summary>
        public int BuyPrice { get; set; } = -1;
    }
}
