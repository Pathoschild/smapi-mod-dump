/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/alanperrow/StardewModding
**
*************************************************/

using HarmonyLib;
using SplitscreenImproved.Compatibility;
using SplitscreenImproved.MusicFix;
using SplitscreenImproved.ShowName;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace SplitscreenImproved
{
    /// <summary>
    /// The mod entry class loaded by SMAPI.
    /// </summary>
    public partial class ModEntry : Mod
    {
        public static ModEntry Instance { get; private set; }

        public static ModConfig Config { get; private set; }

        /// <summary>
        /// The mod entry point, called after the mod is first loaded.
        /// </summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Instance = this;
            Config = helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;

            helper.Events.Display.RenderedActiveMenu += OnRenderedActiveMenu;
            /* DEBUG
            helper.Events.Display.Rendered += OnRendered;
            */
        }

        /// <summary>
        /// Raised after the game is launched, right before the first update tick. This happens once per game session (unrelated to loading saves).
        /// All mods are loaded and initialised at this point, so this is a good time to set up mod integrations.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // Harmony patches
            var harmony = new Harmony(ModManifest.UniqueID);
            harmony.PatchAll();

            // Initialize external mod API integrations.
            var api = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (api != null)
            {
                Initialize(api);
            }
        }

        /// <summary>
        /// Raised after the active menu is drawn to the screen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRenderedActiveMenu(object sender, RenderedActiveMenuEventArgs e)
        {
            ShowNameHelper.DrawPlayerNameScroll(e.SpriteBatch);
        }

        /* DEBUG
        private void OnRendered(object sender, RenderedEventArgs e)
        {
            MusicFixHelper.DrawDebugText(e.SpriteBatch);
        }
        */
    }
}
