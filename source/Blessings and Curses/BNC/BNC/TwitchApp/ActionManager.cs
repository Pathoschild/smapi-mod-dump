/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/GenDeathrow/SDV_BlessingsAndCurses
**
*************************************************/

using System;
using StardewModdingAPI;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using BNC.Actions;
using Humanizer;
using BNC.TwitchApp.Actions;

namespace BNC.TwitchApp
{
    public class ActionManager
    {
        private Dictionary<string, Type> _actions = new Dictionary<string, Type>();
        public ConcurrentQueue<BaseAction> _actionQueue = new ConcurrentQueue<BaseAction>();
        private ConcurrentQueue<BaseAction> _tickQueue = new ConcurrentQueue<BaseAction>();

        public ActionManager()
        {
            AddAction(typeof(HealPlayer));
            AddAction(typeof(SpawnCat));
            AddAction(typeof(Buff));
            AddAction(typeof(CustomBuff));
            AddAction(typeof(GiveMoney));
            AddAction(typeof(GiveStamina));
            AddAction(typeof(Spawn));
            AddAction(typeof(Command));
            AddAction(typeof(Weather));
            AddAction(typeof(BombEvent));
            AddAction(typeof(ScreenFlash));
            AddAction(typeof(MeteorStorm));

            //Init Cat handlers
            SpawnCat.Init();
        }

        private void AddAction(Type action)
        {
            if (!typeof(BaseAction).IsAssignableFrom(action))
            {
                BNC_Core.Logger.Log($"Action {action} was of wrong type", LogLevel.Warn);
                return;
            }

            var type = action.Name.Underscore();
            _actions.Add(type, action);
            BNC_Core.Logger.Log($"Added action: {type}", LogLevel.Debug);
        }

        public void HandleAction(string rawAction)
        {
            BNC_Core.Logger.Log($"ActionManager: Raw action {rawAction}", LogLevel.Debug);
            try
            {
                var o = JsonConvert.DeserializeObject<JObject>(rawAction);

                var type = (string)o["actions"][0]["type"];
                
                var actionType = _actions[type ?? "invalid"];
                if (actionType == null)
                {
                    BNC_Core.Logger.Log($"Faild finding action {(string)o["actions"][0]["type"]}", LogLevel.Error);
                    return;
                }

                var actionObj = o["actions"][0].ToObject(actionType);
                if (!(actionObj is BaseAction action)) return;

                BNC_Core.Logger.Log($"ActionManager: Handling action {actionObj.ToString()}", LogLevel.Debug);

                 _actionQueue.Enqueue(action);
            }
            catch (Exception e)
            {
                BNC_Core.Logger.Log($"Error parsing action: {e}", LogLevel.Error);
            }
        }

        public void HandleMessage(string message)
        {
            BNC_Core.Logger.Log($"ActionManager: Handling message {message}", LogLevel.Info);
            _actionQueue.Enqueue(new MessageAction(message));
        }


        public void Update()
        {
            if (!_actionQueue.TryDequeue(out var action)) return;

            if (action.TryAfter.HasValue)
            {
                if (action.TryAfter.Value > DateTime.Now)
                {
                    _actionQueue.Enqueue(action);
                    return;
                }
                action.TryAfter = null;
            }
            BNC_Core.Logger.Log($"Update Tick... {action}", LogLevel.Debug);
            var response = action.Handle();
            BNC_Core.Logger.Log($"Resonse {response}", LogLevel.Debug);
            switch (response)
            {
                case ActionResponse.Retry:
                    _actionQueue.Enqueue(action);
                    break;
                case ActionResponse.Done:
                    {
                        break;
                    }
            }
        }

        public void UpdateTickable()
        {
            if (!_actionQueue.TryDequeue(out var action)) return;
            var response = action.Handle();

            switch (response)
            {
                case ActionResponse.Retry:
                    _actionQueue.Enqueue(action);
                    break;
                case ActionResponse.Done:
                {
                    break;
                }
            }
        }
    }


    public enum ActionResponse
    {
        Done,
        Retry
    }
}
