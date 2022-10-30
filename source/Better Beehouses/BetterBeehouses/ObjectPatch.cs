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
using Netcode;
using StardewValley;
using MathF = System.MathF;
using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.Xna.Framework;
using System.Linq;

namespace BetterBeehouses
{
    [HarmonyPatch(typeof(Object))]
    class ObjectPatch
    {
        private static ILHelper minutesElapsedPatch = new ILHelper("Object: Minutes Elapsed")
            .SkipTo(new CodeInstruction[] { 
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, typeof(Object).MethodNamed("get_name")),
                new(OpCodes.Ldstr, "Bee House"),
                new(OpCodes.Callvirt, typeof(string).MethodNamed("Equals", new[]{typeof(string)}))
            })
            .Skip(2)
            .Remove()
            .Add(new CodeInstruction[] { 
                new(OpCodes.Call,typeof(ObjectPatch).MethodNamed(nameof(CanProduceHere)))
            })
            .Finish();

        private static ILHelper dayUpdatePatch = new ILHelper("Object: Day Update")
            .SkipTo(new CodeInstruction[] {
                new(OpCodes.Callvirt, typeof(GameLocation).MethodNamed("GetSeasonForLocation")),
                new(OpCodes.Ldstr, "winter"),
                new(OpCodes.Callvirt, typeof(string).MethodNamed("Equals",new[]{typeof(string)}))
            })
            .Add(new CodeInstruction[]
            {
                new(OpCodes.Ldarg_1),
                new(OpCodes.Call, typeof(ObjectPatch).MethodNamed(nameof(CantProduceToday)))
            })
            .SkipTo(new CodeInstruction[] { 
                new(OpCodes.Ldfld,typeof(Object).FieldNamed("minutesUntilReady")),
                new(OpCodes.Ldsfld,typeof(Game1).FieldNamed("timeOfDay"))
            })
            .Transform(new CodeInstruction[]{
                new(OpCodes.Call,typeof(Utility).MethodNamed("CalculateMinutesUntilMorning",new[]{typeof(int),typeof(int)}))
            }, ChangeDays)
            .Finish();

