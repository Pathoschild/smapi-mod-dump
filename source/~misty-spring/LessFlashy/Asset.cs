/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace LessFlashy;

public static class Asset
{
    private static ModConfig Config => ModEntry.Config;

    private const string All = "All";
    private const string Basic = "Basic";
    private const string NoFlash = "NoFlash";
    private const string Minimal = "Minimal";
    internal static IModHelper Helper => ModEntry.Help;
    
    public static void Requested(object? sender, AssetRequestedEventArgs e)
    {
        if (e.NameWithoutLocale.BaseName.StartsWith("Maps/"))
        {
            Maps(e);
        }

        if (e.NameWithoutLocale.BaseName.StartsWith("LooseSprites/"))
        {
            LooseSprites(e);
        }

        if (e.NameWithoutLocale.BaseName.StartsWith("Minigames/"))
        {
            Minigames(e);
        }
        
        if (e.NameWithoutLocale.BaseName.StartsWith("Characters/Monsters/"))
        {
            Monsters(e);
        }
        
        if (e.NameWithoutLocale.BaseName.StartsWith("TileSheets/"))
        {
            Tilesheets(e);
        }
        
        if (e.NameWithoutLocale.BaseName.Equals("Characters/Junimo") && Config.Animations == Minimal)
        {
            e.Edit(asset =>
            {
                var imageAsset = asset.AsImage();
                var img = imageAsset.Data;
                var from = new Rectangle(0,109,16,19);
                var xs = new[] { 16, 32, 48, 64, 80, 96, 112 };
                foreach (var x in xs)
                {
                    imageAsset.PatchImage(img, from, new Rectangle(x,109,16,19));
                }
            }, 
            AssetEditPriority.Late + 10);
        }
        
        if (Config.Animations != Minimal)
            return;
        
        //patch mill to have no animation if Minimal is installed
        if (e.NameWithoutLocale.BaseName.Equals("Buildings/Mill"))
            e.Edit(asset =>
            {
                var imageAsset = asset.AsImage();
                var img = imageAsset.Data;
                var fromArea = new Rectangle(64,0,32, 32);
                var rects = new Rectangle[]
                {
                    new(96, 0,32,32),
                    new(128, 0,32,32),
                    new(160, 0,32,32),
                    new(192, 0,32,32),
                    new(64, 32,32,32),
                    new(96, 32,32,32),
                    new(128, 32,32,32),
                    new(160, 32,32,32),
                    new(192, 32,32,32)
                };
                foreach (var toArea in rects)
                {
                    imageAsset.PatchImage(img, fromArea, toArea);
                }
            }, 
            AssetEditPriority.Late + 10);
    }

