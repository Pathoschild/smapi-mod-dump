/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NormanPCN/StardewValleyMods
**
*************************************************/

using System;
using NormanPCN.Utils;

// this is not a real randum number tester. more of timing and basic sanity checks/tests.

Console.WriteLine("Hello Random Numbers!\n");

string[] RndGenNames = { "XorShiftWow", "XorShiftPlus", "NR_Ranq1", "NR_Ran" };

void timing(uint seed32)
{
    ulong seed64 = seed32;

    const int randomRange = 1000;

    var sw = new System.Diagnostics.Stopwatch();

    long r;
    double bounded1;
    double bounded2;
    double unbound;
    //uint spinLock = 0;

    const uint loopCount = 10 * 1000 * 1000;

    Console.WriteLine($"Timing loops. count={loopCount:N0}");
    for (uint rtype = RandomNumbers.XorShiftWow; rtype <= RandomNumbers.NR_Ran; rtype++)
    {
        var rnd = new RandomNumbers(seed32, rtype);

        r = 0;
        sw.Restart();
        for (uint i = 0; i < loopCount; i++)
        {

            //do {
            //    if ((spinLock == 0) && (System.Threading.Interlocked.CompareExchange(ref spinLock, 1, 0) == 0))
            //        break;
            //}
            //while (true);

            r += rnd.Next();

            //spinLock = 0;
        }
        sw.Stop();
        unbound = sw.Elapsed.TotalMilliseconds;

        rnd.Reseed(seed32);
        long r1 = 0;
        sw.Restart();
        for (uint i = 0; i < loopCount; i++)
        {
            r1 += rnd.Next(randomRange);
        }
        sw.Stop();
        bounded1 = sw.Elapsed.TotalMilliseconds;

        rnd.Reseed(seed32);
        r = 0;
        sw.Restart();
        for (uint i = 0; i < loopCount; i++)
        {
            r += rnd.Next(0, randomRange);
        }
        sw.Stop();
        bounded2 = sw.Elapsed.TotalMilliseconds;

        if (r != r1)
            Console.WriteLine("Sums different");

        rnd.Reseed(seed32);
        r = 0;
        sw.Restart();
        for (uint i = 0; i < loopCount; i++)
        {
            r += rnd.NextU(0, 10*1000000);
        }
        sw.Stop();
        double unbiased = sw.Elapsed.TotalMilliseconds;

        Console.WriteLine($"rtype={RndGenNames[rtype]}, full={unbound:F2}, bounded1={bounded1:F2}, bounded2={bounded2:F2}, unbiased={unbiased:F2}");
    }

    r = 0;
    sw.Restart();
    for (uint i = 0; i < loopCount; i++)
    {
        r += OneTimeRandom.Rnd(seed32 + i, 0);
    }
    sw.Stop();
    unbound = sw.Elapsed.TotalMilliseconds;

    r = 0;
    sw.Restart();
    for (uint i = 0; i < loopCount; i++)
    {
        r += OneTimeRandom.Rnd(seed32 + i, randomRange);
    }
    sw.Stop();
    bounded1 = sw.Elapsed.TotalMilliseconds;

    r = 0;
    sw.Restart();
    for (uint i = 0; i < loopCount; i++)
    {
        r += OneTimeRandom.Rnd(seed32 + i, 0, randomRange);
    }
    sw.Stop();
    bounded2 = sw.Elapsed.TotalMilliseconds;

    Console.WriteLine($"OneTime32, full={unbound:F2}, bounded1={bounded1:F2}, bounded2={bounded2:F2}");

    r = 0;
    sw.Restart();
    for (uint i = 0; i < loopCount; i++)
    {
        r += OneTimeRandom.Rnd(seed64 + i, 0);
    }
    sw.Stop();
    unbound = sw.Elapsed.TotalMilliseconds;
    r = 0;
    sw.Restart();
    for (uint i = 0; i < loopCount; i++)
    {
        r += OneTimeRandom.Rnd(seed64 + i, randomRange);
    }
    sw.Stop();
    bounded1 = sw.Elapsed.TotalMilliseconds;
    r = 0;
    sw.Restart();
    for (uint i = 0; i < loopCount; i++)
    {
        r += OneTimeRandom.Rnd(seed64 + i, 0, randomRange);
    }
    sw.Stop();
    bounded2 = sw.Elapsed.TotalMilliseconds;
    Console.WriteLine($"OneTime64, full={unbound:F2}, bounded1={bounded1:F2}, bounded2={bounded2:F2}");

    Console.WriteLine();
}

