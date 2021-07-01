/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SpaceShared;
using StardewValley;
using xTile.Dimensions;
using xTile.Display;
using xTile.Layers;
using xTile.ObjectModel;

namespace LocationLayerTool.Patches
{
    [HarmonyPatch(typeof(Layer), "Draw")]
    public static class DrawPrefix
    {
        private static int rendering = 0;
        public static void Prefix( Layer __instance, IDisplayDevice displayDevice, xTile.Dimensions.Rectangle mapViewport, Location displayOffset, bool wrapAround, int pixelZoom )
        {
            if ( __instance.Id == "Back" && rendering == 0 && Game1.currentLocation.Map.Properties.ContainsKey( "RenderBehind" ) )
            {
                rendering++;
                try
                {
                    string prop = Game1.currentLocation.getMapProperty( "RenderBehind" );
                    string[] fields = prop.Split( ' ' );
                    string locName = fields[ 0 ];
                    int offsetX = 0, offsetY = 0;
                    if ( fields.Length >= 3 )
                    {
                        offsetX = int.Parse( fields[ 1 ] );
                        offsetY = int.Parse( fields[ 2 ] );
                    }
                    float scale = 1f;
                    if ( fields.Length >= 4 )
                    {
                        scale = float.Parse( fields[ 3 ] );
                    }
                    DoRendering( locName, offsetX, offsetY, scale );
                }
                catch ( Exception e )
                {
                    Log.error( "Exception while rendering: " + e );
                }
                rendering--;
            }
        }

        
        private static IDisplayDevice displayDevice;
        private static SpriteBatch spriteBatch;
        public static RenderTarget2D renderTarget;
        public static RenderTarget2D lightmap;
        private static void DoRendering( string locName, int offsetX, int offsetY, float scale )
        {
            if ( displayDevice == null )
            {
                var ddType = Type.GetType( "StardewModdingAPI.Framework.Rendering.SDisplayDevice, StardewModdingAPI" );
                var ddCon = ddType.GetConstructor( new Type[] { typeof( ContentManager ), typeof( GraphicsDevice ) } );
                displayDevice = ( IDisplayDevice ) ddCon.Invoke( new object[] { Game1.content, Game1.graphics.GraphicsDevice }  );
                spriteBatch = new SpriteBatch( Game1.graphics.GraphicsDevice );
            }
            if ( renderTarget == null || renderTarget.Width != Game1.graphics.GraphicsDevice.Viewport.Width || renderTarget.Height != Game1.graphics.GraphicsDevice.Viewport.Height )
            {
                renderTarget = new RenderTarget2D( Game1.graphics.GraphicsDevice, Game1.graphics.GraphicsDevice.Viewport.Width, Game1.graphics.GraphicsDevice.Viewport.Height, false, Game1.graphics.GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.None, 0, RenderTargetUsage.PreserveContents );

                /*var screenField = Mod.instance.Helper.Reflection.GetField< RenderTarget2D >( Game1.game1, "screen" );
                var screen = screenField.GetValue();
                if ( screen.RenderTargetUsage != RenderTargetUsage.PreserveContents )
                {
                    screen = new RenderTarget2D( Game1.graphics.GraphicsDevice, screen.Width, screen.Height, false, screen.Format, screen.DepthStencilFormat, screen.MultiSampleCount, RenderTargetUsage.PreserveContents );
                    screenField.SetValue( screen );
                }
                */
            }
            if ( lightmap == null )
            {
                lightmap = allocateLightmapNoDispose();
            }

            var lightmapField = Mod.instance.Helper.Reflection.GetField< RenderTarget2D >( typeof( Game1 ), "_lightmap" );
            var lightingBlend = Mod.instance.Helper.Reflection.GetField<BlendState>(Game1.game1, "lightingBlend");

            var oldHud = Game1.displayHUD;
            var oldDd = Game1.mapDisplayDevice;
            var oldSb = Game1.spriteBatch;
            var oldLoc = Game1.currentLocation;
            var oldTarget = (RenderTarget2D)( Game1.graphics.GraphicsDevice.GetRenderTargets().Length > 0 ? Game1.graphics.GraphicsDevice.GetRenderTargets()[ 0 ].RenderTarget : null );
            var oldDebris = Game1.debrisWeather;
            var oldLighting = Game1.drawLighting;
            var oldLights = Game1.currentLightSources;
            var oldLightmap = Game1.lightmap;
            var oldOutdoor = Game1.outdoorLight;
            var oldAmbient = Game1.ambientLight;
            var oldLightBlend = lightingBlend.GetValue();
            Game1.displayHUD = false;
            Game1.mapDisplayDevice = displayDevice;
            Game1.spriteBatch = spriteBatch;
            Game1.currentLocation = Game1.getLocationFromName( locName );
            Game1.debrisWeather = null;
            //Game1.drawLighting = false;
            Game1.currentLightSources = new HashSet<LightSource>(); buildLightSources();
            lightmapField.SetValue( lightmap );
            //Game1.outdoorLight = Color.White;
            Game1.ambientLight = new Color( 254, 254, 254, 0 );
            lightingBlend.SetValue( BlendState.Additive );
            Game1.graphics.GraphicsDevice.SetRenderTarget( renderTarget );
            try
            {
                Game1.currentLocation.map.LoadTileSheets( Game1.mapDisplayDevice );
                Mod.instance.Helper.Reflection.GetMethod( Game1.game1, "_draw" ).Invoke( new GameTime(), renderTarget );
            }
            catch ( Exception e )
            {
                Log.trace( "Exception rendering: " + e );
            }
            Game1.displayHUD = true;
            Game1.mapDisplayDevice = oldDd;
            Game1.spriteBatch = oldSb;
            Game1.currentLocation = oldLoc;
            Game1.debrisWeather = oldDebris;
            Game1.game1.takingMapScreenshot = false;
            Game1.drawLighting = oldLighting;
            Game1.currentLightSources = oldLights;
            lightmapField.SetValue( oldLightmap );
            Game1.outdoorLight = oldOutdoor;
            Game1.ambientLight = oldAmbient;
            lightingBlend.SetValue( oldLightBlend );
            Game1.graphics.GraphicsDevice.SetRenderTarget( oldTarget );

            Game1.spriteBatch.Draw( renderTarget, new Vector2( offsetX * Game1.tileSize, offsetY * Game1.tileSize ), null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0 );
        }

