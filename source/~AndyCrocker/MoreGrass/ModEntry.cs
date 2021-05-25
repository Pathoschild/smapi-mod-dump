/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoreGrass.Config;
using MoreGrass.Patches;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;
using System.IO;
using System.Reflection;

namespace MoreGrass
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod, IAssetEditor
    {
        /*********
        ** Fields
        *********/
        /// <summary>Whether the default game sprites should get reloaded.</summary>
        /// <remarks>This is the case when a mod invalidates the games grass cache (if a mod retextures the sprites for example).</remarks>
        private bool ReloadDefaultSprites = false;


        /*********
        ** Accessors 
        *********/
        /// <summary>The mod configuration.</summary>
        public ModConfig Config { get; private set; }

        /// <summary>The sprite pool for spring grass sprites.</summary>
        public SpritePool SpringSpritePool { get; } = new SpritePool();

        /// <summary>The sprite pool for summer grass sprites.</summary>
        public SpritePool SummerSpritePool { get; } = new SpritePool();

        /// <summary>The sprite pool for fall grass sprites.</summary>
        public SpritePool FallSpritePool { get; } = new SpritePool();

        /// <summary>The sprite pool for winter grass sprites.</summary>
        public SpritePool WinterSpritePool { get; } = new SpritePool();

        /// <summary>The singleton instance of <see cref="ModEntry"/>.</summary>
        public static ModEntry Instance { get; private set; }


        /*********
        ** Public Methods 
        *********/
        /// <summary>The mod entry point.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Instance = this;
            Config = this.Helper.ReadConfig<ModConfig>();

            this.Helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;

            ApplyHarmonyPatches();
            LoadContentPacks();
            LoadDefaultGrass(this.Helper.Content.Load<Texture2D>(Path.Combine("TerrainFeatures", "grass"), ContentSource.GameContent));

            this.Monitor.Log($"A total of {SpringSpritePool.Count} spring sprites have been loaded");
            this.Monitor.Log($"A total of {SummerSpritePool.Count} summer sprites have been loaded");
            this.Monitor.Log($"A total of {FallSpritePool.Count} fall sprites have been loaded");
            this.Monitor.Log($"A total of {WinterSpritePool.Count} winter sprites have been loaded");

            this.Monitor.Log($"Have default sprites been including in spring? {SpringSpritePool.IncludeDefaultGrass}");
            this.Monitor.Log($"Have default sprites been including in summer? {SummerSpritePool.IncludeDefaultGrass}");
            this.Monitor.Log($"Have default sprites been including in fall? {FallSpritePool.IncludeDefaultGrass}");
            this.Monitor.Log($"Have default sprites been including in winter? {WinterSpritePool.IncludeDefaultGrass}");
        }

        /// <inheritdoc/>
        /// <remarks>This is used to detect when the grass sprite's are getting edited (so the local cache can be reconstructed with the edited version).</remarks>
        public bool CanEdit<T>(IAssetInfo asset)
        {
            if (asset.AssetName.ToLower() == Path.Combine("terrainfeatures", "grass"))
                ReloadDefaultSprites = true;

            return false;
        }

        /// <inheritdoc/>
        public void Edit<T>(IAssetData asset) { }


        /*********
        ** Private Methods 
        *********/
        /// <summary>Invoked once per tick.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        /// <remarks>This is used to reconstruct the default grass cache after it gets edited.</remarks>
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!ReloadDefaultSprites)
                return;

            LoadDefaultGrass(this.Helper.Content.Load<Texture2D>(Path.Combine("TerrainFeatures", "grass"), ContentSource.GameContent));
            ReloadDefaultSprites = false;
        }

        /// <summary>Applies the harmony patches for patching game code.</summary>
        private void ApplyHarmonyPatches()
        {
            // create a new harmony instance for patching source code
            var harmony = HarmonyInstance.Create(ModManifest.UniqueID);

            // apply the patches
            harmony.Patch(
                original: AccessTools.Method(typeof(Grass), nameof(Grass.reduceBy)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(GrassPatch), nameof(GrassPatch.ReduceByPrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Grass), nameof(Grass.seasonUpdate)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(GrassPatch), nameof(GrassPatch.SeasonUpdatePrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Grass), nameof(Grass.loadSprite)),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(GrassPatch), nameof(GrassPatch.LoadSpritePostFix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Grass), nameof(Grass.performToolAction)),
                transpiler: new HarmonyMethod(AccessTools.Method(typeof(GrassPatch), nameof(GrassPatch.PerformToolActionTranspiler)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Grass), nameof(Grass.setUpRandom)),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(GrassPatch), nameof(GrassPatch.SetupRandomPostFix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Grass), nameof(Grass.draw)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(GrassPatch), nameof(GrassPatch.DrawPrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.growWeedGrass)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(GameLocationPatch), nameof(GameLocationPatch.GrowWeedGrassPrefix)))
            );

            // winter grass compatibility patch
            if (this.Helper.ModRegistry.IsLoaded("cat.wintergrass"))
            {
                try
                {
                    this.Monitor.Log("Patching WinterGrass for compatibility");

                    var winterGrassModData = this.Helper.ModRegistry.Get("cat.wintergrass");
                    var winterGrassInstance = (Mod)winterGrassModData.GetType().GetProperty("Mod", BindingFlags.Public | BindingFlags.Instance).GetValue(winterGrassModData);

                    harmony.Patch(
                        original: AccessTools.Method(winterGrassInstance.GetType(), "FixGrassColor"),
                        prefix: new HarmonyMethod(AccessTools.Method(typeof(WinterGrassPatch), nameof(WinterGrassPatch.FixGrassColorPrefix)))
                    );
                }
                catch
                {
                    this.Monitor.Log("Couldn't disable Winter Grass, this may cause texture bugs in winter. Winter Grass is not needed with this mod as this mod adds the ability for grass to live and/or grow in winter", LogLevel.Warn);
                }
            }
        }

        /// <summary>Loads all the content packs for the mod.</summary>
        /// <returns><see langword="true"/> if the default grass sprites should get loaded; otherwise, <see langword="false"/>.</returns>
        private void LoadContentPacks()
        {
            var includeDefaultGrass = true;

            foreach (var contentPack in this.Helper.ContentPacks.GetOwned())
            {
                this.Monitor.Log($"Loading {contentPack.Manifest.Name}");

                // read the content pack config file (and rewrite it to make sure one gets created if it doesn't already exist)
                var contentPackConfig = contentPack.ReadJsonFile<ContentPackConfig>("config.json") ?? new ContentPackConfig();
                contentPack.WriteJsonFile("config.json", contentPackConfig);

                // check if default grass shouldn't be included
                if (!contentPackConfig.EnableDefaultGrass)
                    includeDefaultGrass = false;

                // load seasonal grass sprites
                foreach (var season in new[] { "spring", "summer", "fall", "winter" })
                {
                    var seasonFolder = Path.Combine(contentPack.DirectoryPath, season);
                    if (!Directory.Exists(seasonFolder))
                        continue;

                    this.Monitor.Log($"Loading {season} files");
                    LoadSpritesFromDirectory(seasonFolder, contentPack, season);
                }
            }

            SpringSpritePool.IncludeDefaultGrass = includeDefaultGrass;
            SummerSpritePool.IncludeDefaultGrass = includeDefaultGrass;
            FallSpritePool.IncludeDefaultGrass = includeDefaultGrass;
            WinterSpritePool.IncludeDefaultGrass = includeDefaultGrass;
        }

        /// <summary>Loads all the sprites from the specified directory into the correct sprite pool.</summary>
        /// <param name="directory">The absolute directory containing the sprites.</param>
        /// <param name="contentPack">The content pack currently being loaded.</param>
        /// <param name="season">The season to load the images into.</param>
        private void LoadSpritesFromDirectory(string directory, IContentPack contentPack, string season)
        {
            foreach (var file in Directory.GetFiles(directory))
            {
                // ensure file is an image file
                if (!file.EndsWith(".png"))
                {
                    this.Monitor.Log($"Invalid file in folder: {file}");
                    continue;
                }

                // get the grass texture
                var relativePath = Path.Combine(season, Path.GetFileName(file));

                // a rare bug on Unix causes Directory.GetFiles(string) to return invalid files, it'll return a list of the expected files as well as a copy of each file but prefixed with "._"
                // these files don't actually exist and cause the below to throw an exception, I tried checking if the files started with "._" but that didn't work, in the end silently
                // catching the exception and ignoring it seemed to be the only way for it to work. silently catching shouldn't be a problem here as that shouldn't throw any other exception anyway
                Texture2D grassTexture;
                try { grassTexture = contentPack.LoadAsset<Texture2D>(relativePath); }
                catch { continue; }

                if (grassTexture == null)
                {
                    this.Monitor.Log($"Failed to get grass sprite. Path expected: {relativePath}");
                    continue;
                }

                // add the texture to the correct sprite pool
                switch (season)
                {
                    case "spring": SpringSpritePool.AddCustomGrass(grassTexture); break;
                    case "summer": SummerSpritePool.AddCustomGrass(grassTexture); break;
                    case "fall": FallSpritePool.AddCustomGrass(grassTexture); break;
                    case "winter": WinterSpritePool.AddCustomGrass(grassTexture); break;
                }
            }
        }

        /// <summary>Loads the default grass sprites into the sprite pools.</summary>
        /// <param name="grassTexture">The grass texture.</param>
        private void LoadDefaultGrass(Texture2D grassTexture)
        {
            foreach (var season in new[] { "spring", "summer", "fall", "winter" })
            {
                // clear any existing default grass sprites in the sprite pool
                switch (season)
                {
                    case "spring": SpringSpritePool.ClearDefaultGrass(); break;
                    case "summer": SummerSpritePool.ClearDefaultGrass(); break;
                    case "fall": FallSpritePool.ClearDefaultGrass(); break;
                    case "winter": WinterSpritePool.ClearDefaultGrass(); break;
                }

                // calculate the default grass bounds
                var yOffset = 0;
                switch (season)
                {
                    case "summer": yOffset = 21; break;
                    case "fall": yOffset = 41; break;
                    case "winter": yOffset = 81; break;
                }
                var grassBounds = new[] { new Rectangle(0, yOffset, 15, 20), new Rectangle(16, yOffset, 15, 20), new Rectangle(30, yOffset, 15, 20) };

                // load the individual grass sprites in the correct sprite pool using the above calculated bounds
                foreach (var grassBound in grassBounds)
                {
                    // create a new texture using the grassBound
                    var grassSprite = new Texture2D(Game1.graphics.GraphicsDevice, grassBound.Width, grassBound.Height);
                    var grassData = new Color[grassBound.Width * grassBound.Height];
                    grassTexture.GetData(0, grassBound, grassData, 0, grassData.Length);
                    grassSprite.SetData(grassData);

                    // add sprite to correct sprite pool
                    switch (season)
                    {
                        case "spring": SpringSpritePool.AddDefaultGrass(grassSprite); break;
                        case "summer": SummerSpritePool.AddDefaultGrass(grassSprite); break;
                        case "fall": FallSpritePool.AddDefaultGrass(grassSprite); break;
                        case "winter": WinterSpritePool.AddDefaultGrass(grassSprite); break;
                    }
                }
            }
        }
    }
}
