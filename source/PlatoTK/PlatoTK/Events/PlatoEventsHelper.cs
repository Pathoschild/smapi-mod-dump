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
using xTile.ObjectModel;

namespace PlatoTK.Events
{
    internal class PlatoEventsHelper : IPlatoEventsHelper
    {
        public event EventHandler<IQuestionRaisedEventArgs> QuestionRaised;
        public event EventHandler<IQuestionAnsweredEventArgs> QuestionAnswered;
        public event EventHandler<ITVChannelSelectedEventArgs> TVChannelSelected;
        public event EventHandler<ICallingEventCommandEventArgs> CallingEventCommand;
        public event EventHandler<ICalledEventCommandEventArgs> CalledEventCommand;
        public event EventHandler<ICallingTileActionEventArgs> CallingTileAction;

        internal readonly IPlatoHelper Plato;

        public PlatoEventsHelper(IPlatoHelper helper)
        {
            Plato = helper;
            Patching.EventPatches.InitializePatch(helper);
        }

        public void HandleAnswer(Response answer, Action callback, string lastQuestionKey)
        {
            QuestionAnswered?.Invoke(this, new QuestionAnsweredEventArgs(answer, callback, lastQuestionKey));
        }

        public void HandleQuestion(string question, List<Response> choices, Action<string> setQuestion, Action<Response> addResponse, Action<Response> removeResponse, bool isTV, Action callback)
        {
            QuestionRaised?.Invoke(this, new QuestionRaisedEventArgs(question, choices, setQuestion, addResponse, removeResponse, isTV, callback));
        }

        public void HandleChannelSelection(string name, TV tvInstance, Action callback)
        {
            TVChannelSelected?.Invoke(this, new TVChannelSelectedEventArgs(name, tvInstance, callback));
        }

        public void HandleEventCommand(string[] commands, Event eventInstance, GameTime time, GameLocation location, Action callback, bool post)
        {
            if (post)
                CalledEventCommand?.Invoke(this, new CallingEventCommandEventArgs(commands, eventInstance, time, location, callback));
            else
                CallingEventCommand?.Invoke(this, new CallingEventCommandEventArgs(commands, eventInstance, time, location, callback));

        }

        public void HandleTileAction(string[] commands, Farmer who, GameLocation location, string layer, Point position, Action<bool> callback)
        {
            var e = new CallingTileActionEventArgs(commands, who, location, layer, position, callback);


            if (e.Tile != null 
                && e.Tile.Properties.TryGetValue("@Action_Conditions", out PropertyValue conditions) 
                && !Plato.CheckConditions(conditions.ToString(), commands[0]))
                return;

            CallingTileAction?.Invoke(this, e);
        }
    }

}
