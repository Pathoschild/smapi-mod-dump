/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/SolidFoundations
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolidFoundations.Framework.Models.Backport;
using SolidFoundations.Framework.Models.ContentPack;
using SolidFoundations.Framework.Utilities;
using SolidFoundations.Framework.Utilities.Backport;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Events;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using xTile.Dimensions;
using Object = StardewValley.Object;

namespace SolidFoundations.Framework.Patches.Core
{
    // TODO: When updated to SDV v1.6, delete this patch
    internal class GamePatch : PatchTemplate
    {

        private readonly Type _object = typeof(Game1);

        internal GamePatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_object, "UpdateLocations", new[] { typeof(GameTime) }), transpiler: new HarmonyMethod(typeof(GamePatch), nameof(UpdateLocationsTranspiler)));
        }

        private static IEnumerable<CodeInstruction> UpdateLocationsTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            try
            {
                object loopOperand = null;

                bool foundMultiplayerFix = false;
                bool foundGeneralFix = false;

                var list = instructions.ToList();
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].opcode == OpCodes.Br_S)
                    {
                        loopOperand = list[i].operand;
                        continue;
                    }

                    if (foundMultiplayerFix is false && list[i].opcode == OpCodes.Ldloc_3 && list[i + 1].opcode == OpCodes.Ldfld && (list[i + 1].operand as FieldInfo).Name == "indoors")
                    {
                        var startingPoint = i;
                        list.Insert(startingPoint, new CodeInstruction(OpCodes.Ldloc_3));
                        list.Insert(startingPoint + 1, new CodeInstruction(OpCodes.Isinst, typeof(GenericBuilding)));
                        list.Insert(startingPoint + 2, new CodeInstruction(OpCodes.Brfalse_S, loopOperand));

                        foundMultiplayerFix = true;
                        continue;
                    }
                    else if (foundGeneralFix is false && list[i].opcode == OpCodes.Stloc_S)
                    {
                        if (list[i - 1].opcode == OpCodes.Callvirt && (list[i - 1].operand as MethodInfo).Name == "get_Value" && list[i - 2].opcode == OpCodes.Ldfld && (list[i - 2].operand as FieldInfo).Name == "indoors")
                        {
                            var startingPoint = i + 1;
                            list.Insert(startingPoint, new CodeInstruction(OpCodes.Ldarg_1));
                            list.Insert(startingPoint + 1, new CodeInstruction(OpCodes.Ldloc_S, list[i].operand));
                            list.Insert(startingPoint + 2, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(GamePatch), nameof(SkipBuildingForUpdateCheck), new[] { typeof(GameTime), typeof(GameLocation) })));
                            list.Insert(startingPoint + 3, new CodeInstruction(OpCodes.Brfalse_S, loopOperand));

                            foundGeneralFix = true;
                            continue;
                        }
                    }
                }
                _monitor.Log($"Transpiler for Game1.UpdateLocations results: foundMultiplayerFix -> {foundMultiplayerFix} | foundGeneralFix -> {foundGeneralFix}", LogLevel.Trace);

                return list;
            }
            catch (Exception e)
            {
                _monitor.Log($"There was an issue modifying the instructions for Game1.UpdateLocations: {e}", LogLevel.Error);
                return instructions;
            }
        }


        // TODO: Remove this once this framework has been updated for SDV v1.6
        private static bool SkipBuildingForUpdateCheck(GameTime time, GameLocation interior)
        {
            if (interior is not null && Game1.locations.Contains(interior) is false)
            {
                if (interior.farmers.Any())
                {
                    interior.UpdateWhenCurrentLocation(time);
                }
                interior.updateEvenIfFarmerIsntHere(time);
            }

            return false;
        }
    }
}
