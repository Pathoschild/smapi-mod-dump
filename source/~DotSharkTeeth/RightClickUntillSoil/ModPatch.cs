/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DotSharkTeeth/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;


namespace RightClickUntillSoil
{
    public partial class ModEntry
    {
        public static void Game1pressActionButton_postfix(ref bool __result)
        {

            if (!__result || !IsHoldingHoe() ||!Game1.didPlayerJustRightClick() || !IsWithinRadius || !IsHoeDirt(UseToolLocation))
                return;

            Game1.currentLocation.playSound("woodyHit");
            Game1.player.stopJittering();
            var dir = Utility.getDirectionFromChange(UseToolLocation, Game1.player.getTileLocation());
            if (dir == -1) {
                dir = Game1.player.FacingDirection;
            }
            Game1.player.faceDirection(dir);
            Game1.player.UsingTool = true;
            Game1.player.CanMove = false;
            Game1.player.freezePause = (int)(420 *Config.ToolSpeed);
            AnimatePlayer(dir);
            //Game1.player.FarmerSprite.animateOnce(295 + dir, Config.ToolSpeed/5, 0, endOfBehaviorFunction);
            Game1.currentLocation.terrainFeatures.Remove(UseToolLocation);

            __result = false;
        }

        public static bool IsWithinRadius
        {
            get => Utility.tileWithinRadiusOfPlayer((int)UseToolLocation.X, (int)UseToolLocation.Y, Config.ToolRadius, Game1.player);
        }

        public static Vector2 UseToolLocation 
        {
            get => Config.UseToolLocation ? ToolTile : Game1.currentCursorTile;
        }

        public static Vector2 ToolTile
        {
            get => new Vector2((int)(Game1.player.GetToolLocation().X / Game1.tileSize), (int)(Game1.player.GetToolLocation().Y / Game1.tileSize));
        }

        public static void AnimatePlayer(int direction) {
            AnimatedSprite.endOfAnimationBehavior endOfBehaviorFunction = new AnimatedSprite.endOfAnimationBehavior((who) => {
                who.UsingTool = false;
                who.CanMove = true;
            });
            switch (direction) {
                case 0:
                    Game1.player.FarmerSprite.animateOnce(new[]
                    {
                        new FarmerSprite.AnimationFrame(66, (int)(150 * Config.ToolSpeed), false, false),
                        new FarmerSprite.AnimationFrame(67, (int)(40 * Config.ToolSpeed), false, false),
                        new FarmerSprite.AnimationFrame(68, (int)(40 * Config.ToolSpeed), false, false),
                        new FarmerSprite.AnimationFrame(69, (int)(40 * Config.ToolSpeed), false, false),
                        new FarmerSprite.AnimationFrame(70, (int)(150 * Config.ToolSpeed), false, false, endOfBehaviorFunction, true),
                    });
                    break;
                case 1:
                    Game1.player.FarmerSprite.animateOnce(new[]
                    {
                        new FarmerSprite.AnimationFrame(48, (int)(150 * Config.ToolSpeed), false, false),
                        new FarmerSprite.AnimationFrame(49, (int)(40 * Config.ToolSpeed), false, false),
                        new FarmerSprite.AnimationFrame(50, (int)(40 * Config.ToolSpeed), false, false),
                        new FarmerSprite.AnimationFrame(51, (int)(40 * Config.ToolSpeed), false, false),
                        new FarmerSprite.AnimationFrame(52, (int)(150 * Config.ToolSpeed), false, false, endOfBehaviorFunction, true),
                    });
                    break;
                case 2:
                    Game1.player.FarmerSprite.animateOnce(new[]
                    {
                        new FarmerSprite.AnimationFrame(66, (int)(150 * Config.ToolSpeed), false, false),
                        new FarmerSprite.AnimationFrame(67, (int)(40 * Config.ToolSpeed), false, false),
                        new FarmerSprite.AnimationFrame(68, (int)(40 * Config.ToolSpeed), false, false),
                        new FarmerSprite.AnimationFrame(69, (int)(40 * Config.ToolSpeed), false, false),
                        new FarmerSprite.AnimationFrame(70, (int)(75 * Config.ToolSpeed), false, false, endOfBehaviorFunction, true),
                    });
                    break;
                case 3:
                    Game1.player.FarmerSprite.animateOnce(new[]
                    {
                        new FarmerSprite.AnimationFrame(48, (int)(150 * Config.ToolSpeed), false, true),
                        new FarmerSprite.AnimationFrame(49, (int)(40 * Config.ToolSpeed), false, true),
                        new FarmerSprite.AnimationFrame(50, (int)(40 * Config.ToolSpeed), false, true),
                        new FarmerSprite.AnimationFrame(51, (int)(40 * Config.ToolSpeed), false, true),
                        new FarmerSprite.AnimationFrame(52, (int)(150 * Config.ToolSpeed), false, true, endOfBehaviorFunction, true),
                    });
                    break;
            }
        }
        
    }
}
