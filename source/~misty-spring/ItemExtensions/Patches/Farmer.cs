/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using HarmonyLib;
using ItemExtensions.Models;
using ItemExtensions.Models.Contained;
using ItemExtensions.Models.Items;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Triggers;
using Object = StardewValley.Object;
using static ItemExtensions.Additions.ModKeys;

namespace ItemExtensions.Patches;

public class FarmerPatches
{
#if DEBUG
    private const LogLevel Level = LogLevel.Debug;
#else
    private const LogLevel Level =  LogLevel.Trace;
#endif
    
    private static void Log(string msg, LogLevel lv = Level) => ModEntry.Mon.Log(msg, lv);
    internal static void Apply(Harmony harmony)
    {
        Log($"Applying Harmony patch \"{nameof(FarmerPatches)}\": prefixing SDV method \"Farmer.eatObject\".");
        
        harmony.Patch(
            original: AccessTools.Method(typeof(Farmer), nameof(Farmer.eatObject)),
            prefix: new HarmonyMethod(typeof(FarmerPatches), nameof(Pre_eatObject))
        );
        
        Log($"Applying Harmony patch \"{nameof(FarmerPatches)}\": postfixing SDV method \"Farmer.eatObject\".");
        
        harmony.Patch(
            original: AccessTools.Method(typeof(Farmer), nameof(Farmer.eatObject)),
            postfix: new HarmonyMethod(typeof(FarmerPatches), nameof(Post_eatObject))
        );
    }

    #region harmony
    internal static bool Pre_eatObject(Farmer __instance, Object o, bool overrideFullness = false)
    {
        try
        {
            //if no obj data (shouldn't happen)
            if (!Game1.objectData.TryGetValue(o.ItemId, out var objectData))
                return true;

            //if can't eat/drink, run og method
            if (objectData.IsDrink)
            {
                if (__instance.IsLocalPlayer && __instance.hasBuff("7") && !overrideFullness)
                    return true;
            }
            else if (objectData.Edibility != -300)
            {
                if (__instance.hasBuff("6") && !overrideFullness)
                {
                    return true;
                }
            }

            //if no main data OR no eatingdata
            if (ModEntry.Data.TryGetValue(o.QualifiedItemId, out var data) == false)
            {
                if (objectData.CustomFields is null || objectData.CustomFields.Any() == false)
                    return true;

#if DEBUG
                var log = "Custom fields: ";
                foreach (var keyvalue in objectData.CustomFields)
                {
                    log += $"{keyvalue.Key} : {keyvalue.Value}";
                }

                Log(log);
#endif

                //if also none in moddata, run og
                var flag1 = objectData.CustomFields.TryGetValue(CustomEating, out var customField);
                var flag2 = objectData.CustomFields.TryGetValue(DrinkColor, out var drinkColor);

                //if you have a drink color set, it will prioritize that over animation
                if (flag2)
                {
                    if (flag1 == false)
                        AddDrinkColor(__instance, drinkColor);

                    return true;
                }

                //if no key
                if (AnimateFromObject(__instance, customField) == false)
                    return true;

                __instance.mostRecentlyGrabbedItem = o;
                __instance.reduceActiveItemByOne();
                return false;
            }

            if (data is null || data == new ItemData())
                return true;

            if (data.Eating is null || data.Eating == new FarmerAnimation())
                return true;

            CheckExtraActions(data.Eating);

            if (__instance.getFacingDirection() != 2)
                __instance.faceDirection(2);

            __instance.itemToEat = o;
            __instance.mostRecentlyGrabbedItem = o;
            //__instance.forceCanMove();
            __instance.completelyStopAnimatingOrDoingAction();

            __instance.freezePause = 20000;
            __instance.CanMove = false;
            __instance.isEating = true;

            if (__instance.isEmoteAnimating)
                __instance.EndEmoteAnimation();

            __instance.itemToEat = o;
            __instance.mostRecentlyGrabbedItem = o;
            __instance.reduceActiveItemByOne();

            AnimateFromMod(__instance, data.Eating);

            return false;
        }
        catch (Exception e)
        {
            Log($"Error: {e}", LogLevel.Error);
            return true;
        }
    }

