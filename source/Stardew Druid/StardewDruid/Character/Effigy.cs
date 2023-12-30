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
using StardewDruid.Cast;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;

namespace StardewDruid.Character
{
    public class Effigy : StardewDruid.Character.Character
    {
        public List<Vector2> ritesDone;

        public Effigy()
        {
        }

        public Effigy(Vector2 position, string map)
          : base(position, map, nameof(Effigy))
        {
            ritesDone = new List<Vector2>();
            HideShadow = true;
        }

        public override void draw(SpriteBatch b, float alpha = 1f)
        {
            
            if (!Context.IsMainPlayer)
            {
                
                base.draw(b, alpha);

                return;

            }

            if (IsInvisible || !Utility.isOnScreen(Position, 128))
            {
                return;
            }

            if (base.IsEmoting && !Game1.eventUp)
            {
                Vector2 localPosition2 = getLocalPosition(Game1.viewport);
                localPosition2.Y -= 32 + Sprite.SpriteHeight * 4;
                b.Draw(Game1.emoteSpriteSheet, localPosition2, new Microsoft.Xna.Framework.Rectangle(base.CurrentEmoteIndex * 16 % Game1.emoteSpriteSheet.Width, base.CurrentEmoteIndex * 16 / Game1.emoteSpriteSheet.Width * 16, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, getStandingY() / 10000f);
            }
                
            Vector2 localPosition = getLocalPosition(Game1.viewport);

            if (timers.ContainsKey("idle"))
            {
                int num = timers["idle"] / 200 % 2 == 0 ? 0 : 32;

                b.Draw(
                    Sprite.Texture,
                    localPosition + new Vector2(32f, 16f),
                    new Rectangle(num, 352, 32, 32),
                    Color.White,
                    0f,
                    new Vector2(Sprite.SpriteWidth / 2, Sprite.SpriteHeight * 3f / 4f),
                    Math.Max(0.2f, scale) * 4f,
                    flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    Math.Max(0f, drawOnTop ? 0.991f : ((float)getStandingY() / 10000f))
                );

                return;

            }

            b.Draw(
                Sprite.Texture,
                localPosition + new Vector2(32f, 16f),
                Sprite.SourceRect,
                Color.White,
                0f,
                new Vector2(Sprite.SpriteWidth / 2, Sprite.SpriteHeight * 3f / 4f),
                Math.Max(0.2f, scale) * 4f,
                flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                Math.Max(0f, drawOnTop ? 0.991f : ((float)getStandingY() / 10000f))
                );


            b.Draw(
                Game1.shadowTexture,
                localPosition + new Vector2(32f, 40f),
                Game1.shadowTexture.Bounds,
                Color.White * 0.65f,
                0f,
                new Vector2(Game1.shadowTexture.Bounds.Center.X, Game1.shadowTexture.Bounds.Center.Y),
                4f,
                SpriteEffects.None,
                Math.Max(0.0f, getStandingY() / 10000f) - 0.0001f
                );


        }

        public override bool checkAction(Farmer who, GameLocation l)
        {
            base.checkAction(who, l);
            if (Context.IsMainPlayer)
                AdjustJacket();
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

                if (vector2List.Count >= 24)
                {

                    break;

                }
                    
            }
           
            List<Vector2> collection = base.RoamAnalysis();
            
            vector2List.AddRange(collection);
            
            return vector2List;
        
        }

        public void AnimateCast()
        {
            switch (moveDirection)
            {
                case 0:
                    switch (Mod.instance.CurrentBlessing())
                    {
                        case "mists":
                            Sprite.currentFrame = 26;
                            break;
                        case "stars":
                            Sprite.currentFrame = 34;
                            break;
                        case "fates":
                            Sprite.currentFrame = 42;
                            break;
                        default:
                            Sprite.currentFrame = 18;
                            break;
                    }
                    break;
                case 1:
                    Sprite.currentFrame = 17;
                    break;
                case 2:
                    Sprite.currentFrame = 16;
                    break;
                case 3:
                    Sprite.currentFrame = 19;
                    break;
            }
            Sprite.UpdateSourceRect();
        }

        public override void AnimateMovement(GameTime time)
        {
            if (timers.ContainsKey("cast"))
            {
                AnimateCast();
            }
            else
            {
                moveDown = false;
                moveLeft = false;
                moveRight = false;
                moveUp = false;
                FacingDirection = moveDirection;
                switch (moveDirection)
                {
                    case 0:
                        Sprite.AnimateUp(time, 0, "");
                        moveUp = true;
                        AdjustJacket();
                        break;
                    case 1:
                        Sprite.AnimateRight(time, 0, "");
                        moveRight = true;
                        break;
                    case 2:
                        Sprite.AnimateDown(time, 0, "");
                        moveDown = true;
                        break;
                    default:
                        Sprite.AnimateLeft(time, 0, "");
                        moveLeft = true;
                        break;
                }
            }
        }

        public void AdjustJacket()
        {
            if (moveDirection != 0 || Sprite.currentFrame < 8 || Sprite.currentFrame >= 12)
                return;
            switch (Mod.instance.CurrentBlessing())
            {
                case "weald":
                    return;
                case "mists":
                    Sprite.currentFrame += 12;
                    break;
                case "stars":
                    Sprite.currentFrame += 20;
                    break;
                case "fates":
                    Sprite.currentFrame += 28;
                    break;
            }
            Sprite.UpdateSourceRect();
        }

        public override void ReachedRoamPosition()
        {
            Vector2 vector2 = new(roamVectors[roamIndex].X / 64f, roamVectors[roamIndex].Y / 64f);//Vector2.op_Division(roamVectors[roamIndex], 64f);
            if (ritesDone.Contains(vector2) || !currentLocation.Objects.ContainsKey(vector2) || !currentLocation.Objects[vector2].IsScarecrow())
                return;
            Halt();
            AnimateCast();
            timers["cast"] = 30;
            Rite rite = Mod.instance.NewRite(false);
            bool Reseed = !Mod.instance.EffectDisabled("Seeds");
            for (int level = 1; level < 5; ++level)
            {
                foreach (Vector2 tilesWithinRadius in ModUtility.GetTilesWithinRadius(currentLocation, vector2, level))
                {
                    if (currentLocation.terrainFeatures.ContainsKey(tilesWithinRadius) && currentLocation.terrainFeatures[tilesWithinRadius].GetType().Name.ToString() == "HoeDirt")
                        rite.effectCasts[tilesWithinRadius] = new StardewDruid.Cast.Weald.Crop(tilesWithinRadius, rite, Reseed, true);
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