    private static void LooseSprites(AssetRequestedEventArgs e)
    {
        
        if (e.NameWithoutLocale.BaseName.Equals("LooseSprites/Cursors") && Config.Animations is Minimal)
        {
            e.Edit(asset =>
            {
                var imageAsset = asset.AsImage();
                var img = imageAsset.Data;
                var from = new Rectangle(368,16,16,16);
                for (var i = 1; i < 8; i++)
                {
                    imageAsset.PatchImage(img, from, new Rectangle(368 + 16 * i, 16, 16, 16));
                }
            }, 
            AssetEditPriority.Late + 10);
        }

        if (e.NameWithoutLocale.BaseName.Equals("LooseSprites/Movies") && Config.Animations is NoFlash or Minimal)
        {
            e.Edit(asset =>
            {
                var imageAsset = asset.AsImage();
                var img = imageAsset.Data;
                var mysterium = new Rectangle(304,128,90,61);
                imageAsset.PatchImage(img, mysterium, new Rectangle(400,128,90,61));

                var howls = new Rectangle(112, 896, 90, 61);
                imageAsset.PatchImage(img, howls, new Rectangle(208,960,90,61));
                imageAsset.PatchImage(img, howls, new Rectangle(304,960,90,61));
                imageAsset.PatchImage(img, howls, new Rectangle(400,960,90,61));
            }, 
            AssetEditPriority.Late + 10);
        }
        
        /*
        if (e.NameWithoutLocale.BaseName.Equals("LooseSprites/Cursors2"))
        {
            e.Edit(asset =>
            {
                var imageAsset = asset.AsImage();
                var img = imageAsset.Data;
                var from = new Rectangle();
                imageAsset.PatchImage(img, from, to);
            }, 
            AssetEditPriority.Late + 10);
        }
        
        if (e.NameWithoutLocale.BaseName.StartsWith("LooseSprites/CraneGame"))
        {
            e.Edit(asset =>
            {
                var imageAsset = asset.AsImage();
                var img = imageAsset.Data;
                var from = new Rectangle();
                imageAsset.PatchImage(img, from, to);
            }, 
            AssetEditPriority.Late + 10);
        }*/

        if (e.NameWithoutLocale.BaseName.Equals("LooseSprites/ForgeMenu") && Config.Forge)
        {
            e.Edit(asset =>
            {
                var imageAsset = asset.AsImage();
                var img = imageAsset.Data;
                var from = new Rectangle(0,80,13,14);
                for (var i = 1; i < 12; i++)
                {
                    imageAsset.PatchImage(img, from, new Rectangle(13 * i, 80, 13, 14));
                }
            }, 
            AssetEditPriority.Late + 10);
        }

        if (e.NameWithoutLocale.BaseName.Equals("LooseSprites/tailoring") && Config.Sewing)
        {
            e.Edit(asset =>
            {
                var imageAsset = asset.AsImage();
                var img = imageAsset.Data;
                var from = new Rectangle(0,156,24,48);
                for (var i = 1; i < 5; i++)
                {
                    imageAsset.PatchImage(img, from, new Rectangle(24 * i, 156, 24, 48));
                }
            }, 
            AssetEditPriority.Late + 10);
        }
    }

