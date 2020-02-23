using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Xna.Framework;

namespace ModSettingsTab.Framework
{
    /// <summary>
    /// saves the mod to favorites
    /// </summary>
    public static class FavoriteData
    {
        private class SaveData
        {
            public List<string> List;
            public Dictionary<string, Rectangle> Bookmarks;
        }

        /// <summary>
        /// collection of selected mods
        /// </summary>
        private static Queue<string> _favoriteQueue;

        /// <summary>
        /// save timer, anti-click protection
        /// </summary>
        private static readonly Timer SaveTimer;

        /// <summary>
        /// list of favorite mod options
        /// </summary>
        public static readonly List<Mod> ModList;

        public delegate void Update();

        public static Update UpdateMod;
        public static readonly Dictionary<string, Rectangle> FavoriteTabSource;
        private static readonly Queue<Rectangle> FreeFavoriteTabSource;


        static FavoriteData()
        {
            FavoriteTabSource = new Dictionary<string, Rectangle>();
            ModList = new List<Mod>();
            SaveTimer = new Timer(1200.0)
            {
                Enabled = false,
                AutoReset = false
            };
            SaveTimer.Elapsed += (t, e) =>
            {
                Helper.Data.WriteJsonFile("data/favorite.json",
                    new SaveData
                    {
                        List = _favoriteQueue.ToList(),
                        Bookmarks = FavoriteTabSource
                    });
                UpdateFavoriteOptionsAsync();
            };

            var freeBookmarks = new[]
            {
                new Rectangle(0, 128, 32, 24),
                new Rectangle(32, 128, 32, 24),
                new Rectangle(0, 152, 32, 24),
                new Rectangle(32, 152, 32, 24),
                new Rectangle(0, 176, 32, 24),
            };

            var data = Helper.Data.ReadJsonFile<SaveData>("data/favorite.json");
            if (data == null)
            {
                _favoriteQueue = new Queue<string>();
                FreeFavoriteTabSource = new Queue<Rectangle>(freeBookmarks);
                return;
            }

            // check if all mods are loaded
            for (var i = data.List.Count - 1; i >= 0; i--)
            {
                if (Helper.ModRegistry.IsLoaded(data.List[i])) continue;
                data.Bookmarks.Remove(data.List[i]);
                data.List.RemoveAt(i);
            }

            _favoriteQueue = new Queue<string>(data.List);
            FavoriteTabSource = data.Bookmarks;
            if (_favoriteQueue.Count > 5)
                _favoriteQueue = new Queue<string>(_favoriteQueue.Take(5));

            // delete used free bookmarks
            var bookmarks = data.Bookmarks.Select(b => b.Value).ToArray();
            FreeFavoriteTabSource = new Queue<Rectangle>(freeBookmarks.Except(bookmarks));
        }

        /// <summary>
        /// Checks if the mod is bookmarked
        /// </summary>
        /// <param name="uniqueId">
        /// unique mod identifier
        /// </param>
        /// <returns></returns>
        public static bool IsFavorite(string uniqueId) => _favoriteQueue.Contains(uniqueId);

        /// <summary>
        /// changes bookmark state
        /// </summary>
        /// <param name="uniqueId">
        /// unique mod identifier
        /// </param>
        public static void ChangeStatus(string uniqueId)
        {
            if (IsFavorite(uniqueId))
            {
                // update the queue
                var newFavorite = _favoriteQueue.Where(id => id != uniqueId);
                _favoriteQueue = new Queue<string>(newFavorite);
                // markAsInactive
                ModData.ModList[uniqueId].Favorite = false;
                // mark the bookmark as free
                FreeFavoriteTabSource.Enqueue(FavoriteTabSource[uniqueId]);
                // delete from bookmarks
                FavoriteTabSource.Remove(uniqueId);
            }
            else
            {
                // add to favorites
                _favoriteQueue.Enqueue(uniqueId);
                ModData.ModList[uniqueId].Favorite = true;
                if (_favoriteQueue.Count > 5)
                {
                    // if there are already more than 5 bookmarks, delete the oldest
                    var modId = _favoriteQueue.Dequeue();
                    ModData.ModList[modId].Favorite = false;
                    // also free bookmark
                    FreeFavoriteTabSource.Enqueue(FavoriteTabSource[modId]);
                    FavoriteTabSource.Remove(modId);
                }

                // add a free bookmark
                FavoriteTabSource.Add(uniqueId, FreeFavoriteTabSource.Dequeue());
            }
            SaveTimer.Reset();
        }

        /// <summary>
        /// asynchronously updates the list of settings of selected mods
        /// </summary>
        private static async void UpdateFavoriteOptionsAsync()
        {
            await Task.Run(LoadOptions);
            UpdateMod();
        }

        public static void LoadOptions()
        {
            ModList.Clear();
            foreach (var id in _favoriteQueue)
            {
                ModList.Add(ModData.ModList[id]);
            }

            for (var i = ModList.Count; i > 0; i--)
            {
                var id = ModList[i - 1].Manifest.UniqueID;
                if (!FavoriteTabSource.ContainsKey(id))
                    FavoriteTabSource.Add(id, FreeFavoriteTabSource.Dequeue());
            }
        }
    }
}