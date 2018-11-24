using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

//
// smcq - Seed Maker Crop Quality
// Forked from "Better Quality More Seeds"
//
// Author: mcoocr
// Original Author: Space Baby
//
// Version 1.0 23.11.2018 mcoocr: initial release
// Version 1.1 23.11.2018 mcoocr: possible bugfix, game removes object during tick event handling causing NullReferenceException (using List with object reference instead of Dictionary)
//

namespace smcq
{
  public class ManagedSeedMaker
  {
    public StardewValley.Object refSeedMaker;
    public StardewValley.Object refDroppedObject;
    public bool hasBeenChecked;
    public bool isDeprecated;

    public ManagedSeedMaker(StardewValley.Object seedMaker, StardewValley.Object droppedObject, bool isChecked)
    {
      refSeedMaker = seedMaker;
      refDroppedObject = droppedObject;
      hasBeenChecked = isChecked;
      isDeprecated = false;
    }
  }

  public class Smcq : StardewModdingAPI.Mod
  {
    List<ManagedSeedMaker> arSeedMakers;

    StardewValley.Object previousHeldItem = null;
    GameLocation previousLocation = null;
    bool isInitialized = false;

    public override void Entry(IModHelper helper)
    {
      SaveEvents.AfterLoad += InitializeMod;
      SaveEvents.AfterReturnToTitle += ResetMod;
      GameEvents.UpdateTick += ModUpdate;

      arSeedMakers = new List<ManagedSeedMaker>();
      arSeedMakers.Clear();

      isInitialized = false;
    }

    private void InitializeMod(object sender, EventArgs e)
    {
      previousLocation = Game1.player.currentLocation;
      isInitialized = true;
    }

    private void ResetMod(object sender, EventArgs e)
    {
      isInitialized = false;
    }

    private void ModUpdate(object sender, EventArgs e)
    {
      if (!isInitialized)
        return;

      if ((Game1.player.currentLocation == null) || (Game1.player.currentLocation.name == null))
        return;

      if ((previousLocation.name != Game1.player.currentLocation.name)) // get Seed Makers in current location, save them to array
      {
        arSeedMakers.Clear();

        foreach (Vector2 x in Game1.player.currentLocation.objects.Keys)
        {
          GameLocation gl = Game1.player.currentLocation;

          if (gl.objects[x] == null)
            continue;

          if (gl.objects[x].name.Equals("Seed Maker"))
          {
            //this.Monitor.Log($"existing (managed: {gl.objects[x].heldObject != null})");

            arSeedMakers.Add(new ManagedSeedMaker(gl.objects[x], null, gl.objects[x].heldObject != null ? true : false));
          }
        }
      }
      else // load new placed Seed Makers  in current location into array
      {
        foreach (ManagedSeedMaker sm in arSeedMakers)
        {
          sm.isDeprecated = true;
        }

        foreach (Vector2 x in Game1.player.currentLocation.objects.Keys)
        {
          GameLocation gl = Game1.player.currentLocation;

          if (gl.objects[x] == null)
            continue;

          if (gl.objects[x].name.Equals("Seed Maker"))
          {
            if (!arSeedMakers.Any(z => z.refSeedMaker == gl.objects[x])) // found new, yet unmanaged Seed Maker
            {
              //this.Monitor.Log($"new (managed: {gl.objects[x].heldObject != null})");

              arSeedMakers.Add(new ManagedSeedMaker(gl.objects[x], null, gl.objects[x].heldObject != null ? true : false));
            }
            else
            {
              arSeedMakers.First(z => z.refSeedMaker == gl.objects[x]).isDeprecated = false;
            }
          }
        }

        // clear non-existent managed Seed Makers
        for(int x = 0; x < arSeedMakers.Count; x++)
        {
          if(arSeedMakers[x].isDeprecated)
          {
            //this.Monitor.Log($"drop");
            arSeedMakers.RemoveAt(x--);
          }
        }
      }

      previousLocation = Game1.player.currentLocation;

      // check if an managed Seed Maker got crops inserted, if so add additional seeds based on crop quality
      foreach (ManagedSeedMaker msm in arSeedMakers)
      {
        StardewValley.Object sm = msm.refSeedMaker;

        if (msm.refSeedMaker == null)
          continue;

        // new crop placed in managed Seed Maker
        if (sm.heldObject.Value != null && msm.hasBeenChecked == false && msm.refDroppedObject == null)
        {
          int x = 0;

          //this.Monitor.Log($"trigger");

          msm.refDroppedObject = previousHeldItem;
          msm.hasBeenChecked = true;

          //this.Monitor.Log($"quality: {msm.refDroppedObject.quality}");

          x = ((msm.refDroppedObject.quality == 4) ? (msm.refDroppedObject.quality - 1) : (msm.refDroppedObject.quality));

          //this.Monitor.Log($"stack: {sm.heldObject.Get().stack.Value}");
          //this.Monitor.Log($"add: {x}");

          sm.heldObject.Get().stack.Value = (sm.heldObject.Get().stack.Value + x);

          //this.Monitor.Log($"now: {sm.heldObject.Get().stack.Value}");
        }

        // seeds grabbed from Seed Maker, reset for new crops
        if (sm.heldObject.Value == null && msm.hasBeenChecked == true)
        {
          //this.Monitor.Log($"drop");

          msm.refDroppedObject = null;
          msm.hasBeenChecked = false;
        }
      }

      previousHeldItem = Game1.player.ActiveObject;
    }
  }
}
