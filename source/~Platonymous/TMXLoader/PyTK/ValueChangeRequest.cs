/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMXLoader
{
    public class ValueChangeRequest<TKey,TValue>
    {
        public TKey Key { get; set; }
        public TValue Value { get; set; }

        public TValue Fallback { get; set; }

        public ValueChangeRequest(TKey key,TValue value, TValue fallback)
        {
            this.Key = key;
            this.Value = value;
            this.Fallback = fallback;
        }
    }
}
