/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System;
using System.Linq;
using System.Xml.Serialization;
using SpaceCore.Patches;
using SpaceShared;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Monsters;
using StardewValley.Objects;
using StardewValley.Quests;
using StardewValley.TerrainFeatures;

namespace SpaceCore.Framework
{
    /// <summary>Manages the serialization for <see cref="LoadGameMenuPatcher"/> and <see cref="SaveGamePatcher"/>.</summary>
    internal class SerializerManager
    {
        /*********
        ** Fields
        *********/
        private bool InitializedSerializers;

        // Update these each game update
        private readonly Type[] VanillaMainTypes =
        {
            typeof(Tool),
            typeof(GameLocation),
            typeof(Duggy),
            typeof(Bug),
            typeof(BigSlime),
            typeof(Ghost),
            typeof(Child),
            typeof(Pet),
            typeof(Dog),
            typeof(Cat),
            typeof(Horse),
            typeof(GreenSlime),
            typeof(LavaCrab),
            typeof(RockCrab),
            typeof(ShadowGuy),
            typeof(SquidKid),
            typeof(Grub),
            typeof(Fly),
            typeof(DustSpirit),
            typeof(Quest),
            typeof(MetalHead),
            typeof(ShadowGirl),
            typeof(Monster),
            typeof(JunimoHarvester),
            typeof(TerrainFeature)
        };
        private readonly Type[] VanillaFarmerTypes =
        {
            typeof(Tool)
        };
        private readonly Type[] VanillaGameLocationTypes =
        {
            typeof(Tool),
            typeof(Duggy),
            typeof(Ghost),
            typeof(GreenSlime),
            typeof(LavaCrab),
            typeof(RockCrab),
            typeof(ShadowGuy),
            typeof(Child),
            typeof(Pet),
            typeof(Dog),
            typeof(Cat),
            typeof(Horse),
            typeof(SquidKid),
            typeof(Grub),
            typeof(Fly),
            typeof(DustSpirit),
            typeof(Bug),
            typeof(BigSlime),
            typeof(BreakableContainer),
            typeof(MetalHead),
            typeof(ShadowGirl),
            typeof(Monster),
            typeof(JunimoHarvester),
            typeof(TerrainFeature)
        };


        /*********
        ** Accessors
        *********/
        public readonly string Filename = "spacecore-serialization.json";
        public readonly string FarmerFilename = "spacecore-serialization-farmer.json";

        /// <summary>The save filename currently being loaded, if any.</summary>
        public string LoadFileContext = null;


        /*********
        ** Public methods
        *********/
        public void InitializeSerializers()
        {
            if (this.InitializedSerializers || !SpaceCore.ModTypes.Any())
                return;
            this.InitializedSerializers = true;

            Log.Trace("Reinitializing serializers...");

            SaveGame.serializer = this.InitializeSerializer(typeof(SaveGame), this.VanillaMainTypes);
            SaveGame.farmerSerializer = this.InitializeSerializer(typeof(Farmer), this.VanillaFarmerTypes);
            SaveGame.locationSerializer = this.InitializeSerializer(typeof(GameLocation), this.VanillaGameLocationTypes);
        }

        public XmlSerializer InitializeSerializer(Type baseType, Type[] extra = null)
        {
            var types = extra?.Length > 0
                ? extra.Concat(SpaceCore.ModTypes)
                : SpaceCore.ModTypes;

            XmlSerializer serializer = new(baseType, types.ToArray());
            this.NotifyPyTk(serializer);
            return serializer;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Notify PyTK that the serializers were changed, if it's installed.</summary>
        /// <param name="serializer">The XML serializer which changed.</param>
        private void NotifyPyTk(XmlSerializer serializer)
        {
            if (!SpaceCore.Instance.Helper.ModRegistry.IsLoaded("Platonymous.Toolkit"))
                return;

            const string errorPrefix = "PyTK is installed, but we couldn't notify it about serializer changes. PyTK serialization might not work correctly.\nTechnical details:";
            try
            {
                // fetch PyTK mod
                var mod = Type.GetType("PyTK.PyTKMod, PyTK");
                if (mod == null)
                {
                    Log.Monitor.LogOnce($"{errorPrefix} couldn't fetch its mod instance.");
                    return;
                }

                // fetch notify method
                const string methodName = "SerializersReinitialized";
                var method = mod.GetMethod(methodName);
                if (method == null)
                {
                    Log.Monitor.LogOnce($"{errorPrefix} couldn't fetch its '{methodName}' method.");
                    return;
                }

                // notify
                method.Invoke(null, new object[] { serializer });
            }
            catch (Exception ex)
            {
                Log.Monitor.LogOnce($"{errorPrefix} {ex}", LogLevel.Warn);
            }
        }
    }
}
