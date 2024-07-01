/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buffs;
using StardewValley.Tools;
using weizinai.StardewValleyMod.LazyMod.Framework.Config;
using SObject = StardewValley.Object;

namespace weizinai.StardewValleyMod.LazyMod.Automation;

internal class AutoFood : Automate
{
    private readonly Dictionary<SObject, string?> foodData = new();

    public AutoFood(ModConfig config, Func<int, List<Vector2>> getTileGrid) : base(config, getTileGrid)
    {
    }

    public override void AutoDoFunction(GameLocation location, Farmer player, Tool? tool, Item? item)
    {
        if (tool is FishingRod && player.UsingTool) return;

        this.FindFoodFromInventory(player);
        if (!this.foodData.Any()) return;

        // 自动吃食物-体力
        if (this.Config.AutoEatFoodForStamina) this.AutoEatFoodForStamina(player);
        // 自动吃食物-生命值
        if (this.Config.AutoEatFoodForHealth) this.AutoEatFoodForHealth(player);
        // 自动吃食物-Buff
        if (this.Config.AutoEatBuffFood) this.AutoEatBuffFood(player);
        // 自动喝饮料-Buff
        if (this.Config.AutoDrinkBuffDrink) this.AutoDrinkBuffDrink(player);
    }

    // 自动吃食物-体力
    private void AutoEatFoodForStamina(Farmer player)
    {
        if (player.Stamina > player.MaxStamina * this.Config.AutoEatFoodStaminaRate) return;

        if (!this.Config.IntelligentFoodSelectionForStamina)
        {
            this.EatFirstFood(player, this.foodData.Keys.First());
            return;
        }

        if (this.foodData.Count > 1)
        {
            var allFood = this.foodData.Keys.OrderBy(food => (-food.Stack, food.Price / food.Edibility)).ToList();
            if (allFood[0].Stack - allFood[1].Stack > this.Config.RedundantStaminaFoodCount)
            {
                this.EatFirstFood(player, allFood[0]);
                return;
            }
        }

        var food = this.foodData.Keys.OrderBy(food => (food.Price / food.Edibility, -food.Stack)).First();
        this.EatFirstFood(player, food);
    }

    // 自动吃食物-生命值
    private void AutoEatFoodForHealth(Farmer player)
    {
        if (player.health > player.maxHealth * this.Config.AutoEatFoodHealthRate) return;

        if (!this.Config.IntelligentFoodSelectionForHealth)
        {
            this.EatFirstFood(player, this.foodData.Keys.First());
            return;
        }

        if (this.foodData.Count > 1)
        {
            var allFood = this.foodData.Keys.OrderBy(food => (-food.Stack, food.Price / food.Edibility)).ToList();
            if (allFood[0].Stack - allFood[1].Stack > this.Config.RedundantHealthFoodCount)
            {
                this.EatFirstFood(player, allFood[0]);
                return;
            }
        }

        var food = this.foodData.Keys.OrderBy(food => (food.Price / food.Edibility, -food.Stack)).First();
        this.EatFirstFood(player, food);
    }

    // 自动吃增益食物
    private void AutoEatBuffFood(Farmer player)
    {
        if (player.buffs.AppliedBuffs.Values.Any(buff => buff.id is "food")) return;
        var foodList = this.GetBuffFoodList("food", this.Config.FoodBuffMaintain1, this.Config.FoodBuffMaintain2);
        if (!foodList.Any()) foodList = this.GetBuffFoodList("food", this.Config.FoodBuffMaintain1);
        if (!foodList.Any()) foodList = this.GetBuffFoodList("food", this.Config.FoodBuffMaintain2);
        if (!foodList.Any()) return;
        this.EatFirstFood(player, foodList.First());
    }

    // 自动喝增益饮料
    private void AutoDrinkBuffDrink(Farmer player)
    {
        if (player.buffs.AppliedBuffs.Values.Any(buff => buff.id is "drink")) return;
        var foodList = this.GetBuffFoodList("drink", this.Config.DrinkBuffMaintain1, this.Config.DrinkBuffMaintain2);
        if (!foodList.Any()) foodList = this.GetBuffFoodList("drink", this.Config.DrinkBuffMaintain1);
        if (!foodList.Any()) foodList = this.GetBuffFoodList("drink", this.Config.DrinkBuffMaintain2);
        if (!foodList.Any()) return;
        this.EatFirstFood(player, foodList.First());
    }

    private List<SObject> GetBuffFoodList(string buffId, params BuffType[] buffTypes)
    {
        var foodList = new List<SObject>();
        var buffEffectsData = new Dictionary<BuffType, float>();
        foreach (var (food, foodBuffId) in this.foodData)
        {
            if (foodBuffId == buffId)
            {
                var buffs = food.GetFoodOrDrinkBuffs().ToList();
                buffEffectsData.Clear();
                foreach (var buff in buffs)
                {
                    this.InitBuffEffectsData(buffEffectsData, buff.effects);
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
        this.foodData.Clear();
        foreach (var item in player.Items)
        {
            if (item is not SObject { Edibility: > 0 } obj) continue;
            var buffs = obj.GetFoodOrDrinkBuffs().ToList();
            if (!buffs.Any())
            {
                this.foodData.TryAdd(obj, null);
                continue;
            }

            if (buffs.Any(buff => buff.id == "food"))
            {
                this.foodData.TryAdd(obj, "food");
                continue;
            }

            if (buffs.Any(buff => buff.id == "drink"))
            {
                this.foodData.TryAdd(obj, "drink");
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
        player.eatObject(food, this.CheckFoodOverrideStamina(food));
        player.FacingDirection = direction;
        this.ConsumeItem(player, food);
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