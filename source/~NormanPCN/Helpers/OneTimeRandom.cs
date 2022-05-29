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
using System.Runtime.CompilerServices;

namespace NormanPCN.Utils
{
    /// <summary>
    /// numerical recipes...<br/>
    /// Every once in a while, you want a random sequence H(i) whose values you can visit or revisit in any order of i’s.<br/>
    /// That is to say, you want a random hash of the integers i, one that passes serious tests for randomness, even for very ordered sequences of i’s.
    /// <br/>
    /// this class provides 32 and 64-bit integer calculations for a randomized result.<br/>
    /// Using the 64-bit ulong seed methods use the 64-bit calculations.<br/>
    /// Using the 32-bit uint seed methods use the 32-bit calculations.<br/>
    /// </summary>
    public static class OneTimeRandom
    {
        private const double uintToDouble = 2.32830643653869629E-10;// 1.0 / 2*32
        private const double ulongToDouble = 5.42101086242752217E-20;// 1.0 / 2**64

        // both flags for best performance in resulting code.
        // the optimization flag seems to make the real diff (it seems to do both).
        // these procs are so short inlining is important for performance.

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private static uint RanHash32(uint seed)
        {
            // different generators feeding into the next.
            // LCG->XorShift->MLCG->XorShift

            // 32-bit using numerical recipes rec values for sub functions
            unchecked
            {
                uint v = (seed * 1372383749) + 1289706101; // I1
                v ^= v << 13; // G1
                v ^= v >> 17;
                v ^= v << 5;
                v *= 1597334677; //J1
                v ^= v >> 9; //G3
                v ^= v << 17;
                v ^= v >> 6;
                return v;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private static ulong RanHash64(ulong seed)
        {
            // different generators feeding into the next.
            // LCG->XorShift->MLCG->XorShift

            // verbatum from Numerical recipes Ranhash.
            unchecked
            {
                ulong v = (seed * 3935559000370003845) + 2691343689449507681; //C1
                v ^= v >> 21; //A7
                v ^= v << 37;
                v ^= v >> 4;
                v *= 4768777513237032717; //D3
                v ^= v << 20; //A2
                v ^= v >> 41;
                v ^= v << 5;
                return v;
            }
        }

        /// <summary>
        /// return a floating point random number in the range [0..1.0).<br/>
        /// performs 32-bit calculations.
        /// </summary>
        /// <param name="seed">the seed value to generate the random number</param>
        /// <returns>double in the range [0..1.0)</returns>
        public static double RndDouble(uint seed)
        {
            return RanHash32(seed) * uintToDouble;
        }

        /// <summary>
        /// return a floating point random number in the range [0..1.0).<br/>
        /// performs 64-bit calculations.
        /// </summary>
        /// <param name="seed">the seed value to generate the random number</param>
        /// <returns>double in the range [0..1.0)</returns>
        public static double RndDouble(ulong seed)
        {
            return RanHash64(seed) * ulongToDouble;
        }

        /// <summary>
        /// return a positive integer random number in a range.<br/>
        /// performs 32-bit calculations.
        /// </summary>
        /// <param name="seed">the seed value to generate the random number</param>
        /// <param name="range">0=full positive integer range, otherwise the range is [0..range)</param>
        /// <returns>range=0=full positive integer range. <br/>otherwise the range is [0..range)</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static int Rnd(uint seed, int range)
        {
            if (range >= 0)
            {
                uint v = RanHash32(seed);
                if (range == 0)
                    return (int)(v & 0x7fffffff);// return the positive 32-bit signed integer range
                else
                    return (int)(((ulong)v * (ulong)range) >> 32);//decent compiler should do this efficiently.
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(range), "range < 0");
            }
        }

        /// <summary>
        /// return a positive integer random number in a range.<br/>
        /// performs 64-bit calculations.
        /// </summary>
        /// <param name="seed">the seed value to generate the random number</param>
        /// <param name="range">0=full positive integer range, otherwise the range is [0..range)</param>
        /// <returns>range=0 returns the full positive integer range. <br/>otherwise the range is [0..range)</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static int Rnd(ulong seed, int range)
        {
            if (range >= 0)
            {
                ulong v = RanHash64(seed);
                if (range == 0)
                    return (int)((v >> 8) & 0x7fffffff);// return the positive 32-bit signed integer range. take middle bits.
                else
                    // double only has 52 explicit bits in mantissa. thus low bits of a long are unused.
                    // no 128-bit int, we do the float thing. otherwise (int128)ulong * (int128)ulong >> 64
                    //     would only expect an int128 avail in a 64-bit mode specific target. p-code is agnostic
                    return (int)((double)v * ulongToDouble * (double)range);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(range), "range < 0");
            }
        }

        /// <summary>
        /// return a positive integer random number in a range.<br/>
        /// (maxValue-minValue) &lt;= Int32.MaxValue.<br/>
        /// performs 32-bit calculations.
        /// </summary>
        /// <param name="seed">the seed value to generate the random number</param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns>integer in the range [minValue..maxValue)</returns>
        /// <exception cref="ArgumentException"></exception>
        public static int Rnd(uint seed, int minValue, int maxValue)
        {
            if (minValue < maxValue)
            {
                long range = (long)maxValue - (long)minValue;
                if (range <= (long)Int32.MaxValue)
                {
                    // decent compiler should do (ulong)uint * (ulong)uint, and shift efficiently.
                    return (int)(((ulong)RanHash32(seed) * (ulong)range) >> 32) + minValue;
                }
                else
                {
                    throw new ArgumentException("range too large");
                }
            }
            else
            {
                throw new ArgumentException("minValue >= maxValue");
            }
        }

        /// <summary>
        /// return a positive integer random number in a range.<br/>
        /// maxValue-minValue &lt;= Int32.MaxValue.<br/>
        /// performs 64-bit calculations.
        /// </summary>
        /// <param name="seed">the seed value to generate the random number</param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns>integer in the range [minValue..maxValue)</returns>
        /// <exception cref="ArgumentException"></exception>
        public static int Rnd(ulong seed, int minValue, int maxValue)
        {
            if (minValue < maxValue)
            {
                long range = (long)maxValue - (long)minValue;
                if (range <= (long)Int32.MaxValue)
                {
                    // double only has 52 explicit bits in mantissa. thus low bits of a long are unused.
                    // it would be faster to take a 32-bit subrange of a ulong result and do the int mul range thing.
                    //     just possibly more bias with very large ranges relative to 32-bit.
                    // no 128-bit int, so we do the float thing with ulong results.
                    //     otherwise (int128)ulong * (int128)ulong >> 64. decent compiler still needed.
                    //     also, would only expect an int128 to be avail in a 64-bit specific target. p-code is agnostic
                    return (int)((double)RanHash64(seed) * ulongToDouble * (double)range) + minValue;
                    //return (int)(((ulong)(uint)(RanHash64(seed) >> 8) * (ulong)range) >> 32) + minValue;
                }
                else
                {
                    throw new ArgumentException("range too large");
                }
            }
            else
            {
                throw new ArgumentException("minValue >= maxValue");
            }
        }
    }
}
