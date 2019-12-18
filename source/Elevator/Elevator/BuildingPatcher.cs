using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley;
using StardewValley.Buildings;
using System;

namespace Elevator
{
	class BuildingPatcher_patchAction : Patch
	{
		protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(Building), "doAction");

		public static bool Prefix(Vector2 tileLocation, Farmer who, NetPoint ___humanDoor, NetInt ___tileX, NetInt ___tileY, Building __instance)
		{
			int doorWidth = 2;
			int xOff = 5;//___humanDoor.X
			int xDist = (int)tileLocation.X - (xOff + ___tileX.Value);
			
			if (who.IsLocalPlayer && CabinHelper.IsElevatorBuilding(__instance)
				&& xDist < doorWidth && xDist >= 0 && tileLocation.Y == ___humanDoor.Y + ___tileY.Value)
			{

				if (who.mount != null)
				{
					Game1.showRedMessage(Game1.content.LoadString("Strings\\Buildings:DismountBeforeEntering"));
					return false;
				}
				if (who.team.buildLock.IsLocked())
				{
					Game1.showRedMessage(Game1.content.LoadString("Strings\\Buildings:CantEnter"));
					return false;
				}

				//indoors.Value.isStructure.Value = true;

				who.currentLocation.playSoundAt("crystal", tileLocation);

				if (Game1.activeClickableMenu == null)
					Game1.activeClickableMenu = new ElevatorMenu();

				return false;
			} 
			return true;
		}
	}

	class BuildingPatcher_patchResetTexture : Patch
	{
		protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(Building), "resetTexture");

		public static bool Prefix(Building __instance)
		{
			if (CabinHelper.IsElevatorBuilding(__instance))
			{
				__instance.texture = new Lazy<Texture2D>(() => ModEntry.ElevatorBuildingTexture );
				return false;
			}
			return true;
		}
	}

	//This makes it so when you hover over the double doors it still makes your mouse have the hover icon/hand thing
	class BuildingPatcher_patchIsActionableTile : Patch
	{
		protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(Building), "isActionableTile");
		
		public static bool Postfix(bool b, int xTile, int yTile, Farmer who, Building __instance, NetPoint ___humanDoor, bool __result, NetInt ___tileX, NetInt ___tileY)
		{

			if (!CabinHelper.IsElevatorBuilding(__instance))
				return __result;


			int doorWidth = 2;
			int xOff = 5;//___humanDoor.X
			int dist = xTile - (___tileX.Value + xOff);

			if (___humanDoor.X >= 0  && yTile == ___tileY.Value + ___humanDoor.Y && 
				dist < doorWidth && dist >= 0)
				return true;

			return false;
		}
	}
}
