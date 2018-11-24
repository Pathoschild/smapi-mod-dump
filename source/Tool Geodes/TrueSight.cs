using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolGeodes
{
    public static class TrueSight
    {
        private static Dictionary<int, StardewValley.Object> drawObjs = new Dictionary<int, StardewValley.Object>();
        internal static void onDrawWorld(object sender, RenderedWorldEventArgs args)
        {
            if (!Context.IsWorldReady)
                return;

            var b = args.SpriteBatch;

            foreach (var pair in Game1.currentLocation.netObjects.Pairs)
            {
                var pos = pair.Key;
                var obj = pair.Value;

                int doDraw = -1;
                if (Game1.player.HasAdornment(ToolType.Pickaxe, Mod.Config.GEODE_OBJ_TRUESIGHT) > 0)
                {
                    if (!(Game1.currentLocation is MineShaft))
                    {
                        if (obj.ParentSheetIndex == 343 || obj.ParentSheetIndex == 450)
                        {

                            Random rand = new Random((int)Game1.stats.DaysPlayed + (int)Game1.uniqueIDForThisGame / 2 + (int)pos.X * 2000 + (int)pos.Y);
                            if (rand.NextDouble() < 0.035 && Game1.stats.DaysPlayed > 1U)
                                doDraw = 535 + (Game1.stats.DaysPlayed <= 60U || rand.NextDouble() >= 0.2 ? (Game1.stats.DaysPlayed <= 120U || rand.NextDouble() >= 0.2 ? 0 : 2) : 1);
                            if (rand.NextDouble() < 0.035 * (Game1.player.professions.Contains(21) ? 2.0 : 1.0) && Game1.stats.DaysPlayed > 1U)
                                doDraw = 382;
                            if (rand.NextDouble() < 0.01 && Game1.stats.DaysPlayed > 1U)
                                doDraw = 390;

                            if (doDraw == 390) // 390 is more stone
                                continue;

                        }
                    }
                    else if (obj.Name.Contains("Stone"))
                    {
                        doDraw = mineDrops(obj.ParentSheetIndex, (int)pos.X, (int)pos.Y, Game1.player, (Game1.currentLocation as MineShaft));
                    }
                }
                if ( Game1.player.HasAdornment(ToolType.Hoe, Mod.Config.GEODE_OBJ_TRUESIGHT ) > 0 )
                {
                    if ( obj.ParentSheetIndex == 590 )
                    {
                        doDraw = digUpArtifactSpot((int)pos.X, (int)pos.Y, Game1.player, obj.name);
                    }
                }

                if (doDraw != -1)
                {
                    if (doDraw == -2)
                    {
                        var ts = Game1.content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>(Game1.currentLocation.Map.TileSheets[0].ImageSource);
                        b.Draw(ts, Game1.GlobalToLocal(Game1.viewport, new Vector2(pos.X * 64, pos.Y * 64)), new Rectangle(208, 160, 16, 16), new Color(255, 255, 255, 128), 0, Vector2.Zero, 4, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 1);
                    }
                    else
                    {
                        if (!drawObjs.ContainsKey(doDraw))
                        {
                            drawObjs.Add(doDraw, new StardewValley.Object(new Vector2(0, 0), doDraw, 1));
                        }
                        var dobj = drawObjs[doDraw];
                        dobj.drawInMenu(b, Game1.GlobalToLocal(Game1.viewport, new Vector2(pos.X * 64, pos.Y * 64)), 0.8f, 0.5f, 1, false, Color.White, false);
                    }
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("SMAPI.CommonErrors", "AvoidNetField")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("SMAPI.CommonErrors", "AvoidImplicitNetFieldCast")]
        public static int mineDrops(int tileIndexOfStone, int x, int y, Farmer who, StardewValley.Locations.MineShaft ms)
        {
            int mineLevel = ms.mineLevel;
            int stonesLeftOnThisLevel = Mod.instance.Helper.Reflection.GetProperty<int>(ms, "stonesLeftOnThisLevel").GetValue();
            bool ladderHasSpawned = Mod.instance.Helper.Reflection.GetField<bool>(ms, "ladderHasSpawned").GetValue();

            if (who == null)
                who = Game1.player;
            double num1 = Game1.dailyLuck / 2.0 + (double)who.MiningLevel * 0.005 + (double)who.LuckLevel * 0.001;
            Random r = new Random(x * 1000 + y + mineLevel + (int)Game1.uniqueIDForThisGame / 2);
            r.NextDouble();
            double num2 = tileIndexOfStone == 40 || tileIndexOfStone == 42 ? 1.2 : 0.8;
            //if (tileIndexOfStone != 34 && tileIndexOfStone != 36 && tileIndexOfStone != 50)
            //    ;
            --stonesLeftOnThisLevel;
            double num3 = 0.02 + 1.0 / (double)Math.Max(1, stonesLeftOnThisLevel) + (double)who.LuckLevel / 100.0 + Game1.dailyLuck / 5.0;
            if (ms.characters.Count == 0)
                num3 += 0.04;
            if (!ladderHasSpawned && (stonesLeftOnThisLevel == 0 || r.NextDouble() < num3))
                return -2;
            int bs = breakStone(tileIndexOfStone, x, y, who, r, ms);
            if (bs != -1)
                return bs;
            if (tileIndexOfStone == 44)
            {
                int num4 = r.Next(59, 70);
                int objectIndex = num4 + num4 % 2;
                if (who.timesReachedMineBottom == 0)
                {
                    if (mineLevel < 40 && objectIndex != 66 && objectIndex != 68)
                        objectIndex = r.NextDouble() < 0.5 ? 66 : 68;
                    else if (mineLevel < 80 && (objectIndex == 64 || objectIndex == 60))
                        objectIndex = r.NextDouble() < 0.5 ? (r.NextDouble() < 0.5 ? 66 : 70) : (r.NextDouble() < 0.5 ? 68 : 62);
                }
                return objectIndex;
            }
            else
            {
                if (r.NextDouble() < 0.022 * (1.0 + num1) * (who.professions.Contains(22) ? 2.0 : 1.0))
                {
                    int objectIndex = 535 + (ms.getMineArea(-1) == 40 ? 1 : (ms.getMineArea(-1) == 80 ? 2 : 0));
                    if (ms.getMineArea(-1) == 121)
                        objectIndex = 749;
                    if (who.professions.Contains(19) && r.NextDouble() < 0.5)
                        return objectIndex;
                    return objectIndex;
                }
                if (mineLevel > 20 && r.NextDouble() < 0.005 * (1.0 + num1) * (who.professions.Contains(22) ? 2.0 : 1.0))
                {
                    if (who.professions.Contains(19) && r.NextDouble() < 0.5)
                        return 749;
                    return 749;
                }
                if (r.NextDouble() < 0.05 * (1.0 + num1) * num2)
                {
                    r.Next(1, 3);
                    r.NextDouble();
                    double num4 = 0.1 * (1.0 + num1);
                    if (r.NextDouble() < 0.25)
                    {
                        return 382;
                    }
                    else
                        return ms.getOreIndexForLevel(mineLevel, r);
                }
                else
                {
                    if (r.NextDouble() >= 0.5)
                        return -1;
                }
            }

            return -1;
        }

        // Note: I can't get this to work for 74 (prismatic shards) correctly
        // I commented out the part that makes it appear for now
        [System.Diagnostics.CodeAnalysis.SuppressMessage("SMAPI.CommonErrors", "AvoidNetField")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("SMAPI.CommonErrors", "AvoidImplicitNetFieldCast")]
        public static int breakStone(int indexOfStone, int x, int y, Farmer who, Random r, MineShaft ms)
        {
            int ret = -1;
            int num1 = who.professions.Contains(18) ? 1 : 0;
            switch (indexOfStone)
            {
                case 2:
                    ret = 72;
                    break;
                case 4:
                    ret = 64;
                    break;
                case 6:
                    ret = 70;
                    break;
                case 8:
                    ret = 66;
                    break;
                case 10:
                    ret = 68;
                    break;
                case 12:
                    ret = 60;
                    break;
                case 14:
                    ret = 62;
                    break;
                case 75:
                    ret = 535;
                    break;
                case 76:
                    ret = 536;
                    break;
                case 77:
                    ret = 537;
                    break;
                case 290:
                    ret = 380;
                    r.NextDouble(); r.NextDouble();
                    break;
                case 668:
                case 670:
                    ret = 390;
                    r.NextDouble(); r.NextDouble();
                    if (r.NextDouble() < 0.08)
                    {
                        ret = 382;
                        break;
                    }
                    break;
                case 751:
                    ret = 378;
                    r.NextDouble(); r.NextDouble();
                    break;
                case 764:
                    ret = 384;
                    r.NextDouble(); r.NextDouble();
                    break;
                case 765:
                    ret = 386; ;
                    r.NextDouble(); r.NextDouble();
                    if (r.NextDouble() < 0.04)
                        ;// ret = 74;
                    break;
            }
            if (who.professions.Contains(19) && r.NextDouble() < 0.5)
            {
                switch (indexOfStone)
                {
                    case 2:
                        ret = 72;
                        break;
                    case 4:
                        ret = 64;
                        break;
                    case 6:
                        ret = 70;
                        break;
                    case 8:
                        ret = 66;
                        break;
                    case 10:
                        ret = 68;
                        break;
                    case 12:
                        ret = 60;
                        break;
                    case 14:
                        ret = 62;
                        break;
                }
            }
            if (indexOfStone == 46)
            {
                r.Next(1, 4);
                r.Next(1, 5);
                if (r.NextDouble() < 0.25)
                    ;// ret = 74;
            }
            if (((bool)((NetFieldBase<bool, NetBool>)ms.isOutdoors) || (bool)((NetFieldBase<bool, NetBool>)ms.treatAsOutdoors)) && ret == -1)
            {
                double num2 = Game1.dailyLuck / 2.0 + (double)who.MiningLevel * 0.005 + (double)who.LuckLevel * 0.001;
                Random random = new Random(x * 1000 + y + (int)Game1.stats.DaysPlayed + (int)Game1.uniqueIDForThisGame / 2);

                if (who.professions.Contains(21) && random.NextDouble() < 0.05 * (1.0 + num2))
                    ret = 382;
                if (random.NextDouble() < 0.05 * (1.0 + num2))
                {
                    random.Next(1, 3);
                    random.NextDouble();
                    double num3 = 0.1 * (1.0 + num2);
                    ret = 382;
                }
            }
            if (who.hasMagnifyingGlass && r.NextDouble() < 0.01)
            {
                var unseenSecretNote = ms.tryToCreateUnseenSecretNote(who);
                if (unseenSecretNote != null)
                    ret = unseenSecretNote.ParentSheetIndex;
            }
            return ret;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("SMAPI.CommonErrors", "AvoidNetField")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("SMAPI.CommonErrors", "AvoidImplicitNetFieldCast")]
        public static int digUpArtifactSpot(int xLocation, int yLocation, Farmer who, string name)
        {
            Random random = new Random(xLocation * 2000 + yLocation + (int)Game1.uniqueIDForThisGame / 2 + (int)Game1.stats.DaysPlayed);
            int objectIndex = -1;
            foreach (KeyValuePair<int, string> keyValuePair in (IEnumerable<KeyValuePair<int, string>>)Game1.objectInformation)
            {
                string[] strArray1 = keyValuePair.Value.Split('/');
                if (strArray1[3].Contains("Arch"))
                {
                    string[] strArray2 = strArray1[6].Split(' ');
                    int index = 0;
                    while (index < strArray2.Length)
                    {
                        if (strArray2[index].Equals((string)((NetFieldBase<string, NetString>)Game1.currentLocation.name)) && random.NextDouble() < Convert.ToDouble(strArray2[index + 1], (IFormatProvider)CultureInfo.InvariantCulture))
                        {
                            objectIndex = keyValuePair.Key;
                            break;
                        }
                        index += 2;
                    }
                }
                if (objectIndex != -1)
                    break;
            }
            if (random.NextDouble() < 0.2 && !(Game1.currentLocation is Farm))
                objectIndex = 102;
            if (objectIndex == 102 && who.archaeologyFound.ContainsKey(102) && who.archaeologyFound[102][0] >= 21)
                objectIndex = 770;
            if (objectIndex != -1)
            {
                return objectIndex;
            }
            else if (Game1.currentSeason.Equals("winter") && random.NextDouble() < 0.5 && !(Game1.currentLocation is Desert))
            {
                if (random.NextDouble() < 0.4)
                    return 416;
                else
                    return 412;
            }
            else
            {
                Dictionary<string, string> dictionary = Game1.content.Load<Dictionary<string, string>>("Data\\Locations");
                if (!dictionary.ContainsKey((string)((NetFieldBase<string, NetString>)Game1.currentLocation.name)))
                    return -1;
                string[] strArray = dictionary[(string)((NetFieldBase<string, NetString>)Game1.currentLocation.name)].Split('/')[8].Split(' ');
                if (strArray.Length == 0 || strArray[0].Equals("-1"))
                    return -1;
                int index1 = 0;
                while (index1 < strArray.Length)
                {
                    if (random.NextDouble() <= Convert.ToDouble(strArray[index1 + 1]))
                    {
                        int index2 = Convert.ToInt32(strArray[index1]);
                        if (Game1.objectInformation.ContainsKey(index2))
                        {
                            if (Game1.objectInformation[index2].Split('/')[3].Contains("Arch") || index2 == 102)
                            {
                                if (index2 == 102 && who.archaeologyFound.ContainsKey(102) && who.archaeologyFound[102][0] >= 21)
                                    index2 = 770;
                                return index2;
                            }
                        }
                        if (index2 == 330 && who.hasMagnifyingGlass && Game1.random.NextDouble() < 0.11)
                        {
                            StardewValley.Object unseenSecretNote = Game1.currentLocation.tryToCreateUnseenSecretNote(who);
                            if (unseenSecretNote != null)
                            {
                                return 79;
                            }
                        }
                        return index2;
                    }
                    index1 += 2;
                }
            }
            return -1;
        }
    }
}
