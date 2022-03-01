/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

// ReSharper disable PossibleLossOfFraction
namespace DaLion.Stardew.Professions.Framework.Extensions;

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Enums;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using StardewValley.Monsters;

using Common.Extensions;
using Events.GameLoop;
using SuperMode;
using Utility;

using SObject = StardewValley.Object;

#endregion using directives

internal static class FarmerExtensions
{
    /// <summary>Whether the farmer has a particular profession.</summary>
    /// <param name="profession">The index of the profession.</param>
    public static bool HasProfession(this Farmer farmer, Profession profession, bool prestiged = false)
    {
        return farmer.professions.Contains((int) profession + (prestiged ? 100 : 0));
    }

    /// <summary>Whether the farmer has any of the specified professions.</summary>
    /// <param name="professionNames">Sequence of profession names.</param>
    public static bool HasAnyOfProfessions(this Farmer farmer, params string[] professionNames)
    {
        return professionNames.Any(name =>
            Enum.TryParse<Profession>(name, out var profession) &&
            farmer.professions.Contains((int) profession));
    }

    /// <summary>Whether the farmer has any of the specified professions.</summary>
    /// <param name="professionNames">Sequence of profession names.</param>
    /// <param name="firstMatch">The first profession acquired by the player among the specified profession names.</param>
    public static bool HasAnyOfProfessions(this Farmer farmer, string[] professionNames, out string firstMatch)
    {
        firstMatch = professionNames.FirstOrDefault(name =>
            Enum.TryParse<Profession>(name, out var profession) &&
            farmer.professions.Contains((int) profession));
        return firstMatch is not null;
    }

    /// <summary>Whether the farmer has acquired any other professions branching from the specified profession.</summary>
    /// <param name="professionIndex">A profession index (0 to 29).</param>
    /// <param name="otherProfessions">
    ///     An array of acquired professions in the same branch as
    ///     <paramref name="professionIndex">, excluding itself.
    /// </param>
    public static bool HasOtherProfessionsInBranch(this Farmer farmer, int professionIndex,
        out int[] otherProfessions)
    {
        var otherProfessionsInBranch = (professionIndex % 6) switch
        {
            0 => new[] {professionIndex + 1},
            1 => new[] {professionIndex - 1},
            2 => new[] {professionIndex + 1, professionIndex + 2, professionIndex + 3},
            3 => new[] {professionIndex - 1, professionIndex + 1, professionIndex + 2},
            4 => new[] {professionIndex - 2, professionIndex - 1, professionIndex + 1},
            5 => new[] {professionIndex - 3, professionIndex - 2, professionIndex - 1},
            _ => Array.Empty<int>()
        };

        otherProfessions = farmer.professions.Intersect(otherProfessionsInBranch).ToArray();
        return otherProfessions.Any();
    }

    /// <summary>Whether the farmer has acquired all professions branching from the specified profession.</summary>
    /// <param name="professionIndex">A profession index (0 to 29).</param>
    public static bool HasAllProfessionsInBranch(this Farmer farmer, int professionIndex)
    {
        return professionIndex % 6 == 0 && farmer.professions.Contains(professionIndex + 2) &&
               farmer.professions.Contains(professionIndex + 3) ||
               professionIndex % 6 == 1 && farmer.professions.Contains(professionIndex + 3) &&
               farmer.professions.Contains(professionIndex + 4) ||
               professionIndex % 6 > 1;
    }

    /// <summary>Whether the farmer has all six professions in the specified skill.</summary>
    public static bool HasAllProfessionsInSkill(this Farmer farmer, int which)
    {
        return farmer.NumberOfProfessionsInSkill(which) == 6;
    }

    /// <summary>Whether the farmer has all 30 vanilla professions.</summary>
    public static bool HasAllProfessions(this Farmer farmer)
    {
        var allProfessions = Enumerable.Range(0, 30).ToList();
        return farmer.professions.Intersect(allProfessions).Count() == allProfessions.Count();
    }

    /// <summary>Get the last 1st-tier profession acquired by the farmer in the specified skill.</summary>
    /// <param name="skill">The skill index.</param>
    /// <returns>The last acquired profession, or -1 if none was found.</returns>
    public static int GetCurrentBranchForSkill(this Farmer farmer, int skill)
    {
        var lastIndex = farmer.professions.ToList().FindLastIndex(p => p == skill * 6 || p == skill * 6 + 1);
        return lastIndex >= 0
            ? farmer.professions[lastIndex]
            : lastIndex;
    }

