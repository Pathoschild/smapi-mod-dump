/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/stardew-access/stardew-access
**
*************************************************/

using StardewModdingAPI.Events;
using StardewValley;

namespace stardew_access.Features;

public abstract class FeatureBase
{
    public static FeatureBase Instance => throw new Exception("Override Instance property!!");

    public abstract void Update(object? sender, UpdateTickedEventArgs e);

    public virtual bool OnButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        return false;
    }

    public virtual void OnButtonsChanged(object? sender, ButtonsChangedEventArgs e)
    {
    }
}