/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using HarmonyLib;
using PlatoTK.Events;
using PlatoTK.Lua;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PlatoTK
{
    public class PlatoTKMod : Mod
    {
        public override void Entry(IModHelper helper)
        {
            var plato = helper.GetPlatoHelper();
            PlatoHelper.EventsInternal = new PlatoEventsHelper(plato);
            PlatoHelper.ConditionsProvider.Add(new LuaConditionsProvider(plato));

            helper.Events.GameLoop.GameLaunched += ApplyCompatPatches;

            var lua = helper.GetPlatoHelper().Lua;
            helper.ConsoleCommands.Add("L#", "Execute Lua script with PlatoTK", (s, p) =>
              {
                  try
                  {
                      if (p.Length == 0)
                          Monitor.Log("No lua script provided.", LogLevel.Trace);
                      else
                      {
                          bool returnValue = p[0] == "log";
                          string code = string.Join(" ", returnValue ? p.Skip(1) : p);
                          Monitor.Log("Running Lua: " + code, LogLevel.Trace);

                          if (!returnValue)
                              lua.CallLua(code);
                          else
                          {
                              var result = lua.CallLua<object>(code);

                              if (!(result is string) && result is IEnumerable objs && objs.Cast<object>() is IEnumerable<object> results)
                                  Monitor.Log($"Results ({results.Count()}): {string.Join(",", objs.Cast<object>().Select(o => o.ToString()))}", LogLevel.Info);
                              else
                                  Monitor.Log("Result: " + (result?.ToString() ?? "null"), LogLevel.Info);
                          }

                          Monitor.Log("OK", LogLevel.Info);
                      }
                  }
                  catch (Exception e)
                  {
                      Monitor.Log(e.ToString(), LogLevel.Error);
                  }
              });
            helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;
            helper.Events.Player.InventoryChanged += Player_InventoryChanged;
        }

        private void Player_InventoryChanged(object sender, InventoryChangedEventArgs e)
        {
            if (e.Added.Count() > 0)
                e.Added.ToList().ForEach(i =>
                {
                    if (!CheckItem(i))
                        e.Player.removeItemFromInventory(i);
                });
        }

        private void GameLoop_SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            Clean();
        }

        private void Clean()
        {
            Game1.player.items.Where(i => i != null).ToList().ForEach(i =>
            {
                if (!CheckItem(i))
                    Game1.player.removeItemFromInventory(i);
            });

            foreach (GameLocation location in Game1.locations)
            {
                foreach (var key in location.objects.Keys.ToList())
                {
                    if (!CheckItem(location.objects[key]))
                        location.objects.Remove(key);
                    else if (location.objects[key] is Chest c)
                        c.items.Where(i => i != null).ToList().ForEach(i =>
                        {
                            if (!CheckItem(i))
                                c.items.Remove(i);
                        });
                }
#if ANDROID
#else
                foreach (var furniture in location.furniture.ToList())
                    if (!CheckItem(furniture))
                        location.furniture.Remove(furniture);
#endif
            }
        }

        private bool CheckItem(Item i)
        {
            try
            {
                if (i.GetType().Assembly != typeof(Item).Assembly)
                    return true;

                i.getDescription();

                if (i is Furniture f)
                    typeof(Furniture).GetMethod("getData", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(f, null);

                if (i is StardewValley.Object o)
                    o.getDescription();

                return true;
            }
            catch
            {
                return false;
            }
        }

        private void ApplyCompatPatches(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        {
            Monitor.Log("Apply Patches", LogLevel.Trace);
            Harmony instance = new Harmony("PlatoTk.CompatPatches");
            Compat.SpaceCorePatches.PatchSpaceCore(Helper, instance);
        }
    }
}
