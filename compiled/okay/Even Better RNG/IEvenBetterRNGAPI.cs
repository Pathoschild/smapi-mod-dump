﻿/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;

namespace EvenBetterRNG
    {
    public interface IEvenBetterRNGAPI
        {
        /// <summary>
        /// Get the main XoShiRo PRNG instance that continually replace Game1.random
        /// </summary>
        /// <returns>Main XoShiRo PRNG that adheres to System.Random interface</returns>
        Random GetMainRandom();

        /// <summary>
        /// Get a new XoShiRo PRNG. Seed will be automatically determined by the XoShiRoPRNG.Net library
        /// </summary>
        /// <returns>New PRNG that adheres to System.Random interface</returns>
        Random GetNewRandom();

        /// <summary>
        /// Get a new XoShiRo PRNG with specified seed, for replicability purpose.
        /// </summary>
        /// <param name="seed">A 32-bit number that will be used as PRNG seed</param>
        /// <returns>New PRNG that adheres to System.Random interface</returns>
        Random GetNewRandom(int seed);

        /// <summary>
        /// Get a new XoShiRo PRNG with specified seed, for replicability purpose.
        /// </summary>
        /// <param name="seed">A 64-bit number that will be used as PRNG seed</param>
        /// <returns>New PRNG that adheres to System.Random interface</returns>
        Random GetNewRandom(long seed);

        /// <summary>
        /// Get a named XoShiRo PRNG. If not found, create one. Seed determined by library
        /// </summary>
        /// <param name="name">A string with which the PRNG will be named</param>
        /// <returns>A PRNG object associated with the name</returns>
        Random GetNamedRandom(string name);

        /// <summary>
        /// Get a named XoShiRo PRNG. If not found, create one with specified seed.
        /// </summary>
        /// <param name="name">A string with which the PRNG will be named</param>
        /// <param name="seed">The seed with which to create the PRNG if needed</param>
        /// <returns>A PRNG object associated with the name</returns>
        Random GetNamedRandom(string name, long seed);

        /// <summary>
        /// Try registering a callback to be invoked when EBRNG has finished overriding Daily Luck
        /// </summary>
        /// <param name="callback">A void function with null parameters</param>
        /// <returns>True if successful, false if otherwise</returns>
        bool TryRegisterDailyLuckOverrideCallback(Action callback);

        /// <summary>
        /// Try advancing internal RNG state in a secure, unpredictable way.
        /// </summary>
        /// <param name="rng">RNG to advance</param>
        void SecurelyAdvanceRNGState(Random rng);
        }
    }
