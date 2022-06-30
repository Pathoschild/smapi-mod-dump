/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/FarmhouseVisits
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections;
using System.Collections.Generic;

namespace FarmVisitors
{
    //the info for SCHEDULED npcs
    internal class ScheduleData
    {
        public int From { get; set; }
        public int To { get; set; }

        public string EntryBubble { get; set; }
        public string EntryQuestion { get; set; }

        public string EntryDialogue { get; set; }
        public string ExitDialogue { get; set; }

        public List<string> Dialogues { get; set; }

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
        }
    }
    //data temporarily stored about visiting npc
    internal class TempNPC
    {
        public string Name { get; set; }
        public AnimatedSprite Sprite { get; set; }
        public string AnimationMessage { get; set; }
        public string CurrentLocation { get; set; }
        public Vector2 Position { get; set; }
        public int Facing { get; set; }
        public Stack<Dialogue> CurrentPreVisit { get; set; }
        public Dictionary<string, string> AllPreVisit { get; }

        public TempNPC()
        {

        }

        public TempNPC(TempNPC n)
        {
            Name = n.Name;

            Sprite = n.Sprite;
            AnimationMessage = n.AnimationMessage;

            CurrentLocation = n.CurrentLocation;
            Position = n.Position;
            Facing = n.Facing;

            CurrentPreVisit = n.CurrentPreVisit;
            AllPreVisit = n.AllPreVisit;
        }

        public TempNPC(NPC visit)
        {
            Name = visit.Name;
            
            Sprite = visit.Sprite;
            AnimationMessage = visit.endOfRouteMessage.Value;

            CurrentLocation = visit.currentLocation.Name;
            Position = visit.Position;
            Facing = visit.facingDirection.Value;
            
            CurrentPreVisit = visit.CurrentDialogue;
            AllPreVisit = visit.Dialogue;
        }
    }
}