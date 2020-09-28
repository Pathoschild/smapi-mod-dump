using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System;
using System.Collections.Generic;

namespace SunscreenMod
{
    class SkinColors : IAssetEditor
    {
        /*********
        ** Accessors
        *********/
        static IMonitor Monitor => ModEntry.Instance.Monitor;
        static ModConfig Config => ModConfig.Instance;

        Sunburn Burn => ModEntry.Instance.Burn;


        /*********
        ** Fields
        *********/
        /// <summary>List of color arrays each representing the 3 skin color values used for a level of sunburn.</summary>
        readonly List<Color[]> SunburnColors = new List<Color[]>()
        {
            new Color[] { new Color(63, 3, 3), new Color(175, 22, 22), new Color(255, 122, 122) }, //Level 1 burn colors
            new Color[] { new Color(53, 2, 2), new Color(150, 19, 19), new Color(230, 97, 97)   }, //Level 2 burn colors
            new Color[] { new Color(43, 1, 1), new Color(125, 16, 16), new Color(205, 72, 72)   }  //Level 3 burn colors
        };


        /*********
        ** Helper methods
        *********/
        /// <summary>Gets the appropriate skin color value for a given sunburn level. In single player this is always the same, but it changes by sunburn level in multiplayer.</summary>
        /// <param name="sunburnLevel">The level of sunburn to obtain a skin color value for.</param>
        /// <returns>Integer representing a skin color value in the game.</returns>
        public static int GetSunburntSkinIndex(int sunburnLevel)
        {
            if (Context.IsMultiplayer)
            {
                int level = Math.Min(sunburnLevel, 3);
                return Config.BurnSkinColorIndex[level - 1];
            }
            else
            {
                return Config.BurnSkinColorIndex[0];
            }
        }

        /// <summary>Overwrite the color data for 3 pixels in a given row of an image.</summary>
        /// <param name="image">The image to modify the pixels of</param>
        /// <param name="row">The image row (indexed at 1) to edit</param>
        /// <param name="pixels">The new color values to use</param>
        private void SetPixelsInRow(Texture2D image, int row, Color[] pixels) //NOT zero-indexed!
        {
            Rectangle rect = new Rectangle(0, row - 1, 3, 1);
            image.SetData(0, rect, pixels, 0, pixels.Length);
        }


        /*********
        ** Public methods
        *********/
        /// <summary>Get whether this instance can edit the given asset.</summary>
        /// <typeparam name="_T">The asset Type</typeparam>
        /// <param name="asset">Basic metadata about the asset being loaded</param>
        /// <returns>Return true for asset Characters\Farmer\skinColors, false otherwise</returns>
        public bool CanEdit<_T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals($"Characters\\Farmer\\skinColors") &&
                    Config.SkinColorChange &&
                    ModEntry.Instance.IsSaveReady;
        }

        /// <summary>Edit the pixels of the Characters\Farmer\skinColors image to use as sunburned render values.</summary>
        /// <typeparam name="_T">The asset Type</typeparam>
        /// <param name="asset">A helper which encapsulates metadata about an asset and enables changes to it</param>
        public void Edit<_T>(IAssetData asset)
        {
            var editor = asset.AsImage();
            int editcount = 0;
            //Carefully patch specific pixels of the skinColors asset
            if (Context.IsMultiplayer)
            {
                for (int i = 0; 1 < 3; i++)
                {
                    SetPixelsInRow(editor.Data, Config.BurnSkinColorIndex[i], SunburnColors[i]);
                    editcount++;
                }
            }
            else
            {
                if (Burn.IsSunburnt())
                {
                    int level = Burn.SunburnLevel;
                    SetPixelsInRow(editor.Data, Config.BurnSkinColorIndex[0], SunburnColors[level - 1]);
                    editcount++;
                }
            }
            Monitor.Log($"Made {editcount} edits to Characters\\Farmer\\skinColors.", Config.DebugMode ? LogLevel.Info : LogLevel.Trace);
        }
    }
}