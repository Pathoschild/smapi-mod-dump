using System;

namespace TehPers.CoreMod.Api.Json {
    /// <summary>Indicates that the property should be commented based on its <see cref="System.ComponentModel.DescriptionAttribute"/>.</summary>
    /// <inheritdoc />
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class JsonDescribeAttribute : Attribute { }
}
