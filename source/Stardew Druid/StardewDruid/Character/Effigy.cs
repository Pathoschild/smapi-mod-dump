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

namespace StardewDruid.Character
{
    public class Effigy : StardewDruid.Character.Character
    {
        new public CharacterData.characters characterType = CharacterData.characters.Effigy;

        public List<Vector2> ritesDone = new();

        public Texture2D weaponTexture;
        public Dictionary<int, List<Rectangle>> weaponFrames;

        public Texture2D swipeTexture;
        public Dictionary<int, List<Rectangle>> swipeFrames;

        public Effigy()
        {
        }

        public Effigy(CharacterData.characters type)
          : base(type)
        {

            
        }


        public override void LoadOut()
        {
            base.LoadOut();

            specialScheme = IconData.schemes.stars;

            weaponTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "WeaponSword.png"));

            weaponFrames = new()
            {
                [256] = new()
                {
                    new Rectangle(0, 0, 64, 64),
                    new Rectangle(64, 0, 64, 64),
                    new Rectangle(128, 0, 64, 64),
                    new Rectangle(192, 0, 64, 64),
                    new Rectangle(0, 192, 64, 64),
                    new Rectangle(64, 192, 64, 64),
                    new Rectangle(128, 192, 64, 64),
                    new Rectangle(192, 192, 64, 64),
                },
                [288] = new()
                {
                    new Rectangle(0, 128, 64, 64),
                    new Rectangle(64, 128, 64, 64),
                    new Rectangle(128, 128, 64, 64),
                    new Rectangle(192, 128, 64, 64),
                    new Rectangle(0, 256, 64, 64),
                    new Rectangle(64, 256, 64, 64),
                    new Rectangle(128, 256, 64, 64),
                    new Rectangle(192, 256, 64, 64),
                },
                [320] = new()
                {
                    new Rectangle(0, 64, 64, 64),
                    new Rectangle(64, 64, 64, 64),
                    new Rectangle(128, 64, 64, 64),
                    new Rectangle(192, 64, 64, 64),
                    new Rectangle(0, 320, 64, 64),
                    new Rectangle(64, 320, 64, 64),
                    new Rectangle(128, 320, 64, 64),
                    new Rectangle(64, 320, 64, 64),
                },
            };

            swipeTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "WeaponSwipe.png"));

            swipeFrames = new()
            {
                [256] = new()
                {
                    new Rectangle(0, 0, 64, 64),
                    new Rectangle(64, 0, 64, 64),
                    new Rectangle(128, 0, 64, 64),
                    new Rectangle(192, 0, 64, 64),
                    new Rectangle(0, 192, 64, 64),
                    new Rectangle(64, 192, 64, 64),
                    new Rectangle(128, 192, 64, 64),
                    new Rectangle(192, 192, 64, 64),
                },
                [288] = new()
                {
                    new Rectangle(0, 128, 64, 64),
                    new Rectangle(64, 128, 64, 64),
                    new Rectangle(128, 128, 64, 64),
                    new Rectangle(192, 128, 64, 64),
                    new Rectangle(0, 256, 64, 64),
                    new Rectangle(64, 256, 64, 64),
                    new Rectangle(128, 256, 64, 64),
                    new Rectangle(192, 256, 64, 64),
                },
                [320] = new()
                {
                    new Rectangle(0, 64, 64, 64),
                    new Rectangle(64, 64, 64, 64),
                    new Rectangle(128, 64, 64, 64),
                    new Rectangle(192, 64, 64, 64),

                },
            };

        }
        

        public override void DrawWeapon(SpriteBatch b, Vector2 spriteVector, float drawLayer, Rectangle frame)
        {
            
            b.Draw(
                 weaponTexture,
                 spriteVector - new Vector2(64, 64f),
                 weaponFrames[frame.Y][frame.X == 0 ? 0 : frame.X/32],
                 Color.White,
                 0f,
                 Vector2.Zero,
                 4f,
                 (netDirection.Value % 2 == 0 && netAlternative.Value == 3) || netDirection.Value == 3 ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                 drawLayer + 0.0001f
             );

            int swipeIndex = frame.X == 0 ? 0 : frame.X / 32;

            if(swipeFrames[frame.Y].Count <= swipeIndex)
            {

                return;

            }

            if(netSmashActive.Value && netDashProgress.Value < 2)
            {

                return;

            }

            b.Draw(
                 swipeTexture,
                 spriteVector - new Vector2(64, 64f),
                 swipeFrames[frame.Y][swipeIndex],
                 Color.White * 0.65f,
                 0f,
                 Vector2.Zero,
                 4f,
                 (netDirection.Value % 2 == 0 && netAlternative.Value == 3) || netDirection.Value == 3 ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                 drawLayer + 0.0002f
             );

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

        public int IdleFrame()
        {

            int interval = 12000 / idleFrames[0].Count();

            int timeLapse = (int)(Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 12000);

            if (timeLapse == 0) { return 0; }

            int frame = (int)timeLapse / interval;

            return frame;

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

            if(specialTimer == 20)
            {

                Artifice artificeHandle = new();

                artificeHandle.ArtificeScarecrow(currentLocation, workVector);

            }

        }

    }

}