    private static void Maps(AssetRequestedEventArgs e)
    {
        if (e.NameWithoutLocale.BaseName.Equals("Maps/Mines/volcano_dungeon") && Config.Lava)
        {
            e.Edit(asset =>
            {
                var imageAsset = asset.AsImage();
                var img = imageAsset.Data;
                var from = new Rectangle(0,320,16, 64);
                for (var i = 1; i < 10; i++)
                {
                    imageAsset.PatchImage(img, from, new Rectangle(16 * i, 320, 16, 64));
                }
            }, 
            AssetEditPriority.Late + 10);
        }
        
        if (e.NameWithoutLocale.BaseName.EndsWith("_beach") && Config.Water)
        {
            var isBeach = e.NameWithoutLocale.BaseName.Equals("Maps/spring_beach") ||
                             e.NameWithoutLocale.BaseName.Equals("Maps/summer_beach") ||
                             e.NameWithoutLocale.BaseName.Equals("Maps/fall_beach") ||
                             e.NameWithoutLocale.BaseName.Equals("Maps/winter_beach");

            if (!isBeach)
                return;
            
            e.Edit(asset =>
            {
                var imageAsset = asset.AsImage();
                var img = imageAsset.Data;
                var from = new Rectangle(80,128,16,128);
                for (var i = 1; i < 7; i++)
                {
                    imageAsset.PatchImage(img, from, new Rectangle(80 + 16 * i, 128,16,128));
                }
            }, 
            AssetEditPriority.Late + 10);
        }

        if (e.NameWithoutLocale.BaseName.Equals("Maps/DesertTiles") && Config.Animations is NoFlash or Minimal)
        {
            e.Edit(asset =>
            {
                var imageAsset = asset.AsImage();
                var img = imageAsset.Data;
                
                var lightBulb = new Rectangle(144,192,16,16);
                imageAsset.PatchImage(img, lightBulb, new Rectangle(160,192,16,16));
                
                var sign = new Rectangle(32,176,48,16);
                imageAsset.PatchImage(img, sign, new Rectangle(80,176,48,16));
                imageAsset.PatchImage(img, sign, new Rectangle(128,176,48,16));
            }, 
            AssetEditPriority.Late + 10);
        }
        
        if (e.NameWithoutLocale.BaseName.Equals("Maps/Festivals"))
        {
            e.Edit(asset =>
            {
                var imageAsset = asset.AsImage();
                var img = imageAsset.Data;
                var from = new Rectangle(288,352,32,16);
                imageAsset.PatchImage(img, from, new Rectangle(320,352,32,16));
                imageAsset.PatchImage(img, from, new Rectangle(352,352,32,16));
                imageAsset.PatchImage(img, from, new Rectangle(320,336,32,16));
                imageAsset.PatchImage(img, from, new Rectangle(352,336,32,16));
            }, 
            AssetEditPriority.Late + 10);
        }
        
        if (e.NameWithoutLocale.BaseName.EndsWith("island_tilesheet_1") && Config.Water)
        {
            e.Edit(asset =>
            {
                var imageAsset = asset.AsImage();
                var img = imageAsset.Data;
                var beach1 = new Rectangle(80, 128, 16, 128);
                for (var i = 1; i < 7; i++)
                {
                    imageAsset.PatchImage(img, beach1, new Rectangle(80 + 16 * i, 128,16, 128));
                }

                var beach2 = new Rectangle(176,320,16,64);
                for (var i = 1; i < 7; i++)
                {
                    imageAsset.PatchImage(img, beach2, new Rectangle(176 + 16 * i, 320,16, 64));
                }

                var beach3 = new Rectangle(352,608,16,32);
                for (var i = 1; i < 7; i++)
                {
                    imageAsset.PatchImage(img, beach3, new Rectangle(352 + 16 * i, 608,16, 32));
                }
            }, 
            AssetEditPriority.Late + 10);
        }
        
        if (e.NameWithoutLocale.BaseName.StartsWith("Maps/MovieTheater") && e.NameWithoutLocale.BaseName.EndsWith("_TileSheet") && Config.Animations is NoFlash or Minimal)
        {
            e.Edit(asset =>
            {
                var imageAsset = asset.AsImage();
                var img = imageAsset.Data;
                var from = new Rectangle(32, 192, 32, 32);
                imageAsset.PatchImage(img, from, new Rectangle(80, 192, 32, 32));
                imageAsset.PatchImage(img, from, new Rectangle(80, 224, 32, 32));
            }, 
            AssetEditPriority.Late + 10);
        }

        if (e.NameWithoutLocale.BaseName.Equals("Maps/springobjects") && Config.Animations is NoFlash or Minimal)
        {
            /*
             * spring4 ToArea: 256, 224, 128, 16 ONLY if minimal
             * at (48, 16, 80, 16) we used to patch the splash animation
             * spring1 at (0, 240, 128, 16)
             */
            e.Edit(asset =>
            {
                var imageAsset = asset.AsImage();
                var patch = Helper.ModContent.Load<Texture2D>("assets/springobjects.png");
                imageAsset.PatchImage(patch, targetArea: new Rectangle(0, 240, 128, 16));
                
                if (Config.Animations == Minimal)
                    imageAsset.PatchImage(patch, targetArea: new Rectangle(256, 224, 128, 16));
            }, 
            AssetEditPriority.Late + 10);
        }
        
        if(e.NameWithoutLocale.BaseName.EndsWith("_town") && Config.Animations is NoFlash or Minimal)
        {
            //we patch all at once in same code
            var isSeasonal = e.NameWithoutLocale.BaseName.Equals("Maps/spring_town") ||
                             e.NameWithoutLocale.BaseName.Equals("Maps/summer_town") ||
                             e.NameWithoutLocale.BaseName.Equals("Maps/fall_town") ||
                             e.NameWithoutLocale.BaseName.Equals("Maps/winter_town");

            if (!isSeasonal)
                return;
            
            e.Edit(asset =>
            {
                var imageAsset = asset.AsImage();
                var img = imageAsset.Data;
                
                var upperLights = new Rectangle(192, 1040, 80, 16);
                imageAsset.PatchImage(img, upperLights, new Rectangle(272,1040,80,16));
                imageAsset.PatchImage(img, upperLights, new Rectangle(352,1040,80,16));
                
                var singularLight = new Rectangle(193, 1092, 6, 23);
                imageAsset.PatchImage(img,singularLight, new Rectangle(202,1092,6,23));
                imageAsset.PatchImage(img,singularLight, new Rectangle(211,1092,6,23));
                imageAsset.PatchImage(img,singularLight, new Rectangle(229,1092,6,23));
                imageAsset.PatchImage(img,singularLight, new Rectangle(202,1092,6,23));

                var thatEntireBatch = new Rectangle(192, 1092, 35, 23);
                imageAsset.PatchImage(img,thatEntireBatch,new Rectangle(237,1092,35,23));
                
                var last = new Rectangle(192, 1092, 80, 23);
                imageAsset.PatchImage(img,last,new Rectangle(272, 1092, 80, 23));
                imageAsset.PatchImage(img,last,new Rectangle(352, 1092, 80, 23));
            }, 
            AssetEditPriority.Late + 10);
        }

        if (e.NameWithoutLocale.BaseName.Equals("Maps/qiNutRoom_tilesheet") && Config.Animations is Minimal) //used to include noflash
        {
            e.Edit(asset =>
            {
                var imageAsset = asset.AsImage();
                var img = imageAsset.Data;
                var bun = new Rectangle(112, 144, 16, 32);
                var diamond = new Rectangle(128, 128, 16, 16);
                var xCoords = new[] { 144, 160, 176, 192, 208 };
                foreach (var x in xCoords)
                {
                    imageAsset.PatchImage(img, bun, new Rectangle(x,144,16,32));
                    imageAsset.PatchImage(img, diamond, new Rectangle(x,128,16,16));
                }
                imageAsset.PatchImage(img, bun, new Rectangle(128,128,16,32));

                var question = new Rectangle(0, 128, 32, 16);
                imageAsset.PatchImage(img, question, new Rectangle(32,128,32,16));
                imageAsset.PatchImage(img, question, new Rectangle(32,16,32,16));
            }, 
            AssetEditPriority.Late + 10);
        }
    }

