using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapUtilities.Trees
{
    public class Branch : TreePart
    {
        public Branch(TreeRenderer renderer, Microsoft.Xna.Framework.Rectangle sprite)
        {
            children = new List<TreePart>();
            spriteSheet = renderer.species.treeSheet;
            this.renderer = renderer;
            this.sprite = sprite;
            rotation = 0f;
            depth = 0;
        }
    }
}
