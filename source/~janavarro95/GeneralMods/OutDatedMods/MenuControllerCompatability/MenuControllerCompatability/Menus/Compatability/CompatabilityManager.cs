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

namespace Compatability
{
    class CompatabilityManager
    {
        public static bool characterCustomizer;
        public static bool loadMenu;
        public static bool aboutMenu;
        public static bool doUpdate;
        public static Compatability.CompatInterface compatabilityMenu;

        public static void doUpdateSet(bool f)
        {
           doUpdate = f;
        }

        public static void doUpdateGet()
        {

        }
    }
}
