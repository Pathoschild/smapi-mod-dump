/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Prism-99/Think-n-Talk
**
*************************************************/

using System;
using System.Collections.Generic;
using StardewModdingAPI;
using System.IO;
using System.Linq;
using StardewValley;
using Microsoft.Xna.Framework;
using xTile;
using xTile.Tiles;
using xTile.ObjectModel;
using System.Text;
using SDVObject = StardewValley.Object;
using StardewValley.Objects;
using Microsoft.Xna.Framework.Graphics;

namespace StardewModHelpers
{
    internal static class SDV_Logger
    {
        public static IMonitor Monitor;
        public static bool Debug;
        public static List<string> LogLines;
        private static string RootDir;
        //private static PrismLogger Logger;
        public static void Init(IMonitor mMonitor, string sRootDir, bool bDebug)
        {
            //Logger = new PrismLogger(sRootDir, bDebug, Monitor);
            Monitor = mMonitor;
            Debug = bDebug;
            LogLines = new List<string> { };
            RootDir = sRootDir;
        }
        public static void LogMessage(string message, LogLevel lLevel, string label, bool showInDebugPanel)
        {
            var dt = DateTime.Now;
            string time = dt.ToString("yyyy-MM-dd") + " " + dt.ToString("HH:mm");
            string messageWithLabel = (label == "" ? ("   " + message) : (message == "" ? label : label + ": " + message));
            string line = messageWithLabel;
            string detailline = "[" + time + "][" + lLevel.ToString() + "] " + messageWithLabel + "\n";
            if (Debug || lLevel == LogLevel.Error) LogLines.Add(detailline);
            //if ((Debug && showInDebugPanel) || lLevel == LogLevel.Error|| lLevel==LogLevel.Info)
            //{
            Monitor.Log(line, lLevel);
            //}
        }
        public static void LogMessage(string message, string label = null, bool showInDebugPanel = false)
        {
            LogMessage(message, LogLevel.Trace, label, showInDebugPanel);
        }

        public static void LogMessage(string message, LogLevel lLevel)
        {
            LogMessage(message, lLevel, "", false);
        }
        public static void LogError(string sMethodId, Exception err)
        {
            LogError(sMethodId, "Threw an error.");
            //LogError("Details", err.ToString());
            DumpObject("err", err);
        }
        public static void LogError(string sMethodId, string sErrText)
        {
            LogMessage(sErrText, LogLevel.Error, sMethodId, true);
        }
        public static void LogDebug(string sMethodId, string sErrText)
        {
            LogMessage(sErrText, LogLevel.Debug, sMethodId, false);
        }
        public static void LogTrace(string sMethodId, string sErrText)
        {
            LogMessage(sErrText, LogLevel.Trace, sMethodId, false);
        }
        public static void LogWarning(string sMethodId, string sErrText)
        {
            LogMessage(sErrText, LogLevel.Warn, sMethodId, false);
        }
        public static void LogInfo(string sMethodId, string sErrText)
        {
            LogMessage(sErrText, LogLevel.Info, sMethodId, false);
        }
        public static void LogAlert(string sMethodId, string sErrText)
        {
            LogMessage(sErrText, LogLevel.Alert, sMethodId, true);
        }

        public static void DumpObject(string sName,NPC oDump)
        {
            //
            //  dump NPC Characteristics
            //
            LogTrace("NPC Dump", oDump.Name);
            DumpObject("  sprite textureName", oDump.sprite.Value.textureName);
            DumpObject("  Portrait Name", oDump.Portrait.Name);
            DumpObject("  birthday_Day", oDump.birthday_Day);
            DumpObject("  birthday_Season", oDump.birthday_Season.Value);
            DumpObject("  datable", oDump.datable.Value);
            DumpObject("  defaultMap", oDump.defaultMap.Value);
            DumpObject("  gender", oDump.gender.Value);
            DumpObject("  isSleeping", oDump.isSleeping.Value);
            DumpObject("  CanSocialize", oDump.CanSocialize);
            DumpObject("  isVillager", oDump.isVillager());
            DumpObject("  getMugShotSourceRect", oDump.getMugShotSourceRect());
        }

