/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ilyaki/CapitalistSplitMoney
**
*************************************************/

using Netcode;
using StardewValley;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CapitalistSplitMoney
{
	class ProcessIncomingMessagePatch : Patch
	{
		protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(GameServer), "processIncomingMessage");

		public static bool Prefix(IncomingMessage message, GameServer __instance)
		{
			Multiplayer multiplayer = ModEntry.ModHelper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();

			//Console.WriteLine($"Received packet {message.MessageType} by {message.FarmerID}/{message.SourceFarmer.Name}");

			if (message.MessageType == 13 && Game1.IsServer)
			{
				NetRoot<FarmerTeam> team = new NetRoot<FarmerTeam>(new FarmerTeam());
				team.Value.money.Minimum = null;
				multiplayer.readObjectDelta(message.Reader, team);

				int deltaMoney = team.Value.money.Value - 500;//new FarmerTeam() has 500 as default.
				Console.WriteLine($"Delta money for {message.FarmerID}/{message.SourceFarmer.Name} = {deltaMoney}");

				if (deltaMoney != 0)
				{
					if (ModEntry.MoneyData.PlayerMoney.ContainsKey(message.FarmerID))
						ModEntry.MoneyData.PlayerMoney[message.FarmerID] += deltaMoney;
					else
						ModEntry.MoneyData.PlayerMoney.Add(message.FarmerID, Math.Max(0, ModEntry.Config.StartingMoney + deltaMoney));

					Console.WriteLine($"{message.FarmerID} new total money = {ModEntry.MoneyData.PlayerMoney[message.FarmerID]}");

					return false;
				}

				#region Reset streams
				var stream = new MemoryStream(message.Data);
				var bin = new BinaryReader(stream);

				//(msg.stream = new MemoryStream(data);)
				var streamField = ModEntry.ModHelper.Reflection.GetField<MemoryStream>(message, "stream");
				streamField.SetValue(stream);

				//(msg.reader = new BinaryReader(stream);)
				var readerField = ModEntry.ModHelper.Reflection.GetField<BinaryReader>(message, "reader");
				readerField.SetValue(bin);
				#endregion

				multiplayer.processIncomingMessage(message);

				if (multiplayer.isClientBroadcastType(message.MessageType))
				{
					ModEntry.ModHelper.Reflection.GetMethod(__instance, "rebroadcastClientMessage").Invoke(message);
				}

				return false;
			}
			else if (message.MessageType == 6 && Game1.IsServer)
			{
				var readLocationMethod = ModEntry.ModHelper.Reflection.GetMethod(multiplayer, "readLocation");
				GameLocation location3 = readLocationMethod.Invoke<GameLocation>(message.Reader);
				if (location3 != null)
				{
					Item[] ro = null;
					if (location3 is Farm f)
					{
						ro = f.shippingBin.ToArray();
					}

					multiplayer.readObjectDelta(message.Reader, location3.Root);

					if (location3 is Farm farm)
					{
						BinListener.StartListen(message.FarmerID, farm);
					}
				}

				ModEntry.ModHelper.Reflection.GetMethod(__instance, "rebroadcastClientMessage").Invoke(message);

				return false;
			}

			return true;
		}

	}

	class SendServerIntroductionListener : Patch
	{
		//Only runs on server. When a client joins
		protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(GameServer), "sendServerIntroduction");

		public static void Postfix(long peer)
		{
			var multiplayer = ModEntry.ModHelper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();

			NetRoot<FarmerTeam> fakeTeamRoot = new NetRoot<FarmerTeam>(new FarmerTeam());
			fakeTeamRoot.Value.money.Minimum = null;

			int moneyTheyHave = ModEntry.Config.StartingMoney;
			if (ModEntry.MoneyData.PlayerMoney.ContainsKey(peer))
				moneyTheyHave = ModEntry.MoneyData.PlayerMoney[peer];
			else
				ModEntry.MoneyData.PlayerMoney.Add(peer, moneyTheyHave);

			Console.WriteLine($"Setting {peer}'s money to {moneyTheyHave}");
			fakeTeamRoot.Value.money.Set(-1 * Game1.player.teamRoot.Value.money.Value + moneyTheyHave);

			byte[] fakeData = multiplayer.writeObjectDeltaBytes(fakeTeamRoot);

			BroadcastFarmerDeltasPatch.AllowOnce = true;
			Game1.server.sendMessage(peer, 13, Game1.player, fakeData);
			BroadcastFarmerDeltasPatch.AllowOnce = false;
		}
	}

	class BroadcastFarmerDeltasPatch : Patch
	{
		protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(Multiplayer), "broadcastFarmerDeltas");

		internal static bool AllowOnce = false;

		public static bool Prefix(Multiplayer __instance)
		{
			if (!Game1.IsServer)
				return true;

			var reflection = ModEntry.ModHelper.Reflection;
			var broadcastFarmerDeltaMethod = reflection.GetMethod(__instance, "broadcastFarmerDelta");

			foreach (NetFarmerRoot item in __instance.farmerRoots())
			{
				if (item.Dirty && Game1.player.UniqueMultiplayerID == item.Value.UniqueMultiplayerID)
				{
					//NetIntDelta actualValue = item.Value.teamRoot.Value.money;
					//Console.WriteLine($"Trying to sync {item.Value.Name} with everyone.  value = {actualValue}");
					//ModEntry.ModHelper.Reflection.GetField<NetIntDelta>(item.Value.teamRoot.Value, "money").SetValue(new NetIntDelta(0));

					broadcastFarmerDeltaMethod.Invoke(item.Value, __instance.writeObjectDeltaBytes(item));

					//ModEntry.ModHelper.Reflection.GetField<NetIntDelta>(item.Value.teamRoot.Value, "money").SetValue(actualValue);
				}
			}

			if (Game1.player.teamRoot.Dirty)
			{
				var method = reflection.GetMethod(__instance, "broadcastTeamDelta");
				byte[] deltaBytes = __instance.writeObjectDeltaBytes(Game1.player.teamRoot);

				NetRoot<FarmerTeam> team = new NetRoot<FarmerTeam>(new FarmerTeam());
				team.Value.money.Minimum = null;

				using (var mem = new MemoryStream(deltaBytes))
				using (var bin = new BinaryReader(mem))
				{
					__instance.readObjectDelta(bin, team);

					if (AllowOnce || team.Value.money.Value == 500)
					{
						//Not trying to change the money, so pass
						AllowOnce = false;
						method.Invoke(deltaBytes);
					}
				}
			}
			return false;
		}
	}
}
