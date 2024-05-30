/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FlyingTNT/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using static StardewValley.FarmerSprite;

namespace Swim
{
    /// <summary>
    /// A set of patches that fix the Farmer's animations when swimming, and when in a bathing suit.
    /// 
    /// In order to make this code somewhat maintaiable, I am going to try to document it very well. Here, I am going to give an overview of the code. I am going to start with how the game
    /// manages animating the farmer and then I will discuss the problems I encountered as I was making this and the ways in which this class solves them.
    /// 
    /// 1. Overview of rendering
    /// The actual rendering of the Farmer happens in FarmerRenderer.draw, so that is the focus of most of the patches.
    /// The rendering is split into pieces - the FarmerRenderer draws the body and pants and hair and eyes etc. sepatately
    /// Each frame of an animation is represented by an AnimationFrame object, which defines things like whether the frame should be flipped, if the player should be offset,...
    /// Each frame has a frame number, and there are 126 animation frames.
    /// Nine of these frames correspond to bathing suit frames - three in each direction (right is just left but flipped, or maybe vice versa)
    /// The frame number dictates which farmer body and arm sprites are drawn
    /// The farmer's current pants item dictates which pants are drawn, the shirt item the shirt, etc
    /// Within each pants texture, there are 126 individual sprites (not the shirt; they're done differently), each of which corresponds to an animation frame.
    /// The bathing suit differs from the other frames in two very significant ways, which are 100% of the reason I had to make this 1000-lines-of-code file:
    /// (i) The entire bathing suit is on the pants sprite sheet - when the game draws the bathing suit, it draws the whole thing as the pants and no shirt
    ///    - Of those 126 sprites on each pants texture, 9 of them are the bathing suit
    ///    - The player's shirt and hat are not displayed when the player is wearing their bathing clothes (even the bikini part of the bthing suit is in the pants texture)
    /// (ii) The bathing suit body model has the arms included in it (no arms are drawn), while all other models are armless and have the arms drawn separately
    /// 
    /// 2. Problems and solutions
    /// To start, there are two problems that predate this file:
    /// 
    /// (i) Footsteps display while swimming
    ///  - aedenthorn added a prefix to FarmerRenderer.checkForFootstep to prevent footprints from displaying when the player is swimming
    ///  
    /// (ii) The scuba mask is not displayed while swimming
    ///  - In general, no hats are displayed while swimming, but it would make sense for the scuba mask to be displyed
    ///  - Notably, the above isn't actually true; when swimming, the player's body below the neck and their pants aren't displayed
    ///  - However, their shirt and hat are only not displayed when they are wearing bathing clothes
    ///  - This makes sense when you remember bathing clothes are displayed as pants
    ///  - As such, aedenthorn solved this problem by forcing the player to not wear bathing clothes when they were wearing the scuba gear
    ///  - This worked, but it also caused the player's shirt to be displayed in addition to their mask, causing a limbless torso to appear while they swam
    ///  - I didn't like this, so I added a transpiler to FarmerRenderer.drawHairAndAccesories (where the player's hat and shirt are drawn)
    ///    - This transplier modified the check that prevented the hat from displaying when the player was wearing a bathing suit so that it is tied to a config option
    ///    - This allows the player to wear hats while they swim (I was originally going to do just the mask, but I thought support for all hats was better)
    /// 
    /// Then, the problem that prompted the creation of this file: 
    /// When the player performs any action that isn't walking while wearing a bathing suit, the bathing suit is not displayed.
    /// As mentioned above, of the 126 animation frames only 9 correspond to the bathing suit. Notably, this means that the each pant in the pants texture sheet has 126 textures, 9 of which
    /// are the bathing suit. If the current frame index is a bathing suit frame, the bating suit is displayed as the pants. If the index corresponds to any other frame (such as swinging a tool
    /// or running), the normal pants are displayed (look at the pants texture sheet if you don't understand; the nine bathing suit frames are in the bottom left of each pants item). For example, say
    /// my pants item is a skirt and I am currently wearing a bathing suit: while I walk around, the bathing suit animation shows just fine. However, if I use my axe or grab a forage or do anything
    /// else the skirt is displayed instead of the bathing suit. Additionally, because I am still technically wearing a bathing suit, my shirt is not displayed. As such, it looks like I am wearing
    /// a skirt and no shirt, while it should ideally still display the bathing suit.
    /// 
    /// To solve this, I added a prefix to FarmerRenderer.draw that takes the frame index it's drawing and maps it through AnimationFrameToBathingSuitFrameMap.
    /// AnimationFrameToBathingSuitFrameMap is an array whose indicies are the 126 animation frames and whose values are the indicies of the bathing suit animation frame I thought best represented the frame. 
    /// Some frames are unmapped, either because I didn't think a bathing suit frame would do them justice, or because I simply haven't encountered them yet in my testing.
    /// This solution is pretty good - the player's bathing suit doesn't 'fall off' (except in the unmapped frames), and the animations are generally pretty reasonable. However there is one flaw:
    /// as I mentioned earlier, the animation frame index dictates three things: which player body sprite is drawn, which pant sprite is drawn, and which arm sprite is drawn. The map does fix the first
    /// two - we want the bathing suit pants and body to be drawn. However, normally no arms are drawn when drawing the bathing suit, so when say using a tool, it looks like the tool is being controlled
    /// by telepathy. In fact, it is slightly worse because of the specifics of how the player body spritesheet is laid out: in general, each body frame's arms are located to the right of it in the spritesheet.
    /// Which specific arm is to be drawn is specified by an AnimationFrame's armOffset parameter (corresponds to the number of frames to the right to look). Note that we do not patch this value in the prefix
    ///  - because each bathing suit has its arms baked in to the body sprite, its arm sprite is blank and so it doesn't matter. However, ConcernedApe has made use of the fact that the bathing suits don't
    /// need arm frames by storing the pan arm frames in the bathing suit arm slots. (this is necessary because there needs to be a frame for each quality of pan, and that is too many frames to fit in the 
    /// normal allotted space. Normally this isn't a problem because the bathing suit frames' arm offsets are usually set to -1 (less than zero offsets are not drawn). However, because these frames we
    /// are mapping were not originally meant to be bathing suit frames, their arm offsets are usually not -1. This has the effect of causing the player to look like they are holding a pan behind their
    /// back when they are walking upward. We fix this by also setting the set the arm offset to -1 if the frame is a bathing suit frame in the draw prefix.
    /// In all, this makes it so that the player's bathing suit doesn't fall off, although their arms are always at their sides no matter the frame, so frames in which they are using tools look kind of wonky.
    /// 
    /// At this point, I will mention that these patches are divided into three levels, which are configuralble with a config option. Level 0 is just the old patches that predate this file - the patch to prevent
    /// footsteps while swimming and the pach to allow hats while in the bathing suit. Level 1 includes those patches, plus the AnimationFrame constructor prefix and postfix I just mentioned. Level 2 includes the
    /// level 0 patches, plus a set of fixes that allows the correct arms to be displayed when wearing the bathing suit, which I will get into shortly. The reason I made this a config option is compatibility - the
    /// level 0 patches are pretty tame and unliely to break compatibility with anything. The level 1 patches are more intrusive, but I feel they're still pretty safe. However, the level 2 patches really go off
    /// the deep end. They transpile FarmerRenderer methods in like 45 additional places (although, 43 of those are just the same two pathches over and over, and I have plans to minimize their footprints), and
    /// they add fairly intrusive pre- and post- fixes to FarmerRenderer.draw. Additionally, the pathces that allow the arms to be drawn will not work natively with mods that add custom bathing suit sprites,
    /// although as I will talk about later, adding support should be fairly simple on their end and it's not like it would crash; just in frames where we are fixing the arms, the custom bathing suit would not 
    /// display.
    /// 
    /// So without further ado, the level 2 patches:
    /// First of all, we are not still mapping the animation frames directly because we need to preserve what arms we should draw. As such, we need a different way of making sure the game draws
    /// the bathing suit pants and body. We do this by adding a transpiler to FarmerRender.draw. The game decides which pants/body to draw using a Rectangle which defines the location of the sprite
    /// in the spriteSheet. Normally, this is based on the frame index, but our transpiler changes that Rectangle to be the correct bathing suit rectangle. We ues getSwimSourceRectangle for the body
    /// and getSwimSourceRectanglePant for the pants. This perserves the arm index while also drawing the correct body, so when the player uses a tool etc. their arms are actually visible. However,
    /// this introcudes a problem: the player's feature offset (the position of their hair and earrings relative to their body) is also based on the frame index, so now the player's hair moves
    /// independently of their body, which looks odd to say the least. All of these offsets are stored in FarmerRenderer.featureXOffsetPerFrame (and one for the Y offset), which are arrays that map 
    /// each animation frame to its offset. We replace all references to this with calls to our own MapFarmerRendererFeatureXOffset() and Y. If the player is not wearing bathing clothes, this just
    /// returns FarmerRenderer.featureXOffsetPerFrame, but if they are, it returns FarmerRendererFeatureXOffsetPerFrameMapped, which is just FarmerRenderer.featureXOffsetPerFrame but mapped through
    /// AnimationFrameToBathingSuitFrameMap. We replace all instances of these in FarmerRenderer.draw and FarmerRenderer.drawHairAndAccesories (there are like 40!) in our transpilers. Now the player's
    /// bathing suit, body, hair, and arms are all in the correct position. However, there is one last problem: as I mentioned earlier, the player's arms for the bathing suit animation are included 
    /// in the bathing suit sprite itself. As such, when the player is using a tool and whatnot, their arms are not only shown swimging the tool, but also hanging at their sides.
    /// </summary>
    internal class AnimationManager
    {
        private static IMonitor SMonitor;
        private static ModConfig Config;
        private static IModHelper SHelper;

