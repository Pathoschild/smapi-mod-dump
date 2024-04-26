/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Smoked-Fish/AnythingAnywhere
**
*************************************************/

using HarmonyLib;
using StardewValley;
using StardewModdingAPI;
using StardewValley.Objects;
using StardewValley.Menus;
using System.Collections.Generic;
using System;

namespace AnythingAnywhere.Framework.Patches.StandardObjects
{
    internal class MiniJukeboxPatch : PatchTemplate
    {
        private readonly Type _object = typeof(MiniJukebox);

        internal MiniJukeboxPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }
        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_object, nameof(MiniJukebox.checkForAction), new[] { typeof(Farmer), typeof(bool) }), prefix: new HarmonyMethod(GetType(), nameof(CheckForActionPrefix)));
        }

        // Enable jukebox functionality outside of the farm
        private static bool CheckForActionPrefix(MiniJukebox __instance, Farmer who, ref bool __result, bool justCheckingForActivity = false)
        {
            if (!ModEntry.modConfig.EnableJukeboxFunctionality)
            {
                return true; //run the original method
            }
            if (justCheckingForActivity)
            {
                __result = true;
                return false;
            }
            GameLocation location = __instance.Location;
            if (location.IsOutdoors && location.IsRainingHere())
            {
                Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:Mini_JukeBox_OutdoorRainy"));
            }
            else
            {
                List<string> jukeboxTracks = Utility.GetJukeboxTracks(Game1.player, Game1.player.currentLocation);
                jukeboxTracks.Insert(0, "turn_off");
                jukeboxTracks.Add("random");
                Game1.activeClickableMenu = new ChooseFromListMenu(jukeboxTracks, __instance.OnSongChosen, isJukebox: true, location.miniJukeboxTrack.Value);
            }
            __result = true;
            return false;
        }
    }
}
