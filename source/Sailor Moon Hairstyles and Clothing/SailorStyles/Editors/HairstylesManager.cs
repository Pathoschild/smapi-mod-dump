/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/SailorStyles
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SailorStyles.Editors
{
	internal static class HairstylesManager
	{
        internal static bool TryLoad(AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo(ModConsts.GameContentHairstyleImagePath))
            {
                e.LoadFromModFile<Texture2D>(ModConsts.LocalHairstylesSpritesPath + ".png", AssetLoadPriority.Exclusive);
                return true;
            }
            return false;
        }

        internal static bool TryEdit(AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo(ModConsts.GameContentHairstyleDataPath))
            {
                e.Edit(Edit, AssetEditPriority.Late);
                return true;
            }
            return false;
        }

		internal static void Edit(IAssetData asset)
		{
			const int styleW = 16;
			const int styleH = 128;

			// Dictionary data
			var data = asset.AsDictionary<int, string>().Data;
			var newData = new Dictionary<int, string>();

			// Image data
			Texture2D hairstylesTexture = Game1.content.Load
				<Texture2D>
				(ModConsts.GameContentHairstyleImagePath);
			int count = 0;
            Color[] pixelData = GC.AllocateUninitializedArray<Color>(styleW * styleH);

			// Find index of first non-empty slot in custom hairstyles spritesheet
			for (int x = (hairstylesTexture.Width / styleW) - 1; x >= 0 && count == 0; --x)
			{
				for (int y = (hairstylesTexture.Height / styleH) - 1; y >= 0 && count == 0; --y)
				{
					int cur = x + (hairstylesTexture.Width / styleW * y);
					hairstylesTexture.GetData(0, new Rectangle(x * styleW, y * styleH, styleW, styleH), pixelData, 0, pixelData.Length);
					if (!pixelData.All(colour => colour.A == 0))
					{
						count = cur + 1;
					}
				}
			}
			Log.D($"Hairstyles to add: {count}",
				ModEntry.Config.DebugMode);

			// Add in HairData entries for every hairstyle up until the last non-empty hairstyle in the spritesheet
			const bool hasLeftStyle = true;
			const bool isBaldStyle = false;
			const int coveredStyleId = -1;
			int startId = int.Parse(ModEntry.Instance.ModManifest.UpdateKeys.First().Split(':')[1]) * 100;
			if (data.Keys.Count(key => key >= startId && key < startId + count) is int conflicts && conflicts > 0)
			{
				Log.W($"Identified ID conflicts with {conflicts} hairstyles: Please share a log file on the mod page!");
			}
			for (int i = 0; i < count; ++i)
			{
				int styleId = startId + i;
				if (data.ContainsKey(styleId))
				{
					string[] style = data[styleId].Split('/');
					Log.W($"Overriding hairstyle at {styleId} ({style[0]} at X:{style[1]}, Y:{style[2]})");
				}

				int x = i % 8;
				int y = (int)(i * 0.125) * 8;
				newData.Add(styleId, $"{ModConsts.HairstylesSheetId}/{x}/{y}/{hasLeftStyle}/{coveredStyleId}/{isBaldStyle}");
			}

            if (ModEntry.Config.DebugMode)
                Log.D($"HairData: {string.Join('\n', newData.Select((kvp) => $"{kvp.Key}: {kvp.Value}"))}");

            foreach (var (key, value) in newData)
                data.Add(key, value);
		}
	}
}