/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/BetterBeehouses
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace BetterBeehouses
{
    [HarmonyPatch(typeof(Utility))]
    class UtilityPatch
    {
        private static ILHelper flowerPatch = new ILHelper("findCloseFlower")
            .SkipTo(new CodeInstruction[]
            {
                new(OpCodes.Ldloc_0),
                new(OpCodes.Callvirt, typeof(Queue<Vector2>).MethodNamed("Dequeue")),
                new(OpCodes.Stloc_2)
            })
            .Transform(new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, typeof(GameLocation).FieldNamed("terrainFeatures")),
                new(OpCodes.Ldloc_2)
            }, AddCheck)
            .Transform(new CodeInstruction[]
            {
                new(OpCodes.Call, typeof(Item).MethodNamed("get_Category")),
                new(OpCodes.Ldc_I4_S, -80),
                new(OpCodes.Bne_Un),
                null
            }, AddContextTag)
            .Finish();

        [HarmonyPatch("findCloseFlower", new Type[]{typeof(GameLocation),typeof(Vector2),typeof(int),typeof(Func<Crop, bool>)})]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> findCloseFlower(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            return flowerPatch.Run(instructions, generator);
        }
        private static IEnumerable<CodeInstruction> AddCheck(IList<CodeInstruction> codes)
        {
            var label = flowerPatch.Generator.DefineLabel();

            yield return new(OpCodes.Ldarg_0);
            yield return new(OpCodes.Ldloc_2);
            yield return new(OpCodes.Ldarg_3);
            yield return new(OpCodes.Call, typeof(UtilityPatch).MethodNamed("GetPotCrop"));
            yield return new(OpCodes.Dup);
            yield return new(OpCodes.Brfalse, label);
            yield return new(OpCodes.Ret);
            yield return new CodeInstruction(OpCodes.Pop).WithLabels(label);
            foreach (var code in codes)
                yield return code;
        }
        private static IEnumerable<CodeInstruction> AddContextTag(IList<CodeInstruction> codes)
        {
            var jump = flowerPatch.Generator.DefineLabel();
            var label = flowerPatch.Generator.DefineLabel();
            codes[0].labels.Add(label);
            codes[3].labels.Add(jump);

            yield return new(OpCodes.Dup);
            yield return new(OpCodes.Ldstr, "honey_source");
            yield return new(OpCodes.Call, typeof(StardewValley.Object).MethodNamed("HasContextTag"));
            yield return new(OpCodes.Brfalse, label);
            yield return new(OpCodes.Pop);
            yield return new(OpCodes.Br, jump);
            foreach (var code in codes)
                yield return code;
        }
        public static Crop GetPotCrop(GameLocation loc, Vector2 tile, Func<Crop, bool> extraCheck)
        {
            if(loc.objects.TryGetValue(tile, out StardewValley.Object obj))
            {
                if (obj is IndoorPot pot) //pot crop
                {
                    if (ModEntry.config.UseForageFlowers && pot.heldObject.Value != null) //forage in pot
                    {
                        var ho = pot.heldObject.Value;
                        if (ho.CanBeGrabbed && (ho.Category == -80 || ho.HasContextTag("honey_source")))
                            return Utils.CropFromObj(ho);
                    }
                    Crop crop = pot.hoeDirt.Value?.crop;
                    if (crop is null)
                        return null;
                    var co = new StardewValley.Object(crop.indexOfHarvest.Value, 1);
                    return Utils.GetProduceHere(loc, ModEntry.config.UsePottedFlowers) && IsGrown(crop, extraCheck) && (co.Category == -80 || co.HasContextTag("honey_source")) ?
                        crop : null; //flower in pot
                } else
                {
                    return ModEntry.config.UseForageFlowers && obj.CanBeGrabbed && (obj.Category == -80 || obj.HasContextTag("honey_source")) ? Utils.CropFromObj(obj) : null;
                    //non-pot forage
                }
            }
            return null;
        }
        public static bool IsGrown(Crop crop, Func<Crop, bool> extraCheck)
            =>  crop != null &&
                crop.currentPhase.Value >= crop.phaseDays.Count - 1 &&
                !crop.dead.Value && (extraCheck == null || extraCheck(crop));
    }
}