    private static void Minigames(AssetRequestedEventArgs e)
    {
        if (e.NameWithoutLocale.BaseName.Equals("Minigames/TitleButtons") && Config.Animations == Minimal)
        {
            e.Edit(asset =>
            {
                var imageAsset = asset.AsImage();
                var img = imageAsset.Data;
                imageAsset.PatchImage(img,  new Rectangle(0, 303, 85, 141),  new Rectangle(85,303,85,141));
            }, 
            AssetEditPriority.Late + 10);
        }

        if (e.NameWithoutLocale.BaseName.Equals("Minigames/MaruComet") && Config.Animations is NoFlash or Minimal)
        {
            e.Edit(asset =>
            {
                var imageAsset = asset.AsImage();
                var img = imageAsset.Data;
                var fromArea = new Rectangle(0, 240, 143, 143);
                imageAsset.PatchImage(img, fromArea,  new Rectangle(143, 240, 143, 143));
                imageAsset.PatchImage(img, fromArea,  new Rectangle(286, 240, 143, 143));
            }, 
            AssetEditPriority.Late + 10);
        }
    }

    private static void Monsters(AssetRequestedEventArgs e)
    {
        if ((e.NameWithoutLocale.BaseName.StartsWith("Characters/Monsters/Armored Bug") || e.NameWithoutLocale.BaseName.StartsWith("Characters/Monsters/Bug")) && Config.Bugs)
        {
            e.Edit(asset =>
            {
                var imageAsset = asset.AsImage();
                var img = imageAsset.Data;
                var from = new Rectangle(0, 0, 16, 64);
                imageAsset.PatchImage(img, from, new Rectangle(16, 0, 16, 64));
                imageAsset.PatchImage(img, from, new Rectangle(32, 0, 16, 64));
                imageAsset.PatchImage(img, from, new Rectangle(48, 0, 16, 64));
            }, 
            AssetEditPriority.Late + 10);
        }
        
        if ((e.NameWithoutLocale.BaseName.StartsWith("Characters/Monsters/Fly") || e.NameWithoutLocale.BaseName.StartsWith("Characters/Monsters/Grub")) && Config.Bugs)
        {
            e.Edit(asset =>
            {
                var imageAsset = asset.AsImage();
                var img = imageAsset.Data;
                var from = new Rectangle(0, 0, 16, 120);
                imageAsset.PatchImage(img, from, new Rectangle(16, 0, 16, 120));
                imageAsset.PatchImage(img, from, new Rectangle(32, 0, 16, 120));
                imageAsset.PatchImage(img, from, new Rectangle(48, 0, 16, 120));
            }, 
            AssetEditPriority.Late + 10);
        }
        
        if (e.NameWithoutLocale.BaseName.StartsWith("Characters/Monsters/Magma") && Config.Magma)
        {
            e.Edit(asset =>
            {
                var imageAsset = asset.AsImage();
                var img = imageAsset.Data;
                //copy 1st one into 2nd
                imageAsset.PatchImage(img, new Rectangle(0, 0, 16, 64), new Rectangle(16, 0, 16, 64));
                //copy 4th into 5th/6th
                imageAsset.PatchImage(img, new Rectangle(64, 0, 16, 64), new Rectangle(80, 0, 16, 64));
                imageAsset.PatchImage(img, new Rectangle(64, 0, 16, 64), new Rectangle(96, 0, 16, 64));
                //copy 3rd one into 4th
                imageAsset.PatchImage(img,new Rectangle(32, 0, 16, 64), new Rectangle(64, 0, 16, 64));
            }, 
            AssetEditPriority.Late + 10);
        }
        
        if (e.NameWithoutLocale.BaseName.Equals("Characters/Monsters/Serpent") && Config.Serpents)
        {
            e.Edit(asset =>
            {
                var imageAsset = asset.AsImage();
                var img = imageAsset.Data;
                var range = new[] { 32, 64, 96 };
                var from = new Rectangle(0, 0, 32, 32);

                foreach (var x in range)
                {
                    imageAsset.PatchImage(img, from, new Rectangle(x, 0, 32, 32));
                    imageAsset.PatchImage(img, from, new Rectangle(x, 32, 32, 32));
                }
                
                imageAsset.PatchImage(img, from, new Rectangle(0, 32, 32, 32));
            }, 
            AssetEditPriority.Late + 10);
        }
    }
    
