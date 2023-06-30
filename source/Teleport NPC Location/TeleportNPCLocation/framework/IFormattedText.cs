/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/chencrstu/TeleportNPCLocation
**
*************************************************/

using Microsoft.Xna.Framework;

namespace TeleportNPCLocation.framework
{
    /// <summary>A snippet of formatted text.</summary>
    internal interface IFormattedText
    {
        /// <summary>The font color (or <c>null</c> for the default color).</summary>
        Color? Color { get; }

        /// <summary>The text to format.</summary>
        string? Text { get; }

        /// <summary>Whether to draw bold text.</summary>
        bool Bold { get; }
    }
}