    /// <summary>Get the last level 2nd-tier profession acquired by the farmer in the specified skill branch.</summary>
    /// <param name="branch">The branch (level 5 profession) index.</param>
    /// <returns>The last acquired profession, or -1 if none was found.</returns>
    public static int GetCurrentProfessionForBranch(this Farmer farmer, int branch)
    {
        var lastIndex = farmer.professions.ToList().FindLastIndex(p => branch % 6 == 0
            ? p == branch + 2 || p == branch + 3
            : p == branch + 3 || p == branch + 4);
        return lastIndex >= 0
            ? farmer.professions[lastIndex]
            : lastIndex;
    }

    /// <summary>Get all the farmer's professions associated with a specific skill.</summary>
    /// <param name="which">The skill index.</param>
    /// <param name="excludeTierOneProfessions">Whether to exclude level 5 professions from the result.</param>
    public static IEnumerable<int> GetAllProfessionsForSkill(this Farmer farmer, int which,
        bool excludeTierOneProfessions = false)
    {
        return farmer.professions.Intersect(excludeTierOneProfessions
            ? Enumerable.Range(which * 6 + 2, 4)
            : Enumerable.Range(which * 6, 6));
    }

    /// <summary>Count the number of professions acquired by the player in the specified skill.</summary>
    /// <param name="which">The skill index.</param>
    /// <param name="excludeTierOneProfessions">Whether to exclude level 5 professions from the count.</param>
    public static int NumberOfProfessionsInSkill(this Farmer farmer, int which,
        bool excludeTierOneProfessions = false)
    {
        return excludeTierOneProfessions
            ? farmer.professions.Count(p => p / 6 == which && p % 6 > 1)
            : farmer.professions.Count(p => p / 6 == which);
    }

    /// <summary>Whether the farmer can reset the specified skill for prestige.</summary>
    /// <param name="skillType">A skill index (0 to 4).</param>
    public static bool CanResetSkill(this Farmer farmer, SkillType skillType)
    {
        var isSkillLevelTen = farmer.GetUnmodifiedSkillLevel((int) skillType) == 10;
        var justLeveledUp = farmer.newLevels.Contains(new((int) skillType, 10));
        var hasAtLeastOneButNotAllProfessionsInSkill =
            farmer.NumberOfProfessionsInSkill((int) skillType, true) is > 0 and < 4;
        var alreadyResetThisSkill =
            EventManager.TryGet<PrestigeDayEndingEvent>(out var prestigeDayEnding) &&
            prestigeDayEnding.SkillsToReset.Value.Contains(skillType);

        return isSkillLevelTen && !justLeveledUp && hasAtLeastOneButNotAllProfessionsInSkill &&
               !alreadyResetThisSkill;
    }

    /// <summary>Whether the farmer can reset any skill for prestige.</summary>
    public static bool CanResetAnySkill(this Farmer farmer)
    {
        return Enum.GetValues<SkillType>().Any(farmer.CanResetSkill);
    }

    /// <summary>Get the cost of resetting the specified skill.</summary>
    /// <param name="skillType">The desired skill.</param>
    public static int GetResetCost(this Farmer farmer, SkillType skillType)
    {
        var multiplier = ModEntry.Config.SkillResetCostMultiplier;
        if (multiplier <= 0f) return 0;

        var count = farmer.NumberOfProfessionsInSkill((int) skillType, true);
#pragma warning disable 8509
        var baseCost = count switch
#pragma warning restore 8509
        {
            1 => 10000,
            2 => 50000,
            3 => 100000
        };

        return (int) (baseCost * multiplier);
    }

