using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using NpcAdventure.Loader;
using NpcAdventure.Model;
using NpcAdventure.Objects;
using NpcAdventure.StateMachine.State;
using NpcAdventure.StateMachine.StateFeatures;
using NpcAdventure.Utils;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;

namespace NpcAdventure.StateMachine
{

    internal class CompanionStateMachine
    {
        /// <summary>
        /// Allowed states in machine
        /// </summary>
        public enum StateFlag
        {
            RESET,
            AVAILABLE,
            RECRUITED,
            UNAVAILABLE,
        }
        public CompanionManager CompanionManager { get; private set; }
        public NPC Companion { get; private set; }
        public CompanionMetaData Metadata { get; }
        public IContentLoader ContentLoader { get; private set; }
        public IMonitor Monitor { get; }
        public Chest Bag { get; private set; }
        public Dictionary<StateFlag, ICompanionState> States { get; private set; }
        private ICompanionState currentState;

        public CompanionStateMachine(CompanionManager manager, NPC companion, CompanionMetaData metadata, IContentLoader loader, IMonitor monitor = null)
        {
            this.CompanionManager = manager;
            this.Companion = companion;
            this.Metadata = metadata;
            this.ContentLoader = loader;
            this.Monitor = monitor;
            this.Bag = new Chest(true);
        }

        /// <summary>
        /// Our companion name (Refers NPC name)
        /// </summary>
        public string Name
        {
            get
            {
                return this.Companion.Name;
            }
        }

        public StateFlag CurrentStateFlag { get; private set; }
        public Dictionary<int, SchedulePathDescription> BackedupSchedule { get; internal set; }
        public bool RecruitedToday { get; private set; }

        /// <summary>
        /// Change companion state machine state
        /// </summary>
        /// <param name="stateFlag">Flag of allowed state</param>
        private void ChangeState(StateFlag stateFlag)
        {
            if (this.States == null)
                throw new InvalidStateException("State machine is not ready! Call setup first.");

            if (!this.States.TryGetValue(stateFlag, out ICompanionState newState))
                throw new InvalidStateException($"Invalid state {stateFlag.ToString()}. Is state machine correctly set up?");

            if (this.currentState == newState)
                return;

            if (this.currentState != null)
            {
                this.currentState.Exit();
            }

            newState.Entry();
            this.currentState = newState;
            this.Monitor.Log($"{this.Name} changed state: {this.CurrentStateFlag.ToString()} -> {stateFlag.ToString()}");
            this.CurrentStateFlag = stateFlag;
        }

        /// <summary>
        /// Setup state handlers
        /// </summary>
        /// <param name="stateHandlers"></param>
        public void Setup(Dictionary<StateFlag, ICompanionState> stateHandlers)
        {
            if (this.States != null)
                throw new InvalidOperationException("State machine is already set up!");

            this.States = stateHandlers;
            this.ResetStateMachine();
        }

        /// <summary>
        /// Companion speaked a dialogue
        /// </summary>
        /// <param name="speakedDialogue"></param>
        public void DialogueSpeaked(Dialogue speakedDialogue)
        {
            // Convert state to dialogue detector (if state implements it)
            IDialogueDetector detector = this.currentState as IDialogueDetector;

            if (detector != null)
            {
                detector.OnDialogueSpeaked(speakedDialogue); // Handle this dialogue
            }
        }

        /// <summary>
        /// Setup companion for new day
        /// </summary>
        public void NewDaySetup()
        {
            if (this.CurrentStateFlag != StateFlag.RESET)
                throw new InvalidStateException($"State machine {this.Name} must be in reset state!");

            // Today is festival day? Player can't recruit this companion
            if (Utility.isFestivalDay(Game1.dayOfMonth, Game1.currentSeason))
            {
                this.Monitor.Log($"{this.Name} is unavailable to recruit due to festival today.");
                this.MakeUnavailable();
                return;
            }

            // Setup dialogues for companion for this day
            DialogueHelper.SetupCompanionDialogues(this.Companion, this.ContentLoader.LoadStrings($"Dialogue/{this.Name}"));

            // Spoused or married with her/him? Enhance dialogues with extra spouse dialogues for this day
            if (Helper.IsSpouseMarriedToFarmer(this.Companion, this.CompanionManager.Farmer) && this.ContentLoader.CanLoad($"Dialogue/{this.Name}Spouse"))
                DialogueHelper.SetupCompanionDialogues(this.Companion, this.ContentLoader.LoadStrings($"Dialogue/{this.Name}Spouse"));

            this.RecruitedToday = false;
            this.MakeAvailable();
        }

