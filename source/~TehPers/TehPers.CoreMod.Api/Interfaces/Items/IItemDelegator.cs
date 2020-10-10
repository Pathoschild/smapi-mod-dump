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
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TehPers.CoreMod.Api.Drawing;
using TehPers.CoreMod.Api.Drawing.Sprites;
using TehPers.CoreMod.Api.Items.ItemProviders;

namespace TehPers.CoreMod.Api.Items {
    public interface IItemDelegator {
        /// <summary>Tries to register a particular key. Once registered, an index will be assigned to the key whenever the item is available to be used.</summary>
        /// <param name="key">The key to register.</param>
        /// <returns>True if successful, false if the key is already registered.</returns>
        bool TryRegisterKey(in ItemKey key);

        /// <summary>Tries to get the index associated with a particular key.</summary>
        /// <param name="key">The item key to get the index for.</param>
        /// <param name="index">The index assigned to the given key, if one is assigned.</param>
        /// <returns>True if successful, false otherwise.</returns>
        bool TryGetIndex(in ItemKey key, out int index);

        /// <summary>Overrides when an item is drawn from a particular sprite sheet.</summary>
        /// <param name="key">The key for the item to override.</param>
        /// <param name="parentSheet">The sprite sheet the item would normally be drawn from by the game.</param>
        /// <param name="override">The callback which overrides the drawing call. It accepts the drawing info, the source position offset percentage, and the source size percentage and may override the drawing behavior.</param>
        void OverrideSprite(in ItemKey key, ISpriteSheet parentSheet, Action<IDrawingInfo, Vector2, Vector2> @override);

        /// <summary>Returns all the registered item providers.</summary>
        /// <returns>An <see cref="IEnumerable{T}"/> containing all the registered item providers.</returns>
        IEnumerable<IItemProvider> GetItemProviders();

        /// <summary>Tries to convert a string into the item key represented by it.</summary>
        /// <param name="source">The source string.</param>
        /// <param name="itemKey">The resulting item key, if successful.</param>
        /// <returns>True if successful, false otherwise.</returns>
        bool TryParseKey(string source, out ItemKey itemKey);
    }
}