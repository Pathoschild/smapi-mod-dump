/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mrveress/SDVMods
**
*************************************************/

using Microsoft.Xna.Framework;
using SeedMachines.Framework.BigCraftables;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeedMachines.Framework
{
    class EventHandlers
    {
        public static void OnGameLaunched(object sender, GameLaunchedEventArgs args)
        {
            ModEntry.dataLoader = new DataLoader();
        }
        public static void OnDayStarted(object sender, DayStartedEventArgs args)
        {
            IBigCraftableWrapper.addAllRecipies();
            BigCraftablesDynamicInjector.injectDynamicsInCurrentLocation();
        }

        public static void OnDayEnding(object sender, DayEndingEventArgs args)
        {
            BigCraftablesDynamicInjector.removeDynamicsInAllLocations();
        }

        public static void OnWarped(object sender, WarpedEventArgs args)
        {
            if (args.IsLocalPlayer)
            {
                BigCraftablesDynamicInjector.injectDynamicsInCurrentLocation();
            }
        }

        public static void OnObjectListChanged(object sender, ObjectListChangedEventArgs args)
        {
            BigCraftablesDynamicInjector.injectDynamicsInCurrentLocation();
        }

        public static void OnButtonPressed(object sender, ButtonPressedEventArgs args)
        {
            if (!Context.IsWorldReady)
                return;

            if (
                Context.IsPlayerFree
                && (args.Button.IsActionButton() || Constants.TargetPlatform == GamePlatform.Android)
                && Game1.currentLocation.objects.ContainsKey(args.Cursor.Tile)
                && Game1.currentLocation.objects[args.Cursor.Tile] is IBigCraftable
                && args.Cursor.Tile.Equals(args.Cursor.GrabTile)
            )
            {
                ((IBigCraftable)Game1.currentLocation.objects[args.Cursor.Tile]).onClick(args);
            }
        }
    }
}
