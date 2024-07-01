/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/focustense/StardewRadialMenu
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;

namespace RadialMenu.Gmcm;

/// <summary>
/// Public API for https://github.com/jltaylor-us/StardewGMCMOptions.
/// </summary>
/// <remarks>
/// We only use it for the color picker right now.
/// </remarks>
public interface IGMCMOptionsAPI
{
    /// <summary>Add a <c cref="Color">Color</c> option at the current position in the GMCM form.</summary>
    /// <param name="mod">The mod's manifest.</param>
    /// <param name="getValue">Get the current value from the mod config.</param>
    /// <param name="setValue">Set a new value in the mod config.</param>
    /// <param name="name">The label text to show in the form.</param>
    /// <param name="tooltip">The tooltip text shown when the cursor hovers on the field, or <c>null</c> to disable the tooltip.</param>
    /// <param name="showAlpha">Whether the color picker should allow setting the Alpha channel</param>
    /// <param name="colorPickerStyle">Flags to control how the color picker is rendered.  <see cref="ColorPickerStyle"/></param>
    /// <param name="fieldId">The unique field ID for use with GMCM's <c>OnFieldChanged</c>, or <c>null</c> to auto-generate a randomized ID.</param>
    public void AddColorOption(IManifest mod, Func<Color> getValue, Action<Color> setValue, Func<string> name,
        Func<string>? tooltip = null, bool showAlpha = true, uint colorPickerStyle = 0, string? fieldId = null);

    #pragma warning disable format
    /// <summary>
    /// Flags to control how the <c cref="ColorPickerOption">ColorPickerOption</c> widget is displayed.
    /// </summary>
    [Flags]
    public enum ColorPickerStyle : uint {
        Default = 0,
        RGBSliders    = 0b00000001,
        HSVColorWheel = 0b00000010,
        HSLColorWheel = 0b00000100,
        AllStyles     = 0b11111111,
        NoChooser     = 0,
        RadioChooser  = 0b01 << 8,
        ToggleChooser = 0b10 << 8
    }
    #pragma warning restore format
}
