using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue | AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class TupleElementNamesAttribute : Attribute {
        public IList<string> TransformNames { get; }

        public TupleElementNamesAttribute(string[] transformNames) {
            this.TransformNames = transformNames ?? throw new ArgumentNullException(nameof(transformNames));
        }

        public TupleElementNamesAttribute() {
            this.TransformNames = new List<string>();
        }
    }
}
