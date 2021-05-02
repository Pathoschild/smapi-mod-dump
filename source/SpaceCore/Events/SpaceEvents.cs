/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/SpaceCore_SDV
**
*************************************************/

using Microsoft.Xna.Framework;
using SpaceCore.Utilities;
using SpaceShared;
using StardewValley;
using StardewValley.Events;
using StardewValley.Network;
using System;

namespace SpaceCore.Events
{
    public class SpaceEvents
    {
        // This occurs before loading starts.
        // Locations should be added here so that SaveData.loadDataToLocations picks them up
        public static event EventHandler OnBlankSave;

        // When the shipping menu pops up, level up menus, ...
        public static event EventHandler<EventArgsShowNightEndMenus> ShowNightEndMenus;

        // Lets you hook into Utillity.pickFarmEvent
        public static event EventHandler<EventArgsChooseNightlyFarmEvent> ChooseNightlyFarmEvent;

        // When the player is done eating an item.
        // Check what item using player.itemToEat
        public static event EventHandler OnItemEaten;

        // When a tile "Action" is activated
        public static event EventHandler<EventArgsAction> ActionActivated;

        // When a tile "TouchAction" is activated
        public static event EventHandler<EventArgsAction> TouchActionActivated;

        // Server side, when a client joins
        public static event EventHandler<EventArgsServerGotClient> ServerGotClient;

        // Right before a gift is given to someone. Sender is farmer.
        public static event EventHandler<EventArgsBeforeReceiveObject> BeforeGiftGiven;

        // When a gift is given to someone. Sender is farmer.
        public static event EventHandler<EventArgsGiftGiven> AfterGiftGiven;

        // Before the player is about to warp. Can cancel warping or change the target location.
        public static event EventHandler<EventArgsBeforeWarp> BeforeWarp;

        // When a bomb explodes
        public static event EventHandler<EventArgsBombExploded> BombExploded;

        internal static void InvokeOnBlankSave()
        {
            Log.trace("Event: OnBlankSave");
            if (OnBlankSave == null)
                return;
            Util.invokeEvent("SpaceEvents.OnBlankSave", OnBlankSave.GetInvocationList(), null);
        }

        internal static void InvokeShowNightEndMenus(EventArgsShowNightEndMenus args)
        {
            Log.trace("Event: ShowNightEndMenus");
            if (ShowNightEndMenus == null)
                return;
            Util.invokeEvent( "SpaceEvents.ShowNightEndMenus", ShowNightEndMenus.GetInvocationList(), null, args);
        }

        internal static FarmEvent InvokeChooseNightlyFarmEvent(FarmEvent vanilla)
        {
            var args = new EventArgsChooseNightlyFarmEvent();
            args.NightEvent = vanilla;

            Log.trace("Event: ChooseNightlyFarmEvent");
            if (ChooseNightlyFarmEvent == null)
                return args.NightEvent;
            Util.invokeEvent("SpaceEvents.ChooseNightlyFarmEvent", ChooseNightlyFarmEvent.GetInvocationList(), null, args);
            return args.NightEvent;
        }

        internal static void InvokeOnItemEaten(Farmer farmer)
        {
            Log.trace("Event: OnItemEaten");
            if (OnItemEaten == null || !farmer.IsLocalPlayer)
                return;
            Util.invokeEvent("SpaceEvents.OnItemEaten", OnItemEaten.GetInvocationList(), farmer);
        }

        internal static bool InvokeActionActivated(Farmer who, string action, xTile.Dimensions.Location pos)
        {
            Log.trace("Event: ActionActivated");
            if (ActionActivated == null || !who.IsLocalPlayer )
                return false;
            var arg = new EventArgsAction(false, action, pos);
            return Util.invokeEventCancelable("SpaceEvents.ActionActivated", ActionActivated.GetInvocationList(), who, arg);
        }

        internal static bool InvokeTouchActionActivated(Farmer who, string action, xTile.Dimensions.Location pos)
        {
            Log.trace("Event: TouchActionActivated");
            if (TouchActionActivated == null || !who.IsLocalPlayer )
                return false;
            var arg = new EventArgsAction(true, action, pos);
            return Util.invokeEventCancelable("SpaceEvents.TouchActionActivated", TouchActionActivated.GetInvocationList(), who, arg);
        }

        internal static void InvokeServerGotClient(GameServer server, long peer)
        {
            var args = new EventArgsServerGotClient();
            args.FarmerID = peer;

            Log.trace("Event: ServerGotClient");
            if (ServerGotClient == null)
                return;
            Util.invokeEvent("SpaceEvents.ServerGotClient", ServerGotClient.GetInvocationList(), server, args);
        }

        internal static bool InvokeBeforeReceiveObject( NPC npc, StardewValley.Object obj, Farmer farmer )
        {
            Log.trace( "Event: BeforeReceiveObject" );
            if ( BeforeGiftGiven == null )
                return false;
            var arg = new EventArgsBeforeReceiveObject(npc, obj);
            return Util.invokeEventCancelable( "SpaceEvents.BeforeReceiveObject", BeforeGiftGiven.GetInvocationList(), farmer, arg );
        }

        internal static void InvokeAfterGiftGiven(NPC npc, StardewValley.Object obj, Farmer farmer)
        {
            Log.trace("Event: AfterGiftGiven");
            if (AfterGiftGiven == null)
                return;
            var arg = new EventArgsGiftGiven(npc, obj);
            Util.invokeEvent("SpaceEvents.AfterGiftGiven", AfterGiftGiven.GetInvocationList(), farmer, arg);
        }

        internal static bool InvokeBeforeWarp(ref LocationRequest req, ref int targetX, ref int targetY, ref int facing)
        {
            Log.trace("Event: BeforeWarp");
            if (BeforeWarp == null)
                return false;
            var arg = new EventArgsBeforeWarp(req, targetX, targetY, facing);
            bool ret = Util.invokeEventCancelable("SpaceEvents.BeforeWarp", BeforeWarp.GetInvocationList(), Game1.player, arg);
            req = arg.WarpTargetLocation;
            targetX = arg.WarpTargetX;
            targetY = arg.WarpTargetY;
            facing = arg.WarpTargetFacing;
            return ret;
        }

        internal static void InvokeBombExploded(Farmer who, Vector2 tileLocation, int radius)
        {
            Log.trace("Event: BombExploded");
            if (BombExploded == null)
                return;
            var arg = new EventArgsBombExploded(tileLocation, radius);
            Util.invokeEvent("SpaceEvents.BombExploded", BombExploded.GetInvocationList(), who, arg);
        }
    }
}
