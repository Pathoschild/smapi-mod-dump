/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Netcode;
using StardewValley;

namespace MoonMisadventures.VirtualProperties
{
    public static class FarmerTeam_LunarKey
    {
        internal class Holder { public readonly NetBool Value = new(); }

        internal static ConditionalWeakTable< FarmerTeam, Holder > values = new();

        public static void set_hasLunarKey( this FarmerTeam farmer, NetBool newVal )
        {
            // We don't actually want a setter for this one, since it should be readonly
            // Net types are weird
            // Or do we? Serialization
        }

        public static NetBool get_hasLunarKey( this FarmerTeam farmer )
        {
            var holder = values.GetOrCreateValue( farmer );
            return holder.Value;
        }
    }
}
