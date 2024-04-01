/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley/personal-indoor-farm
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalIndoorFarm.Lib
{
    public class Main
    {

        public static void Initialize()
        {
            AssetRequested.Initialize();
            Door.Initialize();
            Painting.Initialize();
            PersonalFarm.Initialize();
            _GameLocation.Initialize();
            DayUpdate.Initialize();
            SpaceTimeSynchronizer.Initialize();
            SelectionMenu.Initialize();
        }
    }
}
