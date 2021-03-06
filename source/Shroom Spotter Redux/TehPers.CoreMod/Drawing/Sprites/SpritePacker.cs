/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.Xna.Framework;
using TehPers.CoreMod.Api.Drawing.Sprites;

namespace TehPers.CoreMod.Drawing.Sprites {
    internal class SpritePacker {
        public void Fit(IEnumerable<ISprite> sprites, out int width, out int height, out IEnumerable<LinkedSprite> linkedSprites) {
            Node root = new Node(0, 0);
            
            // Add each sprite to the sprite sheet from biggest to smallest
            foreach (ISprite sprite in sprites.OrderByDescending(s => Math.Max(s.Width, s.Height))) {
                // Try to add the current sprite without resizing the sprite sheet
                if (!this.TryAddSprite(root, sprite)) {
                    // Force the sprite to be added, resizing the sprite sheet
                    root = this.ForceAddSprite(root, sprite);
                }
            }

            // Convert the tree into linked sprites
            linkedSprites = from node in root.EnumerateInternalNodes()
                            let destination = new Rectangle(node.Bounds.X, node.Bounds.Y, node.Sprite.Width, node.Sprite.Height)
                            select new LinkedSprite(node.Sprite, destination);

            // Set the width and height of the sprite sheet
            width = root.Bounds.Width;
            height = root.Bounds.Height;
        }

        private bool TryAddSprite(Node node, ISprite sprite) {
            if (node.Used) {
                return this.TryAddSprite(node.Right, sprite) || this.TryAddSprite(node.Down, sprite);
            }

            if (node.Bounds.Width < sprite.Width || node.Bounds.Height < sprite.Height) {
                return false;
            }

            node.Split(sprite);
            return true;
        }

        private Node ForceAddSprite(Node root, ISprite sprite) {
            // Check which directions would be easy to expand in
            bool canGrowRight = sprite.Height <= root.Bounds.Height;
            bool canGrowDown = sprite.Width <= root.Bounds.Width;

            // Check which directions should be expanded in to keep the shape relatively square
            bool shouldGrowRight = canGrowRight && (root.Bounds.Height >= root.Bounds.Width + sprite.Width);
            bool shouldGrowDown = canGrowDown && (root.Bounds.Width >= root.Bounds.Height + sprite.Height);

            if (shouldGrowRight) {
                root = root.ExpandRight(sprite.Width);
            } else if (shouldGrowDown) {
                root = root.ExpandDown(sprite.Height);
            } else if (canGrowRight) {
                root = root.ExpandRight(sprite.Width);
            } else if (canGrowDown) {
                root = root.ExpandDown(sprite.Height);
            } else if (root.Bounds.Width == 0 && root.Bounds.Height == 0 && root.Sprite == null) {
                root = new Node(sprite.Width, sprite.Height);
            } else {
                // This should not occur, but put the sprite on the bottom of the sprite sheet, expanding it in both directions as needed
                root = root.ExpandRight(sprite.Width - root.Bounds.Width);
                root = root.ExpandDown(sprite.Height);
            }

            if (!this.TryAddSprite(root, sprite)) {
                throw new Exception("Could not add sprite");
            }

            return root;
        }

        public class LinkedSprite {
            public ISprite SourceSprite { get; }
            public Rectangle Destination { get; }

            public LinkedSprite(ISprite sourceSprite, Rectangle destination) {
                this.SourceSprite = sourceSprite;
                this.Destination = destination;
            }
        }

        public class Node {
            public Rectangle Bounds { get; private set; }
            public ISprite Sprite { get; private set; }
            public Node Right { get; private set; }
            public Node Down { get; private set; }
            public bool Used { get; private set; }

            public Node(int width, int height) : this(new Rectangle(0, 0, width, height)) { }

            private Node(Rectangle bounds) {
                this.Bounds = bounds;
            }

            public void Split(ISprite sprite) {
                this.Sprite = sprite;
                this.Used = true;
                this.Right = new Node(new Rectangle(this.Bounds.X + sprite.Width, this.Bounds.Y, this.Bounds.Width - sprite.Width, sprite.Height));
                this.Down = new Node(new Rectangle(this.Bounds.X, this.Bounds.Y + sprite.Height, this.Bounds.Width, this.Bounds.Height - sprite.Height));
            }

            public Node ExpandRight(int addedWidth) {
                return new Node(new Rectangle(this.Bounds.X, this.Bounds.Y, this.Bounds.Width + addedWidth, this.Bounds.Height)) {
                    // Right node is the expanded space
                    Right = new Node(new Rectangle(this.Bounds.Right, this.Bounds.Y, addedWidth, this.Bounds.Height)),

                    // Down node is the current node
                    Down = this,

                    // Don't store any sprites in this node
                    Used = true
                };
            }

            public Node ExpandDown(int addedHeight) {
                return new Node(new Rectangle(this.Bounds.X, this.Bounds.Y, this.Bounds.Width, this.Bounds.Height + addedHeight)) {
                    // Right node is the current node
                    Right = this,

                    // Down node is the expanded space
                    Down = new Node(new Rectangle(this.Bounds.X, this.Bounds.Bottom, this.Bounds.Width, addedHeight)),

                    // Don't store any sprites in this node
                    Used = true
                };
            }

            public IEnumerable<Node> EnumerateInternalNodes() {
                Queue<Node> remainingNodes = new Queue<Node>();
                remainingNodes.Enqueue(this);

                while (remainingNodes.Any()) {
                    Node cur = remainingNodes.Dequeue();

                    // Yield the current node if it has a sprite
                    if (cur.Sprite != null) {
                        yield return cur;
                    }

                    // Check the right child if it exists
                    if (cur.Right != null) {
                        remainingNodes.Enqueue(cur.Right);
                    }

                    // Check the down child if it exists
                    if (cur.Down != null) {
                        remainingNodes.Enqueue(cur.Down);
                    }
                }
            }
        }
    }
}