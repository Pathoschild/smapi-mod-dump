/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ylsama/RightClickMoveMode
**
*************************************************/

using System;
using MouseMoveMode;
using HarmonyLib;
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
            if (ModEntry.config.RightClickMoveModeDefault)
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
            if (ModEntry.config.RightClickMoveModeDefault)
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
