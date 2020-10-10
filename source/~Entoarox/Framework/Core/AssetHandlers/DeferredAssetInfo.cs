/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Entoarox/StardewMods
**
*************************************************/

using System;

namespace Entoarox.Framework.Core.AssetHandlers
{
    internal class DeferredAssetInfo
    {
        /*********
        ** Accessors
        *********/
        public Type Type { get; set; }
        public Delegate Handler { get; set; }


        /*********
        ** Public methods
        *********/
        public DeferredAssetInfo(Type type, Delegate handler)
        {
            this.Type = type;
            this.Handler = handler;
        }
    }
}
