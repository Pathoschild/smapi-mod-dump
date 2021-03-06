/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/andyruwruw/stardew-valley-water-bot
**
*************************************************/

using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace WaterBot.Framework
{
    class ActionableTile
    {
        private Point stand;

        private List<Point> executeOn;

        private Action action;

        public enum Action
        {
            Water,
            Refill,
        }

        public ActionableTile(Action action)
        {
            this.action = action;
            this.executeOn = new List<Point>();
        }

        public ActionableTile(Point stand, Action action)
        {
            this.stand = stand;
            this.action = action;
        }

        public ActionableTile(Point stand, List<Point> executeOn, Action action)
        {
            this.stand = stand;
            this.executeOn = executeOn;
            this.action = action;
        }

        public void setStand(Point stand)
        {
            this.stand = stand;
        }

        public Point getStand()
        {
            return this.stand;
        }

        public Action getAction()
        {
            return action;
        }

        public void setExecuteOn(List<Point> executeOn)
        {
            this.executeOn = executeOn;
        }

        public void pushExecuteOn(Point executeOn)
        {
            this.executeOn.Add(executeOn);
        }

        public Point Pop()
        {
            if (this.executeOn.Count == 0)
            {
                return new Point(-1, -1);
            }

            Point temp = this.executeOn[0];
            this.executeOn.RemoveAt(0);
            return temp;
        }

        public bool isDone()
        {
            return (this.executeOn.Count == 0);
        }
    }
}
