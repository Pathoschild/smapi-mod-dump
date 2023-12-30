/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

// Decompiled with JetBrains decompiler
// Type: StardewDruid.Character.Actor
// Assembly: StardewDruid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24DA4344-683E-4959-87A6-C0A858BCC7DA
// Assembly location: C:\Users\piers\source\repos\StardewDruid\StardewDruid\bin\Debug\net5.0\StardewDruid.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

#nullable disable
namespace StardewDruid.Character
{
    public class Actor : StardewDruid.Character.Character
    {
        public bool drawSlave;

        public Actor(Vector2 position, string map, string Name)
          : base(position, map, Name)
        {
        }

        public override void draw(SpriteBatch b, float alpha = 1f)
        {
            if (Context.IsMainPlayer && this.drawSlave)
            {
                foreach (NPC character in this.currentLocation.characters)
                {
                    if (!(character is StardewDruid.Character.Character))
                        character.drawAboveAlwaysFrontLayer(b);
                }
                this.drawAboveAlwaysFrontLayer(b);
            }
            base.draw(b, alpha);
        }
    }
}
