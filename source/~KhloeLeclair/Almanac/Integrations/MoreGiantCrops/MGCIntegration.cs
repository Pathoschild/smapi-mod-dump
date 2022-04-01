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

using Microsoft.Xna.Framework.Graphics;

using Netcode;
using StardewModdingAPI;

using Leclair.Stardew.Common.Integrations;

namespace Leclair.Stardew.Almanac.Integrations.MoreGiantCrops {
	public class MGCIntegration : BaseIntegration<ModEntry> {

		private readonly Type MGCEntry;
		private readonly Type CropPatcher;

		public MGCIntegration(ModEntry mod)
		: base(mod, "spacechase0.MoreGiantCrops", "1.1.0") {

			if (!IsLoaded)
				return;

			try {
				MGCEntry = Type.GetType("MoreGiantCrops.Mod, MoreGiantCrops");
				CropPatcher = Type.GetType("MoreGiantCrops.Patches.CropPatcher, MoreGiantCrops");
				if (CropPatcher == null)
					throw new ArgumentNullException("CropPatcher");

			} catch (Exception) {
				Log($"Unable to find CropPatcher. Will not be able to determine if MoreGiantCrops has made a crop giant.", LogLevel.Debug);
			}
		}

		public bool IsGiantCrop(int id) {
			// Boxing is fun~
			return IsGiantCrop(new NetInt(id));
		}

		public bool IsGiantCrop(NetInt id) {
			if (!IsLoaded || CropPatcher == null)
				return false;

			var method = Self.Helper.Reflection.GetMethod(CropPatcher, "CheckCanBeGiant", false);
			if (method == null)
				return false;

			try {
				return method.Invoke<bool>(id);
			} catch (Exception ex) {
				Log($"Error calling CheckCanBeGiant.", LogLevel.Trace, ex);
			}

			return false;
		}

		public Texture2D GetGiantCropTexture(int id) {
			if (!IsLoaded || MGCEntry == null)
				return null;

			var dict = Self.Helper.Reflection.GetField<Dictionary<int, Texture2D>>(MGCEntry, "Sprites", false)?.GetValue();
			if (dict == null || !dict.TryGetValue(id, out Texture2D result))
				return null;

			return result;
		}
	}
}
