/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-unlockable-areas
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Netcode;
using StardewValley.Network;
using Unlockable_Areas.NetLib;

namespace Unlockable_Areas.Lib
{
    public sealed class ModData
    {
        public Dictionary<string, bool> UnlockablePurchased { get; set; } = new Dictionary<string, bool>();
    }
}
