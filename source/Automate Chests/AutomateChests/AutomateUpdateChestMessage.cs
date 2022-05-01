/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/SinZ163/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;

namespace AutomateChests
{
    /// <summary>A network message that indicates a specific chest has changed options and should be updated.</summary>
    /// A clone of https://github.com/Pathoschild/StardewMods/blob/4d50f9e72c1d55f9ff9be3bdb339099816f27fb7/Common/Messages/AutomateUpdateChestMessage.cs
    internal class AutomateUpdateChestMessage
    {
        /// <summary>The location name containing the chest.</summary>
        public string LocationName { get; set; }

        /// <summary>The chest's tile position.</summary>
        public Vector2 Tile { get; set; }
    }
}