        private static readonly PerScreen<Farmer> currentlyDrawingFarmer = new PerScreen<Farmer>();
        private static Dictionary<long, FarmerTextureState> TextureStateDictionary = new();

        const int LegacyPatches = 0;
        const int MediumPatches = 1;
        const int AllPatches = 2;

        public static void Initialize(IMonitor monitor, IModHelper helper, ModConfig config)
        {
            SMonitor = monitor;
            Config = config;
            SHelper = helper;
        }

        public static void EditAssets(object sender, AssetRequestedEventArgs e)
        {
            if (Config.AnimationPatches != AllPatches)
                return;

            if(e.NameWithoutLocale.StartsWith("FlyingTNT.Swim/DisarmedModels"))
            {
                string[] path = SplitPath(e.NameWithoutLocale.BaseName);

                if (e.DataType == typeof(Texture2D))
                {
                    e.LoadFrom(() => Game1.content.Load<Texture2D>("Characters/Farmer" + PathUtilities.PreferredAssetSeparator + path[2]), AssetLoadPriority.Medium);
                }
                else
                {
                    SMonitor.Log($"Error loading file {"FlyingTNT.Swim/DisarmedModels/" + path[2]}! Invalid type ({e.DataType})", LogLevel.Error);
                    return;
                }

                e.Edit(asset =>
                {
                    IAssetDataForImage image = asset.AsImage();
                    UnarmBathingModels(image, path[2].Contains("girl"));
                });
            }else if(e.NameWithoutLocale.IsEquivalentTo("Characters/Farmer/Pants"))
            {
                e.Edit(asset =>
                {
                    IAssetDataForImage image = asset.AsImage();
                    UpdateBathingSuits(image);
                });
            }else if(e.NameWithoutLocale.IsEquivalentTo("FlyingTNT.Swim/GirlSwimsuitArmless"))
            {
                if (e.DataType == typeof(Texture2D))
                {
                    e.LoadFromModFile<Texture2D>("assets/GirlSwimsuitArmless.png", AssetLoadPriority.Low);
                }
                else if (e.DataType == typeof(IRawTextureData))
                {
                    e.LoadFromModFile<IRawTextureData>("assets/GirlSwimsuitArmless.png", AssetLoadPriority.Low);
                }
                else
                {
                    SMonitor.Log($"Error loading file FlyingTNT.Swim/GirlSwimsuitArmless! Invalid type ({e.DataType})", LogLevel.Error);
                    return;
                }
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("FlyingTNT.Swim/BoySwimsuitArmless"))
            {
                if (e.DataType == typeof(Texture2D))
                {
                    e.LoadFromModFile<Texture2D>("assets/BoySwimsuitArmless.png", AssetLoadPriority.Low);
                }
                else if (e.DataType == typeof(IRawTextureData))
                {
                    e.LoadFromModFile<IRawTextureData>("assets/BoySwimsuitArmless.png", AssetLoadPriority.Low);
                }
                else
                {
                    SMonitor.Log($"Error loading file FlyingTNT.Swim/BoySwimsuitArmless! Invalid type ({e.DataType})", LogLevel.Error);
                    return;
                }
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("FlyingTNT.Swim/PlayerBaseBoyArmless"))
            {
                if (e.DataType == typeof(Texture2D))
                {
                    e.LoadFromModFile<Texture2D>("assets/PlayerBaseBoyArmless.png", AssetLoadPriority.Low);
                }
                else if (e.DataType == typeof(IRawTextureData))
                {
                    e.LoadFromModFile<IRawTextureData>("assets/PlayerBaseBoyArmless.png", AssetLoadPriority.Low);
                }
                else
                {
                    SMonitor.Log($"Error loading file FlyingTNT.Swim/PlayerBaseBoyArmless! Invalid type ({e.DataType})", LogLevel.Error);
                    return;
                }
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("FlyingTNT.Swim/PlayerBaseGirlArmless"))
            {
                if (e.DataType == typeof(Texture2D))
                {
                    e.LoadFromModFile<Texture2D>("assets/PlayerBaseGirlArmless.png", AssetLoadPriority.Low);
                }
                else if (e.DataType == typeof(IRawTextureData))
                {
                    e.LoadFromModFile<IRawTextureData>("assets/PlayerBaseGirlArmless.png", AssetLoadPriority.Low);
                }
                else
                {
                    SMonitor.Log($"Error loading file FlyingTNT.Swim/PlayerBaseGirlArmless! Invalid type ({e.DataType})", LogLevel.Error);
                    return;
                }
            }
        }

