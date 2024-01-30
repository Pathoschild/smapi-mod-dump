/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using System.Collections.Generic;

namespace FarmVisitors.Datamodels;

/// <summary>
/// Information for scheduled NPCs.
/// </summary>
internal class ScheduleData
{
    public int From { get; set; }
    public int To { get; set; }
    public bool MustBeExact { get; set; }

    public string EntryBubble { get; set; }
    public string EntryQuestion { get; set; }

    public string EntryDialogue { get; set; }
    public string ExitDialogue { get; set; }

    public List<string> Dialogues { get; set; }

    public ExtraBehavior Extras {get;set;} = new();

    public ScheduleData()
    {
    }

    public ScheduleData(ScheduleData sd)
    {
        From = sd.From;
        To = sd.To;

        EntryBubble = sd.EntryBubble;
        EntryQuestion = sd.EntryQuestion;

        EntryDialogue = sd.EntryDialogue;
        ExitDialogue = sd.ExitDialogue;

        Dialogues = sd.Dialogues;

        Extras = sd.Extras;
    }
}
//data temporarily stored about visiting npc

internal class ExtraBehavior
{
    public bool Force { get; set; }
    public string Mail { get; set; }
    public string GameStateQuery { get; set; }
    //public bool ShouldSleepOver { get; set; } //in the future, add opt for them sleeping over if time matches, & custom dialogue for it

    public ExtraBehavior()
    {
    }

    public ExtraBehavior(ExtraBehavior b)
    {
        Force = b.Force;
        Mail = b.Mail;
        GameStateQuery = b.GameStateQuery;
        //ShouldSleepOver = b.ShouldSleepOver;
    }

}