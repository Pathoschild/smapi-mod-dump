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
using StardewDruid.Event;
using StardewDruid.Map;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace StardewDruid.Character
{
    public class Effigy : StardewDruid.Character.Character
    {
        public List<Vector2> ritesDone;
        public int riteIcon;
        public bool showIcon;
        public Texture2D bombTexture;
        public Texture2D iconsTexture;

        public NetBool netCastActive = new(false);
        public Dictionary<int, Rectangle> castFrames;

        public Effigy()
        {
        }

        public Effigy(Vector2 position, string map)
          : base(position, map, nameof(Effigy))
        {

            
        }

        public override void LoadOut()
        {

            characterTexture = CharacterData.CharacterTexture(Name);

            barrages = new();

            roamVectors = new List<Vector2>();

            eventVectors = new List<Vector2>();

            targetVectors = new();

            opponentThreshold = 640;

            gait = 2f;

            modeActive = mode.random;

            behaviourActive = behaviour.idle;

            idleInterval = 120;

            moveLength = 4;
            moveInterval = 12;

            specialInterval = 30;

            walkFrames = WalkFrames(32, 16);

            dashFrames = walkFrames;

            specialFrames = new()
            {
                [0] = new(0, 192, 32, 32),
                [1] = new(32, 192, 32, 32),
            };

            haltFrames = new()
            {
                [0] = new (0, 160, 32, 32),
                [1] = new (32, 160, 32, 32),
            };

            castFrames = new()
            {
                [0] = new(32, 128, 16, 32),
                [1] = new(16, 128, 16, 32),
                [2] = new(0, 128, 16, 32),
                [3] = new(48, 128, 16, 32),
            };

            ritesDone = new List<Vector2>();

            bombTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "BlueBomb.png"));

            iconsTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Icons.png"));

            loadedOut = true;

        }

        protected override void initNetFields()
        {
            base.initNetFields();
            NetFields.AddFields(new INetSerializable[1]
            {
                 netCastActive

            });
        }

        public override void draw(SpriteBatch b, float alpha = 1f)
        {
            
            if (IsInvisible || !Utility.isOnScreen(Position, 128))
            {
                return;
            }

            if(characterTexture == null)
            {

                return;

            }

            if (base.IsEmoting && !Game1.eventUp)
            {
                Vector2 localPosition2 = getLocalPosition(Game1.viewport);
                localPosition2.Y -= 160;
                b.Draw(Game1.emoteSpriteSheet, localPosition2, new Microsoft.Xna.Framework.Rectangle(base.CurrentEmoteIndex * 16 % Game1.emoteSpriteSheet.Width, base.CurrentEmoteIndex * 16 / Game1.emoteSpriteSheet.Width * 16, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, getStandingY() / 10000f);
            }
                
            Vector2 localPosition = getLocalPosition(Game1.viewport);

            if (netHaltActive.Value)
            {

                int chooseFrame = idleFrame.Value % 4;

                if(chooseFrame < 2 || !currentLocation.IsOutdoors)
                {
                    b.Draw(
                        characterTexture,
                        localPosition - new Vector2(0, 64),
                        walkFrames[netDirection.Value][0],
                        Color.White,
                        0f,
                        Vector2.Zero,
                        4f,
                        flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                        drawOnTop ? 0.991f : ((float)getStandingY() / 10000f)
                        );

                    DrawIcon(b, localPosition);

                    DrawShadow(b, localPosition);

                    return;

                }

                b.Draw(
                    characterTexture,
                    localPosition - new Vector2(32f, 64f),
                    haltFrames[chooseFrame - 2],
                    Color.White,
                    0f,
                    Vector2.Zero,
                    4f,
                    flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    Math.Max(0f, drawOnTop ? 0.991f : ((float)getStandingY() / 10000f))
                );

                b.Draw(
                    characterTexture,
                    localPosition - new Vector2(chooseFrame > 0 ? 32f : 0, 64f) + new Vector2(2f,4f),
                    haltFrames[chooseFrame-2],
                    Color.Black * 0.25f,
                    0f,
                    Vector2.Zero,
                    4f,
                    flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    Math.Max(0f, drawOnTop ? 0.990f : ((float)getStandingY() / 10000f) - 0.001f)
                );

                return;
            
            }

            if (netCastActive.Value)
            {

                b.Draw(
                    characterTexture,
                    localPosition - new Vector2(0,64),
                    castFrames[netDirection.Value],
                    Color.White,
                    0f,
                    Vector2.Zero,
                    4f,
                    flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    drawOnTop ? 0.991f : ((float)getStandingY() / 10000f)
                    );

                DrawIcon(b, localPosition);

                DrawShadow(b, localPosition);

                return;

            }
            
            if (netSpecialActive.Value)
            {

                b.Draw(
                    characterTexture,
                    localPosition - new Vector2(32, 64),
                    specialFrames[specialFrame.Value],
                    Color.White,
                    0f,
                    Vector2.Zero,
                    4f,
                    netDirection.Value == 3 ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    drawOnTop ? 0.991f : ((float)getStandingY() / 10000f)
                    );

                if (specialFrame.Value == 0)
                {

                    b.Draw(
                        bombTexture,
                        localPosition + new Vector2(netDirection.Value == 3 ? 48f : -48f, -40f),
                        new Rectangle(0, 0, 64, 64),
                        Color.White,
                        0f,
                        Vector2.Zero,
                        1f,
                        SpriteEffects.None,
                        Math.Max(0.0f, (getStandingY() / 10000f) + 0.0001f)
                        );

                }

                DrawShadow(b, localPosition);

                return;

            }

            b.Draw(
                characterTexture,
                localPosition - new Vector2(0, 64),
                walkFrames[netDirection.Value][moveFrame.Value],
                Color.White,
                0f,
                Vector2.Zero,
                4f,
                flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                drawOnTop ? 0.991f : ((float)getStandingY() / 10000f)
                );

            DrawIcon(b, localPosition);

            DrawShadow(b, localPosition);

        }

        public void DrawIcon(SpriteBatch b, Vector2 localPosition)
        {

            if (netDirection.Value != 0 || netSpecialActive.Value)
            {
                return;
            }

            int riteIcon;

            switch (Mod.instance.CurrentBlessing())
            {

                case "mists":
                    riteIcon = 2;
                    break;
                case "stars":
                    riteIcon = 3;
                    break;
                case "fates":
                    riteIcon = 4;
                    break;
                case "ether":
                    riteIcon = 6;
                    break;
                default: // weald
                    riteIcon = 1;
                    break;

            }

            b.Draw(
                iconsTexture,
                localPosition + new Vector2(16, 1),
                new Rectangle((riteIcon % 4) * 8, (riteIcon / 4) * 8, 8, 8),
                Color.White,
                0f,
                new Vector2(0, 0),
                4f,
                SpriteEffects.None,
                drawOnTop ? 0.992f : ((float)getStandingY() / 10000f) + 0.001f
            );

        }

        public void DrawShadow(SpriteBatch b, Vector2 localPosition)
        {
            b.Draw(
                Game1.shadowTexture,
                localPosition + new Vector2(6, 44f),
                Game1.shadowTexture.Bounds,
                Color.White * 0.65f,
                0f,
                Vector2.Zero,
                4f,
                SpriteEffects.None,
                Math.Max(0.0f, (getStandingY() / 10000f) - 0.0001f)
                );

        }

        public override bool checkAction(Farmer who, GameLocation l)
        {

            if(!base.checkAction(who, l))
            {

                return false;

            };
                
            if (!Mod.instance.dialogue.ContainsKey(nameof(Effigy)))
            {

                Dictionary<string, StardewDruid.Dialogue.Dialogue> dialogue = Mod.instance.dialogue;

                StardewDruid.Dialogue.Effigy effigy = new StardewDruid.Dialogue.Effigy();

                effigy.npc = this;

                dialogue[nameof(Effigy)] = effigy;

            }

            Mod.instance.dialogue[nameof(Effigy)].DialogueApproach();

            return true;

        }

        public override void ResetActives()
        {
            base.ResetActives();

            netCastActive.Set(false);

        }

        public override void UpdateBehaviour()
        {

            base.UpdateBehaviour();

            if (netCastActive.Value)
            {

                if (!netSpecialActive.Value)
                {

                    netCastActive.Set(false);

                }

            }

        }

        public override List<Vector2> RoamAnalysis()
        {
            
            List<Vector2> vector2List = new List<Vector2>();

            int takeABreak = 0;

            foreach (Dictionary<Vector2, StardewValley.Object> dictionary in currentLocation.Objects)
            {
                
                foreach (KeyValuePair<Vector2, StardewValley.Object> keyValuePair in dictionary)
                {
                    
                    if (keyValuePair.Value.IsScarecrow())
                    {

                        Vector2 scareVector = new(keyValuePair.Key.X * 64f, keyValuePair.Key.Y * 64f);

                        vector2List.Add(scareVector);

                        takeABreak++;

                    }

                    if (takeABreak >= 4)
                    {

                        vector2List.Add(new Vector2(-1f));

                        takeABreak = 0;

                    }

                }

                if (vector2List.Count >= 30)
                {

                    break;

                }
                    
            }
           
            List<Vector2> collection = base.RoamAnalysis();
            
            vector2List.AddRange(collection);
            
            return vector2List;
        
        }

        public override bool MonsterAttack(StardewValley.Monsters.Monster targetMonster)
        {

            float distance = Vector2.Distance(Position, targetMonster.Position);

            if (distance >= 128f && distance <= 640f)
            {

                Vector2 vector2 = new(targetMonster.Position.X - Position.X - 32f, targetMonster.Position.Y - Position.Y);//Vector2.op_Subtraction(((StardewValley.Character)targetOpponents.First<Monster>()).Position, Vector2.op_Addition(Position, new Vector2(32f, 0.0f)));

                if ((double)Math.Abs(vector2.Y) <= 128.0)
                {

                    netDirection.Set(1);

                    if (vector2.X < 0.001)
                    {
                        netDirection.Set(3);

                    }

                    netSpecialActive.Set(true);

                    behaviourActive = behaviour.special;

                    specialTimer = 60;

                    NextTarget(targetMonster.Position, -1);

                    ResetAll();

                    BarrageHandle fireball = new(currentLocation, targetMonster.getTileLocation(), getTileLocation(), 2, 1, "Blue", -1, Mod.instance.DamageLevel(), 3, 2);

                    fireball.type = BarrageHandle.barrageType.fireball;

                    barrages.Add(fireball);

                }
                else
                {

                    netCastActive.Set(true);

                    behaviourActive = behaviour.special;

                    netSpecialActive.Set(true);

                    specialTimer = 30;

                    NextTarget(targetMonster.Position, -1);

                    ResetAll();

                    List<int> diff = ModUtility.CalculatePush(currentLocation, targetMonster, Position, 64);

                    ModUtility.HitMonster(currentLocation, Game1.player, targetMonster, Mod.instance.DamageLevel() / 2, false, diffX: diff[0], diffY: diff[1]);

                    ModUtility.AnimateBolt(currentLocation, new Vector2(targetMonster.getTileLocation().X, targetMonster.getTileLocation().Y - 1), 1200);

                }

                return true;

            }

            return false;

        }

        public override void ReachedRoamPosition()
        {
            Vector2 vector2 = new(roamVectors[roamIndex].X / 64f, roamVectors[roamIndex].Y / 64f);//Vector2.op_Division(roamVectors[roamIndex], 64f);

            if(Game1.currentSeason == "winter")
            {
                return;
            }

            if (ritesDone.Contains(vector2) || !currentLocation.Objects.ContainsKey(vector2) || !currentLocation.Objects[vector2].IsScarecrow())
            {
                return;
            }

            netCastActive.Set(true);

            netSpecialActive.Set(true);

            specialTimer = 30;

            ResetAll();

            Rite rite = Mod.instance.NewRite(false);

            bool Reseed = !Mod.instance.EffectDisabled("Seeds");

            for (int level = 1; level < (Mod.instance.PowerLevel()+2); level++)
            {
                
                foreach (Vector2 tilesWithinRadius in ModUtility.GetTilesWithinRadius(currentLocation, vector2, level))
                {
                    
                    if (currentLocation.terrainFeatures.ContainsKey(tilesWithinRadius) && currentLocation.terrainFeatures[tilesWithinRadius].GetType().Name.ToString() == "HoeDirt")
                    {
                        rite.effectCasts[tilesWithinRadius] = new StardewDruid.Cast.Weald.Crop(tilesWithinRadius, rite, Reseed, true);
                    }

                }
            
            }
            
            if (currentLocation.Name == Game1.player.currentLocation.Name && Utility.isOnScreen(Position, 128))
            {
                
                ModUtility.AnimateRadiusDecoration(currentLocation, vector2, "Weald", 1f, 1f, 1500f);
                
                Game1.player.currentLocation.playSoundPitched("discoverMineral", 1000, 0);
            
            }
            
            rite.CastEffect(false);
            
            ritesDone.Add(vector2);
        
        }

    }

}
