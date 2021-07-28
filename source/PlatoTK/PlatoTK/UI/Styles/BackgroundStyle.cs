/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PlatoTK.UI.Components;
using System;

namespace PlatoTK.UI.Styles
{
    public enum BackgroundFill
    {
        Stretch,
        Original,
        Cover,
        Contain,
        Pattern,
        TileMap
    }

    public class BackgroundStyle : Style
    {
        public BackgroundStyle(IPlatoHelper helper, string option = "")
            : base(helper,option)
        {
            Priority = -2;
        }

        public override string[] PropertyNames => new string[] { "Image", "BackgroundImage", "BackgroundMaxScale", "BackgroundOpacity","BackgroundFill", "BackgroundColor", "BackgroundSize", "BackgroundSourceRectangle" };

        public float BackgroundOpacity { get; set; } = 1f;

        public Color BackgroundColor { get; set; } = Color.Transparent;

        public Func<IComponent, Texture2D> BackgroundImage { get; set; }

        public Func<IComponent, Texture2D> Image { get; set; }

        public Rectangle? BackgroundBounds { get; set; }

        public Rectangle? BackgroundSourceRectangle { get; set; }

        public BackgroundFill FillStyle { get; set; } = BackgroundFill.Original;

        public float BackgroundMaxScale { get; set; } = float.MaxValue;

        public override void Apply(IComponent component)
        {
            if (BackgroundColor.A != 0)
            {
                component.RemoveDrawInstructions(d => d.Tag == "bgColor");

                var bgColorInstruction = new DrawInstruction(
                    "bgColor",
                    Helper.Content.Textures.WhitePixel,
                    new Rectangle(0, 0, component.Bounds.Width, component.Bounds.Height),
                    null, BackgroundColor, 0f, Vector2.Zero, SpriteEffects.None, 1f);

                component.AddDrawInstructions(bgColorInstruction);
            }

            if (BackgroundImage?.Invoke(component) is Texture2D bgImage)
            {
                component.RemoveDrawInstructions(d => d.Tag == "bgImage");

                BackgroundBounds = new Rectangle(0, 0, bgImage.Width, bgImage.Height);
                
                Rectangle drawBounds = BackgroundBounds.Value;

                float fitToHeight = (float) component.Bounds.Height / (float)BackgroundBounds.Value.Height;
                float fitToWidth = (float)component.Bounds.Width / (float)BackgroundBounds.Value.Width;
                int tileWidth = (int)Math.Ceiling(component.Bounds.Width / (float)drawBounds.Width);
                int tileHeight = (int)Math.Ceiling(component.Bounds.Height / (float)drawBounds.Height);

                switch (FillStyle)
                {
                    case BackgroundFill.Stretch:
                        {
                            drawBounds = new Rectangle(0, 0, component.Bounds.Width, component.Bounds.Height);
                            break;
                        }
                    case BackgroundFill.Contain:
                        {
                            float scale = Math.Min(Math.Min(fitToHeight, fitToWidth), BackgroundMaxScale);
                            int w = (int)(BackgroundBounds.Value.Width * scale);
                            int h = (int)(BackgroundBounds.Value.Height * scale);
                            drawBounds = new Rectangle((component.Bounds.Width - w) / 2, (component.Bounds.Height - h) / 2, w, h);
                            break;
                        }
                    case BackgroundFill.Cover:
                        {
                            float scale = Math.Max(fitToHeight, fitToWidth);
                            drawBounds = new Rectangle(0, 0, (int)(BackgroundBounds.Value.Width * scale), (int)(BackgroundBounds.Value.Height * scale));
                            break;
                        }
                    case BackgroundFill.TileMap:
                        {
                            drawBounds = new Rectangle(0, 0, drawBounds.Width / 3, drawBounds.Height / 3);
                            break;
                        }
                    case BackgroundFill.Original:
                    case BackgroundFill.Pattern:
                    default:
                        break;
                }

                switch (FillStyle)
                {
                    case BackgroundFill.Stretch:
                    case BackgroundFill.Cover:
                    case BackgroundFill.Contain:
                    case BackgroundFill.Original:
                        {
                            var bgImageInstruction = new DrawInstruction(
                                "bgImage",
                            bgImage,
                            drawBounds,
                            BackgroundSourceRectangle, Color.White * BackgroundOpacity, 0f, Vector2.Zero, SpriteEffects.None, 1f);
                            component.AddDrawInstructions(bgImageInstruction);
                            break;
                        }
                    case BackgroundFill.TileMap:
                        {
                            if (!BackgroundSourceRectangle.HasValue)
                                BackgroundSourceRectangle = BackgroundBounds;

                            for (int x = 0; x < tileWidth; x++)
                                for (int y = 0; y < tileHeight; y++)
                                {
                                    int dx = x == 0 ? 0 : x == tileWidth - 1 ? 2 : 1;
                                    int dy = y == 0 ? 0 : y == tileHeight - 1 ? 2 : 1;
                                    var sWidth = BackgroundSourceRectangle.Value.Width / 3;
                                    var sHeight = BackgroundSourceRectangle.Value.Height / 3;
                                    var sX = BackgroundSourceRectangle.Value.X;
                                    var sY = BackgroundSourceRectangle.Value.Y;
                                    var sRect = new Rectangle(sX + (sWidth * dx), sY + (sHeight * dy), sWidth, sHeight);
                                    var bgImageInstruction = new DrawInstruction(
                                        "bgImage",
                            bgImage,
                            new Rectangle(x * BackgroundBounds.Value.Width, y * BackgroundBounds.Value.Height, BackgroundBounds.Value.Width, BackgroundBounds.Value.Height),
                            sRect,
                            Color.White * BackgroundOpacity, 0f, Vector2.Zero, SpriteEffects.None, 1f);
                                    component.AddDrawInstructions(bgImageInstruction);
                                }
                            break;
                        }
                    case BackgroundFill.Pattern:
                        {
                            for (int x = 0; x < tileWidth; x++)
                                for (int y = 0; y < tileHeight; y++)
                                {
                                    var bgImageInstruction = new DrawInstruction(
                                        "bgImage",
                            bgImage,
                            new Rectangle(drawBounds.Width * x, drawBounds.Height * y, drawBounds.Width, drawBounds.Height),
                            BackgroundSourceRectangle,
                            Color.White * BackgroundOpacity, 0f, Vector2.Zero, SpriteEffects.None, 1f);
                                    component.AddDrawInstructions(bgImageInstruction);
                                }
                            break;
                        }
                }
            }

            if (Image?.Invoke(component) is Texture2D image)
            {
                var imageInstruction = new DrawInstruction(
                    "image",
                            image,
                            new Rectangle(0, 0, component.Bounds.Width, component.Bounds.Height),
                            new Rectangle(0,0, image.Width, image.Height),
                            Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);
                component.AddDrawInstructions(imageInstruction);
            }

            base.Apply(component);
        }

