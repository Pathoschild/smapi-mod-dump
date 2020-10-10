/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/gunnargolf/DynamicChecklist
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewValley;

namespace DynamicChecklist.Graph
{
    public class ShortestPath
    {
        private List<Step> Steps { get; set; } = new List<Step>();

        public void AddStep(GameLocation location, Vector2 position)
        {
            this.Steps.Add(new Step(location, position));
        }

        public Step GetNextStep(GameLocation playerLocation)
        {
            return this.Steps.FirstOrDefault();
        }
    }
}

public struct Step
{
    public Step(GameLocation location, Vector2 position)
    {
        this.Location = location;
        this.Position = position;
    }

    public GameLocation Location { get; }

    public Vector2 Position { get; }
}