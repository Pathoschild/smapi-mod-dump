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
    public class Effigy : StardewDruid.Character.Character
    {

        public WeaponRender weaponRender;

        public List<Vector2> ritesDone = new();

        public Effigy()
        {
        }

        public Effigy(CharacterHandle.characters type)
          : base(type)
        {

            
        }

        public override void LoadOut()
        {
            
            characterType = CharacterHandle.characters.Effigy;

            weaponRender = new();

            weaponRender.LoadWeapon(WeaponRender.weapons.sword);

            base.LoadOut();

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
                localPosition - new Vector2(32f, 64f),
                idleFrames[0][chooseFrame],
                Color.White,
                0f,
                Vector2.Zero,
                4f,
                netDirection.Value == 1 || netAlternative.Value == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                drawLayer
            );

            b.Draw(
                characterTexture,
                localPosition - new Vector2(30f, 60f),
                idleFrames[0][chooseFrame],
                Color.Black * 0.25f,
                0f,
                Vector2.Zero,
                4f,
                netDirection.Value == 1 || netAlternative.Value == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                drawLayer - 0.001f
            );

            return;

        }

        public override List<Vector2> RoamAnalysis()
        {

            List<Vector2> collection = base.RoamAnalysis();

            if (Game1.currentSeason == "winter")
            {
                
                return collection;

            }

            List<Vector2> scarelist = new List<Vector2>();

            int takeABreak = 0;

            foreach (Dictionary<Vector2, StardewValley.Object> dictionary in currentLocation.Objects)
            {
                
                foreach (KeyValuePair<Vector2, StardewValley.Object> keyValuePair in dictionary)
                {
                    
                    if (keyValuePair.Value.IsScarecrow())
                    {

                        Vector2 scareVector = new(keyValuePair.Key.X * 64f, keyValuePair.Key.Y * 64f);

                        scarelist.Add(scareVector);

                        takeABreak++;

                    }

                    if (takeABreak >= 4)
                    {

                        scarelist.Add(new Vector2(-1f));

                        takeABreak = 0;

                    }

                }

                if (scarelist.Count >= 30)
                {

                    break;

                }
                    
            }

            scarelist.AddRange(collection);
            
            return scarelist;
        
        }

        public override bool TargetWork()
        {

            if (Game1.currentSeason == "winter")
            {
                return false;
            }

            if(!currentLocation.IsFarm && !currentLocation.IsGreenhouse)
            {

                return false;

            }

            if (currentLocation.objects.Count() < 0)
            {
                return false;
            }
            
            List<Vector2> tileVectors;

            for (int i = 0; i < 4; i++)
            {

                tileVectors = ModUtility.GetTilesWithinRadius(currentLocation, occupied, i);

                foreach (Vector2 scarevector in tileVectors)
                {

                    if (ritesDone.Contains(scarevector))
                    {

                        continue;

                    }

                    if (currentLocation.objects.ContainsKey(scarevector))
                    {

                        if (currentLocation.Objects[scarevector].IsScarecrow())
                        {

                            ResetActives();

                            LookAtTarget(scarevector * 64,true);

                            netSpecialActive.Set(true);

                            netWorkActive.Set(true);

                            specialTimer = 90;

                            workVector = scarevector;

                            ritesDone.Add(scarevector);

                            return true;

                        }

                    }
                
                }
            
            }

            return false;

        }

        public override void PerformWork()
        {

            if(specialTimer == 80)
            {

                if (currentLocation.Name == Game1.player.currentLocation.Name && Utility.isOnScreen(Position, 128))
                {

                    Mod.instance.iconData.DecorativeIndicator(currentLocation, Position, IconData.decorations.weald, 3f, new());

                    TemporaryAnimatedSprite skyAnimation = Mod.instance.iconData.SkyIndicator(currentLocation, Position, IconData.skies.valley, 1f, new() { interval = 1000, });

                    skyAnimation.scaleChange = 0.002f;

                    skyAnimation.motion = new(-0.064f, -0.064f);

                    skyAnimation.timeBasedMotion = true;

                    Game1.player.currentLocation.playSound("discoverMineral", null, 1000);

                }

            }

            if(specialTimer == 50 && !Mod.instance.Config.disableSeeds)
            {

                Cultivate cultivateEvent = new();

                cultivateEvent.EventSetup(workVector * 64, "effigy_cultivate_" + workVector.ToString());

                cultivateEvent.inabsentia = true;

                cultivateEvent.EventActivate();

            }

            if(specialTimer == 20 && !Game1.IsRainingHere(currentLocation))
            {

                Artifice artificeHandle = new();

                artificeHandle.ArtificeScarecrow(currentLocation, workVector);

            }

        }

        public override bool SpecialAttack(StardewValley.Monsters.Monster monster)
        {

            ResetActives();

            netSpecialActive.Set(true);

            specialTimer = 90;

            cooldownTimer = cooldownInterval;

            LookAtTarget(monster.Position, true);

            SpellHandle special = new(currentLocation, monster.Position, GetBoundingBox().Center.ToVector2(), 192, -1, Mod.instance.CombatDamage() / 2);

            int outdoors = currentLocation.IsOutdoors ? 2 : 3;

            switch (Mod.instance.randomIndex.Next(outdoors))
            {

                case 2:

                    special.type = SpellHandle.spells.orbital;

                    special.missile = IconData.missiles.rockfall;

                    special.sound = sounds.boulderBreak;

                    Mod.instance.iconData.DecorativeIndicator(currentLocation, Position, IconData.decorations.weald, 3f, new());

                    break;

                case 1:

                    special.type = SpellHandle.spells.bolt;

                    Mod.instance.iconData.DecorativeIndicator(currentLocation, Position, IconData.decorations.mists, 3f, new());

                    break;

                case 0:

                    special.type = SpellHandle.spells.orbital;

                    special.missile = IconData.missiles.meteor;

                    special.projectile = 3;

                    special.sound = sounds.flameSpellHit;

                    Mod.instance.iconData.DecorativeIndicator(currentLocation, Position, IconData.decorations.stars, 3f, new());

                    break;


            }

            special.display = IconData.impacts.impact;

            special.power = 3;

            Mod.instance.spellRegister.Add(special);

            return true;

        }
        public override void NewDay()
        {

            ritesDone.Clear();

        }


    }

}
