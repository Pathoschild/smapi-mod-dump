/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/DialogueExtension
**
*************************************************/

using JetBrains.Annotations;
using LightInject;

namespace SDV.Shared.Abstractions.Utility
{
  public class WrapperFactory : IWrapperFactory
  {
    public TInterface CreateInstance<TInterface>()
      => (TInterface) _serviceFactory.GetInstance(typeof(TInterface));

    public TInterface CreateInstance<TInterface>(object item)
      => (TInterface)_serviceFactory.GetInstance(typeof(TInterface), new []{item});

    public TInterface CreateInstance<TInterface>(params object[] args)
    {
      return (TInterface) _serviceFactory.GetInstance(typeof(TInterface), args);
    }

    [NotNull] private readonly IServiceFactory _serviceFactory;
    public WrapperFactory([NotNull] IServiceFactory factory) => _serviceFactory = factory;
    //public WrapperFactory() => _serviceFactory = new ServiceContainer();
  }
}
