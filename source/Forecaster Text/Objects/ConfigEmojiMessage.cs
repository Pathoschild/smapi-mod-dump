/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/GStefanowich/SDV-Forecaster
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace ForecasterText.Objects {
    internal class ConfigEmojiMessage : ConfigEmojiComponent {
        private readonly Mod Mod;
        private readonly ConfigEmojiMenu Parent;
        private readonly List<ChatSnippet> Snippets = new();
        private readonly LocalizedContentManager.LanguageCode Language = LocalizedContentManager.LanguageCode.en;
        public float Alpha = 1f;
        public Color Color = Color.Black;
        
        private readonly ConfigMessageRenderer Setter;
        
        private bool _Dirty;
        public bool Dirty {
            get => this._Dirty || this.Value != this.CachedValue;
            set => this._Dirty = value;
        }
        
        public override uint Value {
            get => this.Parent.Value;
            set => this.Parent.Value = value;
        }
        public uint CachedValue;
        
        public ConfigEmojiMessage(
            Mod mod,
            ConfigEmojiMenu parent,
            ConfigMessageParsingRenderer setter
        ) {
            this.Mod = mod;
            this.Parent = parent;
            
            this.Setter = raw => setter(raw) is {} str ? this.ConvertString(str) : null;
            
            // Load the value for the first time
            this.UpdateText();
        }
        
        public ConfigEmojiMessage(
            Mod mod,
            ConfigEmojiMenu parent,
            ConfigMessageRenderer setter
        ) {
            this.Mod = mod;
            this.Parent = parent;
            
            this.Setter = setter;
            
            // Load the value for the first time
            this.UpdateText();
        }
        
        public override void OnDraw(SpriteBatch b, Vector2I vector)
            => this.DrawShiftedVector(b, new Vector2I { X = vector.X - 600, Y = vector.Y + 85 });
        
        private void DrawShiftedVector(SpriteBatch b, Vector2I vector) {
            if (this.Dirty)
                this.UpdateText();
            
            float num1 = 0.0f;
            float num2 = 0.0f;
            for (int index = 0; index < this.Snippets.Count; ++index) {
                if (this.Snippets[index].emojiIndex != -1)
                    b.Draw(ConfigEmojiMenu.EmojiTextures, new Vector2((float) ((double) vector.X + (double) num1 + 20), (float) ((double) vector.Y + (double) num2 + 16)), new Rectangle?(new Rectangle(this.Snippets[index].emojiIndex * 9 % ConfigEmojiMenu.EmojiTextures.Width, this.Snippets[index].emojiIndex * 9 / ConfigEmojiMenu.EmojiTextures.Width * 9, 9, 9)), Color.White * this.Alpha, 0.0f, new Vector2(4.5f, 4.5f), 3f, SpriteEffects.None, 0.99f);
                else if (this.Snippets[index].message != null) {
                    if (this.Snippets[index].message.Equals(Environment.NewLine)) {
                        num1 = 0.0f;
                        num2 += ChatBox.messageFont(this.Language).MeasureString("(").Y;
                    } else
                        b.DrawString(ChatBox.messageFont(this.Language), this.Snippets[index].message, new Vector2((float) vector.X + num1, (float) vector.Y + num2), this.Color * this.Alpha, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.99f);
                }
                num1 += this.Snippets[index].myLength;
                if ((double) num1 >= 888.0) {
                    num1 = 0.0f;
                    num2 += ChatBox.messageFont(this.Language).MeasureString("(").Y;
                    if (this.Snippets.Count > index + 1 && this.Snippets[index + 1].message != null && this.Snippets[index + 1].message.Equals(Environment.NewLine))
                        ++index;
                }
            }
        }
        
        private void UpdateText() {
            try {
                // Clear the snippets
                this.Snippets.Clear();
                if (this.Setter is {} renderer && renderer(this) is {} ienumerable)
                    this.Snippets.AddRange(ienumerable);
            } finally {
                // Always update the value
                this.CachedValue = this.Value;
            }
        }
        
        private IEnumerable<ChatSnippet> ConvertString(string raw) {
            if (raw is not {Length: >0})
                return null;
            
            StringBuilder builder = new();
            List<ChatSnippet> list = new();
            
            for (int i = 0; i < raw.Length; i++) {
                if (raw[i] != '[')
                    builder.Append(raw[i]);
                else {
                    // Dump the builder
                    this.WriteTo(list, builder);
                    
                    int num1 = raw.IndexOf(']', i);
                    int num2 = -1;
                    if (num1 + 1 < raw.Length)
                        num2 = raw.IndexOf('[', i + 1);
                    if (num1 != -1 && (num2 == -1 || num2 > num1)) {
                        string str = raw.Substring(i + 1, num1 - i - 1);
                        if (int.TryParse(str, out int result) && result < this.Parent.TotalEmojis) {
                            list.Add(new ChatSnippet(result));
                        } else {
                            builder.Append(str);
                        }
                        
                        i = num1;
                    } else {
                        builder.Append('[');
                    }
                }
            }
            
            // Clear the remainder of the builder
            this.WriteTo(list, builder);
            
            return list;
        }
        
        private void WriteTo(ICollection<ChatSnippet> list, StringBuilder builder) {
            if (builder.Length > 0) {
                list.Add(new ChatSnippet(builder.ToString(), this.Language));
                builder.Clear();
            }
        }
    }
}
