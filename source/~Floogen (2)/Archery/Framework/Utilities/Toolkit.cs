/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/Archery
**
*************************************************/

using Archery.Framework.Interfaces.Internal;
using Archery.Framework.Models.Generic;
using Archery.Framework.Models.Weapons;
using Archery.Framework.Objects.Items;
using Archery.Framework.Objects.Weapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Archery.Framework.Utilities
{
    internal class Toolkit
    {
        internal const string ARENA_MAP_NAME = "Custom_PeacefulEnd.Archery.Arena";
        internal static Texture2D _recolorableBaseTexture;
        internal static Color _hitboxColor = new Color(216, 55, 0, 210);

        internal static float IncrementAndGetLayerDepth(ref float layerDepth)
        {
            layerDepth += 0.0001f;
            return layerDepth;
        }

        internal static bool PlaySound(Sound sound, string sourceId, Vector2 sourcePosition)
        {
            if (sound is null || sound.IsValid() is false || Game1.soundBank is null)
            {
                return false;
            }

            int actualPitch = 1200;
            if (sound.Pitch != -1 && sound.PitchRandomness is not null)
            {
                actualPitch = sound.Pitch + sound.PitchRandomness.Get(Game1.random);
            }

            try
            {
                ICue cue = Game1.soundBank.GetCue(sound.Name);
                cue.SetVariable("Pitch", actualPitch);

                var actualVolume = sound.Volume;
                if (sound.MaxDistance > 0)
                {
                    float distance = Vector2.Distance(sourcePosition, Game1.player.getStandingPosition());
                    actualVolume = Math.Min(1f, 1f - distance / sound.MaxDistance) * sound.Volume * Game1.options.ambientVolumeLevel;
                }
                cue.Volume = actualVolume;

                if (cue.Volume <= 0)
                {
                    return true;
                }

                cue.Play();
                try
                {
                    if (!cue.IsPitchBeingControlledByRPC)
                    {
                        cue.Pitch = Utility.Lerp(-1f, 1f, actualPitch / 2400f);
                    }
                }
                catch (Exception ex)
                {
                    Archery.monitor.LogOnce($"Failed to play ({sound.Name}) given for {sourceId}: {ex}", StardewModdingAPI.LogLevel.Warn);
                    return false;
                }
            }
            catch (Exception ex2)
            {
                Archery.monitor.LogOnce($"Failed to play ({sound.Name}) given for {sourceId}: {ex2}", StardewModdingAPI.LogLevel.Warn);
                return false;
            }

            return true;
        }

        internal static void GiveBow(string command, string[] args)
        {
            var bowFilter = args.Length > 0 ? args[0] : null;

            WeaponModel weaponModel = null;
            if (string.IsNullOrEmpty(bowFilter) is false)
            {
                weaponModel = Archery.modelManager.GetSpecificModel<WeaponModel>(bowFilter);
            }
            else
            {
                weaponModel = Archery.modelManager.GetRandomWeaponModel(WeaponType.Bow);
            }

            if (weaponModel is not null)
            {
                Game1.player.addItemByMenuIfNecessary(Bow.CreateInstance(weaponModel));
            }
        }

        internal static void GiveArrow(string command, string[] args)
        {
            var arrowFilter = args.Length > 0 ? args[0] : null;

            AmmoModel ammoModel = null;
            if (string.IsNullOrEmpty(arrowFilter) is false)
            {
                ammoModel = Archery.modelManager.GetSpecificModel<AmmoModel>(arrowFilter);
            }
            else
            {
                ammoModel = Archery.modelManager.GetRandomAmmoModel(AmmoType.Arrow);
            }

            if (ammoModel is not null)
            {
                Game1.player.addItemByMenuIfNecessary(Arrow.CreateInstance(ammoModel, 999));
            }
        }

        internal static void DisplayIds(string command, string[] args)
        {
            var displayType = args.Length > 0 ? args[0] : "Weapon";

            if (displayType.Equals("Ammo", StringComparison.OrdinalIgnoreCase))
            {
                Archery.monitor.Log("Displaying ammo IDs:", LogLevel.Info);
                foreach (var ammoModel in Archery.modelManager.GetAllModels().Where(t => t is AmmoModel).OrderBy(t => t.ContentPack.Manifest.UniqueID))
                {
                    Archery.monitor.Log(ammoModel.Id, LogLevel.Debug);
                }
            }
            else
            {
                Archery.monitor.Log("Displaying weapon IDs:", LogLevel.Info);
                foreach (var weaponModel in Archery.modelManager.GetAllModels().Where(t => t is WeaponModel).OrderBy(t => t.ContentPack.Manifest.UniqueID))
                {
                    Archery.monitor.Log(weaponModel.Id, LogLevel.Debug);
                }
            }
        }

        internal static void SuppressToolButtons()
        {
            foreach (var button in Game1.options.useToolButton)
            {
                Archery.modHelper.Input.Suppress(button.ToSButton());
            }

            Archery.modHelper.Input.Suppress(SButton.ControllerX);
        }

        internal static bool AreToolButtonSuppressed()
        {
            foreach (var button in Game1.options.useToolButton)
            {
                if (Archery.modHelper.Input.IsSuppressed(button.ToSButton()))
                {
                    return true;
                }
            }

            return Archery.modHelper.Input.IsSuppressed(SButton.ControllerX);
        }

        internal static bool AreSpecialAttackButtonsPressed()
        {
            if (Game1.input.GetMouseState().RightButton == ButtonState.Pressed)
            {
                return true;
            }
            if (Game1.input.GetGamePadState().Buttons.A == ButtonState.Pressed)
            {
                return true;
            }
            if (Game1.isOneOfTheseKeysDown(Game1.input.GetKeyboardState(), Game1.options.actionButton))
            {
                return true;
            }

            return false;
        }

        internal static bool WereSpecialAttackButtonsPressed()
        {
            if (Game1.oldMouseState.RightButton == ButtonState.Pressed)
            {
                return true;
            }
            if (Game1.oldPadState.Buttons.A == ButtonState.Pressed)
            {
                return true;
            }
            if (Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.actionButton))
            {
                return true;
            }

            return false;
        }

        internal static void TeleportToArena(string command, string[] args)
        {
            // Create the arena if needed
            if (Game1.locations.Any(l => l.Name == ARENA_MAP_NAME) is false)
            {
                var arena = new GameLocation(Archery.assetManager.arenaMapPath, ARENA_MAP_NAME);
                Game1.locations.Add(arena);

                // Add the braziers
                List<Vector2> brazierTiles = new List<Vector2>()
                {
                    new Vector2(11, 15),
                    new Vector2(21, 15),
                    new Vector2(11, 23),
                    new Vector2(21, 23),
                    new Vector2(9, 18),
                    new Vector2(23, 18)
                };

                foreach (var tile in brazierTiles)
                {
                    if (arena.objects.ContainsKey(tile))
                    {
                        continue;
                    }

                    Torch torch = new Torch(tile, 149, bigCraftable: true);
                    torch.shakeTimer = 25;
                    if (torch.placementAction(arena, (int)tile.X * 64, (int)tile.Y * 64, null))
                    {
                        var actualTorch = arena.getObjectAtTile((int)tile.X, (int)tile.Y);
                        actualTorch.IsOn = true;
                        actualTorch.initializeLightSource(actualTorch.TileLocation);
                    }
                }

                // Add the target dummies, if DGA and the relevant pack is installed
                var dgaAPI = Archery.apiManager.GetDynamicGameAssetsApi();
                if (dgaAPI is not null)
                {
                    List<Vector2> dummyTiles = new List<Vector2>()
                    {
                        new Vector2(13, 19),
                        new Vector2(20, 19),
                        new Vector2(17, 22)
                    };

                    foreach (var tile in dummyTiles)
                    {
                        var targetDummy = dgaAPI.SpawnDGAItem("PeacefulEnd.PracticeDummy/PracticeDummy") as StardewValley.Object;
                        if (arena.objects.ContainsKey(tile) || targetDummy is null)
                        {
                            continue;
                        }

                        targetDummy.placementAction(arena, (int)tile.X * 64, (int)tile.Y * 64, null);
                    }

                    var targetTile = new Vector2(16, 11);
                    var knockbackDummy = dgaAPI.SpawnDGAItem("PeacefulEnd.PracticeDummy/KnockbackDummy") as StardewValley.Object;
                    if (arena.objects.ContainsKey(targetTile) is false && knockbackDummy is not null)
                    {
                        knockbackDummy.placementAction(arena, (int)targetTile.X * 64, (int)targetTile.Y * 64, null);
                    }

                    targetTile = new Vector2(16, 15);
                    var maxHitDummy = dgaAPI.SpawnDGAItem("PeacefulEnd.PracticeDummy/MaxHitDummy") as StardewValley.Object;
                    if (arena.objects.ContainsKey(targetTile) is false && maxHitDummy is not null)
                    {
                        maxHitDummy.placementAction(arena, (int)targetTile.X * 64, (int)targetTile.Y * 64, null);
                    }
                }
            }

            // Warp the farmer to the arena
            Game1.warpFarmer(ARENA_MAP_NAME, 16, 19, false);
        }

        internal static void DrawHitBox(SpriteBatch b, Rectangle bounds, float rotation = 0f)
        {
            if (_recolorableBaseTexture is null)
            {
                _recolorableBaseTexture = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
                _recolorableBaseTexture.SetData(new[] { Color.White });
            }

            b.Draw(_recolorableBaseTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2(bounds.X, bounds.Y)), bounds, _hitboxColor, rotation, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
    }
}
