/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kqrse/StardewValleyMods-aedenthorn
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using System.Collections.Generic;

namespace CustomSpousePatioRedux
{
    public class OutdoorAreaData
    {
        public Dictionary<string, Vector2> areas;
        public Dictionary<string, OutdoorArea> dict = new Dictionary<string, OutdoorArea>();
    }

    public class OutdoorArea
    {
        public Vector2 corner;
        public string location;
    }
}