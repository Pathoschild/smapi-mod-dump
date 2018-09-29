using System;
using RightClickMoveMode;
using Harmony;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewModdingAPI;

namespace Game1Patch
{
    [HarmonyPatch(typeof(Farmer)), HarmonyPatch("Halt")]
    internal class HaltPatch
    {
        public static bool Prefix(Game1 __instance)
        {
            if (ModEntry.isRightClickMoveModeOn)
            {
                if (!ModEntry.isMovingAutomaticaly || ModEntry.isBeingAutoCommand)
                    return true;
                else
                    return false;
            }
            else
                return true;
        }
    }
    
    [HarmonyPatch(typeof(Game1)), HarmonyPatch("UpdateControlInput"), HarmonyPatch(new Type[]
    {
        typeof(GameTime)
    })]
    internal class UpdateControlInputPatch
    {
        public static void Postfix(Game1 __instance)
        {
            if (ModEntry.isRightClickMoveModeOn)
            {
                if (!ModEntry.isBeingControl && Context.IsPlayerFree)
                {
                    ModEntry.isBeingAutoCommand = true;
                    ModEntry.MoveVectorToCommand();
                    Game1.player.setRunning(true, false);
                    Game1.player.running = true;
                    ModEntry.isBeingAutoCommand = false;
                }
                else
                    ModEntry.isBeingAutoCommand = false;
            }
        }
    }
    
}