        /// <summary>
        /// Dump items from companion's bag to farmer (player) house
        /// </summary>
        public void DumpBagInFarmHouse()
        {
            FarmHouse farm = (FarmHouse)Game1.getLocationFromName("FarmHouse");
            Vector2 place = Utility.PointToVector2(farm.getRandomOpenPointInHouse(Game1.random));
            Package dumpedBag = new Package(this.Bag.items.ToList(), place)
            {
                GivenFrom = this.Name,
                Message = this.ContentLoader.LoadString("Strings/Strings:bagItemsSentLetter", this.CompanionManager.Farmer.Name, this.Companion.displayName)
            };

            farm.objects.Add(place, dumpedBag);
            this.Bag = new Chest(true);

            this.Monitor.Log($"{this.Companion} delivered bag contents into farm house at position {place}");
        }

        /// <summary>
        /// Make companion AVAILABLE to recruit
        /// </summary>
        public void MakeAvailable()
        {
            this.ChangeState(StateFlag.AVAILABLE);
        }

        /// <summary>
        /// Make companion UNAVAILABLE to recruit
        /// </summary>
        public void MakeUnavailable()
        {
            this.ChangeState(StateFlag.UNAVAILABLE);
        }

        /// <summary>
        /// Reset companion's state machine
        /// </summary>
        public void ResetStateMachine()
        {
            this.ChangeState(StateFlag.RESET);
        }

        /// <summary>
        /// Dismiss recruited companion
        /// </summary>
        /// <param name="keepUnavailableOthers">Keep other companions unavailable?</param>
        internal void Dismiss(bool keepUnavailableOthers = false)
        {
            this.ResetStateMachine();

            if (this.currentState is ICompanionIntegrator integrator)
                integrator.ReintegrateCompanionNPC();

            this.BackedupSchedule = null;
            this.ChangeState(StateFlag.UNAVAILABLE);
            this.CompanionManager.CompanionDissmised(keepUnavailableOthers);
        }

        /// <summary>
        /// Recruit this companion
        /// </summary>
        public void Recruit()
        {
            this.BackedupSchedule = this.Companion.Schedule;
            this.RecruitedToday = true;

            this.ChangeState(StateFlag.RECRUITED);
            this.CompanionManager.CompanionRecuited(this.Companion.Name);
        }

        public void Dispose()
        {
            if (this.currentState != null)
                this.currentState.Exit();

            this.States.Clear();
            this.States = null;
            this.currentState = null;
            this.Companion = null;
            this.CompanionManager = null;
            this.ContentLoader = null;
        }

        /// <summary>
        /// Resolve dialogue request
        /// </summary>
        public void ResolveDialogueRequest()
        {
            // Can this companion to resolve player's dialogue request?
            if (!this.CanDialogueRequestResolve())
                return;

            // Handle dialogue request resolution in current machine state
            (this.currentState as IRequestedDialogueCreator).CreateRequestedDialogue();
        }

        /// <summary>
        /// Can request a dialogue for this companion in current state?
        /// </summary>
        /// <returns>True if dialogue request can be resolved</returns>
        public bool CanDialogueRequestResolve()
        {
            return this.currentState is IRequestedDialogueCreator dcreator && dcreator.CanCreateDialogue;
        }
    }

    class InvalidStateException : Exception
    {
        public InvalidStateException(string message) : base(message)
        {
        }
    }
}
