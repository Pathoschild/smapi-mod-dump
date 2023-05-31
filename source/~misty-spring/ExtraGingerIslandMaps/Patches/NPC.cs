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

namespace ExtraGingerIslandMaps.Patches
{
    [HarmonyPatch(typeof(NPC))]
    internal static class NpcPatches
    {

        internal static void Apply(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), "startRouteBehavior"),
                postfix: new HarmonyMethod(typeof(NpcPatches), nameof(Postfix_startRouteBehavior))
                );

            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), "finishRouteBehavior"),
                postfix: new HarmonyMethod(typeof(NpcPatches), nameof(Postfix_finishRouteBehavior))
                );
        }

        [HarmonyPostfix]
        [HarmonyPatch("startRouteBehavior")]
        internal static void Postfix_startRouteBehavior(ref NPC instance, string behaviorName)
        {
            switch (behaviorName)
		    {
                case "jackie_fishing":
                    instance.extendSourceRect(0, 32);
			        instance.Sprite.tempSpriteHeight = 64;
                    instance.drawOffset = new Vector2(0f, 96f); //orig: 96
			        instance.Sprite.ignoreSourceRectUpdates = false;
			        if (Utility.isOnScreen(Utility.Vector2ToPoint(instance.Position), 64, instance.currentLocation))
			        {
                        instance.currentLocation.playSound("slosh", instance.Tile);
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
        internal static void Postfix_finishRouteBehavior(ref NPC instance, string behaviorName)
	    {
		    switch (behaviorName)
		    {
		    case "jackie_floor":
		    case "jackie_fishing":
			    instance.reloadSprite();
			    instance.Sprite.SpriteWidth = 16;
			    instance.Sprite.SpriteHeight = 32;
			    instance.Sprite.UpdateSourceRect();
                instance.drawOffset = Vector2.Zero;
			    instance.Halt();
                instance.movementPause = 1;
			    break;
		    }
	    }
    }
}