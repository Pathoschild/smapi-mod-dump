/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/DialogueExtension
**
*************************************************/

using Microsoft.Xna.Framework.Input;
using StardewValley;

namespace SDV.Shared.Abstractions
{
  public class ResponseWrapper : IResponseWrapper
  {
    public string responseKey;
    public string responseText;
    public Keys hotkey;

    public IResponseWrapper SetHotKey(Keys key)
    {
      this.hotkey = key;
      return this;
    }

    public Response GetBaseType { get; }
    public ResponseWrapper(Response item) => GetBaseType = item;
  }
}