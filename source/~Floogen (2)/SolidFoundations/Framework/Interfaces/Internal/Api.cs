/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/SolidFoundations
**
*************************************************/

using Microsoft.Xna.Framework;
using SolidFoundations.Framework.Models.ContentPack.Actions;
using SolidFoundations.Framework.Utilities;
using StardewValley;
using StardewValley.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidFoundations.Framework.Interfaces.Internal
{
    public interface IApi
    {
        public class BroadcastEventArgs : EventArgs
        {
            public string BuildingId { get; set; }
            public Building Building { get; set; }
            public Farmer Farmer { get; set; }
            public Point TriggerTile { get; set; }
            public string Message { get; set; }
        }

        event EventHandler<BroadcastEventArgs> BroadcastSpecialActionTriggered;
        event EventHandler BeforeBuildingSerialization; // TODO: Make this obsolete with SDV v1.6
        event EventHandler AfterBuildingRestoration; // TODO: Make this obsolete with SDV v1.6

        public void AddBuildingFlags(Building building, List<string> flags, bool isTemporary = true);
        public void RemoveBuildingFlags(Building building, List<string> flags);
        public bool DoesBuildingHaveFlag(Building building, string flag);
    }

    public class Api : IApi
    {
        public event EventHandler<IApi.BroadcastEventArgs> BroadcastSpecialActionTriggered;
        public event EventHandler BeforeBuildingSerialization;
        public event EventHandler AfterBuildingRestoration;

        internal void OnSpecialActionTriggered(IApi.BroadcastEventArgs e)
        {
            EventHandler<IApi.BroadcastEventArgs> handler = BroadcastSpecialActionTriggered;
            if (handler is not null)
            {
                handler(this, e);
            }
        }

        internal void OnBeforeBuildingSerialization(EventArgs e)
        {
            EventHandler handler = BeforeBuildingSerialization;
            if (handler is not null)
            {
                handler(this, e);
            }
        }

        internal void OnAfterBuildingRestoration(EventArgs e)
        {
            EventHandler handler = AfterBuildingRestoration;
            if (handler is not null)
            {
                handler(this, e);
            }
        }

        public void AddBuildingFlags(Building building, List<string> flags, bool isTemporary = true)
        {
            foreach (var flag in flags)
            {
                var flagKey = String.Concat(ModDataKeys.FLAG_BASE, ".", flag.ToLower());
                building.modData[flagKey] = (isTemporary ? SpecialAction.FlagType.Temporary : SpecialAction.FlagType.Permanent).ToString();
            }
        }

        public void RemoveBuildingFlags(Building building, List<string> flags)
        {
            foreach (var flag in flags)
            {
                var flagKey = String.Concat(ModDataKeys.FLAG_BASE, ".", flag.ToLower());
                building.modData.Remove(flagKey);
            }
        }

        public bool DoesBuildingHaveFlag(Building building, string flag)
        {
            var flagKey = String.Concat(ModDataKeys.FLAG_BASE, ".", flag.ToLower());
            return building.modData.Keys.Any(k => k.Equals(flagKey, StringComparison.OrdinalIgnoreCase));
        }
    }
}
