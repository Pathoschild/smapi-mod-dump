/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewMods
**
*************************************************/

using StardewModdingAPI;

namespace PlayerCoordinates
{
    public class ModConfig
    {
        public SButton CoordinateHUDToggle  { get; } = SButton.F8;
        public SButton LogCoordinates { get; set; } = SButton.F9;
        public SButton SwitchToCursorCoords { get; set; } = SButton.F10;
    }
}