void ShortSequence(uint seed32)
{
    ulong seed64 = seed32;

    const uint shortListCount = 20;
    const int randomRange = 10;

    Console.WriteLine("Short sSequence");

    Console.WriteLine("Random");
    Console.Write("    ");
    RandomNumbers rnd = new RandomNumbers(seed32, RandomNumbers.DefaultRNG);
    for (uint i = 0; i < shortListCount; i++)
    {
        Console.Write($"{rnd.Next(0, randomRange)}, ");
    }
    Console.WriteLine();

    Console.WriteLine("OneTime32");
    Console.Write("    ");
    for (uint i = 0; i < shortListCount; i++)
    {
        Console.Write($"{OneTimeRandom.Rnd(seed32 + i, randomRange)}, ");
    }
    Console.WriteLine();
    Console.WriteLine();

    Console.WriteLine("OneTime32 float");
    Console.Write("    ");
    for (uint i = 1; i <= shortListCount; i++)
    {
        Console.Write($"{OneTimeRandom.RndDouble(seed32 + i):F4}, ");
        if (i % 10 == 0)
        {
            Console.WriteLine();
            Console.Write("    ");
        }
    }
    Console.WriteLine();

    Console.WriteLine("OneTime64");
    Console.Write("    ");
    for (uint i = 0; i < shortListCount; i++)
    {
        Console.Write($"{OneTimeRandom.Rnd(seed64 + i, randomRange)}, ");
    }
    Console.WriteLine();

    Console.WriteLine("OneTime64 float");
    Console.Write("    ");
    for (uint i = 1; i <= shortListCount; i++)
    {
        Console.Write($"{OneTimeRandom.RndDouble(seed64 + i):F4}, ");
        if (i % 10 == 0)
        {
            Console.WriteLine();
            Console.Write("    ");
        }
    }
    Console.WriteLine();
}

void Density(uint seed32)
{
    const int densityRange = 10 * 1000;

    bool[] density = new bool[densityRange];

    for (uint rtype = RandomNumbers.XorShiftWow; rtype <= RandomNumbers.NR_Ran; rtype++)
    {
        var rnd = new RandomNumbers(seed32, rtype);

        Array.Fill<bool>(density, false);
        //for (int i = 0; i < density.Length; i++)
        //    density[i] = false;

        for (int i = 0; i < density.Length; i++)
            density[rnd.Next(densityRange)] = true;

        int count = 0;
        for (int i = 0; i < density.Length; i++)
            if (density[i])
                count++;

        Console.WriteLine($"Density rtype={RndGenNames[rtype]}, (Range={densityRange}), density={((float)count / (float)densityRange):F3}");
    }

    Console.WriteLine();
}

void Distribution(uint seed32)
{
    const int runCountMinMax = 10 * 1000;
    const int runCountGroups = 1 * 1000;
    const int groupCount = 10;
    const int rndRange = 1000;
    const int groupSize = rndRange / groupCount;
    const uint rtype = RandomNumbers.DefaultRNG;

    int[] counts = new int[groupCount];

    var rnd = new RandomNumbers(seed32, rtype);

    int r;
    int min = Int32.MaxValue;
    int max = Int32.MinValue;
    for (int i = 0; i < runCountMinMax; i++)
    {
        r = rnd.Next();

        if (r < min)
            min = r;
        if (r > max)
            max = r;
    }

    int above = 0;
    double rr;
    double minR = 1.0;
    double maxR = 0.0;
    rnd.Reseed(seed32);
    for (int i = 0; i < runCountMinMax; i++)
    {
        rr = rnd.NextDouble();

        if (rr < minR)
            minR = rr;
        if (rr > maxR)
            maxR = rr;
        if (rr >= 0.5)
            above++;
    }
    int below = runCountMinMax - above;

    Console.WriteLine($"rtype={RndGenNames[rtype]}, Distribution of {runCountMinMax:N0} numbers.");
    Console.WriteLine($"    Median: Above={above}, Below={below}");
    Console.WriteLine($"    Min/Max: min={min}, Max={max}");
    Console.WriteLine($"    MinR/MaxR: minR={minR:F6}, maxR={maxR:F6}");

    Array.Fill<int>(counts, 0);
    rnd.Reseed(seed32);

    for (int i = 1; i <= runCountGroups; i++)
    {
        r = rnd.Next(rndRange);
        counts[r / groupSize]++;
    }

    Console.WriteLine($"Distribution of {runCountGroups} numbers over {groupCount} groups. range={rndRange}");
    for (int i = 0; i < counts.Length; i++)
    {
        Console.WriteLine($"    Group{i}    {counts[i]}");
    }

    Console.WriteLine();
}