        public static void DumpObject(string sName, SDVObject oObject)
        {
            if (oObject == null)
            {
                LogTrace("SDVObject " + sName, " is null");
            }
            else
            {
                LogTrace("SDVObject " + sName, "Dump");
                DumpObject("   Type", oObject.GetType().Name);
                DumpObject("   name", oObject.Name);
                DumpObject("   displayname", oObject.DisplayName);
                DumpObject("   category", oObject.category.Value);
                DumpObject("   parentindex", oObject.parentSheetIndex.Value);
                DumpObject("   description", oObject.getDescription());
                DumpObject("   contexttags value", oObject.GetContextTagList());
                DumpObject("   parentSheetIndex", oObject.parentSheetIndex.Value);
                DumpObject("   heldObject", oObject.heldObject.Value == null ? "Nothing" : oObject.heldObject.Value.displayName ?? oObject.heldObject.Value.name);
                DumpObject("   preservedParentSheetIndex", oObject.preservedParentSheetIndex);
            }
        }
        public static void DumpObject(string sName, Item oItem)
        {
            if (oItem == null)
            {
                LogTrace("Item " + sName, " is null");
            }
            else
            {
                LogTrace("Item " + sName, "Dump");
                DumpObject("   Type", oItem.GetType().Name);
                DumpObject("   name", oItem.Name);
                DumpObject("   displayname", oItem.DisplayName);
                DumpObject("   category", oItem.category.Value);
                DumpObject("   parentindex", oItem.parentSheetIndex.Value);
                DumpObject("   description", oItem.getDescription());
                DumpObject("   contexttags value", oItem.GetContextTagList());
                DumpObject("   parentSheetIndex", oItem.parentSheetIndex.Value);
            }
        }
        internal static void DumpObject(string sName, Rectangle oRect)
        {
            LogTrace(sName, "Rectangle (" + oRect.Left.ToString() + "," + oRect.Top.ToString() + "," + oRect.Width.ToString() + "," + oRect.Height.ToString() + ")");
        }
        internal static void DumpObject(string sName, List<Int32> dList)
        {
            LogTrace(sName, dList.Count == 0 ? "List<Int32> is empty" : string.Join(", ", dList));
        }
        internal static void DumpObject(string sName, List<string> dList)
        {
            LogTrace(sName, dList.Count == 0 ? "List<string> is empty" : "'" + string.Join("', '", dList) + "'");
        }
        internal static void DumpObject(string sName, string sValue)
        {
            LogTrace(sName, sValue == null ? "null" : "'" + sValue.Replace("\n", " ^ ") + "'");
        }
        internal static void DumpObject(string sName, int iValue)
        {
            LogTrace(sName, iValue.ToString());
        }
        internal static void DumpObject(string sName, double dValue)
        {
            LogTrace(sName, dValue.ToString());
        }
        internal static void DumpObject(string sName, bool bValue)
        {
            LogTrace(sName, bValue.ToString());
        }
        internal static void DumpObject(string sName, List<GameLocation> lDump)
        {
            LogTrace("List<GameLocation> " + sName, "Dump of " + lDump.Count.ToString() + " GameLocations");
            LogTrace("{", "");
            foreach (GameLocation oGL in lDump)
            {
                DumpObject("oGL", oGL);
                LogTrace(",", "");
            }
            LogTrace("}", "");
        }
        internal static void DumpObject(string sName, GameLocation oLocation)
        {
            LogTrace("GameLocation " + sName, "");
            LogTrace("", "{");

            DumpObject("   name", oLocation.Name);
            DumpObject("   mapPath", oLocation.mapPath.Value);
            DumpObject("map", oLocation.map);

            LogTrace("", "}");
        }
        internal static void DumpObject(string sName, Map oMap)
        {
            if (oMap == null)
            {
                LogTrace("Map is null", sName);
            }
            else
            {
                LogTrace("xTile.Map ", sName);
                LogTrace("", "{");

                DumpObject("   Description", oMap.Description);
                DumpObject("   DisplayWidth", oMap.DisplayWidth);
                DumpObject("   DisplayHeight", oMap.DisplayHeight);
                DumpObject("   assetPath", oMap.assetPath);
                DumpObject("   Id", oMap.Id);
                DumpObject("Properties", (PropertyCollection)oMap.Properties);
                LogTrace("", "}");
            }

        }
        internal static void DumpObject(string sName, PropertyCollection oProps)
        {
            LogTrace("PropertyCollection " + sName, "Dump of " + oProps.Count.ToString() + " PropertyValues");
            LogTrace("{", "");
            foreach (string key in oProps.Keys)
            {
                DumpObject(key, oProps[key]);
            }

            LogTrace("}", "");
        }
        internal static void DumpObject(string sName, PropertyValue oProp)
        {
            DumpObject(sName, oProp.ToString());
        }

        internal static void DumpObject(string sName, List<TileSheet> lTiles)
        {
            LogTrace("List<TileSheet> " + sName, "Dump of " + lTiles.Count.ToString() + " TileSheets");
            if (lTiles == null)
            {
                LogDebug("", "List<TileSheet> is null");
            }
            else
            {
                LogTrace("{", "");
                foreach (TileSheet oTile in lTiles)
                {
                    DumpObject("oTile", oTile);
                    LogTrace(",", "");
                }
                LogTrace("}", "");
            }
        }
        internal static void DumpObject(string sName, TileSheet oTile)
        {
            LogTrace("TileSheet " + sName, "");
            if (oTile == null)
            {
                LogTrace("", "TileSheet is null");
            }
            else
            {
                LogTrace("", "{");
                DumpObject("   Description", oTile.Description);
                DumpObject("   Map", oTile.Map);
                DumpObject("   TileCount", oTile.TileCount);
                DumpObject("Properties", oTile.Properties);
                LogTrace("", "}");
            }
        }

