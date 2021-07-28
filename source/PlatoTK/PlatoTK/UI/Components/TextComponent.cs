/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using BmFont;
using Microsoft.Xna.Framework;
using System;

namespace PlatoTK.UI.Components
{
    public class TextComponent : Component
    {
        public Func<IComponent, string> Text { get; set; }
        public Func<IComponent, Font> Font { get; set; }

        public float Scale { get; set; } = 1f;

        public Color Color { get; set; } = Color.White;

        public override string ComponentName => "Text";

        public int TextLimit { get; set; } = -1;

        public TextComponent(IPlatoHelper helper)
            : base(helper)
        {

        }

        public override IComponent New(IPlatoHelper helper)
        {
            return new TextComponent(helper);
        }

        public override void ParseAttribute(string attribute, string value)
        {
            ParsedData p = new ParsedData(attribute, value);

            if (Parsed.Contains(p))
                return;

            Parsed.Add(p);

            if (attribute.ToLower() == "content")
                Text = (c) => c.GetWrapper().TryLoadText(value, c, out string text) ? text : "";
            else if (attribute.ToLower() == "font")
                Font = (c) => c.GetWrapper().TryGet(value, out Font font) ? font : null;
            else if (attribute.ToLower() == "textscale")
                Scale = float.TryParse(value, out float scale) ? scale : 1f;
            else if (attribute.ToLower() == "textlimit")
                TextLimit = int.TryParse(value, out int limit) ? limit : -1;
            else
            {
                Parsed.Remove(p);
                base.ParseAttribute(attribute, value);
            }
        }

        protected virtual void PrepareText(bool draw = false)
        {
            int dx = 0; 
            int dy = 0;
            int height = 0;
            int width = 0;
            if (Font?.Invoke(this) is Font current)
            {
                string text = (Text?.Invoke(this) ?? "");

                text = TextLimit == -1 || text.Length <= TextLimit ? text : text.Substring(0, TextLimit - 2) + "...";

                foreach (char c in text)
                {
                    FontChar fc;
                    if (current.CharacterMap.TryGetValue(c, out fc))
                    {
                        var sourceRectangle = new Rectangle(fc.X, fc.Y, fc.Width, fc.Height);
                        height = Math.Max(height, (int)(sourceRectangle.Height * Scale));

                        if (draw)
                        {
                            var position = new Vector2(dx + fc.XOffset, dy + fc.YOffset);
                            Rectangle destination = new Rectangle((int)position.X, (int)position.Y, (int)(sourceRectangle.Width * Scale), (int)(sourceRectangle.Height * Scale));
                            DrawInstruction textDraw = new DrawInstruction(Id + " text", current.FontPages[fc.Page], destination, sourceRectangle, Color, 0f, Vector2.Zero, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 1f);
                            AddDrawInstructions(textDraw);
                        }

                        width = (dx) + (int)(sourceRectangle.Width * Scale);
                        dx += (int)(fc.XAdvance * Scale);
                    }
                }
            }

            if (!draw)
            {
                DefaultBounds = new Rectangle(0, 0, width, height);
                Bounds = DefaultBounds;
            }
        }

        public override void Setup()
        {
            base.Setup();
            PrepareText();
        }

        public override void Compose(IComponent parent)
        {
            base.Compose(parent);
            PrepareText(true);
        }

    }
}
