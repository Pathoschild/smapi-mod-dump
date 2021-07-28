/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;

namespace PlatoTK.Events
{
    public interface IPlatoEventsHelper
    {
        event EventHandler<IQuestionRaisedEventArgs> QuestionRaised;
        event EventHandler<IQuestionAnsweredEventArgs> QuestionAnswered;
        event EventHandler<ITVChannelSelectedEventArgs> TVChannelSelected;
        event EventHandler<ICallingEventCommandEventArgs> CallingEventCommand;
        event EventHandler<ICalledEventCommandEventArgs> CalledEventCommand;
        event EventHandler<ICallingTileActionEventArgs> CallingTileAction;

        void HandleAnswer(Response answer, Action callback, string lastQuestionKey);
        void HandleQuestion(string question, List<Response> choices, Action<string> setQuestion, Action<Response> addResponse, Action<Response> removeResponse, bool isTV, Action callback);
        void HandleChannelSelection(string name, TV tvInstance, Action callback);
        void HandleEventCommand(string[] commands, Event eventInstance, GameTime time, GameLocation location, Action callback, bool post);
        void HandleTileAction(string[] commands, Farmer who, GameLocation location, string layer, Point position, Action<bool> callback);
    }
}
