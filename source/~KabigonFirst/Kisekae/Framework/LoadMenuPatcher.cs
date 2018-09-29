using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using SFarmer = StardewValley.Farmer;
using Kisekae.Config;

namespace Kisekae.Framework {
    class LoadMenuPatcher {
        /*********
        ** Private Properties
        *********/
        /// <summary>Global Mod Interface.</summary>
        private IMod m_env;
        /// <summary>The core component responsible to reshape farmer </summary>
        private FarmerMakeup m_farmerPatcher;
        /// <summary>The last patched load menu, if the game hasn't loaded yet.</summary>
        private IClickableMenu m_previousLoadMenu;
        /// <summary>The farmer data for all saves, if the game hasn't loaded yet.</summary>
        private List<SFarmer> m_farmers = new List<SFarmer>();
        /// <summary>The per-save configs for valid saves indexed by save name, if the game hasn't loaded yet.</summary>
        private Dictionary<string, LocalConfig> m_farmerConfigs = new Dictionary<string, LocalConfig>();
        /// <summary>Whether the init task in load menu started.</summary>
        private bool m_taskStarted = false;

        public LoadMenuPatcher(IMod env, FarmerMakeup farmerPatcher) {
            m_env = env;
            m_farmerPatcher = farmerPatcher;
        }

        public void init() {
            // load per-save configs
            ReadLocalConfigs(m_farmerConfigs);

            SaveEvents.AfterReturnToTitle += SaveEvents_AfterReturnToTitle;
            // patch load menu hook
            GameEvents.UpdateTick += Event_PatchLoadMenu;
        }

        /// <summary>The event handler called when the player stops a session and returns to the title screen..</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void SaveEvents_AfterReturnToTitle(object sender, EventArgs e) {
            // reset state
            m_previousLoadMenu = null;
            m_farmers.Clear();

            // load per-save configs
            m_farmerConfigs.Clear();
            ReadLocalConfigs(m_farmerConfigs);

            // restore load-menu patcher
            GameEvents.UpdateTick += Event_PatchLoadMenu;
        }

        /// <summary>The event handler to monitor if load menu is active and patch it.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void Event_PatchLoadMenu(object sender, EventArgs e) {
            // patch load menu
            if (!Game1.hasLoadedGame) {
                if (Game1.activeClickableMenu is TitleMenu && TitleMenu.subMenu is LoadGameMenu loadMenu) {
                    if (loadMenu == null || loadMenu == this.m_previousLoadMenu) {
                        return;
                    }
                    this.PatchLoadMenu(loadMenu);
                }
                return;
            }

            // remove load menu patcher, moved to load menu UpdateTick event
            this.m_farmers.Clear();
            this.m_farmerConfigs.Clear();
            if (!string.IsNullOrEmpty(LocalConfig.s_perSaveConfigPath)) {
                GameEvents.UpdateTick -= Event_PatchLoadMenu;
            }
        }

        private void PatchLoadMenuFarmerTexture() {
            foreach (SFarmer farmer in m_farmers) {
                LocalConfig config;
                if (!m_farmerConfigs.TryGetValue(farmer.slotName, out config)) {
                    continue;
                }
                // override textures
                m_farmerPatcher.m_farmer = farmer;
                m_farmerPatcher.m_config = config;
                m_farmerPatcher.ApplyConfig();
            }
        }

        private void Event_PatchLoadMenuFarmer(object sender, EventArgs e) {
            if (!(Game1.activeClickableMenu is TitleMenu)) {
                GameEvents.UpdateTick -= Event_PatchLoadMenuFarmer;
                return;
            }

            // get load menu
            LoadGameMenu loadMenu = TitleMenu.subMenu as LoadGameMenu;
            if (loadMenu == null || loadMenu != m_previousLoadMenu) {
                GameEvents.UpdateTick -= Event_PatchLoadMenuFarmer;
                return;
            }

            PatchLoadMenuFarmerTexture();
            GameEvents.UpdateTick -= Event_PatchLoadMenuFarmer;
        }

        /// <summary>Patch the textures in the load menu if it's active.</summary>
        private void PatchLoadMenu(LoadGameMenu loadMenu) {
            // ignore multiplayer load menu
            if (loadMenu is CoopMenu) {
                return;
            }

            //Monitor.Log("PatchLoadMenu");
            // wait until menu is initialised
            IReflectedField<Task<List<SFarmer>>> tsk = m_env.Helper.Reflection.GetField<Task<List<SFarmer>>>(loadMenu, "_initTask");
            if (!m_taskStarted) {
                if (tsk == null || tsk.GetValue() == null) {
                    return;
                }

                if (!tsk.GetValue().IsCompleted) {
                    m_taskStarted = true;
                    return;
                }
            } else {
                if (tsk != null && tsk.GetValue() != null && !tsk.GetValue().IsCompleted) {
                    return;
                }
            }

            m_taskStarted = false;
            this.m_previousLoadMenu = loadMenu;

            // load saves if empty
            if (!m_farmers.Any()) {
                m_farmers = m_env.Helper.Reflection.GetMethod(typeof(LoadGameMenu), "FindSaveGames").Invoke<List<SFarmer>>();
            }

            // create a new List<MenuSlot> instance to replace menuSlots
            Type elementType = Type.GetType("StardewValley.Menus.LoadGameMenu+MenuSlot, Stardew Valley");
            Type listType = typeof(List<>).MakeGenericType(new Type[] { elementType });
            object list = Activator.CreateInstance(listType);

            /*
            // add an instance to new list
            Type SaveFileSlotType = Type.GetType("StardewValley.Menus.LoadGameMenu+SaveFileSlot, Stardew Valley");
            object e = Activator.CreateInstance(SaveFileSlotType, loadMenu, Farmers[1]);
            list.GetType().GetMethod("Add").Invoke(list, new[] { e });
            */

            // inject modified farmers
            m_env.Helper.Reflection.GetField<object>(loadMenu, "menuSlots").SetValue(list);
            m_env.Helper.Reflection.GetMethod(loadMenu, "addSaveFiles").Invoke(m_farmers);

            // override textures. Since the texture will be override after addSaveFiles, do it in next UpdateTick
            GameEvents.UpdateTick += Event_PatchLoadMenuFarmer;
        }

        /// <summary>Read all per-save configs from disk.</summary>
        private bool ReadLocalConfigs(Dictionary<string, LocalConfig> cfg) {
            // get saves path
            string savePath = Constants.SavesPath;
            if (!Directory.Exists(savePath)) {
                return false;
            }

            // get save names
            string[] directories = Directory.GetDirectories(savePath);
            if (!directories.Any()) {
                return true;
            }

            // get per-save configs
            foreach (string saveDir in directories) {
                // get config
                string localConfigPath = Path.Combine("psconfigs", $"{new DirectoryInfo(saveDir).Name}.json");
                LocalConfig farmerConfig = m_env.Helper.ReadJsonFile<LocalConfig>(localConfigPath);
                if (farmerConfig == null) {
                    farmerConfig = new LocalConfig();
                    m_env.Monitor.Log("create new save:"+ localConfigPath, LogLevel.Info);
                    m_env.Helper.WriteJsonFile(localConfigPath, farmerConfig);
                }
                farmerConfig.SaveName = new DirectoryInfo(saveDir).Name;
                cfg[farmerConfig.SaveName] = farmerConfig;
            }
            return true;
        }
    }
}
