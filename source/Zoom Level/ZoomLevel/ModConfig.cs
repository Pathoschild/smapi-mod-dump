/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/thespbgamer/ZoomLevel
**
*************************************************/

using StardewModdingAPI.Utilities;

namespace ZoomLevel
{
    internal class ModConfig
    {
        public KeybindList KeybindListHoldToChangeUI { get; set; } = KeybindList.Parse("LeftShift, RightShift, LeftTrigger + RightTrigger");
        public KeybindList KeybindListIncreaseZoomOrUI { get; set; } = KeybindList.Parse("OemPeriod, RightStick");
        public KeybindList KeybindListDecreaseZoomOrUI { get; set; } = KeybindList.Parse("OemComma, LeftStick");
        public KeybindList KeybindListResetZoomOrUI { get; set; } = KeybindList.Parse("");
        public KeybindList KeybindListMaxZoomOrUI { get; set; } = KeybindList.Parse("");
        public KeybindList KeybindListMinZoomOrUI { get; set; } = KeybindList.Parse("");
        public KeybindList KeybindListToggleUI { get; set; } = KeybindList.Parse("");
        public KeybindList KeybindListToggleHideUIWithCertainZoom { get; set; } = KeybindList.Parse("");

        public float ZoomLevelIncreaseValue { get; set; } = 0.05f;
        public float ZoomLevelDecreaseValue { get; set; } = -0.05f;

        public float MaxZoomOutLevelAndUIValue { get; set; } = 0.35f;

        public float MaxZoomInLevelAndUIValue { get; set; } = 2.00f;

        public float ResetZoomOrUIValue { get; set; } = 1.00f;
        public float ZoomLevelThatHidesUI { get; set; } = 0.35f;

        public bool SuppressControllerButton { get; set; } = true;
        public bool ZoomAndUIControlEverywhere { get; set; } = false;
        public bool IsHideUIWithCertainZoom { get; set; } = false;

    }
}