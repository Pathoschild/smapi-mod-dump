/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ribeena/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DynamicBodies.UI;
using StardewValley;

namespace DynamicBodies.Framework
{
    internal class ShoesPalette
    {
        static private Dictionary<string, Color[]> palettes = new Dictionary<string, Color[]>();

        static public void LoadPalettes()
        {
            LoadPalettes(Game1.content.Load<Texture2D>("Characters\\Farmer\\shoeColors"));
        }
        static public void LoadPalettes(Texture2D defaultImage)
        {
            Color[] imagedata = new Color[defaultImage.Width*defaultImage.Height];
            defaultImage.GetData(imagedata);
            for (int h = 0; h < defaultImage.Height; h++)
            {
                if (!palettes.ContainsKey("" + h))
                {
                    if (imagedata[h * 4].A == 255)
                    {
                        palettes.Add("" + h, new Color[] { imagedata[h * 4], imagedata[h * 4 + 1], imagedata[h * 4 + 2], imagedata[h * 4 + 3] });
                    }
                }
            }

            if (palettes.ContainsKey("12"))
            {
                palettes.Remove("12"); //Remove default naked color
            }
        }

        static public void LoadPalette(IRawTextureData defaultImage, int offset, string id)
        {
            Color[] imagedata = defaultImage.Data;
            palettes.Add(id, new Color[] { imagedata[offset], imagedata[offset+1], imagedata[offset+2], imagedata[offset+3] });
        }

        static public Color[] GetColors(string id)
        {
            if (!palettes.ContainsKey(id))
            {
                ModEntry.debugmsg($"ID wasn't present... reloading texture", LogLevel.Debug);
                LoadPalettes();
                if (!palettes.ContainsKey(id))
                {
                    ModEntry.debugmsg($"ID still not present... defaulting to 2", LogLevel.Debug);
                    return palettes["2"];
                }
            }
            return palettes[id];
        }

        static public string First()
        {
            return palettes.Keys.First();
        }

        static public string Next(string id)
        {
            string[] keys = palettes.Keys.ToArray();
            //find element
            int index = 0;
            for(int i = 0; i < keys.Length; i++)
            {
                if (keys[i] == id)
                {
                    index = i;
                    break;
                }
            }
            index++;//next element
            if(index == keys.Length) index = 0;//wrap

            return keys[index];
        }

        static public string FindClosestColor(Color match, int index)
        {
            string currentIndex = "";
            int similarity = int.MaxValue;
            //find a similar colour
            foreach (KeyValuePair<string, Color[]> kvp in palettes)
            {
                int difference = Math.Abs(match.R - kvp.Value[index].R) + Math.Abs(match.G - kvp.Value[index].G) + Math.Abs(match.B - kvp.Value[index].B);
                if (difference < similarity)
                {
                    similarity = difference;
                    currentIndex = kvp.Key;
                }
            }
            return currentIndex;
        }
    }
}
