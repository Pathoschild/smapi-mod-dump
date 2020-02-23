using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Quests;
using System;
using System.Collections.Generic;

namespace NpcAdventure.Events
{
    public interface ISpecialModEvents
    {
        event EventHandler<ILocationRenderedEventArgs> RenderedLocation;
        event EventHandler<IMailEventArgs> MailboxOpen;
        event EventHandler<IQuestCompletedArgs> QuestCompleted;
        event EventHandler<IQuestReloadObjectiveArgs> ReloadObjective;
    }

    public interface IQuestReloadObjectiveArgs
    {
        Quest Quest { get; }
    }

    public interface IQuestCompletedArgs
    {
        Quest Quest { get; }
    }

    public interface ILocationRenderedEventArgs
    {
        SpriteBatch SpriteBatch { get; }
    }

    public interface IMailEventArgs
    {
        string FullLetterKey { get; }
        string LetterKey { get; }
        IList<string> Mailbox { get; }
        Farmer Player { get; }
    }
}
