/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ApryllForever/NuclearBombLocations
**
*************************************************/

using System;

namespace NuclearBombLocations;

//
// Summary:
//     Wraps newer .NET features that improve performance, but aren't available on .NET
//     Framework platforms.
internal static class LegacyShimsEvil
{
    //
    // Summary:
    //     Get an empty array without allocating a new array each time.
    //
    // Type parameters:
    //   T:
    //     The array value type.
    public static T[] EmptyArray<T>()
    {
        return Array.Empty<T>();
    }
}


/*      From On Asset Requested for Building Making 
 * 
 *   /* {  Attempt at making building using tractor code. Failing.


                 e.Edit(editor =>
                 {
                     var data = editor.AsDictionary<string, BuildingData>().Data;

                     data["MermaidSlimeTent"] = new BuildingData
                     {
                         Name = "Slime Tent",
                         Description = "This expirimental Navy structure provides the perfect home for slimes on your farm!",
                         Texture = $"{this.PublicAssetBasePath}/SlimeTent",
                         BuildingType = typeof(SlimeHutch).FullName,
                         SortTileOffset = 1,

                         SourceRect = {
                         X = 0,
                         Y = 0,
                         Width = 48,
                         Height = 80
           },

                         Builder = "MermaidRangerAnabelle",
                         BuildCost = 29000,
                         BuildMaterials = new[]
                         {
                             new BuildingMaterial()
                             {
                                 ItemId =  "(O)335",
                                 Amount = 20,
                             },
                             new BuildingMaterial()
                              {
                                 ItemId = "(O)337",
                                 Amount = 1
                              },
                             new BuildingMaterial()
                             {
                                 ItemId = "(O)428",
                                 Amount = 10,
                             },
                         }.ToList(),
                         BuildDays = 1,

                         Size = new Point(3, 4),
                         HumanDoor = new Point(1, 2),
                         IndoorMap = "Custom_SlimeTentInside",
                         IndoorMapType = typeof(SlimeTent).FullName,


                         //CollisionMap = "XXXX\nXOOX"
                     };
                 });
             }
            */
//if (e.NameWithoutLocale.IsEquivalentTo("Maps/Custom_SlimeTentInside"))
// {



//  e.Edit(asset =>
// {
//var editor = asset.AsMap();

// Map sourceMap = this.Helper.ModContent.Load<Map>("SlimeTentInside.tmx");

// sourceMap.

//Sinz Idea
//I would check out how Tractor Mod handles replacing the whole horse thing with Tractor thing, and that having a reference to the slimetent Building instance would give you the GameLocation instance that you can then mess with, though if you want to do netfield schenigans then you are in harmony patch land anyway
//  the mapPath variable on GameLocation instance would be the Maps\\{ IndoorMap}
// value which you can use to only modify Slime Tents but not Slime Hutches

//editor.PatchMap(sourceMap, targetArea: new Microsoft.Xna.Framework.Rectangle(30, 10, 20, 20));
//   });

// }





//if (e.NameWithoutLocale.IsEquivalentTo("Maps/Custom_SlimeTentInside"))
// {
//   this.Helper.ModContent.Load<Map>("Assets/Custom_SlimeTentInside.tmx");


//}
//Maps\Custom_SlimeTentInside


// e.Edit(asset =>
//{
//var editor = asset.AsMap();

// Map sourceMap = ModEntry.Helper.ModContent.Load<Map>("AtomicScienceSilo.tmx");
//Map sourceMap2 = ModEntry.Helper.ModContent.Load<Map>("AtomicScienceSilo.tmx");
//editor.PatchMap(sourceMap, targetArea: new Rectangle(30, 10, 20, 20));
// });


