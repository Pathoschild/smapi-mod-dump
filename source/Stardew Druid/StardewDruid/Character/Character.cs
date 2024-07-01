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
using StardewDruid.Data;
using StardewDruid.Dialogue;
using StardewDruid.Journal;
using StardewDruid.Location;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.Characters;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Monsters;
using StardewValley.Network;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Timers;
using static StardewDruid.Cast.SpellHandle;
using static StardewValley.Minigames.TargetGame;

namespace StardewDruid.Character
{
    public class Character : NPC
    {

        public Texture2D characterTexture;
        public CharacterHandle.characters characterType = CharacterHandle.characters.none;
        //public List<Vector2> targetVectors = new();

        public int overhead;

        public Vector2 occupied;
        public Vector2 destination;
        public Dictionary<Vector2,int> traversal = new();
        public Vector2 tether;

        public float gait;

        public NetInt netDirection = new NetInt(0);
        public NetInt netAlternative = new NetInt(0);
        public NetBool netWorkActive = new NetBool(false);

        public List<Vector2> roamVectors = new();
        public int roamIndex;
        public double roamLapse;

        public NetBool netSceneActive = new NetBool(false);
        public Dictionary<int,Vector2> eventVectors = new();
        public List<Vector2> closedVectors = new();
        public int eventIndex;
        public string eventName;
        public bool loadedOut;

        public enum mode
        {
            home,
            scene,
            track,
            roam,
            random,
        }

        public mode modeActive;

        public enum pathing
        {
            none,
            monster,
            player,
            roam,
            scene,
            random,
            running,

        }

        public pathing pathActive;

        public Dictionary<int, List<Rectangle>> walkFrames = new();
        public Dictionary<int, List<Rectangle>> dashFrames = new();
        public Dictionary<int, List<Rectangle>> smashFrames = new();
        public Dictionary<int, List<Rectangle>> idleFrames = new();
        public Dictionary<int, List<Rectangle>> haltFrames = new();
        public Dictionary<int, List<Rectangle>> sweepFrames = new();
        public Dictionary<int, List<Rectangle>> specialFrames = new();
        public Dictionary<int, List<Rectangle>> workFrames = new();
        public Dictionary<int, List<Rectangle>> alertFrames = new();

        public NetBool netHaltActive = new NetBool(false);
        public NetBool netStandbyActive = new NetBool(false);
        public int idleTimer;
        public int stationaryTimer;
        public bool onAlert;

        public int collidePriority;
        public int collideTimer;
        public int moveTimer;
        public int moveInterval;
        public int moveFrame;
        public bool moveRetreat;
        public bool walkSide;
        public int lookTimer;
        public int followTimer;
        public int attentionTimer;
        public bool running;

        public NetBool netDashActive = new NetBool(false);
        public int dashFrame;
        public int dashInterval;
        public int dashPeak;
        public int dashHeight;
        public NetInt netDashProgress = new NetInt(0);

        public Vector2 pathFrom;
        public float pathTotal;
        public float pathProgress;
        public Vector2 pathIncrement;
        public int pathSegment;

        public NetBool netSmashActive = new NetBool(false);

        public NetBool netSweepActive = new NetBool(false);
        public int sweepTimer;
        public int sweepFrame;
        public int sweepInterval;

        public NetBool netSpecialActive = new NetBool(false);
        public int specialTimer;
        public int specialInterval;
        public int specialCeiling;
        public int specialFloor;
        public int specialFrame;
        public Vector2 workVector;

        public NetBool netDazeActive = new NetBool(false);
        public int cooldownTimer;
        public int cooldownInterval;
        public int hitTimer;
        public int pushTimer;

        public int moveDirection;
        public int altDirection;
        public int trackDashProgress;
        public Vector2 setPosition = Vector2.Zero;
        
        public Character()
        {
        }

        public Character(CharacterHandle.characters type)
          : base(
                new AnimatedSprite(Path.Combine("Characters","Abigail")), 
                CharacterHandle.CharacterStart(CharacterHandle.CharacterHome(type)),
                CharacterHandle.CharacterLocation(CharacterHandle.CharacterHome(type)),
                2, 
                type.ToString(), 
                CharacterHandle.CharacterPortrait(type), 
                false
                )
        {

            characterType = type;

            willDestroyObjectsUnderfoot = false;

            HideShadow = true;

            SimpleNonVillagerNPC = true;

            SettleOccupied();

            LoadOut();

        }

        protected override void initNetFields()
        {
            base.initNetFields();
            NetFields.AddField(netDirection, "netDirection");
            NetFields.AddField(netAlternative, "netAlternative");
            NetFields.AddField(netHaltActive, "netHaltActive");
            NetFields.AddField(netStandbyActive, "netStandbyActive");
            NetFields.AddField(netSceneActive, "netSceneActive");
            NetFields.AddField(netDashActive, "netDashActive");
            NetFields.AddField(netDashProgress, "netDashProgress");
            NetFields.AddField(netSmashActive, "netSmashActive");
            NetFields.AddField(netSweepActive, "netSweepActive");
            NetFields.AddField(netSpecialActive, "netSpecialActive");
            NetFields.AddField(netWorkActive, "netWorkActive");
            NetFields.AddField(netDazeActive, "netDazeActive");
        }

        public virtual void LoadIntervals()
        {

            modeActive = mode.random;

            collidePriority = new Random().Next(20);

            gait = 1.6f;

            moveInterval = 12;

            specialInterval = 30;

            specialCeiling = 1;

            specialFloor = 1;

            cooldownInterval = 300;

            workFrames = specialFrames;

            dashPeak = 128;

            dashInterval = 9;

            sweepInterval = 9;

        }

