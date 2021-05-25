/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://sourceforge.net/p/sdvmod-even-better-rng/
**
*************************************************/

/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using StardewModdingAPI;

using ThreadMonitor = System.Threading.Monitor;

namespace EvenBetterRNG
    {
    public class EvenBetterRNGAPI: IEvenBetterRNGAPI
        {
        internal static IMonitor Monitor;
        internal readonly static Dictionary<string, Random> NamedPRNGs = new Dictionary<string, Random>();
        internal readonly static object LockNamedDict = new object();

        public Random GetNamedRandom(string name) {
            try {
                ThreadMonitor.Enter(LockNamedDict);
                if (!NamedPRNGs.ContainsKey(name)) NamedPRNGs.Add(name, GetNewRandom());
                }
            finally {
                ThreadMonitor.Exit(LockNamedDict);
                }
            return NamedPRNGs[name];
            }

        public Random GetNamedRandom(string name, long seed) {
            try {
                ThreadMonitor.Enter(LockNamedDict);
                if (!NamedPRNGs.ContainsKey(name)) NamedPRNGs.Add(name, GetNewRandom(seed));
                }
            finally {
                ThreadMonitor.Exit(LockNamedDict);
                }
            return NamedPRNGs[name];
            }

        public Random GetNewRandom() {
            Monitor?.Log("GetNewRandom() invoked via API");
            return (Random)Activator.CreateInstance(EvenBetterRNG.PRNG_Class);
            }

        public Random GetNewRandom(int seed) {
            Monitor?.Log($"GetNewRandom({seed}) invoked via API");
            return (Random)Activator.CreateInstance(EvenBetterRNG.PRNG_Class, (long)seed);
            }

        public Random GetNewRandom(long seed) {
            Monitor?.Log($"GetNewRandom({seed}) invoked via API");
            return (Random)Activator.CreateInstance(EvenBetterRNG.PRNG_Class, seed);
            }
        }
    }
