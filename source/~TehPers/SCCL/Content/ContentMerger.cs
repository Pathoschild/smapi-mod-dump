using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TehPers.Stardew.Framework;
using TehPers.Stardew.SCCL.API;
using TehPers.Stardew.SCCL.Configs;
using xTile.Dimensions;

namespace TehPers.Stardew.SCCL.Content {
    public class ContentMerger {
        public HashSet<string> Dirty { get; } = new HashSet<string>();
        private HashSet<Type> Unmergables { get; } = new HashSet<Type>();
        private HashSet<string> Failed { get; } = new HashSet<string>();
        private Dictionary<string, object> Assets { get; } = new Dictionary<string, object>();
        private Dictionary<string, object> Originals { get; } = new Dictionary<string, object>();
        internal Dictionary<string, Size> RequiredSize { get; } = new Dictionary<string, Size>();

        internal ContentMerger() {
#pragma warning disable
            ContentEvents.AssetLoading += this.AssetLoading;
#pragma warning restore
        }

        public void AssetLoading(object sender, IContentEventHelper e) {
            try {
                Originals[e.AssetName] = e.Data;
                Dirty.Remove(e.AssetName);
                if (this.Merge(e.AssetName, e.Data)) {
                    e.ReplaceWith(Assets[e.AssetName]);
                } else {
                    Assets[e.AssetName] = e.Data;
                }
            } catch (Exception ex) {
                ex.ToString();
            }
        }

        public void RefreshAssets() {
            // Copies the Dirty hash set with ToHashSet so that if another thread changes Dirty, it doesn't mess up this loop
                foreach (string assetName in Dirty) {
                    // If this asset hasn't failed to refresh and it's been loaded and it successfully merges (will attempt to load only if the other two are true)
                    if (!Failed.Contains(assetName) && Assets.ContainsKey(assetName) && !this.Merge(assetName, Originals.GetDefault(assetName, null))) {
                        ModEntry.INSTANCE.Monitor.Log($"Failed to merge {assetName}. Will not try to merge it again.", LogLevel.Warn);
                        Failed.Add(assetName);
                    }
                }
                Dirty.Clear();
        }

        public IEnumerable<KeyValuePair<string, T>> GetModAssets<T>(string assetName) {
            // Update load order with any missing mods
            ModConfig config = ModEntry.INSTANCE.config;
            config.LoadOrder.AddRange(
                from modKV in ContentAPI.mods
                let mod = modKV.Key
                where !config.LoadOrder.Contains(mod)
                select mod
                );
            ModEntry.INSTANCE.Helper.WriteConfig(config);

            return (
                from injector in ContentAPI.mods.Values
                where injector.Enabled
                orderby config.LoadOrder.IndexOf(injector.Name)
                where injector.ModContent.ContainsKey(assetName)
                from asset in injector.ModContent[assetName]
                where asset is T
                select new KeyValuePair<string, T>(injector.Name, (T) asset)
                );
        }

