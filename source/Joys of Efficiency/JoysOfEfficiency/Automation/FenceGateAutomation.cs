using System.Collections.Generic;
using System.Linq;
using JoysOfEfficiency.Utils;
using Microsoft.Xna.Framework;
using StardewValley;
using Object = StardewValley.Object;

namespace JoysOfEfficiency.Automation
{
    internal enum FenceType
    {
        Vertical,
        Horizontal,
        Invalid
    }

    internal class FenceGateAutomation
    {
        public static void TryToggleGate(Farmer player)
        {
            GameLocation location = player.currentLocation;
            foreach (Fence fence in Util.GetObjectsWithin<Fence>(2, true).Where(x=>x.isGate))
            {
                bool flag = false;
                List<Fence> fencesToOperate = new List<Fence> {fence};
                FenceType type = GetFenceType(location, fence);
                if (type != FenceType.Invalid)
                {
                    fencesToOperate.Add(GetConnectedGate(location, fence.TileLocation, type));
                    if (IsPlayerInClose(fencesToOperate, type, player) && IsPlayerFaceOrBack(type, player))
                    {
                        flag = true;
                    }
                }
                else if (IsSingleFence(location, fence))
                {
                    if (IsPlayerInClose(fence, player))
                    {
                        flag = true;
                    }
                }
                OperateFences(fencesToOperate, flag);
            }
        }

        private static bool IsPlayerFaceOrBack(FenceType type, Farmer farmer)
        {
            int facing = farmer.FacingDirection;
            if (facing % 2 == 0)
            {
                //Player is facing upside/downside.
                return type == FenceType.Horizontal;
            }
            else
            {
                return type == FenceType.Vertical;
            }
        }

        private static bool IsPlayerInClose(Fence fence, Farmer player)
        {
            Vector2 oVec = fence.TileLocation;
            Vector2 pVec = player.getTileLocation();
            return pVec == oVec || 
                   pVec == oVec + new Vector2(1, 0) || pVec == oVec + new Vector2(-1, 0) ||
                   pVec == oVec + new Vector2(0, 1) || pVec == oVec + new Vector2(0, -1);
        }

        private static bool IsPlayerInClose(List<Fence> fences, FenceType type, Farmer player)
        {
            if (fences == null || !fences.Any())
            {
                return false;
            }
            foreach (Fence fence in fences.Where(f => f != null))
            {
                Vector2 oVec = fence.TileLocation;
                Vector2 pVec = player.getTileLocation();
                if (oVec == pVec)
                {
                    return true;
                }

                switch (type)
                {
                    case FenceType.Vertical when (pVec == oVec + new Vector2(1, 0) || pVec == oVec + new Vector2(-1, 0)):
                    case FenceType.Horizontal when (pVec == oVec + new Vector2(0, 1) || pVec == oVec + new Vector2(0, -1)):
                        return true;
                }
            }

            return false;
        }

        private static void OperateFences(IEnumerable<Fence> fencesToOperate, bool open)
        {
            foreach (Fence fence in fencesToOperate.Where(f=>f != null && f.isGate.Value))
            {
                int gatePosition = fence.gatePosition.Value;
                if (open && gatePosition == 0)
                {
                    fence.gatePosition.Value = 88;
                    Game1.playSound("doorClose");
                }
                else if (!open && gatePosition >= 88)
                {
                    fence.gatePosition.Value = 0;
                    Game1.playSound("doorClose");
                }
            }
        }

        private static Fence GetConnectedGate(GameLocation location, Vector2 fenceLoc, FenceType type)
        {
            if (type == FenceType.Horizontal)
            {
                if (location.Objects.TryGetValue(fenceLoc + new Vector2(-1, 0), out Object obj) &&
                    obj is Fence fence && fence.isGate)
                {
                    return fence;
                }
                if (location.Objects.TryGetValue(fenceLoc + new Vector2(1, 0), out obj) &&
                    obj is Fence fence2 && fence2.isGate)
                {
                    return fence2;
                }
            }
            else if (type == FenceType.Vertical)
            {
                if (location.Objects.TryGetValue(fenceLoc + new Vector2(0, -1), out Object obj2) &&
                    obj2 is Fence fence3 && fence3.isGate)
                {
                    return fence3;
                }
                if (location.Objects.TryGetValue(fenceLoc + new Vector2(0, 1), out obj2) &&
                    obj2 is Fence fence4 && fence4.isGate)
                {
                    return fence4;
                }
            }

            return null;
        }

        private static bool IsSingleFence(GameLocation location, Fence fence)
        {
            return GetSurroundingObjects<Fence>(location, fence.TileLocation).Any(f => !f.isGate);
        }

        private static bool IsGatesSerial(GameLocation location, Vector2 fenceLoc, int dX, int dY, out int gateCount)
        {
            Vector2 oVec = fenceLoc;
            Vector2 dVec = new Vector2(dX, dY);
            gateCount = 0;
            while (true)
            {
                oVec += dVec;
                if (!location.Objects.TryGetValue(oVec, out Object obj) || !(obj is Fence fence))
                {
                    return false;
                }

                if (!fence.isGate)
                {
                    break;
                }
            }

            dVec *= -1;

            while (true)
            {
                oVec += dVec;
                if (!location.Objects.TryGetValue(oVec, out Object obj) || !(obj is Fence fence))
                {
                    return false;
                }

                if (!fence.isGate)
                {
                    break;
                }

                gateCount++;
            }

            return true;
        }

        private static FenceType GetFenceType(GameLocation location, Fence fence)
        {
            bool flagH = false, flagV = false;
            if (IsGatesSerial(location, fence.TileLocation, -1, 0, out int countH) && countH < 3)
            {
                flagH = true;
            }
            if (IsGatesSerial(location, fence.TileLocation, 0, -1, out int countV) && countV < 3)
            {
                flagV = true;
            }

            if (flagV && flagH)
            {
                return FenceType.Invalid;
            }

            return flagV ? FenceType.Vertical : (flagH ? FenceType.Horizontal : FenceType.Invalid);
        }

        private static List<T> GetSurroundingObjects<T>(GameLocation loc, Vector2 oVec) where  T:Object
        {
            List<T> objects = new List<T>();
            if (loc.Objects.TryGetValue(oVec + new Vector2(-1, 0), out Object obj) && obj is T t)
            {
                objects.Add(t);
            }
            if (loc.Objects.TryGetValue(oVec + new Vector2(0, -1), out obj) && obj is T t2)
            {
                objects.Add(t2);
            }
            if (loc.Objects.TryGetValue(oVec + new Vector2(1, 0), out obj) && obj is T t3)
            {
                objects.Add(t3);
            }
            if (loc.Objects.TryGetValue(oVec + new Vector2(0, 1), out obj) && obj is T t4)
            {
                objects.Add(t4);
            }

            return objects;
        }
    }
}
