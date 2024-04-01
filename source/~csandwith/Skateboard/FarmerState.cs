/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/csandwith/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Skateboard
{
    public class FarmerState
    {
        public List<int> dirs = new List<int>();
        public Vector2 pos = Vector2.Zero;
        public Vector2 drawOffset = Vector2.Zero;
        public bool shouldShadowBeOffset = false;
    }
}