        public override IStyle New(IPlatoHelper helper, string option = "")
        {
            return new BackgroundStyle(helper,option);
        }
       
        public override void Parse(string property, string value, IComponent component)
        {
            switch (property.ToLower())
            {
                case "image":
                    {
                        if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
                        {
                            Image = null;
                            return;
                        }
                        
                        Image = (c) => c.GetWrapper().TryLoadTexture(value, c, out Texture2D texture) ? texture : null;

                        return;
                    }
                case "backgroundimage":
                    {
                        if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
                        {
                            BackgroundImage = null;
                            return;
                        }

                        BackgroundImage = (c) => c.GetWrapper().TryLoadTexture(value, c, out Texture2D texture) ? texture : null;

                        return;
                    }
                case "backgroundmaxscale": 
                    BackgroundMaxScale = float.TryParse(value, out float ms) && ms > 0 ? ms : float.MaxValue;
                    return;
                case "backgroundopacity":
                    BackgroundOpacity = float.Parse(value);
                    return;
                case "backgroundfill":
                    {
                        switch (value.ToLower())
                        {
                            case "stretch": FillStyle = BackgroundFill.Stretch;break;
                            case "original": FillStyle = BackgroundFill.Original; break;
                            case "cover": FillStyle = BackgroundFill.Cover; break;
                            case "contain": FillStyle = BackgroundFill.Contain; break;
                            case "pattern": FillStyle = BackgroundFill.Pattern; break;
                            case "tilemap": FillStyle = BackgroundFill.TileMap; break;
                        }
                        return;
                    }
                case "backgroundcolor":
                    {
                        if (Helper.Content.Textures.TryParseColorFromString(value, out Color color))
                            BackgroundColor = color;

                        return;
                    }
                case "backgroundsize":
                    {
                        int width;
                        int height;
                        int x = 0;
                        int y = 0;
                        if (BackgroundBounds.HasValue)
                        {
                            x = BackgroundBounds.Value.X;
                            y = BackgroundBounds.Value.Y;
                        }

                        string[] parts = value.Replace("px","").Split(' ');
                        if (parts.Length == 0)
                            return;
                        string widthValue = parts[0];
                        string heightValue = parts[0];
                        if (parts.Length > 1)
                            heightValue = parts[1];

                        if (widthValue.EndsWith("%", StringComparison.OrdinalIgnoreCase))
                            width = (int)((float.Parse(widthValue.Substring(0, widthValue.Length - 1).Trim()) / 100f) * component.Bounds.Width);
                        else
                            width = int.Parse(widthValue.Trim());

                        if (heightValue.EndsWith("%", StringComparison.OrdinalIgnoreCase))
                            height = (int)((float.Parse(heightValue.Substring(0, heightValue.Length - 1).Trim()) / 100f) * component.Bounds.Width);
                        else
                            height = int.Parse(heightValue.Trim());

                        BackgroundBounds = new Rectangle(x, y, width, height);

                        return;
                    }
            }
        }
    }
}