        public static void Patch(Harmony harmony)
        {
            // Always run
            harmony.Patch(
               original: AccessTools.Method(typeof(FarmerSprite), "checkForFootstep"),
               prefix: new HarmonyMethod(typeof(AnimationManager), nameof(FarmerSprite_checkForFootstep_Prefix))
            );

            // Always run
            harmony.Patch(
               original: AccessTools.Method(typeof(FarmerRenderer), nameof(FarmerRenderer.drawHairAndAccesories)),
               transpiler: new HarmonyMethod(typeof(AnimationManager), nameof(FarmerRenderer_drawHairAndAccessories_Transpiler))
            );
            
            if(Config.AnimationPatches == AllPatches)
            {
                harmony.Patch(
                   original: AccessTools.Method(typeof(FarmerRenderer), nameof(FarmerRenderer.draw), new Type[] { typeof(SpriteBatch), typeof(FarmerSprite.AnimationFrame), typeof(int), typeof(Rectangle), typeof(Vector2), typeof(Vector2), typeof(float), typeof(int), typeof(Color), typeof(float), typeof(float), typeof(Farmer) }),
                   prefix: new HarmonyMethod(typeof(AnimationManager), nameof(FarmerRenderer_draw_AllPatches_Prefix)),
                   postfix: new HarmonyMethod(typeof(AnimationManager), nameof(FarmerRenderer_draw_AllPatches_Postfix)),
                   transpiler: new HarmonyMethod(typeof(AnimationManager), nameof(FarmerRenderer_draw_Transpiler))
                );

                harmony.Patch(
                    original: AccessTools.Method(typeof(FarmerRenderer), nameof(FarmerRenderer.drawHairAndAccesories)),
                    prefix: new HarmonyMethod(typeof(AnimationManager), nameof(FarmerRenderer_drawHairAndAccessories_Prefix))
                );
            }
            else if(Config.AnimationPatches == MediumPatches)
            {
                harmony.Patch(
                   original: AccessTools.Method(typeof(FarmerRenderer), nameof(FarmerRenderer.draw), new Type[] { typeof(SpriteBatch), typeof(FarmerSprite.AnimationFrame), typeof(int), typeof(Rectangle), typeof(Vector2), typeof(Vector2), typeof(float), typeof(int), typeof(Color), typeof(float), typeof(float), typeof(Farmer) }),
                   prefix: new HarmonyMethod(typeof(AnimationManager), nameof(FarmerRenderer_draw_MediumPatches_Prefix)),
                   postfix: new HarmonyMethod(typeof(AnimationManager), nameof(FarmerRenderer_draw_Postfix))
                );
            }
            else
            {
                harmony.Patch(
                   original: AccessTools.Method(typeof(FarmerRenderer), nameof(FarmerRenderer.draw), new Type[] { typeof(SpriteBatch), typeof(FarmerSprite.AnimationFrame), typeof(int), typeof(Rectangle), typeof(Vector2), typeof(Vector2), typeof(float), typeof(int), typeof(Color), typeof(float), typeof(float), typeof(Farmer) }),
                   prefix: new HarmonyMethod(typeof(AnimationManager), nameof(FarmerRenderer_draw_Prefix)),
                   postfix: new HarmonyMethod(typeof(AnimationManager), nameof(FarmerRenderer_draw_Postfix))
                );
            }

            if (Config.AnimationPatches == AllPatches)
            {
                harmony.Patch(
                    original: AccessTools.Method(typeof(Farmer), nameof(Farmer.GetDisplayPants)),
                    postfix: new HarmonyMethod(typeof(AnimationManager), nameof(Farmer_GetDisplayPants_Postfix))
                );

                harmony.Patch(
                    original: AccessTools.Constructor(typeof(FarmerRenderer), new Type[] { typeof(string), typeof(Farmer)}),
                    postfix: new HarmonyMethod(typeof(AnimationManager), nameof(FramerRenderer_Constructor_Postfix))
                );

                harmony.Patch(
                    original: AccessTools.Constructor(typeof(FarmerRenderer), new Type[] {}),
                    postfix: new HarmonyMethod(typeof(AnimationManager), nameof(FramerRenderer_Constructor_Postfix))
                );

                harmony.Patch(
                    original: AccessTools.Method(typeof(Farmer), nameof(Farmer.ShirtHasSleeves)),
                    postfix: new HarmonyMethod(typeof(AnimationManager), nameof(Farmer_ShirtHasSleeves_Postfix))
                );
            }
        }

        #region PATCHES

        public static void GameLoop_SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            if (Context.IsMainPlayer)
                TextureStateDictionary.Clear();
        }

        public ref struct FarmerRendererDrawState
        {
            public bool wasSwimming = false;
            public Texture2D oldTexture = null;
            public string oldTextureName = null;
            public bool wasSpriteDirty = false;
            public bool wasSkinDirty = false;
            public bool wasEyesDirty = false;
            public bool wasShirtDirty = false;
            public bool wasPantsDirty = false;
            public bool wasShoesDirty = false;
            public bool wasBaseTextureDirty = false;

            public FarmerRendererDrawState(){}
        }

