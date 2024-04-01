/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Mizzion.Stardew.Common.Integrations.GenericModConfigMenu;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Extensions;
using Object = StardewValley.Object;

namespace Increased_Artifact_Spots
{
    public class IncreasedArtifactSpots : Mod
    {
        //Number of actual spawned artifact spots
        private int _spawnedSpots;

        //Debug setting
        private bool _debugging;

        //The Mods config
        private ModConfig _config;

        private ITranslationHelper _i18N;

        private IGenericModConfigMenuApi _cfgMenu;

        //Populate location names
        private Dictionary<GameLocation, List<Vector2>> _locations;
        private List<GameLocation> _validLocations;
        private Dictionary<GameLocation, List<Vector2>> _validSpots;

        


        public override void Entry(IModHelper helper)
        {
            //Initiate the config file
            _config = helper.ReadConfig<ModConfig>();
           
            _i18N = Helper.Translation;

            //Set whether or not debugging is enabled
            _debugging = false;
            
            _validSpots = new Dictionary<GameLocation, List<Vector2>>();
            
            //Set up new Console Command
            helper.ConsoleCommands.Add("artifacts", "Shows how many Artifact Spots were spawned per location..\n\nUsage: artifacts <value>\n- value: can be all, or a location name.", ShowSpots);
            helper.ConsoleCommands.Add("destroy_artifacts", "Will destroy all Artifact Spots.\n\nUsage: destroy_artifacts <value>\n- value: can be all, or modded.", DestroySpots);
            helper.ConsoleCommands.Add("artifacts_reload_spots",
                "Will re-populate the valid artifact spots. Use if you use custom maps.", ReloadMaps);

            
            if (_debugging)
            {
                helper.ConsoleCommands.Add("spawn_artifacts", "Debug command. Will Spawn new spots.\n\nUsage: destroy_artifacts", SpawnArtiSpots);
                
            }
            //Events
            helper.Events.GameLoop.GameLaunched += GameLaunched;
            helper.Events.GameLoop.DayStarted += DayStarted;
            helper.Events.Input.ButtonPressed += ButtonPressed;
        }

       //Event Methods
       private void GameLaunched(object sender, GameLaunchedEventArgs e)
       {
           _cfgMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
           if (_cfgMenu is null) return;

           //Register mod
           _cfgMenu.Register(
               mod: ModManifest,
               reset: () => _config = new ModConfig(),
               save: () => Helper.WriteConfig(_config)
           );

           _cfgMenu.AddSectionTitle(
               mod: ModManifest,
               text: () => _i18N.Get("config_mod_name"),
               tooltip: null
           );

           _cfgMenu.AddNumberOption(
               mod: ModManifest,
               getValue: () => _config.AverageArtifactSpots,
               setValue: value => _config.AverageArtifactSpots = value,
               name: () => _i18N.Get("config_artifact_average_artifact_spots_text"),
               tooltip: () => _i18N.Get("config_artifact_average_artifact_spots_description")
           );/*
           _cfgMenu.AddBoolOption(
               mod: ModManifest,
               getValue: () => _config.ForceAverageArtifacts,
               setValue: value => _config.ForceAverageArtifacts = value,
               name: () => _i18N.Get("config_artifact_force_average_spawn_spots_text"),
               tooltip: () => _i18N.Get("config_artifact_force_average_spawn_spots_description")
           );*/
           _cfgMenu.AddBoolOption(
               mod: ModManifest,
               getValue: () => _config.ShowSpawnedNumArtifactSpots,
               setValue: value => _config.ShowSpawnedNumArtifactSpots = value,
               name: () => _i18N.Get("config_artifact_show_spawned_spots_text"),
               tooltip: () => _i18N.Get("config_artifact_show_spawned_spots_description")
           );
           _cfgMenu.AddBoolOption(
               mod: ModManifest,
               getValue: () => _config.SpawnArtifactsOnFarm,
               setValue: value => _config.SpawnArtifactsOnFarm = value,
               name: () => _i18N.Get("config_artifact_spawn_farm_text"),
               tooltip: () => _i18N.Get("config_artifact_spawn_farm_description")
           );
       }

       private void DayStarted(object sender, DayStartedEventArgs e)
       {
           _validLocations = new List<GameLocation>();

           GetValidLocations();

           if(!_config.ValidDefaultSpots.Locations.Any())
               GrabValidSpawnPoints();
           
           SpawnSpots();
       }

