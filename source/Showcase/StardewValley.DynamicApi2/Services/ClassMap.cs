/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/Stardew_Valley_Showcase_Mod
**
*************************************************/

using System;
using System.Collections.Generic;
using StardewValley.Objects;

namespace Igorious.StardewValley.DynamicApi2.Services
{
    public sealed class ClassMap
    {
        private static readonly Lazy<ClassMap> Lazy = new Lazy<ClassMap>(() => new ClassMap());
        public static ClassMap Instance => Lazy.Value;
        private ClassMap() { }

        private readonly Dictionary<int, Type> _furnitureMapping = new Dictionary<int, Type>();
        public IReadOnlyDictionary<int, Type> FurnitureMapping => _furnitureMapping;

        public ClassMap Furniture<T>(int id) where T : Furniture
        {
            _furnitureMapping.Add(id, typeof(T));
            return this;
        }
    }
}