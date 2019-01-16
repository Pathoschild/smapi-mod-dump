using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI.Events;
using Microsoft.Xna.Framework;

namespace FollowerNPC.AI_States
{
    class AI_StateIdle: AI_State
    {
        private Character me;
        private Character leader;
        private AI_StateMachine stateMachine;
        private Random r;
        private float followRadius;
        private bool hasPublicAnimations;
        private string[] privateLocations;

        private List<FarmerSprite.AnimationFrame>[] idleAnimations;
        private List<FarmerSprite.AnimationFrame>[] otherAnimations;
        private bool[] idleAnimationLoops;
        private bool[] idleAnimationsShy;
        private Dialogue idleDialogue;
        private bool idleDialogueSeen;
        private int animationRestartTimer;

        public AI_StateIdle(Character me, Character leader, AI_StateMachine machine)
        {
            this.me = me;
            this.leader = leader;
            this.stateMachine = machine;
            this.r = new Random((int)Game1.uniqueIDForThisGame + (int)Game1.stats.DaysPlayed + Game1.timeOfDay);
            this.followRadius = (7.5f * Game1.tileSize) * (7.5f * Game1.tileSize);
            privateLocations = new string[16] { "FarmHouse", "Farm", "Saloon", "Beach",
                "Mountain", "Forest", "BusStop", "Desert", "ArchaeologyHouse", "Woods",
                "Railroad", "Summit", "CommunityCenter", "Greenhouse", "Backwoods", "BeachNightMarket"
            };
            switch (me.Name)
            {
                case "Abigail":
                    idleAnimations = new List<FarmerSprite.AnimationFrame>[2];
                    idleAnimations[0] = new List<FarmerSprite.AnimationFrame>()
                    {
                        new FarmerSprite.AnimationFrame(16, 400),
                        new FarmerSprite.AnimationFrame(17, 400),
                        new FarmerSprite.AnimationFrame(18, 400),
                        new FarmerSprite.AnimationFrame(19, 400)
                    };
                    AnimatedSprite.endOfAnimationBehavior abigailSittingLoop =
                        new AnimatedSprite.endOfAnimationBehavior(AbigailSittingLoop);
                    idleAnimations[1] = new List<FarmerSprite.AnimationFrame>()
                    {
                        new FarmerSprite.AnimationFrame(26, 1000),
                        new FarmerSprite.AnimationFrame(27, 1000, false, false, abigailSittingLoop, true)
                    };
                    idleAnimationLoops = new bool[2] {true, false};
                    idleAnimationsShy = new bool[2] {true, true };
                    hasPublicAnimations = false;
                    otherAnimations = new List<FarmerSprite.AnimationFrame>[1];
                    otherAnimations[0] = new List<FarmerSprite.AnimationFrame>()
                    {
                        new FarmerSprite.AnimationFrame(27,1000)
                    };
                    break;
                case "Alex":
                    idleAnimations = new List<FarmerSprite.AnimationFrame>[2];
                    idleAnimations[0] = new List<FarmerSprite.AnimationFrame>()
                    {
                        new FarmerSprite.AnimationFrame(16, 100),
                        new FarmerSprite.AnimationFrame(17, 100),
                        new FarmerSprite.AnimationFrame(18, 100),
                        new FarmerSprite.AnimationFrame(19, 100),
                        new FarmerSprite.AnimationFrame(20, 100),
                        new FarmerSprite.AnimationFrame(21, 100),
                        new FarmerSprite.AnimationFrame(22, 100),
                        new FarmerSprite.AnimationFrame(23, 4000)
                    };
                    AnimatedSprite.endOfAnimationBehavior alexSittingLoop =
                        new AnimatedSprite.endOfAnimationBehavior(AlexSittingLoop);
                    idleAnimations[1] = new List<FarmerSprite.AnimationFrame>()
                    {
                        new FarmerSprite.AnimationFrame(36, 100),
                        new FarmerSprite.AnimationFrame(38, 100),
                        new FarmerSprite.AnimationFrame(35, 250, false, false, alexSittingLoop, true)
                    };
                    idleAnimationLoops = new bool[2] { true, false };
                    idleAnimationsShy = new bool[2] { true, true };
                    hasPublicAnimations = false;
                    otherAnimations = new List<FarmerSprite.AnimationFrame>[1];
                    otherAnimations[0] = new List<FarmerSprite.AnimationFrame>()
                    {
                        new FarmerSprite.AnimationFrame(35,1000)
                    };
                    break;
                case "Elliott":
                    idleAnimations = new List<FarmerSprite.AnimationFrame>[1];
                    AnimatedSprite.endOfAnimationBehavior elliottReadingLoop =
                        new AnimatedSprite.endOfAnimationBehavior(ElliottReadingLoop);
                    idleAnimations[0] = new List<FarmerSprite.AnimationFrame>()
                    {
                        new FarmerSprite.AnimationFrame(32, 100),
                        new FarmerSprite.AnimationFrame(33, 100),
                        new FarmerSprite.AnimationFrame(34, 100, false, false, ElliottReadingLoop, true)
                    };
                    idleAnimationLoops = new bool[1] { false };
                    idleAnimationsShy = new bool[1] { false };
                    hasPublicAnimations = true;
                    otherAnimations = new List<FarmerSprite.AnimationFrame>[1];
                    otherAnimations[0] = new List<FarmerSprite.AnimationFrame>()
                    {
                        new FarmerSprite.AnimationFrame(34,1000)
                    };
                    break;
                case "Emily":
                    idleAnimations = new List<FarmerSprite.AnimationFrame>[2];
                    idleAnimations[0] = new List<FarmerSprite.AnimationFrame>()
                    {
                        new FarmerSprite.AnimationFrame(24, 2000),
                        new FarmerSprite.AnimationFrame(25, 2000)
                    };
                    idleAnimations[1] = new List<FarmerSprite.AnimationFrame>()
                    {
                        new FarmerSprite.AnimationFrame(18, 300),
                        new FarmerSprite.AnimationFrame(19, 300),
                        new FarmerSprite.AnimationFrame(18, 300),
                        new FarmerSprite.AnimationFrame(19, 300),
                        new FarmerSprite.AnimationFrame(18, 300),

                        new FarmerSprite.AnimationFrame(22, 300),
                        new FarmerSprite.AnimationFrame(23, 300),
                        new FarmerSprite.AnimationFrame(22, 300),
                        new FarmerSprite.AnimationFrame(23, 300),
                        new FarmerSprite.AnimationFrame(22, 300),

                        new FarmerSprite.AnimationFrame(16, 300),
                        new FarmerSprite.AnimationFrame(17, 300),
                        new FarmerSprite.AnimationFrame(16, 300),
                        new FarmerSprite.AnimationFrame(17, 300),
                        new FarmerSprite.AnimationFrame(16, 300),

                        new FarmerSprite.AnimationFrame(20, 300),
                        new FarmerSprite.AnimationFrame(21, 300),
                        new FarmerSprite.AnimationFrame(20, 300),
                        new FarmerSprite.AnimationFrame(21, 300),
                        new FarmerSprite.AnimationFrame(20, 300)
                    };
                    idleAnimationLoops = new bool[2] { true, true };
                    idleAnimationsShy = new bool[2] { true, true };
                    hasPublicAnimations = false;
                    break;
                case "Haley":
                    idleAnimations = new List<FarmerSprite.AnimationFrame>[2];
                    AnimatedSprite.endOfAnimationBehavior haleyCameraFlip =
                        new AnimatedSprite.endOfAnimationBehavior(HaleyCameraFlip);
                    idleAnimations[0] = new List<FarmerSprite.AnimationFrame>()
                    {
                        new FarmerSprite.AnimationFrame(30, 1500),
                        new FarmerSprite.AnimationFrame(31, 100),
                        new FarmerSprite.AnimationFrame(24, 1000),
                        new FarmerSprite.AnimationFrame(31, 100),
                        new FarmerSprite.AnimationFrame(30, 1499),
                        new FarmerSprite.AnimationFrame(30, 1, false, false, haleyCameraFlip, true),
                    };
                    idleAnimations[1] = new List<FarmerSprite.AnimationFrame>()
                    {
                        new FarmerSprite.AnimationFrame(33, 1500),
                        new FarmerSprite.AnimationFrame(32, 100),
                        new FarmerSprite.AnimationFrame(25, 1000),
                        new FarmerSprite.AnimationFrame(32, 100),
                        new FarmerSprite.AnimationFrame(33, 1499),
                        new FarmerSprite.AnimationFrame(33, 1, false, false, haleyCameraFlip, true)
                    };
                    idleAnimationLoops = new bool[2] { true, true };
                    idleAnimationsShy = new bool[2] { false, false };
                    hasPublicAnimations = true;
                    otherAnimations = new List<FarmerSprite.AnimationFrame>[1];
                    otherAnimations[0] = new List<FarmerSprite.AnimationFrame>()
                    {
                        new FarmerSprite.AnimationFrame(33, 1500, false, true, null, false),
                        new FarmerSprite.AnimationFrame(32, 100, false, true, null, false),
                        new FarmerSprite.AnimationFrame(25, 1000, false, true, null, false),
                        new FarmerSprite.AnimationFrame(32, 100, false, true, null, true),
                        new FarmerSprite.AnimationFrame(33, 1499, false, true, null, true),
                        new FarmerSprite.AnimationFrame(33, 1, false, true, haleyCameraFlip, true)
                    };
                    break;
                case "Harvey":
                    idleAnimations = new List<FarmerSprite.AnimationFrame>[2];
                    idleAnimations[0] = new List<FarmerSprite.AnimationFrame>()
                    {
                        new FarmerSprite.AnimationFrame(42, 2000),
                        new FarmerSprite.AnimationFrame(43, 6000)
                    };
                    idleAnimations[1] = new List<FarmerSprite.AnimationFrame>()
                    {
                        new FarmerSprite.AnimationFrame(36, 5000),
                        new FarmerSprite.AnimationFrame(37, 100),
                        new FarmerSprite.AnimationFrame(38, 800),
                        new FarmerSprite.AnimationFrame(37, 100)
                    };
                    idleAnimationLoops = new bool[2] { true, true };
                    idleAnimationsShy = new bool[2] { false, false };
                    hasPublicAnimations = true;
                    break;
                case "Leah":
                    idleAnimations = new List<FarmerSprite.AnimationFrame>[1];
                    idleAnimations[0] = new List<FarmerSprite.AnimationFrame>()
                    {
                        new FarmerSprite.AnimationFrame(32, 1000),
                        new FarmerSprite.AnimationFrame(33, 1000),
                        new FarmerSprite.AnimationFrame(34, 1000),
                        new FarmerSprite.AnimationFrame(33, 1000),
                        new FarmerSprite.AnimationFrame(32, 1000),
                        new FarmerSprite.AnimationFrame(35, 3000)
                    };
                    idleAnimationLoops = new bool[1] { true };
                    idleAnimationsShy = new bool[1] { false };
                    hasPublicAnimations = true;
                    break;
                case "Maru":
                    idleAnimations = null;
                    idleAnimationLoops = null;
                    break;
                case "Penny":
                    idleAnimations = new List<FarmerSprite.AnimationFrame>[1];
                    AnimatedSprite.endOfAnimationBehavior pennyReadingLoop =
                        new AnimatedSprite.endOfAnimationBehavior(PennyReadingLoop);
                    idleAnimations[0] = new List<FarmerSprite.AnimationFrame>()
                    {
                        new FarmerSprite.AnimationFrame(16, 100),
                        new FarmerSprite.AnimationFrame(17, 100),
                        new FarmerSprite.AnimationFrame(19, 100),
                        new FarmerSprite.AnimationFrame(18, 1000, false, false, pennyReadingLoop, true)
                    };
                    idleAnimationLoops = new bool[1] { false };
                    idleAnimationsShy = new bool[1] { false };
                    hasPublicAnimations = true;
                    otherAnimations = new List<FarmerSprite.AnimationFrame>[1];
                    otherAnimations[0] = new List<FarmerSprite.AnimationFrame>()
                    {
                        new FarmerSprite.AnimationFrame(18, 1000)
                    };
                    break;
                case "Sam":
                    idleAnimations = new List<FarmerSprite.AnimationFrame>[2];
                    idleAnimations[0] = new List<FarmerSprite.AnimationFrame>()
                    {
                        new FarmerSprite.AnimationFrame(16, 200),
                        new FarmerSprite.AnimationFrame(17, 200),
                        new FarmerSprite.AnimationFrame(18, 200),
                        new FarmerSprite.AnimationFrame(19, 2000)
                    };
                    idleAnimations[1] = new List<FarmerSprite.AnimationFrame>()
                    {
                        new FarmerSprite.AnimationFrame(20, 200),
                        new FarmerSprite.AnimationFrame(21, 200),
                        new FarmerSprite.AnimationFrame(20, 200),
                        new FarmerSprite.AnimationFrame(21, 200),
                        new FarmerSprite.AnimationFrame(20, 200),

                        new FarmerSprite.AnimationFrame(22, 200),
                        new FarmerSprite.AnimationFrame(23, 200),
                        new FarmerSprite.AnimationFrame(22, 200),
                        new FarmerSprite.AnimationFrame(23, 200),
                        new FarmerSprite.AnimationFrame(22, 200),

                        new FarmerSprite.AnimationFrame(20, 200),
                        new FarmerSprite.AnimationFrame(21, 200),
                        new FarmerSprite.AnimationFrame(20, 200),
                        new FarmerSprite.AnimationFrame(21, 200),
                        new FarmerSprite.AnimationFrame(20, 200),

                        new FarmerSprite.AnimationFrame(22, 200, false, true, null, false),
                        new FarmerSprite.AnimationFrame(23, 200, false, true, null, false),
                        new FarmerSprite.AnimationFrame(22, 200, false, true, null, false),
                        new FarmerSprite.AnimationFrame(23, 200, false, true, null, false),
                        new FarmerSprite.AnimationFrame(22, 200, false, true, null, false)
                    };
                    idleAnimationLoops = new bool[2] { true, true };
                    idleAnimationsShy = new bool[2] { false, true };
                    hasPublicAnimations = true;
                    break;
                case "Sebastian":
                    idleAnimations = new List<FarmerSprite.AnimationFrame>[1];
                    AnimatedSprite.endOfAnimationBehavior sebastianStopSmoking =
                        new AnimatedSprite.endOfAnimationBehavior(SebastianStopSmoking);
                    idleAnimations[0] = new List<FarmerSprite.AnimationFrame>()
                    {
                        new FarmerSprite.AnimationFrame(16, 8000),
                        new FarmerSprite.AnimationFrame(17, 100),
                        new FarmerSprite.AnimationFrame(18, 100),
                        new FarmerSprite.AnimationFrame(19, 100),
                        new FarmerSprite.AnimationFrame(20, 100),
                        new FarmerSprite.AnimationFrame(21, 1000),
                        new FarmerSprite.AnimationFrame(22, 100),
                        new FarmerSprite.AnimationFrame(23, 100, false, false, sebastianStopSmoking, true)
                    };
                    idleAnimationLoops = new bool[] { true };
                    idleAnimationsShy = new bool[1] { true };
                    hasPublicAnimations = false;
                    break;
                case "Shane":
                    idleAnimations = new List<FarmerSprite.AnimationFrame>[2];
                    AnimatedSprite.endOfAnimationBehavior shaneStopLooping =
                        new AnimatedSprite.endOfAnimationBehavior(ShaneStopLooping);
                    idleAnimations[0] = new List<FarmerSprite.AnimationFrame>()
                    {
                        new FarmerSprite.AnimationFrame(30, 500),
                        new FarmerSprite.AnimationFrame(31, 1000),
                        new FarmerSprite.AnimationFrame(30, 500),
                        new FarmerSprite.AnimationFrame(31, 1000),
                        new FarmerSprite.AnimationFrame(30, 500),
                        new FarmerSprite.AnimationFrame(31, 1000),
                        new FarmerSprite.AnimationFrame(30, 500),
                        new FarmerSprite.AnimationFrame(31, 1000),
                        new FarmerSprite.AnimationFrame(19, 6000, false, false, shaneStopLooping, true)
                    };
                    idleAnimations[1] = new List<FarmerSprite.AnimationFrame>()
                    {
                        new FarmerSprite.AnimationFrame(20, 4000),
                        new FarmerSprite.AnimationFrame(21, 100),
                        new FarmerSprite.AnimationFrame(22, 100),
                        new FarmerSprite.AnimationFrame(23, 100),
                        new FarmerSprite.AnimationFrame(24, 1200),
                        new FarmerSprite.AnimationFrame(23, 100),
                        new FarmerSprite.AnimationFrame(22, 100),
                        new FarmerSprite.AnimationFrame(21, 100, false, false, shaneStopLooping, true)
                    };
                    idleAnimationLoops = new bool[2] { true, true };
                    idleAnimationsShy = new bool[2] { false, false };
                    hasPublicAnimations = true;
                    break;
            }
        }

