/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Tocseoj/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.Objects;
using StardewValley.Inventories;
using StardewValley.Menus;
using StardewValley.Objects;
using SObject = StardewValley.Object;

namespace Tocseoj.Stardew.BigCropBonus;

internal class SellingBonus(IMonitor Monitor, IManifest ModManifest, IModHelper Helper, ModConfig Config)
	: ModComponent(Monitor, ModManifest, Helper, Config)
{
	// QualifiedItemId -> ObjectData
	private Dictionary<string, ObjectData> bonusItemsToCreate = [];

  public void InsertTemporaryBonusItems(AssetRequestedEventArgs e)
	{
		if (e.NameWithoutLocale.IsEquivalentTo("Data/Objects")) {
			Monitor.Log("Retrieving Data/Objects asset for bonus item insertion.");
			if (bonusItemsToCreate.Count == 0) return;

			e.Edit(asset => {
				var objects = asset.AsDictionary<string, ObjectData>().Data;
				foreach ((string key, ObjectData data) in bonusItemsToCreate) {
					objects[key] = data;
				}
			}, AssetEditPriority.Default);
		}
	}

	public void ReceiveBonusItemsGenerated(Dictionary<string, ObjectData> bonusItems)
	{
		if (Game1.player.IsMainPlayer) return;
		Monitor.Log("Received bonus items from main player.");
		bonusItemsToCreate = bonusItems;
		Helper.GameContent.InvalidateCache("Data/Objects");
	}

	public void ClearTemporaryBonusItems()
	{
		bonusItemsToCreate.Clear();
		Helper.GameContent.InvalidateCache("Data/Objects");
	}

	public void CalculateAndShipBonusItems()
	{
		if (!Game1.player.IsMainPlayer && !Game1.player.useSeparateWallets) return;

		BigCropList bigCrops = new();
		if (bigCrops.Count == 0) return;
		Monitor.Log($"Found {bigCrops.Count} big crops.");

		// TODO : Test what happens if you can demolish main bin
		Inventory primaryBin = (Inventory)Game1.getFarm().getShippingBin(Game1.player);
		List<Inventory> myShippingBins = GetShippingBins(Game1.player);
		// Combo id -> (object w/bonus, stack size, bonus value)
		Dictionary<string, ValueTuple<SObject, int, float>> bonusItems = [];

		foreach (Inventory bin in myShippingBins) {
			foreach (SObject item in bin.OfType<SObject>()) {
				if (!bigCrops.HasBigCropOf(item, out string cropId)) return;
				string comboIdentifier = $"Q{item.Quality}_{item.QualifiedItemId}";
				float modifier = Config.SellModifier * bigCrops.GetCount(cropId);
				int stack = bin.ReduceId(item.QualifiedItemId, item.Stack);
				if (bonusItems.ContainsKey(comboIdentifier)) {
					(_, int existingStack, _) = bonusItems[comboIdentifier];
					bonusItems[comboIdentifier] = (item, existingStack + stack, modifier);
				} else {
					bonusItems[comboIdentifier] = (item, stack, modifier);
				}
			}
			bin.RemoveEmptySlots();
		}
		Monitor.Log($"Found {bonusItems.Count} items with bonus.");

    // Add bonus items to object data
    foreach ((string comboId, (SObject item, _, float modifier)) in bonusItems) {
			string generatedItemId = $"Tocseoj.BigCropBonus_{comboId}";
			ObjectData generatedObjectData = new() {
				Name = $"{item.Name} Bonus",
				DisplayName = $"{Math.Round(modifier * 100)}% big crop bonus",
				Description = "Your bonus for having a Giant Crop.",
				Type = item.Type,
				Category = item.Category,
				Price = (int)Math.Ceiling(item.Price * modifier),
				Texture = null,
				SpriteIndex = 26,
			};
			bonusItemsToCreate[generatedItemId] = generatedObjectData;
			// If shared wallet only host will calculate what the bonus items are
			// So we need to send the bonus items to other players so they aren't Error items
			if (Game1.player.IsMainPlayer && !Game1.player.useSeparateWallets)
				Helper.Multiplayer.SendMessage(bonusItemsToCreate, "Tocseoj.Stardew.BigCropBonus.BonusItemsGenerated", modIDs: [ModManifest.UniqueID]);
		}
		Helper.GameContent.InvalidateCache("Data/Objects");

  	// Add items to shipping bins
    foreach ((string comboId, (SObject item, int stack, _)) in bonusItems) {
			string generatedItemId = $"Tocseoj.BigCropBonus_{comboId}";
			item.Stack = stack; // set stack back to original value
			SObject bonus = new(generatedItemId, stack, false, -1, item.Quality);
			Monitor.Log($"Adding {item.DisplayName}(x{stack}) and {bonus.DisplayName}(x{stack}) to shipping bin");
			primaryBin.Add(item);
			primaryBin.Add(bonus);
		}
	}

	private static List<Inventory> GetShippingBins(Farmer player)
	{
		List<Inventory> shippingBins = [];
		Utility.ForEachLocation(location => {
			if (location is Farm farm) {
				shippingBins.Add((Inventory)farm.getShippingBin(player));
			}
			// get all mini shipping bins
			foreach (SObject objects in location.objects.Values) {
				if (objects is Chest chest && chest.SpecialChestType == Chest.SpecialChestTypes.MiniShippingBin) {
					if (player.useSeparateWallets && chest.separateWalletItems.TryGetValue(player.UniqueMultiplayerID, out Inventory items)) {
						shippingBins.Add(items);
					} else {
						shippingBins.Add(chest.Items); // TODO : test if this works with shared wallets
					}
				}
			}
			return true;
		});
		return shippingBins;
	}

	private void SoldItem(Farmer player, Item item, int count)
	{
		if (Game1.activeClickableMenu is ShopMenu shopMenu) {
			if (item is SObject validItem) {
				BigCropList bigCrops = new();
				if (!bigCrops.HasBigCropOf(validItem, out string cropId)) return;
				float modifier = Config.SellModifier * bigCrops.GetCount(cropId);

				int bonusMoney = (int)Math.Ceiling(validItem.Price * modifier * count);
				ShopMenu.chargePlayer(player, shopMenu.currency, -bonusMoney);

				Monitor.Log($"Sold {count} {validItem.DisplayName}(s) for {validItem.sellToStorePrice()}g ea. which matches a giant crop {cropId}. Total bonus: {bonusMoney}g");
			}
		}
	}
}