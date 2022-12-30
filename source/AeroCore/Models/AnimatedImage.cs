/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Tlitookilakin/AeroCore
**
*************************************************/

using AeroCore.Generics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System;
using System.Collections.Generic;

namespace AeroCore.Models
{
    public class AnimatedImage
    {
        public Animation Animation { get; set; }
        public string Path
        {
            get => path;
            set
            {
                if (path != value)
                    texture.Reload();
                path = value;
            }
        }
        public Rectangle Region
        {
            get => region;
            set
            {
                region = value;
                if (Animation is not null)
                    Animation.Region = value;
            }
        }

        private string path;
        private Rectangle region;
        private LazyAsset<Texture2D> texture;
        private bool ignoreLocale = false;
        private IModHelper modHelper = null;

        public AnimatedImage(IModHelper helper = null, bool IgnoreLocale = true)
        {
            SetMeta(helper, IgnoreLocale);
        }
        public void SetMeta(IModHelper helper = null, bool IgnoreLocale = true)
        {
            if (ignoreLocale == IgnoreLocale || helper is null || helper == modHelper)
                return;

            helper ??= modHelper ?? ModEntry.helper;
            modHelper = helper;
            ignoreLocale = IgnoreLocale;
            texture = new(helper, () => path, ignoreLocale);
        }
        public void Animate(int millis) => Animation?.Animate(millis);
        public (Rectangle, Texture2D) GetDrawable()
            => Animation is null ? (Region, texture.Value) : (Animation.FrameRegion, texture.Value);
        public void Draw(SpriteBatch b, Rectangle area, Color? tint = null)
            => b.Draw(texture.Value, area, Animation is null ? Region : Animation.FrameRegion, tint ?? Color.White);
    }
}
