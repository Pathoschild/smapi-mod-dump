using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using ModSettingsTab.Framework.Components;

namespace ModSettingsTab.Framework
{
    public static class ModManager
    {
        private class SaveData
        {
            public string Active;
            public Dictionary<string, List<string>> Packs;
        }

        /// <summary>
        /// save timer, anti-click protection
        /// </summary>
        private static readonly Timer SaveTimer;
        private static readonly Timer SetPackTimer;
        
        public delegate void Update();
        public static Update UpdateMod;


        public static readonly Dictionary<string, List<string>> PackList;
        public static readonly List<OptionsElement> Options = new List<OptionsElement>();
        private static string _activePack;

        static ModManager()
        {
            SetPackTimer = new Timer(2000.0)
            {
                Enabled = false,
                AutoReset = false
            };
            SetPackTimer.Elapsed += (t, e) => SetModPack();
            SaveTimer = new Timer(1200.0)
            {
                Enabled = false,
                AutoReset = false
            };
            SaveTimer.Elapsed += (t, e) =>
            {
                Helper.Data.WriteJsonFile("data/ModPacks.json",
                    new SaveData
                    {
                        Packs = PackList,
                        Active = ActivePack
                    });
            };
            var data = Helper.Data.ReadJsonFile<SaveData>("data/ModPacks.json");
            PackList = data?.Packs ?? new Dictionary<string, List<string>>();
            _activePack = data?.Active;
        }

        public static string ActivePack
        {
            get => _activePack;
            set
            {
                _activePack = value;
                SetPackTimer.Reset();
            }
        }

        public static void AddModPack(IEnumerable<string> modId, string modPackName)
        {
            if (PackList.ContainsKey(modPackName) || PackList.Count > 3) return;
            PackList.Add(modPackName, new List<string>(modId));
            SaveTimer.Reset();
            UpdateOptionsAsync();
        }

//        public static void RemoveFromModPack(string uniqueId, string modPackName)
//        {
//            if (!PackList.ContainsKey(modPackName)) return;
//            if (!PackList[modPackName].Contains(uniqueId))
//            {
//                PackList[modPackName].Remove(uniqueId);
//                SaveTimer.Reset();
//                UpdateOptionsAsync();
//            }
//
//            if (PackList[modPackName].Count != 0) return;
//            RemoveModPack(modPackName);
//        }

        public static void RemoveModPack(string modPackName)
        {
            if (!PackList.ContainsKey(modPackName)) return;
            PackList.Remove(modPackName);
            SaveTimer.Reset();
            UpdateOptionsAsync();
            if (ActivePack == modPackName) ActivePack = "";
        }

        private static async void SetModPack()
        {
            if (!PackList.ContainsKey(ActivePack)) return;
            await SetPack(ActivePack);
            SaveTimer.Reset();
            ModData.NeedReload = true;
        }

        private static Task SetPack(string modPackName)
        {
            return Task.Run(() =>
            {
                foreach (var mod in Options.Cast<ModManagerToggle>())
                {
                    mod.Disable = !mod.ModPack.Contains(modPackName);
                }
            });
        }
        
        private static async void UpdateOptionsAsync()
        {
            await Task.Run(LoadOptions);
            UpdateMod();
        }

        public static void LoadOptions()
        {
            Options.Clear();
            var modList = ModData.ModList.Where(mod => mod.Key != "GilarF.ModSettingsTab");
            foreach (var mod in modList)
            {
                var modPack = (
                        from pack in PackList
                        where pack.Value.Contains(mod.Key)
                        select pack.Key)
                    .ToList();
                Options.Add(new ModManagerToggle(mod.Key, modPack));
            }
        }
    }
}