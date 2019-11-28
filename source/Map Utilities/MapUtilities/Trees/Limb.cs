using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;

namespace MapUtilities.Trees
{
    public class Limb : TreePart
    {
        public Limb(TreeRenderer renderer, Microsoft.Xna.Framework.Rectangle sprite)
        {
            children = new List<TreePart>();
            spriteSheet = renderer.species.treeSheet;
            this.renderer = renderer;
            this.sprite = sprite;
            rotation = 0f;
            depth = 0;
        }

        public virtual void addGravity(bool addToChildren = false)
        {
            float massHeld = getExtent() / 10f;
            float distance = renderer.treeStructure.getHeightOf(this, new Type[] { typeof(Stump), typeof(Trunk) }) / 10f;

            float parentRotation = renderer.treeStructure.findTotalRotationOfChild(this) - rotation;

            Logger.log("Limb has a weight of " + massHeld + ", and a distance of " + distance);

            float rotationFactor = Math.Max((massHeld * (1-renderer.species.limbStregnth)) - (distance * (1-renderer.species.limbStregnth) * 0.5f), 0f);

            Logger.log("Rotation factor = " + rotationFactor + "; Max(" + massHeld + " * " + renderer.species.limbStregnth + ") - (" + distance + " * " + renderer.species.limbStregnth + " * 0.5), 0)");

            //float newRot = ((rotationFactor * TreeRenderer.degreesToRadians(180f)) + rotation) / 2f - parentRotation;

            float rotDifference = TreeRenderer.degreesToRadians(180f) - (rotation + parentRotation);

            float newRot = (rotDifference * rotationFactor) + rotation;

            Logger.log("Rotating to " + TreeRenderer.radiansToDegrees(newRot) + " degrees...");
            rotation = newRot;

            if (addToChildren)
            {
                foreach (TreePart child in children)
                {
                    if (child is Limb)
                        (child as Limb).addGravity(addToChildren);
                }
            }
        }
    }
}
