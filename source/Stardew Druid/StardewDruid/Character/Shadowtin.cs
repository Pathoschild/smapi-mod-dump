/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewDruid.Cast;
using StardewDruid.Cast.Mists;
using StardewDruid.Cast.Weald;
using StardewDruid.Data;
using StardewDruid.Event;
using StardewDruid.Render;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.FruitTrees;
using StardewValley.Internal;
using StardewValley.Monsters;
using StardewValley.Network;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using static StardewDruid.Cast.SpellHandle;

namespace StardewDruid.Character
{
    public class Shadowtin : StardewDruid.Character.Character
    {

        public WeaponRender weaponRender;

        public List<Vector2> ritesDone = new();

        public Shadowtin()
        {
        }

        public Shadowtin(CharacterHandle.characters type)
          : base(type)
        {

            
        }

        public override void LoadOut()
        {
            
            characterType = CharacterHandle.characters.Shadowtin;

            weaponRender = new();

            weaponRender.LoadWeapon(WeaponRender.weapons.carnyx);

            base.LoadOut();

            idleFrames = new()
            {
                [0] = new()
                {
                    new Rectangle(128, 320, 64, 64),
                    new Rectangle(192, 320, 64, 64),
                    new Rectangle(0, 320, 64, 64),
                    new Rectangle(64, 320, 64, 64),
                },
                [1] = new()
                {
                    new Rectangle(192, 320, 64, 64),
                    new Rectangle(0, 320, 64, 64),
                    new Rectangle(64, 320, 64, 64),
                    new Rectangle(128, 320, 64, 64),
                },
                [2] = new()
                {
                    new Rectangle(0, 320, 64, 64),
                    new Rectangle(64, 320, 64, 64),
                    new Rectangle(128, 320, 64, 64),
                    new Rectangle(192, 320, 64, 64),
                },
                [3] = new()
                {
                    new Rectangle(64, 320, 64, 64),
                    new Rectangle(128, 320, 64, 64),
                    new Rectangle(192, 320, 64, 64),
                    new Rectangle(0, 320, 64, 64),
                },
            };
        }

        public override void draw(SpriteBatch b, float alpha = 1)
        {

            if (IsInvisible || !Utility.isOnScreen(Position, 128))
            {
                return;
            }

            if (characterTexture == null)
            {

                return;

            }

            Vector2 localPosition = getLocalPosition(Game1.viewport);

            float drawLayer = (float)StandingPixel.Y / 10000f;

            bool flippant = (netDirection.Value % 2 == 0 && netAlternative.Value == 3);

            bool flippity = flippant || netDirection.Value == 3;

            DrawEmote(b);

            if (netStandbyActive.Value)
            {

                DrawStandby(b, localPosition, drawLayer);

                return;

            }
            else if (netHaltActive.Value)
            {

                if (onAlert && idleTimer > 0)
                {

                    DrawAlert(b, localPosition, drawLayer);

                }
                else
                {
                    b.Draw(
                        characterTexture,
                        localPosition - new Vector2(32, 64f),
                        haltFrames[netDirection.Value][0],
                        Color.White,
                        0f,
                        Vector2.Zero,
                        4f,
                        flippant ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                        drawLayer
                    );
                }

            }
            else if (netSweepActive.Value)
            {

                Vector2 sweepVector = localPosition - new Vector2(32, 64f);

                b.Draw(
                     characterTexture,
                     localPosition - new Vector2(32, 64f),
                     sweepFrames[netDirection.Value][sweepFrame],
                     Color.White,
                     0f,
                     Vector2.Zero,
                     4f,
                     flippity ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                     drawLayer
                 );

                weaponRender.DrawWeapon(b, sweepVector, drawLayer, new() { source = sweepFrames[netDirection.Value][sweepFrame], flipped = flippity });

                weaponRender.DrawSwipe(b, sweepVector, drawLayer, new() { source = sweepFrames[netDirection.Value][sweepFrame], flipped = flippity });

            }
            else if (netSpecialActive.Value)
            {

                b.Draw(
                    characterTexture,
                    localPosition - new Vector2(32, 64f),
                    specialFrames[netDirection.Value][specialFrame],
                    Color.White,
                    0.0f,
                    Vector2.Zero,
                    4f,
                    flippant ? (SpriteEffects)1 : 0,
                    drawLayer
                );

            }
            else if (netDashActive.Value)
            {

                int dashSeries = netDirection.Value + (netDashProgress.Value * 4);

                int dashSetto = Math.Min(dashFrame, (dashFrames[dashSeries].Count - 1));

                b.Draw(
                    characterTexture,
                    localPosition - new Vector2(32, 64f + dashHeight),
                    dashFrames[dashSeries][dashSetto],
                    Color.White,
                    0f,
                    Vector2.Zero,
                    4f,
                    flippant ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    drawLayer
                );

            }
            else if (netSmashActive.Value)
            {
                int smashSeries = netDirection.Value + (netDashProgress.Value * 4);

                int smashSetto = Math.Min(dashFrame, (smashFrames[smashSeries].Count - 1));

                Vector2 smashVector = localPosition - new Vector2(32, 64f + dashHeight);

                b.Draw(
                    characterTexture,
                    smashVector,
                    smashFrames[smashSeries][smashSetto],
                    Color.White,
                    0f,
                    Vector2.Zero,
                    4f,
                    flippity ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    drawLayer
                );

                weaponRender.DrawWeapon(b, smashVector, drawLayer, new() { source = smashFrames[smashSeries][smashSetto], flipped = flippity });
                
                if(netDashProgress.Value >= 2)
                {

                    weaponRender.DrawSwipe(b, smashVector, drawLayer, new() { source = smashFrames[smashSeries][smashSetto], flipped = flippity });

                }

            }
            else
            {

                if (onAlert && idleTimer > 0)
                {

                    DrawAlert(b, localPosition, drawLayer);

                }
                else
                {

                    b.Draw(
                        characterTexture,
                        localPosition - new Vector2(32, 64f),
                        walkFrames[netDirection.Value][moveFrame],
                        Color.White,
                        0f,
                        Vector2.Zero,
                        4f,
                        flippant ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                        drawLayer
                    );

                }

            }

            DrawShadow(b, localPosition, drawLayer);

        }
        public override void DrawAlert(SpriteBatch b, Vector2 localPosition, float drawLayer)
        {

            Vector2 alertVector = localPosition - new Vector2(32, 64f);

            Rectangle alertFrame = alertFrames[netDirection.Value][0];

            bool flippant = (netDirection.Value % 2 == 0 && netAlternative.Value == 3);

            b.Draw(
                 characterTexture,
                 alertVector,
                 alertFrame,
                 Color.White,
                 0f,
                 Vector2.Zero,
                 4f,
                 flippant ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                 drawLayer
             );

            weaponRender.DrawWeapon(b, alertVector, drawLayer, new() { source = alertFrame, flipped = flippant });

        }

