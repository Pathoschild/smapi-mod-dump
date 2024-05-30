/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KyuubiRan/TimeWatch
**
*************************************************/

using StardewModdingAPI;

namespace TimeWatch.Options;

internal class ModConfig
{
    public SButton IncreaseTimeKeyBind { get; set; } = SButton.Add;
    public SButton DecreaseTimeKeyBind { get; set; } = SButton.Subtract;
    public bool ShowTimeChangedNotify { get; set; } = true;
    public bool UpdateGameObjects { get; set; } = false;
    public bool MultiPlayHostOnly { get; set; } = false;
    public int DefaultSeekTimeValue { get; set; } = 6; // Default 1h
    public int HoldShiftSeekTimeValue { get; set; } = 1; // Default 10 min
    public int HoldCtrlSeekTimeValue { get; set; } = 3; // Default 30 min
    public int MaximumStorableTime { get; set; } = 18; // Default 18 hours
    public int DailyMaximumStorableTime { get; set; } = 18; // Default 3 hours
}