       private void ButtonPressed(object sender, ButtonPressedEventArgs e)
       {
           if (!Context.IsWorldReady)
               return;

           if (e.IsDown(SButton.F5))
           {
               _config = Helper.ReadConfig<ModConfig>();
               Monitor.Log("The Configuration file was reloaded.");
           }

           if (e.IsDown(SButton.NumPad4) && _debugging)
           {
               SpawnSpots();
           }

           if (e.IsDown(SButton.RightShift) && _debugging)
           {
               if (!_validLocations.Any())
                   return;
               
               foreach (var l in _validLocations)
               {
                   Monitor.Log($"Location: {l.DisplayName}");
               }
           }
               
       }
       
       //Private Methods

       private void SpawnSpots()
       {
           _spawnedSpots = 0;
           _locations = new Dictionary<GameLocation, List<Vector2>>();
           var rnd = Utility.CreateDaySaveRandom();
           
           //Show artifact spots daily
           if(_config.ShowSpawnedNumArtifactSpots)
               Game1.showGlobalMessage(_i18N.Get("artifact_start"));

           

           foreach (var location in _validLocations)
           {
               var validCoord = _config.ValidDefaultSpots.Locations.Where(obj => obj.Key == location.Name);

               var triesToDo = _config.AverageArtifactSpots - _spawnedSpots <= 0
                   ? 0
                   : rnd.Next(_config.AverageArtifactSpots - _spawnedSpots);

               var triesDone = 0;
               var totalSpawnedMap = 0;

               foreach (var c in validCoord)
               {
                   var spawned = new List<Vector2>();

                   if (c.Value.Count < triesToDo)
                       triesToDo = c.Value.Count;
                   
                   while (triesDone < triesToDo && c.Key == location.Name)
                   {
                       var coo = c.Value;
                       var coo1 = rnd.Next(0, coo.Count - 1);
                       var pickedCoord = coo[coo1];

                       if (!location.IsTileOccupiedBy(pickedCoord))
                       {
                           location.objects.TryAdd(pickedCoord, ItemRegistry.Create<Object>("(O)590"));
                           spawned.Add(pickedCoord);
                           _spawnedSpots++;
                           totalSpawnedMap++;
                       }
                       triesDone++;
                   }
                   _locations.TryAdd(location, spawned);
               }
               if(_debugging)
                   Monitor.Log($"Spawned: {totalSpawnedMap} artifact spots in map: {location.DisplayName}. TriesToDo: {triesToDo}, TriesDone: {triesDone} Total Spawned: {_spawnedSpots}");
               
               Monitor.Log($"Spawned: {totalSpawnedMap} artifact spots in map: {location.DisplayName}.", LogLevel.Info);
           }
           
           if (_config.ShowSpawnedNumArtifactSpots)
               Game1.showGlobalMessage(_i18N.Get("artifact_spawned", new { artifact_spawns = _spawnedSpots }));
           
       }
       
       
       private void SpawnArtiSpots(string command, string[] args)
       {
           if (args.Length < 1)
           {
               return;
           }
           _spawnedSpots = 0;
           _locations = new Dictionary<GameLocation, List<Vector2>>();
           var rnd = Utility.CreateDaySaveRandom();

           var arg = args[0];
           
           //Show artifact spots daily
           if(_config.ShowSpawnedNumArtifactSpots)
               Game1.showGlobalMessage(_i18N.Get("artifact_start"));

           switch (arg)
           {
               case "all":
                   foreach (var locs in _validLocations)
                   {
                       var spawned = 0;
                       var validCoord = _config.ValidDefaultSpots.Locations.Where(obj => obj.Key == locs.Name);

                       foreach (var c in validCoord)
                       {
                           if (c.Key != locs.Name) continue;
                           foreach (var co in c.Value.Where(co => !locs.IsTileOccupiedBy(co, CollisionMask.All, CollisionMask.None, false)))
                           {
                               locs.objects.TryAdd(co, ItemRegistry.Create<Object>("(O)590"));
                               spawned++;
                               _spawnedSpots++;
                           }
                       }
                       if(_debugging)
                           Monitor.Log($"Spawned: {spawned} artifact spots in map: {locs.DisplayName}", LogLevel.Info);
                   }
                   break;
               
               default:
                   Monitor.Log("No No");
                   break;
           }
           
           if (_config.ShowSpawnedNumArtifactSpots)
               Game1.showGlobalMessage(_i18N.Get("artifact_spawned", new { artifact_spawns = _spawnedSpots }));
           
       }
       private void DestroySpots(string command, string[] args)
        {
            if (args.Length < 1)
            {
                return;
            }

            var arg = args[0];

            switch (arg)
            {
                case "all":
                    foreach (var loc2 in Game1.locations)
                    {
                        var numRemoved2 = 0;
                        var artifactsFound = loc2.objects.Pairs.Where(obj => obj.Value.QualifiedItemId == "(O)590");
                        foreach (var art in artifactsFound)
                        {
                            loc2.objects.Remove(art.Key);
                            numRemoved2++;
                        }
                        if (numRemoved2 > 0)
                        {
                            Monitor.Log($"Removed {numRemoved2} artifact spots from {loc2.DisplayName}.", LogLevel.Info);
                        }
                    }
                    _locations.Clear();
                    break;
                case "modded":
                    foreach (var loc in Game1.locations)
                    {
                        var numRemoved = 0;
                        var found = _locations.Where(obj => obj.Key.Equals(loc));
                        foreach (var item in found)
                        {
                            var artFound = item.Key.objects.Pairs.Where(obj => obj.Value.QualifiedItemId == "(O)590");
                            foreach (var artt in artFound)
                            {
                                loc.objects.Remove(artt.Key);
                                numRemoved++;
                            }
                        }
                        if (numRemoved > 0)
                        {
                            Monitor.Log($"Removed {numRemoved} artifact spots from {loc.DisplayName}", (LogLevel)2);
                        }
                    }
                    _locations.Clear();
                    break;
                case "debug":
                    foreach (var validSpots in _validSpots)
                    {
                        var i = validSpots.Value;
                        var spotLoc = i.Aggregate("", (current, vSpot) => current + $", (X: {vSpot.X}, Y: {vSpot.Y})");
                        Monitor.Log("Map: " + validSpots.Key.DisplayName + " Spot: " + spotLoc);
                    }
                    _locations.Clear();
                    break;
                default:
                    Monitor.Log("Command must include all, modded, or debug.");
                    break;
            }
        }
       
