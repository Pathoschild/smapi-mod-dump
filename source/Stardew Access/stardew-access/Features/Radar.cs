/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/stardew-access/stardew-access
**
*************************************************/

using Microsoft.Xna.Framework;
using stardew_access.Utils;
using StardewValley;
using StardewValley.Objects;

namespace stardew_access.Features
{
    internal class Radar
    {
        private readonly List<Vector2> closed;
        private readonly List<Furniture> furnitures;
        private readonly List<NPC> npcs;
        public List<string> exclusions;
        private List<string> temp_exclusions;
        public List<string> focus;
        public bool isRunning;
        public bool radarFocus = false;
        public int delay, range;

        public Radar()
        {
            delay = 3000;
            range = 5;

            isRunning = false;
            closed = new List<Vector2>();
            furnitures = new List<Furniture>();
            npcs = new List<NPC>();
            exclusions = new List<string>();
            temp_exclusions = new List<string>();
            focus = new List<string>();

            exclusions.Add("stone");
            exclusions.Add("weed");
            exclusions.Add("twig");
            exclusions.Add("coloured stone");
            exclusions.Add("ice crystal");
            exclusions.Add("clay stone");
            exclusions.Add("fossil stone");
            exclusions.Add("street lamp");
            exclusions.Add("crop");
            exclusions.Add("tree");
            exclusions.Add("flooring");
            exclusions.Add("water");
            exclusions.Add("debris");
            exclusions.Add("grass");
            exclusions.Add("decoration");
            exclusions.Add("bridge");
            exclusions.Add("other");

            /* Not excluded Categories
             * 
             * 
             * exclusions.Add("farmer");
             * exclusions.Add("animal");
             * exclusions.Add("npc");
             * exclusions.Add("furniture")
             * exclusions.Add("building");
             * exclusions.Add("resource clump");
             * exclusions.Add("mine item");
             * exclusions.Add("container"); 
             * exclusions.Add("bundle"); 
             * exclusions.Add("door"); 
             * exclusions.Add("machine"); 
             * exclusions.Add("interactable"); 
             */
        }

        public void Run()
        {
            if (MainClass.radarDebug)
                Log.Debug($"\n\nRead Tile started");

            Vector2 currPosition = Game1.player.getTileLocation();

            closed.Clear();
            furnitures.Clear();
            npcs.Clear();

            SearchNearbyTiles(currPosition, range);

            if (MainClass.radarDebug)
                Log.Debug($"\nRead Tile stopped\n\n");
        }

        /// <summary>
        /// Search the area using Breadth First Search algorithm(BFS).
        /// </summary>
        /// <param name="center">The starting point.</param>
        /// <param name="limit">The limiting factor or simply radius of the search area.</param>
        /// <param name="playSound">True by default if False then it will not play sound and only return the list of detected tiles(for api).</param>
        /// <returns>A dictionary with all the detected tiles along with the name of the object on it and it's category.</returns>
        public Dictionary<Vector2, (string, string)> SearchNearbyTiles(Vector2 center, int limit, bool playSound = true)
        {
            var currentLocation = Game1.currentLocation;
            Dictionary<Vector2, (string, string)> detectedTiles = new();

            Queue<Vector2> toSearch = new();
            HashSet<Vector2> searched = new();
            int[] dirX = { -1, 0, 1, 0 };
            int[] dirY = { 0, 1, 0, -1 };

            toSearch.Enqueue(center);
            searched.Add(center);

            while (toSearch.Count > 0)
            {
                Vector2 item = toSearch.Dequeue();
                if (playSound)
                    CheckTileAndPlaySound(item, currentLocation);
                else
                {
                    (bool, string?, string) tileInfo = CheckTile(item, currentLocation);
                    if (tileInfo.Item1 && tileInfo.Item2 != null)
                    {
                        // Add detected tile to the dictionary
                        detectedTiles.Add(item, (tileInfo.Item2, tileInfo.Item3));
                    }
                }

                for (int i = 0; i < 4; i++)
                {
                    Vector2 dir = new(item.X + dirX[i], item.Y + dirY[i]);

                    if (IsValid(dir, center, searched, limit))
                    {
                        toSearch.Enqueue(dir);
                        searched.Add(dir);
                    }
                }
            }

            searched.Clear();
            return detectedTiles;
        }

