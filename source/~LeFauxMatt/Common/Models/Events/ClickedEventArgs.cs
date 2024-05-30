/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

#if IS_FAUXCORE
namespace StardewMods.FauxCore.Common.Models.Events;

using Microsoft.Xna.Framework;
using StardewMods.FauxCore.Common.Services.Integrations.FauxCore;

#else
namespace StardewMods.Common.Models.Events;

using Microsoft.Xna.Framework;
using StardewMods.Common.Services.Integrations.FauxCore;
#endif

/// <inheritdoc />
internal sealed class ClickedEventArgs : IClicked
{
    /// <summary></summary>
    /// <param name="button">The button pressed.</param>
    /// <param name="cursor">The cursor position.</param>
    public ClickedEventArgs(SButton button, Point cursor)
    {
        this.Button = button;
        this.Cursor = cursor;
    }

    public SButton Button { get; }

    /// <inheritdoc />
    public Point Cursor { get; }
}