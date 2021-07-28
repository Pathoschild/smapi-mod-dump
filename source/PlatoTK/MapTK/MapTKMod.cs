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
using MapTK.SpouseRooms;
using MapTK.Locations;
using MapTK.MapExtras;
using MapTK.FestivalSpots;
using System.Collections.Generic;
using MapTK.TileActions;

namespace MapTK
{
    public class MapTKMod : Mod
    {
        internal static LocationsHandler LocationsHandler;
        internal static MapExtrasHandler MapExtrasHandler;
        internal static SpouseRoomHandler SpouseRoomHandler;
        internal static FestivalSpotsHandler FestivalSpotsHandler;
        internal static TileActionsHandler TileActionsHandler;
        internal static readonly List<string> CompatOptions = new List<string>();

        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.GameLaunched += SetCompatOptions;
            LocationsHandler = new LocationsHandler(helper);
            MapExtrasHandler = new MapExtrasHandler(helper);
            SpouseRoomHandler = new SpouseRoomHandler(helper);
            FestivalSpotsHandler = new FestivalSpotsHandler(helper);
            TileActionsHandler = new TileActionsHandler(helper);
            helper.Content.AssetLoaders.Add(new GameAssetLoader(helper));
        }

        private void SetCompatOptions(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        {
            if (Helper.ModRegistry.IsLoaded("DigitalCarbide.SpriteMaster"))
                CompatOptions.Add("SpriteMaster");
        }

        public override object GetApi()
        {
            return new MapTK.Api.MapTKAPI();
        }
    }
}
