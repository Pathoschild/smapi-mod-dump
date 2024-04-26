/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using StardewValley;
using StardewValley.Buffs;
using StardewValley.Tools;
using SObject = StardewValley.Object;

namespace LazyMod.Framework.Automation;

public class AutoFood : Automate
{
    private readonly ModConfig config;
    private readonly Dictionary<SObject, string?> foodData = new();

    public AutoFood(ModConfig config)
    {
        this.config = config;
    }

    public override void AutoDoFunction(GameLocation? location, Farmer player, Tool? tool, Item? item)
    {
        if (tool is FishingRod fishingRod && (fishingRod.isReeling || fishingRod.isFishing || fishingRod.pullingOutOfWater)) return;

        FindFoodFromInventory(player);
        if (!foodData.Any()) return;

        // 自动吃食物-体力
        if (config.AutoEatFoodForStamina) AutoEatFoodForStamina(player);
        // 自动吃食物-生命值
        if (config.AutoEatFoodForHealth) AutoEatFoodForHealth(player);
        // 自动吃食物-Buff
        if (config.AutoEatBuffFood) AutoEatBuffFood(player);
        // 自动喝饮料-Buff
        if (config.AutoDrinkBuffDrink) AutoDrinkBuffDrink(player);
        
    }

    // 自动吃食物-体力
    private void AutoEatFoodForStamina(Farmer player)
    {
        if (player.Stamina > player.MaxStamina * config.AutoEatFoodStaminaRate) return;

        if (!config.IntelligentFoodSelectionForStamina)
        {
            EatFirstFood(player, foodData.Keys.First());
            return;
        }

        var food = foodData.Keys.OrderBy(food => (food.Price / food.Edibility, -food.Stack)).First();
        EatFirstFood(player, food);
    }

    // 自动吃食物-生命值
    private void AutoEatFoodForHealth(Farmer player)
    {
        if (player.health > player.maxHealth * config.AutoEatFoodHealthRate) return;

        if (!config.IntelligentFoodSelectionForHealth)
        {
            EatFirstFood(player, foodData.Keys.First());
            return;
        }

        var food = foodData.Keys.OrderBy(food => (food.Price / food.Edibility, -food.Stack)).First();
        EatFirstFood(player, food);
    }

    // 自动吃增益食物
    private void AutoEatBuffFood(Farmer player)
    {
        if (player.buffs.AppliedBuffs.Values.Any(buff => buff.id is "food")) return;
        var foodList = GetBuffFoodList("food", config.FoodBuffMaintain1, config.FoodBuffMaintain2);
        if (!foodList.Any()) foodList = GetBuffFoodList("food", config.FoodBuffMaintain1);
        if (!foodList.Any()) foodList = GetBuffFoodList("food", config.FoodBuffMaintain2);
        if (!foodList.Any()) return;
        if (!foodList.Any()) return;
        EatFirstFood(player, foodList.First());
    }
    
    // 自动喝增益饮料
    private void AutoDrinkBuffDrink(Farmer player)
    {
        if (player.buffs.AppliedBuffs.Values.Any(buff => buff.id is "drink")) return;
        var foodList = GetBuffFoodList("drink", config.DrinkBuffMaintain1, config.DrinkBuffMaintain2);
        if (!foodList.Any()) foodList = GetBuffFoodList("drink", config.DrinkBuffMaintain1);
        if (!foodList.Any()) foodList = GetBuffFoodList("drink", config.DrinkBuffMaintain2);
        if (!foodList.Any()) return;
        EatFirstFood(player, foodList.First());
    }
    
    private List<SObject> GetBuffFoodList(string buffId, params BuffType[] buffTypes)
    {
        var foodList = new List<SObject>();
        var buffEffectsData = new Dictionary<BuffType, float>();
        foreach (var (food, foodBuffId) in foodData)
        {
            if (foodBuffId == buffId)
            {
                var buffs = food.GetFoodOrDrinkBuffs().ToList();
                buffEffectsData.Clear();
                foreach (var buff in buffs)
                {
                    InitBuffEffectsData(buffEffectsData, buff.effects);
                    var buffType = buffEffectsData.Keys;
                    if (buffTypes.All(buffType.Contains))
                        foodList.Add(food);
                }
            }
        }
        return foodList;
    }

    private void InitBuffEffectsData(Dictionary<BuffType, float> buffEffectsData, BuffEffects buffEffects)
    {
        buffEffectsData.Clear();
        if (buffEffects.CombatLevel.Value > 0) buffEffectsData.TryAdd(BuffType.Combat, buffEffects.CombatLevel.Value);
        if (buffEffects.FarmingLevel.Value > 0) buffEffectsData.TryAdd(BuffType.Farming, buffEffects.FarmingLevel.Value);
        if (buffEffects.FishingLevel.Value > 0) buffEffectsData.TryAdd(BuffType.Fishing, buffEffects.FishingLevel.Value);
        if (buffEffects.MiningLevel.Value > 0) buffEffectsData.TryAdd(BuffType.Mining, buffEffects.MiningLevel.Value);
        if (buffEffects.LuckLevel.Value > 0) buffEffectsData.TryAdd(BuffType.Luck, buffEffects.LuckLevel.Value);
        if (buffEffects.ForagingLevel.Value > 0) buffEffectsData.TryAdd(BuffType.Foraging, buffEffects.ForagingLevel.Value);
        if (buffEffects.MaxStamina.Value > 0) buffEffectsData.TryAdd(BuffType.MaxStamina, buffEffects.MaxStamina.Value);
        if (buffEffects.MagneticRadius.Value > 0) buffEffectsData.TryAdd(BuffType.MagneticRadius, buffEffects.MagneticRadius.Value);
        if (buffEffects.Speed.Value > 0) buffEffectsData.TryAdd(BuffType.Speed, buffEffects.Speed.Value);
        if (buffEffects.Defense.Value > 0) buffEffectsData.TryAdd(BuffType.Defense, buffEffects.Defense.Value);
        if (buffEffects.Attack.Value > 0) buffEffectsData.TryAdd(BuffType.Attack, buffEffects.Attack.Value);
    }

    private void FindFoodFromInventory(Farmer player)
    {
        foodData.Clear();
        foreach (var item in player.Items)
        {
            if (item is not SObject { Edibility: > 0 } obj) continue;
            var buffs = obj.GetFoodOrDrinkBuffs().ToList();
            if (!buffs.Any())
            {
                foodData.TryAdd(obj, null);
                continue;
            }

            if (buffs.Any(buff => buff.id == "food"))
            {
                foodData.TryAdd(obj, "food");
                continue;
            }

            if (buffs.Any(buff => buff.id == "drink"))
            {
                foodData.TryAdd(obj, "drink");
            }
        }
    }

    private bool CheckFoodOverrideStamina(SObject food)
    {
        return food.GetFoodOrDrinkBuffs().Any(buff => buff.effects.MaxStamina.Value > 0);
    }

    private void EatFirstFood(Farmer player, SObject food)
    {
        if (player.isEating) return;
        var direction = player.FacingDirection;
        player.eatObject(food, CheckFoodOverrideStamina(food));
        player.FacingDirection = direction;
        ConsumeItem(player, food);
    }

    public static BuffType GetBuffType(string name)
    {
        return Enum.TryParse<BuffType>(name, out var buffType) ? buffType : BuffType.None;
    }
}

public enum BuffType
{
    Combat,
    Farming,
    Fishing,
    Mining,
    Luck,
    Foraging,
    MaxStamina,
    MagneticRadius,
    Speed,
    Defense,
    Attack,
    None
}