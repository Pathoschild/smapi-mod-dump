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
using System;
using System.Collections.Generic;
using System.Linq;

namespace HappyHomeDesigner.Framework
{
	internal class InventoryWatcher
	{
		private static readonly string[] RareCatalogueIDs = 
			["(F)JunimoCatalogue", "(F)WizardCatalogue", "(F)TrashCatalogue", "(F)JojaCatalogue", "(F)RetroCatalogue"];

		public static void Init(IModHelper helper)
		{
			helper.Events.Player.InventoryChanged += InventoryChanged;
			helper.Events.GameLoop.SaveLoaded += SaveLoaded;
		}

		private static void SaveLoaded(object sender, SaveLoadedEventArgs e)
		{
			// already unlocked, skip checks
			if (Game1.MasterPlayer.hasOrWillReceiveMail(AssetManager.CARD_MAIL))
				return;

			// scan for rare catalogues in inventories
			var required = new List<string>(RareCatalogueIDs);
			Utility.ForEachItem((item) => {
				FindRareCatalogue(item, required);
				return required.Count is not 0;
			});

			// scan for catalogues in world
			if (required.Count is not 0)
			{
				Utility.ForEachLocation((where) => {
					foreach(var furn in where.furniture)
					{
						if (required.Remove(furn.QualifiedItemId))
						{
							Game1.MasterPlayer.modData[ModEntry.MOD_ID + "_Found_" + furn.QualifiedItemId] = "T";
							if (required.Count is 0)
								return false;
						}
					}
					return true;
				});
			}

			// validate
			if (required.Count is 0 || HasAll(Game1.MasterPlayer.modData))
				Game1.addMailForTomorrow(AssetManager.CARD_MAIL, false, true);
		}

		private static void InventoryChanged(object sender, InventoryChangedEventArgs ev)
		{
			if (Game1.MasterPlayer.hasOrWillReceiveMail(AssetManager.CARD_MAIL))
			{
				// if the player has unlocked and obtained the deluxe catalogue, mail them about fairy dust
				if (
					!Game1.MasterPlayer.hasOrWillReceiveMail(AssetManager.FAIRY_MAIL) &&
					ev.Added.Where(i => 
						i is not null && i.Stack > 0 && 
						i.QualifiedItemId is "(F)" + AssetManager.DELUXE_ID
					).Any()
				)
				{
					Game1.addMailForTomorrow(AssetManager.FAIRY_MAIL, false, true);
				}

				return;
			}

			bool rareAdded = false;
			foreach (var item in ev.Added)
				rareAdded = FindRareCatalogue(item);

			if (rareAdded && HasAll(Game1.MasterPlayer.modData))
				Game1.addMailForTomorrow(AssetManager.CARD_MAIL, false, true);
		}

		private static bool FindRareCatalogue(Item item, IList<string> cache = null)
		{
			if (item is not null && item.Stack > 0)
			{
				if (
					(cache is not null && cache.Remove(item.QualifiedItemId)) ||
					(cache is null && RareCatalogueIDs.Contains(item.QualifiedItemId))
				){
					Game1.MasterPlayer.modData[ModEntry.MOD_ID + "_Found_" + item.QualifiedItemId] = "T";
					return true;
				}
			}
			return false;
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
