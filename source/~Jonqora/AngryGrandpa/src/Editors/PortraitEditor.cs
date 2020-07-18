using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;

namespace AngryGrandpa
{
	/// <summary>The class for editing Portrait image assets.</summary>
	internal class PortraitEditor : IAssetEditor
	{
		/*********
        ** Public methods
        *********/
		protected static IModHelper Helper => ModEntry.Instance.Helper;
		protected static IMonitor Monitor => ModEntry.Instance.Monitor;
		protected static ModConfig Config => ModConfig.Instance;


		/*********
        ** Fields
        *********/
		protected static ITranslationHelper i18n = Helper.Translation;


		/*********
        ** Public methods
        *********/
		/// <summary>Get whether this instance can edit the given asset.</summary>
		/// <typeparam name="_T">The asset Type</typeparam>
		/// <param name="asset">Basic metadata about the asset being loaded</param>
		/// <returns>Return true for asset Portraits\Grandpa, false otherwise</returns>
		public bool CanEdit<_T> (IAssetInfo asset)
		{
			return asset.AssetNameEquals($"Portraits\\Grandpa");
		}

		/// <summary>Extend the Portraits\Grandpa asset image size and patch with new portrait expressions.</summary>
		/// <typeparam name="_T">The asset Type</typeparam>
		/// <param name="asset">A helper which encapsulates metadata about an asset and enables changes to it</param>
		public void Edit<_T> (IAssetData asset)
		{
			var editor = asset.AsImage();
			Texture2D sourceImage = Helper.Content.Load<Texture2D>("assets\\Grandpa.png", ContentSource.ModFolder);

			editor.ExtendImage(minWidth: 128, minHeight: 384);
			editor.PatchImage(sourceImage);
		}
	}
}