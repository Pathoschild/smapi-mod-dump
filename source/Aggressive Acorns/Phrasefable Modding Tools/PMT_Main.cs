/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/phrasefable/StardewMods
**
*************************************************/

using JetBrains.Annotations;
using StardewModdingAPI;

namespace Phrasefable_Modding_Tools
{
    [UsedImplicitly]
    public partial class PhrasefableModdingTools : Mod
    {
        public override void Entry([NotNull] IModHelper helper)
        {
            SetUp_Ground();

            SetUp_Tally();

            SetUp_EventLogging();

            SetUp_Clear();
        }
    }
}
