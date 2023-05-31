/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/Archery
**
*************************************************/

using Archery.Framework.Utilities.Backport;
using StardewValley;

namespace Archery.Framework.Models.Generic
{
    public class QueryableModel
    {
        public string UnlockCondition { get; set; }

        internal bool HasRequirements(Farmer who)
        {
            return GameStateQuery.CheckConditions(UnlockCondition, target_farmer: who);
        }
    }
}
