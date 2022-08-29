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
using Microsoft.Xna.Framework;

namespace DynamicBodies.Framework
{
    public class ColourCacher
    {
		Dictionary<string, Dictionary<ColourReplacements, List<int>>> recolorOffsets;//structured like layer[colourtoreplace[pixelstochange]]
		Dictionary<ColourReplacements, Color> coloursToReplace;
		public enum ColourReplacements : ushort
		{
			ShirtD = 256,
			ShirtM = 257,
			ShirtL = 258,
			SkinD = 260,
			SkinM = 261,
			SkinL = 262,
			SkinMD = 263,//extended
			SkinML = 264,//extended
			SkinAltD = 265,//extended
			SkinAltM = 266,//extended
			SkinAltL = 267,//extended
			ShoesD = 268,
			ShoesMD = 269,
			ShoesML = 270,
			ShoesL = 271,
			ScleraD = 274,//added
			ScleraL = 275,//added
			IrisLeftD = 276,
			IrisLeftL = 277,
			Pupil = 278,//added
			IrisRightD = 279,//extended
			IrisRightL = 280,//extended
		}

		public ColourCacher()
        {
			recolorOffsets = new Dictionary<string, Dictionary<ColourReplacements, List<int>>>();
        }

		public bool hasColoursToReplace()
        {
			return coloursToReplace != null;
        }
		public void SetColoursToReplace(Dictionary<ColourReplacements,Color> replacingPairs)
        {
			coloursToReplace = replacingPairs;
            if (ModEntry.Config.debugmsgs)
            {
				ModEntry.debugmsg("Colours to replace...", StardewModdingAPI.LogLevel.Debug);
				foreach(ColourReplacements replacement in replacingPairs.Keys)
                {
					ModEntry.debugmsg($"   [{(int)replacement}] - {replacingPairs[replacement].ToString()}", StardewModdingAPI.LogLevel.Debug);
				}
			}
        }

		public void GeneratePixelIndices(string layer_name, List<ColourReplacements> toReplace, Color[] pixels)
		{
			//Start a new layer if needed
            if (!recolorOffsets.ContainsKey(layer_name))
            {
				recolorOffsets[layer_name] = new Dictionary<ColourReplacements, List<int>>();
            }

			//Reset the layer cache for these colours
			if (toReplace.Count > 0)
			{
				foreach (ColourReplacements replacement in toReplace)
				{
					recolorOffsets[layer_name][replacement] = new List<int>();
				}

				for (int i = 0; i < pixels.Length; i++)
				{
					//Look through the list of colours
					foreach (ColourReplacements replacement in toReplace)
					{
						if (pixels[i].Equals(coloursToReplace[replacement]))
						{
							//Add the pixel to the layer cache
							recolorOffsets[layer_name][replacement].Add(i);
						}
					}
				}
			}
		}
		public void ClearLayer(string layer_name)
        {
			if (recolorOffsets.ContainsKey(layer_name))
			{
				recolorOffsets[layer_name].Clear();
			}
        }

		public void Clear()
		{
			recolorOffsets.Clear();
		}
		public void RecolorLayer(ref Color[] pixels, string layer_name, Dictionary<ColourReplacements,Color> new_colours)
		{
			if (recolorOffsets.ContainsKey(layer_name))
			{
				foreach (ColourReplacements replacement in new_colours.Keys)
				{
					if (recolorOffsets[layer_name].ContainsKey(replacement))
					{
						for (int i = 0; i < recolorOffsets[layer_name][replacement].Count; i++)
						{
							pixels[recolorOffsets[layer_name][replacement][i]] = new_colours[replacement];
						}
					}
				}
			}
		}

	}
}
