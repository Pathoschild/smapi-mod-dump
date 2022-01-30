/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/DialogueExtension
**
*************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using JetBrains.Annotations;

namespace SDV.Shared.Abstractions.Utility
{
  public abstract class DynamicStaticWrapper<T> : DynamicObject where T : class
  {
    [NotNull] private readonly IDictionary<string, Delegate> _dictFunctions = new Dictionary<string, Delegate>();
    [NotNull] private readonly IDictionary _dictProperties = new Hashtable();

    public override bool TrySetMember(SetMemberBinder binder, object value)
    {
      try
      {
        if (value is Delegate @delegate)
          _dictFunctions[binder.Name.ToLower()] = @delegate;
        else
          _dictProperties[binder.Name.ToLower()] = value;
        return true;
      }
      catch
      {
        return false;
      }
    }

    public override bool TryGetMember(GetMemberBinder binder, out object result)
    {
      try
      {
        if (_dictProperties.Contains(binder.Name.ToLower()))
        {
          result = _dictProperties[binder.Name.ToLower()];
          return true;
        }

        result = typeof(T).InvokeMember(binder.Name, BindingFlags.Static, null, null, null);
        return true;
      }
      catch
      {
        result = default;
        return false;
      }
    }

    public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
    {
      try
      {
        if (_dictFunctions.ContainsKey(binder.Name.ToLower()))
        {
          result = _dictFunctions[binder.Name.ToLower()].DynamicInvoke(args);
          return true;
        }

        result = "this is a method";
        result = typeof(T).InvokeMember(binder.Name, BindingFlags.Static, null, null, args);
        return true;
      }
      catch
      {
        result = default;
        return false;
      }
    }
  }
}