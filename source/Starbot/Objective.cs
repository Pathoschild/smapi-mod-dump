/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/bwdymods/SDV-Starbot
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starbot
{
    public abstract class Objective
    {
        //once this returns true, the bot will look for a new objective, or sleep if there aren't any.
        public virtual bool IsComplete { get; set; }

        //if a task fails 3 times it will be removed from the day's agenda
        public virtual int FailureCount { get; set; }

        public abstract string AnnounceMessage { get; }
        public abstract string UniquePoolId { get; }
        public abstract bool Cooperative { get; }

        public virtual void Fail()
        {
            FailureCount++;
            Reset();
        }

        public virtual void Reset()
        {

        }

        //called whenever the bot is "bored" until IsComplete is true
        public virtual void Step()
        {

        }

        public virtual void CantMoveUpdate() {

        }
    }
}