        public override void EnterState()
        {
            base.EnterState();
            int i = r.Next(idleAnimationLoops.Length);
            if (!privateLocations.Contains(me.currentLocation.Name) &&
                idleAnimationsShy[i])
            {
                do
                {
                    i = ++i >= idleAnimationLoops.Length ? 0 : i;
                } while (idleAnimationsShy[i]);
            }
            me.Sprite.setCurrentAnimation(idleAnimations[i]);
            me.Sprite.loop = idleAnimationLoops[i];
            if (!idleDialogueSeen)
            {
                Dialogue d = GenerateAnIdleDialogue(i);
                idleDialogue =
                    d ?? throw new Exception(
                        "Tried to push an idle dialogue, but there weren't any for this character!");
                stateMachine.owner.stateMachine.companion.CurrentDialogue.Push(d);
            }
            animationRestartTimer = -1;
            ModEntry.modHelper.Events.Display.MenuChanged += Display_MenuChanged;
        }

        

        public override void ExitState()
        {
            base.ExitState();
            ModEntry.modHelper.Events.Display.MenuChanged -= Display_MenuChanged;
            TryRemoveIdleDialogue();
        }

        public override void Update(UpdateTickedEventArgs e)
        {
            eAI_State potentialNewState = TransitionsCheck();
            if (potentialNewState != eAI_State.nil)
                stateMachine.ChangeState(potentialNewState);

            if (--animationRestartTimer == 0)
            {
                RestartAnimation();
            }
        }

