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

namespace weizinai.StardewValleyMod.TestMod.Framework;

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
        this.arrowRotationDeceleration = -0.0006283185307179586;
    }

    public void Update()
    {
        for (int i = 0; i < this.total; i++)
        {
            this.arrowRotation = 0;
            this.arrowRotationVelocity = Math.PI / 16.0;
            this.arrowRotationVelocity += Game1.random.Next(0, 15) * Math.PI / 256.0;
            if (Game1.random.NextBool())
            {
                this.arrowRotationVelocity += Math.PI / 64.0;
            }

            while (this.arrowRotationVelocity > 0)
            {
                double oldVelocity = this.arrowRotationVelocity;
                this.arrowRotationVelocity += this.arrowRotationDeceleration;
                if (this.arrowRotationVelocity <= Math.PI / 80.0 && oldVelocity > Math.PI / 80.0)
                {
                    bool colorChoiceGreen = true;
                    if (this.arrowRotation > Math.PI / 2.0 && this.arrowRotation <= 4.319689898685965 && Game1.random.NextDouble() < Game1.player.LuckLevel / 15f)
                    {
                        if (colorChoiceGreen)
                        {
                            this.arrowRotationVelocity = Math.PI / 48.0;
                        }
                    }
                    else if ((this.arrowRotation + Math.PI) % (Math.PI * 2.0) <= 4.319689898685965 && !colorChoiceGreen && Game1.random.NextDouble() < Game1.player.LuckLevel / 20f)
                    {
                        this.arrowRotationVelocity = Math.PI / 48.0;
                    }
                }

                if (this.arrowRotationVelocity <= 0.0)
                {
                    this.arrowRotationVelocity = 0.0;
                    bool colorChoiceGreen2 = true;
                    bool won = false;
                    if (this.arrowRotation > Math.PI / 2.0 && this.arrowRotation <= 4.71238898038469)
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
                        this.count++;
                    }
                }

                this.arrowRotation += this.arrowRotationVelocity;
                this.arrowRotation %= Math.PI * 2.0;
            }
        }
    }
}