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
