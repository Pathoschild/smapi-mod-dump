using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using Netcode;


namespace MapUtilities.Trees
{
    public class Stump : TreePart
    {
        public Stump(TreeRenderer renderer, Microsoft.Xna.Framework.Rectangle sprite)
        {
            children = new List<TreePart>();
            spriteSheet = renderer.species.treeSheet;
            this.renderer = renderer;
            this.sprite = sprite;
            rotation = 0f;
            depth = 0;
        }

        public override void draw(SpriteBatch b, Vector2 treePos, float x, float y, float currentRotation, float depthOffset = 0f)
        {
            Vector2 local = Game1.GlobalToLocal(Game1.viewport, new Vector2(treePos.X * 64, treePos.Y * 64));

            b.Draw(spriteSheet, new Vector2(x + local.X, y + local.Y), sprite, renderer.transparency, rotation + currentRotation, new Vector2(sprite.Width / 2, sprite.Height - 1), 4f, SpriteEffects.None, (treePos.Y * 64) / 10000 + ((treePos.X * 64) % 9 - depth * 10) / 10000 + depthOffset);

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

            if (renderer.tree.stump && !(bool) ((NetFieldBase<bool, NetBool>) renderer.falling.GetValue() ))
                return;

            foreach (TreePart child in children)
            {
                child.draw(b, treePos, x + (float)rotatedX, y + (float)rotatedY, rotation + currentRotation + renderer.shakeRotation.GetValue(), depthOffset + 0.0001f + (children.IndexOf(child) * 0.001f));
            }
        }
    }
}
