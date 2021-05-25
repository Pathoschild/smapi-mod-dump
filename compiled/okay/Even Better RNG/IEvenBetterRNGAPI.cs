using System;

namespace EvenBetterRNG
    {
    public interface IEvenBetterRNGAPI
        {
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


        }
    }