        #region Mergers
        public bool Merge(string assetName, object orig) {
            try {
                ModConfig config = ModEntry.INSTANCE.config;
                Type t = null;
                if (orig != null) {
                    t = orig.GetType();
                } else {
                    IEnumerable<KeyValuePair<string, object>> modAssets = GetModAssets<object>(assetName);

                    if (modAssets.Any()) t = modAssets.First().Value.GetType();

                    if (t == null || modAssets.Any(asset => asset.Value.GetType() != t)) {
                        ModEntry.INSTANCE.Monitor.Log($"Failed to predict type of {assetName}. This is probably due to conflicting mods.", LogLevel.Error);
                        ModEntry.INSTANCE.Monitor.Log($"Related injectors: {String.Join(", ", modAssets.Select(e => e.Key).ToHashSet())}", LogLevel.Error);
                        return false;
                    }
                }

                if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Dictionary<,>)) {
                    this.GetType().GetMethod("MergeDictionary", BindingFlags.NonPublic | BindingFlags.Instance)
                        .MakeGenericMethod(t.GetGenericArguments())
                        .Invoke(this, new object[] { orig, assetName });
                } else if (orig is Texture2D texture) {
                    if (texture == null || texture.Format == SurfaceFormat.Color)
                        this.MergeTextures<Color>(texture, assetName);
                    else {
                        ModEntry.INSTANCE.Monitor.Log($"Cannot merge this texture format, overriding instead: {Enum.GetName(typeof(SurfaceFormat), texture.Format)}", LogLevel.Info);
                        this.ReplaceIfExists(orig, assetName);
                    }
                } else {
                    if (!Unmergables.Contains(t)) {
                        ModEntry.INSTANCE.Monitor.Log($"Cannot merge this type, overriding instead: {t.ToString()}", LogLevel.Trace);
                        Unmergables.Add(t);
                    }
                    this.ReplaceIfExists(orig, assetName);
                }
                return true;
            } catch (Exception ex) {
                ModEntry.INSTANCE.Monitor.Log("An error occurred while merging " + assetName, LogLevel.Error);
                ModEntry.INSTANCE.Monitor.Log(ex.Message, LogLevel.Error);
            }
            return false;
        }

        private void MergeDictionary<TKey, TVal>(Dictionary<TKey, TVal> orig, string assetName) {
            Dictionary<TKey, TVal> final = orig != null ? new Dictionary<TKey, TVal>(orig) : new Dictionary<TKey, TVal>();
            Dictionary<TKey, TVal> diffs = new Dictionary<TKey, TVal>();
            Dictionary<TKey, string> diffMods = new Dictionary<TKey, string>();

            bool collision = false;
            foreach (KeyValuePair<string, Dictionary<TKey, TVal>> modKV in GetModAssets<Dictionary<TKey, TVal>>(assetName)) {
                bool warned = false;
                foreach (KeyValuePair<TKey, TVal> injection in modKV.Value) {
                    TVal val = injection.Value;
                    if (typeof(TVal) == typeof(string) && final.ContainsKey(injection.Key)) {
                        if ((final[injection.Key] as string).Count(c => c == '/') != (injection.Value as string).Count(c => c == '/')) {
                            if (!warned) {
                                ModEntry.INSTANCE.Monitor.Log($"{modKV.Key} might be loading an outdated asset: {assetName}", LogLevel.Warn);
                                ModEntry.INSTANCE.Monitor.Log("If the game crashes or black-screens, try disabling this mod first.", LogLevel.Warn);
                            }

                            string entry = val as string;
                            if (TryPort(ref entry, assetName)) {
                                val = (TVal) (object) entry;
                                if (!warned) ModEntry.INSTANCE.Monitor.Log("Attempted to forward-port the mod.", LogLevel.Warn);
                            }

                            warned = true;
                        }
                    }

                    if (!(final.ContainsKey(injection.Key) && final[injection.Key].Equals(val))) {
                        if (!collision && diffs.ContainsKey(injection.Key)) {
                            ModEntry.INSTANCE.Monitor.Log($"Collision detected between {diffMods[injection.Key]} and {modKV.Key}! Overwriting...", LogLevel.Warn);
                            collision = true;
                        }

                        diffs[injection.Key] = val;
                        diffMods[injection.Key] = modKV.Key;
                    }
                }
            }

            foreach (KeyValuePair<TKey, TVal> diff in diffs)
                final[diff.Key] = diff.Value;

            if (diffs.Count > 0)
                ModEntry.INSTANCE.Monitor.Log($"{string.Join(", ", diffMods.Values.ToHashSet())} injected {diffs.Count} changes into {assetName}.xnb", LogLevel.Info);

            if (Assets.ContainsKey(assetName)) {
                if (Assets[assetName] is Dictionary<TKey, TVal> asset) {
                    asset.Clear();
                    foreach (KeyValuePair<TKey, TVal> entry in final)
                        asset[entry.Key] = entry.Value;
                }
            } else Assets[assetName] = final;
        }

        // TODO: Needs to have texture injection offsets, so mods can inject a texture at (u, v) in the original
        private void MergeTextures<TFormat>(Texture2D orig, string assetName) where TFormat : struct {
            // Calculate size of texture
            Size texSize = orig != null ? new Size(orig.Width, orig.Height) : new Size(0, 0);
            if (RequiredSize.ContainsKey(assetName)) {
                Size reqSize = RequiredSize[assetName];
                texSize = new Size(Math.Max(texSize.Width, reqSize.Width), Math.Max(texSize.Height, reqSize.Height));
            }

            // Create data array, and fill it with original texture's data if possible
            TFormat[] sizedOrigData = new TFormat[texSize.Area];
            if (orig != null) {
                TFormat[] origData = new TFormat[orig.Width * orig.Height];
                orig.GetData(origData);
                for (int y = 0; y < orig.Height; y++)
                    for (int x = 0; x < orig.Width; x++)
                        sizedOrigData[y * texSize.Width + x] = origData[y * orig.Width + x];
            }
            TFormat[] diffData = new TFormat[sizedOrigData.Length];
            sizedOrigData.CopyTo(diffData, 0);

            Dictionary<int, string> diffMods = new Dictionary<int, string>();
            foreach (KeyValuePair<string, OffsetTexture2D> modKV in GetModAssets<OffsetTexture2D>(assetName)) {
                string mod = modKV.Key;
                OffsetTexture2D offsetTexture = modKV.Value;
                Texture2D modTexture = offsetTexture.Texture;
                bool collision = false;
                TFormat[] modData = new TFormat[modTexture.Width * modTexture.Height];
                Size modSize = new Size(modTexture.Width, modTexture.Height);
                modTexture.GetData(modData);

                if (modSize != texSize)
                    ModEntry.INSTANCE.Monitor.Log($"Mod's texture is too large for the texture, so it's being trimmed: {mod}", LogLevel.Warn);

                for (int y = offsetTexture.Offset.Y; y < texSize.Height; y++) {
                    int modY = y - offsetTexture.Offset.Y;
                    if (modY >= modSize.Height) break;
                    for (int x = offsetTexture.Offset.X; x < texSize.Width; x++) {
                        int modX = x - offsetTexture.Offset.X;
                        if (modX >= modSize.Width) continue;

                        // Index of the pixel in the original texture, aka without offset
                        int i = y * texSize.Width + x; // Use the original's index because we're keeping that width for the final

                        TFormat pixel = default(TFormat);
                        try {
                            // Get the pixel with the offset
                            pixel = modData[modY * modSize.Width + modX];
                        } catch (Exception ex) {
                            ex.ToString();
                        }

                        if (i >= sizedOrigData.Length || !sizedOrigData[i].Equals(pixel)) {
                            if (!collision && diffMods.ContainsKey(i)) {
                                ModEntry.INSTANCE.Monitor.Log($"Collision detected between {mod} and {diffMods[i]}! Overwriting...", LogLevel.Warn);
                                collision = true;
                            }
                            diffData[i] = pixel;
                            diffMods[i] = mod;
                        }
                    }
                }
            }

            if (diffMods.Count > 0)
                ModEntry.INSTANCE.Monitor.Log($"{string.Join(", ", diffMods.Values.ToHashSet())} injected changes into {assetName}.xnb", LogLevel.Info);

            Texture2D merged = new Texture2D(Game1.graphics.GraphicsDevice, texSize.Width, (int) Math.Ceiling((double) diffData.Length / texSize.Width));
            merged.SetData(diffData.ToArray());

            if (Assets.ContainsKey(assetName)) {
                // Replace the data in the existing asset with the data in the merged asset
                Texture2D asset = Assets[assetName] as Texture2D;
                TFormat[] assetData = new TFormat[asset.Width * asset.Height];
                asset.GetData(assetData);
                if (asset != null && asset.Format == merged.Format) {
                    for (int y = 0; y < asset.Height; y++) {
                        if (y >= texSize.Height) break;
                        for (int x = 0; x < asset.Width; x++) {
                            if (x >= texSize.Width) break;
                            assetData[x + y * asset.Width] = diffData[x + y * texSize.Width];
                        }
                    }
                }
                asset.SetData(assetData);
            } else {
                // Use the merged asset
                Assets[assetName] = merged;
            }
        }

        private void ReplaceIfExists<T>(T orig, string assetName) {
            ModEntry.INSTANCE.Monitor.Log($"ReplaceIfExists<{typeof(T).Name}> {assetName}", LogLevel.Trace);
            if (Assets.ContainsKey(assetName)) {
                ModEntry.INSTANCE.Monitor.Log($"Could not overwrite {assetName} ({typeof(T).Name})", LogLevel.Warn);
                return;
            }

            T replaced = orig;
            string diffMod = null;

            foreach (KeyValuePair<string, T> modKV in GetModAssets<T>(assetName)) {
                if (diffMod != null)
                    ModEntry.INSTANCE.Monitor.Log($"Collision detected between {diffMod} and {modKV.Key}! Overwriting...", LogLevel.Warn);

                replaced = modKV.Value;
                diffMod = modKV.Key;
            }

            if (diffMod != null)
                ModEntry.INSTANCE.Monitor.Log($"{diffMod} replaced {assetName}.xnb", LogLevel.Info);

            Assets[assetName] = replaced;
        }
        #endregion

        #region Forward Porting
        /// <summary>Ports a xnb ObjectInformation.xnb entry from 1.11 to 1.2+</summary>
        private bool TryPort(ref string asset, string assetName) {
            switch (assetName) {
                case "Data\\ObjectInformation":
                    asset = TryPortObjectInformation(asset);
                    return true;
            }
            return false;
        }

        private string TryPortObjectInformation(string asset) {
            List<string> data = asset.Split('/').ToList();
            data.Insert(4, data[0]);
            return string.Join("/", data);
        }
        #endregion
    }
}
