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
    public sealed class ModConfig
    {
        public KeybindList KeybindListHoldToChangeUI { get; set; } = KeybindList.Parse("LeftShift, RightShift, LeftTrigger + RightTrigger");
        public KeybindList KeybindListIncreaseZoomOrUI { get; set; } = KeybindList.Parse("OemPeriod, RightStick");
        public KeybindList KeybindListDecreaseZoomOrUI { get; set; } = KeybindList.Parse("OemComma, LeftStick");
        public KeybindList KeybindListResetZoomOrUI { get; set; } = KeybindList.Parse("");
        public KeybindList KeybindListMaxZoomOrUI { get; set; } = KeybindList.Parse("");
        public KeybindList KeybindListMinZoomOrUI { get; set; } = KeybindList.Parse("");
        public KeybindList KeybindListZoomToCurrentMapSize { get; set; } = KeybindList.Parse("");
        public KeybindList KeybindListPresetZoomAndUIValues { get; set; } = KeybindList.Parse("");

        public KeybindList KeybindListMovementCameraUp { get; set; } = KeybindList.Parse("");
        public KeybindList KeybindListMovementCameraDown { get; set; } = KeybindList.Parse("");
        public KeybindList KeybindListMovementCameraLeft { get; set; } = KeybindList.Parse("");
        public KeybindList KeybindListMovementCameraRight { get; set; } = KeybindList.Parse("");
        public KeybindList KeybindListMovementCameraReset { get; set; } = KeybindList.Parse("");

        public KeybindList KeybindListToggleUIVisibility { get; set; } = KeybindList.Parse("");
        public KeybindList KeybindListToggleHideUIWithCertainZoom { get; set; } = KeybindList.Parse("");
        public KeybindList KeybindListToggleAnyKeyToResetCamera { get; set; } = KeybindList.Parse("");
        public KeybindList KeybindListToggleAutoZoomToCurrentMapSize { get; set; } = KeybindList.Parse("");
        public KeybindList KeybindListTogglePresetOnLoadSaveFile { get; set; } = KeybindList.Parse("");

        public float ZoomOrUILevelIncreaseValue { get; set; } = 0.05f;
        public float ZoomOrUILevelDecreaseValue { get; set; } = -0.05f;
        public float ResetZoomOrUIValue { get; set; } = 1.00f;
        public float MaxZoomOrUIValue { get; set; } = 2.00f;
        public float MinZoomOrUIValue { get; set; } = 0.35f;
        public float ZoomLevelThatHidesUI { get; set; } = 0.35f;
        public int CameraMovementSpeedValue { get; set; } = 15;
        public float PresetZoomLevelValue { get; set; } = 0.86f;
        public float PresetUIScaleValue { get; set; } = 0.75f;

        public bool SuppressControllerButtons { get; set; } = true;
        public bool AutoZoomToCurrentMapSize { get; set; } = false;
        public bool AnyButtonToCenterCamera { get; set; } = true;
        public bool HideUIWithCertainZoom { get; set; } = false;
        public bool PresetOnLoadSaveFile { get; set; } = false;
        public bool ZoomAndUIControlEverywhere { get; set; } = false;
    }
}