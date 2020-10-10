/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sakorona/SDVMods
**
*************************************************/

using StardewModdingAPI;
using System;
using System.Collections.Generic;

namespace TwilightShards.LunarDisturbances.Integrations
{
    /// <summary>The Content Patcher API which other mods can access.</summary>
    public interface IContentPatcherAPI
    {
        void RegisterToken(IManifest mod, string name, Func<IEnumerable<string>> getValue);
    }
}
