/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ophaneom/Stardew-Valley-Mods
**
*************************************************/

using Microsoft.Xna.Framework;

namespace AbilitiesExperienceBars
{
    public static class BarController
    {
        //Control Vars
        private static int[] expPerLevel = new int[] { 100, 380, 770, 1300, 2150, 3300, 4800, 6900, 10000, 15000 };

        //Functions
        public static Rectangle GetExperienceBar(Vector2 barPosition, Vector2 barSize, int actualExp, int level, int maxPossibleLevel, int scale)
        {
            float percentage;
            if (level >= maxPossibleLevel)
            {
                percentage = barSize.X;
            }
            else if (level == 0)
            {
                percentage = ((float)actualExp / (float)expPerLevel[level]) * barSize.X;
            }
            else
            {
                percentage = ((float)actualExp - (float)expPerLevel[level - 1]) / ((float)expPerLevel[level] - (float)expPerLevel[level - 1]) * barSize.X;
            }

            Rectangle barRect = new Rectangle((int)barPosition.X, (int)barPosition.Y, (int)percentage * scale, (int)barSize.Y * scale);
            return barRect;
        }
        public static string GetExperienceText(int actualExp, int level, int maxPossibleLevel)
        {
            string expText;

            if (level == 0)
            {
                expText = $"{actualExp}/{expPerLevel[level]}";
            }
            else if (level >= maxPossibleLevel)
            {
                expText = $"{actualExp} exp.";
            }
            else
            {
                expText = $"{actualExp - expPerLevel[level - 1]}/{expPerLevel[level] - expPerLevel[level - 1]}";
            }

            return expText;
        }
        public static Vector2 GetMouseHoveringBar(Vector2 mousePos, Vector2 initialPos, int barQuantity, Vector2 barSize, float barSpacement)
        {
            Vector2 infoPosition = Vector2.Zero;

            for (var i = 0; i < barQuantity - 1; i++)
            {
                if (mousePos.X >= initialPos.X && mousePos.X <= barSize.X &&
                    mousePos.Y >= initialPos.Y + (barSpacement * i) && mousePos.Y <= barSize.Y)
                {
                    infoPosition = new Vector2(initialPos.X, initialPos.Y + (barSpacement * i));
                }
            }

            return infoPosition;
        }
        public static int AdjustBackgroundSize(int barQuantity, int barHeight, int barSpacement)
        {
            int size = (barSpacement + barHeight) * barQuantity;

            return size;
        }
        public static float AdjustLevelScale(int scale, int actualLevel, int maxLevel)
        {
            float t = 1;
            if (actualLevel < maxLevel)
            {
                switch (scale)
                {
                    case 0:
                        t = 0f;
                        break;
                    case 1:
                        t = 0.50f;
                        break;
                    case 2:
                        t = 0.75f;
                        break;
                    case 3:
                        t = 1f;
                        break;
                    case 4:
                        t = 1.25f;
                        break;
                    case 5:
                        t = 1.50f;
                        break;
                }
            }
            else
            {
                switch (scale)
                {
                    case 0:
                        t = 0f;
                        break;
                    case 1:
                        t = 0.25f;
                        break;
                    case 2:
                        t = 0.50f;
                        break;
                    case 3:
                        t = 0.75f;
                        break;
                    case 4:
                        t = 1f;
                        break;
                    case 5:
                        t = 1.25f;
                        break;
                }
            }

            float levelScale = t;
            return levelScale;
        }
        public static float AdjustExperienceScale(int scale)
        {
            float t = 0.7f;
            switch (scale)
            {
                case 0:
                    t = 0f;
                    break;
                case 1:
                    t = 0.3f;
                    break;
                case 2:
                    t = 0.5f;
                    break;
                case 3:
                    t = 0.7f;
                    break;
                case 4:
                    t = 0.9f;
                    break;
                case 5:
                    t = 1.1f;
                    break;
            }

            float levelScale = t;
            return levelScale;
        }
    }
}