    private static void Tilesheets(AssetRequestedEventArgs e)
    {
        if (e.NameWithoutLocale.BaseName.Equals("TileSheets/animations"))
        {
            e.Edit(asset =>
            {
                var imageAsset = asset.AsImage();
                var img = imageAsset.Data;

                if (Config.FishingBubbles != All)
                {
                    var fishingTex = "assets/Fishing/" + Config.FishingBubbles switch
                    {
                        "Basic" => "transparent.png",
                        "Minimal" => "static.png",
                        _ => "move.png"
                    };
                    var fishing = Helper.ModContent.Load<Texture2D>(fishingTex);
                    
                    imageAsset.PatchImage(fishing, new Rectangle(0,0,640, 64), new Rectangle(0,3264, 640, 64));
                }
                
                if (Config.Animations == All)
                    return;
                
                if (Config.Animations == Basic)
                {
                    imageAsset.PatchImage(img, new Rectangle(0, 0, 64, 1152), new Rectangle(64, 0, 64, 1152));
                    imageAsset.PatchImage(img, new Rectangle(128, 0, 64, 1152), new Rectangle(192, 0, 64, 1152));
                    imageAsset.PatchImage(img, new Rectangle(256, 0, 64, 1152), new Rectangle(320, 0, 64, 1152));
                    imageAsset.PatchImage(img, new Rectangle(384, 0, 64, 1152), new Rectangle(448, 0, 64, 1152));
                }
                else if (Config.Animations is NoFlash or Minimal)
                {
                    //erases all "flashy" animations
                    
                    var zonesX = new[] { 0, 128, 256, 384 };
                    var zonesY = new[] { 0, 192, 320, 640 };
                    foreach (var x in zonesX)
                    {
                        foreach (var y in zonesY)
                        {
                            imageAsset.PatchImage(img, new Rectangle(512, 0, 128, 128), new Rectangle(x, y, 128, 128));
                        }
                    }
                    
                }

                //remove teleport warps
                var empty = new Rectangle(512, 0, 128, 128);
                var zonesX2 = new[] { 0, 128, 256, 384 };
                foreach (var x in zonesX2)
                {
                    imageAsset.PatchImage(img, empty, new Rectangle(x, 960, 128, 128));
                    imageAsset.PatchImage(img, empty, new Rectangle(x, 1152, 128, 128));
                }
                    
                var empty2 = new Rectangle(0, 1344, 320, 64);
                imageAsset.PatchImage(img, empty2, new Rectangle(0, 1280, 320, 64));
                imageAsset.PatchImage(img, empty2, new Rectangle(0, 384, 320, 64));
                imageAsset.PatchImage(img, empty2, new Rectangle(320, 384, 320, 64));
            }, 
            AssetEditPriority.Late + 10);
        }

        if (e.NameWithoutLocale.BaseName.Equals("TileSheets/Craftables") && Config.SlimeBall)
        {
            e.Edit(asset =>
            {
                var imageAsset = asset.AsImage();
                var img = imageAsset.Data;
                var from = new Rectangle(0,224,16,32);
                for (var i = 1; i < 6; i++)
                {
                    imageAsset.PatchImage(img, from, new Rectangle(16 * i,224,16,32));
                }
            }, 
            AssetEditPriority.Late + 10);
        }
        
        if (e.NameWithoutLocale.BaseName.Equals("TileSheets/emotes") && Config.Animations == Minimal)
        {
            e.Edit(asset =>
            {
                var imageAsset = asset.AsImage();
                var img = imageAsset.Data;
                //var yAxis = new[] { 0, 32, 48, 64, 80, 96, 112, 128, 144, 160, 176, 192, 208, 226, 240 };
                
                for (var i = 1; i < 16; i++)
                {
                    var from = new Rectangle(32, i * 16, 16, 16);
                    imageAsset.PatchImage(img, from, new Rectangle(0, i * 16, 16, 16));
                    imageAsset.PatchImage(img, from, new Rectangle(16, i * 16, 16, 16));
                    imageAsset.PatchImage(img, from, new Rectangle(48, i * 16, 16, 16));
                }
            }, 
            AssetEditPriority.Late + 10);
        }
        
        if (e.NameWithoutLocale.BaseName.Equals("TileSheets/rain") && Config.Rain.Equals(1.0f) == false)
        {
            e.Edit(asset =>
            {
                var imageAsset = asset.AsImage();
                var img = imageAsset.Data;
                try
                {
                    var colors = new Color[img.Width * img.Height];
                    img.GetData(colors);

                    var newColors = new List<Color>();
                    foreach (var color in colors)
                    {
                        newColors.Add(color * Config.Rain);
                    }

                    img.SetData(newColors.ToArray());
                }
                catch (Exception exception)
                {
                    ModEntry.Log($"Error: {exception}", LogLevel.Error);
                    throw;
                }
            }, 
            AssetEditPriority.Late + 10);
        }
    }
}