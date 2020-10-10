/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/hawkfalcon/Stardew-Mods
**
*************************************************/

using StardewModdingAPI;

namespace TillableGround {
    class ModConfig {
        public bool AllowTillingAnywhere { get; set; } = false;
        public SButton AllowTillingKeybind { get; set; } = SButton.H;
        public SButton PreventTillingKeybind { get; set; } = SButton.U;
    }
}
