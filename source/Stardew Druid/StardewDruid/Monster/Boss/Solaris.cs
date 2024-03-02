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
using StardewModdingAPI;
using StardewValley;
using StardewValley.Network;
using System;
using System.Collections.Generic;

namespace StardewDruid.Monster.Boss
{

    public class Solaris : Boss.Dragon
    {

        public Solaris() { }

        public Solaris(Vector2 vector, int combatModifier, string SpawnName)
            : base(vector, combatModifier, SpawnName, "Fly")
        {

            Slipperiness = 24 + Game1.random.Next(-10, 10);

            IsWalkingTowardPlayer = true;

            if (realName.Value.Contains("Voidle"))
            {

                objectsToDrop.Add(769);

                if (Game1.random.Next(3) == 0)
                {
                    objectsToDrop.Add(769);
                }

            }
            else
            {

                objectsToDrop.Add(768);

                if (Game1.random.Next(3) == 0)
                {
                    objectsToDrop.Add(768);
                }

            }

            if (realName.Value.Contains("Zero"))
            {

                Health = combatModifier * 10;

                Scale = 0.75f;

                DamageToFarmer = Math.Max(5, Math.Min(10, combatModifier / 2));

            }
            else if (realName.Value.Contains("Prime"))
            {

                Health = combatModifier * 25;

                Scale = 1.25f;

                DamageToFarmer = Math.Max(10, Math.Min(20, combatModifier));


            }
            else
            {
                Health = combatModifier * 100;

                DamageToFarmer = Math.Max(10, Math.Min(40, combatModifier * 2));

            }

            MaxHealth = Health;

        }

        public override void LoadOut()
        {
            BaseWalk();
            BaseFlight();
            BaseSpecial();

            abilities = 1;

            ouchList = new()
            {
                "seep", "SEEP"
            };

            if (realName.Value.Contains("Zero"))
            {
                
                walkFrames = WalkFrames(16,16);

                specialFrames = walkFrames;

                flightFrames = walkFrames;

                overHead = new(16, -64);
            
            }
            else
            {

                overHead = new(16, -144);
            
            }

            flightIncrement = 12;

            if (realName.Value.Contains("Zero"))
            {
                cooldownInterval = 160;

            }
            else if (realName.Value.Contains("Prime"))
            {

                cooldownInterval = 80;

                flightSpeed = 15;

            }
            else
            {

                cooldownInterval = 240;

                flightSpeed = 12;

            }
            
            loadedOut = true;

        }

        public override Rectangle GetBoundingBox()
        {

            float height = 16 * Scale;

            float width = 16 * Scale;

            Rectangle box = new((int)(Position.X + 32 - width / 2), (int)(Position.Y + 32 - height / 2), (int)width, (int)height);

            return box;

        }

        public override void draw(SpriteBatch b)
        {
            return;
        }

        public override void drawAboveAllLayers(SpriteBatch b)
        {
            if (!Utility.isOnScreen(Position, 128))
            {

                return;

            }

            if (currentLocation != null && currentLocation.treatAsOutdoors.Value)
            {
                
                return;

            }

            b.Draw(characterTexture, getLocalPosition(Game1.viewport) + new Vector2(32f, GetBoundingBox().Height / 2 - 32), new Rectangle?(walkFrames[netDirection.Value][netWalkFrame.Value]), Color.White * 0.85f, 0.0f, new Vector2(8f, 16f), Math.Max(0.2f, scale) * 4f, netAlternative.Value == 3 || netDirection.Value == 3 ? (SpriteEffects)1 : 0, Math.Max(0f, drawOnTop ? 0.991f : (getStandingY() + 8) / 10000f));

            b.Draw(Game1.shadowTexture, getLocalPosition(Game1.viewport) + new Vector2(32f, GetBoundingBox().Height / 2), Game1.shadowTexture.Bounds, Color.White, 0f, new Vector2(Game1.shadowTexture.Bounds.Center.X, Game1.shadowTexture.Bounds.Center.Y), 4f, SpriteEffects.None, (getStandingY() - 1) / 10000f);

        }

        public override void drawAboveAlwaysFrontLayer(SpriteBatch b)
        {

            base.drawAboveAlwaysFrontLayer(b);

            if (currentLocation != null && !currentLocation.treatAsOutdoors.Value)
            {

                return;

            }

            b.Draw(characterTexture, getLocalPosition(Game1.viewport) + new Vector2(32f, GetBoundingBox().Height / 2 - 32), new Rectangle?(walkFrames[netDirection.Value][netWalkFrame.Value]), Color.White * 0.85f, 0.0f, new Vector2(8f, 16f), Math.Max(0.2f, scale) * 4f, netAlternative.Value == 3 || netDirection.Value == 3 ? (SpriteEffects)1 : 0, Math.Max(0f, drawOnTop ? 0.991f : (getStandingY() + 8) / 10000f));

            b.Draw(Game1.shadowTexture, getLocalPosition(Game1.viewport) + new Vector2(32f, GetBoundingBox().Height / 2), Game1.shadowTexture.Bounds, Color.White, 0f, new Vector2(Game1.shadowTexture.Bounds.Center.X, Game1.shadowTexture.Bounds.Center.Y), 4f, SpriteEffects.None, (getStandingY() - 1) / 10000f);

        
        }

    }

}