        private static ILHelper dropDownPatch = new ILHelper("Object: Drop Down Action")
            .SkipTo(new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, typeof(Object).MethodNamed("get_name")),
                new(OpCodes.Ldstr,"Bee House"),
                new(OpCodes.Callvirt, typeof(string).MethodNamed("Equals", new[]{typeof(string)}))
            })
            .SkipTo(new CodeInstruction[] {
                new(OpCodes.Ldfld,typeof(Object).FieldNamed("minutesUntilReady")),
                new(OpCodes.Ldsfld,typeof(Game1).FieldNamed("timeOfDay"))
            })
            .Transform(new CodeInstruction[]{
                new(OpCodes.Call,typeof(Utility).MethodNamed("CalculateMinutesUntilMorning",new[]{typeof(int),typeof(int)}))
            }, ChangeDays)
            .Finish();

        private static ILHelper checkForActionPatch = new ILHelper("Object: Check for Action")
            .SkipTo(new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, typeof(Object).PropertyGetter(nameof(Object.name))),
                new(OpCodes.Ldstr,"Bee House"),
                new(OpCodes.Callvirt,typeof(string).MethodNamed("Equals",new[]{typeof(string)}))
            })
            .SkipTo(new CodeInstruction[]
            {
                new(OpCodes.Callvirt, typeof(Character).PropertyGetter(nameof(Character.currentLocation))),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, typeof(Object).FieldNamed("tileLocation"))
            })
            .Skip()
            .Remove()
            .Add(new CodeInstruction(OpCodes.Call,typeof(ObjectPatch).MethodNamed(nameof(GetSearchRange))))
            .SkipTo(new CodeInstruction[]
            {
                new(OpCodes.Ldstr," Honey")
            })
            .SkipTo(new CodeInstruction[]
            {
                new(OpCodes.Ldc_I4_0),
                new(OpCodes.Callvirt, typeof(string).MethodNamed("Split", new[]{typeof(char),typeof(System.StringSplitOptions)})),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Ldelem_Ref),
                new(OpCodes.Call, typeof(System.Convert).MethodNamed("ToInt32", new[]{typeof(string)}))
            })
            .SkipTo(new CodeInstruction[]
            {
                new(OpCodes.Callvirt, typeof(Object).PropertySetter(nameof(Object.Price)))
            })
            .Add(new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, typeof(Object).FieldNamed("heldObject")),
                new(OpCodes.Callvirt, typeof(NetRef<Object>).PropertyGetter(nameof(NetRef<Object>.Value))),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Ldnull),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, typeof(Object).PropertyGetter(nameof(Object.TileLocation))),
                new(OpCodes.Call, typeof(ObjectPatch).MethodNamed(nameof(ManipulateObject)))
            })
            .SkipTo(new CodeInstruction[]
            {
                new(OpCodes.Call,typeof(Game1).PropertyGetter(nameof(Game1.currentLocation))),
                new(OpCodes.Call,typeof(Game1).MethodNamed("GetSeasonForLocation")),
                new(OpCodes.Ldstr, "winter"),
                new(OpCodes.Callvirt, typeof(string).MethodNamed("Equals",new[] { typeof(string) }))
            })
            .Add(new CodeInstruction[]
            {
                new(OpCodes.Call,typeof(Game1).PropertyGetter(nameof(Game1.currentLocation))),
                new(OpCodes.Call,typeof(ObjectPatch).MethodNamed(nameof(CantProduceToday)))
            })
            .SkipTo(new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, typeof(Object).PropertyGetter(nameof(Object.name))),
                new(OpCodes.Ldstr, "Bee House"),
                new(OpCodes.Callvirt, typeof(string).MethodNamed("Equals",new[] { typeof(string) }))
            })
            .SkipTo(new CodeInstruction[] {
                new(OpCodes.Ldstr, "winter"),
                new(OpCodes.Callvirt, typeof(string).MethodNamed("Equals",new[] { typeof(string) }))
            })
            .Add(new CodeInstruction[]
            {
                new(OpCodes.Ldarg_1),
                new(OpCodes.Call, typeof(ObjectPatch).MethodNamed(nameof(CantProduceToday)))
            })
            .SkipTo(new CodeInstruction[] {
                new(OpCodes.Ldfld,typeof(Object).FieldNamed("minutesUntilReady")),
                new(OpCodes.Ldsfld,typeof(Game1).FieldNamed("timeOfDay"))
            })
            .Transform(new CodeInstruction[]{
                new(OpCodes.Call,typeof(Utility).MethodNamed("CalculateMinutesUntilMorning",new[]{typeof(int),typeof(int)}))
            }, ChangeDays)
            .Finish();

        //---------

        [HarmonyPatch("minutesElapsed")]
        [HarmonyTranspiler]
        internal static IEnumerable<CodeInstruction> minutesElapsed(IEnumerable<CodeInstruction> instructions) => minutesElapsedPatch.Run(instructions);

        [HarmonyPatch("DayUpdate")]
        [HarmonyTranspiler]
        [HarmonyPriority(Priority.VeryLow)]
        internal static IEnumerable<CodeInstruction> DayUpdate(IEnumerable<CodeInstruction> instructions) => dayUpdatePatch.Run(instructions);

        [HarmonyPatch("performDropDownAction")]
        [HarmonyTranspiler]
        [HarmonyPriority(Priority.VeryLow)]
        internal static IEnumerable<CodeInstruction> DropDown(IEnumerable<CodeInstruction> instructions) => dropDownPatch.Run(instructions);

        [HarmonyPatch("checkForAction")]
        [HarmonyTranspiler]
        [HarmonyPriority(Priority.VeryLow)]
        internal static IEnumerable<CodeInstruction> checkForAction(IEnumerable<CodeInstruction> instructions) => checkForActionPatch.Run(instructions);

        //--------

        internal static IEnumerable<CodeInstruction> ChangeDays(IList<CodeInstruction> codes)
        {
            var op = new CodeInstruction(OpCodes.Call, typeof(ObjectPatch).MethodNamed(nameof(GetProduceDays)));
            op.labels.AddRange(codes[0].labels);
            codes[0].labels.Clear();
            yield return op;
            foreach (var code in codes)
                yield return code;
        }
        public static void ManipulateObject(Object obj, Farmer who, GameLocation where, Vector2 tile)
        {
            where ??= who?.currentLocation ?? Game1.currentLocation;
            obj.Quality = GetQuality(who);
            float val = obj.Price * ModEntry.config.ValueMultiplier;
            int cap = ModEntry.config.CapFactor;
            if(val > cap)
                val = cap * MathF.Pow(
                    val / cap,
                    1f / (1f + ModEntry.config.CapCurve)
                );
            obj.Price = (int)(val + .5f);
            var test = UtilityPatch.GetAllNearFlowers(where, tile, ModEntry.config.FlowerRange).ToArray();
			if (ModEntry.config.UseFlowerBoost)
                obj.Stack += System.Math.Max(UtilityPatch.GetAllNearFlowers(where, tile, ModEntry.config.FlowerRange).Count() - 1, 0) 
                    / ModEntry.config.FlowersPerBoost;
        }
        public static bool CantProduceToday(bool isWinter, GameLocation loc) 
            => isWinter && !Utils.GetProduceHere(loc, ModEntry.config.ProduceInWinter);
        public static int GetSearchRange() 
            => ModEntry.config.FlowerRange;
        public static int GetProduceDays(int original) 
            => System.Math.Max(original * ModEntry.config.DaysToProduce / 4, 1);
        public static bool CanProduceHere(GameLocation loc)
        {
            var where = ModEntry.config.UsableIn;
            return where is Config.UsableOptions.Anywhere || loc.IsOutdoors || loc.isGreenhouse.Value && where is not Config.UsableOptions.Outdoors;
        }
        public static int GetQuality(Farmer who)
        {
            //based on Crop.harvest()
            if (!ModEntry.config.UseQuality)
                return 0;

            float boost = who.eventsSeen.Contains(2120303) ? ModEntry.config.BearBoost : 1f;

            double chanceForGoldQuality = 0.2 * (who.FarmingLevel / 10.0) + 0.2 * boost * ((who.FarmingLevel + 2.0) / 12.0) + 0.01;
            double chanceForSilverQuality = System.Math.Min(0.75, chanceForGoldQuality * 2.0);
            return (Game1.random.NextDouble() < chanceForGoldQuality / 2.0) ? 4 :
                (Game1.random.NextDouble() < chanceForGoldQuality) ? 2 :
                (Game1.random.NextDouble() < chanceForSilverQuality) ? 1 : 0;
        }
    }
}