        public virtual void LoadOut()
        {

            LoadIntervals();

            characterTexture = CharacterHandle.CharacterTexture(characterType);

            haltFrames = FrameSeries(32, 32, 0, 0, 1);

            walkFrames = FrameSeries(32, 32, 0, 128, 6, FrameSeries(32, 32, 0, 0, 1));

            idleFrames = new()
            {
                [0] = new()
                {
                    new Rectangle(128, 0, 32, 32),
                    new Rectangle(160, 0, 32, 32),
                },
                [1] = new()
                {
                    new Rectangle(128, 0, 32, 32),
                    new Rectangle(160, 0, 32, 32),
                },
                [2] = new()
                {
                    new Rectangle(128, 0, 32, 32),
                    new Rectangle(160, 0, 32, 32),
                },
                [3] = new()
                {
                    new Rectangle(128, 0, 32, 32),
                    new Rectangle(160, 0, 32, 32),
                },
            };

            specialFrames = new()
            {
                [0] = new()
                {

                    new(64, 64, 32, 32),
                    new(96, 64, 32, 32),

                },
                [1] = new()
                {

                    new(64, 32, 32, 32),
                    new(96, 32, 32, 32),

                },
                [2] = new()
                {

                    new(64, 0, 32, 32),
                    new(96, 0, 32, 32),

                },
                [3] = new()
                {

                    new(64, 96, 32, 32),
                    new(96, 96, 32, 32),

                },

            };

            dashFrames = new()
            {
                [0] = new()
                {
                    new(0, 192, 32, 32),
                },
                [1] = new()
                {
                    new(0, 160, 32, 32),
                },
                [2] = new()
                {
                    new(0, 128, 32, 32),
                },
                [3] = new()
                {
                    new(0, 224, 32, 32),
                },
                [4] = new()
                {
                    new(32, 64, 32, 32),
                },
                [5] = new()
                {
                    new(32, 32, 32, 32),
                },
                [6] = new()
                {
                    new(32, 0, 32, 32),
                },
                [7] = new()
                {
                    new(32, 96, 32, 32),
                },
                [8] = new()
                {
                    new(96,192,32,32),
                    new(128,192,32,32),
                    new(160,192,32,32),
                },
                [9] = new()
                {
                    new(96,160,32,32),
                    new(128,160,32,32),
                    new(160,160,32,32),
                },
                [10] = new()
                {
                    new(96,128,32,32),
                    new(128,128,32,32),
                    new(160,128,32,32),
                },
                [11] = new()
                {
                    new(96,224,32,32),
                    new(128,224,32,32),
                    new(160,224,32,32),
                },
            };

            smashFrames = new()
            {
                [0] = new()
                {
                    new(0, 320, 32, 32),new(32, 320, 32, 32),
                },
                [1] = new()
                {
                    new(0, 288, 32, 32),new(32, 288, 32, 32),
                },
                [2] = new()
                {
                    new(0, 256, 32, 32),new(32, 256, 32, 32),
                },
                [3] = new()
                {
                    new(0, 288, 32, 32),new(32, 288, 32, 32),
                },
                [4] = new()
                {
                    new(64, 320, 32, 32),
                },
                [5] = new()
                {
                    new(64, 288, 32, 32),
                },
                [6] = new()
                {
                    new(64, 256, 32, 32),
                },
                [7] = new()
                {
                    new(64, 288, 32, 32),
                },
                [8] = new()
                {
                    new(96, 320, 32, 32),
                },
                [9] = new()
                {
                    new(96, 288, 32, 32),
                },
                [10] = new()
                {
                    new(96, 256, 32, 32),
                },
                [11] = new()
                {
                    new(96, 288, 32, 32),
                },
            };

            sweepFrames = new()
            {
                [0] = new()
                {
                    new Rectangle(192, 288, 32, 32),
                    new Rectangle(224, 288, 32, 32),
                    new Rectangle(128, 288, 32, 32),
                    new Rectangle(160, 288, 32, 32),
                },
                [1] = new()
                {
                    new Rectangle(128, 256, 32, 32),
                    new Rectangle(160, 256, 32, 32),
                    new Rectangle(192, 256, 32, 32),
                    new Rectangle(224, 256, 32, 32),
                },
                [2] = new()
                {
                    new Rectangle(128, 288, 32, 32),
                    new Rectangle(160, 288, 32, 32),
                    new Rectangle(192, 288, 32, 32),
                    new Rectangle(224, 288, 32, 32),
                },
                [3] = new()
                {
                    new Rectangle(128, 256, 32, 32),
                    new Rectangle(160, 256, 32, 32),
                    new Rectangle(192, 256, 32, 32),
                    new Rectangle(224, 256, 32, 32),
                },
            };

            alertFrames = new()
            {
                [0] = new()
                {
                    new Rectangle(192, 320, 32, 32),
                },
                [1] = new()
                {
                    new Rectangle(160, 320, 32, 32),
                },
                [2] = new()
                {
                    new Rectangle(128, 320, 32, 32),
                },
                [3] = new()
                {
                    new Rectangle(224, 320, 32, 32),
                },
            };

            loadedOut = true;

        }

        public virtual Dictionary<int, List<Rectangle>> FrameSeries( int width, int height, int startX = 0, int startY = 0, int length = 6, Dictionary<int, List<Rectangle>> frames = null)
        {

            if(frames == null)
            {
                frames = new()
                {
                    [0] = new(),
                    [1] = new(),
                    [2] = new(),
                    [3] = new(),
                };
            }

            Dictionary<int, int> normalSequence = new()
            {
                [0] = 2,
                [1] = 1,
                [2] = 0,
                [3] = 3
            };

            foreach (KeyValuePair<int, int> keyValuePair in normalSequence )
            {

                for (int index = 0; index < length; index++)
                {
                    
                    Rectangle rectangle = new(startX, startY, width, height);
                    
                    rectangle.X += width * index;
                    
                    rectangle.Y += height * keyValuePair.Value;
                    
                    frames[keyValuePair.Key].Add(rectangle);
                
                }

            }

            return frames;

        }

        public override void draw(SpriteBatch b, float alpha = 1f)
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

            DrawEmote(b);

