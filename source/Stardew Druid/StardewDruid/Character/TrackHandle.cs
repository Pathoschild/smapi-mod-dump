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
// Type: StardewDruid.Character.TrackHandle
// Assembly: StardewDruid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 24DA4344-683E-4959-87A6-C0A858BCC7DA
// Assembly location: C:\Users\piers\source\repos\StardewDruid\StardewDruid\bin\Debug\net5.0\StardewDruid.dll

using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;

#nullable disable
namespace StardewDruid.Character
{
    public class TrackHandle
    {
        public string trackFor;
        public string trackLocation;
        public Vector2 trackPlayer;
        public List<Vector2> trackVectors;
        public int trackLimit;

        public TrackHandle(string For)
        {
            this.trackVectors = new List<Vector2>();
            this.trackPlayer = new Vector2(-99f);
            this.trackLimit = 24;
            this.trackFor = For;
        }

        public void TrackPlayer()
        {
            if (this.trackLocation != Game1.player.currentLocation.Name)
            {
                this.trackLocation = Game1.player.currentLocation.Name;
                this.trackPlayer = new Vector2(-99f);
                this.trackVectors.Clear();
            }
            Vector2 position = Game1.player.Position;
            if ((double)Vector2.Distance(position, this.trackPlayer) >= 64.0)
            {
                this.trackPlayer = position;
                this.trackVectors.Add(position);
                if (this.trackVectors.Count >= this.trackLimit)
                    this.trackVectors.RemoveAt(0);
            }
            if (!(Mod.instance.characters[this.trackFor].currentLocation.Name != Game1.player.currentLocation.Name) || this.trackVectors.Count < 3)
                return;
            Mod.instance.characters[this.trackFor].WarpToTarget();
        }

        public void TruncateTo(int requirement)
        {
            int num = Math.Min(requirement, this.trackVectors.Count);
            List<Vector2> vector2List = new List<Vector2>();
            for (int index = this.trackVectors.Count - num; index < this.trackVectors.Count; ++index)
                vector2List.Add(this.trackVectors[index]);
            this.trackVectors = vector2List;
        }

        public Vector2 NextVector()
        {
            Vector2 trackVector = this.trackVectors[0];
            this.trackVectors.RemoveAt(0);
            return trackVector;
        }
    }
}
