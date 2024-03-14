/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sonozuki/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;

namespace CleanCellar
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod, IAssetLoader, IAssetEditor
    {
        /// <summary>The mod entry point.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper) { }

        /// <summary>Get whether this instance can edit the given asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("townInterior") || asset.AssetNameEquals("Maps/townInterior");
        }

        /// <summary>Edit a matched asset.</summary>
        /// <param name="asset">A helper which encapsulates metadata about an asset and enables changes to it.</param>
        public void Edit<T>(IAssetData asset)
        {
            Texture2D townInteriorTexture = this.Helper.Content.Load<Texture2D>("assets/townInterior.png", ContentSource.ModFolder);
            asset.AsImage().PatchImage(townInteriorTexture, sourceArea: new Rectangle(0, 0, 512, 1088), targetArea: new Rectangle(0, 0, 512, 1088), PatchMode.Overlay);
        }

        /// <summary>Get whether this instance can load the initial version of the given asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public bool CanLoad<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("Maps/Cellar");
        }

        /// <summary>Load a matched asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public T Load<T>(IAssetInfo asset)
        {
            return this.Helper.Content.Load<T>("assets/Cellar.tbin", ContentSource.ModFolder);
        }
    }
}
