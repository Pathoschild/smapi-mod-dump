/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using StardewModdingAPI.Utilities;

namespace CameraPan.Framework;

/// <summary>
/// The config class for this mod.
/// </summary>
[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:Elements should appear in the correct order", Justification = "Fields kept near accessors.")]
public sealed class ModConfig
{
    private int speed = 8;

    /// <summary>
    /// Gets or sets the speed to move the camera at.
    /// </summary>
    public int Speed
    {
        get => this.speed;
        set => this.speed = Math.Clamp(value, 1, 20);
    }

    private int xRange = 1000;

    private int yRange = 1000;

    /// <summary>
    /// Gets or sets the maximum distance the focal point can be from the player, on the x axis.
    /// </summary>
    public int XRange
    {
        get => this.xRange;
        set => this.xRange = Math.Max(value, 1);
    }

    /// <summary>
    /// Gets or sets the maximum distance the focal point can be from the player, on the y axis.
    /// </summary>
    public int YRange
    {
        get => this.yRange;
        set => this.yRange = Math.Max(value, 1);
    }

    private bool keepPlayerOnScreen = true;

    public bool KeepPlayerOnScreen
    {
        get => this.keepPlayerOnScreen;
        set => this.keepPlayerOnScreen = value;
    }

    public KeybindList ResetButton { get; set; } = new(singleKey: SButton.Escape);

    #region internal

    private int xRangeActual = 1000;
    private int yRangeActual = 1000;

    internal int XRangeInternal => this.xRangeActual;

    internal int YRangeInternal => this.yRangeActual;

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
