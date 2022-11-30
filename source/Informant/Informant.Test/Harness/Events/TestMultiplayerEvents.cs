/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-informant
**
*************************************************/

using System;
using StardewModdingAPI.Events;

namespace StardewTests.Harness.Events; 

public class TestMultiplayerEvents : IMultiplayerEvents {
    public event EventHandler<PeerContextReceivedEventArgs>? PeerContextReceived;
    public event EventHandler<PeerConnectedEventArgs>? PeerConnected;
    public event EventHandler<ModMessageReceivedEventArgs>? ModMessageReceived;
    public event EventHandler<PeerDisconnectedEventArgs>? PeerDisconnected;
}