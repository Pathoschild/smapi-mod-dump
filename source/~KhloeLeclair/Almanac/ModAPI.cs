/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Leclair.Stardew.Common;

using StardewModdingAPI;

using StardewValley;

namespace Leclair.Stardew.Almanac {

	public interface IAlmanacAPI {
		void AddCropProvider(ICropProvider provider);

		void RemoveCropProvider(ICropProvider provider);

		void SetCropPriority(IManifest manifest, int priority);

		void SetCropCallback(IManifest manifest, Action action);
		void ClearCropCallback(IManifest manifest);

		void AddCrop(
			IManifest manifest,

			string id,

			Item item,
			string name,

			bool isTrellisCrop,
			bool isGiantCrop,
			bool isPaddyCrop,

			IList<int> phases,
			IList<Texture2D> phaseSpriteTextures,
			IList<Rectangle?> phaseSpriteSources,
			IList<Color?> phaseSpriteColors,
			IList<Texture2D> phaseSpriteOverlayTextures,
			IList<Rectangle?> phaseSpriteOverlaySources,
			IList<Color?> phaseSpriteOverlayColors,

			int regrow,

			WorldDate start,
			WorldDate end
		);

		void AddCrop(
			IManifest manifest,

			string id,

			Item item,
			string name,

			Texture2D spriteTexture,
			Rectangle? spriteSource,
			Color? spriteColor,
			Texture2D spriteOverlayTexture,
			Rectangle? spriteOverlaySource,
			Color? spriteOverlayColor,

			bool isTrellisCrop,
			bool isGiantCrop,
			bool isPaddyCrop,

			IList<int> phases,
			IList<Texture2D> phaseSpriteTextures,
			IList<Rectangle?> phaseSpriteSources,
			IList<Color?> phaseSpriteColors,
			IList<Texture2D> phaseSpriteOverlayTextures,
			IList<Rectangle?> phaseSpriteOverlaySources,
			IList<Color?> phaseSpriteOverlayColors,

			int regrow,

			WorldDate start,
			WorldDate end
		);

		void AddCrop(
			IManifest manifest,

			string id,

			Item item,
			string name,
			SpriteInfo sprite,

			bool isTrellisCrop,
			bool isGiantCrop,
			bool isPaddyCrop,

			IEnumerable<int> phases,
			IEnumerable<SpriteInfo> phaseSprites,

			int regrow,

			WorldDate start,
			WorldDate end
		);

		void RemoveCrop(IManifest manifest, string id);

		void ClearCrops(IManifest manifest);

		List<CropInfo> GetSeasonCrops(int season);

		List<CropInfo> GetSeasonCrops(string season);

		void InvalidateCrops();
	}

	public class ModAPI : IAlmanacAPI {
		private readonly ModEntry Mod;

		public ModAPI(ModEntry mod) {
			Mod = mod;
		}

		#region Crop Providers

		public void AddCropProvider(ICropProvider provider) {
			Mod.Crops.AddProvider(provider);
		}

		public void RemoveCropProvider(ICropProvider provider) {
			Mod.Crops.RemoveProvider(provider);
		}

		#endregion

		#region Manual Mod Crops

		public void SetCropPriority(IManifest manifest, int priority) {
			var provider = Mod.Crops.GetModProvider(manifest, priority != 0);
			if (provider != null && provider.Priority != priority) {
				provider.Priority = priority;
				Mod.Crops.SortProviders();
			}
		}

		public void SetCropCallback(IManifest manifest, Action action) {
			var provider = Mod.Crops.GetModProvider(manifest, action != null);
			if (provider != null)
				provider.SetCallback(action);
		}

		public void ClearCropCallback(IManifest manifest) {
			SetCropCallback(manifest, null);
		}

