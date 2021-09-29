/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;

namespace CustomCritters.Framework.CritterData
{
    internal class BehaviorModel
    {
        public string Type { get; set; }
        public float Speed { get; set; }
        public List<BehaviorPatrolPoint> PatrolPoints { get; set; } = new();
        public int PatrolPointDelay { get; set; }
        public int PatrolPointDelayAddRandom { get; set; }
    }
}
