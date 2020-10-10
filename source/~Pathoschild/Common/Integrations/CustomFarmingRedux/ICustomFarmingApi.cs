/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace Pathoschild.Stardew.Common.Integrations.CustomFarmingRedux
{
    /// <summary>The API provided by the Custom Farming Redux mod.</summary>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "The naming convention is defined by the Custom Farming Redux mod.")]
    public interface ICustomFarmingApi
    {
        /*********
        ** Public methods
        *********/
        /// <summary>Get metadata for a custom machine and draw metadata for an object.</summary>
        /// <param name="dummy">The item that would be replaced by the custom item.</param>
        Tuple<Item, Texture2D, Rectangle, Color> getRealItemAndTexture(StardewValley.Object dummy);
    }
}
