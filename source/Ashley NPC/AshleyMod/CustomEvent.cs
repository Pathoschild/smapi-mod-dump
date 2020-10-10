/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Club559/AshleyMod
**
*************************************************/

using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AshleyMod
{
  public class CustomEvent : Event
  {
    public int EventID = -1;

    public CustomEvent(string eventString, int eventID = -1)
      :base(eventString, eventID)
    {
      this.EventID = eventID;
    }
    
    public CustomEvent()
      :base()
    {

    }
  }
}
