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
using System.Linq;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Buildings;
using StardewValley.Monsters;
using StardewValley.Tools;
using StardewValley;
using StardewModdingAPI;
using SpaceShared;
using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuclearBombLocations
{
   // [XmlInclude(typeof(SlimeTent))]
    [XmlType("Mods_ApryllForever_NuclearBombLocations_SlimeTent")]
    public class SlimeTent : NuclearLocation
	{

		[XmlElement("slimeTentMatingsLeft")]
		public readonly NetInt slimeTentMatingsLeft = new NetInt();

		public readonly NetArray<bool, NetBool> waterSpots = new NetArray<bool, NetBool>(4);

		protected int _slimeCapacity = -1;

		public SlimeTent()
		{
		}

		public SlimeTent(IModContentHelper content)
        : base(content, "SlimeTentInside", "SlimeTentInside")
        {
		}
       // internal class Holder { public readonly NetRef<GameLocation> Value = new(); }


        //internal static ConditionalWeakTable<Building, Holder> values = new();
       // public static NetRef<GameLocation> get_SlimeTent(SlimeTent tent, Building building)
		//{
         //   var holder = values.GetOrCreateValue(building);
         //   return holder.Value;
       // }
       // public static void set_SlimeTent(SlimeTent tent, IEquatable<GameLocation> newVal)
       // {
            // We don't actually want a setter for this one, since it should be readonly
            // Net types are weird
            // Or do we? Serialization
       // }

        protected override void initNetFields()
		{
			base.initNetFields();
			base.NetFields.AddField(this.slimeTentMatingsLeft, "slimeTentMatingsLeft").AddField(this.waterSpots, "waterSpots");
		}

		/// <inheritdoc />
		public override void OnParentBuildingUpgraded(Building building)
		{
			base.OnParentBuildingUpgraded(building);
			this._slimeCapacity = -1;
		}



        public bool isFull()
        {
            return base.characters.Count >= 23;
        }


		public override bool canSlimeMateHere()
		{
			int matesLeft;
			matesLeft = this.slimeTentMatingsLeft;
			this.slimeTentMatingsLeft.Value--;
			if (!this.isFull())
			{
				return matesLeft > 0;
			}
			return false;
		}

		public override bool canSlimeHatchHere()
		{
			return !this.isFull();
		}

		public override void DayUpdate(int dayOfMonth)
		{
			int waters;
			waters = 0;
			int startIndex;
			startIndex = Game1.random.Next(this.waterSpots.Length);
			for (int j = 0; j < this.waterSpots.Length; j++)
			{
				if (this.waterSpots[(j + startIndex) % this.waterSpots.Length] && waters * 5 < base.characters.Count)
				{
					waters++;
					this.waterSpots[(j + startIndex) % this.waterSpots.Length] = false;
				}
			}
			for (int i = base.objects.Length - 1; i >= 0; i--)
			{
				StardewValley.Object sprinkler;
				sprinkler = base.objects.Values.ElementAt(i);
				if (sprinkler.IsSprinkler())
				{
					foreach (Vector2 v in sprinkler.GetSprinklerTiles())
					{
						if (v.X == 16f && v.Y >= 6f && v.Y <= 9f)
						{
							this.waterSpots[(int)v.Y - 6] = true;
						}
					}
				}
			}
			for (int numSlimeBalls = Math.Min(base.characters.Count / 5, waters); numSlimeBalls > 0; numSlimeBalls--)
			{
				int tries;
				tries = 50;
				Vector2 tile;
				tile = base.getRandomTile();
				while ((!this.CanItemBePlacedHere(tile) || this.doesTileHaveProperty((int)tile.X, (int)tile.Y, "NPCBarrier", "Back") != null || tile.Y >= 12f) && tries > 0)
				{
					tile = base.getRandomTile();
					tries--;
				}
				if (tries > 0)
				{
					base.objects.Add(tile, ItemRegistry.Create<StardewValley.Object>("(BC)56"));
				}
			}
			while ((int)this.slimeTentMatingsLeft > 0)
			{
				if (base.characters.Count > 1 && !this.isFull() && base.characters[Game1.random.Next(base.characters.Count)] is GreenSlime mate && (int)mate.ageUntilFullGrown <= 0)
				{
					for (int distance = 1; distance < 10; distance++)
					{
						GreenSlime otherMate;
						otherMate = (GreenSlime)Utility.checkForCharacterWithinArea(mate.GetType(), mate.Position, this, new Rectangle((int)mate.Position.X - 64 * distance, (int)mate.Position.Y - 64 * distance, 64 * (distance * 2 + 1), 64 * (distance * 2 + 1)));
						if (otherMate != null && otherMate.cute != mate.cute && (int)otherMate.ageUntilFullGrown <= 0)
						{
							mate.mateWith(otherMate, this);
							break;
						}
					}
				}
				this.slimeTentMatingsLeft.Value--;
			}
			this.slimeTentMatingsLeft.Value = base.characters.Count / 5 + 1;
			base.DayUpdate(dayOfMonth);
		}
        public Building getBuilding()
        {
            foreach (Building b in Game1.getFarm().buildings)
            {
                if (b.indoors.Value != null && b.indoors.Value.Equals(this))
                {
                    return b;
                }
            }
            return null;
        }

        public override void TransferDataFromSavedLocation(GameLocation l)
		{
			if (l is SlimeTent slimeHutch)
			{
				for (int i = 0; i < this.waterSpots.Length; i++)
				{
					if (i < slimeHutch.waterSpots.Count)
					{
						this.waterSpots[i] = slimeHutch.waterSpots[i];
					}
				}
			}
			base.TransferDataFromSavedLocation(l);
		}

		public override bool performToolAction(Tool t, int tileX, int tileY)
		{
			if (t is WateringCan && tileX >= 5 && tileX <= 8 && tileY >= 6 && tileY <= 9)
			{
				this.waterSpots[tileY - 6] = true;
			}
			return false;
		}

		public override void UpdateWhenCurrentLocation(GameTime time)
		{
			base.UpdateWhenCurrentLocation(time);
			for (int i = 0; i < this.waterSpots.Length; i++)
			{
				if (this.waterSpots[i])
				{
					base.setMapTileIndex(6, 6 + i, 2135, "Buildings");
                    base.setMapTileIndex(7, 6 + i, 2135, "Buildings");
                }
				else
				{
					base.setMapTileIndex(6, 6 + i, 2134, "Buildings");
                    base.setMapTileIndex(7, 6 + i, 2134, "Buildings");
                }
			}
		}
	}
}