    internal static void Post_eatObject(Farmer __instance, Object o, bool overrideFullness = false)
    {
        try
        {
            if (!Game1.objectData.TryGetValue(o.ItemId, out var objectData))
                return;

            int delay;

            //if no main data OR no eatingdata
            if (!ModEntry.Data.TryGetValue(o.QualifiedItemId, out var data))
            {
                if (objectData.CustomFields is null)
                    return;

                if (!objectData.CustomFields.TryGetValue(AfterEating, out var customField))
                    return;

                AnimateFromObject(__instance, customField);
                return;
            }

            if (data is null || data == new ItemData())
                return;

            if (data.Eating is not null && data.Eating != new FarmerAnimation())
            {
                delay = data.Eating.ActualAnimation.Length;
            }
            else
            {
                delay = 640;
            }

            if (data.AfterEating is null || data.AfterEating == new FarmerAnimation())
            {
                if (objectData.CustomFields is null)
                    return;

                if (!objectData.CustomFields.TryGetValue(AfterEating, out var customField))
                    return;

                AnimateFromObject(__instance, customField);
                return;
            }

            CheckExtraActions(data.AfterEating);

            __instance.freezePause = 20000;
            __instance.CanMove = false;

            if (__instance.isEmoteAnimating)
                __instance.EndEmoteAnimation();

            Game1.delayedActions.Add(new DelayedAction(delay, AnimationFromMod));
            return;

            void AnimationFromMod() => AnimateFromMod(__instance, data.AfterEating);
        }
        catch (Exception e)
        {
            Log($"Error: {e}", LogLevel.Error);
            throw;
        }
    }
    #endregion
    
    #region custom
    private static void CheckExtraActions(FarmerAnimation data)
    {
        if (!string.IsNullOrWhiteSpace(data.ShowMessage))
            Game1.addHUDMessage(new HUDMessage(data.ShowMessage));

        if (!string.IsNullOrWhiteSpace(data.PlaySound))
        {
            if (data.SoundDelay == 0)
                Game1.playSound(data.PlaySound);
            else
            {
                void PlaySound() => Game1.playSound(data.PlaySound);
                
                Game1.delayedActions.Add(new DelayedAction(data.SoundDelay, PlaySound));
            }
        }
        
        if (!string.IsNullOrWhiteSpace(data.PlayMusic))
        {
            Game1.changeMusicTrack(data.PlayMusic);
        }
        
        if (!string.IsNullOrWhiteSpace(data.TriggerAction))
        {
            TriggerActionManager.TryRunAction(data.TriggerAction, out var error, out var exception);
            
            if(!string.IsNullOrWhiteSpace(error))
                Log($"Error: {error}. {exception}", LogLevel.Error);
        }
    }

    private static bool AnimateFromObject(Farmer who, string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return false;
        
        Log("Animating from object.");
        
        if (IsDefaultAnimation(key))
        {
            var animation2 = GetDefaultAnimation(key);
            var frames = GetDefaultFrames(key);
            who.FarmerSprite.animateOnce(animation2, 80f, frames);
            return true;
        }
        
        if (ModEntry.EatingAnimations.TryGetValue(key, out var eatingAnimation))
        {
            who.FarmerSprite.animateOnce(eatingAnimation.ActualAnimation);
            CheckItemAnimation(eatingAnimation, who);
            return true;
        }

        return false;
    }

    private static void AnimateFromMod(Farmer who, FarmerAnimation data)
    {
        /* Checks are done in this priority:
         * 1. Default animations
         * 2. Animations declared in moddata
         * 3. Item's own animation
         */
        try
        {
            
            if (AnimateFromObject(who, data.AnimateFrom))
            {
                return;
            }/*
            else
            {
                if (!data.IsValid(out var parsedData))
                    return;
                
                who.FarmerSprite.animateOnce(parsedData.ActualAnimation);
                CheckItemAnimation(parsedData, who);
            }*/
            
            if (!data.IsValid(out var parsedData))
                return;
                
            who.FarmerSprite.animateOnce(parsedData.ActualAnimation);
            CheckItemAnimation(parsedData, who);
        }
        catch (Exception e)
        {
            Log($"Error: {e}.", LogLevel.Error);
        }
    }

