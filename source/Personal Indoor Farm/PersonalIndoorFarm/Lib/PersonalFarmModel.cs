/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley/personal-indoor-farm
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley.GameData.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalIndoorFarm.Lib
{
    public class PersonalFarmModel : LocationData
    {
        public string MapAsset_T0;
        public string MapAsset_T1;
        public string MapAsset_T2;

        public Point ArrivalTile_T0;
        public Point ArrivalTile_T1;
        public Point ArrivalTile_T2;

        public string Preview;
    }
}
