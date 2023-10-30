/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewDruid
{
    class MultiplayerData
    {

        public Dictionary<long, StaticData> farmhandData;

        public MultiplayerData()
        {

            farmhandData = new Dictionary<long, StaticData>();

        }

    }

}
