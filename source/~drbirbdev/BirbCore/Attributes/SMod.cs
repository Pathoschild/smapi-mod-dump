/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

#nullable enable
using System;
using StardewModdingAPI;

namespace BirbCore.Attributes;
public class SMod() : ClassHandler(1)
{
    public override void Handle(Type type, object? instance, IMod mod, object[]? args = null)
    {
        if (this.Priority < 1)
        {
            Log.Error("ModEntry cannot be loaded with priority < 1");
            return;
        }
        base.Handle(type, mod, mod, args);
    }

    public class Api(string uniqueId, bool isRequired = true) : FieldHandler
    {
        public bool IsRequired = isRequired;

        protected override void Handle(string name, Type fieldType, Func<object?, object?> getter, Action<object?, object?> setter, object? instance, IMod mod, object[]? args = null)
        {
            object? api = mod.Helper.ModRegistry.GetType().GetMethod("GetApi", 1, [typeof(string)])
                ?.MakeGenericMethod(fieldType)
                .Invoke(mod.Helper.ModRegistry, [uniqueId]);
            if (api is null && this.IsRequired)
            {
                Log.Error($"[{name}] Can't access required API");
            }
            setter(instance, api);
        }
    }

    public class Instance : FieldHandler
    {
        protected override void Handle(string name, Type fieldType, Func<object?, object?> getter, Action<object?, object?> setter, object? instance, IMod mod, object[]? args = null)
        {
            setter(instance, mod);
        }
    }
}
