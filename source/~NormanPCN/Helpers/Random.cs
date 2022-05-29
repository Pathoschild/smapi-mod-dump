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
    public class RandomNumbers
    {
        public const uint XorShiftWow = 0;
        public const uint XorShiftPlus = 1;
        public const uint NR_Ranq1 = 2;
        public const uint NR_Ran = 3;
        public const uint DefaultRNG = XorShiftWow;

        private uint genType;

        // state variables
        private uint xorw_v;
        private uint xorw_w;
        private uint xorw_z;
        private uint xorw_y;
        private uint xorw_x;
        private uint incr;
        private ulong ran_u;
        private ulong ran_v;
        private ulong ran_w;
        private ulong xorp_0;
        private ulong xorp_1;

        private const double uintToDouble = 2.32830643653869629E-10;// 1.0 / 2*32
        private const double ulongToDouble = 5.42101086242752217E-20;// 1.0 / 2**64

        //private delegate ulong RandomNumberFunc();
        //private RandomNumberFunc randFunc;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint XorShift32(uint seed)
        {
            seed ^= seed << 13;
            seed ^= seed >> 17;
            seed ^= seed << 5;
            return seed;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong XorShift64(ulong seed)
        {
            seed ^= seed << 13;
            seed ^= seed >> 7;
            seed ^= seed << 17;
            return seed;
        }

        /// <summary>
        /// generate a random seed value. based on the current system time, process and thread ids.
        /// </summary>
        /// <returns>random seed value of type uint</returns>
        public static uint GetRandomSeed()
        {
            long seed = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            //DateTime epoc = new DateTime(2000, 1, 1);
            //long seed = (ulong)(DateTime.UtcNow - epoc).TotalSeconds;

            seed += Environment.CurrentManagedThreadId;
            seed += Environment.ProcessId;
            seed += Environment.TickCount64;
            //seed += System.Threading.Thread.CurrentThread.ManagedThreadId;
            //seed += System.Diagnostics.Process.GetCurrentProcess().Id;


            return (uint)XorShift64((ulong)seed);
        }

        public RandomNumbers() : this(GetRandomSeed(), DefaultRNG)
        {
        }

        public RandomNumbers(uint seed, uint genType = DefaultRNG)
        {
            this.genType = genType;

            if ((genType < XorShiftWow) || (genType > NR_Ran))
                throw new ArgumentOutOfRangeException(nameof(genType));

            //switch (genType)
            //{
            //    case XorShiftWow:
            //        this.randFunc = xorwow;
            //        break;
            //    case XorShiftPlus:
            //        this.randFunc = xorp;
            //        break;
            //    case NR_Ranq1:
            //        this.randFunc = Ranq1;
            //        break;
            //    case NR_Ran:
            //        this.randFunc = Ran;
            //        break;
            //    default:
            //        throw new ArgumentOutOfRangeException(nameof(genType));
            //}

            Reseed(seed);
        }

        // both flags for best performance in resulting code.
        // the optimization flag seems to make the real diff (it seems to do both?).
        // what is "regular" optimization?  these are pretty simple/trivial procs, why "aggressive" needed?
        // these procs are so short and fast inlining is important for performance.

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private uint xorwow()
        {
            uint s = xorw_v;
            uint t = xorw_x;

            xorw_x = xorw_y;
            xorw_y = xorw_z;
            xorw_z = xorw_w;
            xorw_w = s;

            unchecked
            {
                t ^= t >> 2;
                t ^= t << 1;
                t ^= s ^ (s << 4);
                xorw_v = t;
                incr += 362437;
                return t + incr;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private ulong xorp()
        {
            ulong t = xorp_0;
            ulong s = xorp_1;

            unchecked
            {
                xorp_0 = s;
                t ^= t << 23;
                t ^= t >> 18;
                t ^= s ^ (s >> 5);
                xorp_1 = t;

                return t + s;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private ulong Ranq1()
        {
            // a so called XorShift64* algorithm
            ulong v = ran_v;
            unchecked
            {
                v ^= v >> 21;
                v ^= v << 35;
                v ^= v >> 4;
                ran_v = v;
                return v * 2685821657736338717;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private ulong Ran()
        {
            ulong u = ran_u;
            ulong v = ran_v;
            ulong w = ran_w;

            unchecked
            {
                u = u * 2862933555777941757 + 7046029254386353087;
                ran_u = u;
                v ^= v >> 17;
                v ^= v << 31;
                v ^= v >> 8;
                ran_v = v;
                w = 4294957665U * (w & 0xffffffff) + (w >> 32);
                ran_w = w;
                ulong x = u ^ (u << 21);
                x ^= x >> 35;
                x ^= x << 4;
                return (x + v) ^ w;
            }
        }

        /// <summary>
        /// reseed, reinit, the random number generator using the new seed value.<br/>
        /// if seed==0, then a random seed value is generated.
        /// </summary>
        /// <param name="seed">if seed==0, then a random seed value is generated.</param>
        /// <exception cref="InvalidOperationException"></exception>
        public void Reseed(uint seed)
        {
            if (seed == 0)
                seed = GetRandomSeed();

            switch (genType)
            {
                case XorShiftWow:
                    incr = 6615241;
                    seed = XorShift32(seed);
                    xorw_x = seed;
                    seed = XorShift32(seed);
                    xorw_y = seed;
                    seed = XorShift32(seed);
                    xorw_z = seed;
                    seed = XorShift32(seed);
                    xorw_w = seed;
                    seed = XorShift32(seed);
                    xorw_v = seed;
                    return;
                case XorShiftPlus:
                    xorp_0 = XorShift64((ulong)seed ^ 4101842887655102017);
                    xorp_1 = XorShift64(xorp_0);
                    return;
                case NR_Ranq1:
                    ran_v = (ulong)(seed) ^ 4101842887655102017;
                    ran_v = Ranq1();
                    return;
                case NR_Ran:
                    ran_v = 4101842887655102017;
                    ran_w = 1;
                    ran_u = (ulong)seed ^ ran_v;
                    Ran();
                    ran_v = ran_u;
                    Ran();
                    ran_w = ran_v;
                    Ran();
                    return;
                default:
                    throw new InvalidOperationException("genType invalid");
            }

        }

        /// <summary>
        /// return a floating point random number in the range [0..1.0)
        /// </summary>
        /// <returns>double in the range [0..1.0)</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public double NextDouble()
        {
            switch (genType)
            {
                // double only has 52 explicit bits in mantissa. thus low bits of a long are unused.
                case XorShiftWow:
                    return (double)xorwow() * uintToDouble;
                case XorShiftPlus:
                    return (double)xorp() * ulongToDouble;
                case NR_Ranq1:
                    return (double)Ranq1() * ulongToDouble;
                case NR_Ran:
                    return (double)Ran() * ulongToDouble;
                default:
                    throw new InvalidOperationException("genType invalid");
            }
        }

        /// <summary>
        /// returns a random number in the full positive range of Int32
        /// </summary>
        /// <returns>int in a range [0..Int32.MaxValue]</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public int Next()
        {
            switch (genType)
            {
                // return signed int positive range.
                // take middle bits of long results.
                case XorShiftWow:
                    return (int)(xorwow() & 0x7fffffff);
                case XorShiftPlus:
                    return (int)((xorp() >> 8) & 0x7fffffff);
                case NR_Ranq1:
                    return (int)((Ranq1() >> 8) & 0x7fffffff);
                case NR_Ran:
                    return (int)((Ran() >> 8) & 0x7fffffff);
                default:
                    throw new InvalidOperationException("genType invalid");
            }
            
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private int rndRange(uint range)
        {
            // inline can only happen with very plain code. if/else, no loops, no exceptions. gen procs have no branches/loops.
            //
            // double only has 52 explicit bits in mantissa. thus low bits of a long are unused.
            // decent compiler should do (ulong)uint * (ulong)uint, and shift efficiently.
            // no 128-bit int so we do the float thing with ulong results.
            //     otherwise (int128)ulong * (int128)ulong >> 64. decent compiler still needed.
            //     also, would only expect an int128 to be avail in a 64-bit specific target. p-code is agnostic
            //
            // it would be faster to take a 32-bit subrange of a ulong result and do the int mul range thing.
            //     using the full range random does lower bias with larger ranges.
            if (genType == XorShiftWow)
            {
                return (int)(((ulong)xorwow() * (ulong)range) >> 32);
            }
            else if (genType == XorShiftPlus)
            {
                return (int)((double)xorp() * ulongToDouble * (double)range);
                //return (int)(((ulong)((uint)(xorp() >> 8)) * (ulong)range) >> 32);
                //return (int)(xorp() % (ulong)range);
            }
            else if (genType == NR_Ranq1)
            {
                return (int)((double)Ranq1() * ulongToDouble * (double)range);
                //return (int)(((ulong)((uint)(Ranq1() >> 8)) * (ulong)range) >> 32);
                //return (int)(Ranq1() % (ulong)range);
            }
            else// if (genType == NR_Ran)
            {
                return (int)((double)Ran() * ulongToDouble * (double)range);
                //return (int)(((ulong)((uint)(Ran() >> 8)) * (ulong)range) >> 32);
                //return (int)(Ran() % (ulong)range);
            }
        }

        /// <summary>
        /// returns a random number in limited bounded range.
        /// </summary>
        /// <param name="maxValue"></param>
        /// <returns>int in a range [0..maxValue)</returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public int Next(int maxValue)
        {
            if ((genType >= XorShiftWow) && (genType <= NR_Ran))
            {
                if (maxValue > 0)
                {
                    return rndRange((uint)maxValue);
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(maxValue));
                }
            }
            else
            {
                throw new InvalidOperationException("genType invalid");
            }
        }

        /// <summary>
        /// returns a random number in a limited bounded range.<br/>
        /// (maxValue-minValue) &lt;= Int32.MaxValue
        /// </summary>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns>int in a range [minValue..maxValue)</returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public int Next(int minValue, int maxValue)
        {
            if ((genType >= XorShiftWow) && (genType <= NR_Ran))
            {
                if (minValue < maxValue)
                {
                    uint range = (uint)((long)maxValue - (long)minValue);
                    if (range <= (uint)Int32.MaxValue)
                    {
                        return rndRange(range) + minValue;
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
            else
            {
                throw new InvalidOperationException("genType invalid");
            }
        }

        private int unbiasedRange32(uint range, Func<uint> rndNum)
        {
            // negate unsigned number. an identity/trick. -range = (2**32 - range). without double precision math.
            // negate is promoted so we have to trunc it down. decent compiler should do that well.

            // Lemire unbiased mult moduluo algorithm. https://github.com/lemire/fastrange
            // second opt (threshold tweak) found at https://www.pcg-random.org/posts/bounded-rands.html

            uint x = rndNum();
            ulong m = (ulong)x * (ulong)range;
            uint leftover = (uint)m;
            if (leftover < range)
            {
                uint threshold = (uint)(-range) % range;
                // threshold tweak
                //uint threshold = (uint)-range;
                //if (threshold >= range)
                //{
                //    threshold -= range;
                //    if (threshold >= range)
                //        threshold %= range;
                //}
                while (leftover < threshold)
                {
                    x = rndNum();
                    m = (ulong)x * (ulong)range;
                    leftover = (uint)m;
                }
            }
            return (int)(m >> 32);
            //uint x, r;
            //do
            //{
            //    x = rndNum();
            //    r = x % range;
            //} while ((x - r) > (uint)-range);
            //return (int)r;
        }

        private int unbiasedRange64(uint range, Func<ulong> rndNum)
        {
            // have a func using the full rnd64 source simply reduces the occurance of bias.
            //     and thus reduces the frequency of this code looping.
            // negate unsigned number. an identity/trick. -range = (2**32 - range). without double precision math.
            // negate is promoted so we have to trunc it down. decent compiler should do that well.
            ulong x;
            uint r;
            do
            {
                x = rndNum();
                r = (uint) (x % (ulong)range);
            } while ((uint)(x - r) > (uint)-range);

            return (int)r;
        }

        /// <summary>
        /// returns a random number within a range. the result has no modulo bias.<br/>
        /// modulo bias often does not matter.<br/>
        /// only when the requested range is high relative to the full native range of the random number generator.
        /// </summary>
        /// <param name="maxValue"></param>
        /// <returns>a number in the range [minValue..maxValue)</returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public int NextU(int maxValue)
        {
            if (maxValue > 0)
            {
                switch (genType)
                {
                    case XorShiftWow:
                        return unbiasedRange32((uint)maxValue, xorwow);
                    case XorShiftPlus:
                        return unbiasedRange64((uint)maxValue, xorp);
                        //return unbiasedRange32((uint)maxValue, () => (uint)(xorp() >> 8));
                    case NR_Ranq1:
                        return unbiasedRange64((uint)maxValue, Ranq1);
                        //return unbiasedRange32((uint)maxValue, () => (uint)(Ranq1() >> 8));
                    case NR_Ran:
                        return unbiasedRange64((uint)maxValue, Ran);
                        //return unbiasedRange32((uint)maxValue, () => (uint)(Ran() >> 8));
                    default:
                        throw new InvalidOperationException("genType invalid");
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(maxValue));
            }
        }

        /// <summary>
        /// returns a random number within a range. the result has no modulo bias.<br/>
        /// modulo bias often does not matter.<br/>
        /// only when the requested range is high relative to the full native range of the random number generator.
        /// maxValue-minValue must be &lt;= Int32.MaxValue
        /// </summary>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns>a number in the range [minValue..maxValue)</returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public int NextU(int minValue, int maxValue)
        {
            if (minValue < maxValue)
            {
                uint range = (uint)((long)maxValue - (long)minValue);
                if (range <= (uint)Int32.MaxValue)
                {
                    switch (genType)
                    {
                        case XorShiftWow:
                            return unbiasedRange32(range, xorwow) + minValue;
                        case XorShiftPlus:
                            return unbiasedRange64(range, xorp) + minValue;
                            //return unbiasedRange32(range, () => (uint)(xorp() >> 8)) + minValue;
                        case NR_Ranq1:
                            return unbiasedRange64(range, Ranq1) + minValue;
                            //return unbiasedRange32(range, () => (uint)(Ranq1() >> 8)) + minValue;
                        case NR_Ran:
                            return unbiasedRange64(range, Ran) + minValue;
                            //return unbiasedRange32(range, () => (uint)(Ran() >> 8)) + minValue;
                        default:
                            throw new InvalidOperationException("genType invalid");
                    }
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

        public void NextBytes(byte[] buffer)
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));

            int i = 0;
            int l = buffer.Length;
            while (i < l)
            {
                ulong d;
                int b = 8;
                switch (genType)
                {
                    case XorShiftWow:
                        d = xorwow();
                        b = 4;
                        break;
                    case XorShiftPlus:
                        d = xorp();
                        break;
                    case NR_Ranq1:
                        d = Ranq1();
                        break;
                    case NR_Ran:
                        d = Ran();
                        break;
                    default:
                        throw new InvalidOperationException("genType invalid");
                }
                for (int j = 0; j < b; i++)
                {
                    if (i < l)
                    {
                        buffer[i] = (byte)(d & 0xff);
                        d >>= 8;
                        i++;
                    }
                }
            }
        }
    }
}
