/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMP
**
*************************************************/

using StardewValley.TerrainFeatures;

namespace StardewValleyMP.States
{
    public class TreeState : State
    {
        public bool stump;
        public bool tapped;
        public bool seed;

        public TreeState(Tree tree)
        {
            stump = tree.stump;
            tapped = tree.tapped;
            seed = tree.hasSeed;
        }

        public override bool isDifferentEnoughFromOldStateToSend(State obj)
        {
            TreeState state = obj as TreeState;
            if (state == null) return false;

            if (stump != state.stump) return true;
            if (tapped != state.tapped) return true;
            if (seed != state.seed) return true;

            return false;
        }

        public override string ToString()
        {
            return base.ToString() + " " + stump + " " + tapped + " " + seed;
        }
    }
}
