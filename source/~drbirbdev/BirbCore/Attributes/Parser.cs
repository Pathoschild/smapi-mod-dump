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
    internal static event EventHandler? Priority1Event;
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
    public static void ParseAll(IMod mod)
    {
        Assembly assembly = mod.GetType().Assembly;

        Log.Init(mod.Monitor, assembly);

        foreach (Type type in assembly.GetTypes())
        {
            ClassHandler[] classHandlers = (ClassHandler[])Attribute.GetCustomAttributes(type, typeof(ClassHandler));
            if (classHandlers.Length == 0)
            {
                continue;
            }

            object? instance = Activator.CreateInstance(type);
            foreach (ClassHandler handler in classHandlers)
            {
                switch (handler.Priority)
                {
                    case 0:
                        handler.Handle(type, instance, mod);
                        break;
                    case 1:
                        Priority1Event += (sender, e) => WrapHandler(handler, type, instance, mod);
                        break;
                    case 2:
                        Priority2Event += (sender, e) => WrapHandler(handler, type, instance, mod);
                        break;
                    case 3:
                        Priority3Event += (sender, e) => WrapHandler(handler, type, instance, mod);
                        break;
                    case 4:
                        Priority4Event += (sender, e) => WrapHandler(handler, type, instance, mod);
                        break;
                    case 5:
                        Priority5Event += (sender, e) => WrapHandler(handler, type, instance, mod);
                        break;
                    case 6:
                        Priority6Event += (sender, e) => WrapHandler(handler, type, instance, mod);
                        break;
                    case 7:
                        Priority7Event += (sender, e) => WrapHandler(handler, type, instance, mod);
                        break;
                    case 8:
                        Priority8Event += (sender, e) => WrapHandler(handler, type, instance, mod);
                        break;
                    case 9:
                        Priority9Event += (sender, e) => WrapHandler(handler, type, instance, mod);
                        break;
                }
            }
        }

        new Harmony(mod.ModManifest.UniqueID).PatchAll(assembly);
    }

    private static void WrapHandler(ClassHandler handler, Type type, object? instance, IMod mod)
    {
        try
        {
            handler.Handle(type, instance, mod);
        }
        catch (Exception e)
        {
            mod.Monitor.Log($"BirbCore failed to parse {handler.GetType().Name} class {type}: {e}", LogLevel.Error);
        }
    }

    internal static void InitEvents()
    {
        ModEntry.Instance.Helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
        ModEntry.Instance.Helper.Events.GameLoop.UpdateTicking += GameLoop_UpdateTicking;
        ModEntry.Instance.Helper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;
    }

    private static void GameLoop_GameLaunched(object? sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
    {
        Log.Trace("=== Running Priority 1 events ===");
        Priority1Event?.Invoke(sender, EventArgs.Empty);
    }

    private static void GameLoop_UpdateTicking(object? sender, StardewModdingAPI.Events.UpdateTickingEventArgs e)
    {
        switch (e.Ticks)
        {
            case 0:
                Log.Trace("=== Running Priority 2 events ===");
                Priority2Event?.Invoke(sender, EventArgs.Empty);
                break;
            case 1:
                Log.Trace("=== Running Priority 4 events ===");
                Priority4Event?.Invoke(sender, EventArgs.Empty);
                break;
            case 2:
                Log.Trace("=== Running Priority 6 events ===");
                Priority6Event?.Invoke(sender, EventArgs.Empty);
                break;
            case 3:
                Log.Trace("=== Running Priority 8 events ===");
                Priority8Event?.Invoke(sender, EventArgs.Empty);
                break;
            default:
                ModEntry.Instance.Helper.Events.GameLoop.UpdateTicking -= GameLoop_UpdateTicking;
                break;
        }
    }

    private static void GameLoop_UpdateTicked(object? sender, StardewModdingAPI.Events.UpdateTickedEventArgs e)
    {
        switch (e.Ticks)
        {
            case 0:
                Log.Trace("=== Running Priority 3 events ===");
                Priority3Event?.Invoke(sender, EventArgs.Empty);
                break;
            case 1:
                Log.Trace("=== Running Priority 5 events ===");
                Priority5Event?.Invoke(sender, EventArgs.Empty);
                break;
            case 2:
                Log.Trace("=== Running Priority 7 events ===");
                Priority7Event?.Invoke(sender, EventArgs.Empty);
                break;
            case 3:
                Log.Trace("=== Running Priority 9 events ===");
                Priority9Event?.Invoke(sender, EventArgs.Empty);
                break;
            default:
                ModEntry.Instance.Helper.Events.GameLoop.UpdateTicked -= GameLoop_UpdateTicked;
                break;
        }
    }
}

