/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/DialogueExtension
**
*************************************************/

namespace SDV.Shared.Abstractions
{
  public interface IWrappedType<T> where T : class
  {
    T GetBaseType { get; }
    //void SetBaseType(T item);
  }
}
