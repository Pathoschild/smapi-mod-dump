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
using StardewValley.Objects;
using SObject = StardewValley.Object;

namespace Tocseoj.Stardew.BigCropBonus;

internal class EatingBonus(IMonitor Monitor, IManifest ModManifest, IModHelper Helper, ModConfig Config)
	: ModComponent(Monitor, ModManifest, Helper, Config)
{
	// UniqueMultiplayerID -> player.isEating
	private readonly Dictionary<long, bool> wasEatingLastTick = [];
	private readonly Dictionary<long, ValueTuple<int, int>> newTotalEatValues = [];

	public void AteItem(Item item, Farmer player, int stack)
	{
		if (item is not SObject food) return;
		Monitor.Log($"Eating {food.DisplayName}.");
		BigCropList bigCrops = new();

		if (!bigCrops.HasBigCropOf(food, out string cropId)) return;

		Monitor.Log($"Ate {food.DisplayName} which matches a giant crop {cropId}.");
		float modifier = Config.EatModifier * bigCrops.GetCount(cropId);

		if (stack != 1) Monitor.Log($"{player.Name} ate {stack} {item.DisplayName}(s), which isn't supported by BigCropBonus. Giving bonus health/stamina for just 1.", LogLevel.Warn);

		int bonusStaminaToHeal = (int)Math.Ceiling(food.staminaRecoveredOnConsumption() * modifier);
		int bonusHealthToHeal = (int)Math.Ceiling(food.healthRecoveredOnConsumption() * modifier);

		// update player's health and stamina
		player.Stamina = Math.Min(player.MaxStamina, player.Stamina + bonusStaminaToHeal);
		player.health = Math.Min(player.maxHealth, player.health + bonusHealthToHeal);

		int totalGainedStamina = food.staminaRecoveredOnConsumption() + bonusStaminaToHeal;
		int totalGainedHealth = food.healthRecoveredOnConsumption() + bonusHealthToHeal;

		newTotalEatValues[player.UniqueMultiplayerID] = (totalGainedStamina, totalGainedHealth);
	}
}