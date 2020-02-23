using NpcAdventure.Story.Messaging;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;

namespace NpcAdventure.Story
{
    internal class GameMaster : IGameMaster
    {
        private readonly IDataHelper dataHelper;

        public event EventHandler<IGameMasterEventArgs> MessageReceived;

        private List<IScenario> Scenarios { get; set; }

        internal IMonitor Monitor { get; }

        public GameMasterState Data { get; private set; }

        public GameMasterMode Mode { get; private set; }
        public StoryHelper StoryHelper { get; private set; }

        public GameMaster(IModHelper helper, StoryHelper storyHelper, IMonitor monitor)
        {
            this.dataHelper = helper.Data;
            this.StoryHelper = storyHelper;
            this.Monitor = monitor;
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

        private class GameMasterEventArgs : IGameMasterEventArgs
        {
            public IGameMasterMessage Message { get; set; }
            public Farmer Player { get; set; }
            public bool IsLocal { get; set; }
        }
    }
}