        private eAI_State TransitionsCheck()
        {
            if (CheckIfLeaderLeftFollowRadius())
                return eAI_State.followFarmer;
            return eAI_State.nil;
        }

        private void Display_MenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.OldMenu != null)
            {
                DialogueBox db = (e.OldMenu as DialogueBox);
                if (db != null)
                {
                    Dialogue d = (Dialogue)typeof(DialogueBox).GetField("characterDialogue", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
                    if (d != null && d.speaker != null && 
                        stateMachine.owner != null && d.speaker.Equals(stateMachine.owner.stateMachine.companion))
                    {
                        if (d.Equals(idleDialogue))
                        {
                            idleDialogueSeen = true;
                            idleDialogue = null;
                        }
                    }
                }
            }
        }

        private bool CheckIfLeaderLeftFollowRadius()
        {
            Vector2 i = new Vector2(me.GetBoundingBox().Center.X, me.GetBoundingBox().Center.Y);
            Vector2 l = new Vector2(leader.GetBoundingBox().Center.X, leader.GetBoundingBox().Center.Y);
            return (l - i).LengthSquared() > followRadius;
        }

        public bool CanBeIdleHere()
        {
            if (idleAnimations == null)
                return false;
            NPC n = (me as NPC);
            return (privateLocations.Contains(me.currentLocation.Name) ||
                    (n != null && me.currentLocation.Equals(n.DefaultMap)) ||
                     hasPublicAnimations);
        }

