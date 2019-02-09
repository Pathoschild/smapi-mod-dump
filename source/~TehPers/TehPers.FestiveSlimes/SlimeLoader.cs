using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using TehPers.CoreMod.Api.Environment;
using TehPers.CoreMod.Api.Structs;

namespace TehPers.FestiveSlimes {
    internal class SlimeLoader : IAssetLoader {
        private readonly IMod _mod;
        private readonly string _slimeName;

        public SlimeLoader(IMod mod, string slimeName) {
            this._mod = mod;
            this._slimeName = slimeName;
        }

        public bool CanLoad<T>(IAssetInfo asset) {
            return SDateTime.Today.Season == Season.Fall && asset.AssetNameEquals($@"Characters\Monsters\{this._slimeName}");
        }

        public T Load<T>(IAssetInfo asset) {
            return (T) (object) this._mod.Helper.Content.Load<Texture2D>($@"assets\{this._slimeName}\textures\fall.png");
        }
    }
}
