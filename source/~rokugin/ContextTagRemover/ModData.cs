/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/rokugin/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextTagRemover {
    internal record ModData {

        public Dictionary<string, string[]> ObjectTags { get; }

        public ModData(Dictionary<string, string[]> objectTags) {
            ObjectTags = objectTags ?? new();
        }

    }
}
