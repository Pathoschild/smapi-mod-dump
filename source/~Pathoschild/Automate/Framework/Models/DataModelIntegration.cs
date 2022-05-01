/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

#nullable disable

namespace Pathoschild.Stardew.Automate.Framework.Models
{
    /// <summary>A mod which adds custom machine recipes and requires a separate automation component.</summary>
    internal class DataModelIntegration
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The unique ID of the main mod.</summary>
        public string Id { get; set; }

        /// <summary>The display name for the main mod.</summary>
        public string Name { get; set; }

        /// <summary>The unique ID of the suggested bridge mod.</summary>
        public string SuggestedId { get; set; }

        /// <summary>The display name for the suggested bridge mod.</summary>
        public string SuggestedName { get; set; }

        /// <summary>The mod page for the suggested bridge mod.</summary>
        public string SuggestedUrl { get; set; }
    }
}
