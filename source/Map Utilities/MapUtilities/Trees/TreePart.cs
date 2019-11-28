using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Netcode;
using StardewValley.TerrainFeatures;
using StardewValley;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapUtilities.Trees
{
    public abstract class TreePart
    {
        public float rotation;
        public float depth;
        public bool left = false;

        public Texture2D spriteSheet;
        public Rectangle sprite;
        public TreeRenderer renderer;

        public List<TreePart> children;

        public virtual void draw(SpriteBatch b, Vector2 treePos, float x, float y, float currentRotation, float depthOffset = 0f)
        {
            Vector2 local = Game1.GlobalToLocal(Game1.viewport, new Vector2(treePos.X * 64, treePos.Y * 64));

            b.Draw(spriteSheet, new Vector2(x + local.X, y + local.Y), sprite, renderer.transparency, rotation + currentRotation, new Vector2(sprite.Width / 2, sprite.Height - 1), 4f, left ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (treePos.Y * 64) / 10000 + ((treePos.X * 64) % 9 - depth * 10) / 10000 + depthOffset);

            float xOffset = 0f;
            float yOffset = 0f;

            //Offset so that the new endpoint sits on top of the tree part section.  No matter what size it is, the next section is at the end of it.
            //At this point, the endpoint is envisioned where the part sits vertically, and the end point is vertically up from the anchor point.  We rotate it after this.
            //We're back to using yOffset, since the entire tree is not going to rotate, just this part.
            yOffset -= sprite.Height * 4f;

            //Math!
            //We will rotate the two offsets, getting us a new anchor point for the children.
            //Rotating the offsets makes sure the axis of rotation is this part's anchor, not the root of the tree itself.
            //Otherwise you'd have weird floating branches.
            double rotatedX = xOffset * Math.Cos(rotation + currentRotation) - yOffset * Math.Sin(rotation + currentRotation);
            double rotatedY = xOffset * Math.Sin(rotation + currentRotation) + yOffset * Math.Cos(rotation + currentRotation);

            //Logger.log("xOffset: " + xOffset + " yOffset: " + yOffset + " rotatedX: " + rotatedX + " rotatedY: " + rotatedY);
            
            foreach (TreePart child in children)
            {
                child.draw(b, treePos, x + (float)rotatedX, y + (float)rotatedY, rotation + currentRotation, depthOffset + 0.0001f + (children.IndexOf(child) * 0.001f));
            }
        }

        public virtual void performSetup()
        {

        }

        public virtual void addPart(TreePart part)
        {
            if (children.Contains(part))
            {
                Logger.log("Attempted to add already-existing part.");
                return;
            }
            if (this.isParentOf(part))
            {
                Logger.log("Attempted to add a child to a parent that already contained it!");
                return;
            }
            if (part.isParentOf(this))
            {
                Logger.log("Attempted to add a parent as a child!");
                return;
            }
            part.depth = depth;
            part.left = left;
            Logger.log("Part is on " + (part.left ? "left" : "right") + ", added to parent on " + (left ? "left" : "right"));
            children.Add(part);
        }

        public virtual TreePart findAnyEnd(Type[] acceptableParents = null)
        {
            foreach(TreePart child in children)
            {
                if(acceptableParents == null || acceptableParents.Contains(child.GetType()))
                {
                    return child.findAnyEnd(acceptableParents);
                }
            }
            return this;
        }

        public virtual List<TreePart> findAllEnds(Type[] acceptableParents = null)
        {
            List<TreePart> ends = new List<TreePart>();
            foreach(TreePart child in children)
            {
                if (acceptableParents == null || acceptableParents.Contains(child.GetType()))
                {
                    foreach(TreePart end in child.findAllEnds(acceptableParents))
                    {
                        ends.Add(end);
                    }
                }
            }
            if(children.Count == 0)
            {
                ends.Add(this);
            }
            return ends;
        }

        public virtual float findTotalRotationOfChild(TreePart part)
        {
            if (!this.isParentOf(part))
                return 0f;
            return findTotalRotationInternal(part);
        }

        internal virtual float findTotalRotationInternal(TreePart part)
        {
            float totalChildRotation = 0f;
            if (part == this)
            {
                totalChildRotation += rotation;
                //Logger.log("Part found: total rotation now rotation of child: " + totalChildRotation);
            }
            foreach(TreePart child in children)
            {
                totalChildRotation += child.findTotalRotationInternal(part);
                //Logger.log("Total rotation now " + totalChildRotation);
            }
            if(totalChildRotation != 0f)
            {
                totalChildRotation += rotation;
            }
            return totalChildRotation;
        }

        

        public virtual int getHeightOf(TreePart goal, Type[] ignoreTypes = null)
        {
            foreach(TreePart child in children)
            {
                int thisValue = 0;
                if(ignoreTypes == null || !ignoreTypes.Contains(this.GetType()))
                {
                    //Logger.log("This part was " + this.GetType().ToString() + ", which was acceptable.");
                    thisValue = 1;
                }
                if(child == goal)
                {
                    //Logger.log("Child was goal, result is now " + (1 + thisValue));
                    return 1 + thisValue;
                }
                int childHeight = child.getHeightOf(goal, ignoreTypes);
                if(childHeight >= 1)
                {
                    //Logger.log("Running total was greater than 0.  Total is now " + (childHeight + thisValue));
                    return childHeight + thisValue;
                }
            }
            return -1;
        }

        public virtual int getExtent(Type[] acceptableParents = null, int currentHeight = 0)
        {
            int greatest = currentHeight + 1;
            foreach(TreePart child in children)
            {
                if(acceptableParents == null || !acceptableParents.Contains(child.GetType()))
                {
                    int childHeight = child.getExtent(acceptableParents, currentHeight + 1);
                    if (childHeight > greatest)
                    {
                        greatest = childHeight;
                    }
                }
            }
            return greatest;
        }

        public virtual bool isParentOf(TreePart part)
        {
            if (this == part)
                return true;
            foreach(TreePart child in children)
            {
                if (child.isParentOf(part))
                    return true;
            }
            return false;
        }
    }
}
