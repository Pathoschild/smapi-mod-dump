/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/PurrplingMod
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using ExpandedPreconditionsUtility;
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

    internal class CompanionStateMachine : IDisposable
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
        private readonly bool isVillager;

        public CompanionStateMachine(CompanionManager manager, NPC companion, CompanionMetaData metadata, IContentLoader loader, IReflectionHelper reflection, IMonitor monitor = null)
        {
            this.isVillager = false;
            this.Name = companion?.Name;
            this.CompanionManager = manager;
            this.Companion = companion;
            this.Metadata = metadata;
            this.ContentLoader = loader;
            this.Monitor = monitor;
            this.Bag = new Chest();
            this.Reflection = reflection;
            this.SpokenDialogues = new HashSet<string>();
            this.Dialogues = new DialogueProvider(this, loader);
        }

        public CompanionStateMachine(string name, CompanionManager manager, CompanionMetaData metadata, IContentLoader loader, IReflectionHelper reflection, IMonitor monitor = null) : this(manager, null, metadata, loader, reflection, monitor)
        {
            this.Name = name;
            this.isVillager = true;
            this.ReloadNpc();
        }

        /// <summary>
        /// Our companion name (Refers NPC name)
        /// </summary>
        public string Name { get; private set; }

        public StateFlag CurrentStateFlag { get; private set; }
        public Dictionary<int, SchedulePathDescription> BackedupSchedule { get; internal set; }
        public bool BackedUpIgnoreScheduleToday { get; internal set; }
        public bool RecruitedToday { get; private set; }
        public bool SuggestedToday { get; internal set; }
        public bool CanSuggestToday { get; private set; }
        public HashSet<string> SpokenDialogues { get; private set; }
        public DialogueProvider Dialogues { get; }

        public void ReloadNpc()
        {
            if (!this.isVillager)
                return;

            var npcDispositions = Game1.content.Load<Dictionary<string, string>>(@"Data\NPCDispositions");

            if (!npcDispositions.ContainsKey(this.Name))
            {
                this.Monitor?.Log($"Unable to initialize companion `{this.Name}`, because this NPC cannot be found in the game. " +
                        "Are you trying to add a custom NPC as a companion? Check the mod which adds this NPC into the game. " +
                        "Don't report this as a bug to NPC Adventures unless it's a vanilla game NPC.", LogLevel.Error);
                return;
            }

            this.Companion = Game1.getCharacterFromName(this.Name);
        }

        /// <summary>
        /// Change companion state machine state
        /// </summary>
        /// <param name="stateFlag">Flag of allowed state</param>
        private void ChangeState(StateFlag stateFlag)
        {
            if (this.States == null)
                throw new InvalidStateException($"State machine for companion '{this.Name}' is not ready! Call setup first.");

            if (!this.States.TryGetValue(stateFlag, out ICompanionState newState))
                throw new InvalidStateException($"Invalid state {stateFlag} for companion '{this.Name}'. Is state machine correctly set up?");

            if (this.currentState == newState)
                return;

            if (this.currentState != null)
            {
                this.currentState.Exit();
            }

            newState.Entry();
            this.currentState = newState;
            this.Monitor.Log($"{this.Name} changed state: {this.CurrentStateFlag} -> {stateFlag}");
            this.CurrentStateFlag = stateFlag;
        }

        public ICompanionState GetCurrentStateBehavior()
        {
            return this.currentState;
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
        /// <param name="spokenDialogue"></param>
        public void OnDialogueSpoken(Dialogue spokenDialogue)
        {
            // Convert state to dialogue detector (if state implements it)
            if (this.currentState is IDialogueDetector detector)
            {
                detector.OnDialogueSpoken(spokenDialogue); // Handle this dialogue
            }

            if (spokenDialogue is CompanionDialogue companionDialogue && companionDialogue.SpecialAttributes.Contains("session"))
            {
                // Remember session spoken dialogue this day (forget morning)
                this.SpokenDialogues.Add(companionDialogue.Tag);
            }
        }

        /// <summary>
        /// Setup companion for new day
        /// </summary>
        public void SetupForNewDay()
        {
            if (this.CurrentStateFlag != StateFlag.RESET)
                throw new InvalidStateException($"State machine {this.Name} must be in reset state!");

            // Today is festival day? Player can't recruit this companion
            if (!this.CanBeAvailableToday())
            {
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
        /// Check if the companion state can be set to AVAILABLE state
        /// </summary>
        /// <returns></returns>
        private bool CanBeAvailableToday()
        {
            if (Utility.isFestivalDay(Game1.dayOfMonth, Game1.currentSeason))
            {
                this.Monitor.Log($"{this.Name} can't be available today for recruit due to festival day.");
                return false; 
            }

            if (!string.IsNullOrEmpty(this.Metadata.Availability))
            {
                bool canBeAvailable = this.CompanionManager
                    .EPU
                    .CheckConditions(this.Metadata.Availability.Replace(';', '/'));

                if (!canBeAvailable)
                    this.Monitor.Log($"Availability condition `{this.Metadata.Availability}` is not met for '{this.Name}'. Companion is unavailable for recruit.");

                return canBeAvailable;
            }

            return true;
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
        /// Kills state machine current state behavior 
        /// and forces empty behavior with RESET state flag.
        /// </summary>
        public void Kill()
        {
            var state = this.currentState;

            this.currentState = null;
            this.CurrentStateFlag = StateFlag.RESET;

            state?.Exit();
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
            this.currentState?.Exit();
            this.currentState = null;

            foreach (var state in this.States.Values.OfType<IDisposable>())
            {
                state.Dispose();
            }

            this.States.Clear();
            this.States = null;
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

    abstract class CompanionStateException : Exception
    {
        public CompanionStateException(string message) : base(message)
        {
        }
    }

    class InvalidStateException : CompanionStateException
    {
        public InvalidStateException(string message) : base(message)
        {
        }
    }

    class TransitionStateException : CompanionStateException
    {
        public TransitionStateException(string message) : base(message)
        {
        }
    }
}
