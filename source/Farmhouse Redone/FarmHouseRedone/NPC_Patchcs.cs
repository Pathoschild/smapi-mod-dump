/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mjSurber/FarmHouseRedone
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using System.Reflection.Emit;
using StardewValley;
using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;

namespace FarmHouseRedone
{
    class NPC_marriageDuties_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);

            List<int> indicesToDelete = new List<int>();

            for(int i = 0; i < codes.Count; i++)
            {
                if(codes[i].opcode == OpCodes.Callvirt && codes[i].operand.ToString().Contains("getPorchStandingSpot"))
                {
                    Logger.Log("Replacing vanilla getPorchStandingSpot() call at index " + i + "...");
                    codes[i] = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(FarmState), nameof(FarmState.getPorchStandingSpotAndLog)));
                    Logger.Log("Index " + i + ": " + codes[i].ToString());
                    //codes[i] = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Logger), nameof(Logger.Log), new Type[] { typeof(string) }));
                    indicesToDelete.Add(i-1);
                }
            }

            indicesToDelete.Reverse();

            foreach(int index in indicesToDelete)
            {
                //Logger.Log("Deleting index " + index + ": " + codes[index].ToString());
                //codes.RemoveAt(index);
                codes[index] = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(FarmState), nameof(FarmState.setUpBaseFarm)));
                codes.Insert(index, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(FarmState), nameof(FarmState.init))));

                for(int readIndex = index - 3; readIndex < index + 4; readIndex++)
                {
                    Logger.Log("Index " + readIndex + ": " + codes[readIndex].ToString());
                }
                //codes.Insert(index, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(FarmState), nameof(FarmState.getPorchStandingSpot))))
                //codes.Insert(index, new CodeInstruction(OpCodes.Ldstr, "Using patched getPorchStandingSpot()..."));
            }
            return codes.AsEnumerable();
        }
    }

    class NPC_setUpForOutdoorPatioActivity_Patch
    {
        public static bool Prefix(NPC __instance)
        {
            Logger.Log("Prefixing " + __instance.name + "'s patio activity...");

            if (PatioManager.patio == null)
            {
                Logger.Log("Patio not constructed!");
                return true;
            }
            
            Vector2 standingSpot = new Vector2(FarmState.spouseOutdoorLocation.X + PatioManager.patio.offset.X, FarmState.spouseOutdoorLocation.Y + PatioManager.patio.offset.Y);
            
            Game1.warpCharacter(__instance, "Farm", standingSpot);
            __instance.currentMarriageDialogue.Clear();
            __instance.addMarriageDialogue("MarriageDialogue", "patio_" + __instance.Name, false);
            string name = __instance.name;
            __instance.setTilePosition((int)standingSpot.X, (int)standingSpot.Y);

            IReflectedField<NetBool> shouldPlaySpousePatioAnimation = FarmHouseStates.reflector.GetField<NetBool>(__instance, "shouldPlaySpousePatioAnimation");

            shouldPlaySpousePatioAnimation.GetValue().Value = true;
            return false;
        }
    }

    class NPC_doPlaySpousePatioAnimation_Patch
    {
        public static bool Prefix(NPC __instance)
        {
            Logger.Log("Prefixing " + __instance.name + "'s patio animation...");
            if (PatioManager.patio == null)
                return true;
            Logger.Log("Patio exists...");
            List<FarmerSprite.AnimationFrame> frames = PatioManager.patio.getAnimation();
            if (frames.Count < 1)
                return true;
            Logger.Log("Patio animation was provided.");
            __instance.Sprite.setCurrentAnimation(frames);
            return false;
        }
    }
}
