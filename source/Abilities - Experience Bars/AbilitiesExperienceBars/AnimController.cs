/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ophaneom/Abilities-Experience-Bars
**
*************************************************/

using Microsoft.Xna.Framework;

namespace AbilitiesExperienceBars
{
    public static class AnimController
    {
        //Functions
        public static Vector2 Animate(Vector2 boxPos, Vector2 dirTo, float velocity, string dirName)
        {
            switch (dirName)
            {
                case "top":
                    if (boxPos.Y > dirTo.Y)
                    {
                        boxPos.Y -= velocity;
                        return new Vector2(boxPos.X, boxPos.Y);
                    }
                    else
                    {
                        boxPos.Y = dirTo.Y;
                        return new Vector2(boxPos.X, boxPos.Y);
                    }

                case "bottom":
                    if (boxPos.Y < dirTo.Y)
                    {
                        boxPos.Y += velocity;
                        return new Vector2(boxPos.X, boxPos.Y);
                    }
                    else
                    {
                        boxPos.Y = dirTo.Y;
                        return new Vector2(boxPos.X, boxPos.Y);
                    }

                case "left":
                    if (boxPos.X > dirTo.X)
                    {
                        boxPos.X -= velocity;
                        return new Vector2(boxPos.X, boxPos.Y);
                    }
                    else
                    {
                        boxPos.X = dirTo.X;
                        return new Vector2(boxPos.X, boxPos.Y);
                    }

                case "right":
                    if (boxPos.X < dirTo.X)
                    {
                        boxPos.X += velocity;
                        return new Vector2(boxPos.X, boxPos.Y);
                    }
                    else
                    {
                        boxPos.X = dirTo.X;
                        return new Vector2(boxPos.X, boxPos.Y);
                    }
            }

            return boxPos;
        }

        public static Color AnimateColor(Color color, float velocity)
        {
            float cR = color.R * velocity;
            float cG = color.G * velocity;
            float cB = color.B * velocity;

            return new Color((int)cR, (int)cG, (int)cB, color.A);
        }
    }
}