    /// <summary>Resets a specific skill level, removing all associated recipes and bonuses but maintaining profession perks.</summary>
    /// <param name="skillType">The skill to reset.</param>
    public static void ResetSkill(this Farmer farmer, SkillType skillType)
    {
        // reset skill level
        switch (skillType)
        {
            case SkillType.Farming:
                farmer.FarmingLevel = 0;
                break;

            case SkillType.Fishing:
                farmer.FishingLevel = 0;
                break;

            case SkillType.Foraging:
                farmer.ForagingLevel = 0;
                break;

            case SkillType.Mining:
                farmer.MiningLevel = 0;
                break;

            case SkillType.Combat:
                farmer.CombatLevel = 0;
                break;

            case SkillType.Luck:
            default:
                return;
        }

        var toRemove = farmer.newLevels.Where(p => p.X == (int) skillType);
        foreach (var item in toRemove) farmer.newLevels.Remove(item);

        // reset skill experience
        farmer.experiencePoints[(int) skillType] = 0;

        if (ModEntry.Config.ForgetRecipesOnSkillReset)
        {
            var forgottenRecipesDict = Game1.player.ReadData(DataField.ForgottenRecipesDict).ToDictionary<string, int>(",", ";");

            // remove associated crafting recipes
            foreach (var recipe in farmer.GetCraftingRecipesForSkill(skillType))
            {
                if (!forgottenRecipesDict.TryAdd(recipe, farmer.craftingRecipes[recipe]))
                    forgottenRecipesDict[recipe] += farmer.craftingRecipes[recipe];
                farmer.craftingRecipes.Remove(recipe);
            }

            // remove associated cooking recipes
            foreach (var recipe in farmer.GetCookingRecipesForSkill(skillType))
            {
                if (!forgottenRecipesDict.TryAdd(recipe, farmer.cookingRecipes[recipe]))
                    forgottenRecipesDict[recipe] += farmer.craftingRecipes[recipe];
                farmer.cookingRecipes.Remove(recipe);
            }

            Game1.player.WriteData(DataField.ForgottenRecipesDict, forgottenRecipesDict.ToString(",", ";"));
        }

        // revalidate health
        if (skillType == SkillType.Combat) LevelUpMenu.RevalidateHealth(farmer);
    }

    /// <summary>Get all the farmer's crafting recipes associated with a specific skill.</summary>
    /// <param name="skillType">The desired skill.</param>
    public static IEnumerable<string> GetCraftingRecipesForSkill(this Farmer farmer, SkillType skillType)
    {
        return CraftingRecipe.craftingRecipes.Where(r =>
                r.Value.Split('/')[4].Contains(skillType.ToString()) && farmer.craftingRecipes.ContainsKey(r.Key))
            .Select(recipe => recipe.Key);
    }

    /// <summary>Get all the farmer's cooking recipes associated with a specific skill.</summary>
    /// <param name="skillType">The desired skill.</param>
    public static IEnumerable<string> GetCookingRecipesForSkill(this Farmer farmer, SkillType skillType)
    {
        return CraftingRecipe.cookingRecipes.Where(r =>
                r.Value.Split('/')[3].Contains(skillType.ToString()) && farmer.cookingRecipes.ContainsKey(r.Key))
            .Select(recipe => recipe.Key);
    }

    /// <summary>Get all available Super Mode's not currently registered.</summary>
    public static IEnumerable<SuperModeIndex> GetUnchosenSuperModes(this Farmer farmer)
    {
        return farmer.professions.Where(p => Enum.IsDefined(typeof(SuperModeIndex), p)).Cast<SuperModeIndex>();
    }

    /// <summary>Whether the farmer has caught the specified fish at max size.</summary>
    /// <param name="index">The fish's index.</param>
    public static bool HasCaughtMaxSized(this Farmer farmer, int index)
    {
        if (!farmer.fishCaught.ContainsKey(index) || farmer.fishCaught[index][1] <= 0) return false;

        var fishData = Game1.content
            .Load<Dictionary<int, string>>(PathUtilities.NormalizeAssetName("Data/Fish"))
            .Where(p => !p.Key.IsAnyOf(152, 153, 157) && !p.Value.Contains("trap"))
            .ToDictionary(p => p.Key, p => p.Value);

        if (!fishData.TryGetValue(index, out var specificFishData)) return false;

        var dataFields = specificFishData.Split('/');
        return farmer.fishCaught[index][1] >= Convert.ToInt32(dataFields[4]);
    }

    /// <summary>The price bonus applied to animal produce sold by Producer.</summary>
    public static float GetProducerPriceBonus(this Farmer farmer)
    {
        return Game1.getFarm().buildings.Where(b =>
            (b.owner.Value == farmer.UniqueMultiplayerID || !Context.IsMultiplayer) &&
            b.buildingType.Contains("Deluxe") && ((AnimalHouse) b.indoors.Value).isFull()).Sum(_ => 0.05f);
    }

    /// <summary>The bonus catching bar speed for prestiged Fisher.</summary>
    public static float GetFisherBonusCatchingBarSpeed(this Farmer farmer, int whichFish)
    {
        return farmer.fishCaught.TryGetValue(whichFish, out var caughtData)
            ? caughtData[0] >= ModEntry.Config.FishNeededForInstantCatch
                ? 1f
                : Math.Max(caughtData[0] * (0.1f / ModEntry.Config.FishNeededForInstantCatch) * 0.0002f, 0.002f)
            : 0.002f;
    }

