using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using StardewValley.Projectiles;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using xTile.Dimensions;

using xRectangle = xTile.Dimensions.Rectangle;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using xTile.Layers;
using StardewValley.Tools;
using StardewValley.Locations;

namespace ShaderExample
{
    class Class1 : Mod
    {
        public static Effect effect;


        public override void Entry(IModHelper helper)
        {
            //StardewModdingAPI.Events.GraphicsEvents.OnPreRenderEvent += GraphicsEvents_OnPreRenderEvent;

            //Need to make checks to see what location I am at and have custom shader functions for those events.

            StardewModdingAPI.Events.GraphicsEvents.OnPostRenderEvent += GraphicsEvents_OnPreRenderEvent;
            //StardewModdingAPI.Events.GraphicsEvents.OnPreRenderEvent += GraphicsEvents_OnPreRenderEvent;

            //StardewModdingAPI.Events.GraphicsEvents.OnPostRenderEvent += GraphicsEvents_OnPreRenderEvent1;
            effect = Helper.Content.Load<Effect>(Path.Combine("Content", "Shaders", "GreyScaleEffect.xnb"));
        }

        public void applyCurrentShader(bool ignoreOutdoorLight = false,bool addLightGlow=false,int r=0, int g=0, int b=0,int a=0)
        {
            
            SetInstanceField(typeof(SpriteBatch), Game1.spriteBatch, effect, "customEffect");
            Class1.effect.CurrentTechnique.Passes[0].Apply();
            if (Game1.player.currentLocation != null && Game1.activeClickableMenu==null)
            {
                if (Game1.currentLocation.ignoreOutdoorLighting.Value == false&&ignoreOutdoorLight==false)
                {
                    //Monitor.Log("WTF");
                    Monitor.Log(Game1.outdoorLight.ToString());
                    Matrix projection = Matrix.CreateOrthographicOffCenter(0,Game1.graphics.GraphicsDevice.Viewport.Width, Game1.graphics.GraphicsDevice.Viewport.Height, 0, 0, 1);
                    Matrix halfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);
                    //effect.Parameters["MatrixTransform"].SetValue(halfPixelOffset * projection);
                    effect.Parameters["ambientRed"].SetValue(Game1.outdoorLight.R);
                    effect.Parameters["ambientGreen"].SetValue(Game1.outdoorLight.G);
                    effect.Parameters["ambientBlue"].SetValue(Game1.outdoorLight.B);

                    effect.Parameters["addLightGlow"].SetValue(addLightGlow);

                    effect.Parameters["addedRed"].SetValue(r);
                    effect.Parameters["addedGreen"].SetValue(g);
                    effect.Parameters["addedBlue"].SetValue(b);
                    effect.Parameters["addedAlpha"].SetValue(b);

                    effect.Parameters["timeOfDay"].SetValue(Game1.timeOfDay);
                }
                else
                {
                    //Set to 255 to symbolize a white effect.
                    effect.Parameters["ambientRed"].SetValue(255f);
                    effect.Parameters["ambientGreen"].SetValue(255f);
                    effect.Parameters["ambientBlue"].SetValue(255f);
                    effect.Parameters["timeOfDay"].SetValue(600f);

                    effect.Parameters["addLightGlow"].SetValue(addLightGlow);

                    effect.Parameters["addedRed"].SetValue(r);
                    effect.Parameters["addedGreen"].SetValue(g);
                    effect.Parameters["addedBlue"].SetValue(b);
                    effect.Parameters["addedAlpha"].SetValue(b);
                }
            }
            else
            {
                effect.Parameters["ambientRed"].SetValue(255f);
                effect.Parameters["ambientGreen"].SetValue(255f);
                effect.Parameters["ambientBlue"].SetValue(255f);
                effect.Parameters["timeOfDay"].SetValue(600f);

                effect.Parameters["addLightGlow"].SetValue(addLightGlow);

                effect.Parameters["addedRed"].SetValue(r);
                effect.Parameters["addedGreen"].SetValue(g);
                effect.Parameters["addedBlue"].SetValue(b);
                effect.Parameters["addedAlpha"].SetValue(b);
            }
        }

