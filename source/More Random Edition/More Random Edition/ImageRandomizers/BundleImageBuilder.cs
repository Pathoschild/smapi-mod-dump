/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Randomizer
{
	public class BundleImageBuilder : ImageBuilder
	{
		/// <summary>
		/// A map of the bundle position in the dictionary to the id it belongs to
		/// </summary>
		private Dictionary<SpriteOverlayData, Bundle> OverlayDataToBundlesMap;

		/// <summary>
		/// The list of bundle image names - without the file extention
		/// </summary>
		private readonly List<string> BundleImageNames;

		public BundleImageBuilder() : base()
		{
            Rng = RNG.GetFarmRNG(nameof(BundleImageBuilder));
			GlobalStardewAssetPath = "LooseSprites/JunimoNote";
            SubDirectory = "Bundles";

			ImageHeightInPx = 32;
			ImageWidthInPx = 32;
			OffsetWidthInPx = 32;
			OffsetHeightInPx = 32;
			InitialHeightOffetInPx = 180;

			SetUpPointsToBundlesMap();
			OverlayData = OverlayDataToBundlesMap.Keys.ToList();

			BundleImageNames = Directory.GetFiles($"{ImageDirectory}")
				.Where(x => x.EndsWith(".png"))
				.Select(x => Path.GetFileNameWithoutExtension(x))
				.OrderBy(x => x).ToList();
			ValidateImages();
		}

		/// <summary>
		/// Sets up the map to link bundle points to their IDs
		/// </summary>
		private void SetUpPointsToBundlesMap()
		{
			int itemsPerRow = GetItemsPerRow();
			OverlayDataToBundlesMap = new Dictionary<SpriteOverlayData, Bundle>();
			foreach (RoomInformation room in BundleRandomizer.Rooms)
			{
				foreach (Bundle bundle in room.Bundles)
				{
					var overlayData = new SpriteOverlayData(
						GlobalStardewAssetPath, 
						x: bundle.Id % itemsPerRow, 
						y: bundle.Id / itemsPerRow);
					OverlayDataToBundlesMap[overlayData] = bundle;
				}
			}
		}

		/// <summary>
		/// Gets a random file name that matches the bundle type at the given position
		/// Will remove the name found from the list
		/// </summary>
		/// <param name="overlayData">The overlay data</param>
		/// <returns>The selected file name</returns>
		protected override string GetRandomFileName(SpriteOverlayData overlayData)
		{
			Bundle bundle = OverlayDataToBundlesMap[overlayData];
			return Path.Combine(ImageDirectory, $"{bundle.ImageName}.png");
		}

		/// <summary>
		/// Whether the settings premit random bundle images
		/// </summary>
		/// <returns>True if so, false otherwise</returns>
		public override bool ShouldSaveImage()
		{
			return Globals.Config.Bundles.Randomize;
		}

		/// <summary>
		/// Whether the settings premit random bundle images
		/// </summary>
		/// <returns>True if so, false otherwise</returns>
		protected override bool ShouldSaveImage(SpriteOverlayData overlayData)
		{
			if (!Globals.Config.Bundles.Randomize) { return false; }

			Bundle bundle = OverlayDataToBundlesMap[overlayData];

			if (BundleImageNames.Contains(bundle.ImageName))
			{
				return true;
			}

			Globals.ConsoleWarn($"Could not find bundle image: {ImageDirectory}/{bundle.ImageName}.png");
			return false;
		}

		/// <summary>
		/// Validates that all the potentially needed bundle images exist
		/// </summary>
		private void ValidateImages()
		{
			foreach (BundleTypes bundleType in Enum.GetValues(typeof(BundleTypes)))
			{
				string bundleString = bundleType.ToString();

				if (bundleType == BundleTypes.None)
				{
					return;
				}
				else if (bundleString.StartsWith("Vault"))
				{
					int maxSuffix = 6;
					if (bundleString == "Vault2500" || bundleString == "Vault25000")
					{
						maxSuffix = 7;
					}

					for (int bundleSuffix = 1; bundleSuffix <= maxSuffix; bundleSuffix++)
					{
						ValidateImage($"{bundleString}-{bundleSuffix}");
					}
				}

				else if (bundleType == BundleTypes.AllLetter)
				{
					string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
					foreach (char letter in letters)
					{
						ValidateImage($"{bundleString}{letter}");
					}
				}

				else
				{
					ValidateImage(bundleString);
				}
			}
		}

		/// <summary>
		/// Validates that the image exists given the name
		/// </summary>
		private void ValidateImage(string fileName)
		{
			if (!BundleImageNames.Contains(fileName))
			{
				Globals.ConsoleWarn($"Could not validate bundle image: {ImageDirectory}/{fileName}.png");
			}
		}
	}
}