        public static void FarmerRenderer_draw_AllPatches_Prefix(FarmerRenderer __instance, Farmer who, ref int currentFrame, ref AnimationFrame animationFrame, ref NetString ___textureName, ref bool ____spriteDirty, ref bool ____eyesDirty, ref bool ____skinDirty, ref bool ____shirtDirty, ref bool ____pantsDirty, ref bool ____shoesDirty, ref bool ____baseTextureDirty, ref FarmerRendererDrawState __state)
        {
            try
            {
                if (who.swimming.Value)
                {

                    if(Game1.player.currentLocation.Name.StartsWith("Custom_Underwater"))
                    {
                        who.swimming.Value = false;
                        __state.wasSwimming = true;
                    }
                    else
                    {
                        animationFrame.positionOffset = 0;
                    }
                }

                currentlyDrawingFarmer.Value = who;

                if (!who.bathingClothes.Value || FarmerRenderer.isDrawingForUI)
                    return;

                if (GetTextureState(who) is not FarmerTextureState textureState)
                {
                    MapAnimationFrameToBathingSuitAnimation(who, ref currentFrame);
                    return;
                }

                if (TryGetShouldUseArmless(currentFrame, out bool shouldUseArmless) && shouldUseArmless)
                {
                    string which = SplitPath(__instance.textureName.Value)[2];

                    string textureName = "FlyingTNT.Swim/DisarmedModels/" + which;

                    if(textureState.texture is null || textureState.texture.IsDisposed)
                    {
                        SMonitor.Log($"Reloading Screen {Context.ScreenId} Texture!");
                        textureState.texture = GetUniqueScreenTexture(textureName);
                    }

                    IReflectedField<Texture2D> baseTexture = SHelper.Reflection.GetField<Texture2D>(__instance, "baseTexture");
                    __state.oldTexture = baseTexture.GetValue();
                    baseTexture.SetValue(textureState.texture);

                    if(textureState.spriteDirty)
                    {
                        __state.wasSpriteDirty = ____spriteDirty;
                        __state.wasSkinDirty = ____skinDirty;
                        __state.wasEyesDirty = ____eyesDirty;
                        __state.wasShirtDirty = ____shirtDirty;
                        __state.wasPantsDirty = ____pantsDirty;
                        __state.wasShoesDirty = ____shoesDirty;
                        __state.wasBaseTextureDirty = ____baseTextureDirty;

                        __state.oldTextureName = ___textureName.Value;
                        ___textureName.Set(textureName);

                        SMonitor.Log("Sprite Dirty");
                        if (textureState.skinDirty)
                        {
                            SMonitor.Log("Skin Dirty");
                            ____skinDirty = true;
                            textureState.skinDirty = false;
                        }
                        if (textureState.eyesDirty)
                        {
                            SMonitor.Log("Eyes Dirty");
                            ____eyesDirty = true;
                            textureState.eyesDirty = false;
                        }
                        if (textureState.shirtDirty)
                        {
                            SMonitor.Log("Shirt Dirty");
                            ____shirtDirty = true;
                            textureState.shirtDirty = false;
                        }
                        if (textureState.pantsDirty)
                        {
                            SMonitor.Log("Pants Dirty");
                            ____pantsDirty = true;
                            textureState.pantsDirty = false;
                        }
                        if (textureState.shoesDirty)
                        {
                            SMonitor.Log("Shoes Dirty");
                            ____shoesDirty = true;
                            textureState.shoesDirty = false;
                        }
                        ____spriteDirty = true;
                        textureState.spriteDirty = false;
                    }
                    ____baseTextureDirty = false; // If it is true, it will undo our changes to the baseTexture.

                }
                else
                {
                    animationFrame.armOffset = 0;
                }

                MapAnimationFrameToBathingSuitAnimation(who, ref currentFrame);
            }
            catch (Exception ex)
            {
                SMonitor.Log($"Failed in {nameof(FarmerRenderer_draw_AllPatches_Prefix)}:\n{ex}", LogLevel.Error);
            }
        }

        internal static void FarmerRenderer_draw_AllPatches_Postfix(FarmerRenderer __instance, Farmer who, ref NetString ___textureName, ref bool ____spriteDirty, ref bool ____eyesDirty, ref bool ____skinDirty, ref bool ____shirtDirty, ref bool ____pantsDirty, ref bool ____shoesDirty, ref bool ____baseTextureDirty, FarmerRendererDrawState __state)
        {
            try
            {
                if (__state.wasSwimming)
                {
                    who.swimming.Value = true;
                }

                if (FarmerRenderer.isDrawingForUI)
                {
                    return;
                }

                if (__state.oldTexture is not null)
                {
                    SHelper.Reflection.GetField<Texture2D>(__instance, "baseTexture").SetValue(__state.oldTexture);
                }

                if(__state.oldTextureName is not null)
                {
                    ___textureName.Set(__state.oldTextureName);
                    ____spriteDirty = __state.wasSpriteDirty;
                }

                if(__state.wasEyesDirty)
                {
                    ____eyesDirty = true;
                }

                if (__state.wasSkinDirty)
                {
                    ____skinDirty = true;
                }
                
                if (__state.wasShirtDirty)
                {
                    ____shirtDirty = true;
                }
                
                if (__state.wasPantsDirty)
                {
                    ____pantsDirty = true;
                }
                
                if (__state.wasShoesDirty)
                {
                    ____shoesDirty = true;
                }

                ____baseTextureDirty = __state.wasBaseTextureDirty;
            }
            catch (Exception ex)
            {
                SMonitor.Log($"Failed in {nameof(FarmerRenderer_draw_AllPatches_Postfix)}:\n{ex}", LogLevel.Error);
            }
        }

        public static void FarmerRenderer_draw_MediumPatches_Prefix(Farmer who, ref FarmerSprite.AnimationFrame animationFrame, ref int currentFrame, ref Rectangle sourceRect, ref bool __state)
        {
            try
            {
                if (who.swimming.Value)
                {
                    if (Game1.player.currentLocation.Name.StartsWith("Custom_Underwater"))
                    {
                        who.swimming.Value = false;
                        __state = true;
                    }
                    else
                    {
                        animationFrame.positionOffset = 0;
                    }
                }

                if (!who.bathingClothes.Value || FarmerRenderer.isDrawingForUI)
                    return;

                if (MapAnimationFrameToBathingSuitAnimation(who, ref currentFrame))
                {
                    animationFrame.frame = currentFrame;
                    animationFrame.armOffset = 0;
                    sourceRect = new Rectangle(currentFrame * who.FarmerSprite.SpriteWidth % 96, currentFrame * who.FarmerSprite.SpriteWidth / 96 * who.FarmerSprite.SpriteHeight, who.FarmerSprite.SpriteWidth, who.FarmerSprite.SpriteHeight);
                }
            }
            catch (Exception ex)
            {
                SMonitor.Log($"Failed in {nameof(FarmerRenderer_draw_MediumPatches_Prefix)}:\n{ex}", LogLevel.Error);
            }
        }

        public static void FarmerRenderer_draw_Prefix(Farmer who, ref bool __state)
        {
            try
            {
                if (who.swimming.Value && Game1.player.currentLocation.Name.StartsWith("Custom_Underwater"))
                {
                    who.swimming.Value = false;
                    __state = true;
                }
            }
            catch (Exception ex)
            {
                SMonitor.Log($"Failed in {nameof(FarmerRenderer_draw_Prefix)}:\n{ex}", LogLevel.Error);
            }
        }

        internal static void FarmerRenderer_draw_Postfix(Farmer who, bool __state)
        {
            try
            {
                if (__state)
                {
                    who.swimming.Value = true;
                }

            }
            catch (Exception ex)
            {
                SMonitor.Log($"Failed in {nameof(FarmerRenderer_draw_Postfix)}:\n{ex}", LogLevel.Error);
            }
        }

