/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/DynamicDialogues
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using System;

namespace DynamicDialogues
{
    /*
     * impl. ideas:
     * - face towards farmer
     * - wait to stop moving before applying dialogue
     *      (smth like :
     *          ApplyWhenMoving defaulting to true,
     *          or WaitToStopMoving which defaults to false
     *      )
     * I may (or may not) add these in the future
     */

    /// <summary>
    /// A user-friendly class for the framework, all values are strings.
    /// </summary>
    internal class RawDialogues
    {
        public int Time { get; set; } = -1;  //time to add dialogue at
        public string Location { get; set; } = "any";  //location npc has to be in

        public string Dialogue { get; set; } = null;  //the dialogue
        public bool ClearOnMove { get; set; } = false;  //if to clear dialogue on move
        public bool Override { get; set; } = false;  //if to delete previous dialogues
        public bool Immediate { get; set; } = false;  // will print dialogue right away if NPC is in location
        public bool Force { get; set; } = false;  // if Immediate, prints dialogue regardless of location

        public bool IsBubble { get; set; } = false; //showtextoverhead instead
        
        public string FaceDirection { get; set; } //string to change facing to
        public bool Jump { get; set; } = false; //makes npc jump when addition is placed
        public int Shake { get; set; } = -1; //shake for x milliseconds
        public int Emote { get; set; } = -1; //emote int (if allowed)

        public RawDialogues()
        {
        }

        public RawDialogues(RawDialogues md)
        {
            Time = md.Time;
            Location = md.Location;

            Dialogue = md.Dialogue;
            ClearOnMove = md.ClearOnMove;
            Override = md.Override;

            IsBubble = md.IsBubble;
            Jump = md.Jump;
            Shake = md.Shake;
            Emote = md.Emote;
            FaceDirection = md.FaceDirection;
        }
#if DEBUG
        public RawDialogues(string word)
        {
            Time = (-1 * word.Length);
            Location = $"{word} location";

            Dialogue = $"{word} dialogue";
            ClearOnMove = false;
            Override = false;

            IsBubble = false;
            Jump = false;
            Shake = (1 * word.Length);
            Emote = (1 * word.Length);
            FaceDirection = word;
        }
#endif
    }
    internal class RawNotifs
    {
        public int Time { get; set; } = -1; //time to show at
        public string Location  { get; set; } = "any"; //the location to show at
        public string Message { get; set; } //msg to display
        public string Sound { get; set; } //sound to make
        //(Maybe?) string Icon { get; set; } = "; //icon
        //public int FadeOut { get; set; } = -1; //fadeout is auto set by game
        
        public bool IsBox { get; set; } = false; //if box instead

        public RawNotifs()
        {
        }

        public RawNotifs(RawNotifs rn)
        {
            Time = rn.Time;
            Location = rn.Location;

            Message = rn.Message;
            Sound = rn.Sound;

            IsBox = rn.IsBox;
        }
    }

    internal class RawQuestions
    {
        public string Question { get; set; }
        public string Answer { get; set; }
        public int MaxTimesAsked { get; set; } = 0; //0 meaning forever avaiable
        public string Location { get; set; } = "any"; //if avaiable only in a specific location
        public int From { get; set; } = 610; //from this hour
        public int To { get; set; } = 2550; //until this hour

        public RawQuestions()
        {
        }

        public RawQuestions(RawQuestions q)
        {
            Question = q.Question;
            Answer = q.Answer;

            MaxTimesAsked = q.MaxTimesAsked;
            Location = q.Location;
            
            From = q.From;
            To = q.To;
        }
    }
}