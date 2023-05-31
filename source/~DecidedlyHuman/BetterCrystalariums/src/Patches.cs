/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System;
using BetterCrystalariums.Utilities;
using StardewModdingAPI;
using StardewValley;
using Object = StardewValley.Object;

namespace BetterCrystalariums
{
    public class Patches
    {
        private static IMonitor _monitor;
        private static IModHelper _helper;
        private static Logger _logger;
        private static ModConfig _config;

        public Patches(IMonitor m, IModHelper h, Logger l, ModConfig c)
        {
            _monitor = m;
            _helper = h;
            _logger = l;
            _config = c;
        }

        public static bool ObjectDropIn_Prefix(Object __instance, Item dropInItem, bool probe, Farmer who)
        {
            if (_config.DebugMode)
            {
                // We're debugging, so we want to spit out as much information as possible.
                Item objectInMachine = __instance.heldObject;

                if (objectInMachine != null)
                {
                    _logger.Log("Debug output:\tVariable\t\t\t\tDetails");
                    _logger.Log($"\t\tFarmer.Name: \t\t\t\t{who.Name}");
                    _logger.Log($"\t\t__instance.Name \t\t\t{__instance.Name}");
                    _logger.Log($"\t\tdropInItem.Name \t\t\t{dropInItem.Name}");
                    _logger.Log($"\t\tdropInItem.Category \t\t\t{dropInItem.Category}");

                    _logger.Log($"\t\tName of object in machine \t\t{objectInMachine.Name}");
                    _logger.Log($"\t\tCategory of object in machine \t\t{objectInMachine.Category}");
                    _logger.Log($"{Environment.NewLine}");
                }
            }

            // Firstly, if the item the player is holding isn't a mineral, we don't want to do anything.
            if (dropInItem.Category != -2)
                return true;

            // Secondly, if the object isn't a crystalarium, we do nothing.
            if (!__instance.Name.Equals("Crystalarium"))
                return true;

            // At this point, we know the player is holding a crystalarium-able item, and is interacting with a crystalarium.

            // We get the object held in the crystalarium, cast to an Item.
            Item heldObject = __instance.heldObject;

            if (heldObject != null)
                // Then, if the object in the crystalarium doesn't match what the playe's holding, we display our warning, and stop the replacement.
                if (!heldObject.Name.Equals(dropInItem.Name))
                {
                    Game1.showRedMessage($"{_helper.Translation.Get("bettercrystalariums.wrong-mineral")}");
                    return false;
                }

            return true;
        }
    }
}
