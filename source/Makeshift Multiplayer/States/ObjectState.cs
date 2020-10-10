/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMP
**
*************************************************/

using Object = StardewValley.Object;

namespace StardewValleyMP.States
{
    public class ObjectState : State
    {
        public bool hasSomething;
        public int ready;

        public ObjectState(Object obj)
        {
            hasSomething = (obj.heldObject != null);
            ready = obj.minutesUntilReady;
        }

        public override bool isDifferentEnoughFromOldStateToSend(State obj)
        {
            ObjectState state = obj as ObjectState;
            if (state == null) return false;

            if (hasSomething != state.hasSomething) return true;
            if (ready > state.ready) return true;

            return false;
        }

        public override string ToString()
        {
            return base.ToString() + " " + hasSomething + " " + ready;
        }
    }
}
