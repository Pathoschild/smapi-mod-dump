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

namespace Igorious.StardewValley.DynamicAPI.Attributes
{
    /// <summary>
    /// Marks property as expression with specific parameters.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ExpressionAttribute : Attribute
    {
        public Type DelegateType { get; }

        public ExpressionAttribute(Type delegateType)
        {
            DelegateType = delegateType;
        }
    }
}
