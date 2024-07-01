/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/focustense/StardewRadialMenu
**
*************************************************/

namespace RadialMenu.Config;

internal interface ICursorConfiguration
{
    public float TriggerDeadZone { get; }
    public bool SwapTriggers { get; }
    public ThumbStickPreference ThumbStickPreference { get; }
    public float ThumbStickDeadZone { get;}
}
