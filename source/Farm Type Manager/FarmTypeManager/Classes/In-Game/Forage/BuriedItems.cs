/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/FarmTypeManager
**
*************************************************/

using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>A modified subclass of StardewValley.Object designed to act like an artifact spot, but with easily customizable contents.</summary>
        /// <remarks>This class is not currently designed to be saved by the game's native processes. All instances should be removed from the game before saving (i.e. end of day).</remarks>
        public class BuriedItems : StardewValley.Object
        {
            /// <summary>The list of items contained by this burial spot.</summary>
            /// <remarks>Replaces the object(s) that would normally be produced by an artifact spot.</remarks>
            [XmlElement("Items")]
            public readonly NetObjectList<Item> Items = new NetObjectList<Item>();

            public BuriedItems()
                : base()
            {
            }

            /// <summary>Create a new buried item location with the specified contents.</summary>
            /// <param name="items">>A set of items the container will drop when broken. Null or empty lists are valid.</param>
            public BuriedItems(IEnumerable<Item> items)
                : base("590", 1) //use the typical constructor for an artifact spot
            {
                Items.AddRange(items); //add the provided set of items to this object's list
            }

            protected override void initNetFields()
            {
                base.initNetFields();
                NetFields.AddField(Items); //include this class's custom field
            }

            /// <summary>An override that indicates this object type should be passable, despite the base method's result.</summary>
            public override bool isPassable()
            {
                return true;
            }

            /// <summary>An override that produces customizable items instead of the normal artifact spot object(s).</summary>
            public override bool performToolAction(Tool t)
            {
                //imitate the base method's initial validation process
                if (this.isTemporarilyInvisible)
                    return false;

                if (t is Hoe) //if the buried items are being dug up
                {
                    GameLocation location = Location;
                    if (location == null) return false;
                    Vector2 tile = TileLocation;

                    releaseContents();

                    //perform the base method's other artifact spot tasks
                    if (!location.terrainFeatures.ContainsKey(tile))
                        location.makeHoeDirt(tile, false);
                    playNearbySoundAll("hoeHit");
                    if (location.objects.ContainsKey(tile))
                        location.objects.Remove(tile);
                }


                return false; //always return false when this check is completed
            }

            /// <summary>Drops the items from this object's "items" list.</summary>
            /// <param name="location">The location of this object.</param>
            /// <remarks>This replaces the method's original behavior and no longer takes Farmer as an argument.</remarks>
            public void releaseContents()
            {
                if (Items == null || Items.Count < 1) { return; } //if there are no items listed, do nothing

                Vector2 itemPosition = new Vector2(boundingBox.Center.X, boundingBox.Center.Y); //get the "pixel" location where these items should spawn

                foreach (Item item in Items) //for each item in this container's item list
                {
                    Game1.createItemDebris(item, itemPosition, Utility.RNG.Next(4), Location); //spawn the item as "debris" at this location
                }
            }
        }
    }
}
