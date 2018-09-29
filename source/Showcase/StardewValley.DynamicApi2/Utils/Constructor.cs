using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Igorious.StardewValley.DynamicApi2.Utils
{
    public abstract class ConstructorBase
    {
        private ConstructorInfo ConstructorInfo { get; }
        protected Delegate Delegate { get; }

        protected ConstructorBase(Type type, params Type[] args)
        {
            ConstructorInfo = type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, args, null);
            Delegate = CreateDelegate(ConstructorInfo);
        }

        protected abstract Type GetDelegateType();

        public abstract object Invoke(params object[] args);

        private Delegate CreateDelegate(ConstructorInfo constructor)
        {
            var constructorParam = constructor.GetParameters();
            var method = new DynamicMethod(string.Empty, constructor.DeclaringType, constructorParam.Select(p => p.ParameterType).ToArray());
            var gen = method.GetILGenerator();
            for (var i = 0; i < constructorParam.Length; ++i)
            {
                switch (i)
                {
                    case 0: gen.Emit(OpCodes.Ldarg_0); break;
                    case 1: gen.Emit(OpCodes.Ldarg_1); break;
                    case 2: gen.Emit(OpCodes.Ldarg_2); break;
                    case 3: gen.Emit(OpCodes.Ldarg_3); break;
                    default: gen.Emit(OpCodes.Ldarg_S, i); break;
                }
            }
            gen.Emit(OpCodes.Newobj, constructor);
            gen.Emit(OpCodes.Ret);
            return method.CreateDelegate(GetDelegateType());
        }
    }

    public sealed class Constructor<TClass> : ConstructorBase, IConstructor<TClass>
    {
        public Constructor() : this(typeof(TClass)) { }
        public Constructor(Type type) : base(type) { }
        public TClass Invoke() => ((Func<TClass>)Delegate).Invoke();
        public override object Invoke(params object[] args) => Invoke();
        protected override Type GetDelegateType() => typeof(Func<TClass>);
    }

    public sealed class Constructor<TArg, TClass> : ConstructorBase, IConstructor<TArg, TClass>
    {
        public Constructor() : this(typeof(TClass)) { }
        public Constructor(Type type) : base(type, typeof(TArg)) { }
        public TClass Invoke(TArg arg) => ((Func<TArg, TClass>)Delegate).Invoke(arg);
        public override object Invoke(params object[] args) => Invoke((TArg)args[0]);
        protected override Type GetDelegateType() => typeof(Func<TArg, TClass>);
    }

    public sealed class Constructor<TArg1, TArg2, TClass> : ConstructorBase, IConstructor<TArg1, TArg2, TClass>
    {
        public Constructor() : this(typeof(TClass)) { }
        public Constructor(Type type) : base(type, typeof(TArg1), typeof(TArg2)) { }
        public TClass Invoke(TArg1 arg1, TArg2 arg2) => ((Func<TArg1, TArg2, TClass>)Delegate).Invoke(arg1, arg2);
        public override object Invoke(params object[] args) => Invoke((TArg1)args[0], (TArg2)args[1]);
        protected override Type GetDelegateType() => typeof(Func<TArg1, TArg2, TClass>);
    }
}