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
using System.Collections.Generic;
using System.Linq;

namespace AnimalProduceExpansion.API
{
  internal static class Api
  {
    private static readonly IDictionary<Type, object> RegisteredApis = new Dictionary<Type, object>();

    internal static T GetApi<T>() =>
      (T) RegisteredApis.First(a => a.Key == typeof(T)).Value;

    internal static void RegisterApi<T>(this T api) =>
      RegisteredApis.Add(typeof(T), api);
  }
}