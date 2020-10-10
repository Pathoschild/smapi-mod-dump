/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/StardevValleyNewMachinesMod
**
*************************************************/

using System.Linq;
using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Objects;
using Igorious.StardewValley.DynamicAPI.Services;
using Igorious.StardewValley.DynamicAPI.Utils;
using Igorious.StardewValley.NewMachinesMod.Data;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Tools;

namespace Igorious.StardewValley.NewMachinesMod.SmartObjects
{
    public class BigWarpTotem : SmartObject
    {
        public BigWarpTotem() : base(ClassMapperService.Instance.GetItemID<BigWarpTotem>())
        {
            BoundingTileHeight = Config.TileHeight;
            BoundingTileWidth = Config.TileWidth;
            SpriteHeight = Config.TileHeight;
            SpriteWidth = Config.TileWidth;
        }

        private WarpTotemInformation Config => NewMachinesMod.Config.Totems.First(t => t.ID == ParentSheetIndex);

        public override int maximumStackSize() => 1;

        protected override bool CanDoAction(Farmer farmer)
        {
            return farmer.IsMainPlayer && farmer.canMove && !Game1.fadeToBlack;
        }

        protected override bool OnPickaxeAction(Pickaxe pickaxe)
        {
            PlaySound(Sound.Hammer);
            return true;
        }

        protected override bool OnAxeAction(Axe axe)
        {
            PlaySound(Sound.Hammer);
            return true;
        }

        protected override bool DoAction(Farmer farmer)
        {
            if (!CanDoAction(farmer)) return false;

            Game1.player.jitterStrength = 1;
            PlaySound(Sound.Warrior);
            Game1.player.faceDirection(2);
            Game1.player.CanMove = false;
            Game1.player.temporarilyInvincible = true;
            Game1.player.temporaryInvincibilityTimer = -4000;
            Game1.changeMusicTrack("none");
            Game1.player.FarmerSprite.animateOnce(new[]
            {
                new FarmerSprite.AnimationFrame(57, 2000, false, false),
                new FarmerSprite.AnimationFrame(Game1.player.FarmerSprite.currentFrame, 0, false, false, TotemWarpBegin, true)
            });

            var playerPosition = Game1.player.position;
            AddAnimatedSprite(new Vector2(0, -1), 0.01f, 1, 0, 1, playerPosition + new Vector2(0, -TileSize * 3f / 2));
            AddAnimatedSprite(new Vector2(0, -0.5f), 0.005f, 0.5f, 10, 0.9999f, playerPosition + new Vector2(-TileSize, -TileSize * 3f / 2));
            AddAnimatedSprite(new Vector2(0, -0.5f), 0.005f, 0.5f, 20, 0.9988f, playerPosition + new Vector2(TileSize, -TileSize * 3f / 2));
            Game1.screenGlowOnce(Microsoft.Xna.Framework.Color.LightBlue, false);
            Utility.addSprinklesToLocation(Game1.currentLocation, Game1.player.getTileX(), Game1.player.getTileY(), 16, 16, 1300, 20, Microsoft.Xna.Framework.Color.White, null, true);
            return true;
        }

        private void AddAnimatedSprite(Vector2 motion, float scaleChange, float scale, int delay, float depth, Vector2 position)
        {
            Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(parentSheetIndex, 9999, 1, 999, position, false, false, false, 0)
            {
                motion = motion,
                scaleChange = scaleChange,
                scale = scale,
                alpha = 1,
                alphaFade = 0.0075f,
                shakeIntensity = 1,
                delayBeforeAnimationStart = delay,
                initialPosition = position,
                xPeriodic = true,
                xPeriodicLoopTime = 1000,
                xPeriodicRange = TileSize / 16,
                layerDepth = depth,
                sourceRect = SourceRect,
                Texture = Game1.objectSpriteSheet,
            });
        }

        private void TotemWarpBegin(Farmer farmer)
        {
            var farmerX = (int)farmer.position.X;
            var farmerY = (int)farmer.position.Y;
            for (var index = 0; index < 12; ++index)
            {
                farmer.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(354, Game1.random.Next(25, 75), 6, 1, new Vector2(Game1.random.Next(farmerX - TileSize * 4, farmerX + TileSize * 3), Game1.random.Next(farmerY - TileSize * 4, farmerY + TileSize * 3)), false, Game1.random.NextDouble() < 0.5));
            }
            PlaySound(Sound.Wand);
            Game1.displayFarmer = false;
            Game1.player.freezePause = 1000;
            Game1.flashAlpha = 1;
            DelayedAction.fadeAfterDelay(TotemWarpEnd, 1000);
            var num = 0;
            for (var index = farmer.getTileX() + 8; index >= farmer.getTileX() - 8; --index)
            {
                farmer.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(6, new Vector2(index, farmer.getTileY()) * TileSize, Microsoft.Xna.Framework.Color.White, 8, false, 50)
                {
                    layerDepth = 1,
                    delayBeforeAnimationStart = num * 25,
                    motion = new Vector2(-0.25f, 0),
                });
                ++num;
            }
        }

        private Point GetShrinePosition()
        {
            switch (Config.WarpLocation)
            {
                case LocationName.Farm: return new Point(48, 7);
                case LocationName.Mountain: return new Point(31, 20);
                case LocationName.Beach: return new Point(20, 4);
                // TODO: Dynamic shrines?
                default: return Point.Zero;
            }
        }

        private void TotemWarpEnd()
        {
            var shrinePosition = GetShrinePosition();
            if (shrinePosition == Point.Zero)
            {
                Log.Error($"Shrine not found for location {Config.WarpLocation}!");
            }
            else
            {
                Game1.warpFarmer(Config.WarpLocation, shrinePosition.X, shrinePosition.Y, false);
            }

            Game1.fadeToBlackAlpha = 0.99f;
            Game1.screenGlow = false;
            Game1.player.temporarilyInvincible = false;
            Game1.player.temporaryInvincibilityTimer = 0;
            Game1.displayFarmer = true;
        }
    }
}
