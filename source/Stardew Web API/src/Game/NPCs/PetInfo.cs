/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zunderscore/StardewWebApi
**
*************************************************/

using StardewValley;
using StardewValley.Characters;

namespace StardewWebApi.Game.NPCs;

public class PetInfo : NPCInfo
{
    private readonly Pet _pet;

    public PetInfo(NPC npc) : base(npc)
    {
        _pet = (Pet)_npc;
    }

    public Guid PetId => _pet.petId.Value;
    public string PetType => _pet.petType.Value;
    public int TimesPet => _pet.timesPet.Value;
}