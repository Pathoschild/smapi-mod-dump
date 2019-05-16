using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightingTest
{
    class Program
    {
        static int uniqueID = 123456;
        static int daysToPlay = 10000;
        static float luckLevel = 0;

        static void Main(string[] args)
        {
            var lucks = new double[] { -.1, -.05, 0, .05, .1 };
            var datas = new List<List<int>>();
            foreach(double l in lucks)
            {
                var data = new List<int>();
                for(int day = 0; day<daysToPlay; day++)
                {
                    data.Add(howManyStrikes(l, day));
                }
                datas.Add(data);
            }
            var sb = new StringBuilder();
            sb.AppendLine(string.Join(",", lucks));
            for (int day = 0; day < daysToPlay; day++)
            {
                sb.AppendLine(string.Join(",", datas.Select(x => x[day])));
            }
            System.IO.StreamWriter file = new System.IO.StreamWriter("hereIam.txt");
            file.WriteLine(sb.ToString()); // "sb" is the StringBuilder
        }

        private static int howManyStrikes(double luck, int daysPlayed)
        {
            int total = 0;
            foreach(int time in GetTimes())
            {
                var random = new Random(uniqueID + daysPlayed + time);
                if (random.NextDouble() < 0.125 + luck + (luckLevel / 100f))
                {
                    total++;
                }
            }
            return total;
        }

        private static IEnumerable<int> GetTimes()
        {
            var i = 600;
            while (i < 2400)
            {
                yield return i;
                i += 10;
                if (i % 100 == 60)
                {
                    i += 40;
                }
            }
        }
    }
}
