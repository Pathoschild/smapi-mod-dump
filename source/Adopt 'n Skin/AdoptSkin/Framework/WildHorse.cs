using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;
using StardewValley.Characters;
using StardewValley.Menus;

namespace AdoptSkin.Framework
{
    class WildHorse
    {

        /// <summary>Outdoor locations in which wild horses can spawn</summary>
        internal static List<string> SpawningMaps = new List<string>
        {
            "Forest", "BusStop", "Mountain", "Town", "Railroad", "Beach"
        };
        /// <summary>RNG for selecting randomized aspects</summary>
        private readonly Random Randomizer = new Random();
        /// <summary>The identifying number for a wild horse</summary>
        internal static readonly int WildID = 3000;

        internal Horse HorseInstance;
        public int SkinID { get; }
        public GameLocation Map { get; }
        public Vector2 Tile { get; }



        /// <summary>Creates a new WildHorse instance</summary>
        internal WildHorse()
        {
            // Create WildHorse traits
            SkinID = Randomizer.Next(1, ModEntry.HorseAssets["horse"].Count + 1);
            string mapName = SpawningMaps[Randomizer.Next(0, SpawningMaps.Count)];
            Map = Game1.getLocationFromName(mapName);
            Tile = GetRandomSpawnLocation(Map);

            // Create Horse instance
            HorseInstance = new Horse(new Guid(), (int)Tile.X, (int)Tile.Y)
            {
                Sprite = new AnimatedSprite(ModEntry.GetSkinFromID("horse", SkinID).AssetKey, 7, 32, 32),

                // Temporary WildHorse traits
                Manners = WildID,
                Name = "Wild horse",
                displayName = "Wild horse"
            };

            // Put that thing where it belongs
            Game1.warpCharacter(HorseInstance, Map, Tile);

            if (ModEntry.Config.NotifyHorseSpawn)
            {
                string message = $"A wild horse has been spotted at: {Map.Name} -- {Tile.X}, {Tile.Y}";
                ModEntry.SMonitor.Log(message, LogLevel.Debug);
                Game1.chatBox.addInfoMessage(message);
            }
        }


        /// <summary>Remove this WildHorse's Horse instance from its map</summary>
        internal void RemoveFromWorld()
        {
            if (HorseInstance == null)
                return;

            Game1.removeThisCharacterFromAllLocations(this.HorseInstance);
        }


        /// <summary>Returns a tile from the given map, with a reasonable attempt to be accessible to the player, to spawn the WildHorse at</summary>
        internal static Vector2 GetRandomSpawnLocation(GameLocation map)
        {
            // Make sure the tile is reasonably accessible
            Vector2 randomTile = map.getRandomTile();
            while (!CreationHandler.IsTileAccessible(map, randomTile))
            {
                randomTile = map.getRandomTile();
            }

            return randomTile;
        }
    }
}
