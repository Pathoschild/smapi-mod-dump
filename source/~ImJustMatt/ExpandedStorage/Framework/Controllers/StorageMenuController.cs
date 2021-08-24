/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

using System;
using XSAutomate.Common.Extensions;

namespace ExpandedStorage.Framework.Controllers
{
    internal class StorageMenuController
    {
        internal readonly int Capacity;

        internal readonly int Offset;

        internal readonly int Padding;

        internal readonly int Rows;

        internal StorageMenuController(StorageConfigController config)
        {
            Capacity = config.Capacity switch
            {
                0 => -1, // Vanilla
                { } capacity when capacity < 0 => 72, // Unlimited
                { } capacity when capacity < 12 => capacity,
                { } capacity => Math.Min(72, capacity.RoundUp(12)) // Specific
            };

            Rows = Capacity > 0 ? (int) Math.Ceiling(Capacity / 12f) : 3;

            Padding = config.Option("ShowSearchBar", true) == StorageConfigController.Choice.Enable ? 24 : 0;

            Offset = 64 * (Rows - 3);
        }
    }
}