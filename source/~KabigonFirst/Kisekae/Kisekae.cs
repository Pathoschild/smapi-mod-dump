/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KabigonFirst/StardewValleyMods
**
*************************************************/

using System;
using Kisekae.Framework;
using Kisekae.Config;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace Kisekae {
    /// <summary>The main entry point.</summary>
    public class Kisekae : Mod, IAssetLoader {
        /*********
        ** Properties
        *********/
        /// <summary>The global config settings.</summary>
        private GlobalConfig m_globalConfig;
        /// <summary>Encapsulates the underlying mod texture management.</summary>
        private ContentHelper m_contentHelper;
        /// <summary>The core component responsible to reshape farmer </summary>
        private FarmerMakeup m_farmerPatcher;
        /// <summary>The dresser in the farmhouse.</summary>
        private Dresser m_dresser;
        /// <summary>The patcher takes care of load menu </summary>
        private LoadMenuPatcher m_loadMenuPatcher;

        /// <summary>The current per-save config settings.</summary>
        private LocalConfig m_playerConfig;
        /// <summary>Whether the mod is initialised.</summary>
        private bool m_isInitialised => m_contentHelper != null;
        /// <summary>Whether the game world is loaded and ready.</summary>
        private bool m_isLoaded => this.m_isInitialised && Game1.hasLoadedGame;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper) {
            // load settings
            m_globalConfig = helper.ReadConfig<GlobalConfig>();
            // check if the mod should be enalbed
            if (!m_globalConfig.IsEnable) {
                return;
            }
            // initialize components
            LocalConfig.s_env = this;
            m_contentHelper = new ContentHelper(this);
            FarmerMakeup.MaleFaceTypes = m_globalConfig.MaleFaceTypes;
            FarmerMakeup.MaleNoseTypes = m_globalConfig.MaleNoseTypes;
            FarmerMakeup.MaleBottomsTypes = m_globalConfig.MaleBottomsTypes;
            FarmerMakeup.MaleShoeTypes = m_globalConfig.MaleShoeTypes;
            FarmerMakeup.FemaleFaceTypes = m_globalConfig.MaleFaceTypes;
            FarmerMakeup.FemaleNoseTypes = m_globalConfig.FemaleNoseTypes;
            FarmerMakeup.FemaleBottomsTypes = m_globalConfig.FemaleBottomsTypes;
            FarmerMakeup.FemaleShoeTypes = m_globalConfig.FemaleShoeTypes;
            FarmerMakeup.HideMaleSkirts = m_globalConfig.HideMaleSkirts;
            m_farmerPatcher = new FarmerMakeup(this, m_contentHelper);

            m_dresser = new Dresser(this);
            m_dresser.m_isVisible = m_globalConfig.ShowDresser;
            m_dresser.m_stoveInCorner = m_globalConfig.StoveInCorner;
            m_dresser.init();

            m_loadMenuPatcher = new LoadMenuPatcher(this, m_farmerPatcher);
            m_loadMenuPatcher.init();

            // hook events
            helper.Events.Input.ButtonPressed  += OnButtonPressed;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
        }

        /// <summary>Get whether this instance can load the initial version of the given asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public bool CanLoad<T>(IAssetInfo asset) {
            if (m_globalConfig.IsEnable) {
                return m_contentHelper.CanLoad<T>(asset);
            }
            return false;
        }

        /// <summary>Load a matched asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public T Load<T>(IAssetInfo asset) {
            try {
                return m_contentHelper.Load<T>(asset);
            } catch {
                return (T)(object)null;
            }
        }

        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e) {
            if (!m_isLoaded) {
                return;
            }

            // open the customisation menu if the player activated the dresser
            if (e.Button.IsActionButton()) {
                if (Game1.player.UsingTool || Game1.pickingTool || Game1.menuUp || (Game1.eventUp && !Game1.currentLocation.currentEvent.playerControlSequence) || Game1.nameSelectUp || Game1.numberOfSelectedItems != -1 || Game1.fadeToBlack || Game1.activeClickableMenu != null) {
                    return;
                }
                // check if the activated tile is the Dresser or Wizard Shrine
                Vector2 grabTile = new Vector2(Game1.getOldMouseX() + Game1.viewport.X, Game1.getOldMouseY() + Game1.viewport.Y) / Game1.tileSize;
                if (!Utility.tileWithinRadiusOfPlayer((int)grabTile.X, (int)grabTile.Y, 1, Game1.player)) {
                    grabTile = Game1.player.GetGrabTile();
                }

                xTile.ObjectModel.PropertyValue propertyValue = null;
                Game1.currentLocation.map.GetLayer("Buildings").PickTile(new xTile.Dimensions.Location((int)grabTile.X * 64, (int)grabTile.Y * 64), Game1.viewport.Size)?.Properties.TryGetValue("Action", out propertyValue);
                if ((propertyValue != null && propertyValue == "WizardShrine")) {
                    Game1.currentLocation.afterQuestion = Answer_WizardShrine;
                    Game1.currentLocation.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:WizardTower_WizardShrine").Replace('\n', '^'), Game1.currentLocation.createYesNoResponses(), "WizardShrine");
                    this.Helper.Input.Suppress(e.Button);
                    return;
                }

                if (!m_dresser.IsDresser(grabTile)) {
                    return;
                }
            } else if (m_globalConfig.MenuAccessKey != SButton.None && e.Button == m_globalConfig.MenuAccessKey && Game1.activeClickableMenu == null) {
            } else {
                return;
            }

            // open menu
            Game1.playSound("bigDeSelect");
            OpenMenu();
            this.Helper.Input.Suppress(e.Button);
        }

        /// <summary>Process the answer in front of Wizard Shrine.</summary>
        private void Answer_WizardShrine(Farmer farmer, string ans) {
            if (ans != "Yes") {
                return;
            }
            if (Game1.player.Money >= 500) {
                OpenMenu();
                Game1.player.Money -= 500;
                try {
                    IReflectedField<Multiplayer> multiplayer = Helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer");
                    multiplayer.GetValue()?.globalChatInfoMessage("Makeover", Game1.player.Name);
                } catch { }
            } else {
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:NotEnoughMoney2"));
            }
        }

        /// <summary>Raised after the player loads a save slot and the world is initialised.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaveLoaded(object sender, EventArgs e) {
            // load config
            m_playerConfig = this.Helper.Data.ReadJsonFile<LocalConfig>(LocalConfig.s_perSaveConfigPath) ?? new LocalConfig();
            // patch player textures
            m_farmerPatcher.m_farmer = Game1.player;
            m_farmerPatcher.m_config = m_playerConfig;
            m_farmerPatcher.ApplyConfig();
        }

        /// <summary>Open custimization menu.</summary>
        private void OpenMenu() {
            m_farmerPatcher.m_farmer = Game1.player;
            m_farmerPatcher.m_config = m_playerConfig;
            Game1.activeClickableMenu = new MenuFarmerMakeup(this, m_farmerPatcher, m_globalConfig);
        }
    }
}
