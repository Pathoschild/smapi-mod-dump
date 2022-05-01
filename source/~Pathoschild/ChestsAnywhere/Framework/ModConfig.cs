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

using Pathoschild.Stardew.Common.Utilities;

namespace Pathoschild.Stardew.ChestsAnywhere.Framework
{
    /// <summary>The mod configuration.</summary>
    internal class ModConfig
    {
        /*********
        ** Accessors
        *********/
        /// <summary>Whether to show the chest name in a tooltip when you point at a chest.</summary>
        public bool ShowHoverTooltips { get; set; } = true;

        /// <summary>Whether to enable access to the shipping bin.</summary>
        public bool EnableShippingBin { get; set; } = true;

        /// <summary>Whether to add an 'organize' button in chest UIs for the player inventory.</summary>
        public bool AddOrganizePlayerInventoryButton { get; set; } = true;

        /// <summary>The range at which chests are accessible.</summary>
        public ChestRange Range { get; set; } = ChestRange.Unlimited;

        /// <summary>The key bindings.</summary>
        public ModConfigKeys Controls { get; set; } = new();

        /// <summary>The locations in which to disable remote chest lookups.</summary>
        public InvariantHashSet DisabledInLocations { get; set; } = new();
    }
}
