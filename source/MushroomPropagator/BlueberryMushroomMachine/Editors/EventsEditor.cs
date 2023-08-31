/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/BlueberryMushroomMachine
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System.Collections.Generic;

namespace BlueberryMushroomMachine.Editors
{
	internal static class EventsEditor
	{
		public static bool ApplyEdit(AssetRequestedEventArgs e)
		{
			if (e.NameWithoutLocale.IsEquivalentTo(@"Characters/Dialogue/Robin"))
			{
				e.Edit(apply: (IAssetData asset) =>
				{
					IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
					const string key = "event.4637.0000.0000";
					if (!data.ContainsKey(key))
					{
						data.Add(key, ModEntry.I18n.Get(key));
					}
				});
				return true;
			}
			else if (e.NameWithoutLocale.IsEquivalentTo(@"Data/Events/Farm"))
			{
				e.Edit(apply: EventsEditor.ApplyEventEdit);
				return true;
			}
			return false;
		}

		public static void ApplyEventEdit(IAssetData asset)
		{
			IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
			IDictionary<string, string> json = ModEntry.Instance.Helper.ModContent.Load
				<IDictionary<string, string>>
				(ModValues.EventsPath);

			foreach (string key in json.Keys)
			{
				if (key.StartsWith(ModValues.EventId.ToString()))
				{
					if (Game1.player.HouseUpgradeLevel >= 3)
					{
						Log.D("Event conditions:" +
							  $" disabled=[{ModEntry.Config.DisabledForFruitCave}]" +
							  $" caveChoice=[{Game1.MasterPlayer.caveChoice}]",
							ModEntry.Config.DebugMode);

						if (ModEntry.Config.DisabledForFruitCave && Game1.MasterPlayer.caveChoice.Value != Farmer.caveMushrooms)
						{
							return;
						}

						if (!data.ContainsKey(key))
						{
							string value = string.Format(
								json[key],
								ModEntry.I18n.Get("event.4637.0001.0000"),
								ModEntry.I18n.Get("event.4637.0001.0001"),
								ModEntry.I18n.Get("event.4637.0001.0002"),
								ModEntry.I18n.Get("event.4637.0001.0003"),
								ModValues.PropagatorInternalName);
							Log.D($"Injecting event.",
								ModEntry.Config.DebugMode);
							data.Add(key, value);
						}
					}
				}
			}
		}
	}
}
