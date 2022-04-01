/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/stardew-access/stardew-access
**
*************************************************/

using StardewModdingAPI.Utilities;

namespace stardew_access
{
    internal class ModConfig
    {
        public Boolean VerboseCoordinates { get; set; } = true;
        public Boolean ReadTile { get; set; } = true;
        public Boolean SnapMouse { get; set; } = true;
        public Boolean Radar { get; set; } = false;
        public Boolean RadarStereoSound { get; set; } = true;
        public Boolean ReadFlooring { get; set; } = false;

        #region KeyBinds

        // https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Input#SButton button key codes
        public KeybindList LeftClickMainKey { get; set; } = KeybindList.Parse("LeftControl + Enter");
        public KeybindList RightClickMainKey { get; set; } = KeybindList.Parse("LeftShift + Enter");
        public KeybindList LeftClickAlternateKey { get; set; } = KeybindList.Parse("OemOpenBrackets");
        public KeybindList RightClickAlternateKey { get; set; } = KeybindList.Parse("OemCloseBrackets");
        public KeybindList HealthNStaminaKey { get; set; } = KeybindList.Parse("H");
        public KeybindList PositionKey { get; set; } = KeybindList.Parse("K");
        public KeybindList LocationKey { get; set; } = KeybindList.Parse("LeftAlt + K");
        public KeybindList MoneyKey { get; set; } = KeybindList.Parse("R");
        public KeybindList TimeNSeasonKey { get; set; } = KeybindList.Parse("Q");
        public KeybindList ReadTileKey { get; set; } = KeybindList.Parse("J");
        public KeybindList ReadStandingTileKey { get; set; } = KeybindList.Parse("LeftAlt + J");

        #endregion

        // TODO Add the exclusion and focus list too
        // public String ExclusionList { get; set; } = "test";
    }
}
