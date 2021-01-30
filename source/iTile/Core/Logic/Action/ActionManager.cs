/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ha1fdaew/iTile
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Linq;
using System.Collections.Generic;
using xTile.Tiles;

namespace iTile.Core.Logic.Action
{
    public class ActionManager : Manager
    {
        private static List<IAction> executors = new List<IAction>();

        public ActionManager()
        {
            Init();
        }

        public Tile PerformAction(Action action, Vector2 pos, Tile tile = null)
        {
            return FindAndExecute(action, pos, tile);
        }

        private Tile FindAndExecute(Action action, Vector2 pos, Tile tile = null)
        {
            return executors.FirstOrDefault(exec =>
            {
                ActionAttr attr = (ActionAttr)Attribute.GetCustomAttribute(exec.GetType(), typeof(ActionAttr));
                return attr != null && attr.Action == action;
            })?.Execute(pos, tile);
        }

        public T GetActionExecutor<T>() where T : IAction
        {
            return (T)executors.FirstOrDefault(action => action is T);
        }

        public override void Init()
        {
            executors.Add(new CopyTile());
            executors.Add(new PasteTile());
            executors.Add(new DeleteTile());
            executors.Add(new RestoreTile());
        }

        public enum Action
        {
            Copy,
            Paste,
            Delete,
            Restore
        }
    }
}
