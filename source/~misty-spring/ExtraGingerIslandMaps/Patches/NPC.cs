/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;

namespace ExtraGingerIslandMaps
{
    [HarmonyPatch(typeof(NPC))]
    internal static class NPCPatches
    {

        internal static void Apply(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), "startRouteBehavior"),
                postfix: new HarmonyMethod(typeof(NPCPatches), nameof(NPCPatches.Postfix_startRouteBehavior))
                );

            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), "finishRouteBehavior"),
                postfix: new HarmonyMethod(typeof(NPCPatches), nameof(NPCPatches.Postfix_finishRouteBehavior))
                );
        }

        [HarmonyPostfix]
        [HarmonyPatch("startRouteBehavior")]
        internal static void Postfix_startRouteBehavior(ref NPC __instance, string behaviorName)
        {
            switch (behaviorName)
		    {
                case "jackie_fishing":
                    __instance.extendSourceRect(0, 32);
			        __instance.Sprite.tempSpriteHeight = 64;
                    __instance.drawOffset.Value = new Vector2(0f, 96f); //orig: 96
			        __instance.Sprite.ignoreSourceRectUpdates = false;
			        if (Utility.isOnScreen(Utility.Vector2ToPoint(__instance.Position), 64, __instance.currentLocation))
			        {
                        __instance.currentLocation.playSoundAt("slosh", __instance.getTileLocation());
			        }
			        break;

                    /*
                     
                case "jackie_floor":
                    __instance.extendSourceRect(32, 0);
			        //__instance.Sprite.SpriteWidth = 64; //theres no TempWidth
                    //__instance.drawOffset.Value = new Vector2(0f, 0f);
			        __instance.Sprite.ignoreSourceRectUpdates = false;

                    break;
                    */
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch("finishRouteBehavior")]
        internal static void Postfix_finishRouteBehavior(ref NPC __instance, string behaviorName)
	    {
		    switch (behaviorName)
		    {
		    case "jackie_floor":
		    case "jackie_fishing":
			    __instance.reloadSprite();
			    __instance.Sprite.SpriteWidth = 16;
			    __instance.Sprite.SpriteHeight = 32;
			    __instance.Sprite.UpdateSourceRect();
                __instance.drawOffset.Value = Vector2.Zero;
			    __instance.Halt();
                __instance.movementPause = 1;
			    break;
		    }
	    }
    }
}