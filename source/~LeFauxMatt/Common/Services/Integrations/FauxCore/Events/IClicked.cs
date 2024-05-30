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
namespace StardewMods.FauxCore.Common.Services.Integrations.FauxCore;

using Microsoft.Xna.Framework;

#else
namespace StardewMods.Common.Services.Integrations.FauxCore;

using Microsoft.Xna.Framework;
#endif

/// <summary>The event arguments when a component is clicked.</summary>
public interface IClicked
{
    /// <summary>Gets the button pressed.</summary>
    SButton Button { get; }

    /// <summary>Gets the cursor position.</summary>
    Point Cursor { get; }
}