        /// <summary>
        /// Search the entire location using Breadth First Search algorithm(BFS).
        /// </summary>
        /// <returns>A dictionary with all the detected tiles along with the name of the object on it and it's category.</returns>
        public static Dictionary<Vector2, (string, string)> SearchLocation()
        {
            //var watch = new Stopwatch();
            //watch.Start();
            var currentLocation = Game1.currentLocation;
            Dictionary<Vector2, (string, string)> detectedTiles = new();
            Vector2 position = Vector2.Zero;
            (bool, string? name, string category) tileInfo;

            Queue<Vector2> toSearch = new();
            HashSet<Vector2> searched = new();
            int[] dirX = { -1, 0, 1, 0 };
            int[] dirY = { 0, 1, 0, -1 };
            int count = 0;

            toSearch.Enqueue(Game1.player.getTileLocation());
            searched.Add(Game1.player.getTileLocation());

            //watch.Stop();
            //var elapsedMs = watch.ElapsedMilliseconds;
            //Log.Debug($"Search init duration: {elapsedMs}");
            //watch.Reset();
            //watch.Start();
            while (toSearch.Count > 0)
            {
                Vector2 item = toSearch.Dequeue();
                tileInfo = CheckTile(item, currentLocation, true);
                if (tileInfo.Item1 && tileInfo.name != null)
                {
                    // Add detected tile to the dictionary
                    detectedTiles.Add(item, (tileInfo.name, tileInfo.category));
                }

                count++;

                for (int i = 0; i < 4; i++)
                {
                    Vector2 dir = new(item.X + dirX[i], item.Y + dirY[i]);

                    if (!searched.Contains(dir) && (TileInfo.IsWarpPointAtTile(currentLocation, (int)dir.X, (int)dir.Y) || currentLocation.isTileOnMap(dir)))
                    {
                        toSearch.Enqueue(dir);
                        searched.Add(dir);
                    }
                }
            }
            //watch.Stop();
            //elapsedMs = watch.ElapsedMilliseconds;
            //Log.Debug($"Search loop duration: {elapsedMs}; {count} iterations.");
            searched.Clear();
            return detectedTiles;
        }

        /// <summary>
        /// Checks if the provided tile position is within the range/radius and whether the tile has already been checked or not.
        /// </summary>
        /// <param name="item">The position of the tile to be searched.</param>
        /// <param name="center">The starting point of the search.</param>
        /// <param name="searched">The list of searched items.</param>
        /// <param name="limit">The radius of search</param>
        /// <returns>Returns true if the tile is valid for search.</returns>
        public static bool IsValid(Vector2 item, Vector2 center, HashSet<Vector2> searched, int limit)
        {
            if (Math.Abs(item.X - center.X) > limit)
                return false;
            if (Math.Abs(item.Y - center.Y) > limit)
                return false;

            if (searched.Contains(item))
                return false;

            return true;
        }

        public static (bool, string? name, string category) CheckTile(Vector2 position, GameLocation currentLocation, bool lessInfo = false)
        {
            (string? name, CATEGORY? category) = TileInfo.GetNameWithCategoryAtTile(position, currentLocation, lessInfo);
            if (name == null)
                return (false, null, CATEGORY.Others.ToString());

            category ??= CATEGORY.Others;

            return (true, name, category.ToString());
            
        }

