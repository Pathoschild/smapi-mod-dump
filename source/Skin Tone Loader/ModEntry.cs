/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/HeyImAmethyst/SkinToneLoader
**
*************************************************/

using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkinToneLoader.Framework;
using SkinToneLoader.Framework.ContentEditors;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Objects;

namespace SkinToneLoader
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        // Instance of ContentPackHelper
        private ContentPackHelper PackHelper;

        // Instance of Harmony Helper
        public HarmonyHelper HarmonyHelper;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        /// 

        private SkinToneEditor skinToneEditor;

        public string contentPackSkinToneFolder = "SkinTone"; 
        public string contentPackSkinTonePNGName = "skinTones.png";
        public string moddedSkinTonesFileName = "moddedSkinTones.png";
        

        // Directory where the skin color files are stored
        private DirectoryInfo moddedSkinColorDirectory;
        public string moddedSkinColorsPathString;

        bool wasEditMade;
        public bool isFashionSenseInstalled;

        public override void Entry(IModHelper helper)
        {

            moddedSkinColorsPathString = Path.Combine(Helper.DirectoryPath, moddedSkinTonesFileName);
            moddedSkinColorDirectory = new DirectoryInfo(moddedSkinColorsPathString);

            InitializeClasses();
            SetUpEvents();
            CheckIfFashionSenseIsInstalled();

            try
            {
                HarmonyHelper.InitializeAndPatch();
            }
            catch (Exception e)
            {
                Monitor.Log($"Issue with Harmony patching: {e}", LogLevel.Info);
                return;
            }
        }

        /// <summary>
        /// Initializes the needed classes for the mod.
        /// </summary>
        private void InitializeClasses()
        {
            PackHelper = new ContentPackHelper(this);
            HarmonyHelper = new HarmonyHelper(this);
            skinToneEditor = new SkinToneEditor(this, PackHelper);
        }

        /// <summary>
        /// Sets up the events for the mod.
        /// </summary>
        private void SetUpEvents()
        {
            Helper.Events.Content.AssetRequested += OnAssetRequested;
            Helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            Helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
        }

        /// <summary>
        /// Event that is called when the game launches.
        /// </summary>
        /// <param name="sender"> The object</param>
        /// <param name="e"> The Game Launched Event argument</param>
        /// <remarks> This is used to load the content packs and dresser.png</remarks>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // Read The Content Packs
            PackHelper.ReadContentPacks();
        }

        /// <summary>
        /// Event that is called when a save is loaded.
        /// </summary> 
        /// <param name="sender"> The object</param>
        /// <param name="e"> The Save Loaded Event arguement</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            if(Context.IsWorldReady)
                SkinToneConfigModelManager.SaveCharacterLayout(this);
        }

        /// <inheritdoc cref="IContentEvents.AssetRequested" />
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.Name.IsEquivalentTo("Characters\\Farmer\\skinColors"))
            {

                if (!wasEditMade)
                {
                    e.Edit(asset =>
                    {
                        EditSkin(asset);
                    });
                }

                e.Edit(asset =>
                {
                    var editor = asset.AsImage();
                    Texture2D sourceImage = Helper.ModContent.Load<Texture2D>(moddedSkinTonesFileName);
                    editor.ReplaceWith(sourceImage);
                });

            }
        }

        /// <summary>
        /// Edits the farmer's skin texture png file.
        /// </summary>
        /// <param name="asset">The asset file</param>
        private void EditSkin(IAssetData asset)
        {
            skinToneEditor.Asset = asset;
            skinToneEditor.EditSkinTexture();
            wasEditMade = true;
        }


        /// <summary>
        /// Checks if the modded skin tone directory exists.
        /// </summary>
        public bool DoesModdedSkinToneDirectoryExists()
        {
            return moddedSkinColorDirectory.Exists;
        }

        /// <summary>
        /// Checks if the mod Fashion Sense is currently installed.
        /// </summary>
        private void CheckIfFashionSenseIsInstalled()
        {
            isFashionSenseInstalled = Helper.ModRegistry.IsLoaded("PeacefulEnd.FashionSense");
            Monitor.Log($"Fashion Sense Installed: {isFashionSenseInstalled}", LogLevel.Trace);
        }
    }
}
