/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System;
using System.Reflection;
using BirbCore.Attributes;
using HarmonyLib;
using StardewValley;
using StardewValley.Tools;

namespace RanchingToolUpgrades;

[HarmonyPatch(typeof(MilkPail), nameof(MilkPail.DoFunction))]
class MilkPail_DoFunction
{
    public static void Prefix(Farmer who, MilkPail __instance)
    {
        try
        {
            FarmAnimal animal = ModEntry.Instance.Helper.Reflection.GetField<FarmAnimal>(__instance, "animal").GetValue();
            RanchToolUtility.GetExtraEffects(animal, __instance, who);
        }
        catch (Exception e)
        {
            Log.Error($"Failed in {MethodBase.GetCurrentMethod()?.DeclaringType}\n{e}");
        }

    }
}

[HarmonyPatch(typeof(Shears), nameof(Shears.DoFunction))]
class Shears_DoFunction
{
    public static void Prefix(Farmer who, Shears __instance)
    {
        try
        {
            FarmAnimal animal = ModEntry.Instance.Helper.Reflection.GetField<FarmAnimal>(__instance, "animal").GetValue();
            RanchToolUtility.GetExtraEffects(animal, __instance, who);
        }
        catch (Exception e)
        {
            Log.Error($"Failed in {MethodBase.GetCurrentMethod()?.DeclaringType}\n{e}");
        }

    }
}

class RanchToolUtility
{
    public static void GetExtraEffects(FarmAnimal animal, Tool tool, Farmer farmer)
    {
        if (animal?.currentProduce.Value == null || !animal.isAdult() || !animal.CanGetProduceWithTool(tool))
        {
            return;
        }

        // do extra friendship effect
        int extraFriendship = ModEntry.Config.ExtraFriendshipBase * tool.UpgradeLevel;
        animal.friendshipTowardFarmer.Value = Math.Min(1000, animal.friendshipTowardFarmer.Value + extraFriendship);
        Log.Debug($"Applied extra friendship {extraFriendship}.  Total friendship: {animal.friendshipTowardFarmer.Value}");

        // do quality bump effect
        float higherQualityChance = ModEntry.Config.QualityBumpChanceBase * tool.UpgradeLevel;
        if (higherQualityChance > Game1.random.NextDouble())
        {
            switch (animal.produceQuality.Value)
            {
                case 0:
                    animal.produceQuality.Set(1);
                    break;
                case 1:
                    animal.produceQuality.Set(2);
                    break;
                case 2:
                    animal.produceQuality.Set(4);
                    break;
            }
            Log.Debug($"Quality Bump Chance {higherQualityChance}, succeeded.  New quality {animal.produceQuality.Value}");
        }
        else
        {
            Log.Debug($"Quality Bump Chance {higherQualityChance} failed.");
        }

        // do extra produce effect
        int extraProduce = 0;
        for (int i = 0; i < tool.UpgradeLevel; i++)
        {
            if (ModEntry.Config.ExtraProduceChance > Game1.random.NextDouble())
            {
                extraProduce++;
            }
        }
        Log.Debug($"Extra Produce Chance {ModEntry.Config.ExtraProduceChance} generated {extraProduce} additional produce from {tool.UpgradeLevel} draws.");
        if (extraProduce <= 0)
        {
            return;
        }

        SObject produce = ItemRegistry.Create<SObject>("(O)" + animal.currentProduce.Value, extraProduce, animal.produceQuality.Value);
        produce.CanBeSetDown = false;
        farmer.addItemToInventory(produce);
    }
}
