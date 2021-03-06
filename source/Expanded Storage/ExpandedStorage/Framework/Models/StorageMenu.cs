/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/ExpandedStorage
**
*************************************************/

using System;
using ImJustMatt.Common.Extensions;

namespace ImJustMatt.ExpandedStorage.Framework.Models
{
    internal class StorageMenu
    {
        internal readonly int Capacity;

        internal readonly int Offset;

        internal readonly int Padding;

        internal readonly int Rows;

        internal StorageMenu(StorageConfig storage)
        {
            Capacity = storage.Capacity switch
            {
                0 => -1, // Vanilla
                { } capacity when capacity < 0 => 72, // Unlimited
                { } capacity when capacity < 12 => capacity,
                { } capacity => Math.Min(72, capacity.RoundUp(12)) // Specific
            };

            Rows = Math.Max(3, (int) Math.Ceiling(Capacity / 12f));

            Padding = storage.Option("ShowSearchBar") == StorageConfig.Choice.Enable ? 24 : 0;

            Offset = 64 * (Rows - 3);
        }
    }
}