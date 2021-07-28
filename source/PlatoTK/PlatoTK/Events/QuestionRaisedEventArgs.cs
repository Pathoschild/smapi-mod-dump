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
using System.Collections.Generic;

namespace PlatoTK.Events
{
    internal class QuestionRaisedEventArgs : IQuestionRaisedEventArgs
    {
        internal Action Callback { get; }
        public string Question { get; }
        public string LastQuestionKey => Game1.currentLocation.lastQuestionKey;

        public bool IsTV { get; }
        
        public List<Response> Choices { get; }

        internal readonly Action<string> SetQuestionCallback;
        internal readonly Action<Response> AddResponseCallback;
        internal readonly Action<Response> RemoveResponseCallback;

        public QuestionRaisedEventArgs(string question, List<Response> choices, Action<string> setQuestion, Action<Response> addResponse, Action<Response> removeResponse, bool isTV, Action callback)
        {
            Question = question;
            SetQuestionCallback = setQuestion;
            AddResponseCallback = addResponse;
            RemoveResponseCallback = removeResponse;
            Choices = choices;
            IsTV = isTV;
            Callback = callback;
        }

        public void RemoveResponse(Response respone)
        {
            RemoveResponseCallback(respone);
        }

        public void AddResponse(Response response)
        {
            AddResponseCallback(response);
        }

        public void SetQuestion(string question)
        {
            SetQuestionCallback(question);
        }

        public void PaginateResponses()
        {
            Callback();
        }
    }
}
