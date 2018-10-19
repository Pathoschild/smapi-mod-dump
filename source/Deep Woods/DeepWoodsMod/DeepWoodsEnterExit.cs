
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using System;
using System.Collections.Generic;
using xTile.Dimensions;
using static DeepWoodsMod.DeepWoodsGlobals;

namespace DeepWoodsMod
{
    public class DeepWoodsEnterExit
    {
        public class DeepWoodsExit : INetObject<NetFields>
        {
            public NetFields NetFields { get; } = new NetFields();
            public readonly NetInt exitDir = new NetInt(0);
            public readonly NetString myDeepWoodsName = new NetString();
            public readonly NetPoint location = new NetPoint(Point.Zero);
            public readonly NetString targetDeepWoodsName = new NetString();
            public readonly NetPoint targetLocation = new NetPoint(Point.Zero);
            public ExitDirection ExitDir { get { return (ExitDirection)exitDir.Value; } }
            public Location Location { get { return new Location(location.Value.X, location.Value.Y); } }
            public Location TargetLocation
            {
                get
                {
                    return new Location(targetLocation.Value.X, targetLocation.Value.Y);
                }
                set
                {
                    targetLocation.Value = new Point(value.X, value.Y);
                }
            }
            public string TargetDeepWoodsName
            {
                get
                {
                    return targetDeepWoodsName.Value;
                }
                set
                {
                    targetDeepWoodsName.Value = value;
                }
            }
            public DeepWoodsExit()
            {
                this.InitNetFields();
            }
            public DeepWoodsExit(DeepWoods myDeepWoods, ExitDirection exitDir, Location location)
            {
                this.InitNetFields();
                this.myDeepWoodsName.Value = myDeepWoods.Name;
                this.exitDir.Value = (int)exitDir;
                this.location.Value = new Point(location.X, location.Y);
            }
            private void InitNetFields()
            {
                this.NetFields.AddFields(myDeepWoodsName, exitDir, location, targetDeepWoodsName, targetLocation);
            }
        }


        public enum EnterDirection
        {
            FROM_LEFT,
            FROM_TOP,
            FROM_RIGHT,
            FROM_BOTTOM
        }

        public enum ExitDirection
        {
            RIGHT,
            BOTTOM,
            LEFT,
            TOP
        }

        public static EnterDirection ExitDirToEnterDir(ExitDirection exitDir)
        {
            switch (exitDir)
            {
                case ExitDirection.RIGHT:
                    return EnterDirection.FROM_LEFT;
                case ExitDirection.BOTTOM:
                    return EnterDirection.FROM_TOP;
                case ExitDirection.LEFT:
                    return EnterDirection.FROM_RIGHT;
                case ExitDirection.TOP:
                    return EnterDirection.FROM_BOTTOM;
                default:
                    return EnterDirection.FROM_TOP;
            }
        }

        public static ExitDirection EnterDirToExitDir(EnterDirection enterDir)
        {
            switch (enterDir)
            {
                case EnterDirection.FROM_LEFT:
                    return ExitDirection.RIGHT;
                case EnterDirection.FROM_TOP:
                    return ExitDirection.BOTTOM;
                case EnterDirection.FROM_RIGHT:
                    return ExitDirection.LEFT;
                case EnterDirection.FROM_BOTTOM:
                    return ExitDirection.TOP;
                default:
                    return ExitDirection.BOTTOM;
            }
        }

        public static int EnterDirToFacingDirection(EnterDirection enterDir)
        {
            switch (enterDir)
            {
                case EnterDirection.FROM_LEFT:
                    return 1;
                case EnterDirection.FROM_TOP:
                    return 2;
                case EnterDirection.FROM_RIGHT:
                    return 3;
                case EnterDirection.FROM_BOTTOM:
                    return 0;
                default:
                    return 2;
            }
        }

        public static ExitDirection CastEnterDirToExitDir(EnterDirection enterDir)
        {
            switch (enterDir)
            {
                case EnterDirection.FROM_LEFT:
                    return ExitDirection.LEFT;
                case EnterDirection.FROM_TOP:
                    return ExitDirection.TOP;
                case EnterDirection.FROM_RIGHT:
                    return ExitDirection.RIGHT;
                case EnterDirection.FROM_BOTTOM:
                    return ExitDirection.BOTTOM;
                default:
                    return ExitDirection.TOP;
            }
        }

        public static EnterDirection CastExitDirToEnterDir(ExitDirection exitDir)
        {
            switch (exitDir)
            {
                case ExitDirection.LEFT:
                    return EnterDirection.FROM_LEFT;
                case ExitDirection.TOP:
                    return EnterDirection.FROM_TOP;
                case ExitDirection.RIGHT:
                    return EnterDirection.FROM_RIGHT;
                case ExitDirection.BOTTOM:
                    return EnterDirection.FROM_BOTTOM;
                default:
                    return EnterDirection.FROM_TOP;
            }
        }

        public static List<ExitDirection> AllExitDirsBut(ExitDirection exclude)
        {
            List<ExitDirection> possibleExitDirs = new List<ExitDirection>{
                ExitDirection.BOTTOM,
                ExitDirection.LEFT,
                ExitDirection.RIGHT,
                ExitDirection.TOP
            };
            possibleExitDirs.Remove(exclude);
            return possibleExitDirs;
        }

        public static Dictionary<ExitDirection, Location> CreateExitDictionary(EnterDirection enterDir, Location enterLocation, IList<DeepWoodsExit> exits)
        {
            Dictionary<ExitDirection, Location> exitDictionary = new Dictionary<ExitDirection, Location>();
            exitDictionary.Add(CastEnterDirToExitDir(enterDir), enterLocation);
            foreach (var exit in exits)
            {
                if (!exitDictionary.ContainsKey(exit.ExitDir))
                    exitDictionary.Add(exit.ExitDir, exit.Location);
                else if (exitDictionary[exit.ExitDir] != exit.Location)
                    throw new ApplicationException("Invalid state in CreateExitDictionary: got conflicting enter and exit locations!");
            }
            return exitDictionary;
        }
    }
}
