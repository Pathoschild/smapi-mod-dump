/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraShared.Integrations.GMCMAttributes;

using Microsoft.Xna.Framework;

using StardewModdingAPI.Utilities;

namespace CameraPan.Framework;

/// <summary>
/// The config class for this mod.
/// </summary>
[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:Elements should appear in the correct order", Justification = "Fields kept near accessors.")]
public sealed class ModConfig
{
    /// <summary>
    /// Gets or sets a value indicating how the panning should be toggled.
    /// </summary>
    public ToggleBehavior ToggleBehavior { get; set; } = ToggleBehavior.Toggle;

    /// <summary>
    /// Gets or sets a value indicating whether or not mouse panning should be enabled.
    /// </summary>
    public bool UseMouseToPan { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating how middle click scroll should behave.
    /// </summary>
    public ClickAndDragBehavior ClickAndDragBehavior { get; set; } = ClickAndDragBehavior.DragMap;

    /// <summary>
    /// Gets or sets the color for which to draw the Mozilla Arrows TM.
    /// </summary>
    [GMCMDefaultColor(127, 255, 0, 255)]
    public Color ClickAndDragColor { get; set; } = Color.Chartreuse;

    /// <summary>
    /// Gets or sets a value indicating whether or not to hard snap the camera back if the player takes damage.
    /// </summary>
    public bool ResetWhenDamageTaken { get; set; } = false;

    private int speed = 8;

    /// <summary>
    /// Gets or sets the speed to move the camera at.
    /// </summary>
    [GMCMRange(1, 24)]
    public int Speed
    {
        get => this.speed;
        set => this.speed = Math.Clamp(value, 1, 24);
    }

    private int xRange = 1000;

    private int yRange = 1000;

    /// <summary>
    /// Gets or sets the maximum distance the focal point can be from the player, on the x axis.
    /// </summary>
    [GMCMSection("Boundaries", 5)]
    public int XRange
    {
        get => this.xRange;
        set => this.xRange = Math.Max(value, 1);
    }

    /// <summary>
    /// Gets or sets the maximum distance the focal point can be from the player, on the y axis.
    /// </summary>
    [GMCMSection("Boundaries", 5)]
    public int YRange
    {
        get => this.yRange;
        set => this.yRange = Math.Max(value, 1);
    }

    /// <summary>
    /// Gets or sets a value indicating whether or not players should be kept on screen.
    /// </summary>
    [GMCMSection("Boundaries", 5)]
    public bool KeepPlayerOnScreen { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether or not to show arrows to other players in multiplayer.
    /// </summary>
    [GMCMSection("Boundaries", 5)]
    public bool ShowArrowsToOtherPlayers { get; set; } = true;

    /// <summary>
    /// Gets or sets the color for which to draw an arrow pointing at the player, if the player is off screen.
    /// </summary>
    [GMCMSection("Boundaries", 5)]
    [GMCMDefaultColor(255, 0, 0, 255)]
    public Color SelfColor { get; set; } = Color.Red;

    /// <summary>
    /// Gets or sets the color for which to draw an arrow pointing at other players on the map.
    /// </summary>
    [GMCMSection("Boundaries", 5)]
    [GMCMDefaultColor(0, 0, 255, 255)]
    public Color FriendColor { get; set; } = Color.Blue;

    /// <summary>
    /// Gets or sets a value indicating which button should be used to toggle the panning.
    /// </summary>
    [GMCMSection("Keybind", 10)]
    public KeybindList ToggleButton { get; set; } = new(new(SButton.O), new(SButton.RightTrigger));

    /// <summary>
    /// Gets or sets the button used to reset the camera behind the player.
    /// </summary>
    [GMCMSection("Keybind", 10)]
    public KeybindList ResetButton { get; set; } = new(new(SButton.R), new(SButton.RightStick));

    /// <summary>
    /// Gets or sets the keybind used for ClickToScroll.
    /// </summary>
    [GMCMSection("Keybind", 10)]
    public KeybindList ClickToScroll { get; set; } = new(SButton.MouseMiddle);

    /// <summary>
    /// Gets or sets the button used to set the camera upwards.
    /// </summary>
    [GMCMSection("Keybind", 10)]
    public KeybindList UpButton { get; set; } = new(new(SButton.Up), new(SButton.RightThumbstickUp));

    /// <summary>
    /// Gets or sets the button used to set the camera downwards.
    /// </summary>
    [GMCMSection("Keybind", 10)]
    public KeybindList DownButton { get; set; } = new(new(SButton.Down), new(SButton.RightThumbstickDown));

    /// <summary>
    /// Gets or sets the button used to set the camera leftwards.
    /// </summary>
    [GMCMSection("Keybind", 10)]
    public KeybindList LeftButton { get; set; } = new(new(SButton.Left), new(SButton.RightThumbstickLeft));

    /// <summary>
    /// Gets or sets the button used to set the camera rightwards.
    /// </summary>
    [GMCMSection("Keybind", 10)]
    public KeybindList RightButton { get; set; } = new(new(SButton.Right), new(SButton.RightThumbstickRight));


    /// <summary>
    /// Gets or sets the behavior of the camera in an indoors location.
    /// </summary>
    [GMCMSection("Camera", 20)]
    public CameraBehavior IndoorsCameraBehavior { get; set; } = CameraBehavior.Both;

    /// <summary>
    /// Gets or sets the behavior of the camera in an outdoors location.
    /// </summary>
    [GMCMSection("Camera", 20)]
    public CameraBehavior OutdoorsCameraBehavior { get; set; } = CameraBehavior.Offset;

    /// <summary>
    /// Gets or sets the behavior of the camera per-map.
    /// </summary>
    [GMCMDefaultIgnore]
    public Dictionary<string, PerMapCameraBehavior> PerMapCameraBehavior { get; set; } = new();

    #region internal

    private int xRangeActual = 1000;
    private int yRangeActual = 1000;

    internal int XRangeInternal => this.xRangeActual;

    internal int YRangeInternal => this.yRangeActual;

    /// <summary>
    /// Recalculates the actual bounds that should be used for the viewport, taking into account the window size.
    /// </summary>
    internal void RecalculateBounds()
    {
        if (this.KeepPlayerOnScreen)
        {
            this.xRangeActual = Math.Max(0, Math.Min(this.XRange, (Game1.viewport.Width / 2) - 64));
            this.yRangeActual = Math.Max(0, Math.Min(this.YRange, (Game1.viewport.Height / 2) - 128));
        }
        else
        {
            this.xRangeActual = this.XRange;
            this.yRangeActual = this.YRange;
        }
    }

    #endregion
}

#region enums

/// <summary>
/// Gets the per-map behavior for the camera.
/// </summary>
public enum PerMapCameraBehavior
{
    /// <inheritdoc cref="CameraBehavior.Vanilla"/>
    Vanilla = CameraBehavior.Vanilla,

    /// <inheritdoc cref="CameraBehavior.Locked"/>
    Locked = CameraBehavior.Locked,

    /// <inheritdoc cref="CameraBehavior.Offset"/>
    Offset = CameraBehavior.Offset,

    /// <inheritdoc cref="CameraBehavior.Both"/>
    Both = CameraBehavior.Both,

    /// <summary>
    /// Uses the default for the indoors/outdoors.
    /// </summary>
    ByIndoorsOutdoors = 0b1 << 3,
}

/// <summary>
/// The behavior used for middle click, basically.
/// </summary>
public enum ClickAndDragBehavior
{
    /// <summary>
    /// No middle click scroll.
    /// </summary>
    Off,

    /// <summary>
    /// Middle click to drag the map with you.
    /// </summary>
    DragMap,

    /// <summary>
    /// The firefox autoscroll thingie.
    /// </summary>
    AutoScroll,
}

#endregion