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
using Omegasis.HappyBirthday.Framework.Events.EventPreconditions;
using Omegasis.StardustCore.Events.Preconditions;

namespace Omegasis.HappyBirthday.Framework.Events.Compatibility
{
    /// <summary>
    /// An event precondition to add compatibility for resolving conflicts with Stardew Valley Expanded maps and Vanilla maps since SDVE adds new tile data to maps which can cause Happy Birthday events to get stuck.
    /// </summary>
    public class IsStardewValleyExpandedInstalledPrecondition : EventPrecondition
    {

        public const string EventPreconditionId = "Omegasis.HappyBirthday.Framework.Events.Compatibility.StardewValleyExpanded.IsStardewValleyExpandedInstalledPrecondition";

        /// <summary>
        /// Checks to see if Stardew Valley Expanded should be loaded or not.
        /// </summary>
        public bool shouldBeLoaded;
        public IsStardewValleyExpandedInstalledPrecondition()
        {
            this.shouldBeLoaded = false;
        }

        public IsStardewValleyExpandedInstalledPrecondition(bool ShouldBeLoaded)
        {
            this.shouldBeLoaded = ShouldBeLoaded;
        }

        public override string ToString()
        {
            return IsStardewValleyExpandedInstalledPrecondition.EventPreconditionId + " " +this.shouldBeLoaded;
        }

        public override bool meetsCondition()
        {
            //Check to make sure both conditions match each other, since we want to return true if both conditions are false, so that way the vanilla events can load properly.
            bool isSDVELoaded = HappyBirthdayModCore.Instance.Helper.ModRegistry.IsLoaded("FlashShifter.SVECode");
            if(this.shouldBeLoaded && isSDVELoaded)
            {
                return true;
            }
            if(this.shouldBeLoaded==false && isSDVELoaded == false)
            {
                return true;
            }

            return false;
        }

    }
}
