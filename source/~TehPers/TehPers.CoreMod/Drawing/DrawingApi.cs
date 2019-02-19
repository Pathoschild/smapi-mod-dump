using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using TehPers.CoreMod.Api;
using TehPers.CoreMod.Api.Conflux.Matching;
using TehPers.CoreMod.Api.Drawing;
using TehPers.CoreMod.Api.Drawing.Sprites;
using TehPers.CoreMod.Api.Structs;
using TehPers.CoreMod.Drawing.Sprites;

namespace TehPers.CoreMod.Drawing {
    internal class DrawingApi : IDrawingApi {
        private static readonly Lazy<Texture2D> _whitePixel = new Lazy<Texture2D>(() => {
            Texture2D whitePixel = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
            whitePixel.SetData(new[] { Color.White });
            return whitePixel;
        });

        private readonly IApiHelper _coreApiHelper;
        private readonly TextureTracker _textureTracker;

        public Texture2D WhitePixel => DrawingApi._whitePixel.Value;
        public ISpriteSheet ObjectSpriteSheet { get; }
        public ISpriteSheet WeaponSpriteSheet { get; }
        public ISpriteSheet CraftableSpriteSheet { get; }
        public IHatSpriteSheet HatSpriteSheet { get; }

        public DrawingApi(IApiHelper coreApiHelper, TextureTracker textureTracker) {
            this._coreApiHelper = coreApiHelper;
            this._textureTracker = textureTracker;

            this.ObjectSpriteSheet = new SpriteSheet(this.GetTrackedTexture(new AssetLocation("Maps/springobjects", ContentSource.GameContent)), 16, 16);
            this.WeaponSpriteSheet = new SpriteSheet(this.GetTrackedTexture(new AssetLocation("TileSheets/weapons", ContentSource.GameContent)), 16, 16);
            this.CraftableSpriteSheet = new SpriteSheet(this.GetTrackedTexture(new AssetLocation("TileSheets/Craftables", ContentSource.GameContent)), 16, 32);
            this.HatSpriteSheet = new HatSpriteSheet(this.GetTrackedTexture(new AssetLocation("Characters/Farmer/hats", ContentSource.GameContent)));
        }

        public ITrackedTexture GetTrackedTexture(AssetLocation asset) {
            return this._textureTracker.GetOrCreateTrackedTexture(asset);
        }

        public ISpriteSheet CreateSimpleSpriteSheet(ITrackedTexture trackedTexture, int spriteWidth, int spriteHeight) {
            string preferredProperty = trackedTexture.CurrentTexture.Match<Texture2D, string>()
                .When(this.ObjectSpriteSheet.TrackedTexture.CurrentTexture, nameof(DrawingApi.ObjectSpriteSheet))
                .When(this.CraftableSpriteSheet.TrackedTexture.CurrentTexture, nameof(DrawingApi.CraftableSpriteSheet))
                .Else((string) null);

            if (preferredProperty != null) {
                this._coreApiHelper.Owner.Monitor.Log($"A new sprite sheet was created for a texture which already has a sprite sheet provided. {nameof(IDrawingInfo)}.{preferredProperty} should be used instead.", LogLevel.Warn);
            }

            return new SpriteSheet(trackedTexture, spriteWidth, spriteHeight);
        }
    }
}