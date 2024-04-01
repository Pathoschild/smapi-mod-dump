/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Annosz/UIInfoSuite2
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Reflection;

/// Reflector#GetPropertyGetter<TValue> provides cached readonly access to properties through reflection.
/// Where TValue can be a supertype of the actual property type.
/// Based on SMAPI's Reflector class.
namespace UIInfoSuite2.Infrastructure.Reflection;

public interface IReflectedGetProperty<TValue>
{
  PropertyInfo PropertyInfo { get; }

  TValue GetValue();
}

public class ReflectedGetProperty<TValue> : IReflectedGetProperty<TValue>
{
  private readonly string DisplayName;
  private readonly Func<TValue>? GetMethod;

  public ReflectedGetProperty(Type parentType, object? obj, PropertyInfo property, bool isStatic)
  {
    // validate input
    if (parentType == null)
    {
      throw new ArgumentNullException(nameof(parentType));
    }

    if (property == null)
    {
      throw new ArgumentNullException(nameof(property));
    }

    // validate static
    if (isStatic && obj != null)
    {
      throw new ArgumentException("A static property cannot have an object instance.");
    }

    if (!isStatic && obj == null)
    {
      throw new ArgumentException("A non-static property must have an object instance.");
    }


    DisplayName = $"{parentType.FullName}::{property.Name}";
    PropertyInfo = property;

    if (PropertyInfo.GetMethod != null)
    {
      try
      {
        GetMethod = (Func<TValue>)Delegate.CreateDelegate(typeof(Func<TValue>), obj, PropertyInfo.GetMethod);
      }
      catch (ArgumentException)
      {
        if (PropertyInfo.PropertyType.IsEnum)
        {
          Type enumType = PropertyInfo.PropertyType;
          GetMethod = delegate
          {
            var enumDelegate = Delegate.CreateDelegate(
              typeof(Func<>).MakeGenericType(Enum.GetUnderlyingType(enumType)),
              obj,
              PropertyInfo.GetMethod
            );
            return (TValue)Enum.ToObject(enumType, enumDelegate.DynamicInvoke()!);
          };
        }
        else
        {
          throw;
        }
      }
    }
  }

  public PropertyInfo PropertyInfo { get; }

  public TValue GetValue()
  {
    if (GetMethod == null)
    {
      throw new InvalidOperationException($"The {DisplayName} property has no get method.");
    }

    try
    {
      return GetMethod();
    }
    catch (InvalidCastException)
    {
      throw new InvalidCastException(
        $"Can't convert the {DisplayName} property from {PropertyInfo.PropertyType.FullName} to {typeof(TValue).FullName}."
      );
    }
    catch (Exception ex)
    {
      throw new Exception($"Couldn't get the value of the {DisplayName} property", ex);
    }
  }
}

public class Reflector
{
  private readonly IntervalMemoryCache<string, MemberInfo?> Cache = new();

  public void NewCacheInterval()
  {
    Cache.StartNewInterval();
  }

  public IReflectedGetProperty<TValue> GetPropertyGetter<TValue>(object obj, string name, bool required = true)
  {
    // validate
    if (obj == null)
    {
      throw new ArgumentNullException(nameof(obj), "Can't get a instance property from a null object.");
    }

    // get property from hierarchy
    IReflectedGetProperty<TValue>? property = GetGetPropertyFromHierarchy<TValue>(
      obj.GetType(),
      obj,
      name,
      BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public
    );
    if (required && property == null)
    {
      throw new InvalidOperationException(
        $"The {obj.GetType().FullName} object doesn't have a '{name}' instance property."
      );
    }

    return property!;
  }

  private IReflectedGetProperty<TValue>? GetGetPropertyFromHierarchy<TValue>(
    Type type,
    object? obj,
    string name,
    BindingFlags bindingFlags
  )
  {
    bool isStatic = bindingFlags.HasFlag(BindingFlags.Static);
    PropertyInfo? property = GetCached(
      'p',
      type,
      name,
      isStatic,
      () =>
      {
        for (Type? curType = type; curType != null; curType = curType.BaseType)
        {
          PropertyInfo? propertyInfo = curType.GetProperty(name, bindingFlags);
          if (propertyInfo != null)
          {
            type = curType;
            return propertyInfo;
          }
        }

        return null;
      }
    );

    return property != null ? new ReflectedGetProperty<TValue>(type, obj, property, isStatic) : null;
  }

  private TMemberInfo? GetCached<TMemberInfo>(
    char memberType,
    Type type,
    string memberName,
    bool isStatic,
    Func<TMemberInfo?> fetch
  ) where TMemberInfo : MemberInfo
  {
    var key = $"{memberType}{(isStatic ? 's' : 'i')}{type.FullName}:{memberName}";
    return (TMemberInfo?)Cache.GetOrSet(key, fetch);
  }

  public class IntervalMemoryCache<TKey, TValue> where TKey : notnull
  {
    private Dictionary<TKey, TValue> HotCache = new();
    private Dictionary<TKey, TValue> StaleCache = new();

    public TValue GetOrSet(TKey cacheKey, Func<TValue> get)
    {
      // from hot cache
      if (HotCache.TryGetValue(cacheKey, out TValue? value))
      {
        return value;
      }

      // from stale cache
      if (StaleCache.TryGetValue(cacheKey, out value))
      {
        HotCache[cacheKey] = value;
        return value;
      }

      // new value
      value = get();
      HotCache[cacheKey] = value;
      return value;
    }

    public void StartNewInterval()
    {
      StaleCache.Clear();
      if (HotCache.Count is not 0)
      {
        (StaleCache, HotCache) = (HotCache, StaleCache); // swap hot cache to stale
      }
    }
  }
}
