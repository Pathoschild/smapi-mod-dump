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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PlatoTK.APIs;
using StardewValley;
using System.Collections.Generic;
using System.Linq;

namespace PlatoTK.Presets
{
    internal class ArcadeMachinePreset
    {
        internal static HashSet<ArcadeMachineSpecs> Machines = new HashSet<ArcadeMachineSpecs>();
        internal static bool Patched = false;
        public static void CheckForActionPatch(ref bool __result, StardewValley.Object __instance, bool justCheckingForActivity)
        {
            if (ShouldPatch(__instance, out ArcadeMachineSpecs machine))
            {
                if (justCheckingForActivity)
                {
                    __result = true;
                    return;
                }

                machine.Start();
                __result = true;
            }
        }

        public static void Setup(IPlatoHelper helper, ArcadeMachineSpecs specs)
        {
            if (!Patched)
            {
                var instance = new Harmony("PlatoTK.ArcadeMachine");

                instance.Patch(
                    original: AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.checkForAction)),
                    postfix: new HarmonyMethod(AccessTools.Method(typeof(ArcadeMachinePreset), nameof(CheckForActionPatch))));

                helper.Events.CallingTileAction += Events_CallingTileAction;

                if (helper.ModHelper.ModRegistry.GetApi<IArcadeApi>("spacechase0.ArcadeRoom") is IArcadeApi arcade)
                    arcade.OnRoomSetup += (x, y) =>
                    {
                        if (Game1.getLocationFromName("Arcade") is GameLocation a)
                            AddArcades(a);

                        helper.ModHelper.Events.Player.Warped += (s, e) =>
                        {
                            if (e.NewLocation is GameLocation l && l.Name == "Arcade")
                                AddArcades(e.NewLocation);
                        };
                    };

                Patched = true;
            }

            if (helper.ModHelper.ModRegistry.GetApi<IMobilePhoneApi>("aedenthorn.MobilePhone") is IMobilePhoneApi phone)
            {
                    Texture2D appIcon = helper.ModHelper.Content.Load<Texture2D>(specs.Icon, StardewModdingAPI.ContentSource.GameContent);
                    bool success = phone.AddApp(specs.Id, specs.Name, () =>
                    {
                        specs.Start();
                    }, appIcon);
            }
        }

        private static void Events_CallingTileAction(object sender, Events.ICallingTileActionEventArgs e)
        {
            if (e.Trigger == "OpenArcade")
            {
                e.TakeOver(false);
                if (Machines.FirstOrDefault(m => m.Id == e.Parameter[0]) is ArcadeMachineSpecs specs)
                    specs.Start();
            }
        }


        private static void AddArcades(GameLocation arcade)
        {
            foreach (ArcadeMachineSpecs specs in Machines.Where(m => m.Spot != Vector2.Zero))
                if (!arcade.map.TileSheets.Any(t => t.Id == $"zzz_{specs.Id}"))
                {
                    var arcadeTilesheet = new xTile.Tiles.TileSheet(arcade.map, specs.Sprite, new xTile.Dimensions.Size(1, 2), new xTile.Dimensions.Size(16, 16));
                    arcadeTilesheet.Id = $"zzz_{specs.Id}";
                    arcade.Map.AddTileSheet(arcadeTilesheet);
                    Game1.mapDisplayDevice.LoadTileSheet(arcadeTilesheet);
                    int ts = arcade.Map.TileSheets.IndexOf(arcadeTilesheet);
                    arcade.setMapTile((int)specs.Spot.X, (int)specs.Spot.Y, 1, "Buildings", $"OpenArcade {specs.Id}", ts);
                    arcade.setMapTile((int)specs.Spot.X, (int)specs.Spot.Y - 1, 0, "Front", null, ts);
                }
        }


        public static void Add(ArcadeMachineSpecs specs, IPlatoHelper helper)
        {
            if (!Machines.Contains(specs))
            {
                Machines.Add(specs);

                if(helper.ModHelper.ModRegistry.GetApi<IArcadeApi>("spacechase0.ArcadeRoom") is IArcadeApi arcade)
                    specs.Spot = arcade.ReserveMachineSpot();

                Setup(helper, specs);
            }
        }

        public static bool ShouldPatch(StardewValley.Object obj, out ArcadeMachineSpecs machine)
        {
            var search = Machines.FirstOrDefault(m => obj.netName.Value.Equals(m.ObjectName));

            machine = search;

            return machine != null;
        }
    }
}
