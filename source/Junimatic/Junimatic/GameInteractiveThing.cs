/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NermNermNerm/Junimatic
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace NermNermNerm.Junimatic
{
    public abstract class GameInteractiveThing
    {
        protected GameInteractiveThing(object gameObject, Point accessPoint)
        {
            this.AccessPoint = accessPoint;
            this.GameObject = gameObject;
        }

        public Point AccessPoint { get; }

        /// <summary>
        ///   This points to the object within the game code.  It exists because we need to be
        ///   able to tell if two instances of this class refer to the same thing.
        /// </summary>
        public object GameObject { get; }
    }
}
