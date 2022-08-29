/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using StardewModdingAPI;
using System.Collections.Generic;
using System.Linq;
using PlatoTK;
using PlatoTK.Reflection;
using HarmonyLib;
using StardewValley;
using System;

namespace MapTK.FestivalSpots
{
    internal class FestivalSpotsHandler
    {
        private static IModHelper Helper;
        internal const string FestivalPlacementDataAsset = @"Data/Festivals/MapTKPlacements";

        public FestivalSpotsHandler(IModHelper helper)
        {
            Helper = helper;
            helper.GetPlatoHelper().Content.Injections.InjectLoad(FestivalPlacementDataAsset, new Dictionary<string, FestivalNPCData>());
            helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;

        }

        private void GameLoop_GameLaunched(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        {
            var harmony = new Harmony("Platonymous.MapTK.FestivalSpots");

            Helper.GetPlatoHelper().Events.CalledEventCommand += Events_CalledEventCommand;

            var api = Helper.ModRegistry.GetApi<PlatoTK.APIs.IContentPatcher>("Pathoschild.ContentPatcher");
            api.RegisterToken(Helper.ModRegistry.Get(Helper.ModRegistry.ModID).Manifest, "FestivalSpots", new FestivalSpotsToken());
        }

        private void Events_CalledEventCommand(object sender, PlatoTK.Events.ICalledEventCommandEventArgs e)
        {
            if(e.Trigger.ToLower() == "loadactors")
            {
                Dictionary<string, FestivalNPCData> npcData = Helper.GameContent.Load<Dictionary<string, FestivalNPCData>>(FestivalPlacementDataAsset);

                npcData.Keys
                    .ToList()
                    .ForEach(key =>
                        npcData[key].Placements
                        .Where(p => (e.Location.mapPath.Value.ToLower().EndsWith(p.Festival.ToLower()) && (p.Phase == "All" || p.Phase.ToLower() == e.Parameter[0].ToLower())))
                        .ToList()
                        .ForEach((fpd) =>
                        {
                            if (npcData[key].NPC == "")
                                npcData[key].NPC = key;
                            e.Event.actors.RemoveAll(a => a.Name == npcData[key].NPC);
                            e.Event.actors.RemoveAll(a => a.getTileX() == fpd.X && a.getTileY() == fpd.Y);
                            
                            if(fpd.X != -1)
                                e.Event.CallAction("addActor", npcData[key].NPC, fpd.X, fpd.Y, fpd.Facing, e.Location);
                        }));
            }
        }
        
        internal void command_loadActors(Event __instance, GameLocation location, string[] split)
        {
               
        }
    }
}
