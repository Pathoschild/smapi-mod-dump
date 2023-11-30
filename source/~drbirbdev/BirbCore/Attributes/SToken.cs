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
using System.Collections.Generic;
using System.Reflection;
using BirbCore.APIs;
using StardewModdingAPI;

namespace BirbCore.Attributes;

/// <summary>
/// Specifies a method or class as a content patcher simple or advanced token.
/// </summary>
public class SToken : ClassHandler
{
    private static IContentPatcherApi? Api;

    public SToken() : base(2)
    {

    }

    public override void Handle(Type type, object? instance, IMod mod, object[]? args = null)
    {
        if (this.Priority < 1)
        {
            Log.Error("Tokens cannot be loaded with priority < 1");
            return;
        }

        Api = mod.Helper.ModRegistry.GetApi<IContentPatcherApi>("Pathoschild.ContentPatcher");
        if (Api == null)
        {
            Log.Error("Content Patcher is not enabled, so will skip parsing");
            return;
        }
        base.Handle(type, null, mod);

        return;
    }

    public class Token : MethodHandler
    {
        public override void Handle(MethodInfo method, object? instance, IMod mod, object[]? args = null)
        {
            if (Api == null)
            {
                Log.Error("Content Patcher is not enabled, so will skip parsing");
                return;
            }
            Api.RegisterToken(mod.ModManifest, method.Name, method.CreateDelegate<Func<IEnumerable<string>>>(instance));
        }
    }

    public class AdvancedToken : ClassHandler
    {
        public override void Handle(Type type, object? instance, IMod mod, object[]? args = null)
        {
            instance = Activator.CreateInstance(type);
            if (instance is null)
            {
                Log.Error("Content Patcher advanced api requires an instance of token class. Provided token class may be static?");
                return;
            }
            base.Handle(type, instance, mod);
            if (Api == null)
            {
                Log.Error("Content Patcher is not enabled, so will skip parsing");
                return;
            }
            Api.RegisterToken(mod.ModManifest, type.Name, instance);

            return;
        }
    }
}
