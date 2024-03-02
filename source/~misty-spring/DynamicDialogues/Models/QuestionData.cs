/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

// ReSharper disable ClassNeverInstantiated.Global
namespace DynamicDialogues.Models;

///<summary>Questions added by player, which might lead to events or quests.</summary>
internal class QuestionData
{
    public string Question { get; }
    public string Answer { get; }
    public int MaxTimesAsked { get; } //0 meaning forever avaiable
    public string Location { get; } = "any"; //if avaiable only in a specific location
    public int From { get; } = 610; //from this hour
    public int To { get; } = 2550; //until this hour
    public string QuestToStart { get; }
    public PlayerConditions Conditions { get; } = new();
    public string TriggerAction { get; }

    public QuestionData()
    {
    }

    public QuestionData(QuestionData q)
    {
        Question = q.Question;
        Answer = q.Answer;

        MaxTimesAsked = q.MaxTimesAsked;
        Location = q.Location;
        
        Conditions = q.Conditions;
        TriggerAction = q.TriggerAction;

        From = q.From;
        To = q.To;

        QuestToStart = q.QuestToStart;
    }
}
