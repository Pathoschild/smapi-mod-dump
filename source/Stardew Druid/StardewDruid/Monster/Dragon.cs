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
using StardewDruid.Character;
using StardewDruid.Data;
using StardewDruid.Event;
using StardewDruid.Render;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using StardewValley.Events;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace StardewDruid.Monster
{
    public class Dragon : StardewDruid.Monster.Boss
    {
        
        // ============================= Dragon Specific

        public NetBool netBreathActive = new NetBool(false);

        public DragonRender dragonRender;

        public Dragon()
        {
        }

        public Dragon(Vector2 vector, int CombatModifier, string name = "Dragon", string template = "Pepper Rex")
          : base(vector, CombatModifier, name, template)
        {

            SpawnData.MonsterDrops(this, SpawnData.drops.dragon);

        }

        public override void LoadOut()
        {

            baseMode = 2;

            baseJuice = 5;

            basePulp = 50;

            gait = 2;

            overHead = new(0, -224);

            walkInterval = 9;

            flightInterval = 9;

            flightSpeed = 12;

            sweepInterval = 6;

            cooldownInterval = 150;

            specialCeiling = 2;

            specialFloor = 0;

            specialInterval = 12;

            flightSet = true;

            smashSet = true;

            sweepSet = true;

            specialSet = true;

            channelSet = true;

            dragonRender = new();

            loadedOut = true;

        }

        protected override void initNetFields()
        {
            base.initNetFields();
            NetFields.AddField(netBreathActive,"netBreathActive");

        }

        public override Rectangle GetBoundingBox()
        {
            
            Vector2 position = Position;

            float netScale = GetScale();

            //Vector2 baseVector = new(Position.X + 32f - (64f* netScale), Position.Y + 64f - (64f * netScale));
            Vector2 baseVector = new(Position.X + 32f - (56f * netScale), Position.Y + 64f - (48f * netScale));

            Rectangle baseRectangle = new((int)baseVector.X,(int)baseVector.Y, (int)(48f * netScale), (int)(48f * netScale));

            //baseRectangle.X += (int)(8 * netScale);

            //baseRectangle.Width -= (int)(16 * netScale);

            //baseRectangle.Y += (int)(16 * netScale);

            //baseRectangle.Height -= (int)(16 * netScale);

            return baseRectangle;

        }

        public override void draw(SpriteBatch b, float alpha = 1f)
        {
            if (IsInvisible || !Utility.isOnScreen(Position, 128))
            {
                return;
            }

            Vector2 localPosition = getLocalPosition(Game1.viewport);

            float drawLayer = (float)StandingPixel.Y / 10000f;

            DrawEmote(b, localPosition, drawLayer);

            if (netFlightActive.Value || netSmashActive.Value)
            {

                return;

            }

            float netScale = GetScale();

            bool flippant = ((netDirection.Value % 2 == 0 && netAlternative.Value == 3) || netDirection.Value == 3);

            if (netSweepActive.Value)
            {

                if (netSpecialActive.Value)
                {

                    dragonRender.drawSweep(b, localPosition, new() { direction = netDirection.Value, scale = netScale, version = 1, frame = sweepFrame, flip = flippant, layer = drawLayer, });

                }
                else
                {

                    dragonRender.drawSweep(b, localPosition, new() { direction = netDirection.Value, scale = netScale, frame = sweepFrame, flip = flippant, layer = drawLayer, });

                }

                return;

            }

            if (netSpecialActive.Value || netChannelActive.Value)
            {

                dragonRender.drawWalk(b, localPosition, new() { direction = netDirection.Value, scale = netScale, version = 1, breath = netBreathActive.Value, frame = walkFrame, flip = flippant, layer = drawLayer, });

            }
            else
            {

                dragonRender.drawWalk(b, localPosition, new() { direction = netDirection.Value, scale = netScale, frame = netHaltActive.Value ? 0 : walkFrame, flip = flippant, layer = drawLayer, });

            }

        }


        public override void drawAboveAlwaysFrontLayer(SpriteBatch b)
        {
            
            Vector2 localPosition = Position - new Vector2((float)Game1.viewport.X, (float)Game1.viewport.Y);

            if (netFlightActive.Value || netSmashActive.Value)
            {

                float netScale = GetScale();

                bool flippant = ((netDirection.Value % 2 == 0 && netAlternative.Value == 3) || netDirection.Value == 3);

                float drawLayer = (float)StandingPixel.Y / 10000f;

                int useFrame = flightFrame;

                if(netFlightProgress.Value == 1)
                {

                    useFrame = flightFrame % 4;

                }

                if (netSpecialActive.Value)
                {

                    dragonRender.drawFlight(b, localPosition, new() { direction = netDirection.Value, version = 1, breath = netBreathActive.Value, flight = flightHeight, scale = netScale, frame = useFrame, flip = flippant, layer = drawLayer, });

                }
                else
                {

                    dragonRender.drawFlight(b, localPosition, new() { direction = netDirection.Value, flight = flightHeight, scale = netScale, frame = useFrame, flip = flippant, layer = drawLayer, });

                }

            }

            DrawTextAboveHead(b, localPosition);

        }

        public override float GetScale()
        {

            return 2f + (netMode.Value * 0.5f);

        }

        public override bool PerformSpecial(Vector2 farmerPosition)
        {

            specialTimer = 60;

            SetCooldown(1);

            netSpecialActive.Set(true);

            netBreathActive.Set(true);

            currentLocation.playSound("furnace");

            List<Vector2> zero = BlastZero();

            SpellHandle burn = new(currentLocation, zero[0] * 64, Position, (int)GetScale() * 32 + 96, GetThreat());

            burn.type = SpellHandle.spells.explode;

            burn.scheme = IconData.schemes.ember;

            burn.display = IconData.impacts.combustion;

            burn.instant = true;

            burn.added = new() { SpellHandle.effects.embers, };

            Mod.instance.spellRegister.Add(burn);

            return true;

        }


        public override bool PerformChannel(Vector2 target)
        {

            return PerformFlight(target);

            /*specialTimer = (specialCeiling + 1) * specialInterval * 2;

            netChannelActive.Set(true);

            netBreathActive.Set(false);

            SetCooldown(2f);

            int offset = Mod.instance.randomIndex.Next(2);

            float radius = GetScale();

            for (int k = 0; k < 5; k++)
            {

                Vector2 impact = target;
                
                if(k < 4)
                {

                    List<Vector2> castSelection = ModUtility.GetTilesWithinRadius(currentLocation, ModUtility.PositionToTile(target), 5, true, (k * 2) + offset);

                    if (castSelection.Count == 0)
                    {

                        continue;

                    }

                    impact = castSelection[Mod.instance.randomIndex.Next(castSelection.Count)] * 64;

                }

                SpellHandle missile = new(currentLocation, impact, Position, (int)radius * 32 + 96, GetThreat() * 0.75f);

                missile.type = SpellHandle.spells.ballistic;

                missile.display = IconData.impacts.impact;

                missile.missile = IconData.missiles.fireball;

                missile.projectileSpeed = 1f;

                missile.projectile = (int)radius;

                missile.added = new() { SpellHandle.effects.burn, };

                missile.boss = this;

                Mod.instance.spellRegister.Add(missile);

            }

            return true;*/

        }

        public override void ClearSpecial()
        {

            base.ClearSpecial();

            netBreathActive.Set(false);

        }

        public override bool ValidPush()
        {

            return false;

        }

        public override int WalkCount()
        {

            return 6;

        }

        public override int IdleCount()
        {

            return 1;

        }

        public override int SweepCount()
        {

            return 6;

        }

        public override int FlightCount(int segment = 0)
        {

            switch (segment)
            {
                
                default:

                case 0: 
                    
                    return 1;

                case 1: 
                    
                    return 4;

                case 2: 
                    
                    return 1;

                case 3: 
                    
                    return 6;

            }

        }

        public override int SmashCount(int segment = 0)
        {

            return FlightCount(segment);

        }

    }

}