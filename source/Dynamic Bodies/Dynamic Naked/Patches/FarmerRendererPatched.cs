/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ribeena/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Color = Microsoft.Xna.Framework.Color;

using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Network;
using Netcode;

using DynamicBodies.Data;
using DynamicBodies.Framework;
using StardewValley.Objects;
using System.Drawing;

namespace DynamicBodies.Patches
{
    public class FarmerRendererPatched
    {

        public static RenderTarget2D renderTarget;
        public static RenderTarget2D _cachedRenderer;

        public FarmerRendererPatched(Harmony harmony)
		{

			//Intervene with the loading process so we can store separate textures per user
			//and add event listeners when FarmerRender is made
			harmony.Patch(
				original: AccessTools.Constructor(typeof(FarmerRenderer), new[] { typeof(string), typeof(Farmer) }),
				postfix: new HarmonyMethod(GetType(), nameof(post_FarmerRenderer_setup))
			);

            //Draw the Hair, beards and naked overlay
            harmony.Patch(
                original: AccessTools.Method(typeof(FarmerRenderer), nameof(FarmerRenderer.drawHairAndAccesories), new Type[] { typeof(SpriteBatch), typeof(int), typeof(Farmer), typeof(Vector2), typeof(Vector2), typeof(float), typeof(int), typeof(float), typeof(Color), typeof(float) }),
                prefix: new HarmonyMethod(GetType(), nameof(pre_DrawHairAndAccesories))
            );

            //Intervene when texture has changed to ensure lcmo is in place
            harmony.Patch(
                original: AccessTools.Method(typeof(FarmerRenderer), "textureChanged"),
                prefix: new HarmonyMethod(GetType(), nameof(pre_TextureChanged))
            );
            //Event for base texture changed
            harmony.Patch(
                original: AccessTools.Method(typeof(FarmerRenderer), nameof(FarmerRenderer.MarkSpriteDirty)),
                prefix: new HarmonyMethod(GetType(), nameof(pre_MarkSpriteDirty))
            );
            //Calculate any dirty flags for recaching the image
            harmony.Patch(
                original: AccessTools.Method(typeof(FarmerRenderer), nameof(FarmerRenderer.draw), new[] { typeof(SpriteBatch), typeof(FarmerSprite.AnimationFrame), typeof(int), typeof(Rectangle), typeof(Vector2), typeof(Vector2), typeof(float), typeof(int), typeof(Color), typeof(float), typeof(float), typeof(Farmer) }),
                prefix: new HarmonyMethod(GetType(), nameof(pre_Draw))
            );
            //Draw new mini portraits
            harmony.Patch(
                original: AccessTools.Method(typeof(FarmerRenderer), nameof(FarmerRenderer.drawMiniPortrat), new[] { typeof(SpriteBatch), typeof(Vector2), typeof(float), typeof(float), typeof(int), typeof(Farmer) }),
                prefix: new HarmonyMethod(GetType(), nameof(pre_drawMiniPortrat))
            );
        }

        //Adjust the base texture before rendering and add event listeners
        private static void post_FarmerRenderer_setup(FarmerRenderer __instance, ref LocalizedContentManager ___farmerTextureManager, ref NetString ___textureName, ref NetColor ___eyes, ref NetInt ___skin, ref NetInt ___shoes, ref NetInt ___shirt, ref NetInt ___pants, string textureName, Farmer farmer)
		{
			ModEntry.debugmsg($"LCMO in farmerRenderer constructor for {farmer.Name}/{farmer.UniqueMultiplayerID}", LogLevel.Debug);
			//Add a wrapping layer around the texture manager for the farmerrenderer
			LocalizedContentManagerOverride lcmo = new LocalizedContentManagerOverride(___farmerTextureManager.ServiceProvider, ___farmerTextureManager.RootDirectory);
			___farmerTextureManager = (LocalizedContentManager)lcmo.CreateTemporary(ModEntry.context, farmer);          
        }

        public static void FieldChanged(string field, Farmer who)
        {
            PlayerBaseExtended pbe = PlayerBaseExtended.Get(who);
            if(pbe == null)
            {
                try
                {
                    pbe = new PlayerBaseExtended(who);
                } catch (NullReferenceException e)
                {
                    return;//Abort
                }
            }
            if(field == "shirt")
            {
                if (ModEntry.dga != null) DGAItems.ShirtFixes(who.shirtItem.Get());
                pbe.dirtyLayers["shirt"] = true;
                //pbe.shirt = -1;//force shirt change
            }
            if(field == "shoes")
            {
                if (ModEntry.dga != null) DGAItems.ShoeFixes(who.boots.Get());
                pbe.dirtyLayers["shoes"] = true;
                pbe.shoes = -1;//force shoe change
            }
            if (field == "pants")
            {
                pbe.pants = -1;//force pants change
                //Update stored pants colour, darken it
                if(who.pantsItem.Value != null)
                {
                    if (ModEntry.dga != null) DGAItems.PantsFixes(who.pantsItem.Get());
                    pbe.paletteCache[16] = changeBrightness(who.pantsColor.Value, new Color(50,50,50), false).ToVector4();
                }
            }

            if (field == "hat")
            {
                if (ModEntry.dga != null) DGAItems.HatFixes(who.hat.Get());
            }

            pbe.dirty = true;
        }

        //Mark the Playerbase extended dirty if the standard is dirty
        public static void pre_MarkSpriteDirty(FarmerRenderer __instance, LocalizedContentManager ___farmerTextureManager)
        {
            LocalizedContentManagerOverride lcmo = ___farmerTextureManager as LocalizedContentManagerOverride;
            if (lcmo != null)
            {
                PlayerBaseExtended pbe = PlayerBaseExtended.Get(lcmo.who);
                if (pbe != null)
                {
                    pbe.UpdateTextures(lcmo.who);
                }
            }
        }

        //Replace texturechanged to use the cacheimage
        public static bool pre_TextureChanged(FarmerRenderer __instance, ref Texture2D ___baseTexture, NetString ___textureName, LocalizedContentManager ___farmerTextureManager)
        {
            ModEntry.debugmsg($"TextureChanged() called", LogLevel.Debug);
            LocalizedContentManagerOverride lcmo = ___farmerTextureManager as LocalizedContentManagerOverride;
            if (lcmo != null)
            {
                PlayerBaseExtended pbe = PlayerBaseExtended.Get(lcmo.who);
                ___baseTexture = pbe.cacheImage;
            }
            else
            {
                ___baseTexture = null;
                ModEntry.debugmsg($"LCMO wasn't loaded - repatching loading", LogLevel.Debug);
            }

            if (___baseTexture == null)
            {
                //Vanilla code fallback
                Texture2D source_texture = ___farmerTextureManager.Load<Texture2D>(___textureName.Value);
                ___baseTexture = new Texture2D(Game1.graphics.GraphicsDevice, source_texture.Width, source_texture.Height);
                Color[] data = new Color[source_texture.Width * source_texture.Height];
                source_texture.GetData(data, 0, data.Length);
                ___baseTexture.SetData(data);
            }

            return false;
        }

