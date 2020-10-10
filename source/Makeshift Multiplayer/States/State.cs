/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMP
**
*************************************************/

namespace StardewValleyMP.States
{
    public abstract class State
    {
        // Always call the newer one first
        public abstract bool isDifferentEnoughFromOldStateToSend(State obj);
    }
}
