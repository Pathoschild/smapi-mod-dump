/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/elfuun1/FlowerDanceFix
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.Network;
using StardewValley.Objects;
using StardewModdingAPI;
using Harmony;
using Microsoft.Xna.Framework.Graphics;

namespace FlowerDanceFix
{
    class CustomDance
    {
       /*
        public static bool HasFDFSprites(NPC character)
        {
            try
            {
                Game1.temporaryContent.Load<Texture2D>("Characters\\" + NPC.getTextureNameForCharacter(character.name.Value) + "_FDF");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string BuildEventWarp(List<NetDancePartner> females)
        {
            int n = females.Count;
            int q = n % 2;

            StringBuilder eventWarp = new StringBuilder();

            switch (q)
            {
                case 0:
                    int e = 13;
                    int count = 1;
                    int osc = 0;
                    while (count < n && count < 8)
                    {
                        eventWarp.Append("/warp Girl" + count + " " + (e + (2 * Math.Pow(-1, osc))));
                        eventWarp.Append("/warp Guy" + count + " " + (e + (((osc + 2) % 2) * 2 * Math.Pow(-1, osc))));
                        count += ((osc + 2) % 2);
                        osc++;

                    }
                    
                    
                    
                    
                    return "even";
                case 1:
                    int odd = 14; 
                    return "odd";
                default:
                    return "this isn't a thing that should happen, excuse me???";
            }
        }

        public static string xCoordinateBuilder(int start, int max)
        {

        }

        //Example Code by PathosChild
        public bool CanLoad(IAssetInfo asset)
        {
            return this.GetNpcSprite(asset) != null;
        }

        public T Load<T>(IAssetInfo asset)
        {
            return this.GetNpcSprite(asset);
        }

        private Texture2D GetNpcSprite(IAssetInfo asset)
        {
            var segments = PathUtilities.GetSegments(asset.AssetName);

            bool isNpcSprite =
               typeof(T) == typeof(Texture2D)
               && segments.Length == 2
               && string.Equals(segments[0], "Characters", StringComparison.OrdinalIgnoreCase);
            if (!isNpcSprite)
                return null;

            FileInfo file = new FileInfo(Path.Combine(this.Helper.DirectoryPath, "assets", $"{segments[1]}.png"));
            return file.Exists
                ? this.Helper.Content.Load<Texture2D>($"assets/{file.Name}")
                : null;
        }
        */
    }
}

