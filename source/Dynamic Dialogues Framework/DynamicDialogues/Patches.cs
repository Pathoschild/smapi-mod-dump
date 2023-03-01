/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/DynamicDialogues
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using System;
using static HarmonyLib.Code;

/*using System.Collections.Generic;
using Object = StardewValley.Object;
using StardewValley.Network;
using StardewValley.Objects;
using StardewValley.Tools;
*/

namespace DynamicDialogues
{
    internal class Patches
    {
        public static bool SayHiTo_Prefix(ref NPC __instance, Character c)
        {
            var instancename =__instance.Name; 
            var cname = (c as NPC).Name;
            var mainAndRef = (instancename, cname);
            var refAndMain = (cname, instancename);

            try
            {
                //if a (thisnpc, othernpc) key exists
                if (ModEntry.Greetings.ContainsKey((mainAndRef)))
                {
                    //log, then use previous key to find value
                    ModEntry.Mon.Log($"Found greeting patch for {__instance.Name}");
                    __instance.showTextAboveHead(ModEntry.Greetings[(mainAndRef)]);

                    //if that other npc has a key for thisnpc
                    if (ModEntry.Greetings.ContainsKey(refAndMain))
                    {
                        //same as before
                        ModEntry.Mon.Log($"Found greeting patch for {(c as NPC).Name}");
                        (c as NPC).showTextAboveHead(ModEntry.Greetings[(refAndMain)], -1, 2, 3000, 1000 + Game1.random.Next(500));
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                ModEntry.Mon.Log($"Error while applying patch: {ex}", StardewModdingAPI.LogLevel.Error);
            }

            return true;
        }

        internal static bool PrefixTryGetCommand(Event __instance, GameLocation location, GameTime time, string[] split)
        {
            if (split.Length < 2) //scene has optional parameters, so its 2 OR more
            {
                return true;
            }
            else if (split[0].Equals("AddScene")) //, StringComparison.Ordinal)
            {
                EventScene.Add(__instance, location, time, split);
                return false;
            }
            else if (split[0].Equals("RemoveScene")) //, StringComparison.Ordinal
            {
                EventScene.Remove(__instance, location, time, split);
                return false;
            }
        return true;
        }

        /*
        internal static bool TryPlayerControlSequence(ref Event __instance, string id)
        {
            if(id != "returnItem" || id != "findMultiple" || id != "chest")
            {
                return true;
            }

            __instance.playerControlSequenceID = id;
            __instance.playerControlSequence = true;
            Game1.player.CanMove = true;
            Game1.viewportFreeze = false;
            Game1.forceSnapOnNextViewportUpdate = true;
            Game1.globalFade = false;
            __instance.doingSecretSanta = false;
            switch (id)
            {
                case "returnItem":
                    __instance.playerControlTargetTile = new Point(53, 8);
                    __instance.props.Add(new Object(new Vector2(53f, 8f), 742, 1)
                    {
                        Flipped = false
                    });
                    Game1.player.canOnlyWalk = false;
                    break;
                case "findMultiple":
                    {
                        for (int i = 0; i < Game1.currentLocation.map.GetLayer("Paths").LayerWidth; i++)
                        {
                            for (int j = 0; j < Game1.currentLocation.map.GetLayer("Paths").LayerHeight; j++)
                            {
                                if (Game1.currentLocation.map.GetLayer("Paths").Tiles[i, j] != null)
                                {
                                    __instance.festivalProps.Add(new Prop(ModEntry.springobjects, Game1.currentLocation.map.GetLayer("Paths").Tiles[i, j].TileIndex, 1, 1, 1, i, j));
                                }
                            }
                        }

                        __instance.festivalTimer = 52000;
                        __instance.currentCommand++;
                        break;
                    }
                case "chest":
                    __instance.farmer.currentLocation.objects.Add(new Vector2(33f, 13f), new Chest(0, new List<Item>
                {
                    new Object(373, 1)
                }, new Vector2(33f, 13f)));
                    break;
            }
            return false;
        }*/
        //to make custom events, should also patch: public void festivalUpdate,
    }
}