        private void GraphicsEvents_OnPreRenderEvent(object sender, EventArgs e)
        {
          
            try
            {

                Game1.spriteBatch.End();
            }
            catch(Exception err)
            {
                return;
            }
            if (Game1.activeClickableMenu != null)
            {
                Game1.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
                applyCurrentShader();
                Game1.activeClickableMenu.draw(Game1.spriteBatch);
                Game1.spriteBatch.End();
            }

            if (Game1.player.currentLocation == null)
            {
                Game1.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
                return;
            }
            Game1.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
            //drawBack();
            drawMapPart1();
            Game1.spriteBatch.End();

            Game1.spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
            //drawMapPart2();
            applyCurrentShader();
            AHHHH();
            Game1.spriteBatch.End();

            
            Game1.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
            applyCurrentShader(false);
            Game1.player.currentLocation.drawAboveFrontLayer(Game1.spriteBatch);
            Game1.spriteBatch.End();

            Game1.spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
            applyCurrentShader();
            if (Game1.currentLocation.Map.GetLayer("AlwaysFront") != null)
            {
                Game1.mapDisplayDevice.BeginScene(Game1.spriteBatch);
                Game1.currentLocation.Map.GetLayer("AlwaysFront").Draw(Game1.mapDisplayDevice, Game1.viewport, Location.Origin, false, 4);
                Game1.mapDisplayDevice.EndScene();
            }
            //Game1.currentLocation.drawAboveAlwaysFrontLayer(Game1.spriteBatch);
            Game1.player.currentLocation.drawAboveAlwaysFrontLayer(Game1.spriteBatch);
            
            Game1.spriteBatch.End();


            if (Game1.activeClickableMenu != null)
            {
                Game1.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
                applyCurrentShader();
                Game1.activeClickableMenu.draw(Game1.spriteBatch);
                Game1.spriteBatch.End();
                Game1.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
                applyCurrentShader();
                if (Game1.activeClickableMenu is StardewValley.Menus.GameMenu)
                {
                    if ((Game1.activeClickableMenu as StardewValley.Menus.GameMenu).currentTab == 3) return;
                    //Draw menu tabs.
                    var tabField = GetInstanceField(typeof(StardewValley.Menus.GameMenu), Game1.activeClickableMenu, "tabs");
                    var tabs = (List<ClickableComponent>)tabField;
                    foreach (ClickableComponent tab in tabs)
                    {
                        int num = 0;
                        switch (tab.name)
                        {
                            case "catalogue":
                                num = 7;
                                break;
                            case "collections":
                                num = 5;
                                break;
                            case "coop":
                                num = 1;
                                break;
                            case "crafting":
                                num = 4;
                                break;
                            case "exit":
                                num = 7;
                                break;
                            case "inventory":
                                num = 0;
                                break;
                            case "map":
                                num = 3;
                                break;
                            case "options":
                                num = 6;
                                break;
                            case "skills":
                                num = 1;
                                break;
                            case "social":
                                num = 2;
                                break;
                        }
                        Game1.spriteBatch.Draw(Game1.mouseCursors, new Vector2((float)tab.bounds.X, (float)(tab.bounds.Y + ((Game1.activeClickableMenu as StardewValley.Menus.GameMenu).currentTab == (Game1.activeClickableMenu as StardewValley.Menus.GameMenu).getTabNumberFromName(tab.name) ? 8 : 0))), new Rectangle?(new Rectangle(num * 16, 368, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.00001f);
                        if (tab.name.Equals("skills"))
                            Game1.player.FarmerRenderer.drawMiniPortrat(Game1.spriteBatch, new Vector2((float)(tab.bounds.X + 8), (float)(tab.bounds.Y + 12 + ((Game1.activeClickableMenu as StardewValley.Menus.GameMenu).currentTab == (Game1.activeClickableMenu as StardewValley.Menus.GameMenu).getTabNumberFromName(tab.name) ? 8 : 0))), 0.00011f, 3f, 2, Game1.player);
                    }

                    if ((Game1.activeClickableMenu as StardewValley.Menus.GameMenu).currentTab == 2)
                    {


                        var pageField = GetInstanceField(typeof(StardewValley.Menus.GameMenu), Game1.activeClickableMenu, "pages");
                        var pages = (List<IClickableMenu>)pageField;

                        var socialPage = pages.ElementAt(2);
                        var v = (StardewValley.Menus.SocialPage)socialPage;
                        if (v == null)
                        {
                            Monitor.Log("WHATTT?????");
                        }
                        v = (StardewValley.Menus.SocialPage)v;


                        int numFarmers = (int)GetInstanceField(typeof(StardewValley.Menus.SocialPage), v, "numFarmers");

                        getInvokeMethod(v, "drawHorizontalPartition", new object[]{
                        Game1.spriteBatch, v.yPositionOnScreen + IClickableMenu.borderWidth + 128 + 4, true
                        });
                        getInvokeMethod(v, "drawHorizontalPartition", new object[]{
                        Game1.spriteBatch, v.yPositionOnScreen + IClickableMenu.borderWidth + 192 + 32 + 20, true
                        });
                        getInvokeMethod(v, "drawHorizontalPartition", new object[]{
                        Game1.spriteBatch, v.yPositionOnScreen + IClickableMenu.borderWidth + 320 + 36, true
                        });
                        getInvokeMethod(v, "drawHorizontalPartition", new object[]{
                        Game1.spriteBatch, v.yPositionOnScreen + IClickableMenu.borderWidth + 384 + 32 + 52, true
                        });
                        Rectangle scissorRectangle = Game1.spriteBatch.GraphicsDevice.ScissorRectangle;
                        Rectangle rectangle = scissorRectangle;
                        rectangle.Y = Math.Max(0, rowPosition(v, numFarmers - 1));
                        rectangle.Height -= rectangle.Y;
                        Game1.spriteBatch.GraphicsDevice.ScissorRectangle = rectangle;
                        try
                        {

                            getInvokeMethod(v, "drawVerticalPartition", new object[]
                            {
                            Game1.spriteBatch,
                             v.xPositionOnScreen + 256 + 12,
                             true
                            });
                        }
                        finally
                        {
                            Game1.spriteBatch.GraphicsDevice.ScissorRectangle = scissorRectangle;
                        }
                        getInvokeMethod(v, "drawVerticalPartition", new object[]
                            {
                            Game1.spriteBatch,
                             v.xPositionOnScreen + 256 + 12+340,
                             true
                            });



                        int slotPosition2 = (int)GetInstanceField(typeof(StardewValley.Menus.SocialPage), v, "slotPosition");




                        var sprites = (List<ClickableTextureComponent>)GetInstanceField(typeof(StardewValley.Menus.SocialPage), v, "sprites");
                        var names = (List<object>)GetInstanceField(typeof(StardewValley.Menus.SocialPage), v, "names");
                        for (int slotPosition = slotPosition2; slotPosition < slotPosition2 + 5; ++slotPosition)
                        {
                            if (slotPosition < sprites.Count)
                            {
                                if (names[slotPosition] is string)
                                    getInvokeMethod(v, "drawNPCSlot", new object[]{
                                    Game1.spriteBatch, slotPosition
                                    });
                                else if (names[slotPosition] is long)
                                    getInvokeMethod(v, "drawFarmerSlot", new object[]{
                                    Game1.spriteBatch, slotPosition
                                    });
                            }
                        }


                        (GetInstanceField(typeof(SocialPage), v, "upButton") as ClickableTextureComponent).draw(Game1.spriteBatch);
                        (GetInstanceField(typeof(SocialPage), v, "downButton") as ClickableTextureComponent).draw(Game1.spriteBatch);
                        Rectangle scrollBarRunner = (Rectangle)(GetInstanceField(typeof(SocialPage), v, "scrollBarRunner"));
                        IClickableMenu.drawTextureBox(Game1.spriteBatch, Game1.mouseCursors, new Rectangle(403, 383, 6, 6), scrollBarRunner.X, scrollBarRunner.Y, scrollBarRunner.Width, scrollBarRunner.Height, Color.White, 4f, true);
                        (GetInstanceField(typeof(SocialPage), v, "scrollBar") as ClickableTextureComponent).draw(Game1.spriteBatch);
                        string hoverText = (GetInstanceField(typeof(SocialPage), v, "hoverText") as string);
                        if (!hoverText.Equals(""))
                            IClickableMenu.drawHoverText(Game1.spriteBatch, hoverText, Game1.smallFont, 0, 0, -1, (string)null, -1, (string[])null, (Item)null, 0, -1, -1, -1, -1, 1f, (CraftingRecipe)null);
                    }

                    if ((Game1.activeClickableMenu as StardewValley.Menus.GameMenu).currentTab == 4)
                    {
                        var pageField = GetInstanceField(typeof(StardewValley.Menus.GameMenu), Game1.activeClickableMenu, "pages");
                        var pages = (List<IClickableMenu>)pageField;

                        var craftingPage = pages.ElementAt(4);
                        Monitor.Log(craftingPage.GetType().ToString());
                        var v = (StardewValley.Menus.CraftingPage)craftingPage;
                        Framework.Drawers.Menus.craftingPageDraw((craftingPage as StardewValley.Menus.CraftingPage), Game1.spriteBatch);
                    }

                }
                try
                {
                    Game1.activeClickableMenu.upperRightCloseButton.draw(Game1.spriteBatch);
                }
                catch(Exception err)
                {

                }
                Game1.activeClickableMenu.drawMouse(Game1.spriteBatch);
                Game1.spriteBatch.End();
            }
            //Location specific drawing done here


            //Game1.spriteBatch.End();

            Game1.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
            applyCurrentShader(true);
            if(Game1.activeClickableMenu==null&& Game1.eventUp==false)getInvokeMethod(Program.gamePtr, "drawHUD", new object[] { });
            
                     if (Game1.hudMessages.Count > 0 && (!Game1.eventUp || Game1.isFestival()))
                            {
                                for (int i = Game1.hudMessages.Count - 1; i >= 0; --i)
                                    Game1.hudMessages[i].draw(Game1.spriteBatch, i);
                            }

            Game1.spriteBatch.End();

            
            for (int index = 0; index < Game1.currentLightSources.Count; ++index)
            {
                if (Utility.isOnScreen((Vector2)(Game1.currentLightSources.ElementAt<LightSource>(index).position), (int)((double)(float)(Game1.currentLightSources.ElementAt<LightSource>(index).radius) * 64.0 * 4.0)))
                {
                    //Game1.spriteBatch.Draw(Game1.currentLightSources.ElementAt<LightSource>(index).lightTexture, Game1.GlobalToLocal(Game1.viewport, (Vector2)(Game1.currentLightSources.ElementAt<LightSource>(index).position)) / (float)(Game1.options.lightingQuality / 2), new Microsoft.Xna.Framework.Rectangle?(Game1.currentLightSources.ElementAt<LightSource>(index).lightTexture.Bounds), (Color)(Game1.currentLightSources.ElementAt<LightSource>(index).color), 0.0f, new Vector2((float)Game1.currentLightSources.ElementAt<LightSource>(index).lightTexture.Bounds.Center.X, (float)Game1.currentLightSources.ElementAt<LightSource>(index).lightTexture.Bounds.Center.Y), (float)(Game1.currentLightSources.ElementAt<LightSource>(index).radius) / (float)(Game1.options.lightingQuality / 2), SpriteEffects.None, 0.9f);
                    Game1.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
                    Color c = (Color)(Game1.currentLightSources.ElementAt<LightSource>(index).color);
                    applyCurrentShader(false,true,0,0,0);
                    Game1.spriteBatch.Draw(Game1.currentLightSources.ElementAt<LightSource>(index).lightTexture, Game1.GlobalToLocal(Game1.viewport, (Vector2)(Game1.currentLightSources.ElementAt<LightSource>(index).position)), new Microsoft.Xna.Framework.Rectangle?(Game1.currentLightSources.ElementAt<LightSource>(index).lightTexture.Bounds), (Color)(Game1.currentLightSources.ElementAt<LightSource>(index).color.Value), 0.0f, new Vector2((float)Game1.currentLightSources.ElementAt<LightSource>(index).lightTexture.Bounds.Center.X, (float)Game1.currentLightSources.ElementAt<LightSource>(index).lightTexture.Bounds.Center.Y), (float)(Game1.currentLightSources.ElementAt<LightSource>(index).radius/2), SpriteEffects.None, 0.9f);
                    Game1.spriteBatch.End();
                }
            }
           

            Game1.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
            applyCurrentShader(true);
            drawMouse();
            Game1.spriteBatch.End();
            

            Game1.spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
            //applyCurrentShader();
            //drawMouse();
        }


        private int rowPosition(IClickableMenu menu,int i)
        {
            int slotPosition2 = (int)GetInstanceField(typeof(StardewValley.Menus.SocialPage), menu, "slotPosition");
            int num1 = i - slotPosition2;
            int num2 = 112;
            return menu.yPositionOnScreen + IClickableMenu.borderWidth + 160 + 4 + num1 * num2;
        }
        


        public void drawMapPart1()
        {
            //Game1.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
            applyCurrentShader();
            foreach (var layer in Game1.player.currentLocation.map.Layers)
            {
                //do back and buildings
                if (layer.Id == "Paths" || layer.Id=="AlwaysFront"|| layer.Id=="Front" ) continue;
                //if (layer.Id != "Back" || layer.Id != "Buildings") continue;
                //Framework.Drawers.Layer.drawLayer(layer,Game1.mapDisplayDevice, Game1.viewport, new xTile.Dimensions.Location(0, 0), false, Game1.pixelZoom);
                layer.Draw(Game1.mapDisplayDevice, Game1.viewport, new xTile.Dimensions.Location(0, 0), false, Game1.pixelZoom);

            }
            //Game1.spriteBatch.End();
        }

        public static object getInvokeMethod(object target, string name ,object[] param)
        {
            var hello=target.GetType().GetMethod(name, BindingFlags.Public | BindingFlags.NonPublic| BindingFlags.Instance| BindingFlags.Static);
            return hello.Invoke(target, param);
        }


        public void AHHHH()
        {
            if (!Game1.currentLocation.shouldHideCharacters())
            {
                if (Game1.CurrentEvent == null)
                {
                    foreach (NPC character in Game1.currentLocation.characters)
                    {
                        if (!(bool)(character.swimming) && !character.HideShadow && Game1.currentLocation.shouldShadowBeDrawnAboveBuildingsLayer(character.getTileLocation()))
                            Game1.spriteBatch.Draw(Game1.shadowTexture, Game1.GlobalToLocal(Game1.viewport, character.Position + new Vector2((float)(character.Sprite.SpriteWidth * 4) / 2f, (float)(character.GetBoundingBox().Height + (character.IsMonster ? 0 : 12)))), new Microsoft.Xna.Framework.Rectangle?(Game1.shadowTexture.Bounds), Color.White, 0.0f, new Vector2((float)Game1.shadowTexture.Bounds.Center.X, (float)Game1.shadowTexture.Bounds.Center.Y), (float)(4.0 + (double)character.yJumpOffset / 40.0) * (float)(character.scale), SpriteEffects.None, Math.Max(0.0f, (float)character.getStandingY() / 10000f) - 1E-06f);
                    }
                }
                else
                {
                    foreach (NPC actor in Game1.CurrentEvent.actors)
                    {
                        if (!(bool)(actor.swimming) && !actor.HideShadow && Game1.currentLocation.shouldShadowBeDrawnAboveBuildingsLayer(actor.getTileLocation()))
                            Game1.spriteBatch.Draw(Game1.shadowTexture, Game1.GlobalToLocal(Game1.viewport, actor.Position + new Vector2((float)(actor.Sprite.SpriteWidth * 4) / 2f, (float)(actor.GetBoundingBox().Height + (actor.IsMonster ? 0 : 12)))), new Microsoft.Xna.Framework.Rectangle?(Game1.shadowTexture.Bounds), Color.White, 0.0f, new Vector2((float)Game1.shadowTexture.Bounds.Center.X, (float)Game1.shadowTexture.Bounds.Center.Y), (float)(4.0 + (double)actor.yJumpOffset / 40.0) * (float)(actor.scale), SpriteEffects.None, Math.Max(0.0f, (float)actor.getStandingY() / 10000f) - 1E-06f);
                    }
                }
                foreach (Farmer farmer in Game1.currentLocation.farmers)
                {
                    if (!(bool)(farmer.swimming) && !farmer.isRidingHorse() && (Game1.currentLocation != null && Game1.currentLocation.shouldShadowBeDrawnAboveBuildingsLayer(farmer.getTileLocation())))
                    {
                        SpriteBatch spriteBatch = Game1.spriteBatch;
                        Texture2D shadowTexture = Game1.shadowTexture;
                        Vector2 local = Game1.GlobalToLocal(farmer.Position + new Vector2(32f, 24f));
                        Microsoft.Xna.Framework.Rectangle? sourceRectangle = new Microsoft.Xna.Framework.Rectangle?(Game1.shadowTexture.Bounds);
                        Color white = Color.White;
                        double num1 = 0.0;
                        Microsoft.Xna.Framework.Rectangle bounds = Game1.shadowTexture.Bounds;
                        double x = (double)bounds.Center.X;
                        bounds = Game1.shadowTexture.Bounds;
                        double y = (double)bounds.Center.Y;
                        Vector2 origin = new Vector2((float)x, (float)y);
                        double num2 = 4.0 - (!farmer.running && !farmer.UsingTool || farmer.FarmerSprite.currentAnimationIndex <= 1 ? 0.0 : (double)Math.Abs(FarmerRenderer.featureYOffsetPerFrame[farmer.FarmerSprite.CurrentFrame]) * 0.5);
                        int num3 = 0;
                        double num4 = 0.0;
                        spriteBatch.Draw(shadowTexture, local, sourceRectangle, white, (float)num1, origin, (float)num2, (SpriteEffects)num3, (float)num4);
                    }
                }
            }
            if ((Game1.eventUp || Game1.killScreen) && (!Game1.killScreen && Game1.currentLocation.currentEvent != null))
                Game1.currentLocation.currentEvent.draw(Game1.spriteBatch);
            if (Game1.player.currentUpgrade != null && Game1.player.currentUpgrade.daysLeftTillUpgradeDone <= 3 && Game1.currentLocation.Name.Equals("Farm"))
                Game1.spriteBatch.Draw(Game1.player.currentUpgrade.workerTexture, Game1.GlobalToLocal(Game1.viewport, Game1.player.currentUpgrade.positionOfCarpenter), new Microsoft.Xna.Framework.Rectangle?(Game1.player.currentUpgrade.getSourceRectangle()), Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, (float)(((double)Game1.player.currentUpgrade.positionOfCarpenter.Y + 48.0) / 10000.0));
            Game1.currentLocation.draw(Game1.spriteBatch);
            if (Game1.eventUp && Game1.currentLocation.currentEvent != null)
            {
                string messageToScreen = Game1.currentLocation.currentEvent.messageToScreen;
            }
            if (Game1.player.ActiveObject == null && (Game1.player.UsingTool || Game1.pickingTool) && (Game1.player.CurrentTool != null && (!Game1.player.CurrentTool.Name.Equals("Seeds") || Game1.pickingTool)))
                Game1.drawTool(Game1.player);
            if (Game1.currentLocation.Name.Equals("Farm"))
                Monitor.Log("DRAW farm buildings here");
                //getInvokeMethod((Game1.currentLocation as GameLocation), "drawFarmBuildings", new object[] { });
            if (Game1.tvStation >= 0)
                Game1.spriteBatch.Draw(Game1.tvStationTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2(400f, 160f)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(Game1.tvStation * 24, 0, 24, 15)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-08f);
            if (Game1.panMode)
            {
                Game1.spriteBatch.Draw(Game1.fadeToBlackRect, new Microsoft.Xna.Framework.Rectangle((int)Math.Floor((double)(Game1.getOldMouseX() + Game1.viewport.X) / 64.0) * 64 - Game1.viewport.X, (int)Math.Floor((double)(Game1.getOldMouseY() + Game1.viewport.Y) / 64.0) * 64 - Game1.viewport.Y, 64, 64), Color.Lime * 0.75f);
                foreach (Warp warp in (Game1.currentLocation.warps))
                    Game1.spriteBatch.Draw(Game1.fadeToBlackRect, new Microsoft.Xna.Framework.Rectangle(warp.X * 64 - Game1.viewport.X, warp.Y * 64 - Game1.viewport.Y, 64, 64), Color.Red * 0.75f);
            }
            Game1.mapDisplayDevice.BeginScene(Game1.spriteBatch);
            Game1.currentLocation.Map.GetLayer("Front").Draw(Game1.mapDisplayDevice, Game1.viewport, Location.Origin, false, 4);
            Game1.mapDisplayDevice.EndScene();
            //Game1.currentLocation.drawAboveFrontLayer(Game1.spriteBatch);
            //Game1.spriteBatch.End();
        }

        public void drawMapPart2()
        {
            //Game1.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
            applyCurrentShader();
            foreach (var layer in Game1.player.currentLocation.map.Layers)
            {
                //do front, and always front.
                if (layer.Id == "Back" || layer.Id == "Buildings" || layer.Id=="Paths") continue;
                //if (layer.Id != "Back" || layer.Id != "Buildings") continue;
                //Framework.Drawers.Layer.drawLayer(layer,Game1.mapDisplayDevice, Game1.viewport, new xTile.Dimensions.Location(0, 0), false, Game1.pixelZoom);
                layer.Draw(Game1.mapDisplayDevice, Game1.viewport, new xTile.Dimensions.Location(0, 0), false, Game1.pixelZoom);

            }
            //Game1.spriteBatch.End();
        }

        /// <summary>
        /// Returns the value of the data snagged by reflection.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="instance"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static object GetInstanceField(Type type, object instance, string fieldName)
        {
            BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic| BindingFlags.Static;
            FieldInfo field = type.GetField(fieldName, bindFlags);
            /*
            FieldInfo[] meh = type.GetFields(bindFlags);
            foreach(var v in meh)
            {
                if (v.Name == null)
                {
                    continue;
                }
                Monitor.Log(v.Name);
            }
            */
            return field.GetValue(instance);
        }

        public static void SetInstanceField(Type type, object instance, object value, string fieldName)
        {
            BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                | BindingFlags.Static;
            FieldInfo field = type.GetField(fieldName, bindFlags);
            field.SetValue(instance, value);
            return;
        }

        public void drawMouse()
        {

            if ((Game1.getOldMouseX() != 0 || Game1.getOldMouseY() != 0) && Game1.currentLocation != null)
            {
                if ((double)Game1.mouseCursorTransparency <= 0.0 || !Utility.canGrabSomethingFromHere(Game1.getOldMouseX() + Game1.viewport.X, Game1.getOldMouseY() + Game1.viewport.Y, Game1.player) || Game1.mouseCursor == 3)
                {
                    if (Game1.player.ActiveObject != null && Game1.mouseCursor != 3 && !Game1.eventUp)
                    {
                        if ((double)Game1.mouseCursorTransparency >= 0.0 || Game1.options.showPlacementTileForGamepad)
                        {
                            Game1.player.ActiveObject.drawPlacementBounds(Game1.spriteBatch, Game1.currentLocation);
                            if ((double)Game1.mouseCursorTransparency >= 0.0)
                            {
                                bool flag = Utility.playerCanPlaceItemHere(Game1.currentLocation, Game1.player.CurrentItem, Game1.getMouseX() + Game1.viewport.X, Game1.getMouseY() + Game1.viewport.Y, Game1.player) || Utility.isThereAnObjectHereWhichAcceptsThisItem(Game1.currentLocation, Game1.player.CurrentItem, Game1.getMouseX() + Game1.viewport.X, Game1.getMouseY() + Game1.viewport.Y) && Utility.withinRadiusOfPlayer(Game1.getMouseX() + Game1.viewport.X, Game1.getMouseY() + Game1.viewport.Y, 1, Game1.player);
                                Game1.player.CurrentItem.drawInMenu(Game1.spriteBatch, new Vector2((float)(Game1.getMouseX() + 16), (float)(Game1.getMouseY() + 16)), flag ? (float)((double)Game1.dialogueButtonScale / 75.0 + 1.0) : 1f, flag ? 1f : 0.5f, 0.999f);
                            }
                        }
                    }
                    else if (Game1.mouseCursor == 0 && Game1.isActionAtCurrentCursorTile)
                    {

                        Game1.mouseCursor = Game1.isInspectionAtCurrentCursorTile ? 5 : 2;
                    }
                }
                if (!Game1.options.hardwareCursor)
                {

                    Game1.mouseCursorTransparency = 0.0001f;
                    Game1.spriteBatch.Draw(Game1.mouseCursors, new Vector2((float)Game1.getMouseX(), (float)Game1.getMouseY()), new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, Game1.mouseCursor, 16, 16)), Color.White, 0.0f, Vector2.Zero, (float)(4.0 + (double)Game1.dialogueButtonScale / 150.0), SpriteEffects.None, 1f);

                }
                Game1.wasMouseVisibleThisFrame = (double)Game1.mouseCursorTransparency > 0.0;
            }

            /*
            Game1.mouseCursorTransparency = 0;
            if(Game1.mouseCursor!=5|| Game1.mouseCursor != 2)
            {
                Game1.mouseCursor = 0;
                Game1.spriteBatch.Draw(Game1.mouseCursors, new Vector2((float)Game1.getMousePosition().X, (float)Game1.getMousePosition().Y), new Microsoft.Xna.Framework.Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, Game1.mouseCursor, 16, 16)), Color.White, 0.0f, Vector2.Zero, (float)(4.0 + (double)Game1.dialogueButtonScale / 150.0), SpriteEffects.None, 1f);
            }
            */


        }

        protected void drawOverlays()
        {
            SpriteBatch spriteBatch = Game1.spriteBatch;
            // spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, (DepthStencilState)null, (RasterizerState)null);
            applyCurrentShader(true);
            foreach(var v in Game1.onScreenMenus)
            {
                v.draw(spriteBatch);
            }
            if ((Game1.displayHUD || Game1.eventUp) && (Game1.currentBillboard == 0 && Game1.gameMode == (byte)3) && (!Game1.freezeControls && !Game1.panMode))
                drawMouse();
            //spriteBatch.End();
        }
    }
}
