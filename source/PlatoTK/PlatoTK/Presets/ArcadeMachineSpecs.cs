/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using System;
using Microsoft.Xna.Framework;

namespace PlatoTK.Presets
{
    internal class ArcadeMachineSpecs
    {
        public string Name { get; set; }

        public string Id { get; set; }

        public string ObjectName { get; set; }

        public Action Start { get; set; }

        public string Sprite { get; set; }

        public string Icon { get; set; }

        public Vector2 Spot { get; set; } = Vector2.Zero;

        public ArcadeMachineSpecs(string name, string id, string objectName, Action start, string sprite, string icon)
        {
            Name = name;
            ObjectName = objectName;
            Id = id;
            Start = start;
            Sprite = sprite;
            Icon = icon;
        }
    }
}