void PlayCraps(uint seed32)
{
    const uint rtype = RandomNumbers.DefaultRNG;

    const int crapsGames = 200*100;
    const double ideal = 244.0 / 495.0;
    const int idealCount = (int)(ideal * (double)(crapsGames));
    const int throwsSize = 21;

    var rnd = new RandomNumbers(seed32, rtype);

    int[] throws = new int[throwsSize];
    Array.Fill<int>(throws, 0);
    int win = 0;
    int loss = 0;
    for (int i = 0; i < crapsGames; i++)
    {
        int d1 = rnd.Next(1, 6+1);
        int d2 = rnd.Next(1, 6+1);
        int sum = d1 + d2;
        int gameThrow = 0;

        if (sum == 2 || sum == 3 || sum == 12)
        {
            loss++;
            throws[0]++;
        }
        else if (sum == 7 || sum == 11)
        {
            win++;
            throws[0]++;
        }
        else
        {
            int point = sum;
            while (true)
            {
                d1 = rnd.Next(1, 6+1);
                d2 = rnd.Next(1, 6+1);
                sum = d1 + d2;
                gameThrow++;
                if (sum == point)
                {
                    win++;
                    if (gameThrow < throwsSize)
                        throws[gameThrow]++;
                    else
                        throws[throwsSize-1]++;//throws[^1]++;// throws[throws.Length-1]++;
                    break;
                }
                else if (sum == 7)
                {
                    loss++;
                    break;
                }
            }
        }
    }

    Console.WriteLine($"rtype={RndGenNames[rtype]}, Craps games played = {crapsGames}. ideal win = {idealCount}, % = {ideal * 100.0:F2}");
    Console.WriteLine($"Wins = {win}, % = {(double)win / (double)crapsGames * 100:F2}");
    Console.WriteLine($"Losses = {loss}, % = {(double)loss / (double)crapsGames * 100:F2}");
    Console.Write("Throws = ");
    for (int i = 0; i < throws.Length; i++)
    {
        Console.Write(throws[i]);
        if (((i+1) % 10) == 0)
            Console.Write("\n         ");
        else
            Console.Write(", ");
    }
    Console.WriteLine();

    Console.WriteLine();
}