    /// <summary>The price bonus applied to fish sold by Angler.</summary>
    public static float GetAnglerPriceBonus(this Farmer farmer)
    {
        var fishData = Game1.content.Load<Dictionary<int, string>>(PathUtilities.NormalizeAssetName("Data/Fish"))
            .Where(p => !p.Key.IsAlgae() && !p.Value.Contains("trap"))
            .ToDictionary(p => p.Key, p => p.Value);

        var bonus = 0f;
        foreach (var (key, value) in farmer.fishCaught.Pairs)
        {
            if (!fishData.TryGetValue(key, out var specificFishData)) continue;

            var dataFields = specificFishData.Split('/');
            if (ObjectLookups.LegendaryFishNames.Contains(dataFields[0]))
                bonus += 0.05f;
            else if (value[1] >= Convert.ToInt32(dataFields[4]))
                bonus += 0.01f;
        }

        return Math.Min(bonus, ModEntry.Config.AnglerMultiplierCeiling);
    }

    /// <summary>The amount of "catching" bar to compensate for Aquarist.</summary>
    public static float GetAquaristCatchingBarCompensation(this Farmer farmer)
    {
        var fishTypes = Game1.getFarm().buildings
            .OfType<FishPond>()
            .Where(pond => (pond.owner.Value == farmer.UniqueMultiplayerID || !Context.IsMultiplayer) && pond.fishType.Value > 0)
            .Select(pond => pond.fishType.Value);

        return Math.Min(fishTypes.Distinct().Count() * 0.000165f, 0.002f);
    }

    /// <summary>The price bonus applied to all items sold by Conservationist.</summary>
    public static float GetConservationistPriceMultiplier(this Farmer farmer)
    {
        return 1f + farmer.ReadDataAs<float>(DataField.ConservationistActiveTaxBonusPct);
    }

    /// <summary>The quality of items foraged by Ecologist.</summary>
    public static int GetEcologistForageQuality(this Farmer farmer)
    {
        var itemsForaged = farmer.ReadDataAs<uint>(DataField.EcologistItemsForaged);
        return itemsForaged < ModEntry.Config.ForagesNeededForBestQuality
            ? itemsForaged < ModEntry.Config.ForagesNeededForBestQuality / 2
                ? SObject.medQuality
                : SObject.highQuality
            : SObject.bestQuality;
    }

    /// <summary>The quality of minerals collected by Gemologist.</summary>
    public static int GetGemologistMineralQuality(this Farmer farmer)
    {
        var mineralsCollected = farmer.ReadDataAs<uint>(DataField.GemologistMineralsCollected);
        return mineralsCollected < ModEntry.Config.MineralsNeededForBestQuality
            ? mineralsCollected < ModEntry.Config.MineralsNeededForBestQuality / 2
                ? SObject.medQuality
                : SObject.highQuality
            : SObject.bestQuality;
    }

    /// <summary>The reduction to cooldown of special moves performed by prestiged Brute.</summary>
    public static float GetPrestigedBruteCooldownReduction(this Farmer farmer)
    {
        return 1f - farmer.attackIncreaseModifier + (farmer.CurrentTool is not null
            ? farmer.CurrentTool.GetEnchantmentLevel<RubyEnchantment>() * 0.1f
            : 0f);
    }

    /// <summary>The reduction to cooldown of special moves performed by prestiged Poacher.</summary>
    public static float GetPrestigedPoacherCooldownReduction(this Farmer farmer)
    {
        return 1f - farmer.critChanceModifier + farmer.critPowerModifier + (farmer.CurrentTool is not null
            ? farmer.CurrentTool.GetEnchantmentLevel<AquamarineEnchantment>() +
              farmer.CurrentTool.GetEnchantmentLevel<JadeEnchantment>() * 0.1f
            : 0f);
    }

    /// <summary>Enumerate the Slimes currently inhabiting owned Slimes Hutches.</summary>
    public static IEnumerable<GreenSlime> GetRaisedSlimes(this Farmer farmer)
    {
        return Game1.getFarm().buildings
            .Where(b => (b.owner.Value == farmer.UniqueMultiplayerID || !Context.IsMultiplayer) &&
                        b.indoors.Value is SlimeHutch && !b.isUnderConstruction())
            .SelectMany(b => b.indoors.Value.characters.OfType<GreenSlime>());
    }

    /// <summary>The chance to fire a free consecutive shot for Desperado.</summary>
    public static float GetDesperadoDoubleStrafeChance(this Farmer farmer)
    {
        var healthPercent = (double) farmer.health / farmer.maxHealth;
        return (float) Math.Min(2 / (healthPercent + 1.5) - 0.75, 0.5f);
    }

