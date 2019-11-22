using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewValley;

namespace CustomKissingMod
{
    internal class FarmerOverrides
    {
        public static void checkAction(Farmer __instance, ref Farmer who, ref bool __result, GameLocation location)
        {
            if (__result != true)
            {
                if (!__instance.isMoving() 
                    && !__instance.isInBed.Value
                    && !__instance.UsingTool
                    && __instance.ActiveObject == null 
                    && who.ActiveObject == null 
                    && !__instance.FarmerSprite.PauseForSingleAnimation 
                    && !who.FarmerSprite.PauseForSingleAnimation
                    && (who.team.GetFriendship(who.UniqueMultiplayerID,__instance.UniqueMultiplayerID).IsMarried()
                    || who.team.GetFriendship(who.UniqueMultiplayerID, __instance.UniqueMultiplayerID).IsEngaged()))
                {
                    __instance.faceGeneralDirection(who.getStandingPosition(), 0, false);
                    who.faceGeneralDirection(__instance.getStandingPosition(), 0, false);
                    if (__instance.FacingDirection == 3 || __instance.FacingDirection == 1)
                    {
                        bool flip = __instance.FacingDirection == 1;

                        __instance.CanMove = false;
                        __instance.FarmerSprite.PauseForSingleAnimation = false;
                        __instance.faceDirection(flip ? 1 : 3);
                        __instance.FarmerSprite.animateOnce(new List<FarmerSprite.AnimationFrame>()
                        {
                            new FarmerSprite.AnimationFrame(101, 1000, 0, false, !flip,
                                (AnimatedSprite.endOfAnimationBehavior) null, false, 0),
                            new FarmerSprite.AnimationFrame(6, 1, false, !flip,
                                new AnimatedSprite.endOfAnimationBehavior(Farmer.completelyStopAnimating), true)
                        }.ToArray());

                        if (who.IsLocalPlayer)
                        {
                            DataLoader.Helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue().broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite[1]
                            {
                                new TemporaryAnimatedSprite("LooseSprites\\Cursors",
                                    new Microsoft.Xna.Framework.Rectangle(211, 428, 7, 6), 2000f, 1, 0,
                                    new Vector2((float) __instance.getTileX(), (float) __instance.getTileY()) *
                                    64f + new Vector2(16f, -64f), false, false, 1f, 0.0f, Color.White, 4f, 0.0f,
                                    0.0f, 0.0f, false)
                                {
                                    motion = new Vector2(0.0f, -0.5f),
                                    alphaFade = 0.01f
                                }
                            });
                            location.playSound("dwop");
                        }

                        who.CanMove = false;
                        who.FarmerSprite.PauseForSingleAnimation = false;
                        who.faceDirection(flip ? 3 : 1);
                        who.FarmerSprite.animateOnce(new List<FarmerSprite.AnimationFrame>()
                        {
                            new FarmerSprite.AnimationFrame(101, 1000, 0, false, flip,
                                (AnimatedSprite.endOfAnimationBehavior) null, false, 0),
                            new FarmerSprite.AnimationFrame(6, 1, false, flip,
                                new AnimatedSprite.endOfAnimationBehavior(Farmer.completelyStopAnimating), true)
                        }.ToArray());
                        __result = true;

                        DataLoader.Helper.Multiplayer.SendMessage(new KissingMessage(who.UniqueMultiplayerID,__instance.UniqueMultiplayerID), CustomKissingModEntry.MessageType);
                    }
                }
            }
        }
    }
}
