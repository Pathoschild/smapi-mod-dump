/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MisappliedPhysicalities
{
    public interface ILogicObject
    {
        // Signals are from 0 to 1
        public InOutType GetLogicTypeForSide( Side side );
        double GetLogicFrom( Side side );
        void SendLogicTo( Side side, double signal );
    }
}
