/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;

namespace StardustCore.Events.Preconditions.PlayerSpecific
{
    public class CommunityCenterCompleted: EventPrecondition
    {
        /// <summary>
        /// False means the community center doesn't need to be completed.
        /// </summary>
        bool needsToBeCompleted;

        public CommunityCenterCompleted()
        {

        }

        public CommunityCenterCompleted(bool NeedsToBeCompleted)
        {
            this.needsToBeCompleted = NeedsToBeCompleted;
        }

        public override string ToString()
        {
            return "Omegasis.EventFramework.Preconditions.Player.CommunityCenterCompleted?";
        }

        public override bool meetsCondition()
        {
            return this.needsToBeCompleted == Game1.player.hasCompletedCommunityCenter();
        }

    }
}
