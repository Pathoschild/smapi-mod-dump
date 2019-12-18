using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModSettingsTab.Framework;
using ModSettingsTab.Framework.Integration;
using StardewModdingAPI;
using Mod = ModSettingsTab.Framework.Mod;
using OptionsElement = ModSettingsTab.Framework.Components.OptionsElement;

namespace ModSettingsTab
{
    public static class ModData
    {
        public const int Offset = 192;

        public static bool NeedReload;

        public static readonly Api Api;

        public static TabConfig Config;

        /// <summary>
        /// collection of loaded mods, only those that have settings
        /// </summary>
        public static ModList ModList { get; private set; }

        /// <summary>
        /// list of all modification settings
        /// </summary>
        public static List<OptionsElement> Options;

        public static SmapiIntegration SMAPI;

        /// <summary>
        /// list of favorite mod options
        /// </summary>
        public static readonly List<Mod> FavoriteMod;
        public static readonly Texture2D Tabs;
        public static Dictionary<string,Rectangle> FavoriteTabSource;
        public static Queue<Rectangle> FreeFavoriteTabSource;

        public delegate void Update();

        public static Update UpdateFavoriteMod;

        static ModData()
        {
            Api = new Api();
            Config = ModEntry.Helper.ReadConfig<TabConfig>();
            FavoriteMod = new List<Mod>();
            Tabs = ModEntry.Helper.Content.Load<Texture2D>("assets/Tabs.png");
            FavoriteTabSource = new Dictionary<string, Rectangle>();
        }

        /// <summary>
        /// initialization of master data
        /// </summary>
        public static async void Init()
        {
            await LoadOptions();
            ModEntry.Console.Log($"Load {ModList.Count} mods and {Options.Count} Options",
                LogLevel.Info);
        }

        /// <summary>
        /// asynchronously updates the list of settings of selected mods
        /// </summary>
        public static async void UpdateFavoriteOptionsAsync()
        {
            await Task.Run(LoadFavoriteOptions);
            UpdateFavoriteMod();
        }

        private static Task LoadOptions()
        {
            return Task.Run(() =>
            {
                ModList = new ModList();
                SMAPI = new SmapiIntegration();
                Options = ModList.SelectMany(mod => mod.Value.Options).ToList();
                LoadFavoriteOptions();
            });
        }

        private static void LoadFavoriteOptions()
        {
            FavoriteMod.Clear();
            foreach (var id in FavoriteData.Favorite)
            {
                FavoriteMod.Add(ModList[id]);
            }
            for (var i = FavoriteMod.Count; i > 0 ; i--)
            {
                var id = FavoriteMod[i-1].Manifest.UniqueID;
                if (!FavoriteTabSource.ContainsKey(id))
                    FavoriteTabSource.Add(id, FreeFavoriteTabSource.Dequeue());
            }
            
        }
    }
}