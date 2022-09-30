/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/bwdy/SDVModding
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;

namespace MinecartPatcher
{
	internal class AssetLoader : IAssetLoader
	{
		public bool CanLoad<T>(IAssetInfo asset) => asset.Name.IsEquivalentTo("MinecartPatcher.Minecarts");
		public T Load<T>(IAssetInfo asset)
		{
			return (T)(object)new Dictionary<string, MinecartInstance>()
			{
				{"minecartpatcher.busstop", new MinecartInstance() { VanillaPassthrough = "Minecart_Bus", DisplayName = Game1.content.LoadString("Strings\\Locations:MineCart_Destination_BusStop"), LocationName = "BusStop", LandingPointX = 4, LandingPointY = 4, LandingPointDirection = 2, IsUnderground = false, MailCondition = null }},
				{"minecartpatcher.town", new MinecartInstance() { VanillaPassthrough = "Minecart_Town", DisplayName = Game1.content.LoadString("Strings\\Locations:MineCart_Destination_Town"), LocationName = "Town", LandingPointX = 105, LandingPointY = 80, LandingPointDirection = 1, IsUnderground = false, MailCondition = null }},
				{"minecartpatcher.mines", new MinecartInstance() { VanillaPassthrough = "Minecart_Mines", DisplayName = Game1.content.LoadString("Strings\\Locations:MineCart_Destination_Mines"), LocationName = "Mine", LandingPointX = 13, LandingPointY = 9, LandingPointDirection = 1, IsUnderground = true, MailCondition = null }},
				{"minecartpatcher.quarry", new MinecartInstance() { VanillaPassthrough = "Minecart_Quarry", DisplayName = Game1.content.LoadString("Strings\\Locations:MineCart_Destination_Quarry"), LocationName = "Mountain", LandingPointX = 124, LandingPointY = 12, LandingPointDirection = 2, IsUnderground = false, MailCondition = "ccCraftsRoom" }},
			};
		}
	}
}
