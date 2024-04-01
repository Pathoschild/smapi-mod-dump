/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/HappyHomeDesigner
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Mods;
using System.Linq;

namespace HappyHomeDesigner.Framework
{
	internal class InventoryWatcher
	{
		private static readonly string[] RareCatalogueIDs = 
			{"(F)JunimoCatalogue", "(F)WizardCatalogue", "(F)TrashCatalogue", "(F)JojaCatalogue", "(F)RetroCatalogue"};

		public static void Init(IModHelper helper)
		{
			helper.Events.Player.InventoryChanged += InventoryChanged;
		}

		private static void InventoryChanged(object sender, InventoryChangedEventArgs ev)
		{
			if (Game1.MasterPlayer.hasOrWillReceiveMail(AssetManager.CARD_MAIL))
				return;

			bool rareAdded = false;
			foreach (var item in ev.Added)
			{
				if (item is null || item.Stack <= 0)
					continue;

				if (RareCatalogueIDs.Contains(item.QualifiedItemId))
				{
					rareAdded = true;
					Game1.MasterPlayer.modData[ModEntry.MOD_ID + "_Found_" + item.QualifiedItemId] = "T";
				}
			}

			if (rareAdded && HasAll(Game1.MasterPlayer.modData))
				Game1.addMailForTomorrow(AssetManager.CARD_MAIL, false, true);
		}

		private static bool HasAll(ModDataDictionary data)
		{
			for (int i = 0; i < RareCatalogueIDs.Length; i++)
				if (!data.ContainsKey(ModEntry.MOD_ID + "_Found_" + RareCatalogueIDs[i]))
					return false;

			return true;
		}
	}
}
