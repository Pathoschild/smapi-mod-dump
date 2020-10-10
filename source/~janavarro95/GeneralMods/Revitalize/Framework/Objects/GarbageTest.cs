/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using PyTK.CustomElementHandler;
using StardewValley;
using StardewValley.Objects;

namespace Revitalize.Framework.Objects
{
    public class GarbageTest:CustomObject
    {
        public GarbageTest()
        {

        }

        public GarbageTest(BasicItemInformation info) : base(info)
        {

        }

        public GarbageTest(BasicItemInformation info,Vector2 TileLocation) : base(info,TileLocation)
        {

        }

        public override Item getOne()
        {
            return new GarbageTest(this.info);
        }

        public override ICustomObject recreate(Dictionary<string, string> additionalSaveData, object replacement)
        {
            CustomObjectData data = CustomObjectData.collection[additionalSaveData["id"]];
            return new GarbageTest((BasicItemInformation)data, (replacement as Chest).TileLocation);
        }

    }
}