        internal static void DumpObject(string sName, List<Dictionary<string, object>> lDump)
        {
            LogTrace("List<Dictionary<int,object>> " + sName, "Dump of " + lDump.Count.ToString() + " Dictionary<string,object>");
            LogTrace("{", "");

            foreach (Dictionary<string, object> oSubItem in lDump)
            {
                DumpObject("oSubItem", oSubItem);
            }
            LogTrace("}", "");
        }
        internal static void DumpObject(string sName, Dictionary<string, object> dDump)
        {
            if (dDump == null)
            {
                LogTrace("Dictionary<string, object> " + sName, " is null");
            }
            else
            {
                try
                {
                    LogTrace("Dictionary<string, object> " + sName, "Dictionary item count: " + dDump.Count.ToString());
                    LogTrace("{", "");
                    foreach (string key in dDump.Keys)
                    {
                        if (dDump[key] == null)
                        {
                            LogTrace(key, "null");
                        }
                        else
                        {
                            if (dDump[key] is List<Dictionary<string, object>>)
                            {
                                DumpObject(key, (List<Dictionary<string, object>>)dDump[key]);
                            }
                            else
                            {
                                DumpObject(key, dDump[key]);
                            }
                        }

                    }
                    LogTrace("}", "");
                }
                catch (Exception ex)
                {
                    LogError("Dictionary<string,object> " + sName, ex.ToString());
                }
            }
        }
        internal static void DumpObject(string sName, Dictionary<int, int> dcDump)
        {
            LogTrace("Dictionary<int,int> " + sName, "Dump of " + dcDump.Count.ToString() + " Dictionary<int,int>");
            LogTrace("{", "");

            foreach (int key in dcDump.Keys)
            {
                DumpObject(key.ToString(), dcDump[key]);
            }

            LogTrace("}", "");
        }
        internal static void DumpObject(string sName, Dictionary<int, string> dcDump)
        {
            LogTrace("Dictionary<int,string> " + sName, "Dump of " + dcDump.Count.ToString() + " Dictionary<int,string>");
            LogTrace("{", "");

            foreach (int key in dcDump.Keys)
            {
                DumpObject(key.ToString(), dcDump[key]);
            }

            LogTrace("}", "");
        }
        internal static void DumpObject(string sName, Dictionary<string, string> dcDump)
        {
            if (dcDump == null)
            {
                LogTrace("Dictionary<string,string> " + sName, "is null");
            }
            else
            {
                LogTrace("Dictionary<string,string> " + sName, "Dump of " + dcDump.Count.ToString() + " values");
                LogTrace("{", "");

                foreach (string key in dcDump.Keys)
                {
                    DumpObject(key, dcDump[key]);
                }

                LogTrace("}", "");
            }
        }

        internal static void DumpObject(string sName, Furniture oFurniture)
        {
            if (oFurniture == null)
            {
                LogTrace("Chest " + sName, "is null");
            }
            else
            {
                LogTrace("Chest " + sName, "");
                LogTrace("", "{");
                DumpObject("   name", oFurniture.Name);
                DumpObject("   netName", oFurniture.netName.Value);
                DumpObject("   category", oFurniture.category.Value);
                DumpObject("   chestType", oFurniture.furniture_type.Value);
                DumpObject("   DisplayName", oFurniture.DisplayName);
                DumpObject("   CategoryName", oFurniture.getCategoryName());
                DumpObject("   ContextTags", oFurniture.GetContextTagList());
                DumpObject("   Description", oFurniture.getDescription());
                DumpObject("   RemainingStackSpace", oFurniture.getRemainingStackSpace());
                DumpObject("   heldObject", oFurniture.heldObject);
                DumpObject("   modData", oFurniture.modData);

                LogTrace("", "}");
            }
        }
        internal static void DumpObject(string sName, Chest oChest)
        {
            if (oChest == null)
            {
                LogTrace("Chest " + sName, "is null");
            }
            else
            {
                LogTrace("Chest " + sName, "");
                LogTrace("", "{");
                DumpObject("   name", oChest.Name);
                DumpObject("   netName", oChest.netName.Value);
                DumpObject("   category", oChest.category.Value);
                DumpObject("   chestType", oChest.chestType.Value);
                DumpObject("   DisplayName", oChest.DisplayName);
                DumpObject("   CategoryName", oChest.getCategoryName());
                DumpObject("   ContextTags", oChest.GetContextTagList());
                DumpObject("   Description", oChest.getDescription());
                DumpObject("   RemainingStackSpace", oChest.getRemainingStackSpace());
                DumpObject("   items", oChest.items);
                DumpObject("   modData", oChest.modData);

                LogTrace("", "}");
            }
        }
        internal static void DumpObject(string sName, ModDataDictionary oData)
        {
            if (oData == null)
            {
                LogTrace("ModDataDictionary " + sName, "is null");
            }
            else
            {
                LogTrace("ModDataDictionary " + sName, "");
                LogTrace("{", "");

                foreach (string key in oData.Keys)
                {
                    DumpObject("  " + key, oData[key]);
                }
                LogTrace("}", "");
            }
        }
        internal static void DumpObject(string sName, Netcode.NetObjectList<Item> oList)
        {
            if (oList == null)
            {
                LogTrace("NetObjectList<Item> " + sName, "is null");
            }
            else
            {
                LogTrace("NetObjectList<Item> " + sName, "");
                LogTrace("{", "");
                foreach (Item oItem in oList)
                {
                    DumpObject("oItem", oItem);
                    LogTrace(",", "");
                }
                LogTrace("{", "");
            }
        }