    private static void CheckItemAnimation(FarmerAnimation animation, Farmer who)
    {
        Game1.delayedActions.Add(new DelayedAction(animation.TotalTime() + 50, BuffsAndEmote));
        
        if (animation.HideItem) 
            return;
        
        if (animation.Food is null || animation.Food == new FoodAnimation()) 
            return; 
        
        if (who.itemToEat is null) 
            return; 
        
        var dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(who.itemToEat.QualifiedItemId);
        var texture = dataOrErrorItem.TextureName;
        
        if (!string.IsNullOrWhiteSpace(animation.Food.CustomTexture))
            texture = animation.Food.CustomTexture;

        var position = who.Position + new Vector2(-21f, -112f) + animation.Food.Offset;
        var color = Utility.StringToColor(animation.Food.Color) ?? Color.White;
        var delay = animation.Food.Duration * animation.Food.Frames + animation.Food.Delay;

        var animatedSprite = new TemporaryAnimatedSprite(
            textureName: texture,
            sourceRect: dataOrErrorItem.GetSourceRect(),
            animationInterval: animation.Food.Duration,
            animationLength: animation.Food.Frames,
            numberOfLoops: animation.Food.Loops,
            position: position,
            flicker: false, 
            flipped: animation.Food.Flip, //false 
            layerDepth: (float) (who.StandingPixel.Y / 10000.0 + 0.009999999776482582), 
            alphaFade: 0.0f, 
            color: color, 
            scale: 4f * animation.Food.Scale, 
            scaleChange: 0.0f, 
            rotation: 0.0f + animation.Food.Rotation, 
            rotationChange: 0.0f
            )
        {
            alpha = animation.Food.Transparency,
            motion = animation.Food.Motion,
            acceleration = animation.Food.Speed,
            xStopCoordinate = (int)(position.X + animation.Food.StopX),
            yStopCoordinate =  (int)(position.Y + animation.Food.StopY),
            delayBeforeAnimationStart = animation.Food.Delay,
            startSound = animation.Food.StartSound
            //endSound = animation.Food.EndSound
        };

        if (animation.Food.Crunch)
        {
            animatedSprite.endFunction = AnimateCrunch;
        }
        else
        {
            Game1.delayedActions.Add(new DelayedAction((int)delay - 100, PlaySound));
        }
        
        if (animation.Food.Light != null && animation.Food.Light != new LightData())
        {
            animatedSprite.light = true;
            animatedSprite.lightcolor = animation.Food.Light.GetColor();
            animatedSprite.lightRadius = animation.Food.Light.Size;
            animatedSprite.lightID = (int)Game1.random.NextInt64();
        }
        
        Game1.Multiplayer.broadcastSprites(who.currentLocation, animatedSprite);
        who.itemToEat = null;
        return;

        void AnimateCrunch(int extrainfo)
        {
            if (Game1.currentLocation.Equals(who.currentLocation))
                Game1.playSound("eat");

            var sourceRect1 = dataOrErrorItem.GetSourceRect();
            for (var index = 0; index < 8; ++index)
            {
                var sourceRect2 = sourceRect1.Clone();
                sourceRect2.X += 8;
                sourceRect2.Y += 8;
                sourceRect2.Width = 4;
                sourceRect2.Height = 4;
                
                var animatedSprite2 = new TemporaryAnimatedSprite(
                    texture, 
                    sourceRect2, 
                    animationInterval: 400f, 
                    animationLength: 1, 
                    numberOfLoops: 0,
                    position: who.Position + new Vector2(24f, -48f),//new Vector2(animatedSprite.xStopCoordinate, animatedSprite.yStopCoordinate), 
                    flicker: false, 
                    flipped: false, 
                    layerDepth: (float)(who.StandingPixel.Y / 10000.0 + 0.009999999776482582), 
                    alphaFade: 0.0f, 
                    color: color, 
                    scale: 4f, 
                    scaleChange: 0.0f, 
                    rotation: 0.0f, 
                    rotationChange: 0.0f
                    ) 
                {
                    motion = new Vector2(x: Game1.random.Next(-30, 31) / 10f, y: Game1.random.Next(-6, -3)),
                    acceleration = new Vector2(x: 0.0f, y: 0.5f),
                    alpha = animation.Food.Transparency
                };
                Game1.Multiplayer.broadcastSprites(who.currentLocation, animatedSprite2);
            }
        }
        
        void PlaySound() => Game1.playSound(animation.Food.EndSound);

        void BuffsAndEmote()
        {
            if(animation.Emote > -1)
                who.doEmote(animation.Emote);
            
            foreach (var buff in who.mostRecentlyGrabbedItem.GetFoodOrDrinkBuffs())
            {
                who.applyBuff(buff);
            }
        }
    }

    #endregion
    
    #region just drink
    private static void AddDrinkColor(Farmer who, string drinkColor)
    {
        var color = Utility.StringToColor(drinkColor) ?? Color.Brown;
        //anim 294, interval 80f 8 frames
        var drinkingFrames = new[]
        {
            (90, 250),
            (91, 150),
            (92, 250),
            (93, 200), //glug
            (92, 250),
            (93, 200), //glug
            (92, 250),
            (93, 200), //glug
            (91, 250),
            (90, 50),
        };

        var delay = 0;
        foreach (var tuple in drinkingFrames)
        {
            var frame = tuple.Item1 - 90;
            var duration = tuple.Item2 - 10;
            Animate(color, frame, duration, delay, who);
            delay += duration;
        }
    }

