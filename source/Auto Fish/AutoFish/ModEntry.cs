/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/WhiteMinds/mod-sv-autofish
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using GenericModConfigMenu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;

namespace AutoFish
{
    public class ModEntry : Mod
    {
        /// <summary>
        ///     正在捕捉宝箱
        /// </summary>
        private bool _catching;

        /// <summary>
        ///     配置文件
        /// </summary>
        private ModConfig Config = null!;

        private bool isContinusFishing;
        
        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.Input.ButtonsChanged += OnButtonsChanged;
            helper.Events.Display.MenuChanged += OnMenuChanged;
        }
        
        
        private void OnMenuChanged(object? sender, MenuChangedEventArgs e)
        {
            if (!(e.NewMenu is ItemGrabMenu itemGrab) || itemGrab.source != 3)
                return;
            
            IList<Item> actualInventory = itemGrab.ItemsToGrabMenu.actualInventory;
            for (int index = actualInventory.Count - 1; index >= 0; --index)
            {
                if (Game1.player.addItemToInventoryBool(actualInventory.ElementAt(index)))
                    actualInventory.RemoveAt(index);
            }
            if (actualInventory.Count == 0)
                itemGrab.exitThisMenu();
            else
                Monitor.Log("物品栏已满！",LogLevel.Warn);
        }

        private IReflectedField<MouseState>? _currentMouseState;
        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            configMenu.Register(
                ModManifest,
                () => Config = new ModConfig(),
                () => Helper.WriteConfig(Config)
                );

