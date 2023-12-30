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
using StardewDruid.Map;
using StardewValley;
using StardewValley.BellsAndWhistles;
using System;
using System.Collections.Generic;
using System.IO;

#nullable disable
namespace StardewDruid.Monster
{
    public class Reaper : Boss
    {
        public int bobHeight;
        public Queue<Rectangle> blastZone;
        public int blastRadius;

        public Reaper()
        {
        }

        public Reaper(Vector2 vector, int CombatModifier)
          : base(vector, CombatModifier, "Reaper")
        {
            blastZone = new Queue<Rectangle>();
        }

        public override void LoadOut()
        {

            characterTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", Name + ".png"));

            loadedOut = true;

        }

        public override void BaseMode()
        {
            Health = combatModifier * 12;
            MaxHealth = Health;
            DamageToFarmer = (int)(combatModifier * 0.1);
            ouchList = new List<string>()
              {
                "reap",
                "...you cannot defy fate...",
                "...pain..."
              };
            dialogueList = new List<string>()
              {
                "Do not touch the Prime!",
                "How long has it been since I saw...",
                "The dragon's power is mine to use!",
                "I will not stray from my purpose",
                "Are you a spy of the fallen one?",
                "The undervalley... I must...",
                "I will reap, and reap, and reap"
              };
            flightIncrement = 12;
            flightHeight = 4;
            flightCeiling = 2;
            flightFloor = 2;
            flightLast = 3;
            flightIncrement = 9;
            fireCeiling = 0;
            fireFloor = 0;
            specialThreshold = 480;
            reachThreshold = 64;
            haltActive = true;
            haltTimer = 20;
            cooldownTimer = 48;
            blastRadius = 1;
            fireInterval = 24;
            cooldownInterval = 48;
        }

        public override void HardMode()
        {
            Health *= 3;
            Health /= 2;
            MaxHealth = Health;
            DamageToFarmer *= 3;
            DamageToFarmer /= 2;
            ouchList = new List<string>()
              {
                "reap",
                "...you cannot defy fate...",
                "I'VE HAD ENOUGH OF THIS"
              };
            dialogueList = new List<string>()
              {
                "The dragon's power is mine to use!",
                "I will not stray from my purpose",
                "Are you a spy of the fallen one?",
                "The undervalley... I must...",
                "I will reap, and reap, and reap",
                "FORTUMEI... PLEASE... I BEG YOU",
                "ALL WILL BE REAPED"
              };
            blastRadius = 2;
            fireInterval = 18;
            cooldownInterval = 40;
        }

        public override Rectangle GetBoundingBox()
        {
            Vector2 position = Position;
            return new Rectangle((int)position.X - 32, (int)position.Y - netFlightHeight.Value, 128, 144);
        }

