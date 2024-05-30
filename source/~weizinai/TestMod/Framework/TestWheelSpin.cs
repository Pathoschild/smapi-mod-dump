/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using StardewValley;
using StardewValley.Extensions;

namespace TestMod.Framework;

public class TestWheelSpin
{
    public double arrowRotation;

    public double arrowRotationVelocity;

    public double arrowRotationDeceleration;

    private int timerBeforeStart;

    public int count;
    public int total;

    public TestWheelSpin(int total)
    {
        this.total = total;
        arrowRotationDeceleration = -0.0006283185307179586;
    }

    public void Update()
    {
        for (int i = 0; i < total; i++)
        {
            arrowRotation = 0;
            arrowRotationVelocity = Math.PI / 16.0;
            arrowRotationVelocity += Game1.random.Next(0, 15) * Math.PI / 256.0;
            if (Game1.random.NextBool())
            {
                arrowRotationVelocity += Math.PI / 64.0;
            }

            while (arrowRotationVelocity > 0)
            {
                double oldVelocity = arrowRotationVelocity;
                arrowRotationVelocity += arrowRotationDeceleration;
                if (arrowRotationVelocity <= Math.PI / 80.0 && oldVelocity > Math.PI / 80.0)
                {
                    bool colorChoiceGreen = true;
                    if (arrowRotation > Math.PI / 2.0 && arrowRotation <= 4.319689898685965 && Game1.random.NextDouble() < Game1.player.LuckLevel / 15f)
                    {
                        if (colorChoiceGreen)
                        {
                            arrowRotationVelocity = Math.PI / 48.0;
                        }
                    }
                    else if ((arrowRotation + Math.PI) % (Math.PI * 2.0) <= 4.319689898685965 && !colorChoiceGreen && Game1.random.NextDouble() < Game1.player.LuckLevel / 20f)
                    {
                        arrowRotationVelocity = Math.PI / 48.0;
                    }
                }

                if (arrowRotationVelocity <= 0.0)
                {
                    arrowRotationVelocity = 0.0;
                    bool colorChoiceGreen2 = true;
                    bool won = false;
                    if (arrowRotation > Math.PI / 2.0 && arrowRotation <= 4.71238898038469)
                    {
                        if (!colorChoiceGreen2)
                        {
                            won = true;
                        }
                    }
                    else if (colorChoiceGreen2)
                    {
                        won = true;
                    }

                    if (won)
                    {
                        count++;
                    }
                }

                arrowRotation += arrowRotationVelocity;
                arrowRotation %= Math.PI * 2.0;
            }
        }
    }
}