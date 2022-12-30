/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Tlitookilakin/AeroCore
**
*************************************************/

using AeroCore.API;
using AeroCore.Utils;
using StardewValley;

namespace AeroCore.Models
{
    public class HeldItemEventArgs : IHeldItemEventArgs
    {
        public Item Item { get; }
        public Farmer Who { get; }
        public string StringId { get; }

        internal HeldItemEventArgs(Farmer who, Item item)
        {
            Who = who;
            Item = item;
            StringId = item.GetStringID();
        }
    }
}
