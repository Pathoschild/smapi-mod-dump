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
using StardewValley;
using StardewValley.TerrainFeatures;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace MoreGrass
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Accessors 
        *********/
        /// <summary>The mod configuration.</summary>
        public ModConfig Config { get; private set; }

        /// <summary>The list of all loaded spring grass sprites.</summary>
        public List<Texture2D> SpringGrassSprites { get; private set; } = new List<Texture2D>();

        /// <summary>The list of all loaded summer grass sprites.</summary>
        public List<Texture2D> SummerGrassSprites { get; private set; } = new List<Texture2D>();

        /// <summary>The list of all loaded fall grass sprites.</summary>
        public List<Texture2D> FallGrassSprites { get; private set; } = new List<Texture2D>();

        /// <summary>The list of all loaded winter grass sprites.</summary>
        public List<Texture2D> WinterGrassSprites { get; private set; } = new List<Texture2D>();

        /// <summary>The singleton instance of <see cref="MoreGrass.ModEntry"/>.</summary>
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

            ApplyHarmonyPatches();
            bool loadDefaultGrass = LoadContentPacks();
            if (loadDefaultGrass)
                LoadDefaultGrass();
            else
                this.Monitor.Log("Skipping loading default sprites due to content pack config");

            // once all content packs are loaded, make sure there is atleast one sprite in each sprite pool
            ValidateSpritePools();

            this.Monitor.Log($"A total of {SpringGrassSprites.Count} spring sprites have been loaded");
            this.Monitor.Log($"A total of {SummerGrassSprites.Count} summer sprites have been loaded");
            this.Monitor.Log($"A total of {FallGrassSprites.Count} fall sprites have been loaded");
            this.Monitor.Log($"A total of {WinterGrassSprites.Count} winter sprites have been loaded");
        }


        /*********
        ** Private Methods 
        *********/
        /// <summary>Applies the harmony patches for patching game code.</summary>
        private void ApplyHarmonyPatches()
        {
            // create a new harmony instance for patching source code
            var harmony = HarmonyInstance.Create(ModManifest.UniqueID);

            // apply the patches
            harmony.Patch(
                original: AccessTools.Method(typeof(Grass), nameof(Grass.seasonUpdate)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(GrassPatch), nameof(GrassPatch.SeasonUpdatePrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Grass), nameof(Grass.loadSprite)),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(GrassPatch), nameof(GrassPatch.LoadSpritePostFix)))
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
        private bool LoadContentPacks()
        {
            var loadDefaultGrass = true;

            foreach (var contentPack in this.Helper.ContentPacks.GetOwned())
            {
                this.Monitor.Log($"Loading {contentPack.Manifest.Name}");

                // read the content pack config file
                var contentPackConfig = contentPack.ReadJsonFile<ContentPackConfig>("config.json");
                if (contentPackConfig == null)
                    contentPack.WriteJsonFile("config.json", new ContentPackConfig());

                // check if default grass shouldn't be loaded
                if (!contentPackConfig.EnableDefaultGrass)
                    loadDefaultGrass = false;

                // load seasonal grass sprites
                var seasons = new[] { "spring", "summer", "fall", "winter" };
                foreach (var season in seasons)
                {
                    var seasonFolder = Path.Combine(contentPack.DirectoryPath, season);
                    if (!Directory.Exists(seasonFolder))
                        continue;

                    this.Monitor.Log($"Loading {season} files");
                    LoadSpritesFromDirectory(seasonFolder, contentPack, season);
                }
            }

            return loadDefaultGrass;
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
                var grassTexture = contentPack.LoadAsset<Texture2D>(relativePath);
                if (grassTexture == null)
                {
                    this.Monitor.Log($"Failed to get grass sprite. Path expected: {relativePath}");
                    continue;
                }

                // add the texture to the correct sprite pool
                switch (season)
                {
                    case "spring": SpringGrassSprites.Add(grassTexture); break;
                    case "summer": SummerGrassSprites.Add(grassTexture); break;
                    case "fall": FallGrassSprites.Add(grassTexture); break;
                    case "winter": WinterGrassSprites.Add(grassTexture); break;
                }
            }
        }

        /// <summary>Loads the default grass sprites into the sprite pools.</summary>
        private void LoadDefaultGrass()
        {
            var seasons = new[] { "spring", "summer", "fall", "winter" };
            foreach (var season in seasons)
                LoadDefaultGrass(season);
        }

        /// <summary>Loads the default game grass sprites for the given season into its respective sprite pool.</summary>
        /// <param name="season">The season of grass sprites to load.</param>
        private void LoadDefaultGrass(string season)
        {
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
            var grassTexture = this.Helper.Content.Load<Texture2D>(Path.Combine("TerrainFeatures", "grass"), ContentSource.GameContent);
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
                    case "spring": SpringGrassSprites.Add(grassSprite); break;
                    case "summer": SummerGrassSprites.Add(grassSprite); break;
                    case "fall": FallGrassSprites.Add(grassSprite); break;
                    case "winter": WinterGrassSprites.Add(grassSprite); break;
                }
            }
        }
        
        /// <summary>Ensures there is atleast one sprite in each season pool.</summary>
        /// <remarks>If there is a sprite pool without any sprites, then the default grass sprites for that season will get loaded.</remarks>
        private void ValidateSpritePools()
        {
            if (SpringGrassSprites.Count == 0)
            {
                Monitor.Log("Loading default spring sprites as no loaded sprites found.");
                LoadDefaultGrass("spring");
            }

            if (SummerGrassSprites.Count == 0)
            {
                Monitor.Log("Loading default summer sprites as no loaded sprites found.");
                LoadDefaultGrass("summer");
            }

            if (FallGrassSprites.Count == 0)
            {
                Monitor.Log("Loading default fall sprites as no loaded sprites found.");
                LoadDefaultGrass("fall");
            }

            if (WinterGrassSprites.Count == 0)
            {
                Monitor.Log("Loading default winter sprites as no loaded sprites found.");
                LoadDefaultGrass("winter");
            }
        }
    }
}
