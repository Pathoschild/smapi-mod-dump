/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Gaphodil/BetterJukebox
**
*************************************************/

using System.Collections.Generic;

namespace Gaphodil.BetterJukebox.Framework
{
    public class FilterListConfig(string s)
    {
        public List<string> content = ToList(s);

        private static List<string> ToList(string s)
        {
            List<string> l = new(s.Split(','));
            for (int i = 0; i < l.Count; i++)
            {
                l[i] = l[i].Trim();
            }
            return l;
        }

        public override string ToString()
        {
            return string.Join(",", [.. content]);
        }
    }
}
