/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/DialogueExtension
**
*************************************************/

namespace SDV.Shared.Abstractions.Utility
{
  public interface IWrapperFactory
  {
    TInterface CreateInstance<TInterface>();
    TInterface CreateInstance<TInterface>(object item);
    TInterface CreateInstance<TInterface>(params object[] args);
  }
}