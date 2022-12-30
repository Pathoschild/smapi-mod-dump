/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Tlitookilakin/AeroCore
**
*************************************************/

using AeroCore.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace AeroCore.ReflectedValue
{
    public interface IStaticValue<T> : IValue<T>
    {
        public T Get();
        public void Set(T value);
    }
    public interface IValue<T>
    {
        public T Get(object owner);
        public void Set(object owner, T value);
    }
    public interface IValue : IValue<object> { }
    public struct StaticProperty<T> : IStaticValue<T>
    {
        private readonly MethodInfo Getter;
        private readonly MethodInfo Setter;
        internal StaticProperty(Type owner, string name)
        {
            Getter = owner.PropertyGetter(name);
            Setter = owner.PropertySetter(name);
        }
        public T Get() => (T)(Getter?.Invoke(null, Array.Empty<object>()) ?? default(T));
        public void Set(T value) => Setter?.Invoke(null, new object[] {value});
        public T Get(object owner) => Get();
        public void Set(object owner, T value) => Set(value);
    }
    public struct StaticField<T> : IStaticValue<T>
    {
        private readonly FieldInfo Field;
        internal StaticField(Type owner, string name)
        {
            Field = owner.FieldNamed(name);
        }
        public T Get() => (T)(Field?.GetValue(null) ?? default(T));
        public void Set(T value) => Field?.SetValue(null, value);
        public T Get(object owner) => Get();
        public void Set(object owner, T value) => Set(value);
    }
    public struct InstanceProperty<T> : IValue<T>
    {
        private readonly MethodInfo Getter;
        private readonly MethodInfo Setter;
        internal InstanceProperty(Type owner, string name)
        {
            Getter = owner.PropertyGetter(name);
            Setter = owner.PropertySetter(name);
        }
        public T Get(object instance) => (T)(Getter?.Invoke(instance, Array.Empty<object>()) ?? default(T));
        public void Set(object instance, T value) => Setter?.Invoke(instance, new object[] { value });
    }
    public struct InstanceField<T> : IValue<T>
    {
        private readonly FieldInfo Field;
        internal InstanceField(Type owner, string name)
        {
            Field = owner.FieldNamed(name);
        }
        public T Get(object instance) => (T)(Field?.GetValue(instance) ?? default(T));
        public void Set(object instance, T value) => Field?.SetValue(instance, value);
    }
    public class ValueChain : IValue
    {
        private readonly MethodInfo GetValue;
        private readonly MethodInfo SetValue;
        private readonly FieldInfo Field;
        private readonly IValue<object> Parent;
        private readonly object[] Args = Array.Empty<object>();
        private readonly Type Output;
        private readonly bool IsField = false;
        internal ValueChain(Type owner, MethodInfo getter, object[] args = null, IValue<object> parent = null)
        {
            Parent = parent;
            GetValue = getter;
            Output = getter?.ReturnType;
            if (args is not null)
                Args = args;
            // cannot set value on method refs (duh)
        }
        internal ValueChain(Type owner, FieldInfo field, IValue<object> parent = null)
        {
            Parent = parent;
            Field = field;
            Output = field?.FieldType;
            IsField = true;
        }
        internal ValueChain(Type owner, string name, IValue<object> parent = null)
        {
            Parent = parent;
            Field = owner.FieldNamed(name);
            GetValue = owner.PropertyGetter(name);
            SetValue = owner.PropertySetter(name);
            IsField = Field is not null;
            Output = IsField ? Field.FieldType : GetValue.ReturnType;
            if (Output is null)
                throw new NullReferenceException($"Type '{owner.FullName}' does not contain field or property '{name}'.");
        }
        private object GetSource(object owner)
            => Parent is null ? owner : Parent.Get(owner);
        public T Get<T>(object owner)
            => (T)(IsField ? Field.GetValue(GetSource(owner)) : GetValue.Invoke(GetSource(owner), Args));
        public void Set<T>(object owner, T value) 
        { 
            if (IsField)
                Field.SetValue(GetSource(owner), value);
            else
                SetValue?.Invoke(GetSource(owner), new object[] {value});
        }
        public object Get(object owner) => Get<object>(owner);
        public void Set(object owner, object value) => Set<object>(owner, value);
        public T Call<T>(object owner, string method, params object[] args)
            => method is null && !IsField ?
            (T)GetValue.Invoke(owner, args) :
            (T)Output.MethodNamed(method)?.Invoke(Get(owner), args);
        public void Call(object owner, string method, params object[] args)
        {
            if (method is null && !IsField)
                GetValue.Invoke(owner, args);
            else
                Output.MethodNamed(method)?.Invoke(Get(owner), args);
        }
        public ValueChain ValueRef<V>(string name)
            => new(Output, name, this);
        public ValueChain MethodRef<V>(string name, Type[] arg_types, params object[] args)
        {
            var method = arg_types is null ? Output.MethodNamed(name) : Output.MethodNamed(name, arg_types);
            if (method is null)
                throw new NullReferenceException($"Type '{Output.FullName}' does not contain method '{name}'.");
            else
                return new(Output, method, args, this);
        }
    }
}
