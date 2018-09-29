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