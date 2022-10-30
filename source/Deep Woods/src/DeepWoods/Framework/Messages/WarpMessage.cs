/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/maxvollmer/DeepWoodsMod
**
*************************************************/

using Microsoft.Xna.Framework;

namespace DeepWoodsMod.Framework.Messages
{
    internal class WarpMessage
    {
        public int Level { get; set; }
        public string Name { get; set; }
        public Vector2 EnterLocation { get; set; }
    }
}
