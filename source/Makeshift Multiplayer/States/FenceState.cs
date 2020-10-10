/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMP
**
*************************************************/

using StardewValley;

namespace StardewValleyMP.States
{
    public class FenceState : State
    {
        public int pos;

        public FenceState(Fence fence)
        {
            pos = fence.gatePosition;
        }

        public override bool isDifferentEnoughFromOldStateToSend(State obj)
        {
            FenceState state = obj as FenceState;
            if (state == null) return false;

            if (pos != state.pos)
            {
                return true;
            }

            return false;
        }

        public override string ToString()
        {
            return base.ToString() + " " + pos;
        }
    }
}
