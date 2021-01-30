/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/gzhynko/StardewMods
**
*************************************************/

using System.Collections.Generic;
using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace GrapesAllYearRound
{
    /// <summary> The mod entry class loaded by SMAPI. </summary>
    public class ModEntry : Mod, IAssetEditor
    {
        #region Public methods
        
        /// <summary> The mod entry point, called after the mod is first loaded. </summary>
        /// <param name="helper"> Provides simplified APIs for writing mods. </param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.DayStarted += HandleDayStart;
        }

        /// <summary>Get whether this instance can edit the given asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("Data/Crops") || asset.AssetNameEquals("TileSheets/crops");
        }

        /// <summary>Edit crop data to make grapes grow all year round.</summary>
        /// <param name="asset">A helper which encapsulates metadata about an asset and enables changes to it.</param>
        public void Edit<T>(IAssetData asset)
        {
            if (!asset.AssetNameEquals("Data/Crops") && !asset.AssetNameEquals("TileSheets/crops")) return;

            if (asset.AssetNameEquals("TileSheets/crops"))
            {
                if (Game1.currentSeason != "winter") return;

                var editor = asset.AsImage();
                var sourceImage = Helper.Content.Load<Texture2D>("assets/grape_winter.png");
                
                editor.PatchImage(sourceImage, targetArea: new Rectangle(0, 610, 128, 32));

                return;
            }
            
            IDictionary<int, string> data = asset.AsDictionary<int, string>().Data;
            foreach (var itemId in data.Keys)
            {
                if (itemId != 301) continue;
                    
                var fields = data[itemId].Split('/');
                fields[1] = "spring summer fall winter";
                data[itemId] = string.Join("/", fields);
                
                break;
            }
        }
        
        #endregion
        #region Private methods
        
        private void ApplyHarmonyPatches()
        {
            var harmony = HarmonyInstance.Create("GZhynko.GrapesAllYearRound");

            harmony.Patch(
                AccessTools.Method(typeof(Crop), nameof(Crop.newDay)),
                postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(HarmonyPatches.CropNewDay))
            );
        }

        private void HandleDayStart(object sender, DayStartedEventArgs e)
        {
            Helper.Content.InvalidateCache("TileSheets/crops");
        }

        /// <summary> Raised after the game is launched, right before the first update tick. This happens once per game session (unrelated to loading saves). All mods are loaded and initialised at this point, so this is a good time to set up mod integrations. </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            ApplyHarmonyPatches();
        }
        
        #endregion
    }
}
