/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/SolidFoundations
**
*************************************************/

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SolidFoundations.Framework.Models.ContentPack
{
    [Obsolete("This class is only used to load the old cache of custom buildings from SDV v1.5.5+ into SDV v1.6 and should not be used otherwise.")]
    public class GenericBuilding
    {
        [XmlIgnore]
        public ExtendedBuildingModel Model { get; set; }
        public string Id { get; set; }
        public string LocationName { get; set; }

        private List<LightSource> LightSources { get; set; } = new List<LightSource>();

        // Start of backported properties
        [XmlIgnore]
        public string buildingLocation;

        [XmlElement("buildingChests")]
        public List<Chest> buildingChests = new List<Chest>();
        public readonly float animalDoorOpenAmount;
        [XmlIgnore]
        protected Dictionary<string, string> _buildingMetadata = new Dictionary<string, string>();
        protected int _lastHouseUpgradeLevel = -1;
        protected Vector2 _chimneyPosition = Vector2.Zero;
        protected bool? _hasChimney;
        protected int chimneyTimer = 500;
        public NetString skinID;
        public bool hasLoaded;
        public readonly string upgradeName;
        public GameLocation? indoors { get; set; }
        public int tileX { get; set; }
        public int tileY { get; set; }
        public readonly string nonInstancedIndoors;

        public GenericBuilding() : base()
        {

        }
    }
}
