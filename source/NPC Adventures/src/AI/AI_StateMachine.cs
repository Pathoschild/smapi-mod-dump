/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/PurrplingMod
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using NpcAdventure.AI.Controller;
using NpcAdventure.HUD;
using NpcAdventure.Loader;
using NpcAdventure.StateMachine;
using NpcAdventure.Utils;
using PurrplingCore.Internal;
using PurrplingCore.Timing;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NpcAdventure.AI
{
    /// <summary>
    /// State machine for companion AI
    /// </summary>
    internal partial class  AI_StateMachine : IUpdateable, IDrawable
    {
        public enum State
        {
            FOLLOW,
            FIGHT,
            IDLE,
            FORAGE,
            SPIRITUAL,
            FISH
        }

        private const string FORAGING_COOLDOWN = "foragingCooldown";
        private const string FISHING_COOLDOWN = "fishingCooldown";
        private const string CHANGE_STATE_COOLDOWN = "changeStateCooldown";
        private const string SCARED_COOLDOWN = "scaredCooldown";
        public readonly NPC npc;
        public readonly Farmer farmer;

        public bool IsLovedMonster(Monster monster)
        {
            if (!this.Csm.HasSkill("spiritual"))
                return false;

            return this.controllers[State.SPIRITUAL] is LovePeaceController lovePeaceController 
                && lovePeaceController.IsLovedMonster(monster);
        }

        private readonly CompanionDisplay hud;
        private readonly IModEvents events;
        internal IMonitor Monitor { get; private set; }

        private readonly IContentLoader loader;
        private readonly Countdown cooldown = new Countdown();
        private Dictionary<State, IController> controllers;

        internal AI_StateMachine(CompanionStateMachine csm, CompanionDisplay hud, IModEvents events, IMonitor monitor)
        {
            this.events = events ?? throw new ArgumentException(nameof(events));
            this.Monitor = monitor ?? throw new ArgumentNullException(nameof(monitor));
            this.Csm = csm ?? throw new ArgumentNullException(nameof(csm));
            this.hud = hud ?? throw new ArgumentNullException(nameof(hud));
            this.loader = csm.ContentLoader;
            this.npc = csm.Companion;
            this.farmer = csm.CompanionManager.Farmer;
        }

        public State CurrentState { get; private set; }
        internal IController CurrentController { get => this.controllers[this.CurrentState]; }

        internal CompanionStateMachine Csm { get; }
        public IContentLoader ContentLoader => this.Csm.ContentLoader;

        public event EventHandler<EventArgsLocationChanged> LocationChanged;

        /// <summary>
        /// Setup AI state machine
        /// </summary>
        public void Setup()
        {
            this.controllers = new Dictionary<State, IController>()
            {
                [State.FOLLOW] = new FollowController(this),
                [State.FIGHT] = new FightController(this, this.loader, this.events, this.Csm.Metadata.Sword),
                [State.IDLE] = new IdleController(this, this.loader),
                [State.FORAGE] = new ForageController(this, this.events),
                [State.SPIRITUAL] = new LovePeaceController(this, this.events),
                [State.FISH] = new FishController(this, this.events),
            };

            // By default AI following the player
            this.ChangeState(State.FOLLOW);

            // Do not forage immediatelly after recruit
            this.cooldown.Set(FORAGING_COOLDOWN, 500);

            this.events.GameLoop.TimeChanged += this.GameLoop_TimeChanged;
        }

        private void GameLoop_TimeChanged(object sender, TimeChangedEventArgs e)
        {
            this.lifeSaved = false;
        }

        public bool PerformAction()
        {
            if (this.Csm.HasSkill("doctor") && (this.farmer.health < this.farmer.maxHealth / 3) && this.healCooldown == 0 && this.medkits != -1)
            {
                this.TryHealFarmer();
                return true;
            }

            if (this.Csm.HasSkill("forager")
                && this.controllers[State.FORAGE] is ForageController fc
                && fc.HasAnyForage()
                && fc.GiveForageTo(this.farmer))
            {
                Game1.drawDialogue(this.npc, this.Csm.Dialogues.GetFriendSpecificDialogueText(this.farmer, "giveForages"));
                return true;
            }

            if (this.Csm.HasSkill("fisherman")
                && this.controllers[State.FISH] is FishController fishController
                && fishController.HasAnyFish()
                && fishController.GiveFishesTo(this.farmer))
            {
                Game1.drawDialogue(this.npc, this.Csm.Dialogues.GetFriendSpecificDialogueText(this.farmer, "giveFishes"));
                return true;
            }

            return false;
        }

        public void ChangeState(State state)
        {
            this.Monitor.Log($"AI changes state {this.CurrentState} -> {state}");

            if (this.CurrentController != null)
            {
                this.CurrentController.Deactivate();
            }

            this.CurrentState = state;
            this.CurrentController.Activate();
            this.hud.SetCompanionState(state);
        }

        private bool IsThereAnyMonster(float distance)
        {
            Monster monster = Helper.GetNearestMonsterToCharacter(this.npc, distance);

            return monster != null && Helper.IsValidMonster(monster);
        }

        public bool IsFarmerNear(float maxDistance = 11f)
        {
            return Helper.Distance(this.farmer.getTileLocationPoint(), this.npc.getTileLocationPoint()) < maxDistance;
        }

        private bool CanForage()
        {
            return this.IsFarmerNear() 
                && this.CurrentState != State.FORAGE
                && !this.cooldown.IsRunning(CHANGE_STATE_COOLDOWN)
                && !this.cooldown.IsRunning(FORAGING_COOLDOWN)
                && this.controllers[State.FORAGE] is ForageController fc 
                && fc.CanForage() 
                && Game1.random.Next(1, 6) == 1;
        }

        private void CheckPotentialStateChange()
        {
            if (this.npc.swimming.Value || this.swimsuit)
            {
                if (this.CurrentState != State.FOLLOW)
                {
                    // Force follow when companion swim and is not in following state
                    this.ChangeState(State.FOLLOW);
                }

                // Do not change state while companion are swimming
                return;
            }

            if (this.Csm.HasSkillsAny("fighter", "warrior")
                && !this.cooldown.IsRunning(CHANGE_STATE_COOLDOWN)
                && this.CurrentState != State.FIGHT
                && this.IsFarmerNear()
                && this.IsThereAnyMonster(
                    this.Csm.HasSkill("warrior") 
                    ? FightController.DEFEND_TILE_RADIUS_WARRIOR 
                    : FightController.DEFEND_TILE_RADIUS))
            {
                this.ChangeState(State.FIGHT);
                this.Monitor.Log("A 50ft monster is here!");
            }

            if (this.Csm.HasSkill("spiritual")
                && !this.cooldown.IsRunning(CHANGE_STATE_COOLDOWN)
                && this.CurrentState != State.SPIRITUAL
                && (this.controllers[State.SPIRITUAL] as LovePeaceController).IsAngryMonstersHere)
            {
                this.ChangeState(State.SPIRITUAL);
            }

            if (this.CurrentState != State.FOLLOW && this.CurrentController.IsIdle)
            {
                this.cooldown.Set(CHANGE_STATE_COOLDOWN, 100);
                this.ChangeState(State.FOLLOW);
            }

            if (this.Csm.HasSkill("forager") && this.FollowOrIdle() && this.CanForage())
            {
                this.cooldown.Set(FORAGING_COOLDOWN, Game1.random.Next(500, 2000));
                this.ChangeState(State.FORAGE);
            }

            if (this.Csm.HasSkill("fisherman") && this.FollowOrIdle() && this.CanFish())
            {
                this.cooldown.Set(FORAGING_COOLDOWN, Game1.random.Next(5000, 20000));
                this.ChangeState(State.FISH);
            }

            if (this.CurrentState == State.FOLLOW && this.CurrentController.IsIdle)
            {
                this.cooldown.Increase(FORAGING_COOLDOWN, Game1.random.Next(300, 700));
                this.ChangeState(State.IDLE);
            }
        }

        private bool CanFish()
        {
            bool isFarmerFishing = this.farmer.UsingTool && this.farmer.CurrentTool is FishingRod;
            return this.IsFarmerNear()
                && this.CurrentState != State.FISH
                && !this.cooldown.IsRunning(CHANGE_STATE_COOLDOWN)
                && !this.cooldown.IsRunning(FISHING_COOLDOWN)
                && !this.farmer.isMoving()
                && this.npc.currentLocation.waterTiles != null
                && this.controllers[State.FISH] is FishController fishController
                && fishController.CanFish()
                && (isFarmerFishing || Game1.random.Next(1, 24) == 1);
        }

        private bool FollowOrIdle()
        {
            return this.CurrentState == State.FOLLOW || this.CurrentState == State.IDLE;
        }

        public void Update(UpdateTickedEventArgs e)
        {
            if (e.IsMultipleOf(15))
            {
                this.DoSideEffects();
                this.CheckPotentialStateChange();
            }

            this.cooldown.Update(e);
            this.CheckSwimming();

            if (this.Csm.HasSkill("doctor"))
                this.UpdateDoctor(e);

            if (this.CurrentController != null)
                this.CurrentController.Update(e);

            foreach (var controller in this.controllers.Values)
                controller.SideUpdate(e);
        }

        /// <summary>
        /// Do side effects (like be scared and etc)
        /// </summary>
        private void DoSideEffects()
        {
            // Be scared
            if (this.Csm.HasSkill("scared")
                && this.IsThereAnyMonster(4f)
                && !this.cooldown.IsRunning(SCARED_COOLDOWN)
                && Game1.random.Next(0, this.GetNotBeScaredChanceIndex()) == 1)
            {
                this.npc.Halt();
                this.npc.shake(1000);
                this.npc.jump();
                this.npc.currentLocation.playSound("batScreech");

                // Scared companion can occassionally cry
                if (!this.npc.IsEmoting && Game1.random.Next(0, 8) == 1)
                {
                    this.npc.doEmote(28);
                    this.farmer.changeFriendship(-1, this.npc);
                }

                this.cooldown.Set(SCARED_COOLDOWN, Game1.random.Next(800, 2400));
                this.cooldown.Set(CHANGE_STATE_COOLDOWN, 100);

                if (this.CurrentState != State.FOLLOW)
                {
                    this.ChangeState(State.FOLLOW);
                }
            }
        }

        private int GetNotBeScaredChanceIndex()
        {
            int notBeScaredChance = 4 + this.farmer.CombatLevel / 2;

            if (this.Csm.HasSkill("warrior"))
            {
                return notBeScaredChance * 2;
            }

            return notBeScaredChance;
        }

        public void ChangeLocation(GameLocation l)
        {
            GameLocation previousLocation = this.npc.currentLocation;
            
            // Warp NPC to player's location at theirs position
            Helper.WarpTo(this.npc, l, this.farmer.getTileLocationPoint());

            this.cooldown.Set(CHANGE_STATE_COOLDOWN, 30);

            // Fire location changed event
            this.OnLocationChanged(previousLocation, this.npc.currentLocation);
        }

        private void OnLocationChanged(GameLocation previous, GameLocation next)
        {
            EventArgsLocationChanged args = new EventArgsLocationChanged()
            {
                PreviousLocation = previous,
                CurrentLocation = next,
            };

            this.LocationChanged?.Invoke(this, args);
        }

        public void Dispose()
        {
            this.events.GameLoop.TimeChanged -= this.GameLoop_TimeChanged;
            this.CurrentController.Deactivate();

            foreach (var controller in this.controllers.OfType<IDisposable>())
                controller.Dispose();

            this.controllers.Clear();
            this.controllers = null;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Context.IsWorldReady && this.CurrentController is IDrawable drawableController)
            {
                drawableController.Draw(spriteBatch);
            }

            if (Context.IsWorldReady && this.Csm.HasSkill("spiritual"))
            {
                (this.controllers[State.SPIRITUAL] as LovePeaceController).DrawLove(spriteBatch);
            }
        }
    }
}
