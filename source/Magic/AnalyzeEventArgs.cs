/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/Magic
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic
{
    public class AnalyzeEventArgs
    {
        public int TargetX;
        public int TargetY;

        public AnalyzeEventArgs(int tx, int ty)
        {
            TargetX = tx;
            TargetY = ty;
        }
    }
}
