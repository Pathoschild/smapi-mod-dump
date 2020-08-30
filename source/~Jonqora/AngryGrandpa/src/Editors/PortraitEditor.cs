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

		private readonly bool OverrideEdits;


		/*********
        ** Constructor
        *********/
		/// <summary>
		/// Creates a new editor, specifying whether to make early edits or late (override) edits to portrait assets.
		/// </summary>
		/// <param name="overrideEdits">True for a late editor that will override all Content Patcher changes. False for an early editor whose changes may be overwritten.</param>
		internal PortraitEditor(bool overrideEdits)
		{
			OverrideEdits = overrideEdits;
		}

			
		/*********
        ** Public methods
        *********/
		/// <summary>Get whether this instance can edit the given asset.</summary>
		/// <typeparam name="_T">The asset Type</typeparam>
		/// <param name="asset">Basic metadata about the asset being loaded</param>
		/// <returns>Return true for asset Portraits\Grandpa, false otherwise</returns>
		public bool CanEdit<_T> (IAssetInfo asset)
		{
			// true if "auto" and !OverrideEdits OR if !"auto" and OverrideEdits
			return asset.AssetNameEquals($"Portraits\\Grandpa") &&
				Config.PortraitStyle == ModConfig.PortraitStyleDefault ^ this.OverrideEdits;
		}

		/// <summary>Extend the Portraits\Grandpa asset image size and patch with new portrait expressions.</summary>
		/// <typeparam name="_T">The asset Type</typeparam>
		/// <param name="asset">A helper which encapsulates metadata about an asset and enables changes to it</param>
		public void Edit<_T> (IAssetData asset)
		{
			string filepath = "assets\\Grandpa.png";
			//Determine the filepath for the correct portrait assets
			if (OverrideEdits)
			{
				if (Config.PortraitStyle == "Poltergeister")
				{
					filepath = "assets\\Poltergeister\\Grandpa.png";
					Monitor.LogOnce("Loading Poltergeister-style expressive portraits. This will take priority over other mods that change grandpa's portrait.", LogLevel.Info);
				}
				else if (Config.PortraitStyle == "Vanilla") //Maintain the default
				{
					Monitor.LogOnce("Loading Vanilla-style expressive portraits. This will take priority over other mods that change grandpa's portrait.", LogLevel.Info);
				}
			}
			else //Early edits only, no override
			{
				if (Helper.ModRegistry.IsLoaded("Poltergeister.SlightlyEditedPortraits") ||
					Helper.ModRegistry.IsLoaded("Poltergeister.SeasonalCuteCharacters"))
				{
					filepath = "assets\\Poltergeister\\Grandpa.png";
					Monitor.LogOnce("Compatible portrait mod found! Loading Poltergeister-style expressive portraits to match.", LogLevel.Info); 
				}
				else //No compatible portrait mod found
				{
					Monitor.LogOnce("Loading default Vanilla-style expressive portraits. Other mods that change the grandpa portrait may be able to overwrite these images.", LogLevel.Info);
				}
			}

			Texture2D sourceImage;
			//Load the asset using the indicated filepath
			try
			{
				sourceImage = Helper.Content.Load<Texture2D>(filepath, ContentSource.ModFolder);
			}
			catch //Default back to vanilla in case we can't find the right asset
			{
				Monitor.LogOnce($"Loading grandpa portrait asset at {filepath} failed. Reverting to default mod asset.", LogLevel.Warn);
				sourceImage = Helper.Content.Load<Texture2D>("assets\\Grandpa.png", ContentSource.ModFolder);
			}

			//Extend and patch the game portraits
			var editor = asset.AsImage();
			editor.ExtendImage(minWidth: 128, minHeight: 384);
			editor.PatchImage(sourceImage);
		}
	}
}