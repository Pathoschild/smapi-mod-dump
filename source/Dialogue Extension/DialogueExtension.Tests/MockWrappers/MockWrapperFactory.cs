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
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SDV.Shared.Abstractions;
using SDV.Shared.Abstractions.Utility;

namespace DialogueExtension.Tests.MockWrappers
{
  public class MockWrapperFactory : IWrapperFactory
  {
    public TInterface CreateInstance<TInterface>() => CreateInstance<TInterface>(new object[0]);

    public TInterface CreateInstance<TInterface>(object item) => CreateInstance<TInterface>(new []{item});

    public TInterface CreateInstance<TInterface>(params object[] args)
    {
      if (_factories.ContainsKey(typeof(TInterface)))
        return (TInterface)_factories[typeof(TInterface)].DynamicInvoke((IEnumerable<object>)args);
      throw new KeyNotFoundException($"Type {typeof(TInterface)} not found");
    }

    [NotNull] private readonly IDictionary<Type,Func<IEnumerable<object>, object>> _factories = new Dictionary<Type, Func<IEnumerable<object>, object>>()
    {
      { typeof(ICharacterWrapper), new Func<IEnumerable<object>,ICharacterWrapper>(objects => new MockCharacterWrapper(objects)) },
      { typeof(IFriendshipWrapper), new Func<IEnumerable<object>,IFriendshipWrapper>(objects => new MockFriendshipWrapper(objects)) },
      { typeof(IGameWrapper), new Func<IEnumerable<object>,IGameWrapper>(objects => new MockGameWrapper(objects)) },
      { typeof(INPCWrapper), new Func<IEnumerable<object>,INPCWrapper>(objects => new MockNPCWrapper(objects)) },
      { typeof(IDialogueWrapper), new Func<IEnumerable<object>, IDialogueWrapper>(objects => new MockDialogueWrapper(objects))}
    };

    public void SetInstance<TInterface>(Func<IEnumerable<object>, object> setter)
    {
      if (_factories.ContainsKey(typeof(TInterface)))
        _factories[typeof(TInterface)] = setter;

    }
  }
}
