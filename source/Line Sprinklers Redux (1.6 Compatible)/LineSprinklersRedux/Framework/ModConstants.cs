/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/rtrox/LineSprinklersRedux
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LineSprinklersRedux.Framework
{
    internal class ModConstants
    {
        public static string ModKeySpace => $"{ModEntry.Helper!.ModRegistry.ModID}";

        // ContextTags
        public static string MainContextTag = $"{ModKeySpace}_LineSprinklers";

        public static string OverlayDummyItemID = $"{ModKeySpace}_DummyOverlayItem";

    }
}