    /// <summary>Read a string from the <see cref="ModDataDictionary" />.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue">The default value to return if the field does not exist.</param>
    internal static string ReadData(this Farmer farmer, DataField field, string defaultValue = "")
    {
        return Game1.MasterPlayer.modData.Read($"{ModEntry.Manifest.UniqueID}/{farmer.UniqueMultiplayerID}/{field}",
            defaultValue);
    }

    /// <summary>Read a field from the <see cref="ModDataDictionary" /> as <typeparamref name="T" />.</summary>
    /// <param name="field">The field to read from.</param>
    /// <param name="defaultValue"> The default value to return if the field does not exist.</param>
    internal static T ReadDataAs<T>(this Farmer farmer, DataField field, T defaultValue = default)
    {
        return Game1.MasterPlayer.modData.ReadAs($"{ModEntry.Manifest.UniqueID}/{farmer.UniqueMultiplayerID}/{field}",
            defaultValue);
    }

    /// <summary>Write to a field in the <see cref="ModDataDictionary" />, or remove the field if supplied with a null or empty value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    internal static void WriteData(this Farmer farmer, DataField field, string value)
    {
        if (Context.IsMultiplayer && !Context.IsMainPlayer)
        {
            // request the main player
            ModEntry.ModHelper.Multiplayer.SendMessage(value, $"RequestUpdateData/Write/{field}",
                new[] { ModEntry.Manifest.UniqueID }, new[] { Game1.MasterPlayer.UniqueMultiplayerID });
            return;
        }

        Game1.player.modData.Write($"{ModEntry.Manifest.UniqueID}/{farmer.UniqueMultiplayerID}/{field}", value);
        Log.D($"[ModData]: Wrote {value} to {farmer.Name}'s {field}.");
    }

    /// <summary>Write to a field in the <see cref="ModDataDictionary" />, only if it doesn't yet have a value.</summary>
    /// <param name="field">The field to write to.</param>
    /// <param name="value">The value to write, or <c>null</c> to remove the field.</param>
    internal static bool WriteDataIfNotExists(this Farmer farmer, DataField field, string value)
    {
        if (Game1.MasterPlayer.modData.ContainsKey($"{ModEntry.Manifest.UniqueID}/{farmer.UniqueMultiplayerID}/{field}"))
        {
            Log.D($"[ModData]: The data field {field} already existed.");
            return true;
        }

        if (Context.IsMultiplayer && !Context.IsMainPlayer)
            ModEntry.ModHelper.Multiplayer.SendMessage(value, $"RequestUpdateData/Write/{field}",
                new[] { ModEntry.Manifest.UniqueID },
                new[] { Game1.MasterPlayer.UniqueMultiplayerID }); // request the main player
        else Game1.player.WriteData(field, value);

        return false;
    }

    /// <summary>Increment the value of a numeric field in the <see cref="ModDataDictionary" /> by an arbitrary amount.</summary>
    /// <param name="field">The field to update.</param>
    /// <param name="amount">Amount to increment by.</param>
    internal static void IncrementData<T>(this Farmer farmer, DataField field, T amount)
    {
        if (Context.IsMultiplayer && !Context.IsMainPlayer)
        {
            // request the main player
            ModEntry.ModHelper.Multiplayer.SendMessage(amount, $"RequestUpdateData/Increment/{field}",
                new[] { ModEntry.Manifest.UniqueID }, new[] { Game1.MasterPlayer.UniqueMultiplayerID });
            return;
        }

        Game1.player.modData.Increment($"{ModEntry.Manifest.UniqueID}/{farmer.UniqueMultiplayerID}/{field}", amount);
        Log.D($"[ModData]: Incremented {farmer.Name}'s {field} by {amount}.");
    }

    /// <summary>Increment the value of a numeric field in the <see cref="ModDataDictionary" /> by 1.</summary>
    /// <param name="field">The field to update.</param>
    internal static void IncrementData<T>(this Farmer farmer, DataField field)
    {
        if (Context.IsMultiplayer && !Context.IsMainPlayer)
        {
            // request the main player
            ModEntry.ModHelper.Multiplayer.SendMessage(1, $"RequestUpdateData/Increment/{field}",
                new[] { ModEntry.Manifest.UniqueID }, new[] { Game1.MasterPlayer.UniqueMultiplayerID });
            return;
        }

        Game1.player.modData.Increment($"{ModEntry.Manifest.UniqueID}/{farmer.UniqueMultiplayerID}/{field}",
            "1".Parse<T>());
        Log.D($"[ModData]: Incremented {farmer.Name}'s {field} by 1.");
    }
}