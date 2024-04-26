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

namespace RainbowTrail
{
    public class PositionInfo
    {
        public Vector2 position;
        public int direction;

        public PositionInfo(Vector2 pos, int d)
        {
            position = pos;
            direction = d;
        }
    }
}