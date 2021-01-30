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

using StardewModdingAPI;
using StardewValley.Characters;
using Harmony;

namespace NotFarFromTheTree {
    public class ModEntry : Mod, IAssetLoader {
        public static IMonitor MONITOR;
        public static string MOD_ID;
        public static IModHelper MOD_HELPER;
        
        /*
         * Mod Initializer
         */
        public override void Entry(IModHelper helper) {
            // Initialize the Harmony Override Console
            ModEntry.MOD_ID = this.ModManifest.UniqueID;
            ModEntry.MOD_HELPER = this.Helper;
            ModEntry.MONITOR = this.Monitor;
            
            // Load Harmony (For Patching)
            HarmonyInstance harmony = HarmonyInstance.Create(ModEntry.MOD_ID);
            
            // Patch the method in Harmony
            harmony.Patch(
                AccessTools.Method(typeof(Child), nameof(Child.reloadSprite)),
                new HarmonyMethod(typeof(ChildOverride), nameof(ChildOverride.reloadSprite))
            );
            
            // When receiving messages from other mods
            helper.Events.Multiplayer.ModMessageReceived += ModEvents.OnMessageNotification;
            
            // When exiting a game, dump the Cached values
            helper.Events.GameLoop.ReturnedToTitle += (sender, args) => ChildDat.CHILDREN_PARENT.Clear();
            
            // Register command
            helper.ConsoleCommands.Add(
                "genetics",
                "Change the resemblance of one of your children to an NPC.\n\nUsage: child_parent <Child> <Parent>\n- Child: The name of your child.\n- Parent: An NPC from the town (IE; \"Harvey\", \"Emily\", \"Haley\"...).\n",
                ModEvents.CommandGeneticsBase
            );
        }
        
        /*
         * Asset Handling Overrides
         */
        bool IAssetLoader.CanLoad<T>(IAssetInfo asset) => Assets.Has(asset.AssetName) || Assets.IsWrapped(asset.AssetName);
        T IAssetLoader.Load<T>(IAssetInfo asset) => (T)(object)Assets.Get(asset.AssetName);
    }
}
