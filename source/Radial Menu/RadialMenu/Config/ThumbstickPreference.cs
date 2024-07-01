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

/// <summary>
/// Controls which thumbstick is used to select items from a radial menu.
/// </summary>
/// <remarks>
/// Thumbstick preference typically needs to be coordinated with <see cref="ItemActivationMethod"/>,
/// i.e. in order to avoid very awkward gestures such as right trigger + right thumb + action
/// button on the right-hand side of the controller. If you are using a traditional controller
/// layout, it is recommended to use either <see cref="ItemActivationMethod.ThumbStickPress"/> or
/// <see cref="ItemActivationMethod.TriggerRelease"/> when choosing any preference other than
/// <see cref="AlwaysLeft"/>.
/// </remarks>
public enum ThumbStickPreference
{
    /// <summary>
    /// Always use the left thumbstick, regardless of which menu is open.
    /// </summary>
    AlwaysLeft,
    /// <summary>
    /// Always use the right thumbstick, regardless of which menu is open.
    /// </summary>
    AlwaysRight,
    /// <summary>
    /// Use the thumbstick that is on the same side as the trigger button used to open the menu.
    /// </summary>
    SameAsTrigger
};
