/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace UtilityGrid
{
    public class PipeGroup
    {
        public List<Vector2> pipes = new List<Vector2>();
        public Dictionary<Vector2, UtilityObject> objects = new Dictionary<Vector2, UtilityObject>();
        public float excessUse;
    }
}