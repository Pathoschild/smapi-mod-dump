/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using PlatoTK.Content;
using PlatoTK.Patching;
using PlatoTK.Network;
using PlatoTK.UI;
using StardewValley;
using System;
using PlatoTK.Events;
using System.Collections.Generic;
using System.Linq;
using PlatoTK.Lua;
using PlatoTK.Utils;
using PlatoTK.Presets;

namespace PlatoTK
{
    internal class PlatoHelper : IPlatoHelper
    {
        internal static readonly List<IConditionsProvider> ConditionsProvider = new List<IConditionsProvider>();
        public ISharedDataHelper SharedData { get; }
        public IHarmonyHelper Harmony { get; }

        public ILuaHelper Lua { get; }
        public IPresetHelper Presets { get; }

        private static readonly EventConditionsProvider DefaultEventsConditionsProvider = new EventConditionsProvider();
        public IPlatoEventsHelper Events => EventsInternal;

        internal static IPlatoEventsHelper EventsInternal { get; set; }

        public IContentHelper Content { get; }

        public StardewModdingAPI.IModHelper ModHelper { get; }

        public IUIHelper UI { get; }

        public IBasicUtils Utilities { get; }
        public PlatoHelper(StardewModdingAPI.IModHelper helper)
        {
            ModHelper = helper;
            SharedData = new SharedDataHelper(this);
            Harmony = new HarmonyHelper(this);
            Content = new ContentHelper(this);
            UI = new UIHelper(this);
            Lua = new LuaHelper(this);
            Presets = new PresetHelper(this);
            Utilities = new BasicUtils(this);
        }

        public bool CheckConditions(string conditions, object caller)
        {
            if (string.IsNullOrEmpty(conditions))
                return true;

            foreach (string condition in conditions.Replace(@"\/", "--div--").Split('/'))
                if(TryCheckConditions(condition.Replace("--div--", "/"), caller, out bool result))
                    return result;

            return true;
        }

        internal static bool TryCheckConditions(string conditions, object caller, out bool result, bool includeDefault = true)
        {
            if (string.IsNullOrEmpty(conditions))
            {
                result = true;
                return true;
            }

            bool compare = true;
            conditions = conditions.Trim();

            string trigger = conditions.Split(' ')[0];
            if (trigger[0] == '!')
            {
                compare = false;
                trigger = trigger.Substring(1);
            }

            if (trigger.ToLower() == "true" || trigger.ToLower() == "false")
            {
                result = conditions.ToLower() == "true";
                result = compare == result;
                return true;
            }

            if (ConditionsProvider.FirstOrDefault(c => c.CanHandleConditions(trigger)) is IConditionsProvider cp)
            {
                result = cp.CheckConditions(conditions, caller);
                result = compare == result;

                return true;
            }

            if (includeDefault)
            {
                result = DefaultEventsConditionsProvider.CheckConditions(conditions, caller);
                result = compare == result;
                return true;
            }

            result = true;
            result = compare == result;

            return false;
        }

        public void AddConditionsProvider(IConditionsProvider provider)
        {
            if (ConditionsProvider.Any(p => p.Id == provider.Id))
                ConditionsProvider.Add(provider);
        }

        public DelayedAction SetDelayedAction(int delay, Action action)
        {
            DelayedAction d = new DelayedAction(delay, () => action());
           
            Game1.delayedActions.Add(d);
            return d;
        }

        public void SetDelayedUpdateAction(int delay, Action action)
        {
            SetTickHandler(action, delay, false);
        }

        public void SetTickDelayedUpdateAction(int delay, Action action)
        {
            SetTickHandler(action, delay, true);
        }

        private void SetTickHandler(Action action, int delay, bool ticks)
        {
            long target = !ticks ? Game1.currentGameTime.TotalGameTime.Milliseconds + delay : delay;

            EventHandler<StardewModdingAPI.Events.UpdateTickingEventArgs> tickHandler = null;
            tickHandler = (sender, e) =>
            {
                if (ticks)
                    target--;

                if (!ticks && Game1.currentGameTime.TotalGameTime.Milliseconds >= target)
                    target = 0;

                if (target <= 0)
                {
                    ModHelper.Events.GameLoop.UpdateTicking -= tickHandler;
                    action();
                }
            };

            ModHelper.Events.GameLoop.UpdateTicking += tickHandler;
        }
    }
}
