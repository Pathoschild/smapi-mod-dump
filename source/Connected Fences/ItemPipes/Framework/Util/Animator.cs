/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sergiomadd/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItemPipes.Framework.Nodes;
using ItemPipes.Framework.Nodes.ObjectNodes;
using StardewValley;
using Microsoft.Xna.Framework;

namespace ItemPipes.Framework.Util
{
    public static class Animator
    {

        public static void AnimateItemMovement(List<PipeNode> path, IOPipeNode target, IOPipeNode source, Item item)
        {
            var current = Game1.currentGameTime.TotalGameTime.TotalMilliseconds;
            foreach(PipeNode pipe in path)
            {
                pipe.StartItemMovementAnimation(current, target, source, item);
                current += pipe.ItemTimer;
            }
        }

        public static void AnimatePipeConnection(List<PipeNode> path)
        {
            var current = Game1.currentGameTime.TotalGameTime.TotalMilliseconds;
            var endTime = current + 60 * path.Count;
            foreach (PipeNode pipe in path)
            {
                pipe.Connecting = true;
                pipe.StartConnectionAnimation(current, endTime);
                current += 60;
            }
        }
    }
}
