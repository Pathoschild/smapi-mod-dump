/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/MysticalBuildings
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaveOfMemories.Framework.Models
{
    public class ModConfig
    {
        public int StatueOfGreedRefreshInDays { get; set; } = 1;
        public int QuizzicalStatueRefreshInDays { get; set; } = 1;
        public int CrumblingMineshaftRefreshInDays { get; set; } = 1;
        public int PhantomClockDaysToGoBack { get; set; } = 7;
        public int OrbOfReflectionRefreshInDays { get; set; } = 1;
        public int ObeliskOfWeatherRefreshInDays { get; set; } = 1;
    }
}