void RunsTest(uint seed32)
{
    const uint rtype = RandomNumbers.DefaultRNG;
    const int runsSample = 10 * 1000;
    const int maxRuns = 10;

    var rnd = new RandomNumbers(seed32, rtype);

    Console.WriteLine($"rtype={RndGenNames[rtype]}, Runs tests. Sample={runsSample:N0}");

    int i;

    double[] table = new double[runsSample];
    for (i = 0; i < runsSample; i++)
        table[i] = rnd.NextDouble();

    int[] upCount = new int[maxRuns + 1];
    int[] downCount = new int[maxRuns + 1];
    for (i = 0; i < maxRuns+1; i++)
    {
        upCount[i] = 0;
        downCount[i] = 0;
    }
    int j;
    int thisRun;
    i = 0;
    do
    {
        if (i+1 < runsSample)
        {
            if (table[i] >= 0.5)
            {
                j = i + 1;
                while ((j < runsSample) && (table[j] >= 0.5))
                {
                    j++;
                }
                thisRun = j - i;
                i += thisRun;

                thisRun--;
                if (thisRun > maxRuns)
                    thisRun = maxRuns;
                upCount[thisRun]++;
            }
            else
            {
                j = i + 1;
                while ((j < runsSample) && (table[j] < 0.5))
                {
                    j++;
                }
                thisRun = j - i;
                i += thisRun;

                thisRun--;
                if (thisRun > maxRuns)
                    thisRun = maxRuns;
                downCount[thisRun]++;

            }
        }
        else
        {
            if (i < runsSample)
            {
                if (table[i] >= 0.5)
                    upCount[0]++;
                else
                    downCount[0]++;
            }
            i++;
        }
    } while (i < runsSample);

    Console.WriteLine("Runs Above/Below median.");
    for (i = 0; i < maxRuns+1; i++)
    {
        Console.WriteLine($"    {i+1:D2}: Above={upCount[i]}, Below={downCount[i]}");
    }
    Console.WriteLine();

    for (i = 0; i < maxRuns+1; i++)
    {
        upCount[i] = 0;
        downCount[i] = 0;
    }
    double thresh;
    i = 0;
    do
    {
        if (i + 1 < runsSample)
        {
            if (table[i] <= table[i+1])
            {
                thresh = table[i];
                j = i + 1;
                while ((j < runsSample) && (table[j] >= thresh))
                {
                    thresh = table[j];
                    j++;
                }
                thisRun = j - i;
                i += thisRun;
                
                thisRun--;
                if (thisRun > maxRuns)
                    thisRun = maxRuns;
                upCount[thisRun]++;
            }
            else
            {
                thresh = table[i];
                j = i + 1;
                while ((j < runsSample) && (table[j] <= thresh))
                {
                    thresh = table[j];
                    j++;
                }
                thisRun = j - i;
                i += thisRun;

                thisRun--;
                if (thisRun > maxRuns)
                    thisRun = maxRuns;
                downCount[thisRun]++;

            }
        }
        else
        {
            i++;
        }
    } while (i < runsSample);

    Console.WriteLine("Runs test. Conseq ascend/decend.");
    for (i = 0; i < maxRuns+1; i++)
    {
        Console.WriteLine($"    {i+1:D2}: Ascend={upCount[i]}, Decend={downCount[i]}");
    }

    Console.WriteLine();
}

uint seed32 = RandomNumbers.GetRandomSeed();
//seed32 = 0x12345678;

timing(seed32);
var p = new PatternCheck(seed32, RandomNumbers.DefaultRNG);
p.run();
ShortSequence(seed32);
Density(seed32);
Distribution(seed32);
PlayCraps(seed32);
RunsTest(seed32);

public class PatternCheck
{
    const int RepeatRange = 1000;
    const int Million = 1 * 1000 * 1000;
    const int RepeatLength = 10 * Million;
    bool NumberSaved;
    int LastNumber;
    RandomNumbers rnd;

    public PatternCheck(uint seed32, uint rtype)
    {
        rnd = new RandomNumbers(seed32, rtype);
        NumberSaved = false;
    }

    int GetNext(bool save)
    {
        int r;
        if (save)
        {
            NumberSaved = true;
            r = rnd.Next(RepeatRange);
            LastNumber = r;
        }
        else
        {
            if (NumberSaved)
            {
                NumberSaved = false;
                r = LastNumber;
            }
            else
            {
                r = rnd.Next(RepeatRange);
            }
        }
        return r;
    }

    public void run()
    {
        const int numPattern = 10;

        int[] pattern = new int[numPattern];
        int[] matches = new int[numPattern+1];
        int match;
        int r;

        Console.WriteLine($"Repeat sequence test. ");
        Console.WriteLine($"Random numbers checked = {RepeatLength:N0}");
        Console.WriteLine($"Random Number Range    = {RepeatRange}");

        Array.Fill<int>(pattern, 0);
        Array.Fill<int>(matches, 0);
        pattern[numPattern-1] = RepeatRange;
        for (int i = 0; i < pattern.Length; i++)
            pattern[i] = GetNext(false);

        match = 0;
        NumberSaved = false;
        r = GetNext(false);
        for (int i = 0; i < RepeatLength; i++)
        {
            if (r == pattern[match])
            {
                match++;
                r = GetNext(true);

                if (match == numPattern)
                {
                    Console.Write($"\nRepeating Pattern found at {i}\n");
                    break;
                }
            }
            else
            {
                if (match > 0)
                {
                    if (match <= numPattern)
                        matches[match]++;
                    match = 0;
                }
                r = GetNext(false);
            }
        }

        Console.Write($"\nRepeated sequences 1-{numPattern}\n");
        for (int i= 1; i < matches.Length; i++)
        {
            Console.Write($"{matches[i]} ");
            if (i % 10 == 0)
                Console.WriteLine();
        }
        Console.WriteLine("\n");
    }
}