        public override void DrawStandby(SpriteBatch b, Vector2 localPosition, float drawLayer)
        {

            int chooseFrame = IdleFrame();

            b.Draw(
                 characterTexture,
                 localPosition - new Vector2(88, 112f),
                 idleFrames[netDirection.Value][chooseFrame],
                 Color.White,
                 0f,
                 Vector2.Zero,
                 3.75f,
                 flip || (netDirection.Value % 2 == 0 && netAlternative.Value == 3) ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                 drawLayer
             );

            return;

        }

        public override bool TargetWork()
        {

            Vector2 currentVector = new((int)(Position.X / 64), (int)(Position.Y / 64));

            List<Vector2> objectVectors = new List<Vector2>();

            for (int i = 0; i < 6; i++)
            {

                if (currentLocation.objects.Count() == 0)
                {
                    break;

                }

                objectVectors = ModUtility.GetTilesWithinRadius(currentLocation, currentVector, i); ;

                foreach (Vector2 objectVector in objectVectors)
                {

                    if (currentLocation.objects.ContainsKey(objectVector))
                    {

                        StardewValley.Object targetObject = currentLocation.objects[objectVector];

                        if (targetObject.Name.Contains("Artifact Spot") || targetObject.isForage())
                        {

                            workVector = objectVector;

                            Mod.instance.iconData.AnimateQuickWarp(currentLocation, Position, true);

                            Position = (workVector * 64);

                            SettleOccupied();

                            Mod.instance.iconData.AnimateQuickWarp(currentLocation, Position);

                            netWorkActive.Set(true);

                            netSpecialActive.Set(true);

                            specialTimer = 60;

                            return true;

                        }

                    }

                }

            }

            return false;

        }

        public override void PerformWork()
        {

            if (specialTimer == 30)
            {

                if (currentLocation.objects.ContainsKey(workVector))
                {

                    StardewValley.Object targetObject = currentLocation.objects[workVector];

                    if (targetObject.Name.Contains("Artifact Spot"))
                    {

                        currentLocation.digUpArtifactSpot((int)workVector.X, (int)workVector.Y, Game1.player);

                        currentLocation.objects.Remove(workVector);

                    }
                    else if (targetObject.isForage())
                    {

                        if (!Mod.instance.chests.ContainsKey(CharacterHandle.characters.Shadowtin))
                        {

                            Mod.instance.chests[CharacterHandle.characters.Shadowtin] = new();

                        }

                        Chest chest = Mod.instance.chests[CharacterHandle.characters.Shadowtin];

                        StardewValley.Item objectInstance = targetObject.getOne();

                        if (chest.addItem(objectInstance) != null)
                        {

                            ThrowHandle throwItem = new(Game1.player, Position, objectInstance);

                        }

                        currentLocation.objects.Remove(workVector);

                    }

                }

            }

        }

    }

}
