/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewDruid.Map
{
    static class FireData
    {

        public static Dictionary<string, Vector2> FirePoints()
        {

            Dictionary<string, Vector2> firePoints = new()
            {
                ["Mountain"] = new Vector2(29,9),
                ["Beach"] = new Vector2(48,20),
                ["Forest"] = new Vector2(47, 97),
            };

            return firePoints;

        }


    }
}