        public override void draw(SpriteBatch b, float alpha = 1f)
        {
            if (IsInvisible || !Utility.isOnScreen(Position, 128))
            {
                return;
            }

            if (!loadedOut)
            {

                Sprite.spriteTexture = CharacterData.CharacterTexture(Name);

                Sprite.textureName.Value = "18465_" + Name;

                Sprite.loadedTexture = Sprite.textureName.Value;

                LoadOut();

            }

            Vector2 localPosition = getLocalPosition(Game1.viewport);
            float drawLayer = Game1.player.getDrawLayer();
            if (IsEmoting && !Game1.eventUp)
            {
                localPosition.Y -= 32 + Sprite.SpriteHeight * 4;
                b.Draw(Game1.emoteSpriteSheet, localPosition, new Rectangle?(new Rectangle(CurrentEmoteIndex * 16 % Game1.emoteSpriteSheet.Width, CurrentEmoteIndex * 16 / Game1.emoteSpriteSheet.Width * 16, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, 0, drawLayer);
            }
            b.Draw(Game1.shadowTexture, new(localPosition.X, localPosition.Y + 128f), new Rectangle?(Game1.shadowTexture.Bounds), Color.White, 0.0f, Vector2.Zero, 4f, 0, Math.Max(0.0f, getStandingY() / 10000f) - 1E-06f);
            if (netDashActive)
            {
                Rectangle rectangle = new(netFlightFrame * 64, 0, 64, 48);
                //b.Draw(Sprite.Texture, new Vector2(localPosition.X - 96f, localPosition.Y - 48f - netFlightHeight), new Rectangle?(rectangle), Color.White * 0.65f, rotation, new Vector2(0.0f, 0.0f), 4f, flip || netDirection == 3 ? (SpriteEffects)1 : 0, drawLayer);
                b.Draw(characterTexture, new Vector2(localPosition.X - 96f, localPosition.Y - 48f - netFlightHeight), new Rectangle?(rectangle), Color.White * 0.65f, rotation, new Vector2(0.0f, 0.0f), 4f, flip || netDirection == 3 ? (SpriteEffects)1 : 0, drawLayer);

            }
            else if (netFireActive)
            {
                if (bobHeight <= 0)
                    ++bobHeight;
                else if (bobHeight >= 64)
                    --bobHeight;
                //b.Draw(Sprite.Texture, new Vector2(localPosition.X - 96f, localPosition.Y - 48f - bobHeight), new Rectangle?(new Rectangle(256, 0, 64, 48)), Color.White * 0.65f, rotation, new Vector2(0.0f, 0.0f), 4f, flip || netDirection == 3 ? (SpriteEffects)1 : 0, drawLayer);
                b.Draw(characterTexture, new Vector2(localPosition.X - 96f, localPosition.Y - 48f - bobHeight), new Rectangle?(new Rectangle(256, 0, 64, 48)), Color.White * 0.65f, rotation, new Vector2(0.0f, 0.0f), 4f, flip || netDirection == 3 ? (SpriteEffects)1 : 0, drawLayer);

            }
            else
            {
                //b.Draw(Sprite.Texture, new Vector2(localPosition.X - 96f, localPosition.Y - 48f), new Rectangle?(new Rectangle(0, 0, 64, 48)), Color.White * 0.65f, 0.0f, new Vector2(0.0f, 0.0f), 4f, flip ? (SpriteEffects)1 : 0, drawLayer);
                b.Draw(characterTexture, new Vector2(localPosition.X - 96f, localPosition.Y - 48f), new Rectangle?(new Rectangle(0, 0, 64, 48)), Color.White * 0.65f, 0.0f, new Vector2(0.0f, 0.0f), 4f, flip ? (SpriteEffects)1 : 0, drawLayer);

            }

        }

        public override void drawAboveAlwaysFrontLayer(SpriteBatch b)
        {
            if (textAboveHeadTimer <= 0 || textAboveHead == null)
                return;
            Vector2 localPosition = getLocalPosition(Game1.viewport);
            SpriteText.drawStringWithScrollCenteredAt(b, textAboveHead, (int)localPosition.X, (int)localPosition.Y - 160, "", textAboveHeadAlpha, textAboveHeadColor, 1, (float)(getTileY() * 64 / 10000.0 + 1.0 / 1000.0 + getTileX() / 10000.0), false);
        }

        public override void SpecialAttack()
        {
            Vector2 zero = Vector2.Zero;
            int num = new Random().Next(4);
            switch (moveDirection)
            {
                case 0:

                    zero = new((int)(Position.X / 64.0) + 3, (int)(Position.Y / 64.0) - (4 + num));
                    if (altDirection == 3 || flip)
                    {
                        zero.X -= 6f;
                        break;
                    }
                    break;
                case 1:

                    zero = new((int)(Position.X / 64.0) + (5 + num), (int)(Position.Y / 64.0));
                    break;
                case 2:

                    zero = new((int)(Position.X / 64.0) + 3, (int)(Position.Y / 64.0) + (4 + num));
                    if (altDirection == 3 || flip)
                    {
                        zero.X -= 6f;
                        break;
                    }
                    break;
                default:

                    zero = new((int)(Position.X / 64.0) - (5 + num), (int)(Position.Y / 64.0));
                    break;
            }

            Vector2 zero64 = new(zero.X * 64, zero.Y * 64);
            currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(0, 125f, 4, 1, new(zero64.X - 32 - (32 * blastRadius), zero64.Y - 32 - (32 * blastRadius)), false, false)
            {
                sourceRect = new Rectangle(0, 0, 64, 64),
                sourceRectStartingPos = new Vector2(0.0f, 0.0f),
                texture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "EnergyBomb.png")),
                scale = 2f + blastRadius,
                timeBasedMotion = true,
                layerDepth = 999f,
                rotationChange = 0.00628f,
                alphaFade = 1f / 1000f
            });
            currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(23, 500f, 6, 1, new Vector2(zero.X * 64f, zero.Y * 64f), false, Game1.random.NextDouble() < 0.5)
            {
                texture = Game1.mouseCursors,
                light = true,
                lightRadius = 2 + blastRadius,
                lightcolor = Color.Black,
                alphaFade = 0.03f,
                Parent = currentLocation
            });
            Rectangle rectangle = new((int)((zero.X - (double)blastRadius) * 64.0), (int)((zero.Y - (double)blastRadius) * 64.0), 64 + blastRadius * 128, 64 + blastRadius * 128);
            blastZone.Enqueue(rectangle);

            DelayedAction.functionAfterDelay(TriggerBlast, 375);
        }

        public void TriggerBlast()
        {
            ModUtility.DamageFarmers(currentLocation, blastZone.Dequeue(), (int)(DamageToFarmer * 0.4), this);
        }
    }
}
