using System;
using System.Collections.Generic;
using System.Linq;
using NpcAdventure.StateMachine;
using StardewValley;
using StardewModdingAPI;
using NpcAdventure.Driver;
using NpcAdventure.Loader;
using StardewModdingAPI.Events;
using NpcAdventure.Utils;
using NpcAdventure.StateMachine.State;
using static NpcAdventure.StateMachine.CompanionStateMachine;
using NpcAdventure.Model;
using Microsoft.Xna.Framework.Graphics;
using NpcAdventure.Events;

namespace NpcAdventure
{
    internal class CompanionManager
    {
        private readonly DialogueDriver dialogueDriver;
        private readonly HintDriver hintDriver;
        private readonly IMonitor monitor;
        public Dictionary<string, CompanionStateMachine> PossibleCompanions { get; }
        public Config Config { get; }

        public Farmer Farmer
        {
            get
            {
                if (Context.IsWorldReady)
                    return Game1.player;
                return null;
            }
        }

        public CompanionManager(DialogueDriver dialogueDriver, HintDriver hintDriver, Config config, IMonitor monitor)
        {
            this.dialogueDriver = dialogueDriver ?? throw new ArgumentNullException(nameof(dialogueDriver));
            this.hintDriver = hintDriver ?? throw new ArgumentNullException(nameof(hintDriver));
            this.monitor = monitor ?? throw new ArgumentNullException(nameof(monitor));
            this.PossibleCompanions = new Dictionary<string, CompanionStateMachine>();
            this.Config = config;

            this.dialogueDriver.DialogueRequested += this.DialogueDriver_DialogueRequested;
            this.dialogueDriver.DialogueChanged += this.DialogueDriver_DialogueChanged;
            this.hintDriver.CheckHint += this.HintDriver_CheckHint;
        }

        /// <summary>
        /// Dialogue event handler. 
        /// Works with previous dialogue when dialogue has been changed
        /// </summary>
        /// <param name="sender">Who sent it?</param>
        /// <param name="e">Changed dialogue event arguments</param>
        private void DialogueDriver_DialogueChanged(object sender, DialogueChangedArgs e)
        {
            NPC n = e.PreviousDialogue?.speaker; // Who said previous dialogue?

            // No previous dialogue? Forget it.
            if (e.PreviousDialogue == null || n == null)
                return;

            // Check if spoken it any our companion
            if (this.PossibleCompanions.TryGetValue(n.Name, out CompanionStateMachine csm))
            {
                csm.DialogueSpeaked(e.PreviousDialogue); // Companion can react on this dialogue
            }
        }

        /// <summary>
        /// Handle check hint event from event driver
        /// </summary>
        /// <param name="sender">Who sent this event?</param>
        /// <param name="e">Hint checker event arguments</param>
        private void HintDriver_CheckHint(object sender, CheckHintArgs e)
        {
            if (e.Npc == null)
                return;

            // Show dialogue icon hint (NPC can speak bubble) if:
            // - cursor is on our companion 
            // - and they can resolve our dialogue requests 
            // - and has'nt any dialogues in queue 
            // - and we can ask this companion for following (recruit)
            if (this.PossibleCompanions.TryGetValue(e.Npc.Name, out CompanionStateMachine csm)
                && csm.Name == e.Npc?.Name
                && csm.CanDialogueRequestResolve()
                && e.Npc.CurrentDialogue.Count == 0
                && Helper.CanRequestDialog(this.Farmer, e.Npc))
            {
                this.hintDriver.ShowHint(HintDriver.Hint.DIALOGUE);
            }
        }

        /// <summary>
        /// Handle requested dialogue event
        /// </summary>
        /// <param name="sender">Who sent this event?</param>
        /// <param name="e">Dialogue event arguments</param>
        private void DialogueDriver_DialogueRequested(object sender, DialogueRequestArgs e)
        {
            if (this.PossibleCompanions.TryGetValue(e.WithWhom.Name, out CompanionStateMachine csm) && csm.Name == e.WithWhom.Name)
            {
                csm.ResolveDialogueRequest();
            }
        }

