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
using SFarmer = StardewValley.Farmer;

namespace StardewValleyMP.States
{
    public class ChestState : State
    {
        public int opener;

        public ChestState(Chest chest)
        {
            SFarmer farmer = ( SFarmer ) Util.GetInstanceField( typeof( Chest ), chest, "opener" );
            opener = farmer != null ? Multiplayer.getFarmerId(farmer) : -1;

            // TODO: Move this to a proper place
            if (Util.GetInstanceField(typeof(Chest), chest, "opener") == null && chest.frameCounter == -2) // It's stuck open
            {
                chest.frameCounter = 2;
            }
        }

        public override bool isDifferentEnoughFromOldStateToSend(State obj)
        {
            ChestState state = obj as ChestState;
            if (state == null) return false;

            if (opener != state.opener)
            {
                return true;
            }

            return false;
        }

        public override string ToString()
        {
            return base.ToString() + " " + opener;
        }
    }
}