        public void CheckTileAndPlaySound(Vector2 position, GameLocation currentLocation)
        {
            try
            {
                if (currentLocation.isObjectAtTile((int)position.X, (int)position.Y))
                {
                    (string? name, CATEGORY category) objDetails = TileInfo.GetObjectAtTile(currentLocation, (int)position.X, (int)position.Y);
                    string? objectName = objDetails.name;
                    CATEGORY category = objDetails.category;
                    StardewValley.Object obj = currentLocation.getObjectAtTile((int)position.X, (int)position.Y);

                    if (objectName != null)
                    {
                        objectName = objectName.ToLower().Trim();

                        if (obj is Furniture furniture)
                        {
                            if (!furnitures.Contains(furniture))
                            {
                                furnitures.Add(furniture);
                                PlaySoundAt(position, objectName, category, currentLocation);
                            }
                        }
                        else
                            PlaySoundAt(position, objectName, category, currentLocation);

                    }
                }
                else
                {
                    (string? name, CATEGORY? category) = TileInfo.GetNameWithCategoryAtTile(position, currentLocation);
                    if (name != null)
                    {
                        category ??= CATEGORY.Others;

                        PlaySoundAt(position, name, category, currentLocation);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error($"{e.Message}\n{e.StackTrace}\n{e.Source}");
            }
        }

        public void PlaySoundAt(Vector2 position, string searchQuery, CATEGORY category, GameLocation currentLocation)
        {
            #region Check whether to skip the object or not

            // Skip if player is directly looking at the tile
            if (CurrentPlayer.FacingTile.Equals(position))
                return;

            if (!radarFocus)
            {
                if ((exclusions.Contains(category.ToString().ToLower().Trim()) || exclusions.Contains(searchQuery.ToLower().Trim())))
                    return;

                // Check if a word in searchQuery matches the one in exclusions list
                string[] sqArr = searchQuery.ToLower().Trim().Split(" ");
                for (int j = 0; j < sqArr.Length; j++)
                {
                    if (exclusions.Contains(sqArr[j]))
                        return;
                }
            }
            else
            {
                if (focus.Count >= 0)
                {
                    bool found = false;

                    // Check if a word in searchQuery matches the one in focus list
                    string[] sqArr = searchQuery.ToLower().Trim().Split(" ");
                    for (int j = 0; j < sqArr.Length; j++)
                    {
                        if (focus.Contains(sqArr[j]))
                        {
                            found = true;
                            break;
                        }
                    }

                    // This condition has to be after the for loop
                    if (!found && !(focus.Contains(category.ToString().ToLower().Trim()) || focus.Contains(searchQuery.ToLower().Trim())))
                        return;
                }
                else
                    return;
            }
            #endregion

            if (MainClass.radarDebug)
                Log.Error($"{radarFocus}\tObject:{searchQuery.ToLower().Trim()}\tPosition: X={position.X} Y={position.Y}");

            int px = (int)Game1.player.getTileX(); // Player's X postion
            int py = (int)Game1.player.getTileY(); // Player's Y postion

            int ox = (int)position.X; // Object's X postion
            int oy = (int)position.Y; // Object's Y postion

            int dx = ox - px; // Distance of object's X position
            int dy = oy - py; // Distance of object's Y position

            if (dy < 0 && (Math.Abs(dy) >= Math.Abs(dx))) // Object is at top
            {
                currentLocation.localSoundAt(GetSoundName(category, "top"), position);
            }
            else if (dx > 0 && (Math.Abs(dx) >= Math.Abs(dy))) // Object is at right
            {
                currentLocation.localSoundAt(GetSoundName(category, "right"), position);
            }
            else if (dx < 0 && (Math.Abs(dx) > Math.Abs(dy))) // Object is at left
            {
                currentLocation.localSoundAt(GetSoundName(category, "left"), position);
            }
            else if (dy > 0 && (Math.Abs(dy) > Math.Abs(dx))) // Object is at bottom
            {
                currentLocation.localSoundAt(GetSoundName(category, "bottom"), position);
            }

        }

        public static string GetSoundName(CATEGORY category, string post)
        {
            string soundName = $"_{post}";

            if (!MainClass.Config.RadarStereoSound)
                soundName = $"_mono{soundName}";

            if (category == CATEGORY.Farmers) // Villagers and farmers
                soundName = $"npc{soundName}";
            else if (category == CATEGORY.FarmAnimals) // Farm Animals
                soundName = $"npc{soundName}";
            else if (category == CATEGORY.NPCs) // Other npcs, also includes enemies
                soundName = $"npc{soundName}";
            else if (category == CATEGORY.WaterTiles) // Water tiles
                soundName = $"obj{soundName}";
            else if (category == CATEGORY.Furnitures) // Furnitures
                soundName = $"obj{soundName}";
            else if (category == CATEGORY.Others) // Other Objects
                soundName = $"obj{soundName}";
            else if (category == CATEGORY.Crops) // Crops
                soundName = $"obj{soundName}";
            else if (category == CATEGORY.Trees) // Trees
                soundName = $"obj{soundName}";
            else if (category == CATEGORY.Buildings) // Buildings
                soundName = $"obj{soundName}";
            else if (category == CATEGORY.MineItems) // Mine items
                soundName = $"obj{soundName}";
            else if (category == CATEGORY.Containers) // Chests
                soundName = $"obj{soundName}";
            else if (category == CATEGORY.Debris) // Grass and debris
                soundName = $"obj{soundName}";
            else if (category == CATEGORY.Flooring) // Flooring
                soundName = $"obj{soundName}";
            else // Default
                soundName = $"obj{soundName}";

            return soundName;
        }

        public bool ToggleFocus()
        {
            radarFocus = !radarFocus;

            if (radarFocus)
                EnableFocus();
            else
                DisableFocus();

            return radarFocus;
        }

        public void EnableFocus()
        {
            temp_exclusions = exclusions.ToList();
            exclusions.Clear();
        }

        public void DisableFocus()
        {
            exclusions = temp_exclusions.ToList();
            temp_exclusions.Clear();
        }
    }
}
