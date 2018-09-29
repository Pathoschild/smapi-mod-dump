using System;
using System.Collections.Generic;
using Igorious.StardewValley.DynamicApi2.Contracts;
using Igorious.StardewValley.DynamicApi2.Extensions;
using Igorious.StardewValley.DynamicApi2.Utils;
using StardewValley.Objects;
using Object = StardewValley.Object;

namespace Igorious.StardewValley.DynamicApi2.Services
{
    public sealed class Wrapper
    {
        private static readonly Lazy<Wrapper> Lazy = new Lazy<Wrapper>(() => new Wrapper());
        public static Wrapper Instance => Lazy.Value;
        private Wrapper() { }

        private Dictionary<Type, IConstructor<int, Object>> CustomTypeConstructors { get; } = new Dictionary<Type, IConstructor<int, Object>>();
        private Dictionary<Type, IConstructor<Object>> BasicTypeConstructors { get; } = new Dictionary<Type, IConstructor<Object>>();

        public Object Wrap(Object basicObject)
        {
            if (basicObject == null) return null;

            basicObject.heldObject = Wrap(basicObject.heldObject);

            var customType = TryGetType(basicObject);
            if (customType == null || basicObject.GetType() == customType) return basicObject;

            var customCtor = CustomTypeConstructors.GetOrAddValue(customType, () => new Constructor<int, Object>(customType));
            var customObject = customCtor.Invoke(basicObject.ParentSheetIndex);
            Cloner.Instance.CopyData(basicObject, customObject);
            (customObject as IInitializable)?.Initialize();
            return customObject;
        }

        public Object Unwrap(Object customObject)
        {
            if (customObject == null) return null;

            customObject.heldObject = Unwrap(customObject.heldObject);

            var customType = TryGetType(customObject);
            if (customObject.GetType() != customType) return customObject;

            IConstructor<Object> basicCtor;
            switch (customObject)
            {
                case Furniture furniture:
                    basicCtor = GetBasicCtor<Furniture>();
                    break;
                default:
                    basicCtor = GetBasicCtor<Object>();
                    break;
            }

            var basicObject = basicCtor.Invoke();
            Cloner.Instance.CopyData(customObject, basicObject);
            return basicObject;
        }

        private IConstructor<Object> GetBasicCtor<TBasicType>() where TBasicType : Object
        {
            return BasicTypeConstructors.GetOrAddValue(typeof(TBasicType), () => new Constructor<TBasicType>());
        }

        private Type TryGetType(Object o)
        {
            var repository = ClassMap.Instance;
            switch (o)
            {
                case Furniture furniture:
                    return repository.FurnitureMapping.TryGetValue(o.ParentSheetIndex, out Type type) ? type : null;
                default:
                    return null;
            }
        }
    }
}