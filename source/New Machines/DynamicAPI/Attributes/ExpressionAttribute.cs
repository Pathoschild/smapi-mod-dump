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
