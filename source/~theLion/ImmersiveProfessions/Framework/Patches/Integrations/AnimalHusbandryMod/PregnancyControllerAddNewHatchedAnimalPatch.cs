/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Patches.Integrations.AnimalHusbandryMod;

#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Buildings;

using Stardew.Common.Extensions;
using Stardew.Common.Harmony;
using Extensions;

#endregion using directives

[UsedImplicitly]
internal class PregnancyControllerAddNewHatchedAnimalPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal PregnancyControllerAddNewHatchedAnimalPatch()
    {
        try
        {
            Original = "AnimalHusbandryMod.animals.PregnancyController".ToType().MethodNamed("addNewHatchedAnimal");
        }
        catch
        {
            // ignored
        }
    }

    #region harmony patches

    /// <summary>Patch for Rancher husbanded animals to have random starting friendship.</summary>

    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> PregnancyControllerAddNewHatchedAnimalTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// Injected: AddNewHatchedAnimalSubroutine(farmAnimal);
        /// Before: AnimalHouse animalHouse = building.indoors.Value as AnimalHouse; 

        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldloc_0),
                    new CodeInstruction(OpCodes.Ldfld, typeof(Building).Field(nameof(Building.indoors)))
                )
                .RetreatUntil(
                    new CodeInstruction(OpCodes.Nop)
                )
                .Insert(
                    new CodeInstruction(OpCodes.Ldloc_1),
                    new CodeInstruction(OpCodes.Call,
                        typeof(PregnancyControllerAddNewHatchedAnimalPatch).MethodNamed(
                            nameof(AddNewHatchedAnimalSubroutine)))
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching Rancher husbanded newborn friendship.\nHelper returned {ex}");
            transpilationFailed = true;
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region private methods

    private static void AddNewHatchedAnimalSubroutine(FarmAnimal newborn)
    {
        var owner = Game1.getFarmer(newborn.ownerID.Value);
        if (!owner.HasProfession(Profession.Rancher)) return;

        newborn.friendshipTowardFarmer.Value =
            200 + new Random(newborn.myID.GetHashCode()).Next(-50, 51);
    }

    #endregion private methods
}