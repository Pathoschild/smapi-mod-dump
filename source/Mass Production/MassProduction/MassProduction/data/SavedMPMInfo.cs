/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JacquePott/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassProduction
{
    [Serializable]
    public class SavedMPMInfo
    {
        public string LocationName;
        public int CoordinateX;
        public int CoordinateY;
        public string UpgradeKey;

        public Vector2 GetCoordinates()
        {
            return new Vector2(CoordinateX, CoordinateY);
        }

        public string GetIDString()
        {
            return $"{LocationName}_{CoordinateX}_{CoordinateY}";
        }
    }
}
