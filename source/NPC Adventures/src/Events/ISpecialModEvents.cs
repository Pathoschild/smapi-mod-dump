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
