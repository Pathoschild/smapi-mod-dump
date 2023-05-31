/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using StardewModdingAPI.Utilities;

namespace BetterReturnScepter
{
    public class ModConfig
    {
        public bool CountWarpMenuAsScepterUsage = false;
        public bool EnableMultiObeliskSupport = false;
        public KeybindList OpenObeliskWarpMenuController = KeybindList.Parse("BigButton");
        public KeybindList OpenObeliskWarpMenuKbm = KeybindList.Parse("OemTilde");
        public KeybindList ReturnToLastPoint = KeybindList.Parse("LeftStick");
    }
}
