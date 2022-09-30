/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sergiomadd/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;

namespace ItemPipes.Framework.Nodes.ObjectNodes
{
    public class IridiumExtractorPipeNode : OutputPipeNode
    {
        public IridiumExtractorPipeNode()
        {

        }
        public IridiumExtractorPipeNode(Vector2 position, GameLocation location, StardewValley.Object obj) : base(position, location, obj)
        {
            Tier = 3;
            ItemTimer = 150;
            Flux = 20;
        }
    }
}
