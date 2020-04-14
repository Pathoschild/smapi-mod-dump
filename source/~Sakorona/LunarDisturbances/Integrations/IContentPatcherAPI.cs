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
