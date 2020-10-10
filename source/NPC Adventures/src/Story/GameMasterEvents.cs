/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/PurrplingMod
**
*************************************************/

using NpcAdventure.Events;
using PurrplingCore.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NpcAdventure.Story
{
    internal class GameMasterEvents : IGameMasterEvents
    {
        private readonly EventManager eventManager;

        public GameMasterEvents(EventManager eventManager)
        {
            this.eventManager = eventManager;
        }

        public event EventHandler<ICheckEventEventArgs> CheckEvent
        {
            add => this.eventManager.CheckEvent.Add(value);
            remove => this.eventManager.CheckEvent.Remove(value);
        }

        public class EventManager
        {
            public ManagedEvent<ICheckEventEventArgs> CheckEvent = new ManagedEvent<ICheckEventEventArgs>(nameof(CheckEvent));
        }

        public class CheckEventEventArgs : ICheckEventEventArgs
        {
            public CheckEventEventArgs(GameLocation location, Farmer player)
            {
                this.Location = location;
                this.Player = player;
            }

            public GameLocation Location { get; set; }

            public Farmer Player { get; set; }
        }
    }
}
