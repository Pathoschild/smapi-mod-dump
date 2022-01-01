/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System;
using StardewModdingAPI;

namespace TehPers.Core.Api.Content
{
    /// <summary>
    /// Tracks changes to assets.
    /// </summary>
    public interface IAssetTracker
    {
        /// <summary>
        /// Invoked whenever an asset is loaded or reloaded.
        /// </summary>
        public event EventHandler<IAssetData>? AssetLoading;
    }
}
