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
using Netcode;
using StardewValley.Network;

namespace Slothsoft.Challenger.Models; 

public static class NetRefExtensions {
    
    /// <summary>
    /// Reads the model from the save data if the user is allowed to do so, else does nothing
    /// and hopes the player will get the value from the Net framework.
    /// </summary>
    internal static TModel? GetOrRead<TModel>(this NetRef<TModel> netRef, string key) 
        where TModel : class, INetObject<INetSerializable> {
        if (netRef.Value == null) {
            // the first player to do this is always the master player, the other players take this first value
            if (IsHostPlayer()) {
                netRef.Value = ChallengerMod.Instance.Helper.Data.ReadSaveData<TModel>(key) ?? Activator.CreateInstance<TModel>();
            }
        }
        return netRef.Value;
    }

    internal static bool IsHostPlayer() {
        return Context.IsMainPlayer && Context.IsOnHostComputer;
    }

    /// <summary>
    /// Writes the model to the save data if the user is allowed to do so, else does nothing
    /// and hopes the master player will get informed by the Net framework and store it.
    /// </summary>
    internal static void Write<TModel>(this NetRef<TModel> netRef, string key, TModel value)
        where TModel : class, INetObject<INetSerializable> {
        netRef.Value = value;
        // Only the host player needs to store the data
        if (IsHostPlayer()) {
            ChallengerMod.Instance.Helper.Data.WriteSaveData(key, value);
        }
    }
    
    /// <summary>
    /// Reads the model from the save data if the user is allowed to do so, else does nothing
    /// and hopes the player will get the value from the Net framework.
    /// </summary>
    internal static TModel? GetOrRead<TModel>(this NetStringDictionary<TModel, NetRef<TModel>> netRef, string key) 
        where TModel : class, INetObject<INetSerializable> {
        var result = netRef.ContainsKey(key) ? netRef[key] : null;
        if (result == null) {
            // the first player to do this is always the master player, the other players take this first value
            if (IsHostPlayer()) {
                result = ChallengerMod.Instance.Helper.Data.ReadSaveData<TModel>(key) ?? Activator.CreateInstance<TModel>();
                netRef[key] = result;
            }
        }
        return result;
    }

    /// <summary>
    /// Writes the model to the save data if the user is allowed to do so, else does nothing
    /// and hopes the master player will get informed by the Net framework and store it.
    /// </summary>
    internal static void Write<TModel>(this NetStringDictionary<TModel, NetRef<TModel>> netRef, string key, TModel value)
        where TModel : class, INetObject<INetSerializable> {
        netRef[key] = value;
        // Only the host player needs to store the data
        if (IsHostPlayer()) {
            ChallengerMod.Instance.Helper.Data.WriteSaveData(key, value);
        }
    }
}