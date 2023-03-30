/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using PlatoUI.Content;
using PlatoUI.UI;
using StardewValley;
using System;

namespace PlatoUI
{
    internal class PlatoHelper : IPlatoUIHelper
    {

        public StardewModdingAPI.IModHelper ModHelper { get; }

        public IUIHelper UI { get; }
        public IContentHelper Content { get; }


        public PlatoHelper(StardewModdingAPI.IModHelper helper)
        {
            ModHelper = helper;
            UI = new UIHelper(this);
            Content = new ContentHelper(this);
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
