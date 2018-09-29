// Decompiled with JetBrains decompiler
// Type: SerializerUtils.SerializerUtility
// Assembly: SerializerUtils, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1302D1C7-A0C7-48FC-8C60-C8B9A49AF5B0
// Assembly location: C:\Users\owner\Documents\Visual Studio 2015\Projects\github\Stardew_Valley_Mods\Stardew_Valley_Mods\Save_Anywhere_V2\Save_Anywhere_V2\bin\Debug\SerializerUtils.dll

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Monsters;
using StardewValley.Quests;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SerializerUtils
{
  public class SerializerUtility : Mod
  {
    protected static Type[] vanillaTypes = new Type[27]
    {
      typeof (Tool),
      typeof (GameLocation),
      typeof (Crow),
      typeof (Duggy),
      typeof (Bug),
      typeof (BigSlime),
      typeof (Fireball),
      typeof (Ghost),
      typeof (Child),
      typeof (Pet),
      typeof (Dog),
      typeof (StardewValley.Characters.Cat),
      typeof (Horse),
      typeof (GreenSlime),
      typeof (LavaCrab),
      typeof (RockCrab),
      typeof (ShadowGuy),
      typeof (SkeletonMage),
      typeof (SquidKid),
      typeof (Grub),
      typeof (Fly),
      typeof (DustSpirit),
      typeof (Quest),
      typeof (MetalHead),
      typeof (ShadowGirl),
      typeof (Monster),
      typeof (TerrainFeature)
    };
    public static Type[] vanillaFarmerTypes = new Type[1]
    {
      typeof (Tool)
    };
    public static List<Type> newTypes = new List<Type>();
    public static List<Type> newFarmerTypes = new List<Type>()
    {
      typeof (Tool)
    };

    public override void Entry(params object[] objects)
    {
      GameEvents.GameLoaded += new EventHandler(SerializerUtility.Event_GameLoaded);
      Command.RegisterCommand("include_types", "Includes types to serialize", (string[]) null).CommandFired += new EventHandler<EventArgsCommand>(SerializerUtility.Command_IncludeTypes);
    }

    public static void Command_IncludeTypes(object sender, EventArgsCommand e)
    {
    }

    public static void Event_GameLoaded(object sender, EventArgs e)
    {
      Command.CallCommand("include_types");
      SerializerUtility.InvokeSerializerOverride();
    }

    public static void InvokeSerializerOverride()
    {
      List<Type> list1 = new List<Type>();
      list1.AddRange((IEnumerable<Type>) SerializerUtility.vanillaTypes);
      list1.AddRange((IEnumerable<Type>) SerializerUtility.newTypes);
      List<Type> list2 = new List<Type>();
      list2.AddRange((IEnumerable<Type>) SerializerUtility.vanillaFarmerTypes);
      list2.AddRange((IEnumerable<Type>) SerializerUtility.newFarmerTypes);
      SaveGame.serializer = new XmlSerializer(typeof (SaveGame), list1.ToArray());
      SaveGame.farmerSerializer = new XmlSerializer(typeof (Farmer), list2.ToArray());
    }

    public static void AddType(Type t)
    {
      SerializerUtility.newTypes.Add(t);
    }

    public static void AddFarmerType(Type t)
    {
      SerializerUtility.newFarmerTypes.Add(t);
    }
  }
}
