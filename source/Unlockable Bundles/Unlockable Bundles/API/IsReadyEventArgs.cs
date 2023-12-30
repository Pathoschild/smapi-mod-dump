/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-unlockable-bundles
**
*************************************************/

using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unlockable_Bundles.API
{
    public class IsReadyEventArgs : IIsReadyEventArgs
    {
        public Farmer Who { get; }
        public IsReadyEventArgs(Farmer who)
        {
            this.Who = who;
        }
    }
}