            if (netStandbyActive.Value)
            {

                DrawStandby(b, localPosition, drawLayer);

                return;

            }
            else if (netHaltActive.Value)
            {
                
                if (onAlert)
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
                        (netDirection.Value % 2 == 0 && netAlternative.Value == 3) ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
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
                     (netDirection.Value % 2 == 0 && netAlternative.Value == 3) || netDirection.Value == 3 ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                     drawLayer
                 );

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
                    (netDirection.Value % 2 == 0 && netAlternative.Value == 3) ? (SpriteEffects)1 : 0,
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
                    (netDirection.Value % 2 == 0 && netAlternative.Value == 3) ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
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
                    (netDirection.Value % 2 == 0 && netAlternative.Value == 3) || netDirection.Value == 3 ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    drawLayer
                );

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
                        (netDirection.Value % 2 == 0 && netAlternative.Value == 3) ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                        drawLayer
                    );

                }

            }

            DrawShadow(b, localPosition, drawLayer);

        }

        public override void DrawEmote(SpriteBatch b)
        {

            if (IsEmoting && !Game1.eventUp)
            {

                float drawLayer = (float)StandingPixel.Y / 10000f;

                b.Draw(Game1.emoteSpriteSheet, Game1.GlobalToLocal(Position)-new Vector2(0,overhead == 0 ? 144 : overhead), new Microsoft.Xna.Framework.Rectangle(base.CurrentEmoteIndex * 16 % Game1.emoteSpriteSheet.Width, base.CurrentEmoteIndex * 16 / Game1.emoteSpriteSheet.Width * 16, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, drawLayer);
            
            }
            else if (netSceneActive.Value && eventName != null)
            {
                
                if (Mod.instance.eventRegister.ContainsKey(eventName))
                {

                    if (Mod.instance.eventRegister[eventName].dialogueLoader.ContainsKey(Name))
                    {

                        float drawLayer = (float)StandingPixel.Y / 10000f;

                        b.Draw(
                            Mod.instance.iconData.displayTexture,
                            Game1.GlobalToLocal(Position) - new Vector2(0, overhead == 0 ? 144 : overhead),
                            Mod.instance.iconData.QuestDisplay(Journal.Quest.questTypes.approach),
                            Color.White,
                            0f,
                            Vector2.Zero,
                            4f,
                            SpriteEffects.None,
                            drawLayer
                        );

                    }

                }
                
            }
            else if (netDazeActive.Value)
            {

                float drawLayer = (float)StandingPixel.Y / 10000f;

                b.Draw(
                    Mod.instance.iconData.displayTexture,
                    Game1.GlobalToLocal(Position) - new Vector2(0, overhead == 0 ? 144 : overhead),
                    Mod.instance.iconData.DisplayRect(IconData.displays.daze),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    4f,
                    SpriteEffects.None,
                    drawLayer
                );

            }
            else if (Mod.instance.dialogue.ContainsKey(characterType))
            {

                if(Mod.instance.dialogue[characterType].promptDialogue.Count > 0)
                {

                    float drawLayer = (float)StandingPixel.Y / 10000f;

                    b.Draw(
                        Mod.instance.iconData.displayTexture, 
                        Game1.GlobalToLocal(Position) - new Vector2(0, overhead == 0 ? 144 : overhead),
                        Mod.instance.iconData.QuestDisplay(Mod.instance.dialogue[characterType].promptDialogue.First().Value), 
                        Color.White, 
                        0f, 
                        Vector2.Zero, 
                        4f, 
                        SpriteEffects.None, 
                        drawLayer
                    );

                }

            }

        }

        public virtual void DrawAlert(SpriteBatch b, Vector2 localPosition, float drawLayer)
        {

            Vector2 alertVector = localPosition - new Vector2(32, 64f);

            Rectangle alertFrame = alertFrames[netDirection.Value][0];

            b.Draw(
                 characterTexture,
                 alertVector,
                 alertFrame,
                 Color.White,
                 0f,
                 Vector2.Zero,
                 4f,
                 (netDirection.Value % 2 == 0 && netAlternative.Value == 3) ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                 drawLayer
             );

        }

        public virtual void DrawStandby(SpriteBatch b, Vector2 localPosition, float drawLayer)
        {



        }

        public int IdleFrame()
        {

            int interval = 12 / idleFrames[0].Count();

            int timeLapse = (int)(Game1.currentGameTime.TotalGameTime.TotalSeconds % 12);

            if (timeLapse == 0) { return 0; }

            int frame = (int)timeLapse / interval;

            return frame;

        }

        public virtual void DrawShadow(SpriteBatch b, Vector2 localPosition, float drawLayer, float offset = 0)
        {

            b.Draw(
                Game1.shadowTexture,
                localPosition + new Vector2(10 + offset, 44f),
                Game1.shadowTexture.Bounds,
                Color.White * 0.75f,
                0f,
                Vector2.Zero,
                4f,
                SpriteEffects.None,
                drawLayer - 0.0001f
                );

        }

        public override Rectangle GetBoundingBox()
        {

            return new Rectangle((int)Position.X + 8, (int)Position.Y + 8, 48, 48);

        }

        public virtual Rectangle GetHitBox()
        {
            return new Rectangle((int)Position.X - 32, (int)Position.Y - 32, 128, 128);
        }

        public override void reloadSprite(bool onlyAppearance = false)
        {
            base.reloadSprite(onlyAppearance);
            Portrait = CharacterHandle.CharacterPortrait(characterType);

        }

        public override void reloadData()
        {
            CharacterDisposition characterDisposition = CharacterHandle.CharacterDisposition(characterType);
            Age = characterDisposition.Age;
            Manners = characterDisposition.Manners;
            SocialAnxiety = characterDisposition.SocialAnxiety;
            Optimism = characterDisposition.Optimism;
            Gender = characterDisposition.Gender;
            datable.Value = characterDisposition.datable;
            Birthday_Season = characterDisposition.Birthday_Season;
            Birthday_Day = characterDisposition.Birthday_Day;
            id = characterDisposition.id;
        }

        public override void reloadDefaultLocation()
        {
            
            if(characterType == CharacterHandle.characters.none)
            {

                characterType = CharacterHandle.characters.Effigy;

            }

            DefaultMap = CharacterHandle.CharacterLocation(CharacterHandle.CharacterHome(characterType));

            DefaultPosition = CharacterHandle.CharacterStart(CharacterHandle.CharacterHome(characterType));


        }

        public override void receiveGift(StardewValley.Object o, Farmer giver, bool updateGiftLimitInfo = true, float friendshipChangeMultiplier = 1, bool showResponse = true)
        {

        }

        public override bool checkAction(Farmer who, GameLocation l)
        {

            if (Mod.instance.Config.actionButtons.GetState() == SButtonState.Held)
            {

                return false;

            }

            if (Mod.instance.Config.specialButtons.GetState() == SButtonState.Held)
            {

                return false;

            }

            if (Mod.instance.eventRegister.ContainsKey("transform"))
            {

                Mod.instance.CastMessage("Unable to converse while transformed");

                return false;
            }


            if (l.Name != currentLocation.Name)
            {

                CharacterMover mover = new(characterType);

                mover.RemovalSet(l.Name);

                Mod.instance.movers[characterType] = mover;

                return false;

            }

            foreach (NPC character in currentLocation.characters)
            {

                if (character is StardewValley.Monsters.Monster monster && (double)Vector2.Distance(Position, monster.Position) <= 800.0)
                {

                    return false;

                }

            }

            if (netDashActive.Value || netSpecialActive.Value)
            {

                return false;

            }

            if (!EngageDialogue())
            {
                
                return false;
            
            }

            LookAtTarget(who.Position, true);

            return true;

        }

        public virtual bool EngageDialogue()
        {

            if (!Mod.instance.dialogue.ContainsKey(characterType))
            {
                
                Mod.instance.dialogue[characterType] = new(characterType);

            }

            if (netSceneActive.Value)
            {

                if(eventName == null)
                {

                    return false;

                }

                if (Mod.instance.eventRegister.ContainsKey(eventName))
                {

                    if (Mod.instance.eventRegister[eventName].DialogueNext(this))
                    {
                        
                        Halt();

                        return true;

                    };

                }

                return false;

            }

            if(Mod.instance.activeEvent.Count > 0)
            {

                return false;

            }

            Halt();

            Mod.instance.dialogue[characterType].DialogueApproach();

            return true;

        }

        public override void Halt()
        {

            netHaltActive.Set(true);

            TargetIdle();

        }

        public virtual void ResetActives(bool clearEvents = false)
        {

            ClearIdle();

            if (netStandbyActive.Value)
            {

                netStandbyActive.Set(false);

            }

            ClearMove();

            StopMoving();

            ClearSweep();

            ClearSpecial();

            ResetTimers();

            SettleOccupied();

            if (clearEvents)
            {
                eventVectors.Clear();

            }

        }

        public virtual void ResetTimers()
        {

            idleTimer = 0;

            moveTimer = 0;

            sweepTimer = 0;

            specialTimer = 0;

            cooldownTimer = 0;

            hitTimer = 0;

            lookTimer = 0;

            collideTimer = 0;

            pushTimer = 0;

            dashHeight = 0;

            followTimer = 0;

            attentionTimer = 0;

            stationaryTimer = 0;

        }

        public virtual void ClearIdle()
        {

            if (netHaltActive.Value)
            {

                netHaltActive.Set(false);

            }

            if (netDazeActive.Value)
            {

                netDazeActive.Set(false);

            }

            idleTimer = 0;

            onAlert = false;

        }

        public virtual void ClearMove()
        {

            pathActive = pathing.none;

            destination = Vector2.Zero;

            traversal.Clear();

            if (netDashActive.Value)
            {

                netDashActive.Set(false);

            }

            if (netSmashActive.Value)
            {

                netSmashActive.Set(false);

            }

            netDashProgress.Set(0);

            pathProgress = 0;

            pathTotal = 0;

            dashFrame = 0;

            stationaryTimer = moveInterval * 2;

            //moveTimer = 0;

            //moveFrame = 0;

        }

        public virtual void StopMoving()
        {

            moveTimer = 0;

            moveFrame = 0;

        }

        public virtual void ClearSweep(bool apply = false)
        {

            if (netSweepActive.Value)
            {

                netSweepActive.Set(false);

            }

            sweepFrame = 0;

            sweepTimer = 0;

        }

        public virtual void ClearSpecial()
        {
            
            if (netSpecialActive.Value)
            {

                netSpecialActive.Set(false);

            }

            specialTimer = 0;

            specialFrame = 0;

            if (netWorkActive.Value)
            {

                netWorkActive.Set(false);

            }

        }

        public void LookAtTarget(Vector2 target, bool force = false)
        {
            
            if (lookTimer > 0 && !force) { return; }

            List<int> directions = ModUtility.DirectionToTarget(Position, target);

            moveDirection = directions[0];

            altDirection = directions[1];

            netDirection.Set(moveDirection);

            netAlternative.Set(altDirection);

            lookTimer = (int)(2f * MoveSpeed(Vector2.Distance(Position,target)));

        }

        public override void performTenMinuteUpdate(int timeOfDay, GameLocation l)
        {

        }

        public override void behaviorOnFarmerPushing()
        {

            if (Context.IsMainPlayer && !netSceneActive.Value)
            {

                pushTimer += 2;

                if(pushTimer > 10)
                {
                    
                    if (ChangeBehaviour(true))
                    {

                        TargetRandom(4);

                    }

                    pushTimer = 0;

                }

            }

        }

        public virtual void normalUpdate(GameTime time, GameLocation location)
        {

            if (!loadedOut)
            {
                LoadOut();
            }

            if (Context.IsMainPlayer)
            {

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

                updateEmote(time);

            }

        }

        public override void update(GameTime time, GameLocation location)
        {

            if (location.Name != currentLocation.Name)
            {

                CharacterMover mover = new(characterType);
                
                mover.RemovalSet(location.Name);

                Mod.instance.movers[characterType] = mover;

                return;

            }

            normalUpdate(time, location);

            if (!Context.IsMainPlayer)
            {
                
                UpdateMultiplayer();

                return;

            }

            if (modeActive == mode.scene)
            {

                ProgressScene();

                return;

            }

            UpdateBehaviour();

            ChooseBehaviour();

            Traverse();

        }

        // ========================================
        // SET BEHAVIOUR
        public virtual void ProgressScene()
        {

            if (netHaltActive.Value)
            {

                netHaltActive.Set(false);

            }

            UpdateSweep();

            UpdateSpecial();

            if (eventVectors.Count > 0)
            {

                KeyValuePair<int, Vector2> eventVector = eventVectors.First();

                float distance = Vector2.Distance(Position, eventVector.Value);

                //LookAtTarget(eventVector.Value);

                Position = ModUtility.PathMovement(Position, eventVector.Value, MoveSpeed(distance));

                if(netDashActive.Value || netSmashActive.Value)
                {
                    
                    if (pathProgress > 0)
                    {
                        
                        pathProgress--;

                    }

                }

                UpdateMove();

                if (Vector2.Distance(Position, eventVector.Value) <= 4f)
                {

                    Position = eventVector.Value;

                    eventVectors.Remove(eventVector.Key);

                    if (Mod.instance.eventRegister.ContainsKey(eventName))
                    {

                        Mod.instance.eventRegister[eventName].EventScene(eventVector.Key);

                    }

                    if(eventVectors.Count > 0)
                    {

                        LookAtTarget(eventVectors.First().Value, true);

                    }
                    else
                    {
                        
                        ClearMove();
                    
                    }

                }

            }
            else
            {

                StopMoving();

            }

        }

        public virtual void TargetEvent(int key, Vector2 target, bool clear = true)
        {

            pathActive = pathing.scene;

            if (clear)
            {

                eventVectors.Clear();

            }

            eventVectors.Add(key, target);

            destination = ModUtility.PositionToTile(target); //target / 64; //ModUtility.PositionToTile(target);

            if (eventVectors.Count == 1)
            {

                LookAtTarget(target, true);

            }

        }

        public virtual bool ChangeBehaviour(bool urgent = false)
        {

            if (netHaltActive.Value)
            {
                /*if (collideTimer <= 0)
                {

                    if (CollideCharacters())
                    {
 
                        return true;

                    }

                }*/

                return false;

            }

            if (netStandbyActive.Value)
            {

                return false;

            }
            
            if (netSpecialActive.Value)
            {

                return false;

            }

            if (netSweepActive.Value)
            {

                return false;

            }

            if (netDashActive.Value)
            {

                return false;

            }

            if (netSmashActive.Value)
            {

                return false;

            }

            if (urgent)
            {

                return true;

            }

            if (destination != Vector2.Zero)
            {

                if (ArrivedDestination())
                {

                    return true;

                }

                if(pathActive == pathing.player)
                {

                    if (TrackToClose())
                    {

                        ClearMove();

                        return true;

                    }

                    if (modeActive == mode.track)
                    {
                        
                        if (TrackToFar())
                        {
                            
                            followTimer = 0;
                            
                            ClearMove();

                            return true;

                        }

                    }

                }

                return false;

            }

            if (idleTimer > 0)
            {

                if (modeActive == mode.track)
                {

                    // need to stay where the action is

                    if (TrackToFar(640,7))
                    {
                        
                        followTimer = 0;

                        ClearIdle();

                        return true;

                    }

                }

                if (collideTimer<= 0)
                {
                    
                    if (CollideCharacters(occupied))
                    {

                        ClearIdle();

                        TargetRandom(4);

                    }

                }

                return false;

            }

            return true;

        }

        public virtual void ChooseBehaviour()
        {

            if (!ChangeBehaviour())
            {

                return;

            }

            switch (modeActive)
            {

                case mode.track:

                    if (TargetMonster())
                    {

                        return;

                    }

                    if (TargetWork())
                    { 
                        
                        return; 
                    
                    }

                    if (TargetTrack())
                    {

                        return;

                    };

                    TargetRandom(12);

                    return;

                case mode.roam:

                    if (TargetWork())
                    {

                        return;

                    }

                    if (TargetRoam())
                    {

                        return;

                    };

                    TargetRandom(12);

                    return;

            }

            TargetRandom();

        }

        public virtual bool TargetIdle(int timer = -1)
        {

            StopMoving();

            if (!netSceneActive.Value)
            {

                if (ModUtility.TileAccessibility(currentLocation, occupied) != 0)
                {

                    WarpToEntrance();

                }

            }

            if (timer == -1)
            {

                switch (modeActive)
                {

                    case mode.track:
                    case mode.roam:

                        timer = 360;

                        break;

                    default: //mode.scene: mode.standby:

                        timer = 720;

                        break;

                }

            }

            Random random = new();

            if(random.Next(2) == 0)
            {

                moveDirection = 2;

                netDirection.Set(2);

            }

            idleTimer = timer;

            return true;

        }

        public virtual bool TargetMonster()
        {

            List<StardewValley.Monsters.Monster> monsters = ModUtility.MonsterProximity(currentLocation, new() { Position, }, 640f, true);

            if (monsters.Count > 0)
            {

                if (cooldownTimer <= 0)
                {

                    foreach (StardewValley.Monsters.Monster monster in monsters)
                    {

                        if (MonsterAttack(monster))
                        {

                            return true;

                        }

                    }

                }

                onAlert = true;

                return TargetIdle(180);

            }

            return false;

        }

        public virtual bool MonsterAttack(StardewValley.Monsters.Monster monster)
        {

            float distance = Vector2.Distance(Position, monster.Position);

            string terrain = ModUtility.GroundCheck(currentLocation, new Vector2((int)(monster.Position.X/64),(int)(monster.Position.Y/64)));

            if (distance >= 192f)
            {

                if(terrain != "ground")
                {
                    return SpecialAttack(monster);
                }

                switch (Mod.instance.randomIndex.Next(3))
                {
                    case 0:

                        return SpecialAttack(monster);

                    case 1:

                        return SmashAttack(monster);

                    default:

                        return PathTarget(monster.Position, 2, 1);

                }

            }

            return SweepAttack(monster);

        }

        public virtual bool SmashAttack(StardewValley.Monsters.Monster monster)
        {

            ResetActives();

            if (PathTarget(monster.Position, 2, 1))
            {
                
                pathActive = pathing.monster;

                SetDash(monster.Position,true);

                cooldownTimer = cooldownInterval / 2;

                return true;

            }

            return false;

        }

        public virtual bool SweepAttack(StardewValley.Monsters.Monster monster)
        {

            ResetActives();

            if (PathTarget(monster.Position, 2, 0))
            {

                pathActive = pathing.monster;

                netSweepActive.Set(true);

                cooldownTimer = cooldownInterval / 2;

                sweepFrame = 0;

                sweepTimer = sweepFrames[0].Count() * sweepInterval;

                //int stun = Math.Max(monster.stunTime.Value, 500);

                //monster.stunTime.Set(stun);

                return true;

            }

            return false;

        }

        public virtual bool SpecialAttack(StardewValley.Monsters.Monster monster)
        {

            ResetActives();

            netSpecialActive.Set(true);

            specialTimer = 90;

            cooldownTimer = cooldownInterval;

            LookAtTarget(monster.Position, true);

            SpellHandle fireball = new(Game1.player, new() { monster, }, Mod.instance.CombatDamage() / 2);

            fireball.origin = GetBoundingBox().Center.ToVector2();

            fireball.type = SpellHandle.spells.missile;

            fireball.missile = IconData.missiles.fireball;
            
            fireball.display = IconData.impacts.impact;

            fireball.added = new() { SpellHandle.effects.aiming, };

            fireball.power = 3;

            Mod.instance.spellRegister.Add(fireball);

            return true;

        }

        public virtual void ConnectSweep()
        {

            SpellHandle swipeEffect = new(Game1.player, Position, 192, Mod.instance.CombatDamage() / 2);

            swipeEffect.instant = true;

            swipeEffect.added = new() { effects.push, };

            swipeEffect.sound = sounds.swordswipe;

            swipeEffect.display = IconData.impacts.flashbang;

            Mod.instance.spellRegister.Add(swipeEffect);

        }

        public virtual void SetDash(Vector2 target, bool smash = false)
        {

            LookAtTarget(target,true);

            StopMoving();

            if (!smash)
            {

                netDashActive.Set(true);

            }
            else
            {

                netSmashActive.Set(true);

            }

            pathFrom = Position;

            float pathDistance = Vector2.Distance(pathFrom, target);

            pathIncrement = ModUtility.PathFactor(Position, target) * MoveSpeed(pathDistance);

            pathProgress = (int)(Vector2.Distance(Position, target) / Vector2.Distance(new(0, 0), pathIncrement));

            pathTotal = pathProgress;

            pathSegment = dashInterval;

            int pathRequirement;

            if (!smash)
            {

                pathRequirement = dashFrames[0].Count + dashFrames[4].Count + dashFrames[8].Count;
            }
            else
            {
                pathRequirement = smashFrames[0].Count + smashFrames[4].Count + smashFrames[8].Count;

            }

            int pathSqueeze = (int)(pathProgress / pathRequirement);

            if(pathSqueeze < dashInterval)
            {

                pathSegment = pathSqueeze;

            }

        }

        public virtual bool TargetTrack()
        {
            
            if(followTimer > 0)
            {

                return false;

            }

            if (!Mod.instance.trackers.ContainsKey(characterType))
            {
                
                return false;
            
            }

            if (TrackToClose())
            {
                
                if(attentionTimer == 0)
                {
                    
                    // idle or random direction

                    followTimer = 120;
                    
                    return false;

                }

                // keep attention on farmer

                idleTimer = 10;

                LookAtTarget(Mod.instance.trackers[characterType].followPlayer.Position);

                return true;

            }

            if (TrackToFar())
            {

                Vector2 lastPosition = Position;

                if (Mod.instance.trackers[characterType].WarpToPlayer())
                {

                    return true;

                }

            }

            if (PathTrack())
            {

                pathActive = pathing.player;

                return true;

            }

            return false;

        }

        public virtual bool TrackToClose(int close = 128)
        {

            if (Mod.instance.trackers.ContainsKey(characterType))
            {
                
                if (Vector2.Distance(Position, Mod.instance.trackers[characterType].followPlayer.Position) <= close)
                {

                    return true;

                }

            }
            else
            {

                if (Vector2.Distance(Position, Game1.player.Position) <= close)
                {

                    return true;

                }

            }

            return false;

        }

        public virtual bool TrackToFar(int limit = 960, int nodeLimit = 20)
        {

            if (netStandbyActive.Value)
            {

                return false;

            }

            if (Mod.instance.trackers[characterType].nodes.Count >= nodeLimit)
            {

                return true;

            }

            if (Vector2.Distance(Position, Mod.instance.trackers[characterType].followPlayer.Position) >= limit || !Utility.isOnScreen(Position, 128))
            {

                return true;

            }

            /*if (cooldownTimer <= 0)
            {
                
                if (ModUtility.MonsterProximity(currentLocation, new() { Mod.instance.trackers[characterType].followPlayer.Position }, 384f).Count > 0)
                {

                    return true;

                }

            }*/

            return false;

        }

        public virtual bool TargetRoam()
        {

            if (roamVectors.Count == 0)
            {

                return false;

            }

            Vector2 roamVector = roamVectors[roamIndex];

            if (roamVector.X < 0)
            {

                UpdateRoam();

                TargetIdle(720);

                netStandbyActive.Set(true);

                return true;

            }

            if (Vector2.Distance(roamVectors[roamIndex], Position) <= 192f || roamLapse < Game1.currentGameTime.TotalGameTime.TotalMinutes)
            {

                UpdateRoam();

                return true;

            }

            if (PathTarget(roamVectors[roamIndex], 2, 2))
            {
                
                pathActive = pathing.roam;

                return true;

            }

            return false;

        }

        public virtual void TargetRandom(int level = 8)
        {

            Random random = new Random();

            int decision = random.Next(level);

            switch (decision)
            {
                case 0:
                case 1:
                case 2:
                case 3:

                    int newDirection = random.Next(10);

                    if (newDirection >= 8)
                    {

                        newDirection = ModUtility.DirectionToTarget(Position, tether)[2];

                    }

                    List<int> directions = new()
                    {

                        (newDirection + 4) % 8,
                        (newDirection + 6) % 8,
                        (newDirection + 2) % 8,

                    };

                    foreach (int direction in directions)
                    {

                        if (PathTarget(occupied*64, 0, 1, direction))
                        {
                            
                            pathActive = pathing.random;

                            return;

                        }

                    }

                    break;

            }

            TargetIdle();

        }

        public virtual bool TargetWork()
        {

            return false;

        }

        public virtual void PerformWork()
        {


        }

        // ========================================
        // UPDATE

        public virtual void UpdateBehaviour()
        {

            UpdateIdle();

            UpdateMove();

            UpdateSweep();

            UpdateSpecial();

            if (cooldownTimer > 0)
            {

                cooldownTimer--;

            }

            if (hitTimer > 0)
            {

                hitTimer--;

            }

            if (lookTimer > 0)
            {

                lookTimer--;

            }

            if (collideTimer > 0)
            {

                collideTimer--;

            }

            if (pushTimer > 0)
            {

                pushTimer--;


            }

            if (dashHeight > 0)
            {

                dashHeight--;

            }

            if(followTimer > 0)
            {

                followTimer--;

            }

            if (attentionTimer > 0)
            {

                attentionTimer--;

            }


        }

        public virtual void UpdateMultiplayer()
        {
            
            if (netHaltActive.Value)
            {
                
                idleTimer++;

                CheckAlert();

                return;

            }

            if (netSweepActive.Value)
            {

                sweepTimer++;

                if (sweepTimer == sweepInterval)
                {

                    sweepFrame++;

                    sweepTimer = 0;

                }

            }
            else
            {
                sweepFrame = 0;

                sweepTimer = 0;

            }

            if (netSpecialActive.Value)
            {

                specialTimer++;

                if (specialTimer == specialInterval)
                {

                    specialFrame++;

                    if (specialFrame > specialCeiling)
                    {

                        specialFrame = specialFloor;

                    }

                    specialTimer = 0;

                }

                return;

            }
            else
            {
                specialFrame = 0;

                specialTimer = 0;

            }

            if (setPosition != Position || netDirection.Value != moveDirection || netAlternative.Value != altDirection)
            {

                setPosition = Position;

                moveDirection = netDirection.Value;

                altDirection = netAlternative.Value;

                moveTimer--;

                if (netDashActive.Value || netSmashActive.Value)
                {
                    if (netDashProgress.Value == 0 && dashHeight <= dashPeak)
                    {

                        dashHeight += 2;

                    }
                    else if (netDashProgress.Value == 2 && dashHeight > 0)
                    {

                        dashHeight -= Math.Min(dashHeight, 2);

                    }

                    if (netDashProgress.Value != trackDashProgress)
                    {

                        dashFrame = 0;

                        trackDashProgress = netDashProgress.Value;

                        moveTimer = moveInterval;

                    }

                }
                else if (dashHeight > 0)
                {

                    dashHeight -= Math.Min(dashHeight,2);

                }

                if (moveTimer <= 0)
                {

                    moveFrame++;

                    if (moveFrame >= walkFrames[0].Count)
                    {

                        moveFrame = 1;
                    
                    }

                    moveTimer = moveInterval;

                    moveTimer -= 3;

                    dashFrame++;

                    stationaryTimer = 30;

                    idleTimer = 0;

                }

                return;

            }

            if (stationaryTimer > 0)
            {

                stationaryTimer--;

                if (stationaryTimer == 0)
                {

                    moveFrame = 0;

                    moveTimer = 0;

                }

            }
            else
            {

                idleTimer++;
            
            }

        }

        public virtual void UpdateEvent(int index)
        {
            
            if (eventName == null)
            {

                return;

            }

            if (!Mod.instance.eventRegister.ContainsKey(eventName))
            {

                eventName = null;

                return;

            }

            Mod.instance.eventRegister[eventName].EventScene(index);
        
        }

        public virtual void UpdateIdle()
        {

            if (netHaltActive.Value)
            {

                if (idleTimer <= 0)
                {

                    ClearIdle();

                    ClearMove();

                    return;

                }

            }

            if (idleTimer > 0)
            {

                idleTimer--;

                CheckAlert();

            }

        }

        public virtual void CheckAlert()
        {

            if (idleTimer % 10 == 0)
            {

                onAlert = false;

                List<StardewValley.Monsters.Monster> monsters = ModUtility.MonsterProximity(currentLocation, new() { Position }, 960);

                if (monsters.Count > 0)
                {

                    onAlert = true;

                    if (Context.IsMainPlayer)
                    {

                        LookAtTarget(monsters.First().Position, true);

                    }

                }

            }

        }

        public virtual void UpdateSweep()
        {

            if (sweepTimer > 0)
            {

                sweepTimer--;

            }

            if (netSweepActive.Value)
            {

                if (sweepTimer <= 0)
                {

                    ClearSweep(true);

                }
                else
                {

                    if (sweepTimer % sweepInterval == 0)
                    {

                        sweepFrame++;

                    }


                    if (sweepTimer == sweepInterval)
                    {

                        ConnectSweep();

                    }

                }

            }

        }

        public virtual void UpdateMove()
        {

            if (destination == Vector2.Zero)
            {

                return;

            }

            if (moveTimer > 0)
            {

                moveTimer--;

            }

            float distance = Vector2.Distance(Position, destination*64);

            if (netDashActive.Value || netSmashActive.Value)
            {

                DashAscension();

                if(pathProgress % pathSegment != 0)
                {
                    
                    return;

                }

                dashFrame++;

                if (netDashActive.Value)
                {

                    if (pathProgress + (pathSegment * dashFrames[0].Count) <= pathTotal)
                    {
                        
                        if (netDashProgress.Value != 1)
                        {

                            netDashProgress.Set(1);

                            dashFrame = 0;

                        }

                    }

                    if(pathProgress <= (pathSegment * dashFrames[8].Count))
                    {

                        if(netDashProgress.Value != 2)
                        {

                            netDashProgress.Set(2);

                            dashFrame = 0;

                        }

                    }

                }

                if (netSmashActive.Value)
                {

                    if (pathProgress + (pathSegment * smashFrames[0].Count) <= pathTotal)
                    {

                        if (netDashProgress.Value != 1)
                        {

                            netDashProgress.Set(1);

                            dashFrame = 0;

                        }

                    }

                    if (pathProgress <= (pathSegment * smashFrames[8].Count))
                    {

                        if (netDashProgress.Value != 2)
                        {

                            netDashProgress.Set(2);

                        }

                    }

                    if(pathProgress == pathSegment)
                    {

                        ConnectSweep();

                    }

                }

                return;

            }

            if (moveTimer <= 0)
            {

                moveTimer = (int)MoveSpeed(distance, true);

                moveFrame++;

                /*int right = 1 + ((walkFrames.Count - 1) / 2);

                if (moveFrame == 1)
                {

                    if (walkSide)
                    {

                        moveFrame = 1 + ((walkFrames[0].Count - 1) / 2);

                        walkSide = false;

                    }
                    else
                    {

                        walkSide = true;

                    }

                }

                if (moveFrame == right)
                {

                    walkSide = false;

                }*/

                if (moveFrame >= walkFrames[0].Count)
                {

                    moveFrame = 1;

                    //walkSide = true;

                }

            }

        }

        public virtual void DashAscension()
        {

            if (dashPeak == 0)
            {

                return;

            }

            float distance = Vector2.Distance(pathFrom, destination*64);

            float length = distance / 2;

            float lengthSq = (length * length);

            float heightFr = 4 * dashPeak;

            float coefficient = lengthSq / heightFr;

            int midpoint = (int)(pathTotal / 2);

            float newHeight = 0;

            if (pathProgress != midpoint)
            {
                float newLength;

                if (pathProgress < midpoint)
                {

                    newLength = length * (midpoint - pathProgress) / midpoint;

                }
                else
                {

                    newLength = (length * (pathProgress - midpoint) / midpoint);

                }

                float newLengthSq = newLength * newLength;

                float coefficientFr = (4 * coefficient);

                newHeight = newLengthSq / coefficientFr;

            }

            dashHeight = dashPeak - (int)newHeight;

        }

        public virtual void UpdateSpecial()
        {

            if (specialTimer > 0)
            {

                specialTimer--;

            }

            if (netSpecialActive.Value)
            {

                if (specialTimer <= 0)
                {

                    ClearSpecial();

                }
                else if (specialTimer % specialInterval == 0)
                {

                    specialFrame++;

                    if(specialFrame > specialCeiling)
                    {

                        specialFrame = specialFloor;

                    }

                }

            }

            if (netWorkActive.Value)
            {
                
                if (specialTimer <= 0)
                {

                    ClearSpecial();

                }

                PerformWork();

            }

        }

        public void UpdateRoam()
        {

            roamLapse = Game1.currentGameTime.TotalGameTime.TotalMinutes + 1.0;

            roamIndex++;

            if (roamIndex == roamVectors.Count)
            {
                
                roamVectors.Clear();

                roamIndex = 0;

                roamLapse = Game1.currentGameTime.TotalGameTime.TotalMinutes + 1.0;

                roamVectors = RoamAnalysis();

                return;

            }

            tether = roamVectors[roamIndex];

        }

        public virtual float MoveSpeed(float distance = 0, bool useFrames = false)
        {

            float useSpeed = gait;

            float useFrame = moveInterval;

            switch (pathActive)
            {

                case pathing.running:

                    useFrame -= 3;

                    useSpeed = gait * 4f;

                    break;

                case pathing.monster:

                    useSpeed = gait * 2f;

                    useFrame -= 2;

                    break;

                case pathing.scene:

                    
                    if (distance > 640)
                    {

                        useFrame -= 3;

                        useSpeed = gait * 3f;

                    }
                    else if (distance > 360)
                    {

                        useFrame -= 2;

                        useSpeed = gait * 2.25f;

                    }
                    else
                    {
                        useFrame -= 1;

                        useSpeed = gait * 1.5f;

                    }

                    break;

                case pathing.player:

                    if(modeActive == mode.track)
                    {

                        distance = Vector2.Distance(Position, Mod.instance.trackers[characterType].followPlayer.Position);

                    }
                    else
                    {

                        distance = Vector2.Distance(Position, Game1.player.Position);

                    }

                    if (distance > 512)
                    {

                        

                        if (netDashActive.Value || netSmashActive.Value)
                        {

                            useSpeed = gait * 2f;

                        }
                        else
                        {

                            useFrame -= 4;
                            useSpeed = gait * 4f;

                        }

                    }
                    else if (distance > 256)
                    {

                        if (netDashActive.Value || netSmashActive.Value)
                        {

                            useSpeed = gait * 1.5f;

                        }
                        else
                        {

                            useFrame -= 2;
                            useSpeed = gait * 3f;

                        }

                    }
                    else
                    {

                        useSpeed = gait;

                    }

                    break;

                case pathing.random:

                    break;

                case pathing.roam:

                    if (distance > 360)
                    {

                        useFrame -= 2;

                        useSpeed *= 1.5f;

                    }

                    break;

                case pathing.none:
                    
                    break;

            }

            if (netDashActive.Value || netSmashActive.Value)
            {

                useSpeed *= 2.5f;

            }

            return useFrames ? useFrame : useSpeed;

        }

        // ========================================
        // MOVEMENT

        public bool PathTarget(Vector2 target, int ability, int proximity, int direction = -1)
        {

            Vector2 center = ModUtility.PositionToTile(target);

            if(center == occupied && proximity == 0)
            {

                return true;

            }

            if (direction == -1)
            {

                // direction from target (center) to origin (occupied), will search for tiles in between

                direction = ModUtility.DirectionToTarget(target, Position)[2]; // uses 64

            }

            Dictionary<Vector2, int> paths = ModUtility.TraversalToTarget(currentLocation, occupied, center, ability, proximity, direction); // uses tiles

            if (paths.Count > 0)
            {

                destination = paths.Keys.Last();

                traversal = paths;

                LookAtTarget(destination * 64, false); // uses 64

                return true;

            }

            return false;

        }

        public bool PathTrack()
        {

            // check if can walk / jump to player

            if (PathTarget(Mod.instance.trackers[characterType].followPlayer.Position, 1, 2))
            {

                // dont need the tracked path now

                Mod.instance.trackers[characterType].nodes.Clear();

                return true;

            }

            Dictionary<Vector2, int> paths = Mod.instance.trackers[characterType].NodesToTraversal();

            if(paths.Count > 0)
            {

                // walk / jump to the start of the path

                if (PathTarget(paths.Keys.First()*64, 1, 0))
                {

                    // add remaining path segments to traversal

                    if(paths.Count > 1)
                    {
                        
                        for (int p = paths.Count - 2; p >= 0; p--)
                        {

                            traversal.Append(paths.ElementAt(p));

                        }

                    }

                }
                {
                    
                    // might have to warp to the start of the path

                    paths[paths.ElementAt(0).Key] = 2;

                    traversal = paths;

                }

                destination = traversal.Keys.Last();

                return true;

            }

            return false;

        }

        public virtual void SettleOccupied()
        {

            occupied = ModUtility.PositionToTile(Position);//new Vector2((int)(Position.X / 64), (int)(Position.Y / 64));

        }

        public virtual void Traverse()
        {

            if (destination == Vector2.Zero || netHaltActive.Value || netSceneActive.Value)
            {

                if (stationaryTimer > 0)
                {

                    stationaryTimer--;

                    if (stationaryTimer <= 0)
                    {

                        StopMoving();

                    }

                }

                SettlePosition();

                return;

            }

            if (ArrivedDestination())
            {

                return;

            }

            KeyValuePair<Vector2,int> target = traversal.First();

            if (target.Value == 2)
            {

                Mod.instance.iconData.AnimateQuickWarp(currentLocation, Position, true);

                Position = target.Key * 64;

                Mod.instance.iconData.AnimateQuickWarp(currentLocation, Position);

                occupied = target.Key;

                traversal.Remove(target.Key);

            }
            else
            {

                if(target.Value == 1 && !netDashActive.Value)
                {

                    SetDash(target.Key * 64);

                }

                if(netDashActive.Value || netSmashActive.Value)
                {

                    if(pathTotal <= 0)
                    {

                        SetDash(target.Key * 64, netSmashActive.Value);

                    }

                    Position += pathIncrement;

                    pathProgress--;

                    if(pathProgress <= 0)
                    {

                        occupied = target.Key;

                        traversal.Remove(target.Key);

                    }

                }
                else
                {

                    if (!netSweepActive.Value)
                    {

                        LookAtTarget(target.Key * 64, false);

                    };

                    float speed = MoveSpeed(Vector2.Distance(Position, target.Key * 64));

                    Position = ModUtility.PathMovement(Position, target.Key * 64, speed);

                    float remain = Vector2.Distance(Position, target.Key * 64);

                    if (remain <= 4f)
                    {

                        occupied = target.Key;

                        traversal.Remove(target.Key);

                    }

                }

            }

            ArrivedDestination();

        }

        public virtual bool ArrivedDestination()
        {

            if (occupied == destination || traversal.Count == 0)
            {

                ClearMove();

                return true;

            }

            return false;

        }

        public virtual void SettlePosition()
        {

            // Settle position slowly shifts the character towards the set occupied tile
            // This is because Position can be offset by the floating coordinates obtained from traversal
            // and the occupied tile position might not match up
            /*
            Vector2 occupation = occupied * 64;

            if (Position != occupation)
            {

                if(Vector2.Distance(Position,occupation) >= 32f)
                {

                    LookAtTarget(occupation, false);

                }

                Position = ModUtility.PathMovement(Position, occupation, 2);

            }*/

        }

        public virtual bool TightPosition()
        {

            if (destination != Vector2.Zero)
            {

                return false;

            }

            if (Position / 64 == new Vector2((int)(Position.X/64), (int)(Position.Y / 64)))
            {

                return true;

            }

            return false;

        }

        public virtual bool CollideCharacters(Vector2 tile)
        {
            //------------- Collision check

            collideTimer = 180;

            foreach(Farmer farmer in currentLocation.farmers)
            {

                if(farmer.Tile == tile)
                {

                    return false;

                }

            }

            foreach (NPC NPChar in currentLocation.characters)
            {

                if (NPChar is StardewDruid.Character.Actor || /*NPChar is StardewDruid.Character.Dragon ||*/ NPChar == this || NPChar is StardewValley.Monsters.Monster)
                {

                    continue;

                }

                if (NPChar is StardewDruid.Character.Character Buddy)
                {

                    Vector2 check = Buddy.destination != Vector2.Zero ? Buddy.destination : Buddy.occupied;

                    if(tile == check)
                    {

                        if (Buddy.collidePriority > collidePriority)
                        {

                            return true;

                        }

                    }

                }
                else if(!NPChar.isMoving() && NPChar.Tile == tile)
                {

                    return true;

                }

            }

            return false;

        }

        // ========================================
        // ADJUST MODE

        public virtual void WarpToEntrance()
        {

            ResetActives();

            Vector2 warppoint = new Vector2(-1);

            if(modeActive == mode.track)
            {

                Mod.instance.trackers[characterType].WarpToPlayer();

                return;

            }

            if (currentLocation is MineShaft)
            {

                warppoint = WarpData.WarpXZone(currentLocation);

                if (warppoint != Vector2.Zero)
                {

                    for(int i = 0; i < 5; i++)
                    {

                        if(i == 4)
                        {

                            CharacterHandle.CharacterWarp(this, CharacterHandle.CharacterHome(characterType));

                            return;

                        }

                        if (ModUtility.GroundCheck(currentLocation, new Vector2((int)(warppoint.X / 64), (int)(warppoint.Y / 64)),true) == "ground")
                        {

                            break;

                        }

                        warppoint += new Vector2(0, 64);

                    }

                    Mod.instance.iconData.AnimateQuickWarp(currentLocation, Position, true);

                    Position = warppoint;

                    SettleOccupied();

                    Mod.instance.iconData.AnimateQuickWarp(currentLocation, Position);

                    return;

                }

            }
            else
            {

                warppoint = WarpData.WarpEntrance(currentLocation, Position);

                if (warppoint != Vector2.Zero)
                {

                    int centerDirection = ModUtility.DirectionToCenter(currentLocation, Position)[2];

                    Vector2 centerMovement = ModUtility.DirectionAsVector(centerDirection) * 64;

                    for (int i = 0; i < 5; i++)
                    {

                        if (i == 4)
                        {

                            CharacterHandle.CharacterWarp(this, CharacterHandle.CharacterHome(characterType));

                            return;

                        }

                        Vector2 warppointTile = new Vector2((int)(warppoint.X / 64), (int)(warppoint.Y / 64));

                        string groundCheck = ModUtility.GroundCheck(currentLocation, warppointTile, true);

                        if (groundCheck == "ground")
                        {
                            
                            break;

                        }

                        warppoint += centerMovement;

                    }

                    Mod.instance.iconData.AnimateQuickWarp(currentLocation, Position, true);

                    Position = warppoint;

                    SettleOccupied();

                    Mod.instance.iconData.AnimateQuickWarp(currentLocation, Position);

                    return;

                }

            }

            CharacterHandle.CharacterWarp(this, CharacterHandle.CharacterHome(characterType));

            return;

        }

        public virtual void SwitchToMode(mode modechoice, Farmer player)
        {

            ResetActives();

            ResetTimers();

            RemoveCompanionBuff(player);

            netSceneActive.Set(false);

            netStandbyActive.Set(false);

            Mod.instance.trackers.Remove(characterType);

            switch (modechoice)
            {

                case mode.home:

                    modeActive = mode.random;

                    CharacterHandle.CharacterWarp(this, CharacterHandle.CharacterHome(characterType), true);

                    tether = CharacterHandle.RoamTether(currentLocation);

                    break;

                case mode.random:

                    modeActive = mode.random;

                    break;

                case mode.track:

                    Mod.instance.trackers[characterType] =  new TrackHandle(characterType, player);

                    modeActive = mode.track;

                    CompanionBuff(player);

                    break;

                case mode.scene:

                    modeActive = mode.scene;

                    netSceneActive.Set(true);

                    //netStandbyActive.Set(true);

                    break;

                case mode.roam:

                    CharacterHandle.CharacterWarp(this, CharacterHandle.locations.farm, true);

                    roamVectors.Clear();

                    roamIndex = 0;

                    roamLapse = Game1.currentGameTime.TotalGameTime.TotalMinutes + 1.0;

                    roamVectors = RoamAnalysis();
                    
                    modeActive = mode.roam;

                    tether = CharacterHandle.RoamTether(currentLocation);

                    break;

            }

        }

        public virtual void CompanionBuff(Farmer player)
        {

        }

        public virtual void RemoveCompanionBuff(Farmer player)
        {


        }

        public virtual List<Vector2> RoamAnalysis()
        {
            
            int layerWidth = currentLocation.map.Layers[0].LayerWidth;
            
            int layerHeight = currentLocation.map.Layers[0].LayerHeight;
            
            int num = layerWidth / 8;

            int midWidth = (layerWidth / 2) * 64;
            
            int fifthWidth = (layerWidth / 5) * 64;
            
            int nextWidth = (layerWidth * 64) - fifthWidth;
            
            int midHeight = (layerHeight / 2) * 64;
            
            int fifthHeight = (layerHeight / 5) * 64;
            
            int nextHeight = (layerHeight * 64) - fifthHeight;

            List<Vector2> roamList = new()
            {
                new(midWidth,midHeight),
                new(midWidth,midHeight),
                new(midWidth,midHeight),
                new(midWidth,midHeight),
                new(nextWidth,nextHeight),
                new(nextWidth,fifthHeight),
                new(fifthWidth,nextHeight),
                new(fifthWidth,fifthHeight),

            };

            if(currentLocation.IsOutdoors)
            {
                roamList = new()
                {
                    new(midWidth, midHeight),
                    new(midWidth, midHeight),
                    new(midWidth, midHeight),
                    new(midWidth, midHeight),
                    new(nextWidth, nextHeight),
                    new(nextWidth, fifthHeight),
                    new(fifthWidth, nextHeight),
                    new(fifthWidth, fifthHeight),
                    new(-1f),
                    new(-1f),
                    new(-1f),
                    new(-1f),
                };

            }

            List<Vector2> randomList = new();

            Random random = new Random();

            for(int i = 0; i < 12; i++)
            {

                if(roamList.Count == 0)
                {
                    
                    break;

                }

                int j = random.Next(roamList.Count);

                Vector2 randomVector = roamList[j];

                randomList.Add(randomVector);

                roamList.Remove(randomVector);

            }

            return randomList;

        }

        public virtual void NewDay()
        {



        }

    }

}
