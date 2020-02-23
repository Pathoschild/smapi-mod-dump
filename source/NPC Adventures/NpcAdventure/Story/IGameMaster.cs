using NpcAdventure.Story.Messaging;
using StardewValley;
using System;

namespace NpcAdventure.Story
{
    public interface IGameMaster
    {
        GameMasterMode Mode { get; }
        GameMasterState Data { get; }
        event EventHandler<IGameMasterEventArgs> MessageReceived;
        void RegisterScenario(IScenario scenario);
        void SyncData();
        void SendEventMessage(IGameMasterMessage message);
    }

    public interface IGameMasterEventArgs
    {
        IGameMasterMessage Message { get; }
        Farmer Player { get; }
        bool IsLocal { get; }
    }

    public enum GameMasterMode
    {
        OFFLINE,
        MASTER,
        SLAVE,
    };
}