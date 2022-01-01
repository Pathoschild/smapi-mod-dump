/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using TehPers.Core.Api.DI;
using TehPers.Core.Api.Json;
using TehPers.Core.Json;

namespace TehPers.Core.Modules
{
    public class ModJsonModule : ModModule
    {
        public override void Load()
        {
            this.GlobalProxyRoot.Bind<IJsonProvider>()
                .To<CommentedJsonProvider>()
                .InSingletonScope();
        }
    }
}