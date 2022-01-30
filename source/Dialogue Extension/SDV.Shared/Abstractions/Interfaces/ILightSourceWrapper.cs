/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/DialogueExtension
**
*************************************************/

using Netcode;
using StardewValley;

namespace SDV.Shared.Abstractions
{
  public interface ILightSourceWrapper : IWrappedType<LightSource>
  {
    int Identifier { get; set; }
    long PlayerID { get; set; }
    NetFields NetFields { get; }
    ILightSourceWrapper Clone();
  }
}