        static void FramerRenderer_Constructor_Postfix(NetColor ___eyes, NetInt ___skin, NetString ___shoes, NetString ___shirt, NetString ___pants)
        {
            try
            {
                ___eyes.fieldChangeVisibleEvent += delegate
                {
                    if (GetTextureState(Game1.player) is not FarmerTextureState state)
                    {
                        return;
                    }
                    state.spriteDirty = true;
                    state.eyesDirty = true;
                };
                ___skin.fieldChangeVisibleEvent += delegate
                {
                    if (GetTextureState(Game1.player) is not FarmerTextureState state)
                    {
                        return;
                    }
                    state.spriteDirty = true;
                    state.skinDirty = true;
                    state.shirtDirty = true;
                };
                ___shoes.fieldChangeVisibleEvent += delegate
                {
                    if (GetTextureState(Game1.player) is not FarmerTextureState state)
                    {
                        return;
                    }
                    state.spriteDirty = true;
                    state.shoesDirty = true;
                };
                ___shirt.fieldChangeVisibleEvent += delegate
                {
                    if (GetTextureState(Game1.player) is not FarmerTextureState state)
                    {
                        return;
                    }
                    state.spriteDirty = true;
                    state.shirtDirty = true;
                };
                ___pants.fieldChangeVisibleEvent += delegate
                {
                    if (GetTextureState(Game1.player) is not FarmerTextureState state)
                    {
                        return;
                    }
                    state.spriteDirty = true;
                    state.pantsDirty = true;
                };
            }
            catch (Exception ex)
            {
                SMonitor.Log($"Failed in {nameof(FramerRenderer_Constructor_Postfix)}:\n{ex}", LogLevel.Error);
            }
        }

        public static bool FarmerSprite_checkForFootstep_Prefix()
        {
            try
            {
                if (Game1.player.swimming.Value)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                SMonitor.Log($"Failed in {nameof(FarmerSprite_checkForFootstep_Prefix)}:\n{ex}", LogLevel.Error);
            }
            return true;
        }

        public static void Farmer_GetDisplayPants_Postfix(Farmer __instance, ref int spriteIndex)
        {
            try
            {
                if (!__instance.bathingClothes.Value || FarmerRenderer.isDrawingForUI)
                    return;

                spriteIndex = TryGetShouldUseArmless(__instance.FarmerSprite.CurrentFrame, out bool shouldUseArmless) && shouldUseArmless ? 0 : 1;
            }
            catch (Exception ex)
            {
                SMonitor.Log($"Failed in {nameof(Farmer_GetDisplayPants_Postfix)}:\n{ex}", LogLevel.Error);
            }
        }


        /// <summary>
        /// Patches the check in FarmerRenderer.drawHairAndAccessories that causes the player's hat to not be drawn when they are wearing a bathing suit.
        /// Instead, replaces that with a call to SwimUitls.ShouldNotDrawHat.
        /// </summary>
        /// <param name="instructions"></param>
        /// <returns></returns>
        public static IEnumerable<CodeInstruction> FarmerRenderer_drawHairAndAccessories_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            /*
             * 
             * 0  IL_0e8d: ldarg.3
	         * 1  IL_0e8e: ldfld class Netcode.NetRef`1<class StardewValley.Objects.Hat> StardewValley.Farmer::hat
	         * 2  IL_0e93: callvirt instance !0 class Netcode.NetFieldBase`2<class StardewValley.Objects.Hat, class Netcode.NetRef`1<class StardewValley.Objects.Hat>>::get_Value()
	         * 3  IL_0e98: brfalse IL_116d
             *
             *    // We want to edit this section so that instead of checking farmer.bathingClothes, it calls ShouldNotDrawHat()
	         * 4  IL_0e9d: ldarg.3 <- keep (it loads the Farmer object onto the stack)
	         * 5  IL_0e9e: ldfld class Netcode.NetBool StardewValley.Farmer::bathingClothes <- delete (we only need 3 instructions)
	         * 6  IL_0ea3: call bool Netcode.NetBool::op_Implicit(class Netcode.NetBool) <- replace operand with ShouldNotDrawHat()
	         * 7  IL_0ea8: brtrue IL_116d <- keep
             *
	         * 8  IL_0ead: ldarg.3
	         * 9  IL_0eae: callvirt instance class StardewValley.FarmerSprite StardewValley.Farmer::get_FarmerSprite()
	         * 10 IL_0eb3: callvirt instance valuetype StardewValley.FarmerSprite/AnimationFrame StardewValley.FarmerSprite::get_CurrentAnimationFrame()
	         * 11 IL_0eb8: ldfld bool StardewValley.FarmerSprite/AnimationFrame::flip
             */

            try
            {
                SMonitor.Log($"Transpiling FarmerRenderer.drawHairAndAccessories");

                bool hat = true;

                // We want codes[i] to be that first ldarg. We are going to check every instruction shown above because for this method, this code is kind of generic (similar checks happen multiple times)
                for (int i = 0; i < codes.Count; i++)
                {
                    if (hat && codes[i].opcode == OpCodes.Ldarg_3 && codes[i + 1].opcode == OpCodes.Ldfld && codes[i + 2].opcode == OpCodes.Callvirt && codes[i + 3].opcode == OpCodes.Brfalse &&
                        codes[i + 4].opcode == OpCodes.Ldarg_3 && codes[i + 5].opcode == OpCodes.Ldfld && codes[i + 6].opcode == OpCodes.Call && codes[i + 7].opcode == OpCodes.Brtrue &&
                        codes[i + 8].opcode == OpCodes.Ldarg_3 && codes[i + 9].opcode == OpCodes.Callvirt && codes[i + 10].opcode == OpCodes.Callvirt && codes[i + 11].opcode == OpCodes.Ldfld &&
                        (FieldInfo)codes[i + 5].operand == AccessTools.Field(typeof(Farmer), nameof(Farmer.bathingClothes)) && (MethodInfo)codes[i + 6].operand == AccessTools.Method(typeof(NetBool), "op_Implicit") &&
                        (FieldInfo)codes[i + 11].operand == AccessTools.Field(typeof(FarmerSprite.AnimationFrame), nameof(FarmerSprite.AnimationFrame.flip)))
                    {
                        SMonitor.Log("Adding hat patch!");
                        codes[i + 5].opcode = OpCodes.Nop;
                        codes[i + 5].operand = null;

                        codes[i + 6].operand = AccessTools.Method(typeof(AnimationManager), nameof(ShouldNotDrawHat));

                        hat = false;
                    }
                }
            }
            catch (Exception ex)
            {
                SMonitor.Log($"Failed in {nameof(FarmerRenderer_drawHairAndAccessories_Transpiler)}:\n{ex}", LogLevel.Error);
            }

