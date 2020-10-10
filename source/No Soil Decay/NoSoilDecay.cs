/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ThatNorthernMonkey/NoSoilDecay
**
*************************************************/

using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Storm.ExternalEvent;
using Storm.StardewValley.Event;
using Storm.StardewValley.Wrapper;
using Microsoft.Xna.Framework;
using System.IO;
using Storm.StardewValley.Accessor;
using System;
using System.Text.RegularExpressions;

namespace NoSoilDecay
{
    [Mod]
    public class NoSoilDecay : DiskResource
    {
        public JsonTerrainFeatures JsonTerrainFeatures { get; set; }
        public int TimeLastSaved { get; set; }
        public int TimeToSaveNext { get; set; }
        public bool HasDeserializedToday { get; set; }
        public string TileDecayJsonFilePath { get; set; }
        public string CurrentGameId { get; set; }
        public bool IsFirstRun { get; set; }
        public int TimeOfDay { get; set; }




        public NoSoilDecay()
        {
            JsonTerrainFeatures = new JsonTerrainFeatures();
            IsFirstRun = false;
            HasDeserializedToday = false;
        }

        [Subscribe]
        public void Initialize(InitializeEvent @e)
        {
            

        }

        [Subscribe]
        public void DeserializeTerrainFeaturesAfterNewDay(PostNewDayEvent @e)
        {
            HasDeserializedToday = false;
        }

        [Subscribe]
        public void HaveSoilDecaySlowerOnNewDay(PostUpdateEvent @e)
        {

        }

