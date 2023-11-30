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
using System.Reflection;
using BirbCore.Extensions;
using HarmonyLib;
using StardewModdingAPI;

namespace BirbCore.Attributes;
public class Parser
{
    public static event EventHandler? Priority1Event;
    internal static event EventHandler? Priority2Event;
    internal static event EventHandler? Priority3Event;
    internal static event EventHandler? Priority4Event;
    internal static event EventHandler? Priority5Event;
    internal static event EventHandler? Priority6Event;
    internal static event EventHandler? Priority7Event;
    internal static event EventHandler? Priority8Event;
    internal static event EventHandler? Priority9Event;

    /// <summary>
    /// Parse a SMAPI mod for BirbCore attributes. Does a variety of setup for SMAPI and Stardew Valley related objects,
    /// including setting up Assets with the content pipeline, creating command line commands, initializing config files,
    /// loading and saving data to properties, creating events and delegates, initializing APIs, and more.
    /// </summary>
    /// <param name="mod">The mod being parsed.</param>
    /// <param name="assembly">The assembly to scan for attributes. Defaults to the assembly calling ParseAll.</param>
    public static void ParseAll(IMod mod, Assembly? assembly = null)
    {
        assembly ??= Assembly.GetCallingAssembly();

        Log.Init(mod.Monitor, assembly);

        foreach (Type type in assembly.GetTypes())
        {
            foreach (Attribute attribute in type.GetCustomAttributes())
            {
                if (attribute is ClassHandler handler)
                {
                    switch (handler.Priority)
                    {
                        case 0: handler.Handle(type, null, mod); break;
                        case 1: Priority1Event += (sender, e) => handler.Handle(type, null, mod); break;
                        case 2: Priority2Event += (sender, e) => handler.Handle(type, null, mod); break;
                        case 3: Priority3Event += (sender, e) => handler.Handle(type, null, mod); break;
                        case 4: Priority4Event += (sender, e) => handler.Handle(type, null, mod); break;
                        case 5: Priority5Event += (sender, e) => handler.Handle(type, null, mod); break;
                        case 6: Priority6Event += (sender, e) => handler.Handle(type, null, mod); break;
                        case 7: Priority7Event += (sender, e) => handler.Handle(type, null, mod); break;
                        case 8: Priority8Event += (sender, e) => handler.Handle(type, null, mod); break;
                        case 9: Priority9Event += (sender, e) => handler.Handle(type, null, mod); break;
                    }
                }
            }
        }

        new Harmony(mod.ModManifest.UniqueID).PatchAll(assembly);
    }

    internal static void InitEvents()
    {
        ModEntry.Instance.Helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
        ModEntry.Instance.Helper.Events.GameLoop.UpdateTicking += GameLoop_UpdateTicking;
        ModEntry.Instance.Helper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;
    }

    private static void GameLoop_GameLaunched(object? sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
    {
        Priority1Event?.Invoke(sender, new EventArgs());
    }

    private static void GameLoop_UpdateTicking(object? sender, StardewModdingAPI.Events.UpdateTickingEventArgs e)
    {
        switch (e.Ticks)
        {
            case 0: Priority2Event?.Invoke(sender, new EventArgs()); break;
            case 1: Priority4Event?.Invoke(sender, new EventArgs()); break;
            case 2: Priority6Event?.Invoke(sender, new EventArgs()); break;
            case 3: Priority8Event?.Invoke(sender, new EventArgs()); break;
            default: ModEntry.Instance.Helper.Events.GameLoop.UpdateTicking -= GameLoop_UpdateTicking; break;
        }
    }

    private static void GameLoop_UpdateTicked(object? sender, StardewModdingAPI.Events.UpdateTickedEventArgs e)
    {
        switch (e.Ticks)
        {
            case 0: Priority3Event?.Invoke(sender, new EventArgs()); break;
            case 1: Priority5Event?.Invoke(sender, new EventArgs()); break;
            case 2: Priority7Event?.Invoke(sender, new EventArgs()); break;
            case 3: Priority9Event?.Invoke(sender, new EventArgs()); break;
            default: ModEntry.Instance.Helper.Events.GameLoop.UpdateTicked -= GameLoop_UpdateTicked; break;
        }
    }
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public abstract class ClassHandler : Attribute
{
    public int Priority = 0;

    public ClassHandler(int priority = 0)
    {
        this.Priority = priority;
    }

    public virtual void Handle(Type type, object? instance, IMod mod, object[]? args = null)
    {
        foreach (FieldInfo fieldInfo in type.GetFields(ReflectionExtensions.AllDeclared))
        {
            foreach (Attribute attribute in fieldInfo.GetCustomAttributes())
            {
                if (attribute is FieldHandler handler)
                {
                    handler.Handle(fieldInfo, instance, mod, args);
                }
            }
        }
        foreach (PropertyInfo propertyInfo in type.GetProperties(ReflectionExtensions.AllDeclared))
        {
            foreach (Attribute attribute in propertyInfo.GetCustomAttributes())
            {
                if (attribute is FieldHandler handler)
                {
                    handler.Handle(propertyInfo, instance, mod, args);
                }
            }
        }
        foreach (MethodInfo method in type.GetMethods(ReflectionExtensions.AllDeclared))
        {
            foreach (Attribute attribute in method.GetCustomAttributes())
            {
                if (attribute is MethodHandler handler)
                {
                    handler.Handle(method, instance, mod, args);
                }
            }
        }
    }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public abstract class MethodHandler : Attribute
{
    public abstract void Handle(MethodInfo method, object? instance, IMod mod, object[]? args = null);
}

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public abstract class FieldHandler : Attribute
{
    public void Handle(FieldInfo fieldInfo, object? instance, IMod mod, object[]? args = null)
    {
        this.Handle(fieldInfo.Name, fieldInfo.FieldType, fieldInfo.GetValue, fieldInfo.SetValue, instance, mod, args);
    }

    public void Handle(PropertyInfo propertyInfo, object? instance, IMod mod, object[]? args = null)
    {
        this.Handle(propertyInfo.Name, propertyInfo.PropertyType, propertyInfo.GetValue, propertyInfo.SetValue, instance, mod, args);
    }

    public abstract void Handle(string name, Type fieldType, Func<object?, object?> getter, Action<object?, object?> setter, object? instance, IMod mod, object[]? args = null);
}
