/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/WarpNetwork
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarpNetwork
{
    class CustomLocationHandler
    {
        public Action<string> Warp { get; set; }
        public Func<string, bool> GetEnabled { get; set; }
        public Func<string, string> GetLabel { get; set; }
        public CustomLocationHandler(Action<string> w, Func<string, bool> e, Func<string, string> l)
        {
            Warp = w;
            GetEnabled = e;
            GetLabel = l;
        }
    }
}