        //Replace the drawing of Farmer Renderer
        [HarmonyBefore(new string[] { "spacechase0.DynamicGameAssets" })]
        private static bool pre_Draw(FarmerRenderer __instance, ref Vector2 ___positionOffset, ref Vector2 ___rotationAdjustment, ref bool ____sickFrame, ref bool ____spriteDirty, ref bool ____eyesDirty, ref bool ____shirtDirty, ref bool ____pantsDirty, ref bool ____shoesDirty, ref bool ____skinDirty, ref bool ____baseTextureDirty, ref LocalizedContentManager ___farmerTextureManager, ref Dictionary<string, Dictionary<int, List<int>>> ____recolorOffsets, ref Texture2D ___baseTexture, ref string ___textureName, SpriteBatch b, FarmerSprite.AnimationFrame animationFrame, int currentFrame, Rectangle sourceRect, Vector2 position, Vector2 origin, float layerDepth, int facingDirection, Color overrideColor, float rotation, float scale, Farmer who)
        {
            if (who.isFakeEventActor && Game1.eventUp)
            {
                who = Game1.player;
            }

            //Calculate if any sprites farmer sprite sections need redrawing
            PlayerBaseExtended pbe = PlayerBaseExtended.Get(who);
            if (pbe == null)
            {
                pbe = new PlayerBaseExtended(who, __instance.textureName.Value);
                ModEntry.SetModDataDefaults(who);

                if (who.accessory.Value < 6 && who.accessory.Value > 0)
                {
                    who.modData["DB.beard"] = "Vanilla's Accessory " + (who.accessory.Value + 1).ToString();
                    who.accessory.Set(0);
                }
            }

            //Copy any dirty flags
            pbe.dirtyLayers["sprite"] = pbe.dirtyLayers["sprite"] || ____spriteDirty;
            pbe.dirtyLayers["baseTexture"] = pbe.dirtyLayers["baseTexture"] || ____baseTextureDirty;
            pbe.dirtyLayers["eyes"] = pbe.dirtyLayers["eyes"] || ____eyesDirty || pbe.dirtyLayers["baseTexture"];
            pbe.dirtyLayers["skin"] = pbe.dirtyLayers["skin"] || ____skinDirty || pbe.dirtyLayers["baseTexture"];
            pbe.dirtyLayers["shoes"] = pbe.dirtyLayers["skin"] || ____shoesDirty || pbe.dirtyLayers["baseTexture"];
            pbe.dirtyLayers["pants"] = pbe.dirtyLayers["pants"] || ____pantsDirty;
            pbe.dirtyLayers["shirt"] = pbe.dirtyLayers["shirt"] || ____shirtDirty || pbe.dirtyLayers["baseTexture"];
            //Wipe all dirty flags
            ____spriteDirty = false;
            ____baseTextureDirty = false;
            ____eyesDirty = false;
            ____skinDirty = false;
            ____shoesDirty = false;
            ____pantsDirty = false;
            ____shirtDirty = false;

            //Overriding the texture loading didn't apply during construction, make it happen
            if (!pbe.overrideCheck)
            {
                LocalizedContentManagerOverride lcmo = ___farmerTextureManager as LocalizedContentManagerOverride;
                if (lcmo == null)
                {
                    lcmo = new LocalizedContentManagerOverride(___farmerTextureManager.ServiceProvider, ___farmerTextureManager.RootDirectory);
                    ___farmerTextureManager = (LocalizedContentManager)lcmo.CreateTemporary(ModEntry.context, who);
                }
                pbe.overrideCheck = true;
            }

            //Never generated the texture... i guess
            if(___baseTexture == null)
            {
                ModEntry.debugmsg($"FarmerRenderer loaded a new sprite for {__instance.textureName}", LogLevel.Debug);
                ___baseTexture = GetFarmerBaseSprite(who, __instance.textureName);
            }

            //Flat the positions to whole pixels
            position.X = (int)position.X;
            position.Y = (int)position.Y;
            ___positionOffset.Y = (int)___positionOffset.X;
            ___positionOffset.X = (int)___positionOffset.Y;

            //TODO move this dirty texture check to not run so often
            //pbe.UpdateTextures(who);

            if (pbe.dirty)
            {
                //Check if the texture needs updating
                if (pbe.shirt != who.GetShirtIndex() || pbe.dirtyLayers["shirt"])
                {
                    pbe.shirt = who.GetShirtIndex();
                    if (who.shirtItem.Value == null)
                    {
                        pbe.sleeveLength = "Sleeveless";
                        if(pbe.nakedUpper.CheckForOption("sleeve short"))
                        {
                            pbe.sleeveLength = "Short";
                        }
                        if (pbe.nakedUpper.CheckForOption("sleeve"))
                        {
                            pbe.sleeveLength = "Normal";
                        }
                        if (pbe.nakedUpper.CheckForOption("sleeve long"))
                        {
                            pbe.sleeveLength = "Long";
                        }
                    }
                    else
                    {
                        if (who.bathingClothes.Value && who.modData.ContainsKey("DB.bathers") && who.modData["DB.bathers"] == "false")
                        {

                            pbe.sleeveLength = "Sleeveless";
                            if (pbe.nakedUpper.CheckForOption("sleeve short"))
                            {
                                pbe.sleeveLength = "Short";
                            }
                            if (pbe.nakedUpper.CheckForOption("sleeve"))
                            {
                                pbe.sleeveLength = "Normal";
                            }
                            if (pbe.nakedUpper.CheckForOption("sleeve long"))
                            {
                                pbe.sleeveLength = "Long";
                            }

                        }
                        else
                        {
                            pbe.sleeveLength = ModEntry.AssignShirtLength(who.shirtItem.Value as Clothing, who.IsMale);
                        }
                    }

                    if (who.shirtItem.Value != null)
                    {
                        if (who.shirtItem.Value.GetOtherData().Contains("DB.PantsOverlay"))
                        {
                            foreach (ShirtOverlay shirtOverlay in ModEntry.shirtOverlays)
                            {
                                if (shirtOverlay.overlays.ContainsKey(who.shirtItem.Value.Name))
                                {
                                    ModEntry.debugmsg($"Shirt Overlay Index [{shirtOverlay.GetIndex(who.shirtItem.Value.Name, who.isMale)}] override for [{who.GetShirtIndex()}]", LogLevel.Debug);
                                    pbe.shirtOverlayIndex = shirtOverlay.GetIndex(who.shirtItem.Value.Name, who.isMale);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            pbe.shirtOverlayIndex = -1;
                        }
                    }
                    ____shirtDirty = true;
                    pbe.dirty = true;
                }

                if (pbe.shoes != who.shoes.Value)
                {
                    pbe.shoes = who.shoes.Value;
                    pbe.shoeStyle = "Normal";
                    Boots equippedBoots = (Boots)who.boots;
                    if (pbe.shoes == 12 || equippedBoots == null)
                    {
                        pbe.shoeStyle = "None";
                    }
                    ____spriteDirty = true;
                    pbe.dirty = true;
                }

                //Wipe hair calculations
                pbe.ResetHairTextures();
                //Wipe naked overlays
                pbe.nakedLower.texture = null;
                pbe.nakedUpper.texture = null;
                //recalculate the recolouring parts
                __instance.MarkSpriteDirty();
            }

            //Draw the character
            
            //Replacement to the default recolouring cache system of FarmerRenderer using a shader to palette swap
            ExecuteRecolorActionsOnBaseSprite(pbe, who);

            //All fixes of rendering should be done
            pbe.dirty = false;

            if (FarmerRenderer.isDrawingForUI)
            {
                DrawHairBackUI(pbe, ___positionOffset, b, facingDirection, who, position, origin, scale, currentFrame, rotation, overrideColor, layerDepth, ((!Game1.isUsingBackToFrontSorting) ? 1 : (-1)));
            }

            AdjustedVanillaMethods.drawBase(__instance, ref ___rotationAdjustment, ref ___positionOffset, ref pbe.cacheImage, b, animationFrame, currentFrame, ref sourceRect, ref position, origin, layerDepth, facingDirection, overrideColor, rotation, scale, who);
            
            if (pbe.arm.textures.ContainsKey("cache") && pbe.arm.textures["cache"] != null)
            {
                AdjustedVanillaMethods.drawArmBack(__instance, ref ___rotationAdjustment, ___positionOffset, pbe.arm.textures["cache"], b, animationFrame, currentFrame, sourceRect, position, origin, layerDepth, facingDirection, overrideColor, rotation, scale, who);
            }
            
            ///////////////////////////////
            /// Setup a new overlay drawing for upper body
            //no shirt
            bool drawNakedOverlay = who.shirtItem.Value == null;
            if (!drawNakedOverlay && who.bathingClothes.Value)
            {
                if (who.modData.ContainsKey("DB.bathers"))
                {
                    drawNakedOverlay = who.modData["DB.bathers"] == "false";
                }
            }

            Texture2D nakedUpperTexture = pbe.GetNakedUpperTexture();
            if (drawNakedOverlay)
            {
                if (nakedUpperTexture != null)
                {
                    Vector2 animoffset = Vector2.Zero;

                    Rectangle overlay_rect = sourceRect;
                    bool flipped = animationFrame.flip;
                    if (!pbe.nakedUpper.fullAnimation)
                    {
                        //Simple one frame per direction like shirts
                        overlay_rect.X = 0;
                        switch (who.facingDirection.Value)
                        {
                            case 0:
                                overlay_rect.Y = 2*sourceRect.Height;
                                break;
                            case 1:
                                overlay_rect.Y = sourceRect.Height;
                                break;
                            case 2:
                                overlay_rect.Y = 0;
                                break;
                            case 3:
                                overlay_rect.Y = 3*sourceRect.Height;
                                flipped = false;
                                break;
                        }
                        animoffset = new Vector2((float)(FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4), (float)(FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4) + (float)(int)__instance.heightOffset.Value * scale);

                    }
                    float layerOffset = ((who.FarmerSprite.CurrentAnimationFrame.frame == 5) ? 0.00096f : 9.6E-08f);

                    if (!FarmerRenderer.isDrawingForUI && (bool)who.swimming.Value)
                    {
                        //don't draw it in the water
                        overlay_rect.Height = 16;
                        b.Draw(nakedUpperTexture, position + origin + ___positionOffset + animoffset, overlay_rect, overrideColor, rotation, origin, 4f * scale, flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth + layerOffset);
                    }
                    else
                    {

                        if (FarmerRenderer.isDrawingForUI)
                        {
                            //Change the frame for UI version
                            overlay_rect.Y = 0;
                        }
                        b.Draw(nakedUpperTexture, position + origin + ___positionOffset + animoffset, overlay_rect, overrideColor, rotation, origin, 4f * scale, flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth + layerOffset);
                        
                    }
                }
            }


            bool drawPants = true;
            Texture2D nakedLowerTexture = pbe.GetNakedLowerTexture();
            Texture2D pantsTexture = FarmerRenderer.pantsTexture;
            if (pbe.body.textures.ContainsKey("pants") && pbe.body.textures["pants"] != null)
            {
                pantsTexture = pbe.body.textures["pants"];
            }
            if (who.GetPantsIndex() == 14 || who.pantsItem.Value == null)
            {
                
                if (nakedLowerTexture != null)
                {
                    drawPants = false;
                }
            }
            if (drawPants && who.bathingClothes.Value)
            {
                if (who.modData.ContainsKey("DB.bathers"))
                {
                    drawPants = who.modData["DB.bathers"] == "true";
                }
            }
            bool pants_not_drawn = true;
            if (ModEntry.dga != null)
            {
                Texture2D pantsTextureDGA = GetFarmerPants(who.pantsItem.Get(), pbe.body.name);
                if (pantsTextureDGA != null)
                {
                    pants_not_drawn = false;
                    int pantIndex = DGAItems.GetDGAPants(ModEntry.dga.GetDGAItemId(who.pantsItem.Get())).GetIndex(pbe.body.name);

                    AdjustedVanillaMethods.drawPants(pantsTextureDGA, pantIndex, ___positionOffset, b, animationFrame, currentFrame, sourceRect, position, origin, layerDepth, facingDirection, overrideColor, rotation, scale, who);
                }
            }

            if (drawPants && pants_not_drawn) AdjustedVanillaMethods.drawPants(pantsTexture, AdjustedVanillaMethods.ClampPants(who), ___positionOffset, b, animationFrame, currentFrame, sourceRect, position, origin, layerDepth, facingDirection, overrideColor, rotation, scale, who);
            if (nakedLowerTexture != null && pbe.nakedLower.CheckForOption("below accessories")) DrawLowerNaked(__instance, ___positionOffset, ___rotationAdjustment, ___baseTexture, animationFrame, sourceRect, b, facingDirection, who, position, origin, scale, currentFrame, rotation, overrideColor, layerDepth, 9.2E-08f, 9.2E-08f);
            AdjustedVanillaMethods.drawEyes(__instance, ref ___rotationAdjustment, ref ___positionOffset, ref pbe.cacheImage, b, animationFrame, currentFrame, sourceRect, position, origin, layerDepth, facingDirection, overrideColor, rotation, scale, who);
            //ensure subsequent layers are above the eyes
            layerDepth += 1.2E-07f;
            __instance.drawHairAndAccesories(b, facingDirection, who, position, origin, scale, currentFrame, rotation, overrideColor, layerDepth);
            AdjustedVanillaMethods.drawArms(__instance, ref ___rotationAdjustment, ref ___positionOffset, ref pbe.cacheImage, b, animationFrame, currentFrame, sourceRect, position, origin, layerDepth, facingDirection, overrideColor, rotation, scale, who, (pbe.arm.textures.ContainsKey("cache") && pbe.arm.textures["cache"] != null));

            //Draw trinket 5
            if (pbe.trinkets[4].option != "Default")
            {
                DrawTrinket(4, __instance, pbe, ___positionOffset, b, facingDirection, who, position, origin, scale, currentFrame, rotation, overrideColor, layerDepth + 5E-05f, ((!Game1.isUsingBackToFrontSorting) ? 1 : (-1)));
            }
            //prevent further rendering
            return false;
        }


        internal static void ExecuteRecolorActionsOnBaseSprite(PlayerBaseExtended pbe, Farmer who)
        {
            bool updatePalette = false;
            if (pbe.dirtyLayers["sprite"])
            {
                updatePalette = true;
                pbe.dirtyLayers["sprite"] = false;
                if (pbe.dirtyLayers["baseTexture"])
                {

                    //this.textureChanged();
                    pbe.dirtyLayers["eyes"] = true;
                    pbe.dirtyLayers["shoes"] = true;
                    //pbe.dirtyLayers["pants"] = true;//Pants aren't in the base sprite..?
                    pbe.dirtyLayers["skin"] = true;
                    pbe.dirtyLayers["shirt"] = true;


                    //Replacement to farmerTextureManager.Load<Texture2D>
                    pbe.sourceImage = GetFarmerBaseSprite(who);
                    ModEntry.debugmsg($"Got a base texture: {pbe.sourceImage != null}", LogLevel.Debug);

                    pbe.dirtyLayers["baseTexture"] = false;

                }
            }

            updatePalette = updatePalette || pbe.dirtyLayers["eyes"] || pbe.dirtyLayers["skin"] || pbe.dirtyLayers["shoes"]
                            || pbe.dirtyLayers["shirt"] || pbe.dirtyLayers["bodyHair"];

            if (updatePalette) UpdatePalette(pbe, who);
        }

        internal static void UpdatePalette(PlayerBaseExtended pbe, Farmer who)
        {
            //Change the pixel colours on the cached image
            foreach (String layer in pbe.dirtyLayers.Keys)
            {
                //Update the colour cache of each dirty layer
                if (pbe.dirtyLayers[layer])
                {
                    switch (layer)
                    {
                        case "eyes":
                            UpdateEyePalette(who, pbe);
                            pbe.dirtyLayers[layer] = false;
                            break;
                        case "shoes":
                            UpdateShoePalette(who, pbe);
                            pbe.dirtyLayers[layer] = false;
                            break;
                        case "skin":
                            UpdateSkinPalette(who, pbe);
                            pbe.dirtyLayers[layer] = false;
                            break;
                        case "shirt":
                            UpdateShirtPalette(who, pbe);
                            pbe.dirtyLayers[layer] = false;
                            break;
                    }
                    
                }
            }

            /*
             //https://github.com/Floogen/DynamicReflections/blob/development/DynamicReflections/Framework/Utilities/SpriteBatchToolkit.cs

            SpriteBatchToolkit.StartRendering(DynamicReflections.playerWaterReflectionRender);

                var currentRenderer = Game1.graphics.GraphicsDevice.GetRenderTargets();
                if (currentRenderer is not null && currentRenderer.Length > 0 && currentRenderer[0].RenderTarget is not null)
                {
                    _cachedRenderer = currentRenderer[0].RenderTarget as RenderTarget2D;
                }

                Game1.graphics.GraphicsDevice.SetRenderTarget(renderTarget2D);

            // Draw the scene
            Game1.graphics.GraphicsDevice.Clear(Color.Transparent);

            DrawReflectionViaMatrix();

            // Drop the render target
            SpriteBatchToolkit.StopRendering();
                Game1.graphics.GraphicsDevice.SetRenderTarget(_cachedRenderer);
                _cachedRenderer = null;

            Game1.graphics.GraphicsDevice.Clear(Game1.bgColor);


            if (renderTarget2D is not null)
            {
                renderTarget2D.Dispose();
            }

            var height = Game1.graphics.GraphicsDevice.PresentationParameters.BackBufferHeight;
            var width = Game1.graphics.GraphicsDevice.PresentationParameters.BackBufferWidth;
            if (shouldUseScreenDimensions is true && Game1.game1 is not null && Game1.game1.screen is not null)
            {
                height = Game1.game1.screen.Height;
                width = Game1.game1.screen.Width;
            }

            renderTarget2D = new RenderTarget2D(
                Game1.graphics.GraphicsDevice,
                width,
                height,
                false,
                Game1.graphics.GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.None);

            */


            if (pbe.sourceImage != null)
            {
                //Use a pixel shader to handle the recolouring
                //set up the palette render
                ModEntry.paletteSwap.Parameters["xTargetPalette"].SetValue(pbe.paletteCache);

                RenderTarget2D renderTarget = new RenderTarget2D(Game1.graphics.GraphicsDevice, pbe.sourceImage.Width, pbe.sourceImage.Height, false, Game1.graphics.GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.None);
                //Store current render targets
                RenderTargetBinding[] currentRenderTargets = Game1.graphics.GraphicsDevice.GetRenderTargets();

                if (currentRenderTargets is not null && currentRenderTargets.Length > 0 && currentRenderTargets[0].RenderTarget is not null)
                {
                    _cachedRenderer = currentRenderTargets[0].RenderTarget as RenderTarget2D;
                    ModEntry.debugmsg($"Current render targets are {currentRenderTargets.Length}", LogLevel.Debug);
                }

                Game1.graphics.GraphicsDevice.SetRenderTarget(renderTarget);

                Game1.graphics.GraphicsDevice.Clear(Color.Transparent);

                using (SpriteBatch sb = new SpriteBatch(renderTarget.GraphicsDevice))
                {
                    sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, effect: ModEntry.paletteSwap);
                    sb.Draw(pbe.sourceImage, new Rectangle(0, 0, pbe.sourceImage.Width, pbe.sourceImage.Height), Color.White);
                    sb.End();
                }

                Color[] pixel_data = new Color[renderTarget.Width * renderTarget.Height];
                renderTarget.GetData(pixel_data);
                pbe.cacheImage.SetData(pixel_data);

                if (renderTarget is not null)
                {
                    renderTarget.Dispose();
                }

                //return current render target
                Game1.graphics.GraphicsDevice.SetRenderTarget(_cachedRenderer);
                _cachedRenderer = null;

                //Overlay the bodyhair onto the base skin
                Texture2D bodyHairText = null;

                if (pbe.bodyHair.option != "Default")
                {
                    bodyHairText = pbe.GetBodyHairTexture(who);
                }
                pbe.dirtyLayers["bodyHair"] = false;

                if (bodyHairText != null)
                {
                    ModEntry.debugmsg("UpdatePalette: Drawing bodyhair", LogLevel.Debug);
                    IAssetDataForImage editor = ModEntry.context.Helper.ModContent.GetPatchHelper(pbe.cacheImage).AsImage();
                    editor.PatchImage(bodyHairText, new Rectangle(0, 0, bodyHairText.Width, bodyHairText.Height), targetArea: new Rectangle(0, 0, bodyHairText.Width, bodyHairText.Height), PatchMode.Overlay);
                }

                if (pbe.arm.textures.ContainsKey("source") && pbe.arm.textures["source"] != null)
                {
                    //Render the back arms
                    pbe.arm.textures["cache"] = PlayerBaseExtended.ApplyPaletteColors(pbe.arm.textures["source"]);
                }

            }

            

        }

        public static Texture2D GetFarmerBaseSprite(Farmer who, string texture = "")
        {
            Texture2D bodyText2D = null;
            bool returnNew = texture == "";
            //Start modifying the base texture
            IAssetDataForImage editor = null;

            //Fix up the farmer base with options
            if ((texture.Length > 0 && texture.StartsWith("Characters\\Farmer\\farmer_") || returnNew))
            {
                string gender = "";
                if (!who.IsMale) { gender = "f_"; }

                //monitor.Log($"Edit [{pbeKeyPair.Key}] {pbeKeyPair.Value.baseStyle} image through Edit<t>", LogLevel.Debug);
                PlayerBaseExtended pbe = PlayerBaseExtended.Get(who);
                if (pbe == null)
                {
                    pbe = new PlayerBaseExtended(who, texture);
                    ModEntry.SetModDataDefaults(who);
                    pbe.dirty = true;

                    if (who.accessory.Value < 6 && who.accessory.Value > 0)
                    {
                        who.modData["DB.beard"] = "Vanilla's Accessory " + (who.accessory.Value + 1).ToString();
                        who.accessory.Set(0);
                    }
                }

                string bald = "";
                if (who.IsBaldHairStyle(who.hair.Value))
                {
                    bald = "_bald";
                }

                //Base texture needs to be redone
                if (pbe.dirtyLayers["baseTexture"])
                {
                    //Load the base texture from this mod
                    if (pbe.body.option == "Default")
                    {
                        bodyText2D = ModEntry.context.Helper.ModContent.Load<Texture2D>($"assets\\Character\\{gender}farmer_base.png");
                    }
                    else
                    {
                        try
                        {
                            //Otherwise load it from a content pack
                            bodyText2D = pbe.body.provider.ModContent.Load<Texture2D>($"assets\\bodies\\{gender}{pbe.body.file}.png");
                            if (pbe.body.provider.HasFile($"assets\\bodies\\{pbe.body.file}_shirts.png"))
                            {
                                pbe.body.textures["shirt"] = pbe.body.provider.ModContent.Load<Texture2D>($"assets\\bodies\\{pbe.body.file}_shirts.png");
                            } else
                            {
                                pbe.body.textures["shirt"] = null;
                            }
                            if (pbe.body.provider.HasFile($"assets\\bodies\\{pbe.body.file}_shirts_overlay.png"))
                            {
                                pbe.body.textures["shirt_overlay"] = pbe.body.provider.ModContent.Load<Texture2D>($"assets\\bodies\\{pbe.body.file}_shirts_overlay.png");
                            }
                            else
                            {
                                pbe.body.textures["shirt_overlay"] = null;
                            }
                            if (pbe.body.provider.HasFile($"assets\\bodies\\{pbe.body.file}_pants.png"))
                            {
                                pbe.body.textures["pants"] = pbe.body.provider.ModContent.Load<Texture2D>($"assets\\bodies\\{pbe.body.file}_pants.png");
                            }
                            else
                            {
                                pbe.body.textures["pants"] = null;
                            }
                        } catch (NullReferenceException e)
                        {
                            //Fallback
                            bodyText2D = ModEntry.context.Helper.ModContent.Load<Texture2D>($"assets\\Character\\{gender}farmer_base.png");
                        }
                    }

                    editor = ModEntry.context.Helper.ModContent.GetPatchHelper(bodyText2D).AsImage();

                    if (pbe.dirtyLayers["baseTexture"]) ModEntry.debugmsg("base was dirty", LogLevel.Debug);

                    IRawTextureData faceText2D;
                    if (pbe.face.option == "Default")
                    {
                        faceText2D = ModEntry.context.Helper.ModContent.Load<IRawTextureData>($"assets\\Character\\{gender}face{bald}.png");
                    }
                    else
                    {
                        faceText2D = pbe.face.provider.ModContent.Load<IRawTextureData>($"assets\\faces\\{pbe.face.file}{bald}.png");
                    }
                    editor.PatchImage(faceText2D, new Rectangle(0, 0, 96, faceText2D.Height), targetArea: new Rectangle(0, 0, 96, faceText2D.Height), PatchMode.Overlay);
                    if(faceText2D.Width > 96)
                    {
                        //Add custom blink frames
                        editor.PatchImage(faceText2D, new Rectangle(96, 0, 32, 24), targetArea: new Rectangle(256, 0, 32, 24), PatchMode.Replace);
                    }

                    IRawTextureData shoes;
                    if (pbe.body.option != "Default" && pbe.body.provider.HasFile($"assets\\bodies\\{pbe.body.file}_feet.png"))
                    {
                        shoes = pbe.face.provider.ModContent.Load<IRawTextureData>($"assets\\bodies\\{pbe.body.file}_feet.png");
                    }
                    else
                    {
                        shoes = ModEntry.context.Helper.ModContent.Load<IRawTextureData>($"assets\\Character\\feet.png");
                    }
                    
                    if (pbe.shoeStyle == "None")
                    {
                        
                        ModEntry.debugmsg($"Drawing feet.", LogLevel.Debug);
                    }
                    else
                    {
                        Boots equippedBoots = (Boots)who.boots;
                        if (equippedBoots == null)
                        {
                            shoes = ModEntry.context.Helper.ModContent.Load<IRawTextureData>($"assets\\Character\\feet.png");
                            ModEntry.debugmsg($"Default feet as nothing equipped found.", LogLevel.Debug);
                        }
                        else
                        {
                            if (ModEntry.shoeOverrides.ContainsKey(equippedBoots.Name))
                            {
                                shoes = ModEntry.shoeOverrides[equippedBoots.Name].contentPack.ModContent.Load<IRawTextureData>(ModEntry.shoeOverrides[equippedBoots.Name].file);
                                ModEntry.debugmsg($"Override specific shoe for [{equippedBoots.Name}].", LogLevel.Debug);
                            }
                            else
                            {
                                List<string> roughMatches = ModEntry.shoeOverrides.Keys.Where(key => equippedBoots.Name.StartsWith(key)).ToList();
                                if (roughMatches.Count > 0)
                                {
                                    shoes = ModEntry.shoeOverrides[roughMatches[0]].contentPack.ModContent.Load<IRawTextureData>(ModEntry.shoeOverrides[roughMatches[0]].file);
                                    ModEntry.debugmsg($"Override shoes group for [{equippedBoots.Name}].", LogLevel.Debug);
                                }
                                else
                                {
                                    shoes = ModEntry.context.Helper.ModContent.Load<IRawTextureData>($"assets\\Character\\shoes_Normal.png");
                                    ModEntry.debugmsg($"Default shoes for [{equippedBoots.Name}].", LogLevel.Debug);
                                }
                            }
                        }

                    }

                    editor.PatchImage(shoes, new Rectangle(0, 0, shoes.Width, shoes.Height), targetArea: new Rectangle(0, 0, shoes.Width, shoes.Height), PatchMode.Overlay);
                }

                if (pbe.dirtyLayers["arm"] || pbe.dirtyLayers["baseTexture"])
                {
                    if (editor == null)
                    {
                        editor = ModEntry.context.Helper.ModContent.GetPatchHelper(pbe.sourceImage).AsImage();
                        pbe.dirtyLayers["sprite"] = true;
                    }

                    IRawTextureData armsText2D;
                    if (pbe.arm.option == "Default")
                    {
                        armsText2D = ModEntry.context.Helper.ModContent.Load<IRawTextureData>($"assets\\Character\\{gender}arm_{pbe.sleeveLength}.png");
                        pbe.arm.textures["source"] = ModEntry.context.Helper.ModContent.Load<Texture2D>($"assets\\Character\\{gender}arm_{pbe.sleeveLength}_back.png");
                    }
                    else
                    {
                        bool customarm = false;
                        //Patch the arms
                        try
                        {
                            armsText2D = pbe.arm.provider.ModContent.Load<IRawTextureData>($"assets\\arms\\{pbe.arm.file}_{pbe.sleeveLength}.png");
                            customarm = true;
                        }
                        catch (NullReferenceException e)
                        {
                            //Fallback
                            armsText2D = ModEntry.context.Helper.ModContent.Load<IRawTextureData>($"assets\\Character\\{gender}arm_{pbe.sleeveLength}.png");

                            pbe.arm.textures["source"] = ModEntry.context.Helper.ModContent.Load<Texture2D>($"assets\\Character\\{gender}arm_{pbe.sleeveLength}_back.png");
                        }

                        if(customarm)
                        {
                            //Check for back arm from mod content
                            try
                            {
                                if (pbe.arm.provider.HasFile($"assets\\arms\\{pbe.arm.file}_{pbe.sleeveLength}_back.png"))
                                {
                                    Texture2D armsTextBack = pbe.arm.provider.ModContent.Load<Texture2D>($"assets\\arms\\{pbe.arm.file}_{pbe.sleeveLength}_back.png");
                                    pbe.arm.textures["source"] = armsTextBack;
                                }
                                else
                                {
                                    pbe.arm.textures["source"] = null;
                                }
                            }
                            catch (NullReferenceException e)
                            {
                                //Fallback
                                pbe.arm.textures["source"] = null;
                            }
                        }
                    }
                    
                    //Top row
                    editor.PatchImage(armsText2D, new Rectangle(0, 0, armsText2D.Width - 32, 32), targetArea: new Rectangle(96, 0, armsText2D.Width - 32, 32), PatchMode.Replace);
                    //remainder
                    editor.PatchImage(armsText2D, new Rectangle(0, 32, armsText2D.Width, armsText2D.Height - 32), targetArea: new Rectangle(96, 32, armsText2D.Width, armsText2D.Height - 32), PatchMode.Replace);
                }

                if (pbe.dirtyLayers["baseTexture"] && pbe.nose.option != "Default")
                {
                    IRawTextureData noseText2D = pbe.nose.provider.ModContent.Load<IRawTextureData>($"assets\\nose\\{pbe.nose.file}.png");
                    editor.PatchImage(noseText2D, new Rectangle(0, 0, noseText2D.Width, noseText2D.Height), targetArea: new Rectangle(0, 0, noseText2D.Width, noseText2D.Height), PatchMode.Overlay);
                }

                if (pbe.dirtyLayers["baseTexture"] && pbe.eyes.option != "Default")
                {
                    IRawTextureData eyesText2D = pbe.eyes.provider.ModContent.Load<IRawTextureData>($"assets\\eyes\\{pbe.eyes.file}.png");
                    editor.PatchImage(eyesText2D, new Rectangle(0, 0, 96, eyesText2D.Height), targetArea: new Rectangle(0, 0, 96, eyesText2D.Height), PatchMode.Overlay);
                    if (eyesText2D.Width > 96)
                    {
                        //Add custom blink frames
                        editor.PatchImage(eyesText2D, new Rectangle(96, 0, 32, 24), targetArea: new Rectangle(256, 0, 32, 24), PatchMode.Replace);
                    }
                }

                if (pbe.dirtyLayers["baseTexture"] && pbe.ears.option != "Default")
                {
                    IRawTextureData earsText2D = pbe.ears.provider.ModContent.Load<IRawTextureData>($"assets\\ears\\{pbe.ears.file}.png");
                    editor.PatchImage(earsText2D, new Rectangle(0, 0, earsText2D.Width, earsText2D.Height), targetArea: new Rectangle(0, 0, earsText2D.Width, earsText2D.Height), PatchMode.Overlay);
                }

                //Needs redrawing
                if (pbe.dirtyLayers["baseTexture"] || pbe.dirtyLayers["sprite"] || pbe.cacheImage == null)
                {
                    //Store the updated version
                    pbe.cacheImage = null;

                    pbe.cacheImage = new Texture2D(Game1.graphics.GraphicsDevice, bodyText2D.Width, bodyText2D.Height);
                    Color[] data = new Color[bodyText2D.Width * bodyText2D.Height];
                    bodyText2D.GetData(data, 0, data.Length);
                    pbe.cacheImage.SetData(data);

                    //Render any extended colours on the base
                    //pbe.cacheImage = PlayerBaseExtended.ApplyExtendedSkinColor(who.skin.Value, pbe.cacheImage);

                    pbe.dirtyLayers["sprite"] = false;
                }

                if (pbe.dirtyLayers["trinkets"])
                {
                    for (int i = 0; i < 5; i++)
                    {
                        if (pbe.trinkets[i].option != "Default")
                        {
                            pbe.trinkets[i].texture = pbe.GetTrinketTexture(who, i);
                        }
                    }
                    pbe.dirtyLayers["trinkets"] = false;
                }

                if (!returnNew)
                {
                    //Return the cached image
                    return pbe.cacheImage;
                }

            }



            return bodyText2D;
        }


        public static Color changeBrightness(Color c, Color amount, bool lighter = true)
        {
            int adjust = lighter ? 1 : -1;
            c.R = (byte)Math.Min(255, Math.Max(0, c.R + amount.R * adjust));
            c.G = (byte)Math.Min(255, Math.Max(0, c.G + amount.G * adjust));
            c.B = (byte)Math.Min(255, Math.Max(0, c.B + amount.B * adjust));
            return c;
        }

        private static void UpdateEyePalette(Farmer who, PlayerBaseExtended pbe)
        {
            Color lightest_color = who.newEyeColor.Value;
            if (lightest_color.A < byte.MaxValue) lightest_color.A = byte.MaxValue;

            //Adjust dark eye colour by the difference between the standard colour
            Color darken = new Color(59, 25, 9);

            Color darker_color = changeBrightness(lightest_color, darken, false);
            if (lightest_color.Equals(darker_color))
            {
                changeBrightness(lightest_color, darken, true);
            }
            pbe.paletteCache[20] = lightest_color.ToVector4();
            pbe.paletteCache[21] = darker_color.ToVector4();

            Color lightest_r_color = PlayerBaseExtended.GetColorSetting(who, "eyeColorR");
            if (who.modData.ContainsKey("DB.eyeColorR"))
            {
                //Allow for two eye colours
                Color darker_r_color = changeBrightness(lightest_r_color, darken, false);
                if (lightest_r_color.Equals(darker_r_color))
                {
                    changeBrightness(lightest_r_color, darken, true);
                }
                pbe.paletteCache[23] = lightest_r_color.ToVector4();
                pbe.paletteCache[24] = darker_r_color.ToVector4();
            }
            else
            {
                pbe.paletteCache[23] = lightest_color.ToVector4();
                pbe.paletteCache[24] = darker_color.ToVector4();
            }

            //Allow for sclera colours
            Color lightest_s_color = PlayerBaseExtended.GetColorSetting(who, "eyeColorS");
            if (!lightest_s_color.Equals(Color.Transparent))
            {
                //Difference in the white/grey colour
                darken = new Color(65, 85, 84);

                Color darker_s_color = changeBrightness(lightest_s_color, darken, false);
                if (lightest_s_color.Equals(darker_s_color))
                {
                    lightest_s_color = changeBrightness(darker_s_color, darken, true);
                }
                pbe.paletteCache[18] = lightest_s_color.ToVector4();
                pbe.paletteCache[19] = darker_s_color.ToVector4();
            }

            //Allow for ash colours
            Color lash_color = PlayerBaseExtended.GetColorSetting(who, "lash");
            if (lash_color.Equals(Color.Transparent))
            {
                pbe.paletteCache[17] = new Color(15, 10, 8).ToVector4();
            } else
            {
                pbe.paletteCache[17] = lash_color.ToVector4();
            }

        }

        private static void UpdateSkinPalette(Farmer who, PlayerBaseExtended pbe)
        {
            //Calculate the skin colours
            int which = who.skin.Value;
            Texture2D skinColors = Game1.content.Load<Texture2D>("Characters/Farmer/skinColors");
            Texture2D glandColors = Game1.content.Load<Texture2D>("Mods/ribeena.dynamicbodies/assets/Character/extendedSkinColors.png");

            Color[] skinColorsData = new Color[skinColors.Width * skinColors.Height];
            if (which < 0) which = skinColors.Height - 1;
            if (which > skinColors.Height - 1) which = 0;
            skinColors.GetData(skinColorsData);

            Color[] glandColorsData = new Color[glandColors.Width * glandColors.Height];
            if (which < 0) which = glandColors.Height - 1;
            if (which > glandColors.Height - 1) which = 0;
            glandColors.GetData(glandColorsData);

            //Store what the colours are

            if (skinColors.Width == 3)
            {
                pbe.paletteCache[4] = skinColorsData[which * 3 % (skinColors.Height * 3)].ToVector4();//Dark
                pbe.paletteCache[5] = skinColorsData[which * 3 % (skinColors.Height * 3) + 1].ToVector4();//Medium
                pbe.paletteCache[6] = skinColorsData[which * 3 % (skinColors.Height * 3) + 2].ToVector4();//Light
                //Lerp the other colours
                pbe.paletteCache[7] = Color.Lerp(skinColorsData[which * 3 % (skinColors.Height * 3)], skinColorsData[which * 3 % (skinColors.Height * 3) + 1], 0.5f).ToVector4();
                pbe.paletteCache[8] = Color.Lerp(skinColorsData[which * 3 % (skinColors.Height * 3) + 1], skinColorsData[which * 3 % (skinColors.Height * 3) + 2], 0.5f).ToVector4();
            }
            else if (skinColors.Width == 5)
            {
                //Oooo someone went all in with a 5 width skin
                pbe.paletteCache[4] = skinColorsData[which * 5 % (skinColors.Height * 5)].ToVector4();
                pbe.paletteCache[5] = skinColorsData[which * 5 % (skinColors.Height * 5) + 1].ToVector4();
                pbe.paletteCache[6] = skinColorsData[which * 5 % (skinColors.Height * 5) + 2].ToVector4();
                pbe.paletteCache[7] = skinColorsData[which * 5 % (skinColors.Height * 5) + 3].ToVector4();
                pbe.paletteCache[8] = skinColorsData[which * 5 % (skinColors.Height * 5) + 4].ToVector4();
            }
            if (glandColors.Width == 2)
            {
                //Original format compatibility
                pbe.paletteCache[9] = glandColorsData[which * 2 % (glandColors.Height * 2)].ToVector4();
                pbe.paletteCache[11] = glandColorsData[which * 2 % (glandColors.Height * 2) + 1].ToVector4();
                //Lerp the other colour
                pbe.paletteCache[10] = Color.Lerp(glandColorsData[which * 2 % (glandColors.Height * 2)], glandColorsData[which * 2 % (glandColors.Height * 2) + 1], 0.5f).ToVector4();
            }
            else if (glandColors.Width == 3)
            {
                pbe.paletteCache[9] = glandColorsData[which * 3 % (glandColors.Height * 3)].ToVector4();
                pbe.paletteCache[10] = glandColorsData[which * 3 % (glandColors.Height * 3) + 1].ToVector4();
                pbe.paletteCache[11] = glandColorsData[which * 3 % (glandColors.Height * 3) + 2].ToVector4();
            }
        }

        private static void UpdateShoePalette(Farmer who, PlayerBaseExtended pbe)
        {
            Boots boots = who.boots.Value;
            
            if (boots != null)
            {

                if (ModEntry.dga != null)
                {
                    string colorID = boots.indexInColorSheet.Value+"";//Start with default
                    if (ModEntry.dga.GetDGAItemId(boots) != null && !boots.modData.ContainsKey("DGA.FarmerColors"))
                    {
                        boots.modData["DGA.FarmerColors"] = ModEntry.dga.GetDGAItemId(boots);
                        ModEntry.debugmsg("DGA boots palette stored on item", LogLevel.Debug);
                    }

                    if (boots.modData.ContainsKey("DGA.FarmerColors")) { colorID = boots.modData["DGA.FarmerColors"]; }
                    else { boots.modData["DGA.FarmerColors"] = colorID; }

                    Color[] shoeColors = ShoesPalette.GetColors(colorID);
                    pbe.paletteCache[12] = shoeColors[0].ToVector4();
                    pbe.paletteCache[13] = shoeColors[1].ToVector4();
                    pbe.paletteCache[14] = shoeColors[2].ToVector4();
                    pbe.paletteCache[15] = shoeColors[3].ToVector4();
                }
                else
                {
                    int which = boots.indexInColorSheet.Value;

                    Texture2D shoeColors = Game1.content.Load<Texture2D>("Characters\\Farmer\\shoeColors");
                    Color[] shoeColorsData = new Color[shoeColors.Width * shoeColors.Height];
                    shoeColors.GetData(shoeColorsData);
                    pbe.paletteCache[12] = shoeColorsData[which * 4 % (shoeColors.Height * 4)].ToVector4();
                    pbe.paletteCache[13] = shoeColorsData[which * 4 % (shoeColors.Height * 4) + 1].ToVector4();
                    pbe.paletteCache[14] = shoeColorsData[which * 4 % (shoeColors.Height * 4) + 2].ToVector4();
                    pbe.paletteCache[15] = shoeColorsData[which * 4 % (shoeColors.Height * 4) + 3].ToVector4();
                }
            }
        }

        public static Texture2D GetFarmerShirt(Farmer who, Item shirt, bool color = false, bool overlay = false, string bodystyle = "")
        {
            if (ModEntry.dga == null) return null;

            Texture2D toReturn = null;
            string shirt_id = ModEntry.dga.GetDGAItemId(shirt);
            if (shirt_id == null)
            {
                return toReturn;
            }
            else { 
                if(DGAItems.GetDGAShirt(shirt_id) != null)
                {
                    toReturn = DGAItems.GetDGAShirt(shirt_id).GetTexture(who.isMale, color, overlay, bodystyle);
                }
            }
            return toReturn;
        }

        public static Texture2D GetFarmerPants(Item pants, string bodystyle = "")
        {
            if (ModEntry.dga == null) return null;

            Texture2D toReturn = null;
            string pants_id = ModEntry.dga.GetDGAItemId(pants);
            if (pants_id == null)
            {
                return toReturn;
            }
            else
            {
                if (DGAItems.GetDGAPants(pants_id) != null)
                {
                    toReturn = DGAItems.GetDGAPants(pants_id).GetTexture(bodystyle);
                }
            }
            return toReturn;
        }

        private static void UpdateShirtPalette(Farmer who, PlayerBaseExtended pbe)
        {
            if (who.shirtItem.Get() == null) return; //Shirt was removed so no need to change the colour

            Texture2D shirtsTexture = GetFarmerShirt(who, who.shirtItem.Get());
            if(shirtsTexture == null) shirtsTexture = FarmerRenderer.shirtsTexture;

            Color[] shirtData = new Color[shirtsTexture.Bounds.Width * shirtsTexture.Bounds.Height];
            shirtsTexture.GetData(shirtData);

            int index = AdjustedVanillaMethods.ClampShirt(who.GetShirtIndex()) * 8 / 128 * 32 * shirtsTexture.Bounds.Width + AdjustedVanillaMethods.ClampShirt(who.GetShirtIndex()) * 8 % 128 + shirtsTexture.Width * 4;
            
            int dye_index = index + 128;
            //for custom shirts
            if (shirtsTexture.Bounds.Width <= 8)
            {
                index = shirtsTexture.Width * 4;
                dye_index = index;
            }

            //Sleeveless is handles by textures now, so no logic here for that

            Color color = Utility.MakeCompletelyOpaque(who.GetShirtColor());
            Color shirtSleeveColor = shirtData[dye_index];
            Color clothes_color = color;
            if (shirtSleeveColor.A < byte.MaxValue)
            {
                shirtSleeveColor = shirtData[index];
                clothes_color = Color.White;
            }
            shirtSleeveColor = Utility.MultiplyColor(shirtSleeveColor, clothes_color);
            pbe.paletteCache[0] = shirtSleeveColor.ToVector4();

            shirtSleeveColor = shirtData[dye_index - shirtsTexture.Width];
            if (shirtSleeveColor.A < byte.MaxValue)
            {
                shirtSleeveColor = shirtData[index - shirtsTexture.Width];
                clothes_color = Color.White;
            }
            shirtSleeveColor = Utility.MultiplyColor(shirtSleeveColor, clothes_color);
            pbe.paletteCache[1] = shirtSleeveColor.ToVector4();

            shirtSleeveColor = shirtData[dye_index - shirtsTexture.Width * 2];
            if (shirtSleeveColor.A < byte.MaxValue)
            {
                shirtSleeveColor = shirtData[index - shirtsTexture.Width * 2];
                clothes_color = Color.White;
            }
            shirtSleeveColor = Utility.MultiplyColor(shirtSleeveColor, clothes_color);
            pbe.paletteCache[2] = shirtSleeveColor.ToVector4();
        }

        private static void DrawLowerNaked(FarmerRenderer __instance, Vector2 ___positionOffset, Vector2 ___rotationAdjustment, Texture2D ___baseTexture, FarmerSprite.AnimationFrame animationFrame, Rectangle sourceRect, SpriteBatch b, int facingDirection, Farmer who, Vector2 position, Vector2 origin, float scale, int currentFrame, float rotation, Color overrideColor, float layerDepth, float layerOff1, float layeroff2)
        {
            PlayerBaseExtended pbe = PlayerBaseExtended.Get(who);

            ///////////////////////////////
            /// Setup a new overlay drawing for lower body
            //no pants
            bool drawNakedOverlay = who.GetPantsIndex() == 14 || who.pantsItem.Value == null;
            if (!drawNakedOverlay && who.bathingClothes.Value)
            {
                if (who.modData.ContainsKey("DB.bathers"))
                {
                    drawNakedOverlay = who.modData["DB.bathers"] == "false";
                }
            }

            if (drawNakedOverlay)
            {
                Texture2D nakedOverlayTexture = pbe.GetNakedLowerTexture();

                if (nakedOverlayTexture != null)
                {
                    Rectangle pants_rect = new Rectangle(sourceRect.X, sourceRect.Y, sourceRect.Width, sourceRect.Height);
                    float layerOffset = layerOff1;
                    if (who.getFacingDirection() == 2)
                    {
                        layerOffset = layeroff2;//above arms when facing forward
                    }

                    if (!FarmerRenderer.isDrawingForUI && (bool)who.swimming.Value)
                    {
                        //don't draw it in the water
                    }
                    else
                    {

                        if (FarmerRenderer.isDrawingForUI)
                        {
                            //Change the frame for UI version
                            sourceRect.X = 0;
                            sourceRect.Y = 0;

                            float layerFix = 2E-05f + 3E-05f;
                            b.Draw(nakedOverlayTexture, position + origin + ___positionOffset, sourceRect, overrideColor, rotation, origin, 4f * scale, animationFrame.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth + (layerOffset + layerFix));
                        }
                        else
                        {
                            b.Draw(nakedOverlayTexture, position + origin + ___positionOffset, sourceRect, overrideColor, rotation, origin, 4f * scale, animationFrame.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth + layerOffset);
                        }
                    }
                }
            }
        }

        private static bool pre_DrawHairAndAccesories(FarmerRenderer __instance, bool ___isDrawingForUI, Vector2 ___positionOffset, Vector2 ___rotationAdjustment, LocalizedContentManager ___farmerTextureManager, Texture2D ___baseTexture, NetInt ___skin, bool ____sickFrame, ref Rectangle ___hairstyleSourceRect, ref Rectangle ___shirtSourceRect, ref Rectangle ___accessorySourceRect, ref Rectangle ___hatSourceRect, SpriteBatch b, int facingDirection, Farmer who, Vector2 position, Vector2 origin, float scale, int currentFrame, float rotation, Color overrideColor, float layerDepth)
        {

            if (b != null)
            {

                int sort_direction = ((!Game1.isUsingBackToFrontSorting) ? 1 : (-1));

                //Get details that the standard Draw method uses
                FarmerSprite.AnimationFrame animationFrame = who.FarmerSprite.CurrentAnimationFrame;
                Rectangle sourceRect = who.FarmerSprite.SourceRect;

                //Vector2 positionOffset = Vector2.Zero;
                //positionOffset.Y = (int)(animationFrame.positionOffset * 4f);
                //positionOffset.X = (int)(animationFrame.xOffset * 4f);

                PlayerBaseExtended pbe = PlayerBaseExtended.Get(who);

                //Draw the shirts
                if (!who.bathingClothes.Value && who.shirtItem.Get() != null)
                {
                    bool shirt_not_drawn = true;
                    int shirtindex = -1;
                    Texture2D shirtsTexture = FarmerRenderer.shirtsTexture;
                    if (pbe.body.textures.ContainsKey("shirt") && pbe.body.textures["shirt"] != null)
                    {
                        shirtsTexture = pbe.body.textures["shirt"];
                    }

                    if(ModEntry.dga != null)
                    {
                        Texture2D shirtsTextureDGA = GetFarmerShirt(who, who.shirtItem.Get(), false, false, pbe.body.name);
                        if (shirtsTextureDGA != null)
                        {
                            shirt_not_drawn = false;
                            shirtsTexture = shirtsTextureDGA;
                            shirtindex = DGAItems.GetDGAShirt(ModEntry.dga.GetDGAItemId(who.shirtItem.Get())).GetIndex(who.isMale, false, false, pbe.body.name);

                            AdjustedVanillaMethods.DrawShirt(__instance, shirtsTexture, ___positionOffset, ___rotationAdjustment, ref ___shirtSourceRect, b, facingDirection, who, position, origin, scale, currentFrame, rotation, overrideColor, layerDepth, false, shirtindex);
                            //try color layer
                            shirtsTextureDGA = GetFarmerShirt(who, who.shirtItem.Get(), true, false, pbe.body.name);
                            if (shirtsTextureDGA != null)
                            {
                                shirtindex = DGAItems.GetDGAShirt(ModEntry.dga.GetDGAItemId(who.shirtItem.Get())).GetIndex(who.isMale, true, false, pbe.body.name);

                                AdjustedVanillaMethods.DrawShirt(__instance, shirtsTextureDGA, ___positionOffset, ___rotationAdjustment, ref ___shirtSourceRect, b, facingDirection, who, position, origin, scale, currentFrame, rotation, overrideColor, layerDepth + 1.2E-07f, false, shirtindex);
                            }

                            //try overlay layer
                            shirtsTextureDGA = GetFarmerShirt(who, who.shirtItem.Get(), false, true, pbe.body.name);
                            if (shirtsTextureDGA != null && who.pants.Value != 14 && who.pantsItem.Get() != null && who.pantsItem.Get().dyeable)
                            {
                                shirtindex = DGAItems.GetDGAShirt(ModEntry.dga.GetDGAItemId(who.shirtItem.Get())).GetIndex(who.isMale, false, true, pbe.body.name);

                                AdjustedVanillaMethods.DrawShirt(__instance, shirtsTextureDGA, ___positionOffset, ___rotationAdjustment, ref ___shirtSourceRect, b, facingDirection, who, position, origin, scale, currentFrame, rotation, overrideColor.Equals(Color.White) ? Utility.MakeCompletelyOpaque(who.GetPantsColor()) : overrideColor, layerDepth + 1.2E-07f + 1.4E-07f, false, shirtindex);
                            }
                        }
                    }

                    if(shirt_not_drawn) AdjustedVanillaMethods.DrawShirt(__instance, shirtsTexture, ___positionOffset, ___rotationAdjustment, ref ___shirtSourceRect, b, facingDirection, who, position, origin, scale, currentFrame, rotation, overrideColor, layerDepth);

                    //layerDepth += 1.4E-07f;
                    if (shirt_not_drawn && who.modData.ContainsKey("DB.overallColor") && who.modData["DB.overallColor"] == "true")
                    {
                        if (who.pants.Value != 14 && who.pantsItem.Get() != null && who.pantsItem.Get().dyeable)
                        {
                            //Draw the tinted overalls/highwaisted pants
                            Texture2D overalls_texture = Game1.content.Load<Texture2D>("Mods/ribeena.dynamicbodies/assets/Character/shirts_overlay.png");
                            

                            if (pbe.shirtOverlayIndex >= 0)
                            {
                                //TODO how does a shirt overlay added with JSON work with a body...?
                                AdjustedVanillaMethods.DrawShirt(__instance, overalls_texture, ___positionOffset, ___rotationAdjustment, ref ___shirtSourceRect, b, facingDirection, who, position, origin, scale, currentFrame, rotation, overrideColor.Equals(Color.White) ? Utility.MakeCompletelyOpaque(who.GetPantsColor()) : overrideColor, layerDepth + 1.8E-07f + 1.4E-07f, false, pbe.shirtOverlayIndex);
                            }
                            else
                            {
                                //Only default shirts will allow overlays
                                if (pbe.body.textures.ContainsKey("shirt_overlay") && pbe.body.textures["shirt_overlay"] != null)
                                {
                                    overalls_texture = pbe.body.textures["shirt_overlay"];
                                }

                                AdjustedVanillaMethods.DrawShirt(__instance, overalls_texture, ___positionOffset, ___rotationAdjustment, ref ___shirtSourceRect, b, facingDirection, who, position, origin, scale, currentFrame, rotation, overrideColor.Equals(Color.White) ? Utility.MakeCompletelyOpaque(who.GetPantsColor()) : overrideColor, layerDepth + 1.8E-07f + 1.4E-07f, false);
                            }
                        }
                    }
                }

                //Draw trinket 1
                if (pbe.trinkets[0].option != "Default")
                {
                    DrawTrinket(0, __instance, pbe, ___positionOffset, b, facingDirection, who, position, origin, scale, currentFrame, rotation, overrideColor, layerDepth + (3.2E-07f + 1.95E-05f) / 2f, sort_direction);
                }

                //Draw the beards
                Texture2D beardTexture = null;
                if (!pbe.beard.OptionMatchesModData(who))
                {
                    pbe.beard.SetOptionFromModData(who, ModEntry.beardOptions);
                }
                else
                {
                    int accessory_id = (int)who.accessory.Value;
                    if (accessory_id >= 0 && accessory_id < 6)
                    {
                        //Move beard setting
                        pbe.beard.file = accessory_id.ToString();
                        pbe.beard.provider = null;
                        who.changeAccessory(-1);
                    }
                }

                if (pbe.beard.option != "Default")
                {
                    Rectangle accessorySourceRect;
                    int beardheight = 16;
                    if (pbe.beard.provider == null)
                    {
                        int accessory_id = (int)who.accessory.Value;
                        if (pbe.beard.file.Length == 1)
                        {
                            accessory_id = int.Parse(pbe.beard.file);
                        }
                        //get the accessory from vanilla
                        beardTexture = pbe.GetBeardTexture(who, accessory_id, FarmerRenderer.accessoriesTexture, new Rectangle(16 * accessory_id, 0, 16, 32));
                        accessorySourceRect = new Rectangle(0, 0, 16, 16);
                    }
                    else
                    {
                        //get the content pack
                        beardTexture = pbe.GetBeardTexture(who);
                        accessorySourceRect = new Rectangle(0, 0, 16, 32);
                        beardheight = 32;
                    }
                    //crop the beard if they are swimming
                    if (!FarmerRenderer.isDrawingForUI && (bool)who.swimming.Value)
                    {
                        accessorySourceRect.Height = 16;
                    }

                    switch (facingDirection)
                    {
                        case 0:
                            if (accessorySourceRect.Height == 32)
                            {
                                accessorySourceRect.Y = accessorySourceRect.Height * 2;
                                b.Draw(beardTexture, position + origin + ___positionOffset + new Vector2(FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4, FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4 + (int)__instance.heightOffset.Value), accessorySourceRect, Color.White, rotation, origin, 4f * scale, SpriteEffects.None, layerDepth + 1.95E-05f);
                            }
                            break;
                        case 1:
                            accessorySourceRect.Y = beardheight;
                            b.Draw(beardTexture, position + origin + ___positionOffset + ___rotationAdjustment + new Vector2(FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4, 4 + FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4 + (int)__instance.heightOffset.Value), accessorySourceRect, Color.White, rotation, origin, 4f * scale, SpriteEffects.None, layerDepth + 1.95E-05f);
                            break;
                        case 2:
                            b.Draw(beardTexture, position + origin + ___positionOffset + ___rotationAdjustment + new Vector2(FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4, 8 + FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4 + (int)__instance.heightOffset.Value - 4), accessorySourceRect, Color.White, rotation, origin, 4f * scale, SpriteEffects.None, layerDepth + 1.95E-05f);
                            break;
                        case 3:
                            accessorySourceRect.Y = beardheight;
                            b.Draw(beardTexture, position + origin + ___positionOffset + ___rotationAdjustment + new Vector2(-FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4, 4 + FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4 + (int)__instance.heightOffset.Value), accessorySourceRect, Color.White, rotation, origin, 4f * scale, SpriteEffects.FlipHorizontally, layerDepth + 1.95E-05f);
                            break;
                    }
                }

                //Draw trinket 2
                if (pbe.trinkets[1].option != "Default")
                {
                    DrawTrinket(1, __instance, pbe, ___positionOffset, b, facingDirection, who, position, origin, scale, currentFrame, rotation, overrideColor, layerDepth + (1.95E-05f + 2.9E-05f) / 2f, sort_direction);
                }

                //Draw Vanilla accessories
                if ((int)who.accessory.Value >= 6)
                {
                    AdjustedVanillaMethods.DrawAccessory(__instance, ___positionOffset, ___rotationAdjustment, ref ___accessorySourceRect, b, facingDirection, who, position, origin, scale, currentFrame, rotation, overrideColor, layerDepth);
                }

                DrawHair(pbe, ___positionOffset, b, facingDirection, who, position, origin, scale, currentFrame, rotation, overrideColor, layerDepth, sort_direction);

                //Draw trinket 3
                if (pbe.trinkets[2].option != "Default")
                {
                    DrawTrinket(2, __instance, pbe, ___positionOffset, b, facingDirection, who, position, origin, scale, currentFrame, rotation, overrideColor, layerDepth + (ModEntry.hairlayer + 3.9E-05f) / 2f, sort_direction);
                }

                //Draw naked
                if (!pbe.nakedLower.CheckForOption("below accessories")) DrawLowerNaked(__instance, ___positionOffset, ___rotationAdjustment, ___baseTexture, animationFrame, sourceRect, b, facingDirection, who, position, origin, scale, currentFrame, rotation, overrideColor, layerDepth, ModEntry.FS_pantslayer, 5.95E-05f);

                //Draw the Vanilla hat
                if (who.hat.Value != null && !who.bathingClothes.Value)
                {
                    AdjustedVanillaMethods.DrawHat(__instance, ___positionOffset, ref ___hatSourceRect, b, facingDirection, who, position, origin, scale, currentFrame, rotation, layerDepth);
                }

                //Draw trinket 4
                if (pbe.trinkets[3].option != "Default")
                {
                    DrawTrinket(3, __instance, pbe, ___positionOffset, b, facingDirection, who, position, origin, scale, currentFrame, rotation, overrideColor, layerDepth + (3.9E-05f + 4.9E-05f) / 2f, sort_direction);
                }

            }
            //prevent further rendering
            return false;

        }

        public static void DrawTrinket(int trinket, FarmerRenderer __instance, PlayerBaseExtended pbe, Vector2 ___positionOffset, SpriteBatch b, int facingDirection, Farmer who, Vector2 position, Vector2 origin, float scale, int currentFrame, float rotation, Color overrideColor, float layerDepth, float sort_direction)
        {
            List<string> all_trinkets = ModEntry.getContentPackOptions(ModEntry.trinketOptions[trinket]).ToList();
            int current_index = all_trinkets.IndexOf((who.modData.ContainsKey("DB.trinket"+ trinket)) ? who.modData["DB.trinket" +trinket] : "Default");
            Trinkets.ContentPackTrinketOption option = ModEntry.trinketOptions[trinket][current_index] as Trinkets.ContentPackTrinketOption;

            bool flip = !option.settings.usesUniqueLeftSprite;
            Vector2 offsetPosition = Vector2.Zero;
            offsetPosition.X -= (option.settings.extraWidth / 2f) * 4f;
            offsetPosition.Y = option.settings.yOffset * 4f;

            Texture2D trinket_texture = pbe.GetTrinketTexture(who, trinket);
            Rectangle trinketSourceRect = ExpandedAnimations.getFrameRectangle(who, option.settings, option.settings.usesUniqueLeftSprite ? trinket_texture.Height / 4 : trinket_texture.Height / 3, 16 + option.settings.extraWidth, facingDirection);

            if (trinket_texture != null)
            {
                switch (facingDirection)
                {
                    case 0:
                        b.Draw(trinket_texture, position + origin + ___positionOffset + offsetPosition + new Vector2(FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4, FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4 + (int)__instance.heightOffset.Value), trinketSourceRect, Color.White, rotation, origin, 4f * scale, SpriteEffects.None, layerDepth);
                        break;
                    case 1:
                        b.Draw(trinket_texture, position + origin + ___positionOffset + offsetPosition + new Vector2(FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4, 4 + FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4 + (int)__instance.heightOffset.Value), trinketSourceRect, Color.White, rotation, origin, 4f * scale, SpriteEffects.None, layerDepth);
                        break;
                    case 2:
                        b.Draw(trinket_texture, position + origin + ___positionOffset + offsetPosition + new Vector2(FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4, 8 + FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4 + (int)__instance.heightOffset.Value - 4), trinketSourceRect, Color.White, rotation, origin, 4f * scale, SpriteEffects.None, layerDepth);
                        break;
                    case 3:
                        b.Draw(trinket_texture, position + origin + ___positionOffset + offsetPosition + new Vector2(-FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4, 4 + FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4 + (int)__instance.heightOffset.Value), trinketSourceRect, Color.White, rotation, origin, 4f * scale, flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth);
                        break;
                }
            }
        }

        public static void DrawHair(PlayerBaseExtended pbe, Vector2 ___positionOffset, SpriteBatch b, int facingDirection, Farmer who, Vector2 position, Vector2 origin, float scale, int currentFrame, float rotation, Color overrideColor, float layerDepth, float sort_direction)
        {
            bool drawHair = true;

            if (who.hat.Value != null && who.hat.Value.hairDrawType.Value == 2) //Hat says hide any hair
            {
                drawHair = false;
            }
            if (drawHair)
            {
                ///////////////////////////////
                /// Setup overlay for rendering new two tone 43
                /// 
                int hair_style = who.getHair(); // (ignore_hat: true);
                HairStyleMetadata hair_metadata = Farmer.GetHairStyleMetadata(who.hair.Value);
                if (pbe.hairStyle.option.StartsWith("Vanilla"))
                {
                    //Use the modData version
                    hair_style = int.Parse(pbe.hairStyle.file);
                    hair_metadata = Farmer.GetHairStyleMetadata(hair_style);
                }

                Texture2D hair_texture = null;
                bool flip = false;

                //Cached and recoloured hair only has the one version
                Rectangle hairstyleSourceRect = new Rectangle(0, 0, 16, 32);
                Vector2 offsetPosition = new Vector2(0, 0);

                if (FarmerRenderer.isDrawingForUI)
                {
                    //hair_draw_layer = 1.15E-07f;
                    layerDepth = 0.7f;
                    //context.Monitor.Log($"UI layer is [{layerDepth}].", LogLevel.Debug);
                    facingDirection = 2;
                }

                if (pbe.hairStyle.option == "Default" || pbe.hairStyle.option.StartsWith("Vanilla"))
                {
                    if (who != null && who.hat.Value != null && who.hat.Value.hairDrawType.Value == 1 && hair_metadata != null && hair_metadata.coveredIndex != -1)
                    {
                        hair_style = hair_metadata.coveredIndex;
                        hair_metadata = Farmer.GetHairStyleMetadata(hair_style);
                    }
                    Rectangle hairstyleSourceOriginalRect = new Rectangle(hair_style * 16 % FarmerRenderer.hairStylesTexture.Width, hair_style * 16 / FarmerRenderer.hairStylesTexture.Width * 96, 16, 32 * 3);


                    if (hair_metadata != null)
                    {
                        hairstyleSourceOriginalRect = new Rectangle(hair_metadata.tileX * 16, hair_metadata.tileY * 16, 16, 32);
                        if (hair_metadata.usesUniqueLeftSprite)
                        {
                            hairstyleSourceOriginalRect.Height = 32 * 4;
                        }
                        hair_texture = pbe.GetHairTexture(who, hair_style, hair_metadata.texture, hairstyleSourceOriginalRect);
                    }
                    else
                    {
                        hair_texture = pbe.GetHairTexture(who, hair_style, FarmerRenderer.hairStylesTexture, hairstyleSourceOriginalRect);
                    }

                    //Adjust rect of what to draw
                    switch (facingDirection)
                    {
                        case 0:
                            hairstyleSourceRect.Offset(0, 64);
                            break;
                        case 1:
                            hairstyleSourceRect.Offset(0, 32);
                            break;
                        case 3:
                            flip = true;
                            if (hair_metadata != null && hair_metadata.usesUniqueLeftSprite)
                            {
                                flip = false;
                                hairstyleSourceRect.Offset(0, 96);
                            }
                            else
                            {
                                hairstyleSourceRect.Offset(0, 32);
                            }
                            break;
                    }
                }
                else
                {


                    List<string> all_hairs = ModEntry.getContentPackOptions(ModEntry.hairOptions).ToList();
                    int current_index = all_hairs.IndexOf((who.modData.ContainsKey("DB.hairStyle")) ? who.modData["DB.hairStyle"] : "Default");
                    ExtendedHair.ContentPackHairOption option = ModEntry.hairOptions[current_index] as ExtendedHair.ContentPackHairOption;

                    string has_hat = "";
                    if (who.hat.Value != null && who.hat.Value.hairDrawType.Value == 1)
                    {
                        if (option.hasHatTexture)
                        {
                            has_hat = "hat";
                        }
                        else
                        {
                            //TODO handle hairDrawType 0 not really obscured and hairDrawType 1 partially obscure
                        }
                    }

                    flip = !option.settings.usesUniqueLeftSprite;
                    offsetPosition.X -= (option.settings.extraWidth / 2f) * 4f;
                    offsetPosition.Y = option.settings.yOffset * 4f;

                    hair_texture = pbe.GetHairStyleTexture(who, has_hat);
                    hairstyleSourceRect = ExpandedAnimations.getFrameRectangle(who, option.settings, option.settings.usesUniqueLeftSprite ? hair_texture.Height / 4 : hair_texture.Height / 3, 16 + option.settings.extraWidth, facingDirection);

                    if (option.hasBackTexture && !FarmerRenderer.isDrawingForUI)
                    {
                        float hair_back_layer = -2.25E-05f;
                        Texture2D backHairTexture;
                        if (option.hasHatTexture)
                        {
                            backHairTexture = pbe.GetHairStyleTexture(who, "back_hat");
                        }
                        else
                        {
                            backHairTexture = pbe.GetHairStyleTexture(who, "back");
                        }

                        switch (facingDirection)
                        {
                            case 0:
                                b.Draw(backHairTexture, position + origin + ___positionOffset + offsetPosition + new Vector2(FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4, FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4 + 4 + ((who.IsMale && hair_style >= 16) ? (-4) : ((!who.IsMale && hair_style < 16) ? 4 : 0))), hairstyleSourceRect, Color.White, rotation, origin, 4f * scale, SpriteEffects.None, layerDepth + hair_back_layer * (float)sort_direction);
                                break;
                            case 1:
                                b.Draw(backHairTexture, position + origin + ___positionOffset + offsetPosition + new Vector2(FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4, FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4 + ((who.IsMale && (int)who.hair.Value >= 16) ? (-4) : ((!who.IsMale && (int)who.hair.Value < 16) ? 4 : 0))), hairstyleSourceRect, Color.White, rotation, origin, 4f * scale, SpriteEffects.None, layerDepth + hair_back_layer * (float)sort_direction);
                                break;
                            case 2:
                                b.Draw(backHairTexture, position + origin + ___positionOffset + offsetPosition + new Vector2(FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4, FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4 + ((who.IsMale && (int)who.hair.Value >= 16) ? (-4) : ((!who.IsMale && (int)who.hair.Value < 16) ? 4 : 0))), hairstyleSourceRect, Color.White, rotation, origin, 4f * scale, SpriteEffects.None, layerDepth + hair_back_layer * (float)sort_direction);
                                break;
                            case 3:
                                b.Draw(backHairTexture, position + origin + ___positionOffset + offsetPosition + new Vector2(-FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4, FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4 + ((who.IsMale && (int)who.hair.Value >= 16) ? (-4) : ((!who.IsMale && (int)who.hair.Value < 16) ? 4 : 0))), hairstyleSourceRect, Color.White, rotation, origin, 4f * scale, flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth + hair_back_layer * (float)sort_direction);
                                break;
                        }
                    }
                }



                float hair_draw_layer = ModEntry.hairlayer; //2.25E-05f //2.2E-05f;

                //float base_layer = layerDepth;



                if (hair_texture != null)
                {
                    switch (facingDirection)
                    {
                        case 0:
                            b.Draw(hair_texture, position + origin + ___positionOffset + offsetPosition + new Vector2(FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4, FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4 + 4 + ((who.IsMale && hair_style >= 16) ? (-4) : ((!who.IsMale && hair_style < 16) ? 4 : 0))), hairstyleSourceRect, Color.White, rotation, origin, 4f * scale, SpriteEffects.None, layerDepth + hair_draw_layer * (float)sort_direction);
                            break;
                        case 1:
                            b.Draw(hair_texture, position + origin + ___positionOffset + offsetPosition + new Vector2(FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4, FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4 + ((who.IsMale && (int)who.hair.Value >= 16) ? (-4) : ((!who.IsMale && (int)who.hair.Value < 16) ? 4 : 0))), hairstyleSourceRect, Color.White, rotation, origin, 4f * scale, SpriteEffects.None, layerDepth + hair_draw_layer * (float)sort_direction);
                            break;
                        case 2:
                            b.Draw(hair_texture, position + origin + ___positionOffset + offsetPosition + new Vector2(FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4, FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4 + ((who.IsMale && (int)who.hair.Value >= 16) ? (-4) : ((!who.IsMale && (int)who.hair.Value < 16) ? 4 : 0))), hairstyleSourceRect, Color.White, rotation, origin, 4f * scale, SpriteEffects.None, layerDepth + hair_draw_layer * (float)sort_direction);
                            break;
                        case 3:
                            b.Draw(hair_texture, position + origin + ___positionOffset + offsetPosition + new Vector2(-FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4, FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4 + ((who.IsMale && (int)who.hair.Value >= 16) ? (-4) : ((!who.IsMale && (int)who.hair.Value < 16) ? 4 : 0))), hairstyleSourceRect, Color.White, rotation, origin, 4f * scale, flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth + hair_draw_layer * (float)sort_direction);
                            break;
                    }
                }
            }
        }
        public static void DrawHairBackUI(PlayerBaseExtended pbe, Vector2 ___positionOffset, SpriteBatch b, int facingDirection, Farmer who, Vector2 position, Vector2 origin, float scale, int currentFrame, float rotation, Color overrideColor, float layerDepth, float sort_direction, bool cropped = false)
        {
            bool drawHair = true;

            if (who.hat.Value != null && who.hat.Value.hairDrawType.Value == 2) //Hat says hide any hair
            {
                drawHair = false;
            }
            if (drawHair)
            {
                ///////////////////////////////
                /// Setup overlay for rendering new two tone 43
                /// 
                int hair_style = who.getHair(); // (ignore_hat: true);
                HairStyleMetadata hair_metadata = Farmer.GetHairStyleMetadata(who.hair.Value);
                if (pbe.hairStyle.option.StartsWith("Vanilla"))
                {
                    //Use the modData version
                    hair_style = int.Parse(pbe.hairStyle.file);
                    hair_metadata = Farmer.GetHairStyleMetadata(hair_style);
                }

                Texture2D hair_texture = null;
                bool flip = false;

                //Cached and recoloured hair only has the one version
                Rectangle hairstyleSourceRect = new Rectangle(0, 0, 16, 32);
                Vector2 offsetPosition = new Vector2(0, 0);


                if (pbe.hairStyle.option == "Default" || pbe.hairStyle.option.StartsWith("Vanilla"))
                {
                    
                }
                else
                {
                    List<string> all_hairs = ModEntry.getContentPackOptions(ModEntry.hairOptions).ToList();
                    int current_index = all_hairs.IndexOf((who.modData.ContainsKey("DB.hairStyle")) ? who.modData["DB.hairStyle"] : "Default");
                    ExtendedHair.ContentPackHairOption option = ModEntry.hairOptions[current_index] as ExtendedHair.ContentPackHairOption;

                    string has_hat = "";
                    if (who.hat.Value != null && who.hat.Value.hairDrawType.Value == 1)
                    {
                        if (option.hasHatTexture)
                        {
                            has_hat = "hat";
                        }
                        else
                        {
                            //TODO handle hairDrawType 0 not really obscured and hairDrawType 1 partially obscure
                        }
                    }

                    flip = !option.settings.usesUniqueLeftSprite;
                    offsetPosition.X -= (option.settings.extraWidth / 2f) * 4f;
                    offsetPosition.Y = option.settings.yOffset * 4f;

                    hair_texture = pbe.GetHairStyleTexture(who, has_hat);
                    hairstyleSourceRect = ExpandedAnimations.getFrameRectangle(who, option.settings, option.settings.usesUniqueLeftSprite ? hair_texture.Height / 4 : hair_texture.Height / 3, 16 + option.settings.extraWidth, facingDirection);

                    if (option.hasBackTexture)
                    {
                        float hair_back_layer = -2.25E-05f;
                        Texture2D backHairTexture;
                        if (option.hasHatTexture)
                        {
                            backHairTexture = pbe.GetHairStyleTexture(who, "back_hat");
                        }
                        else
                        {
                            backHairTexture = pbe.GetHairStyleTexture(who, "back");
                        }

                        switch (facingDirection)
                        {
                            case 0:
                                b.Draw(backHairTexture, position + origin + ___positionOffset + offsetPosition + new Vector2(FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4, FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4 + 4 + ((who.IsMale && hair_style >= 16) ? (-4) : ((!who.IsMale && hair_style < 16) ? 4 : 0))), hairstyleSourceRect, Color.White, rotation, origin, 4f * scale, SpriteEffects.None, layerDepth + hair_back_layer * (float)sort_direction);
                                break;
                            case 1:
                                b.Draw(backHairTexture, position + origin + ___positionOffset + offsetPosition + new Vector2(FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4, FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4 + ((who.IsMale && (int)who.hair.Value >= 16) ? (-4) : ((!who.IsMale && (int)who.hair.Value < 16) ? 4 : 0))), hairstyleSourceRect, Color.White, rotation, origin, 4f * scale, SpriteEffects.None, layerDepth + hair_back_layer * (float)sort_direction);
                                break;
                            case 2:
                                b.Draw(backHairTexture, position + origin + ___positionOffset + offsetPosition + new Vector2(FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4, FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4 + ((who.IsMale && (int)who.hair.Value >= 16) ? (-4) : ((!who.IsMale && (int)who.hair.Value < 16) ? 4 : 0))), hairstyleSourceRect, Color.White, rotation, origin, 4f * scale, SpriteEffects.None, layerDepth + hair_back_layer * (float)sort_direction);
                                break;
                            case 3:
                                b.Draw(backHairTexture, position + origin + ___positionOffset + offsetPosition + new Vector2(-FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4, FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4 + ((who.IsMale && (int)who.hair.Value >= 16) ? (-4) : ((!who.IsMale && (int)who.hair.Value < 16) ? 4 : 0))), hairstyleSourceRect, Color.White, rotation, origin, 4f * scale, flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth + hair_back_layer * (float)sort_direction);
                                break;
                        }
                    }
                }
            }
        }

        public static bool pre_drawMiniPortrat(FarmerRenderer __instance, ref Texture2D ___baseTexture, SpriteBatch b, Vector2 position, float layerDepth, float scale, int facingDirection, Farmer who)
        {
            //Stick to a pixel
            position.X = (int)position.X;
            position.Y = (int)position.Y;

            PlayerBaseExtended pbe = PlayerBaseExtended.Get(who);
            //this.executeRecolorActions(who);
            //facingDirection = 2;
            //bool flip = false;
            //int yOffset = 0;
            int feature_y_offset = FarmerRenderer.featureYOffsetPerFrame[0];

            int sort_direction = ((!Game1.isUsingBackToFrontSorting) ? 1 : (-1));

            bool drawHair = true;
            Rectangle hairstyleSourceRect = new Rectangle(0, 0, 16, 16);
            Texture2D hair_texture = null;

            if (who.hat.Value != null && who.hat.Value.hairDrawType.Value == 2) //Hat says hide any hair
            {
                drawHair = false;
            }
            if (drawHair)
            {
                ///////////////////////////////
                /// Setup overlay for rendering new two tone 43
                /// 
                int hair_style = who.getHair(); // (ignore_hat: true);
                HairStyleMetadata hair_metadata = Farmer.GetHairStyleMetadata(who.hair.Value);
                if (pbe.hairStyle.option.StartsWith("Vanilla"))
                {
                    //Use the modData version
                    hair_style = int.Parse(pbe.hairStyle.file);
                    hair_metadata = Farmer.GetHairStyleMetadata(hair_style);
                }

                Vector2 offsetPosition = new Vector2(0, 0);


                if (pbe.hairStyle.option == "Default" || pbe.hairStyle.option.StartsWith("Vanilla"))
                {
                    if (who != null && who.hat.Value != null && who.hat.Value.hairDrawType.Value == 1 && hair_metadata != null && hair_metadata.coveredIndex != -1)
                    {
                        hair_style = hair_metadata.coveredIndex;
                        hair_metadata = Farmer.GetHairStyleMetadata(hair_style);
                    }
                    Rectangle hairstyleSourceOriginalRect = new Rectangle(hair_style * 16 % FarmerRenderer.hairStylesTexture.Width, hair_style * 16 / FarmerRenderer.hairStylesTexture.Width * 96, 16, 32 * 3);


                    if (hair_metadata != null)
                    {
                        hairstyleSourceOriginalRect = new Rectangle(hair_metadata.tileX * 16, hair_metadata.tileY * 16, 16, 32);
                        if (hair_metadata.usesUniqueLeftSprite)
                        {
                            hairstyleSourceOriginalRect.Height = 32 * 4;
                        }
                        hair_texture = pbe.GetHairTexture(who, hair_style, hair_metadata.texture, hairstyleSourceOriginalRect);
                    }
                    else
                    {
                        hair_texture = pbe.GetHairTexture(who, hair_style, FarmerRenderer.hairStylesTexture, hairstyleSourceOriginalRect);
                    }

                }
                else
                {


                    List<string> all_hairs = ModEntry.getContentPackOptions(ModEntry.hairOptions).ToList();
                    int current_index = all_hairs.IndexOf((who.modData.ContainsKey("DB.hairStyle")) ? who.modData["DB.hairStyle"] : "Default");
                    ExtendedHair.ContentPackHairOption option = ModEntry.hairOptions[current_index] as ExtendedHair.ContentPackHairOption;

                    string has_hat = "";
                    if (who.hat.Value != null && who.hat.Value.hairDrawType.Value == 1)
                    {
                        if (option.hasHatTexture)
                        {
                            has_hat = "hat";
                        }
                        else
                        {
                            //TODO handle hairDrawType 0 not really obscured and hairDrawType 1 partially obscure
                        }
                    }
                    offsetPosition.X -= (option.settings.extraWidth / 2f) * 4f;
                    offsetPosition.Y = option.settings.yOffset * 4f;

                    hair_texture = pbe.GetHairStyleTexture(who, has_hat);
                    hairstyleSourceRect = ExpandedAnimations.getFrameRectangle(who, option.settings, option.settings.usesUniqueLeftSprite ? hair_texture.Height / 4 : hair_texture.Height / 3, 16 + option.settings.extraWidth, facingDirection);
                    hairstyleSourceRect.X += (hairstyleSourceRect.Width - 16) / 2;
                    hairstyleSourceRect.Width = 16;
                    hairstyleSourceRect.Height = 16;

                    if (option.hasBackTexture && !FarmerRenderer.isDrawingForUI)
                    {
                        Texture2D backHairTexture;
                        if (option.hasHatTexture)
                        {
                            backHairTexture = pbe.GetHairStyleTexture(who, "back_hat");
                        }
                        else
                        {
                            backHairTexture = pbe.GetHairStyleTexture(who, "back");
                        }

                        b.Draw(backHairTexture, position + new Vector2(0f, (who.IsMale ? 0 : -4)) * scale / 4f + new Vector2(0f, feature_y_offset * 4 + ((who.IsMale && (int)who.hair >= 16) ? (-4) : ((!who.IsMale && (int)who.hair < 16) ? 4 : 0))) * scale / 4f, hairstyleSourceRect, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth - 1.1E-07f * (float)sort_direction);

                    }
                }

            }



            //Draw the base
            b.Draw(pbe.cacheImage, position + new Vector2(0f, (who.IsMale ? 0 : -4)) * scale / 4f, new Rectangle(0, 0, 16, who.isMale ? 15 : 16), Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
            
            //Draw the beards
            Texture2D beardTexture = null;
            if (pbe.beard.option != "Default")
            {
                Rectangle accessorySourceRect;
                if (pbe.beard.provider == null)
                {
                    int accessory_id = (int)who.accessory.Value;
                    if (pbe.beard.file.Length == 1)
                    {
                        accessory_id = int.Parse(pbe.beard.file);
                    }
                    //get the accessory from vanilla
                    beardTexture = pbe.GetBeardTexture(who, accessory_id, FarmerRenderer.accessoriesTexture, new Rectangle(16 * accessory_id, 0, 16, 32));
                    accessorySourceRect = new Rectangle(0, 0, 16, 16);
                }
                else
                {
                    //get the content pack
                    beardTexture = pbe.GetBeardTexture(who);
                    accessorySourceRect = new Rectangle(0, 0, 16, 32);
                }
                //crop the beard for the mini
                accessorySourceRect.Height = who.isMale ? 15 : 16;

                b.Draw(beardTexture, position + new Vector2(0f, feature_y_offset * 4 + 4) * scale / 4f, accessorySourceRect, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth + 1.1E-08f * (float)sort_direction);
            }

            //Draw Vanilla accessories
            if ((int)who.accessory.Value >= 6)
            {
                Rectangle accessorySourceRect = new Rectangle((int)who.accessory.Value * 16 % FarmerRenderer.accessoriesTexture.Width, (int)who.accessory.Value * 16 / FarmerRenderer.accessoriesTexture.Width * 32, 16, 16);
                accessorySourceRect.Height = who.isMale ? 15 : 16;

                b.Draw(FarmerRenderer.accessoriesTexture, position + new Vector2(0f, feature_y_offset * 4 + 4) * scale / 4f, accessorySourceRect, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth + 0.9E-07f * (float)sort_direction);

            }

            //Draw the hair

            if (drawHair)
            {
                if (hair_texture != null)
                {
                    b.Draw(hair_texture, position + new Vector2(0f, (who.IsMale ? 0 : -4)) * scale / 4f + new Vector2(0f, feature_y_offset * 4 + ((who.IsMale && (int)who.hair >= 16) ? (-4) : ((!who.IsMale && (int)who.hair < 16) ? 4 : 0))) * scale / 4f, hairstyleSourceRect, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth + 1.1E-07f * (float)sort_direction);
                }
            }
            //prevent further rendering
            return false;
        }
    }
}
