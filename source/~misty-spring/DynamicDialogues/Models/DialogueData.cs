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

/// <summary>
/// A class used for Dialogues with special commands (e.g jump, emote etc).
/// </summary>
internal class DialogueData
{
    public int Time { get; } = -1;  //time to add dialogue at, mut. exclusive w/ from-to
    public int From { get; } = 600; //from this hour
    public int To { get; } = 2600; //until this hour
    public string Location { get; } = "any";  //location npc has to be in

    public string Dialogue { get; } //the dialogue
    public bool ClearOnMove { get; } //if to clear dialogue on move
    public bool Override { get; } //if to delete previous dialogues
    public bool Immediate { get; } // will print dialogue right away if NPC is in location
    public bool Force { get; } // if Immediate, prints dialogue regardless of location
    //public bool ApplyWhenMoving { get; set; } = false;

    public bool IsBubble { get; } //show text overhead instead

    public string FaceDirection { get; } //string to change facing to
    public bool Jump { get; } //makes npc jump when addition is placed
    public int Shake { get; } = -1; //shake for x milliseconds
    public int Emote { get; } = -1; //emote int (if allowed)

    public AnimationData Animation { get; } = new(); //animation to play, if any
    public PlayerConditions Conditions { get; } = new();
    public string TriggerAction { get; }

    public DialogueData()
    {
    }

    public DialogueData(DialogueData md)
    {
        Time = md.Time;
        From = md.From;
        To = md.To;
        
        Conditions = md.Conditions;
        TriggerAction = md.TriggerAction;
        
        Location = md.Location;

        Dialogue = md.Dialogue;
        ClearOnMove = md.ClearOnMove;
        Override = md.Override;

        IsBubble = md.IsBubble;
        Jump = md.Jump;
        Shake = md.Shake;
        Emote = md.Emote;
        FaceDirection = md.FaceDirection;

        Immediate = md.Immediate;
        Force = md.Force;
        
        Animation = md.Animation;
    }
}

