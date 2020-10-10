/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Entoarox/StardewMods
**
*************************************************/

namespace Entoarox.Framework.Advanced
{
    public static class TupleSyntax
    {
        /*********
        ** Public methods
        *********/
        public static PolyTuple<T1, T2> T<T1, T2>(T1 item1, T2 item2)
        {
            return new PolyTuple<T1, T2>(item1, item2);
        }

        public static PolyTuple<T1, T2, T3> T<T1, T2, T3>(T1 item1, T2 item2, T3 item3)
        {
            return new PolyTuple<T1, T2, T3>(item1, item2, item3);
        }

        public static PolyTuple<T1, T2, T3, T4> T<T1, T2, T3, T4>(T1 item1, T2 item2, T3 item3, T4 item4)
        {
            return new PolyTuple<T1, T2, T3, T4>(item1, item2, item3, item4);
        }

        public static PolyTuple<T1, T2, T3, T4, T5> T<T1, T2, T3, T4, T5>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5)
        {
            return new PolyTuple<T1, T2, T3, T4, T5>(item1, item2, item3, item4, item5);
        }
    }
}
