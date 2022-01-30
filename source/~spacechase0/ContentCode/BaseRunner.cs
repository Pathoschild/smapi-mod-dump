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
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;

namespace ContentCode
{
    public abstract class BaseRunner
    {
        public IContentPack ContentPack { get; internal set; }
        public IReflectionHelper Reflection { get; internal set; }
        public Dictionary<string, object> State { get; internal set; }

        public abstract void Run( object[] args );
    }
}