    private static void Animate(Color color, int frame, int duration, int delay, Farmer who)
    {
        var position = new Vector2(who.Position.X, who.Position.Y - who.FarmerSprite.SpriteHeight * 3);
        var texture = $"Mods/{ModEntry.Id}/Textures/Drink";
        var rect = new Rectangle(16 * frame, 0, 16, 32);
        
        var animatedSprite = new TemporaryAnimatedSprite(
            textureName: texture,
            sourceRect: rect,
            animationInterval: duration,
            animationLength: 1,
            numberOfLoops: 0,
            position: position,
            flicker: false, 
            flipped: false, 
            layerDepth: (float) (who.StandingPixel.Y / 10000.0 + 0.009999999776482582), 
            alphaFade: 0.0f, 
            color: color, 
            scale: 4f, 
            scaleChange: 0.0f, 
            rotation: 0.0f, 
            rotationChange: 0.0f
        )
        {
            delayBeforeAnimationStart = delay
        };
        
        Game1.Multiplayer.broadcastSprites(who.currentLocation, animatedSprite);
    }
    #endregion

    #region default
    private static bool IsDefaultAnimation(string frames)
    {
        return frames switch
        {
            "walkDown" => true,
            "walkRight" => true,
            "walkUp" => true,
            "walkLeft" => true,
            "runDown" => true,
            "runRight" => true,
            "runUp" => true,
            "runLeft" => true,
            "grabDown" => true,
            "grabRight" => true,
            "grabUp" => true,
            "grabLeft" => true,
            "carryWalkDown" => true,
            "carryWalkRight" => true,
            "carryWalkUp" => true,
            "carryWalkLeft" => true,
            "carryRunDown" => true,
            "carryRunRight" => true,
            "carryRunUp" => true,
            "carryRunLeft" => true,
            "eat" => true,
            "sick" => true,
            "swordswipeDown" => true,
            "swordswipeRight" => true,
            "swordswipeUp" => true,
            "swordswipeLeft" => true,
            "punchDown" => true,
            "punchRight" => true,
            "punchUp" => true,
            "punchLeft" => true,
            "harvestItemUp" => true,
            "harvestItemRight" => true,
            "harvestItemDown" => true,
            "harvestItemLeft" => true,
            "shearUp" => true,
            "shearRight" => true,
            "shearDown" => true,
            "shearLeft" => true,
            "milkUp" => true,
            "milkRight" => true,
            "milkDown" => true,
            "milkLeft" => true,
            "passOutTired" => true,
            "drink" => true,
            "fishingUp" => true,
            "fishingRight" => true,
            "fishingDown" => true,
            "fishingLeft" => true,
            "fishingDoneUp" => true,
            "fishingDoneRight" => true,
            "fishingDoneDown" => true,
            "fishingDoneLeft" => true,
            "pan" => true,
            "showHoldingEdible" => true,
            _ => false
        };
    }
    
