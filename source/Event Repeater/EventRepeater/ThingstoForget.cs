/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MissCoriel/Event-Repeater
**
*************************************************/

using System;
using System.Collections.Generic;

//whatever other using stuff. is cool
namespace EventRepeater
{
    public class ThingsToForget
    {
        public List<int> RepeatEvents { get; set; }
        public List<string> RepeatMail { get; set; }
        public List<int> RepeatResponse { get; set; }

        public ThingsToForget()
        {
            RepeatEvents = new List<int>();
            RepeatMail = new List<string>();
            RepeatResponse = new List<int>();
            
        }
    }
}