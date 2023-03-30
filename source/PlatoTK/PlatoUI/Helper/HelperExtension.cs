/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using StardewModdingAPI;
using System.Collections.Generic;
using System.Linq;

namespace PlatoUI
{
    public static class HelperExtension
    {
        public static HashSet<IPlatoUIHelper> Helper = new HashSet<IPlatoUIHelper>();

        public static IPlatoUIHelper GetPlatoUIHelper(this IModHelper helper)
        {
            IPlatoUIHelper platoHelper = Helper.FirstOrDefault(p => p.ModHelper.ModRegistry.ModID == helper.ModRegistry.ModID);
            if (platoHelper is IPlatoUIHelper)
                return platoHelper;

            platoHelper = new PlatoHelper(helper);
            Helper.Add(platoHelper);

            return platoHelper;
        }

        public static IPlatoUIHelper GetPlatoHelper(this Mod mod) => mod.Helper.GetPlatoUIHelper();
    }
}
