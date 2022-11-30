/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/BleakCodex/SpritesInDetail
**
*************************************************/

using ContentPatcher;
using GenericModConfigMenu;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SpritesInDetail
{
    public class ModEntry : Mod
    {

        List<HDTextureInfo> hdTextures = new List<HDTextureInfo>();
        HashSet<string> spritesToInvalidateDaily = new HashSet<string>();

        //This is copied from Stardew Valley FamerRenderer.cs
        //This is needed as Stardew will copy the data between textures to provide coloring
        public static bool textureChanged(ref Texture2D ___baseTexture, ref LocalizedContentManager ___farmerTextureManager, ref NetString ___textureName, ref Dictionary<string, Dictionary<int, List<int>>> ____recolorOffsets)
        {
            if (___baseTexture != null)
            {
                ___baseTexture.Dispose();
                ___baseTexture = null;
            }
            Texture2D source_texture = ___farmerTextureManager.Load<Texture2D>(___textureName.Value);
            if (source_texture is ReplacedTexture)
            {
                ___baseTexture = new ReplacedTexture(new Texture2D(Game1.graphics.GraphicsDevice, source_texture.Width, source_texture.Height), source_texture, ((ReplacedTexture)source_texture).HDTextureInfo);
            } else
            {
                ___baseTexture = new Texture2D(Game1.graphics.GraphicsDevice, source_texture.Width, source_texture.Height);
            }
            Color[] data = new Color[source_texture.Width * source_texture.Height];
            source_texture.GetData(data, 0, data.Length);
            ___baseTexture.SetData(data);

            return false;
        }

        ////This is copied from Stardew Valley FamerRenderer.cs
        //To make recolors an index is built using specific pixels in the texture.
        //If we increase the size of the texture we'll have to make sure to adjust the source locations
        public static bool _GeneratePixelIndices(Dictionary<string, Dictionary<int, List<int>>> ____recolorOffsets, int source_color_index, string texture_name, Color[] pixels)
        {
            int colorLocationMultiplier = 1;
            if (pixels.Length > 193536)
            {
                //Assuming a scale of 4 for non-vanilla textures;
                colorLocationMultiplier = 4;
            }
            Color source_color = pixels[source_color_index * colorLocationMultiplier];
            List<int> pixel_indices = new List<int>();
            for (int i = 0; i < pixels.Length; i++)
            {
                if (pixels[i].PackedValue == source_color.PackedValue)
                {
                    pixel_indices.Add(i);
                }
            }
            ____recolorOffsets[texture_name][source_color_index] = pixel_indices;
            return false;
        }


        List<Tuple<string, IManifest>> tokensToRegister = new List<Tuple<string, IManifest>>();


        public override void Entry(IModHelper helper)
        {
            var harmony = new Harmony(this.ModManifest.UniqueID);

            foreach (MethodInfo method in typeof(ModEntry).GetMethods(BindingFlags.Static | BindingFlags.Public).Where(m => m.Name == "Draw"))
            {
                //Shotgun approach
                harmony.Patch(typeof(SpriteBatch).GetMethod("Draw", method.GetParameters().Select(p => p.ParameterType).Where(t => !t.Name.Contains("SpriteBatch")).ToArray()), new HarmonyMethod(method));
            }


            foreach (IContentPack contentPack in this.Helper.ContentPacks.GetOwned())
            {
                Content? data = contentPack.ReadJsonFile<Content>("content.json");
                dynamic? dynamicConfig = contentPack.ReadJsonFile<dynamic>("config.json");

                Dictionary<string, string> contentPackSettings = new Dictionary<string, string>();

                this.Monitor.Log($"Reading content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version} from {contentPack.DirectoryPath}");

                if (!contentPack.HasFile("content.json"))
                {
                    this.Monitor.Log($"Missing content.json; No changes will be applied.", LogLevel.Error);
                }
                else
                {
                    Dictionary<string, object> config;
                    if (dynamicConfig is null)
                    {
                        config = new Dictionary<string, object>();
                        config.Add("Enabled", "true");
                    }
                    else
                    {
                        config = dynamicConfig.ToObject<Dictionary<string, object>>();
                    }

                    foreach (KeyValuePair<string, object> keyValue in config)
                    {
                        if (keyValue.Value is null)
                        {
                            continue;
                        }

                        contentPackSettings.Add(keyValue.Key, keyValue.Value.ToString() ?? string.Empty);

                        if (keyValue.Key != "Enabled")
                        {
                            //Save a list of tokens we've registered, so that we can form the correct Conditional keys
                            tokensToRegister.Add(new Tuple<string, IManifest>(keyValue.Key, contentPack.Manifest));
                        }
                    }

                    if (data != null)
                    {

                        bool farmerWasReplaced = false;

                        foreach (Sprite sprite in data.Sprites)
                        {
                            Dictionary<string, string?> conditionals = new Dictionary<string, string?>();
                            if (sprite.When is not null)
                            {
                                foreach (var when in sprite.When)
                                {
                                    if (tokensToRegister.Any(t => t.Item1 == when.Key))
                                    {
                                        //Conditionals created from config values need to be accessed via the CP's uid.
                                        conditionals.Add($"{contentPack.Manifest.UniqueID}/{when.Key}", when.Value);
                                    }
                                    else
                                    {
                                        conditionals.Add(when.Key, when.Value);
                                        //For now, we'll mark any sprites which have a non-config When conditional to re-validate daily
                                        spritesToInvalidateDaily.Add(sprite.Target);
                                    }
                                }
                            }

                            bool isFarmerTexture = false;
                            if (sprite.Target.Contains("Farmer"))
                            {
                                isFarmerTexture = true;
                                farmerWasReplaced = true;
                            }

                            this.Monitor.Log($"Found replacement sprite {sprite.Target} {sprite.FromFile} {sprite.SpriteWidth} {sprite.SpriteHeight}", LogLevel.Trace);

                            Texture2D? textureFromFile = null;
                            if (sprite.FromFile is not null)
                            {
                                textureFromFile = contentPack.ModContent.Load<Texture2D>(sprite.FromFile);
                            }

                            HDTextureInfo textureInfo = new HDTextureInfo(sprite, contentPack.Manifest, textureFromFile, conditionals, isFarmerTexture);

                            for(int i = 0; i < sprite.PixelReplacements.Count; i++)
                            {

                                if (sprite.PixelReplacements[i].TargetX == null)
                                {
                                    this.Monitor.Log($"Missing TargetX for replacement {sprite.Target}.PixelReplacements[{i}], skipping...", LogLevel.Error);
                                    continue;
                                }
                                if (sprite.PixelReplacements[i].TargetY == null)
                                {
                                    this.Monitor.Log($"Missing TargetY for replacement {sprite.Target}.PixelReplacements[{i}], skipping...", LogLevel.Error);
                                    continue;
                                }
                                if (sprite.PixelReplacements[i].FromFile == null)
                                {
                                    this.Monitor.Log($"Missing FromFile for replacement {sprite.Target}.PixelReplacements[{i}], skipping...", LogLevel.Error);
                                    continue;
                                }

                                textureInfo.PixelReplacements.Add(new Vector2((float)sprite.PixelReplacements[i].TargetX, (float)sprite.PixelReplacements[i].TargetY), contentPack.ModContent.Load<Texture2D>(sprite.PixelReplacements[i].FromFile));
                            }

                            hdTextures.Add(textureInfo);
                        }

                        //Only patch the default behavior if at least one farmer texture is being replaced
                        if (farmerWasReplaced)
                        {
                            harmony.Patch(typeof(FarmerRenderer).GetMethod("textureChanged", BindingFlags.NonPublic | BindingFlags.Instance), new HarmonyMethod(typeof(ModEntry).GetMethod("textureChanged")));
                            harmony.Patch(typeof(FarmerRenderer).GetMethod("_GeneratePixelIndices", BindingFlags.NonPublic | BindingFlags.Instance), new HarmonyMethod(typeof(ModEntry).GetMethod("_GeneratePixelIndices")));
                        }
                    }

                    if (contentPackSettings.Count > 0)
                    {
                        settings.Add(contentPack.Manifest, contentPackSettings);
                    }
                }
            }

            //Handle daily Conditional checks
            Helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
            //Load ContentPacks and configs, create Config Menus, register Content Patcher tokens
            Helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
            //Wait for ContentPatcherAPI to be available
            Helper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;
            //Handle Content Load and Replacement
            Helper.Events.Content.AssetRequested += Content_AssetRequested;
        }

        private void GameLoop_DayStarted(object? sender, DayStartedEventArgs e)
        {
            if (spritesToInvalidateDaily.Count > 0)
            {
                this.Helper.GameContent.InvalidateCache(asset => spritesToInvalidateDaily.Any(s => s == asset.Name.BaseName));
            }
        }

        public static Dictionary<IManifest, Dictionary<string, string>> settings = new Dictionary<IManifest, Dictionary<string, string>>();
        private void GameLoop_GameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            var configMenuApi = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            var cpApi = this.Helper.ModRegistry.GetApi<IContentPatcherAPI>("Pathoschild.ContentPatcher");

            foreach (Tuple<string, IManifest> tokenKeyToManifest in tokensToRegister)
            {
                cpApi.RegisterToken(tokenKeyToManifest.Item2, tokenKeyToManifest.Item1, () =>
                {
                    return new[] { settings[tokenKeyToManifest.Item2][tokenKeyToManifest.Item1] };
                });
            }

            foreach (IContentPack contentPack in this.Helper.ContentPacks.GetOwned())
            {

                this.Monitor.Log($"Reading content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version} from {contentPack.DirectoryPath}");

                if (contentPack.HasFile("content.json"))
                {
                    Content? data = contentPack.ReadJsonFile<Content>("content.json");
                    dynamic? dynamicConfig = contentPack.ReadJsonFile<dynamic>("config.json");

                    List<string> tokens = new List<string>();

                    if (data is not null && cpApi is not null)
                    {
                        if (configMenuApi is not null)
                        {
                            configMenuApi.Register(
                                mod: contentPack.Manifest,
                                reset: () => settings[contentPack.Manifest]["Enabled"] = "true",
                                save: () =>
                                {
                                    this.Helper.GameContent.InvalidateCache(asset => data.Sprites.Any(s => s.Target == asset.Name.BaseName));
                                    contentPack.WriteJsonFile<dynamic>("config.json", settings[contentPack.Manifest]);
                                }
                            );
                        }
                    }
                }
            }
        }

        public static int tick = 0;
        private void GameLoop_UpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            tick++;
            //Need to wait until at least the second tick to access ContentPatcher Conditions API
            if (tick >= 2)
            {
                var configMenuApi = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
                var cpApi = this.Helper.ModRegistry.GetApi<IContentPatcherAPI>("Pathoschild.ContentPatcher");

                if (configMenuApi != null) {
                    foreach (var setting in settings)
                    {
                        foreach (var contentPackSetting in setting.Value) {
                            if (contentPackSetting.Key == "Enabled")
                            {
                                configMenuApi.AddBoolOption(
                                    mod: setting.Key,
                                    name: () => "Enabled",
                                    getValue: () => { 
                                        return settings[setting.Key]["Enabled"].ToLower() == "true"; 
                                    },
                                    setValue: value =>
                                    {
                                        settings[setting.Key][contentPackSetting.Key] = value.ToString();
                                    }
                                );
                            } else
                            {
                                configMenuApi.AddTextOption(
                                    mod: setting.Key,
                                    name: () => contentPackSetting.Key,
                                    getValue: () => {
                                        return settings[setting.Key][contentPackSetting.Key];
                                    },
                                    setValue: value =>
                                    {
                                        settings[setting.Key][contentPackSetting.Key] = value;
                                    }
                                );
                            }

                            configMenuApi.OnFieldChanged(
                                mod: setting.Key,
                                onChange: (string s, object o) =>
                                {
                                    settings[setting.Key][contentPackSetting.Key] = o.ToString() ?? "";
                                }
                            );
                        }
                    }
                }

                //Clean up subscription we don't care about anymore
                Helper.Events.GameLoop.UpdateTicked -= GameLoop_UpdateTicked;
            }
        }

        private void Content_AssetRequested(object? sender, AssetRequestedEventArgs e)
        {
            foreach (HDTextureInfo info in hdTextures)
            {
                if (e.Name.IsEquivalentTo(info.Target)) 
                {
                    e.Edit(
                        (asset) =>
                        {
                            var cpApi = this.Helper.ModRegistry.GetApi<IContentPatcherAPI>("Pathoschild.ContentPatcher");

                            bool enabled = true;
                            if (settings.ContainsKey(info.ContentPackManifest)&& settings[info.ContentPackManifest].ContainsKey("Enabled") && settings[info.ContentPackManifest]["Enabled"].ToLower() == "false")
                            {
                                enabled = false;
                            }
                            
                            if (enabled && info.Conditionals.Count > 0 && cpApi is not null && cpApi.IsConditionsApiReady)
                            {
                                IManagedConditions managedConditions = cpApi.ParseConditions(info.ContentPackManifest, info.Conditionals, new SemanticVersion("1.28.0"));
                                enabled = managedConditions.IsMatch;
                            }

                            if (enabled)
                            {
                                ReplacedTexture replacement;

                                if (info.Target.Contains("Farmer"))
                                {
                                    replacement = new ReplacedTexture(asset.AsImage().Data, info.HDTexture, info, info.HDTexture.Width, info.HDTexture.Height);
                                    IAssetDataForImage assetImage = asset.AsImage();

                                    //Populate the texture's data. This is neccissary for SiD Farmer changes, though technically we may be able to skip for other sprites
                                    Color[] data = new Color[info.HDTexture.Width * info.HDTexture.Height];
                                    info.HDTexture.GetData(data, 0, data.Length);
                                    replacement.SetData(data);
                                    
                                }
                                else if (info.PixelReplacements.Count > 0)
                                {
                                    replacement = new ReplacedTexture(asset.AsImage().Data, info.HDTexture, info);
                                    IAssetDataForImage assetImage = asset.AsImage();

                                    Color[] data = new Color[assetImage.Data.Width * assetImage.Data.Height];
                                    assetImage.Data.GetData(data, 0, data.Length);
                                    replacement.SetData(data);
                                }
                                else
                                {
                                    replacement = new ReplacedTexture(asset.AsImage().Data, info.HDTexture, info);
                                }

                                this.Monitor.Log($"Replacing Texture for {info.Target}", LogLevel.Trace);
                                asset.AsImage().ReplaceWith(replacement);
                            } else
                            {
                                this.Monitor.Log($"Skipping Replacement of Texture for {info.Target}", LogLevel.Trace);
                            }
                        });
                }
            }
        }



        private static bool spriteAlreadyDrawn = false;
        public static bool DrawReplacedTexture(SpriteBatch __instance, Texture2D texture, Rectangle destination, Rectangle? sourceRectangle, Color color, Vector2 origin, Vector2 scale, float rotation = 0f, SpriteEffects effects = SpriteEffects.None, float layerDepth = 0f)
        {
            //This is because of the overrides calling the higher-parameter method
            //It appears Draw 4/5 are the most commonly called, so it may be unnecessary to inject into the others
            if (spriteAlreadyDrawn || !sourceRectangle.HasValue)
            {
                return true;
            }

            if (texture is ReplacedTexture a && sourceRectangle != null && sourceRectangle.Value is Rectangle r)
            {
                Rectangle updatedDestination;
                Rectangle? updatedSource;
                Vector2 updatedOrigin;

                if (a.HDTextureInfo.HDTexture is null)
                {
                    Vector2 currentSourceLocation = new Vector2(r.X, r.Y);
                    if (a.HDTextureInfo.PixelReplacements.ContainsKey(currentSourceLocation))
                    {
                        Texture2D replacementTexture = a.HDTextureInfo.PixelReplacements[currentSourceLocation];

                        //Replace the attempted render with the replaced value
                        updatedOrigin = new Vector2(origin.X * (float)replacementTexture.Width / (float)r.Width, origin.Y * (float)replacementTexture.Height / (float)r.Height);

                        spriteAlreadyDrawn = true;
                        __instance.Draw(replacementTexture, destination, null, color, rotation, updatedOrigin, effects, layerDepth);
                        spriteAlreadyDrawn = false;
                        return false;
                    }

                    //Nothing to replace
                    return true;
                }


                //Male/Female 'Breather' sprite
                // These are textures drawn on top of the NPC's chest, and stretched, to appear as if they are breathing
                if ((r.Width == 8 && r.Height == 8) || (r.Width == 8 && r.Height == 4))
                {
                    if (a.HDTextureInfo.DisableBreath == true)
                    {
                        return false;
                    }

                    updatedDestination = new Rectangle(destination.X + a.HDTextureInfo.ChestAdjustX, destination.Y + a.HDTextureInfo.ChestAdjustY, (int)(a.HDTextureInfo.ChestSourceWidth * scale.X / 2), (int)(a.HDTextureInfo.ChestSourceHeight * scale.Y / 2));
                    updatedSource = new Rectangle?(new Rectangle((int)(16 * a.HDTextureInfo.WidthScale * (r.X / 16)) + a.HDTextureInfo.ChestSourceX, (int)((32 * a.HDTextureInfo.HeightScale) * (r.Y / 32)) + a.HDTextureInfo.ChestSourceY, a.HDTextureInfo.ChestSourceWidth, a.HDTextureInfo.ChestSourceHeight));
                    updatedOrigin = new Vector2(a.HDTextureInfo.ChestSourceWidth / 2, a.HDTextureInfo.ChestSourceHeight / 2 + 1);
                }
                else
                {
                    //The destination is the X,Y coordinates of the Origin.
                    //Therefore, if you increase the width of the sprite, you have to make sure to update the origin, but don't need to alter the destination.
                    updatedDestination = new Rectangle(destination.X, destination.Y, (int)(a.HDTextureInfo.SpriteWidth * scale.X), (int)(a.HDTextureInfo.SpriteHeight * scale.Y));
                    updatedSource = new Rectangle?(new Rectangle((int)(r.X * a.HDTextureInfo.WidthScale), (int)(r.Y * a.HDTextureInfo.HeightScale), (int)(r.Width * a.HDTextureInfo.WidthScale), (int)(r.Height * a.HDTextureInfo.HeightScale)));

                    //The Origin of the sprite is in the lower center, lower 3/4s, and looks to be around where the upper-center of the shadow is drawn
                    updatedOrigin = new Vector2(a.HDTextureInfo.SpriteOriginX, a.HDTextureInfo.SpriteOriginY);
                    
                    if (a.HDTextureInfo.IsFarmer)
                    {
                        if (origin.X == 0 && origin.Y == 0)
                        {
                            //Headshots
                            if (r.Height == 15)
                            {
                                updatedDestination = destination;
                                updatedSource = new Rectangle?(new Rectangle(16, 64, 32, 32));
                                updatedOrigin = new Vector2(0, 0);
                            }
                            else if (r.Width == 6 && r.Height == 2)
                            {   //Blink them lashes
                                if (r.X == 5)
                                {
                                    //Skin
                                    updatedSource = new Rectangle?(new Rectangle(26, 96, 12, 4));
                                    updatedDestination = destination;
                                }
                                else
                                {
                                    //Eyes
                                    updatedSource = new Rectangle?(new Rectangle(r.X * 4, r.Y * 4, 24, 8));
                                    updatedDestination = destination;
                                }
                                updatedOrigin = new Vector2(0, 0);
                            }
                            else
                            {
                                //Quick fix for Menu Rendering
                                //TODO[LOW]: Figure out a way to allow the origin to be moved and still render correctly on the main menu.
                                updatedOrigin = new Vector2(16, 64);
                            }
                        }
                        else if (origin.X == 0 && origin.Y == 20)
                        {
                            //Rest awhile and listen down
                            updatedOrigin = new Vector2(16, 103);
                        }
                        else if (origin.X == -4 && origin.Y == 24)
                        {
                            //Rest awhile and listen right
                            updatedOrigin = new Vector2(9, 111);
                        }
                        else if (origin.X == 4 && origin.Y == 24)
                        {
                            //Rest awhile and listen left
                            updatedOrigin = new Vector2(23, 111);
                        }
                        else if (origin.X == 0 && origin.Y == 22)
                        {
                            //Rest awhile and listen up
                            updatedOrigin = new Vector2(16, 107);
                        }
                    } else if (r.Height == 24) {
                        //Social Tab
                        updatedDestination = new Rectangle(destination.X, destination.Y - 80, (int)(a.HDTextureInfo.SpriteWidth * scale.X), (int)(a.HDTextureInfo.SpriteHeight * scale.Y)); ;
                        updatedSource = new Rectangle?(new Rectangle(0, 0, 64, 110));
                        updatedOrigin = new Vector2(32, 55);
                    }
                    spriteAlreadyDrawn = true;
                    if (a.HDTextureInfo.IsFarmer)
                    {
                        __instance.Draw(a, updatedDestination, updatedSource, color, rotation, updatedOrigin, effects, layerDepth);
                    } else
                    {
                        __instance.Draw(a.NewTexture, updatedDestination, updatedSource, color, rotation, updatedOrigin, effects, layerDepth);
                    }
                    spriteAlreadyDrawn = false;
                    return false;
                }
            }

            return true;
            
        }

        public static bool Draw(SpriteBatch __instance, Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth)
        {
            return DrawReplacedTexture(__instance, texture, destinationRectangle, sourceRectangle, color, origin, Vector2.One, rotation, effects, layerDepth);
        }
        public static bool Draw(SpriteBatch __instance, Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color)
        {
            return DrawReplacedTexture(__instance, texture, destinationRectangle, sourceRectangle, color, Vector2.Zero, Vector2.One);
        }
        public static bool Draw(SpriteBatch __instance, Texture2D texture, Rectangle destinationRectangle, Color color)
        {
            Rectangle sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
            return DrawReplacedTexture(__instance, texture, destinationRectangle, sourceRectangle, color, Vector2.Zero, Vector2.One);
        }
        public static bool Draw(SpriteBatch __instance, Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
        {
            sourceRectangle = sourceRectangle.HasValue ? sourceRectangle.Value : new Rectangle(0, 0, texture.Width, texture.Height);
            return DrawReplacedTexture(__instance, texture, new Rectangle((int)(position.X), (int)(position.Y), (int)(sourceRectangle.Value.Width * scale.X), (int)(sourceRectangle.Value.Height * scale.Y)), sourceRectangle, color, origin, scale, rotation, effects, layerDepth);
        }
        public static bool Draw(SpriteBatch __instance, Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
        {
            sourceRectangle = sourceRectangle.HasValue ? sourceRectangle.Value : new Rectangle(0, 0, texture.Width, texture.Height);
            return DrawReplacedTexture(__instance, texture, new Rectangle((int)(position.X), (int)(position.Y), (int)(sourceRectangle.Value.Width * scale), (int)(sourceRectangle.Value.Height * scale)), sourceRectangle, color, origin, new Vector2(scale), rotation, effects, layerDepth);
        }
        public static bool Draw(SpriteBatch __instance, Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color)
        {
            sourceRectangle = sourceRectangle.HasValue ? sourceRectangle.Value : new Rectangle(0, 0, texture.Width, texture.Height);
            return DrawReplacedTexture(__instance, texture, new Rectangle((int)(position.X), (int)(position.Y), (int)(sourceRectangle.Value.Width), (int)(sourceRectangle.Value.Height)), sourceRectangle, color, Vector2.Zero, Vector2.One);
        }
        public static bool Draw(SpriteBatch __instance, Texture2D texture, Vector2 position, Color color)
        {
            Rectangle sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
            return DrawReplacedTexture(__instance, texture, new Rectangle((int)(position.X), (int)(position.Y), (int)(texture.Width), (int)(texture.Height)), sourceRectangle, color, Vector2.Zero, Vector2.One);

        }
    }
}
