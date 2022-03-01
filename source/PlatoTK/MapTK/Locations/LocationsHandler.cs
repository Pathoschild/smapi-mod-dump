/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using HarmonyLib;
using PlatoTK;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Locations;
using StardewValley.Monsters;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace MapTK.Locations
{
    internal class LocationsHandler
    {
        const string LocationSaveData = @"MapTK.SaveData.Locations";
        internal const string LocationsDictionary = @"MapTK/Locations";
        internal static IModHelper Helper;
        internal readonly static List<GameLocation> locationStore = new List<GameLocation>();
        readonly static Type[] ExtraTypes = new Type[24]
        {
            typeof(Tool),
            typeof(Duggy),
            typeof(Ghost),
            typeof(GreenSlime),
            typeof(LavaCrab),
            typeof(RockCrab),
            typeof(ShadowGuy),
            typeof(Child),
            typeof(Pet),
            typeof(Dog),
            typeof(Cat),
            typeof(Horse),
            typeof(SquidKid),
            typeof(Grub),
            typeof(Fly),
            typeof(DustSpirit),
            typeof(Bug),
            typeof(BigSlime),
            typeof(BreakableContainer),
            typeof(MetalHead),
            typeof(ShadowGirl),
            typeof(Monster),
            typeof(JunimoHarvester),
            typeof(TerrainFeature)
        };

        readonly static XmlWriterSettings SaveWriterSettings = new XmlWriterSettings()
        {
            ConformanceLevel = ConformanceLevel.Auto,
            CloseOutput = true
        };

        readonly static XmlReaderSettings SaveReaderSettings = new XmlReaderSettings()
        {
            ConformanceLevel = ConformanceLevel.Auto,
            CloseInput = true
        };

        public LocationsHandler(IModHelper helper)
        {
            Helper = helper;

            helper.GetPlatoHelper().Content.Injections.InjectLoad(LocationsDictionary, new Dictionary<string, LocationData>());


            helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
            helper.Events.GameLoop.Saving += GameLoop_Saving;
            helper.Events.GameLoop.Saved += GameLoop_Saved;
        }

        private void GameLoop_Saved(object sender, SavedEventArgs e)
        {
            foreach (var loc in locationStore)
                Game1.locations.Add(loc);

            locationStore.Clear();
        }

        private void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var api = Helper.ModRegistry.GetApi<PlatoTK.APIs.IContentPatcher>("Pathoschild.ContentPatcher");
            api.RegisterToken(Helper.ModRegistry.Get(Helper.ModRegistry.ModID).Manifest, "Locations", new LocationsToken());

            Harmony instance = new Harmony("Platonymous.MapTK.AddLocations");
            instance.Patch(AccessTools.Method(typeof(SaveGame),nameof(SaveGame.loadDataToLocations)), prefix: new HarmonyMethod(typeof(LocationsHandler),nameof(GameLocationsPatch)));

            instance.Patch(AccessTools.Method(typeof(NPC), nameof(NPC.populateRoutesFromLocationToLocationList)), prefix: new HarmonyMethod(typeof(LocationsHandler), nameof(SetLocationsBeforeRoutes)));
        }

        internal static void SetLocationsBeforeRoutes()
        {
            Helper.Content.InvalidateCache(LocationsDictionary);
            var locations = Helper.Content.Load<Dictionary<string, LocationData>>(LocationsDictionary, ContentSource.GameContent);
            var result = new List<GameLocation>();
            locations.Values
                .Where(l => !Game1.locations.Any(g => g.Name == l.Name))
                .ToList()
                .ForEach(l =>
                {
                    var newLocation = GetNewLocation(l);
                    Game1.locations.Add(newLocation);
                    result.Add(newLocation);
                });
        }

        internal static void GameLocationsPatch(List<GameLocation> gamelocations)
        {
            foreach (var location in InitializeNewLocations())
                if (!gamelocations.Any(l => l.Name == location.Name))
                    gamelocations.Add(location);
        }

        private static List<GameLocation> InitializeNewLocations()
        {
            Helper.Content.InvalidateCache(LocationsDictionary);
            var locations = Helper.Content.Load<Dictionary<string, LocationData>>(LocationsDictionary, ContentSource.GameContent);
            var result = new List<GameLocation>();
            locations.Values
                .Where(l => !Game1.locations.Any(g => g.Name == l.Name) || l.Save)
                .ToList()
                .ForEach(l =>
                {
                    if (l.Save && Game1.locations.FirstOrDefault(g => g.Name == l.Name) is GameLocation gl)
                        Game1.locations.Remove(gl);

                    var newLocation = GetNewLocation(l);
                    Game1.locations.Add(newLocation);
                    result.Add(newLocation);
                });

            return result;
        }

        internal static GameLocation GetNewLocation(LocationData data)
        {
            string type = data.Type.ToLower();

            if (data.Save)
                try
                {
                    if (Helper.Data.ReadSaveData<LocationSaveData>($"{LocationSaveData}") is LocationSaveData saveDataStore
                        && saveDataStore.Locations.ContainsKey(data.Name) && saveDataStore.Locations[data.Name] is string savedata
                        && !string.IsNullOrEmpty(savedata))
                    {
                        XmlSerializer serializer = null;

                        if (Type.GetType(data.Type) is Type customType)
                            serializer = new XmlSerializer(customType, ExtraTypes);
                        else
                            serializer = new XmlSerializer(typeof(GameLocation), ExtraTypes);

                        using (StringReader dataReader = new StringReader(savedata))
                        using (var reader = XmlReader.Create(dataReader, SaveReaderSettings))
                            if (serializer.Deserialize(reader) is GameLocation savedLocation)
                                return savedLocation;
                    }
                }
                catch
                {

                }

            GameLocation result;

            switch (type)
            {
                case "buildable": result = new BuildableGameLocation(data.MapPath, data.Name); break;
                case "decoratable": result = new DecoratableLocation(data.MapPath, data.Name); break;
                case "default": result = new GameLocation(data.MapPath, data.Name); break;
                default:
                    {
                        if (Type.GetType(data.Type) is Type customType
                            && Activator.CreateInstance(customType, data.MapPath, data.Name) is GameLocation customLocation)
                            result = customLocation;
                        else
                            result = new GameLocation(data.MapPath, data.Name);
                        break;
                    }
            }

            if (result == null)
                return result;

            if (data.Farm)
                result.isFarm.Set(true);

            if (data.Greenhouse)
                result.isGreenhouse.Set(true);

            if(data.Season != "auto")
                if (result?.Map.Properties.TryGetValue("SeasonOverride", out xTile.ObjectModel.PropertyValue value) ?? false)
                    result.Map.Properties["SeasonOverride"] = data.Season;
                else
                    result.Map.Properties.Add("SeasonOverride", data.Season);

            try
            {
                
                if (data.Save
                    && result is GameLocation
                    && Helper.ModRegistry.IsLoaded("Platonymous.TMXLoader")
                    && !Helper.ModRegistry.Get("Platonymous.TMXLoader").Manifest.Version.IsOlderThan("1.20.0")
                    && Helper.ModRegistry.GetApi<ITMXLAPI>("Platonymous.TMXLoader") is ITMXLAPI api
                    && api.TryGetSaveDataForLocation(result, out GameLocation tmxresult))
                    return tmxresult;
            }
            catch
            {

            }

            return result;
        }

        private void GameLoop_Saving(object sender, SavingEventArgs e)
        {
            locationStore.Clear();
            var locationDataStore = new LocationSaveData();
            var locations = Helper.Content.Load<Dictionary<string, LocationData>>(LocationsDictionary, ContentSource.GameContent);
            Helper.Content.Load<Dictionary<string, LocationData>>(LocationsDictionary, ContentSource.GameContent)
                .Where(l => l.Value.Save && Game1.getLocationFromName(l.Value.Name) is GameLocation)
                .Select(l => Game1.getLocationFromName(l.Value.Name))
                .ToList()
                .ForEach((location) =>
                {
                    try
                    {
                        using (StringWriter dataWriter = new StringWriter())
                        using (var writer = XmlWriter.Create(dataWriter, SaveWriterSettings))
                        {
                            XmlSerializer serializer = new XmlSerializer(location.GetType(), ExtraTypes);
                            serializer.Serialize(writer, location);
                            string savedata = dataWriter.ToString();
                            
                            locationDataStore.Locations.Add(location.Name, savedata);
                        }
                    }
                    catch
                    {

                    }
                });



            locations.Where(ls => Game1.getLocationFromName(ls.Value.Name) is GameLocation l)
                .ToList().ForEach(ls =>
                {
                    var loc = Game1.getLocationFromName(ls.Value.Name);
                    locationStore.Add(loc);
                    Game1.locations.Remove(loc);
                });



            Helper.Data.WriteSaveData($"{LocationSaveData}", locationDataStore);
        }
    }
}
