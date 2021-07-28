/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using StardewValley;
using System;

namespace PlatoTK.Events
{
    internal class QuestionAnsweredEventArgs : IQuestionAnsweredEventArgs
    {
        public Response Answer { get; }

        public string LastQuestionKey { get; }

        internal readonly Action Callback;

        public QuestionAnsweredEventArgs(Response answer, Action callback, string lastQuestionKey)
        {
            Answer = answer;
            Callback = callback;
            LastQuestionKey = lastQuestionKey;
        }

        public void PreventDefault()
        {
            Callback();
        }
    }
}
