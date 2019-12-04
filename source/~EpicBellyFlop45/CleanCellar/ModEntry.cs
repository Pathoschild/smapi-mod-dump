using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;

namespace CleanCellar
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod, IAssetLoader, IAssetEditor
    {
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
        }

        /// <summary>Get whether this instance can edit the given asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public bool CanEdit<T>(IAssetInfo asset)
        {
            // Change the town interior image
            if (asset.AssetNameEquals("townInterior"))
            {
                return true;
            }

            // Change the town interior image
            if (asset.AssetNameEquals("Maps/townInterior"))
            {
                return true;
            }

            return false;
        }

        /// <summary>Edit a matched asset.</summary>
        /// <param name="asset">A helper which encapsulates metadata about an asset and enables changes to it.</param>
        public void Edit<T>(IAssetData asset)
        {
            // Change the town interior image
            if (asset.AssetNameEquals("townInterior"))
            {
                Texture2D townInteriorTexture = this.Helper.Content.Load<Texture2D>("Assets/townInterior.png", ContentSource.ModFolder);
                asset.AsImage().PatchImage(townInteriorTexture, sourceArea: new Rectangle(0, 0, 512, 1088), targetArea: new Rectangle(0, 0, 512, 1088), PatchMode.Overlay);
            }

            // Change the town interior image
            if (asset.AssetNameEquals("Maps/townInterior"))
            {
                Texture2D townInteriorTexture = this.Helper.Content.Load<Texture2D>("Assets/townInterior.png", ContentSource.ModFolder);
                asset.AsImage().PatchImage(townInteriorTexture, sourceArea: new Rectangle(0, 0, 512, 1088), targetArea: new Rectangle(0, 0, 512, 1088), PatchMode.Overlay);
            }
        }

        /// <summary>Get whether this instance can load the initial version of the given asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public bool CanLoad<T>(IAssetInfo asset)
        {
            // Load the new Cellar map
            if (asset.AssetNameEquals("Maps/Cellar"))
            {
                return true;
            }

            return false;
        }

        /// <summary>Load a matched asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public T Load<T>(IAssetInfo asset)
        {
            return this.Helper.Content.Load<T>("Assets/Cellar.tbin", ContentSource.ModFolder);
        }
    }
}
