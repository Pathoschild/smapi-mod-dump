/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/PurrplingMod
**
*************************************************/

using NpcAdventure.Story.Messaging;
using QuestFramework.Api;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;

namespace NpcAdventure.Story
{
    internal class GameMaster : IGameMaster
    {
        private readonly IDataHelper dataHelper;

        private List<IScenario> Scenarios { get; set; }

        internal IMonitor Monitor { get; }
        internal GameMasterEvents.EventManager EventManager { get; }

        public IGameMasterEvents Events { get; }
        public GameMasterState Data { get; private set; }
        public GameMasterMode Mode { get; private set; }
        public StoryHelper StoryHelper { get; private set; }

        public event EventHandler<IGameMasterEventArgs> MessageReceived;

        public GameMaster(IModHelper helper, StoryHelper storyHelper, IMonitor monitor)
        {
            this.dataHelper = helper.Data;
            this.StoryHelper = storyHelper;
            this.Monitor = monitor;
            this.EventManager = new GameMasterEvents.EventManager();
            this.Events = new GameMasterEvents(this.EventManager);
            this.Scenarios = new List<IScenario>();
        }

        internal void Initialize()
        {
            if (Context.IsMainPlayer)
            {
                this.Data = this.dataHelper.ReadSaveData<GameMasterState>("story") ?? new GameMasterState();
            }
            // TODO: Write logic for client. Must fetch GM data from server

            foreach (var scenario in this.Scenarios)
            {
                scenario.Initialize();
            }

            this.Mode = Context.IsMainPlayer ? GameMasterMode.MASTER : GameMasterMode.SLAVE;
            this.Monitor.Log($"Game master initialized in mode: {this.Mode.ToString()}", LogLevel.Info);
            this.Monitor.Log($"Player is eligible to recruit companions: {(this.Data.GetPlayerState().isEligible ? "Yes" : "No")}", LogLevel.Info);
        }

        public void RegisterScenario(IScenario scenario)
        {
            scenario.GameMaster = this;
            this.Scenarios.Add(scenario);
        }

        internal void Uninitialize()
        {
            if (this.Mode == GameMasterMode.OFFLINE)
                return;

            foreach (var scenario in this.Scenarios)
            {
                scenario.Dispose();
            }

            this.Mode = GameMasterMode.OFFLINE;
            this.Monitor.Log("Game master uninitialized!", LogLevel.Info);
        }

        internal void SaveData()
        {
            if (this.Mode == GameMasterMode.MASTER)
                this.dataHelper.WriteSaveData("story", this.Data);
        }

        public void SyncData()
        {
            if (!Context.IsMultiplayer || this.Mode == GameMasterMode.OFFLINE)
                return; // Nothing to sync in singleplayer game or game master is not initialized

            if (this.Mode == GameMasterMode.MASTER)
            {
                // TODO: Write broadcast sync logic Server -> Clients here
            } 

            if (this.Mode == GameMasterMode.SLAVE)
            {
                // TODO: Write sync logic Client -> Server here
            }
        }

        /// <summary>
        /// Send event message to game master's listeners (like scenarios)
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public void SendEventMessage(IGameMasterMessage message)
        {
            if (this.Mode == GameMasterMode.OFFLINE)
                return;

            if (this.Mode == GameMasterMode.MASTER)
            {
                this.MessageReceived?.Invoke(this, new GameMasterEventArgs()
                    {
                        Message = message,
                        Player = Game1.player,
                        IsLocal = true,
                    }
                );

                if (Context.IsMultiplayer)
                {
                    // TODO: Write logic to send message to clients through net
                }
            }

            if (this.Mode == GameMasterMode.SLAVE)
            {
                // TODO: Write logic to send it to server through net
            }
        }

        internal void CheckForEvents(GameLocation location, Farmer player)
        {
            this.EventManager.CheckEvent.Fire(new GameMasterEvents.CheckEventEventArgs(location, player), this);
        }

        private class GameMasterEventArgs : IGameMasterEventArgs
        {
            public IGameMasterMessage Message { get; set; }
            public Farmer Player { get; set; }
            public bool IsLocal { get; set; }
        }
    }
}
