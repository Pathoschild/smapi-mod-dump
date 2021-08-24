/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/gzhynko/StardewMods
**
*************************************************/

using DialogueBoxRedesign.Patching;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Menus;

namespace DialogueBoxRedesign
{
    /// <summary> The mod entry class loaded by SMAPI. </summary>
    public class ModEntry : Mod, IAssetEditor
    {
        #region Variables
        
        public static IMonitor ModMonitor;
        public static IModHelper ModHelper;
        public static ModConfig Config;

        public static Texture2D GradientSample;
        public static Texture2D DarkerGradientSample;

        #endregion
        #region Public methods
        
        /// <summary> The mod entry point, called after the mod is first loaded. </summary>
        /// <param name="helper"> Provides simplified APIs for writing mods. </param>
        public override void Entry(IModHelper helper)
        {
            ModMonitor = Monitor;
            ModHelper = Helper;
            
            Config = Helper.ReadConfig<ModConfig>();
            
            PrepareAssets();
            
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }
        
        public void SaveConfig(ModConfig newConfig)
        {
            Config = newConfig;
            Helper.WriteConfig(newConfig);
        }
        
        /// <summary>Get whether this instance can edit the given asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("LooseSprites/Cursors");
        }

        /// <summary>Edit the friendship jewel textures to make them transparent.</summary>
        /// <param name="asset">A helper which encapsulates metadata about an asset and enables changes to it.</param>
        public void Edit<T>(IAssetData asset)
        {
            if (!asset.AssetNameEquals("LooseSprites/Cursors")) return;

            var editor = asset.AsImage();
            Texture2D sourceImage;

            try
            {
                sourceImage = Helper.Content.Load<Texture2D>("assets/friendshipJewel.png");
            }
            catch (Microsoft.Xna.Framework.Content.ContentLoadException)
            {
                return;
            }

            editor.PatchImage(sourceImage, new Rectangle(0, 0, 44, 55), new Rectangle(140, 532, 44, 55));
            editor.PatchImage(sourceImage, new Rectangle(44, 0, 11, 11), new Rectangle(269, 495, 11, 11));
        }

        #endregion
        #region Private methods
        
        private void PrepareAssets()
        {
            GradientSample = Helper.Content.Load<Texture2D>("assets/gradientSample.png");
            DarkerGradientSample = Helper.Content.Load<Texture2D>("assets/darkerGradientSample.png");
        }
        
        private void ApplyHarmonyPatches()
        {
            var harmony = new Harmony(ModManifest.UniqueID);

            harmony.Patch(
                AccessTools.Method(typeof(DialogueBox), nameof(DialogueBox.drawPortrait)),
                new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.DrawPortrait))
            );
            
            harmony.Patch(
                AccessTools.Method(typeof(DialogueBox), nameof(DialogueBox.drawBox)),
                new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.DrawBox))
            );
            
            harmony.Patch(
                AccessTools.Method(typeof(DialogueBox), nameof(DialogueBox.draw), new [] { typeof(SpriteBatch) }),
                new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.Draw))
            );
        }

        /// <summary> Raised after the game is launched, right before the first update tick. This happens once per game session (unrelated to loading saves). All mods are loaded and initialised at this point, so this is a good time to set up mod integrations. </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            ApplyHarmonyPatches();
            ModConfig.SetUpModConfigMenu(Config, this);
        }
        
        #endregion
    }
}
