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
using StardewValley;
using System;

namespace NpcAdventure.Story
{
    public interface IGameMaster
    {
        GameMasterMode Mode { get; }
        GameMasterState Data { get; }
        IGameMasterEvents Events { get; }

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