using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using TehPers.CoreMod.Api.Conflux.Matching;
using TehPers.CoreMod.Api.Structs;

namespace TehPers.CoreMod.Drawing {
    internal class TextureTracker : IAssetEditor {
        private readonly IMod _owner;
        private readonly Queue<(AssetLocation location, Texture2D texture)> _trackingQueue = new Queue<(AssetLocation location, Texture2D texture)>();

        public TextureTracker(IMod owner) {
            this._owner = owner;

            // Constantly check for any enqueued changes to textures
            owner.Helper.Events.GameLoop.UpdateTicked += (sender, args) => {
                // Make sure the queue is not empty
                if (!this._trackingQueue.Any()) {
                    return;
                }

                // Update the first texture being tracked in the queue
                (AssetLocation location, Texture2D texture) = this._trackingQueue.Dequeue();
                owner.Monitor.Log($"Updating tracked texture: {location}", LogLevel.Trace);
                DrawingDelegator.UpdateTexture(location, texture);
            };
        }

        public bool CanEdit<T>(IAssetInfo asset) {
            // Only track textures
            return typeof(T) == typeof(Texture2D);
        }

        public void Edit<T>(IAssetData asset) {
            // Convert the asset name into a GameAssetLocation
            AssetLocation assetLocation = new AssetLocation(asset.AssetName, ContentSource.GameContent);

            // Queue the texture to be handled later
            this._trackingQueue.Enqueue((assetLocation, asset.GetData<Texture2D>()));
        }

        public TrackedTexture GetOrCreateTrackedTexture(AssetLocation textureLocation) {
            return DrawingDelegator.GetOrCreateTrackedTexture(textureLocation, () => this.GetCurrentTexture(textureLocation));
        }

        private Texture2D GetCurrentTexture(AssetLocation textureLocation) {
            return textureLocation.Source.Match<ContentSource, Texture2D>()
                .When(ContentSource.GameContent, () => Game1.content.Load<Texture2D>(textureLocation.Path))
                .When(ContentSource.ModFolder, () => this._owner.Helper.Content.Load<Texture2D>(textureLocation.Path))
                .ElseThrow();
        }
    }
}
