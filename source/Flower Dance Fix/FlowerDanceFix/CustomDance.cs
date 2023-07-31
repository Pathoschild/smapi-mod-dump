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
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.Network;
using StardewValley.Objects;
using StardewModdingAPI;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;

namespace FlowerDanceFix
{
    class CustomDance
    {
        public static IMonitor Monitor;
        public static IModHelper Helper;
        public static ModConfig Config;

        public static void Initialize(IMonitor monitor, ModConfig config, IModHelper helper)
        {
            Monitor = monitor;
            Config = config;
            Helper = helper;
        }
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

        public static string BuildEventWarpBlock(List<NetDancePartner> upperLineWarp)
        {
        
            /* There will eventually be some code here straightening out issues between custom spectator animations
             * that involve NPCs you can dance with
             * it'll be messy, ugh I'm not looking forward to it
             */
            
            int n = upperLineWarp.Count;
            int q = n % 2;

            StringBuilder eventWarpDancer = new StringBuilder();

            //I gave up on using math to get x coordinate, so here's some arrays instead lmao

            switch (q)
            {
                case 0:

                    int counti = 1;

                    int[] even = {0,13,15,11,17,9,19,7,21,6,22,8,20,10,18,12,16};

                    while (counti <= 16 && counti <= n)
                    {
                        eventWarpDancer.Append($"/warp Girl{counti} {even[counti]} 24");
                        eventWarpDancer.Append($"/warp Guy{counti} {even[counti]} 27");
                        counti++;
                    }
                    
                    break;

                case 1:

                    int countj = 1;

                    int[] odd = {0,14,16,12,18,10,20,8,22,6,21,7,19,9,17,11,15,13};

                    while (countj <= 17 && countj <= n)
                    {
                        eventWarpDancer.Append($"/warp Girl{countj} {odd[countj]} 24");
                        eventWarpDancer.Append($"/warp Guy{countj} {odd[countj]} 27");
                        countj++;
                    }
                    break;
            }
            return eventWarpDancer.ToString();
        }

        public static string BuildShowFrameBlock(List<NetDancePartner> upperLineShowFrame)
        {

            int n = upperLineShowFrame.Count();
            int count = 1;

            StringBuilder eventShowFrame = new StringBuilder();

            while (count <= n)
            {
                eventShowFrame.Append($"/showFrame Girl{count} 40");
                eventShowFrame.Append($"/showFrame Guy{count} 44");
                count++;
            }

            return eventShowFrame.ToString();
        }

        public static string BuildAnimateBlock1(List<NetDancePartner> upperLineAnimate1)
        {

            int n = upperLineAnimate1.Count();
            int count = 1;

            StringBuilder eventAnimate1 = new StringBuilder();

            while (count <= n)
            {
                eventAnimate1.Append($"/animate Girl{count} false true 600 43 41 43 42");
                eventAnimate1.Append($"/animate Guy{count} false true 600 44 45");

                count++;
            }

            return eventAnimate1.ToString();
        }

        public static string BuildAnimateBlock2(List<NetDancePartner> upperLineAnimate2)
        { 
            int n = upperLineAnimate2.Count();
            int count = 1;

            StringBuilder eventAnimate2 = new StringBuilder();

            while (count <= n)
            {
                eventAnimate2.Append($"/animate Girl{count} false true 600 46 47");
                eventAnimate2.Append($"/animate Guy{count} false true 600 46 47");

                count++;
            }

            return eventAnimate2.ToString();
        }

        public static string BuildAnimateBlock3(List<NetDancePartner> upperLineAnimate3)
        {
            int n = upperLineAnimate3.Count();
            int count = 1;

            StringBuilder eventAnimate3 = new StringBuilder();

            while (count <= n)
            {
                eventAnimate3.Append($"/animate Girl{count} false true 600 43 41 43 42");
                eventAnimate3.Append($"/animate Guy{count} false true 600 44 45");

                count++;
            }

            return eventAnimate3.ToString();
        }

        public static string BuildStopAnimationBlock(List<NetDancePartner> upperLineAnimateStop)
        {
            int n = upperLineAnimateStop.Count();
            int count = 1;

            StringBuilder eventStopAnimation = new StringBuilder();

            while (count <= n)
            {
                eventStopAnimation.Append($"/stopAnimation Girl{count} 40");
                eventStopAnimation.Append($"/stopAnimation Guy{count} 44");
                count++;
            }

            return eventStopAnimation.ToString();
        }

        public static string BuildOffsetBlock(List<NetDancePartner> upperLineOffset)
        {
            int n = upperLineOffset.Count();
            int count = 1;

            StringBuilder eventOffset = new StringBuilder();

            while (count <= n)
            {
                eventOffset.Append($"/positionOffset Guy{count} 0 -2");
                count++;
            }

            return eventOffset.ToString();
        }

        public static string BuildGiantOffsetBlock(List<NetDancePartner> upperLineOffsetGiant)
        {
            string offsetBlock = BuildOffsetBlock(upperLineOffsetGiant);
            
            StringBuilder eventOffsetGiant = new StringBuilder();
            for (int z = 0; z < 28; z++)
            {
                eventOffsetGiant.Append(offsetBlock);
                eventOffsetGiant.Append("/pause 300");
            }
            eventOffsetGiant.Append(offsetBlock);

            return eventOffsetGiant.ToString();
        }

        //public static string BuildFDFSpriteChangeBlock()
        //{
            
        //}

        //Example Code by PathosChild
        /*
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

