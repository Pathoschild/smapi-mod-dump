using System;
using System.Collections.Generic;
using System.IO;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;


namespace bwdyworks
{
    public class MapLoader : IAssetLoader
    {
        private Mod Mod;
        private Dictionary<string, string> mapReplacements = new Dictionary<string, string>();
        private List<GameLocation> mapAdditions = new List<GameLocation>();
        private readonly string MapDirectory = Path.Combine("Assets", "Maps");

        public MapLoader(Mod mod)
        {
            Mod = mod;
            Mod.Helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;
            Mod.Helper.Content.AssetLoaders.Add(this);
        }

        private void GameLoop_SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            foreach (var map in mapAdditions)
            {
                Mod.Monitor.Log("Added map " + map.Name, LogLevel.Trace);
                Game1.locations.Add(map);
            }
        }

        private GameLocation LoadMap(string mapName, string tbin, bool outdoors = true, bool farm = false)
        {
            tbin = Path.Combine(MapDirectory, tbin);
            var asset = Mod.Helper.Content.GetActualAssetKey(tbin);
            Mod.Helper.Content.Load<xTile.Map>(tbin);
            return new GameLocation(asset, mapName) { IsOutdoors = outdoors, IsFarm = farm };
        }

        public void ReplaceMap(string mapName, string tbin)
        {
            mapReplacements.Add(mapName, tbin);
        }

        public void AddMap(string mapName, string tbin, bool outdoors = true, bool farm = false)
        {
            mapAdditions.Add(LoadMap(mapName, tbin, outdoors, farm));
        }

        public void AddWarp(string FromMapName, int FromX, int FromY, string ToMapName, int ToX, int ToY)
        {
            
        }

        public bool CanLoad<T>(IAssetInfo asset)
        {
            if (!asset.AssetName.Contains("\\")) return false;
            string test = asset.AssetName.Split('\\')[1];
            if (mapReplacements.ContainsKey(test)) return true;
            return false;
        }

        public T Load<T>(IAssetInfo asset)
        {
            foreach (var map in mapReplacements)
            {
                string test = asset.AssetName.Split('\\')[1];
                if (map.Key == test)
                {
                    Mod.Monitor.Log("Replaced map " + asset.AssetName + " with " + Path.Combine(MapDirectory, map.Value), LogLevel.Trace);
                    return Mod.Helper.Content.Load<T>(Path.Combine(MapDirectory, map.Value));
                }
            }
            throw new InvalidOperationException($"Unexpected asset '{asset.AssetName}'.");
        }

    }
}
