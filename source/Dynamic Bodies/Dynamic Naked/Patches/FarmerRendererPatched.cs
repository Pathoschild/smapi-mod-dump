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
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Network;
using Netcode;

using DynamicBodies.Data;
using StardewValley.Objects;

namespace DynamicBodies.Patches
{
    public class FarmerRendererPatched
    {
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

            harmony.CreateReversePatcher(AccessTools.Method(typeof(FarmerRenderer), "executeRecolorActions", new[] { typeof(Farmer) }),
                new HarmonyMethod(GetType(), nameof(ExecuteRecolorActionsReversePatch))
                ).Patch();
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
            if(field == "shirt" || field == "shoes")
            {
                pbe.cacheImage = null;
            }
            if(field == "shirt")
            {
                pbe.shirt = -1;//force shirt change
            }
            if(field == "shoes")
            {
                pbe.shoes = -1;//force shoe change
            }
            if (field == "pants")
            {
                pbe.pants = -1;//force pants change
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
        private static bool pre_Draw(FarmerRenderer __instance, ref Vector2 ___positionOffset, ref Vector2 ___rotationAdjustment, ref bool ____sickFrame, ref bool ____shirtDirty, ref bool ____spriteDirty, ref LocalizedContentManager ___farmerTextureManager, ref Dictionary<string, Dictionary<int, List<int>>> ____recolorOffsets, ref Texture2D ___baseTexture, ref string ___textureName, SpriteBatch b, FarmerSprite.AnimationFrame animationFrame, int currentFrame, Rectangle sourceRect, Vector2 position, Vector2 origin, float layerDepth, int facingDirection, Color overrideColor, float rotation, float scale, Farmer who)
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
                pbe.dirty = true;

                if (who.accessory.Value < 6 && who.accessory.Value > 0)
                {
                    who.modData["DB.beard"] = "Vanilla's Accessory " + (who.accessory.Value + 1).ToString();
                    who.accessory.Set(0);
                }
            }

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
                if (pbe.shirt != who.GetShirtIndex())
                {
                    pbe.shirt = who.GetShirtIndex();
                    if (who.shirtItem.Value == null)
                    {
                        pbe.sleeveLength = "Sleeveless";
                    }
                    else
                    {
                        pbe.sleeveLength = ModEntry.AssignShirtLength(who.shirtItem.Value as Clothing, who.IsMale);
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


                //Redraw the image
                pbe.cacheImage = null;
                //Wipe the skin colour calculations
                ____recolorOffsets = null;
                //Wipe hair calculations
                pbe.ResetHairTextures();
                //Wipe naked overlays
                pbe.nakedLower.texture = null;
                pbe.nakedUpper.texture = null;
                //recalculate the recolouring parts
                __instance.MarkSpriteDirty();
            }

            //Draw the character
            bool sick_frame = currentFrame == 104 || currentFrame == 105;
            if (____sickFrame != sick_frame)
            {
                ____sickFrame = sick_frame;
                ____shirtDirty = true;
                ____spriteDirty = true;
            }

            ExecuteRecolorActionsReversePatch(__instance, who);
            AdjustedVanillaMethods.drawBase(__instance, ref ___rotationAdjustment, ref ___positionOffset, ref ___baseTexture, b, animationFrame, currentFrame, ref sourceRect, ref position, origin, layerDepth, facingDirection, overrideColor, rotation, scale, who);
            DrawBodyHair(__instance, ___positionOffset, ___rotationAdjustment, ___baseTexture, animationFrame, sourceRect, b, facingDirection, who, position, origin, scale, currentFrame, rotation, overrideColor, layerDepth);

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

            Texture2D nakedUpperTexture = pbe.GetNakedUpperTexture(who.skin.Value);
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
                    }
                    else
                    {

                        if (FarmerRenderer.isDrawingForUI)
                        {
                            //Change the frame for UI version
                            sourceRect.X = 0;
                            sourceRect.Y = 0;
                            b.Draw(nakedUpperTexture, position + origin + ___positionOffset, overlay_rect, overrideColor, rotation, origin, 4f * scale, flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth + layerOffset);
                        }
                        else
                        {
                            b.Draw(nakedUpperTexture, position + origin + ___positionOffset + animoffset, overlay_rect, overrideColor, rotation, origin, 4f * scale, flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth + layerOffset);
                        }
                    }
                }
            }


            bool drawPants = true;
            Texture2D nakedLowerTexture = pbe.GetNakedLowerTexture(who.skin.Value);
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
            if (drawPants) AdjustedVanillaMethods.drawPants(__instance, ref ___rotationAdjustment, ref ___positionOffset, ref ___baseTexture, b, animationFrame, currentFrame, sourceRect, position, origin, layerDepth, facingDirection, overrideColor, rotation, scale, who);
            if (nakedLowerTexture != null && pbe.nakedLower.CheckForOption("below accessories")) DrawLowerNaked(__instance, ___positionOffset, ___rotationAdjustment, ___baseTexture, animationFrame, sourceRect, b, facingDirection, who, position, origin, scale, currentFrame, rotation, overrideColor, layerDepth, 9.2E-08f, 9.2E-08f);
            AdjustedVanillaMethods.drawEyes(__instance, ref ___rotationAdjustment, ref ___positionOffset, ref ___baseTexture, b, animationFrame, currentFrame, sourceRect, position, origin, layerDepth, facingDirection, overrideColor, rotation, scale, who);
            __instance.drawHairAndAccesories(b, facingDirection, who, position, origin, scale, currentFrame, rotation, overrideColor, layerDepth);
            AdjustedVanillaMethods.drawArms(__instance, ref ___rotationAdjustment, ref ___positionOffset, ref ___baseTexture, b, animationFrame, currentFrame, sourceRect, position, origin, layerDepth, facingDirection, overrideColor, rotation, scale, who);
            
            //prevent further rendering
            return false;
        }

        internal static void ExecuteRecolorActionsReversePatch(FarmerRenderer __instance, Farmer who)
        {
            new NotImplementedException("It's a stub!");
        }

        private static void DrawBodyHair(FarmerRenderer __instance, Vector2 ___positionOffset, Vector2 ___rotationAdjustment, Texture2D ___baseTexture, FarmerSprite.AnimationFrame animationFrame, Rectangle sourceRect, SpriteBatch b, int facingDirection, Farmer who, Vector2 position, Vector2 origin, float scale, int currentFrame, float rotation, Color overrideColor, float layerDepth)
        {
            PlayerBaseExtended pbe = PlayerBaseExtended.Get(who);

            if (!pbe.bodyHair.OptionMatchesModData(who))
            {
                pbe.bodyHair.SetOptionFromModData(who, ModEntry.bodyHairOptions);
            }

            if (pbe.bodyHair.option != "Default")
            {
                //Draw the body hair
                b.Draw(pbe.GetBodyHairTexture(who), position + origin + ___positionOffset, sourceRect, Color.White, rotation, origin, 4f * scale, animationFrame.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth + ((who.FarmerSprite.CurrentAnimationFrame.frame == 5) ? 0.00072f : 7.2E-08f));
            }
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
                Texture2D nakedOverlayTexture = pbe.GetNakedLowerTexture(who.skin.Value);

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
                    AdjustedVanillaMethods.DrawShirt(__instance, FarmerRenderer.shirtsTexture, ___positionOffset, ___rotationAdjustment, ref ___shirtSourceRect, b, facingDirection, who, position, origin, scale, currentFrame, rotation, overrideColor, layerDepth);

                    //layerDepth += 1.4E-07f;
                    if (who.modData.ContainsKey("DB.overallColor") && who.modData["DB.overallColor"] == "true")
                    {
                        if (who.pants.Value != 14 && who.pantsItem.Get() != null && who.pantsItem.Get().dyeable)
                        {
                            //Draw the tinted overalls/highwaisted pants
                            Texture2D overalls_texture = Game1.content.Load<Texture2D>("Mods/ribeena.dynamicbodies/assets/Character/shirts_overlay.png");

                            if (pbe.shirtOverlayIndex >= 0)
                            {
                                AdjustedVanillaMethods.DrawShirt(__instance, overalls_texture, ___positionOffset, ___rotationAdjustment, ref ___shirtSourceRect, b, facingDirection, who, position, origin, scale, currentFrame, rotation, overrideColor.Equals(Color.White) ? Utility.MakeCompletelyOpaque(who.GetPantsColor()) : overrideColor, layerDepth + 1.8E-07f + 1.4E-07f, false, pbe.shirtOverlayIndex);

                            }
                            else
                            {

                                AdjustedVanillaMethods.DrawShirt(__instance, overalls_texture, ___positionOffset, ___rotationAdjustment, ref ___shirtSourceRect, b, facingDirection, who, position, origin, scale, currentFrame, rotation, overrideColor.Equals(Color.White) ? Utility.MakeCompletelyOpaque(who.GetPantsColor()) : overrideColor, layerDepth + 1.8E-07f + 1.4E-07f, false);
                            }
                        }
                    }
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

                //Draw Vanilla accessories
                if ((int)who.accessory.Value >= 6)
                {
                    AdjustedVanillaMethods.DrawAccessory(__instance, ___positionOffset, ___rotationAdjustment, ref ___accessorySourceRect, b, facingDirection, who, position, origin, scale, currentFrame, rotation, overrideColor, layerDepth);
                }

                ///////////////////////////////
                /// Setup overlay for rendering new two tone hair
                /// 
                int hair_style = who.getHair(); // (ignore_hat: true);
                HairStyleMetadata hair_metadata = Farmer.GetHairStyleMetadata(who.hair.Value);
                if (who != null && who.hat.Value != null && who.hat.Value.hairDrawType.Value == 1 && hair_metadata != null && hair_metadata.coveredIndex != -1)
                {
                    hair_style = hair_metadata.coveredIndex;
                    hair_metadata = Farmer.GetHairStyleMetadata(hair_style);
                }
                Rectangle hairstyleSourceOriginalRect = new Rectangle(hair_style * 16 % FarmerRenderer.hairStylesTexture.Width, hair_style * 16 / FarmerRenderer.hairStylesTexture.Width * 96, 16, 32 * 3);

                Texture2D hair_texture;
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

                //Cached and recoloured hair only has the one version
                Rectangle hairstyleSourceRect = new Rectangle(0, 0, 16, 32);

                float hair_draw_layer = ModEntry.hairlayer; //2.25E-05f //2.2E-05f;

                //float base_layer = layerDepth;

                if (FarmerRenderer.isDrawingForUI)
                {
                    //hair_draw_layer = 1.15E-07f;
                    layerDepth = 0.7f;
                    //context.Monitor.Log($"UI layer is [{layerDepth}].", LogLevel.Debug);
                    facingDirection = 2;

                }

                bool flip = false;
                switch (facingDirection)
                {
                    case 0:
                        hairstyleSourceRect.Offset(0, 64);
                        b.Draw(hair_texture, position + origin + ___positionOffset + new Vector2(FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4, FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4 + 4 + ((who.IsMale && hair_style >= 16) ? (-4) : ((!who.IsMale && hair_style < 16) ? 4 : 0))), hairstyleSourceRect, Color.White, rotation, origin, 4f * scale, SpriteEffects.None, layerDepth + hair_draw_layer * (float)sort_direction);
                        break;
                    case 1:
                        hairstyleSourceRect.Offset(0, 32);
                        b.Draw(hair_texture, position + origin + ___positionOffset + new Vector2(FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4, FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4 + ((who.IsMale && (int)who.hair.Value >= 16) ? (-4) : ((!who.IsMale && (int)who.hair.Value < 16) ? 4 : 0))), hairstyleSourceRect, Color.White, rotation, origin, 4f * scale, SpriteEffects.None, layerDepth + hair_draw_layer * (float)sort_direction);

                        break;
                    case 2:
                        b.Draw(hair_texture, position + origin + ___positionOffset + new Vector2(FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4, FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4 + ((who.IsMale && (int)who.hair.Value >= 16) ? (-4) : ((!who.IsMale && (int)who.hair.Value < 16) ? 4 : 0))), hairstyleSourceRect, Color.White, rotation, origin, 4f * scale, SpriteEffects.None, layerDepth + hair_draw_layer * (float)sort_direction);
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
                        b.Draw(hair_texture, position + origin + ___positionOffset + new Vector2(-FarmerRenderer.featureXOffsetPerFrame[currentFrame] * 4, FarmerRenderer.featureYOffsetPerFrame[currentFrame] * 4 + ((who.IsMale && (int)who.hair.Value >= 16) ? (-4) : ((!who.IsMale && (int)who.hair.Value < 16) ? 4 : 0))), hairstyleSourceRect, Color.White, rotation, origin, 4f * scale, flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth + hair_draw_layer * (float)sort_direction);
                        break;
                }

                //Draw naked
                if (!pbe.nakedLower.CheckForOption("below accessories")) DrawLowerNaked(__instance, ___positionOffset, ___rotationAdjustment, ___baseTexture, animationFrame, sourceRect, b, facingDirection, who, position, origin, scale, currentFrame, rotation, overrideColor, layerDepth, ModEntry.FS_pantslayer, 5.95E-05f);

                //Draw the Vanilla hat
                if (who.hat.Value != null && !who.bathingClothes.Value)
                {
                    AdjustedVanillaMethods.DrawHat(__instance, ___positionOffset, ref ___hatSourceRect, b, facingDirection, who, position, origin, scale, currentFrame, rotation, layerDepth);
                }

            }
            //prevent further rendering
            return false;

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

            //Draw the base
            b.Draw(___baseTexture, position + new Vector2(0f, (who.IsMale ? 0 : -4)) * scale / 4f, new Rectangle(0, 0, 16, who.isMale ? 15 : 16), Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
            int sort_direction = ((!Game1.isUsingBackToFrontSorting) ? 1 : (-1));

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
            int hair_style = who.getHair(ignore_hat: true);
            HairStyleMetadata hair_metadata = Farmer.GetHairStyleMetadata(who.hair.Value);

            Texture2D hair_texture;
            Rectangle hairstyleSourceOriginalRect = new Rectangle(hair_style * 16 % FarmerRenderer.hairStylesTexture.Width, hair_style * 16 / FarmerRenderer.hairStylesTexture.Width * 96, 16, 32 * 3);

            if (hair_metadata != null)
            {
                hairstyleSourceOriginalRect = new Rectangle(hair_metadata.tileX * 16, hair_metadata.tileY * 16, 16, 32);
                hair_texture = pbe.GetHairTexture(who, hair_style, hair_metadata.texture, hairstyleSourceOriginalRect);
            }
            else
            {
                hair_texture = pbe.GetHairTexture(who, hair_style, FarmerRenderer.hairStylesTexture, hairstyleSourceOriginalRect);
            }
            //Cached hair just has the one style, and we only need the top part of the hair (15 pixels)
            Rectangle hairstyleSourceRect = new Rectangle(0, 0, 16, 15);

            b.Draw(hair_texture, position + new Vector2(0f, (who.IsMale ? 0 : -4)) * scale / 4f + new Vector2(0f, feature_y_offset * 4 + ((who.IsMale && (int)who.hair >= 16) ? (-4) : ((!who.IsMale && (int)who.hair < 16) ? 4 : 0))) * scale / 4f, hairstyleSourceRect, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth + 1.1E-07f * (float)sort_direction);
            //prevent further rendering
            return false;
        }
    }
}
