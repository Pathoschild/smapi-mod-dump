/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/Archery
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using static StardewValley.FarmerSprite;

namespace Archery.Framework.Interfaces
{
    public interface IFashionSenseApi
    {
        public enum Type
        {
            Unknown,
            Hair,
            Accessory,
            [Obsolete("No longer maintained. Use Accessory instead.")]
            AccessorySecondary,
            [Obsolete("No longer maintained. Use Accessory instead.")]
            AccessoryTertiary,
            Hat,
            Shirt,
            Pants,
            Sleeves,
            Shoes,
            Player
        }

        event EventHandler SetSpriteDirtyTriggered;

        public interface IDrawTool
        {
            public Farmer Farmer { get; init; }
            public SpriteBatch SpriteBatch { get; init; }
            public FarmerRenderer FarmerRenderer { get; init; }
            public Texture2D BaseTexture { get; init; }
            public Rectangle FarmerSourceRectangle { get; init; }
            public AnimationFrame AnimationFrame { get; init; }
            public bool IsDrawingForUI { get; init; }
            public Color OverrideColor { get; init; }
            public Color AppearanceColor { get; set; }
            public Vector2 Position { get; init; }
            public Vector2 Origin { get; init; }
            public Vector2 PositionOffset { get; init; }
            public int FacingDirection { get; init; }
            public int CurrentFrame { get; init; }
            public float Scale { get; init; }
            public float Rotation { get; init; }
            public float LayerDepthSnapshot { get; set; }
        }
        KeyValuePair<bool, string> RegisterAppearanceDrawOverride(Type appearanceType, IManifest callerManifest, Func<IDrawTool, bool> appearanceDrawOverride);
        KeyValuePair<bool, string> UnregisterAppearanceDrawOverride(Type appearanceType, IManifest callerManifest);
        KeyValuePair<bool, string> IsDrawOverrideActive(Type appearanceType, IManifest callerManifest);

        KeyValuePair<bool, Color> GetAppearanceColor(Type appearanceType, Farmer target = null);
    }
}
