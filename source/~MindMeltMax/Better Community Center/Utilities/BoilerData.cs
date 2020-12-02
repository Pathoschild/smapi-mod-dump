/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using System.Collections.Generic;

namespace BCC.Utilities
{
    public class BoilerData
    {
        public static List<data> dataList { get; set; }
    }

    public class data
    {
        public int ParentSheetIndex { get; set; }

        public int Stack { get; set; }

        public int HoldingComponentID { get; set; }

        public data(int index, int stack, int component)
        {
            ParentSheetIndex = index;
            Stack = stack;
            HoldingComponentID = component;
        }
    }
}