            configMenu.AddBoolOption(
                ModManifest,
                name: () => Helper.Translation.Get("maxCastPower.name"),
                getValue: () => Config.maxCastPower,
                setValue: value => Config.maxCastPower = value
                );
            configMenu.AddBoolOption(
                ModManifest,
                name: () => Helper.Translation.Get("autoHit.name"),
                getValue: () => Config.autoHit,
                setValue: value => Config.autoHit = value
                );
            configMenu.AddBoolOption(
                ModManifest,
                name: () => Helper.Translation.Get("fastBite.name"),
                getValue: () => Config.fastBite,
                setValue: value => Config.fastBite = value
                );
            configMenu.AddBoolOption(
                ModManifest,
                name: () => Helper.Translation.Get("catchTreasure.name"),
                getValue: () => Config.catchTreasure,
                setValue: value => Config.catchTreasure = value
                );
            configMenu.AddBoolOption(
                ModManifest,
                name: () => Helper.Translation.Get("fasterSpeed.name"),
                getValue: () => Config.fasterSpeed,
                setValue: value => Config.fasterSpeed = value
                );
            configMenu.AddBoolOption(
                ModManifest,
                name: () => Helper.Translation.Get("triggerKeepAutoFish.name"),
                getValue: () => Config.triggerKeepAutoFish,
                setValue: value => Config.triggerKeepAutoFish = value
                );
            configMenu.AddKeybindList(
                ModManifest,
                name: () => Helper.Translation.Get("triggerKeepAutoFish.name"),
                getValue: () => Config.keepAutoFishKey,
                setValue: value => Config.keepAutoFishKey = value
                );
            configMenu.AddBoolOption(
                ModManifest,
                name: () => Helper.Translation.Get("autoLootTreasure.name"),
                getValue: () => Config.autoLootTreasure,
                setValue: value => Config.autoLootTreasure = value
                );
            _currentMouseState = Helper.Reflection.GetField<MouseState>(Game1.input, "_currentMouseState");
        }
        
        private void OnButtonsChanged(object? sender, ButtonsChangedEventArgs e)
        {
            if (Game1.player.CurrentTool is FishingRod && Config.triggerKeepAutoFish && Config.keepAutoFishKey.JustPressed())
            {
                isContinusFishing = !isContinusFishing;
            }
        }


        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            var player = Game1.player;
            if (!Context.IsWorldReady || player == null)
                return;

            if (isContinusFishing && player.Stamina < 0)
            {
                isContinusFishing = false;
                return;
            }
            
            var onPressed = IsOnPressedUseToolButton();

            if (player.CurrentTool is FishingRod fishingRod)
            {
               if (Config.fastBite && fishingRod.timeUntilFishingBite > 0)
                    fishingRod.timeUntilFishingBite /= 2; // 快速咬钩

               if (Config.autoHit && fishingRod is { isNibbling: true, isReeling: false, hit: false, pullingOutOfWater: false, fishCaught: false, showingTreasure: false })
                   fishingRod.DoFunction(player.currentLocation, 1, 1, 1, player); // 自动咬钩
                
               if (isContinusFishing && player.CanMove && fishingRod is {isCasting:false,isTimingCast: false,isFishing:false,isNibbling: false, isReeling: false, hit:false ,pullingOutOfWater: false, showingTreasure:false,castedButBobberStillInAir:false})
               {
                   Game1.pressUseToolButton();
               }
               
               if (Config.maxCastPower)
                   fishingRod.castingPower = 1;

               //use reflection, which may cause some performance problem.
               if (Config.autoLootFishAndTrash && fishingRod.fishCaught)
               {
                   var oldKbState = Game1.oldKBState;
                   foreach (var vaButton in Game1.options.useToolButton)
                   {
                       if (vaButton.key != Keys.None) 
                       {
                           Game1.oldKBState = new KeyboardState(
                               vaButton.key
                           ); 
                       }

                       if (_currentMouseState != null && vaButton.mouseLeft)
                       {
                           MouseState currentMouseStateVal = _currentMouseState.GetValue(); 
                           _currentMouseState.SetValue(new MouseState(
                               currentMouseStateVal.X,
                               currentMouseStateVal.Y, 
                               currentMouseStateVal.ScrollWheelValue,
                               ButtonState.Pressed,
                               currentMouseStateVal.MiddleButton, 
                               currentMouseStateVal.RightButton,
                               currentMouseStateVal.XButton1,
                               currentMouseStateVal.XButton2,
                               currentMouseStateVal.HorizontalScrollWheelValue));
                       }
                   }
                   fishingRod.tickUpdate(Game1.currentGameTime,player);
                   Game1.oldKBState = new KeyboardState();
                   _currentMouseState?.SetValue(new MouseState());
               }
            }

            if (Game1.activeClickableMenu is BobberBar bar) // 自动小游戏
            {
                var barPos = bar.bobberBarPos;
                var barHeight = bar.bobberBarHeight;
                var barSpeed = bar.bobberBarSpeed;
                var barPosMax = 568 - barHeight;

                var fishPos = bar.bobberPosition;
                var fishTargetPos = bar.bobberTargetPosition;
                if (Math.Abs(fishTargetPos - (-1.0f)) < 0.01f)
                    fishTargetPos = fishPos;

                var treasurePos = bar.treasurePosition;
                var treasureCaught = bar.treasureCaught;
                var hasTreasure = bar.treasure;

                var distanceFromCatching = bar.distanceFromCatching;
                var isBossFish = bar.bossFish;
                _catching = Config.catchTreasure && !isBossFish && hasTreasure && !treasureCaught &&
                            (distanceFromCatching > 0.75 || (_catching && distanceFromCatching > 0.15));

                // 默认加速度
                var deltaSpeed = 0.25f * 0.6f;
                if (bar.bobbers.Contains("(O)691")) // 倒刺钩
                    deltaSpeed = 0.25f * 0.3f;

                // 自动钓鱼的加速度
                var autoDeltaSpeed = Config.fasterSpeed ? 0.6f : deltaSpeed;

                var targetPos = _catching ? treasurePos : fishPos;
                var otherPos = _catching ? fishPos : fishTargetPos;
                var offset = Math.Clamp(otherPos - targetPos, -barHeight, barHeight) / 4;

                var targetDisplacement = Math.Clamp(targetPos + offset + 20 - 0.5f * barHeight, 0.0f, barPosMax) - barPos;
                var targetSpeed = GetSpeed(autoDeltaSpeed, targetDisplacement);
                

                barSpeed += onPressed ? deltaSpeed : -deltaSpeed;
                if (barSpeed < targetSpeed)
                    barSpeed += autoDeltaSpeed;
                else if (barSpeed > targetSpeed)
                    barSpeed -= autoDeltaSpeed;

                bar.bobberBarSpeed = barSpeed;
            }
            else
            {
                _catching = false;
            }
        }

        private static bool IsOnPressedUseToolButton()
        {
            return  Game1.oldMouseState.LeftButton == ButtonState.Pressed ||
                   Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.useToolButton) ||
                   (Game1.options.gamepadControls && (Game1.oldPadState.IsButtonDown(Buttons.X) || Game1.oldPadState.IsButtonDown(Buttons.A)));
        }


        private static float GetSpeed(float deltaSpeed, float targetDisplacement)
        {
            return targetDisplacement switch
            {
                > 0 => MathF.Sqrt(2 * deltaSpeed * targetDisplacement),
                0 => 0,
                < 0 => -MathF.Sqrt(2 * deltaSpeed * -targetDisplacement),
                _ => throw new ArgumentOutOfRangeException(nameof(targetDisplacement), targetDisplacement, null)
            };
        }
        
        private void OnDayStarted(object? sender, DayStartedEventArgs e)
        {
            isContinusFishing = false;
        }
    }
}