		public void AddCrop(
			IManifest manifest,

			string id,

			Item item,
			string name,

			bool isTrellisCrop,
			bool isGiantCrop,
			bool isPaddyCrop,

			IList<int> phases,
			IList<Texture2D> phaseSpriteTextures,
			IList<Rectangle?> phaseSpriteSources,
			IList<Color?> phaseSpriteColors,
			IList<Texture2D> phaseSpriteOverlayTextures,
			IList<Rectangle?> phaseSpriteOverlaySources,
			IList<Color?> phaseSpriteOverlayColors,

			int regrow,

			WorldDate start,
			WorldDate end
		) {
			List<SpriteInfo> phaseSprites = new();

			for(int i = 0; i < phases.Count; i++) {
				phaseSprites.Add(new(
					texture: phaseSpriteTextures[i],
					baseSource: phaseSpriteSources[i] ?? phaseSpriteTextures[i].Bounds,
					baseColor: phaseSpriteColors?[i],
					overlayTexture: phaseSpriteOverlayTextures?[i],
					overlaySource: phaseSpriteOverlaySources?[i],
					overlayColor: phaseSpriteOverlayColors?[i]
				));
			}

			AddCrop(
				manifest: manifest,
				id: id,
				item: item,
				name: name,
				sprite: item == null ? null : SpriteHelper.GetSprite(item, Mod.Helper),
				isTrellisCrop: isTrellisCrop,
				isGiantCrop: isGiantCrop,
				isPaddyCrop: isPaddyCrop,
				phases: phases,
				phaseSprites: phaseSprites,
				regrow: regrow,
				start: start,
				end: end
			);
		}

		public void AddCrop(
			IManifest manifest,

			string id,

			Item item,
			string name,

			Texture2D spriteTexture,
			Rectangle? spriteSource,
			Color? spriteColor,
			Texture2D spriteOverlayTexture,
			Rectangle? spriteOverlaySource,
			Color? spriteOverlayColor,

			bool isTrellisCrop,
			bool isGiantCrop,
			bool isPaddyCrop,

			IList<int> phases,
			IList<Texture2D> phaseSpriteTextures,
			IList<Rectangle?> phaseSpriteSources,
			IList<Color?> phaseSpriteColors,
			IList<Texture2D> phaseSpriteOverlayTextures,
			IList<Rectangle?> phaseSpriteOverlaySources,
			IList<Color?> phaseSpriteOverlayColors,

			int regrow,

			WorldDate start,
			WorldDate end
		) {
			List<SpriteInfo> phaseSprites = new();

			for (int i = 0; i < phases.Count; i++) {
				phaseSprites.Add(new(
					texture: phaseSpriteTextures[i],
					baseSource: phaseSpriteSources[i] ?? phaseSpriteTextures[i].Bounds,
					baseColor: phaseSpriteColors?[i],
					overlayTexture: phaseSpriteOverlayTextures?[i],
					overlaySource: phaseSpriteOverlaySources?[i],
					overlayColor: phaseSpriteOverlayColors?[i]
				));
			}

			AddCrop(
				manifest: manifest,
				id: id,
				item: item,
				name: name,
				sprite: new SpriteInfo(
					spriteTexture,
					spriteSource ?? spriteTexture.Bounds,
					spriteColor,
					overlayTexture: spriteOverlayTexture,
					overlaySource: spriteOverlaySource,
					overlayColor: spriteOverlayColor
				),
				isTrellisCrop: isTrellisCrop,
				isGiantCrop: isGiantCrop,
				isPaddyCrop: isPaddyCrop,
				phases: phases,
				phaseSprites: phaseSprites,
				regrow: regrow,
				start: start,
				end: end
			);
		}

		public void AddCrop(
			IManifest manifest,

			string id,

			Item item,
			string name,
			SpriteInfo sprite,

			bool isTrellisCrop,
			bool isGiantCrop,
			bool isPaddyCrop,

			IEnumerable<int> phases,
			IEnumerable<SpriteInfo> phaseSprites,

			int regrow,

			WorldDate start,
			WorldDate end
		) {
			var provider = Mod.Crops.GetModProvider(manifest);
			provider.AddCrop(
				id: id,
				item: item,
				name: name,
				sprite: sprite,
				isTrellisCrop: isTrellisCrop,
				isGiantCrop: isGiantCrop,
				isPaddyCrop: isPaddyCrop,
				phases: phases,
				phaseSprites: phaseSprites,
				regrow: regrow,
				start: start,
				end: end
			);
		}

		public void RemoveCrop(IManifest manifest, string id) {
			var provider = Mod.Crops.GetModProvider(manifest, false);
			if (provider != null)
				provider.RemoveCrop(id);
		}

		public void ClearCrops(IManifest manifest) {
			var provider = Mod.Crops.GetModProvider(manifest, false);
			if (provider != null)
				provider.ClearCrops();
		}

		#endregion

		#region Get Crops

		public List<CropInfo> GetSeasonCrops(int season) {
			return Mod.Crops.GetSeasonCrops(season);
		}

		public List<CropInfo> GetSeasonCrops(string season) {
			return Mod.Crops.GetSeasonCrops(season);
		}

		#endregion

		public void InvalidateCrops() {
			Mod.Crops.Invalidate();
		}

	}
}
