/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Videogamers0/SDV-CombineMachines
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombineMachines.Helpers
{
    public static class DelayHelpers
    {
        /// <summary>Intended to be invoked once during the mod entry, to initialize event subscriptions etc</summary>
        internal static void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.UpdateTicking += GameLoop_UpdateTicking;
            helper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;
        }

        private static readonly List<QueuedAction> QueuedActions = new List<QueuedAction>();

        /// <param name="TicksDelay">The number of game ticks to wait before invoking the given <paramref name="Action"/></param>
        /// <param name="InvokeAfterTick">If true, the action will be invoked immediately after the GameLoop.UpdateTicked event. If false, it will be invoked during GameLoop.UpdateTicking.</param>
        public static void InvokeLater(int TicksDelay, Action Action, double Priority = 10.0, bool InvokeAfterTick = true)
        {
            QueuedActions.Add(new QueuedAction(TicksDelay, Action, Priority, InvokeAfterTick));
        }

        private static void GameLoop_UpdateTicking(object sender, UpdateTickingEventArgs e)
        {
            foreach (QueuedAction QA in QueuedActions.OrderByDescending(x => x.Priority))
            {
                if (QA.GameLoop_UpdateTicking())
                    QueuedActions.Remove(QA);
            }
        }

        private static void GameLoop_UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            foreach (QueuedAction QA in QueuedActions.OrderByDescending(x => x.Priority))
            {
                if (QA.GameLoop_UpdateTicked())
                    QueuedActions.Remove(QA);
            }
        }

        private class QueuedAction
        {
            public int TotalDelay { get; }
            public int RemainingDelay { get; private set; }
            public Action Action { get; }
            public double Priority { get; }
            public bool InvokeAfterTick { get; }
            public bool InvokeBeforeTick { get { return !InvokeAfterTick; } }

            public QueuedAction(int Delay, Action Action, double Priority, bool InvokeAfterTick)
            {
                this.TotalDelay = Delay;
                this.RemainingDelay = Delay;
                this.Action = Action;
                this.Priority = Priority;
                this.InvokeAfterTick = InvokeAfterTick;
            }

            /// <summary>Returns true if the action has been handled and this item can be removed from the queue.</summary>
            public bool GameLoop_UpdateTicked() { return InvokeAfterTick ? HandleTick() : false; }
            /// <summary>Returns true if the action has been handled and this item can be removed from the queue.</summary>
            public bool GameLoop_UpdateTicking() { return InvokeBeforeTick ? HandleTick() : false; }

            private bool HandleTick()
            {
                RemainingDelay--;
                if (RemainingDelay <= 0)
                {
                    try
                    {
                        Action();
                    }
                    catch (Exception ex)
                    {
#if DEBUG
                        LogLevel LogLevel = LogLevel.Debug;
#else
                        LogLevel LogLevel = LogLevel.Trace;
#endif
                        string ErrorMsg = string.Format("Unhandled Error in {0}.{1}.{2}:\n{3}", nameof(DelayHelpers), nameof(QueuedAction), nameof(HandleTick), ex);
                        ModEntry.Logger.Log(ErrorMsg, LogLevel);
                    }

                    return true;
                }
                else
                    return false;
            }
        }
    }
}
