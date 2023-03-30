/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;

namespace SpousesIsland
{
    internal class Information
    {
        internal static Point GetReturnPoint(string npc)
        {
            Point result = npc switch
            {
                "Abigail" => new Point(16, 9),
                "Alex" => new Point(19,6),
                "Elliott" => new Point(9, 9),
                "Emily" => new Point(12,10),
                "Haley" => new Point(8,6),
                "Harvey" => new Point(16,13),
                "Krobus" => new Point(15,10),
                "Leah" => new Point(21,13),
                "Maru" => new Point(18,15),
                "Penny" => new Point(9,12),
                "Sam" => new Point(22,6),
                "Sebastian" => new Point(25,14),
                "Shane" => new Point(20,5),
                "Claire" => new Point(5,6),
                "Lance" => new Point(13,13),
                "Olivia" => new Point(26,6),
                "Sophia" => new Point(7,11),
                "Victor" => new Point(3,5),
                "Wizard" => new Point(14,9),
                _ => Point.Zero
            };

            //if custom npc
            if (result == Point.Zero)
            {
                var r = Game1.random;
                result = r.Next(4) switch
                {
                    0 => new Point(r.Next(1, 11), r.Next(4, 14)),
                    1 => new Point(r.Next(12, 17), r.Next(9, 12)),
                    2 => new Point(r.Next(13, 29), r.Next(12, 15)),
                    3 => new Point(r.Next(18, 29), r.Next(4, 7)),
                    _ => Point.Zero
                };

                var fh = Game1.getLocationFromName("IslandFarmHouse");

                //if not clear, try to get new location
                if(!fh.isTileLocationTotallyClearAndPlaceableIgnoreFloors(result.ToVector2()))
                {
                    for(int i=0; i<10 ; i++)
                    {
                        r = Game1.random;

                        //1-10 & 4-13, 12-16 & 9-11, 13-28 & 12-14, 18-28 & 4-6
                        //+1: last number isnt considered

                        result = r.Next(4) switch
                        {
                            0 => new Point(r.Next(1,11),r.Next(4,14)),
                            1 => new Point(r.Next(12,17),r.Next(9,12)),
                            2 => new Point(r.Next(13,29),r.Next(12,15)),
                            3 => new Point(r.Next(18,29),r.Next(4,7)),
                            _ => Point.Zero
                        };

                        if (fh.isTileLocationTotallyClearAndPlaceableIgnoreFloors(result.ToVector2()))
                            break;
                    }
                }
            }

            return result;
        }

        internal static bool HasMod(string ModID)
        {
            if (ModEntry.Help.ModRegistry.Get(ModID) is not null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Check if a mod's version is older OR equal.
        /// </summary>
        /// <param name="ModID">The UniqueID to check.</param>
        /// <param name="version">The max version to account for.</param>
        /// <returns>true or false depending on result.</returns>
        internal static bool IsVersionOrLower(string ModID, string version)
        {
            //get both ISemanticVersions
            var modversion = ModEntry.Help.ModRegistry.Get(ModID).Manifest.Version;
            _ = SemanticVersion.TryParse(version, out ISemanticVersion max_version);

            ModEntry.Mon.Log($"C2N version :{modversion.ToString()}, checking if it's newer than {version}...");

            //Previously used "IsOlderThan() || modversion == max_version", but this is less bug-prone.
            return !(modversion.IsNewerThan(max_version));
        }

        internal static List<Character> PlayerChildren(Farmer player)
        {
            var Children = new List<Character>();

            var asChild = player.getChildren();

            //get them from kid list if vanilla
            if (asChild?.Count is not 0)
            {
                foreach (var kid in asChild)
                {
                    Children.Add(kid as Character);
                }
            }
            //if none, they must be in C2N's data. get it from there
            if ((Children == null || Children?.Count == 0) && ModEntry.HasC2N_Or_LNPCs)
            {
                /* old method but we can do it without reflection*/
                /*
                var c2n_data = ModEntry.Help.ModRegistry.Get("Loe2Run.ChildToNPC").GetType(); //.GetProperty("copies")
                var c2n_children = ModEntry.Help.Reflection.GetField<Dictionary<string, NPC>>(c2n_data, "copies");
                var values = c2n_children.GetValue().Values;

                */

                var charas = Utility.getHomeOfFarmer(player)?.getCharacters();
                var values = new List<NPC>();

                foreach (var npc in charas)
                {
                    //0 teen?, 1 adult, 2 child
                    if (npc.Age != 2)
                    {
                        continue;
                    }
                    values.Add(npc);
                }

                //if theres still none
                if (values?.Count is 0)
                {
                    return Children;
                }

                //add. we could do a .ToList() but i'd rather be safe
                foreach (var child in values)
                {
                    Children.Add(child as Character);
                }
            }

            return Children;
        }

        internal static List<string> PlayerSpouses(string id)
        {
            var farmer = Game1.getFarmer(long.Parse(id));
            List<string> spouses = PlayerSpouses(farmer);

            return spouses;
        }

        internal static List<string> PlayerSpouses(Farmer farmer)
        {
            List<string> spouses = new();

            foreach (string name in Game1.NPCGiftTastes.Keys)
            {
                //if universal taste OR unmet npc
                if (name.StartsWith("Universal_") || farmer?.friendshipData?.Keys.Contains(name) == false)
                    continue;

                var isMarried = farmer?.friendshipData[name]?.IsMarried() ?? false;
                var isRoommate = farmer?.friendshipData[name]?.IsRoommate() ?? false;
                if (isMarried || isRoommate)
                {
                    spouses.Add(name);
                }
            }
            return spouses;
        }
    }
}
