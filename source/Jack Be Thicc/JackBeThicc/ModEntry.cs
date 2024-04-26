/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DeadRobotDev/StardewMods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace JackBeThicc;

public class ModEntry : Mod
{
	public override void Entry(IModHelper helper)
	{
		helper.Events.Content.AssetRequested += ContentOnAssetRequested;
	}

	private static void ContentOnAssetRequested(object? sender, AssetRequestedEventArgs e)
	{
		if (e.NameWithoutLocale.IsEquivalentTo("Strings/1_6_Strings"))
		{
			e.Edit(asset =>
			{
				var data = asset.AsDictionary<string, string>().Data;
				data["Book_Defense_Name"] = "Jack Be Nimble, Jack Be Thicc";
			});
		}
	}
}