[AttributeUsage(AttributeTargets.Class)]
public abstract class ClassHandler(int priority = 0) : Attribute
{
    public int Priority = priority;

    public virtual void Handle(Type type, object? instance, IMod mod, object[]? args = null)
    {
        string className = this.ToString() ?? "";
        foreach (FieldInfo fieldInfo in type.GetFields(ReflectionExtensions.ALL_DECLARED))
        {
            foreach (Attribute attribute in fieldInfo.GetCustomAttributes())
            {
                string attributeName = attribute.ToString() ?? "";
                if (attribute is FieldHandler handler && attributeName.StartsWith(className))
                {
                    try
                    {
                        handler.Handle(fieldInfo, instance, mod, args);
                    }
                    catch (Exception e)
                    {
                        mod.Monitor.Log($"BirbCore failed to parse {handler.GetType().Name} field {fieldInfo.Name}: {e}", LogLevel.Error);
                    }
                }
            }
        }

        foreach (PropertyInfo propertyInfo in type.GetProperties(ReflectionExtensions.ALL_DECLARED))
        {
            foreach (Attribute attribute in propertyInfo.GetCustomAttributes())
            {
                string attributeName = attribute.ToString() ?? "";
                if (attribute is FieldHandler handler && attributeName.StartsWith(className))
                {
                    try
                    {
                        handler.Handle(propertyInfo, instance, mod, args);
                    }
                    catch (Exception e)
                    {
                        mod.Monitor.Log($"BirbCore failed to parse {handler.GetType().Name} property {propertyInfo.Name}: {e}", LogLevel.Error);
                    }
                }
            }
        }

        foreach (MethodInfo method in type.GetMethods(ReflectionExtensions.ALL_DECLARED))
        {
            foreach (Attribute attribute in method.GetCustomAttributes())
            {
                string attributeName = attribute.ToString() ?? "";
                if (attribute is MethodHandler handler && attributeName.StartsWith(className))
                {
                    try
                    {
                        handler.Handle(method, instance, mod, args);
                    }
                    catch (Exception e)
                    {
                        mod.Monitor.Log($"BirbCore failed to parse {handler.GetType().Name} method {method.Name}: {e}", LogLevel.Error);
                    }
                }
            }
        }
    }
}

[AttributeUsage(AttributeTargets.Method)]
public abstract class MethodHandler : Attribute
{
    public abstract void Handle(MethodInfo method, object? instance, IMod mod, object[]? args = null);
}

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public abstract class FieldHandler : Attribute
{
    public void Handle(FieldInfo fieldInfo, object? instance, IMod mod, object[]? args = null)
    {
        this.Handle(fieldInfo.Name, fieldInfo.FieldType, fieldInfo.GetValue, fieldInfo.SetValue, instance, mod, args);
    }

    public void Handle(PropertyInfo propertyInfo, object? instance, IMod mod, object[]? args = null)
    {
        this.Handle(propertyInfo.Name, propertyInfo.PropertyType, propertyInfo.GetValue, propertyInfo.SetValue,
            instance, mod, args);
    }

    protected abstract void Handle(string name, Type fieldType, Func<object?, object?> getter,
        Action<object?, object?> setter, object? instance, IMod mod, object[]? args = null);
}
