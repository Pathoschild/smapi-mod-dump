/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://sourceforge.net/p/sdvmod-damage-overlay/
**
*************************************************/

/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using StardewModdingAPI.Utilities;

namespace DamageOverlay
    {
    class TextureWithPost
        {
        public readonly Texture2D Texture;
        public Rectangle Crop;
        public float Rotation = 0.0f;
        public Color BlendColor;
        public Vector2 _Scale;
        public Vector2 _Origin;
        public TextureWithPost(Texture2D texture) {
            Texture = texture;
            }

        public TextureWithPost UpScaleTo(Point upDimensions) {
            _Scale = new Vector2(
                1.0f * upDimensions.X / Crop.Width,
                1.0f * upDimensions.Y / Crop.Height
                );
            return this;
            }
        }

    class OverlaySource
        {
        public static IModHelper Helper;

        private readonly string AssetDir;
        private readonly IContentPack ContentPack = null;

        public OverlaySource(string localAssetName) {
            AssetDir = Path.Combine("assets", localAssetName);
            }
        public OverlaySource(IContentPack contentPack, string cpRelativeAssetDir) {
            ContentPack = contentPack;
            AssetDir = cpRelativeAssetDir;
            }

        public OverlayDefinition GetDefinition() {
            string ovjson = Path.Combine(AssetDir, "overlay.json");
            return ContentPack?.ReadJsonFile<OverlayDefinition>(ovjson) ?? Helper.Data.ReadJsonFile<OverlayDefinition>(ovjson);
            }

        public T GetAsset<T>(string filename) {
            string filepath = Path.Combine(AssetDir, filename);
            return ContentPack is null ? Helper.Content.Load<T>(filepath) : ContentPack.LoadAsset<T>(filepath);
            }

        public string OverlayJsonFullPath =>
            Path.Combine(
                ContentPack?.DirectoryPath ?? Helper.DirectoryPath,
                AssetDir,
                "overlay.json"
                );

        public string AssetFullPath(string asset) =>
            Path.Combine(
                ContentPack?.DirectoryPath ?? Helper.DirectoryPath,
                AssetDir,
                asset
                );

        }

    class DamageOverlay {
        private readonly IModHelper Helper;
        private readonly IManifest ModManifest;
        private ModConfig Config;

        readonly Dictionary<string, OverlaySource> OverlaySources = new Dictionary<string, OverlaySource>();
        // Do not 'fold' Textures into Thresholds; we keep it separate to speed up CalculateScale
        readonly Dictionary<string, TextureWithPost> Textures = new Dictionary<string, TextureWithPost>();
        readonly SortedDictionary<int, TextureWithPost> Thresholds = new SortedDictionary<int, TextureWithPost>();

        //private bool GameIsLoaded = false;
        private Vector2? ScreenCenter = null;
        private int PreviousHP = 100;
        private int CurrentHP = 100;
        private bool DisableUntilHPChanges = false;

        private Point CurrentDispSize = new Point();
        private Point PreviousDispSize = new Point();

        private readonly HashSet<string> BuiltInOverlaySets = new HashSet<string> { "simple", "bloody" };

        /**************************************************
         * CONSTRUCTORS
         **************************************************/

        public DamageOverlay(Mod mod) {
            OverlaySource.Helper = mod.Helper;
            Helper = mod.Helper;
            ModManifest = mod.ModManifest;
            Config = mod.Helper.ReadConfig<ModConfig>();
            GetSources();
            LoadOverlay();
            }

        /**************************************************
         * EVENT HANDLERS
         **************************************************/

        public void OnButtonsChanged(object sender, ButtonsChangedEventArgs args) {
            if (!DisableUntilHPChanges && Config.temp_hide_hotkey.JustPressed()) {
                var hudmsg = new HUDMessage(Helper.Translation.Get("do_hidden_temp"), "");
                Game1.addHUDMessage(hudmsg);
                DisableUntilHPChanges = true;
                }
            }

        public void OnGameLaunched(object sender, GameLaunchedEventArgs args) {
            RegisterMenu();
            }

        // Set to low so we _should_ be drawn over any other mods adding accoutrements to the gameworld.
        // Can't rely on this, though, so it's just "best effort" instead of "guarantee".
        // And I guarantee I won't spend any additional effort to 'solve' this 'not-really-a-problem' :D
        [EventPriority(EventPriority.Low)]
        public void OnRenderedWorld(object sender, RenderedWorldEventArgs args) {
            CurrentHP = 100 * Game1.player.health / Game1.player.maxHealth;
            if (DisableUntilHPChanges)
                if (CurrentHP == PreviousHP)
                    return;
                else
                    DisableUntilHPChanges = false;
            PreviousHP = CurrentHP;
            UpdateDispSizes();
            DrawOverlay(args.SpriteBatch);
            }

        public void OnCommand(string _, string[] args) {
            if (args.Length < 1) return;
            switch (args[0]) {
                case "sethp":
                    Log.Info($"setting player's Health Percentage to {args[1]}");
                    Game1.player.health = Convert.ToInt32(args[1]) * Game1.player.maxHealth / 100;
                    return;
                case "calcscale":
                    Log.Info($"Forcing scale recalculation, CurrentDispSize = {CurrentDispSize}");
                    CalculateScales();
                    return;
                case "reload":
                    Log.Info($"Reloading current overlay set '{Config.overlay}'");
                    LoadOverlay();
                    return;
                case "loadset":
                    if (args.Length < 2) {
                        Log.Error("Not enough args");
                        return;
                        }
                    if (!OverlaySources.ContainsKey(args[1])) {
                        Log.Error($"Overlay set '{args[1]}' not defined! Maybe you should run 'rescanpacks' first?");
                        return;
                        }
                    Log.Info($"Changing overlay set to '{args[1]}', loading immediately");
                    Config.overlay = args[1];
                    LoadOverlay();
                    Log.Warn("This change is not yet written to config.json, and thus is NOT permanent!");
                    return;
                case "rescanpacks":
                    Log.Info("Rescanning Content Packs");
                    Log.Warn("This won't affect currently-loaded overlay set; use 'reload' or 'loadset' to effect changes.");
                    GetSources();
                    return;
                }
            }

        public void OnWindowResized(object semder, WindowResizedEventArgs args) {
            UpdateDispSizes();
            }

        /**************************************************
         * Data Structure initialization logic
         **************************************************/

        private void GetSources() {
            OverlaySources.Clear();

            foreach (IContentPack cp in Helper.ContentPacks.GetOwned()) {
                Log.Debug($"Reading content pack: {cp.Manifest.UniqueID}|v{cp.Manifest.Version} from {cp.DirectoryPath}");
                Log.Trace($"  Friendly name: {cp.Manifest.Name}");
                if (!cp.HasFile("content.json")) {
                    Log.Trace("  Does not have content.json, skipping");
                    continue;
                    }
                ContentPackContent cp_content = cp.ReadJsonFile<ContentPackContent>("content.json");
                if (cp_content is null) {
                    Log.Trace("  Error parsing content.json, skipping");
                    continue;
                    }
                foreach (var kvp in cp_content.overlays) {
                    if (BuiltInOverlaySets.Contains(kvp.Key)) {
                        Log.Trace($"  Overwriting built-in overlay set '{kvp.Key}' is forbidden, skipping!");
                        continue;
                        }
                    if (OverlaySources.ContainsKey(kvp.Key))
                        Log.Trace($"  Overwriting previous overlay set '{kvp.Key}'");
                    OverlaySources[kvp.Key] = new OverlaySource(cp, kvp.Value);
                    }
                Log.Trace($"  Added {cp_content.overlays.Count} overlays from {cp.Manifest.UniqueID}");
                }

            foreach(var nm in BuiltInOverlaySets) {
                Log.Debug($"Adding built-in overlay set '{nm}'");
                OverlaySources[nm] = new OverlaySource(nm);
                }
            }

        private void LoadOverlay() {
            Log.Info($"Loading overlay '{Config.overlay}' from assets");

            Thresholds.Clear();
            Textures.Clear();
            PreviousDispSize = new Point();  // This will force scale recalculation

            Dictionary<string, Texture2D> images = new Dictionary<string, Texture2D>();
            OverlaySource source = OverlaySources[Config.overlay];

            OverlayDefinition data = source.GetDefinition();
            if (data is null) {
                Log.Error($"Cannot load overlay definition file {source.OverlayJsonFullPath}");
                Log.Warn("Mod will not run!");
                return;
                }

            foreach(var kvp in data.images) {
                string image_label = kvp.Key;
                string image_filename = kvp.Value;
                Log.Trace($"Loading image {image_label} => {source.AssetFullPath(image_filename)}");
                try {
                    images.Add(image_label, source.GetAsset<Texture2D>(image_filename));
                    }
                catch (Exception ex) {
                    Log.Error($"Failed loading image {image_label} ; technical details:\n{ex}");
                    }
                }
            Log.Trace("Images loaded");

            foreach (var kvp in data.textures) {
                string texture_label = kvp.Key.Trim();
                if (texture_label == "") {
                    Log.Error($"Invalid texture label, skipping!");
                    continue;
                    }
                TextureDefinition texture_data = kvp.Value;
                Log.Trace($"Processing texture {texture_label} => {texture_data}");
                if (!images.TryGetValue(texture_data.image, out Texture2D texturr)) {
                    Log.Error($"Texture '{texture_label}' referring to non-existent image '{texture_data.image}', skipping!");
                    continue;
                    }
                Rectangle _crop = texture_data.crop?.ToRectangle() ?? texturr.Bounds;
                //Color _basecolor = texture_data.blend_color?.ToColor() ?? Color.White;
                Color _basecolor = Color.White;
                var origin = new Vector2(_crop.Width >> 1, _crop.Height >> 1);
                var opacity = 1.0f - Math.Max(0.0f, Math.Min(1.0f, texture_data.transparency));
                Textures.Add(texture_label, new TextureWithPost(texturr) {
                    Crop = _crop,
                    BlendColor = _basecolor * opacity,
                    Rotation = texture_data.rotation,
                    _Origin = origin
                    // _Scale depends on viewport size, so it will be calculated in CalculateScales()
                    });
                }
            Log.Trace("Textures post-processing loaded");

            foreach(var kvp in data.thresholds) {
                int hp = kvp.Key;
                string texture_label = kvp.Value;
                if (!Textures.TryGetValue(texture_label, out TextureWithPost twp)) {
                    Log.Error($"Threshold {hp} refers to non-existent texture '{texture_label}', skipping!");
                    continue;
                    }
                Thresholds.Add(hp, twp);
                }
            Log.Debug($"Sorted thresholds: {string.Join(", ", Thresholds.Keys)}");
            }

        public void CalculateScales() {
            string texture_name;
            TextureWithPost post_process;
            foreach(var kvp in Textures) {
                texture_name = kvp.Key;
                post_process = kvp.Value;
                post_process.UpScaleTo(CurrentDispSize);
                Log.Trace($"Calculated scale for '{texture_name}' : {post_process._Scale}");
                }
            }

        public void UpdateDispSizes() {
            var uirect = Game1.viewport;
            CurrentDispSize = new Point(uirect.Width, uirect.Height);
            if (CurrentDispSize == PreviousDispSize) return;
            Log.Trace($"DispSize changed {PreviousDispSize} -> {CurrentDispSize}");
            CalculateScales();
            ScreenCenter = new Vector2(CurrentDispSize.X >> 1, CurrentDispSize.Y >> 1);
            PreviousDispSize = CurrentDispSize;
            }

        /**************************************************
         * Critical Path
         **************************************************/

        public void DrawOverlay(SpriteBatch b) {
            // Note to Maintainers: Try to keep this method short'n'sweet; reduce calculations -- esp float ones -- as much as possible

            if (ScreenCenter is null) return;

            TextureWithPost twp = null;
            foreach (var kvp in Thresholds) {
                if (CurrentHP > kvp.Key) continue;
                twp = kvp.Value;
                break;
                }
            if (twp is null) return;

            b.Draw(
                texture: twp.Texture,
                position: ScreenCenter.Value,  // where to put the "origin" of source image
                sourceRectangle: twp.Crop,
                color: twp.BlendColor,
                rotation: twp.Rotation,
                origin: twp._Origin,    // where the "origin" point is in the source image
                scale: twp._Scale,
                effects: SpriteEffects.None,
                layerDepth: 0.0f
                );

            // For explanation on 'position' and 'origin', see: https://gamedev.stackexchange.com/a/127692/151274

            }

        /**************************************************
         * Generic Mod Config Menu thingamabobs
         **************************************************/

        public void RegisterMenu() {
            var api = Helper.ModRegistry.GetApi<IGenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");
            if (api == null) {
                Log.Debug("GenericModConfigMenu not installed, skipping menu registry.");
                return;
                }

            api.RegisterModConfig(ModManifest, () => Config = new ModConfig(), () => CommitConfig());

            api.SetDefaultIngameOptinValue(ModManifest, true);
            api.RegisterSimpleOption(ModManifest,
                "Hotkey to temporarily hide overlay", "",
                () => Config.temp_hide_hotkey,
                (KeybindList val) => Config.temp_hide_hotkey = val
                );
            api.RegisterChoiceOption(ModManifest,
                "Overlay set to use", "",
                () => Config.overlay,
                (string val) => Config.overlay = val,
                OverlaySources.Keys.ToArray()
                );
            api.RegisterParagraph(ModManifest,
                "IMPORTANT:\nClicking Save or Save & Close _immediately_ (re)loads the chosen overlay, even if the overlay set is not changed!"
                );

            Log.Trace("Menu registered");
            }

        public void CommitConfig() {
            Helper.WriteConfig<ModConfig>(Config);
            LoadOverlay();
            }
        }
    }
