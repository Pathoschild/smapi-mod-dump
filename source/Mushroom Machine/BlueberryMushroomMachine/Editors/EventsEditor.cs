using System.Collections.Generic;
using StardewValley;
using StardewModdingAPI;

namespace BlueberryMushroomMachine.Editors
{
	class EventsEditor : IAssetEditor
	{
		public bool CanEdit<T>(IAssetInfo asset)
		{
			return asset.AssetNameEquals("Characters\\Dialogue\\Robin")
				|| asset.AssetNameEquals("Data\\Events\\Farm");
		}

		public void Edit<T>(IAssetData asset)
		{
			var data = asset.AsDictionary<string, string>().Data;

			// Event 0000: Robin
			// Pre-Demetrius-event dialogue.
			if (asset.AssetNameEquals("Characters\\Dialogue\\Robin"))
			{
				var json = PropagatorMod.SHelper.Content
					.Load<IDictionary<string, string> >
					(PropagatorData.EventsPath);

				var key = "event.4637.0000.0000";
				data.Add(key, PropagatorMod.i18n.Get(key));
			}

			// Event 0001: Farm, Demetrius
			// Receive Propagator recipe after house upgrade level 3.
			if (asset.AssetNameEquals("Data\\Events\\Farm"))
			{
				var json = PropagatorMod.SHelper.Content
					.Load<IDictionary<string, string> >
					(PropagatorData.EventsPath);

				foreach (var key in json.Keys)
				{
					if (key.StartsWith("46370001"))
					{
						if (Game1.player.HouseUpgradeLevel >= 3)
						{
							if (PropagatorMod.SConfig.DisabledForFruitCave
								&& Game1.player.caveChoice.Value != 2)
								return;

							data.Add(key, string.Format(json[key],
								PropagatorMod.i18n.Get("event.4637.0001.0000"),
								PropagatorMod.i18n.Get("event.4637.0001.0001"),
								PropagatorMod.i18n.Get("event.4637.0001.0002"),
								PropagatorMod.i18n.Get("event.4637.0001.0003")));
						}
					}
				}
			}
		}
	}
}