       private void ShowSpots(string command, string[] args)
        {
            if(args.Length < 1) return;
            var arg = args[0];
            var spawns = new Dictionary<string, int>();
            

            switch (arg)
            {
                case "all":
                    foreach (var loc in Game1.locations)
                    {
                        var artifactsFound = loc.objects.Pairs.Count(obj => obj.Value.QualifiedItemId == "(O)590");

                        if (artifactsFound == 0)
                            continue;
                        
                        var spawnCoords = "";

                        foreach (var spots in loc.objects.Pairs.Where(obj => obj.Value.QualifiedItemId == "(O)590"))
                        {
                            spawnCoords += spawnCoords.Length > 1 ? $"(X:{spots.Key.X}, Y:{spots.Key.Y}), ".TrimEnd(spawnCoords[^2]) : $"(X:{spots.Key.X}, Y:{spots.Key.Y}), ";
                        }
                        Monitor.Log($"{loc.DisplayName} has {artifactsFound} Artifacts. They are at: {spawnCoords}");
                    }
                    /*
                    if (spawns.Count != 0)
                    {
                        foreach (var spawn in spawns)
                        {
                            Monitor.Log($"{spawn.Key}: {spawn.Value}", LogLevel.Info);
                        }
                    }*/
                    break;
                case "modded":
                    foreach (var loc in Game1.locations)
                    {
                        var found = _locations.Count(obj => obj.Key.Equals(loc));

                        if (found != 0)
                            spawns.TryAdd(loc.Name, found);
                    }
                    if (spawns.Count != 0)
                    {
                        foreach (var spawn in spawns)
                        {
                            Monitor.Log($"{spawn.Key}: {spawn.Value}", LogLevel.Info);
                        }
                    }
                    break;
                case "debug":
                    foreach (var loc in Game1.locations)
                    {
                        var artifactsFound = loc.objects.Pairs.Count(obj => obj.Value.QualifiedItemId == "(O)590");

                        if (artifactsFound == 0)
                            continue;
                        
                        var spawnCoords = "";

                        foreach (var spots in loc.objects.Pairs.Where(obj => obj.Value.QualifiedItemId == "(O)590"))
                        {
                            spawnCoords += spawnCoords.Length > 1 ? $"(X:{spots.Key.X}, Y:{spots.Key.Y}), ".TrimEnd(spawnCoords[^2]) : $"(X:{spots.Key.X}, Y:{spots.Key.Y}), ";
                        }
                        Monitor.Log($"{loc.DisplayName} has {artifactsFound} Artifacts. They are at: {spawnCoords}");
                    }
                    break;
                default:
                    Monitor.Log("command must include all, modded or debug.");
                    break;
            }
        }


