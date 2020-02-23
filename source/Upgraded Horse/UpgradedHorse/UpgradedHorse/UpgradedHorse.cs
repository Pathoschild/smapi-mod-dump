using System;

namespace UpgradedHorseMod
{
    public class UpgradedHorse
    {
        public String displayName;
        public HorseData horseData;

        public UpgradedHorse(String displayName, HorseData horseData)
        {
            this.displayName = displayName;
            this.horseData = horseData;
        }

        public string getMoodMessage()
        {
            Random rnd = new Random();
            int r = rnd.Next(1);

            if (!horseData.Full)
            {
                if (r == 0)
                {
                    return "Neighhhhhhhh...";
                }
                else
                {
                    return "NEIGHHHHHH";
                }
            }

            else if (horseData.Friendship < 200)
            {
                if (r == 0)
                {
                    return "Neigh...";
                }
                else
                {
                    return "Neigh, neigh...";
                }
            }

            else if (horseData.Friendship >= 200 && horseData.Friendship < 400)
            {
                if (r == 0)
                {
                    return "Neigh. Neigh.";
                }
                else
                {
                    return "Clip, clop";
                }
            }

            else if (horseData.Friendship >= 400 && horseData.Friendship < 600)
            {
                if (r == 0)
                {
                    return "Neigh. Neigh!";
                }
                else
                {
                    return "Clippity cloppity";
                }
            }

            else if (horseData.Friendship >= 600 && horseData.Friendship < 800)
            {
                if (r == 0)
                {
                    return "Neigh! Neigh!";
                }
                else
                {
                    return "Clippity cloppity clip!";
                }
            }

            else // (horseData.Friendship >= 800)
            {
                if (r == 0)
                {
                    return "Neigh! Neigh! Neigh!";
                }
                else
                {
                    return "Clippity cloppity clippity clop!";
                }
            }
        }
    }
}
