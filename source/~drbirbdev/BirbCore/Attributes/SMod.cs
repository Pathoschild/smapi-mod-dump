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
public class SMod : ClassHandler
{
    public SMod() : base(1)
    {

    }

    public override void Handle(Type type, object? instance, IMod mod, object[]? args = null)
    {
        if (this.Priority < 1)
        {
            Log.Error("ModEntry cannot be loaded with priority < 1");
            return;
        }
        base.Handle(type, mod, mod, args);
    }

    public class Api : FieldHandler
    {
        public string UniqueID;
        public bool IsRequired;

        public Api(string uniqueID, bool isRequired = true)
        {
            this.UniqueID = uniqueID;
            this.IsRequired = isRequired;
        }

        public override void Handle(string name, Type fieldType, Func<object?, object?> getter, Action<object?, object?> setter, object? instance, IMod mod, object[]? args = null)
        {
            object? api = mod.Helper.ModRegistry.GetType().GetMethod("GetApi", 1, new Type[] { typeof(string) })
                ?.MakeGenericMethod(fieldType)
                .Invoke(mod.Helper.ModRegistry, new object[] { this.UniqueID });
            if (api is null && this.IsRequired)
            {
                Log.Error($"[{name}] Can't access required API");
            }
            setter(instance, api);
        }
    }

    public class Instance : FieldHandler
    {
        public override void Handle(string name, Type fieldType, Func<object?, object?> getter, Action<object?, object?> setter, object? instance, IMod mod, object[]? args = null)
        {
            setter(instance, mod);
        }
    }
}
