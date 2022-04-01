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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework.Graphics;

using Netcode;
using StardewModdingAPI;

using Leclair.Stardew.Common.Integrations;

using JsonAssets;

namespace Leclair.Stardew.Almanac.Integrations.JsonAssets {
	public class JAIntegration : BaseAPIIntegration<IJSONAssetsAPI, ModEntry> {

		private readonly Type JAEntry;
		private readonly Type CropPatcher;

		private readonly object JAMod;

		private readonly IReflectedMethod _ResolveObjectId;
		//private readonly IReflectedMethod _CheckCanBeGiant;

		public JAIntegration(ModEntry mod)
		: base(mod, "spacechase0.JsonAssets", "1.10.0") {

			if (!IsLoaded)
				return;

			try {
				JAEntry = Type.GetType("JsonAssets.Mod, JsonAssets");
				if (JAEntry == null)
					throw new ArgumentNullException("JAEntry");

				JAMod = Self.Helper.Reflection.GetField<object>(JAEntry, "instance", false)?.GetValue();
				if (JAMod == null)
					throw new ArgumentNullException("JAMod");

				_ResolveObjectId = Self.Helper.Reflection.GetMethod(JAMod, "ResolveObjectId", false);

			} catch(Exception) {
				Log($"Unable to find JsonAssets. Will not be able to fetch giant crop sprites.");
			}

			try { 
				CropPatcher = Type.GetType("JsonAssets.Patches.CropPatcher, JsonAssets");
				if (CropPatcher == null)
					throw new ArgumentNullException("CropPatcher");

				//_CheckCanBeGiant = Self.Helper.Reflection.GetMethod(CropPatcher, "CheckCanBeGiant", false);

			} catch(Exception) {
				Log($"Unable to find CropPatcher. Will not be able to determine if JsonAssets crops are giant.", LogLevel.Debug);
			}
		}

		public bool IsGiantCrop(int id) {
			// Boxing is fun~
			return GetGiantCropTexture(id) != null;
		}

		private int ResolveObjectId(object obj) {
			if (!IsLoaded || _ResolveObjectId == null)
				return 0;

			try {
				return _ResolveObjectId.Invoke<int>(obj);
			} catch(Exception ex) {
				Log($"Error calling ResolveObjectId.", LogLevel.Trace, ex);
			}

			return 0;
		}

		private IList GetCrops() {
			if (!IsLoaded || JAMod == null)
				return null;

			return Self.Helper.Reflection.GetField<IList>(
				JAMod, "Crops", false)?.GetValue();
		}

		public Texture2D GetGiantCropTexture(int id) {
			IList crops = GetCrops();
			if (crops == null || crops.Count == 0)
				return null;

			foreach(object crop in crops) {
				if (crop == null)
					continue;

				object product = Self.Helper.Reflection.GetProperty<object>(crop, "Product", false)?.GetValue();
				if (product == null)
					continue;

				int crop_id = ResolveObjectId(product);
				if (id == crop_id)
					try {
						return Self.Helper.Reflection.GetProperty<Texture2D>(
							crop, "GiantTexture", false)?.GetValue();
					} catch (Exception ex) {
						Log($"Unable to get giant texture for crop {id}", LogLevel.Warn, ex);
						return null;
					}
			}

			return null;
		}

	}
}
