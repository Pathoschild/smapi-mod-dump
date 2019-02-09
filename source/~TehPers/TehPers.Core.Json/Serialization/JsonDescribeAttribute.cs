using System;

namespace TehPers.Core.Json.Serialization {
    /// <summary>Indicates that a <see cref="DescriptiveJsonConverter" /> should add comments based on <see cref="System.ComponentModel.DescriptionAttribute"/>.</summary>
    /// <inheritdoc />
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class JsonDescribeAttribute : Attribute { }
}
