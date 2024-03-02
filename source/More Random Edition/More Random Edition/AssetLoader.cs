/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;
using System.Collections.Generic;

namespace Randomizer
{
    public class AssetLoader
	{
		private readonly ModEntry _mod;

		private readonly Dictionary<string, string> _customAssetReplacements = new();
        private readonly Dictionary<string, Texture2D> _editedAssetReplacements = new();

        /// <summary>Constructor</summary>
        /// <param name="mod">A reference to the ModEntry</param>
        public AssetLoader(ModEntry mod)
		{
			_mod = mod;
		}

        /// <summary>
        /// When an asset is requested, execute the approriate patcher's code, or replace
		/// the value from our dictionary
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void OnAssetRequested(object sender, AssetRequestedEventArgs e)
		{
            if (e.NameWithoutLocale.IsEquivalentTo(RainPatcher.StardewAssetPath))
            {
                e.Edit(new RainPatcher().OnAssetRequested);
            }
            else if (e.Name.IsEquivalentTo(AnimalIconPatcher.StardewAssetPath))
            {
                e.Edit(new AnimalIconPatcher().OnAssetRequested);
            }
            else if (e.NameWithoutLocale.IsEquivalentTo(TitleScreenPatcher.StardewAssetPath))
            {
                e.Edit(new TitleScreenPatcher().OnAssetRequested);
            }

			// Files that come from our own images: we're replacing an xnb asset with one on our filesystem
            else if (_customAssetReplacements.TryGetValue(e.Name.BaseName, out string customAsset))
            {
                e.LoadFromModFile<Texture2D>(customAsset, AssetLoadPriority.Medium);
            } 

			// Files that we have in memory: we're replacing an xnb asset with a Texture2D object
			else if (_editedAssetReplacements.TryGetValue(e.Name.BaseName, out Texture2D editedAsset))
			{
				e.Edit(asset =>
				{
					var editor = asset.AsImage();
					editor.PatchImage(editedAsset);
                });
            }
        }

		/// <summary>
		/// Adds a replacement to our internal dictionary
		/// </summary>
		/// <param name="originalAsset">The original asset</param>
		/// <param name="replacementAsset">The asset to replace it with</param>
		private void AddReplacement(string originalAsset, string replacementAsset)
		{
			IAssetName normalizedAssetName = _mod.Helper.GameContent.ParseAssetName(originalAsset);
			_customAssetReplacements[normalizedAssetName.BaseName] = replacementAsset;
		}

        /// <summary>
        /// Adds a replacement to our internal dictionary
        /// </summary>
        /// <param name="originalAsset">The original asset</param>
        /// <param name="replacementAsset">The asset to replace it with</param>
        private void AddReplacement(string originalAsset, Texture2D replacementAsset)
        {
            IAssetName normalizedAssetName = _mod.Helper.GameContent.ParseAssetName(originalAsset);
            _editedAssetReplacements[normalizedAssetName.BaseName] = replacementAsset;
        }

        /// <summary>
        /// Adds a set of replacements to our internal dictionary for assets coming from our own files
        /// </summary>
        /// <param name="replacements">Key: the original asset; Value: the asset to replace it with</param>
        private void AddCustomAssetReplacements(Dictionary<string, string> replacements)
		{
			foreach (string key in replacements.Keys)
			{
				AddReplacement(key, replacements[key]);
			}
		}

        /// <summary>
        /// Invalidate all replaced assets so that the changes are reapplied
        /// </summary>
        public void InvalidateCache()
		{
			foreach (string assetName in _customAssetReplacements.Keys)
			{
				_mod.Helper.GameContent.InvalidateCache(assetName);
			}
            ReplaceCatIcon();
        }

        /// <summary>
        /// Replace the assets on the title screen - includes the title screen menu
        /// and the new game menu
        /// </summary>
        public void ReplaceTitleScreenAssets()
		{
			ReplaceTitleScreen();
			ReplaceCatIcon();
        }

		/// <summary>
		/// Replaces the cat icon on the new game and the pause menu if pets are randomized
		/// Otherwise, restore the icon
		/// </summary>
		private void ReplaceCatIcon()
		{
            _mod.Helper.GameContent.InvalidateCache(AnimalIconPatcher.StardewAssetPath);
        }

		/// <summary>
		/// Replaces the title screen graphics and refreshes the settings UI page
		/// </summary>
		private void ReplaceTitleScreen()
		{
            _mod.Helper.GameContent.InvalidateCache(TitleScreenPatcher.StardewAssetPath);
        }

        /// <summary>
        /// Replaces the rain - intended to be called once per day start
        /// <param name="sender">The event sender</param>
        /// <param name="e">The event arguments</param>
        /// </summary>
        public void ReplaceRain()
        {
            if (Globals.Config.RandomizeRain)
            {
                _mod.Helper.GameContent.InvalidateCache(RainPatcher.StardewAssetPath);
            }
        }

        /// <summary>Asset replacements to load when the farm is loaded</summary>
        public void CalculateReplacements()
		{
			_customAssetReplacements.Clear();
			AddCustomAssetReplacements(AnimalSkinRandomizer.Randomize());
		}

		/// <summary>
		/// Randomizes the images - depending on what settings are on
		/// It's still important to build the images to make sure seeds are consistent
        /// 
        /// Note that the cache is invalidated already when the save file is loaded
        /// See ModEntry.CalculateAllReplacements
		/// </summary>
		public void RandomizeImages()
		{
            _editedAssetReplacements.Clear();

            CropGrowthImageBuilder cropGrowthImageBuilder = new();

            HandleImageReplacement(new WeaponImageBuilder());
            HandleImageReplacement(cropGrowthImageBuilder);
            HandleImageReplacement(new SpringObjectsImageBuilder(cropGrowthImageBuilder.CropIdsToLinkingData));
            HandleImageReplacement(new BundleImageBuilder());

            Globals.SpoilerWrite("==== ANIMALS ====");
            HandleImageReplacement(new AnimalRandomizer(AnimalTypes.Horses));
            HandleImageReplacement(new AnimalRandomizer(AnimalTypes.Pets));
            Globals.SpoilerWrite("");

            MonsterHueShifter.GetHueShiftedMonsterAssets().ForEach(monsterData =>
                AddReplacement(monsterData.StardewAssetPath, monsterData.MonsterImage));
        }

		/// <summary>
		/// Adds the image builder's modified asset to the dictionary
        /// Replace the localized version - our cache invalidator will invalidate it and the base one
		/// </summary>
		/// <param name="imageBuilder">The image builder</param>
		private void HandleImageReplacement(ImageBuilder imageBuilder)
		{
            AddReplacement(
                Globals.GetLocalizedFileName(imageBuilder.StardewAssetPath), 
                imageBuilder.GenerateModifiedAsset());

            // Unclear on the best way to do this - but invalidate the cache both for
            // The current locale, and the localized one, since not all assets
            // have translations for all locales
            _mod.Helper.GameContent.InvalidateCache
                (Globals.GetLocalizedFileName(imageBuilder.StardewAssetPath));
            _mod.Helper.GameContent.InvalidateCache(imageBuilder.StardewAssetPath);
        }
	}
}