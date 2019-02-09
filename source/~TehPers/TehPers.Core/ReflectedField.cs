using System;
using System.Linq;
using System.Reflection;

namespace TehPers.Core {
    public class ReflectedField<TObject, TField> {
        public FieldInfo Field { get; }
        public TObject Owner { get; }

        public TField Value {
            get => (TField) this.Field.GetValue(this.Owner);
            set => this.Field.SetValue(this.Owner, value);
        }

        public ReflectedField(TObject owner, string field) : this(owner, field, BindingFlags.NonPublic | BindingFlags.Instance) { }

        public ReflectedField(TObject owner, string field, BindingFlags flags) {
            this.Field = typeof(TObject).GetFields(flags).FirstOrDefault(f => f.Name == field && f.FieldType == typeof(TField));
            this.Owner = owner;

            if (this.Field == null) {
                throw new ArgumentException("Field not found", nameof(field));
            }
        }
    }
}
