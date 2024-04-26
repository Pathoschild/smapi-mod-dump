/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/scayze/multiprairie
**
*************************************************/

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerPrairieKing.Utility
{
    /// <summary>
    /// Bitch ass methods to properly serialize fucking XNA Vector2s. There probably is a proper way,
    /// But for me they always serialized to an empty dict "{ }". So yeah, 
    /// My frustratiopn born solution is to convert them on the fly to a quick and dirty self made Vector class
    /// </summary>
    public class Serialization
    {
        public class SVector2
        {
            public float X { get; set;}
            public float Y { get; set;}

            public SVector2()
            {
                X = 0f;
                Y = 0f;
            }
            public SVector2(float pX, float pY)
            {
                X = pX;
                Y = pY;
            }
        }
        public static List<SVector2> ConvertToSVector2(List<Vector2> list)
        {
            List<SVector2> sList = new();
            foreach (Vector2 v in list)
            {
                sList.Add(new SVector2(v.X,v.Y));
            }
            return sList;
        }

        public static List<Vector2> ConvertFromSVector2(List<SVector2> sList)
        {
            List<Vector2> list = new();
            foreach (SVector2 v in sList)
            {
                list.Add(new Vector2(v.X,v.Y));
            }
            return list;
        }
    }
}
