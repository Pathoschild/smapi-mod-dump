/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/MessyCrops
**
*************************************************/

using StardewModdingAPI;
using System;

namespace MessyCrops
{
	internal class Config
	{
		public int Amount { get; set; } = 3;
		public bool ApplyToTrellis { get; set; } = false;

		public void ResetToDefault()
		{
			Amount = 3;
			ApplyToTrellis = false;
		}
		public void ApplyConfig()
		{
			ModEntry.helper.WriteConfig(this);
		}
		public void RegisterConfig(IManifest manifest)
		{
			if (!ModEntry.helper.ModRegistry.IsLoaded("spacechase0.GenericModConfigMenu"))
				return;

			var api = ModEntry.helper.ModRegistry.GetApi<IGMCMAPI>("spacechase0.GenericModConfigMenu");

			api.Register(manifest, ResetToDefault, ApplyConfig, true);
			api.AddNumberOption(manifest,
				() => Amount,
				(s) => Amount = s,
				() => ModEntry.i18n.Get("config.offset.name"),
				() => ModEntry.i18n.Get("config.offset.desc"),
				0, 7
			);
			api.AddBoolOption(manifest,
				() => ApplyToTrellis,
				(s) => ApplyToTrellis = s,
				() => ModEntry.i18n.Get("config.applyTrellis.name"),
				() => ModEntry.i18n.Get("config.applyTrellis.desc")
			);
		}
	}
}
