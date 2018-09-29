using System;
using Igorious.StardewValley.DynamicAPI.Interfaces;

namespace Igorious.StardewValley.DynamicAPI.Data.Supporting
{
    public sealed class DynamicTypeInfo
    {
        public int ClassID { get; }
        public Type BaseType { get; }

        private DynamicTypeInfo(int classID, Type baseType)
        {
            ClassID = classID;
            BaseType = baseType;
        }

        public static DynamicTypeInfo Create<TBaseClass>(int classID) where TBaseClass : IDynamic
        {
            return new DynamicTypeInfo(classID, typeof(TBaseClass));
        }
    }
}