/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Zamiell/stardew-valley-mods
**
*************************************************/

using StardewModdingAPI.Utilities;

namespace Notifier
{
    public sealed class ModConfig
    {
        public bool ArtifactSpots { get; set; } = true;
        public bool SeedSpots { get; set; } = true;
        public bool Bubbles { get; set; } = true;
        public bool PanPoints { get; set; } = true;
        public KeybindList DebugHotkey { get; set; } = KeybindList.Parse("X");
    }
}
