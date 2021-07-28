/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

namespace Magic
{
    public class AnalyzeEventArgs
    {
        /*********
        ** Accessors
        *********/
        public int TargetX;
        public int TargetY;


        /*********
        ** Public methods
        *********/
        public AnalyzeEventArgs(int tx, int ty)
        {
            this.TargetX = tx;
            this.TargetY = ty;
        }
    }
}
