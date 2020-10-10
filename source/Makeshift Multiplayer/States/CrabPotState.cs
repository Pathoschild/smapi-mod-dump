/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMP
**
*************************************************/

using StardewValley.Objects;

namespace StardewValleyMP.States
{
    public class CrabPotState : ObjectState
    {
        public bool bait;

        public CrabPotState(CrabPot pot) : base( pot )
        {
            bait = ( pot.bait != null );
        }

        public override bool isDifferentEnoughFromOldStateToSend(State obj)
        {
            if (base.isDifferentEnoughFromOldStateToSend(obj)) return true;

            CrabPotState state = obj as CrabPotState;
            if (state == null) return false;

            if (bait != state.bait) return true;

            return false;
        }

        public override string ToString()
        {
            return base.ToString() + " " + bait;
        }
    }
}
