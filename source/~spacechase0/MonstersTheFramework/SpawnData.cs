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
using Microsoft.Xna.Framework;
using SpaceShared;

namespace MonstersTheFramework
{
    public enum SpawnTryTime
    {
        OnDayStart,
        OnLocationChange,
        OnTimeChange,
    }

    public class SpawnData
    {
        public List<Weighted<string>> Who { get; set; }
        public SpawnTryTime When { get; set; }
        public string Where { get; set; }
        public Rectangle? WhereArea { get; set; }
        public int HowMany { get; set; } = 1;
    }
}
