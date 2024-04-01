/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/AlternativeTextures
**
*************************************************/

using AlternativeTextures.Framework.Patches.Buildings;
using AlternativeTextures.Framework.Utilities;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using System;
using System.Linq;

namespace AlternativeTextures.Framework.Managers
{
    internal class MessageManager
    {
        private IMonitor _monitor;
        private IModHelper _helper;
        private string _modID;

        internal enum MessageType
        {
            Unknown,
            BuildingTextureUpdate
        }

        internal class BuildingTextureUpdateMessage
        {
            public string LocationName { get; set; }
            public Guid BuildingID { get; set; }
            public string TextureName { get; set; }
            public string TextureVariation { get; set; }
        }

        public MessageManager(IMonitor monitor, IModHelper helper, string modID)
        {
            _monitor = monitor;
            _helper = helper;
            _modID = modID;
        }

        public void HandleIncomingMessage(ModMessageReceivedEventArgs e)
        {
            if (Enum.TryParse<MessageType>(e.Type, out var type) is false)
            {
                _monitor.LogOnce($"Failed to handle incoming message with type {e.Type}", LogLevel.Trace);
                return;
            }

            switch (type)
            {
                case MessageType.BuildingTextureUpdate:
                    var message = e.ReadAs<BuildingTextureUpdateMessage>();

                    var location = Game1.getLocationFromName(message.LocationName);
                    if (location is not null && location.buildings is not null)
                    {
                        foreach (var building in location.buildings.Where(b => b is not null & b.id.Value == message.BuildingID))
                        {
                            BuildingPatch.ForceResetTexture(building, message.TextureName, message.TextureVariation);
                        }
                    }
                    return;
            }
        }

        public void SendBuildingTextureUpdate(Building building)
        {
            if (building.modData.ContainsKey(ModDataKeys.ALTERNATIVE_TEXTURE_NAME) is false || building.modData.ContainsKey(ModDataKeys.ALTERNATIVE_TEXTURE_VARIATION) is false)
            {
                return;
            }

            var message = new BuildingTextureUpdateMessage()
            {
                LocationName = building.GetParentLocation().NameOrUniqueName,
                BuildingID = building.id.Value,
                TextureName = building.modData[ModDataKeys.ALTERNATIVE_TEXTURE_NAME],
                TextureVariation = building.modData[ModDataKeys.ALTERNATIVE_TEXTURE_VARIATION]
            };
            _helper.Multiplayer.SendMessage(message, MessageType.BuildingTextureUpdate.ToString(), modIDs: new[] { _modID });
        }
    }
}
