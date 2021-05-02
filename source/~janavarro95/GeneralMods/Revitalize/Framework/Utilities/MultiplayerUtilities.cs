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
using Revitalize.Framework.Objects;
using StardewValley;

namespace Revitalize.Framework.Utilities
{
    /// <summary>
    /// Deals with syncing objects in multiplayer.
    /// </summary>
    public static class MultiplayerUtilities
    {
        public static string RequestGUIDMessage = "Revitalize.RequestGUIDObject";
        public static string RequestGUIDMessage_Tile = "Revitalize.RequestGUIDObject_Tile";
        public static string ReceieveGUIDMessage = "Revitalize.ReceieveGUIDObject";
        public static string ReceieveGUIDMessage_Tile = "Revitalize.ReceieveGUIDObject_Tile";
        public static string RequestALLModObjects = "Revitalize.EndOfDayRequestAllObjects";
        public static string RequestObjectUpdateSync = "Revitalize.RequestObjectUpdateSync";

        public static Multiplayer GameMultiplayer;

        /// <summary>
        /// Handles receiving mod messages.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        public static void GetModMessage(object o, StardewModdingAPI.Events.ModMessageReceivedEventArgs e)
        {
            //ModCore.log("Get a mod message: "+e.Type);
            if (e.Type.Equals(RequestGUIDMessage))
            {
                //ModCore.log("Send GUID Request");
                Guid request = Guid.Parse(e.ReadAs<string>());
                SendGuidObject(request);
            }

            if (e.Type.Equals(ReceieveGUIDMessage))
            {
                //ModCore.log("Receieve GUID Request");
                string objStr = e.ReadAs <string>();
                CustomObject v=(CustomObject)ModCore.Serializer.DeserializeFromJSONString<Item>(objStr);
                if (ModCore.CustomObjects.ContainsKey((v as CustomObject).guid) == false)
                {
                    ModCore.CustomObjects.Add((v as CustomObject).guid, v);
                    v.info.forceUpdate();
                    v.updateInfo();
                }
                else
                {
                    ModCore.CustomObjects[(v as CustomObject).guid] = v;
                    v.info.forceUpdate();
                    v.updateInfo();
                }
            }

            if(e.Type.Equals(RequestGUIDMessage_Tile))
            {
                //odCore.log("Send GUID Request FOR TILE");
                Guid request = Guid.Parse(e.ReadAs<string>());
                SendGuidObject_Tile(request);
            }
            if(e.Type.Equals(ReceieveGUIDMessage_Tile))
            {
                //ModCore.log("Receieve GUID Request FOR TILE");
                string objStr = e.ReadAs<string>();
                CustomObject v =(CustomObject)ModCore.Serializer.DeserializeFromJSONString<Item>(objStr);
                if (ModCore.CustomObjects.ContainsKey((v as CustomObject).guid) == false)
                {
                    ModCore.CustomObjects.Add((v as CustomObject).guid, v);
                    v.info.forceUpdate();
                    v.updateInfo();
                }
                else
                {
                    ModCore.CustomObjects[(v as CustomObject).guid] = v;
                    v.info.forceUpdate();
                    v.updateInfo();
                }
            }

            if (e.Type.Equals(RequestALLModObjects))
            {
                List < KeyValuePair<Guid, CustomObject> > list = ModCore.CustomObjects.ToList();
                foreach(var v in list)
                {
                    (v.Value).updateInfo();
                    SendGuidObject(v.Key);
                }
            }

            if (e.Type.Equals(RequestObjectUpdateSync))
            {
                string guidString = e.ReadAs<string>();
                Guid guid = Guid.Parse(guidString);
                if (ModCore.CustomObjects.ContainsKey(guid))
                {
                    ModCore.CustomObjects[guid].getUpdate();
                }
            }
        }

        /// <summary>
        /// Sends a custom object to be synced.
        /// </summary>
        /// <param name="request"></param>
        public static void SendGuidObject(Guid request)
        {
            if (ModCore.CustomObjects.ContainsKey(request))
            {
                //ModCore.log("Send guid request: "+request.ToString());
                //ModCore.CustomObjects[request].forceUpdate();
                ModCore.CustomObjects[request].info.forceUpdate();
                ModCore.CustomObjects[request].updateInfo();

                ModCore.ModHelper.Multiplayer.SendMessage<string>(ModCore.Serializer.ToJSONString(ModCore.CustomObjects[request]), ReceieveGUIDMessage, new string[] { Revitalize.ModCore.Manifest.UniqueID.ToString() });
            }
            else
            {
                ModCore.log("This mod doesn't have the guid object");
            }
        }

        /// <summary>
        /// Sends the container object from the tile component object's guid.
        /// </summary>
        /// <param name="request"></param>
        public static void SendGuidObject_Tile(Guid request)
        {
            if (ModCore.CustomObjects.ContainsKey(request))
            {
                //ModCore.log("Send guid tile request!");
                //(ModCore.CustomObjects[request] as MultiTiledComponent).forceUpdate();
                //(ModCore.CustomObjects[request] as MultiTiledComponent).containerObject.forceUpdate();
                try
                {
                    (ModCore.CustomObjects[request] as MultiTiledComponent).containerObject.updateInfo();
                }
                catch(Exception err)
                {

                }
                ModCore.ModHelper.Multiplayer.SendMessage<string>(ModCore.Serializer.ToJSONString( (ModCore.CustomObjects[request] as MultiTiledComponent).containerObject), ReceieveGUIDMessage_Tile , new string[] { Revitalize.ModCore.Manifest.UniqueID.ToString() });
            }
            else
            {
                //ModCore.log("This mod doesn't have the guid tile");
            }
        }

        /// <summary>
        /// Requests the object from the given guid.
        /// </summary>
        /// <param name="request"></param>
        public static void RequestGuidObject(Guid request)
        {
            ModCore.ModHelper.Multiplayer.SendMessage<string>(request.ToString(),RequestGUIDMessage, new string[] { ModCore.Manifest.UniqueID.ToString() });
        }

        /// <summary>
        /// Requests a container object from  tile component object's guid.
        /// </summary>
        /// <param name="request"></param>
        public static void RequestGuidObject_Tile(Guid request)
        {
            ModCore.ModHelper.Multiplayer.SendMessage<string>(request.ToString(), RequestGUIDMessage_Tile, new string[] { ModCore.Manifest.UniqueID.ToString() });
        }

        /// <summary>
        /// Send a request to all other revitalize mods to get all of the synced guid objects that isn't held by this mod.
        /// </summary>
        public static void RequestALLGuidObjects()
        {
            ModCore.ModHelper.Multiplayer.SendMessage<string>(RequestALLModObjects, RequestALLModObjects,new string[] { ModCore.Manifest.UniqueID.ToString() });
        }

        /// <summary>
        /// Sends a request to all other revitalize mods to update the given guid object.
        /// </summary>
        /// <param name="request"></param>
        public static void RequestUpdateSync(Guid request)
        {
            ModCore.ModHelper.Multiplayer.SendMessage<string>(request.ToString(), RequestObjectUpdateSync, new string[] { ModCore.Manifest.UniqueID.ToString() });
        }

        public static StardewValley.Multiplayer GetMultiplayer()
        {
            if (GameMultiplayer == null)
            {
                Multiplayer multiplayer = ModCore.ModHelper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer", true).GetValue();
                if (multiplayer == null) return null;
                else
                {
                    GameMultiplayer = multiplayer;
                    return GameMultiplayer;
                }
            }
            else
            {
                return GameMultiplayer;
            }
        }

    }
}
