/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-challenger
**
*************************************************/

using System;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Pathoschild.Stardew.Automate;
using Slothsoft.Challenger;
using Slothsoft.Challenger.Models;

namespace Slothsoft.ChallengerAutomate; 

/// <summary>
/// This machine must work something like these:
/// https://github.com/Pathoschild/StardewMods/blob/develop/Automate/Framework/Machines/Objects/KegMachine.cs
/// https://github.com/Pathoschild/StardewMods/blob/develop/Automate/Framework/Machines/Objects/SeedMakerMachine.cs
/// </summary>
internal class MagicalObjectMachine : IMachine {
    /// <summary>The location which contains the machine.</summary>
    public GameLocation Location => GetDelegate()?.Location ?? _location;
    /// <summary>The tile area covered by the machine.</summary>
    public Rectangle TileArea => GetDelegate()?.TileArea ?? _tileArea;
    /// <summary>A unique ID for the machine type.</summary>
    /// <remarks>This value should be identical for two machines if they have the exact same behavior and input logic. For example, if one machine in a group can't process input due to missing items, Automate will skip any other empty machines of that type in the same group since it assumes they need the same inputs.</remarks>
    public string MachineTypeID => "Slothsoft.ChallengerAutomate/Transmuter";

    private readonly SObject _entity;
    private readonly GameLocation _location;
    private readonly Vector2 _tile;
    private readonly Rectangle _tileArea;
    
    private IMachine? _delegate;
    private int? _delegateParentSheetId;
    
    /// <summary>Construct an instance.</summary>
    /// <param name="entity">The underlying entity.</param>
    /// <param name="location">The location which contains the machine.</param>
    /// <param name="tile">The tile covered by the machine.</param>
    public MagicalObjectMachine(SObject entity, GameLocation location, in Vector2 tile)
    {
        _entity = entity;
        _location = location;
        _tile = tile;
        _tileArea = new Rectangle((int)tile.X, (int)tile.Y, 1, 1);
    }

    /// <summary>Get the machine's processing state.</summary>
    public MachineState GetState()
    {
        return GetDelegate()?.GetState() ?? MachineState.Disabled;
    }
    
    private IMachine? GetDelegate()
    {
        var magicalReplacement = ChallengerMod.Instance.GetApi()!.ActiveChallengeMagicalReplacement;
        if (magicalReplacement.ParentSheetIndex == _delegateParentSheetId) {
            // we have the delegate method cached; return it
            return _delegate;
        }
        _delegate = CreateDelegate(magicalReplacement.ParentSheetIndex);
        _delegateParentSheetId = magicalReplacement.ParentSheetIndex;
        return _delegate;
    }

    private IMachine? CreateDelegate(int parentSheetIndex) {
        switch (parentSheetIndex) {
            case ObjectIds.Keg:
                return CreateDelegate("Pathoschild.Stardew.Automate.Framework.Machines.Objects.KegMachine");
            case ObjectIds.SeedMaker:
                return CreateDelegate("Pathoschild.Stardew.Automate.Framework.Machines.Objects.SeedMakerMachine");
            case ObjectIds.PinkyBunny:
                return null;
            default:
                ChallengerAutomateMod.Instance.Monitor.Log("Could not find automation machine for parent sheet index " + parentSheetIndex, LogLevel.Error);
                return null;
        }
    }
    
    private IMachine? CreateDelegate(string typeName) {
        var type = AccessTools.TypeByName(typeName);
        try {
            ChallengerAutomateMod.Instance.Monitor.Log("Create instance of type " + type, LogLevel.Debug);
            return (IMachine?) Activator.CreateInstance(type, _entity, _location, _tile);
        } catch (Exception) {
            ChallengerAutomateMod.Instance.Monitor.Log("Cannot create instance of type " + type, LogLevel.Error);
            return null;
        }
    }

    /// <summary>Get the output item.</summary>
    public ITrackedStack? GetOutput()
    {
        return GetDelegate()?.GetOutput();
    }

    /// <summary>Provide input to the machine.</summary>
    /// <param name="input">The available items.</param>
    /// <returns>Returns whether the machine started processing an item.</returns>
    public bool SetInput(IStorage input)
    {
        return GetDelegate()?.SetInput(input) ?? false;
    }
}