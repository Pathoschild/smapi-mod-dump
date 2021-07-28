/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/StardewValleyMods
**
*************************************************/

// Decompiled with JetBrains decompiler
// Type: Paritee.StardewValley.Core.Characters.Pet
// Assembly: Paritee.StardewValley.Core, Version=2.0.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5A2FE3D9-1A06-4344-9586-3DF16623F5C9
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Mods\Paritee's Better Farm Animal Variety\Paritee.StardewValley.Core.dll

namespace Paritee.StardewValley.Core.Characters
{
  public class Pet
  {
    public static Animal Cat => new Animal(nameof(Cat));

    public static Animal Dog => new Animal(nameof(Dog));
  }
}