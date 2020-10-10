/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/StardevValleyNewMachinesMod
**
*************************************************/

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