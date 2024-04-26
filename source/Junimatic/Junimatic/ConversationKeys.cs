/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NermNermNerm/Junimatic
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;

namespace NermNermNerm.Junimatic
{
    public static class ConversationKeys
    {
        public const string JunimosLastTripToMine = "QuestableTractor.JunimosLastTripToMine";

        // TODO: Move to UnlockMiner

        internal static void EditAssets(IAssetName nameWithoutLocale, IDictionary<string, string> topics)
        {
            if (nameWithoutLocale.IsEquivalentTo("Characters/Dialogue/Lewis"))
            {
                topics[ConversationKeys.JunimosLastTripToMine] = "You've been to the mines I see...  Your grandfather did too, even after his mind began to go.  I worried for him, but there was no stopping him.$0#$b#"
                    + "Then one day he came to me, shaken to the core.  He sobbed 'A big slime ate my friend!' but I know he went to the mines alone...$2#$b#"
                    + "He kept going on like that for several days.  I felt terrible for him, because although *I* knew it wasn't real, it was real enough to him.$0#$b#"
                    + "He never went back to the mines; that was very much for the best.$2";
            }
        }
    }
}