        public Dialogue GenerateAnIdleDialogue(int i)
        {
            Farmer f = stateMachine.owner.stateMachine.manager.farmer;
            NPC c = stateMachine.owner.stateMachine.companion;
            List<string> ret = new List<string>();
            bool repeat = false;

            GetDialogue:
            // If this companion is married to the farmer
            if (f.spouse != null && f.spouse.Equals(c.Name))
            {
                string idleFriendKey = "companion-Idle"+i+"-Friend";
                string idleSpouseKey = "companion-Idle"+i+"-Spouse";
                string idleSpouseOverrideKey = "companion-Idle"+i+"-SpouseOverride";

                // If there are SpouseOverride dialogue(s)
                if (GetAnyDialogueValuesForDialogueKey(idleSpouseOverrideKey, ref ret))
                {
                    return new Dialogue(ret[r.Next(ret.Count)], c);
                }

                // Else, look for Spouse and Friend strings
                else if (GetAnyDialogueValuesForDialogueKey(idleSpouseKey, ref ret) |
                         GetAnyDialogueValuesForDialogueKey(idleFriendKey, ref ret))
                {
                    return new Dialogue(ret[r.Next(ret.Count)], c);
                }
            }
            // Otherwise, if they are just a friend
            else
            {
                string idleFriendKey = "companion-Idle" + (i+1) + "-Friend";

                // Look for Friend strings
                if (GetAnyDialogueValuesForDialogueKey(idleFriendKey, ref ret))
                {
                    return new Dialogue(ret[r.Next(ret.Count)], c);
                }
            }

            if (ret.Count == 0 && !repeat)
            {
                repeat = true;
                foreach (KeyValuePair<string, string> kvp in stateMachine.owner.stateMachine.script)
                    c.Dialogue[kvp.Key] = kvp.Value;
                goto GetDialogue;
            }
            return null;
        }

