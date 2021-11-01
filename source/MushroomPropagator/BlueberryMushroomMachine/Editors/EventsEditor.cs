/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/BlueberryMushroomMachine
**
*************************************************/

using System.Collections.Generic;
using StardewValley;
using StardewModdingAPI;

namespace BlueberryMushroomMachine.Editors
{
	internal class EventsEditor : IAssetEditor
	{
		private readonly bool _isDebugging;

		public EventsEditor()
		{
			_isDebugging = ModEntry.Instance.Config.DebugMode;
		}

		public bool CanEdit<T>(IAssetInfo asset)
		{
			return asset.AssetNameEquals(@"Characters/Dialogue/Robin")
				|| asset.AssetNameEquals(@"Data/Events/Farm");
		}

		public void Edit<T>(IAssetData asset)
		{
			Log.T($"Editing {asset.AssetName}.",
				_isDebugging);

			var data = asset.AsDictionary<string, string>().Data;

			// Event 0000: Robin
			// Pre-Demetrius-event dialogue
			if (asset.AssetNameEquals(@"Characters/Dialogue/Robin"))
			{
				const string key = "event.4637.0000.0000";
				if (!data.ContainsKey(key))
					data.Add(key, ModEntry.Instance.i18n.Get(key));
			}

			// Event 0001: Farm, Demetrius
			// Receive Propagator recipe after house upgrade level 3
			if (asset.AssetNameEquals(@"Data/Events/Farm"))
			{
				var json = ModEntry.Instance.Helper.Content.Load<IDictionary<string, string>>
					(ModValues.EventsPath);
				foreach (var key in json.Keys)
				{
					if (key.StartsWith("46370001"))
					{
						if (Game1.player.HouseUpgradeLevel >= 3)
						{
							Log.D("Event conditions:" +
							      $" disabled=[{ModEntry.Instance.Config.DisabledForFruitCave}]" +
							      $" caveChoice=[{Game1.MasterPlayer.caveChoice}]",
								_isDebugging);
							if (ModEntry.Instance.Config.DisabledForFruitCave
								&& Game1.MasterPlayer.caveChoice.Value != 2)
								return;

							if (!data.ContainsKey(key))
							{
								var value = string.Format(
									json[key], 
									ModEntry.Instance.i18n.Get("event.4637.0001.0000"),
									ModEntry.Instance.i18n.Get("event.4637.0001.0001"),
									ModEntry.Instance.i18n.Get("event.4637.0001.0002"),
									ModEntry.Instance.i18n.Get("event.4637.0001.0003"),
									ModValues.PropagatorInternalName);
								Log.D($"Injecting event.",
									_isDebugging);
								data.Add(key, value);
							}
						}
					}
				}
			}
		}
	}
}
