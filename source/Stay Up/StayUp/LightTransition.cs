/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/su226/StardewValleyMods
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;

namespace Su226.StayUp {
  class LightTransition {
    private readonly int morningBegin;
    private readonly int morningDuration;

    private Color lightColor = Color.Transparent;
    private Color darkColor = new Color(237, 237, 0, 237);
    private Color rainLightColor = new Color(76, 60, 24, 76);
    private Color rainDarkColor = new Color(237, 186, 74, 237);

    public LightTransition() {
      this.morningBegin = M.Config.morningLight / 100 * 60 + M.Config.morningLight % 100;
      this.morningDuration = 360 - this.morningBegin;
      M.Helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
    }

    private void OnUpdateTicked(object o, UpdateTickedEventArgs e) {
      if (Game1.timeOfDay < M.Config.morningLight) {
        Game1.outdoorLight = Game1.isRaining ? this.rainDarkColor : this.darkColor;
      } else if (Game1.timeOfDay < 600) {
        double minute = Game1.timeOfDay / 100 * 60
          + Game1.timeOfDay % 100
          + Game1.gameTimeInterval / 700.0
          - this.morningBegin;
        Game1.outdoorLight = this.GetAverageColor(
          Game1.isRaining ? this.rainLightColor : this.lightColor,
          Game1.isRaining ? this.rainDarkColor : this.darkColor,
          Math.Pow(minute / this.morningDuration, M.Config.morningLightPower)
        );
      } else if (Game1.timeOfDay == 600) {
        Game1.outdoorLight = Game1.isRaining ? this.rainLightColor : this.lightColor;
      }
    }

    private Color GetAverageColor(Color c1, Color c2, double ratio1) {
      double ratio2 = 1 - ratio1;
      return new Color(
        (byte)(c1.R * ratio1 + c2.R * ratio2),
        (byte)(c1.G * ratio1 + c2.G * ratio2),
        (byte)(c1.B * ratio1 + c2.B * ratio2),
        (byte)(c1.A * ratio1 + c2.A * ratio2)
      );
    }
  }
}