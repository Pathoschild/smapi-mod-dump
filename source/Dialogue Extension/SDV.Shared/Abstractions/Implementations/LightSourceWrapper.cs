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
  public class LightSourceWrapper : ILightSourceWrapper
  {
    public LightSourceWrapper(LightSource item) => GetBaseType = item;
    public LightSource GetBaseType { get; }
    public int Identifier { get; set; }
    public long PlayerID { get; set; }
    public NetFields NetFields { get; }
    public ILightSourceWrapper Clone() => null;
  }
}
