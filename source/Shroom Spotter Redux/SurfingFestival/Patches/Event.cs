/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using Harmony;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace SurfingFestival.Patches
{
    [HarmonyPatch( typeof( Event ), nameof( Event.setUpPlayerControlSequence ) )]
    public static class EventPlayerControlSequencePatch
    {
        public static void Postfix( Event __instance, string id )
        {
            if ( id == "surfing" )
            {
                Mod.instance.Helper.Reflection.GetField<NPC>( __instance, "festivalHost" ).SetValue( __instance.getActorByName( "Lewis" ) );
                Mod.instance.Helper.Reflection.GetField<string>( __instance, "hostMessage" ).SetValue( "$q -1 null#Ready for the race?#$r -1 0 yes#Yes, let's start.#$r -1 0 no#Not yet." );
            }
        }
    }

    [HarmonyPatch( typeof( Event ), nameof( Event.setUpFestivalMainEvent ) )]
    public static class EventMainEventPatch
    {
        public static void Postfix( Event __instance )
        {
            if ( !__instance.isSpecificFestival( "summer5" ) )
                return;

            // ...
        }
    }

    [HarmonyPatch( typeof( Event ), nameof( Event.draw ) )]
    public static class EventDrawPatch
    {
        public static void Postfix( Event __instance, SpriteBatch b )
        {
            if ( !__instance.isSpecificFestival( "summer5" ) )
                return;
            
            Mod.instance.DrawObstacles( b );
        }
    }
}
