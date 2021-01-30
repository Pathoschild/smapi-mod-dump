/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/GStefanowich/SDV-NFFTT
**
*************************************************/

/*
 * This software is licensed under the MIT License
 * https://github.com/GStefanowich/SDV-NFFTT
 *
 * Copyright (c) 2019 Gregory Stefanowich
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System.Collections.Concurrent;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;

namespace NotFarFromTheTree {
    public static class Assets {
        private static readonly ConcurrentDictionary<string, Texture2D> ASSETS = new ConcurrentDictionary<string, Texture2D>();
        private const string MOD_PREFIX = "NFFTT";
        
        public static bool Has( string key ) => Assets.ASSETS.ContainsKey(key);
        
        public static Texture2D Get( string key ) {
            while (true) {
                if (Assets.ASSETS.TryGetValue(key, out Texture2D texture) && texture != null)
                    return texture;
                else if (!Assets.LoadSprite(key, Assets.UnWrap(key)))
                    return null;
            }
        }
        
        public static bool SpriteExists( string path ) => File.Exists(Assets.ModLocal(path));
        
        public static bool LoadSprite( string key, string path ) {
            if (!Assets.SpriteExists($"{path}.png")) {
                ModEntry.MONITOR.Log($"Could not find file asset: \"{path}.png\"", LogLevel.Error);
            } else {
                // If the value is not already in the Dictionary
                if (!Assets.ASSETS.TryGetValue(key, out Texture2D assets) || assets == null) {
                    // Load the texture
                    assets = ModEntry.MOD_HELPER.Content.Load<Texture2D>(Assets.ModAsset($"{path}.png"), ContentSource.ModFolder);
                    
                    // Store the texture for future loading
                    Assets.ASSETS[key] = assets;
                }
                return true;
            }
            return false;
        }
        
        public static string WRAP => $"{Assets.MOD_PREFIX}{Path.DirectorySeparatorChar}";
        
        public static string Wrap( string path ) => $"{Assets.WRAP}{path}";
        
        public static string UnWrap( string path ) {
            return Assets.IsWrapped(path) ? path.Substring(Assets.WRAP.Length) : path;
        }
        
        public static bool IsWrapped( string path ) => path.StartsWith(Assets.WRAP);
        
        public static string ModLocal( string file ) => Path.Combine(ModEntry.MOD_HELPER.DirectoryPath, Assets.ModAsset(file));
        
        public static string ModAsset( string asset ) => Path.Combine("assets", asset);
    }
}