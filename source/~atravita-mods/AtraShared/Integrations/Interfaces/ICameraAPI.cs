/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

namespace AtraShared.Integrations.Interfaces;

/// <summary>
/// The API for this mod.
/// </summary>
public interface ICameraAPI
{
    /// <summary>
    /// Gets a value indicating whether panning mode is currently enabled.
    /// </summary>
    public bool IsEnabled { get; }

    /// <summary>
    /// Gets the current camera behavior.
    /// </summary>
    public CameraBehavior Behavior { get; }

    /// <summary>
    /// Gets the current toggle behavior.
    /// </summary>
    public ToggleBehavior ToggleBehavior { get; }

    /// <summary>
    /// Enables panning mode.
    /// </summary>
    public void Enable();

    /// <summary>
    /// Disables panning mode.
    /// </summary>
    public void Disable();

    /// <summary>
    /// Resets the camera over the player by slowly panning back.
    /// </summary>
    public void Reset();

    /// <summary>
    /// Immediately resets the camera back over the player.
    /// </summary>
    public void HardReset();
}

/// <summary>
/// Controls how the camera should behave.
/// Don't copy the [EnumsExtensions] attribute, that's for internal sourcegen.
/// </summary>
[Flags]
public enum CameraBehavior
{
    /// <summary>
    /// Use the vanilla behavior.
    /// </summary>
    Vanilla = 0,

    /// <summary>
    /// Always keep the player in the center.
    /// </summary>
    Locked = 0b1,

    /// <summary>
    /// Apply the offset, if relevant.
    /// </summary>
    Offset = 0b10,

    /// <summary>
    /// Always keep the offset position in the center.
    /// </summary>
    Both = Locked | Offset,
}

/// <summary>
/// Indicates how the camera panning should be toggled.
/// </summary>
public enum ToggleBehavior
{
    /// <summary>
    /// Camera panning should never be allowed.
    /// </summary>
    Never,

    /// <summary>
    /// A hotkey controls camera panning.
    /// </summary>
    Toggle,

    /// <summary>
    /// Holding the camera object allows panning.
    /// </summary>
    Camera,

    /// <summary>
    /// Panning is always enabled.
    /// </summary>
    Always,
}