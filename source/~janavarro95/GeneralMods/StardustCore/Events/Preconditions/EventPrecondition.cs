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

namespace Omegasis.StardustCore.Events.Preconditions
{
    public class EventPrecondition
    {

        public virtual string SerializedPrecondition
        {
            get
            {
                return this.ToString();
            }
        }

        public virtual bool meetsCondition()
        {
            return false;
        }

        public override string ToString()
        {
            return "StarducstCore.Events.Preconditions.EventPrecondition";
        }
    }
}
