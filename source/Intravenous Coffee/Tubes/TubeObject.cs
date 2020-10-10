/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mpcomplete/StardewMods
**
*************************************************/

using System;
using StardewValley;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PyTK.CustomElementHandler;
using System.Collections.Generic;
using StardewValley.Objects;
using PyTK.Extensions;

namespace Tubes
{
    public class Blueprint
    {
        public string Fullid;
        public string Name;
        public string Category;
        public int Price;
        public string Description;
        public string Crafting;

        public CustomObjectData CreateObjectData(Texture2D texture, Type type)
        {
            return new CustomObjectData(
                Fullid,
                $"{Name}/{Price}/-300/{Category} -24/{Name}/{Description}",
                texture,
                Color.White,
                0,
                false,
                type,
                Crafting != null ? new CraftingData(Fullid, Crafting) : null);
        }
    }

    public class JunkObject {
        internal static Blueprint Blueprint = new Blueprint {
            Fullid = "Pneumatic Tube junk",
            Name = "Pneumatic Tube junk",
            Category = "Crafting",
            Price = 100,
            Description = "Temporary internal object left behind when a tube is junked. Player shouldn't see this.",
        };

        internal static CustomObjectData objectData;

        internal static void Init(Texture2D icon)
        {
            objectData = Blueprint.CreateObjectData(icon, null);
        }
    }

    // The Tube object type. This is used whenever the object is not placed on the ground (it's not a terrain feature).
    public class TubeObject : StardewValley.Object, ICustomObject, ISaveElement, IDrawFromCustomObjectData
    {
        internal static Texture2D Icon;
        internal static CustomObjectData ObjectData;

        internal static Blueprint Blueprint = new Blueprint {
            Fullid = "Pneumatic Tube",
            Name = "Pneumatic Tube",
            Category = "Crafting",
            Price = 100,
            Description = "Connects machines together with the magic of vacuums.",
            Crafting = "337 1",
        };

        internal static void Init()
        {
            Icon = TubesMod._helper.Content.Load<Texture2D>(@"Assets/icon.png");
            ObjectData = Blueprint.CreateObjectData(Icon, typeof(TubeObject));

            JunkObject.Init(Icon);
        }

        public CustomObjectData data { get => ObjectData; }

        public TubeObject()
        {
        }

        public TubeObject(CustomObjectData data)
            : base(data.sdvId, 1)
        {
        }

        public TubeObject(CustomObjectData data, Vector2 tileLocation)
            : base(tileLocation, data.sdvId)
        {
        }

        public Dictionary<string, string> getAdditionalSaveData()
        {
            return new Dictionary<string, string>() { { "name", name }, { "price", price.ToString() }, { "stack", stack.ToString() } };
        }

        public object getReplacement()
        {
            return new Chest(true) { playerChoiceColor = Color.Magenta };
        }

        public void rebuild(Dictionary<string, string> additionalSaveData, object replacement)
        {
            name = additionalSaveData["name"];
            price = additionalSaveData["price"].toInt();
            stack = additionalSaveData["stack"].toInt();
        }

        public override Item getOne()
        {
            return new TubeObject(data) { tileLocation = Vector2.Zero, name = name, price = price };
        }

        public ICustomObject recreate(Dictionary<string, string> additionalSaveData, object replacement)
        {
            return new TubeObject(data);
        }

        public override bool isPassable()
        {
            return true;
        }
    }
}
