/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using SpaceShared;

namespace ArcadeRoom
{
    public interface IApi
    {
        Vector2 ReserveMachineSpot();

        event EventHandler OnRoomSetup; 
    }

    public class Api : IApi
    {
        public Vector2 ReserveMachineSpot()
        {
            return Mod.instance.ReserveNextMachineSpot();
        }
        
        public event EventHandler OnRoomSetup;
        internal void InvokeOnRoomSetup()
        {
            Log.trace( "Event: OnRoomSetup" );
            if ( OnRoomSetup == null )
                return;
            Util.invokeEvent( "ArcadeRoom.Api.OnRoomSetup", OnRoomSetup.GetInvocationList(), null );
        }
    }
}