        /// <summary>
        /// When any companion was recruited
        /// </summary>
        /// <param name="companionName">NPC name of companion</param>
        internal void CompanionRecuited(string companionName)
        {
            foreach (var csmKv in this.PossibleCompanions)
            {
                // All other companions are unavailable now (Player can't recruit them right now)
                if (csmKv.Value.Name != companionName)
                    csmKv.Value.MakeUnavailable();
            }

            this.monitor.Log($"You are recruited {companionName} companion.");
        }

        /// <summary>
        /// Reset all state machines of companions
        /// </summary>
        public void ResetStateMachines()
        {
            foreach (var companionKv in this.PossibleCompanions)
                companionKv.Value.ResetStateMachine();
        }

        /// <summary>
        /// Setup all companions for new day
        /// </summary>
        public void NewDaySetup()
        {
            try
            {
                // For each companion state machine call new day setup method
                foreach (var companionKv in this.PossibleCompanions)
                    companionKv.Value.NewDaySetup();

                this.monitor.Log("Companions are successfully setup for new day!", LogLevel.Info);
            }
            catch (InvalidStateException e)
            {
                this.monitor.Log($"Error while trying to setup new day: {e.Message}");
            }
        }

        /// <summary>
        /// Dump player's items from companion's bag
        /// </summary>
        public void DumpCompanionNonEmptyBags()
        {
            foreach (var csm in this.PossibleCompanions)
                if (!csm.Value.Bag.isEmpty())
                    csm.Value.DumpBagInFarmHouse();
        }

        /// <summary>
        /// When any companion dissmised (relieved from duty, unfollow player)
        /// </summary>
        /// <param name="keepUnavailable">Set this companion unavailable after dismiss?</param>
        internal void CompanionDissmised(bool keepUnavailable = false)
        {
            foreach (var csmKv in this.PossibleCompanions)
            {
                if (keepUnavailable)
                    csmKv.Value.MakeUnavailable();
                else if (!csmKv.Value.RecruitedToday)
                    csmKv.Value.MakeAvailable();
            }
        }

        /// <summary>
        /// Companion initializator. Call it after saved game is loaded
        /// </summary>
        /// <param name="loader"></param>
        /// <param name="gameEvents"></param>
        /// <param name="reflection"></param>
        public void InitializeCompanions(IContentLoader loader, IModEvents gameEvents, ISpecialModEvents specialEvents, IReflectionHelper reflection)
        {
            Dictionary<string, string> dispositions = loader.Load<Dictionary<string, string>>("Data/CompanionDispositions");

            foreach (string npcName in dispositions.Keys)
            {
                NPC companion = Game1.getCharacterFromName(npcName, true);

                if (companion == null)
                    throw new Exception($"Can't find NPC with name '{npcName}'");

                CompanionStateMachine csm = new CompanionStateMachine(this, companion, new CompanionMetaData(dispositions[npcName]), loader, reflection, this.monitor);
                Dictionary<StateFlag, ICompanionState> stateHandlers = new Dictionary<StateFlag, ICompanionState>()
                {
                    [StateFlag.RESET] = new ResetState(csm, gameEvents, this.monitor),
                    [StateFlag.AVAILABLE] = new AvailableState(csm, gameEvents, this.monitor),
                    [StateFlag.RECRUITED] = new RecruitedState(csm, gameEvents, specialEvents, this.monitor),
                    [StateFlag.UNAVAILABLE] = new UnavailableState(csm, gameEvents, this.monitor),
                };

                csm.Setup(stateHandlers);
                this.PossibleCompanions.Add(npcName, csm);
            }

            this.monitor.Log($"Initalized {this.PossibleCompanions.Count} companions.", LogLevel.Info);
        }

        /// <summary>
        /// Companion uninitalizer. Call it after game exited (return to title)
        /// </summary>
        public void UninitializeCompanions()
        {
            foreach (var companionKv in this.PossibleCompanions)
            {
                companionKv.Value.Dispose();
                this.monitor.Log($"{companionKv.Key} disposed!");
            }

            this.PossibleCompanions.Clear();
            this.monitor.Log("Companions uninitialized", LogLevel.Info);
        }
    }
}
