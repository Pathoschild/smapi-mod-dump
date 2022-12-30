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

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace ForecasterText.Objects {
    internal sealed class ConfigEmojiMenu : ConfigEmojiComponent {
        private const int PER_ROW = 12;
        private const int ROWS = 5;
        private const int SHIFT = ConfigEmojiMenu.ROWS * ConfigEmojiMenu.PER_ROW;
        
        internal static Texture2D EmojiTextures;
        internal static Texture2D ChatBoxTexture;
        
        internal readonly Mod Mod;
        public readonly IConfT9N T9N;
        
        private readonly List<ClickableComponent> Emojis = new();
        private readonly ClickableComponent UpArrow;
        private readonly ClickableComponent DownArrow;
        
        private readonly Blink Blinking = new();
        
        private readonly Func<uint> Getter;
        private readonly Action<uint> Setter;
        
        public override uint Value {
            get => this.Getter?.Invoke() ?? 0u;
            set {
                this.Blinking.Reset();
                this.Setter?.Invoke(value);
            }
        }
        
        public int TotalEmojis { get; private init; }
        public int PageIndex { get; private set; }
        
        public ConfigEmojiMenu(
            Mod mod,
            IConfT9N t9N,
            Func<uint> get,
            Action<uint> set
        ) {
            ConfigEmojiMenu.EmojiTextures ??= Game1.content.Load<Texture2D>(@"LooseSprites\emojis");
            ConfigEmojiMenu.ChatBoxTexture ??= Game1.content.Load<Texture2D>(@"LooseSprites\chatBox");
            
            this.Mod = mod;
            this.T9N = t9N;
            
            this.Getter = get;
            this.Setter = set;
            
            this.Width = (16 + ConfigEmojiMenu.PER_ROW * 10 * 4) + 44; // 300
            this.Height = (16 + ConfigEmojiMenu.ROWS * 10 * 4) + 32;// 248
            
            for (int row = 0; row < ConfigEmojiMenu.ROWS; row++)
                for (int e = 0; e < ConfigEmojiMenu.PER_ROW; e++)
                    this.Emojis.Add(new ClickableComponent(new Rectangle(16 + e * 10 * 4, 16 + row * 10 * 4, 36, 36), (row + e * ConfigEmojiMenu.PER_ROW).ToString() ?? ""));
            
            this.UpArrow = new ClickableComponent(new Rectangle(16 + ConfigEmojiMenu.PER_ROW * 10 * 4, 16, 32, 20), "");
            this.DownArrow = new ClickableComponent(new Rectangle(16 + ConfigEmojiMenu.PER_ROW * 10 * 4, 156, 32, 20), "");
            
            // Total amount of emojis based on the sheet size (9 width * 9 height per emoji)
            this.TotalEmojis = ConfigEmojiMenu.EmojiTextures.Width / 9 * (ConfigEmojiMenu.EmojiTextures.Height / 9);
            
            this.ResetView();
        }
        
        public override void OnDraw(SpriteBatch b, Vector2I vector) {
            base.OnDraw(b, vector);
            this.DrawBox(b, vector);
            
            //b.Draw(ConfigEmojiMenu.ChatBoxTexture, new Rectangle(vector.X, vector.Y, this.Width, this.Height), new Rectangle?(new Rectangle(0, 0, 300, 56)), Color.White);
            for (int index = 0; index < this.Emojis.Count; ++index) {
                int offset = index + this.PageIndex;
                
                // Get the emoji
                if (this.Emojis[index] is not ClickableComponent emoji || !this.WithinBounds(offset))
                    continue;
                
                // Update the scale of the icon
                bool selected = this.Value == offset;
                if (selected)
                    emoji.scale = this.Blinking.Scale;
                else if (emoji.scale < 1.0)
                    emoji.scale += Blink.SHIFT;
                
                // Draw the emoji
                b.Draw(ConfigEmojiMenu.EmojiTextures, new Vector2((float) (emoji.bounds.X + vector.X + 16), (float) (emoji.bounds.Y + vector.Y + 16)), new Rectangle?(new Rectangle((this.PageIndex + index) * 9 % ConfigEmojiMenu.EmojiTextures.Width, (this.PageIndex + index) * 9 / ConfigEmojiMenu.EmojiTextures.Width * 9, 9, 9)), selected ? Color.White : (Color.DimGray * 0.8f), 0.0f, new Vector2(4.5f, 4.5f), emoji.scale * 4f, SpriteEffects.None, 0.9f);
            }
            
            if (this.UpArrow.scale < 1.0)
                this.UpArrow.scale += 0.05f;
            if (this.DownArrow.scale < 1.0)
                this.DownArrow.scale += 0.05f;
            
            // Draw the up/down buttons
            b.Draw(ConfigEmojiMenu.ChatBoxTexture, new Vector2((float) (this.UpArrow.bounds.X + vector.X + 16), (float) (this.UpArrow.bounds.Y + vector.Y + 10)), new Rectangle?(new Rectangle(156, 300, 32, 20)), Color.White * (this.PageIndex == 0 ? 0.25f : 1f), 0.0f, new Vector2(16f, 10f), this.UpArrow.scale, SpriteEffects.None, 0.9f);
            b.Draw(ConfigEmojiMenu.ChatBoxTexture, new Vector2((float) (this.DownArrow.bounds.X + vector.X + 16), (float) (this.DownArrow.bounds.Y + vector.Y + 10)), new Rectangle?(new Rectangle(192, 300, 32, 20)), Color.White * (this.PageIndex >= this.TotalEmojis - ConfigEmojiMenu.SHIFT ? 0.25f : 1f), 0.0f, new Vector2(16f, 10f), this.DownArrow.scale, SpriteEffects.None, 0.9f);
        }
        
        protected override void OnClick(Vector2I bounds, Vector2I mouse) {
            Rectangle decrease = new(this.UpArrow.bounds.X + bounds.X, this.UpArrow.bounds.Y + bounds.Y, 32, 20);
            Rectangle increase = new(this.DownArrow.bounds.X + bounds.X, this.DownArrow.bounds.Y + bounds.Y, 32, 20);
            
            if (mouse.IsIn(decrease)) {
                if (this.PageIndex != 0)
                    Game1.playSound("Cowboy_Footstep");
                
                this.PageIndex = Math.Max(0, this.PageIndex - ConfigEmojiMenu.SHIFT);
                this.UpArrow.scale = 0.75f;
                
                this.ResetIcons();
            } else if (mouse.IsIn(increase)) {
                if (this.PageIndex < this.TotalEmojis - ConfigEmojiMenu.SHIFT)
                    Game1.playSound("Cowboy_Footstep");
                
                this.PageIndex = Math.Min(this.PageIndex + ConfigEmojiMenu.SHIFT, ConfigEmojiMenu.SHIFT * (int)(Math.Floor((double)this.TotalEmojis / ConfigEmojiMenu.SHIFT)));
                this.DownArrow.scale = 0.75f;
                
                this.ResetIcons();
            } else {
                Vector2I relative = mouse - bounds;
                
                double top = (relative.Y - 16d) / 40d;
                double left = (relative.X - 16d) / 40d;
                
                if (top >= 0.0f && left >= 0.0f) {
                    uint column = (uint)Math.Floor(left);
                    uint row = (uint)Math.Floor(top);
                    int emote = (int)((row * ConfigEmojiMenu.PER_ROW) + column + this.PageIndex);
                    
                    if (column < ConfigEmojiMenu.PER_ROW && row < ConfigEmojiMenu.ROWS && this.WithinBounds(emote)) {
                        this.Value = (uint)emote;
                        Game1.playSound("coin");
                    }
                }
            }
        }
        
        public bool WithinBounds(int emote)
            => emote >= 0 && emote < this.TotalEmojis;
        
        public void ResetView() {
            this.PageIndex = 0;
            
            // Make sure we scroll the current emoji into view by default
            while (this.Value > this.PageIndex + ConfigEmojiMenu.SHIFT)
                this.PageIndex += ConfigEmojiMenu.SHIFT;
            
            this.ResetIcons();
        }
        
        public void ResetIcons() {
            // Reset the emoji scale
            this.Emojis.ForEach(icon => icon.scale = 1.0f);
            
            // Reset the pulse
            this.Blinking.Reset();
        }
    }
}
