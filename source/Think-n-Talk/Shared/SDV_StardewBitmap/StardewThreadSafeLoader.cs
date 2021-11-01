/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Prism-99/Think-n-Talk
**
*************************************************/

//
//  Used for retrieving game graphic assets
//  Textures and sprite sheets
//
//  SDV 1.5.5 switched to use MonoGame which enforces thread access rules
//  more tighly than the base XNA API
//
//  All graphic access must be done on the UI thread or they will fail
//
//  This class leverages existing SMAPI game hooks to ensure graphic
//  loading is done in the required thread.
//

using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System.Collections.Generic;
using System.Threading;
using StardewValley;
using System.IO;

namespace StardewModHelpers
{
    internal static class StardewThreadSafeLoader
    {
        private static List<string> lImagesToLoad = new List<string> { };
        private static Dictionary<string, StardewBitmap> dcImages = new Dictionary<string, StardewBitmap> { };
        private static List<string> lSpriteSheetToLoad = new List<string> { };
        private static Dictionary<string, StardewBitmap> dcSpriteSheets = new Dictionary<string, StardewBitmap> { };
        private static Dictionary<string, Texture2D> lTexturesToLoad = new Dictionary<string, Texture2D> { };
        private static Dictionary<string, MemoryStream> dcTexturesLoaded = new Dictionary<string, MemoryStream> { };
        private static IModHelper oHelper;
        private static int mainThreadId;
        public static void Initialize(IModHelper helper)
        {
            oHelper = helper;
            // making assumpition being initialized in main thread
            mainThreadId = Thread.CurrentThread.ManagedThreadId;
        }
        public static MemoryStream GetTextureMS(string sId, Texture2D oTexture)
        {
            MemoryStream ms;
            //
            //  check if the request can be handled in the current
            //  thread or needs to be queued to be handled in the UI
            //  thread
            //
            if (Thread.CurrentThread.ManagedThreadId == mainThreadId)
            {
                ms = new MemoryStream();
                oTexture.SaveAsPng(ms, oTexture.Width, oTexture.Height);
                //Go To the  beginning of the stream.
                ms.Seek(0, SeekOrigin.Begin);
                //Create the image based on the stream.
                return ms;
            }
            else
            {
                //
                //  Queue up spritesheet request
                //
                lock (lTexturesToLoad)
                {
                    lTexturesToLoad.Add(sId, oTexture);
                }
                oHelper.Events.GameLoop.UpdateTicked += GameLoop_Update_LoadTexture; 
                //
                //  Wait for request to be handled in another thread
                //
                while (!dcTexturesLoaded.ContainsKey(sId))
                {
                    Thread.Sleep(100);
                }
                //
                //  Retrieve results
                //
                lock (dcTexturesLoaded)
                {
                    ms = dcTexturesLoaded[sId];
                    dcTexturesLoaded.Remove(sId);
                }
            }

            return ms;
        }

