using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using NpcAdventure.Dialogues;
using NpcAdventure.Loader;
using NpcAdventure.Model;
using NpcAdventure.Objects;
using NpcAdventure.StateMachine.StateFeatures;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
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
        private IMonitor Monitor { get; }
        public Chest Bag { get; private set; }
        public IReflectionHelper Reflection { get; }
        public Dictionary<StateFlag, ICompanionState> States { get; private set; }
        private ICompanionState currentState;

        public CompanionStateMachine(CompanionManager manager, NPC companion, CompanionMetaData metadata, IContentLoader loader, IReflectionHelper reflection, IMonitor monitor = null)
        {
            this.CompanionManager = manager;
            this.Companion = companion;
            this.Metadata = metadata;
            this.ContentLoader = loader;
            this.Monitor = monitor;
            this.Bag = new Chest();
            this.Reflection = reflection;
            this.SpokenDialogues = new HashSet<string>();
            this.Dialogues = new DialogueProvider(companion, loader);
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
        public bool BackedUpIgnoreScheduleToday { get; internal set; }
        public bool RecruitedToday { get; private set; }
        public bool SuggestedToday { get; internal set; }
        public bool CanSuggestToday { get; private set; }
        public HashSet<string> SpokenDialogues { get; private set; }
        public DialogueProvider Dialogues { get; }

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
            if (this.currentState is IDialogueDetector detector)
            {
                detector.OnDialogueSpoken(speakedDialogue); // Handle this dialogue
            }

            if (speakedDialogue is CompanionDialogue companionDialogue && companionDialogue.SpecialAttributes.Contains("session"))
            {
                // Remember session spoken dialogue this day (forget morning)
                this.SpokenDialogues.Add(companionDialogue.Tag);
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
            this.Dialogues.SetupForNewDay();

            this.RecruitedToday = false;
            this.SuggestedToday = false;
            this.CanSuggestToday = Game1.random.NextDouble() > .5f
                && !(this.Companion.isMarried() && SDate.Now().DayOfWeek == DayOfWeek.Monday);
            this.SpokenDialogues.Clear();
            this.MakeAvailable();
            if (this.CanSuggestToday)
                this.Monitor.Log($"{this.Name} can suggest adventure today!");
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
                Message = this.ContentLoader.LoadString($"Strings/Mail:bagItemsSentLetter.{this.Companion.Name}", this.CompanionManager.Farmer.Name, this.Companion.displayName)
            };

            farm.objects.Add(place, dumpedBag);
            this.Bag = new Chest();

            this.Monitor.Log($"{this.Companion} delivered bag contents into farm house at position {place}");
        }

        public Dialogue GenerateLocationDialogue(GameLocation location, string suffix = "")
        {
            NPC companion = this.Companion;

            // Try generate only once spoken dialogue in game save
            if (this.Dialogues.GenerateStaticDialogue(location, $"companionOnce{suffix}") is CompanionDialogue dialogueOnce
                && !this.CompanionManager.Farmer.hasOrWillReceiveMail(dialogueOnce.Tag))
            {
                // Remember only once spoken dialogue (as received mail. Keep it forever in loaded game after game saved)
                dialogueOnce.Remember = true;
                return dialogueOnce;
            }

            // Try generate standard companion various dialogue. This dialogue can be shown only once per day (per recruited companion session)
            if (this.Dialogues.GenerateDialogue(location, $"companion{suffix}") is CompanionDialogue dialogue && !this.SpokenDialogues.Contains(dialogue.Tag))
            {
                dialogue.SpecialAttributes.Add("session"); // Remember this dialogue in this companion session when will be spoken
                return dialogue;
            }

            // Try generate dialogue which can be shown repeately in day (current companion session). If none defined, returns null
            return this.Dialogues.GenerateDialogue(location, "companionRepeat");
        }

        /// <summary>
        /// Does companion have this skill?
        /// </summary>
        /// <param name="skill">Which skill</param>
        /// <returns>True if companion has this skill, otherwise False</returns>
        public bool HasSkill(string skill)
        {
            return this.Metadata.PersonalSkills.Contains(skill);
        }

        /// <summary>
        /// Does companion have all of these skills?
        /// </summary>
        /// <param name="skills">Which skills</param>
        /// <returns>True if companion has all of them, otherwise False</returns>
        public bool HasSkills(params string[] skills)
        {
            foreach (string skill in skills)
            {
                if (!this.HasSkill(skill))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Does companion have any of these skills?
        /// </summary>
        /// <param name="skills">Which skills</param>
        /// <returns>True if companion have any of these skills, otherwise False</returns>
        public bool HasSkillsAny(params string[] skills)
        {
            foreach (string skill in skills)
            {
                if (this.HasSkill(skill))
                    return true;
            }

            return false;
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
            this.BackedUpIgnoreScheduleToday = this.Companion.ignoreScheduleToday;
            this.RecruitedToday = true;

            // If standing on unpassable tile (like chair, couch or bench), set position to heading passable tile location
            if (!this.Companion.currentLocation.isTilePassable(this.Companion.GetBoundingBox(), Game1.viewport))
            {
                this.Companion.setTileLocation(this.Companion.GetGrabTile());
            }

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
        public bool CheckAction(Farmer who, GameLocation location)
        {
            // Can this companion to resolve player's dialogue request?
            if (!this.CanPerformAction())
                return false;

            // Handle dialogue request resolution in current machine state
            return (this.currentState as IActionPerformer).PerformAction(who, location);
        }

        /// <summary>
        /// Can request a dialogue for this companion in current state?
        /// </summary>
        /// <returns>True if dialogue request can be resolved</returns>
        public bool CanPerformAction()
        {
            return this.currentState is IActionPerformer dcreator && dcreator.CanPerformAction;
        }
    }

    class InvalidStateException : Exception
    {
        public InvalidStateException(string message) : base(message)
        {
        }
    }
}