        internal static void DumpObject(string sName, Exception ex)
        {
            LogTrace("Exeception " + sName, "");
            LogTrace("{", "");
            DumpObject("   Message", ex.Message);
            DumpObject("   StackTrace", ex.StackTrace);
            DumpObject("   GetType", ex.GetType().ToString());
            DumpObject("   Source", ex.Source);
            LogTrace("}", "");
        }




        internal static void DumpObject(string sName, Vector2 vVector)
        {
            LogTrace("Vector2", sName);
            LogTrace("{", "");
            DumpObject("X", vVector.X);
            DumpObject("Y", vVector.Y);
            LogTrace("}", "");
        }
        internal static void DumpObject(string sName, StardewBitmap oBitmap)
        {
            if (oBitmap == null)
            {
                LogTrace($"StardewBitmap {sName}", "is null");
            }
            else
            {
                LogTrace("StardewBitmap", sName);
                LogTrace("{", "");
                DumpObject("    width", oBitmap.Width);
                DumpObject("    height", oBitmap.Height);

                LogTrace("}", "");
            }
        }
        internal static void DumpObject(string sName,Texture2D oTexture )
        {
            if (oTexture == null)
            {
                LogTrace($"Texture2D {sName}", "is null");
            }
            else
            {
                LogTrace("Texture2D", sName);
                LogTrace("{", "");
                DumpObject("   Width", oTexture.Width);
                DumpObject("   Height", oTexture.Height);
                LogTrace("}", "");
            }
        }

        internal static void DumpObject(string sName, object oObject)
        {
            if (oObject == null)
            {
                LogTrace(sName, " is null");
            }
            else
            {
                switch (oObject)
                {
                    case int _:
                        DumpObject(sName, (int)oObject);
                        break;
                    case double _:
                    case float _:
                        DumpObject(sName, (double)oObject);
                        break;
                    case bool _:
                        DumpObject(sName, Convert.ToBoolean(oObject.ToString()));
                        break;
                    case List<Int32> _:
                        DumpObject(sName, (List<Int32>)oObject);
                        break;
                    case List<string> _:
                        DumpObject(sName, (List<string>)oObject);
                        break;
                    case List<Dictionary<string, object>> _:
                        DumpObject(sName, (List<Dictionary<string, object>>)oObject);
                        break;
                    case string _:
                        DumpObject(sName, oObject.ToString());
                        break;
                    case Dictionary<int, int> _:
                        DumpObject(sName, (Dictionary<int, int>)oObject);
                        break;
                    case Dictionary<string, string> _:
                        DumpObject(sName, (Dictionary<string, string>)oObject);
                        break;
                    case Dictionary<string, object> _:
                        DumpObject(sName, (Dictionary<string, object>)oObject);
                        break;
                    default:
                        LogTrace("Object Type", (oObject == null ? "null" : oObject.GetType().ToString()));
                        DumpObject(sName, oObject.ToString());
                        break;
                }
            }
        }
        internal static void DumpObject(string sName, string[] oArray)
        {
            LogTrace("string[] " + sName, oArray.Length.ToString() + " elements");
            for (int iCounter = 0; iCounter < oArray.Length; iCounter++)
            {
                DumpObject("[" + iCounter.ToString() + "]", oArray[iCounter]);
            }
        }


        public static void AppendToLogFile(string sLogFile, string sText)
        {
            if (Debug)
            {
                try
                {
                    using StreamWriter sw = File.AppendText(Path.Combine(RootDir, "logs", sLogFile));
                    sw.WriteLine(sText);
                }
                catch { }
            }
        }
    }
}
