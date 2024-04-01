/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JoXW100/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace UtilityGrid
{
    public class UtilitySystem
    {
        public Dictionary<Vector2, GridPipe> pipes = new Dictionary<Vector2, GridPipe>();
        public List<PipeGroup> groups = new List<PipeGroup>();
        public Dictionary<Vector2, UtilityObjectInstance> objects = new Dictionary<Vector2, UtilityObjectInstance>();

    }
}