    private static int GetDefaultFrames(string frames)
    {
        /*
           walking 4 frames
           
           running:
           down/up 8
           l/r 6
           
           harvest: 4
           eat: 8
           drink: 10
           horse, sit: 1
           Axe/pickaxe/hoe/club special: 5
           Upgraded hoe held: 1
           Scythe/melee: 6
           Daggers: 2
           Watering: 3
           sword special, slingshot: 1
           
           casting:
           down 1 + 5
           l/r 1 + 4
           up 1 + 3
           
           fishing/reeling: 1
           
           Fish Caught:
           down 3
           l/r 1 + 2
           up 1 + 2
           
           panning: 10
           milking/shearing: 2 (2x total)
           faint:5
           nausea 2 (4x total)
           kiss 1
           Flower dance: 
           down 8
           up 4
           
           Reaching, Sitting down, Looking sad, Head scratching/looking sheepish 1
           Playing the harp 3
           Laughing, Opening a jar 2
         */
        
        return frames switch
        {
            /*"walkDown" => 4,
            "walkRight" => 4,
            "walkUp" => 4,
            "walkLeft" => 4,*/
            "runDown" => 8,
            "runRight" => 6,
            "runUp" => 8,
            "runLeft" => 6,
            /*"grabDown" => 4,
            "grabRight" => 4,
            "grabUp" => 4,
            "grabLeft" => 4,
            "carryWalkDown" => 4,
            "carryWalkRight" => 4,
            "carryWalkUp" => 4,
            "carryWalkLeft" => 4,*/
            "carryRunDown" => 8,
            "carryRunRight" => 6,
            "carryRunUp" => 8,
            "carryRunLeft" => 6,
            /*"toolDown" => 160,
            "toolRight" => 168,
            "toolUp" => 176,
            "toolLeft" => 184,
            "toolChooseDown" => 192,
            "toolChooseRight" => 194,
            "toolChooseUp" => 196,
            "toolChooseLeft" => 198,
            "seedThrowDown" => 200,
            "seedThrowRight" => 204,
            "seedThrowUp" => 208,
            "seedThrowLeft" => 212,*/
            "eat" => 10,
            "punchDown" => 5,
            "punchRight" => 5,
            "punchUp" => 5,
            "punchLeft" => 5,
            /*"harvestItemUp" => 4,
            "harvestItemRight" => 4,
            "harvestItemDown" => 4,
            "harvestItemLeft" => 4,
            "shearUp" => 2,
            "shearRight" => 2,
            "shearDown" => 2,
            "shearLeft" => 2,
            "milkUp" => 2,
            "milkRight" => 2,
            "milkDown" => 2,
            "milkLeft" => 2,
            "tired" => 291,
            "tired2" => 292,*/
            "passOutTired" or "faint" or "pass out" or "passOut" => 10,
            "drink" => 10,
            //"fishingUp" => 4,
            "fishingRight" => 5,
            "fishingDown" => 6,
            "fishingLeft" => 5,
            "fishingDoneUp" => 3,
            "fishingDoneRight" => 3,
            "fishingDoneDown" => 3,
            "fishingDoneLeft" => 3,
            "pan" => 10,
            "showHoldingEdible" => 1,
            //"sick" => 4,
            "swordswipeDown" => 6,
            "swordswipeRight" => 6,
            "swordswipeUp" => 6,
            "swordswipeLeft" => 6,
            _ => 4
        };
    }

    private static int GetDefaultAnimation(string name)
    {
        return name.ToLower() switch
        {
            "walkDown" => 0,
            "walkRight" => 8,
            "walkUp" => 16,
            "walkLeft" => 24,
            "runDown" => 32,
            "runRight" => 40,
            "runUp" => 48,
            "runLeft" => 56,
            "grabDown" => 64,
            "grabRight" => 72,
            "grabUp" => 80,
            "grabLeft" => 88,
            "carryWalkDown" => 96,
            "carryWalkRight" => 104,
            "carryWalkUp" => 112,
            "carryWalkLeft" => 120,
            "carryRunDown" => 128,
            "carryRunRight" => 136,
            "carryRunUp" => 144,
            "carryRunLeft" => 152,
            /*"toolDown" => 160,
            "toolRight" => 168,
            "toolUp" => 176,
            "toolLeft" => 184,
            "toolChooseDown" => 192,
            "toolChooseRight" => 194,
            "toolChooseUp" => 196,
            "toolChooseLeft" => 198,
            "seedThrowDown" => 200,
            "seedThrowRight" => 204,
            "seedThrowUp" => 208,
            "seedThrowLeft" => 212,*/
            "eat" => 216,
            "sick" => 224,
            "swordswipeDown" => 232,
            "swordswipeRight" => 240,
            "swordswipeUp" => 248,
            "swordswipeLeft" => 256,
            "punchDown" => 272,
            "punchRight" => 274,
            "punchUp" => 276,
            "punchLeft" => 278,
            "harvestItemUp" => 279,
            "harvestItemRight" => 280,
            "harvestItemDown" => 281,
            "harvestItemLeft" => 282,
            "shearUp" => 283,
            "shearRight" => 284,
            "shearDown" => 285,
            "shearLeft" => 286,
            "milkUp" => 287,
            "milkRight" => 288,
            "milkDown" => 289,
            "milkLeft" => 290,
            /*"tired" => 291,
            "tired2" => 292,*/
            "passOutTired" => 293,
            "drink" => 294,
            "fishingUp" => 295,
            "fishingRight" => 296,
            "fishingDown" => 297,
            "fishingLeft" => 298,
            "fishingDoneUp" => 299,
            "fishingDoneRight" => 300,
            "fishingDoneDown" => 301,
            "fishingDoneLeft" => 302,
            "pan" => 303,
            "showHoldingEdible" => 304,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    #endregion
}