        private void ReloadMaps(string command, string[] args)
        {
            if (_config.ValidDefaultSpots.Locations.Any())
            {
                _config.ValidDefaultSpots.Locations.Clear();
                GrabValidSpawnPoints();
                Monitor.Log("Valid Artifact Spots was reloaded.", LogLevel.Info);
            }
        }
       private void GetValidLocations()
       {
           foreach (var loc in Game1.locations)
           {
               var locationData = loc.GetData(); 

               if(_debugging)
                   Monitor.Log($"Location to be added was {loc.DisplayName}");
               
               if (!loc.IsOutdoors ||
                   locationData is null ||
                   locationData.ArtifactSpots.Count < 1 ||
                   (loc.Name.Contains("Desert") && !Game1.MasterPlayer.mailReceived.Contains("ccVault")) ||
                   (loc.Name.Contains("Island") && !Game1.MasterPlayer.hasCompletedCommunityCenter()) ||
                   ((loc.Name.Contains("Mountain") || loc.Name.Contains("Railroad")) && Game1.stats.DaysPlayed < 31) ||
                   (loc.Name.Contains("Secret")))
                   continue;

               if (!_config.SpawnArtifactsOnFarm && loc.IsFarm)
                   continue;


               if (_validLocations.Contains(loc) || !_config.ValidDefaultSpots.Locations.ContainsKey(loc.Name)) continue;
               
               
               if(_debugging)
                   Monitor.Log($"Location to be added is {loc.DisplayName}");
                   
               _validLocations.Add(loc);

           }
       }

       private void GrabValidSpawnPoints()
       {
           _validSpots.Clear();

           
           var r = Utility.CreateDaySaveRandom();
           
           //Now we Go through all maps and grab valid SpawnSections.

           if (_debugging)
               Monitor.Log($"Starting to gather valid spawn points.");

          
           foreach (var location in Game1.locations)
           {
               var coords = new List<Vector2>();
               for (var x = 0; x < location.Map.Layers[0].LayerWidth/*location.map.DisplayWidth / 64*/; x++)
               {
                   for (var y = 0; y < location.Map.Layers[0].LayerHeight/*location.map.DisplayHeight / 64*/; y++)
                   {
                       
                       if(location.Name.Equals("Forest") && x >= 93f && y <= 22f)
                           continue;
                   

                       var step1 = !location.IsNoSpawnTile(new Vector2(x, y));
                       var step2 = location.doesTileHaveProperty(x, y, "Spawnable", "Back") != null;
                       var step3 = location.doesTileHaveProperty(x, y, "Diggable", "Back") != null;
                       var step4 = location.doesTileHaveProperty(x, y, "Diggable", "Back") == "T";
                       var step5 = location.doesTileHavePropertyNoNull(x, y, "Type", "Back").Equals("Dirt");
                       var step6 = !location.doesEitherTileOrTileIndexPropertyEqual(x, y, "Spawnable",
                           "Back", "F");
                       var step7 = location.getTileIndexAt(x, y, "AlwaysFront2") == -1 ;
                       var step8 = location.getTileIndexAt(x, y, "AlwaysFront3") == -1;
                       var step9 = location.getTileIndexAt(x, y, "Front") == -1;
                       var step10 = !location.isBehindBush(new Vector2(x, y));
                       var step11 = !r.NextBool(0.1) && !location.isBehindTree(new Vector2(x, y));
                       var step12 = location.isTilePassable(new Vector2(x, y));


                       if (step1 &&/* step2 &&*/ step3 && step4 && step5 && step6 && step7 && step8 && step9 && step10 &&
                           step11 && step12)
                       {
                          
                           coords.Add(new Vector2(x, y));
                           if(_debugging)
                               Monitor.Log($"Added X:{x}, Y:{y} to {location.DisplayName}");
                       }
                   }
               }

               if (coords.Any())
               {
                   if(!_config.ValidDefaultSpots.Locations.ContainsKey(location.DisplayName))
                    _config.ValidDefaultSpots.Locations.Add(location.Name, coords);
                   //_validSpots.TryAdd(location, coords);
                   Helper.WriteConfig(_config);
               }
                
               //coords.Clear();
           }
           if (_debugging)
               Monitor.Log($"Finished gathering valid spawn points.");
       }
    }
}
