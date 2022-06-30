/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/GStefanowich/SDV-Forecaster
**
*************************************************/

/*
 * This software is licensed under the MIT License
 * https://github.com/GStefanowich/SDV-Forecaster
 *
 * Copyright (c) 2019 Gregory Stefanowich
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;

namespace ForecasterText.Objects {
    public abstract class ConfigEmojiComponent {
        public abstract uint Value { get; set; }
        
        public int Width {
            get => this.Bounds.X;
            set => this.Bounds = new Vector2I { X = value, Y = this.Bounds.Y };
        }
        public int Height {
            get => this.Bounds.Y;
            set => this.Bounds = new Vector2I { X = this.Bounds.X, Y = value };
        }
        
        public Vector2I Bounds;
        
        protected bool LeftDown { get; private set; }
        
        public virtual void OnDraw(SpriteBatch b, Vector2 vec2F)
            => this.OnDraw(b, (Vector2I)vec2F);
        
        public virtual void OnDraw(SpriteBatch b, Vector2I vector) {
            bool leftDown = Mouse.GetState() is { LeftButton: ButtonState.Pressed };
            
            // If the previous state is DOWN and new state is NOT DOWN
            if (this.LeftDown && !leftDown) {
                Vector2I mouse = new() {
                    X = Game1.getMouseX(),
                    Y = Game1.getMouseY()
                };
                Vector2I bounds = vector + this.Bounds;
                
                if (bounds.Less(mouse) && vector.More(mouse))
                    this.OnClick(vector, mouse);
                else
                    this.OnOutsideClick(mouse);
            }
            
            this.LeftDown = leftDown;
        }
        
        protected virtual void OnClick(Vector2I bounds, Vector2I mouse) {}
        
        protected virtual void OnOutsideClick(Vector2I mouse) {}
        
        /// <summary>
        /// Draw a generic box using <see cref="DialogueBox"/>
        /// </summary>
        protected void DrawBox(SpriteBatch b, Vector2I vector) {
            b.Draw(Game1.mouseCursors, new Rectangle(vector.X, vector.Y, this.Width, this.Height), new Rectangle?(new Rectangle(306, 320, 16, 16)), Color.White);
            b.Draw(Game1.mouseCursors, new Rectangle(vector.X, vector.Y - 20, this.Width, 24), new Rectangle?(new Rectangle(275, 313, 1, 6)), Color.White);
            b.Draw(Game1.mouseCursors, new Rectangle(vector.X + 12, vector.Y + this.Height, this.Width - 20, 32), new Rectangle?(new Rectangle(275, 328, 1, 8)), Color.White);
            b.Draw(Game1.mouseCursors, new Rectangle(vector.X - 32, vector.Y + 24, 32, this.Height - 28), new Rectangle?(new Rectangle(264, 325, 8, 1)), Color.White);
            b.Draw(Game1.mouseCursors, new Rectangle(vector.X + this.Width, vector.Y, 28, this.Height), new Rectangle?(new Rectangle(293, 324, 7, 1)), Color.White);
            b.Draw(Game1.mouseCursors, new Vector2((float) (vector.X - 44), (float) (vector.Y - 28)), new Rectangle?(new Rectangle(261, 311, 14, 13)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
            b.Draw(Game1.mouseCursors, new Vector2((float) (vector.X + this.Width - 8), (float) (vector.Y - 28)), new Rectangle?(new Rectangle(291, 311, 12, 11)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
            b.Draw(Game1.mouseCursors, new Vector2((float) (vector.X + this.Width - 8), (float) (vector.Y + this.Height - 8)), new Rectangle?(new Rectangle(291, 326, 12, 12)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
            b.Draw(Game1.mouseCursors, new Vector2((float) (vector.X - 44), (float) (vector.Y + this.Height - 4)), new Rectangle?(new Rectangle(261, 327, 14, 11)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
        }
    }
}