        protected bool GetAnyDialogueValuesForDialogueKey(string dialogueKey, ref List<string> dialogueValues)
        {
            NPC companion = stateMachine.owner.stateMachine.companion;
            bool ret = false;

            // If there is a, or mulitple, strings
            string multiValue = null;
            string singleValue = null;
            if (companion.Dialogue.TryGetValue(dialogueKey + "1", out multiValue) ||
                companion.Dialogue.TryGetValue(dialogueKey, out singleValue))
                //if (stateMachine.script.TryGetValue(dialogueKey + "1", out multiValue) ||
                //    stateMachine.script.TryGetValue(dialogueKey, out singleValue))
            {
                // If there are multiple strings
                if (multiValue != null)
                {
                    ret = true;
                    dialogueValues.Add(multiValue);
                    int i = 2;
                    while (companion.Dialogue.TryGetValue(dialogueKey + i.ToString(),
                        out multiValue))
                    {
                        i++;
                        dialogueValues.Add(multiValue);
                    }
                }
                // If there is only one string
                else if (singleValue != null)
                {
                    ret = true;
                    dialogueValues.Add(singleValue);
                }
            }
            return ret;
        }

        public bool TryRemoveIdleDialogue()
        {
            if (idleDialogue == null)
                return false;
            Stack<Dialogue> temp = new Stack<Dialogue>(stateMachine.owner.stateMachine.companion.CurrentDialogue.Count);
            Dialogue t;
            bool ret = false;
            while (stateMachine.owner.stateMachine.companion.CurrentDialogue.Count != 0)
            {
                t = stateMachine.owner.stateMachine.companion.CurrentDialogue.Pop();
                if (!t.Equals(idleDialogue))
                    temp.Push(t);
                else
                    ret = true;
            }
            while (temp.Count != 0)
                stateMachine.owner.stateMachine.companion.CurrentDialogue.Push(temp.Pop());
            idleDialogue = null;
            return ret;
        }