        private static RenderTarget2D allocateLightmapNoDispose()
        {
            int width = 2048, height = 2048;
            int num1 = 32;
            float num2 = 1f;
            if ( Game1.options != null )
            {
                num1 = Game1.options.lightingQuality;
                num2 = Game1.options.zoomLevel;
            }
            int width1 = (int)((double)width * (1.0 / (double)num2) + 64.0) / (num1 / 2);
            int height1 = (int)((double)height * (1.0 / (double)num2) + 64.0) / (num1 / 2);
            //if ( Game1.lightmap != null && Game1.lightmap.Width == width1 && Game1.lightmap.Height == height1 )
            //    return null;
            //if ( Game1._lightmap != null )
            //    Game1._lightmap.Dispose();
            return new RenderTarget2D( Game1.graphics.GraphicsDevice, width1, height1, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents );
        }

        private static void buildLightSources()
        {
            Game1.currentLocation.Map.Properties.TryGetValue( "Light", out PropertyValue lightProp );
            if ( lightProp != null && !Game1.currentLocation.ignoreLights.Value )
            {
                string[] strArray = lightProp.ToString().Split(' ');
                for ( int index = 0; index < strArray.Length; index += 3 )
                    Game1.currentLightSources.Add( new LightSource( Convert.ToInt32( strArray[ index + 2 ] ), new Vector2( ( float ) ( Convert.ToInt32( strArray[ index ] ) * 64 + 32 ), ( float ) ( Convert.ToInt32( strArray[ index + 1 ] ) * 64 + 32 ) ), 1f, LightSource.LightContext.MapLight, 0L ) );
            }
            Game1.currentLightSources.UnionWith( Game1.currentLocation.sharedLights.Values );
        }
    }
}