            return codes.AsEnumerable();
        }

        public static void FarmerRenderer_drawHairAndAccessories_Prefix(Farmer who, ref int currentFrame)
        {
            try
            {
                // Only maps if the player is wearing their bathing suit
                MapAnimationFrameToBathingSuitAnimation(who, ref currentFrame);
            }
            catch(Exception ex)
            {
                SMonitor.Log($"Failed in {nameof(FarmerRenderer_drawHairAndAccessories_Prefix)}:\n{ex}", LogLevel.Error);
            }
        }

        public static IEnumerable<CodeInstruction> FarmerRenderer_draw_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            try
            {
                bool pants = true;
                bool body = true;

                SMonitor.Log($"Transpiling FarmerRenderer.draw");

                for (int i = 0; i < codes.Count; i++)
                {
                    if (body && codes[i].opcode == OpCodes.Ldarg_S && codes[i + 1].opcode == OpCodes.Newobj && codes[i + 2].opcode == OpCodes.Ldarg_S && codes[i + 3].opcode == OpCodes.Ldarg_S &&
                        (byte)codes[i].operand == 4 && (byte)codes[i + 2].operand == 9 && (byte)codes[i + 3].operand == 10)
                    {
                        SMonitor.Log($"Editing the base sprite.");

                        codes.Insert(i + 1, CodeInstruction.Call(typeof(AnimationManager), nameof(getSwimSourceRectangle), new Type[] { typeof(Farmer), typeof(Rectangle) }));
                        codes.Insert(i, new CodeInstruction(OpCodes.Ldarg_S, (byte)12));
                        body = false;
                    }

                    // Replace the pants sprite with the swimsuit
                    if (pants && codes[i].opcode == OpCodes.Ldarg_S && codes[i + 1].opcode == OpCodes.Ldfld && codes[i + 2].opcode == OpCodes.Ldarg_S && codes[i + 3].opcode == OpCodes.Ldfld &&
                        codes[i + 4].opcode == OpCodes.Ldarg_S && codes[i + 5].opcode == OpCodes.Ldfld && codes[i + 6].opcode == OpCodes.Ldarg_S && codes[i + 7].opcode == OpCodes.Ldfld &&
                        (FieldInfo)codes[i + 1].operand == AccessTools.Field(typeof(Rectangle), nameof(Rectangle.X)) &&
                        (FieldInfo)codes[i + 3].operand == AccessTools.Field(typeof(Rectangle), nameof(Rectangle.Y)) &&
                        (FieldInfo)codes[i + 5].operand == AccessTools.Field(typeof(Rectangle), nameof(Rectangle.Width)) &&
                        (FieldInfo)codes[i + 7].operand == AccessTools.Field(typeof(Rectangle), nameof(Rectangle.Height)))
                    {
                        SMonitor.Log($"Editing the pants sprite.");

                        codes[i - 1].opcode = OpCodes.Nop;
                        codes[i - 1].operand = null;

                        codes[i].operand = (byte)12;
                        codes[i + 1].opcode = OpCodes.Ldarg_S;
                        codes[i + 1].operand = (byte)4;
                        codes[i + 2].opcode = OpCodes.Call;
                        codes[i + 2].operand = AccessTools.Method(typeof(AnimationManager), nameof(getSwimSourceRectanglePant));

                        for(int j = 3; j < 8; j++)
                        {
                            codes[i + j].opcode = OpCodes.Nop;
                            codes[i + j].operand = null;
                        }

                        codes[i + 8].opcode = OpCodes.Stloc_S;
                        codes[i + 8].operand = (byte)3;

                        /*
                        codes[i + 1].opcode = OpCodes.Call;
                        codes[i + 1].operand = AccessTools.Method(typeof(AnimationManager), nameof(GetPantsRectX));

                        codes[i + 3].opcode = OpCodes.Call;
                        codes[i + 3].operand = AccessTools.Method(typeof(AnimationManager), nameof(GetPantsRectY));

                        codes[i + 5].opcode = OpCodes.Call;
                        codes[i + 5].operand = AccessTools.Method(typeof(AnimationManager), nameof(GetPantsRectWidth));

                        codes[i + 7].opcode = OpCodes.Call;
                        codes[i + 7].operand = AccessTools.Method(typeof(AnimationManager), nameof(GetPantsRectHeight));*/

                        pants = false;
                    }
                }
            }
            catch (Exception ex)
            {
                SMonitor.Log($"Failed in {nameof(FarmerRenderer_draw_Transpiler)}:\n{ex}", LogLevel.Error);
            }

            return codes.AsEnumerable();
        }

        public static bool Farmer_ShirtHasSleeves_Postfix(bool result, Farmer __instance)
        {
            return result && !__instance.bathingClothes.Value;
        }

        #endregion

        #region HELPER_METHODS

        public static bool ShouldNotDrawHat(Farmer farmer)
        {
            return (!Config.DisplayHatWithSwimsuit) && farmer.bathingClothes.Value;
        }

        private const int sidewaysStandstill = 114;
        private const int sidewaysRightFootRaised = 115;
        private const int sidewaysLeftFootRaised = 116;
        private const int upStandstill = 120;
        private const int upLeftFootRaised = 121;
        private const int upRightFootRaised = 122;
        private const int downStandstill = 108;
        private const int downLeftFootRaised = 109;
        private const int downRightFootRaised = 110;

        public static readonly int[] AnimationFrameToBathingSuitFrameMap = new int[] {
            downStandstill, // 0 is just standing still facing down
            downLeftFootRaised, // 1 is left foot partially raised
            downRightFootRaised, // 2 is right foot partially raised
            3, // 3 is Hi emote - also used in other emotes
            4, // 4 is pass out frame 1
            5, // 5 is pass out frame 2
            sidewaysStandstill, // 6 is just standing still facing sideways
            sidewaysRightFootRaised, // 7 is right foot partially raised (walking)
            sidewaysLeftFootRaised, // 8 is left foot partially raised (walking)
            9,
            10, // 10 is Taunt emote frame 2
            sidewaysLeftFootRaised, // 11 is left foot partially raised
            upStandstill, // 12 is just standing still facing up
            upRightFootRaised, // 13 is right foot partially raised
            upLeftFootRaised, // 14 is left foot partially raised
            15,
            16, // 16 is tired
            sidewaysRightFootRaised, // 17 is right foot partially raised
            downLeftFootRaised, // 18 is left foot fully raised
            downRightFootRaised, // 19 is right foot fully raised
            sidewaysLeftFootRaised, // 20 is left foot fully raised
            sidewaysRightFootRaised, // 21 is right foot fully raised
            upRightFootRaised, // 22 is right foot fully raised
            upLeftFootRaised, // 23 is left foot fully raised
            downStandstill, // 24 is using scythe facing down frame 0?
            downStandstill, // 25 is using scythe facing down frame 1
            downStandstill, // 26 is using scythe facing down frame 2
            downStandstill, // 27 is using scythe facing down frame 3
            downStandstill, // 28 is using scythe facing down frame 4
            downStandstill, // 29 is using scythe facing down frame 5
            sidewaysStandstill, // 30 is using scythe facing sideways frame 0?
            sidewaysStandstill, // 31 is using scythe facing sideways frame 1
            sidewaysStandstill, // 32 is using scythe facing sideways frame 2
            sidewaysStandstill, // 33 is using scythe facing sideways frame 3
            sidewaysStandstill, // 34 is using scythe facing sideways frame 4
            sidewaysStandstill, // 35 is using scythe facing sideways frame 5
            upStandstill, // 36 is using tool facing up frame 0?
            upStandstill, // 37 is using tool facing up frame 1 - Also using scythe facing up frame 1
            upStandstill, // 38 is using tool facing up frame 2 - Also using scythe facing up frame 2
            upStandstill, // 39 is using scythe facing up frame 3
            upStandstill, // 40 is using scythe facing up frame 4
            upStandstill, // 41 is using scythe facing up frame 5
            42,
            43,
            upStandstill, // 44 is fishing facing up
            sidewaysStandstill, // 45 is using watering can facing sideways
            upStandstill, // 46 is using watering can facing up
            47,
            sidewaysRightFootRaised, // 48 is sideways charging fishing pole
            sidewaysStandstill, // 49 is using tool facing sideways frame 1
            sidewaysStandstill, // 50 is using tool facing sideways frame 2
            sidewaysStandstill, // 51 is using tool facing sideways frame 3
            sidewaysStandstill, // 52 is using tool facing sideways frame 4
            53,
            downLeftFootRaised, // 54 is grabbing forage facing down frame 1
            downLeftFootRaised, // 55 is grabbing forage facing down frame 2
            downStandstill, // 56 is grabbing forage facing down frame 3
            downStandstill, // 57 is grabbing forage facing down frame 4
            sidewaysLeftFootRaised, // 58 is grabbing forage facing sideways 1
            sidewaysLeftFootRaised, // 59 is grabbing forage facing sideways 2
            sidewaysStandstill, // 60 is grabbing forage facing sideways 3
            sidewaysStandstill, // 61 is grabbing forage facing sideways 4
            upStandstill, // 62 is using tool facing up frame 4 - Also grabbing forage facing up frame 1
            upStandstill, // 63 is using tool facing up frame 3 - Also grabbing forage facing up frame 2
            upStandstill, // 64 is grabbing forage facing up frame 3
            upStandstill, // 65 is grabbing forage facing up frame 4
            downLeftFootRaised, // 66 is down charging fishing pole
            downStandstill, // 67 is using tool facing down frame 1
            downStandstill, // 68 is using tool facing down frame 2
            downStandstill, // 69 is using tool facing down frame 3
            downStandstill, // 70 is using tool facing down frame 4
            71,
            sidewaysRightFootRaised, // 72 is pulling fish out of the water facing sideways
            73,
            downLeftFootRaised, // 74 is pulling fish out of the water facing down
            75,
            upRightFootRaised, // 76 is charging fishing pole facing up
            77,
            downStandstill, // 78 is shear down frame 1
            downStandstill, // 79 is shear down frame 2
            sidewaysStandstill, // 80 is shear sideways frame 1
            sidewaysStandstill, // 81 is shear sideways frame 2
            upStandstill, // 82 is shear up frame 1
            upStandstill, // 83 is shear up frame 2
            downStandstill, // 84 is displaying item (facing down) - Also eat frame 1
            downStandstill, // 85 is eat frame 2
            downStandstill, // 86 is eat frame 3
            downStandstill, // 87 is eat frame 4
            downStandstill, // 88 is eat frame 5
            sidewaysStandstill, // 89 is fishing sideways idle frame
            downStandstill, // 90 is drink frame 1
            downStandstill, // 91 is drink frame 2
            downStandstill, // 92 is drink frame 3
            downStandstill, // 93 is drink frame 4
            94,
            95,
            96,
            97, // 97 is cheer
            98, // 98 is music emote frame 3
            99, // 99 is music emote frame 1
            100, // 100 is music emote frame 2
            101,
            102, // 102 is Laugh emote frame 2
            103, // 103 is Laugh emote frame 1
            104, // 104 is sick frame 1
            105, // 105 is sick frame 2
            106,
            107,
            downStandstill, // 108 is bathing suit down standing still
            downLeftFootRaised, // 109 is bathing suit down left foot raised
            downRightFootRaised, // 110 is bathing suit down right foot raised
            111, // 111 is jar emote
            112, // 112 is jar emote but open
            113,
            sidewaysStandstill, // 114 is bathing suit sideways standing still
            sidewaysRightFootRaised, // 115 is bathing suit sideways right foot raised
            sidewaysLeftFootRaised, // 116 is bathing suit sideways left foot raised
            117,
            118,
            119,
            upStandstill, // 120 is bathing suit up standing still
            upLeftFootRaised, // 121 is bathing suit up left foot raised
            upRightFootRaised, // 122 is bathing suit up right foot raised
            downStandstill, // 123 is pan frame 1
            downStandstill, // 124 is pan frame 2
            downStandstill, // 125 is pan frame 3
        };

        public static readonly bool[] ShouldUseArmlessSpritePerFrame = new bool[]
            {
               false, false, false,  true,  true,  true, false, false, false,  true,// 0
                true, false, false, false, false,  true,  true, false, false, false,// 10
               false, false, false, false,  true,  true,  true,  true,  true,  true,// 20
                true,  true,  true,  true,  true,  true,  true,  true,  true,  true,// 30
                true,  true,  true,  true,  true,  true,  true,  true,  true,  true,// 40
                true,  true,  true,  true,  true,  true,  true,  true,  true,  true,// 50
                true,  true,  true,  true,  true,  true,  true,  true,  true,  true,// 60
                true,  true,  true,  true,  true,  true,  true,  true,  true,  true,// 70
                true,  true,  true,  true,  true,  true,  true,  true,  true,  true,// 80
                true,  true,  true,  true,  true,  true,  true,  true,  true,  true,// 90
                true,  true,  true,  true,  true,  true,  true,  true, false, false,// 100
               false,  true,  true,  true, false, false, false,  true,  true,  true,// 110
               false, false, false,  true,  true,  true                             // 120
            };

        public static bool TryMapAnimationFrame(int frame, out int mappedFrame)
        {
            if(frame >= 0 && frame < AnimationFrameToBathingSuitFrameMap.Length)
            {
                mappedFrame = AnimationFrameToBathingSuitFrameMap[frame];
                return true;
            }

            mappedFrame = frame;
            return false;
        }

        public static bool TryMapAnimationFrame(ref int frame)
        {
            if (frame >= 0 && frame < AnimationFrameToBathingSuitFrameMap.Length)
            {
                frame = AnimationFrameToBathingSuitFrameMap[frame];
                return true;
            }

            return false;
        }

        public static bool TryGetShouldUseArmless(int frame, out bool shouldUseArmless)
        {
            if (frame >= 0 && frame < ShouldUseArmlessSpritePerFrame.Length)
            {
                shouldUseArmless = ShouldUseArmlessSpritePerFrame[frame];
                return true;
            }

            shouldUseArmless = false;
            return false;
        }

        public static bool MapAnimationFrameToBathingSuitAnimation(Farmer farmer, ref int frame)
        {
            if (!farmer.bathingClothes.Value)
                return false;

            int oldFrame = frame;
            TryMapAnimationFrame(ref frame);
            return oldFrame != frame;
        }

        public static Rectangle getSwimSourceRectangle(Farmer farmer, Rectangle ogRect)
        {
            if (!farmer.bathingClothes.Value || FarmerRenderer.isDrawingForUI)
                return ogRect;

            TryMapAnimationFrame(farmer.FarmerSprite.CurrentFrame, out int frame);
            return new Rectangle(frame * farmer.FarmerSprite.SpriteWidth % 96, frame * farmer.FarmerSprite.SpriteWidth / 96 * farmer.FarmerSprite.SpriteHeight, farmer.FarmerSprite.SpriteWidth, farmer.swimming.Value ? farmer.FarmerSprite.SpriteHeight/2 - (int)farmer.yOffset/4 : farmer.FarmerSprite.SpriteHeight);
        }

        // The only difference between this and the above is that this always creates a new rectangle instance (the pants rectangle *needs* to be different from the input).
        public static Rectangle getSwimSourceRectanglePant(Farmer farmer, Rectangle ogRect)
        {
            if (!farmer.bathingClothes.Value || FarmerRenderer.isDrawingForUI)
                return new Rectangle(ogRect.X, ogRect.Y, ogRect.Width, ogRect.Height);

            TryMapAnimationFrame(farmer.FarmerSprite.CurrentFrame, out int frame);
            return new Rectangle(frame * farmer.FarmerSprite.SpriteWidth % 96, frame * farmer.FarmerSprite.SpriteWidth / 96 * farmer.FarmerSprite.SpriteHeight, farmer.FarmerSprite.SpriteWidth, farmer.swimming.Value ? farmer.FarmerSprite.SpriteHeight / 2 - (int)farmer.yOffset / 4 : farmer.FarmerSprite.SpriteHeight);
        }

        public static int GetPantsRectX(Rectangle sourceRect)
        {
            return getSwimSourceRectangle(GetCurrentlyDrawingFarmer(), sourceRect).X;
        }

        public static int GetPantsRectY(Rectangle sourceRect)
        {
            return getSwimSourceRectangle(GetCurrentlyDrawingFarmer(), sourceRect).Y;
        }

        public static int GetPantsRectWidth(Rectangle sourceRect)
        {
            return getSwimSourceRectangle(GetCurrentlyDrawingFarmer(), sourceRect).Width;
        }

        public static int GetPantsRectHeight(Rectangle sourceRect)
        {
            return getSwimSourceRectangle(GetCurrentlyDrawingFarmer(), sourceRect).Height;
        }

        const int bathingSuitTextureStartY = 576; // X is just 0
        static readonly Rectangle replaceRectangle = new Rectangle(0, 14, 16, 32-14);
        public static void UnarmBathingModels(IAssetDataForImage image, bool girl)
        {
            Texture2D textureData = SHelper.GameContent.Load<Texture2D>(girl ? "FlyingTNT.Swim/PlayerBaseGirlArmless" : "FlyingTNT.Swim/PlayerBaseBoyArmless");

            Rectangle sourceRectangle = new Rectangle(replaceRectangle.Left, replaceRectangle.Top, replaceRectangle.Width, replaceRectangle.Height);
            Rectangle targetRectangle = new Rectangle(replaceRectangle.Left, replaceRectangle.Top + bathingSuitTextureStartY, replaceRectangle.Width, replaceRectangle.Height);

            for (int i = 0; i < 3; i++)
            {
                for(int j = 0; j < 3; j++)
                {
                    image.PatchImage(textureData, sourceRectangle, targetRectangle, PatchMode.Replace);
                    sourceRectangle.Offset(0, 32);
                    targetRectangle.Offset(0, 32);
                }
                sourceRectangle.Offset(16, -96);
                targetRectangle.Offset(16, -96);
            }
        }
        public static void UpdateBathingSuits(IAssetDataForImage image)
        {
            Texture2D girlTextureData = SHelper.GameContent.Load<Texture2D>("FlyingTNT.Swim/GirlSwimsuitArmless");
            Texture2D boyTextureData = SHelper.GameContent.Load<Texture2D>("FlyingTNT.Swim/BoySwimsuitArmless");

            Rectangle targetRectangle = new Rectangle(0, 0, 16*3, 32*3);
            // source is just the whole source image so we don't need a rectangle for it

            for (int i = 0; i < 1; i++)
            {
                targetRectangle.Offset(-targetRectangle.Left, -targetRectangle.Top);
                targetRectangle.Offset(i % 10 * 192 + (108 * 16 % 96), i / 10 * 688 + (108 * 16 / 96 * 32));

                image.PatchImage(boyTextureData, null, targetRectangle, PatchMode.Replace);

                targetRectangle.Offset(96, 0);

                image.PatchImage(girlTextureData, null, targetRectangle, PatchMode.Replace);
            }
        }

        public static string[] SplitPath(string path)
        {
            return path.Split(new string[] { "/", "\\"}, StringSplitOptions.None);
        }

        public static void MarkAllDirty(Farmer farmer)
        {
            if(GetTextureState(farmer) is not FarmerTextureState state)
            {
                return;
            }
            state.spriteDirty = true;
            state.eyesDirty = true;
            state.skinDirty = true;
            state.shirtDirty = true;
            state.pantsDirty = true;
            state.shoesDirty = true;
        }

        public static Texture2D GetUniqueScreenTexture(string textureName)
        {
            Texture2D texture = Game1.temporaryContent.CreateTemporary().Load<Texture2D>(textureName);
            return texture;
        }

        public class FarmerTextureState
        {
            public bool spriteDirty = true;
            public bool eyesDirty = true;
            public bool skinDirty = true;
            public bool shoesDirty = true;
            public bool shirtDirty = true;
            public bool pantsDirty = true;
            public Texture2D texture = null;
        }

        public static FarmerTextureState GetTextureState(Farmer farmer)
        {
            if(farmer is null)
            {
                SMonitor.Log("Farmer is null; using Game1.player");
                farmer = Game1.player;
                if(farmer is null)
                {
                    return null;
                }
            }

            if(!TextureStateDictionary.TryGetValue(farmer.UniqueMultiplayerID, out FarmerTextureState state))
            {
                state = new FarmerTextureState();
                TextureStateDictionary.Add(farmer.UniqueMultiplayerID, state);
            }

            return state;
        }

        public static Farmer GetCurrentlyDrawingFarmer()
        {
            if(currentlyDrawingFarmer.Value is null)
            {
                currentlyDrawingFarmer.Value = Game1.player;
            }
            return currentlyDrawingFarmer.Value;
        }

        #endregion
    }
}