        #region NPC Animation Functions

        private void PennyReadingLoop(Farmer leader)
        {
            me.Sprite.setCurrentAnimation(otherAnimations[0]);
            me.Sprite.loop = true;
        }

        private void AbigailSittingLoop(Farmer leader)
        {
            me.Sprite.setCurrentAnimation(otherAnimations[0]);
            me.Sprite.loop = true;
        }

        private void AlexSittingLoop(Farmer leader)
        {
            me.Sprite.setCurrentAnimation(otherAnimations[0]);
            me.Sprite.loop = true;
        }

        private void ElliottReadingLoop(Farmer leader)
        {
            me.Sprite.setCurrentAnimation(otherAnimations[0]);
            me.Sprite.loop = true;
        }

        private void HaleyCameraFlip(Farmer leader)
        {
            int newAnimation = r.Next(3);
            if (newAnimation == 0)
                me.Sprite.setCurrentAnimation(idleAnimations[1]);
            else if (newAnimation == 1)
                me.Sprite.setCurrentAnimation(otherAnimations[0]);
            else
            {
                me.Sprite.setCurrentAnimation(idleAnimations[0]);
            }
        }

        private void SebastianStopSmoking(Farmer leader)
        {
            if (r.Next(3) == 0)
            {
                me.Sprite.loop = false;
                SetAnimationRestartTimer();
            }
        }

        private void ShaneStopLooping(Farmer leader)
        {
            if (r.Next(3) == 0)
            {
                me.Sprite.loop = false;
                SetAnimationRestartTimer();
            }
        }

        private void SetAnimationRestartTimer()
        {
            animationRestartTimer = 60 * r.Next(8, 16);
        }

        private void RestartAnimation()
        {
            int i = r.Next(idleAnimationLoops.Length);
            if (!privateLocations.Contains(me.currentLocation.Name) &&
                idleAnimationsShy[i])
            {
                do
                {
                    i = ++i >= idleAnimationLoops.Length ? 0 : i;
                } while (idleAnimationsShy[i]);
            }
            me.Sprite.setCurrentAnimation(idleAnimations[i]);
            me.Sprite.loop = idleAnimationLoops[i];
            animationRestartTimer = -1;
        }

        #endregion
    }
}