        [Subscribe]
        public void HaveSoilDecaySlowerOnNewDay(PreUpdateEvent @e)
        {

            if (@e.Root.HasLoadedGame)
            {
                // Load the saved state the first time exiting the farmhouse of the day.
                if (!HasDeserializedToday && @e.Location.Name == "Farm")
                {
                    var listOfAtts = new List<string>();
                    var eyeColour = @e.Root.Player.EyeColor.ToString();
                    var hairColour = @e.Root.Player.HairstyleColor.ToString();
                    var hair = @e.Root.Player.Hair.ToString();
                    string name = @e.Root.Player.Name;
                    var farmName = @e.Root.Player.FarmName;

                    listOfAtts.Add(eyeColour);
                    listOfAtts.Add(hairColour);
                    listOfAtts.Add(hair);
                    listOfAtts.Add(name);
                    listOfAtts.Add(farmName);

                    StringBuilder fileName = new StringBuilder();

                    foreach (var l in listOfAtts)
                    {
                        fileName.Append(l);
                    }

                    Regex rgx = new Regex("[^a-zA-Z0-9 -]");
                    var finalFileName = rgx.Replace(fileName.ToString(), "");

                    finalFileName = finalFileName.Replace(" ", string.Empty);
                    finalFileName = finalFileName.ToLower();

                    CurrentGameId = finalFileName;

                    TileDecayJsonFilePath = Path.Combine(ParentPathOnDisk + "\\NoSoilDecay\\SavedTiles\\", CurrentGameId + ".json");
                    CheckOrCreateJsonFile();

                    try
                    {
                        var locations = @e.Root.Locations;

                        for (int i = 0; i < @e.Root.Locations.Count; i++)
                        {
                            var loc = locations[i];

                            if (loc.Name == "Farm")
                            {
                                JsonTerrainFeatures.HoeDirtTile = DeserializeSavedTerrain();

                                if (JsonTerrainFeatures.HoeDirtTile != null)
                                {
                                    var tFeats = @e.Location.TerrainFeatures;
                                    var dirtToCreate = new List<Vector2>();

                                    foreach (var j in JsonTerrainFeatures.HoeDirtTile)
                                    {
                                        var location = j.Key;
                                        if (!tFeats.ContainsKey(location))
                                        {
                                            dirtToCreate.Add(location);
                                        }
                                    }

                                    foreach (var d in dirtToCreate)
                                    {
                                        @e.Location.AddHoeDirtAt(d);
                                    }

                                    var tFeatsTest = @e.Location.TerrainFeatures;

                                    foreach (var j in JsonTerrainFeatures.HoeDirtTile)
                                    {
                                        foreach (var d in dirtToCreate)
                                        {
                                            if (j.Key == d)
                                            {
                                                tFeatsTest[d].As<HoeDirtAccessor, HoeDirt>().Fertilizer = j.Value;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        HasDeserializedToday = true;
                    }

                    catch (Exception exc)
                    {

                    }

                }
            }

            // Save state every 10 in-game minutes
            TimeOfDay = @e.Root.TimeOfDay;
            if (TimeLastSaved == 0)
            {
                TimeLastSaved = TimeOfDay;
                TimeToSaveNext = TimeLastSaved + 10;
            }

            if (TimeOfDay == 600)
            {
                TimeLastSaved = @e.Root.TimeOfDay;
                TimeToSaveNext = TimeLastSaved + 10;
            }

            if (@TimeOfDay >= TimeToSaveNext && !IsTimeWonky())
            {
                TimeLastSaved = @TimeOfDay;
                TimeToSaveNext = TimeLastSaved + 10;

                //Perform the save.
                if (@e.Location.Name == "Farm")
                {
                    var tFeats = @e.Location.TerrainFeatures;
                    foreach (var k in tFeats.Keys)
                    {
                        if (tFeats[k].Is<HoeDirtAccessor>())
                        {
                            var temp = tFeats[k].As<HoeDirtAccessor, HoeDirt>();
                            if (temp.Crop == null)
                            {
                                // If HoeDirt doesnt contain a key that TerrainFeatures does, a HoeDirt has been created and needs to be added.
                                if (!JsonTerrainFeatures.HoeDirtTile.ContainsKey(k))
                                {
                                    JsonTerrainFeatures.HoeDirtTile.Add(k, tFeats[k].As<HoeDirtAccessor, HoeDirt>().Fertilizer);
                                }
                                else if (JsonTerrainFeatures.HoeDirtTile != null)
                                {
                                    // If the HoeDirt has a fertilizer, set its value here.
                                    JsonTerrainFeatures.HoeDirtTile[k] = tFeats[k].As<HoeDirtAccessor, HoeDirt>().Fertilizer;
                                }

                                // If HoeDirt contains key that TerrainFeatures doesn't, player has destroyed the tile. Remove from HoeDirt.
                                var dirtToRemove = new List<Vector2>();

                                if (JsonTerrainFeatures.HoeDirtTile != null)
                                {
                                    foreach (var h in JsonTerrainFeatures.HoeDirtTile)
                                    {
                                        if (tFeats[h.Key] == null)
                                        {
                                            dirtToRemove.Add(h.Key);
                                        }
                                    }
                                    foreach (var h in dirtToRemove)
                                    {
                                        if (JsonTerrainFeatures.HoeDirtTile.ContainsKey(h))
                                        {
                                            JsonTerrainFeatures.HoeDirtTile.Remove(h);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (JsonTerrainFeatures.HoeDirtTile != null)
                    {
                        SerializeSavedTerrain(JsonTerrainFeatures.HoeDirtTile);
                    }
                }
            }
        }

        private void SerializeSavedTerrain(Dictionary<Vector2, int> terrain)
        {
            var terrainLocation = Path.Combine(TileDecayJsonFilePath);
            File.WriteAllBytes(terrainLocation, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(terrain)));
        }

        private Dictionary<Vector2, int> DeserializeSavedTerrain()
        {
            var terrainLocation = Path.Combine(TileDecayJsonFilePath);
            var jsonTerrainFeatures = new JsonTerrainFeatures().HoeDirtTile;
            jsonTerrainFeatures = JsonConvert.DeserializeObject<Dictionary<Vector2, int>>(Encoding.UTF8.GetString(File.ReadAllBytes(terrainLocation)));

            if (jsonTerrainFeatures == null)
            {
                var newJTFeat = new JsonTerrainFeatures();
                return newJTFeat.HoeDirtTile;
            }
            return jsonTerrainFeatures;
        }

        private void CheckOrCreateJsonFile()
        {
            if (!Directory.Exists(ParentPathOnDisk + "\\NoSoilDecay\\SavedTiles\\"))
            {
                Directory.CreateDirectory(ParentPathOnDisk + "\\NoSoilDecay\\SavedTiles\\");
            }

            if (!File.Exists(Path.Combine(TileDecayJsonFilePath)))
            {
                File.Create(Path.Combine(TileDecayJsonFilePath));
            }
        }

        public bool IsTimeWonky()
        {
            //Bad method name - time is ALWAYS wonky in SV.
            var time = TimeToSaveNext.ToString();
            int hour = (int)(TimeToSaveNext.ToString()[0]);
            time = time.Remove(0, 1);
            var newTime = Int32.Parse(time);
            if (newTime >= 60)
            {
                hour++;
                newTime = 00;
                string timeString = hour.ToString();
                timeString += newTime.ToString();
                TimeToSaveNext = Int32.Parse(timeString);
                return true;
            }
            return false;
        }
    }
}
public class JsonTerrainFeatures
{
    public Dictionary<Vector2, int> HoeDirtTile;
    public long UniqueId { get; set; }
    public JsonTerrainFeatures()
    {
        HoeDirtTile = new Dictionary<Vector2, int>();
    }
}



