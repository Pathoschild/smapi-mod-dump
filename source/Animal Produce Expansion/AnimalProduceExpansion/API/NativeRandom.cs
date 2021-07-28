/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/AnimalProduceExpansion
**
*************************************************/

using System;

namespace AnimalProduceExpansion.API
{
  public class NativeRandom : IRandomApi
  {
    public Random GetNewRandom() => new Random();
    public Random GetNewRandom(int seed) => new Random(seed);

    public Random GetNewRandom(long seed) => new Random();
    public Random GetNamedRandom(string name) => new Random();
    public Random GetNamedRandom(string name, long seed) => new Random();
  }
}