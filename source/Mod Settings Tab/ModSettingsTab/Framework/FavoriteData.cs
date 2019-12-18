using System.Collections.Generic;
using System.Linq;
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
        public static Queue<string> Favorite;

        /// <summary>
        /// save timer, anti-click protection
        /// </summary>
        private static readonly Timer SaveTimer;

        static FavoriteData()
        {
            SaveTimer = new Timer(2000.0)
            {
                Enabled = false,
                AutoReset = false
            };
            SaveTimer.Elapsed += (t, e) =>
            {
                ModEntry.Helper.Data.WriteJsonFile("data/favorite.json",
                    new SaveData
                    {
                        List = Favorite.ToList(),
                        Bookmarks = ModData.FavoriteTabSource
                    });
                ModData.UpdateFavoriteOptionsAsync();
            };

            var freeBookmarks = new[]
            {
                new Rectangle(0, 128, 32, 24),
                new Rectangle(32, 128, 32, 24),
                new Rectangle(0, 152, 32, 24),
                new Rectangle(32, 152, 32, 24),
                new Rectangle(0, 176, 32, 24),
            };

            var data = ModEntry.Helper.Data.ReadJsonFile<SaveData>("data/favorite.json");
            if (data == null)
            {
                Favorite = new Queue<string>();
                ModData.FreeFavoriteTabSource = new Queue<Rectangle>(freeBookmarks);
                return;
            }

            // check if all mods are loaded
            for (var i = data.List.Count - 1; i >= 0; i--)
            {
                if (ModEntry.Helper.ModRegistry.IsLoaded(data.List[i])) continue;
                data.Bookmarks.Remove(data.List[i]);
                data.List.RemoveAt(i);
            }

            Favorite = new Queue<string>(data.List);
            ModData.FavoriteTabSource = data.Bookmarks;
            if (Favorite.Count > 5)
                Favorite = new Queue<string>(Favorite.Take(5));

            // delete used free bookmarks
            var bookmarks = data.Bookmarks.Select(b => b.Value).ToArray();
            ModData.FreeFavoriteTabSource = new Queue<Rectangle>(freeBookmarks.Except(bookmarks));
        }

        /// <summary>
        /// Checks if the mod is bookmarked
        /// </summary>
        /// <param name="uniqueId">
        /// unique mod identifier
        /// </param>
        /// <returns></returns>
        public static bool IsFavorite(string uniqueId) => Favorite.Contains(uniqueId);

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
                var newFavorite = Favorite.Where(id => id != uniqueId);
                Favorite = new Queue<string>(newFavorite);
                // markAsInactive
                ModData.ModList[uniqueId].Favorite = false;
                // mark the bookmark as free
                ModData.FreeFavoriteTabSource.Enqueue(ModData.FavoriteTabSource[uniqueId]);
                // delete from bookmarks
                ModData.FavoriteTabSource.Remove(uniqueId);
            }
            else
            {
                // add to favorites
                Favorite.Enqueue(uniqueId);
                ModData.ModList[uniqueId].Favorite = true;
                if (Favorite.Count > 5)
                {
                    // if there are already more than 5 bookmarks, delete the oldest
                    var modId = Favorite.Dequeue();
                    ModData.ModList[modId].Favorite = false;
                    // also free bookmark
                    ModData.FreeFavoriteTabSource.Enqueue(ModData.FavoriteTabSource[modId]);
                    ModData.FavoriteTabSource.Remove(modId);
                }

                // add a free bookmark
                ModData.FavoriteTabSource.Add(uniqueId, ModData.FreeFavoriteTabSource.Dequeue());
            }

            SaveTimer.Stop();
            SaveTimer.Start();
        }
    }
}