        public static StardewBitmap LoadSpriteSheet(string sSheetName)
        {
            StardewBitmap sbResult;
            //
            //  check if the request can be handled in the current
            //  thread or needs to be queued to be handled in the UI
            //  thread
            //
            if (Thread.CurrentThread.ManagedThreadId == mainThreadId)
            {
                sbResult = GetSpriteSheet(sSheetName);
            }
            else
            {
                //
                //  Queue up spritesheet request
                //
                lock (lSpriteSheetToLoad)
                {
                    lSpriteSheetToLoad.Add(sSheetName);
                }
                oHelper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked_LoadSheet;
                //
                //  Wait for request to be handled in another thread
                //
                while (!dcSpriteSheets.ContainsKey(sSheetName))
                {
                    Thread.Sleep(100);
                }
                //
                //  Retrieve results
                //
                lock (dcSpriteSheets)
                {
                    sbResult = dcSpriteSheets[sSheetName];
                    dcSpriteSheets.Remove(sSheetName);
                }
            }

            return sbResult;
        }
        private static StardewBitmap GetSpriteSheet(string sSheetName)
        {
#if TRACE && StardewWeb
            StardewLogger.LogTrace("GetSpritesheet", $"sheetname '{sSheetName}'");
#endif
            //
            //  Fetch requested spritesheet
            //
            switch (sSheetName)
            {
                case "emoteSpriteSheet":
                    return new StardewBitmap(Game1.emoteSpriteSheet);
                case "objectSpriteSheet":
                    return new StardewBitmap(Game1.objectSpriteSheet);
                case "bigCraftableSpriteSheet":
                    return new StardewBitmap(Game1.bigCraftableSpriteSheet);
                default:
                    return new StardewBitmap(oHelper.Content.Load<Texture2D>(sSheetName, ContentSource.GameContent));
            }
        }
        public static StardewBitmap LoadImageInUIThread(string sImage)
        {
            StardewBitmap sbResult;
            //
            //  check if the request can be handled in the current
            //  thread or needs to be queued to be handled in the UI
            //  thread
            //
            if (Thread.CurrentThread.ManagedThreadId == mainThreadId)
            {
                sbResult = new StardewBitmap(oHelper.Content.Load<Texture2D>(sImage, ContentSource.GameContent));
            }
            else
            {
                //
                //  Queue up spritesheet request
                //
                lock (lImagesToLoad)
                {
                    lImagesToLoad.Add(sImage);
                    //
                    // use SMAPI to add hook to fetch image in the UI thread
                    //
                    oHelper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked_LoadImage;
                }
                //
                //  Wait for request to be handled in another thread
                //
                while (!dcImages.ContainsKey(sImage))
                {
                    Thread.Sleep(200);
                }
                //
                //  Retrieve results
                //
                lock (dcImages)
                {
                    sbResult = dcImages[sImage];
                    dcImages.Remove(sImage);
                }
            }

            return sbResult;
        }
        private static void GameLoop_UpdateTicked_LoadImage(object sender, StardewModdingAPI.Events.UpdateTickedEventArgs e)
        {
            //
            //  Get request image in UI thread
            //
            lock (lImagesToLoad)
            {
                foreach (string sImagePath in lImagesToLoad)
                {
                    try
                    {
                        Texture2D txImage = oHelper.Content.Load<Texture2D>(sImagePath, ContentSource.GameContent);
                        lock (dcImages)
                        {
                            dcImages.Add(sImagePath, new StardewBitmap(txImage));
                        }
                    }
                    catch
                    {
                        dcImages.Add(sImagePath, null);
                    }
                }
                //
                //  clear requesst list
                //
                lImagesToLoad.Clear();
                //
                //  unhook event
                //
                oHelper.Events.GameLoop.UpdateTicked -= GameLoop_UpdateTicked_LoadImage;
            }
        }
        private static void GameLoop_UpdateTicked_LoadSheet(object sender, StardewModdingAPI.Events.UpdateTickedEventArgs e)
        {
            lock (lSpriteSheetToLoad)
            {
                foreach (string sImagePath in lSpriteSheetToLoad)
                {
                    lock (dcSpriteSheets)
                    {
                        dcSpriteSheets.Add(sImagePath, GetSpriteSheet(sImagePath));
                    }
                }
                //
                //  clear request list
                //
                lSpriteSheetToLoad.Clear();
                //
                //  unhook event
                //
                oHelper.Events.GameLoop.UpdateTicked -= GameLoop_UpdateTicked_LoadSheet;
            }
        }

        private static void GameLoop_Update_LoadTexture(object sender, StardewModdingAPI.Events.UpdateTickedEventArgs e)
        {
            lock (lSpriteSheetToLoad)
            {
                foreach (string sTextureId in lTexturesToLoad.Keys)
                {
                    lock (dcTexturesLoaded)
                    {
                        MemoryStream ms = new MemoryStream();
                        lTexturesToLoad[sTextureId].SaveAsPng(ms, lTexturesToLoad[sTextureId].Width, lTexturesToLoad[sTextureId].Height);
                        //Go To the  beginning of the stream.
                        ms.Seek(0, SeekOrigin.Begin);
                        //Create the image based on the stream.

                        dcTexturesLoaded.Add(sTextureId, ms);
                    }
                }
                //
                //  clear request list
                //
                lTexturesToLoad.Clear();
                //
                //  unhook event
                //
                oHelper.Events.GameLoop.UpdateTicked -= GameLoop_Update_LoadTexture;
            }
        }
    }
}