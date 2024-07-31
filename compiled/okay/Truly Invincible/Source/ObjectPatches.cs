using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrulyInvincible
{
  public class ObjectPatches
  {
    private static IMonitor Monitor;

    // call this method from your Entry class
    public static void Initialize(IMonitor monitor)
    {
      Monitor = monitor;
    }

    public static bool TakeDamage_Prefix()
    {
      return false;
    }

    public static void enterMineShaft_Prefix(out int __state)
    {
      __state = Game1.player.health;
    }

    public static void enterMineShaft_Postfix(int __state)
    {
      Game1.player.health = __state;
    }
  }
}
