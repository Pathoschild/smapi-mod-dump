/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewDruid.Cast;
using StardewDruid.Cast.Mists;
using StardewDruid.Data;
using StardewDruid.Event;
using StardewDruid.Monster;
using StardewDruid.Render;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Buffs;
using StardewValley.Buildings;
using StardewValley.Monsters;
using StardewValley.Network;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace StardewDruid.Character
{
    public class Dragon : NPC
    {

        public NetLong netAnchor = new NetLong(0);
        public Farmer anchor;
        public bool avatar;

        public DragonRender dragonRender;

        public int moveDirection;
        public int altDirection;
        public NetInt netDirection = new NetInt(0);
        public NetInt netAlternative = new NetInt(0);
        public bool loadedOut;
        public Vector2 setPosition;

        public float walkTimer;
        public int walkFrame;
        public int stationaryTimer;

        public bool endTransform;
        public SButton leftButton;
        public SButton rightButton;

        public NetBool netDashActive = new NetBool(false);

        public int flightFrame;
        public int flightHeight;

        public bool flightActive;
        public int flightDelay;
        public int flightTimer;
        public Vector2 flightPosition;
        public Vector2 flightTo;
        public Vector2 flightInterval;
        public bool flightFlip;
        public int flightIncrement;
        public int strikeTimer;
        public int flightExtend;
        public string flightTerrain;

        public List<Rectangle> sweepFrames;
        public bool sweepActive;
        public NetBool netSweepActive = new NetBool(false);
        public int sweepFrame;
        public int sweepDelay;
        public int sweepTimer;
        public bool cooldownActive;
        public int cooldownTimer;

        public NetBool netSpecialActive = new NetBool(false);
        public bool specialActive;
        public int specialDelay;
        public int specialTimer;
        public int fireTimer;
        public int roarTimer;
        public bool roarActive;

        public NetBool netBreathActive = new NetBool(false);

        public NetBool netDigActive = new NetBool(false);
        public bool digActive;
        public int digTimer;
        public Vector2 digPosition;
        public int digMoment;

        public NetBool netDiveActive = new NetBool(false);
        public NetBool netSwimActive = new NetBool(false);
        public List<float> diveRotates;
        public bool terrainActive;
        public bool swimActive;
        public int swimCheckTimer;
        public bool diveActive;
        public int diveTimer;
        public Vector2 divePosition;
        public int diveMoment;


        public Dragon()
        {
        }

        public Dragon(Farmer Farmer, Vector2 position, string map, string Name)
          : base(new AnimatedSprite(Path.Combine("Characters", "Abigail")), position, map, 2, Name, CharacterHandle.CharacterPortrait(CharacterHandle.characters.none), false)
        {
            
            willDestroyObjectsUnderfoot = false;
            
            DefaultMap = map;
            
            DefaultPosition = position;
            
            HideShadow = true;
            
            breather.Value = false;

            avatar = true;
            
            anchor = Farmer;
           
            netAnchor.Set(Farmer.UniqueMultiplayerID);
            
            moveDirection = Game1.player.FacingDirection;

            netDirection.Set(moveDirection);

            FacingDirection = moveDirection;

            //if (Mod.instance.CurrentProgress >= 31)
            //{

                terrainActive = true;

            //}

            if (ModUtility.GroundCheck(currentLocation, new Vector2((int)(Position.X / 64), (int)(Position.Y / 64))) == "water")
            {

                swimActive = true;

                netSwimActive.Set(true);

            }

            AnimateMovement(false);
            
            LoadOut();
        
        }

        public void LoadOut()
        {

            anchor = Game1.getFarmer(netAnchor.Value);

            dragonRender = new();

            loadedOut = true;

            flightTo = Vector2.Zero;

            flightInterval = Vector2.Zero;

        }

        protected override void initNetFields()
        {
            base.initNetFields();

            NetFields.AddField(netAnchor, "netAnchor");
            NetFields.AddField(netDirection, "netDirection");
            NetFields.AddField(netAlternative, "netAlternative");
            NetFields.AddField(netSweepActive, "netSweepActive");
            NetFields.AddField(netDashActive, "netDashActive");
            NetFields.AddField(netSpecialActive, "netSpecialActive");
            NetFields.AddField(netBreathActive, "netBreathActive");
            NetFields.AddField(netDigActive, "netDigActive");
            NetFields.AddField(netDiveActive, "netDiveActive");
            NetFields.AddField(netSwimActive, "netSwimActive");

        }

        public override void draw(SpriteBatch b, float alpha = 1f)
        {

            if (anchor == null)
            {

                return;

            }

            if (IsInvisible)
            {

                return;
            
            }

            if (!Utility.isOnScreen(Position, 128))
            {

                return;
            
            }
                
            if(avatar && Game1.displayFarmer)
            {

                return;

            }

            if (netDashActive.Value)
            {

                return;

            }

            Vector2 localPosition = getLocalPosition(Game1.viewport);

            bool flippant = ((netDirection.Value % 2 == 0 && netAlternative.Value == 3) || netDirection.Value == 3);

            float drawLayer = anchor.getDrawLayer() + 0.0001f;

            if (netDiveActive.Value)
            {

                dragonRender.drawDive(b, localPosition, new() { direction = netDirection.Value, scale = 3f, frame = diveMoment, flip = flippant, layer = drawLayer, });

                return;
            }

            if(netSwimActive.Value)
            {

                dragonRender.drawSwim(b, localPosition, new() { direction = netDirection.Value, scale = 3f, frame = walkFrame, flip = flippant, layer = drawLayer, });

                return;

            }

            if (netDigActive.Value)
            {

                int digFrame = digMoment % 2;

                dragonRender.drawDig(b, localPosition, new() { direction = netDirection.Value, scale = 3f, frame = digFrame, flip = flippant, layer = drawLayer, });

                return;

            }

            if (netSweepActive.Value)
            {

                if (netSpecialActive.Value)
                {

                    dragonRender.drawSweep(b, localPosition, new() { direction = netDirection.Value, scale = 3f, version = 1, frame = sweepFrame, flip = flippant, layer = drawLayer, });

                }
                else
                {

                    dragonRender.drawSweep(b, localPosition, new() { direction = netDirection.Value, scale = 3f, frame = sweepFrame, flip = flippant, layer = drawLayer, });

                }

                return;

            }

            if (netSpecialActive.Value)
            {

                dragonRender.drawWalk(b, localPosition, new() { direction = netDirection.Value, scale = 3f, version = 1, breath = netBreathActive.Value, frame = walkFrame, flip = flippant, layer = drawLayer, });

            }
            else
            {

                dragonRender.drawWalk(b, localPosition, new() { direction = netDirection.Value, scale = 3f, frame = walkFrame, flip = flippant, layer = drawLayer, });

            }

        }

        public override void drawAboveAlwaysFrontLayer(SpriteBatch b)
        {

            if (anchor == null)
            {

                return;

            }

            if (netDashActive.Value)
            {
                
                Vector2 localPosition = Position - new Vector2((float)Game1.viewport.X,(float)Game1.viewport.Y);

                bool flippant = ((netDirection.Value % 2 == 0 && netAlternative.Value == 3) || netDirection.Value == 3);

                float drawLayer = anchor.getDrawLayer() + 0.0001f;

                if (netSpecialActive.Value)
                {

                    dragonRender.drawFlight(b, localPosition, new() { direction = netDirection.Value, version = 1, breath = netBreathActive.Value, flight = flightHeight, scale = 3f, frame = flightFrame, flip = flippant, layer = drawLayer, });

                }
                else
                {

                    dragonRender.drawFlight(b, localPosition, new() { direction = netDirection.Value, flight = flightHeight, scale = 3f, frame = flightFrame, flip = flippant, layer = drawLayer, });

                }

            }

        }


        public override void reloadSprite(bool onlyAppearance = false)
        {
            
            base.reloadSprite(onlyAppearance);

            Portrait = CharacterHandle.CharacterPortrait(CharacterHandle.characters.none);

        }

        public override bool checkAction(Farmer who, GameLocation l) => false;

        public override void collisionWithFarmerBehavior()
        {
        }

        public override void behaviorOnFarmerPushing()
        {
        }

        public override Rectangle GetBoundingBox() => new Rectangle(-1, -1, 0, 0);

        public override void update(GameTime time, GameLocation location)
        {

            if (!loadedOut)
            {
                
                LoadOut();
            
            }

            if (anchor == null)
            {

                ShutDown();

                return;

            }

            if (!avatar)
            {

                UpdateMultiplayer();

                return;

            }

            if (Game1.isWarping)
            {

                return;

            }

            if (currentLocation != Game1.player.currentLocation)
            {


                return;

            }

            if (Mod.instance.CasterBusy())
            {

                specialActive = false;

                specialTimer = 0;

                netSpecialActive.Set(false);

                netBreathActive.Set(false);

                return;

            }

            if (anchor.FarmerSprite.PauseForSingleAnimation)
            {

                return;

            }

            if (shakeTimer > 0)
            {
                shakeTimer = 0;
            }

            if (textAboveHeadTimer > 0)
            {
                
                if (textAboveHeadPreTimer > 0)
                {
                    
                    textAboveHeadPreTimer -= time.ElapsedGameTime.Milliseconds;
                
                }
                else
                {
                    
                    textAboveHeadTimer -= time.ElapsedGameTime.Milliseconds;

                    if (textAboveHeadTimer > 500)
                    {
                        
                        textAboveHeadAlpha = Math.Min(1f, textAboveHeadAlpha + 0.1f);
                    
                    }
                    else
                    {

                        float newAlpha = textAboveHeadAlpha - 0.04f;

                        textAboveHeadAlpha = newAlpha < 0f ? 0f : newAlpha;
                    
                    }

                }

            }

            int stepAnimation = Game1.player.FarmerSprite.currentAnimationIndex % 4;

            if (stepAnimation == 0)
            {

                Game1.player.FarmerSprite.currentAnimationIndex++;

            }
            else if (stepAnimation == 3)
            {

                Game1.player.FarmerSprite.currentAnimationIndex -= 2;

            }

            updateEmote(time);

            DefenseBuff();

            moveDirection = Game1.player.FacingDirection;

            if (flightActive)
            {
                UpdateFlight();
            }

            if (sweepActive)
            {
                UpdateSweep();
            }

            if (swimActive)
            {
                UpdateSwim();

            }

            if (diveActive)
            {
                UpdateDive();
            }

            if (digActive)
            {

                UpdateDig();

            }

            if(!flightActive)
            {
                UpdateFollow();
            }

            if (specialActive)
            {
                UpdateSpecial();
            }

            if (cooldownActive)
            {
                UpdateCooldown();
            }

        }

        public void UpdateMultiplayer()
        {

            if (netDashActive.Value)
            {

                flightTimer--;

                if(flightTimer <= 0)
                {

                    flightFrame++;

                    if (flightFrame > 4)
                    {
                        flightFrame = 1;

                    }

                    flightTimer = 12;
                
                }

                return;

            }
            else
            {
                flightFrame = 0;

                flightTimer = 12;

            }

            if (netSweepActive.Value)
            {

                sweepTimer--;

                if (sweepTimer <= 0)
                {

                    sweepFrame++;

                    if (sweepFrame > 4)
                    {
                        sweepFrame = 0;
                    }

                    sweepTimer = 9;

                }

                return;

            }
            else
            {
                sweepFrame = 0;

                sweepTimer = 9;

            }

            if (setPosition != Position || netDirection.Value != moveDirection || netAlternative.Value != altDirection)
            {

                if(walkFrame == 0)
                {

                    walkFrame = 1;

                    walkTimer = 9;

                    stationaryTimer = 30;

                }

                walkTimer--;

                if (walkTimer <= 0)
                {

                    moveDirection = netDirection.Value;

                    altDirection = netAlternative.Value;

                    setPosition = Position;

                    walkFrame++;

                    if (walkFrame > 6)
                    {
                    
                        walkFrame = 1;
                    
                    }

                    walkTimer = 9;

                    stationaryTimer = 30;

                }

                return;

            }

            if (stationaryTimer > 0)
            {

                stationaryTimer--;

                if (stationaryTimer == 0)
                {

                    walkFrame = 0;

                    walkTimer = 0;

                }

            }

        }

        public void UpdateFollow()
        {

            if (Position != Game1.player.Position)
            {
                if (moveDirection % 2 == 1)
                {
                    netAlternative.Set(Game1.player.Position.X > (double)Position.X ? 1 : 3);
                }

                netDirection.Set(moveDirection);

                FacingDirection = moveDirection;

                Position = Game1.player.Position;

                AnimateMovement();

                return;

            }

            if (netDirection.Value != moveDirection)
            {

                netDirection.Set(moveDirection);

                FacingDirection = moveDirection;

                netAlternative.Set(netDirection.Value);

                AnimateMovement();

                return;

            }

            if(stationaryTimer > 0)
            {

                stationaryTimer--;

                if(stationaryTimer == 0)
                {

                    walkFrame = 0;

                    walkTimer = 0;

                }

            }

        }

        public void AnimateMovement(bool movement = true)
        {

            if (movement)
            {

                walkTimer--;

                if (walkTimer <= 0)
                {

                    walkFrame++;

                    if (walkFrame > 6)
                    {
                        walkFrame = 1;
                    }

                    walkTimer = 9;

                    stationaryTimer = 5;

                    if (!avatar)
                    {
                        stationaryTimer = 30;

                    }

                }

                return;

            }

            walkFrame = 0;

            walkTimer = 0;

            stationaryTimer = 0;

        }

        public void LeftClickAction(SButton Button)
        {

            leftButton = Button;

            if (netDiveActive.Value || netSweepActive.Value || netDashActive.Value || netDigActive.Value)
            {

                return;

            }

            PerformFlight();

        }

        public void RightClickAction(SButton Button)
        {

            rightButton = Button;

            if (netSwimActive.Value && !netDashActive.Value)
            {

                if (!diveActive && !Mod.instance.eventRegister.ContainsKey("active"))
                {

                    PerformDive();

                }

                return;

            }

            if (!netSpecialActive.Value)
            {

                PerformSpecial();

            }

        }

        public void DefenseBuff()
        {

            BuffEffects buffEffect = new();

            buffEffect.Defense.Set(5);

            Buff speedBuff = new("184655", source: "Rite of the Ether", displaySource: "Rite of the Ether", duration: 3000, displayName: "Dragon Scales", description: "Defense increased by transformation", effects: buffEffect);

            anchor.buffs.Apply(speedBuff);

        }

        public void PerformFlight()
        {

            if (SweepVictims().Count > 0)
            {

                if (AccountStamina(12))
                {
                    PerformSweep();

                    return;

                }

            }

            int flightRange = FlightDestination();

            if (flightRange == 0)
            {
                return;
            }


            if (!Mod.instance.questHandle.IsComplete(Journal.QuestHandle.etherOne))
            {

                Mod.instance.questHandle.UpdateTask(Journal.QuestHandle.etherOne, 1);

            }

            flightActive = true;

            flightDelay = 3;

            flightFrame = 0;

            flightTimer = flightIncrement * flightRange;

            flightInterval = new((flightTo.X - Position.X) / flightTimer, (flightTo.Y - Position.Y) / flightTimer);

            flightPosition = Game1.player.Position;

            strikeTimer = 56;

            flightExtend = flightIncrement * 6;

            Game1.player.temporarilyInvincible = true;

            Game1.player.temporaryInvincibilityTimer = 0;

            Game1.player.currentTemporaryInvincibilityDuration = 100 * flightIncrement;

        }

        public void PerformSweep()
        {

            sweepActive = true;

            sweepDelay = 3;

            sweepFrame = (0);

            sweepTimer = 25;

        }

        public bool UpdateFlight()
        {
            if (flightDelay > 0)
            {
                flightDelay--;

                return true;

            }

            netDashActive.Set(true);

            flightTimer--;

            if (strikeTimer > 0)
            {

                if (strikeTimer % (flightIncrement * 3) == 0)
                {

                    FlightStrike();

                }

                strikeTimer--;

            }

            if (flightTimer == 0)
            {

                flightActive = false;

                netDashActive.Set(false);

                if (flightTerrain == "water")
                {

                    swimActive = true;

                    netSwimActive.Set(true);

                    swimCheckTimer = 60;

                }

                if (strikeTimer == 0)
                {

                    FlightStrike();

                }

                return false;

            }

            if (flightHeight < 128 && flightTimer > 16)
            {

                flightHeight = (flightHeight + 8);

            }
            else if (flightHeight > 0 && flightTimer <= 16)
            {

                flightHeight = (flightHeight - 8);

            }

            flightPosition += flightInterval;

            Game1.player.Position = flightPosition;

            Position = flightPosition;

            if (flightTimer % flightIncrement == 0)
            {

                if (flightFrame == 4)
                {

                    flightFrame=(1);

                }
                else
                {
                    
                    flightFrame=(flightFrame + 1);

                }
                    
                if (Mod.instance.Helper.Input.IsDown(leftButton))
                {
                    
                    int num = FlightDestination();
                    
                    if (num != 0)
                    {
                        
                        flightTimer = flightIncrement * num;
                        
                        flightInterval = new((flightTo.X - Position.X) / flightTimer, (flightTo.Y - Position.Y) / flightTimer);

                    }
                
                }
                
                if (flightTimer == flightIncrement)
                {
                    
                    flightFrame=(0);

                    Game1.player.temporarilyInvincible = true;

                    Game1.player.temporaryInvincibilityTimer = 0;

                    Game1.player.currentTemporaryInvincibilityDuration = 500;
                
                }
            
            }

            return true;

        }

        public void FlightStrike()
        {


            if (!Mod.instance.questHandle.IsComplete(Journal.QuestHandle.etherOne))
            {

                return;

            }

            Rectangle hitBox = new((int)Position.X - 96, (int)Position.Y - 64, 256, 128);

            SpellHandle sweep = new(Game1.player, ModUtility.MonsterIntersect(currentLocation, hitBox, true), Mod.instance.CombatDamage() * 3 / 2);

            sweep.instant = true;

            Mod.instance.spellRegister.Add(sweep);

        }

        public bool UpdateSweep()
        {

            if (sweepDelay > 0)
            {
                sweepDelay--;

                return true;

            }

            if(sweepTimer == 25)
            {

                if (Game1.player.CurrentTool is MeleeWeapon)
                {
                    (Game1.player.CurrentTool as MeleeWeapon).isOnSpecial = true;
                }

                netSweepActive.Set(true);

                flightHeight =(0);

            }

            sweepTimer--;

            if (sweepTimer == 15)
            {

                SweepStrike();

            }

            if (sweepTimer > 12)
            {

                flightHeight=(flightHeight + 2);

            }
            else
            {

                flightHeight=(flightHeight - 2);

            }

            if (sweepTimer <= 0)
            {

                if (Game1.player.CurrentTool is MeleeWeapon)
                {
                    (Game1.player.CurrentTool as MeleeWeapon).isOnSpecial = false;
                }

                sweepActive = false;

                netSweepActive.Set(false);

                flightHeight =(0);

                return false;

            }

            if(sweepTimer % 5 == 0)
            {

                sweepFrame++;

            }

            return true;

        }

        public void UpdateCooldown()
        {

            cooldownTimer--;

            if (cooldownTimer <= 0)
            {
                cooldownActive = false;

            }

        }

        public List<StardewValley.Monsters.Monster> SweepVictims()
        {
            
            Rectangle hitBox = new((int)Position.X - 96, (int)Position.Y - 64, 256, 192);

            return ModUtility.MonsterIntersect(currentLocation, hitBox, true);
        
        }

        public void SweepStrike()
        {
            
            SpellHandle sweep = new(Game1.player, SweepVictims(), Mod.instance.CombatDamage() * 3 / 2);

            sweep.instant = true;

            Mod.instance.spellRegister.Add(sweep);

        }

        public int FlightDestination()
        {

            Dictionary<int, Vector2> flightVectors = new Dictionary<int, Vector2>()
            {
                [0] = new Vector2(1f, -2f),
                [1] = new Vector2(-1f, -2f),
                [2] = new Vector2(2f, 0.0f),
                [3] = new Vector2(1f, 2f),
                [4] = new Vector2(-1f, 2f),
                [5] = new Vector2(-2f, 0.0f),
                [6] = new Vector2(0f, -2f),
                [7] = new Vector2(0f, 2f),
            };

            int key = 0;

            int increment = 8;

            int alternate = netAlternative.Value;

            if (netDirection.Value != moveDirection)
            {
                alternate = netDirection.Value;

            }

            bool cardinal = Mod.instance.Config.cardinalMovement;

            switch (moveDirection)
            {
                case 0:
                    if (alternate == 3)
                    {
                        key = 1;
                    }
                    if (cardinal)
                    {
                        key = 6;
                    }
                    increment = 12;
                    break;
                case 1:
                    key = 2;
                    break;
                case 2:
                    key = 3;
                    if (alternate == 3)
                    {
                        key = 4;
                    }
                    if (cardinal)
                    {
                        key = 7;
                    }
                    increment = 12;
                    break;
                case 3:
                    key = 5;
                    break;
            }

            Vector2 flightOffset = flightVectors[key];

            int flightRange = 16;

            Vector2 tile = new Vector2((int)(Position.X / 64), (int)(Position.Y / 64));

            for (int index = flightRange; index > 0; index--)
            {

                int checkRange = 17 - index;

                if (index > 12)
                {

                    checkRange = index - 12;

                }

                Vector2 neighbour = tile + (flightOffset * checkRange);

                List<string> safeTerrain = new() { "ground", };

                if (terrainActive)
                {
                    safeTerrain.Add("water");
                }

                string groundCheck = ModUtility.GroundCheck(currentLocation, neighbour);

                if (!safeTerrain.Contains(groundCheck))
                {

                    continue;

                }

                if (groundCheck == "water")
                {

                    if (!ModUtility.WaterCheck(currentLocation, neighbour, 2))
                    {

                        continue;

                    }

                }

                Vector2 neighbourPosition = neighbour * 64;

                Rectangle boundingBox = Game1.player.GetBoundingBox();

                int xOffset = boundingBox.X - (int)Game1.player.Position.X;

                int yoffset = boundingBox.Y - (int)Game1.player.Position.Y;

                boundingBox.X = (int)neighbourPosition.X + xOffset;

                boundingBox.Y = (int)neighbourPosition.Y + yoffset;

                if (!currentLocation.isCollidingPosition(boundingBox, Game1.viewport, false, 0, false, Game1.player, false, false, false))
                {

                    flightTo = neighbourPosition;

                    flightIncrement = increment;

                    flightTerrain = groundCheck;

                    netAlternative.Set(alternate);

                    netDirection.Set(moveDirection);

                    FacingDirection = moveDirection;

                    AnimateMovement(false);

                    return checkRange;

                }


            }

            return 0;

        }

        public bool TreasureZone(bool activate = false)
        {

            if (!swimActive && currentLocation.objects.Count() > 0)
            {

                Vector2 tile = new Vector2((int)(Position.X / 64), (int)(Position.Y / 64));

                List<Vector2> tileVectors = ModUtility.GetTilesWithinRadius(currentLocation, tile, 1);

                tileVectors.Add(tile);

                foreach (Vector2 tileVector in tileVectors)
                {

                    if (currentLocation.objects.ContainsKey(tileVector))
                    {

                        StardewValley.Object targetObject = currentLocation.objects[tileVector];

                        if (targetObject.Name.Contains("Artifact Spot"))
                        {

                            if (activate)
                            {

                                currentLocation.digUpArtifactSpot((int)tileVector.X, (int)tileVector.Y, anchor);
                                
                                currentLocation.objects.Remove(tileVector);
                            
                            }

                            digPosition = tileVector * 64f;

                            return true;

                        }

                    }

                    continue;

                }

            }

            if (Mod.instance.eventRegister.ContainsKey("crate"))
            {

                if(Mod.instance.eventRegister["crate"] is Cast.Ether.Crate treasureEvent)
                {

                    if (!treasureEvent.eventActive)
                    {

                        if (Vector2.Distance(Position, treasureEvent.origin) <= 128f)
                        {

                            if (activate)
                            {

                                treasureEvent.EventActivate();

                            }

                            digPosition = treasureEvent.origin;

                            return true;

                        }

                    }

                };

            }

            return false;

        }

        public void PerformSpecial()
        {

            if (!flightActive)
            {
                
                if (TreasureZone())
                {

                    digActive = true;

                    specialDelay = 6;

                    digTimer = -1;

                    return;

                }

                FaceCursorTarget();

            }

            if (!Mod.instance.questHandle.IsComplete(Journal.QuestHandle.etherTwo))
            {

                Mod.instance.questHandle.UpdateTask(Journal.QuestHandle.etherTwo, 1);

            }

            specialActive = true;

            specialDelay = 6;

            specialTimer = -1;

            netSpecialActive.Set(true);

        }
        
        public void UpdateDig()
        {
            if (specialDelay > 0)
            {

                specialDelay--;

                return;

            }

            if(digTimer == -1)
            {

                netDigActive.Set(true);

                digTimer = 108;

                digMoment = 0;

            }

            digTimer--;

            if (digTimer == 0)
            {

                digActive = false;
                
                netDigActive.Set(false);
                
                return;

            }

            Game1.player.Position = digPosition;
            
            Position = digPosition;

            if (digTimer % 12 == 0)
            {

                digMoment++;

            }

            if (digTimer == 24)
            {

                TreasureZone(true);

            }

        }

        public void FaceCursorTarget()
        {
            Point mousePoint = Game1.getMousePosition();

            if (mousePoint.Equals(new(0)))
            {
                return;

            }

            Vector2 viewPortPosition = Game1.viewportPositionLerp;

            Vector2 mousePosition = new(mousePoint.X + viewPortPosition.X, mousePoint.Y + viewPortPosition.Y);

            Vector2 diffPosition = mousePosition - Position;

            float rotate = (float)Math.Atan2(diffPosition.Y, diffPosition.X);

            if(rotate < 0.0001f)
            {

                rotate = (float)(Math.PI * 2) + rotate;

            }

            if(rotate < 0.525 || rotate > 5.75)
            {

                moveDirection = 1;
                altDirection = 0;

            }
            else if(rotate < 1.575)
            {

                moveDirection = 2;
                altDirection = 1;

            }
            else if (rotate < 2.625)
            {

                moveDirection = 2;
                altDirection = 3;

            }
            else if (rotate < 3.675)
            {

                moveDirection = 3;
                altDirection = 0;

            }
            else if (rotate < 4.725)
            {

                moveDirection = 0;
                altDirection = 3;

            }
            else if(rotate < 5.775)
            {

                moveDirection = 0;
                altDirection = 1;

            }

            Game1.player.FacingDirection = moveDirection;

            netDirection.Set(moveDirection);

            FacingDirection = moveDirection;

            netAlternative.Set(altDirection);

            AnimateMovement(false);

        }

        public bool RoarCheck(Vector2 zero)
        {

            Vector2 target = zero * 64;

            roarActive = false;

            if (currentLocation.characters.Count > 0)
            {

                foreach (NPC witness in currentLocation.characters)
                {

                    if (witness is StardewValley.Monsters.Monster)
                    {
                        continue;
                    }

                    if (witness is StardewDruid.Character.Character)
                    {
                        continue;
                    }

                    if (witness is StardewDruid.Character.Dragon)
                    {
                        continue;
                    }

                    if (witness.IsInvisible)
                    {

                        continue;
                    
                    }

                    if (Vector2.Distance(witness.Position, target) < 320)
                    {

                        if (roarTimer <= 0)
                        {

                            showTextAboveHead("RWWWRRR", duration: 2000);

                            currentLocation.playSound("DragonRoar", null, 600);

                        }

                        roarTimer = 120;

                        roarActive = true;

                        netBreathActive.Set(false);

                        return true;

                    }

                }

            }

            return false;

        }

        public bool UpdateSpecial()
        {
            if (specialDelay > 0)
            {

                specialDelay--;

                return true;

            }

            if (specialTimer == -1)
            {

                List<Vector2> zeroes = BlastTarget();

                RoarCheck(zeroes[0]);

                specialTimer = 48;

            }

            specialTimer--;

            fireTimer--;

            roarTimer--;

            if (specialTimer == 0 || netSwimActive.Value)
            {

                specialActive = false;

                netSpecialActive.Set(false);

                netBreathActive.Set(false);

                return false;

            }

            if (Mod.instance.Helper.Input.IsDown(rightButton) && specialTimer == 12)
            {

                specialTimer = 18;

            }

            if (fireTimer <= 0)
            {

                List<Vector2> zeroes = BlastTarget();

                if (RoarCheck(zeroes[0]) || !AccountStamina(5))
                {

                    fireTimer = 24;

                    return true;

                };

                Vector2 minus = new((int)(Position.X / 64), (int)(Position.Y / 64));

                if (roarTimer <= 0)
                {
                    currentLocation.playSound("furnace", null, 600);

                    roarTimer = 120;
                }

                List<Vector2> splash = new();

                if (netSweepActive.Value)
                {

                    splash = new()
                    {
                        zeroes[1],
                        zeroes[1] + new Vector2(3,-4),
                        zeroes[1] + new Vector2(-3,-4),
                        zeroes[1] + new Vector2(5,1),
                        zeroes[1] + new Vector2(3,3),
                        zeroes[1] + new Vector2(-3,3),
                        zeroes[1] + new Vector2(-5,-1),

                    };

                }
                else
                {

                    splash.Add(zeroes[0]);

                }

                for (int i = 0; i < splash.Count; i++)
                {
                    
                    Vector2 burnVector = splash[i];

                    SpellHandle burn = new(Game1.player,burnVector * 64,192,Mod.instance.CombatDamage() / 3);

                    burn.type = SpellHandle.spells.explode;

                    burn.scheme = IconData.schemes.ember;

                    burn.display = IconData.impacts.combustion;

                    burn.instant = true;

                    burn.power = 4;

                    burn.terrain = 2;

                    burn.environment = 2;

                    if(Mod.instance.questHandle.IsComplete(Journal.QuestHandle.etherTwo)){


                        burn.added = new() { SpellHandle.effects.embers, };

                    }

                    Mod.instance.spellRegister.Add(burn);

                }

                netBreathActive.Set(true);

                fireTimer = 24;

            }

            return true;

        }

        public List<Vector2> BlastTarget()
        {

            Vector2 tile = new Vector2((int)(Position.X / 64), (int)(Position.Y / 64));

            Vector2 zero = tile;

            Vector2 start = tile;

            Vector2 weak = tile;

            List<Vector2> zeroes = new();

            bool cardinal = Mod.instance.Config.cardinalMovement;

            switch (netDirection.Value)
            {
                case 0:


                    if (cardinal)
                    {
                        start.Y -= 1;
                        zero.Y -= 5;
                        weak.Y -= 2;
                        break;

                    }

                    zero.X += 3;

                    zero.Y -= 4;

                    weak.X += 1;

                    weak.Y -= 2;

                    if (netAlternative.Value == 3 || flip)
                    {
                        zero.X -= 6;

                        weak.X -= 2;

                        break;

                    }

                    start.Y -= 1;

                    break;

                case 1:

                    zero.X += 5;

                    start.X += 1;

                    weak.X += 2;

                    break;

                case 2:

                    if (cardinal)
                    {

                        zero.Y += 4;

                        weak.Y += 1;

                        break;

                    }

                    zero.X += 3;

                    zero.Y += 3;

                    weak.X += 1;

                    weak.Y += 1;

                    if (netAlternative.Value == 3 || flip)
                    {
                        zero.X -= 6;
                     
                        weak.X -= 2;
                        
                        break;

                    }
                    //start.Y += 1;

                    break;

                default:

                    zero.X -= 5;

                    start.X -= 1;

                    weak.X -= 2;

                    break;

            }

            zeroes.Add(zero);

            zeroes.Add(start);

            zeroes.Add(weak);

            return zeroes;

        }

        public void UpdateSwim()
        {

            if (netDashActive.Value)
            {

                swimActive = false;

                netSwimActive.Set(false);

                return;

            }

            if (ModUtility.GroundCheck(currentLocation, new Vector2((int)(Position.X / 64), (int)(Position.Y / 64))) != "water")
            {

                swimActive = false;

                netSwimActive.Set(false);

                return;

            }

        }

        public void PerformDive()
        {

            diveActive = true;

            netDiveActive.Set(true);

            diveTimer = 240;

            divePosition = Game1.player.Position;

            diveMoment = 0;

            currentLocation.playSound("pullItemFromWater");

            if (!Mod.instance.questHandle.IsComplete(Journal.QuestHandle.etherThree))
            {

                Mod.instance.questHandle.UpdateTask(Journal.QuestHandle.etherThree, 1);

            }

            Mod.instance.iconData.ImpactIndicator(currentLocation, Position - new Vector2(0, 32), IconData.impacts.splash, 3f, new() { interval = 100f, alpha = 0.5f, layer = 999f,});

        }

        public bool UpdateDive()
        {
            diveTimer--;

            if (diveTimer <= 0)
            {

                diveActive = false;
                netDiveActive.Set(false);
                return false;

            }

            Game1.player.Position = divePosition;
            Position = divePosition;

            if (diveTimer % 40 == 0)
            {

                diveMoment++;

            }

            if (diveTimer == 120)
            {

                currentLocation.playSound("quickSlosh");


                Mod.instance.iconData.ImpactIndicator(currentLocation, Position - new Vector2(0, 32), IconData.impacts.splash, 3f, new() { interval = 100f, alpha = 0.5f, layer = 999f, });
                
                Mod.instance.iconData.ImpactIndicator(currentLocation, Position + new Vector2(64, -32), IconData.impacts.fish, 2f, new() { interval = 100f, alpha = 0.5f, layer = 999f, });

                if (TreasureZone(true))
                {

                    return true;

                }

                Vector2 treasurePosition = Position + new Vector2(64, 0);

                StardewValley.Object treasureItem = SpawnData.RandomTreasure(currentLocation, Mod.instance.questHandle.IsComplete(Journal.QuestHandle.etherThree));

                ThrowHandle treasure = new(Game1.player, treasurePosition, treasureItem);

                if (treasure.item.Category == -4)
                {

                    //treasure.setQuality(2);

                    //Game1.player.checkForQuestComplete(null, treasureIndex, 1, treasure.objectInstance, "fish", 7);

                    Game1.player.checkForQuestComplete(null, treasureItem.ParentSheetIndex, 1, null, null, 7);

                    Game1.player.gainExperience(1, 8); // gain fishing experience

                }

                treasure.fade = 0.0005f;

                treasure.height = 192;

                treasure.register();

                Mod.instance.CastMessage("Dove for a " + treasure.item.Name);

            }

            return true;

        }

        public void ShutDown()
        {

            if (flightActive && currentLocation.Name == Game1.player.currentLocation.Name)
            {

                Game1.player.Position = flightTo;

            }

            DelayedAction.functionAfterDelay(RemoveInstance,1);

        }

        public void RemoveInstance()
        {

            currentLocation.characters.Remove(this);

        }

        public bool SafeExit()
        {

            if (swimActive || flightTerrain == "water")
            {

                return false;

            }

            return true;

        }

        public bool AccountStamina(int cost)
        {

            if (Game1.player.Stamina <= 32 || Game1.player.health <= 25)
            {

                Mod.instance.AutoConsume();

            }

            if (Game1.player.Stamina <= cost)
            {

                Mod.instance.CastMessage("Not enough energy to perform skill", 3);

                return false;

            }

            float oldStamina = Game1.player.Stamina;

            float staminaCost = Math.Min(cost, oldStamina - 1);

            Game1.player.Stamina -= staminaCost;

            Game1.player.checkForExhaustion(oldStamina);

            return true;

        }

    }

}
